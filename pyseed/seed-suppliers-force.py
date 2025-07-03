#!/usr/bin/env python3
"""
Python script to seed 20 suppliers into the ABP Angular database.
This version can force seed even if suppliers already exist.
Requirements: pip install mysql-connector-python

Usage:
    python seed-suppliers-force.py          # Check existing and skip if found
    python seed-suppliers-force.py --force  # Force seed even if suppliers exist
"""

import mysql.connector
import uuid
from datetime import datetime, timezone
import sys
import logging
import argparse

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

# Supplier data with Vietnamese information
SUPPLIERS = [
    {
        'name': 'Công ty TNHH Sách Giáo Dục Việt Nam',
        'email': 'info@sachgiaoduc.vn',
        'phone': '0243-123-4567',
        'address': '123 Đường Hai Bà Trưng, Quận Hoàn Kiếm, Hà Nội',
        'website': 'https://www.sachgiaoduc.vn',
        'is_active': True
    },
    {
        'name': 'Nhà xuất bản Trẻ',
        'email': 'contact@nxbtre.com.vn',
        'phone': '0283-456-7890',
        'address': '161B Lý Chính Thắng, Quận 3, TP.HCM',
        'website': 'https://www.nxbtre.com.vn',
        'is_active': True
    },
    {
        'name': 'Công ty Cổ phần Phát hành Sách TP.HCM',
        'email': 'info@phathanh.com.vn',
        'phone': '0283-789-0123',
        'address': '62 Lê Lợi, Quận 1, TP.HCM',
        'website': 'https://www.phathanh.com.vn',
        'is_active': True
    },
    {
        'name': 'Nhà xuất bản Kim Đồng',
        'email': 'info@kimdong.com.vn',
        'phone': '0243-234-5678',
        'address': '55 Quang Trung, Quận Hai Bà Trưng, Hà Nội',
        'website': 'https://www.kimdong.com.vn',
        'is_active': True
    },
    {
        'name': 'Công ty TNHH Thương mại Alpha Books',
        'email': 'sales@alphabooks.vn',
        'phone': '0283-345-6789',
        'address': '84 Đinh Tiên Hoàng, Quận 1, TP.HCM',
        'website': 'https://www.alphabooks.vn',
        'is_active': True
    },
    {
        'name': 'Nhà xuất bản Lao Động',
        'email': 'contact@laodong.com.vn',
        'phone': '0243-456-7890',
        'address': '175 Giảng Võ, Quận Ba Đình, Hà Nội',
        'website': 'https://www.laodong.com.vn',
        'is_active': True
    },
    {
        'name': 'Công ty Cổ phần Văn hóa Phương Nam',
        'email': 'info@phuongnam.com.vn',
        'phone': '0283-567-8901',
        'address': '25 Nguyễn Thị Minh Khai, Quận 3, TP.HCM',
        'website': 'https://www.phuongnam.com.vn',
        'is_active': True
    },
    {
        'name': 'Nhà xuất bản Thanh Niên',
        'email': 'contact@thanhnienpublisher.vn',
        'phone': '0243-678-9012',
        'address': '64 Bà Triệu, Quận Hoàn Kiếm, Hà Nội',
        'website': 'https://www.thanhnienpublisher.vn',
        'is_active': True
    },
    {
        'name': 'Công ty TNHH Đại Nam',
        'email': 'info@dainam.com.vn',
        'phone': '0283-789-0123',
        'address': '456 Lê Văn Sỹ, Quận 3, TP.HCM',
        'website': 'https://www.dainam.com.vn',
        'is_active': True
    },
    {
        'name': 'Nhà xuất bản Hồng Đức',
        'email': 'info@hongduc.com.vn',
        'phone': '0243-890-1234',
        'address': '65 Tràng Thi, Quận Hoàn Kiếm, Hà Nội',
        'website': 'https://www.hongduc.com.vn',
        'is_active': True
    },
    {
        'name': 'Công ty Cổ phần Sách Mcbooks',
        'email': 'contact@mcbooks.vn',
        'phone': '0283-901-2345',
        'address': '71 Nguyễn Chí Thanh, Quận 5, TP.HCM',
        'website': 'https://www.mcbooks.vn',
        'is_active': True
    },
    {
        'name': 'Nhà xuất bản Tổng hợp TP.HCM',
        'email': 'info@tonghop.com.vn',
        'phone': '0283-012-3456',
        'address': '62 Nguyễn Thị Minh Khai, Quận 1, TP.HCM',
        'website': 'https://www.tonghop.com.vn',
        'is_active': True
    },
    {
        'name': 'Công ty TNHH Thái Hà Books',
        'email': 'sales@thaihabooks.com',
        'phone': '0243-123-4567',
        'address': '19 Đinh Tiên Hoàng, Quận Hoàn Kiếm, Hà Nội',
        'website': 'https://www.thaihabooks.com',
        'is_active': True
    },
    {
        'name': 'Nhà xuất bản Văn học',
        'email': 'info@vanhoc.com.vn',
        'phone': '0243-234-5678',
        'address': '18 Nguyễn Trường Tộ, Quận Ba Đình, Hà Nội',
        'website': 'https://www.vanhoc.com.vn',
        'is_active': True
    },
    {
        'name': 'Công ty Cổ phần First News',
        'email': 'contact@firstnews.com.vn',
        'phone': '0283-345-6789',
        'address': '147 Pasteur, Quận 3, TP.HCM',
        'website': 'https://www.firstnews.com.vn',
        'is_active': True
    },
    {
        'name': 'Nhà xuất bản Phụ nữ Việt Nam',
        'email': 'info@phunu.com.vn',
        'phone': '0243-456-7890',
        'address': '39 Hàng Chuối, Quận Hai Bà Trưng, Hà Nội',
        'website': 'https://www.phunu.com.vn',
        'is_active': True
    },
    {
        'name': 'Công ty TNHH Đinh Tị Books',
        'email': 'sales@dinhti.com.vn',
        'phone': '0283-567-8901',
        'address': '240 Lê Thị Hồng Gấm, Quận 1, TP.HCM',
        'website': 'https://www.dinhti.com.vn',
        'is_active': True
    },
    {
        'name': 'Nhà xuất bản Chính trị Quốc gia',
        'email': 'info@chinhtriquocgia.vn',
        'phone': '0243-678-9012',
        'address': '7 Đinh Lễ, Quận Hoàn Kiếm, Hà Nội',
        'website': 'https://www.chinhtriquocgia.vn',
        'is_active': True
    },
    {
        'name': 'Công ty Cổ phần Omega Plus',
        'email': 'contact@omegaplus.vn',
        'phone': '0283-789-0123',
        'address': '59 Đồng Khởi, Quận 1, TP.HCM',
        'website': 'https://www.omegaplus.vn',
        'is_active': True
    },
    {
        'name': 'Nhà xuất bản Tri thức',
        'email': 'info@trithuc.com.vn',
        'phone': '0243-890-1234',
        'address': '25 Lý Thường Kiệt, Quận Hoàn Kiếm, Hà Nội',
        'website': 'https://www.trithuc.com.vn',
        'is_active': True
    }
]

def generate_uuid():
    """Generate a UUID without dashes for ABP compatibility"""
    return str(uuid.uuid4()).replace('-', '')

def create_connection():
    """Create a MySQL database connection"""
    try:
        connection = mysql.connector.connect(**DB_CONFIG)
        logger.info("Successfully connected to MySQL database")
        return connection
    except mysql.connector.Error as err:
        logger.error(f"Error connecting to MySQL: {err}")
        raise

def check_existing_suppliers(cursor):
    """Check if suppliers already exist in the database"""
    cursor.execute("SELECT COUNT(*) FROM AppSuppliers")
    count = cursor.fetchone()[0]
    return count

def clear_existing_suppliers(cursor):
    """Clear existing suppliers from the database"""
    cursor.execute("DELETE FROM AppSuppliers")
    logger.info("Cleared existing suppliers")

def get_existing_supplier_names(cursor):
    """Get names of existing suppliers"""
    cursor.execute("SELECT Name FROM AppSuppliers")
    return [row[0] for row in cursor.fetchall()]

def insert_supplier(cursor, supplier_data):
    """Insert a single supplier into the database"""
    now = datetime.now(timezone.utc)
    
    # Generate UUID for the supplier
    supplier_id = generate_uuid()
    
    # Prepare the insert query
    insert_query = """
    INSERT INTO AppSuppliers (
        Id, Name, Email, Phone, Address, Website, IsActive, 
        CreationTime, CreatorId, LastModificationTime, LastModifierId,
        IsDeleted, DeleterId, DeletionTime, ConcurrencyStamp
    ) VALUES (
        %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s
    )
    """
    
    values = (
        supplier_id,
        supplier_data['name'],
        supplier_data['email'],
        supplier_data['phone'],
        supplier_data['address'],
        supplier_data['website'],
        supplier_data['is_active'],
        now,  # CreationTime
        None,  # CreatorId
        now,  # LastModificationTime
        None,  # LastModifierId
        False,  # IsDeleted
        None,  # DeleterId
        None,  # DeletionTime
        generate_uuid()  # ConcurrencyStamp
    )
    
    cursor.execute(insert_query, values)
    return supplier_id

def seed_suppliers(force=False, clear_existing=False):
    """Main function to seed suppliers into the database"""
    connection = None
    try:
        connection = create_connection()
        cursor = connection.cursor()
        
        # Check if suppliers already exist
        existing_count = check_existing_suppliers(cursor)
        
        if existing_count > 0 and not force:
            logger.info(f"Found {existing_count} existing suppliers. Skipping seeding.")
            logger.info("Use --force to seed anyway, or --clear to clear existing data first.")
            return
        
        if clear_existing and existing_count > 0:
            clear_existing_suppliers(cursor)
            existing_count = 0
        
        if existing_count > 0:
            logger.info(f"Found {existing_count} existing suppliers. Force seeding anyway...")
            existing_names = get_existing_supplier_names(cursor)
            suppliers_to_insert = [s for s in SUPPLIERS if s['name'] not in existing_names]
            logger.info(f"Will insert {len(suppliers_to_insert)} new suppliers (skipping duplicates)")
        else:
            logger.info("No suppliers found. Starting seeding process...")
            suppliers_to_insert = SUPPLIERS
        
        # Insert suppliers
        inserted_count = 0
        for supplier in suppliers_to_insert:
            try:
                supplier_id = insert_supplier(cursor, supplier)
                inserted_count += 1
                logger.info(f"Inserted supplier {inserted_count}/{len(suppliers_to_insert)}: {supplier['name']}")
            except mysql.connector.Error as err:
                if err.errno == 1062:  # Duplicate entry
                    logger.warning(f"Supplier already exists: {supplier['name']}")
                    continue
                else:
                    raise
        
        # Commit the transaction
        connection.commit()
        
        # Final count
        final_count = check_existing_suppliers(cursor)
        logger.info(f"Successfully seeded {inserted_count} suppliers! Total suppliers in database: {final_count}")
        
    except mysql.connector.Error as err:
        logger.error(f"Database error: {err}")
        if connection:
            connection.rollback()
        raise
    except Exception as e:
        logger.error(f"Unexpected error: {e}")
        if connection:
            connection.rollback()
        raise
    finally:
        if connection and connection.is_connected():
            cursor.close()
            connection.close()
            logger.info("Database connection closed")

def main():
    """Main entry point"""
    parser = argparse.ArgumentParser(description='Seed suppliers into ABP Angular database')
    parser.add_argument('--force', action='store_true', help='Force seed even if suppliers exist')
    parser.add_argument('--clear', action='store_true', help='Clear existing suppliers before seeding')
    
    args = parser.parse_args()
    
    try:
        logger.info("Starting supplier seeding process...")
        seed_suppliers(force=args.force, clear_existing=args.clear)
        logger.info("Supplier seeding completed successfully!")
    except Exception as e:
        logger.error(f"Seeding failed: {e}")
        sys.exit(1)

if __name__ == "__main__":
    main()
