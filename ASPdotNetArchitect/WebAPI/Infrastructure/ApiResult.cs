using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace SER.ViewModel;

public class ResultApi
{
    [JsonProperty("result")]
    public bool Result { get; set; } = true;

    [JsonProperty("errorMessage")]
    public string? ErrorMessage { get; set; } = String.Empty;

    [JsonProperty("dataResult")]
    public object? DataResult { get; set; } = null!;
    public ResultApi()
    {
    }

    // Status 200
    public ResultApi(string msg)
    {
        Result = false;
        ErrorMessage = msg;
    }
    public ResultApi(object? Data)
    {
        DataResult = Data;
    }

    // catch Exception for Bad Request
    public ResultApi(Exception ex)
    {
        Result = false;
        if (ex.InnerException is SqlException sqlEx)
        {
            switch (sqlEx.Number)
            {
                case 251:
                    {
                        ErrorMessage = "Sai kiểu dữ liệu";
                        break;
                    }
                case 515:
                    {
                        ErrorMessage = "Giá trị không được để trống";
                        break;
                    }
                case 547:
                    {
                        ErrorMessage = "Khoá ngoại không hợp lệ";
                        break;
                    }
                case 2601:
                    {
                        ErrorMessage = "Khóa chính đã tồn tại";
                        break;
                    }
                case 2627:
                    {
                        ErrorMessage = "Trường dữ liệu trùng lặp";
                        break;
                    }
            }
        }
        else
        {
            ErrorMessage = ex.Message;
        }
    }
}
