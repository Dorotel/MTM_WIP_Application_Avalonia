# MTM Manufacturing Business Domain Context

## 🏭 Manufacturing Domain Knowledge

### **Core Business Concepts**

#### **Inventory Management System**
The MTM WIP Application manages Work-In-Progress (WIP) inventory for manufacturing operations:

- **Parts**: Physical components identified by unique PartIDs
- **Operations**: Workflow steps (represented as numbers: "90", "100", "110", "120")
- **Locations**: Physical storage areas in manufacturing facility
- **Batches**: Groups of identical parts processed together
- **Transactions**: Movement records (IN/OUT/TRANSFER)

#### **Transaction Types and Business Logic**
```
Transaction Type Determination (CRITICAL):
- IN: User is ADDING inventory to system
- OUT: User is REMOVING inventory from system  
- TRANSFER: User is MOVING inventory between locations

Operations are workflow steps, NOT transaction indicators:
- Operation "90" ≠ IN transaction
- Operation "100" ≠ OUT transaction
- Transaction type determined by USER INTENT, not operation number
```

### **Business Rules**

#### **Inventory Rules**
- Every inventory item must have: PartID, Location, Operation, Quantity
- Quantities are always positive integers (no decimals in manufacturing)
- Batch numbers track individual groups through manufacturing process
- Items can only exist in one location at a time per operation

#### **Operation Workflow**
- Operations represent stages in manufacturing process
- Common operations: "90" (Raw Materials), "100" (In Process), "110" (Quality Check), "120" (Finished)
- Parts move through operations sequentially in manufacturing workflow
- Each operation may have different locations and requirements

#### **User Permissions**
- Regular users: Can view and perform basic inventory operations
- Supervisors: Can modify and approve transactions
- Administrators: Can manage master data (parts, locations, operations)

### **Manufacturing Scenarios**

#### **Receiving Materials (IN Transaction)**
User receives raw materials into inventory:
```
Business Intent: Add inventory
Transaction Type: IN
Example: Receive 100 units of PART001 at operation "90" in location "RECEIVING"
```

#### **Consuming Materials (OUT Transaction)**  
User removes inventory for production use:
```
Business Intent: Remove inventory
Transaction Type: OUT
Example: Issue 50 units of PART001 from operation "90" to production floor
```

#### **Moving Between Locations (TRANSFER Transaction)**
User moves inventory without changing operation:
```
Business Intent: Move inventory
Transaction Type: TRANSFER
Example: Move 25 units of PART001 from "RECEIVING" to "STORAGE" at same operation "90"
```

#### **Advancing Through Operations**
User moves parts to next manufacturing stage:
```
Business Intent: Workflow progression
Transaction Type: Combination of OUT (from current operation) + IN (to next operation)
Example: Move PART001 from operation "90" to operation "100"
```

### **Data Relationships**

#### **Master Data Dependencies**
```
Parts (md_part_ids)
├── ItemNumber (primary identifier)
├── Customer (who ordered the part)
├── Description (what the part is)
├── ItemType (classification)
└── IssuedBy (who created the part record)

Locations (md_locations)  
├── Location (primary identifier)
├── Building (physical building)
└── IssuedBy (who created location)

Operations (md_operation_numbers)
├── Operation (primary identifier, usually numeric string)
└── IssuedBy (who created operation)

ItemTypes (md_item_types)
├── ItemType (primary identifier)
└── IssuedBy (who created type)
```

#### **Transaction Logging**
Every inventory change creates transaction record:
```
Transaction Log (inv_transaction)
├── TransactionType (IN/OUT/TRANSFER)
├── PartID (what part)
├── BatchNumber (which batch)
├── FromLocation (source location, null for IN)
├── ToLocation (destination location)
├── Operation (workflow stage)
├── Quantity (how many)
├── User (who performed action)
├── Timestamp (when performed)
└── Notes (additional context)
```

### **Common Business Workflows**

#### **Daily Manufacturing Flow**
1. **Morning Setup**: Check inventory levels for daily production
2. **Material Issue**: Issue raw materials to production (OUT transactions)
3. **Work-in-Progress**: Track parts moving through operations
4. **Quality Control**: Verify parts at inspection operations
5. **Finished Goods**: Receive completed parts into finished inventory (IN transactions)
6. **Shipping**: Issue finished goods to customers (OUT transactions)

#### **Inventory Reconciliation**
1. **Physical Count**: Count actual inventory in locations
2. **System Comparison**: Compare physical count to system records
3. **Adjustment**: Create adjustment transactions for discrepancies
4. **Approval**: Supervisor approval for adjustments above threshold

#### **Parts Tracking**
1. **Receipt**: Parts received with batch numbers
2. **Lot Tracking**: Batch numbers maintained through all operations
3. **Traceability**: Ability to track parts from raw material to finished goods
4. **Quality Issues**: Isolate and track defective batches

### **Error Scenarios and Handling**

#### **Common Business Errors**
- **Insufficient Inventory**: User tries to remove more than available
- **Invalid Location**: User specifies location that doesn't exist
- **Wrong Operation**: User tries to skip operations in workflow
- **Duplicate Batch**: User tries to create duplicate batch number

#### **Business Validation Rules**
- Quantity must be positive
- Location must exist in master data
- Operation must exist in master data
- Part must exist in master data
- User must have permission for operation type

### **Integration Points**

#### **External Systems**
- **ERP System**: Master data synchronization
- **Quality System**: Defect tracking and batch holds
- **Shipping System**: Order fulfillment and logistics
- **Production Planning**: Demand forecasting and scheduling

#### **Reporting Requirements**
- **Inventory Levels**: Current stock by part/location/operation
- **Transaction History**: All movements for audit trail
- **Performance Metrics**: Inventory accuracy, throughput rates
- **Exception Reports**: Negative inventory, aged stock, discrepancies