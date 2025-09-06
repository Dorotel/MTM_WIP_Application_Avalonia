-- Quick test to verify the stored procedure update worked
CALL mtm_wip_application_test.inv_inventory_Add_Item(
    'TEST001',          -- p_PartID
    'TEST_LOC',         -- p_Location  
    '90',               -- p_Operation
    5,                  -- p_Quantity
    'Test',             -- p_ItemType
    'TEST_USER',        -- p_User
    'Test add item',    -- p_Notes
    @status,            -- p_Status (OUT)
    @message            -- p_ErrorMsg (OUT)
);

SELECT @status as Status, @message as Message;
