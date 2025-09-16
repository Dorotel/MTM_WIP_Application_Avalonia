# Reference - Inventory - 259 Pages !

*Converted from PDF*

---

  
Infor VISUAL API Toolkit Inventory 
Class Library Reference 
 
 
  
 Copyright © 2024 Infor  
 
Important Notices  
The material contained in this publication (including any supplementary information) constitutes and 
contains confidential and proprietary information of Infor.  
By gaining access to the attached, you acknowledge and agree that the material (including any 
modification, translation or adaptation of the material) and all copyright, trade secrets and all other 
right, title and interest therein, are the sole property of Infor and that you shall not gain right, title or 
interest in the material (including any modification, translation or adaptation of the material) by virtue 
of your review thereof other than the non- exclusive right to use the material solely in connection with 
and the furtherance of your license and use of software made available to your company from Infor 
pursuant to a separate agreement, the terms of which separate agreement shall govern your use of 
this material and all supplemental related materials ( "Purpose").  
In addition, by accessing the enclosed material, you acknowledge and agree that you are required to maintain such material in strict confidence and that your use of such material is limited to the 
Purpose described above. Although Infor has taken due care to ensure that the material included in 
this publication is accurate and complete, Infor cannot warrant that the information contained in this 
publication is complete, does not contain typographical or other errors, or will meet your specific 
requirements.  As such, Infor does not assume and hereby disclaims all liability, consequential or 
otherwise, for any loss or damage to any person or entity which is caused by or relates to errors or 
omissions in this publication (including any supplementary information), whether such errors or 
omissions result from negligence, accident or any other cause.  
Without limitation, U.S. export control laws and other applicable export and import laws govern your 
use of this material and you will neither export or re- export, directly or indirectly, this material nor any 
related materials or supplemental information in violation of such laws, or use such materials for any 
purpose prohibited by such laws.  
Trademark Acknowledgements  
The word and design marks set forth herein are trademarks and/or registered trademarks of Infor 
and/or related affiliates and subsidiaries. All rights reserved. All other company, product, trade or 
service names referenced may be registered trademarks or t rademarks of their respective owners.  
Publication Information 
Release: Infor VISUAL API Toolkit    
Publication date: August 13, 2024  
 
  
Infor VISUAL API Toolkit  Inventory Class Library Reference  | 3 About this guide  
This guide describes  the objects available in the Infor VISUAL API Toolkit Trace class library.  
Intended audience  
The intended audience of this guide is developers who are using the API Toolkit to extend the 
VISUAL solution.  
Contacting Support  
If you have questions about Infor products, go to the Infor Customer Portal  at 
https://customerportal.infor.com/csmcore / 
If we update this document after the product release, we will post the new version on this Web site. 
We recommend that you check this Web site periodically for updated documentation.  
If you have comments about Infor documentation, contact https://docs.infor.com/en -us. 
Supported languages  
These languages are supported for use with the toolkit:  
• Visual Basic 
• C# 
While it is possible to use any .NET -aware programming language with the toolkit, other languages 
are not officially supported.  
Support information  
4 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Support information  
The API Toolkit will be updated regularly as more class members are added to each assembly, 
schema changes are made, and any reported issues are resolved. Infor Support cannot assist you 
with developing customized code using the API Toolkit. For assistance with customizations, contact 
Infor Consulting Services or your channel partner.  
The functionality provided within the API Toolkit will not be extended beyond the standard 
functionality experienced in the VISUAL application itself. Enhancement requests with compelling 
business cases detailing how suggested alternatives are not viable w ill be evaluated and 
considered.  
Infor is not responsible for data incorrectly entered to the database through the use of the API 
Toolkit. Customers must establish a full test environment to ensure that data created by APIs 
functions in the same manner as data created through the VISUAL i nterface.  
  
Lsa.Vmfg.Inventory Namespace  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 5 Lsa.Vmfg.Inventory Namespace  
  
Classes 
 Class  Description  
 AdjReasons  Maintain Adjustment Reason Codes.  
 HoldReasons  Maintain Hold Reason Codes.  
 Ibt Maintain Interbranch Transfers.  
 IbtReceipt  Transaction to perform Interbranch Transfer receipts.  
 IbtShipment  Transaction to perform Interbranch Transfer Shipments.  
 InventoryTransaction  Inventory Transaction.  
 IssueReasons  Maintain Issue Reason Codes.  
 Location  Maintain Warehouse Locations.  
 Part Maintain Parts.  
 PartAliasTypes  Maintain Part Alias Types.  
 Warehouse  Maintain Warehouses.  
 
  

AdjReasons Class  
6 | Infor VISUAL API Toolkit  Inventory Class Library Reference  AdjReasons Class  
Maintain Adjustment Reason Codes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Inventory.AdjReasons  
 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  AdjReasons  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  AdjReasons  
 Inherits  BusinessDocument  
 
The AdjReasons type exposes the following members.  
Constructors 
 Name  Description  
 AdjReasons()  Constructor  
 AdjReasons(String)  Constructor  
 

AdjReasons Class  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 7 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Adjustment Reason Codes based on search 
criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Adjustment Reason Codes based on search criteria, limited by record count.  
 Exists  Determines if a specific Adjustment Reason Code exists.  
 Find Retrieves a specific Adjustment Reason Code.  
 Load()  Load all Adjustment Reason Codes.  
 Load(String)  Load a specific Adjustment Reason Code.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewAdjReasonRow  Inserts a new row into the ADJ_REASON data table.  
 Save  Save all previously loaded Adjustment Reason Codes to the database.  
 
See Also 
Lsa.Vmfg.Inventory Namespace  
  

AdjReasons Constructor  
8 | Infor VISUAL API Toolkit  Inventory Class Library Reference  AdjReasons Constructor  
Overload List 
 Name  Description  
 AdjReasons()  Constructor  
 AdjReasons(String)  Constructor  
 
See Also 
AdjReasons Class  
Lsa.Vmfg.Inventory Namespace  
  

AdjReasons Constructor  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 9 AdjReasons Constructor  
Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  AdjReasons () 
 
VB 
Public  Sub New 
 
See Also 
AdjReasons Class  
AdjReasons Overload  
Lsa.Vmfg.Inventory Namespace  
  
AdjReasons Constructor (String)  
10 | Infor VISUAL API Toolkit  Inventory Class Library Reference  AdjReasons Constructor (String)  
Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  AdjReasons ( 
 string  databaseInstanceName  
) 
 
VB 
Public  Sub New (  
 databaseInstanceName As String  
) 
 
Parameters  
databaseInstanceName  
Type: System.String  
See Also 
AdjReasons Class  
AdjReasons Overload  
Lsa.Vmfg.Inventory Namespace  
  
AdjReasons.AdjReasons Methods  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 11 AdjReasons.AdjReasons Methods  
The AdjReasons  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Adjustment Reason Codes based on search 
criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Adjustment Reason Codes based on search criteria, limited by record count.  
 Exists  Determines if a specific Adjustment Reason Code exists.  
 Find Retrieves a specific Adjustment Reason Code.  
 Load()  Load all Adjustment Reason Codes.  
 Load(String)  Load a specific Adjustment Reason Code.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewAdjReasonRow  Inserts a new row into the ADJ_REASON data table.  
 Save  Save all previously loaded Adjustment Reason Codes to the database.  
 
See Also 
AdjReasons Class  
Lsa.Vmfg.Inventory Namespace  
  

AdjReasons.Browse Method  
12 | Infor VISUAL API Toolkit  Inventory Class Library Reference  AdjReasons.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Adjustment Reason Codes based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Adjustment Reason Codes based on search criteria, 
limited by record count.  
 
See Also 
AdjReasons Class  
Lsa.Vmfg.Inventory Namespace  
  

AdjReasons.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 13 AdjReasons.Browse Method (String, String, String)  
Retrieve Adjustment Reason Codes based on search criteria.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataSet  Browse ( 
 string  columnNames , 
 string  searchCondition, 
 string  sortColumns 
) 
 
VB 
Public  Overridable  Function Browse  (  
 columnNames  As String , 
 searchCondition  As String , 
 sortColumns  As String 
) As DataSet  
 
Parameters  
columnNames 
Type: System.String  
searchCondition Type: System.String
 
sortColumns  
Type: System.String  
Return Value  
Type: DataSet  
See Also 
AdjReasons Class  
AdjReasons.Browse Method (String, String, String)  
14 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Browse Overload  
Lsa.Vmfg.Inventory Namespace  
  
AdjReasons.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 15 AdjReasons.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve Adjustment Reason Codes based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataSet  Browse ( 
 string  columnNames , 
 string  searchCondition, 
 string  sortColumns , 
 int startRecord, 
 int maxRecords  
) 
 
VB 
Public  Overridable  Function Browse  (  
 columnNames  As String , 
 searchCondition  As String , 
 sortColumns  As String , 
 startRecord As Integer , 
 maxRecords  As Integer  
) As DataSet  
 
Parameters  
columnNames 
Type: System.String  
searchCondition Type: System.String
 
sortColumns  
Type: System.String  
startRecord  
Type: System.Int32  
maxRecords  
Type: System.Int32  
AdjReasons.Browse Method (String, String, String, Int32, Int32)  
16 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Return Value  
Type: DataSet  
See Also 
AdjReasons Class  
Browse Overload  
Lsa.Vmfg.Inventory Namespace  
  
AdjReasons.Exists Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 17 AdjReasons.Exists Method  
Determines if a specific Adjustment Reason Code exists.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  id 
) 
 
VB 
Public  Overridable  Function Exists  (  
 id As String 
) As Boolean 
 
Parameters  
id 
Type: System.String  
Return Value  
Type: Boolean  
See Also 
AdjReasons Class  
Lsa.Vmfg.Inventory Namespace  
  
AdjReasons.Find Method 
18 | Infor VISUAL API Toolkit  Inventory Class Library Reference  AdjReasons.Find Method  
Retrieves a specific Adjustment Reason Code.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  id 
) 
 
VB 
Public  Overridable  Sub Find (  
 id As String 
) 
 
Parameters  
id 
Type: System.String  
See Also 
AdjReasons Class  
Lsa.Vmfg.Inventory Namespace  
  
AdjReasons.Load Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 19 AdjReasons.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Adjustment Reason Codes.  
 Load(String)  Load a specific Adjustment Reason Code.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
AdjReasons Class  
Lsa.Vmfg.Inventory Namespace  
  

AdjReasons.Load Method  
20 | Infor VISUAL API Toolkit  Inventory Class Library Reference  AdjReasons.Load Method  
Load all Adjustment Reason Codes.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
AdjReasons Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
AdjReasons.Load Method (String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 21 AdjReasons.Load Method (String)  
Load a specific Adjustment Reason Code.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  id 
) 
 
VB 
Public  Overridable  Sub Load (  
 id As String 
) 
 
Parameters  
id 
Type: System.String  
See Also 
AdjReasons Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
AdjReasons.Load Method (Stream, String)  
22 | Infor VISUAL API Toolkit  Inventory Class Library Reference  AdjReasons.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  id 
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 id As String 
) 
 
Parameters  
stream  
Type: System.IO.Stream  
id 
Type: System.String  
See Also 
AdjReasons Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
AdjReasons.NewAdjReasonRow Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 23 AdjReasons.NewAdjReasonRow Method  
Inserts a new row into the ADJ_REASON data table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewAdjReasonRow ( 
 string  id 
) 
 
VB 
Public  Function  NewAdjReasonRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
AdjReasons Class  
Lsa.Vmfg.Inventory Namespace  
  
AdjReasons.Save Method  
24 | Infor VISUAL API Toolkit  Inventory Class Library Reference  AdjReasons.Save Method  
Save all previously loaded Adjustment Reason Codes to the database.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
AdjReasons Class  
Lsa.Vmfg.Inventory Namespace  
  
HoldReasons Class  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 25 HoldReasons Class  
Maintain Hold Reason Codes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Inventory.HoldReasons  
 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  HoldReasons  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  HoldReasons  
 Inherits  BusinessDocument  
 
The HoldReasons  type exposes the following members.  
Constructors 
 Name  Description  
 HoldReasons()  Constructor  
 HoldReasons(String)  Constructor  
 

HoldReasons Class  
26 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Hold Reason Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Hold Reason Codes based on search criteria, 
limited by record count.  
 Exists  Determines if a specific Hold Reason Code exists.  
 Find Retrieves a specific Hold Reason Code.  
 Load()  Load all Hold Reason Codes.  
 Load(String)  Load a specific Hold Reason Code.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewHoldReasonRow  Inserts a new row into the HOLD_REASON table.  
 Save  Save all previously loaded Hold Reason Codes to the database.  
 
See Also 
Lsa.Vmfg.Inventory Namespace  
  

HoldReasons Constructor  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 27 HoldReasons Constructor  
Overload List 
 Name  Description  
 HoldReasons()  Constructor  
 HoldReasons(String)  Constructor  
 
See Also 
HoldReasons Class  
Lsa.Vmfg.Inventory Namespace  
  

HoldReasons Constructor  
28 | Infor VISUAL API Toolkit  Inventory Class Library Reference  HoldReasons Constructor  
Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  HoldReasons () 
 
VB 
Public  Sub New 
 
See Also 
HoldReasons Class  
HoldReasons Overload  
Lsa.Vmfg.Inventory Namespace  
  
HoldReasons Constructor (String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 29 HoldReasons Constructor (String)  
Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  HoldReasons ( 
 string  databaseInstanceName  
) 
 
VB 
Public  Sub New (  
 databaseInstanceName As String  
) 
 
Parameters  
databaseInstanceName  
Type: System.String  
See Also 
HoldReasons Class  
HoldReasons Overload  
Lsa.Vmfg.Inventory Namespace  
  
HoldReasons.HoldReasons Methods  
30 | Infor VISUAL API Toolkit  Inventory Class Library Reference  HoldReasons.HoldReasons Methods  
The HoldReasons  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Hold Reason Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Hold Reason Codes based on search criteria, 
limited by record count.  
 Exists  Determines if a specific Hold Reason Code exists.  
 Find Retrieves a specific Hold Reason Code.  
 Load()  Load all Hold Reason Codes.  
 Load(String)  Load a specific Hold Reason Code.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewHoldReasonRow  Inserts a new row into the HOLD_REASON table.  
 Save  Save all previously loaded Hold Reason Codes to the database.  
 
See Also 
HoldReasons Class  
Lsa.Vmfg.Inventory Namespace  
  

HoldReasons.Browse Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 31 HoldReasons.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Hold Reason Codes based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Hold Reason Codes based on search criteria, 
limited by record count.  
 
See Also 
HoldReasons Class  
Lsa.Vmfg.Inventory Namespace  
  

HoldReasons.Browse Method (String, String, String)  
32 | Infor VISUAL API Toolkit  Inventory Class Library Reference  HoldReasons.Browse Method (String, String, String)  
Retrieve Hold Reason Codes based on search criteria.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataSet  Browse ( 
 string  columnNames , 
 string  searchCondition, 
 string  sortColumns 
) 
 
VB 
Public  Overridable  Function Browse  (  
 columnNames  As String , 
 searchCondition  As String , 
 sortColumns  As String 
) As DataSet  
 
Parameters  
columnNames 
Type: System.String  
searchCondition Type: System.String
 
sortColumns  
Type: System.String  
Return Value  
Type: DataSet  
See Also 
HoldReasons Class  
HoldReasons.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 33 Browse Overload  
Lsa.Vmfg.Inventory Namespace  
  
HoldReasons.Browse Method (String, String, String, Int32, Int32)  
34 | Infor VISUAL API Toolkit  Inventory Class Library Reference  HoldReasons.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve Hold Reason Codes based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataSet  Browse ( 
 string  columnNames , 
 string  searchCondition, 
 string  sortColumns , 
 int startRecord, 
 int maxRecords  
) 
 
VB 
Public  Overridable  Function Browse  (  
 columnNames  As String , 
 searchCondition  As String , 
 sortColumns  As String , 
 startRecord As Integer , 
 maxRecords  As Integer  
) As DataSet  
 
Parameters  
columnNames 
Type: System.String  
searchCondition Type: System.String
 
sortColumns  
Type: System.String  
startRecord  
Type: System.Int32  
maxRecords  
Type: System.Int32  
HoldReasons.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 35 Return Value  
Type: DataSet  
See Also 
HoldReasons Class  
Browse Overload  
Lsa.Vmfg.Inventory Namespace  
  
HoldReasons.Exists Method 
36 | Infor VISUAL API Toolkit  Inventory Class Library Reference  HoldReasons.Exists Method  
Determines if a specific Hold Reason Code exists.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  id 
) 
 
VB 
Public  Overridable  Function Exists  (  
 id As String 
) As Boolean 
 
Parameters  
id 
Type: System.String  
Return Value  
Type: Boolean  
See Also 
HoldReasons Class  
Lsa.Vmfg.Inventory Namespace  
  
HoldReasons.Find Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 37 HoldReasons.Find Method  
Retrieves a specific Hold Reason Code.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  id 
) 
 
VB 
Public  Overridable  Sub Find (  
 id As String 
) 
 
Parameters  
id 
Type: System.String  
See Also 
HoldReasons Class  
Lsa.Vmfg.Inventory Namespace  
  
HoldReasons.Load Method  
38 | Infor VISUAL API Toolkit  Inventory Class Library Reference  HoldReasons.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Hold Reason Codes.  
 Load(String)  Load a specific Hold Reason Code.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
HoldReasons Class  
Lsa.Vmfg.Inventory Namespace  
  

HoldReasons.Load Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 39 HoldReasons.Load Method  
Load all Hold Reason Codes.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
HoldReasons Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
HoldReasons.Load Method (String)  
40 | Infor VISUAL API Toolkit  Inventory Class Library Reference  HoldReasons.Load Method (String)  
Load a specific Hold Reason Code.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  id 
) 
 
VB 
Public  Overridable  Sub Load (  
 id As String 
) 
 
Parameters  
id 
Type: System.String  
See Also 
HoldReasons Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
HoldReasons.Load Method (Stream, String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 41 HoldReasons.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  id 
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 id As String 
) 
 
Parameters  
stream  
Type: System.IO.Stream  
id 
Type: System.String  
See Also 
HoldReasons Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
HoldReasons.NewHoldReasonRow Method  
42 | Infor VISUAL API Toolkit  Inventory Class Library Reference  HoldReasons.NewHoldReasonRow Method  
Inserts a new row into the HOLD_REASON table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewHoldReasonRow ( 
 string  id 
) 
 
VB 
Public  Function  NewHoldReasonRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
HoldReasons Class  
Lsa.Vmfg.Inventory Namespace  
  
HoldReasons.Save Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 43 HoldReasons.Save Method  
Save all previously loaded Hold Reason Codes to the database.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
HoldReasons Class  
Lsa.Vmfg.Inventory Namespace  
  
Ibt Class  
44 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Ibt Class  
Maintain Interbranch Transfers.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Inventory.Ibt  
 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  Ibt : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  Ibt 
 Inherits  BusinessDocument  
 
The Ibt type exposes the following members.  
Constructors 
 Name  Description  
 Ibt() Default constructor  
 Ibt(String)  Business Document Constructor  
 

Ibt Class  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 45 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve IBTs based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve IBTs based on search criteria, limited by the values in 
the startRecord and maxRecords parameters.  
 Exists  Checks for the existance of a specific IBT.  
 Find Retrieve a specific IBT. Only returns the top- level data table 
(IBT).  
 Load(String)  Loads a specific IBT.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewIbtBinaryRow  Inserts a new row into the IBT_BINARY table. Only the binary type "D" (long text) is supported.  
 NewIbtLineBinaryRow  Inserts a new row into the IBT_LINE_BINARY table. Only the binary type "D" (long text) is supported.  
 NewIbtLineRow(String)  Inserts a new row into the IBT_LINE table. Auto Numbers the Line Row.  
 NewIbtLineRow(String, Int64)  Inserts a new row into the IBT_LINE table.  
 NewIbtLineTrcCtlRow  Inserts a new row into the IBT_LINE_TRC_CTL table.  
 NewIbtRow(String)  Inserts a new row into the IBT table.  
 NewIbtRow(String, String)  Inserts a new row into the IBT table for a specified Site ID.  
 Save()  Saves all previously loaded IBTs to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
See Also 
Lsa.Vmfg.Inventory Namespace  
  

Ibt Constructor  
46 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Ibt Constructor  
Overload List 
 Name  Description  
 Ibt() Default constructor  
 Ibt(String)  Business Document Constructor  
 
See Also 
Ibt Class  
Lsa.Vmfg.Inventory Namespace  
  

Ibt Constructor  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 47 Ibt Constructor  
Default constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Ibt() 
 
VB 
Public  Sub New 
 
See Also 
Ibt Class  
Ibt Overload  
Lsa.Vmfg.Inventory Namespace  
  
Ibt Constructor (String)  
48 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Ibt Constructor (String)  
Business Document Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Ibt( 
 string  databaseInstanceName  
) 
 
VB 
Public  Sub New (  
 databaseInstanceName As String  
) 
 
Parameters  
databaseInstanceName  
Type: System.String  
See Also 
Ibt Class  
Ibt Overload  
Lsa.Vmfg.Inventory Namespace  
  
Ibt.Ibt Methods  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 49 Ibt.Ibt Methods  
The Ibt type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve IBTs based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve IBTs based on search criteria, limited by the values in 
the startRecord and maxRecords parameters.  
 Exists  Checks for the existance of a specific IBT.  
 Find Retrieve a specific IBT. Only returns the top- level data table 
(IBT).  
 Load(String)  Loads a specific IBT.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewIbtBinaryRow  Inserts a new row into the IBT_BINARY table. Only the binary type "D" (long text) is supported.  
 NewIbtLineBinaryRow  Inserts a new row into the IBT_LINE_BINARY table. Only the binary type "D" (long text) is supported.  
 NewIbtLineRow(String)  Inserts a new row into the IBT_LINE table. Auto Numbers the Line Row.  
 NewIbtLineRow(String, Int64)  Inserts a new row into the IBT_LINE table.  
 NewIbtLineTrcCtlRow  Inserts a new row into the IBT_LINE_TRC_CTL table.  
 NewIbtRow(String)  Inserts a new row into the IBT table.  
 NewIbtRow(String, String)  Inserts a new row into the IBT table for a specified Site ID.  
 Save()  Saves all previously loaded IBTs to the database.  
 Save(Stream)  Save current state of data set to stream.  
 

Ibt.Ibt Methods  
50 | Infor VISUAL API Toolkit  Inventory Class Library Reference  See Also 
Ibt Class  
Lsa.Vmfg.Inventory Namespace  
  
Ibt.Browse Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 51 Ibt.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve IBTs based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve IBTs based on search criteria, limited by the values in 
the startRecord and maxRecords parameters.  
 
See Also 
Ibt Class  
Lsa.Vmfg.Inventory Namespace  
  

Ibt.Browse Method (String, String, String)  
52 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Ibt.Browse Method (String, String, String)  
Retrieve IBTs based on search criteria.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataSet  Browse ( 
 string  columnNames , 
 string  searchCondition, 
 string  sortColumns 
) 
 
VB 
Public  Overridable  Function Browse  (  
 columnNames  As String , 
 searchCondition  As String , 
 sortColumns  As String 
) As DataSet  
 
Parameters  
columnNames 
Type: System.String  
searchCondition Type: System.String
 
sortColumns  
Type: System.String  
Return Value  
Type: DataSet  
See Also 
Ibt Class  
Ibt.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 53 Browse Overload  
Lsa.Vmfg.Inventory Namespace  
  
Ibt.Browse Method (String, String, String, Int32, Int32)  
54 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Ibt.Browse Method (String, String, String, Int32, 
Int32)  
Retrieve IBTs based on search criteria, limited by the values in the startRecord and maxRecords 
parameters.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataSet  Browse ( 
 string  columnNames , 
 string  searchCondition, 
 string  sortColumns , 
 int startRecord, 
 int maxRecords  
) 
 
VB 
Public  Overridable  Function Browse  (  
 columnNames  As String , 
 searchCondition  As String , 
 sortColumns  As String , 
 startRecord As Integer , 
 maxRecords  As Integer  
) As DataSet  
 
Parameters  
columnNames 
Type: System.String  
searchCondition Type: System.String
 
sortColumns  
Type: System.String  
startRecord  
Type: System.Int32  
maxRecords  
Ibt.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 55 Type: System.Int32  
Return Value  
Type: DataSet  
See Also 
Ibt Class  
Browse Overload  
Lsa.Vmfg.Inventory Namespace  
  
Ibt.Exists Method  
56 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Ibt.Exists Method  
Checks for the existance of a specific IBT.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  ibtID 
) 
 
VB 
Public  Overridable  Function Exists  (  
 ibtID As String 
) As Boolean 
 
Parameters  
ibtID 
Type: System.String  
Return Value  
Type: Boolean  
See Also 
Ibt Class  
Lsa.Vmfg.Inventory Namespace  
  
Ibt.Find Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 57 Ibt.Find Method  
Retrieve a specific IBT. Only returns the top- level data table (IBT).  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  ibtID 
) 
 
VB 
Public  Overridable  Sub Find (  
 ibtID As String 
) 
 
Parameters  
ibtID 
Type: System.String  
See Also 
Ibt Class  
Lsa.Vmfg.Inventory Namespace  
  
Ibt.Load Method  
58 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Ibt.Load Method  
Overload List 
 Name  Description  
 Load(String)  Loads a specific IBT.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
Ibt Class  
Lsa.Vmfg.Inventory Namespace  
  

Ibt.Load Method (String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 59 Ibt.Load Method (String)  
Loads a specific IBT.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  ibtID 
) 
 
VB 
Public  Overridable  Sub Load (  
 ibtID As String 
) 
 
Parameters  
ibtID 
Type: System.String  
See Also 
Ibt Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
Ibt.Load Method (Stream, String)  
60 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Ibt.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  ibtID 
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 ibtID As String 
) 
 
Parameters  
stream  
Type: System.IO.Stream  
ibtID 
Type: System.String  
See Also 
Ibt Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
Ibt.NewIbtBinaryRow Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 61 Ibt.NewIbtBinaryRow Method  
Inserts a new row into the IBT_BINARY table. Only the binary type "D" (long text) is supported.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewIbtBinaryRow ( 
 string  ibtID, 
 string  binaryType 
) 
 
VB 
Public  Overridable  Function NewIbtBinaryRow  (  
 ibtID As String , 
 binaryType As String 
) As DataRow  
 
Parameters  
ibtID 
Type: System.String  
binaryType  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Ibt Class  
Lsa.Vmfg.Inventory Namespace  
  
Ibt.NewIbtLineBinaryRow Method  
62 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Ibt.NewIbtLineBinaryRow Method  
Inserts a new row into the IBT_LINE_BINARY table. Only the binary type "D" (long text) is 
supported.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewIbtLineBinaryRow ( 
 string  ibtID, 
 int ibtLineNo , 
 string  binaryType 
) 
 
VB 
Public  Overridable  Function NewIbtLineBinaryRow  (  
 ibtID As String , 
 ibtLineNo As Integer , 
 binaryType As String 
) As DataRow  
 
Parameters  
ibtID 
Type: System.String  
ibtLineNo  
Type: System.Int32  
binaryType  
Type: System.String  
Return Value  
Type: DataRow  
Ibt.NewIbtLineBinaryRow Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 63 See Also 
Ibt Class  
Lsa.Vmfg.Inventory Namespace  
  
Ibt.NewIbtLineRow Method  
64 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Ibt.NewIbtLineRow Method  
Overload List 
 Name  Description  
 NewIbtLineRow(String)  Inserts a new row into the IBT_LINE table. Auto Numbers the Line 
Row.  
 NewIbtLineRow(String, 
Int64)  Inserts a new row into the IBT_LINE table.  
 
See Also 
Ibt Class  
Lsa.Vmfg.Inventory Namespace  
  

Ibt.NewIbtLineRow Method (String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 65 Ibt.NewIbtLineRow Method (String)  
Inserts a new row into the IBT_LINE table. Auto Numbers the Line Row.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewIbtLineRow ( 
 string  ibtID 
) 
 
VB 
Public  Overridable  Function NewIbtLineRow  (  
 ibtID As String 
) As DataRow  
 
Parameters  
ibtID 
Type: System.String  
Return Value  
Type: DataRow  
 
See Also 
Ibt Class  
NewIbtLineRow Overload  
Lsa.Vmfg.Inventory Namespace  
  
Ibt.NewIbtLineRow Method (String, Int64)  
66 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Ibt.NewIbtLineRow Method (String, Int64)  
Inserts a new row into the IBT_LINE table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewIbtLineRow ( 
 string  ibtID, 
 long lineNo  
) 
 
VB 
Public  Overridable  Function NewIbtLineRow  (  
 ibtID As String , 
 lineNo  As Long  
) As DataRow  
 
Parameters  
ibtID 
Type: System.String  
lineNo  
Type: System.Int64  
Return Value  
Type: DataRow  
See Also 
Ibt Class  
NewIbtLineRow Overload  
Lsa.Vmfg.Inventory Namespace  
  
Ibt.NewIbtLineTrcCtlRow Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 67 Ibt.NewIbtLineTrcCtlRow Method  
Inserts a new row into the IBT_LINE_TRC_CTL table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewIbtLineTrcCtlRow ( 
 string  ibtID, 
 long lineNo  
) 
 
VB 
Public  Overridable  Function NewIbtLineTrcCtlRow  (  
 ibtID As String , 
 lineNo  As Long  
) As DataRow  
 
Parameters  
ibtID 
Type: System.String  
lineNo  
Type: System.Int64  
Return Value  
Type: DataRow  
See Also 
Ibt Class  
Lsa.Vmfg.Inventory Namespace  
  
Ibt.NewIbtRow Method  
68 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Ibt.NewIbtRow Method  
Overload List 
 Name  Description  
 NewIbtRow(String)  Inserts a new row into the IBT table.  
 NewIbtRow(String, String)  Inserts a new row into the IBT table for a specified Site ID.  
 
See Also 
Ibt Class  
Lsa.Vmfg.Inventory Namespace  
  

Ibt.NewIbtRow Method (String) 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 69 Ibt.NewIbtRow Method (String)  
Inserts a new row into the IBT table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewIbtRow ( 
 string  ibtID 
) 
 
VB 
Public  Overridable  Function NewIbtRow (  
 ibtID As String 
) As DataRow  
 
Parameters  
ibtID 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Ibt Class  
NewIbtRow Overload  
Lsa.Vmfg.Inventory Namespace  
  
Ibt.NewIbtRow Method (String, String) 
70 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Ibt.NewIbtRow Method (String, String)  
Inserts a new row into the IBT table for a specified Site ID. 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewIbtRow ( 
 string  ibtID, 
 string  siteID  
) 
 
VB 
Public  Overridable  Function NewIbtRow (  
 ibtID As String , 
 siteID  As String  
) As DataRow  
 
Parameters  
ibtID 
Type: System.String  
siteID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Ibt Class  
NewIbtRow Overload  
Lsa.Vmfg.Inventory Namespace  
  
Ibt.Save Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 71 Ibt.Save Method  
Overload List 
 Name  Description  
 Save()  Saves all previously loaded IBTs to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
See Also 
Ibt Class  
Lsa.Vmfg.Inventory Namespace  
  

Ibt.Save Method 
72 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Ibt.Save Method  
Saves all previously loaded IBTs to the database.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Ibt Class  
Save Overload  
Lsa.Vmfg.Inventory Namespace  
  
Ibt.Save Method (Stream)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 73 Ibt.Save Method (Stream)  
Save current state of data set to stream.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save ( 
 Stream  stream  
) 
 
VB 
Public  Overridable  Sub Save  (  
 stream  As Stream  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
See Also 
Ibt Class  
Save Overload  
Lsa.Vmfg.Inventory Namespace  
  
IbtReceipt Class  
74 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IbtReceipt Class  
Transaction to perform Interbranch Transfer receipts.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessTransaction  
          Lsa.Vmfg.Inventory.IbtReceipt  
 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  IbtReceipt  : BusinessTransaction  
 
VB 
<SerializableAttribute> 
Public  Class  IbtReceipt  
 Inherits  BusinessTransaction  
 
The IbtReceipt  type exposes the following members.  
Constructors 
 Name  Description  
 IbtReceipt()  Business Transaction Constructor  
 IbtReceipt(String)  Business Transaction Constructor  
 

IbtReceipt Class  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 75 Methods 
 Name  Description  
 NewReceiptLineRow  Inserts a new row into the IBT_RECEIPT_LINE transaction data table.  
See InventoryTransaction . 
 NewReceiptRow  Inserts a new row into the IBT_RECEIPT transaction data table.  
See InventoryTransaction . 
 NewReceiptTraceRow  Inserts a new row into the TRACE transaction data table.  
See InventoryTransaction . 
 Prepare  Creates an empty dataset for the transaction. You must populate the 
dataset prior to saving the transaction.  
 Save  Saves the transaction(s).  
Transaction 
 Name  Data Set returned 
from Prepare  Description  
 InventoryTransaction  INVENTORY_TRANS This transaction performs inventory transactions. 
The type of transaction that is performed is 
determined by the value of the 
“TRANSACTION_TYPE” column. Please note 
that the transaction is hierarchical, to support Part Traceability. All fields are optional  unless 
otherwise noted. In some cases, a default field value is provided if you do not specify a value in the call to the transaction. The default values are noted in the table.  
See Also 
Lsa.Vmfg.Inventory Namespace  
  

IbtReceipt Constructor  
76 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IbtReceipt Constructor  
Overload List 
 Name  Description  
 IbtReceipt()  Business Transaction Constructor  
 IbtReceipt(String)  Business Transaction Constructor  
 
See Also 
IbtReceipt Class  
Lsa.Vmfg.Inventory Namespace  
  

IbtReceipt Constructor  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 77 IbtReceipt Constructor  
Business Transaction Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  IbtReceipt () 
 
VB 
Public  Sub New 
 
See Also 
IbtReceipt Class  
IbtReceipt Overload  
Lsa.Vmfg.Inventory Namespace  
  
IbtReceipt Constructor (String)  
78 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IbtReceipt Constructor (String)  
Business Transaction Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  IbtReceipt ( 
 string  databaseInstanceName  
) 
 
VB 
Public  Sub New (  
 databaseInstanceName As String  
) 
 
Parameters  
databaseInstanceName  
Type: System.String  
See Also 
IbtReceipt Class  
IbtReceipt Overload  
Lsa.Vmfg.Inventory Namespace  
  
IbtReceipt.IbtReceipt Methods  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 79 IbtReceipt.IbtReceipt Methods  
The IbtReceipt  type exposes the following members.  
Methods 
 Name  Description  
 NewReceiptLineRow  Inserts a new row into the IBT_RECEIPT_LINE transaction data table.  
See InventoryTransaction . 
 NewReceiptRow  Inserts a new row into the IBT_RECEIPT transaction data table.  
See InventoryTransaction . 
 NewReceiptTraceRow  Inserts a new row into the TRACE transaction data table.  
See InventoryTransaction . 
 Prepare  Creates an empty dataset for the transaction. You must populate the 
dataset prior to saving the transaction.  
 Save  Saves the transaction(s).  
 
See Also 
IbtReceipt Class  
InventoryTransaction  
Lsa.Vmfg.Inventory Namespace  
  

IbtReceipt.NewReceiptLineRow Method 
80 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IbtReceipt.NewReceiptLineRow Method  
Inserts a new row into the IBT_RECEIPT_LINE transaction data table. See InventoryTransaction . 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewReceiptLineRow ( 
 string  ibtID, 
 int ibtLineNo 
) 
 
VB 
Public  Overridable  Function NewReceiptLineRow  (  
 ibtID As String , 
 ibtLineNo As Integer  
) As DataRow  
Parameters  
ibtID 
Type: System.String  
ibtLineNo  
Type: System.Int32  
Return Value  
Type: DataRow  
See Also 
IbtReceipt Class  
InventoryTransaction  
Lsa.Vmfg.Inventory Namespace  
  
IbtReceipt.NewReceiptRow Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 81 IbtReceipt.NewReceiptRow Method  
Inserts a new row into the IBT_RECEIPT transaction data table.  
See InventoryTransaction . 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewReceiptRow( 
 string  ibtID 
) 
 
VB 
Public  Overridable  Function NewReceiptRow  (  
 ibtID As String 
) As DataRow  
 
Parameters  
ibtID 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
IbtReceipt Class  
InventoryTransaction  
Lsa.Vmfg.Inventory Namespace  
  
IbtReceipt.NewReceiptTraceRow Method  
82 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IbtReceipt.NewReceiptTraceRow Method  
Inserts a new row into the TRACE transaction data table.  
See InventoryTransaction . 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewReceiptTraceRow ( 
 string  ibtID, 
 int ibtLineNo , 
 string  traceID  
) 
 
VB 
Public  Overridable  Function NewReceiptTraceRow  (  
 ibtID As String , 
 ibtLineNo As Integer , 
 traceID  As String 
) As DataRow  
 
Parameters  
ibtID 
Type: System.String  
ibtLineNo  
Type: System.Int32  
traceID  
Type: System.String  
Return Value  
Type: DataRow  
IbtReceipt.NewReceiptTraceRow Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 83 See Also 
IbtReceipt Class  
InventoryTransaction  
Lsa.Vmfg.Inventory Namespace  
  
IbtReceipt.Prepare Method  
84 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IbtReceipt.Prepare Method  
Creates an empty dataset for the transaction. You must populate the dataset prior to saving the 
transaction.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Prepare()  
 
VB 
Public  Overridable  Sub Prepare  
 
See Also 
IbtReceipt Class  
Lsa.Vmfg.Inventory Namespace  
  
IbtReceipt.Save Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 85 IbtReceipt.Save Method  
Saves the transaction(s).  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
IbtReceipt Class  
Lsa.Vmfg.Inventory Namespace  
  
IbtShipment Class  
86 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IbtShipment Class  
Transaction to perform Interbranch Transfer Shipments.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessTransaction  
          Lsa.Vmfg.Inventory.IbtShipment  
 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  IbtShipment  : BusinessTransaction  
 
VB 
<SerializableAttribute> 
Public  Class  IbtShipment  
 Inherits  BusinessTransaction  
 
The IbtShipment  type exposes the following members.  
Constructors 
 Name  Description  
 IbtShipment()  Business Transaction Constructor  
 IbtShipment(String)  Business Transaction Constructor  
 

IbtShipment Class  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 87 Methods 
 Name  Description  
 NewShipmentLineRow  Inserts a new row into the IBT_SHIPMENT_LINE transaction data 
table.  
See InventoryTransaction . 
 NewShipmentRow  Inserts a new row into the IBT_SHIPMENT transaction data table.  
See InventoryTransaction . 
 NewShipmentTraceRow  Inserts a new row into the TRACE transaction data table.  See InventoryTransaction
. 
 Prepare  Creates an empty dataset for the transaction. You must populate the 
dataset prior to saving the transaction.  
 Save  Saves the transaction(s).  
 
Transaction 
 Name  Data Set returned 
from Prepare  Description  
 InventoryTransaction  INVENTORY_TRANS This transaction performs inventory transactions. The type of transaction that is performed is 
determined by the value of the 
“TRANSACTION_TYPE” column. Please note that the transaction is hierarchical, to support 
Part Traceability. All fields are optional  unless 
otherwise noted. In some cases, a default field value is provided if you do not specify a value in 
the call to the transaction. The default values are noted in the table.  
See Also 
Lsa.Vmfg.Inventory Namespace  
  

IbtShipment Constructor  
88 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IbtShipment Constructor  
Overload List 
 Name  Description  
 IbtShipment()  Business Transaction Constructor  
 IbtShipment(String)  Business Transaction Constructor  
 
See Also 
IbtShipment Class  
Lsa.Vmfg.Inventory Namespace  
  

IbtShipment Constructor  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 89 IbtShipment Constructor  
Business Transaction Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  IbtShipment () 
 
VB 
Public  Sub New 
 
See Also 
IbtShipment Class  
IbtShipment Overload  
Lsa.Vmfg.Inventory Namespace  
  
IbtShipment Constructor (String)  
90 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IbtShipment Constructor (String)  
Business Transaction Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  IbtShipment ( 
 string  databaseInstanceName  
) 
 
VB 
Public  Sub New (  
 databaseInstanceName As String  
) 
 
Parameters  
databaseInstanceName  
Type: System.String  
See Also 
IbtShipment Class  
IbtShipment Overload  
Lsa.Vmfg.Inventory Namespace  
  
IbtShipment.IbtShipment Methods  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 91 IbtShipment.IbtShipment Methods  
The IbtShipment  type exposes the following members.  
Methods 
 Name  Description  
 NewShipmentLineRow  Inserts a new row into the IBT_SHIPMENT_LINE transaction data 
table.  
See InventoryTransaction . 
 NewShipmentRow  Inserts a new row into the IBT_SHIPMENT transaction data table.  
See InventoryTransaction . 
 NewShipmentTraceRow  Inserts a new row into the TRACE transaction data table.  
See InventoryTransaction . 
 Prepare  Creates an empty dataset for the transaction. You must populate the 
dataset prior to saving the transaction.  
 Save  Saves the transaction(s).  
 
See Also 
IbtShipment Class  
InventoryTransaction  
Lsa.Vmfg.Inventory Namespace  
  

IbtShipment.NewShipmentLineRow Method  
92 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IbtShipment.NewShipmentLineRow Method  
Inserts a new row into the IBT_SHIPMENT_LINE transaction data table.  
See InventoryTransaction . 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewShipmentLineRow ( 
 string  ibtID, 
 int ibtLineNo 
) 
 
VB 
Public  Overridable  Function NewShipmentLineRow  (  
 ibtID As String , 
 ibtLineNo As Integer  
) As DataRow  
 
Parameters  
ibtID 
Type: System.String  
ibtLineNo  
Type: System.Int32  
Return Value  
Type: DataRow  
See Also 
IbtShipment Class  
InventoryTransaction  
Lsa.Vmfg.Inventory Namespace  
  
IbtShipment.NewShipmentRow Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 93 IbtShipment.NewShipmentRow Method  
Inserts a new row into the IBT_SHIPMENT transaction data table.  
See InventoryTransaction . 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewShipmentRow ( 
 string  ibtID 
) 
 
VB 
Public  Overridable  Function NewShipmentRow  (  
 ibtID As String 
) As DataRow  
 
Parameters  
ibtID 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
IbtShipment Class  
InventoryTransaction  
Lsa.Vmfg.Inventory Namespace  
  
IbtShipment.NewShipmentTraceRow Method  
94 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IbtShipment.NewShipmentTraceRow Method  
Inserts a new row into the TRACE transaction data table.  
See InventoryTransaction . 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewShipmentTraceRow ( 
 string  ibtID, 
 int ibtLineNo , 
 string  traceID  
) 
 
VB 
Public  Overridable  Function NewShipmentTraceRow  (  
 ibtID As String , 
 ibtLineNo As Integer , 
 traceID  As String 
) As DataRow  
 
Parameters  
ibtID 
Type: System.String  
ibtLineNo  
Type: System.Int32  
traceID  
Type: System.String  
Return Value  
Type: DataRow  
IbtShipment.NewShipmentTraceRow Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 95 See Also 
IbtShipment Class  
InventoryTransaction  
Lsa.Vmfg.Inventory Namespace  
  
IbtShipment.Prepare Method  
96 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IbtShipment.Prepare Method  
Creates an empty dataset for the transaction. You must populate the dataset prior to saving the 
transaction.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Prepare()  
 
VB 
Public  Overridable  Sub Prepare  
 
See Also 
IbtShipment Class  
Lsa.Vmfg.Inventory Namespace  
  
IbtShipment.Save Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 97 IbtShipment.Save Method  
Saves the transaction(s).  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
IbtShipment Class  
Lsa.Vmfg.Inventory Namespace  
  
InventoryTransaction Class  
98 | Infor VISUAL API Toolkit  Inventory Class Library Reference  InventoryTransaction Class  
Inventory Transaction.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessTransaction  
          Lsa.Vmfg.Inventory.InventoryTransaction  
 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  InventoryTransaction : BusinessTransaction 
 
VB 
<SerializableAttribute> 
Public  Class  InventoryTransaction  
 Inherits  BusinessTransaction  
 
The InventoryTransaction type exposes the following members.  
Constructors 
 Name  Description  
 InventoryTransaction()  Business Transaction Constructor  
 InventoryTransaction(String)  Business Transaction Constructor  
 

InventoryTransaction Class  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 99 Methods 
 Name  Description  
 NewInputRow()  Inserts a new row into the INVENTORY_TRANS transaction data table.  
See InventoryTransaction . 
 NewInputRow(Int32)  Inserts a new row into the INVENTORY_TRANS transaction data table.  
See InventoryTransaction . 
 NewTraceRow  Inserts a new row into the TRACE transaction data table.  
See InventoryTransaction . 
 Prepare  Creates an empty dataset for the transaction. You must populate the 
dataset prior to saving the transaction.  
 Save  Saves the transaction(s).  
 
Transaction 
 Name  Data Set returned 
from Prepare  Description  
 InventoryTransaction  INVENTORY_TRANS This transaction performs inventory transactions. 
The type of transaction that is performed is 
determined by the value of the 
“TRANSACTION_TYPE” column. Please note 
that the transaction is hierarchical, to support Part Traceability. All fields are optional  unless 
otherwise noted. In some cases, a default field value is provided if you do not specify a value in 
the call to the transaction. The default values are 
noted in the table.  
See Also 
Lsa.Vmfg.Inventory Namespace  
  

InventoryTransaction Constructor  
100 | Infor VISUAL API Toolkit  Inventory Class Library Reference  InventoryTransaction Constructor  
Overload List 
 Name  Description  
 InventoryTransaction()  Business Transaction Constructor  
 InventoryTransaction(String)  Business Transaction Constructor  
 
See Also 
InventoryTransaction Class  
Lsa.Vmfg.Inventory Namespace  
  

InventoryTransaction Constructor  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 101 InventoryTransaction Constructor  
Business Transaction Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  InventoryTransaction() 
 
VB 
Public  Sub New 
 
See Also 
InventoryTransaction Class  
InventoryTransaction Overload  
Lsa.Vmfg.Inventory Namespace  
  
InventoryTransaction Constructor (String)  
102 | Infor VISUAL API Toolkit  Inventory Class Library Reference  InventoryTransaction Constructor (String)  
Business Transaction Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  InventoryTransaction( 
 string  databaseInstanceName  
) 
 
VB 
Public  Sub New (  
 databaseInstanceName As String  
) 
 
Parameters  
databaseInstanceName  
Type: System.String  
See Also 
InventoryTransaction Class  
InventoryTransaction Overload  
Lsa.Vmfg.Inventory Namespace  
  
InventoryTransaction.InventoryTransaction Methods 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 103 InventoryTransaction.InventoryTransaction Methods  
The InventoryTransaction  type exposes the following members.  
Methods 
 Name  Description  
 NewInputRow()  Inserts a new row into the INVENTORY_TRANS transaction data table.  
See InventoryTransaction . 
 NewInputRow(Int32)  Inserts a new row into the INVENTORY_TRANS transaction data table.  
See InventoryTransaction . 
 NewTraceRow  Inserts a new row into the TRACE transaction data table.  
See InventoryTransaction . 
 Prepare  Creates an empty dataset for the transaction. You must populate the 
dataset prior to saving the transaction.  
 Save  Saves the transaction(s).  
 
See Also 
InventoryTransaction  
InventoryTransaction Class  
Lsa.Vmfg.Inventory Namespace  
  

InventoryTransaction.NewInputRow Method  
104 | Infor VISUAL API Toolkit  Inventory Class Library Reference  InventoryTransaction.NewInputRow Method  
Overload List 
 Name  Description  
 NewInputRow()  Inserts a new row into the INVENTORY_TRANS transaction data table.  
See InventoryTransaction . 
 NewInputRow(Int32)  Inserts a new row into the INVENTORY_TRANS transaction data table.  
See InventoryTransaction . 
 
See Also 
InventoryTransaction  
InventoryTransaction Class  
Lsa.Vmfg.Inventory Namespace  
  

InventoryTransaction.NewInputRow Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 105 InventoryTransaction.NewInputRow Method  
Inserts a new row into the INVENTORY_TRANS transaction data table. See InventoryTransaction . 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewInputRow () 
 
VB 
Public  Overridable  Function NewInputRow  As DataRow  
 
Return Value  
Type: DataRow  
See Also 
InventoryTransaction  
InventoryTransaction Class  
NewInputRow Overload  
Lsa.Vmfg.Inventory Namespace  
  
InventoryTransaction.NewInputRow Method (Int32)  
106 | Infor VISUAL API Toolkit  Inventory Class Library Reference  InventoryTransaction.NewInputRow Method (Int32)  
Inserts a new row into the INVENTORY_TRANS transaction data table. See InventoryTransaction . 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewInputRow ( 
 int entryNo  
) 
 
VB 
Public  Overridable  Function NewInputRow  (  
 entryNo As Integer  
) As DataRow  
 
Parameters  
entryNo  
Type: System.Int32  
Return Value  
Type: DataRow  
See Also 
InventoryTransaction  
InventoryTransaction Class  
NewInputRow Overload  
Lsa.Vmfg.Inventory Namespace  
  
InventoryTransaction.NewTraceRow Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 107 InventoryTransaction.NewTraceRow Method  
Inserts a new row into the TRACE transaction data table. See InventoryTransaction . 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewTraceRow ( 
 int entryNo, 
 string  traceID  
) 
 
VB 
Public  Overridable  Function NewTraceRow  (  
 entryNo As Integer , 
 traceID  As String 
) As DataRow  
Parameters  
entryNo  
Type: System.Int32  
traceID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
InventoryTransaction  
InventoryTransaction Class  
Lsa.Vmfg.Inventory Namespace  
  
InventoryTransaction.Prepare Method 
108 | Infor VISUAL API Toolkit  Inventory Class Library Reference  InventoryTransaction.Prepare Method  
Creates an empty dataset for the transaction. You must populate the dataset prior to saving the 
transaction.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Prepare()  
 
VB 
Public  Overridable  Sub Prepare  
 
See Also 
InventoryTransaction Class  
Lsa.Vmfg.Inventory Namespace  
  
InventoryTransaction.Save Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 109 InventoryTransaction.Save Method  
Saves the transaction(s).  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
InventoryTransaction Class  
Lsa.Vmfg.Inventory Namespace  
  
InventoryTransaction 
110 | Infor VISUAL API Toolkit  Inventory Class Library Reference  InventoryTransaction  
DataSet name returned from Prepare: INVENTORY_TRANS 
Primary Key:  ENTRY_NO.  
Column Name  Type  Description  
ENTRY_NO  Integer  Uniquely numbers each row provided to the 
transaction. Typically you supply one 
transaction at a time, but  you can also 
batch transactions. Be careful how many 
rows you attempt to save per logical 
transaction.  
TRANSACTION_DATE Date  Date of transaction. Defaults to current 
date.  
TRANSACTION_TYPE String  Defines the type of transaction to be performed. The valid values are:  
ADJUST_IN 
ADJUST_OUT  
TRANSFER  
ISSUE RECEIPT 
PART_ID  String  Part ID being transacted. Required.  
SITE_ID  String  The Site ID for which this transaction 
applies. Required.  
TO_WAREHOUSE_ID  String  Warehouse ID receiving inventory . Defaults 
to Part’s primary warehouse. Not 
Applicable for ADJUST_OUT or ISSUE 
transactions  when quantity is positive. 
FROM_WAREHOUSE_ID  String  Warehouse ID issuing inventory.  Defaults 
to Part’s primary warehouse. Not 
applicable for ADJUST_IN or RECEIPT transactions  when quantity is positive.  
TO_LOCATION_ID  String  Location ID receiv ing inventory . Defaults to 
Part’s primary location. Not applicable for 
ADJUST_OUT or ISSUE transactions  
when quantity is positive. 
FROM_LOCATION_ID  String  Location ID issuing inventory . Defaults to 
Part’s primary location. Not applicable for 
ADJUST_IN or RECEIPT transactions  
when quantity is negative. 
InventoryTransaction 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 111 Column Name  Type  Description  
QTY Decimal  If Part is not defined as piece tracked, 
quantity being transacted. If Part is piece 
tracked, do not use this field, use the 
PIECE_COUNT field instead.  
LENGTH  Decimal  Length, required if part is defined as piece 
tracked by length.  
WIDTH  Decimal  Width, required if part is defined as piece tracked by width.  
HEIGHT  Decimal  Height, required if part is piece tracked by height.  
WORKORDER_TYPE String  Required for ISSUE and RECEIPT transactions.  
WORKORDER_BASE_ID  String  Required for ISSUE and RECEIPT transactions.  
WORKORDER_LOT_ID  String  Required for ISSUE and RECEIPT transactions.  
WORKORDER_SPLIT_ID  String  Required for ISSUE and RECEIPT transactions.  
WORKORDER_SUB_ID  String  Required for ISSUE and RECEIPT transactions.  
OPERATION_SEQ_NO  String  Required for ISSUE transactions.  
REQ_PIECE_NO  String  Required for ISSUE transactions.  
ALLOW_NEGATIVE_BALANCE  Boolean Boolean flag to indicate that as a result of 
executing this transaction, the on hand 
balance for this part / location will be 
allowed to go negative. Default value is 
false.  
PIECE_COUNT  Integer  For piece tracked parts, the number of 
pieces for this transaction. For non- piece 
tracked parts, leave this field blank (use the QTY field instead).  
ACCOUNT_ID String  General ledger account to be adjusted by 
transaction. Optional.  
ADJ_REASON_ID  String  Adjustment Reason code. Describes reason for this adjustment. Only applies to 
adjustments. Optional, unless specified as required in Site Maintenance.  
InventoryTransaction 
112 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Column Name  Type  Description  
ISSUE_REASON_ID  String  Issue Reason code. Describes the reason 
for this issue transaction. Only apples to 
issue transactions. Optional, unless 
specified as required in Site Maintenance.  
UNIT_MATERIAL_COST  Decimal  Unit cost of material portion of the 
transaction. Optional.  
UNIT_LABOR_COST  Decimal  Unit cost of labor portion of the transaction. Optional.  
UNIT_BURDEN_COST  Decimal  Unit cost of burden portion of the transaction. Optional.  
UNIT_SERVICE_COST  Decimal  Unit cost of service portion of the transaction. Optional.  
FIXED_COST Decimal  Fixed cost of the transaction. Optional.  
USER_ID  String  User ID of user performing the transaction. 
Defaults to SYSADM.  
DESCRIPTION  String  Optional description for ADJUST_IN and ADJUST_OUT transactions. Not applicable for TRANSFERS. 
Sub-Table Name:  TRACE  
Primary Key: ENTRY_NO, TRACE_ID.   
The Trace sub- table may or may not be required, depending on the Part’s trace profile.  
Column Name  Type  Description 
ENTRY_NO  Integer  Determines which entry this row of trace information belongs to. Must match an existing ENTRY_NO in the INVENTORY_TRANS 
table.  
TRACE_ID  String  Trace ID. Lot or serial number for the parts 
being transacted. If  the part’s trace profile 
supports auto numbering, and you wish to have the Trace Ids auto numbered, you must 
set the TRACE_ID values to the format “<n>” where n is a unique integer.  
ALPHA_PROPERTY_1  String  Alphanumeric property. May be required, 
depending on Part’s trace profile and if the transaction is inbound.  
InventoryTransaction 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 113 Column Name  Type  Description 
ALPHA_PROPERTY_2  String  Alphanumeric property. May be required, 
depending on Part’s trace profile and if the transaction is inbound.  
ALPHA_PROPERTY_3  String  Alphanumeric property. May be required, 
depending on Part’s trace profile and if the 
transaction is inbound.  
ALPHA_PROPERTY_4  String  Alphanumeric property. May be required, 
depending on Part’s trace profile and if the 
transaction is inbound.  
ALPHA_PROPERTY_5  String  Alphanumeric property. May be required, 
depending on Part’s trace profile and if the transaction is inbound.  
NUMERIC_PROPERTY_1  Decimal  Numeric property. May be required, depending 
on Part’s trace profile and if the transaction is 
inbound.  
NUMERIC_PROPERTY_2  Decimal  Numeric property. May be required, depending 
on Part’s trace profile and if the transaction is 
inbound.  
NUMERIC_PROPERTY_3  Decimal  Numeric property. May be required, depending on Part’s trace profile and if the transaction is 
inbound.  
NUMERIC_PROPERTY_4  Decimal  Numeric property. May be required, depending 
on Part’s trace profile and if the transaction is 
inbound.  
NUMERIC_PROPERTY_5  Decimal  Numeric property. May be required, depending 
on Part’s trace profile and if the transaction is 
inbound.  
COMMENTS  String  Optional user comments on specific lot or serial number.  
EXPIRATION_DATE  Date  Expiration date. Determines shelf life of lot. Optional.  
QTY Decimal  Quantity of transaction associated directly with this trace ID. Required. The Sum of all of the trace detail QTY fields must equal the 
inventory transaction’s QTY.  
See Also 
InventoryTransaction Class  
IssueReasons Class  
114 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IbtReceipt.NewReceiptLineRow Method  
IbtReceipt.NewReceiptRow Method  
IbtReceipt.NewReceiptTraceRow Method  
IbtShipment.NewShipmentLineRow Method  
IbtShipment.NewShipmentRow Method  
IbtShipment.NewShipmentTraceRow Method  
InventoryTransaction.NewInputRow Method  
InventoryTransaction.NewInputRow Method (Int32)  
InventoryTransaction.NewTraceRow Method  
Lsa.Vmfg.Inventory Namespace  
IssueReasons Class  
Maintain Issue Reason Codes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Inventory.IssueReasons  
 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  IssueReasons  : BusinessDocument  
 
IssueReasons Class  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 115 VB 
<SerializableAttribute> 
Public  Class  IssueReasons  
 Inherits  BusinessDocument  
 
The IssueReasons  type exposes the following members.  
Constructors 
 Name  Description  
 IssueReasons()  Constructor  
 IssueReasons(String)  Constructor  
 
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Issue Reason Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Issue Reason Codes based on search criteria, 
limited by record count.  
 Exists  Determines if a specific Issue Reason Code exists.  
 Find Retrieves a specific Issue Reason Code.  
 Load()  Load all Issue Reason Codes.  
 Load(String)  Load a specific Issue Reason Code.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewIssueReasonRow  Inserts a new row into the ISSUE_REASON table.  
 Save  Save all previously loaded Issue Reason Codes to the database.  
 

IssueReasons Class  
116 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object 
(Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType  Report types of data object associated with this business object (Overrides BusinessObject.ServicedComponentType.)  
 
See Also 
Lsa.Vmfg.Inventory Namespace  
  

IssueReasons Constructor  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 117 IssueReasons Constructor  
Overload List 
 Name  Description  
 IssueReasons()  Constructor  
 IssueReasons(String)  Constructor  
 
See Also 
IssueReasons Class  
Lsa.Vmfg.Inventory Namespace  
  

IssueReasons Constructor  
118 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IssueReasons Constructor  
Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  IssueReasons () 
 
VB 
Public  Sub New 
 
See Also 
IssueReasons Class  
IssueReasons Overload  
Lsa.Vmfg.Inventory Namespace  
  
IssueReasons Constructor (String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 119 IssueReasons Constructor (String)  
Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  IssueReasons ( 
 string  databaseInstanceName  
) 
 
VB 
Public  Sub New (  
 databaseInstanceName As String  
) 
 
Parameters  
databaseInstanceName  
Type: System.String  
See Also 
IssueReasons Class  
IssueReasons Overload  
Lsa.Vmfg.Inventory Namespace  
  
IssueReasons.IssueReasons Methods  
120 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IssueReasons.IssueReasons Methods  
The IssueReasons  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Issue Reason Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Issue Reason Codes based on search criteria, 
limited by record count.  
 Exists  Determines if a specific Issue Reason Code exists.  
 Find Retrieves a specific Issue Reason Code.  
 Load()  Load all Issue Reason Codes.  
 Load(String)  Load a specific Issue Reason Code.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewIssueReasonRow  Inserts a new row into the ISSUE_REASON table.  
 Save  Save all previously loaded Issue Reason Codes to the database.  
 
See Also 
IssueReasons Class  
Lsa.Vmfg.Inventory Namespace  
  

IssueReasons.Browse Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 121 IssueReasons.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Issue Reason Codes based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Issue Reason Codes based on search criteria, 
limited by record count.  
 
See Also 
IssueReasons Class  
Lsa.Vmfg.Inventory Namespace  
  

IssueReasons.Browse Method (String, String, String)  
122 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IssueReasons.Browse Method (String, String, 
String)  
Retrieve Issue Reason Codes based on search criteria.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataSet  Browse ( 
 string  columnNames , 
 string  searchCondition, 
 string  sortColumns 
) 
 
VB 
Public  Overridable  Function Browse  (  
 columnNames  As String , 
 searchCondition  As String , 
 sortColumns  As String 
) As DataSet  
 
Parameters  
columnNames 
Type: System.String  
searchCondition Type: System.String
 
sortColumns  
Type: System.String  
Return Value  
Type: DataSet  
IssueReasons.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 123 See Also 
IssueReasons Class  
Browse Overload  
Lsa.Vmfg.Inventory Namespace  
  
IssueReasons.Browse Method (String, String, String, Int32, Int32)  
124 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IssueReasons.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve Issue Reason Codes based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataSet  Browse ( 
 string  columnNames , 
 string  searchCondition, 
 string  sortColumns , 
 int startRecord, 
 int maxRecords  
) 
 
VB 
Public  Overridable  Function Browse  (  
 columnNames  As String , 
 searchCondition  As String , 
 sortColumns  As String , 
 startRecord As Integer , 
 maxRecords  As Integer  
) As DataSet  
 
Parameters  
columnNames 
Type: System.String  
searchCondition Type: System.String
 
sortColumns  
Type: System.String  
startRecord  
Type: System.Int32  
maxRecords  
Type: System.Int32  
IssueReasons.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 125 Return Value  
Type: DataSet  
See Also 
IssueReasons Class  
Browse Overload  
Lsa.Vmfg.Inventory Namespace  
  
IssueReasons.Exists Method 
126 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IssueReasons.Exists Method  
Determines if a specific Issue Reason Code exists.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  id 
) 
 
VB 
Public  Overridable  Function Exists  (  
 id As String 
) As Boolean 
 
Parameters  
id 
Type: System.String  
Return Value  
Type: Boolean  
See Also 
IssueReasons Class  
Lsa.Vmfg.Inventory Namespace  
  
IssueReasons.Find Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 127 IssueReasons.Find Method  
Retrieves a specific Issue Reason Code.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  id 
) 
 
VB 
Public  Overridable  Sub Find (  
 id As String 
) 
 
Parameters  
id 
Type: System.String  
See Also 
IssueReasons Class  
Lsa.Vmfg.Inventory Namespace  
  
IssueReasons.Load Method  
128 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IssueReasons.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Issue Reason Codes.  
 Load(String)  Load a specific Issue Reason Code.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
IssueReasons Class  
Lsa.Vmfg.Inventory Namespace  
  

IssueReasons.Load Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 129 IssueReasons.Load Method  
Load all Issue Reason Codes.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
IssueReasons Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
IssueReasons.Load Method (String)  
130 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IssueReasons.Load Method (String)  
Load a specific Issue Reason Code.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  id 
) 
 
VB 
Public  Overridable  Sub Load (  
 id As String 
) 
 
Parameters  
id 
Type: System.String  
See Also 
IssueReasons Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
IssueReasons.Load Method (Stream, String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 131 IssueReasons.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  id 
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 id As String 
) 
 
Parameters  
stream  
Type: System.IO.Stream  
id 
Type: System.String  
See Also 
IssueReasons Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
IssueReasons.NewIssueReasonRow Method  
132 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IssueReasons.NewIssueReasonRow Method  
Inserts a new row into the ISSUE_REASON table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewIssueReasonRow ( 
 string  id 
) 
 
VB 
Public  Function  NewIssueReasonRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
IssueReasons Class  
Lsa.Vmfg.Inventory Namespace  
  
IssueReasons.Save Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 133 IssueReasons.Save Method  
Save all previously loaded Issue Reason Codes to the database.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
IssueReasons Class  
Lsa.Vmfg.Inventory Namespace  
  
IssueReasons.IssueReasons Properties  
134 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IssueReasons.IssueReasons Properties  
The IssueReasons  type exposes the following members.  
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object 
(Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType  Report types of data object associated with this business object (Overrides BusinessObject.ServicedComponentType.)  
 
See Also 
IssueReasons Class  
Lsa.Vmfg.Inventory Namespace  
  

IssueReasons.DataObjectType Property  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 135 IssueReasons.DataObjectType Property  
Report types of data object associated with this business object  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  DataObjectType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  DataObjectType As Type 
 Get 
 
Property Value  
Type: Type  
See Also 
IssueReasons Class  
Lsa.Vmfg.Inventory Namespace  
  
IssueReasons.ServicedComponentType Property 
136 | Infor VISUAL API Toolkit  Inventory Class Library Reference  IssueReasons.ServicedComponentType Property  
Report types of data object associated with this business object  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  ServicedComponentType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  ServicedComponentType As Type  
 Get 
 
Property Value  
Type: Type  
See Also 
IssueReasons Class  
Lsa.Vmfg.Inventory Namespace  
  
Location Class  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 137 Location Class  
Maintain Warehouse Locations.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Inventory.Location  
 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  Location : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  Location  
 Inherits  BusinessDocument  
 
The Location type exposes the following members.  
Constructors 
 Name  Description  
 Location()  Business Document Constructor  
 Location(String)  Business Document Constructor  
 

Location Class  
138 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Locations based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Locations based on search criteria, limited by the 
values of the startRecord and maxRecords parameters.  
 Exists  Determines if a specific Warehouse Location exists.  
 Find Retrieves a specific Warehouse Location fron the database.  
 Load(String, String)  Loads a specific Warehouse Location.  
 Load(Stream, String, String)  Loads from stream and rename using new key.  
 NewLocationRow  Inserts a new row into the LOCATION data table, for a specific Warehouse.  
 Save  Saves all previously loaded Locations to the database.  
 
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object (Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType  Report types of data object associated with this business object (Overrides BusinessObject.ServicedComponentType.)  
 
See Also 
Lsa.Vmfg.Inventory Namespace  
  

Location Constructor  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 139 Location Constructor  
Overload List 
 Name  Description  
 Location()  Business Document Constructor  
 Location(String)  Business Document Constructor  
 
See Also 
Location Class  
Lsa.Vmfg.Inventory Namespace  
  

Location Constructor  
140 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Location Constructor  
Business Document Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Location()  
 
VB 
Public  Sub New 
 
See Also 
Location Class  
Location Overload  
Lsa.Vmfg.Inventory Namespace  
  
Location Constructor (String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 141 Location Constructor (String)  
Business Document Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Location( 
 string  databaseInstanceName  
) 
 
VB 
Public  Sub New (  
 databaseInstanceName As String  
) 
 
Parameters  
databaseInstanceName  
Type: System.String  
See Also 
Location Class  
Location Overload  
Lsa.Vmfg.Inventory Namespace  
  
Location.Location Methods  
142 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Location.Location Methods  
The Location  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Locations based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Locations based on search criteria, limited by the 
values of the startRecord and maxRecords parameters.  
 Exists  Determines if a specific Warehouse Location exists.  
 Find Retrieves a specific Warehouse Location fron the database.  
 Load(String, String)  Loads a specific Warehouse Location.  
 Load(Stream, String, String)  Loads from stream and rename using new key.  
 NewLocationRow  Inserts a new row into the LOCATION data table, for a specific Warehouse.  
 Save  Saves all previously loaded Locations to the database.  
 
See Also 
Location Class  
Lsa.Vmfg.Inventory Namespace  
  

Location.Browse Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 143 Location.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, 
String)  Retrieve Locations based on search criteria.  
 Browse(String, String, 
String, Int32, Int32)  Retrieve Locations based on search criteria, limited by the values 
of the startRecord and maxRecords parameters.  
 
See Also 
Location Class  
Lsa.Vmfg.Inventory Namespace  
  

Location.Browse Method (String, String, String)  
144 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Location.Browse Method (String, String, String)  
Retrieve Locations based on search criteria.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataSet  Browse ( 
 string  columnNames , 
 string  searchCondition, 
 string  sortColumns 
) 
 
VB 
Public  Overridable  Function Browse  (  
 columnNames  As String , 
 searchCondition  As String , 
 sortColumns  As String 
) As DataSet  
 
Parameters  
columnNames 
Type: System.String  
searchCondition Type: System.String
 
sortColumns  
Type: System.String  
Return Value  
Type: DataSet  
See Also 
Location Class  
Location.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 145 Browse Overload  
Lsa.Vmfg.Inventory Namespace  
  
Location.Browse Method (String, String, String, Int32, Int32)  
146 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Location.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve Locations based on search criteria, limited by the values of the startRecord and 
maxRecords parameters.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataSet  Browse ( 
 string  columnNames , 
 string  searchCondition, 
 string  sortColumns , 
 int startRecord, 
 int maxRecords  
) 
 
VB 
Public  Overridable  Function Browse  (  
 columnNames  As String , 
 searchCondition  As String , 
 sortColumns  As String , 
 startRecord As Integer , 
 maxRecords  As Integer  
) As DataSet  
 
Parameters  
columnNames 
Type: System.String  
searchCondition Type: System.String
 
sortColumns  
Type: System.String  
startRecord  
Type: System.Int32  
maxRecords  
Location.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 147 Type: System.Int32  
Return Value  
Type: DataSet  
See Also 
Location Class  
Browse Overload  
Lsa.Vmfg.Inventory Namespace  
  
Location.Exists Method  
148 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Location.Exists Method  
Determines if a specific Warehouse Location exists.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  warehouseID , 
 string  locationID  
) 
 
VB 
Public  Overridable  Function Exists  (  
 warehouseID  As String , 
 locationID  As String 
) As Boolean 
 
Parameters  
warehouseID  
Type: System.String  
locationID  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
Location Class  
Lsa.Vmfg.Inventory Namespace  
  
Location.Find Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 149 Location.Find Method  
Retrieves a specific Warehouse Location fron the database.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  warehouseID , 
 string  locationID  
) 
 
VB 
Public  Overridable  Sub Find (  
 warehouseID  As String , 
 locationID  As String 
) 
 
Parameters  
warehouseID  
Type: System.String  
locationID  
Type: System.String  
See Also 
Location Class  
Lsa.Vmfg.Inventory Namespace  
  
Location.Load Method  
150 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Location.Load Method  
Overload List 
 Name  Description  
 Load(String, String)  Loads a specific Warehouse Location.  
 Load(Stream, String, String)  Loads from stream and rename using new key.  
 
See Also 
Location Class  
Lsa.Vmfg.Inventory Namespace  
  

Location.Load Method (String, String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 151 Location.Load Method (String, String)  
Loads a specific Warehouse Location.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  warehouseID , 
 string  locationID  
) 
 
VB 
Public  Overridable  Sub Load (  
 warehouseID  As String , 
 locationID  As String 
) 
 
Parameters  
warehouseID  
Type: System.String  
locationID  
Type: System.String  
See Also 
Location Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
Location.Load Method (Stream, String, String)  
152 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Location.Load Method (Stream, String, String)  
Loads from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  warehouseID , 
 string  locationID  
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 warehouseID  As String , 
 locationID  As String 
) 
 
Parameters  
stream  
Type: System.IO.Stream  
warehouseID  
Type: System.String  
locationID  
Type: System.String  
See Also 
Location Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
Location.NewLocationRow Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 153 Location.NewLocationRow Method  
Inserts a new row into the LOCATION data table, for a specific Warehouse.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewLocationRow ( 
 string  warehouseID , 
 string  locationID  
) 
 
VB 
Public  Overridable  Function NewLocationRow  (  
 warehouseID  As String , 
 locationID  As String 
) As DataRow  
 
Parameters  
warehouseID  
Type: System.String  
locationID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Location Class  
Lsa.Vmfg.Inventory Namespace  
  
Location.Save Method  
154 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Location.Save Method  
Saves all previously loaded Locations to the database.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Location Class  
Lsa.Vmfg.Inventory Namespace  
  
Location.Location Properties  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 155 Location.Location Properties  
The Location  type exposes the following members.  
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object 
(Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType  Report types of data object associated with this business object (Overrides BusinessObject.ServicedComponentType.)  
 
See Also 
Location Class  
Lsa.Vmfg.Inventory Namespace  
  

Location.DataObjectType Property 
156 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Location.DataObjectType Property  
Report types of data object associated with this business object  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  DataObjectType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  DataObjectType As Type 
 Get 
 
Property Value  
Type: Type  
See Also 
Location Class  
Lsa.Vmfg.Inventory Namespace  
  
Location.ServicedComponentType Property  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 157 Location.ServicedComponentType Property  
Report types of data object associated with this business object  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  ServicedComponentType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  ServicedComponentType As Type  
 Get 
 
Property Value  
Type: Type  
See Also 
Location Class  
Lsa.Vmfg.Inventory Namespace  
  
Part Class  
158 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part Class  
Maintain Parts.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Inventory.Part  
 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  Part : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  Part 
 Inherits  BusinessDocument  
 
The Part type exposes the following members.  
Constructors 
 Name  Description  
 Part()  Business Document Constructor  
 Part(String)  Business Document Constructor  
 

Part Class  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 159 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieves Parts based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieves Parts based on search criteria, limited by the 
values of the startRecord and maxRecords parameters.  
 Exists  Determines if a specific Part exists.  
 Find Retrieves a specific Part. Only the top- level table 
(PART) is returned.  
 Load(String)  Loads a specific Part.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewCycleCountPartRow  Inserts a new row into the CYCLE_COUNT_PART data table.  
 NewDemandForecastRow(String, 
DateTime)  Inserts a new row into the DEMAND_FORECAST data table. Use this method if you are not forecasting by 
warehouse. Otherwise, use the version that has the 
warehouseID parameter.  
 NewDemandForecastRow(String, 
DateTime, String)  Inserts a new row into the DEMAND_FORECAST data table. Use this function if you are not forecasting by warehouse. Otherwise, use the version that has the warehouseID parameter.  
 NewDemandForecastRow(String, 
String, DateTime)  Inserts a new row into the DEMAND_FORECAST data 
table. Use this method if you are forecasting by 
warehouse. Otherwise, use the version that does not have the warehouseID parameter.  
 NewDemandForecastRow(String, 
DateTime, String, String)  Inserts a new row into the DEMAND_FORECAST data 
table. Use this method if you are forecasting by 
warehouse. Otherwise, use the version that does not have the warehouseID parameter.  
 NewPartAliasRow  Inserts a new row into the PART_ALIAS data table.  
 NewPartBinaryRow  Inserts a new row into the PART_BINARY data table. Only the binaryType "D" (long text) is supported.  
 NewPartCOBinaryRow  Inserts a new row into the PART_CO_BINARY data table. Only the binaryType "D" (long text) is supported.  

Part Class  
160 | Infor VISUAL API Toolkit  Inventory Class Library Reference   NewPartCrossSellingRow  Inserts a new row into the PART_CROSS_SELLING 
data table.  
 NewPartLocationRow  Inserts a new row into the PART_LOCATION data table.  
 NewPartMFGBinaryRow  Inserts a new row into the PART_MFG_BINARY data table. Only the binaryType "D" (long text) is supported.  
 NewPartPOBinaryRow  Inserts a new row into the PART_PO_BINARY data table. Only the binaryType "D" (long text) is supported.  
 NewPartRow  Inserts a new row into the PART data table.  
 NewPartShippingRow  Inserts a new row into the PART_SHIPPING data table.  
 NewPartSiteRow  Inserts a new row into the PART_SITE data table.  
 NewPartSubstituteRow  Inserts a new row into the PART_SUBSTITUTE data table.  
 NewPartUnitsConvRow  Inserts a new row into the PART_UNITS_CONV data table.  
 Save()  Saves all changes made to previously loaded Parts to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object (Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType  Report types of data object associated with this business object (Overrides BusinessObject.ServicedComponentType.)  
 
See Also 
Lsa.Vmfg.Inventory Namespace  

Part Class  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 161   
Part Constructor  
162 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part Constructor  
Overload List 
 Name  Description  
 Part()  Business Document Constructor  
 Part(String)  Business Document Constructor  
 
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  

Part Constructor  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 163 Part Constructor  
Business Document Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Part()  
 
VB 
Public  Sub New 
 
See Also 
Part Class  
Part Overload  
Lsa.Vmfg.Inventory Namespace  
  
Part Constructor (String)  
164 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part Constructor (String)  
Business Document Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Part( 
 string  databaseInstanceName  
) 
 
VB 
Public  Sub New (  
 databaseInstanceName As String  
) 
 
Parameters  
databaseInstanceName  
Type: System.String  
See Also 
Part Class  
Part Overload  
Lsa.Vmfg.Inventory Namespace  
  
Part.Part Methods  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 165 Part.Part Methods  
The Part type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieves Parts based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieves Parts based on search criteria, limited by the 
values of the startRecord and maxRecords parameters.  
 Exists  Determines if a specific Part exists.  
 Find Retrieves a specific Part. Only the top- level table 
(PART) is returned.  
 Load(String)  Loads a specific Part.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewCycleCountPartRow  Inserts a new row into the CYCLE_COUNT_PART data table.  
 NewDemandForecastRow(String, 
DateTime)  Inserts a new row into the DEMAND_FORECAST data 
table. Use this method if you are not forecasting by 
warehouse. Otherwise, use the version that has the warehouseID parameter.  
 NewDemandForecastRow(String, 
DateTime, String)  Inserts a new row into the DEMAND_FORECAST data table. Use this function if you are not forecasting by 
warehouse. Otherwise, use the version that has the 
warehouseID parameter.  
 NewDemandForecastRow(String, 
String, DateTime)  Inserts a new row into the DEMAND_FORECAST data table. Use this method if you are forecasting by 
warehouse. Otherwise, use the version that does not 
have the warehouseID parameter.  
 NewDemandForecastRow(String, 
DateTime, String, String)  Inserts a new row into the DEMAND_FORECAST data table. Use this method if you are forecasting by warehouse. Otherwise, use the version that does not have the warehouseID parameter.  
 NewPartAliasRow  Inserts a new row into the PART_ALIAS data table.  

Part.Part Methods  
166 | Infor VISUAL API Toolkit  Inventory Class Library Reference   NewPartBinaryRow  Inserts a new row into the PART_BINARY data table. 
Only the binaryType "D" (long text) is supported.  
 NewPartCOBinaryRow  Inserts a new row into the PART_CO_BINARY data table. Only the binaryType "D" (long text) is supported.  
 NewPartCrossSellingRow  Inserts a new row into the PART_CROSS_SELLING 
data table.  
 NewPartLocationRow  Inserts a new row into the PART_LOCATION data table.  
 NewPartMFGBinaryRow  Inserts a new row into the PART_MFG_BINARY data table. Only the binaryType "D" (long text) is supported.  
 NewPartPOBinaryRow  Inserts a new row into the PART_PO_BINARY data table. Only the binaryType "D" (long text) is supported.  
 NewPartRow  Inserts a new row into the PART data table.  
 NewPartShippingRow  Inserts a new row into the PART_SHIPPING data table.  
 NewPartSiteRow  Inserts a new row into the PART_SITE data table.  
 NewPartSubstituteRow  Inserts a new row into the PART_SUBSTITUTE data table.  
 NewPartUnitsConvRow  Inserts a new row into the PART_UNITS_CONV data table.  
 Save()  Saves all changes made to previously loaded Parts to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  

Part.Browse Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 167 Part.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieves Parts based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieves Parts based on search criteria, limited by the values of 
the startRecord and maxRecords parameters.  
 
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  

Part.Browse Method (String, String, String)  
168 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part.Browse Method (String, String, String)  
Retrieves Parts based on search criteria.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataSet  Browse ( 
 string  columnNames , 
 string  searchCondition, 
 string  sortColumns 
) 
 
VB 
Public  Overridable  Function Browse  (  
 columnNames  As String , 
 searchCondition  As String , 
 sortColumns  As String 
) As DataSet  
 
Parameters  
columnNames 
Type: System.String  
searchCondition Type: System.String
 
sortColumns  
Type: System.String  
Return Value  
Type: DataSet  
See Also 
Part Class  
Part.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 169 Browse Overload  
Lsa.Vmfg.Inventory Namespace  
  
Part.Browse Method (String, String, String, Int32, Int32)  
170 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part.Browse Method (String, String, String, Int32, 
Int32)  
Retrieves Parts based on search criteria, limited by the values of the startRecord and maxRecords 
parameters.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataSet  Browse ( 
 string  columnNames , 
 string  searchCondition, 
 string  sortColumns , 
 int startRecord, 
 int maxRecords  
) 
 
VB 
Public  Overridable  Function Browse  (  
 columnNames  As String , 
 searchCondition  As String , 
 sortColumns  As String , 
 startRecord As Integer , 
 maxRecords  As Integer  
) As DataSet  
 
Parameters  
columnNames 
Type: System.String  
searchCondition Type: System.String
 
sortColumns  
Type: System.String  
startRecord  
Type: System.Int32  
maxRecords  
Part.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 171 Type: System.Int32  
Return Value  
Type: DataSet  
See Also 
Part Class  
Browse Overload  
Lsa.Vmfg.Inventory Namespace  
  
Part.Exists Method  
172 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part.Exists Method  
Determines if a specific Part exists.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  partID  
) 
 
VB 
Public  Overridable  Function Exists  (  
 partID  As String  
) As Boolean 
 
Parameters  
partID  
Type: System.String  
Return Value  
Type: Boolean  
 
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  
Part.Find Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 173 Part.Find Method  
Retrieves a specific Part. Only the top- level table (PART) is returned.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  partID  
) 
 
VB 
Public  Overridable  Sub Find (  
 partID  As String  
) 
 
Parameters  
partID  
Type: System.String  
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  
Part.Load Method  
174 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part.Load Method  
Overload List 
 Name  Description  
 Load(String)  Loads a specific Part.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  

Part.Load Method (String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 175 Part.Load Method (String)  
Loads a specific Part.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  partID  
) 
 
VB 
Public  Overridable  Sub Load (  
 partID  As String  
) 
 
Parameters  
partID  
Type: System.String  
See Also 
Part Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
Part.Load Method (Stream, String)  
176 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  partID  
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 partID  As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
partID  
Type: System.String  
See Also 
Part Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
Part.NewCycleCountPartRow Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 177 Part.NewCycleCountPartRow Method  
Inserts a new row into the CYCLE_COUNT_PART data table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewCycleCountPartRow ( 
 string  partID , 
 string  warehouseID  
) 
 
VB 
Public  Overridable  Function NewCycleCountPartRow  (  
 partID  As String , 
 warehouseID  As String  
) As DataRow  
 
Parameters  
partID  
Type: System.String  
warehouseID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  
Part.NewDemandForecastRow Method 
178 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part.NewDemandForecastRow Method  
Overload List 
 Name  Description  
 NewDemandForecastRow(String, 
DateTime)  Inserts a new row into the DEMAND_FORECAST data 
table. Use this method if you are not forecasting by warehouse. Otherwise, use the version that has the warehouseID parameter.  
 NewDemandForecastRow(String, 
DateTime, String)  Inserts a new row into the DEMAND_FORECAST data 
table. Use this function if you are not forecasting by 
warehouse. Otherwise, use the version that has the warehouseID parameter.  
 NewDemandForecastRow(String, 
String, DateTime)  Inserts a new row into the DEMAND_FORECAST data table. Use this method if you are forecasting by 
warehouse. Otherwise, use the version that does not 
have the warehouseID parameter.  
 NewDemandForecastRow(String, 
DateTime, String, String)  Inserts a new row into the DEMAND_FORECAST data table. Use this method if you are forecasting by 
warehouse. Otherwise, use the version that does not 
have the warehouseID parameter.  
 
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  

Part.NewDemandForecastRow Method (String, DateTime)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 179 Part.NewDemandForecastRow Method (String, 
DateTime)  
Inserts a new row into the DEMAND_FORECAST data table. Use this method if you are not 
forecasting by warehouse. Otherwise, use the version that has the warehouseID parameter.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewDemandForecastRow ( 
 string  partID , 
 DateTime  reqDate  
) 
 
VB 
Public  Overridable  Function NewDemandForecastRow  (  
 partID  As String , 
 reqDate As DateTime 
) As DataRow  
 
Parameters  
partID  
Type: System.String  
reqDate  
Type: System.DateTime  
Return Value  
Type: DataRow  
See Also 
Part Class  
NewDemandForecastRow Overload  
Lsa.Vmfg.Inventory Namespace  
Part.NewDemandForecastRow Method (String, DateTime)  
180 | Infor VISUAL API Toolkit  Inventory Class Library Reference    
Part.NewDemandForecastRow Method (String, DateTime, String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 181 Part.NewDemandForecastRow Method (String, 
DateTime, String)  
Inserts a new row into the DEMAND_FORECAST data table. Use this function if you are not 
forecasting by warehouse. Otherwise, use the version that has the warehouseID parameter.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewDemandForecastRow ( 
 string  partID , 
 DateTime  reqDate, 
 string  siteID  
) 
 
VB 
Public  Overridable  Function NewDemandForecastRow  (  
 partID  As String , 
 reqDate As DateTime , 
 siteID  As String  
) As DataRow  
 
Parameters  
partID  
Type: System.String  
reqDate  
Type: System.DateTime  
siteID  
Type: System.String  
Return Value  
Type: DataRow  
Part.NewDemandForecastRow Method (String, DateTime, String)  
182 | Infor VISUAL API Toolkit  Inventory Class Library Reference  See Also 
Part Class  
NewDemandForecastRow Overload  
Lsa.Vmfg.Inventory Namespace  
  
Part.NewDemandForecastRow Method (String, String, DateTime)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 183 Part.NewDemandForecastRow Method (String, 
String, DateTime)  
Inserts a new row into the DEMAND_FORECAST data table. Use this method if you are forecasting 
by warehouse. Otherwise, use the version that does not have the warehouseID parameter.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewDemandForecastRow ( 
 string  partID , 
 string  warehouseID , 
 DateTime  reqDate  
) 
 
VB 
Public  Overridable  Function NewDemandForecastRow  (  
 partID  As String , 
 warehouseID  As String , 
 reqDate As DateTime 
) As DataRow  
 
Parameters  
partID  
Type: System.String  
warehouseID  
Type: System.String  
reqDate  
Type: System.DateTime  
Return Value  
Type: DataRow  
Part.NewDemandForecastRow Method (String, String, DateTime)  
184 | Infor VISUAL API Toolkit  Inventory Class Library Reference  See Also 
Part Class  
NewDemandForecastRow Overload  
Lsa.Vmfg.Inventory Namespace  
  
Part.NewDemandForecastRow Method (String, DateTime, String, String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 185 Part.NewDemandForecastRow Method (String, 
DateTime, String, String)  
Inserts a new row into the DEMAND_FORECAST data table. Use this method if you are forecasting 
by warehouse. Otherwise, use the version that does not have the warehouseID parameter.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewDemandForecastRow ( 
 string  partID , 
 DateTime  reqDate, 
 string  warehouseID , 
 string  siteID  
) 
 
VB 
Public  Overridable  Function NewDemandForecastRow  (  
 partID  As String , 
 reqDate As DateTime , 
 warehouseID  As String , 
 siteID  As String  
) As DataRow  
 
Parameters  
partID  
Type: System.String  
reqDate  
Type: System.DateTime  
warehouseID  
Type: System.String  
siteID  
Type: System.String  
Part.NewDemandForecastRow Method (String, DateTime, String, String)  
186 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Return Value  
Type: DataRow  
See Also 
Part Class  
NewDemandForecastRow Overload  
Lsa.Vmfg.Inventory Namespace  
  
Part.NewPartAliasRow Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 187 Part.NewPartAliasRow Method  
Inserts a new row into the PART_ALIAS data table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewPartAliasRow( 
 string  partID , 
 string  id 
) 
 
VB 
Public  Overridable  Function NewPartAliasRow  (  
 partID  As String , 
 id As String 
) As DataRow  
 
Parameters  
partID  
Type: System.String  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  
Part.NewPartBinaryRow Method 
188 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part.NewPartBinaryRow Method  
Inserts a new row into the PART_BINARY data table. Only the binaryType "D" (long text) is 
supported.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewPartBinaryRow( 
 string  partID , 
 string  binaryType 
) 
 
VB 
Public  Overridable  Function NewPartBinaryRow  (  
 partID  As String , 
 binaryType As String 
) As DataRow  
 
Parameters  
partID  
Type: System.String  
binaryType  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  
Part.NewPartCOBinaryRow Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 189 Part.NewPartCOBinaryRow Method  
Inserts a new row into the PART_CO_BINARY data table. Only the binaryType "D" (long text) is 
supported.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewPartCOBinaryRow( 
 string  partID , 
 string  binaryType 
) 
 
VB 
Public  Overridable  Function NewPartCOBinaryRow  (  
 partID  As String , 
 binaryType As String 
) As DataRow  
 
Parameters  
partID  
Type: System.String  
binaryType  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  
Part.NewPartCrossSellingRow Method 
190 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part.NewPartCrossSellingRow Method  
Inserts a new row into the PART_CROSS_SELLING data table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewPartCrossSellingRow( 
 string  partID , 
 string  crossSellingPartID  
) 
 
VB 
Public  Overridable  Function NewPartCrossSellingRow  (  
 partID  As String , 
 crossSellingPartID  As String  
) As DataRow  
 
Parameters  
partID  
Type: System.String  
crossSellingPartID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  
Part.NewPartLocationRow Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 191 Part.NewPartLocationRow Method  
Inserts a new row into the PART_LOCATION data table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewPartLocationRow ( 
 string  partID , 
 string  warehouseID , 
 string  locationID  
) 
 
VB 
Public  Overridable  Function NewPartLocationRow  (  
 partID  As String , 
 warehouseID  As String , 
 locationID  As String 
) As DataRow  
 
Parameters  
partID  
Type: System.String  
warehouseID  
Type: System.String  
locationID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Part Class  
Part.NewPartLocationRow Method  
192 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Lsa.Vmfg.Inventory Namespace  
  
Part.NewPartMFGBinaryRow Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 193 Part.NewPartMFGBinaryRow Method  
Inserts a new row into the PART_MFG_BINARY data table. Only the binaryType "D" (long text) is 
supported.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewPartMFGBinaryRow ( 
 string  partID , 
 string  binaryType 
) 
 
VB 
Public  Overridable  Function NewPartMFGBinaryRow  (  
 partID  As String , 
 binaryType As String 
) As DataRow  
 
Parameters  
partID  
Type: System.String  
binaryType  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  
Part.NewPartPOBinaryRow Method 
194 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part.NewPartPOBinaryRow Method  
Inserts a new row into the PART_PO_BINARY data table. Only the binaryType "D" (long text) is 
supported.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewPartPOBinaryRow ( 
 string  partID , 
 string  binaryType 
) 
 
VB 
Public  Overridable  Function NewPartPOBinaryRow  (  
 partID  As String , 
 binaryType As String 
) As DataRow  
 
Parameters  
partID  
Type: System.String  
binaryType  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  
Part.NewPartRow Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 195 Part.NewPartRow Method  
Inserts a new row into the PART data table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewPartRow( 
 string  partID  
) 
 
VB 
Public  Overridable  Function NewPartRow  (  
 partID  As String  
) As DataRow  
 
Parameters  
partID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  
Part.NewPartShippingRow Method 
196 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part.NewPartShippingRow Method  
Inserts a new row into the PART_SHIPPING data table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewPartShippingRow ( 
 string  partID , 
 string  customerID , 
 string  shiptoID , 
 string  containerPartID  
) 
 
VB 
Public  Overridable  Function NewPartShippingRow  (  
 partID  As String , 
 customerID  As String , 
 shiptoID  As String , 
 containerPartID  As String 
) As DataRow  
 
Parameters  
partID  
Type: System.String  
customerID  
Type: System.String  
shiptoID  
Type: System.String  
containerPartID  
Type: System.String  
Return Value  
Type: DataRow  
Part.NewPartShippingRow Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 197 See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  
Part.NewPartSiteRow Method 
198 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part.NewPartSiteRow Method  
Inserts a new row into the PART_SITE data table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewPartSiteRow ( 
 string  partID , 
 string  siteID  
) 
 
VB 
Public  Overridable  Function NewPartSiteRow  (  
 partID  As String , 
 siteID  As String  
) As DataRow  
 
Parameters  
partID  
Type: System.String  
siteID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  
Part.NewPartSubstituteRow Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 199 Part.NewPartSubstituteRow Method  
Inserts a new row into the PART_SUBSTITUTE data table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewPartSubstituteRow ( 
 string  partID , 
 string  substitutePartID  
) 
 
VB 
Public  Overridable  Function NewPartSubstituteRow  (  
 partID  As String , 
 substitutePartID  As String  
) As DataRow  
 
Parameters  
partID  
Type: System.String  
substitutePartID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  
Part.NewPartUnitsConvRow Method 
200 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part.NewPartUnitsConvRow Method  
Inserts a new row into the PART_UNITS_CONV data table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewPartUnitsConvRow ( 
 string  partID , 
 string  fromUM , 
 string  toUM  
) 
 
VB 
Public  Overridable  Function NewPartUnitsConvRow  (  
 partID  As String , 
 fromUM As String , 
 toUM  As String 
) As DataRow  
 
Parameters  
partID  
Type: System.String  
fromUM  
Type: System.String  
toUM  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Part Class  
Part.NewPartUnitsConvRow Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 201 Lsa.Vmfg.Inventory Namespace  
  
Part.Save Method 
202 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part.Save Method  
Overload List 
 Name  Description  
 Save()  Saves all changes made to previously loaded Parts to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  

Part.Save Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 203 Part.Save Method  
Saves all changes made to previously loaded Parts to the database.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Part Class  
Save Overload  
Lsa.Vmfg.Inventory Namespace  
  
Part.Save Method (Stream)  
204 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part.Save Method (Stream)  
Save current state of data set to stream.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save ( 
 Stream  stream  
) 
 
VB 
Public  Overridable  Sub Save  (  
 stream  As Stream  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
See Also 
Part Class  
Save Overload  
Lsa.Vmfg.Inventory Namespace  
  
Part.Part Properties  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 205 Part.Part Properties  
The Part type exposes the following members.  
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object 
(Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType  Report types of data object associated with this business object (Overrides BusinessObject.ServicedComponentType.)  
 
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  

Part.DataObjectType Property  
206 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Part.DataObjectType Property  
Report types of data object associated with this business object  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  DataObjectType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  DataObjectType As Type 
 Get 
 
Property Value  
Type: Type  
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  
Part.ServicedComponentType Property  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 207 Part.ServicedComponentType Property  
Report types of data object associated with this business object  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  ServicedComponentType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  ServicedComponentType As Type  
 Get 
 
Property Value  
Type: Type  
See Also 
Part Class  
Lsa.Vmfg.Inventory Namespace  
  
PartAliasTypes Class  
208 | Infor VISUAL API Toolkit  Inventory Class Library Reference  PartAliasTypes Class  
Maintain Part Alias Types.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Inventory.PartAliasTypes  
 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  PartAliasTypes  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  PartAliasTypes  
 Inherits  BusinessDocument  
 
The PartAliasTypes  type exposes the following members.  
Constructors 
 Name  Description  
 PartAliasTypes()  Business Document Constructor  
 PartAliasTypes(String)  Business Document Constructor  
 

PartAliasTypes Class  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 209 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Part Alias Types based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Part Alias Types based on search criteria, limited by 
the values of the startRecord and maxRecords parameters.  
 Exists  Determines if a specific Part Alias Type exists.  
 Find Retrieves a specific Part Alias Type.  
 Load()  Loads all Part Alias Types.  
 Load(String)  Loads a specific Part Alias Type.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewPartAliasTypeRow  Adds a new row to the PART_ALIAS_TYPE table.  
 Save()  Saves all previously loaded Part Alias Types to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object (Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType  Report types of data object associated with this business object (Overrides BusinessObject.ServicedComponentType.)  
 
See Also 
Lsa.Vmfg.Inventory Namespace  
  

PartAliasTypes Constructor  
210 | Infor VISUAL API Toolkit  Inventory Class Library Reference  PartAliasTypes Constructor  
Overload List 
 Name  Description  
 PartAliasTypes()  Business Document Constructor  
 PartAliasTypes(String)  Business Document Constructor  
 
See Also 
PartAliasTypes Class  
Lsa.Vmfg.Inventory Namespace  
  

PartAliasTypes Constructor  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 211 PartAliasTypes Constructor  
Business Document Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  PartAliasTypes () 
 
VB 
Public  Sub New 
 
See Also 
PartAliasTypes Class  
PartAliasTypes Overload  
Lsa.Vmfg.Inventory Namespace  
  
PartAliasTypes Constructor (String) 
212 | Infor VISUAL API Toolkit  Inventory Class Library Reference  PartAliasTypes Constructor (String)  
Business Document Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  PartAliasTypes ( 
 string  databaseInstanceName  
) 
 
VB 
Public  Sub New (  
 databaseInstanceName As String  
) 
 
Parameters  
databaseInstanceName  
Type: System.String  
See Also 
PartAliasTypes Class  
PartAliasTypes Overload  
Lsa.Vmfg.Inventory Namespace  
  
PartAliasTypes.PartAliasTypes Methods  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 213 PartAliasTypes.PartAliasTypes Methods  
The PartAliasTypes  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Part Alias Types based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Part Alias Types based on search criteria, limited by 
the values of the startRecord and maxRecords parameters.  
 Exists  Determines if a specific Part Alias Type exists.  
 Find Retrieves a specific Part Alias Type.  
 Load()  Loads all Part Alias Types.  
 Load(String)  Loads a specific Part Alias Type.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewPartAliasTypeRow  Adds a new row to the PART_ALIAS_TYPE table.  
 Save()  Saves all previously loaded Part Alias Types to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
See Also 
PartAliasTypes Class  
Lsa.Vmfg.Inventory Namespace  
  

PartAliasTypes.Browse Method  
214 | Infor VISUAL API Toolkit  Inventory Class Library Reference  PartAliasTypes.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, 
String)  Retrieve Part Alias Types based on search criteria.  
 Browse(String, String, 
String, Int32, Int32)  Retrieve Part Alias Types based on search criteria, limited by the 
values of the startRecord and maxRecords parameters.  
 
See Also 
PartAliasTypes Class  
Lsa.Vmfg.Inventory Namespace  
  

PartAliasTypes.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 215 PartAliasTypes.Browse Method (String, String, 
String)  
Retrieve Part Alias Types based on search criteria.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataSet  Browse ( 
 string  columnNames , 
 string  searchCondition, 
 string  sortColumns 
) 
 
VB 
Public  Overridable  Function Browse  (  
 columnNames  As String , 
 searchCondition  As String , 
 sortColumns  As String 
) As DataSet  
 
Parameters  
columnNames 
Type: System.String  
searchCondition Type: System.String
 
sortColumns  
Type: System.String  
Return Value  
Type: DataSet  
PartAliasTypes.Browse Method (String, String, String)  
216 | Infor VISUAL API Toolkit  Inventory Class Library Reference  See Also 
PartAliasTypes Class  
Browse Overload  
Lsa.Vmfg.Inventory Namespace  
  
PartAliasTypes.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 217 PartAliasTypes.Browse Method (String, String, 
String, Int32, Int32)  
Retrieve Part Alias Types based on search criteria, limited by the values of the startRecord and 
maxRecords parameters.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataSet  Browse ( 
 string  columnNames , 
 string  searchCondition, 
 string  sortColumns , 
 int startRecord, 
 int maxRecords  
) 
 
VB 
Public  Overridable  Function Browse  (  
 columnNames  As String , 
 searchCondition  As String , 
 sortColumns  As String , 
 startRecord As Integer , 
 maxRecords  As Integer  
) As DataSet  
 
Parameters  
columnNames 
Type: System.String  
searchCondition Type: System.String
 
sortColumns  
Type: System.String  
startRecord  
Type: System.Int32  
maxRecords  
PartAliasTypes.Browse Method (String, String, String, Int32, Int32)  
218 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Type: System.Int32  
Return Value  
Type: DataSet  
See Also 
PartAliasTypes Class  
Browse Overload  
Lsa.Vmfg.Inventory Namespace  
  
PartAliasTypes.Exists Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 219 PartAliasTypes.Exists Method  
Determines if a specific Part Alias Type exists.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  id 
) 
 
VB 
Public  Overridable  Function Exists  (  
 id As String 
) As Boolean 
 
Parameters  
id 
Type: System.String  
Return Value  
Type: Boolean  
See Also 
PartAliasTypes Class  
Lsa.Vmfg.Inventory Namespace  
  
PartAliasTypes.Find Method  
220 | Infor VISUAL API Toolkit  Inventory Class Library Reference  PartAliasTypes.Find Method  
Retrieves a specific Part Alias Type.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  id 
) 
 
VB 
Public  Overridable  Sub Find (  
 id As String 
) 
 
Parameters  
id 
Type: System.String  
See Also 
PartAliasTypes Class  
Lsa.Vmfg.Inventory Namespace  
  
PartAliasTypes.Load Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 221 PartAliasTypes.Load Method  
Overload List 
 Name  Description  
 Load()  Loads all Part Alias Types.  
 Load(String)  Loads a specific Part Alias Type.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
PartAliasTypes Class  
Lsa.Vmfg.Inventory Namespace  
  

PartAliasTypes.Load Method  
222 | Infor VISUAL API Toolkit  Inventory Class Library Reference  PartAliasTypes.Load Method  
Loads all Part Alias Types.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
PartAliasTypes Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
PartAliasTypes.Load Method (String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 223 PartAliasTypes.Load Method (String)  
Loads a specific Part Alias Type.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  id 
) 
 
VB 
Public  Overridable  Sub Load (  
 id As String 
) 
 
Parameters  
id 
Type: System.String  
See Also 
PartAliasTypes Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
PartAliasTypes.Load Method (Stream, String)  
224 | Infor VISUAL API Toolkit  Inventory Class Library Reference  PartAliasTypes.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  id 
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 id As String 
) 
 
Parameters  
stream  
Type: System.IO.Stream  
id 
Type: System.String  
See Also 
PartAliasTypes Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
PartAliasTypes.NewPartAliasTypeRow Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 225 PartAliasTypes.NewPartAliasTypeRow Method  
Adds a new row to the PART_ALIAS_TYPE table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewPartAliasTypeRow ( 
 string  id 
) 
 
VB 
Public  Function  NewPartAliasTypeRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
PartAliasTypes Class  
Lsa.Vmfg.Inventory Namespace  
  
PartAliasTypes.Save Method 
226 | Infor VISUAL API Toolkit  Inventory Class Library Reference  PartAliasTypes.Save Method  
Overload List 
 Name  Description  
 Save()  Saves all previously loaded Part Alias Types to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
See Also 
PartAliasTypes Class  
Lsa.Vmfg.Inventory Namespace  
  

PartAliasTypes.Save Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 227 PartAliasTypes.Save Method  
Saves all previously loaded Part Alias Types to the database.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
PartAliasTypes Class  
Save Overload  
Lsa.Vmfg.Inventory Namespace  
  
PartAliasTypes.Save Method (Stream)  
228 | Infor VISUAL API Toolkit  Inventory Class Library Reference  PartAliasTypes.Save Method (Stream)  
Save current state of data set to stream.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save ( 
 Stream  stream  
) 
 
VB 
Public  Overridable  Sub Save  (  
 stream  As Stream  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
See Also 
PartAliasTypes Class  
Save Overload  
Lsa.Vmfg.Inventory Namespace  
  
PartAliasTypes.PartAliasTypes Properties  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 229 PartAliasTypes.PartAliasTypes Properties  
The PartAliasTypes  type exposes the following members.  
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object 
(Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType  Report types of data object associated with this business object (Overrides BusinessObject.ServicedComponentType.)  
 
See Also 
PartAliasTypes Class  
Lsa.Vmfg.Inventory Namespace  
  

PartAliasTypes.DataObjectType Property  
230 | Infor VISUAL API Toolkit  Inventory Class Library Reference  PartAliasTypes.DataObjectType Property  
Report types of data object associated with this business object  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  DataObjectType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  DataObjectType As Type 
 Get 
 
Property Value  
Type: Type  
See Also 
PartAliasTypes Class  
Lsa.Vmfg.Inventory Namespace  
  
PartAliasTypes.ServicedComponentType Property 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 231 PartAliasTypes.ServicedComponentType Property  
Report types of data object associated with this business object  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  ServicedComponentType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  ServicedComponentType As Type  
 Get 
 
Property Value  
Type: Type  
See Also 
PartAliasTypes Class  
Lsa.Vmfg.Inventory Namespace  
  
Warehouse Class  
232 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Warehouse Class  
Maintain Warehouses.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Inventory.Warehouse  
 
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  class  Warehouse : BusinessDocument  
 
VB 
Public  Class  Warehouse  
 Inherits  BusinessDocument  
 
The Warehouse type exposes the following members.  
Constructors 
 Name  Description  
 Warehouse()  Business Document Constructor  
 Warehouse(String)  Business Document Constructor  
 

Warehouse Class  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 233 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieves Warehouses based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieves Warehouses based on search criteria, limited by 
the values of the startRecord and maxRecords parameters.  
 Exists  Determines if a specific Warehouse exists.  
 Find Retrieves a specific Warehouse. Only the top- level table 
(WAREHOUSE) is returned.  
 Load(String)  Loads a specific Warehouse.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewLocationRow  Inserts a new row into the LOCATION table.  
 NewTransitTimeRow  Inserts a new row into the TRANSIT_TIME table.  
 NewWarehouseRow  Inserts a new row into the WAREHOUSE table.  
 NewWarehouseWipVasRow  Inserts a new row into the WAREHOUSE_WIP_VAS table.  
 Save()  Saves all previously loaded Warehouses to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object (Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType  Report types of data object associated with this business object (Overrides BusinessObject.ServicedComponentType.)  
 

Warehouse Class  
234 | Infor VISUAL API Toolkit  Inventory Class Library Reference  See Also 
Lsa.Vmfg.Inventory Namespace  
  
Warehouse Constructor  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 235 Warehouse Constructor  
Overload List 
 Name  Description  
 Warehouse()  Business Document Constructor  
 Warehouse(String)  Business Document Constructor  
 
See Also 
Warehouse Class  
Lsa.Vmfg.Inventory Namespace  
  

Warehouse Constructor  
236 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Warehouse Constructor  
Business Document Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Warehouse() 
 
VB 
Public  Sub New 
 
See Also 
Warehouse Class  
Warehouse Overload  
Lsa.Vmfg.Inventory Namespace  
  
Warehouse Constructor (String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 237 Warehouse Constructor (String)  
Business Document Constructor  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Warehouse( 
 string  databaseInstanceName  
) 
 
VB 
Public  Sub New (  
 databaseInstanceName As String  
) 
 
Parameters  
databaseInstanceName  
Type: System.String  
See Also 
Warehouse Class  
Warehouse Overload  
Lsa.Vmfg.Inventory Namespace  
  
Warehouse.Warehouse Methods 
238 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Warehouse.Warehouse Methods  
The Warehouse  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieves Warehouses based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieves Warehouses based on search criteria, limited by 
the values of the startRecord and maxRecords parameters.  
 Exists  Determines if a specific Warehouse exists.  
 Find Retrieves a specific Warehouse. Only the top- level table 
(WAREHOUSE) is returned.  
 Load(String)  Loads a specific Warehouse.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewLocationRow  Inserts a new row into the LOCATION table.  
 NewTransitTimeRow  Inserts a new row into the TRANSIT_TIME table.  
 NewWarehouseRow  Inserts a new row into the WAREHOUSE table.  
 NewWarehouseWipVasRow  Inserts a new row into the WAREHOUSE_WIP_VAS table.  
 Save()  Saves all previously loaded Warehouses to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
See Also 
Warehouse Class  
Lsa.Vmfg.Inventory Namespace  
  

Warehouse.Browse Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 239 Warehouse.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, 
String)  Retrieves Warehouses based on search criteria.  
 Browse(String, String, 
String, Int32, Int32)  Retrieves Warehouses based on search criteria, limited by the 
values of the startRecord and maxRecords parameters.  
 
See Also 
Warehouse Class  
Lsa.Vmfg.Inventory Namespace  
  

Warehouse.Browse Method (String, String, String)  
240 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Warehouse.Browse Method (String, String, String)  
Retrieves Warehouses based on search criteria.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataSet  Browse ( 
 string  columnNames , 
 string  searchCondition, 
 string  sortColumns 
) 
 
VB 
Public  Overridable  Function Browse  (  
 columnNames  As String , 
 searchCondition  As String , 
 sortColumns  As String 
) As DataSet  
 
Parameters  
columnNames 
Type: System.String  
searchCondition Type: System.String
 
sortColumns  
Type: System.String  
Return Value  
Type: DataSet  
See Also 
Warehouse Class  
Warehouse.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 241 Browse Overload  
Lsa.Vmfg.Inventory Namespace  
  
Warehouse.Browse Method (String, String, String, Int32, Int32)  
242 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Warehouse.Browse Method (String, String, String, 
Int32, Int32) 
Retrieves Warehouses based on search criteria, limited by the values of the startRecord and 
maxRecords parameters.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataSet  Browse ( 
 string  columnNames , 
 string  searchCondition, 
 string  sortColumns , 
 int startRecord, 
 int maxRecords  
) 
 
VB 
Public  Overridable  Function Browse  (  
 columnNames  As String , 
 searchCondition  As String , 
 sortColumns  As String , 
 startRecord As Integer , 
 maxRecords  As Integer  
) As DataSet  
 
Parameters  
columnNames 
Type: System.String  
searchCondition Type: System.String
 
sortColumns  
Type: System.String  
startRecord  
Type: System.Int32  
maxRecords  
Warehouse.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 243 Type: System.Int32  
Return Value  
Type: DataSet  
See Also 
Warehouse Class  
Browse Overload  
Lsa.Vmfg.Inventory Namespace  
  
Warehouse.Exists Method  
244 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Warehouse.Exists Method  
Determines if a specific Warehouse exists.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  warehouseID  
) 
 
VB 
Public  Overridable  Function Exists  (  
 warehouseID  As String  
) As Boolean 
 
Parameters  
warehouseID  
Type: System.String  
Return Value  
Type: Boolean  
 
See Also 
Warehouse Class  
Lsa.Vmfg.Inventory Namespace  
  
Warehouse.Find Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 245 Warehouse.Find Method  
Retrieves a specific Warehouse. Only the top- level table (WAREHOUSE) is returned.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  warehouseID  
) 
 
VB 
Public  Overridable  Sub Find (  
 warehouseID  As String  
) 
 
Parameters  
warehouseID  
Type: System.String  
See Also 
Warehouse Class  
Lsa.Vmfg.Inventory Namespace  
  
Warehouse.Load Method 
246 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Warehouse.Load Method  
Overload List 
 Name  Description  
 Load(String)  Loads a specific Warehouse.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
Warehouse Class  
Lsa.Vmfg.Inventory Namespace  
  

Warehouse.Load Method (String)  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 247 Warehouse.Load Method (String)  
Loads a specific Warehouse.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  warehouseID  
) 
 
VB 
Public  Overridable  Sub Load (  
 warehouseID  As String  
) 
 
Parameters  
warehouseID  
Type: System.String  
See Also 
Warehouse Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
Warehouse.Load Method (Stream, String)  
248 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Warehouse.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  warehouseID  
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 warehouseID  As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
warehouseID  
Type: System.String  
See Also 
Warehouse Class  
Load Overload  
Lsa.Vmfg.Inventory Namespace  
  
Warehouse.NewLocationRow Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 249 Warehouse.NewLocationRow Method  
Inserts a new row into the LOCATION table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewLocationRow ( 
 string  warehouseID , 
 string  locationID  
) 
 
VB 
Public  Overridable  Function NewLocationRow  (  
 warehouseID  As String , 
 locationID  As String 
) As DataRow  
 
Parameters  
warehouseID  
Type: System.String  
locationID  
Type: System.String  
Return Value  
Type: DataRow  
 
See Also 
Warehouse Class  
Lsa.Vmfg.Inventory Namespace  
  
Warehouse.NewTransitTimeRow Method  
250 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Warehouse.NewTransitTimeRow Method  
Inserts a new row into the TRANSIT_TIME table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewTransitTimeRow ( 
 string  fromWarehouseID , 
 string  toWarehouseID  
) 
 
VB 
Public  Overridable  Function NewTransitTimeRow  (  
 fromWarehouseID  As String , 
 toWarehouseID  As String 
) As DataRow  
 
Parameters  
fromWarehouseID  
Type: System.String  
toWarehouseID  
Type: System.String  
Return Value  
Type: DataRow  
 
See Also 
Warehouse Class  
Lsa.Vmfg.Inventory Namespace  
  
Warehouse.NewWarehouseRow Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 251 Warehouse.NewWarehouseRow Method  
Inserts a new row into the WAREHOUSE table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewWarehouseRow ( 
 string  WarehouseID , 
 string  siteID  
) 
 
VB 
Public  Overridable  Function NewWarehouseRow  (  
 WarehouseID  As String , 
 siteID  As String  
) As DataRow  
 
Parameters  
WarehouseID  
Type: System.String  
siteID  
Type: System.String  
Return Value  
Type: DataRow  
 
See Also 
Warehouse Class  
Lsa.Vmfg.Inventory Namespace  
  
Warehouse.NewWarehouseWipVasRow Method 
252 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Warehouse.NewWarehouseWipVasRow Method  
Inserts a new row into the WAREHOUSE_WIP_VAS table.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewWarehouseWipVasRow ( 
 string  warehouseID , 
 string  partID , 
 string  wipVasID  
) 
 
VB 
Public  Overridable  Function NewWarehouseWipVasRow  (  
 warehouseID  As String , 
 partID  As String , 
 wipVasID  As String  
) As DataRow  
 
Parameters  
warehouseID  
Type: System.String  
partID  
Type: System.String  
wipVasID  
Type: System.String  
Return Value  
Type: DataRow  
 
Warehouse.NewWarehouseWipVasRow Method 
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 253 See Also 
Warehouse Class  
Lsa.Vmfg.Inventory Namespace  
  
Warehouse.Save Method  
254 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Warehouse.Save Method  
Overload List 
 Name  Description  
 Save()  Saves all previously loaded Warehouses to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
See Also 
Warehouse Class  
Lsa.Vmfg.Inventory Namespace  
  

Warehouse.Save Method  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 255 Warehouse.Save Method  
Saves all previously loaded Warehouses to the database.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Warehouse Class  
Save Overload  
Lsa.Vmfg.Inventory Namespace  
  
Warehouse.Save Method (Stream)  
256 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Warehouse.Save Method (Stream)  
Save current state of data set to stream.  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save ( 
 Stream  stream  
) 
 
VB 
Public  Overridable  Sub Save  (  
 stream  As Stream  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
See Also 
Warehouse Class  
Save Overload  
Lsa.Vmfg.Inventory Namespace  
  
Warehouse.Warehouse Properties  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 257 Warehouse.Warehouse Properties  
The Warehouse  type exposes the following members.  
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object 
(Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType  Report types of data object associated with this business object (Overrides BusinessObject.ServicedComponentType.)  
 
See Also 
Warehouse Class  
Lsa.Vmfg.Inventory Namespace  
  

Warehouse.DataObjectType Property 
258 | Infor VISUAL API Toolkit  Inventory Class Library Reference  Warehouse.DataObjectType Property  
Report types of data object associated with this business object  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  DataObjectType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  DataObjectType As Type 
 Get 
 
Property Value  
Type: Type  
See Also 
Warehouse Class  
Lsa.Vmfg.Inventory Namespace  
  
Warehouse.ServicedComponentType Property  
Infor VISUAL API Toolkit  Inventory Class Library Reference   | 259 Warehouse.ServicedComponentType Property  
Report types of data object associated with this business object  
Namespace:  Lsa.Vmfg.Inventory  
Assembly:  VmfgInventory (in VmfgInventory.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  ServicedComponentType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  ServicedComponentType As Type  
 Get 
 
Property Value  
Type: Type  
See Also 
Warehouse Class  
Lsa.Vmfg.Inventory Namespace  
 
