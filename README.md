# 🕍 Website giới thiệu lễ hội người Khmer

Website giới thiệu và tra cứu thông tin các lễ hội của người Khmer, xây dựng bằng ASP.NET Core và Entity Framework Core.

---

## 🧩 Công nghệ sử dụng

- ASP.NET Core **5.0** (TFM: `net5.0`)
- Entity Framework Core **5.0.0**
- SQL Server / SQL Express / LocalDB
- Bootstrap 5

---

## 🖥️ Yêu cầu hệ thống

- **Hệ điều hành**: Windows 10 x64 (hoặc mới hơn)
- **.NET SDK**: khuyến nghị **.NET 5.0.x**

  Kiểm tra bằng:

  ```bash
  dotnet --list-sdks
  ```

- **Database**:
  - SQL Server / SQL Server Express **hoặc**
  - LocalDB (thường có sẵn khi cài Visual Studio / SQL Express)
- (Khuyến nghị) Cài thêm:
  - SQL Server Management Studio (SSMS) – để quản lý database
  - Git – để clone repository

---

## 📦 Cài đặt nhanh

1. **Clone source**

   ```bash
   git clone <link-repo-github>
   cd <thu-muc-repo>
   ```

2. **Chỉnh chuỗi kết nối SQL Server trong `appsettings.json`**

3. **Chạy migration để tạo database + bảng**

4. **Chạy ứng dụng web**

   ```bash
   dotnet run --project scr/app/KhmerFestival.Web/KhmerFestival.Web.csproj
   ```

5. **Mở trình duyệt**

   - `https://localhost:5001/` (HTTPS), hoặc
   - `http://localhost:5000/` (HTTP), tuỳ cấu hình `launchSettings.json`

---

## 🔧 Cấu hình kết nối SQL Server

File cấu hình nằm tại:

`scr/app/KhmerFestival.Web/appsettings.json`

Nội dung mẫu (ban đầu):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DESKTOP-Q9GD575\\MSSQLSERVER01;Database=khemer_festival;Trusted_Connection=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

👉 Khi chạy trên máy khác, **bắt buộc phải đổi** giá trị `"Server=..."` (và nếu cần thì `"Database=..."`) cho phù hợp với SQL Server trên máy của bạn.

### 🎯 Một số ví dụ connection string

#### 1. Dùng LocalDB (dev nhanh, nhẹ)

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=khmer_festival;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

#### 2. Dùng SQL Server / SQL Express + Windows Authentication

Ví dụ instance là `.\SQLEXPRESS`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=khmer_festival;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

#### 3. Dùng SQL Server với tài khoản SQL (user/password)

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=khmer_festival;User Id=sa;Password=MatKhauCuaBan;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

> 💡 Gợi ý:
> - Mở **SQL Server Management Studio (SSMS)** → màn hình *Connect to Server* → copy đúng phần **Server name** dán vào chuỗi kết nối.
> - Bạn có thể đổi tên database (`Database=...`) nếu muốn, miễn là trùng với DB bạn dùng.

---

## 🗃️ Tạo database & chạy migration (EF Core)

Project dùng **Entity Framework Core 5.0.0**, có hỗ trợ **migrations** và **seeder** để đổ dữ liệu mẫu vào database.

### 1️⃣ Cài tool `dotnet-ef` (nếu chưa có)

```bash
dotnet tool install --global dotnet-ef
# hoặc nếu đã cài thì:
dotnet tool update --global dotnet-ef
```

### 2️⃣ Chạy migration & tạo database

Đứng tại thư mục root của repo (nơi chứa folder `scr/`), chạy:

#### Trường hợp A – ĐÃ có sẵn folder `Migrations` trong project

Chỉ cần:

```bash
dotnet ef database update --project scr/app/KhmerFestival.Web/KhmerFestival.Web.csproj
```

Lệnh này sẽ:

- Tạo database (nếu chưa có)
- Tạo bảng theo model
- Seeder (nếu đã được cấu hình trong code) sẽ tự động insert dữ liệu mẫu khi chạy ứng dụng.

#### Trường hợp B – CHƯA có migration nào (không thấy folder `Migrations`)

Tạo migration đầu tiên:

```bash
dotnet ef migrations add InitialCreate --project scr/app/KhmerFestival.Web/KhmerFestival.Web.csproj
dotnet ef database update --project scr/app/KhmerFestival.Web/KhmerFestival.Web.csproj
```

---

## 🚀 Chạy ứng dụng

1. **Build**:

   ```bash
   dotnet build
   ```

2. **Chạy web**:

   ```bash
   dotnet run --project scr/app/KhmerFestival.Web/KhmerFestival.Web.csproj
   ```

3. **Truy cập website**:

   - `https://localhost:5001/`
   - hoặc `http://localhost:5000/`

---

## 📂 Cấu trúc thư mục

- `scr/app/KhmerFestival.Web/` – Mã nguồn chính của website (ASP.NET Core 5)
- `progress-report/` – Báo cáo tiến độ hàng tuần
- `thesis/` – Tài liệu đồ án (báo cáo, luận văn,…)
- `setup/` – Hướng dẫn setup, dữ liệu test (nếu có)
- `docker/` – File triển khai bằng Docker (nếu có)
- `soft/` – Phần mềm liên quan

---

## 🛠️ Một số lỗi thường gặp

- ❌ **Không kết nối được database** (`SqlException`, `Cannot open database ...`):
  - Kiểm tra lại `ConnectionStrings:DefaultConnection` trong `appsettings.json`
  - Kiểm tra `Server`, `Database`, cách đăng nhập (Windows / SQL Auth)

- ❌ **Không tìm thấy lệnh `dotnet-ef`**:
  - Chạy `dotnet tool install --global dotnet-ef`
  - Đóng mở lại terminal / PowerShell

- ❌ **Lỗi version .NET**:
  - Kiểm tra `dotnet --list-sdks`
  - Đảm bảo có SDK **5.0.x** hoặc tương thích với `net5.0`

---

## 👤 Tác giả

- **Họ tên**: Trần Thị Hồng Nhung  
- **Lớp**: DK24TTC2  
- **Email**: nhungtth040591@tvu-onschool.edu.vn  

---

## 📫 Góp ý & liên hệ

Nếu gặp lỗi trong quá trình setup hoặc chạy dự án, bạn có thể:

- Liên hệ qua email tác giả  
- Hoặc tạo issue trên repository (nếu dự án được đưa lên GitHub/GitLab)
