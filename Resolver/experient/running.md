# nvm 
## Install
* Vô Github -> chọn "Download" (ngay trên cùng)
## Không thể sử dụng "npm":
* **`nvm on`**
* Hoặc update the **environment variables** to set the **PATH** to where node.js is installed

# Code First
## Các lý do không Add-Migration được
* **Thiếu thư viện**: Entity Framework Core (`Design, SQL Server, Tool`)
* `Tất cả Project trong Solution` đểu phải **build success** hết (nếu có project bị lỗi có thể Remove nó)
* Kiểm tra **ConnectionString**

## Migration bị lỗi do không đồng nhất với database
```
Ví dụ Migration cần drop 1 Table nhưng Table đó không tồn tại trong Database
```

* Đầu tiên check xem `migration` trong **`thư mục Migration`** và **`dbo._EFMigrationsHistory`** có đồng nhất với nhau không

* Cần thiết thì xoá Migration trong **thư mục Migration** đi

* **`Rollback Migration`** EntityFramework Core: **update-database -Migration <migration ta muốn>**
(_với EntityFramework là: `update-database -TargetMigration <migration ta muốn>`_)

# Khi chỉnh sửa 1 domain model:
* Sửa lại RequestModel, Model, Mapping Profile, fluent API
* Trước tiên là cứ dùng lệnh **`Add-Migration <commit>`**
* Nhớ lại ta đã chỉnh sửa gì trong model, chỉ giữ lại những thay đổi đó trong Migration còn lại xoá hết

# Join relationship Table:
* Trong trường hợp Bảng 1 có Record thoã mãn "Where()", nhưng Bảng 2 không tồn tại Record link đến Bảng 1; nhưng ta vẫn luốn lấy ra những dữ liệu thoã mãn, còn dữ liệu không thoả mãn thì ta cho giá trị mặc định
```
var qHoSoCongViec = _context.HoSoCongViecs.Where(x => !x.IsDeleted).AsQueryable();
var qDanhSachHoSo = _context.DanhSachHoSos.Include(x => x.HoSoCongViec).AsQueryable();

qDanhSachHoSo = qDanhSachHoSo.Where(_ => _.LoaiDanhSachHoSo == request.LoaiDanhSachHoSo);
if (request.NguoiLapId > 0) qHoSoCongViec = qHoSoCongViec.Where(_ => _.NguoiLapId == request.NguoiLapId);

var result = from hoSoCongViec in qHoSoCongViec
                join hoSo in qDanhSachHoSo on hoSoCongViec.Id equals hoSo.HoSoCongViecID into hoSoGroup
                from hoSo in hoSoGroup.DefaultIfEmpty()
                join user in _context.UserMasters on hoSo.HoSoCongViec.NguoiLapId equals user.UserMasterId into hoSo_user
                from user in hoSo_user.DefaultIfEmpty()
                select new
                {
                    hoSoCongViec.Id,
                    Status = hoSo.Status ?? HoSoStatus.KhoiTao,
                };

return await result.ToListAsync();
```

# Database First:
* Inside Package Manage Console:
```
Scaffold-DbContext 'Server=192.168.1.3\\sql2k16,1436;Initial Catalog=VI_QLVB;user=vietinfo;password=Vietinfo@#@!;TrustServerCertificate=True;' Microsoft.EntityFrameworkCore.SqlServer -Tables DM_DONVI_NGUOIDUNG -Context ApplicationDbContext -OutputDir "Domain/Entities/SqlServerCCKL" -ContextDir "Infrastructure/DatabaseFirst"
```

