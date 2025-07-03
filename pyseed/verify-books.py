#!/usr/bin/env python3
"""
Script to verify the seeded books data
"""

import mysql.connector
import logging

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

def verify_books_data():
    """Verify the seeded books data"""
    connection = None
    try:
        connection = mysql.connector.connect(**DB_CONFIG)
        cursor = connection.cursor()
        
        # Check total books count
        cursor.execute("SELECT COUNT(*) FROM AppBooks")
        books_count = cursor.fetchone()[0]
        logger.info(f"Total books in database: {books_count}")
        
        # Check total book-supplier relationships
        cursor.execute("SELECT COUNT(*) FROM AppBookSuppliers")
        relationships_count = cursor.fetchone()[0]
        logger.info(f"Total book-supplier relationships: {relationships_count}")
        
        # Check average relationships per book
        avg_relationships = relationships_count / books_count if books_count > 0 else 0
        logger.info(f"Average suppliers per book: {avg_relationships:.2f}")
        
        # Check books with different numbers of suppliers
        cursor.execute("""
            SELECT supplier_count, COUNT(*) as book_count
            FROM (
                SELECT BookId, COUNT(*) as supplier_count
                FROM AppBookSuppliers
                GROUP BY BookId
            ) as book_suppliers
            GROUP BY supplier_count
            ORDER BY supplier_count
        """)
        
        supplier_distribution = cursor.fetchall()
        logger.info("Distribution of suppliers per book:")
        for suppliers_per_book, book_count in supplier_distribution:
            logger.info(f"  {suppliers_per_book} suppliers: {book_count} books")
        
        # Show some sample books with their suppliers
        cursor.execute("""
            SELECT b.Id, b.Name, b.Type, b.Price, 
                   GROUP_CONCAT(s.Name SEPARATOR ', ') as suppliers
            FROM AppBooks b
            LEFT JOIN AppBookSuppliers bs ON b.Id = bs.BookId
            LEFT JOIN AppSuppliers s ON bs.SupplierId = s.Id
            GROUP BY b.Id, b.Name, b.Type, b.Price
            ORDER BY b.CreationTime DESC
            LIMIT 10
        """)
        
        sample_books = cursor.fetchall()
        logger.info("\nSample books with their suppliers:")
        for book in sample_books:
            book_id, name, book_type, price, suppliers = book
            logger.info(f"  {name} (Type: {book_type}, Price: {price:,.0f} VND)")
            logger.info(f"    Suppliers: {suppliers}")
        
        # Check price distribution
        cursor.execute("""
            SELECT 
                MIN(Price) as min_price,
                MAX(Price) as max_price,
                AVG(Price) as avg_price,
                COUNT(*) as total_books
            FROM AppBooks
        """)
        
        price_stats = cursor.fetchone()
        min_price, max_price, avg_price, total_books = price_stats
        logger.info(f"\nPrice statistics:")
        logger.info(f"  Min price: {min_price:,.0f} VND")
        logger.info(f"  Max price: {max_price:,.0f} VND")
        logger.info(f"  Average price: {avg_price:,.0f} VND")
        
        # Check book type distribution
        cursor.execute("""
            SELECT Type, COUNT(*) as count
            FROM AppBooks
            GROUP BY Type
            ORDER BY Type
        """)
        
        type_distribution = cursor.fetchall()
        logger.info("\nBook type distribution:")
        type_names = ["Adventure", "Biography", "Dystopia", "Fantastic", "Horror"]
        for book_type, count in type_distribution:
            type_name = type_names[book_type] if book_type < len(type_names) else f"Type {book_type}"
            logger.info(f"  {type_name}: {count} books")
        
    except mysql.connector.Error as err:
        logger.error(f"Database error: {err}")
    finally:
        if connection and connection.is_connected():
            cursor.close()
            connection.close()

if __name__ == "__main__":
    verify_books_data()
