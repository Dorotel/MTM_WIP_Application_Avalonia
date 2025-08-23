# Production Database Schema (mtm_wip_application)

## Overview
This file contains the complete schema definition for the MTM WIP Application production database. This is the MySQL database schema used in the live production environment and should be treated as the authoritative source for table structures, relationships, and constraints.

## ?? CRITICAL DATABASE RULE ??
**ALL database operations in application code MUST use stored procedures ONLY.**
- **PROHIBITED**: Direct SQL queries in C# code
- **REQUIRED**: Use `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()` pattern

## Database Information
- **Database Name**: `mtm_wip_application`
- **Engine**: MySQL 5.7.24+
- **Character Set**: utf8mb4
- **Purpose**: Production inventory management system for MTM (Manitowoc Tool and Manufacturing)

## Related Files
- **Schema File**: `Production_Database_Schema.sql`
- **Stored Procedures**: `Existing_Stored_Procedures.sql` (READ-ONLY)
- **Development Files**: `Development/Database_Files/` (all development work)

---

## Table Definitions

### `app_themes`
**Purpose**: Stores application theme configurations and color schemes.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ThemeName | varchar(100) | PRIMARY KEY, NOT NULL | Unique identifier for the theme |
| SettingsJson | json | NOT NULL | JSON object containing theme settings, colors, and styling information |

**Business Rules**:
- Theme names must be unique across the system
- SettingsJson contains color definitions, font settings, and UI styling parameters
- Used by the application's theme system to provide consistent branding

**Related Tables**: Referenced by user preferences in `usr_ui_settings`

**Stored Procedures**: None currently defined

---

### `inv_inventory`
**Purpose**: Core inventory table storing current inventory items and their details.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ID | int(11) | PRIMARY KEY, AUTO_INCREMENT | Unique identifier for each inventory record |
| PartID | varchar(300) | NOT NULL | Part identifier/number (MTM-specific format) |
| Location | varchar(100) | NOT NULL | Physical location where the part is stored |
| Operation | varchar(100) | NULL | Manufacturing operation number (stored as string) |
| Quantity | int(11) | NOT NULL | Current quantity of the part at this location |
| ItemType | varchar(100) | NOT NULL, DEFAULT 'WIP' | Type of inventory item (WIP, RAW, FINISHED, etc.) |
| ReceiveDate | datetime | NOT NULL, DEFAULT CURRENT_TIMESTAMP | When the item was received into inventory |
| LastUpdated | datetime | NOT NULL, DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP | Last modification timestamp |
| User | varchar(100) | NOT NULL | User who last modified this record |
| BatchNumber | varchar(300) | NULL | Batch tracking number for traceability |
| Notes | varchar(1000) | NULL | Additional notes or comments |

**Indexes**:
- `idx_partid_location` (PartID, Location)
- `idx_operation` (Operation)
- `idx_receivedate` (ReceiveDate)

**Business Rules**:
- PartID follows MTM part numbering conventions
- Operation numbers are stored as strings (e.g., "90", "100", "110")
- Location must exist in `md_locations` table
- BatchNumber provides traceability for quality control
- LastUpdated automatically tracks all modifications

**Related Tables**:
- `md_part_ids` - Part master data
- `md_locations` - Valid location codes
- `md_operation_numbers` - Valid operation numbers
- `inv_transaction` - Transaction history for this inventory

**Stored Procedures**:
- `inv_inventory_Add_Item` - Add new inventory item
- `inv_inventory_Get_ByPartID` - Get inventory by part ID
- `inv_inventory_Get_ByPartIDandOperation` - Get by part and operation
- `inv_inventory_Get_ByUser` - Get inventory by user
- `inv_inventory_Remove_Item` - Remove inventory item
- `inv_inventory_Transfer_Part` - Transfer entire part to new location
- `inv_inventory_Transfer_Quantity` - Transfer partial quantity
- `inv_inventory_Fix_BatchNumbers` - Fix null batch numbers

---

### `inv_inventory_batch_seq`
**Purpose**: Manages batch number sequencing for inventory tracking.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| last_batch_number | bigint(20) | PRIMARY KEY, NOT NULL | Last assigned batch number |
| current_match | int(11) | NOT NULL | Current matching position for batch processing |

**Business Rules**:
- Single row table maintaining sequence state
- last_batch_number increments for each new batch
- current_match tracks progress during batch processing operations
- Used by stored procedures for batch number assignment

**Related Tables**: Used with `inv_inventory` and `inv_transaction` for batch tracking

**Stored Procedures**:
- `assign_BatchNumber_Step0` - Initialize batch migration
- `assign_BatchNumber_Step1` - Assign batch numbers to inventory
- `assign_BatchNumber_Step2` - Assign batch numbers to IN transactions
- `assign_BatchNumber_Step3` - Match OUT transactions with batches
- `assign_BatchNumber_Step4` - Match TRANSFER transactions with batches

---

### `inv_transaction`
**Purpose**: Complete audit trail of all inventory movements and transactions.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ID | int(11) | PRIMARY KEY, AUTO_INCREMENT | Unique transaction identifier |
| TransactionType | enum('IN','OUT','TRANSFER') | NOT NULL | Type of inventory transaction |
| BatchNumber | varchar(300) | NULL | Batch number associated with transaction |
| PartID | varchar(300) | NOT NULL | Part identifier involved in transaction |
| FromLocation | varchar(100) | NULL | Source location (NULL for IN transactions) |
| ToLocation | varchar(100) | NULL | Destination location (NULL for OUT transactions) |
| Operation | varchar(100) | NULL | Manufacturing operation associated with transaction |
| Quantity | int(11) | NOT NULL | Quantity involved in transaction |
| Notes | varchar(1000) | NULL | Transaction notes or comments |
| User | varchar(100) | NOT NULL | User who performed the transaction |
| ItemType | varchar(100) | NOT NULL, DEFAULT 'WIP' | Type of item in transaction |
| ReceiveDate | datetime | NOT NULL, DEFAULT CURRENT_TIMESTAMP | When transaction occurred |

**Indexes**:
- `idx_partid` (PartID)
- `idx_user` (User)
- `idx_datetime` (ReceiveDate)

**Business Rules**:
- **IN**: Items entering inventory (FromLocation=source, ToLocation=NULL)
- **OUT**: Items leaving inventory (FromLocation=source, ToLocation=NULL)
- **TRANSFER**: Items moving between locations (FromLocation=source, ToLocation=destination)
- All transactions must have valid User and PartID
- Quantity must be positive
- ReceiveDate provides chronological ordering

**Related Tables**:
- `inv_inventory` - Current inventory state
- `usr_users` - Transaction user validation
- `md_part_ids` - Part master data

**Stored Procedures**:
- `inv_transaction_Add` - Add new transaction record
- `sp_ReassignBatchNumbers` - Reassign batch numbers for transactions

---

### `log_changelog`
**Purpose**: Tracks application version changes and release notes.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Version | varchar(50) | PRIMARY KEY, NOT NULL | Version number (semantic versioning) |
| Notes | longtext | NULL | Release notes and change descriptions |

**Business Rules**:
- Version follows semantic versioning (e.g., "1.2.3")
- Notes contain markdown-formatted release information
- Used by application to show changelog to users

**Stored Procedures**:
- `log_changelog_Get_Current` - Get latest version information

---

### `log_error`
**Purpose**: Comprehensive error logging and application monitoring.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ID | int(11) | PRIMARY KEY, AUTO_INCREMENT | Unique error log identifier |
| User | varchar(100) | NULL | User associated with the error (if available) |
| Severity | enum('Information','Warning','Error','Critical','High') | NOT NULL, DEFAULT 'Error' | Error severity level |
| ErrorType | varchar(100) | NULL | Category or type of error |
| ErrorMessage | text | NOT NULL | Human-readable error message |
| StackTrace | text | NULL | Full exception stack trace |
| ModuleName | varchar(200) | NULL | Application module where error occurred |
| MethodName | varchar(200) | NULL | Method name where error occurred |
| AdditionalInfo | text | NULL | Additional diagnostic information |
| MachineName | varchar(100) | NULL | Computer name where error occurred |
| OSVersion | varchar(100) | NULL | Operating system version |
| AppVersion | varchar(50) | NULL | Application version when error occurred |
| ErrorTime | datetime | NOT NULL, DEFAULT CURRENT_TIMESTAMP | When the error occurred |

**Indexes**:
- `idx_errortime` (ErrorTime)
- `idx_user` (User)
- `idx_severity` (Severity)
- `idx_errortype` (ErrorType)

**Business Rules**:
- All application errors should be logged here via stored procedures
- Severity levels help prioritize error resolution
- ErrorTime provides chronological error tracking
- Stack traces help with debugging
- User field helps identify user-specific issues

**Stored Procedures**:
- `log_error_Add_Error` - Add new error log entry
- `log_error_Get_All` - Retrieve all error logs
- `log_error_Get_ByUser` - Get errors for specific user
- `log_error_Get_ByDateRange` - Get errors within date range
- `log_error_Get_Unique` - Get unique error combinations
- `log_error_Delete_All` - Clear all error logs
- `log_error_Delete_ById` - Delete specific error log

---

### `md_item_types`
**Purpose**: Master data for valid inventory item types.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ID | int(11) | PRIMARY KEY, AUTO_INCREMENT | Unique identifier |
| ItemType | varchar(100) | NOT NULL, UNIQUE (uq_type) | Item type code |
| IssuedBy | varchar(100) | NOT NULL | User who created this item type |

**Business Rules**:
- ItemType values include: WIP, RAW, FINISHED, TOOLING, etc.
- Used to validate ItemType in inventory and transaction tables
- IssuedBy tracks data governance

**Related Tables**: Referenced by `inv_inventory` and `inv_transaction`

**Stored Procedures**:
- `md_item_types_Add_ItemType` - Add new item type
- `md_item_types_Get_All` - Get all item types
- `md_item_types_Update_ItemType` - Update existing item type
- `md_item_types_Delete_ByID` - Delete by ID
- `md_item_types_Delete_ByType` - Delete by type name

---

### `md_locations`
**Purpose**: Master data for valid inventory storage locations.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ID | int(11) | PRIMARY KEY, AUTO_INCREMENT | Unique identifier |
| Location | varchar(100) | NOT NULL, UNIQUE (uq_location) | Location code |
| Building | varchar(100) | NOT NULL, DEFAULT 'Expo' | Building where location exists |
| IssuedBy | varchar(100) | NOT NULL | User who created this location |

**Business Rules**:
- Location codes must be unique across all buildings
- Building provides physical grouping of locations
- Default building is 'Expo' for historical reasons
- Used to validate locations in inventory and transactions

**Related Tables**: Referenced by `inv_inventory` and `inv_transaction`

**Stored Procedures**:
- `md_locations_Add_Location` - Add new location
- `md_locations_Get_All` - Get all locations
- `md_locations_Update_Location` - Update existing location
- `md_locations_Delete_ByLocation` - Delete by location code

---

### `md_operation_numbers`
**Purpose**: Master data for valid manufacturing operation numbers.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ID | int(11) | PRIMARY KEY, AUTO_INCREMENT | Unique identifier |
| Operation | varchar(100) | NOT NULL, UNIQUE (uq_operation) | Operation number (stored as string) |
| IssuedBy | varchar(100) | NOT NULL | User who created this operation |

**Business Rules**:
- Operations are numeric values stored as strings (e.g., "90", "100", "110")
- Operation numbers follow MTM manufacturing sequence conventions
- Used to validate Operation fields in inventory and transactions

**Related Tables**: Referenced by `inv_inventory` and `inv_transaction`

**Stored Procedures**:
- `md_operation_numbers_Add_Operation` - Add new operation
- `md_operation_numbers_Get_All` - Get all operations
- `md_operation_numbers_Update_Operation` - Update existing operation
- `md_operation_numbers_Delete_ByOperation` - Delete by operation number

---

### `md_part_ids`
**Purpose**: Master data for parts including descriptions and operation sequences.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ID | int(11) | PRIMARY KEY, AUTO_INCREMENT | Unique identifier |
| PartID | varchar(300) | NOT NULL, UNIQUE (uq_item_number) | Part number/identifier |
| Customer | varchar(300) | NOT NULL | Customer associated with part |
| Description | varchar(300) | NOT NULL | Part description |
| IssuedBy | varchar(100) | NOT NULL | User who created this part |
| ItemType | varchar(100) | NOT NULL | Default item type for this part |
| Operations | json | NULL | JSON array of valid operation numbers for this part |

**Business Rules**:
- PartID must be unique across all customers
- Operations JSON contains array of operation numbers (e.g., ["90", "100", "110"])
- Customer field supports multi-customer environments
- Description provides human-readable part information
- ItemType sets default for inventory transactions

**Related Tables**:
- Referenced by `inv_inventory` and `inv_transaction`
- Operations array references `md_operation_numbers`

**Stored Procedures**:
- `md_part_ids_Add_Part` - Add new part
- `md_part_ids_Get_All` - Get all parts
- `md_part_ids_Get_ByItemNumber` - Get specific part
- `md_part_ids_Update_Part` - Update existing part
- `md_part_ids_Delete_ByItemNumber` - Delete by part number

---

### `sys_last_10_transactions`
**Purpose**: User-specific quick access buttons for frequently used transactions.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ID | int(11) | PRIMARY KEY, AUTO_INCREMENT | Unique identifier |
| User | varchar(100) | NOT NULL | User who owns this quick button |
| PartID | varchar(300) | NOT NULL | Part associated with quick button |
| Operation | varchar(100) | NOT NULL | Operation associated with quick button |
| Quantity | int(11) | NOT NULL | Default quantity for quick button |
| ReceiveDate | datetime | NOT NULL, DEFAULT CURRENT_TIMESTAMP | When button was last used/created |
| Position | int(11) | NOT NULL | Display position (1-10) |

**Indexes**:
- `idx_user_datetime` (User, ReceiveDate)

**Business Rules**:
- Each user can have up to 10 quick transaction buttons
- Position determines display order (1-based indexing)
- ReceiveDate tracks last usage for automatic ordering
- Used by UI to provide one-click transaction entry

**Related Tables**:
- `usr_users` - User validation
- `md_part_ids` - Part validation

**Stored Procedures**:
- `sys_last_10_transactions_Get_ByUser` - Get user's quick buttons
- `sys_last_10_transactions_AddQuickButton_1` - Add new quick button
- `sys_last_10_transactions_Update_ByUserAndDate` - Update existing button
- `sys_last_10_transactions_RemoveAndShift_ByUser` - Remove and reorder
- `sys_last_10_transactions_SwapPositions_ByUser` - Swap button positions
- `sys_last_10_transactions_Move_1` - Move button position
- `sys_last_10_transactions_MoveToLast_ByUser` - Move to end
- `sys_last_10_transactions_Reorder_ByUser` - Reorder buttons

---

### `sys_roles`
**Purpose**: Role-based access control system roles.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ID | int(11) | PRIMARY KEY, AUTO_INCREMENT | Unique role identifier |
| RoleName | varchar(50) | NOT NULL, UNIQUE (uq_rolename) | Role name |
| Description | varchar(255) | NULL | Role description |
| Permissions | varchar(1000) | NULL | Comma-separated permissions |
| IsSystem | tinyint(1) | NOT NULL, DEFAULT 0 | True for built-in system roles |
| CreatedBy | varchar(100) | NOT NULL | User who created this role |
| CreatedAt | datetime | NOT NULL, DEFAULT CURRENT_TIMESTAMP | When role was created |

**Business Rules**:
- System roles (IsSystem=1) cannot be deleted
- RoleName must be unique
- Permissions define what actions users with this role can perform
- Common roles: Admin, User, ReadOnly

**Related Tables**: Used by `sys_user_roles` for user assignments

**Stored Procedures**:
- `sys_roles_Get_ById` - Get role by ID

---

### `sys_user_roles`
**Purpose**: Assignment of roles to users for access control.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| UserID | int(11) | PRIMARY KEY, FOREIGN KEY to usr_users(ID) | User receiving the role |
| RoleID | int(11) | PRIMARY KEY, FOREIGN KEY to sys_roles(ID) | Role being assigned |
| AssignedBy | varchar(100) | NOT NULL | User who made the assignment |
| AssignedAt | datetime | NOT NULL, DEFAULT CURRENT_TIMESTAMP | When assignment was made |

**Indexes**:
- `idx_userid` (UserID)
- `idx_roleid` (RoleID)

**Foreign Key Constraints**:
- `sys_user_roles_ibfk_1`: UserID references usr_users(ID) ON DELETE CASCADE
- `sys_user_roles_ibfk_2`: RoleID references sys_roles(ID) ON DELETE CASCADE

**Business Rules**:
- Composite primary key ensures one role per user
- CASCADE deletes remove assignments when user or role is deleted
- AssignedBy provides audit trail for role assignments

**Stored Procedures**:
- `sys_user_roles_Add` - Assign role to user
- `sys_user_roles_Update` - Update user's role
- `sys_user_roles_Delete` - Remove role assignment

---

### `usr_ui_settings`
**Purpose**: User-specific UI preferences and customizations.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| UserId | varchar(64) | PRIMARY KEY, FOREIGN KEY to usr_users(User) | User identifier |
| SettingsJson | json | NOT NULL | UI settings (themes, layouts, preferences) |
| ShortcutsJson | json | NOT NULL | Keyboard shortcuts and quick actions |
| UpdatedAt | datetime | DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP | Last update timestamp |

**Foreign Key Constraints**:
- `usr_ui_settings_ibfk_1`: UserId references usr_users(User)

**Business Rules**:
- One settings record per user
- SettingsJson contains theme preferences, window layouts, etc.
- ShortcutsJson contains user-defined keyboard shortcuts
- UpdatedAt automatically tracks modifications

**Stored Procedures**:
- `usr_ui_settings_Get` - Get user's UI settings
- `usr_ui_settings_SetJsonSetting` - Update specific setting
- `usr_ui_settings_SetThemeJson` - Update theme settings
- `usr_ui_settings_SetShortcutsJson` - Update shortcuts
- `usr_ui_settings_GetShortcutsJson` - Get shortcuts only

---

### `usr_users`
**Purpose**: User account information and application preferences.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ID | int(11) | PRIMARY KEY, AUTO_INCREMENT | Unique user identifier |
| User | varchar(100) | NOT NULL, UNIQUE (uq_user) | Username/login identifier |
| Full Name | varchar(200) | NULL | User's full display name |
| Shift | varchar(50) | NOT NULL, DEFAULT '1' | Work shift assignment |
| VitsUser | tinyint(1) | NOT NULL, DEFAULT 0 | Integration flag for VITS system |
| Pin | varchar(50) | NULL | PIN for quick authentication |
| LastShownVersion | varchar(50) | NOT NULL, DEFAULT '0.0.0.0' | Last application version shown to user |
| HideChangeLog | varchar(50) | NOT NULL, DEFAULT 'false' | Whether to hide changelog display |
| Theme_Name | varchar(50) | NOT NULL, DEFAULT 'Default (Black and White)' | Selected theme name |
| Theme_FontSize | int(11) | NOT NULL, DEFAULT 9 | Font size preference |
| VisualUserName | varchar(50) | NOT NULL, DEFAULT 'User Name' | Display label for username field |
| VisualPassword | varchar(50) | NOT NULL, DEFAULT 'Password' | Display label for password field |
| WipServerAddress | varchar(15) | NOT NULL, DEFAULT '172.16.1.104' | Database server IP address |
| WIPDatabase | varchar(300) | NOT NULL, DEFAULT 'mtm_wip_application' | Database name |
| WipServerPort | varchar(10) | NOT NULL, DEFAULT '3306' | Database server port |

**Business Rules**:
- User field must be unique and serves as primary login identifier
- VitsUser flag indicates integration with legacy VITS system
- Theme preferences control application appearance
- Database connection settings allow per-user database targeting
- LastShownVersion prevents showing changelogs user has already seen

**Related Tables**:
- `sys_user_roles` - Role assignments
- `usr_ui_settings` - Extended UI preferences
- `sys_last_10_transactions` - Quick buttons

**Stored Procedures**:
- `usr_users_Add_User` - Add new user (also creates MySQL user)
- `usr_users_Get_All` - Get all users
- `usr_users_Get_ByUser` - Get specific user
- `usr_users_Update_User` - Update user information
- `usr_users_Delete_User` - Delete user (also removes MySQL user)
- `usr_users_Exists` - Check if user exists

---

## System Maintenance Procedures

### `maint_InsertMissingUserUiSettings`
Creates default UI settings for users missing them.

### `maint_reload_part_ids_and_operation_numbers`
Rebuilds master data from external systems.

### `migrate_user_roles_debug`
Debug utility for user role migration.

### `query_get_all_usernames_and_roles`
Query helper for user/role reporting.

---

## Relationships

### Primary Relationships
1. **Users and Roles**: `usr_users` ? `sys_user_roles` ? `sys_roles`
2. **Users and Settings**: `usr_users` ? `usr_ui_settings`
3. **Inventory and Transactions**: `inv_inventory` ? `inv_transaction` (via PartID, BatchNumber)
4. **Master Data Validation**:
   - `md_part_ids` validates PartID in inventory and transactions
   - `md_locations` validates Location fields
   - `md_operation_numbers` validates Operation fields
   - `md_item_types` validates ItemType fields

### Data Flow
1. **Inventory Receipt**: `inv_transaction` (IN) ? Updates `inv_inventory`
2. **Inventory Usage**: `inv_transaction` (OUT) ? Updates `inv_inventory`
3. **Inventory Transfer**: `inv_transaction` (TRANSFER) ? Updates `inv_inventory`
4. **Batch Tracking**: `inv_inventory_batch_seq` ? Assigns BatchNumber ? Links inventory and transactions

## Critical Business Rules

### Data Integrity
- All inventory changes must be recorded in `inv_transaction` via stored procedures
- BatchNumber provides complete traceability from receipt to usage
- Master data tables ensure referential integrity for codes and types

### Security and Auditing
- All transactions record the performing user
- Error logging captures all application issues via `log_error_Add_Error`
- Role-based security controls access to sensitive operations
- UI settings maintain user personalization without affecting business data

### Performance Considerations
- Indexes on high-query columns (PartID, User, dates)
- JSON columns for flexible configuration storage
- Batch processing for large inventory operations

### Database Access Requirements
- **NO DIRECT SQL** - All operations must use stored procedures
- Use `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()` pattern
- All new procedures developed in `Development/Database_Files/`
- Production procedures are in `Existing_Stored_Procedures.sql` (READ-ONLY)

This schema supports a complete work-in-process inventory management system with full traceability, user management, error tracking, and extensible configuration capabilities.