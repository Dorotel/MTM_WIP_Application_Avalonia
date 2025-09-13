-- ================================================================
-- MTM WIP Application - MySQL User Creation Script
-- Compatible with MySQL 5.7 and phpMyAdmin
-- ================================================================
-- 
-- IMPORTANT: If using phpMyAdmin or shared hosting MySQL, skip the stored procedure
-- and use the Manual SQL Statements section below for better compatibility.
--
-- Error "Table 'mysql.servers' doesn't exist" indicates system table restrictions.
-- In such cases, use the manual approach which is more reliable.
-- ================================================================

USE mtm_wip_application_test;

-- Create a simplified stored procedure for MySQL 5.7/phpMyAdmin compatibility
DELIMITER //
CREATE PROCEDURE CreateUsersFromTable()
BEGIN
    DECLARE done INT DEFAULT FALSE;
    DECLARE username VARCHAR(50);
    DECLARE fullname VARCHAR(100);
    DECLARE user_cursor CURSOR FOR 
        SELECT `User`, `Full Name` 
        FROM usr_users 
        WHERE `User` NOT IN ('[ All Users ]', 'ROOT') 
        AND `User` IS NOT NULL 
        AND `User` != '';
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;
    -- Simple error handler that continues on any SQL exception
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;

    SELECT 'Starting user creation process...' as Status;
    
    -- Create users without checking existence first (simpler approach)
    OPEN user_cursor;
    create_loop: LOOP
        FETCH user_cursor INTO username, fullname;
        IF done THEN
            LEAVE create_loop;
        END IF;
        
        -- Try to drop user first (ignore errors if user doesn't exist)
        SET @sql = CONCAT('DROP USER IF EXISTS ''', username, '''@''%'';');
        SET @sql = CONCAT('DROP USER ''', username, '''@''%'';'); -- MySQL 5.7 compatible
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
        
        -- Create user with empty password (MySQL 5.7 syntax)
        SET @sql = CONCAT('CREATE USER ''', username, '''@''%'';');
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
        
        -- Grant full privileges on mtm_wip_application database
        SET @sql = CONCAT('GRANT ALL PRIVILEGES ON mtm_wip_application.* TO ''', username, '''@''%'';');
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
        
        -- Grant full privileges on mtm_wip_application_test database
        SET @sql = CONCAT('GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO ''', username, '''@''%'';');
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
        
        -- Log the creation (simplified)
        SELECT CONCAT('Processed user: ', username) as Status;
        
    END LOOP;
    CLOSE user_cursor;
    
    -- Flush privileges to apply changes
    FLUSH PRIVILEGES;
    
    SELECT 'User creation process completed!' as Result;
END//
DELIMITER ;

-- Execute the procedure (comment this out if it fails and use manual approach below)
-- CALL CreateUsersFromTable();
-- DROP PROCEDURE CreateUsersFromTable;

-- ================================================================
-- RECOMMENDED: Manual approach for maximum MySQL 5.7/phpMyAdmin compatibility
-- ================================================================
-- Use this approach if you encounter "Table 'mysql.servers' doesn't exist" error
-- or if you're using phpMyAdmin, shared hosting, or restricted MySQL environments.
--
-- Execute these statements in the following order:

-- Step 1: Drop existing users (MySQL 5.7 compatible - ignore errors)
SET sql_notes = 0; -- Disable warnings for non-existent users

-- Check and drop users individually (MySQL 5.7 doesn't support "IF EXISTS")
DROP USER 'DLAFOND'@'%';
DROP USER 'JEGBERT'@'%';
DROP USER 'RECEIVING'@'%';
DROP USER 'SHOP2'@'%';
DROP USER 'SHIPPING'@'%';
DROP USER 'DHAMMONS'@'%';
DROP USER 'JMAUER'@'%';
DROP USER 'KWILKER'@'%';
DROP USER 'MHANDLER'@'%';
DROP USER 'MIKESAMZ'@'%';
DROP USER 'MLAURIN'@'%';
DROP USER 'MLEDVINA'@'%';
DROP USER 'NPITSCH'@'%';
DROP USER 'PBAHR'@'%';
DROP USER 'SCARBON'@'%';
DROP USER 'TTELETZKE'@'%';
DROP USER 'TLINDLOFF'@'%';
DROP USER 'ABEEMAN'@'%';
DROP USER 'DHAGENOW'@'%';
DROP USER 'JORNALES'@'%';
DROP USER 'TSMAXWELL'@'%';
DROP USER 'CMUCHOWSKI'@'%';
DROP USER 'NWUNSCH'@'%';
DROP USER 'GWHITSON'@'%';
DROP USER 'JCASTRO'@'%';
DROP USER 'MVOSS'@'%';
DROP USER 'NLEE'@'%';
DROP USER 'JMILLER'@'%';
DROP USER 'SSNYDER'@'%';
DROP USER 'CSNYDER'@'%';
DROP USER 'BAUSTIN'@'%';
DROP USER 'DEBLAFOND'@'%';
DROP USER 'ASCHULTZ'@'%';
DROP USER 'SJACKSON'@'%';
DROP USER 'DRIEBE'@'%';
DROP USER 'TRADDATZ'@'%';
DROP USER 'SDETTLAFF'@'%';
DROP USER 'JWETAK'@'%';
DROP USER 'KSKATTEBO'@'%';
DROP USER 'AGAUTHIER'@'%';
DROP USER 'MBECKER'@'%';
DROP USER 'RLESSER'@'%';
DROP USER 'AGROELLE'@'%';
DROP USER 'CEHLENBECK'@'%';
DROP USER 'BNEUMAN'@'%';
DROP USER 'MTMDC'@'%';
DROP USER 'KDREWIESKE'@'%';
DROP USER 'MHERNANDEZ'@'%';
DROP USER 'KLEE'@'%';
DROP USER 'ADMININT'@'%';
DROP USER 'CALVAREZ'@'%';
DROP USER 'TYANG'@'%';
DROP USER 'KSMITH'@'%';
DROP USER 'JKOLL'@'%';
DROP USER 'JBEHRMANN'@'%';
DROP USER 'MDRESSEL'@'%';
DROP USER 'DSMITH'@'%';
DROP USER 'APIESCHEL'@'%';

SET sql_notes = 1; -- Re-enable warnings

-- Step 2: Create users without password (MySQL 5.7 syntax)
CREATE USER 'DLAFOND'@'%';
CREATE USER 'JEGBERT'@'%';
CREATE USER 'RECEIVING'@'%';
CREATE USER 'SHOP2'@'%';
CREATE USER 'SHIPPING'@'%';
CREATE USER 'DHAMMONS'@'%';
CREATE USER 'JMAUER'@'%';
CREATE USER 'KWILKER'@'%';
CREATE USER 'MHANDLER'@'%';
CREATE USER 'MIKESAMZ'@'%';
CREATE USER 'MLAURIN'@'%';
CREATE USER 'MLEDVINA'@'%';
CREATE USER 'NPITSCH'@'%';
CREATE USER 'PBAHR'@'%';
CREATE USER 'SCARBON'@'%';
CREATE USER 'TTELETZKE'@'%';
CREATE USER 'TLINDLOFF'@'%';
CREATE USER 'ABEEMAN'@'%';
CREATE USER 'DHAGENOW'@'%';
CREATE USER 'JORNALES'@'%';
CREATE USER 'TSMAXWELL'@'%';
CREATE USER 'CMUCHOWSKI'@'%';
CREATE USER 'NWUNSCH'@'%';
CREATE USER 'GWHITSON'@'%';
CREATE USER 'JCASTRO'@'%';
CREATE USER 'MVOSS'@'%';
CREATE USER 'NLEE'@'%';
CREATE USER 'JMILLER'@'%';
CREATE USER 'SSNYDER'@'%';
CREATE USER 'CSNYDER'@'%';
CREATE USER 'BAUSTIN'@'%';
CREATE USER 'DEBLAFOND'@'%';
CREATE USER 'ASCHULTZ'@'%';
CREATE USER 'SJACKSON'@'%';
CREATE USER 'DRIEBE'@'%';
CREATE USER 'TRADDATZ'@'%';
CREATE USER 'SDETTLAFF'@'%';
CREATE USER 'JWETAK'@'%';
CREATE USER 'KSKATTEBO'@'%';
CREATE USER 'AGAUTHIER'@'%';
CREATE USER 'MBECKER'@'%';
CREATE USER 'RLESSER'@'%';
CREATE USER 'AGROELLE'@'%';
CREATE USER 'CEHLENBECK'@'%';
CREATE USER 'BNEUMAN'@'%';
CREATE USER 'MTMDC'@'%';
CREATE USER 'KDREWIESKE'@'%';
CREATE USER 'MHERNANDEZ'@'%';
CREATE USER 'KLEE'@'%';
CREATE USER 'ADMININT'@'%';
CREATE USER 'CALVAREZ'@'%';
CREATE USER 'TYANG'@'%';
CREATE USER 'KSMITH'@'%';
CREATE USER 'JKOLL'@'%';
CREATE USER 'JBEHRMANN'@'%';
CREATE USER 'MDRESSEL'@'%';
CREATE USER 'DSMITH'@'%';
CREATE USER 'APIESCHEL'@'%';

-- Step 3: Grant privileges
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'DLAFOND'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'JEGBERT'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'RECEIVING'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'SHOP2'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'SHIPPING'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'DHAMMONS'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'JMAUER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'KWILKER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MHANDLER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MIKESAMZ'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MLAURIN'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MLEDVINA'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'NPITSCH'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'PBAHR'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'SCARBON'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'TTELETZKE'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'TLINDLOFF'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'ABEEMAN'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'DHAGENOW'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'JORNALES'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'TSMAXWELL'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'CMUCHOWSKI'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'NWUNSCH'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'GWHITSON'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'JCASTRO'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MVOSS'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'NLEE'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'JMILLER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'SSNYDER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'CSNYDER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'BAUSTIN'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'DEBLAFOND'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'ASCHULTZ'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'SJACKSON'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'DRIEBE'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'TRADDATZ'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'SDETTLAFF'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'JWETAK'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'KSKATTEBO'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'AGAUTHIER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MBECKER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'RLESSER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'AGROELLE'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'CEHLENBECK'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'BNEUMAN'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MTMDC'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'KDREWIESKE'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MHERNANDEZ'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'KLEE'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'ADMININT'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'CALVAREZ'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'TYANG'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'KSMITH'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'JKOLL'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'JBEHRMANN'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'MDRESSEL'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'DSMITH'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application.* TO 'APIESCHEL'@'%';

-- Step 4: Also grant privileges on test database
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'DLAFOND'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'JEGBERT'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'RECEIVING'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'SHOP2'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'SHIPPING'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'DHAMMONS'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'JMAUER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'KWILKER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'MHANDLER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'MIKESAMZ'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'MLAURIN'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'MLEDVINA'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'NPITSCH'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'PBAHR'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'SCARBON'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'TTELETZKE'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'TLINDLOFF'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'ABEEMAN'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'DHAGENOW'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'JORNALES'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'TSMAXWELL'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'CMUCHOWSKI'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'NWUNSCH'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'GWHITSON'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'JCASTRO'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'MVOSS'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'NLEE'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'JMILLER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'SSNYDER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'CSNYDER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'BAUSTIN'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'DEBLAFOND'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'ASCHULTZ'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'SJACKSON'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'DRIEBE'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'TRADDATZ'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'SDETTLAFF'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'JWETAK'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'KSKATTEBO'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'AGAUTHIER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'MBECKER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'RLESSER'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'AGROELLE'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'CEHLENBECK'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'BNEUMAN'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'MTMDC'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'KDREWIESKE'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'MHERNANDEZ'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'KLEE'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'ADMININT'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'CALVAREZ'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'TYANG'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'KSMITH'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'JKOLL'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'JBEHRMANN'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'MDRESSEL'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'DSMITH'@'%';
GRANT ALL PRIVILEGES ON mtm_wip_application_test.* TO 'APIESCHEL'@'%';

-- Step 5: Flush privileges (required after user creation/modification)
FLUSH PRIVILEGES;

-- ================================================================
-- Verification (OPTIONAL - skip if system tables are restricted)
-- ================================================================
-- If your MySQL environment allows system table access, uncomment the following:
-- SELECT User, Host FROM mysql.user WHERE User IN (
--     'DLAFOND', 'JEGBERT', 'RECEIVING', 'SHOP2', 'SHIPPING', 'DHAMMONS', 'JMAUER', 'KWILKER', 'MHANDLER', 
--     'MIKESAMZ', 'MLAURIN', 'MLEDVINA', 'NPITSCH', 'PBAHR', 'SCARBON', 'TTELETZKE', 'TLINDLOFF', 'ABEEMAN', 
--     'DHAGENOW', 'JORNALES', 'TSMAXWELL', 'CMUCHOWSKI', 'NWUNSCH', 'GWHITSON', 'JCASTRO', 'MVOSS', 'NLEE', 
--     'JMILLER', 'SSNYDER', 'CSNYDER', 'BAUSTIN', 'DEBLAFOND', 'ASCHULTZ', 'SJACKSON', 'DRIEBE', 'TRADDATZ', 
--     'SDETTLAFF', 'JWETAK', 'KSKATTEBO', 'AGAUTHIER', 'MBECKER', 'RLESSER', 'AGROELLE', 'CEHLENBECK', 
--     'BNEUMAN', 'MTMDC', 'KDREWIESKE', 'MHERNANDEZ', 'KLEE', 'ADMININT', 'CALVAREZ', 'TYANG', 'KSMITH', 
--     'JKOLL', 'JBEHRMANN', 'MDRESSEL', 'DSMITH', 'APIESCHEL'
-- );

-- Alternative verification: Try to connect as one of the created users
-- This will only work if the user creation was successful
-- Example: SHOW DATABASES; (run this after connecting as one of the created users)

-- ================================================================
-- Instructions for phpMyAdmin Usage
-- ================================================================
-- RECOMMENDED APPROACH for phpMyAdmin/shared hosting:
-- Use the Manual SQL Statements above (lines 92-360) as they are more reliable
-- and don't require system table access.
--
-- 1. For the manual approach (RECOMMENDED):
--    - Execute Step 1: DROP USER statements first (ignore any "user doesn't exist" errors)
--    - Execute Step 2: CREATE USER statements  
--    - Execute Step 3: GRANT privileges on mtm_wip_application database
--    - Execute Step 4: GRANT privileges on mtm_wip_application_test database
--    - Execute Step 5: FLUSH PRIVILEGES and verify
--
-- 2. For the stored procedure approach (if manual fails):
--    - Only try this if your MySQL installation has full system table access
--    - Copy lines 19-80 and execute in phpMyAdmin's SQL tab
--
-- 3. Troubleshooting common errors:
--    - "Table 'mysql.servers' doesn't exist" → Use manual approach instead
--    - "DROP USER failed" → User doesn't exist, this is expected and safe to ignore
--    - "CREATE USER failed" → User already exists, run corresponding DROP USER first
--    - "FLUSH PRIVILEGES failed" → Your MySQL environment restricts system table access
--      In this case, user creation may still work, but verification queries won't
--    - Always run FLUSH PRIVILEGES after user creation/modification (if permitted)
--
-- 4. Testing user creation success:
--    - Try connecting to your database using one of the created usernames
--    - If connection succeeds, user creation was successful
--    - Run SHOW DATABASES; to verify database access permissions