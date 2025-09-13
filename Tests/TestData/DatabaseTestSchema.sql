-- MTM Test Database Schema
-- This script creates the basic tables and stored procedures needed for testing

-- Create database tables
CREATE TABLE IF NOT EXISTS `md_part_ids` (
  `PartID` varchar(50) NOT NULL,
  `Description` varchar(200) DEFAULT NULL,
  `IsActive` tinyint(1) DEFAULT 1,
  PRIMARY KEY (`PartID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `md_operation_numbers` (
  `OperationNumber` varchar(10) NOT NULL,
  `Description` varchar(200) DEFAULT NULL,
  `IsActive` tinyint(1) DEFAULT 1,
  PRIMARY KEY (`OperationNumber`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `md_locations` (
  `Location` varchar(50) NOT NULL,
  `Description` varchar(200) DEFAULT NULL,
  `IsActive` tinyint(1) DEFAULT 1,
  PRIMARY KEY (`Location`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `usr_users` (
  `User` varchar(50) NOT NULL,
  `FullName` varchar(100) DEFAULT NULL,
  `IsActive` tinyint(1) DEFAULT 1,
  PRIMARY KEY (`User`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `inv_inventory` (
  `ID` int AUTO_INCREMENT,
  `PartID` varchar(50) NOT NULL,
  `OperationNumber` varchar(10) NOT NULL,
  `Quantity` int NOT NULL DEFAULT 0,
  `Location` varchar(50) NOT NULL,
  `LastUpdated` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `LastUpdatedBy` varchar(50) DEFAULT NULL,
  `WorkOrder` varchar(50) DEFAULT NULL,
  `BatchNumber` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `idx_part_operation` (`PartID`, `OperationNumber`),
  KEY `idx_location` (`Location`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `inv_transaction_history` (
  `ID` int AUTO_INCREMENT,
  `PartID` varchar(50) NOT NULL,
  `OperationNumber` varchar(10) NOT NULL,
  `Quantity` int NOT NULL,
  `Location` varchar(50) NOT NULL,
  `TransactionType` varchar(20) NOT NULL,
  `Timestamp` timestamp DEFAULT CURRENT_TIMESTAMP,
  `User` varchar(50) DEFAULT NULL,
  `Notes` text DEFAULT NULL,
  `SessionID` varchar(50) DEFAULT NULL,
  `WorkOrder` varchar(50) DEFAULT NULL,
  `BatchNumber` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `idx_part_timestamp` (`PartID`, `Timestamp`),
  KEY `idx_session` (`SessionID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `qb_quick_buttons` (
  `ID` int AUTO_INCREMENT,
  `PartID` varchar(50) NOT NULL,
  `OperationNumber` varchar(10) NOT NULL,
  `Quantity` int NOT NULL,
  `Location` varchar(50) NOT NULL,
  `LastUsed` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UseCount` int DEFAULT 1,
  `User` varchar(50) DEFAULT NULL,
  `CreatedDate` timestamp DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`ID`),
  KEY `idx_user_lastused` (`User`, `LastUsed`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `usr_sessions` (
  `SessionID` varchar(50) NOT NULL,
  `User` varchar(50) NOT NULL,
  `StartTime` timestamp DEFAULT CURRENT_TIMESTAMP,
  `EndTime` timestamp NULL DEFAULT NULL,
  `IsActive` tinyint(1) DEFAULT 1,
  PRIMARY KEY (`SessionID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `log_error_logs` (
  `ID` int AUTO_INCREMENT,
  `ErrorMessage` text NOT NULL,
  `StackTrace` text DEFAULT NULL,
  `Context` varchar(200) DEFAULT NULL,
  `User` varchar(50) DEFAULT NULL,
  `Timestamp` timestamp DEFAULT CURRENT_TIMESTAMP,
  `MachineName` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `idx_timestamp` (`Timestamp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Basic stored procedures for testing
DELIMITER //

CREATE PROCEDURE IF NOT EXISTS `inv_inventory_Add_Item`(
  IN p_PartID VARCHAR(50),
  IN p_OperationNumber VARCHAR(10),
  IN p_Quantity INT,
  IN p_Location VARCHAR(50),
  IN p_User VARCHAR(50),
  IN p_WorkOrder VARCHAR(50),
  IN p_BatchNumber VARCHAR(50)
)
BEGIN
  DECLARE EXIT HANDLER FOR SQLEXCEPTION 
  BEGIN
    ROLLBACK;
    SELECT 0 as Status, 'Error adding inventory item' as Message;
  END;

  START TRANSACTION;
  
  INSERT INTO inv_inventory (PartID, OperationNumber, Quantity, Location, LastUpdatedBy, WorkOrder, BatchNumber)
  VALUES (p_PartID, p_OperationNumber, p_Quantity, p_Location, p_User, p_WorkOrder, p_BatchNumber)
  ON DUPLICATE KEY UPDATE 
    Quantity = Quantity + p_Quantity,
    LastUpdatedBy = p_User,
    LastUpdated = CURRENT_TIMESTAMP;
    
  INSERT INTO inv_transaction_history (PartID, OperationNumber, Quantity, Location, TransactionType, User, WorkOrder, BatchNumber)
  VALUES (p_PartID, p_OperationNumber, p_Quantity, p_Location, 'IN', p_User, p_WorkOrder, p_BatchNumber);
  
  COMMIT;
  SELECT 1 as Status, 'Inventory item added successfully' as Message;
END //

CREATE PROCEDURE IF NOT EXISTS `inv_inventory_Get_ByPartID`(
  IN p_PartID VARCHAR(50)
)
BEGIN
  SELECT PartID, OperationNumber, Quantity, Location, LastUpdated, LastUpdatedBy, WorkOrder, BatchNumber
  FROM inv_inventory 
  WHERE PartID = p_PartID 
  ORDER BY OperationNumber;
  
  SELECT 1 as Status, 'Data retrieved successfully' as Message;
END //

CREATE PROCEDURE IF NOT EXISTS `md_part_ids_Get_All`()
BEGIN
  SELECT PartID, Description, IsActive 
  FROM md_part_ids 
  WHERE IsActive = 1 
  ORDER BY PartID;
  
  SELECT 1 as Status, 'Data retrieved successfully' as Message;
END //

CREATE PROCEDURE IF NOT EXISTS `qb_quick_buttons_Get_Last10`(
  IN p_User VARCHAR(50)
)
BEGIN
  SELECT PartID, OperationNumber, Quantity, Location, LastUsed, UseCount
  FROM qb_quick_buttons 
  WHERE User = p_User 
  ORDER BY LastUsed DESC 
  LIMIT 10;
  
  SELECT 1 as Status, 'Data retrieved successfully' as Message;
END //

CREATE PROCEDURE IF NOT EXISTS `log_error_Add_Error`(
  IN p_ErrorMessage TEXT,
  IN p_StackTrace TEXT,
  IN p_Context VARCHAR(200),
  IN p_User VARCHAR(50),
  IN p_MachineName VARCHAR(100)
)
BEGIN
  INSERT INTO log_error_logs (ErrorMessage, StackTrace, Context, User, MachineName)
  VALUES (p_ErrorMessage, p_StackTrace, p_Context, p_User, p_MachineName);
  
  SELECT 1 as Status, 'Error logged successfully' as Message;
END //

DELIMITER ;