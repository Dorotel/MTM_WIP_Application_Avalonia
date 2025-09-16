# Reference - Shared Library - 24 Pages !

*Converted from PDF*

---

  
Infor VISUAL API Toolkit Shared 
Library Reference 
 
  
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
service names referenced may be registered trademarks or trademarks of their respective owners.  
Publication Information 
Release: Infor VISUAL  
Publication date: August 13, 2024  
 
  
About this guide  
This guide describes  the objects available for use in the Infor VISUAL API Toolkit Shared library.  
NOTE: This class library exposes classes and methods that are not compatible with the VISUAL API 
Toolkit. Only the classes and methods specifically described in this document are compatible. The 
use of any class or method that is not described in this document is not supported.  
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
About this guide  
4 | Infor VISUAL  API Toolkit Shared Library Reference Support information  
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
 
Lsa.Shared Namespace  
Infor VISUAL  API Toolkit Shared Library Reference   | 5 Lsa.Shared Namespace  
  
Classes 
 Class  Description  
 GeneralQuery  General Query. This service allows you to cause a query execution 
under data object control with transmission of the result set to the client side.  
   

GeneralQuery Class  
6 | Infor VISUAL  API Toolkit Shared Library Reference GeneralQuery Class  
General Query. This service allows you to cause a query execution under data object control 
with transmission of the result set to the client side.  
Inheritance Hierarchy 
System.Object  
  System.MarshalByRefObject  
    System.ComponentModel.Component  
      BusinessObject  
        BusinessService  
          Lsa.Shared.GeneralQuery  
 
Namespace:  Lsa.Shared  
Assembly:  LsaShared (in LsaShared.dll) Version: 8.1.0.0 (8.1.0.0)  
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
GeneralQuery Class  
Infor VISUAL  API Toolkit Shared Library Reference   | 7 Constructors 
 Name  Description  
 GeneralQuery()  Constructor.  
 GeneralQuery(String)  Constructor.  
 
Methods 
 Name  Description  
 Execute()  Execute the specified query. All rows are returned.  
 Execute(String)  Execute the specified query. All rows are returned.  
 Execute(Int32, Int32)  Execute the specified query, starting at a specified row 
number, limited by a record count.  
 Execute(String, Int32, 
Int32)  Execute the specified query, starting at a specified row number, limited by a record count.  
 Prepare  Prepare to execute the specified query.  
 
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object (Overrides BusinessObject.DataObjectType.)  
 Parameters   
 

GeneralQuery Class  
8 | Infor VISUAL  API Toolkit Shared Library Reference See Also 
Lsa.Shared Namespace  
  
GeneralQuery Constructor  
Infor VISUAL  API Toolkit Shared Library Reference   | 9 GeneralQuery Constructor  
Overload List 
 Name  Description  
 GeneralQuery()  Constructor.  
 GeneralQuery(String)  Constructor.  
 
See Also 
GeneralQuery Class  
Lsa.Shared Namespace  
  

GeneralQuery Constructor  
10 | Infor VISUAL  API Toolkit Shared Library Reference  GeneralQuery Constructor  
Constructor.  
Namespace:  Lsa.Shared  
Assembly:  LsaShared (in LsaShared.dll) Version: 8.1.0.0 (8.1.0.0)  
Syntax 
C# 
public  GeneralQuery () 
 
VB 
Public  Sub New 
 
See Also 
GeneralQuery Class  
GeneralQuery Overload  
Lsa.Shared Namespace  
  
GeneralQuery Constructor (String)  
Infor VISUAL  API Toolkit Shared Library Reference   | 11 GeneralQuery  Constructor (String)  
Constructor.  
Namespace:  Lsa.Shared  
Assembly:  LsaShared (in LsaShared.dll) Version: 8.1.0.0 (8.1.0.0)  
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
Lsa.Shared Namespace  
  
GeneralQuery.GeneralQuery Methods  
12 | Infor VISUAL  API Toolkit Shared Library Reference  GeneralQuery.GeneralQuery Methods  
The GeneralQuery  type exposes the following members.  
Methods 
 Name  Description  
 Execute()  Execute the specified query. All rows are returned.  
 Execute(String)  Execute the specified query. All rows are returned.  
 Execute(Int32, Int32)  Execute the specified query, starting at a specified row 
number, limited by a record count.  
 Execute(String, Int32, 
Int32)  Execute the specified query, starting at a specified row number, limited by a record count.  
 Prepare  Prepare to execute the specified query.  
 
See Also 
GeneralQuery Class  
Lsa.Shared Namespace  
  

GeneralQuery.Execute Method  
Infor VISUAL  API Toolkit Shared Library Reference   | 13 GeneralQuery.Execute Method  
Overload List 
 Name  Description  
 Execute()  Execute the specified query. All rows are returned.  
 Execute(String)  Execute the specified query. All rows are returned.  
 Execute(Int32, Int32)  Execute the specified query, starting at a specified row 
number, limited by a record count.  
 Execute(String, Int32, 
Int32)  Execute the specified query, starting at a specified row number, limited by a record count.  
 
See Also 
GeneralQuery Class  
Lsa.Shared Namespace  
  

GeneralQuery.Execute Method  
14 | Infor VISUAL  API Toolkit Shared Library Reference  GeneralQuery.Execute Method  
Execute the specified query. All rows are returned.  
Namespace:  Lsa.Shared  
Assembly:  LsaShared (in LsaShared.dll) Version: 8.1.0.0 (8.1.0.0)  
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
Lsa.Shared Namespace  
  
GeneralQuery.Execute Method (String)  
Infor VISUAL  API Toolkit Shared Library Reference   | 15 GeneralQuery.Execute Method (String)  
Execute the specified query. All rows are returned.  
Namespace:  Lsa.Shared  
Assembly:  LsaShared (in LsaShared.dll) Version: 8.1.0.0 (8.1.0.0)  
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
Must match the table name from Prepare.  
Implements 
IGeneralQuery.Execute(String)  
 
See Also 
GeneralQuery Class  
Execute Overload  
Lsa.Shared Namespace  
  
GeneralQuery.Execute Method (Int32, Int32)  
16 | Infor VISUAL  API Toolkit Shared Library Reference  GeneralQuery.Execute Method (Int32, Int32)  
Execute the specified query, starting at a specified row number, limited by a record count.  
Namespace:  Lsa.Shared  
Assembly:  LsaShared (in LsaShared.dll) Version: 8.1.0.0 (8.1.0.0)  
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
GeneralQuery.Execute Method (Int32, Int32)  
Infor VISUAL  API Toolkit Shared Library Reference   | 17 Lsa.Shared Namespace  
  
GeneralQuery.Execute Method (String, Int32, Int32)  
18 | Infor VISUAL  API Toolkit Shared Library Reference  GeneralQuery.Execute Method (String, Int32, Int32)  
Execute the specified query, starting at a specified row number, limited by a record count.  
Namespace:  Lsa.Shared  
Assembly:  LsaShared (in LsaShared.dll) Version: 8.1.0.0 (8.1.0.0)  
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
Must match the table name from Prepare.  
startRecord  
Type: System.Int32  
maxRecords  
Type: System.Int32  
Implements 
IGeneralQuery.Execute(String, Int32, Int32)  
 
GeneralQuery.Execute Method (String, Int32, Int32)  
Infor VISUAL  API Toolkit Shared Library Reference   | 19 See Also 
GeneralQuery Class  
Execute Overload  
Lsa.Shared Namespace  
  
GeneralQuery.Prepare  Method  
20 | Infor VISUAL  API Toolkit Shared Library Reference  GeneralQuery.Prepare Method  
Prepare to execute the specified query.  
Namespace:  Lsa.Shared  
Assembly:  LsaShared (in LsaShared.dll) Version: 8.1.0.0 (8.1.0.0)  
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
Lsa.Shared Namespace  
GeneralQuery.Prepare Method  
Infor VISUAL  API Toolkit Shared Library Reference   | 21   
GeneralQuery.GeneralQuery Properties  
22 | Infor VISUAL  API Toolkit Shared Library Reference  GeneralQuery.GeneralQuery Properties  
The GeneralQuery  type exposes the following members.  
Properties 
 Name  Description  
 DataObjectType  Report types of data object associated with this business object 
(Overrides BusinessObject.DataObjectType.)  
 Parameters   
 
See Also 
GeneralQuery Class  
Lsa.Shared Namespace  
  

GeneralQuery.DataObjectType Property  
Infor VISUAL  API Toolkit Shared Library Reference   | 23 GeneralQuery.DataObjectType Property  
Report types of data object associated with this business object  
Namespace:  Lsa.Shared  
Assembly:  LsaShared (in LsaShared.dll) Version: 8.1.0.0 (8.1.0.0)  
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
Lsa.Shared Namespace  
  
GeneralQuery.Parameters Property  
24 | Infor VISUAL  API Toolkit Shared Library Reference  GeneralQuery.Parameters Property  
Namespace:  Lsa.Shared  
Assembly:  LsaShared (in LsaShared.dll) Version: 8.1.0.0 (8.1.0.0)  
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
Lsa.Shared Namespace  
 
