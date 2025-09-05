1) InventoryViewTab - using either incorrect or hardcoded colors.  all colors must be taken from the current theme.
2) SuggestionOverlayView - When entering this view, focus should be set on the list box's 1st item.  also the list box should span the entire control, from right after the "No exact match found...." text to the buttons on the buttom.
3) Get the following error when clicking a quick button, plus fixed needed.
    3a) The Header needs to be reorganized as its too cramped. The Lightning Bolt icon, "Quick Actions" text, # of Actions text, and the 2 Icon Toggle Buttons are crunched. together.  find a way to remedy this so it keeps its professional look.
    3b) The "actions" text in the # of Actions text is too dull almost unreadable.
    3c) Verify that when clicking a quick button that it passes the Part ID, Quantity, Operation and Location (see 3d) to the current View on the left side of MainView
    3d) add a checkbox at the bottom of the Quickview Panel (inside colored footer) that when checked will pass the Location to the Left Panel's View and if unchecked will not pass the location.
    3e) The Toggle Buttons on top of QuickButtons's Header have clashing contrast (Dark on Dark, in light Themes) and (Light on Light, in Dark Themes).  This needs to be fixed.
    3f) Error Message mentioned above: 2025-09-05 11:34:16.425 info: MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel[0]
      Executing quick action: 39137-0780, 109, 100
MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel: Information: Executing quick action: 39137-0780, 109, 100
2025-09-05 11:34:16.428 dbug: MTM_WIP_Application_Avalonia.ViewModels.MainViewViewModel[0]
      OnQuickActionExecuted event handler triggered - Sender: QuickButtonsViewModel, PartId: 39137-0780, Operation: 109, Quantity: 100
MTM_WIP_Application_Avalonia.ViewModels.MainViewViewModel: Debug: OnQuickActionExecuted event handler triggered - Sender: QuickButtonsViewModel, PartId: 39137-0780, Operation: 109, Quantity: 100
2025-09-05 11:34:16.430 info: MTM_WIP_Application_Avalonia.ViewModels.MainViewViewModel[0]
      Processing quick action for tab 0: 109 - 39137-0780 (100 units)
MTM_WIP_Application_Avalonia.ViewModels.MainViewViewModel: Information: Processing quick action for tab 0: 109 - 39137-0780 (100 units)
2025-09-05 11:34:16.432 dbug: MTM_WIP_Application_Avalonia.ViewModels.MainViewViewModel[0]
      Populating Inventory tab with quick action data
MTM_WIP_Application_Avalonia.ViewModels.MainViewViewModel: Debug: Populating Inventory tab with quick action data
2025-09-05 11:34:16.434 info: MTM_WIP_Application_Avalonia.ViewModels.MainViewViewModel[0]
      Quick action processed successfully and status updated
MTM_WIP_Application_Avalonia.ViewModels.MainViewViewModel: Information: Quick action processed successfully and status updated
2025-09-05 11:34:16.436 dbug: MTM_WIP_Application_Avalonia.Services.QuickButtonsService[0]
      Saving quick button: Position 1, Part 39137-0780
MTM_WIP_Application_Avalonia.Services.QuickButtonsService: Debug: Saving quick button: Position 1, Part 39137-0780
2025-09-05 11:34:16.438 dbug: Helper_Database_StoredProcedure[0]
      Executing stored procedure (no data): qb_quickbuttons_Save
Helper_Database_StoredProcedure: Debug: Executing stored procedure (no data): qb_quickbuttons_Save
2025-09-05 11:34:16.440 info: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: Starting non-query execution of qb_quickbuttons_Save
Helper_Database_StoredProcedure: Information: 🔍 QUICKBUTTON DEBUG: Starting non-query execution of qb_quickbuttons_Save
2025-09-05 11:34:16.442 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: qb_quickbuttons_Save input parameters:
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG: qb_quickbuttons_Save input parameters:
2025-09-05 11:34:16.447 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG:   @p_User = 'JKOLL' (String)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG:   @p_User = 'JKOLL' (String)
2025-09-05 11:34:16.449 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG:   @p_Position = '1' (Int32)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG:   @p_Position = '1' (Int32)
2025-09-05 11:34:16.451 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG:   @p_PartID = '39137-0780' (String)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG:   @p_PartID = '39137-0780' (String)
2025-09-05 11:34:16.453 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG:   @p_Location = '' (String)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG:   @p_Location = '' (String)
2025-09-05 11:34:16.454 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG:   @p_Operation = '109' (String)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG:   @p_Operation = '109' (String)
2025-09-05 11:34:16.456 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG:   @p_Quantity = '100' (Int32)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG:   @p_Quantity = '100' (Int32)
2025-09-05 11:34:16.460 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG:   @p_ItemType = 'Standard' (String)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG:   @p_ItemType = 'Standard' (String)
2025-09-05 11:34:16.463 info: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: SAVE CONTEXT - User '' saving Part '39137-0780' with Operation '109' (Qty: 100) at position 1
Helper_Database_StoredProcedure: Information: 🔍 QUICKBUTTON DEBUG: SAVE CONTEXT - User '' saving Part '39137-0780' with Operation '109' (Qty: 100) at position 1
2025-09-05 11:34:16.488 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: Added parameter @p_User = JKOLL (Type: String)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG: Added parameter @p_User = JKOLL (Type: String)
2025-09-05 11:34:16.491 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: Added parameter @p_Position = 1 (Type: Int32)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG: Added parameter @p_Position = 1 (Type: Int32)
2025-09-05 11:34:16.494 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: Added parameter @p_PartID = 39137-0780 (Type: String)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG: Added parameter @p_PartID = 39137-0780 (Type: String)
2025-09-05 11:34:16.495 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: Added parameter @p_Location =  (Type: String)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG: Added parameter @p_Location =  (Type: String)
2025-09-05 11:34:16.496 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: Added parameter @p_Operation = 109 (Type: String)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG: Added parameter @p_Operation = 109 (Type: String)
2025-09-05 11:34:16.497 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: Added parameter @p_Quantity = 100 (Type: Int32)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG: Added parameter @p_Quantity = 100 (Type: Int32)
2025-09-05 11:34:16.498 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: Added parameter @p_ItemType = Standard (Type: String)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG: Added parameter @p_ItemType = Standard (Type: String)
2025-09-05 11:34:16.500 info: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: Executing non-query with 7 input parameters
Helper_Database_StoredProcedure: Information: 🔍 QUICKBUTTON DEBUG: Executing non-query with 7 input parameters
Exception thrown: 'System.ArgumentException' in System.Private.CoreLib.dll
2025-09-05 11:34:16.746 fail: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: FAILED non-query execution of qb_quickbuttons_Save after 307ms - Error: Parameter 'p_UserID' not found in the collection.
Helper_Database_StoredProcedure: Error: 🔍 QUICKBUTTON DEBUG: FAILED non-query execution of qb_quickbuttons_Save after 307ms - Error: Parameter 'p_UserID' not found in the collection.
2025-09-05 11:34:16.748 fail: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: qb_quickbuttons_Save ERROR DETAILS:
Helper_Database_StoredProcedure: Error: 🔍 QUICKBUTTON DEBUG: qb_quickbuttons_Save ERROR DETAILS:
2025-09-05 11:34:16.751 fail: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG:   Error Type: ArgumentException
Helper_Database_StoredProcedure: Error: 🔍 QUICKBUTTON DEBUG:   Error Type: ArgumentException
2025-09-05 11:34:16.752 fail: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG:   Error Message: Parameter 'p_UserID' not found in the collection.
Helper_Database_StoredProcedure: Error: 🔍 QUICKBUTTON DEBUG:   Error Message: Parameter 'p_UserID' not found in the collection.
2025-09-05 11:34:16.755 fail: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG:   Critical Parameter p_PartID: 39137-0780
Helper_Database_StoredProcedure: Error: 🔍 QUICKBUTTON DEBUG:   Critical Parameter p_PartID: 39137-0780
2025-09-05 11:34:16.766 fail: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG:   Critical Parameter p_Position: 1
Helper_Database_StoredProcedure: Error: 🔍 QUICKBUTTON DEBUG:   Critical Parameter p_Position: 1
2025-09-05 11:34:16.770 fail: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG:   Critical Parameter p_Operation: 109
Helper_Database_StoredProcedure: Error: 🔍 QUICKBUTTON DEBUG:   Critical Parameter p_Operation: 109
2025-09-05 11:34:16.771 fail: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG:   Critical Parameter p_Quantity: 100
Helper_Database_StoredProcedure: Error: 🔍 QUICKBUTTON DEBUG:   Critical Parameter p_Quantity: 100
2025-09-05 11:34:16.773 fail: Helper_Database_StoredProcedure[0]
      Failed to execute stored procedure: qb_quickbuttons_Save
      System.ArgumentException: Parameter 'p_UserID' not found in the collection.
         at MySql.Data.MySqlClient.MySqlParameterCollection.GetParameterFlexible(String parameterName, Boolean throwOnNotFound)
         at MySql.Data.MySqlClient.StoredProcedure.GetAndFixParameter(String spName, MySqlSchemaRow param, Boolean realAsFloat, MySqlParameter returnParameter)
         at MySql.Data.MySqlClient.StoredProcedure.CheckParametersAsync(String spName, Boolean execAsync)
         at MySql.Data.MySqlClient.StoredProcedure.Resolve(Boolean preparing)
         at MySql.Data.MySqlClient.MySqlCommand.ExecuteReaderAsync(CommandBehavior behavior, Boolean execAsync, CancellationToken cancellationToken)
         at MySql.Data.MySqlClient.MySqlCommand.ExecuteNonQueryAsync(Boolean execAsync, CancellationToken cancellationToken)
         at MTM_WIP_Application_Avalonia.Services.Helper_Database_StoredProcedure.ExecuteWithStatus(String connectionString, String procedureName, Dictionary`2 parameters) in C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Services\Database.cs:line 1340
Helper_Database_StoredProcedure: Error: Failed to execute stored procedure: qb_quickbuttons_Save

System.ArgumentException: Parameter 'p_UserID' not found in the collection.
   at MySql.Data.MySqlClient.MySqlParameterCollection.GetParameterFlexible(String parameterName, Boolean throwOnNotFound)
   at MySql.Data.MySqlClient.StoredProcedure.GetAndFixParameter(String spName, MySqlSchemaRow param, Boolean realAsFloat, MySqlParameter returnParameter)
   at MySql.Data.MySqlClient.StoredProcedure.CheckParametersAsync(String spName, Boolean execAsync)
   at MySql.Data.MySqlClient.StoredProcedure.Resolve(Boolean preparing)
   at MySql.Data.MySqlClient.MySqlCommand.ExecuteReaderAsync(CommandBehavior behavior, Boolean execAsync, CancellationToken cancellationToken)
   at MySql.Data.MySqlClient.MySqlCommand.ExecuteNonQueryAsync(Boolean execAsync, CancellationToken cancellationToken)
   at MTM_WIP_Application_Avalonia.Services.Helper_Database_StoredProcedure.ExecuteWithStatus(String connectionString, String procedureName, Dictionary`2 parameters) in C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Services\Database.cs:line 1340
2025-09-05 11:34:16.775 fail: MTM_WIP_Application_Avalonia.Services.QuickButtonsService[0]
      Failed to save quick button: 39137-0780, Error: Error executing stored procedure: Parameter 'p_UserID' not found in the collection.
MTM_WIP_Application_Avalonia.Services.QuickButtonsService: Error: Failed to save quick button: 39137-0780, Error: Error executing stored procedure: Parameter 'p_UserID' not found in the collection.

4) Error when clicking save button in InventoryViewTab
    4a) 2025-09-05 11:42:16.836 dbug: MTM_WIP_Application_Avalonia.Services.ApplicationStateService[0]
      Application state changed: ProgressValue = 10 (was 0)
MTM_WIP_Application_Avalonia.Services.ApplicationStateService: Debug: Application state changed: ProgressValue = 10 (was 0)
2025-09-05 11:42:16.839 dbug: MTM_WIP_Application_Avalonia.Services.ApplicationStateService[0]
      Application state changed: StatusText = Validating inventory data... (was Error: Unknown error)
MTM_WIP_Application_Avalonia.Services.ApplicationStateService: Debug: Application state changed: StatusText = Validating inventory data... (was Error: Unknown error)
2025-09-05 11:42:16.841 dbug: MTM_WIP_Application_Avalonia.Services.ApplicationStateService[0]
      Progress updated: 10% - Validating inventory data...
MTM_WIP_Application_Avalonia.Services.ApplicationStateService: Debug: Progress updated: 10% - Validating inventory data...
2025-09-05 11:42:16.843 dbug: MTM_WIP_Application_Avalonia.Services.ApplicationStateService[0]
      Application state changed: ProgressValue = 25 (was 10)
MTM_WIP_Application_Avalonia.Services.ApplicationStateService: Debug: Application state changed: ProgressValue = 25 (was 10)
2025-09-05 11:42:16.845 dbug: MTM_WIP_Application_Avalonia.Services.ApplicationStateService[0]
      Application state changed: StatusText = Connecting to database... (was Validating inventory data...)
MTM_WIP_Application_Avalonia.Services.ApplicationStateService: Debug: Application state changed: StatusText = Connecting to database... (was Validating inventory data...)
2025-09-05 11:42:16.847 dbug: MTM_WIP_Application_Avalonia.Services.ApplicationStateService[0]
      Progress updated: 25% - Connecting to database...
MTM_WIP_Application_Avalonia.Services.ApplicationStateService: Debug: Progress updated: 25% - Connecting to database...
2025-09-05 11:42:16.849 dbug: MTM_WIP_Application_Avalonia.Services.ApplicationStateService[0]
      Application state changed: ProgressValue = 50 (was 25)
MTM_WIP_Application_Avalonia.Services.ApplicationStateService: Debug: Application state changed: ProgressValue = 50 (was 25)
2025-09-05 11:42:16.851 dbug: MTM_WIP_Application_Avalonia.Services.ApplicationStateService[0]
      Application state changed: StatusText = Saving inventory item... (was Connecting to database...)
MTM_WIP_Application_Avalonia.Services.ApplicationStateService: Debug: Application state changed: StatusText = Saving inventory item... (was Connecting to database...)
2025-09-05 11:42:16.853 dbug: MTM_WIP_Application_Avalonia.Services.ApplicationStateService[0]
      Progress updated: 50% - Saving inventory item...
MTM_WIP_Application_Avalonia.Services.ApplicationStateService: Debug: Progress updated: 50% - Saving inventory item...
2025-09-05 11:42:16.855 info: MTM_WIP_Application_Avalonia.Services.ConfigurationService[0]
      Using connection string: Server=localhost, Database=mtm_wip_application, Uid=JKOLL
MTM_WIP_Application_Avalonia.Services.ConfigurationService: Information: Using connection string: Server=localhost, Database=mtm_wip_application, Uid=JKOLL
2025-09-05 11:42:16.864 dbug: Helper_Database_StoredProcedure[0]
      Executing stored procedure: inv_inventory_Add_Item
Helper_Database_StoredProcedure: Debug: Executing stored procedure: inv_inventory_Add_Item
2025-09-05 11:42:17.097 dbug: Helper_Database_StoredProcedure[0]
      Stored procedure executed: inv_inventory_Add_Item, Status: -1, Rows: 0
Helper_Database_StoredProcedure: Debug: Stored procedure executed: inv_inventory_Add_Item, Status: -1, Rows: 0
warn: DesignTime[0]
      Failed to save inventory item: Unknown error
2025-09-05 11:42:17.106 dbug: MTM_WIP_Application_Avalonia.Services.ApplicationStateService[0]
      Application state changed: ProgressValue = 0 (was 50)
MTM_WIP_Application_Avalonia.Services.ApplicationStateService: Debug: Application state changed: ProgressValue = 0 (was 50)
2025-09-05 11:42:17.107 dbug: MTM_WIP_Application_Avalonia.Services.ApplicationStateService[0]
      Application state changed: StatusText = Error: Unknown error (was Saving inventory item...)
MTM_WIP_Application_Avalonia.Services.ApplicationStateService: Debug: Application state changed: StatusText = Error: Unknown error (was Saving inventory item...)
2025-09-05 11:42:17.108 dbug: MTM_WIP_Application_Avalonia.Services.ApplicationStateService[0]
      Progress updated: 0% - Error: Unknown error
MTM_WIP_Application_Avalonia.Services.ApplicationStateService: Debug: Progress updated: 0% - Error: Unknown error

5) InventoryTabView - Text inside buttons needs to be centered onto the icon to the left of it.  The border on the "Advanced" and "Reset" buttons are almost unviewable, too light on Light Themes.  But look decent on dark themes.
6) InventoryTabView - The Advanced button needs to be a little longer as the "Advanced" text touches the border of the button
7) InventoryTabView - Remove the verson frm the right side of the lower panel as this is shown in MainView
8) MTMDarkTheme - Give a little more range to the dark colors so it does not look completely black.   Suggestion: Think how the windows 11 default theme looks, and them put that in dark mode.
9) MainView - Remove the Signal Bar that is to the left of the Connection Status.
    9a) Place the Signal bar instead inside the Connection Status "Oval", under the {Icon} "Connection: " {Status} and centered.  Also implement this feature so it works on startup.
10) MainView - Move the progress bar to the right of the "Connection Oval" (dont resize it)
11) MainView - The Status Box that is to the left of the Version should expand from the Progress bar to the Version.
12) MainView - Trunc the text inside the Status Box if its close to exceeding the width of its bounds
    12a) On mouseover show the entire message.
13) MainView - Change the Text for the version to just "Ver. " + {Version}.
    13a) Implement this to use the current version of the application stored in the appsettings json file (needs to be added, current versin is 5.0)
14) InventoryTabView or MainView - using MTM Light as refernce, i want the backgorund of the base border / grid changed to match the background of the surrounding area.  In MTM Light's case that would be a color close to white, the color i want changed looks very light tan.
    14a) Add a border color to the border that the "MTM Inventory Entry" Header, Textboxes and RichTextbox live inside.  make it match the color of the border below it.
15) MainView: Tab Control - give the Selected Tab a Border color that contrasts well with its current bacground color.
16) MainView: Shrink the size of the top panel(File, View, Help, Theme Picker) to the normal size of a Menu Bar.  You will need to make adjustments to the ThemeQuickSwither to accomplish this.
17) Verify that MainView, InventoryTabView, QuickButtonsView (as well as the historypanel inside it), are using the FULL loaded theme on startup, as when i click Apply on the ThemeQuickSwithcher after the app starts with the same theme slelected the colors change (should not happen if the current theme is fully loaded)
18) MainWindow: Remove the Top Window Bar and implement a custom one that goes with the current themeing system.  Implement an Exit, Minimize, Maximize button on it as well.  As well as the App Title on it.
19) Add this rule for Completion as well: All compilation warnings must be fixed / addressed.

*** Current Startup Log. ***

    ------------------------------------------------------------------------------
You may only use the Microsoft Visual Studio .NET/C/C++ Debugger (vsdbg) with
Visual Studio Code, Visual Studio or Visual Studio for Mac software to help you
develop and test your applications.
------------------------------------------------------------------------------
[2025-09-05 12:07:41.564] MTM WIP Application Program.Main() starting...
[2025-09-05 12:07:41.621] Command line args: []
[PROGRAM] Application main entry point started with 0 arguments
[2025-09-05 12:07:41.623] Configuring services using ApplicationStartup...
[2025-09-05 12:07:41.638] ConfigureServicesAsync() started
[PROGRAM] Starting service configuration using ApplicationStartup
[2025-09-05 12:07:41.640] Application not yet initialized, starting fresh initialization
[2025-09-05 12:07:41.652] ApplicationStartup.InitializeApplication() started
[STARTUP] Application initialization started with 1 arguments
[2025-09-05 12:07:41.679] ConfigureApplication() started
[2025-09-05 12:07:41.679] Environment: Production
[CONFIG] Environment: Production
[2025-09-05 12:07:41.843] ValidateConfiguration() started
[CONFIG-VALIDATE] Configuration validation started
[2025-09-05 12:07:41.846] DefaultConnection configured
[CONFIG-VALIDATE] DefaultConnection found
[2025-09-05 12:07:41.848] Application name: MTM WIP Application
[CONFIG-VALIDATE] Application name: MTM WIP Application
[2025-09-05 12:07:41.850] Configuration validation completed successfully
[CONFIG-VALIDATE] Configuration validation passed
[2025-09-05 12:07:41.856] Configuration setup completed in 176ms
[CONFIG] Configuration built successfully in 176ms
[2025-09-05 12:07:41.860] Configuration phase completed
[2025-09-05 12:07:41.866] ConfigureLogging() started
[2025-09-05 12:07:41.905] Logging configuration completed in 38ms
[LOGGING] Logging configuration completed in 38ms
[2025-09-05 12:07:41.907] Logging configuration completed
[2025-09-05 12:07:41.908] ConfigureCoreServices() started
[CORE-SERVICES] Core services configuration started
[2025-09-05 12:07:41.940] Core services registration completed in 32ms
[CORE-SERVICES] MTM services registered successfully in 32ms
[2025-09-05 12:07:41.943] Core services configuration completed
[2025-09-05 12:07:41.946] ConfigureApplicationServices() started
[APP-SERVICES] Application services configuration started
[2025-09-05 12:07:41.947] Application services registration completed in 1ms
[APP-SERVICES] Application services registered successfully in 1ms
[2025-09-05 12:07:41.951] Application services configuration completed
[2025-09-05 12:07:41.959] BuildAndValidateServices() started
[BUILD-VALIDATE] Service provider build and validation started
[2025-09-05 12:07:42.004] Service provider built successfully
[BUILD-VALIDATE] Service provider built with validation enabled
[2025-09-05 12:07:42.007] Performing runtime service validation...
[VALIDATION-SUCCESS] IConfigurationService resolved successfully
2025-09-05 12:07:42.106 info: MTM_WIP_Application_Avalonia.Services.ApplicationStateService[0]
      ApplicationStateService initialized with default user: JKOLL
MTM_WIP_Application_Avalonia.Services.ApplicationStateService: Information: ApplicationStateService initialized with default user: JKOLL
[VALIDATION-SUCCESS] IApplicationStateService resolved successfully
2025-09-05 12:07:42.128 dbug: MTM_WIP_Application_Avalonia.Services.NavigationService[0]
      NavigationService constructed successfully
MTM_WIP_Application_Avalonia.Services.NavigationService: Debug: NavigationService constructed successfully
[VALIDATION-SUCCESS] INavigationService resolved successfully
2025-09-05 12:07:42.137 info: MTM_WIP_Application_Avalonia.Services.ThemeService[0]
      ThemeService initialized with 19 available themes, default theme: MTM Default
MTM_WIP_Application_Avalonia.Services.ThemeService: Information: ThemeService initialized with 19 available themes, default theme: MTM Default
[VALIDATION-SUCCESS] IThemeService resolved successfully
2025-09-05 12:07:42.460 info: MTM_WIP_Application_Avalonia.Services.SettingsService[0]
      SettingsService initialized
MTM_WIP_Application_Avalonia.Services.SettingsService: Information: SettingsService initialized
2025-09-05 12:07:42.466 dbug: MTM_WIP_Application_Avalonia.Services.ConfigurationService[0]
      Configuration key 'User:PreferredTheme' not found, using default value
MTM_WIP_Application_Avalonia.Services.ConfigurationService: Debug: Configuration key 'User:PreferredTheme' not found, using default value
2025-09-05 12:07:42.467 dbug: MTM_WIP_Application_Avalonia.Services.ConfigurationService[0]
      Configuration key 'Settings:Theme' not found, using default value
MTM_WIP_Application_Avalonia.Services.ConfigurationService: Debug: Configuration key 'Settings:Theme' not found, using default value
2025-09-05 12:07:42.470 dbug: MTM_WIP_Application_Avalonia.Services.ThemeService[0]
      Retrieved user preferred theme: MTMTheme
MTM_WIP_Application_Avalonia.Services.ThemeService: Debug: Retrieved user preferred theme: MTMTheme
2025-09-05 12:07:42.473 dbug: MTM_WIP_Application_Avalonia.Services.ConfigurationService[0]
      Configuration key 'Settings:AutoSave' not found, using default value
MTM_WIP_Application_Avalonia.Services.ConfigurationService: Debug: Configuration key 'Settings:AutoSave' not found, using default value
2025-09-05 12:07:42.484 dbug: MTM_WIP_Application_Avalonia.Services.ConfigurationService[0]
      Configuration key 'Settings:DefaultLocation' not found, using default value
MTM_WIP_Application_Avalonia.Services.ConfigurationService: Debug: Configuration key 'Settings:DefaultLocation' not found, using default value
2025-09-05 12:07:42.487 dbug: MTM_WIP_Application_Avalonia.Services.ConfigurationService[0]
      Configuration key 'Settings:DefaultOperation' not found, using default value
MTM_WIP_Application_Avalonia.Services.ConfigurationService: Debug: Configuration key 'Settings:DefaultOperation' not found, using default value
2025-09-05 12:07:42.490 dbug: MTM_WIP_Application_Avalonia.Services.ConfigurationService[0]
      Configuration key 'Settings:EnableAdvancedFeatures' not found, using default value
MTM_WIP_Application_Avalonia.Services.ConfigurationService: Debug: Configuration key 'Settings:EnableAdvancedFeatures' not found, using default value
2025-09-05 12:07:42.492 dbug: MTM_WIP_Application_Avalonia.Services.ConfigurationService[0]
      Configuration key 'Settings:WindowWidth' not found, using default value
MTM_WIP_Application_Avalonia.Services.ConfigurationService: Debug: Configuration key 'Settings:WindowWidth' not found, using default value
2025-09-05 12:07:42.494 dbug: MTM_WIP_Application_Avalonia.Services.ConfigurationService[0]
      Configuration key 'Settings:WindowHeight' not found, using default value
MTM_WIP_Application_Avalonia.Services.ConfigurationService: Debug: Configuration key 'Settings:WindowHeight' not found, using default value
2025-09-05 12:07:42.496 dbug: MTM_WIP_Application_Avalonia.Services.ConfigurationService[0]
      Configuration key 'Settings:RememberWindowSize' not found, using default value
MTM_WIP_Application_Avalonia.Services.ConfigurationService: Debug: Configuration key 'Settings:RememberWindowSize' not found, using default value
2025-09-05 12:07:42.500 warn: MTM_WIP_Application_Avalonia.Services.ThemeService[0]
      Application.Current is null, cannot apply theme
MTM_WIP_Application_Avalonia.Services.ThemeService: Warning: Application.Current is null, cannot apply theme
2025-09-05 12:07:42.502 dbug: MTM_WIP_Application_Avalonia.Services.ConfigurationService[0]
      Configuration key 'Settings:DefaultPageSize' not found, using default value
2025-09-05 12:07:42.502 info: MTM_WIP_Application_Avalonia.Services.ThemeService[0]
      Theme changed from MTM Default to MTM Default
MTM_WIP_Application_Avalonia.Services.ConfigurationService: Debug: Configuration key 'Settings:DefaultPageSize' not found, using default value
MTM_WIP_Application_Avalonia.Services.ThemeService: Information: Theme changed from MTM Default to MTM Default
2025-09-05 12:07:42.519 dbug: MTM_WIP_Application_Avalonia.Services.ConfigurationService[0]
      Configuration key 'Settings:EnableRealTimeUpdates' not found, using default value
MTM_WIP_Application_Avalonia.Services.ConfigurationService: Debug: Configuration key 'Settings:EnableRealTimeUpdates' not found, using default value
2025-09-05 12:07:42.523 info: MTM_WIP_Application_Avalonia.Services.SettingsService[0]
      Settings loaded successfully
MTM_WIP_Application_Avalonia.Services.SettingsService: Information: Settings loaded successfully
[VALIDATION-SUCCESS] ISettingsService resolved successfully
2025-09-05 12:07:42.552 info: MTM_WIP_Application_Avalonia.Services.ConfigurationService[0]
      Using connection string: Server=localhost, Database=mtm_wip_application_test, Uid=JKOLL
MTM_WIP_Application_Avalonia.Services.ConfigurationService: Information: Using connection string: Server=localhost, Database=mtm_wip_application_test, Uid=JKOLL
2025-09-05 12:07:42.557 info: MTM_WIP_Application_Avalonia.Services.DatabaseService[0]
      DatabaseService initialized with connection string
MTM_WIP_Application_Avalonia.Services.DatabaseService: Information: DatabaseService initialized with connection string
[VALIDATION-SUCCESS] IDatabaseService resolved successfully
[VALIDATION-SUCCESS] IQuickButtonsService resolved successfully
[VALIDATION-SUCCESS] IProgressService resolved successfully
2025-09-05 12:07:42.559 dbug: MTM_WIP_Application_Avalonia.Services.SuggestionOverlayService[0]
      SuggestionOverlayService created successfully
MTM_WIP_Application_Avalonia.Services.SuggestionOverlayService: Debug: SuggestionOverlayService created successfully
[VALIDATION-SUCCESS] ISuggestionOverlayService resolved successfully
2025-09-05 12:07:42.563 info: MTM_WIP_Application_Avalonia.Services.MasterDataService[0]
      MasterDataService initialized
MTM_WIP_Application_Avalonia.Services.MasterDataService: Information: MasterDataService initialized
[VALIDATION-SUCCESS] IMasterDataService resolved successfully
[2025-09-05 12:07:42.565] Runtime service validation completed successfully
[2025-09-05 12:07:42.566] Performing application validation...
2025-09-05 12:07:42.567 info: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Starting comprehensive application validation
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Information: Starting comprehensive application validation
[2025-09-05 12:07:42.570] StartupValidationService.ValidateApplication() started
[VALIDATION] Comprehensive application validation started
2025-09-05 12:07:42.578 dbug: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Starting service dependency validation
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Debug: Starting service dependency validation
[2025-09-05 12:07:42.580] ValidateServices() started
[SERVICE-VALIDATION] Service validation started
2025-09-05 12:07:42.596 trce: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Critical service IConfiguration validated successfully
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Trace: Critical service IConfiguration validated successfully
[SERVICE-VALIDATION] Critical service validated: IConfiguration
2025-09-05 12:07:42.600 trce: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Critical service ILoggerFactory validated successfully
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Trace: Critical service ILoggerFactory validated successfully
[SERVICE-VALIDATION] Critical service validated: ILoggerFactory
2025-09-05 12:07:42.617 trce: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Critical service IConfigurationService validated successfully
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Trace: Critical service IConfigurationService validated successfully
[SERVICE-VALIDATION] Critical service validated: IConfigurationService
2025-09-05 12:07:42.621 trce: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Critical service IApplicationStateService validated successfully
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Trace: Critical service IApplicationStateService validated successfully
[SERVICE-VALIDATION] Critical service validated: IApplicationStateService
2025-09-05 12:07:42.624 trce: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Critical service INavigationService validated successfully
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Trace: Critical service INavigationService validated successfully
[SERVICE-VALIDATION] Critical service validated: INavigationService
2025-09-05 12:07:42.629 trce: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Optional service IThemeService validated successfully
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Trace: Optional service IThemeService validated successfully
[SERVICE-VALIDATION] Optional service validated: IThemeService
2025-09-05 12:07:42.646 trce: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Optional service ISettingsService validated successfully
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Trace: Optional service ISettingsService validated successfully
[SERVICE-VALIDATION] Optional service validated: ISettingsService
2025-09-05 12:07:42.648 trce: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Optional service IDatabaseService validated successfully
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Trace: Optional service IDatabaseService validated successfully
[SERVICE-VALIDATION] Optional service validated: IDatabaseService
2025-09-05 12:07:42.656 dbug: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Starting ViewModel validation
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Debug: Starting ViewModel validation
[2025-09-05 12:07:42.658] ValidateViewModels() started
[VIEWMODEL-VALIDATION] ViewModel validation started
2025-09-05 12:07:42.703 info: MTM_WIP_Application_Avalonia.Services.ConfigurationService[0]
      Using connection string: Server=localhost, Database=mtm_wip_application_test, Uid=JKOLL
MTM_WIP_Application_Avalonia.Services.ConfigurationService: Information: Using connection string: Server=localhost, Database=mtm_wip_application_test, Uid=JKOLL
2025-09-05 12:07:42.706 trce: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Critical ViewModel InventoryTabViewModel validated successfully
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Trace: Critical ViewModel InventoryTabViewModel validated successfully
[VIEWMODEL-VALIDATION] Critical ViewModel validated: InventoryTabViewModel
[2025-09-05 12:07:42.713] ViewModel validation completed
[VIEWMODEL-VALIDATION] ViewModel validation completed
2025-09-05 12:07:42.715 dbug: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Service validation completed in 137ms with 0 errors and 0 warnings
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Debug: Service validation completed in 137ms with 0 errors and 0 warnings
[2025-09-05 12:07:42.718] Service validation completed in 137ms
[SERVICE-VALIDATION] Service validation completed in 137ms
2025-09-05 12:07:42.720 dbug: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Starting configuration validation
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Debug: Starting configuration validation
[2025-09-05 12:07:42.724] ValidateConfiguration() started
[CONFIG-VALIDATION] Configuration validation started
2025-09-05 12:07:42.736 dbug: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      DefaultConnection string is configured
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Debug: DefaultConnection string is configured
[CONFIG-VALIDATION] DefaultConnection validated
2025-09-05 12:07:42.752 dbug: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Configuration validation completed in 31ms with 0 errors and 1 warnings
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Debug: Configuration validation completed in 31ms with 0 errors and 1 warnings
[2025-09-05 12:07:42.754] Configuration validation completed in 31ms
[CONFIG-VALIDATION] Configuration validation completed in 31ms
2025-09-05 12:07:42.756 info: MTM_WIP_Application_Avalonia.Services.MasterDataService[0]
      Loading master data using MTM stored procedure patterns
MTM_WIP_Application_Avalonia.Services.MasterDataService: Information: Loading master data using MTM stored procedure patterns
2025-09-05 12:07:42.761 dbug: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Starting application requirements validation
2025-09-05 12:07:42.762 info: MTM_WIP_Application_Avalonia.Services.ConfigurationService[0]
      Using connection string: Server=localhost, Database=mtm_wip_application_test, Uid=JKOLL
MTM_WIP_Application_Avalonia.Services.ConfigurationService: Information: Using connection string: Server=localhost, Database=mtm_wip_application_test, Uid=JKOLL
2025-09-05 12:07:42.763 info: MTM_WIP_Application_Avalonia.Services.MasterDataService[0]
      Testing database connection...
MTM_WIP_Application_Avalonia.Services.MasterDataService: Information: Testing database connection...
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Debug: Starting application requirements validation
[2025-09-05 12:07:42.767] ValidateApplicationRequirements() started
[APP-REQUIREMENTS] Application requirements validation started
2025-09-05 12:07:42.779 dbug: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Application requirements validation completed in 17ms
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Debug: Application requirements validation completed in 17ms
[2025-09-05 12:07:42.781] Application requirements validation completed in 17ms
[APP-REQUIREMENTS] Application requirements validation completed in 17ms
2025-09-05 12:07:42.783 info: MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService[0]
      Application validation completed successfully in 216ms with 1 warnings
MTM_WIP_Application_Avalonia.Core.Startup.StartupValidationService: Information: Application validation completed successfully in 216ms with 1 warnings
[2025-09-05 12:07:42.785] Application validation completed successfully in 216ms
[VALIDATION] Application validation completed in 216ms - Valid: True
[2025-09-05 12:07:42.787] Application validation completed successfully
[2025-09-05 12:07:42.787] Service build and validation completed in 828ms
[BUILD-VALIDATE] Service provider build and validation completed in 828ms
2025-09-05 12:07:42.790 info: ApplicationStartup[0]
      Application initialization completed successfully in 1138ms
ApplicationStartup: Information: Application initialization completed successfully in 1138ms
[2025-09-05 12:07:42.798] Application initialization completed in 1138ms
[2025-09-05 12:07:42.799] Service configuration completed in 1160ms
2025-09-05 12:07:42.799 info: object[0]
      Service configuration completed successfully in 1160ms
object: Information: Service configuration completed successfully in 1160ms
[PROGRAM] Service configuration completed successfully in 1160ms
[2025-09-05 12:07:42.814] Starting Avalonia application...
[2025-09-05 12:07:42.814] Platform: Microsoft Windows NT 10.0.22631.0
[2025-09-05 12:07:42.815] Runtime: 8.0.19
[2025-09-05 12:07:42.815] Is Interactive: True
2025-09-05 12:07:42.815 info: object[0]
      Starting Avalonia application with 0 arguments
object: Information: Starting Avalonia application with 0 arguments
[2025-09-05 12:07:42.863] Building Avalonia app...
[PROGRAM] Building Avalonia application
[2025-09-05 12:07:43] App.Initialize() started
2025-09-05 12:07:43.655 info: MTM_WIP_Application_Avalonia.Services.MasterDataService[0]
      ? Database connection test successful - Server: localhost, Database: mtm_wip_application_test
MTM_WIP_Application_Avalonia.Services.MasterDataService: Information: ✅ Database connection test successful - Server: localhost, Database: mtm_wip_application_test
2025-09-05 12:07:43.667 dbug: MTM_WIP_Application_Avalonia.Services.MasterDataService[0]
      Loading Part IDs from database using DatabaseService
MTM_WIP_Application_Avalonia.Services.MasterDataService: Debug: Loading Part IDs from database using DatabaseService
2025-09-05 12:07:43.688 dbug: MTM_WIP_Application_Avalonia.Services.MasterDataService[0]
      Loading Operations from database using DatabaseService
MTM_WIP_Application_Avalonia.Services.MasterDataService: Debug: Loading Operations from database using DatabaseService
2025-09-05 12:07:43.695 dbug: MTM_WIP_Application_Avalonia.Services.MasterDataService[0]
      Loading Locations from database using DatabaseService
MTM_WIP_Application_Avalonia.Services.MasterDataService: Debug: Loading Locations from database using DatabaseService
2025-09-05 12:07:43.702 dbug: MTM_WIP_Application_Avalonia.Services.MasterDataService[0]
      Loading Users from database using DatabaseService
MTM_WIP_Application_Avalonia.Services.MasterDataService: Debug: Loading Users from database using DatabaseService
[2025-09-05 12:07:43] App.Initialize() completed - XAML loaded successfully
[2025-09-05 12:07:43] App.OnFrameworkInitializationCompleted() started
[2025-09-05 12:07:43] Initializing application services...
[2025-09-05 12:07:43] Configuration service obtained
[2025-09-05 12:07:43] Model_AppVariables initialized
2025-09-05 12:07:43.943 info: MTM_WIP_Application_Avalonia.App[0]
      MTM WIP Application framework initialization started
MTM_WIP_Application_Avalonia.App: Information: MTM WIP Application framework initialization started
2025-09-05 12:07:43.954 info: MTM_WIP_Application_Avalonia.App[0]
      Model_AppVariables and database helper initialized successfully
MTM_WIP_Application_Avalonia.App: Information: Model_AppVariables and database helper initialized successfully
[2025-09-05 12:07:43] Logging infrastructure initialized
[2025-09-05 12:07:43] Initializing theme system...
[2025-09-05 12:07:43] Removed 1 conflicting theme dictionaries
[2025-09-05 12:07:43] MTM Light theme resources loaded successfully
[2025-09-05 12:07:43] Tab theme colors applied successfully
[2025-09-05 12:07:43] Default theme initialized - MTM Light theme applied
[2025-09-05 12:07:43] Creating MainWindow with dependency injection...
2025-09-05 12:07:43.996 dbug: MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel[0]
      MainWindowViewModel constructor started
MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel: Debug: MainWindowViewModel constructor started
2025-09-05 12:07:44.022 info: MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel[0]
      MainWindowViewModel initialized with dependency injection - Services injected successfully
MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel: Information: MainWindowViewModel initialized with dependency injection - Services injected successfully
2025-09-05 12:07:44.024 dbug: MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel[0]
      NavigationService type: MTM_WIP_Application_Avalonia.Services.NavigationService
MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel: Debug: NavigationService type: MTM_WIP_Application_Avalonia.Services.NavigationService
2025-09-05 12:07:44.027 dbug: MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel[0]
      ApplicationStateService type: MTM_WIP_Application_Avalonia.Services.ApplicationStateService
MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel: Debug: ApplicationStateService type: MTM_WIP_Application_Avalonia.Services.ApplicationStateService
2025-09-05 12:07:44.029 dbug: MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel[0]
      Subscribing to NavigationService.Navigated event
MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel: Debug: Subscribing to NavigationService.Navigated event
2025-09-05 12:07:44.032 dbug: MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel[0]
      Successfully subscribed to NavigationService.Navigated event
MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel: Debug: Successfully subscribed to NavigationService.Navigated event
2025-09-05 12:07:44.035 dbug: MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel[0]
      Deferring MainView creation to avoid service provider dependency during startup validation
MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel: Debug: Deferring MainView creation to avoid service provider dependency during startup validation
2025-09-05 12:07:44.037 dbug: MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel[0]
      MainWindowViewModel constructor completed successfully
MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel: Debug: MainWindowViewModel constructor completed successfully
2025-09-05 12:07:44.064 info: MTM_WIP_Application_Avalonia.App[0]
      MainWindowViewModel service resolved successfully
MTM_WIP_Application_Avalonia.App: Information: MainWindowViewModel service resolved successfully
[2025-09-05 12:07:44] MainWindowViewModel resolved
2025-09-05 12:07:44.148 dbug: Helper_Database_StoredProcedure[0]
      Direct stored procedure executed: md_locations_Get_All, Rows: 10317
Helper_Database_StoredProcedure: Debug: Direct stored procedure executed: md_locations_Get_All, Rows: 10317
2025-09-05 12:07:44.298 dbug: Helper_Database_StoredProcedure[0]
      Direct stored procedure executed: md_operation_numbers_Get_All, Rows: 75
Helper_Database_StoredProcedure: Debug: Direct stored procedure executed: md_operation_numbers_Get_All, Rows: 75
2025-09-05 12:07:44.347 info: MTM_WIP_Application_Avalonia.App[0]
      Main window created with dependency injection - DataContext set to MainWindowViewModel
2025-09-05 12:07:44.349 dbug: Helper_Database_StoredProcedure[0]
      Stored procedure executed: usr_users_Get_All, Status: -1, Rows: 87
MTM_WIP_Application_Avalonia.App: Information: Main window created with dependency injection - DataContext set to MainWindowViewModel
[2025-09-05 12:07:44] MainWindow created and DataContext set
[2025-09-05 12:07:44] Initializing MainView after platform initialization...
Helper_Database_StoredProcedure: Debug: Stored procedure executed: usr_users_Get_All, Status: -1, Rows: 87
2025-09-05 12:07:44.362 dbug: MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel[0]
      Manually initializing MainView after startup
MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel: Debug: Manually initializing MainView after startup
2025-09-05 12:07:44.397 info: MTM_WIP_Application_Avalonia.Services.ConfigurationService[0]
      Using connection string: Server=localhost, Database=mtm_wip_application, Uid=JKOLL
MTM_WIP_Application_Avalonia.Services.ConfigurationService: Information: Using connection string: Server=localhost, Database=mtm_wip_application, Uid=JKOLL
2025-09-05 12:07:44.406 info: MTM_WIP_Application_Avalonia.ViewModels.RemoveItemViewModel[0]
      RemoveItemViewModel initialized with dependency injection
MTM_WIP_Application_Avalonia.ViewModels.RemoveItemViewModel: Information: RemoveItemViewModel initialized with dependency injection
2025-09-05 12:07:44.409 dbug: MTM_WIP_Application_Avalonia.Services.MasterDataService[0]
      Master data loading already in progress, skipping
MTM_WIP_Application_Avalonia.Services.MasterDataService: Debug: Master data loading already in progress, skipping
2025-09-05 12:07:44.419 info: MTM_WIP_Application_Avalonia.ViewModels.RemoveItemViewModel[0]
      => DataLoading
      Loading ComboBox data from database
MTM_WIP_Application_Avalonia.ViewModels.RemoveItemViewModel: Information: Loading ComboBox data from database
2025-09-05 12:07:44.421 dbug: Helper_Database_StoredProcedure[0]
      => DataLoading
      Executing stored procedure: md_part_ids_Get_All
Helper_Database_StoredProcedure: Debug: Executing stored procedure: md_part_ids_Get_All
2025-09-05 12:07:44.425 info: MTM_WIP_Application_Avalonia.ViewModels.TransferItemViewModel[0]
      TransferItemViewModel initialized with dependency injection
MTM_WIP_Application_Avalonia.ViewModels.TransferItemViewModel: Information: TransferItemViewModel initialized with dependency injection
2025-09-05 12:07:44.429 info: MTM_WIP_Application_Avalonia.ViewModels.TransferItemViewModel[0]
      Loading transfer ComboBox data from database
MTM_WIP_Application_Avalonia.ViewModels.TransferItemViewModel: Information: Loading transfer ComboBox data from database
2025-09-05 12:07:44.447 dbug: Helper_Database_StoredProcedure[0]
      Executing stored procedure: md_part_ids_Get_All
Helper_Database_StoredProcedure: Debug: Executing stored procedure: md_part_ids_Get_All
2025-09-05 12:07:44.459 info: MTM_WIP_Application_Avalonia.ViewModels.MainForm.AdvancedInventoryViewModel[0]
      AdvancedInventoryViewModel initialized with dependency injection
MTM_WIP_Application_Avalonia.ViewModels.MainForm.AdvancedInventoryViewModel: Information: AdvancedInventoryViewModel initialized with dependency injection
2025-09-05 12:07:44.462 info: MTM_WIP_Application_Avalonia.ViewModels.MainForm.AdvancedRemoveViewModel[0]
      Initializing AdvancedRemoveViewModel
MTM_WIP_Application_Avalonia.ViewModels.MainForm.AdvancedRemoveViewModel: Information: Initializing AdvancedRemoveViewModel
2025-09-05 12:07:44.465 info: MTM_WIP_Application_Avalonia.ViewModels.MainForm.AdvancedRemoveViewModel[0]
      AdvancedRemoveViewModel constructor completed - database loading deferred
MTM_WIP_Application_Avalonia.ViewModels.MainForm.AdvancedRemoveViewModel: Information: AdvancedRemoveViewModel constructor completed - database loading deferred
2025-09-05 12:07:44.467 info: MTM_WIP_Application_Avalonia.ViewModels.MainForm.AdvancedRemoveViewModel[0]
      AdvancedRemoveViewModel initialized successfully
MTM_WIP_Application_Avalonia.ViewModels.MainForm.AdvancedRemoveViewModel: Information: AdvancedRemoveViewModel initialized successfully
?????? QuickButtonsViewModel constructor STARTED
🔧🔧🔧 QuickButtonsViewModel constructor STARTED
2025-09-05 12:07:44.473 info: MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel[0]
      QuickButtonsViewModel initialized with dependency injection
MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel: Information: QuickButtonsViewModel initialized with dependency injection
2025-09-05 12:07:44.479 info: MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel[0]
      ?? QuickButtonsViewModel constructor started
MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel: Information: 🔧 QuickButtonsViewModel constructor started
?????? Current user in constructor: 'JKOLL'
🔧🔧🔧 Current user in constructor: 'JKOLL'
2025-09-05 12:07:44.488 info: MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel[0]
      ?? Current user in constructor: 'JKOLL'
MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel: Information: 🔧 Current user in constructor: 'JKOLL'
?????? Starting Dispatcher.UIThread.InvokeAsync for LoadLast10TransactionsAsync
🔧🔧🔧 Starting Dispatcher.UIThread.InvokeAsync for LoadLast10TransactionsAsync
2025-09-05 12:07:44.499 info: MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel[0]
      ?? Starting Dispatcher.UIThread.InvokeAsync for LoadLast10TransactionsAsync
MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel: Information: 🔧 Starting Dispatcher.UIThread.InvokeAsync for LoadLast10TransactionsAsync
?????? QuickButtonsViewModel constructor COMPLETED
🔧🔧🔧 QuickButtonsViewModel constructor COMPLETED
2025-09-05 12:07:44.503 info: MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel[0]
      ?? QuickButtonsViewModel constructor completed
?????? Loading transactions in background - about to call LoadLast10TransactionsAsync
MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel: Information: 🔧 QuickButtonsViewModel constructor completed
🔧🔧🔧 Loading transactions in background - about to call LoadLast10TransactionsAsync
2025-09-05 12:07:44.512 info: MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel[0]
      ?? Loading transactions in background - about to call LoadLast10TransactionsAsync
2025-09-05 12:07:44.513 info: MTM_WIP_Application_Avalonia.ViewModels.MainViewViewModel[0]
      MainViewViewModel initialized with dependency injection
MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel: Information: 🔧 Loading transactions in background - about to call LoadLast10TransactionsAsync
MTM_WIP_Application_Avalonia.ViewModels.MainViewViewModel: Information: MainViewViewModel initialized with dependency injection
?????? LoadLast10TransactionsAsync ENTRY POINT
🔧🔧🔧 LoadLast10TransactionsAsync ENTRY POINT
2025-09-05 12:07:44.525 info: MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel[0]
      ?? LoadLast10TransactionsAsync ENTRY POINT
MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel: Information: 🔧 LoadLast10TransactionsAsync ENTRY POINT
2025-09-05 12:07:44.529 info: MTM_WIP_Application_Avalonia.Services.ProgressService[0]
      Operation started: Loading recent transactions... (Cancellable: False)
MTM_WIP_Application_Avalonia.Services.ProgressService: Information: Operation started: Loading recent transactions... (Cancellable: False)
?????? Current user in LoadLast10TransactionsAsync: 'JKOLL'
🔧🔧🔧 Current user in LoadLast10TransactionsAsync: 'JKOLL'
2025-09-05 12:07:44.534 info: MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel[0]
      ?? Current user in LoadLast10TransactionsAsync: 'JKOLL'
MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel: Information: 🔧 Current user in LoadLast10TransactionsAsync: 'JKOLL'
?????? About to call QuickButtons service with user: JKOLL
🔧🔧🔧 About to call QuickButtons service with user: JKOLL
2025-09-05 12:07:44.544 info: MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel[0]
      ?? About to call QuickButtons service with user: JKOLL
MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel: Information: 🔧 About to call QuickButtons service with user: JKOLL
2025-09-05 12:07:44.563 dbug: MTM_WIP_Application_Avalonia.Services.QuickButtonsService[0]
      Loading last 10 transactions for user: JKOLL
MTM_WIP_Application_Avalonia.Services.QuickButtonsService: Debug: Loading last 10 transactions for user: JKOLL
2025-09-05 12:07:44.567 info: MTM_WIP_Application_Avalonia.Services.QuickButtonsService[0]
      Calling stored procedure sys_last_10_transactions_Get_ByUser with UserId: JKOLL, Limit: 10
MTM_WIP_Application_Avalonia.Services.QuickButtonsService: Information: Calling stored procedure sys_last_10_transactions_Get_ByUser with UserId: JKOLL, Limit: 10
2025-09-05 12:07:44.575 dbug: Helper_Database_StoredProcedure[0]
      Executing stored procedure: sys_last_10_transactions_Get_ByUser
Helper_Database_StoredProcedure: Debug: Executing stored procedure: sys_last_10_transactions_Get_ByUser
2025-09-05 12:07:44.581 info: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: Starting execution of sys_last_10_transactions_Get_ByUser
Helper_Database_StoredProcedure: Information: 🔍 QUICKBUTTON DEBUG: Starting execution of sys_last_10_transactions_Get_ByUser
2025-09-05 12:07:44.583 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: sys_last_10_transactions_Get_ByUser input parameters:
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG: sys_last_10_transactions_Get_ByUser input parameters:
2025-09-05 12:07:44.752 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG:   @p_UserID = 'JKOLL' (String)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG:   @p_UserID = 'JKOLL' (String)
2025-09-05 12:07:44.816 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG:   @p_Limit = '10' (Int32)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG:   @p_Limit = '10' (Int32)
2025-09-05 12:07:44.825 info: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: TRANSACTION GET CONTEXT - Retrieving last 10 transactions for user 'JKOLL'
Helper_Database_StoredProcedure: Information: 🔍 QUICKBUTTON DEBUG: TRANSACTION GET CONTEXT - Retrieving last 10 transactions for user 'JKOLL'
2025-09-05 12:07:44.841 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: Added parameter @p_UserID = JKOLL (Type: String)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG: Added parameter @p_UserID = JKOLL (Type: String)
2025-09-05 12:07:44.843 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: Added parameter @p_Limit = 10 (Type: Int32)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG: Added parameter @p_Limit = 10 (Type: Int32)
2025-09-05 12:07:44.845 info: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: Executing stored procedure with 2 input parameters
Helper_Database_StoredProcedure: Information: 🔍 QUICKBUTTON DEBUG: Executing stored procedure with 2 input parameters
2025-09-05 12:07:44.886 dbug: MTM_WIP_Application_Avalonia.Views.InventoryTabView[0]
      SuggestionOverlayService resolved in constructor: True
MTM_WIP_Application_Avalonia.Views.InventoryTabView: Debug: SuggestionOverlayService resolved in constructor: True
2025-09-05 12:07:44.908 info: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: Procedure sys_last_10_transactions_Get_ByUser completed - Status: 0, Message: 'Retrieved last 10 transactions for user', Rows: 2, Duration: 332ms
SuggestionOverlayService resolved in constructor: True
Helper_Database_StoredProcedure: Information: 🔍 QUICKBUTTON DEBUG: Procedure sys_last_10_transactions_Get_ByUser completed - Status: 0, Message: 'Retrieved last 10 transactions for user', Rows: 2, Duration: 332ms
2025-09-05 12:07:44.918 info: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: sys_last_10_transactions_Get_ByUser result - Status: 0 (SUCCESS), Message: 'Retrieved last 10 transactions for user'
Helper_Database_StoredProcedure: Information: 🔍 QUICKBUTTON DEBUG: sys_last_10_transactions_Get_ByUser result - Status: 0 (SUCCESS), Message: 'Retrieved last 10 transactions for user'
2025-09-05 12:07:44.924 info: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: Transaction get operation returned 2 transactions - SUCCESS
Helper_Database_StoredProcedure: Information: 🔍 QUICKBUTTON DEBUG: Transaction get operation returned 2 transactions - SUCCESS
2025-09-05 12:07:44.927 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: Result columns: ID(Int32), TransactionType(String), PartID(String), FromLocation(String), ToLocation(String), Operation(String), Quantity(Int32), Notes(String), User(String), ItemType(String), ReceiveDate(DateTime), BatchNumber(String), PartDescription(String), Customer(String), Position(Double)
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG: Result columns: ID(Int32), TransactionType(String), PartID(String), FromLocation(String), ToLocation(String), Operation(String), Quantity(Int32), Notes(String), User(String), ItemType(String), ReceiveDate(DateTime), BatchNumber(String), PartDescription(String), Customer(String), Position(Double)
2025-09-05 12:07:44.933 dbug: Helper_Database_StoredProcedure[0]
      ?? QUICKBUTTON DEBUG: First row data: ID=1096, TransactionType=IN, PartID=39137-0780, FromLocation=, ToLocation=, Operation=109, Quantity=100, Notes=, User=JKOLL, ItemType=, ReceiveDate=8/28/2025 4:29:03 PM, BatchNumber=, PartDescription=Stay - Comp, Customer=, Position=1
Helper_Database_StoredProcedure: Debug: 🔍 QUICKBUTTON DEBUG: First row data: ID=1096, TransactionType=IN, PartID=39137-0780, FromLocation=, ToLocation=, Operation=109, Quantity=100, Notes=, User=JKOLL, ItemType=, ReceiveDate=8/28/2025 4:29:03 PM, BatchNumber=, PartDescription=Stay - Comp, Customer=, Position=1
2025-09-05 12:07:44.937 dbug: Helper_Database_StoredProcedure[0]
      Stored procedure executed: sys_last_10_transactions_Get_ByUser, Status: 0, Rows: 2
Helper_Database_StoredProcedure: Debug: Stored procedure executed: sys_last_10_transactions_Get_ByUser, Status: 0, Rows: 2
2025-09-05 12:07:44.940 info: MTM_WIP_Application_Avalonia.Services.QuickButtonsService[0]
      Stored procedure returned 2 rows
MTM_WIP_Application_Avalonia.Services.QuickButtonsService: Information: Stored procedure returned 2 rows
2025-09-05 12:07:44.943 info: MTM_WIP_Application_Avalonia.Services.QuickButtonsService[0]
      Processing 2 rows from stored procedure result
MTM_WIP_Application_Avalonia.Services.QuickButtonsService: Information: Processing 2 rows from stored procedure result
2025-09-05 12:07:44.959 dbug: MTM_WIP_Application_Avalonia.Services.QuickButtonsService[0]
      Available columns: ID, TransactionType, PartID, FromLocation, ToLocation, Operation, Quantity, Notes, User, ItemType, ReceiveDate, BatchNumber, PartDescription, Customer, Position
MTM_WIP_Application_Avalonia.Services.QuickButtonsService: Debug: Available columns: ID, TransactionType, PartID, FromLocation, ToLocation, Operation, Quantity, Notes, User, ItemType, ReceiveDate, BatchNumber, PartDescription, Customer, Position
2025-09-05 12:07:44.963 dbug: MTM_WIP_Application_Avalonia.Services.QuickButtonsService[0]
      Row 0: PartID=39137-0780, Operation=109, Quantity=100
MTM_WIP_Application_Avalonia.Services.QuickButtonsService: Debug: Row 0: PartID=39137-0780, Operation=109, Quantity=100
2025-09-05 12:07:44.966 dbug: MTM_WIP_Application_Avalonia.Services.QuickButtonsService[0]
      Row 1: PartID=39137-0780, Operation=109, Quantity=100
MTM_WIP_Application_Avalonia.Services.QuickButtonsService: Debug: Row 1: PartID=39137-0780, Operation=109, Quantity=100
Loaded 2 transactions for user JKOLL
2025-09-05 12:07:44.971 info: MTM_WIP_Application_Avalonia.Services.QuickButtonsService[0]
      Successfully loaded 2 recent transactions for user JKOLL
MTM_WIP_Application_Avalonia.Services.QuickButtonsService: Information: Successfully loaded 2 recent transactions for user JKOLL
?????? QuickButtons service returned 2 transactions
🔧🔧🔧 QuickButtons service returned 2 transactions
2025-09-05 12:07:44.982 info: MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel[0]
      ?? QuickButtons service returned 2 transactions
MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel: Information: 🔧 QuickButtons service returned 2 transactions
?????? Total QuickButtons collection now has 0 items (0 non-empty, 0 empty)
🔧🔧🔧 Total QuickButtons collection now has 0 items (0 non-empty, 0 empty)
2025-09-05 12:07:44.989 info: MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel[0]
      ?? Total QuickButtons collection now has 0 items (0 non-empty, 0 empty)
MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel: Information: 🔧 Total QuickButtons collection now has 0 items (0 non-empty, 0 empty)
2025-09-05 12:07:44.994 info: MTM_WIP_Application_Avalonia.Services.ProgressService[0]
      Operation completed: Loading recent transactions...
MTM_WIP_Application_Avalonia.Services.ProgressService: Information: Operation completed: Loading recent transactions...
?????? Background loading completed LoadLast10TransactionsAsync
🔧🔧🔧 Dispatcher.UIThread.InvokeAsync completed LoadLast10TransactionsAsync
2025-09-05 12:07:45.003 info: MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel[0]
      ?? Dispatcher.UIThread.InvokeAsync completed LoadLast10TransactionsAsync
MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel: Information: 🔧 Dispatcher.UIThread.InvokeAsync completed LoadLast10TransactionsAsync
[ComboBoxBehavior] ENABLED for Avalonia.Controls.AutoCompleteBox at 12:07:45.026
[ComboBoxBehavior] ENABLED for Avalonia.Controls.AutoCompleteBox at 12:07:45.029
[ComboBoxBehavior] ENABLED for Avalonia.Controls.AutoCompleteBox at 12:07:45.098
[ComboBoxBehavior] ENABLED for Avalonia.Controls.AutoCompleteBox at 12:07:45.103
[ComboBoxBehavior] ENABLED for Avalonia.Controls.AutoCompleteBox at 12:07:45.108
2025-09-05 12:07:45.145 dbug: MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel[0]
      MainViewViewModel resolved successfully for manual initialization, type: MTM_WIP_Application_Avalonia.ViewModels.MainViewViewModel
MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel: Debug: MainViewViewModel resolved successfully for manual initialization, type: MTM_WIP_Application_Avalonia.ViewModels.MainViewViewModel
2025-09-05 12:07:45.454 info: MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel[0]
      MainView manually initialized and set as CurrentView
MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel: Information: MainView manually initialized and set as CurrentView
[2025-09-05 12:07:45] MainView initialized successfully after platform startup
2025-09-05 12:07:45.470 info: MTM_WIP_Application_Avalonia.App[0]
      MainView initialized successfully after Avalonia platform startup
MTM_WIP_Application_Avalonia.App: Information: MainView initialized successfully after Avalonia platform startup
[2025-09-05 12:07:45] Scheduling startup dialog...
2025-09-05 12:07:45.480 dbug: MTM_WIP_Application_Avalonia.App[0]
      Startup dialog task started
MTM_WIP_Application_Avalonia.App: Debug: Startup dialog task started
2025-09-05 12:07:45.484 info: MTM_WIP_Application_Avalonia.App[0]
      Framework initialization completed successfully
MTM_WIP_Application_Avalonia.App: Information: Framework initialization completed successfully
[2025-09-05 12:07:45] Framework initialization completed
2025-09-05 12:07:46.187 dbug: Helper_Database_StoredProcedure[0]
      => DataLoading
      Stored procedure executed: md_part_ids_Get_All, Status: -1, Rows: 2908
Helper_Database_StoredProcedure: Debug: Stored procedure executed: md_part_ids_Get_All, Status: -1, Rows: 2908
2025-09-05 12:07:46.193 dbug: Helper_Database_StoredProcedure[0]
      => DataLoading
      Executing stored procedure: md_operation_numbers_Get_All
Helper_Database_StoredProcedure: Debug: Executing stored procedure: md_operation_numbers_Get_All
2025-09-05 12:07:46.282 dbug: Helper_Database_StoredProcedure[0]
      => DataLoading
      Stored procedure executed: md_operation_numbers_Get_All, Status: -1, Rows: 75
Helper_Database_StoredProcedure: Debug: Stored procedure executed: md_operation_numbers_Get_All, Status: -1, Rows: 75
2025-09-05 12:07:46.286 info: MTM_WIP_Application_Avalonia.ViewModels.RemoveItemViewModel[0]
      => DataLoading
      ComboBox data loaded successfully - Parts: 0, Operations: 0
MTM_WIP_Application_Avalonia.ViewModels.RemoveItemViewModel: Information: ComboBox data loaded successfully - Parts: 0, Operations: 0
2025-09-05 12:07:46.319 dbug: Helper_Database_StoredProcedure[0]
      Stored procedure executed: md_part_ids_Get_All, Status: -1, Rows: 2908
Helper_Database_StoredProcedure: Debug: Stored procedure executed: md_part_ids_Get_All, Status: -1, Rows: 2908
2025-09-05 12:07:46.324 dbug: Helper_Database_StoredProcedure[0]
      Executing stored procedure: md_operation_numbers_Get_All
Helper_Database_StoredProcedure: Debug: Executing stored procedure: md_operation_numbers_Get_All
2025-09-05 12:07:46.345 dbug: Helper_Database_StoredProcedure[0]
      Stored procedure executed: md_operation_numbers_Get_All, Status: -1, Rows: 75
Helper_Database_StoredProcedure: Debug: Stored procedure executed: md_operation_numbers_Get_All, Status: -1, Rows: 75
2025-09-05 12:07:46.349 dbug: Helper_Database_StoredProcedure[0]
      Executing stored procedure: md_locations_Get_All
Helper_Database_StoredProcedure: Debug: Executing stored procedure: md_locations_Get_All
2025-09-05 12:07:46.512 dbug: MTM_WIP_Application_Avalonia.Services.MasterDataService[0]
      Master data loading already in progress, skipping
MTM_WIP_Application_Avalonia.Services.MasterDataService: Debug: Master data loading already in progress, skipping
[2025-09-05 12:07:46] Master data loaded successfully at startup
2025-09-05 12:07:46.515 info: MTM_WIP_Application_Avalonia.App[0]
      Master data loaded successfully at application startup
MTM_WIP_Application_Avalonia.App: Information: Master data loaded successfully at application startup
2025-09-05 12:07:46.556 dbug: Helper_Database_StoredProcedure[0]
      Stored procedure executed: md_locations_Get_All, Status: -1, Rows: 10317
Helper_Database_StoredProcedure: Debug: Stored procedure executed: md_locations_Get_All, Status: -1, Rows: 10317
2025-09-05 12:07:46.559 info: MTM_WIP_Application_Avalonia.ViewModels.TransferItemViewModel[0]
      Transfer ComboBox data loaded successfully
MTM_WIP_Application_Avalonia.ViewModels.TransferItemViewModel: Information: Transfer ComboBox data loaded successfully
Window positioned at 360, 91 with size 1200x850 on screen
Window size constraints set: Min(800x500) Max(1920x1032) Current(1200x850)
DEBUG: MTM_Shared_Logic.PrimaryAction found: True, value: DarkGoldenrod
DEBUG: MTM_Shared_Logic.OverlayTextBrush found: True, value: White
DEBUG: MTM_Shared_Logic.CardBackgroundBrush found: True, value: White
DEBUG: MTM_Shared_Logic.BorderAccentBrush found: True, value: #ffffeb9c
DEBUG: MTM_Shared_Logic.HoverBackground found: True, value: #fffff9d1
DEBUG: MTM_Shared_Logic.BodyText found: True, value: #ff666666
DEBUG: Current RequestedThemeVariant: Light
DEBUG: Merged dictionaries count: 1
DEBUG: MTM Light theme found at index: -1
2025-09-05 12:07:46.640 dbug: MTM_WIP_Application_Avalonia.Views.ThemeQuickSwitcher[0]
      ThemeQuickSwitcher OnLoaded event started
MTM_WIP_Application_Avalonia.Views.ThemeQuickSwitcher: Debug: ThemeQuickSwitcher OnLoaded event started
2025-09-05 12:07:46.643 warn: MTM_WIP_Application_Avalonia.Views.ThemeQuickSwitcher[0]
      Current theme MTMTheme not found in dropdown, defaulting to first item
MTM_WIP_Application_Avalonia.Views.ThemeQuickSwitcher: Warning: Current theme MTMTheme not found in dropdown, defaulting to first item
2025-09-05 12:07:46.648 info: MTM_WIP_Application_Avalonia.Views.ThemeQuickSwitcher[0]
      ThemeQuickSwitcher initialized successfully with 15 themes
MTM_WIP_Application_Avalonia.Views.ThemeQuickSwitcher: Information: ThemeQuickSwitcher initialized successfully with 15 themes
2025-09-05 12:07:46.679 dbug: MTM_WIP_Application_Avalonia.Views.InventoryTabView[0]
      Save button validation completed - CanSave: False
MTM_WIP_Application_Avalonia.Views.InventoryTabView: Debug: Save button validation completed - CanSave: False
2025-09-05 12:07:46.687 dbug: MTM_WIP_Application_Avalonia.Views.InventoryTabView[0]
      Form state initialized successfully
MTM_WIP_Application_Avalonia.Views.InventoryTabView: Debug: Form state initialized successfully
2025-09-05 12:07:46.689 dbug: MTM_WIP_Application_Avalonia.Views.InventoryTabView[0]
      Save button validation completed - CanSave: False
MTM_WIP_Application_Avalonia.Views.InventoryTabView: Debug: Save button validation completed - CanSave: False
2025-09-05 12:07:46.699 info: MTM_WIP_Application_Avalonia.Views.InventoryTabView[0]
      InventoryTabView ViewModel connected successfully
MTM_WIP_Application_Avalonia.Views.InventoryTabView: Information: InventoryTabView ViewModel connected successfully
2025-09-05 12:07:46.704 dbug: MTM_WIP_Application_Avalonia.Views.InventoryTabView[0]
      QuickButtonsView not found in visual tree - integration skipped
MTM_WIP_Application_Avalonia.Views.InventoryTabView: Debug: QuickButtonsView not found in visual tree - integration skipped
2025-09-05 12:07:46.724 dbug: MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel[0]
      ?? Creating button 1: PartId=39137-0780, Operation=109, Quantity=100
MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel: Debug: 🔧 Creating button 1: PartId=39137-0780, Operation=109, Quantity=100
2025-09-05 12:07:46.737 info: MTM_WIP_Application_Avalonia.Services.MasterDataService[0]
      Successfully loaded 2908 Part IDs from database
MTM_WIP_Application_Avalonia.Services.MasterDataService: Information: Successfully loaded 2908 Part IDs from database
2025-09-05 12:07:46.738 info: MTM_WIP_Application_Avalonia.Services.MasterDataService[0]
      Successfully loaded 10317 Locations from database
MTM_WIP_Application_Avalonia.Services.MasterDataService: Information: Successfully loaded 10317 Locations from database
2025-09-05 12:07:46.738 info: MTM_WIP_Application_Avalonia.Services.MasterDataService[0]
      Successfully loaded 75 Operations from database
MTM_WIP_Application_Avalonia.Services.MasterDataService: Information: Successfully loaded 75 Operations from database
2025-09-05 12:07:46.752 info: MTM_WIP_Application_Avalonia.Services.MasterDataService[0]
      Successfully loaded 87 Users from database
MTM_WIP_Application_Avalonia.Services.MasterDataService: Information: Successfully loaded 87 Users from database
2025-09-05 12:07:46.754 info: MTM_WIP_Application_Avalonia.Services.MasterDataService[0]
      Master data loaded successfully from database - Parts: 2908, Operations: 75, Locations: 10317, Users: 87
MTM_WIP_Application_Avalonia.Services.MasterDataService: Information: Master data loaded successfully from database - Parts: 2908, Operations: 75, Locations: 10317, Users: 87
2025-09-05 12:07:46.778 dbug: MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel[0]
      ?? Creating button 2: PartId=39137-0780, Operation=109, Quantity=100
MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel: Debug: 🔧 Creating button 2: PartId=39137-0780, Operation=109, Quantity=100
2025-09-05 12:07:46.817 info: MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel[0]
      ?? Added 2 transaction buttons to QuickButtons collection
MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel: Information: 🔧 Added 2 transaction buttons to QuickButtons collection
2025-09-05 12:07:47.117 dbug: MTM_WIP_Application_Avalonia.App[0]
      Startup dialog execution point reached
MTM_WIP_Application_Avalonia.App: Debug: Startup dialog execution point reached
MTM_WIP_Application_Avalonia.Views.InventoryTabView: Information: SuggestionOverlayService successfully resolved - Type: SuggestionOverlayService
2025-09-05 12:07:51.909 info: MTM_WIP_Application_Avalonia.Views.InventoryTabView[0]
      SuggestionOverlayService successfully resolved - Type: SuggestionOverlayService
SuggestionOverlayService successfully resolved - Type: SuggestionOverlayService
2025-09-05 12:07:52.190 info: MTM_WIP_Application_Avalonia.Views.InventoryTabView[0]
      [FocusLost] PartTextBox lost focus. User entered: ''. PartIds count: 2908
MTM_WIP_Application_Avalonia.Views.InventoryTabView: Information: [FocusLost] PartTextBox lost focus. User entered: ''. PartIds count: 2908
[FocusLost] PartTextBox lost focus. User entered: ''. PartIds count: 2908
2025-09-05 12:07:52.203 info: MTM_WIP_Application_Avalonia.Views.InventoryTabView[0]
      Sample PartIDs: 00061011, 00658500, 01-27991-000, 01-31591-000, 01-32672-000, 01-33016-000, 01-33371-000, 01-34467-000, 01-34577-000, 01-34578-000
MTM_WIP_Application_Avalonia.Views.InventoryTabView: Information: Sample PartIDs: 00061011, 00658500, 01-27991-000, 01-31591-000, 01-32672-000, 01-33016-000, 01-33371-000, 01-34467-000, 01-34577-000, 01-34578-000
Sample PartIDs: 00061011, 00658500, 01-27991-000, 01-31591-000, 01-32672-000, 01-33016-000, 01-33371-000, 01-34467-000, 01-34577-000, 01-34578-000
2025-09-05 12:07:52.214 info: MTM_WIP_Application_Avalonia.Views.InventoryTabView[0]
      Part '' - ExactMatch: False, SemiMatches: 2908
MTM_WIP_Application_Avalonia.Views.InventoryTabView: Information: Part '' - ExactMatch: False, SemiMatches: 2908
Part '' - ExactMatch: False, SemiMatches: 2908
2025-09-05 12:07:52.220 info: MTM_WIP_Application_Avalonia.Views.InventoryTabView[0]
      Part overlay not shown - value is empty
MTM_WIP_Application_Avalonia.Views.InventoryTabView: Information: Part overlay not shown - value is empty
Part overlay not shown - value is empty
2025-09-05 12:07:52.230 dbug: MTM_WIP_Application_Avalonia.Views.InventoryTabView[0]
      Save button validation completed - CanSave: False
MTM_WIP_Application_Avalonia.Views.InventoryTabView: Debug: Save button validation completed - CanSave: False
2025-09-05 12:07:52.285 dbug: MTM_WIP_Application_Avalonia.Views.ThemeQuickSwitcher[0]
      ThemeQuickSwitcher unloaded and cleaned up
MTM_WIP_Application_Avalonia.Views.ThemeQuickSwitcher: Debug: ThemeQuickSwitcher unloaded and cleaned up
2025-09-05 12:07:52.293 dbug: MTM_WIP_Application_Avalonia.Views.InventoryTabView[0]
      InventoryTabView cleanup completed - arrow key handling delegated to MainWindow
MTM_WIP_Application_Avalonia.Views.InventoryTabView: Debug: InventoryTabView cleanup completed - arrow key handling delegated to MainWindow
[2025-09-05 12:07:52.307] Application completed successfully in 10744ms (Avalonia: 9478ms)
2025-09-05 12:07:52.307 info: object[0]
      MTM WIP Application completed successfully - Total: 10744ms, Avalonia: 9478ms
object: Information: MTM WIP Application completed successfully - Total: 10744ms, Avalonia: 9478ms
The program '[572] MTM_WIP_Application_Avalonia.exe' has exited with code 0 (0x0).

*** Current Build Log ***
    PS C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia> dotnet clean

Build succeeded in 1.0s
PS C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia> dotnet restore
Restore complete (0.8s)

Build succeeded in 1.0s
PS C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia> dotnet build  
Restore complete (0.9s)
  MTM_WIP_Application_Avalonia succeeded with 23 warning(s) (24.8s) → bin\Debug\net8.0\MTM_WIP_Application_Avalonia.dll
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Views\MainForm\Panels\QuickButtonsView.axaml.cs(81,60): warning CS8602: Dereference of a possibly null reference.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Views\MainForm\Overlays\ThemeQuickSwitcher.axaml.cs(64,47): warning CS1998: This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Controls\TransactionExpandableButton.axaml.cs(226,9): warning CS8602: Dereference of a possibly null reference.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Controls\TransactionExpandableButton.axaml.cs(227,9): warning CS8602: Dereference of a possibly null reference.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Controls\CollapsiblePanel.axaml.cs(183,9): warning CS8602: Dereference of a possibly null reference.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Controls\CollapsiblePanel.axaml.cs(186,24): warning CS8604: Possible null reference argument for parameter 'element' in 'void Grid.SetColumn(Control element, int value)'.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Controls\CollapsiblePanel.axaml.cs(187,24): warning CS8604: Possible null reference argument for parameter 'element' in 'void Grid.SetColumn(Control element, int value)'.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Controls\CollapsiblePanel.axaml.cs(198,9): warning CS8602: Dereference of a possibly null reference.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Controls\CollapsiblePanel.axaml.cs(201,24): warning CS8604: Possible null reference argument for parameter 'element' in 'void Grid.SetColumn(Control element, int value)'.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Controls\CollapsiblePanel.axaml.cs(202,24): warning CS8604: Possible null reference argument for parameter 'element' in 'void Grid.SetColumn(Control element, int value)'.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Controls\CollapsiblePanel.axaml.cs(213,9): warning CS8602: Dereference of a possibly null reference.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Controls\CollapsiblePanel.axaml.cs(216,21): warning CS8604: Possible null reference argument for parameter 'element' in 'void Grid.SetRow(Control element, int value)'.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Controls\CollapsiblePanel.axaml.cs(217,21): warning CS8604: Possible null reference argument for parameter 'element' in 'void Grid.SetRow(Control element, int value)'.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Controls\CollapsiblePanel.axaml.cs(228,9): warning CS8602: Dereference of a possibly null reference.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Controls\CollapsiblePanel.axaml.cs(231,21): warning CS8604: Possible null reference argument for parameter 'element' in 'void Grid.SetRow(Control element, int value)'.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Controls\CollapsiblePanel.axaml.cs(232,21): warning CS8604: Possible null reference argument for parameter 'element' in 'void Grid.SetRow(Control element, int value)'.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Behaviors\AutoCompleteBoxNavigationBehavior.cs(209,37): warning CS8604: Possible null reference argument for parameter 'item' in 'void ItemsControl.ScrollIntoView(object item)'.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Services\VirtualPanelManager.cs(99,23): warning CS1998: This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Services\ErrorHandling.cs(100,30): warning CS1998: This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.   
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Services\StartupDialog.cs(106,48): warning CS8604: Possible null reference argument for parameter 'owner' in 'Task Window.ShowDialog(Window owner)'.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Services\SettingsPanelStateManager.cs(138,38): warning CS1998: This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Services\SettingsPanelStateManager.cs(205,38): warning CS1998: This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
    C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia\Services\SettingsPanelStateManager.cs(237,38): warning CS1998: This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.

Build succeeded with 23 warning(s) in 26.3s

Workload updates are available. Run `dotnet workload list` for more information.
PS C:\Users\jkoll\source\repos\MTM_WIP_Application_Avalonia\MTM_WIP_Application_Avalonia> 