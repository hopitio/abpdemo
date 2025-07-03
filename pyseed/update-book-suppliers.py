#!/usr/bin/env python3
"""
Script to read BookSuppliers and update Book.Suppliers field
This script consolidates supplier IDs from BookSuppliers table into comma-separated string in Books table
"""

import mysql.connector
import logging
from collections import defaultdict

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

# Database connection configuration
DB_CONFIG = {
    'host': 'localhost',
    'port': 3306,
    'database': 'angular',
    'user': 'root',
    'password': 'root',
    'charset': 'utf8mb4'
}

def get_book_suppliers_mapping():
    """Get mapping of BookId to list of SupplierIds from BookSuppliers table"""
    connection = None
    try:
        connection = mysql.connector.connect(**DB_CONFIG)
        cursor = connection.cursor()
        
        # Check if BookSuppliers table exists
        cursor.execute("SHOW TABLES LIKE 'AppBookSuppliers'")
        if not cursor.fetchone():
            logger.error("AppBookSuppliers table does not exist")
            return {}
        
        # Get all BookSupplier relationships
        cursor.execute("SELECT BookId, SupplierId FROM AppBookSuppliers ORDER BY BookId")
        book_suppliers = cursor.fetchall()
        
        # Group suppliers by book
        book_supplier_mapping = defaultdict(list)
        for book_id, supplier_id in book_suppliers:
            book_supplier_mapping[book_id].append(str(supplier_id))
        
        logger.info(f"Found {len(book_suppliers)} book-supplier relationships")
        logger.info(f"Affecting {len(book_supplier_mapping)} books")
        
        return dict(book_supplier_mapping)
        
    except mysql.connector.Error as e:
        logger.error(f"Database error in get_book_suppliers_mapping: {e}")
        return {}
    except Exception as e:
        logger.error(f"Unexpected error in get_book_suppliers_mapping: {e}")
        return {}
    finally:
        if connection and connection.is_connected():
            cursor.close()
            connection.close()

def check_books_table_structure():
    """Check if Books table has Suppliers column"""
    connection = None
    try:
        connection = mysql.connector.connect(**DB_CONFIG)
        cursor = connection.cursor()
        
        # Check if Books table exists
        cursor.execute("SHOW TABLES LIKE 'AppBooks'")
        if not cursor.fetchone():
            logger.error("AppBooks table does not exist")
            return False
        
        # Check if Suppliers column exists
        cursor.execute("SHOW COLUMNS FROM AppBooks LIKE 'Suppliers'")
        if not cursor.fetchone():
            logger.error("Suppliers column does not exist in AppBooks table")
            logger.info("Please run the migration to add the Suppliers column first")
            return False
        
        logger.info("AppBooks table has Suppliers column")
        return True
        
    except mysql.connector.Error as e:
        logger.error(f"Database error in check_books_table_structure: {e}")
        return False
    except Exception as e:
        logger.error(f"Unexpected error in check_books_table_structure: {e}")
        return False
    finally:
        if connection and connection.is_connected():
            cursor.close()
            connection.close()

def update_book_suppliers_field():
    """Update Books.Suppliers field with comma-separated supplier IDs"""
    connection = None
    try:
        connection = mysql.connector.connect(**DB_CONFIG)
        cursor = connection.cursor()
        
        # Get current books count
        cursor.execute("SELECT COUNT(*) FROM AppBooks")
        total_books = cursor.fetchone()[0]
        logger.info(f"Total books in database: {total_books}")
        
        # Get book-supplier mapping
        book_supplier_mapping = get_book_suppliers_mapping()
        
        if not book_supplier_mapping:
            logger.warning("No book-supplier relationships found")
            return
        
        # Update each book with its suppliers
        updated_count = 0
        skipped_count = 0
        
        for book_id, supplier_ids in book_supplier_mapping.items():
            suppliers_string = ",".join(supplier_ids)
            
            # Update the book's Suppliers field
            update_query = "UPDATE AppBooks SET Suppliers = %s WHERE Id = %s"
            cursor.execute(update_query, (suppliers_string, book_id))
            
            if cursor.rowcount > 0:
                updated_count += 1
                logger.info(f"Updated book {book_id} with suppliers: {suppliers_string}")
            else:
                skipped_count += 1
                logger.warning(f"Book {book_id} not found, skipped")
        
        # Clear Suppliers field for books without suppliers
        cursor.execute("UPDATE AppBooks SET Suppliers = '' WHERE Id NOT IN (SELECT DISTINCT BookId FROM AppBookSuppliers)")
        cleared_count = cursor.rowcount
        
        # Commit the changes
        connection.commit()
        
        logger.info(f"Update completed:")
        logger.info(f"  - Updated books: {updated_count}")
        logger.info(f"  - Skipped books: {skipped_count}")
        logger.info(f"  - Cleared books: {cleared_count}")
        
        # Verify the update
        cursor.execute("SELECT COUNT(*) FROM AppBooks WHERE Suppliers IS NOT NULL AND Suppliers != ''")
        books_with_suppliers = cursor.fetchone()[0]
        logger.info(f"Books with suppliers after update: {books_with_suppliers}")
        
    except mysql.connector.Error as e:
        logger.error(f"Database error in update_book_suppliers_field: {e}")
        if connection:
            connection.rollback()
    except Exception as e:
        logger.error(f"Unexpected error in update_book_suppliers_field: {e}")
        if connection:
            connection.rollback()
    finally:
        if connection and connection.is_connected():
            cursor.close()
            connection.close()

def verify_update():
    """Verify that the update was successful"""
    connection = None
    try:
        connection = mysql.connector.connect(**DB_CONFIG)
        cursor = connection.cursor()
        
        # Get sample books with suppliers
        cursor.execute("""
            SELECT b.Id, b.Name, b.Suppliers, 
                   COUNT(bs.SupplierId) as SupplierCount
            FROM AppBooks b
            LEFT JOIN AppBookSuppliers bs ON b.Id = bs.BookId
            WHERE b.Suppliers IS NOT NULL AND b.Suppliers != ''
            GROUP BY b.Id, b.Name, b.Suppliers
            LIMIT 10
        """)
        
        results = cursor.fetchall()
        
        if results:
            logger.info("Sample books with suppliers:")
            for book_id, book_name, suppliers_string, supplier_count in results:
                suppliers_in_string = len(suppliers_string.split(',')) if suppliers_string else 0
                status = "✓" if suppliers_in_string == supplier_count else "✗"
                logger.info(f"  {status} {book_name} ({book_id}): {suppliers_string} ({suppliers_in_string} vs {supplier_count})")
        else:
            logger.warning("No books with suppliers found")
            
        # Check for inconsistencies
        cursor.execute("""
            SELECT COUNT(*) FROM AppBooks b
            WHERE (
                (b.Suppliers IS NULL OR b.Suppliers = '') 
                AND EXISTS (SELECT 1 FROM AppBookSuppliers bs WHERE bs.BookId = b.Id)
            ) OR (
                (b.Suppliers IS NOT NULL AND b.Suppliers != '') 
                AND NOT EXISTS (SELECT 1 FROM AppBookSuppliers bs WHERE bs.BookId = b.Id)
            )
        """)
        
        inconsistent_count = cursor.fetchone()[0]
        if inconsistent_count > 0:
            logger.warning(f"Found {inconsistent_count} books with inconsistent supplier data")
        else:
            logger.info("All books have consistent supplier data")
        
    except mysql.connector.Error as e:
        logger.error(f"Database error in verify_update: {e}")
    except Exception as e:
        logger.error(f"Unexpected error in verify_update: {e}")
    finally:
        if connection and connection.is_connected():
            cursor.close()
            connection.close()

def main():
    """Main function"""
    logger.info("Starting Book Suppliers field update script")
    
    # Check database structure
    if not check_books_table_structure():
        logger.error("Database structure check failed")
        return
    
    # Update the suppliers field
    update_book_suppliers_field()
    
    # Verify the update
    verify_update()
    
    logger.info("Script completed")

if __name__ == "__main__":
    main()
