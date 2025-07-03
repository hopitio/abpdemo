#!/usr/bin/env python3
"""
Python script to seed 5000 books into the ABP Angular database.
Each book will have 2 random suppliers from the database.
Requirements: pip install mysql-connector-python
"""

import mysql.connector
import uuid
from datetime import datetime, timezone
import sys
import logging
import random
from faker import Faker

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

# Initialize Faker for Vietnamese locale
fake = Faker('vi_VN')

# Book types (enum values)
BOOK_TYPES = [0, 1, 2, 3, 4]  # Adventure, Biography, Dystopia, Fantastic, Horror, Science, ScienceFiction, Poetry

# Vietnamese book name templates
BOOK_NAME_TEMPLATES = [
    "Lịch sử {subject}",
    "Khám phá {subject}",
    "Cẩm nang {subject}",
    "Bí quyết {subject}",
    "Hướng dẫn {subject}",
    "Giới thiệu {subject}",
    "Nghệ thuật {subject}",
    "Phương pháp {subject}",
    "Kiến thức {subject}",
    "Thực hành {subject}",
    "Tự học {subject}",
    "Chuyên đề {subject}",
    "Kỹ năng {subject}",
    "Bài tập {subject}",
    "Lý thuyết {subject}",
    "Ứng dụng {subject}",
    "Nghiên cứu {subject}",
    "Phát triển {subject}",
    "Hiểu biết {subject}",
    "Thành thạo {subject}"
]

BOOK_SUBJECTS = [
    "Toán học", "Vật lý", "Hóa học", "Sinh học", "Lịch sử", "Địa lý", "Văn học", "Tiếng Anh",
    "Tin học", "Kinh tế", "Triết học", "Tâm lý học", "Xã hội học", "Âm nhạc", "Mỹ thuật",
    "Thể dục", "Nấu ăn", "Du lịch", "Sức khỏe", "Làm đẹp", "Kinh doanh", "Marketing",
    "Kế toán", "Luật", "Y học", "Kỹ thuật", "Công nghệ", "Nông nghiệp", "Môi trường",
    "Giáo dục", "Tôn giáo", "Nghệ thuật", "Thời trang", "Kiến trúc", "Thiết kế",
    "Truyền thông", "Báo chí", "Điện ảnh", "Sân khấu", "Nhiếp ảnh", "Thư pháp"
]

def create_connection():
    """Create a database connection"""
    try:
        connection = mysql.connector.connect(**DB_CONFIG)
        logger.info("Database connection successful")
        return connection
    except mysql.connector.Error as err:
        logger.error(f"Error connecting to database: {err}")
        raise

def get_all_suppliers(cursor):
    """Get all suppliers from the database"""
    query = """
    SELECT Id, Name FROM AppSuppliers 
    WHERE IsActive = 1
    """
    cursor.execute(query)
    suppliers = cursor.fetchall()
    logger.info(f"Found {len(suppliers)} active suppliers")
    return suppliers

def generate_book_name():
    """Generate a random Vietnamese book name"""
    template = random.choice(BOOK_NAME_TEMPLATES)
    subject = random.choice(BOOK_SUBJECTS)
    return template.format(subject=subject)

def generate_book_data():
    """Generate random book data"""
    return {
        'id': str(uuid.uuid4()),
        'name': generate_book_name(),
        'type': random.choice(BOOK_TYPES),
        'publish_date': fake.date_between(start_date='-30y', end_date='today'),
        'price': round(random.uniform(50000, 500000), 2),  # Price between 50k-500k VND
        'extra_properties': '{}',
        'concurrency_stamp': str(uuid.uuid4()),
        'creation_time': datetime.now(timezone.utc),
        'creator_id': None,
        'last_modification_time': None,
        'last_modifier_id': None
    }

def insert_book(cursor, book_data):
    """Insert a book into the database"""
    query = """
    INSERT INTO AppBooks (
        Id, Name, Type, PublishDate, Price, ExtraProperties, 
        ConcurrencyStamp, CreationTime, CreatorId, 
        LastModificationTime, LastModifierId
    ) VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
    """
    
    values = (
        book_data['id'],
        book_data['name'],
        book_data['type'],
        book_data['publish_date'],
        book_data['price'],
        book_data['extra_properties'],
        book_data['concurrency_stamp'],
        book_data['creation_time'],
        book_data['creator_id'],
        book_data['last_modification_time'],
        book_data['last_modifier_id']
    )
    
    cursor.execute(query, values)
    return book_data['id']

def assign_suppliers_to_book(cursor, book_id, suppliers):
    """Assign 2 random suppliers to a book"""
    # Select 2 random suppliers
    selected_suppliers = random.sample(suppliers, min(2, len(suppliers)))
    
    for supplier in selected_suppliers:
        supplier_id = supplier[0]  # supplier[0] is the ID
        
        # Check if this relationship already exists
        check_query = """
        SELECT COUNT(*) FROM AppBookSuppliers 
        WHERE BookId = %s AND SupplierId = %s
        """
        cursor.execute(check_query, (book_id, supplier_id))
        if cursor.fetchone()[0] > 0:
            continue  # Skip if relationship already exists
        
        # Insert the relationship
        insert_query = """
        INSERT INTO AppBookSuppliers (BookId, SupplierId, CreationTime)
        VALUES (%s, %s, %s)
        """
        cursor.execute(insert_query, (book_id, supplier_id, datetime.now(timezone.utc)))

def check_existing_books(cursor):
    """Check how many books already exist"""
    cursor.execute("SELECT COUNT(*) FROM AppBooks")
    count = cursor.fetchone()[0]
    return count

def seed_books(target_count=5000):
    """Main function to seed books into the database"""
    connection = None
    try:
        connection = create_connection()
        cursor = connection.cursor()
        
        # Get all suppliers
        suppliers = get_all_suppliers(cursor)
        if len(suppliers) < 2:
            logger.error("Need at least 2 suppliers in the database to assign to books")
            return
        
        # Check existing books
        existing_count = check_existing_books(cursor)
        logger.info(f"Found {existing_count} existing books")
        
        # Calculate how many books to insert
        books_to_insert = target_count - existing_count
        if books_to_insert <= 0:
            logger.info(f"Already have {existing_count} books, which is >= target of {target_count}")
            return
        
        logger.info(f"Will insert {books_to_insert} books to reach target of {target_count}")
        
        # Insert books in batches
        batch_size = 100
        inserted_count = 0
        
        for i in range(0, books_to_insert, batch_size):
            batch_end = min(i + batch_size, books_to_insert)
            current_batch_size = batch_end - i
            
            logger.info(f"Processing batch {i//batch_size + 1}: books {i+1} to {batch_end}")
            
            for j in range(current_batch_size):
                # Generate book data
                book_data = generate_book_data()
                
                # Insert book
                book_id = insert_book(cursor, book_data)
                
                # Assign suppliers to book
                assign_suppliers_to_book(cursor, book_id, suppliers)
                
                inserted_count += 1
                
                if inserted_count % 500 == 0:
                    logger.info(f"Inserted {inserted_count}/{books_to_insert} books...")
            
            # Commit each batch
            connection.commit()
            logger.info(f"Committed batch {i//batch_size + 1}")
        
        logger.info(f"Successfully seeded {inserted_count} books!")
        
        # Final verification
        final_count = check_existing_books(cursor)
        logger.info(f"Total books in database: {final_count}")
        
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
    try:
        logger.info("Starting book seeding process...")
        seed_books(5000)
        logger.info("Book seeding completed successfully!")
    except Exception as e:
        logger.error(f"Seeding failed: {e}")
        sys.exit(1)

if __name__ == "__main__":
    main()
