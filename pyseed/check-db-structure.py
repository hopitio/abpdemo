#!/usr/bin/env python3
"""
Script to check the database table structure
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

def check_table_structure():
    """Check the structure of the AppSuppliers table"""
    connection = None
    try:
        connection = mysql.connector.connect(**DB_CONFIG)
        cursor = connection.cursor()
        
        # Check if table exists
        cursor.execute("SHOW TABLES LIKE 'AppSuppliers'")
        if not cursor.fetchone():
            logger.info("AppSuppliers table does not exist")
            return
        
        # Get table structure
        cursor.execute("DESCRIBE AppSuppliers")
        columns = cursor.fetchall()
        
        logger.info("AppSuppliers table structure:")
        for column in columns:
            logger.info(f"  {column[0]} | {column[1]} | {column[2]} | {column[3]} | {column[4]}")
        
        # Check existing data
        cursor.execute("SELECT COUNT(*) FROM AppSuppliers")
        count = cursor.fetchone()[0]
        logger.info(f"Current suppliers count: {count}")
        
        if count > 0:
            cursor.execute("SELECT Id, Name, Email FROM AppSuppliers LIMIT 5")
            suppliers = cursor.fetchall()
            logger.info("Sample suppliers:")
            for supplier in suppliers:
                logger.info(f"  {supplier[0]} | {supplier[1]} | {supplier[2]}")
        
    except mysql.connector.Error as err:
        logger.error(f"Database error: {err}")
    finally:
        if connection and connection.is_connected():
            cursor.close()
            connection.close()

if __name__ == "__main__":
    check_table_structure()
