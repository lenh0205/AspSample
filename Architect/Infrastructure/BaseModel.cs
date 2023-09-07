using SER.Domain.Entities.SqlServerCCKL;
using System.Text.Json.Serialization;

/// <summary>
/// Base Domain Model
/// </summary>
public class BaseEntity
{
    public Guid Id { get; set; }
    public int CreatedUserId { get; set; }
    public DateTime? CreatedDate { get; set; } = DateTime.Now;
    public int UpdatedUserId { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; } = false;
}


/// <summary>
/// Base View Model
/// </summary>
public class PaginationRequest
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}
public partial class CreatedRequest<TUser>
{
    public CreatedRequest(TUser createdUserId)
    {
        CreatedUserId = createdUserId;
    }
}
public partial class CreatedRequest<TUser>
{
    public TUser CreatedUserId { get; set; }
    [JsonIgnore]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

}
public partial class UpdatedRequest<TKey, TUser>
{
    public UpdatedRequest(TKey id, TUser updatedUserId)
    {
        Id = id;
        UpdatedUserId = updatedUserId;
    }
}
public partial class UpdatedRequest<TKey, TUser>
{
    public TKey Id { get; set; }
    public TUser UpdatedUserId { get; set; }

    [JsonIgnore]
    public DateTime UpdatedDate { get; set; } = DateTime.Now;
}

/// <summary>
/// Request View Model
/// </summary>
public class HoSoCongViecRequestBase
{
    public string? MaHoSo { get; set; } = string.Empty; //Mã hồ sơ
    public string? MaCoQuan { get; set; } = string.Empty; //Mã định danh cơ quan
    public IEnumerable<SER.Domain.Entities.SqlServerCCKL.TepDinhKem>? TepDinhKems { get; set; }
    // ....
}

public class HoSoCongViecCreateRequest : HoSoCongViecRequestBase
{
    [JsonIgnore]
    public DateTime? CreatedDate { get; set; } = DateTime.Now;
}
public class HoSoCongViecUpdateRequest : HoSoCongViecRequestBase
{
    public Guid Id { get; set; }
    [JsonIgnore]
    public DateTime? UpdatedDate { get; set; } = DateTime.Now;
}
public class HoSoCongViecBySearchRequest : PaginationRequest
{
    public int NamHinhThanh { get; set; } = 0;
    // ...
}

/// <summary>
/// For Response View Model
/// </summary>
public class HoSoCongViecResponse
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    //.....
}
