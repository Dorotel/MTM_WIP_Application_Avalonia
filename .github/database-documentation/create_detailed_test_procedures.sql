-- MTM WIP Application - Enhanced Detailed Test Procedures
-- Creates comprehensive testing procedures with detailed results and troubleshooting
-- Generated: September 22, 2025

USE mtm_wip_application_test;

-- Drop existing test procedures if they exist
DROP PROCEDURE IF EXISTS test_quickbutton_procedures_detailed;
DROP PROCEDURE IF EXISTS test_master_data_procedures_detailed;
DROP PROCEDURE IF EXISTS test_user_management_procedures_detailed;
DROP PROCEDURE IF EXISTS test_inventory_procedures_detailed;
DROP PROCEDURE IF EXISTS test_system_procedures_detailed;
DROP PROCEDURE IF EXISTS test_all_procedures_detailed;

DELIMITER $$

-- =============================================================================
-- Enhanced QuickButton Procedures Test with Detailed Results
-- =============================================================================
CREATE PROCEDURE test_quickbutton_procedures_detailed(
    OUT p_Status INT,
    OUT p_ErrorMsg TEXT
)
BEGIN
    DECLARE v_test_user VARCHAR
    (50) DEFAULT 'testuser';
    DECLARE v_qb_status INT DEFAULT 0;
DECLARE v_qb_error VARCHAR
(255) DEFAULT '';
DECLARE v_save_status INT DEFAULT 0;
DECLARE v_save_error VARCHAR
(255) DEFAULT '';
DECLARE v_remove_status INT DEFAULT 0;
DECLARE v_remove_error VARCHAR
(255) DEFAULT '';
DECLARE v_clear_status INT DEFAULT 0;
DECLARE v_clear_error VARCHAR
(255) DEFAULT '';
DECLARE v_test_count INT DEFAULT 0;
DECLARE v_pass_count INT DEFAULT 0;
DECLARE v_details TEXT DEFAULT '';

DECLARE
CONTINUE
HANDLER FOR SQLEXCEPTION
BEGIN
GET DIAGNOSTICS CONDITION 1
            @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT;
SET p_Status
= 0;
SET p_ErrorMsg
= CONCAT
('SQLEXCEPTION during QuickButton testing: ', @errno, ' - ', @text);
END;

-- Test 1: qb_quickbuttons_Get_ByUser
SET v_test_count
= v_test_count + 1;
    CALL qb_quickbuttons_Get_ByUser
(v_test_user, v_qb_status, v_qb_error);
SET v_details
= CONCAT
(v_details, 'Test 1 - qb_quickbuttons_Get_ByUser: Status=', v_qb_status, ', Error="', v_qb_error, '"');
IF v_qb_status IN (0, 1) THEN
SET v_pass_count
= v_pass_count + 1;
SET v_details
= CONCAT
(v_details, ' [PASS]');
    ELSE
SET v_details
= CONCAT
(v_details, ' [FAIL - Expected status 0 or 1]');
END
IF;
    SET v_details
= CONCAT
(v_details, '; ');

-- Test 2: qb_quickbuttons_Save
SET v_test_count
= v_test_count + 1;
    CALL qb_quickbuttons_Save
(v_test_user, 1, 'PART001', '90', 10, 'FLOOR', 'Standard', v_save_status, v_save_error);
SET v_details
= CONCAT
(v_details, 'Test 2 - qb_quickbuttons_Save: Status=', v_save_status, ', Error="', v_save_error, '"');
IF v_save_status = 1 THEN
SET v_pass_count
= v_pass_count + 1;
SET v_details
= CONCAT
(v_details, ' [PASS]');
    ELSE
SET v_details
= CONCAT
(v_details, ' [FAIL - Expected status 1]');
END
IF;
    SET v_details
= CONCAT
(v_details, '; ');

-- Test 3: qb_quickbuttons_Remove
SET v_test_count
= v_test_count + 1;
    CALL qb_quickbuttons_Remove
(v_test_user, 1, v_remove_status, v_remove_error);
SET v_details
= CONCAT
(v_details, 'Test 3 - qb_quickbuttons_Remove: Status=', v_remove_status, ', Error="', v_remove_error, '"');
IF v_remove_status IN (0, 1) THEN
SET v_pass_count
= v_pass_count + 1;
SET v_details
= CONCAT
(v_details, ' [PASS]');
    ELSE
SET v_details
= CONCAT
(v_details, ' [FAIL - Expected status 0 or 1]');
END
IF;
    SET v_details
= CONCAT
(v_details, '; ');

-- Test 4: qb_quickbuttons_Clear_ByUser
SET v_test_count
= v_test_count + 1;
    CALL qb_quickbuttons_Clear_ByUser
(v_test_user, v_clear_status, v_clear_error);
SET v_details
= CONCAT
(v_details, 'Test 4 - qb_quickbuttons_Clear_ByUser: Status=', v_clear_status, ', Error="', v_clear_error, '"');
IF v_clear_status IN (0, 1) THEN
SET v_pass_count
= v_pass_count + 1;
SET v_details
= CONCAT
(v_details, ' [PASS]');
    ELSE
SET v_details
= CONCAT
(v_details, ' [FAIL - Expected status 0 or 1]');
END
IF;

    -- Set final status with detailed results
    IF v_pass_count = v_test_count THEN
SET p_Status
= 1;
SET p_ErrorMsg
= CONCAT
('QuickButton tests ALL PASSED (', v_pass_count, '/', v_test_count, '). Details: ', v_details);
    ELSE
SET p_Status
= 0;
SET p_ErrorMsg
= CONCAT
('QuickButton tests FAILED (', v_pass_count, '/', v_test_count, ' passed). Details: ', v_details);
END
IF;
END$$

-- =============================================================================
-- Enhanced Inventory Procedures Test with Detailed Results
-- =============================================================================
CREATE PROCEDURE test_inventory_procedures_detailed(
    OUT p_Status INT,
    OUT p_ErrorMsg TEXT
)
BEGIN
    DECLARE v_test_count INT DEFAULT 0;
    DECLARE v_pass_count INT DEFAULT 0;
DECLARE v_result_count INT DEFAULT 0;
DECLARE v_details TEXT DEFAULT '';
DECLARE v_procedure_list TEXT DEFAULT '';

DECLARE
CONTINUE
HANDLER FOR SQLEXCEPTION
BEGIN
GET DIAGNOSTICS CONDITION 1
            @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT;
SET p_Status
= 0;
SET p_ErrorMsg
= CONCAT
('SQLEXCEPTION during Inventory testing: ', @errno, ' - ', @text);
END;

-- Get list of inventory procedures for debugging
SELECT GROUP_CONCAT(ROUTINE_NAME SEPARATOR ', '
) INTO v_procedure_list
    FROM INFORMATION_SCHEMA.ROUTINES
    WHERE ROUTINE_SCHEMA = 'mtm_wip_application_test'
    AND ROUTINE_NAME LIKE 'inv_%';

SET v_details
= CONCAT
('Found inventory procedures: [', IFNULL
(v_procedure_list, 'NONE'), ']. ');

-- Test 1: Check if inv_inventory_Get_All exists
SET v_test_count
= v_test_count + 1;
SELECT COUNT(*)
INTO v_result_count
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = 'mtm_wip_application_test'
    AND ROUTINE_NAME = 'inv_inventory_Get_All';

SET v_details
= CONCAT
(v_details, 'Test 1 - inv_inventory_Get_All exists: Count=', v_result_count);
IF v_result_count = 1 THEN
SET v_pass_count
= v_pass_count + 1;
SET v_details
= CONCAT
(v_details, ' [PASS]');
    ELSE
SET v_details
= CONCAT
(v_details, ' [FAIL - Procedure not found]');
END
IF;
    SET v_details
= CONCAT
(v_details, '; ');

-- Test 2: Check if inv_inventory_Get_ByPartID exists
SET v_test_count
= v_test_count + 1;
SELECT COUNT(*)
INTO v_result_count
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = 'mtm_wip_application_test'
    AND ROUTINE_NAME = 'inv_inventory_Get_ByPartID';

SET v_details
= CONCAT
(v_details, 'Test 2 - inv_inventory_Get_ByPartID exists: Count=', v_result_count);
IF v_result_count = 1 THEN
SET v_pass_count
= v_pass_count + 1;
SET v_details
= CONCAT
(v_details, ' [PASS]');
    ELSE
SET v_details
= CONCAT
(v_details, ' [FAIL - Procedure not found]');
END
IF;
    SET v_details
= CONCAT
(v_details, '; ');

-- Test 3: Check if inv_inventory_Get_ByPartIDandOperation exists
SET v_test_count
= v_test_count + 1;
SELECT COUNT(*)
INTO v_result_count
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = 'mtm_wip_application_test'
    AND ROUTINE_NAME = 'inv_inventory_Get_ByPartIDandOperation';

SET v_details
= CONCAT
(v_details, 'Test 3 - inv_inventory_Get_ByPartIDandOperation exists: Count=', v_result_count);
IF v_result_count = 1 THEN
SET v_pass_count
= v_pass_count + 1;
SET v_details
= CONCAT
(v_details, ' [PASS]');
    ELSE
SET v_details
= CONCAT
(v_details, ' [FAIL - Procedure not found]');
END
IF;
    SET v_details
= CONCAT
(v_details, '; ');

-- Test 4: Check total inventory procedure count
SET v_test_count
= v_test_count + 1;
SELECT COUNT(*)
INTO v_result_count
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = 'mtm_wip_application_test'
    AND ROUTINE_NAME LIKE 'inv_%';

SET v_details
= CONCAT
(v_details, 'Test 4 - Total inventory procedures: Count=', v_result_count, ' (Expected >= 8)');
IF v_result_count >= 8 THEN
SET v_pass_count
= v_pass_count + 1;
SET v_details
= CONCAT
(v_details, ' [PASS]');
    ELSE
SET v_details
= CONCAT
(v_details, ' [FAIL - Expected at least 8 procedures]');
END
IF;

    -- Set final status with detailed results
    IF v_pass_count = v_test_count THEN
SET p_Status
= 1;
SET p_ErrorMsg
= CONCAT
('Inventory tests ALL PASSED (', v_pass_count, '/', v_test_count, '). Details: ', v_details);
    ELSE
SET p_Status
= 0;
SET p_ErrorMsg
= CONCAT
('Inventory tests FAILED (', v_pass_count, '/', v_test_count, ' passed). Details: ', v_details);
END
IF;
END$$

-- =============================================================================
-- Enhanced Master Test Procedure - Tests All Categories with Full Details
-- =============================================================================
CREATE PROCEDURE test_all_procedures_detailed(
    OUT p_Status INT,
    OUT p_ErrorMsg TEXT
)
BEGIN
    DECLARE v_qb_status INT DEFAULT 0;
    DECLARE v_qb_error TEXT DEFAULT '';
DECLARE v_inv_status INT DEFAULT 0;
DECLARE v_inv_error TEXT DEFAULT '';
DECLARE v_total_tests INT DEFAULT 2;
DECLARE v_passed_tests INT DEFAULT 0;
DECLARE v_detailed_results TEXT DEFAULT '';

DECLARE
CONTINUE
HANDLER FOR SQLEXCEPTION
BEGIN
GET DIAGNOSTICS CONDITION 1
            @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT;
SET p_Status
= 0;
SET p_ErrorMsg
= CONCAT
('SQLEXCEPTION during comprehensive testing: ', @errno, ' - ', @text);
END;

    -- Test QuickButton procedures with details
    CALL test_quickbutton_procedures_detailed
(v_qb_status, v_qb_error);
SET v_detailed_results
= CONCAT
(v_detailed_results, 'QUICKBUTTONS: ', v_qb_error, ' || ');
IF v_qb_status = 1 THEN
SET v_passed_tests
= v_passed_tests + 1;
END
IF;

    -- Test Inventory procedures with details
    CALL test_inventory_procedures_detailed
(v_inv_status, v_inv_error);
SET v_detailed_results
= CONCAT
(v_detailed_results, 'INVENTORY: ', v_inv_error);
IF v_inv_status = 1 THEN
SET v_passed_tests
= v_passed_tests + 1;
END
IF;

    -- Set final comprehensive status with all details
    IF v_passed_tests = v_total_tests THEN
SET p_Status
= 1;
SET p_ErrorMsg
= CONCAT
('ALL COMPREHENSIVE TESTS PASSED (', v_passed_tests, '/', v_total_tests, ' categories). Full Details: ', v_detailed_results);
    ELSE
SET p_Status
= 0;
SET p_ErrorMsg
= CONCAT
('COMPREHENSIVE TESTS FAILED (', v_passed_tests, '/', v_total_tests, ' categories passed). Full Details: ', v_detailed_results);
END
IF;
END$$

DELIMITER ;

-- Show created detailed procedures
SELECT 'DETAILED TEST PROCEDURES CREATED' as Result,
    ROUTINE_NAME as ProcedureName,
    CREATED as DateCreated
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = 'mtm_wip_application_test'
    AND ROUTINE_NAME LIKE 'test_%_detailed'
ORDER BY ROUTINE_NAME;
