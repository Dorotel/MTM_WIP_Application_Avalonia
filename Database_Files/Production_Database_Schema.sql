-- =====================================================
-- MTM WIP Application - Production Database Schema
-- =====================================================
-- Environment: Production
-- Status: READ-ONLY - Current Production State
-- Last Updated: [To be updated during deployment]
-- =====================================================

-- ?? WARNING: This file is READ-ONLY
-- For schema changes, modify: Development/Database_Files/Development_Database_Schema.sql
-- Deploy changes through proper change management process

-- =====================================================
-- Database Creation and Configuration
-- =====================================================

-- Create database if not exists
CREATE DATABASE IF NOT EXISTS mtm_wip_application
  DEFAULT CHARACTER SET utf8mb4 
  DEFAULT COLLATE utf8mb4_unicode_ci;

USE mtm_wip_application;

-- =====================================================
-- Core Tables
-- =====================================================

-- Parts Master Table
CREATE TABLE parts (
    part_id VARCHAR(50) PRIMARY KEY,
    part_description VARCHAR(255) NOT NULL,
    part_type VARCHAR(50),
    unit_of_measure VARCHAR(10) DEFAULT 'EA',
    standard_cost DECIMAL(10,4) DEFAULT 0.0000,
    active_status BOOLEAN DEFAULT TRUE,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(50),
    modified_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    modified_by VARCHAR(50),
    
    INDEX idx_parts_type (part_type),
    INDEX idx_parts_active (active_status),
    INDEX idx_parts_description (part_description)
);

-- Operations Master Table
CREATE TABLE operations (
    operation_id VARCHAR(10) PRIMARY KEY,
    operation_description VARCHAR(255) NOT NULL,
    operation_type VARCHAR(50),
    standard_time_minutes INT DEFAULT 0,
    active_status BOOLEAN DEFAULT TRUE,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(50),
    
    INDEX idx_operations_type (operation_type),
    INDEX idx_operations_active (active_status)
);

-- Locations Master Table
CREATE TABLE locations (
    location_id VARCHAR(50) PRIMARY KEY,
    location_description VARCHAR(255) NOT NULL,
    location_type VARCHAR(50),
    capacity_limit INT DEFAULT NULL,
    active_status BOOLEAN DEFAULT TRUE,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(50),
    
    INDEX idx_locations_type (location_type),
    INDEX idx_locations_active (active_status)
);

-- Users Table
CREATE TABLE users (
    user_id VARCHAR(50) PRIMARY KEY,
    username VARCHAR(100) UNIQUE NOT NULL,
    full_name VARCHAR(255),
    email VARCHAR(255),
    role VARCHAR(50) DEFAULT 'User',
    active_status BOOLEAN DEFAULT TRUE,
    last_login TIMESTAMP NULL,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(50),
    
    INDEX idx_users_username (username),
    INDEX idx_users_role (role),
    INDEX idx_users_active (active_status)
);

-- =====================================================
-- Inventory Tables
-- =====================================================

-- Main Inventory Table
CREATE TABLE inventory (
    inventory_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    part_id VARCHAR(50) NOT NULL,
    operation_id VARCHAR(10) NOT NULL,
    location_id VARCHAR(50) NOT NULL,
    quantity_on_hand INT NOT NULL DEFAULT 0,
    quantity_allocated INT NOT NULL DEFAULT 0,
    quantity_available INT GENERATED ALWAYS AS (quantity_on_hand - quantity_allocated) STORED,
    last_transaction_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modified_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (part_id) REFERENCES parts(part_id),
    FOREIGN KEY (operation_id) REFERENCES operations(operation_id),
    FOREIGN KEY (location_id) REFERENCES locations(location_id),
    
    UNIQUE KEY unique_inventory (part_id, operation_id, location_id),
    INDEX idx_inventory_part (part_id),
    INDEX idx_inventory_location (location_id),
    INDEX idx_inventory_operation (operation_id),
    INDEX idx_inventory_available (quantity_available)
);

-- Inventory Transactions Table
CREATE TABLE inventory_transactions (
    transaction_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    transaction_type ENUM('IN', 'OUT', 'TRANSFER', 'ADJUSTMENT') NOT NULL,
    part_id VARCHAR(50) NOT NULL,
    operation_id VARCHAR(10) NOT NULL,
    from_location_id VARCHAR(50),
    to_location_id VARCHAR(50),
    quantity INT NOT NULL,
    unit_cost DECIMAL(10,4) DEFAULT 0.0000,
    reference_number VARCHAR(100),
    notes TEXT,
    user_id VARCHAR(50) NOT NULL,
    transaction_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (part_id) REFERENCES parts(part_id),
    FOREIGN KEY (operation_id) REFERENCES operations(operation_id),
    FOREIGN KEY (from_location_id) REFERENCES locations(location_id),
    FOREIGN KEY (to_location_id) REFERENCES locations(location_id),
    FOREIGN KEY (user_id) REFERENCES users(user_id),
    
    INDEX idx_transactions_part (part_id),
    INDEX idx_transactions_date (transaction_date),
    INDEX idx_transactions_user (user_id),
    INDEX idx_transactions_type (transaction_type),
    INDEX idx_transactions_reference (reference_number)
);

-- =====================================================
-- System Tables
-- =====================================================

-- Configuration Table
CREATE TABLE system_configuration (
    config_key VARCHAR(100) PRIMARY KEY,
    config_value TEXT,
    config_description VARCHAR(255),
    data_type VARCHAR(20) DEFAULT 'string',
    modified_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    modified_by VARCHAR(50)
);

-- Error Log Table
CREATE TABLE error_log (
    error_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    error_message TEXT NOT NULL,
    stack_trace TEXT,
    procedure_name VARCHAR(255),
    user_id VARCHAR(50),
    severity_level VARCHAR(20) DEFAULT 'Error',
    additional_info JSON,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (user_id) REFERENCES users(user_id),
    INDEX idx_error_date (created_date),
    INDEX idx_error_user (user_id),
    INDEX idx_error_severity (severity_level)
);

-- Audit Log Table
CREATE TABLE audit_log (
    audit_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    table_name VARCHAR(100) NOT NULL,
    operation_type VARCHAR(20) NOT NULL,
    record_id VARCHAR(100) NOT NULL,
    old_values JSON,
    new_values JSON,
    user_id VARCHAR(50) NOT NULL,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (user_id) REFERENCES users(user_id),
    INDEX idx_audit_table (table_name),
    INDEX idx_audit_date (created_date),
    INDEX idx_audit_user (user_id),
    INDEX idx_audit_operation (operation_type)
);

-- User Sessions Table
CREATE TABLE user_sessions (
    session_id VARCHAR(255) PRIMARY KEY,
    user_id VARCHAR(50) NOT NULL,
    login_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_activity TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    ip_address VARCHAR(45),
    user_agent TEXT,
    active_status BOOLEAN DEFAULT TRUE,
    
    FOREIGN KEY (user_id) REFERENCES users(user_id),
    INDEX idx_sessions_user (user_id),
    INDEX idx_sessions_active (active_status),
    INDEX idx_sessions_activity (last_activity)
);

-- =====================================================
-- Performance and Maintenance Tables
-- =====================================================

-- Performance Metrics Table
CREATE TABLE performance_metrics (
    metric_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    metric_name VARCHAR(100) NOT NULL,
    metric_value DECIMAL(15,4),
    metric_unit VARCHAR(20),
    measurement_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    additional_data JSON,
    
    INDEX idx_metrics_name (metric_name),
    INDEX idx_metrics_date (measurement_date)
);

-- =====================================================
-- Initial Data Setup
-- =====================================================

-- Default Operations
INSERT INTO operations (operation_id, operation_description, operation_type) VALUES
('90', 'Raw Material Receipt', 'RECEIPT'),
('100', 'Production Processing', 'PRODUCTION'),
('110', 'Quality Control', 'QC'),
('120', 'Finished Goods', 'FINISHED'),
('999', 'General/Other', 'OTHER');

-- Default Locations
INSERT INTO locations (location_id, location_description, location_type) VALUES
('RECEIVING', 'Receiving Dock', 'RECEIPT'),
('PRODUCTION', 'Production Floor', 'PRODUCTION'),
('QC_HOLD', 'Quality Control Hold', 'QC'),
('FINISHED', 'Finished Goods', 'SHIPPING'),
('SCRAP', 'Scrap Area', 'WASTE');

-- System Configuration Defaults
INSERT INTO system_configuration (config_key, config_value, config_description, data_type) VALUES
('app_version', '1.0.0', 'Application Version', 'string'),
('max_transaction_days', '365', 'Maximum days to keep transaction history', 'integer'),
('default_location', 'RECEIVING', 'Default location for new items', 'string'),
('enable_audit_logging', 'true', 'Enable audit trail logging', 'boolean'),
('performance_monitoring', 'true', 'Enable performance monitoring', 'boolean');

-- Default Admin User
INSERT INTO users (user_id, username, full_name, email, role) VALUES
('admin', 'admin', 'System Administrator', 'admin@mtm.com', 'Administrator');

-- =====================================================
-- Triggers for Audit Logging
-- =====================================================

-- Inventory Audit Trigger
DELIMITER $$
CREATE TRIGGER tr_inventory_audit_insert 
    AFTER INSERT ON inventory 
    FOR EACH ROW
BEGIN
    INSERT INTO audit_log (table_name, operation_type, record_id, new_values, user_id)
    VALUES ('inventory', 'INSERT', NEW.inventory_id, 
            JSON_OBJECT('part_id', NEW.part_id, 'operation_id', NEW.operation_id, 
                       'location_id', NEW.location_id, 'quantity_on_hand', NEW.quantity_on_hand),
            COALESCE(@current_user_id, 'system'));
END$$

CREATE TRIGGER tr_inventory_audit_update 
    AFTER UPDATE ON inventory 
    FOR EACH ROW
BEGIN
    INSERT INTO audit_log (table_name, operation_type, record_id, old_values, new_values, user_id)
    VALUES ('inventory', 'UPDATE', NEW.inventory_id,
            JSON_OBJECT('quantity_on_hand', OLD.quantity_on_hand, 'quantity_allocated', OLD.quantity_allocated),
            JSON_OBJECT('quantity_on_hand', NEW.quantity_on_hand, 'quantity_allocated', NEW.quantity_allocated),
            COALESCE(@current_user_id, 'system'));
END$$
DELIMITER ;

-- =====================================================
-- Views for Common Queries
-- =====================================================

-- Current Inventory View
CREATE VIEW v_current_inventory AS
SELECT 
    i.inventory_id,
    i.part_id,
    p.part_description,
    i.operation_id,
    o.operation_description,
    i.location_id,
    l.location_description,
    i.quantity_on_hand,
    i.quantity_allocated,
    i.quantity_available,
    i.last_transaction_date
FROM inventory i
    INNER JOIN parts p ON i.part_id = p.part_id
    INNER JOIN operations o ON i.operation_id = o.operation_id
    INNER JOIN locations l ON i.location_id = l.location_id
WHERE p.active_status = TRUE
    AND o.active_status = TRUE
    AND l.active_status = TRUE;

-- Transaction History View
CREATE VIEW v_transaction_history AS
SELECT 
    t.transaction_id,
    t.transaction_type,
    t.part_id,
    p.part_description,
    t.operation_id,
    o.operation_description,
    t.from_location_id,
    fl.location_description as from_location_description,
    t.to_location_id,
    tl.location_description as to_location_description,
    t.quantity,
    t.unit_cost,
    t.reference_number,
    t.user_id,
    u.full_name as user_name,
    t.transaction_date
FROM inventory_transactions t
    INNER JOIN parts p ON t.part_id = p.part_id
    INNER JOIN operations o ON t.operation_id = o.operation_id
    LEFT JOIN locations fl ON t.from_location_id = fl.location_id
    LEFT JOIN locations tl ON t.to_location_id = tl.location_id
    INNER JOIN users u ON t.user_id = u.user_id;

-- =====================================================
-- End of Production Database Schema
-- =====================================================
-- For development changes, see: Development/Database_Files/Development_Database_Schema.sql
-- For new stored procedures, see: Development/Database_Files/New_Stored_Procedures.sql
-- For updated procedures, see: Development/Database_Files/Updated_Stored_Procedures.sql