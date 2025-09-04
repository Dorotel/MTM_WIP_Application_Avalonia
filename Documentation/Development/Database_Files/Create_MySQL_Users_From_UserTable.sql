-- ================================================================
-- MTM WIP Application - MySQL User Creation Script
-- ================================================================
-- This script creates MySQL server users based on the usr_users table
-- Users are created with NO PASSWORD and full privileges on mtm_wip_application database
-- 
-- Usage:
-- 1. Execute this script as MySQL root user
-- 2. All users from usr_users table will be created as MySQL server users
-- 3. Users will have full access to mtm_wip_application database only
--
-- WARNING: This creates users with NO PASSWORD - suitable for local development only
-- ================================================================

-- Ensure we're using the correct database
USE mtm_wip_application;

-- Create a stored procedure to generate user creation statements
DELIMITER //
CREATE PROCEDURE CreateUsersFromTable()
BEGIN
    DECLARE done INT DEFAULT FALSE;
    DECLARE username VARCHAR(50);
    DECLARE fullname VARCHAR(100);
    DECLARE user_cursor CURSOR FOR 
        SELECT `User`, `Full Name` 
        FROM usr_users 
        WHERE `User` NOT IN ('[ All Users ]', 'ROOT') -- Exclude system entries
        AND `User` IS NOT NULL 
        AND `User` != '';
    
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;

    -- Drop existing users first (ignore errors if user doesn't exist)
    OPEN user_cursor;
    read_loop: LOOP
        FETCH user_cursor INTO username, fullname;
        IF done THEN
            LEAVE read_loop;
        END IF;
        
        -- Drop user if exists (MySQL 5.7+ syntax)
        SET @sql = CONCAT('DROP USER IF EXISTS ''', username, '''@''%'';');
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
        
    END LOOP;
    CLOSE user_cursor;
    
    -- Reset the cursor
    SET done = FALSE;
    
    -- Create new users
    OPEN user_cursor;
    create_loop: LOOP
        FETCH user_cursor INTO username, fullname;
        IF done THEN
            LEAVE create_loop;
        END IF;
        
        -- Create user with no password
        SET @sql = CONCAT('CREATE USER ''', username, '''@''%'' IDENTIFIED BY '''';');
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
        
        -- Log the creation
        SELECT CONCAT('Created MySQL user: ', username, ' (', fullname, ')') as Status;
        
    END LOOP;
    CLOSE user_cursor;
    
    -- Flush privileges to apply changes
    FLUSH PRIVILEGES;
    
    SELECT 'User creation completed successfully!' as Result;
END//
DELIMITER ;

-- Execute the procedure
CALL CreateUsersFromTable();

-- Drop the temporary procedure
DROP PROCEDURE CreateUsersFromTable;

-- ================================================================
-- Manual SQL Statements (Alternative approach)
-- ================================================================
-- If the stored procedure approach doesn't work, you can use these manual statements:

-- Drop existing users (ignore errors)
DROP USER IF EXISTS 'DLAFOND'@'%';
DROP USER IF EXISTS 'JEGBERT'@'%';
DROP USER IF EXISTS 'RECEIVING'@'%';
DROP USER IF EXISTS 'SHOP2'@'%';
DROP USER IF EXISTS 'SHIPPING'@'%';
DROP USER IF EXISTS 'DHAMMONS'@'%';
DROP USER IF EXISTS 'JMAUER'@'%';
DROP USER IF EXISTS 'KWILKER'@'%';
DROP USER IF EXISTS 'MHANDLER'@'%';
DROP USER IF EXISTS 'MIKESAMZ'@'%';
DROP USER IF EXISTS 'MLAURIN'@'%';
DROP USER IF EXISTS 'MLEDVINA'@'%';
DROP USER IF EXISTS 'NPITSCH'@'%';
DROP USER IF EXISTS 'PBAHR'@'%';
DROP USER IF EXISTS 'SCARBON'@'%';
DROP USER IF EXISTS 'TTELETZKE'@'%';
DROP USER IF EXISTS 'TLINDLOFF'@'%';
DROP USER IF EXISTS 'ABEEMAN'@'%';
DROP USER IF EXISTS 'DHAGENOW'@'%';
DROP USER IF EXISTS 'JORNALES'@'%';
DROP USER IF EXISTS 'TSMAXWELL'@'%';
DROP USER IF EXISTS 'CMUCHOWSKI'@'%';
DROP USER IF EXISTS 'NWUNSCH'@'%';
DROP USER IF EXISTS 'GWHITSON'@'%';
DROP USER IF EXISTS 'JCASTRO'@'%';
DROP USER IF EXISTS 'MVOSS'@'%';
DROP USER IF EXISTS 'NLEE'@'%';
DROP USER IF EXISTS 'JMILLER'@'%';
DROP USER IF EXISTS 'SSNYDER'@'%';
DROP USER IF EXISTS 'CSNYDER'@'%';
DROP USER IF EXISTS 'BAUSTIN'@'%';
DROP USER IF EXISTS 'DEBLAFOND'@'%';
DROP USER IF EXISTS 'ASCHULTZ'@'%';
DROP USER IF EXISTS 'SJACKSON'@'%';
DROP USER IF EXISTS 'DRIEBE'@'%';
DROP USER IF EXISTS 'TRADDATZ'@'%';
DROP USER IF EXISTS 'SDETTLAFF'@'%';
DROP USER IF EXISTS 'JWETAK'@'%';
DROP USER IF EXISTS 'KSKATTEBO'@'%';
DROP USER IF EXISTS 'AGAUTHIER'@'%';
DROP USER IF EXISTS 'MBECKER'@'%';
DROP USER IF EXISTS 'RLESSER'@'%';
DROP USER IF EXISTS 'AGROELLE'@'%';
DROP USER IF EXISTS 'CEHLENBECK'@'%';
DROP USER IF EXISTS 'BNEUMAN'@'%';
DROP USER IF EXISTS 'MTMDC'@'%';
DROP USER IF EXISTS 'KDREWIESKE'@'%';
DROP USER IF EXISTS 'MHERNANDEZ'@'%';
DROP USER IF EXISTS 'KLEE'@'%';
DROP USER IF EXISTS 'ADMININT'@'%';
DROP USER IF EXISTS 'CALVAREZ'@'%';
DROP USER IF EXISTS 'TYANG'@'%';
DROP USER IF EXISTS 'KSMITH'@'%';
DROP USER IF EXISTS 'JKOLL'@'%';
DROP USER IF EXISTS 'JBEHRMANN'@'%';
DROP USER IF EXISTS 'MDRESSEL'@'%';
DROP USER IF EXISTS 'DSMITH'@'%';
DROP USER IF EXISTS 'APIESCHEL'@'%';

-- Create users with no password
CREATE USER 'DLAFOND'@'%' IDENTIFIED BY '';
CREATE USER 'JEGBERT'@'%' IDENTIFIED BY '';
CREATE USER 'RECEIVING'@'%' IDENTIFIED BY '';
CREATE USER 'SHOP2'@'%' IDENTIFIED BY '';
CREATE USER 'SHIPPING'@'%' IDENTIFIED BY '';
CREATE USER 'DHAMMONS'@'%' IDENTIFIED BY '';
CREATE USER 'JMAUER'@'%' IDENTIFIED BY '';
CREATE USER 'KWILKER'@'%' IDENTIFIED BY '';
CREATE USER 'MHANDLER'@'%' IDENTIFIED BY '';
CREATE USER 'MIKESAMZ'@'%' IDENTIFIED BY '';
CREATE USER 'MLAURIN'@'%' IDENTIFIED BY '';
CREATE USER 'MLEDVINA'@'%' IDENTIFIED BY '';
CREATE USER 'NPITSCH'@'%' IDENTIFIED BY '';
CREATE USER 'PBAHR'@'%' IDENTIFIED BY '';
CREATE USER 'SCARBON'@'%' IDENTIFIED BY '';
CREATE USER 'TTELETZKE'@'%' IDENTIFIED BY '';
CREATE USER 'TLINDLOFF'@'%' IDENTIFIED BY '';
CREATE USER 'ABEEMAN'@'%' IDENTIFIED BY '';
CREATE USER 'DHAGENOW'@'%' IDENTIFIED BY '';
CREATE USER 'JORNALES'@'%' IDENTIFIED BY '';
CREATE USER 'TSMAXWELL'@'%' IDENTIFIED BY '';
CREATE USER 'CMUCHOWSKI'@'%' IDENTIFIED BY '';
CREATE USER 'NWUNSCH'@'%' IDENTIFIED BY '';
CREATE USER 'GWHITSON'@'%' IDENTIFIED BY '';
CREATE USER 'JCASTRO'@'%' IDENTIFIED BY '';
CREATE USER 'MVOSS'@'%' IDENTIFIED BY '';
CREATE USER 'NLEE'@'%' IDENTIFIED BY '';
CREATE USER 'JMILLER'@'%' IDENTIFIED BY '';
CREATE USER 'SSNYDER'@'%' IDENTIFIED BY '';
CREATE USER 'CSNYDER'@'%' IDENTIFIED BY '';
CREATE USER 'BAUSTIN'@'%' IDENTIFIED BY '';
CREATE USER 'DEBLAFOND'@'%' IDENTIFIED BY '';
CREATE USER 'ASCHULTZ'@'%' IDENTIFIED BY '';
CREATE USER 'SJACKSON'@'%' IDENTIFIED BY '';
CREATE USER 'DRIEBE'@'%' IDENTIFIED BY '';
CREATE USER 'TRADDATZ'@'%' IDENTIFIED BY '';
CREATE USER 'SDETTLAFF'@'%' IDENTIFIED BY '';
CREATE USER 'JWETAK'@'%' IDENTIFIED BY '';
CREATE USER 'KSKATTEBO'@'%' IDENTIFIED BY '';
CREATE USER 'AGAUTHIER'@'%' IDENTIFIED BY '';
CREATE USER 'MBECKER'@'%' IDENTIFIED BY '';
CREATE USER 'RLESSER'@'%' IDENTIFIED BY '';
CREATE USER 'AGROELLE'@'%' IDENTIFIED BY '';
CREATE USER 'CEHLENBECK'@'%' IDENTIFIED BY '';
CREATE USER 'BNEUMAN'@'%' IDENTIFIED BY '';
CREATE USER 'MTMDC'@'%' IDENTIFIED BY '';
CREATE USER 'KDREWIESKE'@'%' IDENTIFIED BY '';
CREATE USER 'MHERNANDEZ'@'%' IDENTIFIED BY '';
CREATE USER 'KLEE'@'%' IDENTIFIED BY '';
CREATE USER 'ADMININT'@'%' IDENTIFIED BY '';
CREATE USER 'CALVAREZ'@'%' IDENTIFIED BY '';
CREATE USER 'TYANG'@'%' IDENTIFIED BY '';
CREATE USER 'KSMITH'@'%' IDENTIFIED BY '';
CREATE USER 'JKOLL'@'%' IDENTIFIED BY '';
CREATE USER 'JBEHRMANN'@'%' IDENTIFIED BY '';
CREATE USER 'MDRESSEL'@'%' IDENTIFIED BY '';
CREATE USER 'DSMITH'@'%' IDENTIFIED BY '';
CREATE USER 'APIESCHEL'@'%' IDENTIFIED BY '';

-- Grant privileges on mtm_wip_application database
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

-- Apply all changes
FLUSH PRIVILEGES;

-- Show created users
SELECT User, Host FROM mysql.user WHERE User IN (
    'DLAFOND', 'JEGBERT', 'RECEIVING', 'SHOP2', 'SHIPPING', 'DHAMMONS', 'JMAUER', 'KWILKER', 'MHANDLER', 
    'MIKESAMZ', 'MLAURIN', 'MLEDVINA', 'NPITSCH', 'PBAHR', 'SCARBON', 'TTELETZKE', 'TLINDLOFF', 'ABEEMAN', 
    'DHAGENOW', 'JORNALES', 'TSMAXWELL', 'CMUCHOWSKI', 'NWUNSCH', 'GWHITSON', 'JCASTRO', 'MVOSS', 'NLEE', 
    'JMILLER', 'SSNYDER', 'CSNYDER', 'BAUSTIN', 'DEBLAFOND', 'ASCHULTZ', 'SJACKSON', 'DRIEBE', 'TRADDATZ', 
    'SDETTLAFF', 'JWETAK', 'KSKATTEBO', 'AGAUTHIER', 'MBECKER', 'RLESSER', 'AGROELLE', 'CEHLENBECK', 
    'BNEUMAN', 'MTMDC', 'KDREWIESKE', 'MHERNANDEZ', 'KLEE', 'ADMININT', 'CALVAREZ', 'TYANG', 'KSMITH', 
    'JKOLL', 'JBEHRMANN', 'MDRESSEL', 'DSMITH', 'APIESCHEL'
);

-- ================================================================
-- Test Connection (Optional)
-- ================================================================
-- You can test these users by connecting with:
-- mysql -u USERNAME -h localhost mtm_wip_application
-- (no -p flag needed since there's no password)

-- ================================================================
-- Security Notes:
-- ================================================================
-- WARNING: These users have NO PASSWORD and full database access
-- This is suitable for:
-- - Local development environments
-- - Internal networks with controlled access
-- - Testing environments
--
-- NOT suitable for:
-- - Production environments
-- - Internet-accessible servers
-- - Environments requiring user authentication
--
-- Consider adding passwords in production:
-- ALTER USER 'username'@'%' IDENTIFIED BY 'secure_password';
-- ================================================================
