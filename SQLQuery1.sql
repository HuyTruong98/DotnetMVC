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

ALTER TABLE Orders
ADD Description NVARCHAR(255);


-----------------------------------------------------
-- Áo nam

INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (1, N'Áo thun nam basic', N'Áo thun cotton co giãn, thoáng mát', 259000, 'Available', 1);

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(IDENT_CURRENT('Products'), N'M', N'Trắng', 50);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(IDENT_CURRENT('Products'), N'https://n7media.coolmate.me/uploads/June2025/ao-thun-nam-premium-cotton-thumb-1.png', 1),
(IDENT_CURRENT('Products'), N'https://n7media.coolmate.me/uploads/June2025/ao-thun-nam-premium-cotton-_11-trang.jpg?aio=w-1100', 0),
(IDENT_CURRENT('Products'), N'https://n7media.coolmate.me/uploads/June2025/ao-thun-nam-premium-cotton-2097-trang.jpg?aio=w-1100', 0),
(IDENT_CURRENT('Products'), N'https://n7media.coolmate.me/uploads/June2025/ao-thun-nam-premium-cotton-2093-trang.jpg?aio=w-1100', 0),
(IDENT_CURRENT('Products'), N'https://n7media.coolmate.me/uploads/June2025/ao-thun-nam-premium-cotton-2134-trang.jpg?aio=w-1100', 0);



INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (1, N'Áo dài nam cách tân', N'Chất liệu vải TAFTA mềm mịn
Dáng suông chuẩn form
Áo dài trơn đơn giản, thanh lịch
Lưu ý: Sản phẩm chỉ có áo dài, không kèm quần', 199000, 'Available', 1);

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(IDENT_CURRENT('Products'), N'M', N'Đỏ', 50),
(IDENT_CURRENT('Products'), N'M', N'Trắng', 50),
(IDENT_CURRENT('Products'), N'M', N'Xanh', 50);



INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/12/1-do-ld9201-800x1000.jpg', 1),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/12/ao-dai-nam-dep-thiet-ke-cach-tan-sang-trong-ld9201.png', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/12/3-xam-ld9201-800x1000.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/12/Ao-dai-cach-tan-LADOS-LD9201-vai-TAFTA-chuan-form-dep-480x600.jpg',0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/12/Ao-dai-nam-dep-chat-lieu-TAFTA-sang-trong-LADOS-LD9201-600x600.jpg', 0);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (1, N'Áo Thun Ba Lỗ Nam ', N'Phối Viền Năng Động Phong Cách Thể Thao', 299000, 'Available', 1);

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(IDENT_CURRENT('Products'), N'M', N'Trắng', 50),
(IDENT_CURRENT('Products'), N'M', N'Xám', 50),
(IDENT_CURRENT('Products'), N'M', N'Đen', 50),
(IDENT_CURRENT('Products'), N'M', N'Kem', 50);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/04/ao-thun-ba-lo-nam-vai-cotton-cao-cap-ld9209-600x600.jpg',1),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/04/2-KEM-LD9209-800x800.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/04/1-TRANG-LD9209-800x800.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/04/ao-ba-lo-nam-lados-chat-cotton-ld9209-600x600.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/04/ao-tanktop-nam-phoi-vien-tre-trung-ld9209-600x600.jpg',0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/04/ao-thun-ba-lo-nam-vai-cotton-cao-cap-600x600.jpg',0);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (1, N'Áo polo dài tay ', N'PHỐI CỔ vải da cá 4 chiều cotton 100%', 399000, 'Available', 1);

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(IDENT_CURRENT('Products'), N'M', N'Kem', 50);
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/12/z5965370668403_87a3544c8f0c1f3e4d6f5a19d5dfbccd-800x800.jpg',1),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/09/2d03f3ccbdf24a5db2eccd072aeb6464-1697106130290.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/09/c7da61bf3437b2550385858e9b8c4ea5-1697106130290.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/09/z4774217987319-5124d6d825868d67b0ac1776009a2681-1697106130297-600x600.jpg', 0);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (1, N'Áo polo ngắn tay ', N'phối dây kéo trẻ trung năng động ', 299000, 'Available', 1);

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(IDENT_CURRENT('Products'), N'M', N'Kem', 50),
(IDENT_CURRENT('Products'), N'M', N'Nâu', 50),
(IDENT_CURRENT('Products'), N'M', N'Trắng', 50),
(IDENT_CURRENT('Products'), N'M', N'Xám', 50);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/z5097977013871-efd0343f7ce51e1ac224d85cb2995359-1705987944036-1-800x800.jpeg',1),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/z5097976975705-1dfabaccba544040f3d5982a471bfc77-1705987915131-1-600x600.jpeg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/z5097976895124-b24cd01f9cc2c1c0e47cce7221fb284e-1705987922577-1-600x600.jpeg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/z5097976935048-27c5cc8d897bef223a4a4beda84939a0-1705987966628-1-600x600.jpeg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/z5315743109504-6d2774dfd0acc0c10f2ae65c7bf01d31-1712225401088-1-600x600.jpeg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/z5097976924407-4c41972483843a6f369f6010bd2c8408-1705987924550-1-600x600.jpeg', 0);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (1, N'Áo Sơ Mi Giấu Cúc ', N'Áo Dài Tay Nam Linen Premium Cool ', 399000, 'Available', 1);

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(IDENT_CURRENT('Products'), N'M', N'XÁM ĐẬM	', 50),
(IDENT_CURRENT('Products'), N'M', N'XÁM NHẠT', 50),
(IDENT_CURRENT('Products'), N'M', N'XANH DƯƠNG', 50);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/08/2-XAM-DAM-LD8185-800x800.jpg',1),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/08/1-XANH-DUONG-LD8185-800x800.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/08/3-XAM-NHAT-LD8185-600x600.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/08/ao-so-mi-nam-dai-tay-xanh-duong-lados-ld8185-600x600.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/08/ao-so-mi-linen-cool-dai-tay-nam-lados-ld8185-600x600.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/08/ao-so-mi-nam-dai-tay-phong-cach-toi-gian-lados-ld8185-600x600.jpg', 0);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (1, N'Áo Sơ Mi Nam Dài Tay Caro ', N'Áo Dài Tay Nam Trendy Look giúp bản thân toát lên sự quý phái thời thượng ', 399000, 'Available', 1);

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(IDENT_CURRENT('Products'), N'M', N'Xám', 50),
(IDENT_CURRENT('Products'), N'M', N'Nâu', 50);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/06/2-caro-den-ld8182-768x768.jpg',1),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/06/ao-so-mi-nam-caro-dep-gia-re-ld8182-768x1152.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/06/ao-so-mi-nam-caro-tre-trung-ld8180-768x1152.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/06/1-caro-nau-ld8182-768x768.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/06/ao-so-mi-nam-lados-ld8182-vai-tho-caro-mem-mai-600x600.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/06/ao-so-mi-nam-lados-ld8182-hoa-tiet-caro-thoi-trang-600x600.jpg', 0);

INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (1, N'Áo Sơ Mi Nam Dài Tay Kẻ Sọc', N'Kẻ Sọc Linen Hàn Quốc ', 399000, 'Available', 1);

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(IDENT_CURRENT('Products'), N'M', N'SỌC ĐEN', 50),
(IDENT_CURRENT('Products'), N'M', N'SỌC VÀNG', 50);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/05/1-soc-den-ld8155.jpg',1),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/05/ao-so-mi-nam-dang-dai-tay-nam-tinh-lados-ld8155-768x960.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/05/ao-so-mi-nam-ke-soc-lon-form-dep-lados-ld8155-768x960.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/05/ao-so-mi-nam-form-tre-trung-hien-dai-lados-ld8155-768x960.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/05/1-soc-den-ld8155-600x600.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/05/2-soc-vang-ld8155-768x768.jpg', 0);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (1, N'Áo thun cổ tròn IN LỤA', N'Áo thun cổ tròn IN LỤA form relaxfit THE RUNNING BOY ', 199000, 'Available', 1);

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(IDENT_CURRENT('Products'), N'M', N'Đen', 50),
(IDENT_CURRENT('Products'), N'M', N'kem', 50);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/08/z4863134894086_466c49321dc3485faaa5a11f27055d18.jpg',0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/08/z4863134907350_dce13b415d258564fdd6a8229b052464-800x801.jpg', 1);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (1, N'Áo thun cổ V', N'Áo thun cổ V vải tổ ong nhí', 399000, 'Available', 1);

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(IDENT_CURRENT('Products'), N'M', N'Đen', 50),
(IDENT_CURRENT('Products'), N'M', N'Rêu', 50),
(IDENT_CURRENT('Products'), N'M', N'kem', 50);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/z5460717877329-60f82b31132752cff5562d8af6b09b19-1716349628596-2-800x800.jpeg',1),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/z5460717720454-d1c9a466b71f5f2684216d1445fc6e64-1716349605671-2-800x800.jpeg',0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/06/1-KEM-LD9173.jpg', 2),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/z5484080849150-09ea1c49bb6f87166e7d6576ababec8e-768x1022.jpeg',0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/z5460717624227-d621f9ccabd18db8aa0b9ab3f49a50e1-1716349639478-1-600x600.jpeg',0);



INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (1, N'Áo Thun Nam Cổ Tròn', N'Áo Thun Nam Cổ Tròn Tay Ngắn Basic Raglan LADOS ',299000, 'Available', 1);

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(IDENT_CURRENT('Products'), N'M', N'Đen', 50),
(IDENT_CURRENT('Products'), N'M', N'Nâu nhạt', 50),
(IDENT_CURRENT('Products'), N'M', N'Xanh dương', 50);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/09/3-den-ld9219.jpg',1),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/09/4-nau-nhat-ld9219-800x800.jpg',0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/08/2-xanh-duong-ld9219-800x800.jpg',0);



-----------------------------------------------------
-- Áo nữ
INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (2, N'Áo sơ mi nữ công sở', N'Áo sơ mi nữ dài tay, chất liệu lụa cao cấp', 329000, 'Available', 1);

DECLARE @P2 INT = SCOPE_IDENTITY();
INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P2, N'S', N'Trắng', 20),
(@P2, N'M', N'Xanh dương', 15);
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P2, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af07024/2-trang-af07024.jpg', 1),
(@P2, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af07024/1-trang-af07024.png', 0),
(@P2, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af07024/4-trang-af07024.jpg', 0),
(@P2, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af07024/5-trang-af07024.jpg', 0),
(@P2, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af07024/6-trang-af07024.jpg', 0),
(@P2, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af07022/1-xanh-af07022-1.png', 0),
(@P2, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af07022/3-xanh-af07022-1.jpg', 0),
(@P2, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af07022/5-xanh-af07022-1.jpg', 0),
(@P2, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af07022/7-xanh-af07022-1.jpg', 0);

-----------------------------------------------------
-- Áo nữ 2
INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (2, N'Áo thun nữ tay ngắn', N'Áo thun nữ cotton co giãn 4 chiều, thấm hút mồ hôi tốt', 199000, 'Available', 1);

DECLARE @P3 INT = SCOPE_IDENTITY();
INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P3, N'S', N'Hồng', 25),
(@P3, N'M', N'Đen', 30),
(@P3, N'L', N'Đỏ', 20);
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P3, N'https://image.hm.com/assets/hm/24/c8/24c87c8cf4a0c8cc6c1edcda8b275f6507fa1e2b.jpg?imwidth=1260', 1),
(@P3, N'https://image.hm.com/assets/hm/ac/52/ac524bf19b222dae50455e7f326744e72f99ad1e.jpg?imwidth=1260', 0),
(@P3, N'https://image.hm.com/assets/hm/a1/95/a1950152b9f76ee91387e45e804f2ea2aeff3fec.jpg?imwidth=1260', 0);



-----------------------------------------------------

-- Áo nữ 3
INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (2, N'Áo kiểu nữ tay phồng', N'Áo kiểu nữ cổ vuông tay phồng, phong cách Hàn Quốc', 289000, 'Available', 1);

DECLARE @P4 INT = SCOPE_IDENTITY();
INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P4, N'S', N'Trắng', 18),
(@P4, N'M', N'Kem', 22);
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P4, N'https://image.hm.com/assets/hm/9e/f8/9ef8bdb7402e16bf5e32d505811bfa48c09ec002.jpg?imwidth=1260', 1),
(@P4, N'https://image.hm.com/assets/hm/c8/f5/c8f5ce950ba26372e3a30d6a44c394c027c275c6.jpg?imwidth=1260', 0),
(@P4, N'https://image.hm.com/assets/hm/e5/17/e51762c2bf7f58261c74e44056fbe96e84756dc6.jpg?imwidth=1260', 0),
(@P4, N'https://image.hm.com/assets/hm/da/7f/da7f8c1fa945153783369409f8a392f7fa57a7ef.jpg?imwidth=1260', 0);


-----------------------------------------------------
-- Áo nữ 4
INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (2, N'Áo khoác nữ bomber', N'Áo khoác nữ bomber vải kaki, phong cách cá tính', 459000, 'Available', 0);

DECLARE @P5 INT = SCOPE_IDENTITY();
INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P5, N'M', N'Xanh dương nhạt', 12),
(@P5, N'L', N'Nâu đậm', 10);
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P5, N'https://image.hm.com/assets/hm/20/8a/208a45e43e32f9b292e761f198f531100067c1cc.jpg?imwidth=1260', 1),
(@P5, N'https://image.hm.com/assets/hm/8b/77/8b771db1b75e6ea1af7735533e52923ce7281c43.jpg?imwidth=1260', 0),
(@P5, N'https://image.hm.com/assets/hm/df/3c/df3c9f3c6e022a594c37a1419e823274ea48b456.jpg?imwidth=1260', 0),
(@P5, N'https://image.hm.com/assets/hm/d2/52/d252d4c9b76d4dfd4447ff9a6962ede8f8554235.jpg?imwidth=1260', 0),
(@P5, N'https://image.hm.com/assets/hm/68/88/688810988663633408f05342176111fdbe7a3d44.jpg?imwidth=1260', 0);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (2, N'Áo sơ mi nhún eo', N'Áo sơ mi nhún eo đẹp', 395000, 'Available', 0);

DECLARE @P6 INT = SCOPE_IDENTITY();
INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P6, N'L', N'Trắng', 10);
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P6, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af08009/2-trang-af08009-1.jpg', 1),
(@P6, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af08009/3-trang-af08009-1.jpg', 0),
(@P6, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af08009/4-trang-af08009-1.jpg', 0),
(@P6, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af08009/5-trang-af08009.jpg', 0),
(@P6, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af08009/2.jpg', 0),
(@P6, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af08009/7-trang-af08009.jpg', 0);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (2, N'Áo kiểu túi đắp', N'Áo kiểu nữ đẹp', 450000, 'Available', 0);

DECLARE @P7 INT = SCOPE_IDENTITY();
INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P7, N'L', N'Trắng', 10);
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P7, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af07037/2-trang-af07037.jpg', 1),
(@P7, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af07037/3-trang-af07037.jpg', 0),
(@P7, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af07037/4-trang-af07037.jpg', 0),
(@P7, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af07037/5-trang-af07037.jpg', 0),
(@P7, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af07037/6-trang-af07037.jpg', 0),
(@P7, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af07037/7-trang-af07037.jpg', 0);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (2, N'ASM cơ bản', N'Bao đẹp', 440000, 'Available', 0);

DECLARE @P8 INT = SCOPE_IDENTITY();
INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P8, N'L', N'Hồng', 10);
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P8, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af04053/2-hong-af04053-1.jpg', 1),
(@P8, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af04053/3-hong-af04053-1.jpg', 0),
(@P8, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af04053/4-hong-af04053-1.jpg', 0),
(@P8, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af04053/5-hong-af04053-1.jpg', 0),
(@P8, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af04053/6-hong-af04053-1.jpg', 0),
(@P8, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af04053/7-hong-af04053-1.jpg', 0);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (2, N'Áo somi phối ren', N'Bao đẹp', 330000, 'Available', 0);

DECLARE @P9 INT = SCOPE_IDENTITY();
INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P9, N'L', N'Be', 10);
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P9, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af03040/2-be-af03040.jpg', 1),
(@P9, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af03040/3-be-af03040.jpg', 0),
(@P9, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af03040/4-be-af03040.jpg', 0),
(@P9, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af03040/5-be-af03040.jpg', 0),
(@P9, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af03040/6-be-af03040.jpg', 0);

INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (2, N'Áo somi kiểu tay lật', N'Bao đẹp', 390000, 'Available', 0);

DECLARE @P10 INT = SCOPE_IDENTITY();
INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P10, N'L', N'Xanh', 10);
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P10, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af03019/2-xanh-af03019-1.jpg', 0),
(@P10, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af03019/3-xanh-af03019-1.jpg', 0),
(@P10, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af03019/4-xanh-af03019-1.jpg', 0),
(@P10, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af03040/5-be-af03040.jpg', 0),
(@P10, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af03019/6-xanh-af03019-1.jpg', 0);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (2, N'Áo blazer tay ngắn', N'Bao đẹp', 559000, 'Available', 0);

DECLARE @P11 INT = SCOPE_IDENTITY();
INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P11, N'L', N'Be', 10);
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P11, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/owf05043/3-be-owf05043-1.jpg', 0),
(@P11, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/owf05043/2-be-owf05043-1.jpg', 0),
(@P11, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/owf05043/5-be-owf05043-1.jpg', 0),
(@P11, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/owf05043/3.jpg', 0),
(@P11, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/owf05043/4.jpg', 0);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (2, N'Áo somi kiểu cổ nhọn', N'Bao đẹp', 299000, 'Available', 0);

DECLARE @P12 INT = SCOPE_IDENTITY();
INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P12, N'L', N'Hồng', 10);
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P12, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af04007/2-tim-af04007.jpg', 0),
(@P12, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af04007/3-tim-af04007.jpg', 0),
(@P12, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af04007/4-tim-af04007.jpg', 0),
(@P12, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af04007/5-tim-af04007.jpg', 0),
(@P12, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/af04007/6-tim-af04007.jpg', 0);



-----------------------------------------------------
-----------------------------------------------------
-- Quần jeans nam slimfit
INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (3, N'Quần jeans nam slimfit', N'Quần jeans nam dáng ôm, phong cách trẻ trung', 459000, 'Available', 0);

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(IDENT_CURRENT('Products'), N'30', N'Xanh đậm', 25),
(IDENT_CURRENT('Products'), N'32', N'Đen', 20);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(IDENT_CURRENT('Products'), N'https://n7media.coolmate.me/uploads/December2024/quan-jeans-nam-basics-dang-regular-straight-xanh-sang-1.jpg?aio=w-1100', 1),
(IDENT_CURRENT('Products'), N'https://n7media.coolmate.me/uploads/December2024/quan-jeans-nam-basics-dang-regular-straight-den-1.jpg?aio=w-1100', 0);


-----------------------------------------------------
-- Quần jeans nam ống đứng
INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (3, N'Quần jeans nam ống đứng', N'Quần jeans nam ống đứng, dễ phối đồ, chất liệu denim cao cấp', 499000, 'Available', 1);

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(IDENT_CURRENT('Products'), N'30', N'Xanh nhạt', 18),
(IDENT_CURRENT('Products'), N'32', N'Xanh đậm', 20),
(IDENT_CURRENT('Products'), N'34', N'Đen', 15);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(IDENT_CURRENT('Products'), N'https://cbu01.alicdn.com/img/ibank/O1CN01UqvjEZ1s6tgJpwFc9_!!2214315815718-0-cib.jpg', 1),
(IDENT_CURRENT('Products'), N'https://cbu01.alicdn.com/img/ibank/O1CN01cOKk9l1s6tgI7ML4R_!!2214315815718-0-cib.jpg', 0),
(IDENT_CURRENT('Products'), N'https://cbu01.alicdn.com/img/ibank/O1CN01E1CFJK1s6tgLDW69U_!!2214315815718-0-cib.jpg', 0),
(IDENT_CURRENT('Products'), N'https://yeepvn.sgp1.digitaloceanspaces.com/2023/05/d2c4949b785c788c5cbab89d113df352.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/12/quan-jean-nam-mau-den-ca-tinh-lados-ld4142.jpg', 0);


-----------------------------------------------------
-- Quần jeans nam jogger
INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (3, N'Quần jeans nam jogger', N'Quần jeans jogger co giãn, phối dây rút tiện lợi, thoải mái vận động', 479000, 'Available', 1);

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(IDENT_CURRENT('Products'), N'M', N'Xanh nhạt', 14),
(IDENT_CURRENT('Products'), N'L', N'Xanh đậm', 17),
(IDENT_CURRENT('Products'), N'XL', N'Đen', 10);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/08/1-907-ld4184-800x800.jpg', 1),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/08/quan-jean-nam-sieu-nhe-ld4184-600x600.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/08/quan-jean-sieu-co-gian-ld4184-600x600.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/08/quan-jean-nam-chat-luong-cao-ld4184-600x600.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2025/08/quan-jean-nam-denim-cao-cap-lados-4184-600x600.jpg', 0);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (3, N'Quần short jean nam LƯNG THUN', N'Quần short jean nam LƯNG THUN CÓ DÂY RÚT cao cấp', 279000, 'Available', 1);

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(IDENT_CURRENT('Products'), N'L', N'Xanh đậm', 17),
(IDENT_CURRENT('Products'), N'XL', N'Xanh rêu', 10);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/cdd6b851db85d942ae6286d5f3e1a08a-1695283465968.jpeg', 1),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/b2e1e9cc106f8590f0a90451f343ca7a-1695283467879.jpeg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/7c914203310122b3487c656bd8b20bfe-1695283459523.jpeg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/1b04e7f0b0618f229be674b856a91c18-1695283499717.jpeg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/7ac0daff67b3d870c7c4d47fb5676ccc-1695283507396.jpeg', 0);



INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (3, N'Quần jean nam FORM SLIMFIT ', N'Quần  jean nam bao đẹp', 279000, 'Available', 1);

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(IDENT_CURRENT('Products'), N'L', N'Xanh đậm', 17);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/2-802A-LD4111-800x800.jpg', 1),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/quan-jean-nam-basic-slimfit-thoi-trang-lados-ld4111-600x600.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/quan-jean-nam-dang-om-tre-trung-lados-ld4111-600x600.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/quan-jean-nam-khong-xu-khong-nhan-lados-ld4111-600x600.jpg', 0),
(IDENT_CURRENT('Products'), N'https://lados.vn/wp-content/uploads/2024/07/quan-jean-slimfit-nam-form-dep-lados-ld4111-600x600.jpg', 0);


-----------------------------------------------------
-- Quần nữ
INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (4, N'Quần tây nữ ống suông', N'Quần tây nữ công sở, chất liệu vải cao cấp', 389000, 'Available', 1);
DECLARE @P40 INT = SCOPE_IDENTITY();

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P40, N'S', N'Đen', 12),
(@P40, N'M', N'Nâu', 8);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P40, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf08024/2-den-qf08024.jpg', 1),
(@P40, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf08024/2-den-qf08024.jpg', 0),
(@P40, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf08024/4-den-qf08024.jpg', 0),
(@P40, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf08024/1.jpg',0),
(@P40, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf08024/2.jpg', 0),
(@P40, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf08024/2-nau-qf08024-1.jpg', 0),
(@P40, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf08024/4-nau-qf08024-1.jpg', 0),
(@P40, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf08024/5-nau-qf08024.jpg', 0),
(@P40, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf08024/6-nau-qf08024.jpg', 0),
(@P40, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf08024/3.jpg', 0);




-----------------------------------------------------
-- Quần nữ 1: Quần short nữ
INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (4, N'Quần ống rộng chiết ly', N'Quần short nữ lưng cao, phong cách năng động mùa hè', 259000, 'Available', 1);
DECLARE @P41 INT = SCOPE_IDENTITY();

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P41, N'L', N'Đen', 12);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P41, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf08001/2-den-qf08001-1.jpg', 1),
(@P41, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf08001/3-den-qf08001-1.jpg', 0),
(@P41, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf08001/4-den-qf08001-1.jpg', 0),
(@P41, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf08001/6-den-qf08001.jpg', 0),
(@P41, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf08001/1.jpg', 0);

-----------------------------------------------------
-- Quần nữ 2: Quần jeans nữ
INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (4, N'Quần jeans ống suông', N'Quần jeans nữ dáng skinny, rách nhẹ cá tính', 389000, 'Available', 0);
DECLARE @P42 INT = SCOPE_IDENTITY();

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P42, N'27', N'Xanh đậm', 14);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P42, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qjf07042/2-xanh-qjf07042.jpg', 1),
(@P42, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qjf07042/3-xanh-qjf07042.jpg', 0),
(@P42, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qjf07042/4-xanh-qjf07042.jpg', 0),
(@P42, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qjf07042/3-xdam-qjf07042.jpg', 0),
(@P42, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qjf07042/6-xdam-qjf07042.jpg', 0),
(@P42, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qjf07042/5-xdam-qjf07042.jpg', 0);

-----------------------------------------------------
-- Quần nữ 3: Quần jogger nữ
INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (4, N'Quần baggy nút kiểu', N'Quần jogger nữ vải thun co giãn, phù hợp tập gym, chạy bộ', 299000, 'Available', 1);
DECLARE @P43 INT = SCOPE_IDENTITY();

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P43, N'S', N'Xám', 20);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P43, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf06009/3-den-qf06009.jpg', 1),
(@P43, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf06009/2-den-qf06009.jpg', 0),
(@P43, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf06009/5-den-qf06009.jpg', 0),

(@P43, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf06009/8-den-qf06009.jpg', 0),
(@P43, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf06009/1.jpg', 0),
(@P43, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf06009/7-den-qf06009.jpg', 0);


-----------------------------------------------------
-- Áo nữ 4
INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (2, N'Quần tây ống đứng', N'Bao đẹp', 459000, 'Available', 0);

DECLARE @P44 INT = SCOPE_IDENTITY();
INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P44, N'L', N'đen', 10);
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P44, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf07021/2-den-qf07021.jpg', 1),
(@P44, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf07021/3-den-qf07021.jpg', 0),
(@P44, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf07021/4-den-qf07021.jpg', 0),	
(@P44, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf07021/5-den-qf07021.jpg', 0);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (2, N'Quần ống đứng kèm dây', N'Bao đẹp', 530000, 'Available', 0);

DECLARE @P46 INT = SCOPE_IDENTITY();
INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P46, N'L', N'Đen', 10);
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P46, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf06047/2-den-qf06047.jpg', 1),
(@P46, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf06047/3-den-qf06047.jpg', 0),
(@P46, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf06047/4-den-qf06047.jpg', 0),
(@P46, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf06047/5-den-qf06047.jpg', 0),
(@P46, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf06047/6-den-qf06047.jpg', 0);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (2, N'Quần jeans kiểu ống rộng', N'Bao đẹp', 559000, 'Available', 0);

DECLARE @P47 INT = SCOPE_IDENTITY();
INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P47, N'L', N'Xanh', 10);
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P47, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qjf04031/2-xanh-qjf04031-1.jpg', 1),
(@P47, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qjf04031/3-xanh-qjf04031-1.jpg', 0),
(@P47, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qjf04031/5-xanh-qjf04031-1.jpg', 0),
(@P47, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qjf04031/1.jpg', 0),
(@P47, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qjf04031/6-xanh-qjf04031.jpg', 0);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (2, N'Quần ống rộng chiết ly', N'Bao đẹp', 690000, 'Available', 0);

DECLARE @P48 INT = SCOPE_IDENTITY();
INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P48, N'L', N'Be', 10);
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P48, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf05049/2-be-qf05049.jpg', 1),
(@P48, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf05049/3-be-qf05049.jpg', 0),
(@P48, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf05049/4-be-qf05049.jpg', 0),
(@P48, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf05049/5-be-qf05049.jpg', 0),
(@P48, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf05049/6-be-qf05049.jpg', 0),
(@P48, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf05049/2.jpg', 0);


INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (2, N'Quần tây thêu lưng', N'Bao đẹp', 699000, 'Available', 0);

DECLARE @P49 INT = SCOPE_IDENTITY();
INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P49, N'L', N'Be', 10);
INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P49, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf04052/3-den-qf04052.jpg', 1),
(@P49, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf04052/2-den-qf04052.jpg', 0),
(@P49, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf04052/4-den-qf04052.jpg', 0),
(@P49, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf04052/5-den-qf04052.jpg', 0),
(@P49, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/qf04052/6-den-qf04052.jpg', 0);



INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (4, N'Quần tây ống loe B3', N'Bao đẹp', 259000, 'Available', 1);
DECLARE @P50 INT = SCOPE_IDENTITY();

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P50, N'S', N'Xanh', 20);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P50, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/bqe00006/2-xam-bqe00006-1.jpg', 1),
(@P50, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/bqe00006/3-xam-bqe00006-1.jpg', 0),
(@P50, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/bqe00006/4-xam-bqe00006-1.jpg', 0),
(@P50, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/bqe00006/5-xam-bqe00006-1.jpg', 0),
(@P50, N'https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/gumac3/bqe00006/6-xam-bqe00006-1.jpg', 0);

-----------------------------------------------------
-- Giày dép
INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (5, N'Giày sneaker nam', N'Giày sneaker phong cách Hàn Quốc', 599000, 'Available', 1);
DECLARE @P15 INT = SCOPE_IDENTITY();

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P15, N'41', N'Trắng', 20),
(@P15, N'42', N'Đen', 15);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P15, N'https://product.hstatic.net/1000008082/product/f53_94ce2d0dcfe9434d9042d4466cde7b98_master.jpg', 1),
(@P15, N'https://www.caoto.vn/images/vans/81txgcfbvpl._ac_ux695_.jpg', 0);

-----------------------------------------------------
-- Túi xách & Balo
INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (6, N'Túi xách nữ da PU', N'Túi xách nữ cao cấp, chất liệu da PU mềm mại', 459000, 'Available', 0);
DECLARE @P16 INT = SCOPE_IDENTITY();

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P16, N'Free', N'Nâu', 10),
(@P16, N'Free', N'Đen', 8);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P16, N'https://www.charleskeith.vn/dw/image/v2/BCWJ_PRD/on/demandware.static/-/Sites-vn-products/default/dwa74d9079/images/hi-res/2023-L7-CK2-60681127-29-1.jpg?sw=756&sh=1008', 1),
(@P16, N'https://www.charleskeith.vn/dw/image/v2/BCWJ_PRD/on/demandware.static/-/Sites-vn-products/default/dwb1daeddf/images/hi-res/2023-L7-CK2-60681127-J8-1.jpg?sw=756&sh=1008', 0);

-----------------------------------------------------
-- Phụ kiện (đồng hồ)
INSERT INTO Products (CategoryID, ProductName, Description, Price, Status, IsFeatured)
VALUES (7, N'Đồng hồ nam dây da', N'Đồng hồ nam cao cấp, dây da thật', 1599000, 'Available', 1);
DECLARE @P27 INT = SCOPE_IDENTITY();

INSERT INTO ProductVariants (ProductID, Size, Color, Stock) VALUES
(@P27, N'Free', N'Nâu', 8),
(@P27, N'Free', N'Đen', 6);

INSERT INTO ProductImages (ProductID, ImageURL, IsPrimary) VALUES
(@P27, N'https://cdn.vuanhwatch.com/media/products/21539/dong-ho-nam-daniel-wellington-dw00100128-mau-nau-den-63997fba1f45d-14122022144810.jpg', 1),
(@P27, N'https://cdn.vuanhwatch.com/media/products/21539/dong-ho-nam-daniel-wellington-dw00100128-mau-nau-den-63997fba1153a-14122022144810.jpg', 1),
(@P27, N'https://cdn.vuanhwatch.com/media/products/21539/dong-ho-nam-daniel-wellington-dw00100128-mau-nau-den-63997fba173dd-14122022144810.jpg', 1),
(@P27, N'https://product.hstatic.net/200000597439/product/fs6078-3p-1_f31dfd1fe1ec4f7aa349da7639f01737_1024x1024.jpg', 0),
(@P27, N'https://product.hstatic.net/200000597439/product/fs6078-3p-19_d290119d2e1d4f19850ec232f0c6478f_1024x1024.jpg', 0),
(@P27, N'https://product.hstatic.net/200000597439/product/fs6078-3p-7_2b70c040773c40cd8d1050f48aa56371_1024x1024.jpg', 0),
(@P27, N'https://product.hstatic.net/200000597439/product/fs6078-3p-6_5e838ef9de104332a8796a0b0f1cd2a1_1024x1024.jpg', 0);


