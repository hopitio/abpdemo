#!/usr/bin/env python3
"""
Script to check the Books table structure
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

def check_books_table():
    """Check the structure of the AppBooks table"""
    connection = None
    try:
        connection = mysql.connector.connect(**DB_CONFIG)
        cursor = connection.cursor()
        
        # Check if Books table exists
        cursor.execute("SHOW TABLES LIKE 'AppBooks'")
        if cursor.fetchone():
            logger.info("AppBooks table exists")
            cursor.execute("DESCRIBE AppBooks")
            columns = cursor.fetchall()
            logger.info("AppBooks table structure:")
            for column in columns:
                logger.info(f"  {column[0]} | {column[1]} | {column[2]} | {column[3]} | {column[4]}")
            
            cursor.execute("SELECT COUNT(*) FROM AppBooks")
            count = cursor.fetchone()[0]
            logger.info(f"Current books count: {count}")
        else:
            logger.info("AppBooks table does not exist")
            
        # Check if BookSuppliers table exists
        cursor.execute("SHOW TABLES LIKE 'AppBookSuppliers'")
        if cursor.fetchone():
            logger.info("AppBookSuppliers table exists")
            cursor.execute("DESCRIBE AppBookSuppliers")
            columns = cursor.fetchall()
            logger.info("AppBookSuppliers table structure:")
            for column in columns:
                logger.info(f"  {column[0]} | {column[1]} | {column[2]} | {column[3]} | {column[4]}")
        else:
            logger.info("AppBookSuppliers table does not exist")
            
        # List all tables to see what's available
        cursor.execute("SHOW TABLES")
        tables = cursor.fetchall()
        logger.info("Available tables:")
        for table in tables:
            logger.info(f"  {table[0]}")
        
    except mysql.connector.Error as err:
        logger.error(f"Database error: {err}")
    finally:
        if connection and connection.is_connected():
            cursor.close()
            connection.close()

if __name__ == "__main__":
    check_books_table()
