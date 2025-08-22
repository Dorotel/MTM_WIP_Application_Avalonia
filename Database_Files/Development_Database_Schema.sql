-- phpMyAdmin SQL Dump
-- version 5.2.2
-- https://www.phpmyadmin.net/
--
-- Host: localhost:3306
-- Generation Time: Aug 22, 2025 at 03:38 PM
-- Server version: 5.7.24
-- PHP Version: 8.3.1

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

--
-- Database: `mtm_wip_application_test`
--

-- --------------------------------------------------------

--
-- Table structure for table `app_themes`
--

CREATE TABLE `app_themes` (
  `ThemeName` varchar(100) NOT NULL,
  `SettingsJson` json NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `inv_inventory`
--

CREATE TABLE `inv_inventory` (
  `ID` int(11) NOT NULL,
  `PartID` varchar(300) NOT NULL,
  `Location` varchar(100) NOT NULL,
  `Operation` varchar(100) DEFAULT NULL,
  `Quantity` int(11) NOT NULL,
  `ItemType` varchar(100) NOT NULL DEFAULT 'WIP',
  `ReceiveDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `User` varchar(100) NOT NULL,
  `BatchNumber` varchar(300) DEFAULT NULL,
  `Notes` varchar(1000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `inv_inventory_batch_seq`
--

CREATE TABLE `inv_inventory_batch_seq` (
  `last_batch_number` bigint(20) NOT NULL,
  `current_match` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `inv_transaction`
--

CREATE TABLE `inv_transaction` (
  `ID` int(11) NOT NULL,
  `TransactionType` enum('IN','OUT','TRANSFER') NOT NULL,
  `BatchNumber` varchar(300) DEFAULT NULL,
  `PartID` varchar(300) NOT NULL,
  `FromLocation` varchar(100) DEFAULT NULL,
  `ToLocation` varchar(100) DEFAULT NULL,
  `Operation` varchar(100) DEFAULT NULL,
  `Quantity` int(11) NOT NULL,
  `Notes` varchar(1000) DEFAULT NULL,
  `User` varchar(100) NOT NULL,
  `ItemType` varchar(100) NOT NULL DEFAULT 'WIP',
  `ReceiveDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `log_changelog`
--

CREATE TABLE `log_changelog` (
  `Version` varchar(50) NOT NULL,
  `Notes` longtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `log_error`
--

CREATE TABLE `log_error` (
  `ID` int(11) NOT NULL,
  `User` varchar(100) DEFAULT NULL,
  `Severity` enum('Information','Warning','Error','Critical','High') NOT NULL DEFAULT 'Error',
  `ErrorType` varchar(100) DEFAULT NULL,
  `ErrorMessage` text NOT NULL,
  `StackTrace` text,
  `ModuleName` varchar(200) DEFAULT NULL,
  `MethodName` varchar(200) DEFAULT NULL,
  `AdditionalInfo` text,
  `MachineName` varchar(100) DEFAULT NULL,
  `OSVersion` varchar(100) DEFAULT NULL,
  `AppVersion` varchar(50) DEFAULT NULL,
  `ErrorTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `md_item_types`
--

CREATE TABLE `md_item_types` (
  `ID` int(11) NOT NULL,
  `ItemType` varchar(100) NOT NULL,
  `IssuedBy` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `md_locations`
--

CREATE TABLE `md_locations` (
  `ID` int(11) NOT NULL,
  `Location` varchar(100) NOT NULL,
  `Building` varchar(100) NOT NULL DEFAULT 'Expo',
  `IssuedBy` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `md_operation_numbers`
--

CREATE TABLE `md_operation_numbers` (
  `ID` int(11) NOT NULL,
  `Operation` varchar(100) NOT NULL,
  `IssuedBy` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `md_part_ids`
--

CREATE TABLE `md_part_ids` (
  `ID` int(11) NOT NULL,
  `PartID` varchar(300) NOT NULL,
  `Customer` varchar(300) NOT NULL,
  `Description` varchar(300) NOT NULL,
  `IssuedBy` varchar(100) NOT NULL,
  `ItemType` varchar(100) NOT NULL,
  `Operations` json DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `sys_last_10_transactions`
--

CREATE TABLE `sys_last_10_transactions` (
  `ID` int(11) NOT NULL,
  `User` varchar(100) NOT NULL,
  `PartID` varchar(300) NOT NULL,
  `Operation` varchar(100) NOT NULL,
  `Quantity` int(11) NOT NULL,
  `ReceiveDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Position` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `sys_roles`
--

CREATE TABLE `sys_roles` (
  `ID` int(11) NOT NULL,
  `RoleName` varchar(50) NOT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `Permissions` varchar(1000) DEFAULT NULL,
  `IsSystem` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'True for built-in roles',
  `CreatedBy` varchar(100) NOT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `sys_user_roles`
--

CREATE TABLE `sys_user_roles` (
  `UserID` int(11) NOT NULL,
  `RoleID` int(11) NOT NULL,
  `AssignedBy` varchar(100) NOT NULL,
  `AssignedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `usr_ui_settings`
--

CREATE TABLE `usr_ui_settings` (
  `UserId` varchar(64) NOT NULL,
  `SettingsJson` json NOT NULL,
  `ShortcutsJson` json NOT NULL,
  `UpdatedAt` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `usr_users`
--

CREATE TABLE `usr_users` (
  `ID` int(11) NOT NULL,
  `User` varchar(100) NOT NULL,
  `Full Name` varchar(200) DEFAULT NULL,
  `Shift` varchar(50) NOT NULL DEFAULT '1',
  `VitsUser` tinyint(1) NOT NULL DEFAULT '0',
  `Pin` varchar(50) DEFAULT NULL,
  `LastShownVersion` varchar(50) NOT NULL DEFAULT '0.0.0.0',
  `HideChangeLog` varchar(50) NOT NULL DEFAULT 'false',
  `Theme_Name` varchar(50) NOT NULL DEFAULT 'Default (Black and White)',
  `Theme_FontSize` int(11) NOT NULL DEFAULT '9',
  `VisualUserName` varchar(50) NOT NULL DEFAULT 'User Name',
  `VisualPassword` varchar(50) NOT NULL DEFAULT 'Password',
  `WipServerAddress` varchar(15) NOT NULL DEFAULT '172.16.1.104',
  `WIPDatabase` varchar(300) NOT NULL DEFAULT 'mtm_wip_application',
  `WipServerPort` varchar(10) NOT NULL DEFAULT '3306'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `app_themes`
--
ALTER TABLE `app_themes`
  ADD PRIMARY KEY (`ThemeName`);

--
-- Indexes for table `inv_inventory`
--
ALTER TABLE `inv_inventory`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `idx_partid_location` (`PartID`,`Location`),
  ADD KEY `idx_operation` (`Operation`),
  ADD KEY `idx_receivedate` (`ReceiveDate`);

--
-- Indexes for table `inv_inventory_batch_seq`
--
ALTER TABLE `inv_inventory_batch_seq`
  ADD PRIMARY KEY (`last_batch_number`);

--
-- Indexes for table `inv_transaction`
--
ALTER TABLE `inv_transaction`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `idx_partid` (`PartID`),
  ADD KEY `idx_user` (`User`),
  ADD KEY `idx_datetime` (`ReceiveDate`);

--
-- Indexes for table `log_changelog`
--
ALTER TABLE `log_changelog`
  ADD PRIMARY KEY (`Version`);

--
-- Indexes for table `log_error`
--
ALTER TABLE `log_error`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `idx_errortime` (`ErrorTime`),
  ADD KEY `idx_user` (`User`),
  ADD KEY `idx_severity` (`Severity`),
  ADD KEY `idx_errortype` (`ErrorType`);

--
-- Indexes for table `md_item_types`
--
ALTER TABLE `md_item_types`
  ADD PRIMARY KEY (`ID`),
  ADD UNIQUE KEY `uq_type` (`ItemType`);

--
-- Indexes for table `md_locations`
--
ALTER TABLE `md_locations`
  ADD PRIMARY KEY (`ID`),
  ADD UNIQUE KEY `uq_location` (`Location`);

--
-- Indexes for table `md_operation_numbers`
--
ALTER TABLE `md_operation_numbers`
  ADD PRIMARY KEY (`ID`),
  ADD UNIQUE KEY `uq_operation` (`Operation`);

--
-- Indexes for table `md_part_ids`
--
ALTER TABLE `md_part_ids`
  ADD PRIMARY KEY (`ID`),
  ADD UNIQUE KEY `uq_item_number` (`PartID`);

--
-- Indexes for table `sys_last_10_transactions`
--
ALTER TABLE `sys_last_10_transactions`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `idx_user_datetime` (`User`,`ReceiveDate`);

--
-- Indexes for table `sys_roles`
--
ALTER TABLE `sys_roles`
  ADD PRIMARY KEY (`ID`),
  ADD UNIQUE KEY `uq_rolename` (`RoleName`);

--
-- Indexes for table `sys_user_roles`
--
ALTER TABLE `sys_user_roles`
  ADD PRIMARY KEY (`UserID`,`RoleID`),
  ADD KEY `idx_userid` (`UserID`),
  ADD KEY `idx_roleid` (`RoleID`);

--
-- Indexes for table `usr_ui_settings`
--
ALTER TABLE `usr_ui_settings`
  ADD PRIMARY KEY (`UserId`);

--
-- Indexes for table `usr_users`
--
ALTER TABLE `usr_users`
  ADD PRIMARY KEY (`ID`),
  ADD UNIQUE KEY `uq_user` (`User`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `inv_inventory`
--
ALTER TABLE `inv_inventory`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `inv_transaction`
--
ALTER TABLE `inv_transaction`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `log_error`
--
ALTER TABLE `log_error`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `md_item_types`
--
ALTER TABLE `md_item_types`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `md_locations`
--
ALTER TABLE `md_locations`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `md_operation_numbers`
--
ALTER TABLE `md_operation_numbers`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `md_part_ids`
--
ALTER TABLE `md_part_ids`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `sys_last_10_transactions`
--
ALTER TABLE `sys_last_10_transactions`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `sys_roles`
--
ALTER TABLE `sys_roles`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `usr_users`
--
ALTER TABLE `usr_users`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `sys_user_roles`
--
ALTER TABLE `sys_user_roles`
  ADD CONSTRAINT `sys_user_roles_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `usr_users` (`ID`) ON DELETE CASCADE,
  ADD CONSTRAINT `sys_user_roles_ibfk_2` FOREIGN KEY (`RoleID`) REFERENCES `sys_roles` (`ID`) ON DELETE CASCADE;

--
-- Constraints for table `usr_ui_settings`
--
ALTER TABLE `usr_ui_settings`
  ADD CONSTRAINT `usr_ui_settings_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `usr_users` (`User`);
COMMIT;
