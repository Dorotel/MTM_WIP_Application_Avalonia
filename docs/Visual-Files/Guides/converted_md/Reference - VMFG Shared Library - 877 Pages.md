# Reference - VMFG Shared Library - 877 Pages

*Converted from PDF*

---

  
Infor VISUAL API Toolkit Shared 
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
 
  
Infor VISUAL API Toolkit  Shared Class Library Reference | 3 About this guide  
This guide describes  the objects available in the Infor VISUAL API Toolkit Shared class library.  
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
4 | Infor VISUAL API Toolkit  Shared Class Library Reference  Support  information  
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
  
Lsa.Vmfg .Shared Namespace  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 5 Lsa.Vmfg.Shared Namespace  
  
Classes 
 Class  Description  
 AccountingEntity  Maintain Accounting Entities.  
 AltIndustryCode  Maintain Alternate Industry Codes.  
 ApplGlobal  Obtain Visual Enterprise Application Global Information.  
 Carrier  Maintain Carriers.  
 CommodityCode  Maintain Commodity Codes.  
 Contact  Maintain Contacts.  
 Country  Maintain Countries.  
 Currency  Maintain Currencies.  
 Department  Maintan Departments.  
 DeviationReason  Maintan Deviation Reason Codes.  
 EarningCode  Maintain Earning Codes.  
 Employee  Maintain Employees.  
 FOBPoint  Maintain FOB Points.  
 GeneralQuery  General Query. This service allows you to cause a query 
execution under data object control with transmission of the result set to the client side.  
 GetDemandLinks  Service to obtain all linked demand to a given supply.  
 GetManufacturingJournalInfo  Service to obtain Manufacturing Journal Info.  
 GetPartDefaultWhseLoc  Service to obtain a Part's default warehouse and location IDs.  
 GetSupplyLinks  Service to obtain all linked supply to a given demand.  

Lsa.Vmfg .Shared Namespace  
6 | Infor VISUAL API Toolkit  Shared Class Library Reference   GLInterface  Maintain GL Interface Accounts.  
 HarmonizedTariff  Maintain Harmonized Tariff Codes.  
 Honorific  Maintain Honorific Codes.  
 Indirect  Maintain Indirect IDs.  
 Language  Maintain Language IDs.  
 ModeOfTransport  Maintain Mode Of Transport Codes.  
 NmfcCode  Maintain NMFC Codes.  
 Notation  Maintain Notations.  
 PackageType  Maintain Package Types.  
 ProductCode  Maintain Product Codes.  
 ProjectInfo  Maintain Project Info.  
 QuotePartUnitPrice  Service to obtain a part's unit price based on existing price 
breaks.  
 SalesTax  Maintain Sales Taxes.  
 SalesTaxGroup  Maintain Sales Tax Groups.  
 ServiceUnitCost  Service to obtain service unit cost based on existing price breaks.  
 ServiceUnitofMeasureConv  Calculates the quantity of a service in one unit of measure given a 
quantity of another unit of measure. Similar to the Part Unit of Measure conversion.  
 Shift Maintain Shifts.  
 ShipToAddress  Maintain Ship To Addresses.  
 ShipVia  Maintain Ship Via Codes.  
 Site Maintain Sites.  
 StateProvince  Maintain State / Provinces.  
 StdIndustryCode  Maintain Std Industry Codes.  
 Terms  Maintain Terms.  

Lsa.Vmfg .Shared Namespace  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 7  Territory  Maintain Territories.  
 Unit Maintain Units of Measure.  
 UnitofMeasureConversion  Service to calculate the quantity of a part in one unit of measure 
given a quantity of another unit of measure.  
 Vat Maintain VAT Codes.  
 Wbs Maintain WBS Codes.  
 Withholding  Maintain Withholding Codes.  
AppGlobal Class 
The AppGlobal class is read- only for all users. You can use the class to read information from the 
database, but you cannot add or delete information with this class.  
Classes and Infor VISUAL Financials Global Edition 
If a Financials Global Edition license has been applied to the VISUAL database you registered for 
use with APIs, then these classes are read- only:  
• SalesTax 
• SalesTaxGroup 
• Terms  
• Vat 
• Withholding  
 
  

AccountingEntity Class  
8 | Infor VISUAL API Toolkit  Shared Class Library Reference  AccountingEntity Class  
Maintain Accounting Entities.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.AccountingEntity  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  AccountingEntity  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  AccountingEntity  
 Inherits  BusinessDocument  
 
The AccountingEntity  type exposes the following members.  
Constructors 
 Name  Description  
 AccountingEntity()  Initializes a new instance of the AccountingEntity  class 
 AccountingEntity(String)  Initializes a new instance of the AccountingEntity  class 
 

AccountingEntity Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 9 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve a list of Accounting Entities based on search 
criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Accounting Entities, limited by record count.  
 Exists  Determine if an Accounting Entity exists.  
 Find Find a specific Accounting Entity.  
 Load()  Load all Accounting Entities.  
 Load(String)  Load a specific Accounting Entity row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewAccountingEntityRow  Adds a new row to the ACCOUNTING_ENTITY table.  
 Save  Save Accounting Entities to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

AccountingEntity Constructor  
10 | Infor VISUAL API Toolkit  Shared Class Library Reference  AccountingEntity Constructor  
Overload List 
 Name  Description  
 AccountingEntity()  Initializes a new instance of the AccountingEntity  class 
 AccountingEntity(String)  Initializes a new instance of the AccountingEntity  class 
 
See Also 
AccountingEntity Class  
Lsa.Vmfg.Shared Namespace  
  

AccountingEntity Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 11 AccountingEntity Constructor  
Initializes a new instance of the AccountingEntity  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  AccountingEntity () 
 
VB 
Public  Sub New 
 
See Also 
AccountingEntity Class  
AccountingEntity Overload  
Lsa.Vmfg.Shared Namespace  
  
AccountingEntity Constructor  (String)  
12 | Infor VISUAL API Toolkit  Shared Class Library Reference  AccountingEntity Constructor (String)  
Initializes a new instance of the AccountingEntity  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  AccountingEntity ( 
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
AccountingEntity Class  
AccountingEntity Overload  
Lsa.Vmfg.Shared Namespace  
  
AccountingEntity.AccountingEntity  Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 13 AccountingEntity.AccountingEntity Methods  
The AccountingEntity  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve a list of Accounting Entities based on search 
criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Accounting Entities, limited by record count.  
 Exists  Determine if an Accounting Entity exists.  
 Find Find a specific Accounting Entity.  
 Load()  Load all Accounting Entities.  
 Load(String)  Load a specific Accounting Entity row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewAccountingEntityRow  Adds a new row to the ACCOUNTING_ENTITY table.  
 Save  Save Accounting Entities to the database.  
 
See Also 
AccountingEntity Class  
Lsa.Vmfg.Shared Namespace  
  

AccountingEntity.Browse  Method 
14 | Infor VISUAL API Toolkit  Shared Class Library Reference  AccountingEntity.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve a list of Accounting Entities based on search 
criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Accounting Entities, limited by record count.  
 
See Also 
AccountingEntity Class  
Lsa.Vmfg.Shared Namespace  
  

AccountingEntity.Browse  Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 15 AccountingEntity.Browse Method (String, String, 
String)  
Retrieve a list of Accounting Entities based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
AccountingEntity.Browse  Method (String, String, String) 
16 | Infor VISUAL API Toolkit  Shared Class Library Reference  See Also 
AccountingEntity Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
AccountingEntity.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 17 AccountingEntity.Browse Method (String, String, 
String, Int32, Int32)  
Retrieve Accounting Entities, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
AccountingEntity.Browse  Method (String, String, String, Int32, Int32)  
18 | Infor VISUAL API Toolkit  Shared Class Library Reference  Return Value  
Type: DataSet  
See Also 
AccountingEntity Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
AccountingEntity.Exists  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 19 AccountingEntity.Exists Method  
Determine if an Accounting Entity exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
AccountingEntity Class  
Lsa.Vmfg.Shared Namespace  
  
AccountingEntity.Find Method  
20 | Infor VISUAL API Toolkit  Shared Class Library Reference  AccountingEntity.Find Method  
Find a specific Accounting Entity.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
AccountingEntity Class  
Lsa.Vmfg.Shared Namespace  
  
AccountingEntity.Load  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 21 AccountingEntity.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Accounting Entities.  
 Load(String)  Load a specific Accounting Entity row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
AccountingEntity Class  
Lsa.Vmfg.Shared Namespace  
  

AccountingEntity.Load  Method 
22 | Infor VISUAL API Toolkit  Shared Class Library Reference  AccountingEntity.Load Method  
Load all Accounting Entities.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
AccountingEntity Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
AccountingEntity.Load  Method (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 23 AccountingEntity.Load Method (String)  
Load a specific Accounting Entity row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
AccountingEntity Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
AccountingEntity.Load  Method (Stream, String)  
24 | Infor VISUAL API Toolkit  Shared Class Library Reference  AccountingEntity.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
AccountingEntity Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
AccountingEntity.NewAccountingEntityRow  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 25 AccountingEntity.NewAccountingEntityRow Method  
Adds a new row to the ACCOUNTING_ENTITY table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewAccountingEntityRow ( 
 string  id 
) 
 
VB 
Public  Function  NewAccountingEntityRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
AccountingEntity Class  
Lsa.Vmfg.Shared Namespace  
  
AccountingEntity.Save  Method  
26 | Infor VISUAL API Toolkit  Shared Class Library Reference  AccountingEntity.Save Method  
Save Accounting Entities to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
AccountingEntity Class  
Lsa.Vmfg.Shared Namespace  
  
AltIndustryCode Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 27 AltIndustryCode Class  
Maintain Alternate Industry Codes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.AltIndustryCode  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  AltIndustryCode : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  AltIndustryCode  
 Inherits  BusinessDocument  
 
The AltIndustryCode  type exposes the following members.  
Constructors 
 Name  Description  
 AltIndustryCode()  Initializes a new instance of the AltIndustryCode class 
 AltIndustryCode(String)  Initializes a new instance of the AltIndustryCode class 
 

AltIndustryCode Class  
28 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve existing Alternate Industry Codes based on search 
criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Alternate Industry Codes, based on search criteria, limited by a record count.  
 Exists  Determine if an Alternate Industry Code exists.  
 Find Find an Alternate Industry Code.  
 Load()  Load all Alternate Industry Codes.  
 Load(String)  Load a specific Alternate Industry Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewAICRow  Adds a new row to the AIC table.  
 Save  Save Alternate Industry Code(s) to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

AltIndustryCode Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 29 AltIndustryCode Constructor  
Overload List 
 Name  Description  
 AltIndustryCode()  Initializes a new instance of the AltIndustryCode  class  
 AltIndustryCode(String)  Initializes a new instance of the AltIndustryCode  class  
 
See Also 
AltIndustryCode Class  
Lsa.Vmfg.Shared Namespace  
  

AltIndustryCode Constructor  
30 | Infor VISUAL API Toolkit  Shared Class Library Reference  AltIndustryCode Constructor  
Initializes a new instance of the AltIndustryCode  class  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  AltIndustryCode()  
 
VB 
Public  Sub New 
 
See Also 
AltIndustryCode Class  
AltIndustryCode Overload  
Lsa.Vmfg.Shared Namespace  
  
AltIndustryCode Constructor  (String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 31 AltIndustryCode Constructor (String)  
Initializes a new instance of the AltIndustryCode  class  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  AltIndustryCode( 
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
AltIndustryCode Class  
AltIndustryCode Overload  
Lsa.Vmfg.Shared Namespace  
  
AltIndustryCode.AltIndustryCode  Methods  
32 | Infor VISUAL API Toolkit  Shared Class Library Reference  AltIndustryCode.AltIndustryCode Methods  
The AltIndustryCode  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve existing Alternate Industry Codes based on search 
criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Alternate Industry Codes, based on search criteria, limited by a record count.  
 Exists  Determine if an Alternate Industry Code exists.  
 Find Find an Alternate Industry Code.  
 Load()  Load all Alternate Industry Codes.  
 Load(String)  Load a specific Alternate Industry Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewAICRow  Adds a new row to the AIC table.  
 Save  Save Alternate Industry Code(s) to the database.  
 
See Also 
AltIndustryCode Class  
Lsa.Vmfg.Shared Namespace  
  

AltIndustryCode.Browse  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 33 AltIndustryCode.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve existing Alternate Industry Codes based on search 
criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Alternate Industry Codes, based on search criteria, limited by a record count.  
 
See Also 
AltIndustryCode Class  
Lsa.Vmfg.Shared Namespace  
  

AltIndustryCode.Browse  Method (String, String, String) 
34 | Infor VISUAL API Toolkit  Shared Class Library Reference  AltIndustryCode.Browse Method (String, String, 
String)  
Retrieve existing Alternate Industry Codes based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
AltIndustryCode.Browse  Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 35 See Also 
AltIndustryCode Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
AltIndustryCode.Browse  Method (String, String, String, Int32, Int32)  
36 | Infor VISUAL API Toolkit  Shared Class Library Reference  AltIndustryCode.Browse Method (String, String, 
String, Int32, Int32)  
Retrieve Alternate Industry Codes, based on search criteria, limited by a record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
AltIndustryCode.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 37 Return Value  
Type: DataSet  
See Also 
AltIndustryCode Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
AltIndustryCode.Exists Method  
38 | Infor VISUAL API Toolkit  Shared Class Library Reference  AltIndustryCode.Exists Method  
Determine if an Alternate Industry Code exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  code 
) 
 
VB 
Public  Overridable  Function Exists  (  
 code As String  
) As Boolean 
 
Parameters  
code  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
AltIndustryCode Class  
Lsa.Vmfg.Shared Namespace  
  
AltIndustryCode.Find  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 39 AltIndustryCode.Find Method  
Find an Alternate Industry Code.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Find (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
AltIndustryCode Class  
Lsa.Vmfg.Shared Namespace  
  
AltIndustryCode.Load Method  
40 | Infor VISUAL API Toolkit  Shared Class Library Reference  AltIndustryCode.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Alternate Industry Codes.  
 Load(String)  Load a specific Alternate Industry Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
AltIndustryCode Class  
Lsa.Vmfg.Shared Namespace  
  

AltIndustryCode.Load Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 41 AltIndustryCode.Load Method  
Load all Alternate Industry Codes.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
AltIndustryCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
AltIndustryCode.Load Method (String)  
42 | Infor VISUAL API Toolkit  Shared Class Library Reference  AltIndustryCode.Load Method (String)  
Load a specific Alternate Industry Code row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
AltIndustryCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
AltIndustryCode.Load Method (Stream, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 43 AltIndustryCode.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 code As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
code  
Type: System.String  
See Also 
AltIndustryCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
AltIndustryCode.NewAICRow  Method 
44 | Infor VISUAL API Toolkit  Shared Class Library Reference  AltIndustryCode.NewAICRow Method  
Adds a new row to the AIC table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewAICRow ( 
 string  code 
) 
 
VB 
Public  Function  NewAICRow  (  
 code As String  
) As DataRow  
 
Parameters  
code  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
AltIndustryCode Class  
Lsa.Vmfg.Shared Namespace  
  
AltIndustryCode.Save  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 45 AltIndustryCode.Save Method  
Save Alternate Industry Code(s) to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
AltIndustryCode Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal Class  
46 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal Class  
Obtain Visual Enterprise Application Global Information.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.ApplGlobal  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  ApplGlobal  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  ApplGlobal  
 Inherits  BusinessDocument  
 
The ApplGlobal type exposes the following members.  
Constructors 
 Name  Description  
 ApplGlobal()  Initializes a new instance of the ApplGlobal class 
 ApplGlobal(String)  Initializes a new instance of the ApplGlobal class 
 

ApplGlobal Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 47 Methods 
 Name  Description  
 Find Find The Application Global record.  
 Load  Load The Application Global record.  
 
Properties 
 Name  Description  
 ActualCosting   
 ActualWipCosting   
 AddLocOnTheFly   
 APCostingSource   
 BurdenFromResource   
 BurdenSource   
 CostingMethod   
 CountryID   
 CurrencyDataSet   
 DataObjectType  Report types of data object associated with this business object 
(Overrides BusinessObject.DataObjectType.)  
 DDPEnabled   
 DefLanguageID   
 DeviationRequired   
 EntityID   
 euroCurrencyID   
 exchangeRateDate   

ApplGlobal Class  
48 | Infor VISUAL API Toolkit  Shared Class Library Reference   FifoByLocation   
 IntrastatEnabled   
 IssueNegative   
 MfgEntityID   
 MfgInterfaceUsed   
 MultiSite   
 NewVatEnabled   
 POCostingSource   
 ProjectedWipCosting   
 PurcQuoteType   
 ServicedComponentType   (Overrides BusinessObject.ServicedComponentType.)  
 SiteID   
 StandardCosting   
 SystemCurrencyID   
 VatEnabled   
 WipVasEnabled   
 WithholdingEnabled   
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

ApplGlobal Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 49 ApplGlobal Constructor  
Overload List 
 Name  Description  
 ApplGlobal()  Initializes a new instance of the ApplGlobal  class 
 ApplGlobal(String)  Initializes a new instance of the ApplGlobal  class 
 
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  

ApplGlobal Constructor  
50 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal Constructor  
Initializes a new instance of the ApplGlobal  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  ApplGlobal () 
 
VB 
Public  Sub New 
 
See Also 
ApplGlobal Class  
ApplGlobal Overload  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal Constructor  (String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 51 ApplGlobal Constructor (String)  
Initializes a new instance of the ApplGlobal  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  ApplGlobal ( 
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
ApplGlobal Class  
ApplGlobal Overload  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.ApplGlobal  Methods  
52 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.ApplGlobal Methods  
The ApplGlobal  type exposes the following members.  
Methods 
 Name  Description  
 Find Find The Application Global record.  
 Load  Load The Application Global record.  
 
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  

ApplGlobal.Find  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 53 ApplGlobal.Find Method  
Find The Application Global record.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find() 
 
VB 
Public  Overridable  Sub Find 
 
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.Load Method  
54 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.Load Method  
Load The Application Global record.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.ApplGlobal  Properties  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 55 ApplGlobal.ApplGlobal Properties  
The ApplGlobal  type exposes the following members.  
Properties 
 Name  Description  
 ActualCosting   
 ActualWipCosting   
 AddLocOnTheFly   
 APCostingSource   
 BurdenFromResource   
 BurdenSource   
 CostingMethod   
 CountryID   
 CurrencyDataSet   
 DataObjectType  Report types of data object associated with this business object 
(Overrides BusinessObject.DataObjectType.)  
 DDPEnabled   
 DefLanguageID   
 DeviationRequired   
 EntityID   
 euroCurrencyID   
 exchangeRateDate   
 FifoByLocation   
 IntrastatEnabled   
 IssueNegative   

ApplGlobal.ApplGlobal  Properties  
56 | Infor VISUAL API Toolkit  Shared Class Library Reference   MfgEntityID   
 MfgInterfaceUsed   
 MultiSite   
 NewVatEnabled   
 POCostingSource   
 ProjectedWipCosting   
 PurcQuoteType   
 ServicedComponentType   (Overrides BusinessObject.ServicedComponentType.)  
 SiteID   
 StandardCosting   
 SystemCurrencyID   
 VatEnabled   
 WipVasEnabled   
 WithholdingEnabled   
 
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  

ApplGlobal.ActualCosting  Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 57 ApplGlobal.ActualCosting Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool ActualCosting { get; } 
 
VB 
Public  ReadOnly  Property  ActualCosting As Boolean  
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.ActualWipCosting Property  
58 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.ActualWipCosting Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool ActualWipCosting { get; } 
 
VB 
Public  ReadOnly  Property  ActualWipCosting As Boolean 
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.AddLocOnTheFly  Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 59 ApplGlobal.AddLocOnTheFly Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool AddLocOnTheFly  { get; } 
 
VB 
Public  ReadOnly  Property  AddLocOnTheFly  As Boolean 
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.APCostingSource  Property  
60 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.APCostingSource Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool APCostingSource { get; } 
 
VB 
Public  ReadOnly  Property  APCostingSource As Boolean 
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.BurdenFromResource  Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 61 ApplGlobal.BurdenFromResource Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool BurdenFromResource { get; } 
 
VB 
Public  ReadOnly  Property  BurdenFromResource As Boolean 
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.BurdenSource  Property  
62 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.BurdenSource Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  string  BurdenSource { get; } 
 
VB 
Public  ReadOnly  Property  BurdenSource As String  
 Get 
 
Property Value  
Type: String  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.CostingMethod Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 63 ApplGlobal.CostingMethod Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  string  CostingMethod { get; } 
 
VB 
Public  ReadOnly  Property  CostingMethod As String 
 Get 
 
Property Value  
Type: String  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.CountryID  Property  
64 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.CountryID Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  string  CountryID  { get; } 
 
VB 
Public  ReadOnly  Property  CountryID  As String  
 Get 
 
Property Value  
Type: String  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.CurrencyDataSet  Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 65 ApplGlobal.CurrencyDataSet Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataSet  CurrencyDataSet  { get; } 
 
VB 
Public  ReadOnly  Property  CurrencyDataSet  As DataSet  
 Get 
 
Property Value  
Type: DataSet  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.DataObjectType  Property  
66 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.DataObjectType Property  
Report types of data object associated with this business object  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  DataObjectType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  DataObjectType As Type 
 Get 
 
Property Value  
Type: Type  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.DDPEnabled Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 67 ApplGlobal.DDPEnabled Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool DDPEnabled { get; } 
 
VB 
Public  ReadOnly  Property  DDPEnabled  As Boolean 
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.DefLanguageID  Property  
68 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.DefLanguageID Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  string  DefLanguageID  { get; } 
 
VB 
Public  ReadOnly  Property  DefLanguageID  As String  
 Get 
 
Property Value  
Type: String  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.DeviationRequired Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 69 ApplGlobal.DeviationRequired Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool DeviationRequired { get; } 
 
VB 
Public  ReadOnly  Property  DeviationRequired As Boolean 
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.EntityID  Property  
70 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.EntityID Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  string  EntityID  { get; set; } 
 
VB 
Public  Property  EntityID  As String  
 Get 
 Set 
 
Property Value  
Type: String  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.euroCurrencyID  Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 71 ApplGlobal.euroCurrencyID Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  string  euroCurrencyID  { get; } 
 
VB 
Public  ReadOnly  Property  euroCurrencyID  As String 
 Get 
 
Property Value  
Type: String  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.exchangeRateDate Property  
72 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.exchangeRateDate Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  string  exchangeRateDate { get; } 
 
VB 
Public  ReadOnly  Property  exchangeRateDate As String  
 Get 
 
Property Value  
Type: String  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.FifoByLocation Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 73 ApplGlobal.FifoByLocation Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool FifoByLocation  { get; } 
 
VB 
Public  ReadOnly  Property  FifoByLocation As Boolean 
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.IntrastatEnabled Property  
74 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.IntrastatEnabled Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool IntrastatEnabled { get; } 
 
VB 
Public  ReadOnly  Property  IntrastatEnabled As Boolean  
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.IssueNegative Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 75 ApplGlobal.IssueNegative Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool IssueNegative  { get; } 
 
VB 
Public  ReadOnly  Property  IssueNegative As Boolean  
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.MfgEntityID  Property  
76 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.MfgEntityID Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  string  MfgEntityID  { get; } 
 
VB 
Public  ReadOnly  Property  MfgEntityID  As String  
 Get 
 
Property Value  
Type: String  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.MfgInterfaceUsed  Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 77 ApplGlobal.MfgInterfaceUsed Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool MfgInterfaceUsed { get; } 
 
VB 
Public  ReadOnly  Property  MfgInterfaceUsed As Boolean 
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.MultiSite  Property  
78 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.MultiSite Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool MultiSite  { get; } 
 
VB 
Public  ReadOnly  Property  MultiSite  As Boolean  
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.NewVatEnabled  Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 79 ApplGlobal.NewVatEnabled Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool NewVatEnabled  { get; } 
 
VB 
Public  ReadOnly  Property  NewVatEnabled As Boolean  
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.POCostingSource  Property  
80 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.POCostingSource Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool POCostingSource { get; } 
 
VB 
Public  ReadOnly  Property  POCostingSource As Boolean 
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.ProjectedWipCosting Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 81 ApplGlobal.ProjectedWipCosting Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool ProjectedWipCosting { get; } 
 
VB 
Public  ReadOnly  Property  ProjectedWipCosting  As Boolean 
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.PurcQuoteType  Property  
82 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.PurcQuoteType Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  string  PurcQuoteType { get; } 
 
VB 
Public  ReadOnly  Property  PurcQuoteType As String 
 Get 
 
Property Value  
Type: String  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.ServicedComponentType  Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 83 ApplGlobal.ServicedComponentType Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  ServicedComponentType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  ServicedComponentType As Type  
 Get 
 
Property Value  
Type: Type  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.SiteID  Property  
84 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.SiteID Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  string  SiteID  { get; set; } 
 
VB 
Public  Property  SiteID  As String  
 Get 
 Set 
 
Property Value  
Type: String  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.StandardCosting Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 85 ApplGlobal.StandardCosting Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool StandardCosting { get; } 
 
VB 
Public  ReadOnly  Property  StandardCosting As Boolean  
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.SystemCurrencyID  Property  
86 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.SystemCurrencyID Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  string  SystemCurrencyID { get; } 
 
VB 
Public  ReadOnly  Property  SystemCurrencyID As String  
 Get 
 
Property Value  
Type: String  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.VatEnabled Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 87 ApplGlobal.VatEnabled Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool VatEnabled { get; } 
 
VB 
Public  ReadOnly  Property  VatEnabled As Boolean  
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.WipVasEnabled Property  
88 | Infor VISUAL API Toolkit  Shared Class Library Reference  ApplGlobal.WipVasEnabled Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool WipVasEnabled { get; } 
 
VB 
Public  ReadOnly  Property  WipVasEnabled As Boolean 
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
ApplGlobal.WithholdingEnabled  Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 89 ApplGlobal.WithholdingEnabled Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  bool WithholdingEnabled { get; } 
 
VB 
Public  ReadOnly  Property  WithholdingEnabled  As Boolean  
 Get 
 
Property Value  
Type: Boolean  
See Also 
ApplGlobal Class  
Lsa.Vmfg.Shared Namespace  
  
Carrier Class  
90 | Infor VISUAL API Toolkit  Shared Class Library Reference  Carrier Class  
Maintain Carriers.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Carrier  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  Carrier : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  Carrier  
 Inherits  BusinessDocument  
 
The Carrier type exposes the following members.  
Constructors 
 Name  Description  
 Carrier()  Initializes a new instance of the Carrier class 
 Carrier(String)  Initializes a new instance of the Carrier class 
 

Carrier Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 91 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve existing Carriers based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve existing Carriers, based on search criteria, limited 
by record count.  
 Exists  Determine if a Carrier exists.  
 Find Find a specific Carrier.  
 Load()  Load all Carriers.  
 Load(String)  Load a specific Carrier row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewCarrierRow  Adds a new row to the CARRIER table.  
 Save  Save Carrier(s) to the datbase.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Carrier Constructor  
92 | Infor VISUAL API Toolkit  Shared Class Library Reference  Carrier Constructor  
Overload List 
 Name  Description  
 Carrier()  Initializes a new instance of the Carrier  class 
 Carrier(String)  Initializes a new instance of the Carrier  class 
 
See Also 
Carrier Class  
Lsa.Vmfg.Shared Namespace  
  

Carrier Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 93 Carrier Constructor  
Initializes a new instance of the Carrier  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Carrier()  
 
VB 
Public  Sub New 
 
See Also 
Carrier Class  
Carrier Overload  
Lsa.Vmfg.Shared Namespace  
  
Carrier Constructor  (String) 
94 | Infor VISUAL API Toolkit  Shared Class Library Reference  Carrier Constructor (String)  
Initializes a new instance of the Carrier  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Carrier( 
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
Carrier Class  
Carrier Overload  
Lsa.Vmfg.Shared Namespace  
  
Carrier.Carrier  Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 95 Carrier.Carrier Methods  
The Carrier  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve existing Carriers based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve existing Carriers, based on search criteria, limited 
by record count.  
 Exists  Determine if a Carrier exists.  
 Find Find a specific Carrier.  
 Load()  Load all Carriers.  
 Load(String)  Load a specific Carrier row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewCarrierRow  Adds a new row to the CARRIER table.  
 Save  Save Carrier(s) to the datbase.  
 
See Also 
Carrier Class  
Lsa.Vmfg.Shared Namespace  
  

Carrier.Browse  Method  
96 | Infor VISUAL API Toolkit  Shared Class Library Reference  Carrier.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve existing Carriers based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve existing Carriers, based on search criteria, limited 
by record count.  
 
See Also 
Carrier Class  
Lsa.Vmfg.Shared Namespace  
  

Carrier.Browse  Method (String, String, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 97 Carrier.Browse Method (String, String, String)  
Retrieve existing Carriers based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Carrier Class  
Carrier.Browse  Method (String, String, String)  
98 | Infor VISUAL API Toolkit  Shared Class Library Reference  Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Carrier.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 99 Carrier.Browse Method (String, String, String, Int32, 
Int32)  
Retrieve existing Carriers, based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Carrier.Browse  Method (String, String, String, Int32, Int32)  
100 | Infor VISUAL API Toolkit  Shared Class Library Reference  Return Value  
Type: DataSet  
See Also 
Carrier Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Carrier.Exists Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 101 Carrier.Exists Method  
Determine if a Carrier exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Carrier Class  
Lsa.Vmfg.Shared Namespace  
  
Carrier.Find  Method  
102 | Infor VISUAL API Toolkit  Shared Class Library Reference  Carrier.Find Method  
Find a specific Carrier.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Carrier Class  
Lsa.Vmfg.Shared Namespace  
  
Carrier.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 103 Carrier.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Carriers.  
 Load(String)  Load a specific Carrier row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
Carrier Class  
Lsa.Vmfg.Shared Namespace  
  

Carrier.Load  Method  
104 | Infor VISUAL API Toolkit  Shared Class Library Reference  Carrier.Load Method  
Load all Carriers.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
Carrier Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Carrier.Load  Method (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 105 Carrier.Load Method (String)  
Load a specific Carrier row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Carrier Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Carrier.Load  Method (Stream, String)  
106 | Infor VISUAL API Toolkit  Shared Class Library Reference  Carrier.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Carrier Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Carrier.NewCarrierRow  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 107 Carrier.NewCarrierRow Method  
Adds a new row to the CARRIER table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewCarrierRow ( 
 string  id 
) 
 
VB 
Public  Function  NewCarrierRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Carrier Class  
Lsa.Vmfg.Shared Namespace  
  
Carrier.Save  Method  
108 | Infor VISUAL API Toolkit  Shared Class Library Reference  Carrier.Save Method  
Save Carrier(s) to the datbase.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Carrier Class  
Lsa.Vmfg.Shared Namespace  
  
CommodityCode Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 109 CommodityCode Class  
Maintain Commodity Codes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.CommodityCode  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  CommodityCode  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  CommodityCode 
 Inherits  BusinessDocument  
 
The CommodityCode  type exposes the following members.  
Constructors 
 Name  Description  
 CommodityCode()  Initializes a new instance of the CommodityCode  class 
 CommodityCode(String)  Initializes a new instance of the CommodityCode  class 
 

CommodityCode Class  
110 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Commodity Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Commodity Codes, based on search criteria, 
limited by record count.  
 Exists  Determine if a Commodity Code exists.  
 Find Find a specific Commodity Code.  
 Load()  Load all Commodity Codes.  
 Load(String)  Load a specific Commodity Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewCommodityRow  Adds a new row to the COMMODITY table.  
 Save  Save Commodity Code(s) to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

CommodityCode Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 111 CommodityCode Constructor  
Overload List 
 Name  Description  
 CommodityCode()  Initializes a new instance of the CommodityCode  class 
 CommodityCode(String)  Initializes a new instance of the CommodityCode  class 
 
See Also 
CommodityCode Class  
Lsa.Vmfg.Shared Namespace  
  

CommodityCode Constructor  
112 | Infor VISUAL API Toolkit  Shared Class Library Reference  CommodityCode Constructor  
Initializes a new instance of the CommodityCode  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  CommodityCode()  
 
VB 
Public  Sub New 
 
See Also 
CommodityCode Class  
CommodityCode Overload  
Lsa.Vmfg.Shared Namespace  
  
CommodityCode Constructor  (String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 113 CommodityCode Constructor (String)  
Initializes a new instance of the CommodityCode  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  CommodityCode( 
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
CommodityCode Class  
CommodityCode Overload  
Lsa.Vmfg.Shared Namespace  
  
CommodityCode.CommodityCode  Methods  
114 | Infor VISUAL API Toolkit  Shared Class Library Reference  CommodityCode.CommodityCode Methods  
The CommodityCode  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Commodity Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Commodity Codes, based on search criteria, 
limited by record count.  
 Exists  Determine if a Commodity Code exists.  
 Find Find a specific Commodity Code.  
 Load()  Load all Commodity Codes.  
 Load(String)  Load a specific Commodity Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewCommodityRow  Adds a new row to the COMMODITY table.  
 Save  Save Commodity Code(s) to the database.  
 
See Also 
CommodityCode Class  
Lsa.Vmfg.Shared Namespace  
  

CommodityCode.Browse  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 115 CommodityCode.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Commodity Codes based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Commodity Codes, based on search criteria, 
limited by record count.  
 
See Also 
CommodityCode Class  
Lsa.Vmfg.Shared Namespace  
  

CommodityCode.Browse  Method (String, String, String) 
116 | Infor VISUAL API Toolkit  Shared Class Library Reference  CommodityCode.Browse Method (String, String, 
String)  
Retrieve Commodity Codes based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
CommodityCode.Browse  Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 117 See Also  
CommodityCode Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
CommodityCode.Browse  Method (String, String, String, Int32, Int32)  
118 | Infor VISUAL API Toolkit  Shared Class Library Reference  CommodityCode.Browse Method (String, String, 
String, Int32, Int32)  
Retrieve Commodity Codes, based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
CommodityCode.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 119 Return Value  
Type: DataSet  
See Also 
CommodityCode Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
CommodityCode.Exists Method  
120 | Infor VISUAL API Toolkit  Shared Class Library Reference  CommodityCode.Exists Method  
Determine if a Commodity Code exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  code 
) 
 
VB 
Public  Overridable  Function Exists  (  
 code As String  
) As Boolean 
 
Parameters  
code  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
CommodityCode Class  
Lsa.Vmfg.Shared Namespace  
  
CommodityCode.Find  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 121 CommodityCode.Find Method  
Find a specific Commodity Code.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Find (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
CommodityCode Class  
Lsa.Vmfg.Shared Namespace  
  
CommodityCode.Load Method  
122 | Infor VISUAL API Toolkit  Shared Class Library Reference  CommodityCode.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Commodity Codes.  
 Load(String)  Load a specific Commodity Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
CommodityCode Class  
Lsa.Vmfg.Shared Namespace  
  

CommodityCode.Load Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 123 CommodityCode.Load Method  
Load all Commodity Codes.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
CommodityCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
CommodityCode.Load Method (String)  
124 | Infor VISUAL API Toolkit  Shared Class Library Reference  CommodityCode.Load Method (String)  
Load a specific Commodity Code row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
CommodityCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
CommodityCode.Load Method (Stream, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 125 CommodityCode.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 code As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
code  
Type: System.String  
See Also 
CommodityCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
CommodityCode.NewCommodityRow  Method 
126 | Infor VISUAL API Toolkit  Shared Class Library Reference  CommodityCode.NewCommodityRow Method  
Adds a new row to the COMMODITY table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewCommodityRow ( 
 string  code 
) 
 
VB 
Public  Function  NewCommodityRow  (  
 code As String  
) As DataRow  
 
Parameters  
code  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
CommodityCode Class  
Lsa.Vmfg.Shared Namespace  
  
CommodityCode.Save Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 127 CommodityCode.Save Method  
Save Commodity Code(s) to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
CommodityCode Class  
Lsa.Vmfg.Shared Namespace  
  
Contact Class 
128 | Infor VISUAL API Toolkit  Shared Class Library Reference  Contact Class  
Maintain Contacts.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Contact  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  Contact  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  Contact  
 Inherits  BusinessDocument  
 
The Contact  type exposes the following members.  
Constructors 
 Name  Description  
 Contact()  Constructor  
 Contact(String)  Constructor  
 

Contact Class 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 129 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieves Contact rows based on provided search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieves Contact rows based on provided search criteria and 
limited by startRecord and maxRecord parameters.  
 Exists  Determines if a given Contact ID exists in the database.  
 Find Retrieve a contact using the key provided.  
 Load(String)  Loads the Contact data hierarchy for the contactID provided.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewContactRow  Inserts a new row into the CONTACT table.  
 Save()  Saves changes to a previously loaded contact to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Contact Constructor  
130 | Infor VISUAL API Toolkit  Shared Class Library Reference  Contact Constructor  
Overload List 
 Name  Description  
 Contact()  Constructor  
 Contact(String)  Constructor  
 
See Also 
Contact Class  
Lsa.Vmfg.Shared Namespace  
  

Contact Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 131 Contact Constructor  
Constructor  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Contact () 
 
VB 
Public  Sub New 
 
See Also 
Contact Class  
Contact Overload  
Lsa.Vmfg.Shared Namespace  
  
Contact Constructor  (String)  
132 | Infor VISUAL API Toolkit  Shared Class Library Reference  Contact Constructor (String)  
Constructor  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Contact ( 
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
Contact Class  
Contact Overload  
Lsa.Vmfg.Shared Namespace  
  
Contact.Contact  Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 133 Contact.Contact Methods  
The Contact  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieves Contact rows based on provided search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieves Contact rows based on provided search criteria and 
limited by startRecord and maxRecord parameters.  
 Exists  Determines if a given Contact ID exists in the database.  
 Find Retrieve a contact using the key provided.  
 Load(String)  Loads the Contact data hierarchy for the contactID provided.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewContactRow  Inserts a new row into the CONTACT table.  
 Save()  Saves changes to a previously loaded contact to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
See Also 
Contact Class  
Lsa.Vmfg.Shared Namespace  
  

Contact.Browse Method  
134 | Infor VISUAL API Toolkit  Shared Class Library Reference  Contact.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, 
String)  Retrieves Contact rows based on provided search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieves Contact rows based on provided search criteria and 
limited by startRecord and maxRecord parameters.  
 
See Also 
Contact Class  
Lsa.Vmfg.Shared Namespace  
  

Contact.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 135 Contact.Browse Method (String, String, String)  
Retrieves Contact rows based on provided search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Contact Class  
Contact.Browse Method (String, String, String)  
136 | Infor VISUAL API Toolkit  Shared Class Library Reference  Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Contact.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 137 Contact.Browse Method (String, String, String, Int32, 
Int32)  
Retrieves Contact rows based on provided search criteria and limited by startRecord and 
maxRecord parameters.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Contact.Browse Method (String, String, String, Int32, Int32)  
138 | Infor VISUAL API Toolkit  Shared Class Library Reference  Type: System.Int32  
Return Value  
Type: DataSet  
 
See Also 
Contact Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Contact.Exists Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 139 Contact.Exists Method  
Determines if a given Contact ID exists in the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  contactID  
) 
 
VB 
Public  Overridable  Function Exists  (  
 contactID  As String  
) As Boolean 
 
Parameters  
contactID  
Type: System.String  
Return Value  
Type: Boolean  
 
See Also 
Contact Class  
Lsa.Vmfg.Shared Namespace  
  
Contact.Find Method  
140 | Infor VISUAL API Toolkit  Shared Class Library Reference  Contact.Find Method  
Retrieve a contact using the key provided.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  contactID  
) 
 
VB 
Public  Overridable  Sub Find (  
 contactID  As String  
) 
 
Parameters  
contactID  
Type: System.String  
Remarks 
Only the top- level table is retrieved.  
See Also 
Contact Class  
Lsa.Vmfg.Shared Namespace  
  
Contact.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 141 Contact.Load Method  
Overload List 
 Name  Description  
 Load(String)  Loads the Contact data hierarchy for the contactID provided.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
Contact Class  
Lsa.Vmfg.Shared Namespace  
  

Contact.Load  Method (String)  
142 | Infor VISUAL API Toolkit  Shared Class Library Reference  Contact.Load Method (String)  
Loads the Contact data hierarchy for the contactID provided.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  contactID  
) 
 
VB 
Public  Overridable  Sub Load (  
 contactID  As String  
) 
 
Parameters  
contactID  
Type: System.String  
See Also 
Contact Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Contact.Load  Method (Stream, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 143 Contact.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  contactID  
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 contactID  As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
contactID  
Type: System.String  
See Also 
Contact Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Contact.NewContactRow  Method  
144 | Infor VISUAL API Toolkit  Shared Class Library Reference  Contact.NewContactRow Method  
Inserts a new row into the CONTACT table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewContactRow ( 
 string  contactID  
) 
 
VB 
Public  Overridable  Function NewContactRow  (  
 contactID  As String  
) As DataRow  
 
Parameters  
contactID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Contact Class  
Lsa.Vmfg.Shared Namespace  
  
Contact.Save Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 145 Contact.Save Method  
Overload List 
 Name  Description  
 Save()  Saves changes to a previously loaded contact to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
See Also 
Contact Class  
Lsa.Vmfg.Shared Namespace  
  

Contact.Save Method  
146 | Infor VISUAL API Toolkit  Shared Class Library Reference  Contact.Save Method  
Saves changes to a previously loaded contact to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Contact Class  
Save Overload  
Lsa.Vmfg.Shared Namespace  
  
Contact.Save Method (Stream)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 147 Contact.Save Method (Stream)  
Save current state of data set to stream.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Contact Class  
Save Overload  
Lsa.Vmfg.Shared Namespace  
  
Country Class  
148 | Infor VISUAL API Toolkit  Shared Class Library Reference  Country Class  
Maintain Countries.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Country  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  Country  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  Country  
 Inherits  BusinessDocument  
 
The Country  type exposes the following members.  
Constructors 
 Name  Description  
 Country()  Initializes a new instance of the Country  class 
 Country(String)  Initializes a new instance of the Country  class 
 

Country Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 149 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Contries based on provided search criteria.  
 Browse(String, String, String, Int32, Int32)   
 Exists  Determine if a Country exists.  
 Find Find a Country.  
 Load()  Load all Countries.  
 Load(String)  Load a specific Country row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewCountryRow  Inserts a new row into the COUNTRY table.  
 Save  Save loaded Country or Countries to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Country Constructor  
150 | Infor VISUAL API Toolkit  Shared Class Library Reference  Country Constructor  
Overload List 
 Name  Description  
 Country()  Initializes a new instance of the Country  class 
 Country(String)  Initializes a new instance of the Country  class 
 
See Also 
Country Class  
Lsa.Vmfg.Shared Namespace  
  

Country Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 151 Country Constructor  
Initializes a new instance of the Country  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Country () 
 
VB 
Public  Sub New 
 
See Also 
Country Class  
Country Overload  
Lsa.Vmfg.Shared Namespace  
  
Country Constructor  (String) 
152 | Infor VISUAL API Toolkit  Shared Class Library Reference  Country Constructor (String) 
Initializes a new instance of the Country  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Country ( 
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
Country Class  
Country Overload  
Lsa.Vmfg.Shared Namespace  
  
Country.Country  Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 153 Country.Country Methods  
The Country  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Contries based on provided search criteria.  
 Browse(String, String, String, Int32, Int32)   
 Exists  Determine if a Country exists.  
 Find Find a Country.  
 Load()  Load all Countries.  
 Load(String)  Load a specific Country row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewCountryRow  Inserts a new row into the COUNTRY table.  
 Save  Save loaded Country or Countries to the database.  
 
See Also 
Country Class  
Lsa.Vmfg.Shared Namespace  
  

Country.Browse  Method  
154 | Infor VISUAL API Toolkit  Shared Class Library Reference  Country.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Contries based on provided search criteria.  
 Browse(String, String, String, Int32, Int32)   
 
See Also 
Country Class  
Lsa.Vmfg.Shared Namespace  
  

Country.Browse  Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 155 Country.Browse Method (String, String, String)  
Retrieve Contries based on provided search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
 
Country.Browse  Method (String, String, String) 
156 | Infor VISUAL API Toolkit  Shared Class Library Reference  See Also 
Country Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Country.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 157 Country.Browse Method (String, String, String, Int32, 
Int32)  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Country.Browse  Method (String, String, String, Int32, Int32)  
158 | Infor VISUAL API Toolkit  Shared Class Library Reference  Return Value  
Type: DataSet  
See Also 
Country Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Country.Exists  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 159 Country.Exists Method  
Determine if a Country exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  countryID  
) 
 
VB 
Public  Overridable  Function Exists  (  
 countryID  As String  
) As Boolean 
 
Parameters  
countryID  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
Country Class  
Lsa.Vmfg.Shared Namespace  
  
Country.Find  Method 
160 | Infor VISUAL API Toolkit  Shared Class Library Reference  Country.Find Method  
Find a Country.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  countryID  
) 
 
VB 
Public  Overridable  Sub Find (  
 countryID  As String  
) 
 
Parameters  
countryID  
Type: System.String  
See Also 
Country Class  
Lsa.Vmfg.Shared Namespace  
  
Country.Load Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 161 Country.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Countries.  
 Load(String)  Load a specific Country row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
Country Class  
Lsa.Vmfg.Shared Namespace  
  

Country.Load Method  
162 | Infor VISUAL API Toolkit  Shared Class Library Reference  Country.Load Method  
Load all Countries.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
Country Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Country.Load Method (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 163 Country.Load Method (String)  
Load a specific Country row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  countryID  
) 
 
VB 
Public  Overridable  Sub Load (  
 countryID  As String  
) 
 
Parameters  
countryID  
Type: System.String  
See Also 
Country Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Country.Load Method (Stream, String)  
164 | Infor VISUAL API Toolkit  Shared Class Library Reference  Country.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  countryID  
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 countryID  As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
countryID  
Type: System.String  
See Also 
Country Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Country.NewCountryRow  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 165 Country.NewCountryRow Method  
Inserts a new row into the COUNTRY table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewCountryRow ( 
 string  countryID  
) 
 
VB 
Public  Function  NewCountryRow  (  
 countryID  As String  
) As DataRow  
 
Parameters  
countryID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Country Class  
Lsa.Vmfg.Shared Namespace  
  
Country.Save  Method 
166 | Infor VISUAL API Toolkit  Shared Class Library Reference  Country.Save Method  
Save loaded Country or Countries to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Country Class  
Lsa.Vmfg.Shared Namespace  
  
Currency Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 167 Currency Class  
Maintain Currencies.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Currency  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  class  Currency  : BusinessDocument  
 
VB 
Public  Class  Currency  
 Inherits  BusinessDocument  
 
The Currency  type exposes the following members.  
Constructors 
 Name  Description  
 Currency()  Initializes a new instance of the Currency  class 
 Currency(String)  Initializes a new instance of the Currency  class 
 

Currency Class  
168 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Currencies based on search criteria  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Currencies based on search criteria, limited by 
record count  
 Exists  Determine if a specific Currency exists  
 Find Find a specific Currency  
 Load(String)  Load by Currency ID  
 Load(Stream, String)  Load by stream  
 Load(String, String)  Load by Currency ID and Entity ID  
 NewCurrencyEntityRow  Inserts a new row into the CURRENCY_ENTITY table.  
 NewCurrencyExchangeRow  Inserts a new row into the CURRENCY_EXCHANGE table.  
 NewCurrencyRow  Inserts a new row into the CURRENCY table.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Currency Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 169 Currency Constructor  
Overload List 
 Name  Description  
 Currency()  Initializes a new instance of the Currency  class 
 Currency(String)  Initializes a new instance of the Currency  class 
 
See Also 
Currency Class  
Lsa.Vmfg.Shared Namespace  
  

Currency Constructor  
170 | Infor VISUAL API Toolkit  Shared Class Library Reference  Currency Constructor  
Initializes a new instance of the Currency  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Currency()  
 
VB 
Public  Sub New 
 
See Also 
Currency Class  
Currency Overload  
Lsa.Vmfg.Shared Namespace  
  
Currency Constructor  (String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 171 Currency Constructor (String)  
Initializes a new instance of the Currency  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Currency ( 
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
Currency Class  
Currency Overload  
Lsa.Vmfg.Shared Namespace  
  
Currency.Currency Methods  
172 | Infor VISUAL API Toolkit  Shared Class Library Reference  Currency.Currency Methods  
The Currency  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Currencies based on search criteria  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Currencies based on search criteria, limited by 
record count  
 Exists  Determine if a specific Currency exists  
 Find Find a specific Currency  
 Load(String)  Load by Currency ID  
 Load(Stream, String)  Load by stream  
 Load(String, String)  Load by Currency ID and Entity ID  
 NewCurrencyEntityRow  Inserts a new row into the CURRENCY_ENTITY table.  
 NewCurrencyExchangeRow  Inserts a new row into the CURRENCY_EXCHANGE table.  
 NewCurrencyRow  Inserts a new row into the CURRENCY table.  
 
See Also 
Currency Class  
Lsa.Vmfg.Shared Namespace  
  

Currency.Browse Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 173 Currency.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Currencies based on search criteria  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Currencies based on search criteria, limited by 
record count  
 
See Also 
Currency Class  
Lsa.Vmfg.Shared Namespace  
  

Currency.Browse Method (String, String, String) 
174 | Infor VISUAL API Toolkit  Shared Class Library Reference  Currency.Browse Method (String, String, String)  
Retrieve Currencies based on search criteria  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
 
Currency.Browse Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 175 See Also 
Currency Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Currency.Browse Method (String, String, String, Int32, Int32)  
176 | Infor VISUAL API Toolkit  Shared Class Library Reference  Currency.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve Currencies based on search criteria, limited by record count  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Currency.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 177 Return Value  
Type: DataSet  
 
See Also 
Currency Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Currency.Exists Method 
178 | Infor VISUAL API Toolkit  Shared Class Library Reference  Currency.Exists Method  
Determine if a specific Currency exists  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  currencyID  
) 
 
VB 
Public  Overridable  Function Exists  (  
 currencyID  As String 
) As Boolean 
 
Parameters  
currencyID  
Type: System.String  
Return Value  
Type: Boolean  
 
See Also 
Currency Class  
Lsa.Vmfg.Shared Namespace  
  
Currency.Find  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 179 Currency.Find Method  
Find a specific Currency  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  currencyID  
) 
 
VB 
Public  Overridable  Sub Find (  
 currencyID  As String 
) 
 
Parameters  
currencyID  
Type: System.String  
See Also 
Currency Class  
Lsa.Vmfg.Shared Namespace  
  
Currency.Load  Method  
180 | Infor VISUAL API Toolkit  Shared Class Library Reference  Currency.Load Method  
Overload List 
 Name  Description  
 Load(String)  Load by Currency ID  
 Load(Stream, String)  Load by stream  
 Load(String, String)  Load by Currency ID and Entity ID  
 
See Also 
Currency Class  
Lsa.Vmfg.Shared Namespace  
  

Currency.Load  Method (String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 181 Currency.Load Method (String)  
Load by Currency ID  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  currencyID  
) 
 
VB 
Public  Overridable  Sub Load (  
 currencyID  As String 
) 
 
Parameters  
currencyID  
Type: System.String  
See Also 
Currency Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Currency.Load  Method (Stream, String)  
182 | Infor VISUAL API Toolkit  Shared Class Library Reference  Currency.Load Method (Stream, String)  
Load by stream  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  currencyID  
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 currencyID  As String 
) 
 
Parameters  
stream  
Type: System.IO.Stream  
currencyID  
Type: System.String  
See Also 
Currency Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Currency.Load  Method (String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 183 Currency.Load Method (String, String)  
Load by Currency ID and Entity ID  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  currencyID , 
 string  entityID  
) 
 
VB 
Public  Overridable  Sub Load (  
 currencyID  As String , 
 entityID  As String  
) 
 
Parameters  
currencyID  
Type: System.String  
entityID  
Type: System.String  
See Also 
Currency Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Currency.NewCurrencyEntityRow  Method  
184 | Infor VISUAL API Toolkit  Shared Class Library Reference  Currency.NewCurrencyEntityRow Method  
Inserts a new row into the CURRENCY_ENTITY table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewCurrencyEntityRow ( 
 string  currencyID , 
 string  entityID  
) 
 
VB 
Public  Overridable  Function NewCurrencyEntityRow  (  
 currencyID  As String , 
 entityID  As String  
) As DataRow  
 
Parameters  
currencyID  
Type: System.String  
entityID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Currency Class  
Lsa.Vmfg.Shared Namespace  
  
Currency.NewCurrencyExchangeRow  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 185 Currency.NewCurrencyExchangeRow Method  
Inserts a new row into the CURRENCY_EXCHANGE table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewCurrencyExchangeRow ( 
 string  currencyID , 
 DateTime  effectiveDate , 
 string  entityID  
) 
 
VB 
Public  Overridable  Function NewCurrencyExchangeRow  (  
 currencyID  As String , 
 effectiveDate  As DateTime , 
 entityID  As String  
) As DataRow  
 
Parameters  
currencyID  
Type: System.String  
effectiveDate  
Type: System.DateTime  
entityID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Currency Class  
Currency.NewCurrencyExchangeRow  Method 
186 | Infor VISUAL API Toolkit  Shared Class Library Reference  Lsa.Vmfg.Shared Namespace  
  
Currency.NewCurrencyRow  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 187 Currency.NewCurrencyRow Method  
Inserts a new row into the CURRENCY table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewCurrencyRow ( 
 string  currencyID  
) 
 
VB 
Public  Overridable  Function NewCurrencyRow  (  
 currencyID  As String 
) As DataRow  
 
Parameters  
currencyID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Currency Class  
Lsa.Vmfg.Shared Namespace  
  
Department Class  
188 | Infor VISUAL API Toolkit  Shared Class Library Reference  Department Class  
Maintan Departments.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Department  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  Department  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  Department  
 Inherits  BusinessDocument  
 
The Department type exposes the following members.  
Constructors 
 Name  Description  
 Department()  Initializes a new instance of the Department class 
 Department(String)  Initializes a new instance of the Department class 
 

Department Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 189 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Departments based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Departments based on search criteria, limited by 
record count.  
 Exists  Determine if a Department exists.  
 Find Find a specific Department.  
 Load()  Load all Departments.  
 Load(String)  Load a specific Department row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewDepartmentRow  Adds a row to the DEPARTMENT table.  
 Save  Save previously loaded Department(s) to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Department Constructor  
190 | Infor VISUAL API Toolkit  Shared Class Library Reference  Department Constructor  
Overload List 
 Name  Description  
 Department()  Initializes a new instance of the Department  class 
 Department(String)  Initializes a new instance of the Department  class 
 
See Also 
Department Class  
Lsa.Vmfg.Shared Namespace  
  

Department Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 191 Department Constructor  
Initializes a new instance of the Department  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Department () 
 
VB 
Public  Sub New 
 
See Also 
Department Class  
Department Overload  
Lsa.Vmfg.Shared Namespace  
  
Department Constructor  (String) 
192 | Infor VISUAL API Toolkit  Shared Class Library Reference  Department Constructor (String)  
Initializes a new instance of the Department  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Department ( 
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
Department Class  
Department Overload  
Lsa.Vmfg.Shared Namespace  
  
Department.Department Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 193 Department.Department Methods  
The Department  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Departments based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Departments based on search criteria, limited by 
record count.  
 Exists  Determine if a Department exists.  
 Find Find a specific Department.  
 Load()  Load all Departments.  
 Load(String)  Load a specific Department row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewDepartmentRow  Adds a row to the DEPARTMENT table.  
 Save  Save previously loaded Department(s) to the database.  
 
See Also 
Department Class  
Lsa.Vmfg.Shared Namespace  
  

Department.Browse  Method 
194 | Infor VISUAL API Toolkit  Shared Class Library Reference  Department.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Departments based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Departments based on search criteria, limited by 
record count.  
 
See Also 
Department Class  
Lsa.Vmfg.Shared Namespace  
  

Department.Browse  Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 195 Department.Browse Method (String, String, String)  
Retrieve Departments based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Department Class  
Department.Browse  Method (String, String, String) 
196 | Infor VISUAL API Toolkit  Shared Class Library Reference  Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Department.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 197 Department.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve Departments based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Department.Browse  Method (String, String, String, Int32, Int32)  
198 | Infor VISUAL API Toolkit  Shared Class Library Reference  Return Value  
Type: DataSet  
See Also 
Department Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Department.Exists Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 199 Department.Exists Method  
Determine if a Department exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  departmentID  
) 
 
VB 
Public  Overridable  Function Exists  (  
 departmentID  As String  
) As Boolean 
 
Parameters  
departmentID  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
Department Class  
Lsa.Vmfg.Shared Namespace  
  
Department.Find  Method 
200 | Infor VISUAL API Toolkit  Shared Class Library Reference  Department.Find Method  
Find a specific Department.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Department Class  
Lsa.Vmfg.Shared Namespace  
  
Department.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 201 Department.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Departments.  
 Load(String)  Load a specific Department row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
Department Class  
Lsa.Vmfg.Shared Namespace  
  

Department.Load  Method  
202 | Infor VISUAL API Toolkit  Shared Class Library Reference  Department.Load Method  
Load all Departments.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
Department Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Department.Load  Method (String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 203 Department.Load Method (String)  
Load a specific Department row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Department Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Department.Load  Method (Stream, String)  
204 | Infor VISUAL API Toolkit  Shared Class Library Reference  Department.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Department Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Department.NewDepartmentRow  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 205 Department.NewDepartmentRow Method  
Adds a row to the DEPARTMENT table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewDepartmentRow ( 
 string  id 
) 
 
VB 
Public  Function  NewDepartmentRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Department Class  
Lsa.Vmfg.Shared Namespace  
  
Department.Save Method 
206 | Infor VISUAL API Toolkit  Shared Class Library Reference  Department.Save Method  
Save previously loaded Department(s) to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Department Class  
Lsa.Vmfg.Shared Namespace  
  
DeviationReason Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 207 DeviationReason Class  
Maintan Deviation Reason Codes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.DeviationReason  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  DeviationReason : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  DeviationReason  
 Inherits  BusinessDocument  
 
The DeviationReason  type exposes the following members.  
Constructors 
 Name  Description  
 DeviationReason()  Initializes a new instance of the DeviationReason  class 
 DeviationReason(String)  Initializes a new instance of the DeviationReason  class 
 

DeviationReason Class  
208 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Deviation Reasons based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Deviation Reasons based on search criteria, limited 
by record count.  
 Exists  Determine if a Deviation Reason exists.  
 Find Find a sepcific Deviation Reason.  
 Load()  Load all Deviation Reasons.  
 Load(String)  Load a specific Deviation Reason row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewDeviationRow  Adds a new row to the DEVIATION_REASON table.  
 Save  Save previously loaded Deviation Reason(s) to the 
database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

DeviationReason Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 209 DeviationReason Constructor  
Overload List 
 Name  Description  
 DeviationReason()  Initializes a new instance of the DeviationReason  class 
 DeviationReason(String)  Initializes a new instance of the DeviationReason  class 
 
See Also 
DeviationReason Class  
Lsa.Vmfg.Shared Namespace  
  

DeviationReason Constructor  
210 | Infor VISUAL API Toolkit  Shared Class Library Reference  DeviationReason Constructor  
Initializes a new instance of the DeviationReason  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DeviationReason () 
 
VB 
Public  Sub New 
 
See Also 
DeviationReason Class  
DeviationReason Overload  
Lsa.Vmfg.Shared Namespace  
  
DeviationReason Constructor  (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 211 DeviationReason Constructor (String)  
Initializes a new instance of the DeviationReason  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DeviationReason ( 
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
DeviationReason Class  
DeviationReason Overload  
Lsa.Vmfg.Shared Namespace  
  
DeviationReason.DeviationReason  Methods  
212 | Infor VISUAL API Toolkit  Shared Class Library Reference  DeviationReason.DeviationReason Methods  
The DeviationReason  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Deviation Reasons based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Deviation Reasons based on search criteria, limited 
by record count.  
 Exists  Determine if a Deviation Reason exists.  
 Find Find a sepcific Deviation Reason.  
 Load()  Load all Deviation Reasons.  
 Load(String)  Load a specific Deviation Reason row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewDeviationRow  Adds a new row to the DEVIATION_REASON table.  
 Save  Save previously loaded Deviation Reason(s) to the 
database.  
 
See Also 
DeviationReason Class  
Lsa.Vmfg.Shared Namespace  
  

DeviationReason.Browse Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 213 DeviationReason.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Deviation Reasons based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Deviation Reasons based on search criteria, 
limited by record count.  
 
See Also 
DeviationReason Class  
Lsa.Vmfg.Shared Namespace  
  

DeviationReason.Browse Method (String, String, String) 
214 | Infor VISUAL API Toolkit  Shared Class Library Reference  DeviationReason.Browse Method (String, String, 
String)  
Retrieve Deviation Reasons based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
DeviationReason.Browse Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 215 See Also 
DeviationReason Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
DeviationReason.Browse Method (String, String, String, Int32, Int32)  
216 | Infor VISUAL API Toolkit  Shared Class Library Reference  DeviationReason.Browse Method (String, String, 
String, Int32, Int32)  
Retrieve Deviation Reasons based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
DeviationReason.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 217 Return Value  
Type: DataSet  
See Also 
DeviationReason Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
DeviationReason.Exists  Method  
218 | Infor VISUAL API Toolkit  Shared Class Library Reference  DeviationReason.Exists Method  
Determine if a Deviation Reason exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
DeviationReason Class  
Lsa.Vmfg.Shared Namespace  
  
DeviationReason.Find  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 219 DeviationReason.Find Method  
Find a sepcific Deviation Reason.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
DeviationReason Class  
Lsa.Vmfg.Shared Namespace  
  
DeviationReason.Load  Method 
220 | Infor VISUAL API Toolkit  Shared Class Library Reference  DeviationReason.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Deviation Reasons.  
 Load(String)  Load a specific Deviation Reason row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
DeviationReason Class  
Lsa.Vmfg.Shared Namespace  
  

DeviationReason.Load  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 221 DeviationReason.Load Method  
Load all Deviation Reasons.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
DeviationReason Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
DeviationReason.Load  Method (String)  
222 | Infor VISUAL API Toolkit  Shared Class Library Reference  DeviationReason.Load Method (String)  
Load a specific Deviation Reason row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
DeviationReason Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
DeviationReason.Load  Method (Stream, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 223 DeviationReason.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
DeviationReason Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
DeviationReason.NewDeviationRow  Method 
224 | Infor VISUAL API Toolkit  Shared Class Library Reference  DeviationReason.NewDeviationRow Method  
Adds a new row to the DEVIATION_REASON table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewDeviationRow ( 
 string  id 
) 
 
VB 
Public  Function  NewDeviationRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
DeviationReason Class  
Lsa.Vmfg.Shared Namespace  
  
DeviationReason.Save Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 225 DeviationReason.Save Method  
Save previously loaded Deviation Reason(s) to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
DeviationReason Class  
Lsa.Vmfg.Shared Namespace  
  
EarningCode Class  
226 | Infor VISUAL API Toolkit  Shared Class Library Reference  EarningCode Class  
Maintain Earning Codes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.EarningCode  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  EarningCode  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  EarningCode  
 Inherits  BusinessDocument  
 
The EarningCode  type exposes the following members.  
Constructors 
 Name  Description  
 EarningCode()  Initializes a new instance of the EarningCode  class 
 EarningCode(String)  Initializes a new instance of the EarningCode  class 
 

EarningCode Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 227 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Earning Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Earning Codes based on search criteria, limited 
by record count.  
 Exists  Determine if an Earning Code exists.  
 Find Find a specific Earning Code.  
 Load()  Load all Earning Codes.  
 Load(String)  Load a specific Earning Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewEarningCodeRow  Adds a row to the EARNING_CODE table.  
 Save  Save previously loaded Earning Code(s) to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

EarningCode Constructor  
228 | Infor VISUAL API Toolkit  Shared Class Library Reference  EarningCode Constructor  
Overload List 
 Name  Description  
 EarningCode()  Initializes a new instance of the EarningCode  class 
 EarningCode(String)  Initializes a new instance of the EarningCode  class 
 
See Also 
EarningCode Class  
Lsa.Vmfg.Shared Namespace  
  

EarningCode Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 229 EarningCode Constructor  
Initializes a new instance of the EarningCode  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  EarningCode()  
 
VB 
Public  Sub New 
 
See Also 
EarningCode Class  
EarningCode Overload  
Lsa.Vmfg.Shared Namespace  
  
EarningCode Constructor  (String) 
230 | Infor VISUAL API Toolkit  Shared Class Library Reference  EarningCode Constructor (String)  
Initializes a new instance of the EarningCode  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  EarningCode( 
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
EarningCode Class  
EarningCode Overload  
Lsa.Vmfg.Shared Namespace  
  
EarningCode.EarningCode  Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 231 EarningCode.EarningCode Methods  
The EarningCode  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Earning Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Earning Codes based on search criteria, limited 
by record count.  
 Exists  Determine if an Earning Code exists.  
 Find Find a specific Earning Code.  
 Load()  Load all Earning Codes.  
 Load(String)  Load a specific Earning Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewEarningCodeRow  Adds a row to the EARNING_CODE table.  
 Save  Save previously loaded Earning Code(s) to the database.  
 
See Also 
EarningCode Class  
Lsa.Vmfg.Shared Namespace  
  

EarningCode.Browse  Method  
232 | Infor VISUAL API Toolkit  Shared Class Library Reference  EarningCode.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Earning Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Earning Codes based on search criteria, limited by 
record count.  
 
See Also 
EarningCode Class  
Lsa.Vmfg.Shared Namespace  
  

EarningCode.Browse  Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 233 EarningCode.Browse Method (String, String, String)  
Retrieve Earning Codes based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
EarningCode Class  
EarningCode.Browse  Method (String, String, String) 
234 | Infor VISUAL API Toolkit  Shared Class Library Reference  Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
EarningCode.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 235 EarningCode.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve Earning Codes based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
EarningCode.Browse  Method (String, String, String, Int32, Int32)  
236 | Infor VISUAL API Toolkit  Shared Class Library Reference  Return Value  
Type: DataSet  
See Also 
EarningCode Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
EarningCode.Exists  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 237 EarningCode.Exists Method  
Determine if an Earning Code exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
EarningCode Class  
Lsa.Vmfg.Shared Namespace  
  
EarningCode.Find Method  
238 | Infor VISUAL API Toolkit  Shared Class Library Reference  EarningCode.Find Method  
Find a specific Earning Code.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
EarningCode Class  
Lsa.Vmfg.Shared Namespace  
  
EarningCode.Load  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 239 EarningCode.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Earning Codes.  
 Load(String)  Load a specific Earning Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
EarningCode Class  
Lsa.Vmfg.Shared Namespace  
  

EarningCode.Load  Method 
240 | Infor VISUAL API Toolkit  Shared Class Library Reference  EarningCode.Load Method  
Load all Earning Codes.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
EarningCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
EarningCode.Load  Method (String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 241 EarningCode.Load Method (String)  
Load a specific Earning Code row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
EarningCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
EarningCode.Load  Method (Stream, String)  
242 | Infor VISUAL API Toolkit  Shared Class Library Reference  EarningCode.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
EarningCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
EarningCode.NewEarningCodeRow  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 243 EarningCode.NewEarningCodeRow Method  
Adds a row to the EARNING_CODE table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewEarningCodeRow ( 
 string  id 
) 
 
VB 
Public  Function  NewEarningCodeRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
EarningCode Class  
Lsa.Vmfg.Shared Namespace  
  
EarningCode.Save  Method  
244 | Infor VISUAL API Toolkit  Shared Class Library Reference  EarningCode.Save Method  
Save previously loaded Earning Code(s) to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
EarningCode Class  
Lsa.Vmfg.Shared Namespace  
  
Employee Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 245 Employee Class  
Maintain Employees.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Employee  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  Employee  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  Employee 
 Inherits  BusinessDocument  
 
The Employee  type exposes the following members.  
Constructors 
 Name  Description  
 Employee()  Initializes a new instance of the Employee class 
 Employee(String)  Initializes a new instance of the Employee class 
 

Employee Class  
246 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Employees based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Employees based on search criteria, limited by 
record count.  
 Exists  Determine if an Employee exists.  
 Find Find a specific Employee.  
 Load()  Load all Employees.  
 Load(String)  Load a specific Employee row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewEmployeeRow  Adds a new row to the EMPLOYEE table.  
 NewEmployeeSiteRow  Adds a new row to the EMPLOYEE_SITE table.  
 Save  Save previously loaded Employees to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Employee Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 247 Employee Constructor  
Overload List 
 Name  Description  
 Employee()  Initializes a new instance of the Employee  class 
 Employee(String)  Initializes a new instance of the Employee  class 
 
See Also 
Employee Class  
Lsa.Vmfg.Shared Namespace  
  

Employee Constructor  
248 | Infor VISUAL API Toolkit  Shared Class Library Reference  Employee Constructor  
Initializes a new instance of the Employee  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Employee () 
 
VB 
Public  Sub New 
 
See Also 
Employee Class  
Employee Overload  
Lsa.Vmfg.Shared Namespace  
  
Employee Constructor  (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 249 Employee Constructor (String)  
Initializes a new instance of the Employee  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Employee ( 
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
Employee Class  
Employee Overload  
Lsa.Vmfg.Shared Namespace  
  
Employee.Employee Methods  
250 | Infor VISUAL API Toolkit  Shared Class Library Reference  Employee.Employee Methods  
The Employee  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Employees based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Employees based on search criteria, limited by 
record count.  
 Exists  Determine if an Employee exists.  
 Find Find a specific Employee.  
 Load()  Load all Employees.  
 Load(String)  Load a specific Employee row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewEmployeeRow  Adds a new row to the EMPLOYEE table.  
 NewEmployeeSiteRow  Adds a new row to the EMPLOYEE_SITE table.  
 Save  Save previously loaded Employees to the database.  
 
See Also 
Employee Class  
Lsa.Vmfg.Shared Namespace  
  

Employee.Browse Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 251 Employee.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Employees based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Employees based on search criteria, limited by 
record count.  
 
See Also 
Employee Class  
Lsa.Vmfg.Shared Namespace  
  

Employee.Browse Method (String, String, String) 
252 | Infor VISUAL API Toolkit  Shared Class Library Reference  Employee.Browse Method (String, String, String)  
Retrieve Employees based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Employee Class  
Employee.Browse Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 253 Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Employee.Browse Method (String, String, String, Int32, Int32)  
254 | Infor VISUAL API Toolkit  Shared Class Library Reference  Employee.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve Employees based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Employee.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 255 Return Value  
Type: DataSet  
See Also 
Employee Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Employee.Exists  Method  
256 | Infor VISUAL API Toolkit  Shared Class Library Reference  Employee.Exists Method  
Determine if an Employee exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Employee Class  
Lsa.Vmfg.Shared Namespace  
  
Employee.Find Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 257 Employee.Find Method  
Find a specific Employee.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Employee Class  
Lsa.Vmfg.Shared Namespace  
  
Employee.Load  Method 
258 | Infor VISUAL API Toolkit  Shared Class Library Reference  Employee.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Employees.  
 Load(String)  Load a specific Employee row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
Employee Class  
Lsa.Vmfg.Shared Namespace  
  

Employee.Load  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 259 Employee.Load Method  
Load all Employees.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
Employee Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Employee.Load  Method (String) 
260 | Infor VISUAL API Toolkit  Shared Class Library Reference  Employee.Load Method (String)  
Load a specific Employee row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Employee Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Employee.Load  Method (Stream, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 261 Employee.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Employee Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Employee.NewEmployeeRow  Method 
262 | Infor VISUAL API Toolkit  Shared Class Library Reference  Employee.NewEmployeeRow Method  
Adds a new row to the EMPLOYEE table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewEmployeeRow ( 
 string  id 
) 
 
VB 
Public  Function  NewEmployeeRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Employee Class  
Lsa.Vmfg.Shared Namespace  
  
Employee.NewEmployeeSiteRow  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 263 Employee.NewEmployeeSiteRow Method  
Adds a new row to the EMPLOYEE_SITE table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewEmployeeSiteRow ( 
 string  employeeID , 
 string  siteID  
) 
 
VB 
Public  Function  NewEmployeeSiteRow  (  
 employeeID  As String , 
 siteID  As String  
) As DataRow  
 
Parameters  
employeeID  
Type: System.String  
siteID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Employee Class  
Lsa.Vmfg.Shared Namespace  
  
Employee.Save Method  
264 | Infor VISUAL API Toolkit  Shared Class Library Reference  Employee.Save Method  
Save previously loaded Employees to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Employee Class  
Lsa.Vmfg.Shared Namespace  
  
FOBPoint Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 265 FOBPoint Class  
Maintain FOB Points.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.FOBPoint  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  FOBPoint  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  FOBPoint  
 Inherits  BusinessDocument  
 
The FOBPoint  type exposes the following members.  
Constructors 
 Name  Description  
 FOBPoint()  Initializes a new instance of the FOBPoint  class 
 FOBPoint(String)  Initializes a new instance of the FOBPoint  class 
 

FOBPoint Class  
266 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve FOB Points based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve FOB Points based on search criteria, limited by 
record count.  
 Exists  Determine if an FOB Point exists.  
 Find Find a specific FOB Point.  
 Load()  Load all FOB Points.  
 Load(String)  Load a specific FOB Point row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewFOBPointRow  Adds a new row to the FOB_POINT Point table.  
 Save  Save previously loaded FOB Point(s) to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

FOBPoint Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 267 FOBPoint Constructor  
Overload List 
 Name  Description  
 FOBPoint()  Initializes a new instance of the FOBPoint  class 
 FOBPoint(String)  Initializes a new instance of the FOBPoint  class 
 
See Also 
FOBPoint Class  
Lsa.Vmfg.Shared Namespace  
  

FOBPoint Constructor  
268 | Infor VISUAL API Toolkit  Shared Class Library Reference  FOBPoint Constructor  
Initializes a new instance of the FOBPoint  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  FOBPoint () 
 
VB 
Public  Sub New 
 
See Also 
FOBPoint Class  
FOBPoint Overload  
Lsa.Vmfg.Shared Namespace  
  
FOBPoint Constructor  (String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 269 FOBPoint Constructor (String)  
Initializes a new instance of the FOBPoint  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  FOBPoint ( 
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
FOBPoint Class  
FOBPoint Overload  
Lsa.Vmfg.Shared Namespace  
  
FOBPoint.FOBPoint  Methods  
270 | Infor VISUAL API Toolkit  Shared Class Library Reference  FOBPoint.FOBPoint Methods  
The FOBPoint  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve FOB Points based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve FOB Points based on search criteria, limited by 
record count.  
 Exists  Determine if an FOB Point exists.  
 Find Find a specific FOB Point.  
 Load()  Load all FOB Points.  
 Load(String)  Load a specific FOB Point row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewFOBPointRow  Adds a new row to the FOB_POINT Point table.  
 Save  Save previously loaded FOB Point(s) to the database.  
 
See Also 
FOBPoint Class  
Lsa.Vmfg.Shared Namespace  
  

FOBPoint.Browse  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 271 FOBPoint.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve FOB Points based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve FOB Points based on search criteria, limited by 
record count.  
 
See Also 
FOBPoint Class  
Lsa.Vmfg.Shared Namespace  
  

FOBPoint.Browse  Method (String, String, String) 
272 | Infor VISUAL API Toolkit  Shared Class Library Reference  FOBPoint.Browse Method (String, String, String)  
Retrieve FOB Points based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
FOBPoint Class  
FOBPoint.Browse  Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 273 Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
FOBPoint.Browse  Method (String, String, String, Int32, Int32)  
274 | Infor VISUAL API Toolkit  Shared Class Library Reference  FOBPoint.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve FOB Points based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
FOBPoint.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 275 Return Value  
Type: DataSet  
See Also 
FOBPoint Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
FOBPoint.Exists  Method  
276 | Infor VISUAL API Toolkit  Shared Class Library Reference  FOBPoint.Exists Method  
Determine if an FOB Point exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  code 
) 
 
VB 
Public  Overridable  Function Exists  (  
 code As String  
) As Boolean 
 
Parameters  
code  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
FOBPoint Class  
Lsa.Vmfg.Shared Namespace  
  
FOBPoint.Find  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 277 FOBPoint.Find Method  
Find a specific FOB Point.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Find (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
FOBPoint Class  
Lsa.Vmfg.Shared Namespace  
  
FOBPoint.Load Method  
278 | Infor VISUAL API Toolkit  Shared Class Library Reference  FOBPoint.Load Method  
Overload List 
 Name  Description  
 Load()  Load all FOB Points.  
 Load(String)  Load a specific FOB Point row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
FOBPoint Class  
Lsa.Vmfg.Shared Namespace  
  

FOBPoint.Load Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 279 FOBPoint.Load Method  
Load all FOB Points.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
FOBPoint Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
FOBPoint.Load Method (String)  
280 | Infor VISUAL API Toolkit  Shared Class Library Reference  FOBPoint.Load Method (String)  
Load a specific FOB Point row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
FOBPoint Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
FOBPoint.Load Method (Stream, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 281 FOBPoint.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 code As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
code  
Type: System.String  
See Also 
FOBPoint Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
FOBPoint.NewFOBPointRow  Method 
282 | Infor VISUAL API Toolkit  Shared Class Library Reference  FOBPoint.NewFOBPointRow Method  
Adds a new row to the FOB_POINT Point table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewFOBPointRow( 
 string  code 
) 
 
VB 
Public  Function  NewFOBPointRow  (  
 code As String  
) As DataRow  
 
Parameters  
code  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
FOBPoint Class  
Lsa.Vmfg.Shared Namespace  
  
FOBPoint.Save Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 283 FOBPoint.Save Method  
Save previously loaded FOB Point(s) to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
FOBPoint Class  
Lsa.Vmfg.Shared Namespace  
  
GeneralQuery Class  
284 | Infor VISUAL API Toolkit  Shared Class Library Reference  GeneralQuery Class  
General Query. This service allows you to cause a query execution under data object control with 
transmission of the result set to the client side.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessService  
          Lsa.Vmfg.Shared.GeneralQuery  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  GeneralQuery  : BusinessService ,  
 IGeneralQuery , IDisposable 
 
VB 
<SerializableAttribute> 
Public  Class  GeneralQuery  
 Inherits  BusinessService 
 Implements  IGeneralQuery , IDisposable  
 
The GeneralQuery  type exposes the following members.  
Constructors 
 Name  Description  
 GeneralQuery()  Initializes a new instance of the GeneralQuery class  

GeneralQuery Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 285  GeneralQuery(String)  Initializes a new instance of the GeneralQuery class  
 
Methods 
 Name  Description  
 CreateDataObject  Need a query service. (Overrides 
BusinessService.CreateDataObject().)  
 Execute()  Executes the query.  
 Execute(String)  Executes the query.  
 Execute(Int32, Int32)  Executes the query for a specified number of records  
 Execute(String, Int32, 
Int32)  Executes the query for a specified number of records  
 Prepare  Prepares the specified query for execution.  
 
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object (Overrides BusinessObject.DataObjectType.)  
 Parameters   
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

GeneralQuery Constructor  
286 | Infor VISUAL API Toolkit  Shared Class Library Reference  GeneralQuery Constructor  
Overload List 
 Name  Description  
 GeneralQuery()  Initializes a new instance of the GeneralQuery  class 
 GeneralQuery(String)  Initializes a new instance of the GeneralQuery  class 
 
See Also 
GeneralQuery Class  
Lsa.Vmfg.Shared Namespace  
  

GeneralQuery Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 287 GeneralQuery Constructor  
Initializes a new instance of the GeneralQuery  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  GeneralQuery () 
 
VB 
Public  Sub New 
 
See Also  
GeneralQuery Class  
GeneralQuery Overload  
Lsa.Vmfg.Shared Namespace  
  
GeneralQuery Constructor  (String)  
288 | Infor VISUAL API Toolkit  Shared Class Library Reference  GeneralQuery Constructor (String)  
Initializes a new instance of the GeneralQuery  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  GeneralQuery ( 
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
GeneralQuery Class  
GeneralQuery Overload  
Lsa.Vmfg.Shared Namespace  
  
GeneralQuery.GeneralQuery Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 289 GeneralQuery.GeneralQuery Methods  
The GeneralQuery  type exposes the following members.  
Methods 
 Name  Description  
 CreateDataObject  Need a query service. (Overrides 
BusinessService.CreateDataObject().)  
 Execute()  Executes the query.  
 Execute(String)  Executes the query.  
 Execute(Int32, Int32)  Executes the query for a specified number of records  
 Execute(String, Int32, 
Int32)  Executes the query for a specified number of records  
 Prepare  Prepares the specified query for execution.  
 
See Also 
GeneralQuery Class  
Lsa.Vmfg.Shared Namespace  
  

GeneralQuery.CreateDataObject  Method  
290 | Infor VISUAL API Toolkit  Shared Class Library Reference  GeneralQuery.CreateDataObject Method  
Need a query service.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  IDataService  CreateDataObject () 
 
VB 
Public  Overrides  Function  CreateDataObject  As IDataService 
 
Return Value  
Type: IDataService  
 
See Also 
GeneralQuery Class  
Lsa.Vmfg.Shared Namespace  
  
GeneralQuery.Execute Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 291 GeneralQuery.Execute Method  
Overload List 
 Name  Description  
 Execute()  Executes the query.  
 Execute(String)  Executes the query.  
 Execute(Int32, Int32)  Executes the query for a specified number of records  
 Execute(String, Int32, Int32)  Executes the query for a specified number of records  
 
See Also 
GeneralQuery Class  
Lsa.Vmfg.Shared Namespace  
  

GeneralQuery.Execute Method  
292 | Infor VISUAL API Toolkit  Shared Class Library Reference  GeneralQuery.Execute Method  
Executes the query.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Execute () 
 
VB 
Public  Overridable  Sub Execute  
 
Implements 
IGeneralQuery.Execute()  
 
See Also 
GeneralQuery Class  
Execute Overload  
Lsa.Vmfg.Shared Namespace  
  
GeneralQuery.Execute Method (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 293 GeneralQuery.Execute Method (String)  
Executes the query.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Execute ( 
 string  tableName 
) 
 
VB 
Public  Overridable  Sub Execute (  
 tableName As String  
) 
 
Parameters  
tableName  
Type: System.String  
Implements 
IGeneralQuery.Execute(String)  
 
See Also 
GeneralQuery Class  
Execute Overload  
Lsa.Vmfg.Shared Namespace  
  
GeneralQuery.Execute Method (Int32, Int32)  
294 | Infor VISUAL API Toolkit  Shared Class Library Reference  GeneralQuery.Execute Method (Int32, Int32)  
Executes the query for a specified number of records  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Execute ( 
 int startRecord, 
 int maxRecords  
) 
 
VB 
Public  Overridable  Sub Execute (  
 startRecord As Integer , 
 maxRecords  As Integer  
) 
 
Parameters  
startRecord  
Type: System.Int32  
maxRecords  
Type: System.Int32  
Implements 
IGeneralQuery.Execute(Int32, Int32)  
 
See Also 
GeneralQuery Class  
Execute Overload  
Lsa.Vmfg.Shared Namespace  
  
GeneralQuery.Execute Method (String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 295 GeneralQuery.Execute Method (String, Int32, Int32)  
Executes the query for a specified number of records  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Execute ( 
 string  tableName , 
 int startRecord, 
 int maxRecords  
) 
 
VB 
Public  Overridable  Sub Execute (  
 tableName As String , 
 startRecord As Integer , 
 maxRecords  As Integer  
) 
 
Parameters  
tableName  
Type: System.String  
startRecord  
Type: System.Int32  
maxRecords  
Type: System.Int32  
Implements 
IGeneralQuery.Execute(String, Int32, Int32)  
 
GeneralQuery.Execute Method (String, Int32, Int32)  
296 | Infor VISUAL API Toolkit  Shared Class Library Reference  See Also 
GeneralQuery Class  
Execute Overload  
Lsa.Vmfg.Shared Namespace  
  
GeneralQuery.Prepare  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 297 GeneralQuery.Prepare Method  
Prepares the specified query for execution.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Prepare ( 
 string  tableName , 
 string  sqlCommand  
) 
 
VB 
Public  Overridable  Sub Prepare (  
 tableName As String , 
 sqlCommand As String  
) 
 
Parameters  
tableName  
Type: System.String  
sqlCommand  
Type: System.String  
Implements 
IGeneralQuery.Prepare(String, String)  
 
See Also 
GeneralQuery Class  
Lsa.Vmfg.Shared Namespace  
  
GeneralQuery.GeneralQuery Properties  
298 | Infor VISUAL API Toolkit  Shared Class Library Reference  GeneralQuery.GeneralQuery Properties  
The GeneralQuery  type exposes the following members.  
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object (Overrides 
BusinessObject.DataObjectType.)  
 Parameters   
 
See Also 
GeneralQuery Class  
Lsa.Vmfg.Shared Namespace  
  

GeneralQuery.DataObjectType Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 299 GeneralQuery.DataObjectType Property  
Report types of data object associated with this business object  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  DataObjectType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  DataObjectType As Type 
 Get 
 
Property Value  
Type: Type  
See Also 
GeneralQuery Class  
Lsa.Vmfg.Shared Namespace  
  
GeneralQuery.Parameters Property  
300 | Infor VISUAL API Toolkit  Shared Class Library Reference  GeneralQuery.Parameters Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  ParameterCollection Parameters  { get; } 
 
VB 
Public  ReadOnly  Property  Parameters  As ParameterCollection 
 Get 
 
Property Value  
Type: ParameterCollection  
Implements 
IGeneralQuery.Parameters 
 
See Also 
GeneralQuery Class  
Lsa.Vmfg.Shared Namespace  
  
GetDemandLinks Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 301 GetDemandLinks Class  
Service to obtain all linked demand to a given supply.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessService  
          Lsa.Vmfg.Shared.GetDemandLinks  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  GetDemandLinks  : BusinessService 
 
VB 
<SerializableAttribute> 
Public  Class  GetDemandLinks  
 Inherits  BusinessService 
 
The GetDemandLinks type exposes the following members.  
Constructors 
 Name  Description  
 GetDemandLinks()  Initializes a new instance of the GetDemandLinks class 
 GetDemandLinks(String)  Initializes a new instance of the GetDemandLinks class 
 

GetDemandLinks Class  
302 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Execute  Executes the Service.  
 NewInputRow  Adds a new request row to the Service.  
 Prepare  Prepares the Service for execution.  
 
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object 
(Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType   (Overrides BusinessObject.ServicedComponentType.)  
Data Tables 
 Table Type  Table Name  
 Header Table  DEMAND_LINKS  
 Results Sub- table DEMAND_LINKS_RESULT  
See Also 
Lsa.Vmfg.Shared Namespace  
  

GetDemandLinks Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 303 GetDemandLinks Constructor  
Overload List 
 Name  Description  
 GetDemandLinks()  Initializes a new instance of the GetDemandLinks  class 
 GetDemandLinks(String)  Initializes a new instance of the GetDemandLinks  class 
 
See Also 
GetDemandLinks Class  
Lsa.Vmfg.Shared Namespace  
  

GetDemandLinks Constructor  
304 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetDemandLinks Constructor  
Initializes a new instance of the GetDemandLinks  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  GetDemandLinks () 
 
VB 
Public  Sub New 
 
See Also 
GetDemandLinks Class  
GetDemandLinks Overload  
Lsa.Vmfg.Shared Namespace  
  
GetDemandLinks Constructor  (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 305 GetDemandLinks Constructor (String)  
Initializes a new instance of the GetDemandLinks  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  GetDemandLinks ( 
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
GetDemandLinks Class  
GetDemandLinks Overload  
Lsa.Vmfg.Shared Namespace  
  
GetDemandLinks.GetDemandLinks Methods  
306 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetDemandLinks.GetDemandLinks Methods  
The GetDemandLinks  type exposes the following members.  
Methods 
 Name  Description  
 Execute  Executes the Service.  
 NewInputRow  Adds a new request row to the Service.  
 Prepare  Prepares the Service for execution.  
 
See Also 
GetDemandLinks Class  
Lsa.Vmfg.Shared Namespace  
  

GetDemandLinks.Execute Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 307 GetDemandLinks.Execute Method  
Executes the Service.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Execute () 
 
VB 
Public  Overridable  Sub Execute  
 
See Also 
GetDemandLinks Class  
Lsa.Vmfg.Shared Namespace  
  
GetDemandLinks.NewInputRow  Method  
308 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetDemandLinks.NewInputRow Method  
Adds a new request row to the Service.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
GetDemandLinks Class  
Lsa.Vmfg.Shared Namespace  
  
GetDemandLinks.Prepare Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 309 GetDemandLinks.Prepare Method  
Prepares the Service for execution.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Prepare()  
 
VB 
Public  Overridable  Sub Prepare  
 
See Also 
GetDemandLinks Class  
Lsa.Vmfg.Shared Namespace  
  
GetDemandLinks.GetDemandLinks Properties  
310 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetDemandLinks.GetDemandLinks Properties  
The GetDemandLinks  type exposes the following members.  
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object 
(Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType   (Overrides BusinessObject.ServicedComponentType.)  
 
See Also 
GetDemandLinks Class  
Lsa.Vmfg.Shared Namespace  
  

GetDemandLinks.DataObjectType Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 311 GetDemandLinks.DataObjectType Property  
Report types of data object associated with this business object  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  DataObjectType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  DataObjectType As Type 
 Get 
 
Property Value  
Type: Type  
See Also 
GetDemandLinks Class  
Lsa.Vmfg.Shared Namespace  
  
GetDemandLinks.ServicedComponentType  Property  
312 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetDemandLinks.ServicedComponentType Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  ServicedComponentType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  ServicedComponentType As Type  
 Get 
 
Property Value  
Type: Type  
See Also 
GetDemandLinks Class  
Lsa.Vmfg.Shared Namespace  
  
GetDemandLinks Data Tables  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 313 GetDemandLinks Data Tables  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
DataSet Name returned from Prepare:  DEMAND_LINKS 
Header Table  
Table Name:  DEMAND_LINKS  
Primary Key:  ENTRY_NO  
Column Name  Type  Description 
ENTRY_NO  Integer  Unique integer identifier for this row.  
Required.  
SUPPLY_TYPE  String Type of the supply link. Acceptable types 
include PD, PO, WO, and WH. Required.  
SUPPLY_BASE_ID  String Base ID of the supply order. Required.  
SUPPLY_LOT_ID  String   Lot ID of the supply order. Value depends 
on supply type.  
SUPPLY_SPLIT_ID  String Split ID of the supply order. Value depends on supply type.  
SUPPLY_SUB_ID  String  Sub ID of the supply order. Value depends on supply type.  
SUPPLY_SEQ_NO  Integer  Operation sequence number of the supply order. Value depends on supply type.  
SUPPLY_NO  Integer  Supply Number. Value depends on supply 
type.  
TRANSACTION_QTY Decimal  Total transaction quantity. Required.  
Results Sub-table 
Table Name : DEMAND_LINKS_RESULT  
Column Name  Type  Description 
ENTRY_NO  Integer  Same value as the header table.  
DSL_ID  Integer  Demand- Supply Link ID.  
GetDemandLinks Data Tables  
314 | Infor VISUAL API Toolkit  Shared Class Library Reference  Column Name  Type  Description 
SUPPLY_TYPE  String  Same value as the header table.  
SUPPLY_BASE_ID  String  Same value as the header table.  
SUPPLY_LOT_ID  String  Same value as the header table.  
SUPPLY_SPLIT_ID  String  Same value as the header table.  
SUPPLY_SUB_ID  String  Same value as the header table.  
SUPPLY_SEQ_NO  Integer  Same value as the header table.  
SUPPLY_NO  Integer  Same value as the header table.  
DEMAND_TYPE String  Type of the demand link.  
DEMAND_BASE_ID  String  Base ID of the demand link.  
DEMAND_PART_ID  String  Part ID of the demand link.  
DEMAND_LOT_ID  String  Lot ID of the demand link.  
DEMAND_SPLIT_ID  String  Split ID of the demand link.  
DEMAND_SUB_ID  String  Sub ID of the demand link.  
DEMAND_SEQ_NO  Integer  Operation Sequence Number of 
the demand link.  
DEMAND_NO  Integer  Demand link number.  
PART_ID  String  Part ID of the demand link.  
WAREHOUSE_ID  String  Warehouse ID of the demand link.  
ALLOCATED_QTY  Decimal  Allocated quantity of the demand link. 
RECEIVED_QTY Decimal  Received quantity of the demand link. 
ISSUED_QTY Decimal  Issued quantity of the demand link.  
DEMAND_ORDER_REQD_QTY  Decimal  Required quantity for the demand 
order.  
DEMAND_ORDER_ISSUED_QTY Decimal  Issued quantity for the demand order.  
DEMAND_ORDER_ALLOCATED_QTY  Decimal  Allocated quantity for the demand order.  
DEMAND_ORDER_FULFILLED_QTY  Decimal  Fulfilled quantity for the demand order.  
QTY_TO_BE_ALLOCATED  Decimal  Recommended quantity to be allocated.  
GetDemandLinks Data Tables  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 315 Column Name  Type  Description 
QTY_TO_BE_ISSUED  Decimal  Recommended quantity to be 
issued. 
DEMAN D_REQUIRED_DATE  Date  Date the demand order is required.  
See Also 
GetDemandLinks Class  
Lsa.Vmfg.Shared Namespace  
  
GetManufacturingJournalInfo Class  
316 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetManufacturingJournalInfo Class  
Service to obtain Manufacturing Journal Info.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessService  
          Lsa.Vmfg.Shared.GetManufacturingJournalInfo 
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  GetManufacturingJournalInfo : BusinessService 
 
VB 
<SerializableAttribute> 
Public  Class  GetManufacturingJournalInfo  
 Inherits  BusinessService 
 
The GetManufacturingJournalInfo type exposes the following members.  
Constructors 
 Name  Description  
 GetManufacturingJournalInfo()  Initializes a new instance of the 
GetManufacturingJournalInfo  class 
 GetManufacturingJournalInfo(String)  Initializes a new instance of the GetManufacturingJournalInfo  class 

GetManufacturingJournalInfo Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 317  
Methods 
 Name  Description  
 Execute  Execute the Service.  
 NewInputRow  Adds a new request row to the Service.  
 Prepare  Prepare the Service for execution.  
 
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object 
(Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType   (Overrides BusinessObject.ServicedComponentType.)  
 
Data Tables 
 Table Type  Table Name  
 Header Table  GET_MFG_JOURNAL_INFO  
 Results Sub- table SUB_LEDGER_DETAIL  
 Results Sub- table POSTING_DETAIL  
 Results Sub- table TRANSACTION_DETAIL  
See Also 
Lsa.Vmfg.Shared Namespace  

GetManufacturingJournalInfo Class  
318 | Infor VISUAL API Toolkit  Shared Class Library Reference    
GetManufacturingJournalInfo Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 319 GetManufacturingJournalInfo Constructor  
Overload List 
 Name  Description  
 GetManufacturingJournalInfo()  Initializes a new instance of the 
GetManufacturingJournalInfo  class 
 GetManufacturingJournalInfo(String)  Initializes a new instance of the GetManufacturingJournalInfo
 class 
 
See Also 
GetManufacturingJournalInfo Class  
Lsa.Vmfg.Shared Namespace  
  

GetManufacturingJournalInfo Constructor  
320 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetManufacturingJournalInfo Constructor  
Initializes a new instance of the GetManufacturingJournalInfo  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  GetManufacturingJournalInfo()  
 
VB 
Public  Sub New 
 
See Also 
GetManufacturingJournalInfo Class  
GetManufacturingJournalInfo Overload  
Lsa.Vmfg.Shared Namespace  
  
GetManufacturingJournalInfo Constructor  (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 321 GetManufacturingJournalInfo Constructor (String)  
Initializes a new instance of the GetManufacturingJournalInfo  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  GetManufacturingJournalInfo( 
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
GetManufacturingJournalInfo Class  
GetManufacturingJournalInfo Overload  
Lsa.Vmfg.Shared Namespace  
  
GetManufacturingJournalInfo.GetManufacturingJournalInfo  Methods  
322 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetManufacturingJournalInfo.GetManufacturingJour
nalInfo Methods  
The GetManufacturingJournalInfo  type exposes the following members.  
Methods 
 Name  Description  
 Execute  Execute the Service.  
 NewInputRow  Adds a new request row to the Service.  
 Prepare  Prepare the Service for execution.  
 
See Also 
GetManufacturingJournalInfo Class  
Lsa.Vmfg.Shared Namespace  
  

GetManufacturingJournalInfo.Execute  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 323 GetManufacturingJournalInfo.Execute Method  
Execute the Service.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Execute () 
 
VB 
Public  Overridable  Sub Execute  
 
See Also 
GetManufacturingJournalInfo Class  
Lsa.Vmfg.Shared Namespace  
  
GetManufacturingJournalInfo.NewInputRow  Method  
324 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetManufacturingJournalInfo.NewInputRow Method  
Adds a new request row to the Service.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewInputRow ( 
 string  batchID  
) 
 
VB 
Public  Overridable  Function NewInputRow  (  
 batchID  As String  
) As DataRow  
 
Parameters  
batchID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
GetManufacturingJournalInfo Class  
Lsa.Vmfg.Shared Namespace  
  
GetManufacturingJournalInfo.Prepare  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 325 GetManufacturingJournalInfo.Prepare Method  
Prepare the Service for execution.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Prepare()  
 
VB 
Public  Overridable  Sub Prepare  
 
See Also 
GetManufacturingJournalInfo Class  
Lsa.Vmfg.Shared Namespace  
  
GetManufacturingJournalInfo.GetManufacturingJournalInfo  Properties  
326 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetManufacturingJournalInfo.GetManufacturingJour
nalInfo Properties  
The GetManufacturingJournalInfo  type exposes the following members.  
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object 
(Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType   (Overrides BusinessObject.ServicedComponentType.)  
 
See Also 
GetManufacturingJournalInfo Class  
Lsa.Vmfg.Shared Namespace  
  

GetManufacturingJournalInfo.DataObjectType  Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 327 GetManufacturingJournalInfo.DataObjectType 
Property  
Report types of data object associated with this business object  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  DataObjectType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  DataObjectType As Type 
 Get 
 
Property Value  
Type: Type  
See Also 
GetManufacturingJournalInfo Class  
Lsa.Vmfg.Shared Namespace  
  
GetManufacturingJournalInfo.ServicedComponentType  Property  
328 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetManufacturingJournalInfo.ServicedComponentTy
pe Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  ServicedComponentType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  ServicedComponentType As Type  
 Get 
 
Property Value  
Type: Type  
See Also 
GetManufacturingJournalInfo Class  
Lsa.Vmfg.Shared Namespace  
  
GetManufacturingJournalInfo Data  Tables  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 329 GetManufacturingJournalInfo Data Tables  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
DataSet Name returned from Prepare: GET_MFG_JOURNAL_INFO  
Header Table  
Table Name:  GET_MFG_JOURNAL_INFO  
Primary Key:  ENTRY_NO  
Column Name  Type  Description 
BATCH_ID  String  Unique Journal Batch ID for this row.  
CURRENCY_ID String Currency ID of the manufacturing journal. 
Required.  
Results Sub-table 
Table Name : SUB_LEDGER_DETAIL  
Column Name  Type  Description 
BATCH_ID  String  Same value as the header table.  
Required.  
KEY_NO  Integer  Key number used with Batch ID to 
identify this row.  
CURRENCY_ID String  Same value as the header table.  
DIST_NO  Integer  Distribution number for this row.  
ENTRY_NO  Integer  Distribution Entry Number.  
PURCHASE_ORDER_ID String  Purchase Order ID of the sub-ledger journal.  
WORK_ORDER_ID  String  Work Order ID of the sub- ledger 
journal.  
PROJECT_ID  String  Project ID of the sub- ledger journal.  
CUSTOMER_ORDER_ID String  Customer Order ID of the sub-
ledger journal.  
GetManufacturingJournalInfo Data  Tables  
330 | Infor VISUAL API Toolkit  Shared Class Library Reference  Column Name  Type  Description 
INV_TRANSACTION_ID  Integer  Inventory Transaction ID of the 
sub-ledger journal.  
WBS String  WBS Code.  
PROJ_COST_CAT_ID  String  Project Cost Category ID.  
SOURCE_TYPE  String  Source Type.  
SOURCE_ID String  Source ID.  
DEPARTMENT_ID  String  Department ID.  
COST_CATEGORY_ID  String  Cost Category ID.  
SOURCE_TRANS_ID String  Source Transaction ID.  
BURDEN_ID String  Burden ID.  
INVOICE_ID  String  Payable or Receivable Invoice ID.  
REV_TRANS_TYPE String  Revenue Transaction Type.  
VENDOR_ID  String  Vendor ID.  
PART_ID  String  Part ID. 
CUSTOMER_ID  String  Customer ID.  
EMPLOYEE_ID  String  Employee ID.  
DESCRIPTION  String  Description or name of this 
particular sub- ledger.  
TRANSACTION_DATE Date  Date of the transaction.  
DEBIT_AMOUNT  Decimal  Debit Amount.  
CREDIT_AMOUNT  Decimal  Credit Amount  
DEBIT_ACCOUNT  String  Debit Account.  
CREDIT_ACCOUNT  String  Credit Account.  
USER_ID  String  User ID of the person who 
performed the transaction.  
BATCH_TYPE  String  Batch Type. Types include PUR, WIP, LAB, FG, SLS, ADJ, IND, 
PRJ, BUR, and REV.  
Results Sub-table 
Table Name : POSTING_DETAIL  
GetManufacturingJournalInfo Data  Tables  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 331 Column Name  Type  Description 
BATCH_ID  String  Same value as the header table.  
KEY_NO  Integer  Key number used with Batch ID to 
identify this row.  
KEY_NO_2 Integer  Second key number used with Batch ID to identify this row.  
CURRENCY_ID String  Same value as the header table.  
DIST_NO  Integer  Distribution Number for this row.  
ENTRY_NO  Integer  Distribution Entry Number.  
TRANSACTION_DATE Date  Date of the transaction.  
DEBIT_AMOUNT  Decimal  Debit Amount.  
CREDIT_AMOUNT  Decimal  Credit Amount.  
DEBIT_ACCOUNT  String  Debit Account.  
CREDIT_ACCOUNT  String  Credit Account.  
DEBIT_BALANCE Decimal  Debit Balance.  
CREDIT_BALANCE Decimal  Credit Balance.  
USER_ID  String  User ID of the person who 
performed the transaction.  
SOURCE_TYPE  String  Source Type.  
REV_TRANS_TYPE String  Revenue Transaction Type.  
SUB_BATCH_ID  String  Sub Batch ID.  
Results Sub-table 
Table Name : TRANSACTION_DETAIL  
Column Name  Type  Description 
BATCH_ID  String  Same value as the header table.  
KEY_NO  Integer  Key number used with Batch ID to identify this row.  
KEY_NO_2 Integer  Second key number used with Batch ID to identify this row.  
SUB_BATCH_ID  Integer  Sub Batch ID of the transaction.  
GetManufacturingJournalInfo Data  Tables  
332 | Infor VISUAL API Toolkit  Shared Class Library Reference  Column Name  Type  Description 
CURRENCY_ID String  Same value as the header table.  
DETAIL_TYPE String  Detail Type of the transaction 
detail.  
TRANSACTION_NO  String  Transaction Number.  
TRANSACTION_DATE Date  Date of the transaction detail.  
REFERENCE String  Document Reference of this transaction detail.  
DOCUMENT_ID  String  Document ID.  
TOTAL_AMOUNT  Decimal  Total Material Amount.  
MATERIAL_AMOUNT  Decimal  Actual Material Cost Amount.  
LABOR_AMOUNT  Decimal  Labor Amount.  
BURDENT_AMOUNT  Decimal  Burden Amount.  
SERVICE_AMOUNT  Decimal  Service Amount.  
See Also 
GetManufacturingJournalInfo Class  
Lsa.Vmfg.Shared Namespace  
  
GetPartDefaultWhseLoc Class 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 333  
GetPartDefaultWhseLoc Class  
Service to obtain a Part's default warehouse and location IDs.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessService  
          Lsa.Vmfg.Shared.GetPartDefaultWhseLoc  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  GetPartDefaultWhseLoc  : BusinessService  
 
VB 
<SerializableAttribute> 
Public  Class  GetPartDefaultWhseLoc  
 Inherits  BusinessService 
 
The GetPartDefaultWhseLoc type exposes the following members.  
GetPartDefaultWhseLoc Class 
334 | Infor VISUAL API Toolkit  Shared Class Library Reference  Constructors 
 Name  Description  
 GetPartDefaultWhseLoc()  Initializes a new instance of the GetPartDefaultWhseLoc  
class 
 GetPartDefaultWhseLoc(String)  Initializes a new instance of the GetPartDefaultWhseLoc  
class 
 
Methods 
 Name  Description  
 Execute  Executes the service.  
 NewInputRow  Adds a new request row to the Service.  
 Prepare  Prepares the service for execution.  
 
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object 
(Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType   (Overrides BusinessObject.ServicedComponentType.)  
 
Data Tables 
 Table Type  Table Name  
 Header Table  GET_DEF_WHSE_LOC  

GetPartDefaultWhseLoc Class 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 335  Results Sub- table GET_DEF_WHSE_LOC_RESULT  
See Also 
Lsa.Vmfg.Shared Namespace  
  
GetPartDefaultWhseLoc Constructor  
336 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetPartDefaultWhseLoc Constructor  
Overload List 
 Name  Description  
 GetPartDefaultWhseLoc()  Initializes a new instance of the GetPartDefaultWhseLoc  class 
 GetPartDefaultWhseLoc(String)  Initializes a new instance of the GetPartDefaultWhseLoc  class 
 
See Also 
GetPartDefaultWhseLoc Class  
Lsa.Vmfg.Shared Namespace  
  

GetPartDefaultWhseLoc Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 337 GetPartDefaultWhseLoc Constructor  
Initializes a new instance of the GetPartDefaultWhseLoc  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  GetPartDefaultWhseLoc () 
 
VB 
Public  Sub New 
 
See Also 
GetPartDefaultWhseLoc Class  
GetPartDefaultWhseLoc Overload  
Lsa.Vmfg.Shared Namespace  
  
GetPartDefaultWhseLoc Constructor  (String) 
338 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetPartDefaultWhseLoc Constructor (String)  
Initializes a new instance of the GetPartDefaultWhseLoc  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  GetPartDefaultWhseLoc ( 
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
GetPartDefaultWhseLoc Class  
GetPartDefaultWhseLoc Overload  
Lsa.Vmfg.Shared Namespace  
  
GetPartDefaultWhseLoc.GetPartDefaultWhseLoc Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 339 GetPartDefaultWhseLoc.GetPartDefaultWhseLoc 
Methods  
The GetPartDefaultWhseLoc  type exposes the following members.  
Methods 
 Name  Description  
 Execute  Executes the service.  
 NewInputRow  Adds a new request row to the Service.  
 Prepare  Prepares the service for execution.  
 
See Also 
GetPartDefaultWhseLoc Class  
Lsa.Vmfg.Shared Namespace  
  

GetPartDefaultWhseLoc.Execute Method  
340 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetPartDefaultWhseLoc.Execute Method  
Executes the service.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Execute () 
 
VB 
Public  Overridable  Sub Execute  
 
See Also 
GetPartDefaultWhseLoc Class  
Lsa.Vmfg.Shared Namespace  
  
GetPartDefaultWhseLoc.NewInputRow  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 341 GetPartDefaultWhseLoc.NewInputRow Method  
Adds a new request row to the Service.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
GetPartDefaultWhseLoc Class  
Lsa.Vmfg.Shared Namespace  
  
GetPartDefaultWhseLoc.Prepare Method  
342 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetPartDefaultWhseLoc.Prepare Method  
Prepares the service for execution.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Prepare()  
 
VB 
Public  Overridable  Sub Prepare  
 
See Also 
GetPartDefaultWhseLoc Class  
Lsa.Vmfg.Shared Namespace  
  
GetPartDefaultWhseLoc.GetPartDefaultWhseLoc Properties  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 343 GetPartDefaultWhseLoc.GetPartDefaultWhseLoc 
Properties  
The GetPartDefaultWhseLoc  type exposes the following members.  
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object 
(Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType   (Overrides BusinessObject.ServicedComponentType.)  
 
See Also 
GetPartDefaultWhseLoc Class  
Lsa.Vmfg.Shared Namespace  
  

GetPartDefaultWhseLoc.DataObjectType Property  
344 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetPartDefaultWhseLoc.DataObjectType Property  
Report types of data object associated with this business object  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  DataObjectType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  DataObjectType As Type 
 Get 
 
Property Value  
Type: Type  
See Also 
GetPartDefaultWhseLoc Class  
Lsa.Vmfg.Shared Namespace  
  
GetPartDefaultWhseLoc.ServicedComponentType  Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 345 GetPartDefaultWhseLoc.ServicedComponentType 
Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  ServicedComponentType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  ServicedComponentType As Type  
 Get 
 
Property Value  
Type: Type  
See Also 
GetPartDefaultWhseLoc Class  
Lsa.Vmfg.Shared Namespace  
  
GetPartDefaultWhseLoc Data Tables  
346 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetPartDefaultWhseLoc Data Tables  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
DataSet Name returned from Prepare: GET_DEF_WHSE_LOC  
Header Table  
Table Name:  GET_DEF_WHSE_LOC  
Primary Key:  ENTRY_NO  
Column Name  Type  Description 
ENTRY_NO  Integer  Unique integer identifier for this row.  
Required.  
SITE_ID  String  Site ID. Required.  
PART_ID  String  Part ID to retrieve warehouse information. 
Required.  
WAREHOUSE_ID  String  Warehouse ID. If provided, only Location ID will be retrieved in result table.  
REQUESTED_STATUS  String  Requested Location Status. Acceptable values are ‘A’, ‘H’, ‘U’, and ‘I’.  
INSPECTION  Boolean Inspection Location flag used to retrieve locations that are inspection locations.  
Results Sub-table 
Table Name : SUB_DEF_WHSE_LOC_RESULT  
Column Name  Type  Description 
ENTRY_NO  Integer  Same value as the header table.  
WAREHOUSE_ID  String  Same value as the header table.  
LOCATION_ID  String  Warehouse Location ID.  
PART_ID  String  Same value as the header table.  
GetPartDefaultWhseLoc Data Tables  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 347 See Also 
GetPartDefaultWhseLoc Class  
Lsa.Vmfg.Shared Namespace  
  
GetSupplyLinks Class  
348 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetSupplyLinks Class  
Service to obtain all linked supply to a given demand.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessService  
          Lsa.Vmfg.Shared.GetSupplyLinks  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  GetSupplyLinks  : BusinessService  
 
VB 
<SerializableAttribute> 
Public  Class  GetSupplyLinks  
 Inherits  BusinessService 
 
The GetSupplyLinks  type exposes the following members.  
Constructors 
 Name  Description  
 GetSupplyLinks()  Initializes a new instance of the GetSupplyLinks  class 
 GetSupplyLinks(String)  Initializes a new instance of the GetSupplyLinks  class 
 

GetSupplyLinks Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 349 Methods 
 Name  Description  
 Execute  Executes the Service.  
 NewInputRow  Adds a new request row to the Service.  
 Prepare  Prepares the Service for execution.  
 
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object 
(Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType   (Overrides BusinessObject.ServicedComponentType.)  
Data Tables 
 Table Type  Table Name  
 Header Table  SUPPLY_LINKS  
 Results Sub- table SUPPLY_LINKS_RESULT  
See Also 
Lsa.Vmfg.Shared Namespace  
  

GetSupplyLinks Constructor  
350 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetSupplyLinks Constructor  
Overload List 
 Name  Description  
 GetSupplyLinks()  Initializes a new instance of the GetSupplyLinks  class 
 GetSupplyLinks(String)  Initializes a new instance of the GetSupplyLinks  class 
 
See Also 
GetSupplyLinks Class  
Lsa.Vmfg.Shared Namespace  
  

GetSupplyLinks Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 351 GetSupplyLinks Constructor  
Initializes a new instance of the GetSupplyLinks  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  GetSupplyLinks () 
 
VB 
Public  Sub New 
 
See Also 
GetSupplyLinks Class  
GetSupplyLinks Overload  
Lsa.Vmfg.Shared Namespace  
  
GetSupplyLinks Constructor  (String) 
352 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetSupplyLinks Constructor (String)  
Initializes a new instance of the GetSupplyLinks  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  GetSupplyLinks ( 
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
GetSupplyLinks Class  
GetSupplyLinks Overload  
Lsa.Vmfg.Shared Namespace  
  
GetSupplyLinks.GetSupplyLinks  Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 353 GetSupplyLinks.GetSupplyLinks Methods  
The GetSupplyLinks  type exposes the following members.  
Methods 
 Name  Description  
 Execute  Executes the Service.  
 NewInputRow  Adds a new request row to the Service.  
 Prepare  Prepares the Service for execution.  
 
See Also 
GetSupplyLinks Class  
Lsa.Vmfg.Shared Namespace  
  

GetSupplyLinks.Execute Method 
354 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetSupplyLinks.Execute Method  
Executes the Service.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Execute () 
 
VB 
Public  Overridable  Sub Execute  
 
See Also 
GetSupplyLinks Class  
Lsa.Vmfg.Shared Namespace  
  
GetSupplyLinks.NewInputRow  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 355 GetSupplyLinks.NewInputRow Method  
Adds a new request row to the Service.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
GetSupplyLinks Class  
Lsa.Vmfg.Shared Namespace  
  
GetSupplyLinks.Prepare Method  
356 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetSupplyLinks.Prepare Method  
Prepares the Service for execution.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Prepare()  
 
VB 
Public  Overridable  Sub Prepare  
 
See Also 
GetSupplyLinks Class  
Lsa.Vmfg.Shared Namespace  
  
GetSupplyLinks.GetSupplyLinks  Properties  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 357 GetSupplyLinks.GetSupplyLinks Properties  
The GetSupplyLinks  type exposes the following members.  
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object 
(Overrides BusinessObject.DataObjectType.)  
 ServicedComponentType   (Overrides BusinessObject.ServicedComponentType.)  
 
See Also 
GetSupplyLinks Class  
Lsa.Vmfg.Shared Namespace  
  

GetSupplyLinks.DataObjectType Property  
358 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetSupplyLinks.DataObjectType Property  
Report types of data object associated with this business object  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  DataObjectType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  DataObjectType As Type 
 Get 
 
Property Value  
Type: Type  
See Also 
GetSupplyLinks Class  
Lsa.Vmfg.Shared Namespace  
  
GetSupplyLinks.ServicedComponentType Property  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 359 GetSupplyLinks.ServicedComponentType Property  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  override  Type  ServicedComponentType { get; } 
 
VB 
Public  Overrides  ReadOnly  Property  ServicedComponentType As Type  
 Get 
 
Property Value  
Type: Type  
See Also 
GetSupplyLinks Class  
Lsa.Vmfg.Shared Namespace  
  
GetSupplyLinks Data Tables  
360 | Infor VISUAL API Toolkit  Shared Class Library Reference  GetSupplyLinks Data Tables  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
DataSet Name returned from Prepare:  SUPPLY_LINKS  
Header Table  
Table Name:  SUPPLY_LINKS 
Primary Key:  ENTRY_NO  
Column Name  Type  Description 
ENTRY_NO  Integer  Unique integer identifier for this row.  
Required.  
IS_RETURN Boolean Flag indicating whether the order is a return 
Demand Order. Required  
DEMAND_TYPE String Type of the demand link. Acceptable types include CD, CO, RQ, and WH. Required.  
DEMAND_BASE_ID  String Base ID of the demand link. Required.  
DEMAND_LOT_ID  String   Lot ID of the demand link. Value depends 
on demand type.  
DEMAND _SPLIT_ID  String Split ID of the demand link. Value depends on demand type.  
DEMAND_SUB_ID  String  Sub ID of the demand link. Value depends on demand type.  
DEMAND _SEQ_NO  Integer  Operation sequence number of the demand link. Value depends on demand type.  
DEMAND _NO Integer  Demand Number. Value depends on demand type.  
TRANSACTION_QTY Decimal  Total transaction quantity. Required.  
Results Sub-table 
Table Name : SUPPLY_LINKS_RESULT 
GetSupplyLinks Data Tables  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 361 Column Name  Type  Description 
ENTRY_NO  Integer  Same value as the header table.  
DSL_ID  Integer  Demand- Supply Link ID.  
WO_TYPE  String  Work Order Supply Link Type.  
SUPPLY_TYPE  String  Type of the supply link.  
SUPPLY_BASE_ID  String  Base ID of the supply link.  
SUPPLY_LOT_ID  String  Lot ID of the supply link.  
SUPPLY_SPLIT_ID  String  Split ID of the supply link.  
SUPPLY_SUB_ID  String  Sub ID of the supply link.  
SUPPLY_SEQ_NO  Integer  Operation Sequence Number of 
the supply link.  
SUPPLY_NO  Integer  Supply link number.  
ASSIGN_QTY  Decimal  Quantity assigned to this supply 
link. 
RECEIVE _QTY  Decimal  Quantity to be received (difference of Link Allocated Qty and Link Received Qty).  
LINK_ALLOCATED_QTY  Decimal  Quantity allocated for this supply 
link. 
LINK_RECEIVED_QTY Decimal  Quantity received for this supply link. 
LINK_ISSUED_QTY Decimal  Quantity issued for this supply link.  
ALLOCATED_QTY  Decimal  Allocated quantity of the supply 
order.  
FULFILLED_QTY  Decimal  Fulfilled quantity of the supply order.  
ISSUED_QTY Decimal  Issued quantity of the supply link.  
PART_ID  String  Part ID of the supply link.  
DESIRED_QTY  Decimal  Desired/order quantity of the 
supply order.  
RECEIVED_QTY Decimal  Received quantity of the supply link. 
OVER_ASSIGN_QTY  Decimal  Quantity remaining.  
VENDOR_ID  String  Vendor ID of the supply order.  
GetSupplyLinks Data Tables  
362 | Infor VISUAL API Toolkit  Shared Class Library Reference  Column Name  Type  Description 
VENDOR_PART_ID  String  Vendor Part ID of the supply order.  
DESIRED_REC_DATE  Date  Desired supply order receive date.  
IS_RETURN Boolean Same value as header table.  
See Also 
GetPartDefaultWhseLoc Class  
Lsa.Vmfg.Shared Namespace  
  
GLInterface Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 363 GLInterface Class  
Maintain GL Interface Accounts.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.GLInterface  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  GLInterface  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  GLInterface  
 Inherits  BusinessDocument  
 
The GLInterface  type exposes the following members.  
Constructors 
 Name  Description  
 GLInterface()  Initializes a new instance of the GLInterface  class 
 GLInterface(String)  Initializes a new instance of the GLInterface  class 
 

GLInterface Class  
364 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve GL Interface Accounts based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve GL Interface Accounts based on search criteria, 
limited by record count.  
 Exists  Determine if a GL Interface Account exists.  
 Find Find a specific GL Interface Account.  
 Load()  Load all GL Interface Accounts.  
 Load(Stream, String)  Load from stream and rename using new key.  
 Load(String, String)  Load a specific GL Interface Account row.  
 NewGLInterfaceAcctRow  Adds a new row to the GL INTERFACE ACCT table  
 NewGLInterfaceRow  Adds a new row to the GL_INTERFACE table.  
 Save  Save a previously loaded GL Interface Accounts to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

GLInterface Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 365 GLInterface Constructor  
Overload List 
 Name  Description  
 GLInterface()  Initializes a new instance of the GLInterface  class 
 GLInterface(String)  Initializes a new instance of the GLInterface  class 
 
See Also 
GLInterface Class  
Lsa.Vmfg.Shared Namespace  
  

GLInterface Constructor  
366 | Infor VISUAL API Toolkit  Shared Class Library Reference  GLInterface Constructor  
Initializes a new instance of the GLInterface  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  GLInterface () 
 
VB 
Public  Sub New 
 
See Also 
GLInterface Class  
GLInterface Overload  
Lsa.Vmfg.Shared Namespace  
  
GLInterface Constructor  (String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 367 GLInterface Constructor (String)  
Initializes a new instance of the GLInterface  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  GLInterface ( 
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
GLInterface Class  
GLInterface Overload  
Lsa.Vmfg.Shared Namespace  
  
GLInterface.GLInterface Methods  
368 | Infor VISUAL API Toolkit  Shared Class Library Reference  GLInterface.GLInterface Methods  
The GLInterface  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve GL Interface Accounts based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve GL Interface Accounts based on search criteria, 
limited by record count.  
 Exists  Determine if a GL Interface Account exists.  
 Find Find a specific GL Interface Account.  
 Load()  Load all GL Interface Accounts.  
 Load(Stream, String)  Load from stream and rename using new key.  
 Load(String, String)  Load a specific GL Interface Account row.  
 NewGLInterfaceAcctRow  Adds a new row to the GL INTERFACE ACCT table  
 NewGLInterfaceRow  Adds a new row to the GL_INTERFACE table.  
 Save  Save a previously loaded GL Interface Accounts to the database.  
 
See Also 
GLInterface Class  
Lsa.Vmfg.Shared Namespace  
  

GLInterface.Browse  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 369 GLInterface.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve GL Interface Accounts based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve GL Interface Accounts based on search criteria, 
limited by record count.  
 
See Also 
GLInterface Class  
Lsa.Vmfg.Shared Namespace  
  

GLInterface.Browse  Method (String, String, String) 
370 | Infor VISUAL API Toolkit  Shared Class Library Reference  GLInterface.Browse Method (String, String, String)  
Retrieve GL Interface Accounts based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
GLInterface Class  
GLInterface.Browse  Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 371 Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
GLInterface.Browse  Method (String, String, String, Int32, Int32)  
372 | Infor VISUAL API Toolkit  Shared Class Library Reference  GLInterface.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve GL Interface Accounts based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
GLInterface.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 373 Return Value  
Type: DataSet  
See Also 
GLInterface Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
GLInterface.Exists Method  
374 | Infor VISUAL API Toolkit  Shared Class Library Reference  GLInterface.Exists Method  
Determine if a GL Interface Account exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  siteID , 
 string  interfaceID  
) 
 
VB 
Public  Overridable  Function Exists  (  
 siteID  As String , 
 interfaceID  As String 
) As Boolean 
 
Parameters  
siteID  
Type: System.String  
interfaceID  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
GLInterface Class  
Lsa.Vmfg.Shared Namespace  
  
GLInterface.Find  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 375 GLInterface.Find Method  
Find a specific GL Interface Account.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  siteID , 
 string  interfaceID  
) 
 
VB 
Public  Overridable  Sub Find (  
 siteID  As String , 
 interfaceID  As String 
) 
 
Parameters  
siteID  
Type: System.String  
interfaceID  
Type: System.String  
See Also 
GLInterface Class  
Lsa.Vmfg.Shared Namespace  
  
GLInterface.Load  Method  
376 | Infor VISUAL API Toolkit  Shared Class Library Reference  GLInterface.Load Method  
Overload List 
 Name  Description  
 Load()  Load all GL Interface Accounts.  
 Load(Stream, String)  Load from stream and rename using new key.  
 Load(String, String)  Load a specific GL Interface Account row.  
 
See Also 
GLInterface Class  
Lsa.Vmfg.Shared Namespace  
  

GLInterface.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 377 GLInterface.Load Method  
Load all GL Interface Accounts.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
GLInterface Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
GLInterface.Load  Method (Stream, String)  
378 | Infor VISUAL API Toolkit  Shared Class Library Reference  GLInterface.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
GLInterface Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
GLInterface.Load  Method (String, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 379 GLInterface.Load Method (String, String)  
Load a specific GL Interface Account row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  siteID , 
 string  interfaceID  
) 
 
VB 
Public  Overridable  Sub Load (  
 siteID  As String , 
 interfaceID  As String 
) 
 
Parameters  
siteID  
Type: System.String  
interfaceID  
Type: System.String  
See Also 
GLInterface Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
GLInterface.NewGLInterfaceAcctRow  Method  
380 | Infor VISUAL API Toolkit  Shared Class Library Reference  GLInterface.NewGLInterfaceAcctRow Method  
Adds a new row to the GL INTERFACE ACCT table  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewGLInterfaceAcctRow ( 
 string  siteID , 
 string  interfaceID  
) 
 
VB 
Public  Function  NewGLInterfaceAcctRow  (  
 siteID  As String , 
 interfaceID  As String 
) As DataRow  
 
Parameters  
siteID  
Type: System.String  
interfaceID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
GLInterface Class  
Lsa.Vmfg.Shared Namespace  
  
GLInterface.NewGLInterfaceRow  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 381 GLInterface.NewGLInterfaceRow Method  
Adds a new row to the GL_INTERFACE table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewGLInterfaceRow ( 
 string  id 
) 
 
VB 
Public  Function  NewGLInterfaceRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
GLInterface Class  
Lsa.Vmfg.Shared Namespace  
  
GLInterface.Save Method 
382 | Infor VISUAL API Toolkit  Shared Class Library Reference  GLInterface.Save Method  
Save a previously loaded GL Interface Accounts to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
GLInterface Class  
Lsa.Vmfg.Shared Namespace  
  
HarmonizedTariff Class 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 383 HarmonizedTariff Class  
Maintain Harmonized Tariff Codes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.HarmonizedTariff  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  HarmonizedTariff  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  HarmonizedTariff  
 Inherits  BusinessDocument  
 
The HarmonizedTariff type exposes the following members.  
Constructors 
 Name  Description  
 HarmonizedTariff()  Business Documnet Constructor  
 HarmonizedTariff(String)  Business Document Constructor  
 

HarmonizedTariff Class 
384 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Harmonized Tariff Codes based on search 
criteria.  
 Browse(String, String, String, Int32, 
Int32)   
 Exists  Determine if a Harmonized Tariff Code exists.  
 Find Find a specific Harmonized Tariff Code.  
 Load()  Load all Harmonized Tariff Codes.  
 Load(String)  Load a specific Harmonized Tariff Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewHarmonizedTariffRow  Adds a new row to the HARMONIZED_TARIFF table.  
 Save  Save previously loaded Harmonized Tariff Code(s) to the 
database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

HarmonizedTariff Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 385 HarmonizedTariff Constructor  
Overload List 
 Name  Description  
 HarmonizedTariff()  Business Documnet Constructor  
 HarmonizedTariff(String)  Business Document Constructor  
 
See Also 
HarmonizedTariff Class  
Lsa.Vmfg.Shared Namespace  
  

HarmonizedTariff Constructor  
386 | Infor VISUAL API Toolkit  Shared Class Library Reference  HarmonizedTariff Constructor  
Business Documnet Constructor  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  HarmonizedTariff () 
 
VB 
Public  Sub New 
 
See Also 
HarmonizedTariff Class  
HarmonizedTariff Overload  
Lsa.Vmfg.Shared Namespace  
  
HarmonizedTariff Constructor  (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 387 HarmonizedTariff Constructor (String)  
Business Document Constructor  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  HarmonizedTariff ( 
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
HarmonizedTariff Class  
HarmonizedTariff Overload  
Lsa.Vmfg.Shared Namespace  
  
HarmonizedTariff.HarmonizedTariff  Methods  
388 | Infor VISUAL API Toolkit  Shared Class Library Reference  HarmonizedTariff.HarmonizedTariff Methods  
The HarmonizedTariff  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Harmonized Tariff Codes based on search 
criteria.  
 Browse(String, String, String, Int32, 
Int32)   
 Exists  Determine if a Harmonized Tariff Code exists.  
 Find Find a specific Harmonized Tariff Code.  
 Load()  Load all Harmonized Tariff Codes.  
 Load(String)  Load a specific Harmonized Tariff Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewHarmonizedTariffRow  Adds a new row to the HARMONIZED_TARIFF table.  
 Save  Save previously loaded Harmonized Tariff Code(s) to the 
database.  
 
See Also 
HarmonizedTariff Class  
Lsa.Vmfg.Shared Namespace  
  

HarmonizedTariff.Browse Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 389 HarmonizedTariff.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Harmonized Tariff Codes based on search 
criteria.  
 Browse(String, String, String, Int32, 
Int32)   
 
See Also 
HarmonizedTariff Class  
Lsa.Vmfg.Shared Namespace  
  

HarmonizedTariff.Browse Method (String, String, String) 
390 | Infor VISUAL API Toolkit  Shared Class Library Reference  HarmonizedTariff.Browse Method (String, String, 
String)  
Retrieve Harmonized Tariff Codes based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
 
HarmonizedTariff.Browse Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 391 See Also 
HarmonizedTariff Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
HarmonizedTariff.Browse Method (String, String, String, Int32, Int32)  
392 | Infor VISUAL API Toolkit  Shared Class Library Reference  HarmonizedTariff.Browse Method (String, String, 
String, Int32, Int32)  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
HarmonizedTariff.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 393 Return Value  
Type: DataSet  
See Also 
HarmonizedTariff Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
HarmonizedTariff.Exists Method  
394 | Infor VISUAL API Toolkit  Shared Class Library Reference  HarmonizedTariff.Exists Method  
Determine if a Harmonized Tariff Code exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  code 
) 
 
VB 
Public  Overridable  Function Exists  (  
 code As String  
) As Boolean 
 
Parameters  
code  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
HarmonizedTariff Class  
Lsa.Vmfg.Shared Namespace  
  
HarmonizedTariff.Find  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 395 HarmonizedTariff.Find Method  
Find a specific Harmonized Tariff Code.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Find (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
HarmonizedTariff Class  
Lsa.Vmfg.Shared Namespace  
  
HarmonizedTariff.Load  Method  
396 | Infor VISUAL API Toolkit  Shared Class Library Reference  HarmonizedTariff.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Harmonized Tariff Codes.  
 Load(String)  Load a specific Harmonized Tariff Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
HarmonizedTariff Class  
Lsa.Vmfg.Shared Namespace  
  

HarmonizedTariff.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 397 HarmonizedTariff.Load Method  
Load all Harmonized Tariff Codes.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
HarmonizedTariff Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
HarmonizedTariff.Load  Method (String)  
398 | Infor VISUAL API Toolkit  Shared Class Library Reference  HarmonizedTariff.Load Method (String)  
Load a specific Harmonized Tariff Code row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
HarmonizedTariff Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
HarmonizedTariff.Load  Method (Stream, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 399 HarmonizedTariff.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 code As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
code  
Type: System.String  
See Also 
HarmonizedTariff Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
HarmonizedTariff.NewHarmonizedTariffRow  Method  
400 | Infor VISUAL API Toolkit  Shared Class Library Reference  HarmonizedTariff.NewHarmonizedTariffRow Method  
Adds a new row to the HARMONIZED_TARIFF table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewHarmonizedTariffRow ( 
 string  harmonizedTariffID  
) 
 
VB 
Public  Function  NewHarmonizedTariffRow  (  
 harmonizedTariffID  As String  
) As DataRow  
 
Parameters  
harmonizedTariffID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
HarmonizedTariff Class  
Lsa.Vmfg.Shared Namespace  
  
HarmonizedTariff.Save Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 401 HarmonizedTariff.Save Method  
Save previously loaded Harmonized Tariff Code(s) to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
HarmonizedTariff Class  
Lsa.Vmfg.Shared Namespace  
  
Honorific Class  
402 | Infor VISUAL API Toolkit  Shared Class Library Reference  Honorific Class  
Maintain Honorific Codes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Honorific  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  Honorific  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  Honorific  
 Inherits  BusinessDocument  
 
The Honorific  type exposes the following members.  
Constructors 
 Name  Description  
 Honorific()  Initializes a new instance of the Honorific  class 
 Honorific(String)  Initializes a new instance of the Honorific  class 
 

Honorific Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 403 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Honorific Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Honorific Codes based on search criteria, limited 
by record count.  
 Exists  Determine if a Honorific Code exists.  
 Find Find a specific Honorific Code.  
 Load()  Load all Honorific Codes.  
 Load(String)  Load a specific Honorific Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewHonorificRow  Adds a new row to the HONORIFIC table.  
 Save  Save previously loaded Honorific Code(s) to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Honorific Constructor  
404 | Infor VISUAL API Toolkit  Shared Class Library Reference  Honorific Constructor  
Overload List 
 Name  Description  
 Honorific()  Initializes a new instance of the Honorific  class 
 Honorific(String)  Initializes a new instance of the Honorific  class 
 
See Also 
Honorific Class  
Lsa.Vmfg.Shared Namespace  
  

Honorific Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 405 Honorific Constructor  
Initializes a new instance of the Honorific  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Honorific () 
 
VB 
Public  Sub New 
 
See Also 
Honorific Class  
Honorific Overload  
Lsa.Vmfg.Shared Namespace  
  
Honorific Constructor  (String) 
406 | Infor VISUAL API Toolkit  Shared Class Library Reference  Honorific Constructor (String)  
Initializes a new instance of the Honorific  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Honorific ( 
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
Honorific Class  
Honorific Overload  
Lsa.Vmfg.Shared Namespace  
  
Honorific.Honorific  Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 407 Honorific.Honorific Methods  
The Honorific  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Honorific Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Honorific Codes based on search criteria, limited 
by record count.  
 Exists  Determine if a Honorific Code exists.  
 Find Find a specific Honorific Code.  
 Load()  Load all Honorific Codes.  
 Load(String)  Load a specific Honorific Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewHonorificRow  Adds a new row to the HONORIFIC table.  
 Save  Save previously loaded Honorific Code(s) to the database.  
 
See Also 
Honorific Class  
Lsa.Vmfg.Shared Namespace  
  

Honorific.Browse  Method  
408 | Infor VISUAL API Toolkit  Shared Class Library Reference  Honorific.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Honorific Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Honorific Codes based on search criteria, limited 
by record count.  
 
See Also 
Honorific Class  
Lsa.Vmfg.Shared Namespace  
  

Honorific.Browse  Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 409 Honorific.Browse Method (String, String, String)  
Retrieve Honorific Codes based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Honorific Class  
Honorific.Browse  Method (String, String, String) 
410 | Infor VISUAL API Toolkit  Shared Class Library Reference  Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Honorific.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 411 Honorific.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve Honorific Codes based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Honorific.Browse  Method (String, String, String, Int32, Int32)  
412 | Infor VISUAL API Toolkit  Shared Class Library Reference  Return Value  
Type: DataSet  
See Also 
Honorific Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Honorific.Exists  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 413 Honorific.Exists Method  
Determine if a Honorific Code exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  name  
) 
 
VB 
Public  Overridable  Function Exists  (  
 name As String 
) As Boolean 
 
Parameters  
name  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
Honorific Class  
Lsa.Vmfg.Shared Namespace  
  
Honorific.Find  Method 
414 | Infor VISUAL API Toolkit  Shared Class Library Reference  Honorific.Find Method  
Find a specific Honorific Code.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  name  
) 
 
VB 
Public  Overridable  Sub Find (  
 name As String 
) 
 
Parameters  
name  
Type: System.String  
See Also 
Honorific Class  
Lsa.Vmfg.Shared Namespace  
  
Honorific.Load Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 415 Honorific.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Honorific Codes.  
 Load(String)  Load a specific Honorific Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
Honorific Class  
Lsa.Vmfg.Shared Namespace  
  

Honorific.Load Method  
416 | Infor VISUAL API Toolkit  Shared Class Library Reference  Honorific.Load Method  
Load all Honorific Codes.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
Honorific Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Honorific.Load Method (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 417 Honorific.Load Method (String)  
Load a specific Honorific Code row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  name  
) 
 
VB 
Public  Overridable  Sub Load (  
 name As String 
) 
 
Parameters  
name  
Type: System.String  
See Also 
Honorific Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Honorific.Load Method (Stream, String)  
418 | Infor VISUAL API Toolkit  Shared Class Library Reference  Honorific.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  name  
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 name As String 
) 
 
Parameters  
stream  
Type: System.IO.Stream  
name  
Type: System.String  
See Also 
Honorific Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Honorific.NewHonorificRow  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 419 Honorific.NewHonorificRow Method  
Adds a new row to the HONORIFIC table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewHonorificRow ( 
 string  id 
) 
 
VB 
Public  Function  NewHonorificRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Honorific Class  
Lsa.Vmfg.Shared Namespace  
  
Honorific.Save  Method 
420 | Infor VISUAL API Toolkit  Shared Class Library Reference  Honorific.Save Method  
Save previously loaded Honorific Code(s) to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Honorific Class  
Lsa.Vmfg.Shared Namespace  
  
Indirect Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 421 Indirect Class  
Maintain Indirect IDs.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Indirect  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  Indirect  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  Indirect  
 Inherits  BusinessDocument  
 
The Indirect  type exposes the following members.  
Constructors 
 Name  Description  
 Indirect()  Initializes a new instance of the Indirect  class 
 Indirect(String)  Initializes a new instance of the Indirect  class 
 

Indirect Class  
422 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Indirect IDs based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Indirect IDs based on search criteria, limited by 
record count.  
 Exists  Determine if an Indirect ID exists.  
 Find Find a specific Indirect ID.  
 Load()  Load all Indirect IDs.  
 Load(String)  Load a specific Indirect ID row. 
 Load(Stream, String)  Load from stream and rename using new key.  
 NewIndirectRow  Adds a new row to the INDIRECT table.  
 Save  Save all previously loaded Indirect IDs to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Indirect Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 423 Indirect Constructor  
Overload List 
 Name  Description  
 Indirect()  Initializes a new instance of the Indirect  class 
 Indirect(String)  Initializes a new instance of the Indirect  class 
 
See Also 
Indirect Class  
Lsa.Vmfg.Shared Namespace  
  

Indirect Constructor  
424 | Infor VISUAL API Toolkit  Shared Class Library Reference  Indirect Constructor  
Initializes a new instance of the Indirect  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Indirect () 
 
VB 
Public  Sub New 
 
See Also 
Indirect Class  
Indirect Overload  
Lsa.Vmfg.Shared Namespace  
  
Indirect Constructor  (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 425 Indirect Constructor (String)  
Initializes a new instance of the Indirect  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Indirect ( 
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
Indirect Class  
Indirect Overload  
Lsa.Vmfg.Shared Namespace  
  
Indirect.Indirect  Methods  
426 | Infor VISUAL API Toolkit  Shared Class Library Reference  Indirect.Indirect Methods  
The Indirect  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Indirect IDs based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Indirect IDs based on search criteria, limited by 
record count.  
 Exists  Determine if an Indirect ID exists.  
 Find Find a specific Indirect ID.  
 Load()  Load all Indirect IDs.  
 Load(String)  Load a specific Indirect ID row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewIndirectRow  Adds a new row to the INDIRECT table.  
 Save  Save all previously loaded Indirect IDs to the database.  
 
See Also 
Indirect Class  
Lsa.Vmfg.Shared Namespace  
  

Indirect.Browse Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 427 Indirect.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Indirect IDs based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Indirect IDs based on search criteria, limited by 
record count.  
 
See Also 
Indirect Class  
Lsa.Vmfg.Shared Namespace  
  

Indirect.Browse Method (String, String, String)  
428 | Infor VISUAL API Toolkit  Shared Class Library Reference  Indirect.Browse Method (String, String, String)  
Retrieve Indirect IDs based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Indirect Class  
Indirect.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 429 Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Indirect.Browse Method (String, String, String, Int32, Int32)  
430 | Infor VISUAL API Toolkit  Shared Class Library Reference  Indirect.Browse Method (String, String, String, Int32, 
Int32)  
Retrieve Indirect IDs based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Indirect.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 431 Return Value  
Type: DataSet  
See Also 
Indirect Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Indirect.Exists  Method 
432 | Infor VISUAL API Toolkit  Shared Class Library Reference  Indirect.Exists Method  
Determine if an Indirect ID exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Indirect Class  
Lsa.Vmfg.Shared Namespace  
  
Indirect.Find Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 433 Indirect.Find Method  
Find a specific Indirect ID.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Indirect Class  
Lsa.Vmfg.Shared Namespace  
  
Indirect.Load  Method  
434 | Infor VISUAL API Toolkit  Shared Class Library Reference  Indirect.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Indirect IDs.  
 Load(String)  Load a specific Indirect ID row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
Indirect Class  
Lsa.Vmfg.Shared Namespace  
  

Indirect.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 435 Indirect.Load Method  
Load all Indirect IDs.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
Indirect Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Indirect.Load  Method (String) 
436 | Infor VISUAL API Toolkit  Shared Class Library Reference  Indirect.Load Method (String)  
Load a specific Indirect ID row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Indirect Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Indirect.Load  Method (Stream, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 437 Indirect.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Indirect Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Indirect.NewIndirectRow  Method  
438 | Infor VISUAL API Toolkit  Shared Class Library Reference  Indirect.NewIndirectRow Method  
Adds a new row to the INDIRECT table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewIndirectRow( 
 string  id 
) 
 
VB 
Public  Function  NewIndirectRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Indirect Class  
Lsa.Vmfg.Shared Namespace  
  
Indirect.Save Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 439 Indirect.Save Method  
Save all previously loaded Indirect IDs to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Indirect Class  
Lsa.Vmfg.Shared Namespace  
  
Language Class  
440 | Infor VISUAL API Toolkit  Shared Class Library Reference  Language Class  
Maintain Language IDs.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Language  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  class  Language : BusinessDocument  
 
VB 
Public  Class  Language  
 Inherits  BusinessDocument  
 
The Language  type exposes the following members.  
Constructors 
 Name  Description  
 Language()  Constructor  
 Language(String)  Initializes a new instance of the Language  class 
 

Language Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 441 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Language IDs based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Language IDs based on search criteria, limited by 
record count.  
 Exists  Determine if a Language ID exists.  
 Find Find a specific Language ID.  
 Load()  Load all Language IDs  
 Load(String)  Load a specific Language ID row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewLanguageRow  Add a new LANGUAGE row to the database  
 Save  Save previously loaded Language IDs to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Language Constructor  
442 | Infor VISUAL API Toolkit  Shared Class Library Reference  Language Constructor  
Overload List 
 Name  Description  
 Language()  Constructor  
 Language(String)  Initializes a new instance of the Language  class 
 
See Also 
Language Class  
Lsa.Vmfg.Shared Namespace  
  

Language Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 443 Language Constructor  
Constructor  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Language()  
 
VB 
Public  Sub New 
 
See Also 
Language Class  
Language Overload  
Lsa.Vmfg.Shared Namespace  
  
Language Constructor  (String)  
444 | Infor VISUAL API Toolkit  Shared Class Library Reference  Language Constructor (String)  
Initializes a new instance of the Language  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Language( 
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
Language Class  
Language Overload  
Lsa.Vmfg.Shared Namespace  
  
Language.Language  Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 445 Language.Language Methods  
The Language  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Language IDs based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Language IDs based on search criteria, limited by 
record count.  
 Exists  Determine if a Language ID exists.  
 Find Find a specific Language ID.  
 Load()  Load all Language IDs  
 Load(String)  Load a specific Language ID row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewLanguageRow  Add a new LANGUAGE row to the database  
 Save  Save previously loaded Language IDs to the database.  
 
See Also 
Language Class  
Lsa.Vmfg.Shared Namespace  
  

Language.Browse  Method  
446 | Infor VISUAL API Toolkit  Shared Class Library Reference  Language.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Language IDs based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Language IDs based on search criteria, limited by 
record count.  
 
See Also 
Language Class  
Lsa.Vmfg.Shared Namespace  
  

Language.Browse  Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 447 Language.Browse Method (String, String, String)  
Retrieve Language IDs based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Language Class  
Language.Browse  Method (String, String, String) 
448 | Infor VISUAL API Toolkit  Shared Class Library Reference  Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Language.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 449 Language.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve Language IDs based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Language.Browse  Method (String, String, String, Int32, Int32)  
450 | Infor VISUAL API Toolkit  Shared Class Library Reference  Return Value  
Type: DataSet  
See Also 
Language Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Language.Exists  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 451 Language.Exists Method  
Determine if a Language ID exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  languageID  
) 
 
VB 
Public  Overridable  Function Exists  (  
 languageID  As String  
) As Boolean 
 
Parameters  
languageID  
Type: System.String  
Return Value  
Type: Boolean  
 
See Also 
Language Class  
Lsa.Vmfg.Shared Namespace  
  
Language.Find Method  
452 | Infor VISUAL API Toolkit  Shared Class Library Reference  Language.Find Method  
Find a specific Language ID.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  languageID  
) 
 
VB 
Public  Overridable  Sub Find (  
 languageID  As String  
) 
 
Parameters  
languageID  
Type: System.String  
See Also 
Language Class  
Lsa.Vmfg.Shared Namespace  
  
Language.Load  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 453 Language.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Language IDs  
 Load(String)  Load a specific Language ID row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
Language Class  
Lsa.Vmfg.Shared Namespace  
  

Language.Load  Method 
454 | Infor VISUAL API Toolkit  Shared Class Library Reference  Language.Load Method  
Load all Language IDs  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
Language Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Language.Load  Method (String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 455 Language.Load Method (String)  
Load a specific Language ID row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  languageID  
) 
 
VB 
Public  Overridable  Sub Load (  
 languageID  As String  
) 
 
Parameters  
languageID  
Type: System.String  
See Also 
Language Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Language.Load  Method (Stream, String)  
456 | Infor VISUAL API Toolkit  Shared Class Library Reference  Language.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  languageID  
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 languageID  As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
languageID  
Type: System.String  
See Also 
Language Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Language.NewLanguageRow  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 457 Language.NewLanguageRow Method  
Add a new LANGUAGE row to the database  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewLanguageRow ( 
 string  languageID  
) 
 
VB 
Public  Function  NewLanguageRow  (  
 languageID  As String  
) As DataRow  
 
Parameters  
languageID  
Type: System.String  
Return Value  
Type: DataRow  
 
See Also 
Language Class  
Lsa.Vmfg.Shared Namespace  
  
Language.Save  Method  
458 | Infor VISUAL API Toolkit  Shared Class Library Reference  Language.Save Method  
Save previously loaded Language IDs to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Language Class  
Lsa.Vmfg.Shared Namespace  
  
ModeOfTransport Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 459 ModeOfTransport Class  
Maintain Mode Of Transport Codes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.ModeOfTransport  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  ModeOfTransport  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  ModeOfTransport  
 Inherits  BusinessDocument  
 
The ModeOfTransport  type exposes the following members.  
Constructors 
 Name  Description  
 ModeOfTransport()  Constructor  
 ModeOfTransport(String)  Constructor  
 

ModeOfTransport Class  
460 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Mode Of Transport Codes based on search 
criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Mode Of Transport Codes based on search criteria, limited by record count.  
 Exists  Verify the presence of a specific Mode Of Transport Code in the database.  
 Find Retrieve a specific Mode Of Transport Code.  
 Load()  Loads all of the Mode Of Transport Codes from the database.  
 Load(String)  Loads a specific Mode Of Transport Code from the database.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewModeOfTransportRow  Inserts a new row into the MODE_OF_TRANSPORT table.  
 Save()  Save previously loaded Mode Of Transport Code(s) data to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

ModeOfTransport Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 461 ModeOfTransport Constructor  
Overload List 
 Name  Description  
 ModeOfTransport()  Constructor  
 ModeOfTransport(String)  Constructor  
 
See Also 
ModeOfTransport Class  
Lsa.Vmfg.Shared Namespace  
  

ModeOfTransport Constructor  
462 | Infor VISUAL API Toolkit  Shared Class Library Reference  ModeOfTransport Constructor  
Constructor  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  ModeOfTransport () 
 
VB 
Public  Sub New 
 
See Also 
ModeOfTransport Class  
ModeOfTransport Overload  
Lsa.Vmfg.Shared Namespace  
  
ModeOfTransport Constructor  (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 463 ModeOfTransport Constructor (String)  
Constructor  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  ModeOfTransport ( 
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
ModeOfTransport Class  
ModeOfTransport Overload  
Lsa.Vmfg.Shared Namespace  
  
ModeOfTransport.ModeOfTransport  Methods  
464 | Infor VISUAL API Toolkit  Shared Class Library Reference  ModeOfTransport.ModeOfTransport Methods  
The ModeOfTransport  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Mode Of Transport Codes based on search 
criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Mode Of Transport Codes based on search criteria, limited by record count.  
 Exists  Verify the presence of a specific Mode Of Transport Code in the database.  
 Find Retrieve a specific Mode Of Transport Code.  
 Load()  Loads all of the Mode Of Transport Codes from the database.  
 Load(String)  Loads a specific Mode Of Transport Code from the database.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewModeOfTransportRow  Inserts a new row into the MODE_OF_TRANSPORT table.  
 Save()  Save previously loaded Mode Of Transport Code(s) data to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
See Also 
ModeOfTransport Class  
Lsa.Vmfg.Shared Namespace  
  

ModeOfTransport.Browse  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 465 ModeOfTransport.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Mode Of Transport Codes based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Mode Of Transport Codes based on search criteria, 
limited by record count.  
 
See Also 
ModeOfTransport Class  
Lsa.Vmfg.Shared Namespace  
  

ModeOfTransport.Browse  Method (String, String, String) 
466 | Infor VISUAL API Toolkit  Shared Class Library Reference  ModeOfTransport.Browse Method (String, String, 
String)  
Retrieve Mode Of Transport Codes based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ModeOfTransport.Browse  Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 467 See Also 
ModeOfTransport Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
ModeOfTransport.Browse  Method (String, String, String, Int32, Int32)  
468 | Infor VISUAL API Toolkit  Shared Class Library Reference  ModeOfTransport.Browse Method (String, String, 
String, Int32, Int32)  
Retrieve Mode Of Transport Codes based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ModeOfTransport.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 469 Return Value  
Type: DataSet  
See Also 
ModeOfTransport Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
ModeOfTransport.Exists  Method  
470 | Infor VISUAL API Toolkit  Shared Class Library Reference  ModeOfTransport.Exists Method  
Verify the presence of a specific Mode Of Transport Code in the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  code 
) 
 
VB 
Public  Overridable  Function Exists  (  
 code As String  
) As Boolean 
 
Parameters  
code  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
ModeOfTransport Class  
Lsa.Vmfg.Shared Namespace  
  
ModeOfTransport.Find  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 471 ModeOfTransport.Find Method  
Retrieve a specific Mode Of Transport Code.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Find (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
ModeOfTransport Class  
Lsa.Vmfg.Shared Namespace  
  
ModeOfTransport.Load Method  
472 | Infor VISUAL API Toolkit  Shared Class Library Reference  ModeOfTransport.Load Method  
Overload List 
 Name  Description  
 Load()  Loads all of the Mode Of Transport Codes from the database.  
 Load(String)  Loads a specific Mode Of Transport Code from the database.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
ModeOfTransport Class  
Lsa.Vmfg.Shared Namespace  
  

ModeOfTransport.Load Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 473 ModeOfTransport.Load Method  
Loads all of the Mode Of Transport Codes from the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
ModeOfTransport Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
ModeOfTransport.Load Method (String)  
474 | Infor VISUAL API Toolkit  Shared Class Library Reference  ModeOfTransport.Load Method (String)  
Loads a specific Mode Of Transport Code from the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
ModeOfTransport Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
ModeOfTransport.Load Method (Stream, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 475 ModeOfTransport.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 code As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
code  
Type: System.String  
See Also 
ModeOfTransport Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
ModeOfTransport.NewModeOfTransportRow  Method  
476 | Infor VISUAL API Toolkit  Shared Class Library Reference  ModeOfTransport.NewModeOfTransportRow 
Method  
Inserts a new row into the MODE_OF_TRANSPORT table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewModeOfTransportRow ( 
 string  code 
) 
 
VB 
Public  Function  NewModeOfTransportRow  (  
 code As String  
) As DataRow  
 
Parameters  
code  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
ModeOfTransport Class  
Lsa.Vmfg.Shared Namespace  
  
ModeOfTransport.Save  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 477 ModeOfTransport.Save Method  
Overload List 
 Name  Description  
 Save()  Save previously loaded Mode Of Transport Code(s) data to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
See Also 
ModeOfTransport Class  
Lsa.Vmfg.Shared Namespace  
  

ModeOfTransport.Save  Method  
478 | Infor VISUAL API Toolkit  Shared Class Library Reference  ModeOfTransport.Save Method  
Save previously loaded Mode Of Transport Code(s) data to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
ModeOfTransport Class  
Save Overload  
Lsa.Vmfg.Shared Namespace  
  
ModeOfTransport.Save  Method (Stream)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 479 ModeOfTransport.Save Method (Stream)  
Save current state of data set to stream.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ModeOfTransport Class  
Save Overload  
Lsa.Vmfg.Shared Namespace  
  
NmfcCode Class  
480 | Infor VISUAL API Toolkit  Shared Class Library Reference  NmfcCode Class  
Maintain NMFC Codes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.NmfcCode  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  NmfcCode  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  NmfcCode 
 Inherits  BusinessDocument  
 
The NmfcCode  type exposes the following members.  
Constructors 
 Name  Description  
 NmfcCode()  Initializes a new instance of the NmfcCode  class 
 NmfcCode(String)  Initializes a new instance of the NmfcCode  class 
 

NmfcCode Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 481 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve NMFC Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve NMFC Codes based on search criteria, limited by 
record count.  
 Exists  Determine if an NMFC Code exists.  
 Find Find a specific NMFC Code.  
 Load()  Load all NMFC Codes.  
 Load(String)  Load a specific NMFC Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewNmfcCodeRow  Adds a new row to the NMFC code table.  
 Save  Save all previously loaded NMFC Codes to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

NmfcCode Constructor  
482 | Infor VISUAL API Toolkit  Shared Class Library Reference  NmfcCode Constructor  
Overload List 
 Name  Description  
 NmfcCode()  Initializes a new instance of the NmfcCode  class 
 NmfcCode(String)  Initializes a new instance of the NmfcCode  class 
 
See Also 
NmfcCode Class  
Lsa.Vmfg.Shared Namespace  
  

NmfcCode Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 483 NmfcCode Constructor  
Initializes a new instance of the NmfcCode  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  NmfcCode () 
 
VB 
Public  Sub New 
 
See Also 
NmfcCode Class  
NmfcCode Overload  
Lsa.Vmfg.Shared Namespace  
  
NmfcCode Constructor  (String) 
484 | Infor VISUAL API Toolkit  Shared Class Library Reference  NmfcCode Constructor (String)  
Initializes a new instance of the NmfcCode  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  NmfcCode ( 
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
NmfcCode Class  
NmfcCode Overload  
Lsa.Vmfg.Shared Namespace  
  
NmfcCode.NmfcCode  Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 485 NmfcCode.NmfcCode Methods  
The NmfcCode  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve NMFC Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve NMFC Codes based on search criteria, limited by 
record count.  
 Exists  Determine if an NMFC Code exists.  
 Find Find a specific NMFC Code.  
 Load()  Load all NMFC Codes.  
 Load(String)  Load a specific NMFC Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewNmfcCodeRow  Adds a new row to the NMFC code table.  
 Save  Save all previously loaded NMFC Codes to the database.  
 
See Also 
NmfcCode Class  
Lsa.Vmfg.Shared Namespace  
  

NmfcCode.Browse  Method 
486 | Infor VISUAL API Toolkit  Shared Class Library Reference  NmfcCode.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve NMFC Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve NMFC Codes based on search criteria, limited by 
record count.  
 
See Also 
NmfcCode Class  
Lsa.Vmfg.Shared Namespace  
  

NmfcCode.Browse  Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 487 NmfcCode.Browse Method (String, String, String)  
Retrieve NMFC Codes based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
NmfcCode Class  
NmfcCode.Browse  Method (String, String, String) 
488 | Infor VISUAL API Toolkit  Shared Class Library Reference  Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
NmfcCode.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 489 NmfcCode.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve NMFC Codes based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
NmfcCode.Browse  Method (String, String, String, Int32, Int32)  
490 | Infor VISUAL API Toolkit  Shared Class Library Reference  Return Value  
Type: DataSet  
See Also 
NmfcCode Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
NmfcCode.Exists Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 491 NmfcCode.Exists Method  
Determine if an NMFC Code exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
NmfcCode Class  
Lsa.Vmfg.Shared Namespace  
  
NmfcCode.Find  Method 
492 | Infor VISUAL API Toolkit  Shared Class Library Reference  NmfcCode.Find Method  
Find a specific NMFC Code.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
NmfcCode Class  
Lsa.Vmfg.Shared Namespace  
  
NmfcCode.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 493 NmfcCode.Load Method  
Overload List 
 Name  Description  
 Load()  Load all NMFC Codes.  
 Load(String)  Load a specific NMFC Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
NmfcCode Class  
Lsa.Vmfg.Shared Namespace  
  

NmfcCode.Load  Method  
494 | Infor VISUAL API Toolkit  Shared Class Library Reference  NmfcCode.Load Method  
Load all NMFC Codes.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
NmfcCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
NmfcCode.Load  Method (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 495 NmfcCode.Load Method (String)  
Load a specific NMFC Code row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
NmfcCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
NmfcCode.Load  Method (Stream, String)  
496 | Infor VISUAL API Toolkit  Shared Class Library Reference  NmfcCode.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
NmfcCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
NmfcCode.NewNmfcCodeRow  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 497 NmfcCode.NewNmfcCodeRow Method  
Adds a new row to the NMFC code table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewNmfcCodeRow ( 
 string  id 
) 
 
VB 
Public  Function  NewNmfcCodeRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
NmfcCode Class  
Lsa.Vmfg.Shared Namespace  
  
NmfcCode.Save Method 
498 | Infor VISUAL API Toolkit  Shared Class Library Reference  NmfcCode.Save Method  
Save all previously loaded NMFC Codes to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
NmfcCode Class  
Lsa.Vmfg.Shared Namespace  
  
Notation Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 499 Notation Class  
Maintain Notations.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Notation  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  Notation : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  Notation  
 Inherits  BusinessDocument  
 
The Notation  type exposes the following members.  
Constructors 
 Name  Description  
 Notation()  Initializes a new instance of the Notation  class 
 Notation(String)  Initializes a new instance of the Notation  class 
 

Notation Class  
500 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Notations based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Notations based on search criteria, limited by 
record count.  
 Exists  Determine if Notations exist for a given type and owner.  
 Find Find all Notations for a given type and owner.  
 Load  Load all Notations for a given type and owner.  
 NewNotationRow  Add a new row to the NOTATION table.  
 Save  Save all previously loaded Notations to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Notation Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 501 Notation Constructor  
Overload List 
 Name  Description  
 Notation()  Initializes a new instance of the Notation  class 
 Notation(String)  Initializes a new instance of the Notation  class 
 
See Also 
Notation Class  
Lsa.Vmfg.Shared Namespace  
  

Notation Constructor  
502 | Infor VISUAL API Toolkit  Shared Class Library Reference  Notation Constructor  
Initializes a new instance of the Notation  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Notation()  
 
VB 
Public  Sub New 
 
See Also 
Notation Class  
Notation Overload  
Lsa.Vmfg.Shared Namespace  
  
Notation Constructor  (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 503 Notation Constructor (String)  
Initializes a new instance of the Notation  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Notation( 
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
Notation Class  
Notation Overload  
Lsa.Vmfg.Shared Namespace  
  
Notation.Notation Methods  
504 | Infor VISUAL API Toolkit  Shared Class Library Reference  Notation.Notation Methods  
The Notation  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Notations based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Notations based on search criteria, limited by 
record count.  
 Exists  Determine if Notations exist for a given type and owner.  
 Find Find all Notations for a given type and owner.  
 Load  Load all Notations for a given type and owner.  
 NewNotationRow  Add a new row to the NOTATION table.  
 Save  Save all previously loaded Notations to the database.  
 
See Also 
Notation Class  
Lsa.Vmfg.Shared Namespace  
  

Notation.Browse  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 505 Notation.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Notations based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Notations based on search criteria, limited by 
record count.  
 
See Also 
Notation Class  
Lsa.Vmfg.Shared Namespace  
  

Notation.Browse  Method (String, String, String) 
506 | Infor VISUAL API Toolkit  Shared Class Library Reference  Notation.Browse Method (String, String, String)  
Retrieve Notations based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Notation Class  
Notation.Browse  Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 507 Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Notation.Browse  Method (String, String, String, Int32, Int32)  
508 | Infor VISUAL API Toolkit  Shared Class Library Reference  Notation.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve Notations based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Notation.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 509 Return Value  
Type: DataSet  
See Also 
Notation Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Notation.Exists  Method  
510 | Infor VISUAL API Toolkit  Shared Class Library Reference  Notation.Exists Method  
Determine if Notations exist for a given type and owner.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  type, 
 string  ownerID  
) 
 
VB 
Public  Overridable  Function Exists  (  
 type As String , 
 ownerID  As String 
) As Boolean 
 
Parameters  
type 
Type: System.String  
ownerID  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
Notation Class  
Lsa.Vmfg.Shared Namespace  
  
Notation.Find  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 511 Notation.Find Method  
Find all Notations for a given type and owner.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  type, 
 string  ownerID  
) 
 
VB 
Public  Overridable  Sub Find (  
 type As String , 
 ownerID  As String 
) 
 
Parameters  
type 
Type: System.String  
ownerID  
Type: System.String  
See Also 
Notation Class  
Lsa.Vmfg.Shared Namespace  
  
Notation.Load Method  
512 | Infor VISUAL API Toolkit  Shared Class Library Reference  Notation.Load Method  
Load all Notations for a given type and owner.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  type, 
 string  ownerID  
) 
 
VB 
Public  Overridable  Sub Load (  
 type As String , 
 ownerID  As String 
) 
 
Parameters  
type 
Type: System.String  
ownerID  
Type: System.String  
See Also 
Notation Class  
Lsa.Vmfg.Shared Namespace  
  
Notation.NewNotationRow  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 513 Notation.NewNotationRow Method  
Add a new row to the NOTATION table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewNotationRow ( 
 string  type, 
 string  ownerID  
) 
 
VB 
Public  Overridable  Function NewNotationRow  (  
 type As String , 
 ownerID  As String 
) As DataRow  
 
Parameters  
type 
Type: System.String  
ownerID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Notation Class  
Lsa.Vmfg.Shared Namespace  
  
Notation.Save Method  
514 | Infor VISUAL API Toolkit  Shared Class Library Reference  Notation.Save Method  
Save all previously loaded Notations to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Notation Class  
Lsa.Vmfg.Shared Namespace  
  
PackageType Class 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 515 PackageType Class  
Maintain Package Types.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.PackageType  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  PackageType : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  PackageType  
 Inherits  BusinessDocument  
 
The PackageType type exposes the following members.  
Constructors 
 Name  Description  
 PackageType()  Initializes a new instance of the PackageType  class  
 PackageType(String)  Initializes a new instance of the PackageType  class  
 

PackageType Class 
516 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Package Types based on serach criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Package Types based on serach criteria, limited 
by record count.  
 Exists  Determine if a Package Type exists.  
 Find Find a specific Package Type.  
 Load()  Load all Package Types.  
 Load(String)  Load a specific Package Type row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewPackageTypeRow  Adds a new row to the PACKAGE_TYPE table.  
 Save  Save all previously loaded Package Type(s) to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

PackageType Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 517 PackageType Constructor  
Overload List 
 Name  Description  
 PackageType()  Initializes a new instance of the PackageType  class 
 PackageType(String)  Initializes a new instance of the PackageType  class 
 
See Also 
PackageType Class  
Lsa.Vmfg.Shared Namespace  
  

PackageType Constructor  
518 | Infor VISUAL API Toolkit  Shared Class Library Reference  PackageType Constructor  
Initializes a new instance of the PackageType  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  PackageType()  
 
VB 
Public  Sub New 
 
See Also 
PackageType Class  
PackageType Overload  
Lsa.Vmfg.Shared Namespace  
  
PackageType Constructor  (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 519 PackageType Constructor (String)  
Initializes a new instance of the PackageType  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  PackageType( 
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
PackageType Class  
PackageType Overload  
Lsa.Vmfg.Shared Namespace  
  
PackageType.PackageType  Methods  
520 | Infor VISUAL API Toolkit  Shared Class Library Reference  PackageType.PackageType Methods  
The PackageType  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Package Types based on serach criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Package Types based on serach criteria, limited 
by record count.  
 Exists  Determine if a Package Type exists.  
 Find Find a specific Package Type.  
 Load()  Load all Package Types.  
 Load(String)  Load a specific Package Type row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewPackageTypeRow  Adds a new row to the PACKAGE_TYPE table.  
 Save  Save all previously loaded Package Type(s) to the database.  
 
See Also 
PackageType Class  
Lsa.Vmfg.Shared Namespace  
  

PackageType.Browse Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 521 PackageType.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Package Types based on serach criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Package Types based on serach criteria, limited 
by record count.  
 
See Also 
PackageType Class  
Lsa.Vmfg.Shared Namespace  
  

PackageType.Browse Method (String, String, String)  
522 | Infor VISUAL API Toolkit  Shared Class Library Reference  PackageType.Browse Method (String, String, String)  
Retrieve Package Types based on serach criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
PackageType Class  
PackageType.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 523 Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
PackageType.Browse Method (String, String, String, Int32, Int32)  
524 | Infor VISUAL API Toolkit  Shared Class Library Reference  PackageType.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve Package Types based on serach criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
PackageType.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 525 Return Value  
Type: DataSet  
See Also 
PackageType Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
PackageType.Exists Method  
526 | Infor VISUAL API Toolkit  Shared Class Library Reference  PackageType.Exists Method  
Determine if a Package Type exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  code 
) 
 
VB 
Public  Overridable  Function Exists  (  
 code As String  
) As Boolean 
 
Parameters  
code  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
PackageType Class  
Lsa.Vmfg.Shared Namespace  
  
PackageType.Find  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 527 PackageType.Find Method  
Find a specific Package Type.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Find (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
PackageType Class  
Lsa.Vmfg.Shared Namespace  
  
PackageType.Load  Method  
528 | Infor VISUAL API Toolkit  Shared Class Library Reference  PackageType.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Package Types.  
 Load(String)  Load a specific Package Type row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
PackageType Class  
Lsa.Vmfg.Shared Namespace  
  

PackageType.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 529 PackageType.Load Method  
Load all Package Types.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
PackageType Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
PackageType.Load  Method (String) 
530 | Infor VISUAL API Toolkit  Shared Class Library Reference  PackageType.Load Method (String)  
Load a specific Package Type row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
PackageType Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
PackageType.Load  Method (Stream, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 531 PackageType.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 code As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
code  
Type: System.String  
See Also 
PackageType Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
PackageType.NewPackageTypeRow  Method 
532 | Infor VISUAL API Toolkit  Shared Class Library Reference  PackageType.NewPackageTypeRow Method  
Adds a new row to the PACKAGE_TYPE table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewPackageTypeRow ( 
 string  code 
) 
 
VB 
Public  Function  NewPackageTypeRow  (  
 code As String  
) As DataRow  
 
Parameters  
code  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
PackageType Class  
Lsa.Vmfg.Shared Namespace  
  
PackageType.Save Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 533 PackageType.Save Method  
Save all previously loaded Package Type(s) to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
PackageType Class  
Lsa.Vmfg.Shared Namespace  
  
ProductCode Class  
534 | Infor VISUAL API Toolkit  Shared Class Library Reference  ProductCode Class  
Maintain Product Codes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.ProductCode  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  ProductCode : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  ProductCode  
 Inherits  BusinessDocument  
 
The ProductCode  type exposes the following members.  
Constructors 
 Name  Description  
 ProductCode()  Initializes a new instance of the ProductCode  class 
 ProductCode(String)  Initializes a new instance of the ProductCode  class 
 

ProductCode Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 535 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Product Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Product Codes based on search criteria, limited 
by record count.  
 Exists  Determine if a Product Code exists.  
 Find Find a specific Product Code.  
 Load()  Load all Product Codes.  
 Load(String)  Load a specific Product Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewProductRow  Adds a new row to the PRODUCT table.  
 Save  Save all previously loaded Product Code(s) to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

ProductCode Constructor  
536 | Infor VISUAL API Toolkit  Shared Class Library Reference  ProductCode Constructor  
Overload List 
 Name  Description  
 ProductCode()  Initializes a new instance of the ProductCode  class 
 ProductCode(String)  Initializes a new instance of the ProductCode  class 
 
See Also 
ProductCode Class  
Lsa.Vmfg.Shared Namespace  
  

ProductCode Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 537 ProductCode Constructor  
Initializes a new instance of the ProductCode  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  ProductCode()  
 
VB 
Public  Sub New 
 
See Also 
ProductCode Class  
ProductCode Overload  
Lsa.Vmfg.Shared Namespace  
  
ProductCode Constructor  (String) 
538 | Infor VISUAL API Toolkit  Shared Class Library Reference  ProductCode Constructor (String)  
Initializes a new instance of the ProductCode  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  ProductCode( 
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
ProductCode Class  
ProductCode Overload  
Lsa.Vmfg.Shared Namespace  
  
ProductCode.ProductCode  Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 539 ProductCode.ProductCode Methods  
The ProductCode  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Product Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Product Codes based on search criteria, limited 
by record count.  
 Exists  Determine if a Product Code exists.  
 Find Find a specific Product Code.  
 Load()  Load all Product Codes.  
 Load(String)  Load a specific Product Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewProductRow  Adds a new row to the PRODUCT table.  
 Save  Save all previously loaded Product Code(s) to the database.  
 
See Also 
ProductCode Class  
Lsa.Vmfg.Shared Namespace  
  

ProductCode.Browse  Method  
540 | Infor VISUAL API Toolkit  Shared Class Library Reference  ProductCode.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Product Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Product Codes based on search criteria, limited by 
record count.  
 
See Also 
ProductCode Class  
Lsa.Vmfg.Shared Namespace  
  

ProductCode.Browse  Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 541 ProductCode.Browse Method (String, String, String)  
Retrieve Product Codes based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ProductCode Class  
ProductCode.Browse  Method (String, String, String) 
542 | Infor VISUAL API Toolkit  Shared Class Library Reference  Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
ProductCode.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 543 ProductCode.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve Product Codes based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ProductCode.Browse  Method (String, String, String, Int32, Int32)  
544 | Infor VISUAL API Toolkit  Shared Class Library Reference  Return Value  
Type: DataSet  
See Also 
ProductCode Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
ProductCode.Exists  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 545 ProductCode.Exists Method  
Determine if a Product Code exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  code 
) 
 
VB 
Public  Overridable  Function Exists  (  
 code As String  
) As Boolean 
 
Parameters  
code  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
ProductCode Class  
Lsa.Vmfg.Shared Namespace  
  
ProductCode.Find  Method 
546 | Infor VISUAL API Toolkit  Shared Class Library Reference  ProductCode.Find Method  
Find a specific Product Code.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Find (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
ProductCode Class  
Lsa.Vmfg.Shared Namespace  
  
ProductCode.Load Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 547 ProductCode.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Product Codes.  
 Load(String)  Load a specific Product Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
ProductCode Class  
Lsa.Vmfg.Shared Namespace  
  

ProductCode.Load Method  
548 | Infor VISUAL API Toolkit  Shared Class Library Reference  ProductCode.Load Method  
Load all Product Codes.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
ProductCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
ProductCode.Load Method (String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 549 ProductCode.Load Method (String)  
Load a specific Product Code row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
ProductCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
ProductCode.Load Method (Stream, String)  
550 | Infor VISUAL API Toolkit  Shared Class Library Reference  ProductCode.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 code As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
code  
Type: System.String  
See Also 
ProductCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
ProductCode.NewProductRow  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 551 ProductCode.NewProductRow Method  
Adds a new row to the PRODUCT table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewProductRow ( 
 string  code 
) 
 
VB 
Public  Function  NewProductRow  (  
 code As String  
) As DataRow  
 
Parameters  
code  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
ProductCode Class  
Lsa.Vmfg.Shared Namespace  
  
ProductCode.Save  Method 
552 | Infor VISUAL API Toolkit  Shared Class Library Reference  ProductCode.Save Method  
Save all previously loaded Product Code(s) to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
ProductCode Class  
Lsa.Vmfg.Shared Namespace  
  
ProjectInfo Class 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 553 ProjectInfo Class  
Maintain Project Info.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.ProjectInfo  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  ProjectInfo  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  ProjectInfo  
 Inherits  BusinessDocument  
 
The ProjectInfo  type exposes the following members.  
Constructors 
 Name  Description  
 ProjectInfo()  Constructor  
 ProjectInfo(String)  Constructor  
 

ProjectInfo Class 
554 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, 
String)  Retrieve Project Info information based on search criteria.  
 Browse(String, String, 
String, Int32, Int32)  Retrieve ProjectInfo information base on search criteria. This 
retrieves a limited range of data result set based on the values in 
the startRecord and maxRecords parameters.  
 Exists  Verify the presence of a specific ProjectInfo id in the database.  
 Find Retrieve a specific ProjectInfo table row.  
 Load()  Loads all Project Info records from the database.  
 Load(String)  Loads a specific ProjectInfo from the database.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewProjectInfoRow  Inserts a row into the PROJECT_INFO table.  
 Save()  Save previously loaded ProjectInfo data to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

ProjectInfo Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 555 ProjectInfo Constructor  
Overload List 
 Name  Description  
 ProjectInfo()  Constructor  
 ProjectInfo(String)  Constructor  
 
See Also 
ProjectInfo Class  
Lsa.Vmfg.Shared Namespace  
  

ProjectInfo Constructor  
556 | Infor VISUAL API Toolkit  Shared Class Library Reference  ProjectInfo Constructor  
Constructor  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  ProjectInfo () 
 
VB 
Public  Sub New 
 
See Also 
ProjectInfo Class  
ProjectInfo Overload  
Lsa.Vmfg.Shared Namespace  
  
ProjectInfo Constructor  (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 557 ProjectInfo Constructor (String)  
Constructor  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  ProjectInfo ( 
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
ProjectInfo Class  
ProjectInfo Overload  
Lsa.Vmfg.Shared Namespace  
  
ProjectInfo.ProjectInfo  Methods  
558 | Infor VISUAL API Toolkit  Shared Class Library Reference  ProjectInfo.ProjectInfo Methods  
The ProjectInfo  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, 
String)  Retrieve Project Info information based on search criteria.  
 Browse(String, String, 
String, Int32, Int32)  Retrieve ProjectInfo information base on search criteria. This 
retrieves a limited range of data result set based on the values in the startRecord and maxRecords parameters.  
 Exists  Verify the presence of a specific ProjectInfo id in the database.  
 Find Retrieve a specific ProjectInfo table row.  
 Load()  Loads all Project Info records from the database.  
 Load(String)  Loads a specific ProjectInfo from the database.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewProjectInfoRow  Inserts a row into the PROJECT_INFO table.  
 Save()  Save previously loaded ProjectInfo data to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
See Also 
ProjectInfo Class  
Lsa.Vmfg.Shared Namespace  
  

ProjectInfo.Browse Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 559 ProjectInfo.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, 
String)  Retrieve Project Info information based on search criteria.  
 Browse(String, String, 
String, Int32, Int32)  Retrieve ProjectInfo information base on search criteria. This retrieves 
a limited range of data result set based on the values in the startRecord and maxRecords parameters.  
 
See Also 
ProjectInfo Class  
Lsa.Vmfg.Shared Namespace  
  

ProjectInfo.Browse Method (String, String, String) 
560 | Infor VISUAL API Toolkit  Shared Class Library Reference  ProjectInfo.Browse Method (String, String, String)  
Retrieve Project Info information based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ProjectInfo Class  
ProjectInfo.Browse Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 561 Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
ProjectInfo.Browse Method (String, String, String, Int32, Int32)  
562 | Infor VISUAL API Toolkit  Shared Class Library Reference  ProjectInfo.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve ProjectInfo information base on search criteria. This retrieves a limited range of data result 
set based on the values in the startRecord and maxRecords parameters.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ProjectInfo.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 563 Type: System.Int32  
Return Value  
Type: DataSet  
See Also 
ProjectInfo Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
ProjectInfo.Exists Method  
564 | Infor VISUAL API Toolkit  Shared Class Library Reference  ProjectInfo.Exists Method  
Verify the presence of a specific ProjectInfo id in the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ProjectInfo Class  
Lsa.Vmfg.Shared Namespace  
  
ProjectInfo.Find Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 565 ProjectInfo.Find Method  
Retrieve a specific ProjectInfo table row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ProjectInfo Class  
Lsa.Vmfg.Shared Namespace  
  
ProjectInfo.Load Method  
566 | Infor VISUAL API Toolkit  Shared Class Library Reference  ProjectInfo.Load Method  
Overload List 
 Name  Description  
 Load()  Loads all Project Info records from the database.  
 Load(String)  Loads a specific ProjectInfo from the database.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
ProjectInfo Class  
Lsa.Vmfg.Shared Namespace  
  

ProjectInfo.Load Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 567 ProjectInfo.Load Method  
Loads all Project Info records from the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
ProjectInfo Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
ProjectInfo.Load Method (String)  
568 | Infor VISUAL API Toolkit  Shared Class Library Reference  ProjectInfo.Load Method (String)  
Loads a specific ProjectInfo from the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ProjectInfo Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
ProjectInfo.Load Method (Stream, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 569 ProjectInfo.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ProjectInfo Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
ProjectInfo.NewProjectInfoRow  Method  
570 | Infor VISUAL API Toolkit  Shared Class Library Reference  ProjectInfo.NewProjectInfoRow Method  
Inserts a row into the PROJECT_INFO table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewProjectInfoRow ( 
 string  id 
) 
 
VB 
Public  Function  NewProjectInfoRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
ProjectInfo Class  
Lsa.Vmfg.Shared Namespace  
  
ProjectInfo.Save Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 571 ProjectInfo.Save Method  
Overload List 
 Name  Description  
 Save()  Save previously loaded ProjectInfo data to the database.  
 Save(Stream)  Save current state of data set to stream.  
 
See Also 
ProjectInfo Class  
Lsa.Vmfg.Shared Namespace  
  

ProjectInfo.Save Method  
572 | Infor VISUAL API Toolkit  Shared Class Library Reference  ProjectInfo.Save Method  
Save previously loaded ProjectInfo data to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
ProjectInfo Class  
Save Overload  
Lsa.Vmfg.Shared Namespace  
  
ProjectInfo.Save Method (Stream)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 573 ProjectInfo.Save Method (Stream)  
Save current state of data set to stream.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ProjectInfo Class  
Save Overload  
Lsa.Vmfg.Shared Namespace  
  
QuotePartUnitPrice Class  
574 | Infor VISUAL API Toolkit  Shared Class Library Reference  QuotePartUnitPrice Class  
Service to obtain a part's unit price based on existing price breaks.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessService  
          Lsa.Vmfg.Shared.QuotePartUnitPrice  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  QuotePartUnitPrice  : BusinessService 
 
VB 
<SerializableAttribute> 
Public  Class  QuotePartUnitPrice  
 Inherits  BusinessService 
 
The QuotePartUnitPrice  type exposes the following members.  
Constructors 
 Name  Description  
 QuotePartUnitPrice()  Initializes a new instance of the QuotePartUnitPrice  class 
 QuotePartUnitPrice(String)  Initializes a new instance of the QuotePartUnitPrice  class 
 

QuotePartUnitPrice Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 575 Methods 
 Name  Description  
 Execute  Execute the Service.  
 NewInputRow  Adds a new Service request row.  
 Prepare  Prepare the Service for execution.  
Data Tables 
 Table Type  Table Name  
 Header Table  QT_PART_UNIT_PRICE  
 Results Sub- table QT_PART_UNIT_PRICE_RESULT  
See Also 
Lsa.Vmfg.Shared Namespace  
  

QuotePartUnitPrice Constructor  
576 | Infor VISUAL API Toolkit  Shared Class Library Reference  QuotePartUnitPrice Constructor  
Overload List 
 Name  Description  
 QuotePartUnitPrice()  Initializes a new instance of the QuotePartUnitPrice  class 
 QuotePartUnitPrice(String)  Initializes a new instance of the QuotePartUnitPrice  class 
 
See Also 
QuotePartUnitPrice Class  
Lsa.Vmfg.Shared Namespace  
  

QuotePartUnitPrice Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 577 QuotePartUnitPrice Constructor  
Initializes a new instance of the QuotePartUnitPrice  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  QuotePartUnitPrice () 
 
VB 
Public  Sub New 
 
See Also 
QuotePartUnitPrice Class  
QuotePartUnitPrice Overload  
Lsa.Vmfg.Shared Namespace  
  
QuotePartUnitPrice Constructor  (String)  
578 | Infor VISUAL API Toolkit  Shared Class Library Reference  QuotePartUnitPrice Constructor (String)  
Initializes a new instance of the QuotePartUnitPrice  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  QuotePartUnitPrice ( 
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
QuotePartUnitPrice Class  
QuotePartUnitPrice Overload  
Lsa.Vmfg.Shared Namespace  
  
QuotePartUnitPrice.QuotePartUnitPrice Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 579 QuotePartUnitPrice.QuotePartUnitPrice Methods  
The QuotePartUnitPrice  type exposes the following members.  
Methods 
 Name  Description  
 Execute  Execute the Service.  
 NewInputRow  Adds a new Service request row.  
 Prepare  Prepare the Service for execution.  
 
See Also 
QuotePartUnitPrice Class  
Lsa.Vmfg.Shared Namespace  
  

QuotePartUnitPrice.Execute Method 
580 | Infor VISUAL API Toolkit  Shared Class Library Reference  QuotePartUnitPrice.Execute Method  
Execute the Service.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Execute () 
 
VB 
Public  Overridable  Sub Execute  
 
See Also 
QuotePartUnitPrice Class  
Lsa.Vmfg.Shared Namespace  
  
QuotePartUnitPrice.NewInputRow  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 581 QuotePartUnitPrice.NewInputRow Method  
Adds a new Service request row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
QuotePartUnitPrice Class  
Lsa.Vmfg.Shared Namespace  
  
QuotePartUnitPrice.Prepare  Method 
582 | Infor VISUAL API Toolkit  Shared Class Library Reference  QuotePartUnitPrice.Prepare Method  
Prepare the Service for execution.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Prepare()  
 
VB 
Public  Overridable  Sub Prepare  
 
See Also 
QuotePartUnitPrice Class  
Lsa.Vmfg.Shared Namespace  
  
QuotePartUnitPrice Data Tables  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 583 QuotePartUnitPrice Data Table s 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
DataSet Name returned from Prepare: QT_PART_UNIT_PRICE 
Header Table  
Table Name:  QT_PART_UNIT_PRICE  
Primary Key:  ENTRY_NO  
Column Name  Type  Description 
ENTRY_NO  Integer  Unique integer value for this row. Required.  
CUSTOMER_ID  String  Customer ID, used to look for customer -
specific price breaks.  
PART_ID  String  Part ID. Required.  
SELLING_UM  String  Selling Unit of Measure. Defaults to Part 
U/M if not provided.  
DISCOUNT_CODE  String  Discount Code, used to look for discount -
specific price breaks.  
QTY Decimal  The quantity for which the price is to be 
looked up.  
Results Sub-table 
Table Name : QT_PART_UNIT_PRICE_RESULT  
Column Name  Type  Description 
ENTRY_NO  Integer  Same value as the header table.  
PART_ID  String  Same value as the header table.  
UNIT_PRICE  Decimal  Unit Price. Results of the look up.  
QTY Decimal  Same value as the header table.  
QuotePartUnitPrice Data Tables  
584 | Infor VISUAL API Toolkit  Shared Class Library Reference  See Also 
QuotePartUnitPrice Class  
Lsa.Vmfg.Shared Namespace  
  
SalesTax Class 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 585 SalesTax Class  
Maintain Sales Taxes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.SalesTax  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  SalesTax  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  SalesTax 
 Inherits  BusinessDocument  
 
The SalesTax type exposes the following members.  
Constructors 
 Name  Description  
 SalesTax()  Initializes a new instance of the SalesTax class 
 SalesTax(String)  Initializes a new instance of the SalesTax class 
 

SalesTax Class 
586 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Sales Taxes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Sales Taxes based on search criteria, limited by 
record count.  
 Exists  Determine if a Sales Tax exists.  
 Find Find a specific Sales Tax.  
 Load()  Load all Sales Taxes.  
 Load(String)  Load a specific Sales Tax row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewSalesTaxPctRow  Adds a new row to the SALES_TAX_PCT table.  
 NewSalesTaxRow  Adds a new row to the SALES_TAX table.  
 Save  Save all previously loaded Sales Taxes to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

SalesTax Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 587 SalesTax Constructor  
Overload List 
 Name  Description  
 SalesTax()  Initializes a new instance of the SalesTax  class 
 SalesTax(String)  Initializes a new instance of the SalesTax  class 
 
See Also 
SalesTax Class  
Lsa.Vmfg.Shared Namespace  
  

SalesTax Constructor  
588 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTax Constructor  
Initializes a new instance of the SalesTax  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  SalesTax () 
 
VB 
Public  Sub New 
 
See Also 
SalesTax Class  
SalesTax Overload  
Lsa.Vmfg.Shared Namespace  
  
SalesTax Constructor  (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 589 SalesTax Constructor (String)  
Initializes a new instance of the SalesTax  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  SalesTax ( 
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
SalesTax Class  
SalesTax Overload  
Lsa.Vmfg.Shared Namespace  
  
SalesTax.SalesTax  Methods  
590 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTax.SalesTax Methods  
The SalesTax  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Sales Taxes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Sales Taxes based on search criteria, limited by 
record count.  
 Exists  Determine if a Sales Tax exists.  
 Find Find a specific Sales Tax.  
 Load()  Load all Sales Taxes.  
 Load(String)  Load a specific Sales Tax row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewSalesTaxPctRow  Adds a new row to the SALES_TAX_PCT table.  
 NewSalesTaxRow  Adds a new row to the SALES_TAX table.  
 Save  Save all previously loaded Sales Taxes to the database.  
 
See Also 
SalesTax Class  
Lsa.Vmfg.Shared Namespace  
  

SalesTax.Browse Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 591 SalesTax.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Sales Taxes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Sales Taxes based on search criteria, limited by 
record count.  
 
See Also 
SalesTax Class  
Lsa.Vmfg.Shared Namespace  
  

SalesTax.Browse Method (String, String, String)  
592 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTax.Browse Method (String, String, String)  
Retrieve Sales Taxes based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
SalesTax Class  
SalesTax.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 593 Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
SalesTax.Browse Method (String, String, String, Int32, Int32)  
594 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTax.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve Sales Taxes based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
SalesTax.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 595 Return Value  
Type: DataSet  
See Also 
SalesTax Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
SalesTax.Exists Method  
596 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTax.Exists Method  
Determine if a Sales Tax exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
SalesTax Class  
Lsa.Vmfg.Shared Namespace  
  
SalesTax.Find  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 597 SalesTax.Find Method  
Find a specific Sales Tax.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
SalesTax Class  
Lsa.Vmfg.Shared Namespace  
  
SalesTax.Load  Method  
598 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTax.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Sales Taxes.  
 Load(String)  Load a specific Sales Tax row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
SalesTax Class  
Lsa.Vmfg.Shared Namespace  
  

SalesTax.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 599 SalesTax.Load Method  
Load all Sales Taxes.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
SalesTax Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
SalesTax.Load  Method (String)  
600 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTax.Load Method (String)  
Load a specific Sales Tax row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
SalesTax Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
SalesTax.Load  Method (Stream, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 601 SalesTax.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
SalesTax Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
SalesTax.NewSalesTaxPctRow  Method  
602 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTax.NewSalesTaxPctRow Method  
Adds a new row to the SALES_TAX_PCT table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewSalesTaxPctRow ( 
 string  id, 
 DateTime  effectiveDate  
) 
 
VB 
Public  Function  NewSalesTaxPctRow  (  
 id As String , 
 effectiveDate  As DateTime  
) As DataRow  
 
Parameters  
id 
Type: System.String  
effectiveDate  
Type: System.DateTime  
Return Value  
Type: DataRow  
See Also 
SalesTax Class  
Lsa.Vmfg.Shared Namespace  
  
SalesTax.NewSalesTaxRow  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 603 SalesTax.NewSalesTaxRow Method  
Adds a new row to the SALES_TAX table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewSalesTaxRow ( 
 string  id 
) 
 
VB 
Public  Function  NewSalesTaxRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
SalesTax Class  
Lsa.Vmfg.Shared Namespace  
  
SalesTax.Save Method  
604 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTax.Save Method  
Save all previously loaded Sales Taxes to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
SalesTax Class  
Lsa.Vmfg.Shared Namespace  
  
SalesTaxGroup Class 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 605 SalesTaxGroup Class  
Maintain Sales Tax Groups.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.SalesTaxGroup  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  SalesTaxGroup  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  SalesTaxGroup 
 Inherits  BusinessDocument  
 
The SalesTaxGroup  type exposes the following members.  
Constructors 
 Name  Description  
 SalesTaxGroup()  Initializes a new instance of the SalesTaxGroup  class 
 SalesTaxGroup(String)  Initializes a new instance of the SalesTaxGroup  class 
 

SalesTaxGroup Class 
606 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Sales Tax Groups based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Sales Tax Groups based on search criteria, 
limited by record count.  
 Exists  Determine if a Sales Tax Group exists.  
 Find Find a specific Sales Tax Group.  
 Load()  Load all Sales Tax Groups.  
 Load(String)  Load a specific Sales Tax Group row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewSalesTaxGroupRow  Adds a new row to the SALES_TAX_GROUP table.  
 NewSalesTaxGroupTaxRow  Adds a new row to the SLS_TAX_GRP_TAX table.  
 Save  Save a previously loaded Sales Tax Group(s) to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

SalesTaxGroup Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 607 SalesTaxGroup Constructor  
Overload List 
 Name  Description  
 SalesTaxGroup()  Initializes a new instance of the SalesTaxGroup  class 
 SalesTaxGroup(String)  Initializes a new instance of the SalesTaxGroup  class 
 
See Also 
SalesTaxGroup Class  
Lsa.Vmfg.Shared Namespace  
  

SalesTaxGroup Constructor  
608 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTaxGroup Constructor  
Initializes a new instance of the SalesTaxGroup  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  SalesTaxGroup () 
 
VB 
Public  Sub New 
 
See Also 
SalesTaxGroup Class  
SalesTaxGroup Overload  
Lsa.Vmfg.Shared Namespace  
  
SalesTaxGroup Constructor  (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 609 SalesTaxGroup Constructor (String)  
Initializes a new instance of the SalesTaxGroup  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  SalesTaxGroup ( 
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
SalesTaxGroup Class  
SalesTaxGroup Overload  
Lsa.Vmfg.Shared Namespace  
  
SalesTaxGroup.SalesTaxGroup  Methods  
610 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTaxGroup.SalesTaxGroup Methods  
The SalesTaxGroup  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Sales Tax Groups based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Sales Tax Groups based on search criteria, 
limited by record count.  
 Exists  Determine if a Sales Tax Group exists.  
 Find Find a specific Sales Tax Group.  
 Load()  Load all Sales Tax Groups.  
 Load(String)  Load a specific Sales Tax Group row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewSalesTaxGroupRow  Adds a new row to the SALES_TAX_GROUP table.  
 NewSalesTaxGroupTaxRow  Adds a new row to the SLS_TAX_GRP_TAX table.  
 Save  Save a previously loaded Sales Tax Group(s) to the database.  
 
See Also 
SalesTaxGroup Class  
Lsa.Vmfg.Shared Namespace  
  

SalesTaxGroup.Browse Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 611 SalesTaxGroup.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Sales Tax Groups based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Sales Tax Groups based on search criteria, limited 
by record count.  
 
See Also 
SalesTaxGroup Class  
Lsa.Vmfg.Shared Namespace  
  

SalesTaxGroup.Browse Method (String, String, String)  
612 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTaxGroup.Browse Method (String, String, 
String)  
Retrieve Sales Tax Groups based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
SalesTaxGroup.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 613 See Also 
SalesTaxGroup Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
SalesTaxGroup.Browse Method (String, String, String, Int32, Int32)  
614 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTaxGroup.Browse Method (String, String, 
String, Int32, Int32)  
Retrieve Sales Tax Groups based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
SalesTaxGroup.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 615 Return Value  
Type: DataSet  
See Also 
SalesTaxGroup Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
SalesTaxGroup.Exists Method  
616 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTaxGroup.Exists Method  
Determine if a Sales Tax Group exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
SalesTaxGroup Class  
Lsa.Vmfg.Shared Namespace  
  
SalesTaxGroup.Find  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 617 SalesTaxGroup.Find Method  
Find a specific Sales Tax Group.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
SalesTaxGroup Class  
Lsa.Vmfg.Shared Namespace  
  
SalesTaxGroup.Load  Method  
618 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTaxGroup.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Sales Tax Groups.  
 Load(String)  Load a specific Sales Tax Group row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
SalesTaxGroup Class  
Lsa.Vmfg.Shared Namespace  
  

SalesTaxGroup.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 619 SalesTaxGroup.Load Method  
Load all Sales Tax Groups.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
SalesTaxGroup Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
SalesTaxGroup.Load  Method (String) 
620 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTaxGroup.Load Method (String)  
Load a specific Sales Tax Group row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
SalesTaxGroup Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
SalesTaxGroup.Load  Method (Stream, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 621 SalesTaxGroup.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
SalesTaxGroup Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
SalesTaxGroup.NewSalesTaxGroupRow  Method  
622 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTaxGroup.NewSalesTaxGroupRow Method  
Adds a new row to the SALES_TAX_GROUP table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewSalesTaxGroupRow ( 
 string  id 
) 
 
VB 
Public  Function  NewSalesTaxGroupRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
SalesTaxGroup Class  
Lsa.Vmfg.Shared Namespace  
  
SalesTaxGroup.NewSalesTaxGroupTaxRow  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 623 SalesTaxGroup.NewSalesTaxGroupTaxRow 
Method  
Adds a new row to the SLS_TAX_GRP_TAX table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewSalesTaxGroupTaxRow ( 
 string  groupID , 
 string  taxID  
) 
 
VB 
Public  Function  NewSalesTaxGroupTaxRow  (  
 groupID  As String , 
 taxID  As String  
) As DataRow  
 
Parameters  
groupID  
Type: System.String  
taxID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
SalesTaxGroup Class  
Lsa.Vmfg.Shared Namespace  
  
SalesTaxGroup.Save Method  
624 | Infor VISUAL API Toolkit  Shared Class Library Reference  SalesTaxGroup.Save Method  
Save a previously loaded Sales Tax Group(s) to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
SalesTaxGroup Class  
Lsa.Vmfg.Shared Namespace  
  
ServiceUnitCost Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 625 ServiceUnitCost Class  
Service to obtain service unit cost based on existing price breaks.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessService  
          Lsa.Vmfg.Shared.ServiceUnitCost  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  ServiceUnitCost  : BusinessService 
 
VB 
<SerializableAttribute> 
Public  Class  ServiceUnitCost  
 Inherits  BusinessService 
 
The ServiceUnitCost  type exposes the following members.  
Constructors 
 Name  Description  
 ServiceUnitCost()  Initializes a new instance of the ServiceUnitCost  class 
 ServiceUnitCost(String)  Initializes a new instance of the ServiceUnitCost  class 
 

ServiceUnitCost Class  
626 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Execute  Execute the Service.  
 NewInputRow  Adds a new Service request row.  
 Prepare  Prepare the Service for execution.  
Data Tables 
 Table Type  Table Name  
 Header Table  SERVICE_UNIT_COST  
 Results Sub- table SERVICE_UNIT_COST_RESULT  
See Also 
Lsa.Vmfg.Shared Namespace  
  

ServiceUnitCost Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 627 ServiceUnitCost Constructor  
Overload List 
 Name  Description  
 ServiceUnitCost()  Initializes a new instance of the ServiceUnitCost  class 
 ServiceUnitCost(String)  Initializes a new instance of the ServiceUnitCost  class 
 
See Also 
ServiceUnitCost Class  
Lsa.Vmfg.Shared Namespace  
  

ServiceUnitCost Constructor  
628 | Infor VISUAL API Toolkit  Shared Class Library Reference  ServiceUnitCost Constructor  
Initializes a new instance of the ServiceUnitCost  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  ServiceUnitCost () 
 
VB 
Public  Sub New 
 
See Also 
ServiceUnitCost Class  
ServiceUnitCost Overload  
Lsa.Vmfg.Shared Namespace  
  
ServiceUnitCost Constructor  (String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 629 ServiceUnitCost Constructor (String)  
Initializes a new instance of the ServiceUnitCost  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  ServiceUnitCost ( 
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
ServiceUnitCost Class  
ServiceUnitCost Overload  
Lsa.Vmfg.Shared Namespace  
  
ServiceUnitCost.ServiceUnitCost  Methods  
630 | Infor VISUAL API Toolkit  Shared Class Library Reference  ServiceUnitCost.ServiceUnitCost Methods  
The ServiceUnitCost  type exposes the following members.  
Methods 
 Name  Description  
 Execute  Execute the Service.  
 NewInputRow  Adds a new Service request row.  
 Prepare  Prepare the Service for execution.  
 
See Also 
ServiceUnitCost Class  
Lsa.Vmfg.Shared Namespace  
  

ServiceUnitCost.Execute Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 631 ServiceUnitCost.Execute Method  
Execute the Service.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Execute () 
 
VB 
Public  Overridable  Sub Execute  
 
See Also 
ServiceUnitCost Class  
Lsa.Vmfg.Shared Namespace  
  
ServiceUnitCost.NewInputRow  Method  
632 | Infor VISUAL API Toolkit  Shared Class Library Reference  ServiceUnitCost.NewInputRow Method  
Adds a new Service request row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ServiceUnitCost Class  
Lsa.Vmfg.Shared Namespace  
  
ServiceUnitCost.Prepare  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 633 ServiceUnitCost.Prepare Method  
Prepare the Service for execution.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Prepare()  
 
VB 
Public  Overridable  Sub Prepare  
 
See Also 
ServiceUnitCost Class  
Lsa.Vmfg.Shared Namespace  
  
ServiceUnitCost Data Tables  
634 | Infor VISUAL API Toolkit  Shared Class Library Reference  ServiceUnitCost  Data Tables  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
DataSet Name returned from Prepare: SERVICE_UNIT_COST  
Header Table  
Table Name:  SERVICE_UNIT_COST  
Primary Key:  ENTRY_NO  
Column Name  Type  Description 
ENTRY_NO  Integer  Unique integer value for this row.  
Required.  
TYPE String  Type. Use to look up operation specific 
price breaks.  
BASE_ID  String  Base ID. Use to look up operation specific 
price breaks.  
LOT_ID  String  Lot ID. Use to look up operation specific 
price breaks.  
SPLIT_ID  String  Split ID. Use to look up operation specific 
price breaks.  
SUB_ID  String  Sub ID. Use to look up operation specific 
price breaks.  
OPER_SEQ_NO  Decimal  Operation Sequence Number. Use to look 
up operation specific price breaks.  
VENDOR_ID   Vendor ID. Use to look  up vendor specific 
price breaks.  Required if Service ID or 
Vendor Service ID provided.  
VENDOR_SERVICE_ID   Vendor Service ID. Use to look  up vendor 
specific price breaks.   
SERVICE_ID   Service ID. Use to look  up vendor specific 
price breaks.   
USAGE_UM   Usage Unit of Measure. Used to convert 
quantity to purchase U/M, if usage and 
purchase U/M differ. Required.  
QTY  The quantity for which you want to look up 
the price.  Required.  
ServiceUnitCost Data Tables  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 635 Results Sub-table 
Table Name : SERVICE_UNIT_COST _RESULT  
Column Name  Type  Description 
ENTRY_NO  Integer  Same value as the header table.  
SERVICE_ID  String  Same value as the header table.  
UNIT_COST  Decimal  Unit Cost. Result of the look up.  
BASE_CHARGE Decimal  Base Charge. Result of the look up.  
MIN_CHARGE  Decimal  Minimum Charge. Result of the loop up.  
PURCHASE_QTY  Decimal  Purchase Quantity. Result of the look up.  
See Also 
ServiceUnitCost Class  
Lsa.Vmfg.Shared Namespace  
  
ServiceUnitofMeasureConv Class 
636 | Infor VISUAL API Toolkit  Shared Class Library Reference  ServiceUnitofMeasureConv Class  
Calculates the quantity of a service in one unit of measure given a quantity of another unit of 
measure. Similar to the Part Unit of Measure conversion.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessService  
          Lsa.Vmfg.Shared.ServiceUnitofMeasureConv  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  ServiceUnitofMeasureConv  : BusinessService 
 
VB 
<SerializableAttribute> 
Public  Class  ServiceUnitofMeasureConv  
 Inherits  BusinessService 
 
The ServiceUnitofMeasureConv type exposes the following members.  
Constructors 
 Name  Description  
 ServiceUnitofMeasureConv()  Initializes a new instance of the 
ServiceUnitofMeasureConv  class 

ServiceUnitofMeasureConv Class 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 637  ServiceUnitofMeasureConv(String)  Initializes a new instance of the 
ServiceUnitofMeasureConv  class 
 
Methods 
 Name  Description  
 Execute  Execute the Service.  
 NewInputRow  Adds a new service request row.  
 Prepare  Prepare the Service for execution.  
Data Tables 
 Table Type  Table Name  
 Header Table  SERVICE_CONVERT_UM  
 Results Sub- table CONVERT_UM_RESULT  
See Also 
Lsa.Vmfg.Shared Namespace  
  

ServiceUnitofMeasureConv Constructor  
638 | Infor VISUAL API Toolkit  Shared Class Library Reference  ServiceUnitofMeasureConv Constructor  
Overload List 
 Name  Description  
 ServiceUnitofMeasureConv()  Initializes a new instance of the ServiceUnitofMeasureConv  
class 
 ServiceUnitofMeasureConv(String)  Initializes a new instance of the ServiceUnitofMeasureConv  
class 
 
See Also 
ServiceUnitofMeasureConv Class  
Lsa.Vmfg.Shared Namespace  
  

ServiceUnitofMeasureConv Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 639 ServiceUnitofMeasureConv Constructor  
Initializes a new instance of the ServiceUnitofMeasureConv  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  ServiceUnitofMeasureConv () 
 
VB 
Public  Sub New 
 
See Also 
ServiceUnitofMeasureConv Class  
ServiceUnitofMeasureConv Overload  
Lsa.Vmfg.Shared Namespace  
  
ServiceUnitofMeasureConv Constructor  (String)  
640 | Infor VISUAL API Toolkit  Shared Class Library Reference  ServiceUnitofMeasureConv Constructor (String)  
Initializes a new instance of the ServiceUnitofMeasureConv  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  ServiceUnitofMeasureConv ( 
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
ServiceUnitofMeasureConv Class  
ServiceUnitofMeasureConv Overload  
Lsa.Vmfg.Shared Namespace  
  
ServiceUnitofMeasureConv.ServiceUnitofMeasureConv Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 641 ServiceUnitofMeasureConv.ServiceUnitofMeasureC
onv Methods  
The ServiceUnitofMeasureConv  type exposes the following members.  
Methods 
 Name  Description  
 Execute  Execute the Service.  
 NewInputRow  Adds a new service request row.  
 Prepare  Prepare the Service for execution.  
 
See Also 
ServiceUnitofMeasureConv Class  
Lsa.Vmfg.Shared Namespace  
  

ServiceUnitofMeasureConv.Execute Method  
642 | Infor VISUAL API Toolkit  Shared Class Library Reference  ServiceUnitofMeasureConv.Execute Method  
Execute the Service.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Execute () 
 
VB 
Public  Overridable  Sub Execute  
 
See Also 
ServiceUnitofMeasureConv Class  
Lsa.Vmfg.Shared Namespace  
  
ServiceUnitofMeasureConv.NewInputRow  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 643 ServiceUnitofMeasureConv.NewInputRow Method  
Adds a new service request row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ServiceUnitofMeasureConv Class  
Lsa.Vmfg.Shared Namespace  
  
ServiceUnitofMeasureConv.Prepare Method 
644 | Infor VISUAL API Toolkit  Shared Class Library Reference  ServiceUnitofMeasureConv.Prepare Method  
Prepare the Service for execution.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Prepare()  
 
VB 
Public  Overridable  Sub Prepare  
 
See Also 
ServiceUnitofMeasureConv Class  
Lsa.Vmfg.Shared Namespace  
  
ServiceUnitOfMeasureConv Data Tables  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 645 ServiceUnitOfMeasureConv Data Tables  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
DataSet Name returned from Prepare: SERVICE_CONVERT_UM  
Header Table  
Table Name:  SERVICE_CONVERT_UM  
Primary Key:  ENTRY_NO  
Column Name  Type  Description 
ENTRY_NO  Integer  Unique integer identifier for this row.  
Required.  
SERVICE_ID  String  Service ID. Required.  
FROM_UM  String  Unit of measure you are converting from.  
Required.  
FROM_QTY  Decimal  Quantity expressed in the FROM_UM. 
Required.  
TO_UM  String  Unit of measure you are converting to.  
Required.  
Results Sub-table 
Table Name : CONVERT_UM_RESULT  
Column Name  Type  Description 
ENTRY_NO  Integer  Same value as the header table.  
TO_UM  String  Same value as the header table.  
TO_QTY  Decimal  Quantity expressed in the TO_UM.  
SERVICE_ID  Decimal  Same value as the header table.  
See Also  
ServiceUnitofMeasureConv Class  
ServiceUnitOfMeasureConv Data Tables  
646 | Infor VISUAL API Toolkit  Shared Class Library Reference  Lsa.Vmfg.Shared Namespace  
  
Shift Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 647 Shift Class  
Maintain Shifts.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Shift  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  Shift : BusinessDocument 
 
VB 
<SerializableAttribute> 
Public  Class  Shift 
 Inherits  BusinessDocument  
 
The Shift  type exposes the following members.  
Constructors 
 Name  Description  
 Shift()  Initializes a new instance of the Shift  class 
 Shift(String)  Initializes a new instance of the Shift  class 
 

Shift Class  
648 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Shifts based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Shifts based on search criteria, limited by 
record count.  
 Exists  Determine if a Shift exists.  
 Find Find a specific Shift.  
 Load()  Load all Shifts.  
 Load(String)  Load a specific Shift row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewShiftDayRow  Adds a new row to the SHIFT_DAY table.  
 NewShiftRow  Adds a new row to the SHIFT table.  
 Save  Save all previously loaded Shifts to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Shift Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 649 Shift Constructor  
Overload List 
 Name  Description  
 Shift()  Initializes a new instance of the Shift  class 
 Shift(String)  Initializes a new instance of the Shift  class 
 
See Also 
Shift Class  
Lsa.Vmfg.Shared Namespace  
  

Shift Constructor  
650 | Infor VISUAL API Toolkit  Shared Class Library Reference  Shift Constructor  
Initializes a new instance of the Shift  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Shift() 
 
VB 
Public  Sub New 
 
See Also 
Shift Class  
Shift Overload  
Lsa.Vmfg.Shared Namespace  
  
Shift Constructor  (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 651 Shift Constructor (String)  
Initializes a new instance of the Shift  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Shift( 
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
Shift Class  
Shift Overload  
Lsa.Vmfg.Shared Namespace  
  
Shift.Shift Methods  
652 | Infor VISUAL API Toolkit  Shared Class Library Reference  Shift.Shift Methods  
The Shift type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Shifts based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Shifts based on search criteria, limited by 
record count.  
 Exists  Determine if a Shift exists.  
 Find Find a specific Shift.  
 Load()  Load all Shifts.  
 Load(String)  Load a specific Shift row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewShiftDayRow  Adds a new row to the SHIFT_DAY table.  
 NewShiftRow  Adds a new row to the SHIFT table.  
 Save  Save all previously loaded Shifts to the database.  
 
See Also 
Shift Class  
Lsa.Vmfg.Shared Namespace  
  

Shift.Browse  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 653 Shift.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Shifts based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Shifts based on search criteria, limited by 
record count.  
 
See Also 
Shift Class  
Lsa.Vmfg.Shared Namespace  
  

Shift.Browse  Method (String, String, String) 
654 | Infor VISUAL API Toolkit  Shared Class Library Reference  Shift.Browse Method (String, String, String)  
Retrieve Shifts based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Shift Class  
Shift.Browse  Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 655 Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Shift.Browse  Method (String, String, String, Int32, Int32)  
656 | Infor VISUAL API Toolkit  Shared Class Library Reference  Shift.Browse Method (String, String, String, Int32, 
Int32)  
Retrieve Shifts based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Shift.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 657 Return Value  
Type: DataSet  
See Also 
Shift Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Shift.Exists  Method  
658 | Infor VISUAL API Toolkit  Shared Class Library Reference  Shift.Exists Method  
Determine if a Shift exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Shift Class  
Lsa.Vmfg.Shared Namespace  
  
Shift.Find  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 659 Shift.Find Method  
Find a specific Shift.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Shift Class  
Lsa.Vmfg.Shared Namespace  
  
Shift.Load  Method  
660 | Infor VISUAL API Toolkit  Shared Class Library Reference  Shift.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Shifts.  
 Load(String)  Load a specific Shift row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
Shift Class  
Lsa.Vmfg.Shared Namespace  
  

Shift.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 661 Shift.Load Method  
Load all Shifts.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
Shift Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Shift.Load  Method (String)  
662 | Infor VISUAL API Toolkit  Shared Class Library Reference  Shift.Load Method (String)  
Load a specific Shift row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Shift Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Shift.Load  Method (Stream, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 663 Shift.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Shift Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Shift.NewShiftDayRow  Method  
664 | Infor VISUAL API Toolkit  Shared Class Library Reference  Shift.NewShiftDayRow Method  
Adds a new row to the SHIFT_DAY table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewShiftDayRow( 
 string  shiftID , 
 string  shiftCode 
) 
 
VB 
Public  Function  NewShiftDayRow  (  
 shiftID  As String , 
 shiftCode As String  
) As DataRow  
 
Parameters  
shiftID  
Type: System.String  
shiftCode  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Shift Class  
Lsa.Vmfg.Shared Namespace  
  
Shift.NewShiftRow  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 665 Shift.NewShiftRow Method  
Adds a new row to the SHIFT table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewShiftRow( 
 string  id 
) 
 
VB 
Public  Function  NewShiftRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Shift Class  
Lsa.Vmfg.Shared Namespace  
  
Shift.Save  Method  
666 | Infor VISUAL API Toolkit  Shared Class Library Reference  Shift.Save Method  
Save all previously loaded Shifts to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Shift Class  
Lsa.Vmfg.Shared Namespace  
  
ShipToAddress Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 667 ShipToAddress Class  
Maintain Ship To Addresses.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.ShipToAddress  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  ShipToAddress  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  ShipToAddress  
 Inherits  BusinessDocument  
 
The ShipToAddress  type exposes the following members.  
Constructors 
 Name  Description  
 ShipToAddress()  Initializes a new instance of the ShipToAddress  class 
 ShipToAddress(String)  Initializes a new instance of the ShipToAddress  class 
 

ShipToAddress Class  
668 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Ship To Addresses based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Ship To Addresses based on search criteria, 
limited by record count.  
 Exists  Determine if a Ship To Address exists.  
 Find Find a specific Ship To Addresses.  
 Load()  Load all Ship To Addresses.  
 Load(Int32)  Load a specific Ship To Address row.  
 Load(Stream, Int32)  Load from stream and rename using new key.  
 NewShipToAddrNoRow  Adds a new row to the ship to the SHIPTO_ADDRESS table.  
 Save  Save all previously loaded Ship To Addresses to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

ShipToAddress Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 669 ShipToAddress Constructor  
Overload List 
 Name  Description  
 ShipToAddress()  Initializes a new instance of the ShipToAddress  class  
 ShipToAddress(String)  Initializes a new instance of the ShipToAddress  class  
 
See Also 
ShipToAddress Class  
Lsa.Vmfg.Shared Namespace  
  

ShipToAddress Constructor  
670 | Infor VISUAL API Toolkit  Shared Class Library Reference  ShipToAddress Constructor  
Initializes a new instance of the ShipToAddress  class  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  ShipToAddress () 
 
VB 
Public  Sub New 
 
See Also 
ShipToAddress Class  
ShipToAddress Overload  
Lsa.Vmfg.Shared Namespace  
  
ShipToAddress Constructor  (String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 671 ShipToAddress Constructor (String) 
Initializes a new instance of the ShipToAddress  class  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  ShipToAddress ( 
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
ShipToAddress Class  
ShipToAddress Overload  
Lsa.Vmfg.Shared Namespace  
  
ShipToAddress.ShipToAddress  Methods  
672 | Infor VISUAL API Toolkit  Shared Class Library Reference  ShipToAddress.ShipToAddress Methods  
The ShipToAddress  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Ship To Addresses based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Ship To Addresses based on search criteria, 
limited by record count.  
 Exists  Determine if a Ship To Address exists.  
 Find Find a specific Ship To Addresses.  
 Load()  Load all Ship To Addresses.  
 Load(Int32)  Load a specific Ship To Address row.  
 Load(Stream, Int32)  Load from stream and rename using new key.  
 NewShipToAddrNoRow  Adds a new row to the ship to the SHIPTO_ADDRESS table.  
 Save  Save all previously loaded Ship To Addresses to the database.  
 
See Also 
ShipToAddress Class  
Lsa.Vmfg.Shared Namespace  
  

ShipToAddress.Browse Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 673 ShipToAddress.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Ship To Addresses based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Ship To Addresses based on search criteria, 
limited by record count.  
 
See Also 
ShipToAddress Class  
Lsa.Vmfg.Shared Namespace  
  

ShipToAddress.Browse Method (String, String, String)  
674 | Infor VISUAL API Toolkit  Shared Class Library Reference  ShipToAddress.Browse Method (String, String, 
String)  
Retrieve Ship To Addresses based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ShipToAddress.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 675 See Also 
ShipToAddress Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
ShipToAddress.Browse Method (String, String, String, Int32, Int32)  
676 | Infor VISUAL API Toolkit  Shared Class Library Reference  ShipToAddress.Browse Method (String, String, 
String, Int32, Int32)  
Retrieve Ship To Addresses based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ShipToAddress.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 677 Return Value  
Type: DataSet  
See Also 
ShipToAddress Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
ShipToAddress.Exists Method 
678 | Infor VISUAL API Toolkit  Shared Class Library Reference  ShipToAddress.Exists Method  
Determine if a Ship To Address exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 int addrNo  
) 
 
VB 
Public  Overridable  Function Exists  (  
 addrNo As Integer  
) As Boolean 
 
Parameters  
addrNo  
Type: System.Int32  
Return Value  
Type: Boolean  
See Also 
ShipToAddress Class  
Lsa.Vmfg.Shared Namespace  
  
ShipToAddress.Find  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 679 ShipToAddress.Find Method  
Find a specific Ship To Addresses.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 int addrNo  
) 
 
VB 
Public  Overridable  Sub Find (  
 addrNo As Integer  
) 
 
Parameters  
addrNo  
Type: System.Int32  
See Also 
ShipToAddress Class  
Lsa.Vmfg.Shared Namespace  
  
ShipToAddress.Load Method  
680 | Infor VISUAL API Toolkit  Shared Class Library Reference  ShipToAddress.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Ship To Addresses.  
 Load(Int32)  Load a specific Ship To Address row.  
 Load(Stream, Int32)  Load from stream and rename using new key.  
 
See Also 
ShipToAddress Class  
Lsa.Vmfg.Shared Namespace  
  

ShipToAddress.Load Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 681 ShipToAddress.Load Method  
Load all Ship To Addresses.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
ShipToAddress Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
ShipToAddress.Load Method (Int32) 
682 | Infor VISUAL API Toolkit  Shared Class Library Reference  ShipToAddress.Load Method (Int32)  
Load a specific Ship To Address row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 int addrNo  
) 
 
VB 
Public  Overridable  Sub Load (  
 addrNo As Integer  
) 
 
Parameters  
addrNo  
Type: System.Int32  
See Also 
ShipToAddress Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
ShipToAddress.Load Method (Stream, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 683 ShipToAddress.Load Method (Stream, Int32)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 int addrNo  
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 addrNo As Integer  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
addrNo  
Type: System.Int32  
See Also 
ShipToAddress Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
ShipToAddress.NewShipToAddrNoRow  Method  
684 | Infor VISUAL API Toolkit  Shared Class Library Reference  ShipToAddress.NewShipToAddrNoRow Method  
Adds a new row to the ship to the SHIPTO_ADDRESS table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewShipToAddrNoRow ( 
 int addrNo  
) 
 
VB 
Public  Function  NewShipToAddrNoRow  (  
 addrNo As Integer  
) As DataRow  
 
Parameters  
addrNo  
Type: System.Int32  
Return Value  
Type: DataRow  
See Also 
ShipToAddress Class  
Lsa.Vmfg.Shared Namespace  
  
ShipToAddress.Save  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 685 ShipToAddress.Save Method  
Save all previously loaded Ship To Addresses to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
ShipToAddress Class  
Lsa.Vmfg.Shared Namespace  
  
ShipVia Class  
686 | Infor VISUAL API Toolkit  Shared Class Library Reference  ShipVia Class  
Maintain Ship Via Codes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.ShipVia  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  ShipVia  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  ShipVia 
 Inherits  BusinessDocument  
 
The ShipVia  type exposes the following members.  
Constructors 
 Name  Description  
 ShipVia()  Initializes a new instance of the ShipVia  class 
 ShipVia(String)  Initializes a new instance of the ShipVia  class 
 

ShipVia Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 687 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Ship Via Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Ship Via Codes based on search criteria, limited 
by record count.  
 Exists  Determine if a Ship Via Code exists.  
 Find Find a specific Ship Via Code.  
 Load()  Load all Ship Via Codes.  
 Load(String)  Load a specific Ship Via Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewShipViaRow  Adds a new row to the SHIP_VIA table.  
 Save  Save all previously loaded Ship Via Codes to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

ShipVia Constructor  
688 | Infor VISUAL API Toolkit  Shared Class Library Reference  ShipVia Constructor  
Overload List 
 Name  Description  
 ShipVia()  Initializes a new instance of the ShipVia  class 
 ShipVia(String)  Initializes a new instance of the ShipVia  class 
 
See Also 
ShipVia Class  
Lsa.Vmfg.Shared Namespace  
  

ShipVia Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 689 ShipVia Constructor  
Initializes a new instance of the ShipVia  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  ShipVia () 
 
VB 
Public  Sub New 
 
See Also 
ShipVia Class  
ShipVia Overload  
Lsa.Vmfg.Shared Namespace  
  
ShipVia Constructor  (String) 
690 | Infor VISUAL API Toolkit  Shared Class Library Reference  ShipVia Constructor (String)  
Initializes a new instance of the ShipVia  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  ShipVia ( 
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
ShipVia Class  
ShipVia Overload  
Lsa.Vmfg.Shared Namespace  
  
ShipVia.ShipVia  Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 691 ShipVia.ShipVia Methods  
The ShipVia  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Ship Via Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Ship Via Codes based on search criteria, limited 
by record count.  
 Exists  Determine if a Ship Via Code exists.  
 Find Find a specific Ship Via Code.  
 Load()  Load all Ship Via Codes.  
 Load(String)  Load a specific Ship Via Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewShipViaRow  Adds a new row to the SHIP_VIA table.  
 Save  Save all previously loaded Ship Via Codes to the database.  
 
See Also 
ShipVia Class  
Lsa.Vmfg.Shared Namespace  
  

ShipVia.Browse  Method  
692 | Infor VISUAL API Toolkit  Shared Class Library Reference  ShipVia.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Ship Via Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Ship Via Codes based on search criteria, limited 
by record count.  
 
See Also 
ShipVia Class  
Lsa.Vmfg.Shared Namespace  
  

ShipVia.Browse  Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 693 ShipVia.Browse Method (String, String, String)  
Retrieve Ship Via Codes based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ShipVia Class  
ShipVia.Browse  Method (String, String, String) 
694 | Infor VISUAL API Toolkit  Shared Class Library Reference  Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
ShipVia.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 695 ShipVia.Browse Method (String, String, String, Int32, 
Int32)  
Retrieve Ship Via Codes based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
ShipVia.Browse  Method (String, String, String, Int32, Int32)  
696 | Infor VISUAL API Toolkit  Shared Class Library Reference  Return Value  
Type: DataSet  
See Also 
ShipVia Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
ShipVia.Exists  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 697 ShipVia.Exists Method  
Determine if a Ship Via Code exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  code 
) 
 
VB 
Public  Overridable  Function Exists  (  
 code As String  
) As Boolean 
 
Parameters  
code  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
ShipVia Class  
Lsa.Vmfg.Shared Namespace  
  
ShipVia.Find  Method 
698 | Infor VISUAL API Toolkit  Shared Class Library Reference  ShipVia.Find Method  
Find a specific Ship Via Code.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Find (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
ShipVia Class  
Lsa.Vmfg.Shared Namespace  
  
ShipVia.Load Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 699 ShipVia.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Ship Via Codes.  
 Load(String)  Load a specific Ship Via Code row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
ShipVia Class  
Lsa.Vmfg.Shared Namespace  
  

ShipVia.Load Method  
700 | Infor VISUAL API Toolkit  Shared Class Library Reference  ShipVia.Load Method  
Load all Ship Via Codes.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
ShipVia Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
ShipVia.Load Method (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 701 ShipVia.Load Method (String)  
Load a specific Ship Via Code row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
ShipVia Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
ShipVia.Load Method (Stream, String)  
702 | Infor VISUAL API Toolkit  Shared Class Library Reference  ShipVia.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 code As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
code  
Type: System.String  
See Also 
ShipVia Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
ShipVia.NewShipViaRow  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 703 ShipVia.NewShipViaRow Method  
Adds a new row to the SHIP_VIA table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewShipViaRow ( 
 string  code 
) 
 
VB 
Public  Function  NewShipViaRow  (  
 code As String  
) As DataRow  
 
Parameters  
code  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
ShipVia Class  
Lsa.Vmfg.Shared Namespace  
  
ShipVia.Save  Method 
704 | Infor VISUAL API Toolkit  Shared Class Library Reference  ShipVia.Save Method  
Save all previously loaded Ship Via Codes to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
ShipVia Class  
Lsa.Vmfg.Shared Namespace  
  
Site Class 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 705 Site Class  
Maintain Sites.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Site 
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  Site : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  Site 
 Inherits  BusinessDocument  
 
The Site type exposes the following members.  
Constructors 
 Name  Description  
 Site()  Initializes a new instance of the Site  class 
 Site(String)  Initializes a new instance of the Site  class 
 

Site Class 
706 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Sites based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Sites based on search criteria, limited by record 
count.  
 Exists  Determine if a Site exists.  
 Find Find a specific Site.  
 Load()  Load all Sites.  
 Load(String)  Load a specific Site row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewSiteRow  Adds a new row to the SITE table.  
 NewUserSiteRow  Adds a new row to the USER_SITE table.  
 Save  Save all previously loaded Sites to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Site Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 707 Site Constructor  
Overload List 
 Name  Description  
 Site()  Initializes a new instance of the Site  class 
 Site(String)  Initializes a new instance of the Site  class 
 
See Also 
Site Class  
Lsa.Vmfg.Shared Namespace  
  

Site Constructor  
708 | Infor VISUAL API Toolkit  Shared Class Library Reference  Site Constructor  
Initializes a new instance of the Site  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Site() 
 
VB 
Public  Sub New 
 
See Also 
Site Class  
Site Overload  
Lsa.Vmfg.Shared Namespace  
  
Site Constructor  (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 709 Site Constructor (String)  
Initializes a new instance of the Site  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Site( 
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
Site Class  
Site Overload  
Lsa.Vmfg.Shared Namespace  
  
Site.Site  Methods  
710 | Infor VISUAL API Toolkit  Shared Class Library Reference  Site.Site Methods  
The Site type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Sites based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Sites based on search criteria, limited by record 
count.  
 Exists  Determine if a Site exists.  
 Find Find a specific Site.  
 Load()  Load all Sites.  
 Load(String)  Load a specific Site row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewSiteRow  Adds a new row to the SITE table.  
 NewUserSiteRow  Adds a new row to the USER_SITE table.  
 Save  Save all previously loaded Sites to the database.  
 
See Also 
Site Class  
Lsa.Vmfg.Shared Namespace  
  

Site.Browse Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 711 Site.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Sites based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Sites based on search criteria, limited by record 
count.  
 
See Also 
Site Class  
Lsa.Vmfg.Shared Namespace  
  

Site.Browse Method (String, String, String)  
712 | Infor VISUAL API Toolkit  Shared Class Library Reference  Site.Browse Method (String, String, String)  
Retrieve Sites based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Site Class  
Site.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 713 Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Site.Browse Method (String, String, String, Int32, Int32)  
714 | Infor VISUAL API Toolkit  Shared Class Library Reference  Site.Browse Method (String, String, String, Int32, 
Int32)  
Retrieve Sites based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Site.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 715 Return Value  
Type: DataSet  
See Also 
Site Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Site.Exists Method  
716 | Infor VISUAL API Toolkit  Shared Class Library Reference  Site.Exists Method  
Determine if a Site exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Site Class  
Lsa.Vmfg.Shared Namespace  
  
Site.Find  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 717 Site.Find Method  
Find a specific Site.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Site Class  
Lsa.Vmfg.Shared Namespace  
  
Site.Load  Method  
718 | Infor VISUAL API Toolkit  Shared Class Library Reference  Site.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Sites.  
 Load(String)  Load a specific Site row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
Site Class  
Lsa.Vmfg.Shared Namespace  
  

Site.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 719 Site.Load Method  
Load all Sites.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
Site Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Site.Load  Method (String)  
720 | Infor VISUAL API Toolkit  Shared Class Library Reference  Site.Load Method (String)  
Load a specific Site row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Site Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Site.Load  Method (Stream, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 721 Site.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Site Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Site.NewSiteRow  Method  
722 | Infor VISUAL API Toolkit  Shared Class Library Reference  Site.NewSiteRow Method  
Adds a new row to the SITE table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewSiteRow( 
 string  id 
) 
 
VB 
Public  Function  NewSiteRow  (  
 id As String 
) As DataRow  
 
Parameters  
id 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Site Class  
Lsa.Vmfg.Shared Namespace  
  
Site.NewUserSiteRow  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 723 Site.NewUserSiteRow Method  
Adds a new row to the USER_SITE table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewUserSiteRow( 
 string  siteID , 
 string  userID  
) 
 
VB 
Public  Function  NewUserSiteRow  (  
 siteID  As String , 
 userID  As String  
) As DataRow  
 
Parameters  
siteID  
Type: System.String  
userID  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Site Class  
Lsa.Vmfg.Shared Namespace  
  
Site.Save Method  
724 | Infor VISUAL API Toolkit  Shared Class Library Reference  Site.Save Method  
Save all previously loaded Sites to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also  
Site Class  
Lsa.Vmfg.Shared Namespace  
  
StateProvince Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 725 StateProvince Class  
Maintain State / Provinces.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.StateProvince  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  StateProvince : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  StateProvince  
 Inherits  BusinessDocument  
 
The StateProvince  type exposes the following members.  
Constructors 
 Name  Description  
 StateProvince()  Initializes a new instance of the StateProvince class  
 StateProvince(String)  Initializes a new instance of the StateProvince class  
 

StateProvince Class  
726 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve State Provinces using search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve State Provinces using search criteria, limited by 
record count.  
 Exists  Determine if a State Province exists.  
 Find Find a specific State Province.  
 Load()  Load all State Provinces.  
 Load(String)  Load a specific State Province row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewStateProvinceRow  Adds a new row to the STATE_PROVINCE table.  
 Save  Save all previously loaded State Provinces to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

StateProvince Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 727 StateProvince Constructor  
Overload List 
 Name  Description  
 StateProvince()  Initializes a new instance of the StateProvince  class 
 StateProvince(String)  Initializes a new instance of the StateProvince  class 
 
See Also 
StateProvince Class  
Lsa.Vmfg.Shared Namespace  
  

StateProvince Constructor  
728 | Infor VISUAL API Toolkit  Shared Class Library Reference  StateProvince Constructor  
Initializes a new instance of the StateProvince  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  StateProvince()  
 
VB 
Public  Sub New 
 
See Also 
StateProvince Class  
StateProvince Overload  
Lsa.Vmfg.Shared Namespace  
  
StateProvince Constructor  (String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 729 StateProvince Constructor (String)  
Initializes a new instance of the StateProvince  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  StateProvince( 
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
StateProvince Class  
StateProvince Overload  
Lsa.Vmfg.Shared Namespace  
  
StateProvince.StateProvince  Methods  
730 | Infor VISUAL API Toolkit  Shared Class Library Reference  StateProvince.StateProvince Methods  
The StateProvince  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve State Provinces using search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve State Provinces using search criteria, limited by 
record count.  
 Exists  Determine if a State Province exists.  
 Find Find a specific State Province.  
 Load()  Load all State Provinces.  
 Load(String)  Load a specific State Province row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewStateProvinceRow  Adds a new row to the STATE_PROVINCE table.  
 Save  Save all previously loaded State Provinces to the database.  
 
See Also 
StateProvince Class  
Lsa.Vmfg.Shared Namespace  
  

StateProvince.Browse Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 731 StateProvince.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve State Provinces using search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve State Provinces using search criteria, limited by 
record count.  
 
See Also 
StateProvince Class  
Lsa.Vmfg.Shared Namespace  
  

StateProvince.Browse Method (String, String, String) 
732 | Infor VISUAL API Toolkit  Shared Class Library Reference  StateProvince.Browse Method (String, String, String)  
Retrieve State Provinces using search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
StateProvince Class  
StateProvince.Browse Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 733 Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
StateProvince.Browse Method (String, String, String, Int32, Int32)  
734 | Infor VISUAL API Toolkit  Shared Class Library Reference  StateProvince.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve State Provinces using search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
StateProvince.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 735 Return Value  
Type: DataSet  
See Also 
StateProvince Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
StateProvince.Exists Method 
736 | Infor VISUAL API Toolkit  Shared Class Library Reference  StateProvince.Exists Method  
Determine if a State Province exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  code 
) 
 
VB 
Public  Overridable  Function Exists  (  
 code As String  
) As Boolean 
 
Parameters  
code  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
StateProvince Class  
Lsa.Vmfg.Shared Namespace  
  
StateProvince.Find  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 737 StateProvince.Find Method  
Find a specific State Province.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Find (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
StateProvince Class  
Lsa.Vmfg.Shared Namespace  
  
StateProvince.Load  Method  
738 | Infor VISUAL API Toolkit  Shared Class Library Reference  StateProvince.Load Method  
Overload List 
 Name  Description  
 Load()  Load all State Provinces.  
 Load(String)  Load a specific State Province row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
StateProvince Class  
Lsa.Vmfg.Shared Namespace  
  

StateProvince.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 739 StateProvince.Load Method  
Load all State Provinces.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
StateProvince Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
StateProvince.Load  Method (String) 
740 | Infor VISUAL API Toolkit  Shared Class Library Reference  StateProvince.Load Method (String)  
Load a specific State Province row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
StateProvince Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
StateProvince.Load  Method (Stream, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 741 StateProvince.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 code As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
code  
Type: System.String  
See Also 
StateProvince Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
StateProvince.NewStateProvinceRow  Method 
742 | Infor VISUAL API Toolkit  Shared Class Library Reference  StateProvince.NewStateProvinceRow Method  
Adds a new row to the STATE_PROVINCE table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewStateProvinceRow ( 
 string  code 
) 
 
VB 
Public  Function  NewStateProvinceRow  (  
 code As String  
) As DataRow  
 
Parameters  
code  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
StateProvince Class  
Lsa.Vmfg.Shared Namespace  
  
StateProvince.Save  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 743 StateProvince.Save Method  
Save all previously loaded State Provinces to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
StateProvince Class  
Lsa.Vmfg.Shared Namespace  
  
StdIndustryCode Class 
744 | Infor VISUAL API Toolkit  Shared Class Library Reference  StdIndustryCode Class  
Maintain Std Industry Codes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.StdIndustryCode  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  StdIndustryCode : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  StdIndustryCode  
 Inherits  BusinessDocument  
 
The StdIndustryCode  type exposes the following members.  
Constructors 
 Name  Description  
 StdIndustryCode()  Initializes a new instance of the StdIndustryCode  class 
 StdIndustryCode(String)  Initializes a new instance of the StdIndustryCode  class 
 

StdIndustryCode Class 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 745 Methods 
 Name  Description  
 Browse(String, String, String)  Retreive Std Industry Codes based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retreive Std Industry Codes based on search criteria, 
limited by record count.  
 Exists  Determine if a Std Industry Code exists.  
 Find Find a specific Std Industry Code.  
 Load()  Load all Std Industry Codes.  
 Load(String)  Load a specific Std Industry Code.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewSICRow  Adds a new row to the SIC table.  
 Save  Save all previously loaded Std Industry Codes.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

StdIndustryCode Constructor  
746 | Infor VISUAL API Toolkit  Shared Class Library Reference  StdIndustryCode Constructor  
Overload List 
 Name  Description  
 StdIndustryCode()  Initializes a new instance of the StdIndustryCode  class 
 StdIndustryCode(String)  Initializes a new instance of the StdIndustryCode  class 
 
See Also 
StdIndustryCode Class  
Lsa.Vmfg.Shared Namespace  
  

StdIndustryCode Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 747 StdIndustryCode Constructor  
Initializes a new instance of the StdIndustryCode  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  StdIndustryCode()  
 
VB 
Public  Sub New 
 
See Also 
StdIndustryCode Class  
StdIndustryCode Overload  
Lsa.Vmfg.Shared Namespace  
  
StdIndustryCode Constructor  (String) 
748 | Infor VISUAL API Toolkit  Shared Class Library Reference  StdIndustryCode Constructor (String)  
Initializes a new instance of the StdIndustryCode  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  StdIndustryCode( 
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
StdIndustryCode Class  
StdIndustryCode Overload  
Lsa.Vmfg.Shared Namespace  
  
StdIndustryCode.StdIndustryCode  Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 749 StdIndustryCode.StdIndustryCode Methods  
The StdIndustryCode  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retreive Std Industry Codes based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retreive Std Industry Codes based on search criteria, 
limited by record count.  
 Exists  Determine if a Std Industry Code exists.  
 Find Find a specific Std Industry Code.  
 Load()  Load all Std Industry Codes.  
 Load(String)  Load a specific Std Industry Code.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewSICRow  Adds a new row to the SIC table.  
 Save  Save all previously loaded Std Industry Codes.  
 
See Also 
StdIndustryCode Class  
Lsa.Vmfg.Shared Namespace  
  

StdIndustryCode.Browse  Method  
750 | Infor VISUAL API Toolkit  Shared Class Library Reference  StdIndustryCode.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retreive Std Industry Codes based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retreive Std Industry Codes based on search criteria, 
limited by record count.  
 
See Also 
StdIndustryCode Class  
Lsa.Vmfg.Shared Namespace  
  

StdIndustryCode.Browse  Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 751 StdIndustryCode.Browse Method (String, String, 
String)  
Retreive Std Industry Codes based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
StdIndustryCode.Browse  Method (String, String, String) 
752 | Infor VISUAL API Toolkit  Shared Class Library Reference  See Also 
StdIndustryCode Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
StdIndustryCode.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 753 StdIndustryCode.Browse Method (String, String, 
String, Int32, Int32)  
Retreive Std Industry Codes based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
StdIndustryCode.Browse  Method (String, String, String, Int32, Int32)  
754 | Infor VISUAL API Toolkit  Shared Class Library Reference  Return Value  
Type: DataSet  
See Also 
StdIndustryCode Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
StdIndustryCode.Exists Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 755 StdIndustryCode.Exists Method  
Determine if a Std Industry Code exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  code 
) 
 
VB 
Public  Overridable  Function Exists  (  
 code As String  
) As Boolean 
 
Parameters  
code  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
StdIndustryCode Class  
Lsa.Vmfg.Shared Namespace  
  
StdIndustryCode.Find Method  
756 | Infor VISUAL API Toolkit  Shared Class Library Reference  StdIndustryCode.Find Method  
Find a specific Std Industry Code.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Find (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
StdIndustryCode Class  
Lsa.Vmfg.Shared Namespace  
  
StdIndustryCode.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 757 StdIndustryCode.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Std Industry Codes.  
 Load(String)  Load a specific Std Industry Code.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
StdIndustryCode Class  
Lsa.Vmfg.Shared Namespace  
  

StdIndustryCode.Load  Method  
758 | Infor VISUAL API Toolkit  Shared Class Library Reference  StdIndustryCode.Load Method  
Load all Std Industry Codes.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
StdIndustryCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
StdIndustryCode.Load  Method (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 759 StdIndustryCode.Load Method (String)  
Load a specific Std Industry Code.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
StdIndustryCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
StdIndustryCode.Load  Method (Stream, String)  
760 | Infor VISUAL API Toolkit  Shared Class Library Reference  StdIndustryCode.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 code As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
code  
Type: System.String  
See Also 
StdIndustryCode Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
StdIndustryCode.NewSICRow  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 761 StdIndustryCode.NewSICRow Method  
Adds a new row to the SIC table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewSICRow ( 
 string  code 
) 
 
VB 
Public  Function  NewSICRow  (  
 code As String  
) As DataRow  
 
Parameters  
code  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
StdIndustryCode Class  
Lsa.Vmfg.Shared Namespace  
  
StdIndustryCode.Save Method  
762 | Infor VISUAL API Toolkit  Shared Class Library Reference  StdIndustryCode.Save Method  
Save all previously loaded Std Industry Codes.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
StdIndustryCode Class  
Lsa.Vmfg.Shared Namespace  
  
Terms Class 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 763 Terms Class  
Maintain Terms.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Terms  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  class  Terms  : BusinessDocument  
 
VB 
Public  Class  Terms  
 Inherits  BusinessDocument  
 
The Terms  type exposes the following members.  
Constructors 
 Name  Description  
 Terms()  Constructor  
 Terms(String)  Constructor  
 

Terms Class 
764 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Terms based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Terms based on search criteria, limited by 
record count.  
 Exists  Determine if a Terms record exists.  
 Find Find a specific Terms record.  
 Load(String)  Load all Terms.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewTermsRow  Adds a new TERMS Row to the database  
 Save  Save all previously loaded Terms records to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Terms Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 765 Terms Constructor  
Overload List 
 Name  Description  
 Terms()  Constructor  
 Terms(String)  Constructor  
 
See Also 
Terms Class  
Lsa.Vmfg.Shared Namespace  
  

Terms Constructor  
766 | Infor VISUAL API Toolkit  Shared Class Library Reference  Terms Constructor  
Constructor  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Terms () 
 
VB 
Public  Sub New 
 
See Also 
Terms Class  
Terms Overload  
Lsa.Vmfg.Shared Namespace  
  
Terms Constructor  (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 767 Terms Constructor (String)  
Constructor  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Terms ( 
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
Terms Class  
Terms Overload  
Lsa.Vmfg.Shared Namespace  
  
Terms.Terms  Methods  
768 | Infor VISUAL API Toolkit  Shared Class Library Reference  Terms.Terms Methods  
The Terms  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Terms based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Terms based on search criteria, limited by 
record count.  
 Exists  Determine if a Terms record exists.  
 Find Find a specific Terms record.  
 Load(String)  Load all Terms.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewTermsRow  Adds a new TERMS Row to the database  
 Save  Save all previously loaded Terms records to the database.  
 
See Also 
Terms Class  
Lsa.Vmfg.Shared Namespace  
  

Terms.Browse Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 769 Terms.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Terms based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Terms based on search criteria, limited by 
record count.  
 
See Also 
Terms Class  
Lsa.Vmfg.Shared Namespace  
  

Terms.Browse Method (String, String, String) 
770 | Infor VISUAL API Toolkit  Shared Class Library Reference  Terms.Browse Method (String, String, String)  
Retrieve Terms based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Terms Class  
Terms.Browse Method (String, String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 771 Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Terms.Browse Method (String, String, String, Int32, Int32)  
772 | Infor VISUAL API Toolkit  Shared Class Library Reference  Terms.Browse Method (String, String, String, Int32, 
Int32)  
Retrieve Terms based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Terms.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 773 Return Value  
Type: DataSet  
See Also 
Terms Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Terms.Exists Method  
774 | Infor VISUAL API Toolkit  Shared Class Library Reference  Terms.Exists Method  
Determine if a Terms record exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  termsID  
) 
 
VB 
Public  Overridable  Function Exists  (  
 termsID  As String  
) As Boolean 
 
Parameters  
termsID  
Type: System.String  
Return Value  
Type: Boolean  
 
See Also 
Terms Class  
Lsa.Vmfg.Shared Namespace  
  
Terms.Find  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 775 Terms.Find Method  
Find a specific Terms record.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  termsID  
) 
 
VB 
Public  Overridable  Sub Find (  
 termsID  As String  
) 
 
Parameters  
termsID  
Type: System.String  
See Also 
Terms Class  
Lsa.Vmfg.Shared Namespace  
  
Terms.Load  Method  
776 | Infor VISUAL API Toolkit  Shared Class Library Reference  Terms.Load Method  
Overload List 
 Name  Description  
 Load(String)  Load all Terms.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
Terms Class  
Lsa.Vmfg.Shared Namespace  
  

Terms.Load  Method (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 777 Terms.Load Method (String)  
Load all Terms.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  termsID  
) 
 
VB 
Public  Overridable  Sub Load (  
 termsID  As String  
) 
 
Parameters  
termsID  
Type: System.String  
See Also 
Terms Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Terms.Load  Method (Stream, String)  
778 | Infor VISUAL API Toolkit  Shared Class Library Reference  Terms.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  termsID  
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 termsID  As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
termsID  
Type: System.String  
See Also 
Terms Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Terms.NewTermsRow  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 779 Terms.NewTermsRow Method  
Adds a new TERMS Row to the database  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewTermsRow( 
 string  termsID  
) 
 
VB 
Public  Function  NewTermsRow  (  
 termsID  As String  
) As DataRow  
 
Parameters  
termsID  
Type: System.String  
Return Value  
Type: DataRow  
 
See Also 
Terms Class  
Lsa.Vmfg.Shared Namespace  
  
Terms.Save Method  
780 | Infor VISUAL API Toolkit  Shared Class Library Reference  Terms.Save Method  
Save all previously loaded Terms records to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Terms Class  
Lsa.Vmfg.Shared Namespace  
  
Territory Class 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 781 Territory Class  
Maintain Territories.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Territory  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  Territory  : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  Territory  
 Inherits  BusinessDocument  
 
The Territory  type exposes the following members.  
Constructors 
 Name  Description  
 Territory()  Initializes a new instance of the Territory  class 
 Territory(String)  Initializes a new instance of the Territory  class 
 

Territory Class 
782 | Infor VISUAL API Toolkit  Shared Class Library Reference  Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Territories based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Territories based on search criteria, limited by 
record count.  
 Exists  Determine if a Territory exists.  
 Find Find a specific Territory.  
 Load()  Load all Territories.  
 Load(String)  Load a specific Territory row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewTerritoryRow  Adds a new row to the TERRITORY table.  
 Save  Save All previously loaded Territories to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Territory Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 783 Territory Constructor  
Overload List 
 Name  Description  
 Territory()  Initializes a new instance of the Territory  class 
 Territory(String)  Initializes a new instance of the Territory  class 
 
See Also 
Territory Class  
Lsa.Vmfg.Shared Namespace  
  

Territory Constructor  
784 | Infor VISUAL API Toolkit  Shared Class Library Reference  Territory Constructor  
Initializes a new instance of the Territory  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Territory()  
 
VB 
Public  Sub New 
 
See Also 
Territory Class  
Territory Overload  
Lsa.Vmfg.Shared Namespace  
  
Territory Constructor  (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 785 Territory Constructor (String)  
Initializes a new instance of the Territory  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Territory ( 
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
Territory Class  
Territory Overload  
Lsa.Vmfg.Shared Namespace  
  
Territory.Territory  Methods  
786 | Infor VISUAL API Toolkit  Shared Class Library Reference  Territory.Territory Methods  
The Territory  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Territories based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Territories based on search criteria, limited by 
record count.  
 Exists  Determine if a Territory exists.  
 Find Find a specific Territory.  
 Load()  Load all Territories.  
 Load(String)  Load a specific Territory row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewTerritoryRow  Adds a new row to the TERRITORY table.  
 Save  Save All previously loaded Territories to the database.  
 
See Also 
Territory Class  
Lsa.Vmfg.Shared Namespace  
  

Territory.Browse Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 787 Territory.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Territories based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Territories based on search criteria, limited by 
record count.  
 
See Also 
Territory Class  
Lsa.Vmfg.Shared Namespace  
  

Territory.Browse Method (String, String, String)  
788 | Infor VISUAL API Toolkit  Shared Class Library Reference  Territory.Browse Method (String, String, String)  
Retrieve Territories based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Territory Class  
Territory.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 789 Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Territory.Browse Method (String, String, String, Int32, Int32)  
790 | Infor VISUAL API Toolkit  Shared Class Library Reference  Territory.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve Territories based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Territory.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 791 Return Value  
Type: DataSet  
See Also 
Territory Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Territory.Exists Method  
792 | Infor VISUAL API Toolkit  Shared Class Library Reference  Territory.Exists Method  
Determine if a Territory exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  code 
) 
 
VB 
Public  Overridable  Function Exists  (  
 code As String  
) As Boolean 
 
Parameters  
code  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
Territory Class  
Lsa.Vmfg.Shared Namespace  
  
Territory.Find  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 793 Territory.Find Method  
Find a specific Territory.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Find (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
Territory Class  
Lsa.Vmfg.Shared Namespace  
  
Territory.Load  Method  
794 | Infor VISUAL API Toolkit  Shared Class Library Reference  Territory.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Territories.  
 Load(String)  Load a specific Territory row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
Territory Class  
Lsa.Vmfg.Shared Namespace  
  

Territory.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 795 Territory.Load Method  
Load all Territories.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
Territory Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Territory.Load  Method (String)  
796 | Infor VISUAL API Toolkit  Shared Class Library Reference  Territory.Load Method (String)  
Load a specific Territory row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
Territory Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Territory.Load  Method (Stream, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 797 Territory.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 code As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
code  
Type: System.String  
See Also 
Territory Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Territory.NewTerritoryRow  Method  
798 | Infor VISUAL API Toolkit  Shared Class Library Reference  Territory.NewTerritoryRow Method  
Adds a new row to the TERRITORY table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewTerritoryRow ( 
 string  code 
) 
 
VB 
Public  Function  NewTerritoryRow  (  
 code As String  
) As DataRow  
 
Parameters  
code  
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Territory Class  
Lsa.Vmfg.Shared Namespace  
  
Territory.Save Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 799 Territory.Save Method  
Save All previously loaded Territories to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Territory Class  
Lsa.Vmfg.Shared Namespace  
  
Unit Class  
800 | Infor VISUAL API Toolkit  Shared Class Library Reference  Unit Class  
Maintain Units of Measure.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Unit  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  Unit : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  Unit 
 Inherits  BusinessDocument  
 
The Unit type exposes the following members.  
Constructors 
 Name  Description  
 Unit()  Initializes a new instance of the Unit  class 
 Unit(String)  Initializes a new instance of the Unit  class 
 

Unit Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 801 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Units of Measure based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Units of Measure based on search criteria, limited 
by record count.  
 Exists  Determine if a Unit of Measure exists.  
 Find Find a specific Unit of Measure.  
 Load()  Load all Units of Measure.  
 Load(String)  Load a specific Units of Measure row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewUnitsRow  Adds a row to the UNITS table.  
 Save  Save all previously loaded Unit of Measure to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Unit Constructor  
802 | Infor VISUAL API Toolkit  Shared Class Library Reference  Unit Constructor  
Overload List 
 Name  Description  
 Unit()  Initializes a new instance of the Unit  class 
 Unit(String)  Initializes a new instance of the Unit  class 
 
See Also 
Unit Class  
Lsa.Vmfg.Shared Namespace  
  

Unit Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 803 Unit Constructor  
Initializes a new instance of the Unit  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Unit() 
 
VB 
Public  Sub New 
 
See Also 
Unit Class  
Unit Overload  
Lsa.Vmfg.Shared Namespace  
  
Unit Constructor  (String) 
804 | Infor VISUAL API Toolkit  Shared Class Library Reference  Unit Constructor (String)  
Initializes a new instance of the Unit  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Unit( 
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
Unit Class  
Unit Overload  
Lsa.Vmfg.Shared Namespace  
  
Unit.Unit  Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 805 Unit.Unit Methods  
The Unit type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Units of Measure based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Units of Measure based on search criteria, limited 
by record count.  
 Exists  Determine if a Unit of Measure exists.  
 Find Find a specific Unit of Measure.  
 Load()  Load all Units of Measure.  
 Load(String)  Load a specific Units of Measure row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 NewUnitsRow  Adds a row to the UNITS table.  
 Save  Save all previously loaded Unit of Measure to the database.  
 
See Also 
Unit Class  
Lsa.Vmfg.Shared Namespace  
  

Unit.Browse  Method  
806 | Infor VISUAL API Toolkit  Shared Class Library Reference  Unit.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Units of Measure based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Units of Measure based on search criteria, limited 
by record count.  
 
See Also 
Unit Class  
Lsa.Vmfg.Shared Namespace  
  

Unit.Browse  Method (String, String, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 807 Unit.Browse Method (String, String, String)  
Retrieve Units of Measure based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Unit Class  
Unit.Browse  Method (String, String, String)  
808 | Infor VISUAL API Toolkit  Shared Class Library Reference  Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Unit.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 809 Unit.Browse Method (String, String, String, Int32, 
Int32)  
Retrieve Units of Measure based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Unit.Browse  Method (String, String, String, Int32, Int32)  
810 | Infor VISUAL API Toolkit  Shared Class Library Reference  Return Value  
Type: DataSet  
See Also 
Unit Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Unit.Exists  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 811 Unit.Exists Method  
Determine if a Unit of Measure exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  unit_of_measure  
) 
 
VB 
Public  Overridable  Function Exists  (  
 unit_of_measure As String  
) As Boolean 
 
Parameters  
unit_of_measure  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
Unit Class  
Lsa.Vmfg.Shared Namespace  
  
Unit.Find  Method 
812 | Infor VISUAL API Toolkit  Shared Class Library Reference  Unit.Find Method  
Find a specific Unit of Measure.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  unit_of_measure  
) 
 
VB 
Public  Overridable  Sub Find (  
 unit_of_measure As String  
) 
 
Parameters  
unit_of_measure  
Type: System.String  
See Also 
Unit Class  
Lsa.Vmfg.Shared Namespace  
  
Unit.Load Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 813 Unit.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Units of Measure.  
 Load(String)  Load a specific Units of Measure row.  
 Load(Stream, String)  Load from stream and rename using new key.  
 
See Also 
Unit Class  
Lsa.Vmfg.Shared Namespace  
  

Unit.Load Method  
814 | Infor VISUAL API Toolkit  Shared Class Library Reference  Unit.Load Method  
Load all Units of Measure.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
Unit Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Unit.Load Method (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 815 Unit.Load Method (String)  
Load a specific Units of Measure row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  unit_of_measure  
) 
 
VB 
Public  Overridable  Sub Load (  
 unit_of_measure As String  
) 
 
Parameters  
unit_of_measure  
Type: System.String  
See Also 
Unit Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Unit.Load Method (Stream, String)  
816 | Infor VISUAL API Toolkit  Shared Class Library Reference  Unit.Load Method (Stream, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  unit_of_measure  
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 unit_of_measure As String  
) 
 
Parameters  
stream  
Type: System.IO.Stream  
unit_of_measure  
Type: System.String  
See Also 
Unit Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Unit.NewUnitsRow  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 817 Unit.NewUnitsRow Method  
Adds a row to the UNITS table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  DataRow  NewUnitsRow( 
 string  unit 
) 
 
VB 
Public  Function  NewUnitsRow  (  
 unit As String 
) As DataRow  
 
Parameters  
unit 
Type: System.String  
Return Value  
Type: DataRow  
See Also 
Unit Class  
Lsa.Vmfg.Shared Namespace  
  
Unit.Save  Method 
818 | Infor VISUAL API Toolkit  Shared Class Library Reference  Unit.Save Method  
Save all previously loaded Unit of Measure to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Unit Class  
Lsa.Vmfg.Shared Namespace  
  
UnitofMeasureConversion Class 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 819 UnitofMeasureConversion Class  
Service to calculate the quantity of a part in one unit of measure given a quantity of another unit of 
measure.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessService  
          Lsa.Vmfg.Shared.UnitofMeasureConversion  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  UnitofMeasureConversion : BusinessService  
 
VB 
<SerializableAttribute> 
Public  Class  UnitofMeasureConversion 
 Inherits  BusinessService 
 
The UnitofMeasureConversion type exposes the following members.  
Constructors 
 Name  Description  
 UnitofMeasureConversion()  Initializes a new instance of the UnitofMeasureConversion  
class 

UnitofMeasureConversion Class 
820 | Infor VISUAL API Toolkit  Shared Class Library Reference   UnitofMeasureConversion(String)  Initializes a new instance of the UnitofMeasureConversion  
class 
 
Methods 
 Name  Description  
 Execute  Execute the Service..  
 NewInputRow  Adds a new service request row.  
 Prepare  Prepare the Service for execution.  
Data Tables 
 Table Type  Table Name  
 Header Table  CONVERT_UM  
 Results Sub- table CONVERT_UM_RESULT  
See Also 
Lsa.Vmfg.Shared Namespace  
  

UnitofMeasureConversion Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 821 UnitofMeasureConversion Constructor  
Overload List 
 Name  Description  
 UnitofMeasureConversion()  Initializes a new instance of the UnitofMeasureConversion  
class 
 UnitofMeasureConversion(String)  Initializes a new instance of the UnitofMeasureConversion  
class 
 
See Also 
UnitofMeasureConversion Class  
Lsa.Vmfg.Shared Namespace  
  

UnitofMeasureConversion Constructor  
822 | Infor VISUAL API Toolkit  Shared Class Library Reference  UnitofMeasureConversion Constructor  
Initializes a new instance of the UnitofMeasureConversion  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  UnitofMeasureConversion()  
 
VB 
Public  Sub New 
 
See Also  
UnitofMeasureConversion Class  
UnitofMeasureConversion Overload  
Lsa.Vmfg.Shared Namespace  
  
UnitofMeasureConversion Constructor  (String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 823 UnitofMeasureConversion Constructor (String)  
Initializes a new instance of the UnitofMeasureConversion  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  UnitofMeasureConversion( 
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
UnitofMeasureConversion Class  
UnitofMeasureConversion Overload  
Lsa.Vmfg.Shared Namespace  
  
UnitofMeasureConversion.UnitofMeasureConversion  Methods  
824 | Infor VISUAL API Toolkit  Shared Class Library Reference  UnitofMeasureConversion.UnitofMeasureConversion 
Methods  
The UnitofMeasureConversion  type exposes the following members.  
Methods 
 Name  Description  
 Execute  Execute the Service..  
 NewInputRow  Adds a new service request row.  
 Prepare  Prepare the Service for execution.  
 
See Also 
UnitofMeasureConversion Class  
Lsa.Vmfg.Shared Namespace  
  

UnitofMeasureConversion.Execute Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 825 UnitofMeasureConversion.Execute Method  
Execute the Service..  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Execute () 
 
VB 
Public  Overridable  Sub Execute  
 
See Also 
UnitofMeasureConversion Class  
Lsa.Vmfg.Shared Namespace  
  
UnitofMeasureConversion.NewInputRow  Method  
826 | Infor VISUAL API Toolkit  Shared Class Library Reference  UnitofMeasureConversion.NewInputRow Method  
Adds a new service request row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
UnitofMeasureConversion Class  
Lsa.Vmfg.Shared Namespace  
  
UnitofMeasureConversion.Prepare Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 827 UnitofMeasureConversion.Prepare Method  
Prepare the Service for execution.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Prepare()  
 
VB 
Public  Overridable  Sub Prepare  
 
See Also 
UnitofMeasureConversion Class  
Lsa.Vmfg.Shared Namespace  
  
UnitOfMeasureConversion Data Tables  
828 | Infor VISUAL API Toolkit  Shared Class Library Reference  UnitOfMeasureConversion Data Tables  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
DataSet Name returned from Prepare: CONVERT_UM  
Header Table  
Table Name:  CONVERT_UM  
Primary Key:  ENTRY_NO  
Column Name  Type  Description 
ENTRY_NO  Integer  Unique i nteger identifying this record. 
Required.  
PART_ID  String  The Part ID for which the unit of measure 
conversion will be performed.  Required.  
FROM_UM  String Unit of measure you are converting from.  
Required.  
FROM_QTY  Decimal  Quantity expressed in the FROM_UM.   
Required.  
TO_UM  String Unit of measure you want the quantity converted to. Required.  
Results Sub-table 
Table Name : CONVERT_UM_RESULT  
Column Name  Type  Description 
ENTRY_NO  Integer  Same as header table.  
PART_ID  String  Same as header table.  
TO_UM  String Same as header table.  
TO_QTY  Decimal  Quantity expressed in the TO_UM.  
See Also 
UnitofMeasureConversion Class  
UnitOfMeasureConversion Data Tables  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 829 Lsa.Vmfg.Shared Namespace   
Vat Class  
830 | Infor VISUAL API Toolkit  Shared Class Library Reference  Vat Class  
Maintain VAT Codes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Vat  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
[SerializableAttribute] 
public  class  Vat : BusinessDocument  
 
VB 
<SerializableAttribute> 
Public  Class  Vat 
 Inherits  BusinessDocument  
 
The Vat type exposes the following members.  
Constructors 
 Name  Description  
 Vat()  Default Constructor  
 Vat(String)  Business Document Constructor  
 

Vat Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 831 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve VAT Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve VAT Codes based on search criteria, limited by 
record count.  
 Exists  Determine if a VAT Code exists.  
 Find Find a specific VAT Code.  
 Load  Load all VAT Codes  
 NewVatRow  Add a new row to the VAT Table.  
 Save  Save all Previously loaded VAT Codes to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Vat Constructor  
832 | Infor VISUAL API Toolkit  Shared Class Library Reference  Vat Constructor  
Overload List 
 Name  Description  
 Vat()  Default Constructor  
 Vat(String)  Business Document Constructor  
 
See Also 
Vat Class  
Lsa.Vmfg.Shared Namespace  
  

Vat Constructor  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 833 Vat Constructor  
Default Constructor  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Vat() 
 
VB 
Public  Sub New 
 
See Also 
Vat Class  
Vat Overload  
Lsa.Vmfg.Shared Namespace  
  
Vat Constructor  (String)  
834 | Infor VISUAL API Toolkit  Shared Class Library Reference  Vat Constructor (String)  
Business Document Constructor  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Vat( 
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
Vat Class  
Vat Overload  
Lsa.Vmfg.Shared Namespace  
  
Vat.Vat Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 835 Vat.Vat Methods  
The Vat type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve VAT Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve VAT Codes based on search criteria, limited by 
record count.  
 Exists  Determine if a VAT Code exists.  
 Find Find a specific VAT Code.  
 Load  Load all VAT Codes  
 NewVatRow  Add a new row to the VAT Table.  
 Save  Save all Previously loaded VAT Codes to the database.  
 
See Also 
Vat Class  
Lsa.Vmfg.Shared Namespace  
  

Vat.Browse Method  
836 | Infor VISUAL API Toolkit  Shared Class Library Reference  Vat.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve VAT Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve VAT Codes based on search criteria, limited by 
record count.  
 
See Also 
Vat Class  
Lsa.Vmfg.Shared Namespace  
  

Vat.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 837 Vat.Browse Method (String, String, String)  
Retrieve VAT Codes based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Vat Class  
Vat.Browse Method (String, String, String)  
838 | Infor VISUAL API Toolkit  Shared Class Library Reference  Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Vat.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 839 Vat.Browse Method (String, String, String, Int32, 
Int32)  
Retrieve VAT Codes based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Vat.Browse Method (String, String, String, Int32, Int32)  
840 | Infor VISUAL API Toolkit  Shared Class Library Reference  Return Value  
Type: DataSet  
See Also 
Vat Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Vat.Exists  Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 841 Vat.Exists Method  
Determine if a VAT Code exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  code 
) 
 
VB 
Public  Overridable  Function Exists  (  
 code As String  
) As Boolean 
 
Parameters  
code  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
Vat Class  
Lsa.Vmfg.Shared Namespace  
  
Vat.Find  Method  
842 | Infor VISUAL API Toolkit  Shared Class Library Reference  Vat.Find Method  
Find a specific VAT Code.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Find (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
Vat Class  
Lsa.Vmfg.Shared Namespace  
  
Vat.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 843 Vat.Load Method  
Load all VAT Codes  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
Vat Class  
Lsa.Vmfg.Shared Namespace  
  
Vat.NewVatRow  Method  
844 | Infor VISUAL API Toolkit  Shared Class Library Reference  Vat.NewVatRow Method  
Add a new row to the VAT Table.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewVatRow ( 
 string  code 
) 
 
VB 
Public  Overridable  Function NewVatRow  (  
 code As String  
) As DataRow  
 
Parameters  
code  
Type: System.String  
Return Value  
Type: DataRow  
 
See Also 
Vat Class  
Lsa.Vmfg.Shared Namespace  
  
Vat.Save Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 845 Vat.Save Method  
Save all Previously loaded VAT Codes to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Vat Class  
Lsa.Vmfg.Shared Namespace  
  
Wbs Class  
846 | Infor VISUAL API Toolkit  Shared Class Library Reference  Wbs Class  
Maintain WBS Codes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Wbs  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  class  Wbs : BusinessDocument 
 
VB 
Public  Class  Wbs 
 Inherits  BusinessDocument  
 
The Wbs  type exposes the following members.  
Constructors 
 Name  Description  
 Wbs Initializes a new instance of the Wbs  class 
 

Wbs Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 847 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve WBS Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve WBS Codes based on search criteria, limited by 
record count.  
 Exists  Determine if a WBS Code exists.  
 Find Find a specific WBS Code.  
 Load()  Load all WBS Codes.  
 Load(String, String)  Load a specific WBS row.  
 Load(Stream, String, String)  Load from stream and rename using new key.  
 Save  Save All previously loaded WBS Codes to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Wbs Constructor  
848 | Infor VISUAL API Toolkit  Shared Class Library Reference  Wbs Constructor  
Initializes a new instance of the Wbs  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Wbs( 
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
Wbs Class  
Lsa.Vmfg.Shared Namespace  
  
Wbs.Wbs Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 849 Wbs.Wbs Methods  
The Wbs type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve WBS Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve WBS Codes based on search criteria, limited by 
record count.  
 Exists  Determine if a WBS Code exists.  
 Find Find a specific WBS Code.  
 Load()  Load all WBS Codes.  
 Load(String, String)  Load a specific WBS row.  
 Load(Stream, String, String)  Load from stream and rename using new key.  
 Save  Save All previously loaded WBS Codes to the database.  
 
See Also 
Wbs Class  
Lsa.Vmfg.Shared Namespace  
  

Wbs.Browse Method  
850 | Infor VISUAL API Toolkit  Shared Class Library Reference  Wbs.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve WBS Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve WBS Codes based on search criteria, limited by 
record count.  
 
See Also 
Wbs Class  
Lsa.Vmfg.Shared Namespace  
  

Wbs.Browse Method (String, String, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 851 Wbs.Browse Method (String, String, String)  
Retrieve WBS Codes based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Wbs Class  
Wbs.Browse Method (String, String, String)  
852 | Infor VISUAL API Toolkit  Shared Class Library Reference  Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Wbs.Browse Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 853 Wbs.Browse Method (String, String, String, Int32, 
Int32)  
Retrieve WBS Codes based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Wbs.Browse Method (String, String, String, Int32, Int32)  
854 | Infor VISUAL API Toolkit  Shared Class Library Reference  Return Value  
Type: DataSet  
See Also 
Wbs Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Wbs.Exists Method 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 855 Wbs.Exists Method  
Determine if a WBS Code exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  code , 
 string  projectID  
) 
 
VB 
Public  Overridable  Function Exists  (  
 code As String , 
 projectID  As String 
) As Boolean 
 
Parameters  
code  
Type: System.String  
projectID  
Type: System.String  
Return Value  
Type: Boolean  
See Also 
Wbs Class  
Lsa.Vmfg.Shared Namespace  
  
Wbs.Find  Method  
856 | Infor VISUAL API Toolkit  Shared Class Library Reference  Wbs.Find Method  
Find a specific WBS Code.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  code , 
 string  projectID  
) 
 
VB 
Public  Overridable  Sub Find (  
 code As String , 
 projectID  As String 
) 
 
Parameters  
code  
Type: System.String  
projectID  
Type: System.String  
See Also 
Wbs Class  
Lsa.Vmfg.Shared Namespace  
  
Wbs.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 857 Wbs.Load Method  
Overload List 
 Name  Description  
 Load()  Load all WBS Codes.  
 Load(String, String)  Load a specific WBS row.  
 Load(Stream, String, String)  Load from stream and rename using new key.  
 
See Also 
Wbs Class  
Lsa.Vmfg.Shared Namespace  
  

Wbs.Load  Method  
858 | Infor VISUAL API Toolkit  Shared Class Library Reference  Wbs.Load Method  
Load all WBS Codes.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
Wbs Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Wbs.Load  Method (String, String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 859 Wbs.Load Method (String, String)  
Load a specific WBS row.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  code , 
 string  projectID  
) 
 
VB 
Public  Overridable  Sub Load (  
 code As String , 
 projectID  As String 
) 
 
Parameters  
code  
Type: System.String  
projectID  
Type: System.String  
See Also 
Wbs Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Wbs.Load  Method (Stream, String, String)  
860 | Infor VISUAL API Toolkit  Shared Class Library Reference  Wbs.Load Method (Stream, String, String)  
Load from stream and rename using new key.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 Stream  stream , 
 string  code , 
 string  projectID  
) 
 
VB 
Public  Overridable  Sub Load (  
 stream  As Stream , 
 code As String , 
 projectID  As String 
) 
 
Parameters  
stream  
Type: System.IO.Stream  
code  
Type: System.String  
projectID  
Type: System.String  
See Also 
Wbs Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Wbs.Save  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 861 Wbs.Save Method  
Save All previously loaded WBS Codes to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Wbs Class  
Lsa.Vmfg.Shared Namespace  
  
Withholding Class  
862 | Infor VISUAL API Toolkit  Shared Class Library Reference  Withholding Class  
Maintain Withholding Codes.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessDocument  
          Lsa.Vmfg.Shared.Withholding  
 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  class  Withholding : BusinessDocument  
 
VB 
Public  Class  Withholding  
 Inherits  BusinessDocument  
 
The Withholding type exposes the following members.  
Constructors 
 Name  Description  
 Withholding  Initializes a new instance of the Withholding class 
 

Withholding Class  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 863 Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Withholding Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Withholding Codes based on search criteria, 
limited by record count.  
 Exists  Determine if a Withholding Code exists.  
 Find Find a specific Withholding Code  
 Load()  Load all Withholding Codes  
 Load(String)  Load a specific Withholding Code  
 NewWithholdingRow  Add a new row to the WITHHOLDING Table  
 Save  Save all previously loaded Withholding Codes to the database.  
 
See Also 
Lsa.Vmfg.Shared Namespace  
  

Withholding Constructor  
864 | Infor VISUAL API Toolkit  Shared Class Library Reference  Withholding Constructor  
Initializes a new instance of the Withholding  class 
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  Withholding( 
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
Withholding Class  
Lsa.Vmfg.Shared Namespace  
  
Withholding.Withholding  Methods  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 865 Withholding.Withholding Methods  
The Withholding  type exposes the following members.  
Methods 
 Name  Description  
 Browse(String, String, String)  Retrieve Withholding Codes based on search criteria.  
 Browse(String, String, String, Int32, 
Int32)  Retrieve Withholding Codes based on search criteria, 
limited by record count.  
 Exists  Determine if a Withholding Code exists.  
 Find Find a specific Withholding Code  
 Load()  Load all Withholding Codes  
 Load(String)  Load a specific Withholding Code  
 NewWithholdingRow  Add a new row to the WITHHOLDING Table  
 Save  Save all previously loaded Withholding Codes to the database.  
 
See Also 
Withholding Class  
Lsa.Vmfg.Shared Namespace  
  

Withholding.Browse  Method  
866 | Infor VISUAL API Toolkit  Shared Class Library Reference  Withholding.Browse Method  
Overload List 
 Name  Description  
 Browse(String, String, String)  Retrieve Withholding Codes based on search criteria.  
 Browse(String, String, String, 
Int32, Int32)  Retrieve Withholding Codes based on search criteria, 
limited by record count.  
 
See Also 
Withholding Class  
Lsa.Vmfg.Shared Namespace  
  

Withholding.Browse  Method (String, String, String)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 867 Withholding.Browse Method (String, String, String)  
Retrieve Withholding Codes based on search criteria.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Withholding Class  
Withholding.Browse  Method (String, String, String)  
868 | Infor VISUAL API Toolkit  Shared Class Library Reference  Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Withholding.Browse  Method (String, String, String, Int32, Int32)  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 869 Withholding.Browse Method (String, String, String, 
Int32, Int32) 
Retrieve Withholding Codes based on search criteria, limited by record count.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
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
Withholding.Browse  Method (String, String, String, Int32, Int32)  
870 | Infor VISUAL API Toolkit  Shared Class Library Reference  Return Value  
Type: DataSet  
See Also 
Withholding Class  
Browse Overload  
Lsa.Vmfg.Shared Namespace  
  
Withholding.Exists  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 871 Withholding.Exists Method  
Determine if a Withholding Code exists.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  bool Exists ( 
 string  code 
) 
 
VB 
Public  Overridable  Function Exists  (  
 code As String  
) As Boolean 
 
Parameters  
code  
Type: System.String  
Return Value  
Type: Boolean  
 
See Also 
Withholding Class  
Lsa.Vmfg.Shared Namespace  
  
Withholding.Find Method  
872 | Infor VISUAL API Toolkit  Shared Class Library Reference  Withholding.Find Method  
Find a specific Withholding Code  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Find( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Find (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
Withholding Class  
Lsa.Vmfg.Shared Namespace  
  
Withholding.Load  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 873 Withholding.Load Method  
Overload List 
 Name  Description  
 Load()  Load all Withholding Codes  
 Load(String)  Load a specific Withholding Code  
 
See Also 
Withholding Class  
Lsa.Vmfg.Shared Namespace  
  

Withholding.Load  Method  
874 | Infor VISUAL API Toolkit  Shared Class Library Reference  Withholding.Load Method  
Load all Withholding Codes  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load()  
 
VB 
Public  Overridable  Sub Load  
 
See Also 
Withholding Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Withholding.Load  Method (String) 
Infor VISUAL API Toolkit  Shared Class Library Reference  | 875 Withholding.Load Method (String)  
Load a specific Withholding Code  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Load( 
 string  code 
) 
 
VB 
Public  Overridable  Sub Load (  
 code As String  
) 
 
Parameters  
code  
Type: System.String  
See Also 
Withholding Class  
Load Overload  
Lsa.Vmfg.Shared Namespace  
  
Withholding.NewWithholdingRow  Method  
876 | Infor VISUAL API Toolkit  Shared Class Library Reference  Withholding.NewWithholdingRow Method  
Add a new row to the WITHHOLDING Table  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  DataRow  NewWithholdingRow ( 
 string  withholdingCode  
) 
 
VB 
Public  Overridable  Function NewWithholdingRow  (  
 withholdingCode  As String  
) As DataRow  
 
Parameters  
withholdingCode 
Type: System.String  
Return Value  
Type: DataRow  
 
See Also 
Withholding Class  
Lsa.Vmfg.Shared Namespace  
  
Withholding.Save  Method  
Infor VISUAL API Toolkit  Shared Class Library Reference  | 877 Withholding.Save Method  
Save all previously loaded Withholding Codes to the database.  
Namespace:  Lsa.Vmfg.Shared  
Assembly:  VmfgShared (in VmfgShared.dll) Version: 8.1.100.0 (8.1.100.0)  
Syntax 
C# 
public  virtual  void Save () 
 
VB 
Public  Overridable  Sub Save  
 
See Also 
Withholding Class  
Lsa.Vmfg.Shared Namespace  
 
