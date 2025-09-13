-- ================================================================
-- MTM WIP Application - Grant ALL PRIVILEGES to Existing Users (SAFE VERSION)
-- For MySQL 5.7 and phpMyAdmin compatibility - handles system table issues
-- ================================================================
-- 
-- This script grants ALL PRIVILEGES to users that already exist.
-- Safe version that ignores system table errors.
-- Run this after users have been created but need privilege updates.
--
-- INSTRUCTIONS:
-- 1. Execute all GRANT statements for production database
-- 2. Execute all GRANT statements for test database  
-- 3. Run FLUSH PRIVILEGES at the end
-- ================================================================

-- Disable foreign key checks and SQL mode for compatibility
SET FOREIGN_KEY_CHECKS = 0;
SET SQL_MODE = '';

-- Grant ALL PRIVILEGES on mtm_wip_application database
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

-- Grant ALL PRIVILEGES on mtm_wip_application_test database
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

-- Apply all privilege changes
FLUSH PRIVILEGES;

-- Re-enable foreign key checks
SET FOREIGN_KEY_CHECKS = 1;

-- ================================================================
-- Success Message
-- ================================================================
-- If no errors occurred above, all 56 users now have ALL PRIVILEGES
-- on both mtm_wip_application and mtm_wip_application_test databases.
--
-- To verify: Try connecting as one of the users and run:
-- SHOW DATABASES;
-- USE mtm_wip_application;
-- SHOW TABLES;
-- ================================================================
