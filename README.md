từ file SQLQuery1 chạy trước

sau đó chạy dotnet build check có lỗi không

sau đó chạy dotnet watch run

Đăng ký 1 account vào DB update Role="Admin", lúc này sẽ run được UI của User, Admin

Điều chỉnh appsetting.json -> ở ConnectionStrings: {
"OnlineStoreDB: "phù hợp với SQL Server"
}
