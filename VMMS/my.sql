  CREATE TABLE [crs_bill](
  [BillID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [BillGUID] GUID NOT NULL UNIQUE, 
  [BillCode] NVARCHAR NOT NULL UNIQUE, 
  [BillDate] DATETIME, 
  [SourceGUID] GUID, 
  [SourceCode] NVARCHAR, 
  [CustomerGUID] GUID, 
  [CustomerName] NVARCHAR, 
  [UserGUID] GUID, 
  [UserName] NVARCHAR, 
  [SendName] NVARCHAR, 
  [MobilePhone] NVARCHAR, 
  [CarGUID] GUID, 
  [CarCode] NVARCHAR, 
  [LicensePlate] NVARCHAR, 
  [VIN] NVARCHAR, 
  [ModelGUID] GUID, 
  [ModelName] NVARCHAR, 
  [CarMileage] INTEGER, 
  [TotalMileage] INTEGER, 
  [Mileage] INTEGER, 
  [NextMaintenanceDate] DATETIME, 
  [ItemGUID] GUID, 
  [ItemName] NVARCHAR, 
  [ItemContent] NVARCHAR, 
  [InsuranceCompany] NVARCHAR, 
  [InsuranceDate] DATETIME,
  [TotalDebitNumber] DECIMAL DEFAULT 0, 
  [TotalDebitAmount] DECIMAL DEFAULT 0, 
  [TotalCreditNumber] DECIMAL DEFAULT 0, 
  [TotalCreditAmount] DECIMAL DEFAULT 0, 
  [TotalSalesNumber] DECIMAL DEFAULT 0, 
  [TotalSalesAmount] DECIMAL DEFAULT 0, 
  [TotalDiffAmount] DECIMAL DEFAULT 0, 
  [TotalChargeAmount] DECIMAL DEFAULT 0, 
  [Remark] NVARCHAR, 
  [StatusID] INTEGER DEFAULT 0, 
  [TypeID] INTEGER DEFAULT 0, 
  [DeleteMark] BOOLEAN DEFAULT 0, 
  [UpGUID] GUID NOT NULL, 
  [Uptime] DATETIME, 
  [IsCharge] BOOLEAN DEFAULT 0, 
  [ChargeGUID] GUID, 
  [ChargeTime] DATETIME, 
  [BuilderGUID] GUID, 
  [BuilderTime] DATETIME, 
  [CompleteGUID] GUID, 
  [CompleteTime] DATETIME);


CREATE TABLE [crs_bill_detail](
  [DetailID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [DetailGUID] GUID NOT NULL UNIQUE, 
  [DetailSN] INTEGER NOT NULL, 
  [BillGUID] GUID NOT NULL, 
  [ProductGUID] GUID NOT NULL, 
  [ProductCode] NVARCHAR, 
  [ProductName] NVARCHAR, 
  [UnitGUID] GUID NOT NULL, 
  [TypeGUID] GUID, 
  [PropertyID] INTEGER NOT NULL DEFAULT 0, 
  [DebitLocationGUID] GUID NOT NULL, 
  [DebitNumber] DECIMAL NOT NULL DEFAULT 0, 
  [DebitCost] DECIMAL NOT NULL DEFAULT 0, 
  [DebitAmount] DECIMAL NOT NULL DEFAULT 0, 
  [CreditLocationGUID] GUID NOT NULL, 
  [CreditNumber] DECIMAL NOT NULL DEFAULT 0, 
  [CreditCost] DECIMAL NOT NULL DEFAULT 0, 
  [CreditAmount] DECIMAL NOT NULL DEFAULT 0, 
  [SalesNumber] DECIMAL NOT NULL DEFAULT 0, 
  [SalesPrice] DECIMAL NOT NULL DEFAULT 0, 
  [SalesAmount] DECIMAL NOT NULL DEFAULT 0, 
  [DiffAmount] DECIMAL DEFAULT 0, 
  [ChargeAmount] DECIMAL DEFAULT 0, 
  [Uptime] DATETIME NOT NULL);

CREATE TABLE [crs_car](
  [CarID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [CarGUID] GUID NOT NULL UNIQUE, 
  [CarCode] NVARCHAR NOT NULL UNIQUE, 
  [LicensePlate] NVARCHAR NOT NULL UNIQUE, 
  [VIN] NVARCHAR, 
  [TotalMileage] INTEGER DEFAULT 0, 
  [ModelGUID] GUID, 
  [EngineModel] NVARCHAR, 
  [EngineCapacity] NVARCHAR, 
  [ManufactureDate] DATETIME, 
  [CustomerGUID] GUID, 
  [CustomerName] NVARCHAR, 
  [MobilePhone] NVARCHAR, 
  [Remark] NVARCHAR, 
  [StatusID] INTEGER DEFAULT 0, 
  [TypeID] INTEGER DEFAULT 0, 
  [DeleteMark] BOOLEAN DEFAULT 0, 
  [UpGUID] GUID NOT NULL, 
  [Uptime] DATETIME, 
  [CarColor] NVARCHAR, 
  [NextMaintenanceDate] DATETIME, 
  [InsuranceCompany] NVARCHAR, 
  [InsuranceDate] DATETIME);

CREATE TABLE [crs_config](
  [ConfigID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [StartDate] DATE NOT NULL, 
  [StartYear] INTEGER NOT NULL, 
  [StartMonth] INTEGER NOT NULL, 
  [CurrentDate] DATE NOT NULL, 
  [CurrentYear] INTEGER NOT NULL, 
  [CurrentMonth] INTEGER NOT NULL, 
  [IsExecute] BOOLEAN DEFAULT 0, 
  [UpGUID] GUID NOT NULL, 
  [Uptime] DATETIME NOT NULL);

CREATE TABLE [crs_content](
  [ContentID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [ContentGUID] GUID NOT NULL UNIQUE, 
  [ContentCode] NVARCHAR NOT NULL UNIQUE, 
  [ContentName] NVARCHAR NOT NULL UNIQUE, 
  [Remark] NVARCHAR, 
  [DeleteMark] BOOLEAN DEFAULT 0, 
  [UpGUID] GUID NOT NULL, 
  [Uptime] DATETIME);

CREATE TABLE [crs_customer](
  [CustomerID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [CustomerGUID] GUID NOT NULL UNIQUE, 
  [CustomerCode] NVARCHAR NOT NULL, 
  [CustomerName] NVARCHAR NOT NULL, 
  [MnemonicCode] NVARCHAR, 
  [Phone] NVARCHAR, 
  [Email] NVARCHAR, 
  [LinkAddress] NVARCHAR, 
  [LinkMan] NVARCHAR, 
  [MobilePhone] NVARCHAR, 
  [BankName] NVARCHAR, 
  [BankAccount] NVARCHAR, 
  [TaxNumber] NVARCHAR, 
  [Remark] NVARCHAR, 
  [DeleteMark] BOOLEAN DEFAULT 0, 
  [UpGUID] GUID NOT NULL, 
  [Uptime] DATETIME);

CREATE TABLE [crs_insurancecompany](
  [InsuranceCompanyID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [InsuranceCompanyGUID] GUID NOT NULL UNIQUE, 
  [InsuranceCompanyCode] NVARCHAR NOT NULL UNIQUE, 
  [InsuranceCompanyName] NVARCHAR NOT NULL UNIQUE, 
  [Remark] NVARCHAR, 
  [DeleteMark] BOOLEAN DEFAULT 0, 
  [UpGUID] GUID NOT NULL, 
  [Uptime] DATETIME);

CREATE TABLE [crs_inventory](
  [InventoryID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [ProductGUID] GUID NOT NULL, 
  [LocationGUID] GUID NOT NULL, 
  [InventoryNumber] DECIMAL DEFAULT 0, 
  [InventoryAmount] DECIMAL DEFAULT 0);

CREATE TABLE [crs_item](
  [ItemID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [ItemGUID] GUID NOT NULL UNIQUE, 
  [ItemCode] NVARCHAR NOT NULL UNIQUE, 
  [ItemName] NVARCHAR NOT NULL UNIQUE, 
  [Remark] NVARCHAR, 
  [DeleteMark] BOOLEAN DEFAULT 0, 
  [UpGUID] GUID NOT NULL, 
  [Uptime] DATETIME);

CREATE TABLE [crs_location](
  [LocationID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [LocationGUID] GUID NOT NULL UNIQUE, 
  [LocationCode] NVARCHAR NOT NULL UNIQUE, 
  [LocationName] NVARCHAR NOT NULL UNIQUE, 
  [Remark] NVARCHAR, 
  [DeleteMark] BOOLEAN DEFAULT 0, 
  [UpGUID] GUID NOT NULL, 
  [Uptime] DATETIME);

CREATE TABLE [crs_model](
  [ModelID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [ModelGUID] GUID NOT NULL UNIQUE, 
  [ModelCode] NVARCHAR NOT NULL, 
  [ModelName] NVARCHAR NOT NULL, 
  [DeleteMark] BOOLEAN DEFAULT 0, 
  [UpGUID] GUID NOT NULL, 
  [Uptime] DATETIME);

CREATE TABLE [crs_product](
  [ProductID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [ProductGUID] GUID NOT NULL UNIQUE, 
  [ProductCode] NVARCHAR NOT NULL UNIQUE, 
  [ProductName] NVARCHAR NOT NULL UNIQUE, 
  [MnemonicCode] NVARCHAR, 
  [Barcode] NVARCHAR, 
  [UnitGUID] GUID, 
  [TypeGUID] GUID, 
  [PropertyID] INTEGER DEFAULT 0, 
  [Price] DECIMAL DEFAULT 0, 
  [Remark] NVARCHAR, 
  [DeleteMark] BOOLEAN DEFAULT 0, 
  [UpGUID] GUID NOT NULL, 
  [Uptime] DATETIME, 
  [DiffAmount] DECIMAL DEFAULT 0, 
  [ChargeAmount] DECIMAL DEFAULT 0);

CREATE TABLE [crs_product_model](
  [ModelID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [ProductGUID] GUID NOT NULL, 
  [ModelGUID] GUID NOT NULL, 
  [DeleteMark] BOOLEAN DEFAULT 0, 
  [UpGUID] GUID NOT NULL, 
  [Uptime] DATETIME);

CREATE TABLE [crs_product_type](
  [TypeID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [TypeGUID] GUID NOT NULL UNIQUE, 
  [TypeCode] NVARCHAR NOT NULL, 
  [TypeName] NVARCHAR NOT NULL, 
  [DeleteMark] BOOLEAN DEFAULT 0, 
  [UpGUID] GUID NOT NULL, 
  [Uptime] DATETIME);

CREATE TABLE [crs_remark](
  [RemarkID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [RemarkGUID] GUID NOT NULL UNIQUE, 
  [RemarkCode] NVARCHAR NOT NULL UNIQUE, 
  [RemarkName] NVARCHAR NOT NULL UNIQUE, 
  [Remark] NVARCHAR, 
  [DeleteMark] BOOLEAN DEFAULT 0, 
  [UpGUID] GUID NOT NULL, 
  [Uptime] DATETIME);


CREATE TABLE [crs_unit](
  [UnitID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [UnitGUID] GUID NOT NULL UNIQUE, 
  [UnitCode] NVARCHAR NOT NULL, 
  [UnitName] NVARCHAR NOT NULL, 
  [DeleteMark] BOOLEAN DEFAULT 0, 
  [UpGUID] GUID NOT NULL, 
  [Uptime] DATETIME);

CREATE TABLE [sys_config](
  [ConfigID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [SysCompany] NVARCHAR, 
  [SysAddress] NVARCHAR, 
  [SysPhone] NVARCHAR, 
  [SysFax] NVARCHAR, 
  [SysEmail] NVARCHAR, 
  [Website] NVARCHAR, 
  [Weixin] NVARCHAR, 
  [WeixinQRcode] IMAGE, 
  [UpGUID] GUID, 
  [Uptime] DATETIME, 
  [SoftName] NVARCHAR, 
  [SoftPwd] NVARCHAR, 
  [SoftVersion] NVARCHAR, 
  [SoftDate] DATE, 
  [DbVersion] NVARCHAR, 
  [DbUptime] DATETIME);

CREATE TABLE [sys_user](
  [UserID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [UserGUID] GUID NOT NULL UNIQUE, 
  [UserCode] NVARCHAR NOT NULL, 
  [UserName] NVARCHAR NOT NULL, 
  [UserPwd] NVARCHAR, 
  [DeleteMark] BOOLEAN DEFAULT 0, 
  [CompanyGUID] GUID, 
  [WorkplaceGUID] GUID, 
  [TypeID] INTEGER, 
  [Office] NVARCHAR, 
  [Phone] NVARCHAR, 
  [MobilePhone] NVARCHAR, 
  [WeiXin] NVARCHAR, 
  [QQ] NVARCHAR, 
  [Email] NVARCHAR, 
  [UpGUID] GUID NOT NULL, 
  [Uptime] DATETIME);

ALTER TABLE [sys_config] ADD COLUMN [AutobackupPath] NVARCHAR; 

INSERT INTO sys_config (SoftName,DbVersion) VALUES ("VMMS","2.0425");