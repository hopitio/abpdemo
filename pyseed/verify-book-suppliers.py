#!/usr/bin/env python3
"""
Script to verify BookSuppliers data and check Books.Suppliers field
This script helps to understand the current state before running the update
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

def check_database_tables():
    """Check if required tables exist"""
    connection = None
    try:
        connection = mysql.connector.connect(**DB_CONFIG)
        cursor = connection.cursor()
        
        # Check tables
        tables_to_check = ['AppBooks', 'AppSuppliers', 'AppBookSuppliers']
        existing_tables = []
        
        for table in tables_to_check:
            cursor.execute(f"SHOW TABLES LIKE '{table}'")
            if cursor.fetchone():
                existing_tables.append(table)
                logger.info(f"✓ Table {table} exists")
            else:
                logger.error(f"✗ Table {table} does not exist")
        
        return len(existing_tables) == len(tables_to_check)
        
    except mysql.connector.Error as e:
        logger.error(f"Database error: {e}")
        return False
    finally:
        if connection and connection.is_connected():
            cursor.close()
            connection.close()

def check_suppliers_column():
    """Check if Suppliers column exists in AppBooks table"""
    connection = None
    try:
        connection = mysql.connector.connect(**DB_CONFIG)
        cursor = connection.cursor()
        
        # Check if Suppliers column exists
        cursor.execute("SHOW COLUMNS FROM AppBooks LIKE 'Suppliers'")
        result = cursor.fetchone()
        
        if result:
            logger.info(f"✓ Suppliers column exists: {result[1]} {result[2]}")
            return True
        else:
            logger.warning("✗ Suppliers column does not exist in AppBooks table")
            logger.info("You need to run the migration to add the Suppliers column first")
            return False
        
    except mysql.connector.Error as e:
        logger.error(f"Database error: {e}")
        return False
    finally:
        if connection and connection.is_connected():
            cursor.close()
            connection.close()

def analyze_current_data():
    """Analyze current data in the database"""
    connection = None
    try:
        connection = mysql.connector.connect(**DB_CONFIG)
        cursor = connection.cursor()
        
        # Count records in each table
        cursor.execute("SELECT COUNT(*) FROM AppBooks")
        books_count = cursor.fetchone()[0]
        
        cursor.execute("SELECT COUNT(*) FROM AppSuppliers")
        suppliers_count = cursor.fetchone()[0]
        
        cursor.execute("SELECT COUNT(*) FROM AppBookSuppliers")
        book_suppliers_count = cursor.fetchone()[0]
        
        logger.info(f"Current data counts:")
        logger.info(f"  - Books: {books_count}")
        logger.info(f"  - Suppliers: {suppliers_count}")
        logger.info(f"  - BookSuppliers relationships: {book_suppliers_count}")
        
        # Check Books with Suppliers field
        cursor.execute("SELECT COUNT(*) FROM AppBooks WHERE Suppliers IS NOT NULL AND Suppliers != ''")
        books_with_suppliers = cursor.fetchone()[0]
        logger.info(f"  - Books with Suppliers field populated: {books_with_suppliers}")
        
        # Analyze BookSuppliers distribution
        cursor.execute("""
            SELECT BookId, COUNT(*) as SupplierCount
            FROM AppBookSuppliers
            GROUP BY BookId
            ORDER BY SupplierCount DESC
            LIMIT 5
        """)
        
        top_books = cursor.fetchall()
        if top_books:
            logger.info("Top 5 books with most suppliers:")
            for book_id, supplier_count in top_books:
                cursor.execute("SELECT Name FROM AppBooks WHERE Id = %s", (book_id,))
                book_name = cursor.fetchone()
                name = book_name[0] if book_name else "Unknown"
                logger.info(f"  - {name} ({book_id}): {supplier_count} suppliers")
        
        # Check for books without suppliers
        cursor.execute("""
            SELECT COUNT(*) FROM AppBooks b
            WHERE NOT EXISTS (SELECT 1 FROM AppBookSuppliers bs WHERE bs.BookId = b.Id)
        """)
        books_without_suppliers = cursor.fetchone()[0]
        logger.info(f"  - Books without suppliers: {books_without_suppliers}")
        
        # Show sample BookSuppliers data
        cursor.execute("""
            SELECT bs.BookId, b.Name, bs.SupplierId, s.Name
            FROM AppBookSuppliers bs
            JOIN AppBooks b ON bs.BookId = b.Id
            JOIN AppSuppliers s ON bs.SupplierId = s.Id
            LIMIT 5
        """)
        
        sample_data = cursor.fetchall()
        if sample_data:
            logger.info("Sample BookSuppliers relationships:")
            for book_id, book_name, supplier_id, supplier_name in sample_data:
                logger.info(f"  - {book_name} -> {supplier_name}")
        
        return True
        
    except mysql.connector.Error as e:
        logger.error(f"Database error: {e}")
        return False
    finally:
        if connection and connection.is_connected():
            cursor.close()
            connection.close()

def preview_update():
    """Preview what the update would do"""
    connection = None
    try:
        connection = mysql.connector.connect(**DB_CONFIG)
        cursor = connection.cursor()
        
        # Get books that would be updated
        cursor.execute("""
            SELECT b.Id, b.Name, 
                   GROUP_CONCAT(bs.SupplierId ORDER BY bs.SupplierId SEPARATOR ',') as NewSuppliers,
                   b.Suppliers as CurrentSuppliers
            FROM AppBooks b
            JOIN AppBookSuppliers bs ON b.Id = bs.BookId
            GROUP BY b.Id, b.Name, b.Suppliers
            LIMIT 10
        """)
        
        preview_data = cursor.fetchall()
        if preview_data:
            logger.info("Preview of updates (first 10 books):")
            for book_id, book_name, new_suppliers, current_suppliers in preview_data:
                current = current_suppliers if current_suppliers else "(empty)"
                logger.info(f"  - {book_name}:")
                logger.info(f"    Current: {current}")
                logger.info(f"    New: {new_suppliers}")
        
        # Count total books that would be affected
        cursor.execute("""
            SELECT COUNT(DISTINCT b.Id) 
            FROM AppBooks b
            JOIN AppBookSuppliers bs ON b.Id = bs.BookId
        """)
        
        affected_count = cursor.fetchone()[0]
        logger.info(f"Total books that would be updated: {affected_count}")
        
        return True
        
    except mysql.connector.Error as e:
        logger.error(f"Database error: {e}")
        return False
    finally:
        if connection and connection.is_connected():
            cursor.close()
            connection.close()

def main():
    """Main function"""
    logger.info("Starting Book Suppliers verification script")
    logger.info("=" * 50)
    
    # Check database structure
    logger.info("1. Checking database tables...")
    if not check_database_tables():
        logger.error("Required tables are missing")
        return
    
    logger.info("\n2. Checking Suppliers column...")
    if not check_suppliers_column():
        logger.error("Suppliers column is missing. Please run migration first.")
        return
    
    logger.info("\n3. Analyzing current data...")
    if not analyze_current_data():
        logger.error("Failed to analyze current data")
        return
    
    logger.info("\n4. Previewing update...")
    if not preview_update():
        logger.error("Failed to preview update")
        return
    
    logger.info("\n" + "=" * 50)
    logger.info("Verification completed successfully!")
    logger.info("You can now run 'update-book-suppliers.py' to perform the update.")

if __name__ == "__main__":
    main()
