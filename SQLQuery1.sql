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
    Stock INT NOT NULL DEFAULT 0,
    Status NVARCHAR(50) CHECK (Status IN ('Available','OutOfStock','Promotion')) DEFAULT 'Available',
    CreatedAt DATETIME DEFAULT GETDATE()
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
    ProductID INT NOT NULL FOREIGN KEY REFERENCES Products(ProductID),
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



-- Thêm dữ liệu mẫu cho Users
INSERT INTO Users (Username, PasswordHash, FullName, Email, Phone, Address, Role)
VALUES 
('admin01', 'hashed_password_admin', N'Nguyễn Văn A', 'admin@example.com', '0905123456', N'Đà Nẵng', 'Admin'),
('user01', 'hashed_password_user1', N'Trần Thị B', 'user1@example.com', '0912345678', N'Hà Nội', 'User'),
('user02', 'hashed_password_user2', N'Lê Văn C', 'user2@example.com', '0987654321', N'Hồ Chí Minh', 'User');

-- Thêm dữ liệu mẫu cho Categories
INSERT INTO Categories (CategoryName, Description)
VALUES 
(N'Điện thoại', N'Các loại smartphone, điện thoại di động'),
(N'Laptop', N'Máy tính xách tay, ultrabook, gaming'),
(N'Phụ kiện', N'Phụ kiện điện thoại, máy tính');

-- Thêm dữ liệu mẫu cho Products
INSERT INTO Products (CategoryID, ProductName, Description, Price, Stock, Status)
VALUES
(1, N'iPhone 15 Pro Max', N'Điện thoại Apple cao cấp', 32990000, 20, 'Available'),
(1, N'Samsung Galaxy S24 Ultra', N'Smartphone Android cao cấp', 28990000, 15, 'Available'),
(2, N'Dell XPS 13', N'Laptop mỏng nhẹ cao cấp', 35990000, 10, 'Available'),
(2, N'Asus ROG Strix G16', N'Laptop gaming mạnh mẽ', 40990000, 5, 'Available'),
(3, N'Chuột Logitech MX Master 3S', N'Chuột không dây cao cấp', 2499000, 30, 'Available');

-- Thêm dữ liệu mẫu cho ProductImages
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary)
VALUES
(1, 'https://cdn.tgdd.vn/Products/Images/42/305660', 1)

-- Thêm dữ liệu mẫu cho Orders
INSERT INTO Orders (UserID, Status, TotalAmount)
VALUES
(2, 'Pending', 32990000),
(3, 'Confirmed', 28990000);

-- Thêm dữ liệu mẫu cho OrderDetails
INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice)
VALUES
(1, 1, 1, 32990000),
(2, 2, 1, 28990000);

-- Thêm dữ liệu mẫu cho Promotions
INSERT INTO Promotions (ProductID, DiscountPercent, StartDate, EndDate)
VALUES
(1, 10, '2025-09-01', '2025-09-10'),  -- iPhone giảm 10%
(3, 15, '2025-09-05', '2025-09-20'); -- Dell XPS giảm 15%
