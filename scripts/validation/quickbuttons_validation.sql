-- QuickButtons Stored Procedure Validation Script
-- Purpose: Manual validation of all QuickButtons related stored procedures
-- Environment: Run in phpMyAdmin or MySQL client connected to mtm_wip_application database
-- IMPORTANT: Ensure you are on a test database or have backups; these operations modify data.

-- =============================================
-- 1. Preparation
-- =============================================
SET @test_user = 'QA_TEST_USER1';
SET @part_a = 'QBTEST-PART-A';
SET @part_b = 'QBTEST-PART-B';
SET @op_1 = '90';
SET @op_2 = '100';
SET @qty_small = 5;
SET @qty_large = 42;

-- Clean slate for test user
CALL qb_quickbuttons_Clear_ByUser(@test_user, @p_Status, @p_ErrorMsg);
SELECT @p_Status AS ClearStatus, @p_ErrorMsg AS ClearMessage;

-- =============================================
-- 2. Validate qb_quickbuttons_Get_ByUser (empty state)
-- Expectation: Status >= 0, zero rows.
CALL qb_quickbuttons_Get_ByUser(@test_user, @p_Status, @p_ErrorMsg);
SELECT @p_Status AS GetEmptyStatus, @p_ErrorMsg AS GetEmptyMessage;

-- =============================================
-- 3. Save first button (Position 1)
-- Required params: p_User, p_Position, p_PartID, p_Operation, p_Quantity, p_Location, p_ItemType
CALL qb_quickbuttons_Save(@test_user, 1, @part_a, @op_1, @qty_small, 'LOC-A', 'WIP', @p_Status, @p_ErrorMsg);
SELECT @p_Status AS Save1Status, @p_ErrorMsg AS Save1Message;
CALL qb_quickbuttons_Get_ByUser(@test_user, @p_Status, @p_ErrorMsg);
SELECT @p_Status AS GetAfterSave1Status, @p_ErrorMsg AS GetAfterSave1Message;
SELECT * FROM temp_qb_quickbuttons_result; -- if procedure stages output in temp (adjust if needed)

-- =============================================
-- 4. Save second button (Position 2)
CALL qb_quickbuttons_Save(@test_user, 2, @part_b, @op_2, @qty_large, 'LOC-B', 'WIP', @p_Status, @p_ErrorMsg);
SELECT @p_Status AS Save2Status, @p_ErrorMsg AS Save2Message;
CALL qb_quickbuttons_Get_ByUser(@test_user, @p_Status, @p_ErrorMsg);
SELECT @p_Status AS GetAfterSave2Status, @p_ErrorMsg AS GetAfterSave2Message;

-- =============================================
-- 5. Attempt duplicate part/operation at new position
-- Expectation: Should either replace or return status indicating conflict (document actual behavior)
CALL qb_quickbuttons_Save(@test_user, 3, @part_a, @op_1, 10, 'LOC-C', 'WIP', @p_Status, @p_ErrorMsg);
SELECT @p_Status AS SaveDuplicateStatus, @p_ErrorMsg AS SaveDuplicateMessage;
CALL qb_quickbuttons_Get_ByUser(@test_user, @p_Status, @p_ErrorMsg);
SELECT @p_Status AS GetAfterDuplicateStatus, @p_ErrorMsg AS GetAfterDuplicateMessage;

-- =============================================
-- 6. Remove button at position 2
CALL qb_quickbuttons_Remove(@test_user, 2, @p_Status, @p_ErrorMsg);
SELECT @p_Status AS RemovePos2Status, @p_ErrorMsg AS RemovePos2Message;
CALL qb_quickbuttons_Get_ByUser(@test_user, @p_Status, @p_ErrorMsg);
SELECT @p_Status AS GetAfterRemoveStatus, @p_ErrorMsg AS GetAfterRemoveMessage;

-- =============================================
-- 7. Clear all quick buttons
CALL qb_quickbuttons_Clear_ByUser(@test_user, @p_Status, @p_ErrorMsg);
SELECT @p_Status AS ClearAllStatus, @p_ErrorMsg AS ClearAllMessage;
CALL qb_quickbuttons_Get_ByUser(@test_user, @p_Status, @p_ErrorMsg);
SELECT @p_Status AS GetAfterClearStatus, @p_ErrorMsg AS GetAfterClearMessage;

-- =============================================
-- 8. Edge Cases
-- 8.1 Invalid position (0)
CALL qb_quickbuttons_Save(@test_user, 0, @part_a, @op_1, @qty_small, 'LOC-X', 'WIP', @p_Status, @p_ErrorMsg);
SELECT @p_Status AS SaveInvalidPosStatus, @p_ErrorMsg AS SaveInvalidPosMessage;

-- 8.2 Position above 10
CALL qb_quickbuttons_Save(@test_user, 15, @part_a, @op_1, @qty_small, 'LOC-X', 'WIP', @p_Status, @p_ErrorMsg);
SELECT @p_Status AS SaveOutOfRangePosStatus, @p_ErrorMsg AS SaveOutOfRangePosMessage;

-- 8.3 Null / empty part
CALL qb_quickbuttons_Save(@test_user, 1, '', @op_1, @qty_small, 'LOC-X', 'WIP', @p_Status, @p_ErrorMsg);
SELECT @p_Status AS SaveEmptyPartStatus, @p_ErrorMsg AS SaveEmptyPartMessage;

-- 8.4 Negative quantity
CALL qb_quickbuttons_Save(@test_user, 1, @part_a, @op_1, -5, 'LOC-X', 'WIP', @p_Status, @p_ErrorMsg);
SELECT @p_Status AS SaveNegativeQtyStatus, @p_ErrorMsg AS SaveNegativeQtyMessage;

-- 8.5 Remove non-existent position
CALL qb_quickbuttons_Remove(@test_user, 9, @p_Status, @p_ErrorMsg);
SELECT @p_Status AS RemoveNonExistentStatus, @p_ErrorMsg AS RemoveNonExistentMessage;

-- 8.6 Clear again when already empty
CALL qb_quickbuttons_Clear_ByUser(@test_user, @p_Status, @p_ErrorMsg);
SELECT @p_Status AS ClearWhenEmptyStatus, @p_ErrorMsg AS ClearWhenEmptyMessage;

-- =============================================
-- 9. Transaction linkage test
-- Add a transaction and verify it creates a button via app logic path.
-- NOTE: Direct procedure call: sys_last_10_transactions_Add_Transaction
CALL sys_last_10_transactions_Add_Transaction(
    'IN',
    DATE_FORMAT(NOW(), '%Y%m%d%H%i%s'),
    @part_a,
    '',
    '',
    @op_1,
    7,
    '',
    @test_user,
    'Standard',
    NOW(),
    @p_Status,
    @p_ErrorMsg
);
SELECT @p_Status AS AddTxnStatus, @p_ErrorMsg AS AddTxnMessage;
-- Then check last 10
CALL sys_last_10_transactions_Get_ByUser(@test_user, 10);
-- Re-fetch quick buttons (application would add newly at position 1 through service logic; manual replication requires calling qb_quickbuttons_Save accordingly.)

-- =============================================
-- 10. Cleanup (optional)
CALL qb_quickbuttons_Clear_ByUser(@test_user, @p_Status, @p_ErrorMsg);
SELECT @p_Status AS FinalClearStatus, @p_ErrorMsg AS FinalClearMessage;

-- END OF VALIDATION SCRIPT
