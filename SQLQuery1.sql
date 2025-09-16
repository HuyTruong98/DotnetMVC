-- Tạo Database
CREATE DATABASE OnlineStoreDB;
GO

USE OnlineStoreDB;
GO

-- Bảng Users
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE,
    Phone NVARCHAR(20),
    Address NVARCHAR(255),
    Role NVARCHAR(20) CHECK (Role IN ('Admin','User')) NOT NULL DEFAULT 'User',
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Bảng Categories
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255)
);

-- Bảng Products
CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryID INT NOT NULL FOREIGN KEY REFERENCES Categories(CategoryID),
    ProductName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(50) CHECK (Status IN ('Available','OutOfStock','Promotion')) DEFAULT 'Available',
    CreatedAt DATETIME DEFAULT GETDATE()
    IsFeatured BIT NOT NULL DEFAULT 0
);

-- Bảng ProductVariants (Size, Màu, Tồn kho)
CREATE TABLE ProductVariants (
    VariantID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT NOT NULL FOREIGN KEY REFERENCES Products(ProductID),
    Size NVARCHAR(10) NULL,     
    Color NVARCHAR(50) NULL,    
    Stock INT NOT NULL DEFAULT 0
);

-- Bảng ProductImages (nhiều ảnh cho 1 sản phẩm)
CREATE TABLE ProductImages (
    ImageID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT NOT NULL FOREIGN KEY REFERENCES Products(ProductID),
    ImageURL NVARCHAR(255) NOT NULL,
    IsPrimary BIT DEFAULT 0
);

-- Bảng Orders
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL FOREIGN KEY REFERENCES Users(UserID),
    OrderDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) CHECK (Status IN ('Pending','Confirmed','Shipping','Completed','Cancelled')) DEFAULT 'Pending',
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0
);

-- Bảng OrderDetails
CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL FOREIGN KEY REFERENCES Orders(OrderID),
    VariantID INT NOT NULL FOREIGN KEY REFERENCES ProductVariants(VariantID),
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(18,2) NOT NULL
);

-- Bảng Promotions
CREATE TABLE Promotions (
    PromotionID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT NOT NULL FOREIGN KEY REFERENCES Products(ProductID),
    DiscountPercent INT CHECK (DiscountPercent BETWEEN 0 AND 100),
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL
);


-- Categories (thời trang)
INSERT INTO Categories (CategoryName, Description)
VALUES 
(N'Áo nam', N'Các loại áo sơ mi, áo thun, áo khoác dành cho nam'),
(N'Áo nữ', N'Áo sơ mi, áo kiểu, áo thun, áo khoác dành cho nữ'),
(N'Quần nam', N'Quần jeans, quần tây, quần short cho nam'),
(N'Quần nữ', N'Quần jeans, quần tây cho nữ'),
(N'Giày dép', N'Giày thể thao, giày da, sandal, dép'),
(N'Túi xách & Balo', N'Túi xách thời trang, balo, cặp'),
(N'Phụ kiện', N'Nón, thắt lưng, đồng hồ, trang sức');