#!/usr/bin/env python3
"""
Script to verify the seeded suppliers
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

def verify_suppliers():
    """Verify the seeded suppliers"""
    connection = None
    try:
        connection = mysql.connector.connect(**DB_CONFIG)
        cursor = connection.cursor()
        
        # Get total count
        cursor.execute("SELECT COUNT(*) FROM AppSuppliers")
        total_count = cursor.fetchone()[0]
        logger.info(f"Total suppliers in database: {total_count}")
        
        # Get all suppliers
        cursor.execute("SELECT Name, Email, Phone, Address FROM AppSuppliers ORDER BY Name")
        suppliers = cursor.fetchall()
        
        logger.info("All suppliers in database:")
        for i, supplier in enumerate(suppliers, 1):
            logger.info(f"{i:2d}. {supplier[0]}")
            logger.info(f"    Email: {supplier[1]}")
            logger.info(f"    Phone: {supplier[2]}")
            logger.info(f"    Address: {supplier[3]}")
            logger.info("")
        
    except mysql.connector.Error as err:
        logger.error(f"Database error: {err}")
    finally:
        if connection and connection.is_connected():
            cursor.close()
            connection.close()

if __name__ == "__main__":
    verify_suppliers()
