# Create CRUD Operations - Custom Prompt

## Instructions
Use this prompt when you need to create a complete set of Create, Read, Update, Delete operations for a new table or entity.

## Prompt Template

```
Create complete CRUD stored procedures for [TABLE_NAME/ENTITY_NAME] with the following requirements:

**Table Information**:
- **Table Name**: [table_name]
- **Primary Key**: [primary_key_column(s)]
- **Key Columns**: [list important columns]
- **Business Entity**: [description of what this table represents]

**Required CRUD Operations**:
1. **CREATE**: Add new [entity] record
2. **READ**: Get [entity] by various criteria
3. **UPDATE**: Modify existing [entity] record
4. **DELETE**: Remove [entity] record

**Specific Requirements**:

### Create Operation
- **Procedure Name**: [table]_Add_[Entity]
- **Parameters**: All required columns plus user and timestamp
- **Validation**: [List validation rules]
- **Business Rules**: [List business logic requirements]

### Read Operations
- **Get All**: [table]_Get_All
- **Get By ID**: [table]_Get_ById
- **Get By [Criteria]**: [table]_Get_By[Criteria] (specify criteria)
- **Search**: [table]_Search_By[SearchCriteria] (if needed)

### Update Operation
- **Procedure Name**: [table]_Update_[Entity]
- **Parameters**: ID plus fields to update
- **Validation**: [List validation rules]
- **Business Rules**: [List update restrictions]

### Delete Operation
- **Procedure Name**: [table]_Delete_[Entity]
- **Parameters**: ID plus user information
- **Validation**: [List delete restrictions]
- **Business Rules**: [Soft delete vs hard delete requirements]

**General Requirements**:
1. Add all procedures to Development/Database_Files/New_Stored_Procedures.sql
2. Include comprehensive error handling and transaction management
3. Validate all input parameters and business rules
4. Follow MTM database rules - NO direct SQL in application code
5. Include audit trail for all operations (created/modified by, timestamps)
6. Implement consistent error status codes and messages
7. Create corresponding service layer integration
8. Include comprehensive unit tests

**Security Requirements**:
- User validation for all operations
- Access control where appropriate
- Parameter sanitization
- Audit logging for sensitive operations

**Performance Requirements**:
- Use appropriate indexes for read operations
- Optimize for expected usage patterns
- Include pagination for large result sets
- Batch operations where applicable

Please provide:
1. Complete set of CRUD stored procedures
2. Documentation for each procedure
3. C# service layer integration examples
4. Unit test examples for each operation
5. Table relationship analysis
```

## Usage Examples

### Example 1: Equipment Management
```
Create complete CRUD stored procedures for equipment management with the following requirements:

**Table Information**:
- **Table Name**: sys_equipment
- **Primary Key**: ID (auto-increment)
- **Key Columns**: EquipmentNumber, Description, Location, Status, SerialNumber
- **Business Entity**: Manufacturing equipment and machinery

**Required CRUD Operations**:
1. **CREATE**: Add new equipment record with validation
2. **READ**: Get equipment by various criteria
3. **UPDATE**: Modify equipment details and status
4. **DELETE**: Remove equipment (soft delete with audit trail)

**Specific Requirements**:

### Create Operation
- **Procedure Name**: sys_equipment_Add_Equipment
- **Parameters**: EquipmentNumber, Description, Location, Status, SerialNumber, User, Notes
- **Validation**: 
  - EquipmentNumber must be unique
  - Location must exist in md_locations
  - Status must be valid (Active, Inactive, Maintenance, Retired)
  - User must exist in usr_users
- **Business Rules**: 
  - Auto-assign next available ID
  - Set created timestamp
  - Initial status defaults to 'Active'
  - Log creation in audit table

### Read Operations
- **Get All**: sys_equipment_Get_All (with optional status filter)
- **Get By ID**: sys_equipment_Get_ById
- **Get By Location**: sys_equipment_Get_ByLocation
- **Get By Status**: sys_equipment_Get_ByStatus
- **Search**: sys_equipment_Search_ByKeywords (search description, serial number)

### Update Operation
- **Procedure Name**: sys_equipment_Update_Equipment
- **Parameters**: ID, Description, Location, Status, SerialNumber, User, Notes
- **Validation**: 
  - ID must exist
  - Location must exist in md_locations
  - Status must be valid
  - User must exist in usr_users
- **Business Rules**: 
  - Update modified timestamp
  - Log changes in audit table
  - Validate status transitions
  - Cannot modify EquipmentNumber

### Delete Operation
- **Procedure Name**: sys_equipment_Delete_Equipment
- **Parameters**: ID, User, Reason
- **Validation**: 
  - ID must exist
  - Equipment must not be in use
  - User must have delete permissions
- **Business Rules**: 
  - Soft delete (set status to 'Deleted', don't remove record)
  - Log deletion in audit table
  - Check for dependencies before deletion

[Continue with detailed specifications...]
```

### Example 2: Customer Management
```
Create complete CRUD stored procedures for customer management with the following requirements:

**Table Information**:
- **Table Name**: md_customers
- **Primary Key**: ID (auto-increment)
- **Key Columns**: CustomerCode, CompanyName, ContactName, Email, Phone, Address
- **Business Entity**: Customer companies and contact information

**Required CRUD Operations**:
1. **CREATE**: Add new customer with complete validation
2. **READ**: Get customers by various search criteria
3. **UPDATE**: Modify customer information
4. **DELETE**: Remove customer (with dependency checking)

[Continue with specific requirements...]
```

## CRUD Patterns

### Standard Create Pattern
```sql
DELIMITER $$
CREATE PROCEDURE table_Add_Entity(
    IN p_Field1 VARCHAR(100),
    IN p_Field2 VARCHAR(100),
    IN p_User VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255),
    OUT p_NewID INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 @text = MESSAGE_TEXT;
        SET p_Status = -1;
        SET p_ErrorMsg = CONCAT('Database error in table_Add_Entity: ', @text);
    END;

    SET p_Status = 0;
    SET p_ErrorMsg = '';
    SET p_NewID = 0;

    -- Validate inputs
    IF p_Field1 IS NULL OR p_Field1 = '' THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Field1 is required';
    END IF;

    -- Check business rules
    IF EXISTS (SELECT 1 FROM table WHERE field1 = p_Field1) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Field1 already exists';
    END IF;

    IF p_Status = 0 THEN
        START TRANSACTION;
        
        INSERT INTO table (Field1, Field2, CreatedBy, CreatedAt)
        VALUES (p_Field1, p_Field2, p_User, NOW());
        
        SET p_NewID = LAST_INSERT_ID();
        
        COMMIT;
        SET p_ErrorMsg = CONCAT('Entity created successfully with ID: ', p_NewID);
    END IF;
END$$
DELIMITER ;
```

### Standard Read Pattern
```sql
DELIMITER $$
CREATE PROCEDURE table_Get_ByField(
    IN p_Field1 VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    SET p_Status = 0;
    SET p_ErrorMsg = '';

    -- Validate input
    IF p_Field1 IS NULL OR p_Field1 = '' THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Field1 parameter is required';
    ELSE
        SELECT COUNT(*) INTO v_Count FROM table WHERE Field1 = p_Field1;
        
        IF v_Count = 0 THEN
            SET p_Status = 1;
            SET p_ErrorMsg = 'No records found';
        ELSE
            SELECT * FROM table WHERE Field1 = p_Field1;
            SET p_ErrorMsg = CONCAT('Retrieved ', v_Count, ' record(s)');
        END IF;
    END IF;
END$$
DELIMITER ;
```

### Standard Update Pattern
```sql
DELIMITER $$
CREATE PROCEDURE table_Update_Entity(
    IN p_ID INT,
    IN p_Field1 VARCHAR(100),
    IN p_Field2 VARCHAR(100),
    IN p_User VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 @text = MESSAGE_TEXT;
        SET p_Status = -1;
        SET p_ErrorMsg = CONCAT('Database error in table_Update_Entity: ', @text);
    END;

    SET p_Status = 0;
    SET p_ErrorMsg = '';

    -- Validate inputs
    IF p_ID IS NULL OR p_ID <= 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Valid ID is required';
    END IF;

    -- Check if record exists
    SELECT COUNT(*) INTO v_Count FROM table WHERE ID = p_ID;
    IF v_Count = 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Record with ID ', p_ID, ' not found');
    END IF;

    IF p_Status = 0 THEN
        START TRANSACTION;
        
        UPDATE table 
        SET Field1 = p_Field1,
            Field2 = p_Field2,
            ModifiedBy = p_User,
            ModifiedAt = NOW()
        WHERE ID = p_ID;
        
        COMMIT;
        SET p_ErrorMsg = CONCAT('Record with ID ', p_ID, ' updated successfully');
    END IF;
END$$
DELIMITER ;
```

### Standard Delete Pattern
```sql
DELIMITER $$
CREATE PROCEDURE table_Delete_Entity(
    IN p_ID INT,
    IN p_User VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    DECLARE v_HasDependencies INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 @text = MESSAGE_TEXT;
        SET p_Status = -1;
        SET p_ErrorMsg = CONCAT('Database error in table_Delete_Entity: ', @text);
    END;

    SET p_Status = 0;
    SET p_ErrorMsg = '';

    -- Validate inputs
    IF p_ID IS NULL OR p_ID <= 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Valid ID is required';
    END IF;

    -- Check if record exists
    SELECT COUNT(*) INTO v_Count FROM table WHERE ID = p_ID;
    IF v_Count = 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Record with ID ', p_ID, ' not found');
    END IF;

    -- Check for dependencies
    SELECT COUNT(*) INTO v_HasDependencies FROM dependent_table WHERE table_id = p_ID;
    IF v_HasDependencies > 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Cannot delete record: ', v_HasDependencies, ' dependent records exist');
    END IF;

    IF p_Status = 0 THEN
        START TRANSACTION;
        
        -- Soft delete preferred
        UPDATE table 
        SET IsDeleted = 1,
            DeletedBy = p_User,
            DeletedAt = NOW()
        WHERE ID = p_ID;
        
        -- OR hard delete if appropriate
        -- DELETE FROM table WHERE ID = p_ID;
        
        COMMIT;
        SET p_ErrorMsg = CONCAT('Record with ID ', p_ID, ' deleted successfully');
    END IF;
END$$
DELIMITER ;
```

## Service Layer Integration

### Service Class Example
```csharp
public class EntityService
{
    private readonly DatabaseService _databaseService;

    public EntityService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<Result<int>> AddEntityAsync(EntityModel entity, string user)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_Field1"] = entity.Field1,
            ["p_Field2"] = entity.Field2,
            ["p_User"] = user
        };

        var result = await _databaseService.ExecuteStoredProcedureAsync(
            "table_Add_Entity", parameters);

        if (result.Success)
        {
            var newId = (int)result.OutputParameters["p_NewID"];
            return Result<int>.Success(newId);
        }
        else
        {
            return Result<int>.Failure(result.ErrorMessage);
        }
    }

    public async Task<Result<List<EntityModel>>> GetAllEntitiesAsync()
    {
        var result = await _databaseService.ExecuteStoredProcedureAsync(
            "table_Get_All", new Dictionary<string, object>());

        if (result.Success)
        {
            var entities = MapDataTableToEntities(result.Data);
            return Result<List<EntityModel>>.Success(entities);
        }
        else
        {
            return Result<List<EntityModel>>.Failure(result.ErrorMessage);
        }
    }

    // Additional CRUD methods...
}
```

## Testing Patterns

### Unit Test Example
```csharp
[TestClass]
public class EntityCrudTests
{
    [Test]
    public async Task AddEntity_ShouldReturnSuccess_WhenValidData()
    {
        // Arrange
        var parameters = new Dictionary<string, object>
        {
            ["p_Field1"] = "TestValue1",
            ["p_Field2"] = "TestValue2",
            ["p_User"] = "test_user"
        };

        // Act
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            connectionString, "table_Add_Entity", parameters);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.OutputParameters["p_Status"]);
        Assert.IsTrue((int)result.OutputParameters["p_NewID"] > 0);
    }

    [Test]
    public async Task GetEntity_ShouldReturnData_WhenEntityExists()
    {
        // Test implementation...
    }

    // Additional test methods for all CRUD operations...
}
```

## Related Files
- Add procedures to: `Development/Database_Files/New_Stored_Procedures.sql`
- Document in: `Development/Database_Files/README_New_Stored_Procedures.md`
- Service classes: Create or update entity service classes
- Models: Create/update entity model classes
- Tests: Create comprehensive unit tests for all operations