1) InventoryTabView's Save button needs to activate when PartID, Operation, Location and Quantity all have valid values
2) InventoryTabView's Advanced Button does not open AdvancedInventoryView, this needs to be fixed.
3) Quick Buttons are not loading the user's last 10 transactions as they should, verify that the View is fully implemented and using proper MTM guidlines
4) History Panel (inside of QuickButtons) needs to change from a data-grid to a format similar to Quickbuttons
    4a) Each button should be Expand/Collapsable.
        4a1) When Collapsed it should show the following:
            4a1.1)Icon stating the type of action (IN, OUT, Transfer)
            4a1.2)PartID and Operation
            4a1.3)Colored to show what action it is (IN = Green, OUT = Red, Transfer = Yellow) - These colors need to also be added to each theme as they should be customizable
        4b1) When Expanded it should show the same as 4a1, as well as any other relevant information left out for that action.
        4b2) Use the collapsable panel to acheave collapsing / expanding.  modify it though so it has 4 rounded corners with a rounded header (Make a new Control based off of Colapsable Panel)
5) When Saving in InventoryTabView make sure it uses the correct Stored Procedure, validate it's values against the Server File in the Development Folder.
    5a) When the Save is SUCCESSFUL
        5a1) If Quickbuttons does not contain a button that has BOTH the PartID and Operation, add it otherwise skip adding it for this transaction.
        5a2) Verify that when it saved to TransactionHistory that It does so ONLY after a SUCCESSFUL save.
        5a3) Verify that the TransactionHistory progress follows MTMGuidelines - Validate Caller vs Stored Procedure vs Server
        5a4) Clean PartID, Operation, Location and Quantity Fields
6) InventoryTabView: Change the Quantity Textbox to Validate the same way as the other 3, not against the server, but it needs to start blank (not prefilled with 1)
    6b) If Qty is empty - Error colored textbox with error message (as PartID, Operation and Location Do)
    6c) If Qty = Letter - same as 6b (acheive this by clearing the textbox, this should case an empty box and hence case the error watermark and color to trigger)
    6d) if Qty is Negative - same as 6b
7) Look at Each Theme, check for proper Text to Background Contrast in all Themes no Dark on Dark, Light on Light
    7b) Add Properties for Icon color, Icon Background (if it does not exist)
    7c) Add Properties for any missing Text vs Backgrounds you can come up with by looking through all views. (Example: Texbox Text vs Texbox Background)
    7d) After fixing updating the Themeing System, update InventoryTabView to use proper Theme Properties.
