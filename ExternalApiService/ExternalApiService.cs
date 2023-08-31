using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Data;
using SER.Domain.Entities.SqlServerCCKL;
using SER.Infrastructure;
using SER.Domain.Entities.SqlServerCCKL.Danhmuc;
using SER.ViewModel.HoSoCongViec.Requests;
using Serilog;

namespace SER.Domain.Services
{
    public interface IExternalApiServices
    {
        void GetDataFromConfig();
        object? ReceiveExternalData(Boolean manual = false);
        object? SendExternalData(HoSoCongViecSendDataRequest request);
        object? SendMultipleExternalData(HoSoCongViecSendMultipleDataRequest request);
    }
    public class ExternalApiServices : IExternalApiServices
    {
        private string baseAddress = "";
        private string loginAddress = "";
        private string APIPostAddress = "";
        private string APIGetAddress = "";
        private string access_token = "";
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private List<KeyValuePair<string, string>> formData = new List<KeyValuePair<string, string>>();
        private List<KeyValuePair<string, string>> dataRequest = new List<KeyValuePair<string, string>>();
        public ExternalApiServices(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public void GetDataFromConfig()
        {
            baseAddress = _configuration.GetSection("hmdigiAddress")["baseAddress"];
            loginAddress = _configuration.GetSection("hmdigiAddress")["loginAddress"];
            APIPostAddress = _configuration.GetSection("hmdigiAddress")["APIPostAddress"];
            APIGetAddress = _configuration.GetSection("hmdigiAddress")["APIGetAddress"];
            formData = _configuration.GetSection("hmdigiUser").GetChildren().ToDictionary(x => x.Key, y => y.Value).ToList();
            dataRequest = _configuration.GetSection("dataRequest").GetChildren().ToDictionary(x => x.Key, y => y.Value).ToList();
        }

        private bool login()
        {
            if (string.IsNullOrEmpty(baseAddress))
                GetDataFromConfig();

            Console.WriteLine("Prepare API client");
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            var request = new HttpRequestMessage(HttpMethod.Post, loginAddress);

            Console.WriteLine("Prepare API request");
            var byteArray = new UTF8Encoding().GetBytes("clientid:self");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            request.Content = new FormUrlEncodedContent(formData);

            Console.WriteLine("Request token");
            var response = client.SendAsync(request).Result;
            bool se = false;
            /*if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("DeserializeObject for token");
                string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                dataLogin dataUser = JsonConvert.DeserializeObject<dataLogin>(responseString);
                se = true;
                access_token = dataUser.access_token;
            }*/
            if (response.IsSuccessStatusCode)
            {

                string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                JObject dataUser = JsonConvert.DeserializeObject<JObject>(responseString);
                se = true;
                access_token = dataUser["access_token"].ToString();
            }
            else
            {
                Console.WriteLine("Đăng nhập không thành công!");
            }
            client.Dispose();
            if (!se) access_token = "";
            return se;
        }

        public object? ReceiveExternalData(Boolean manual = false)
        {
            HttpResponseMessage response;
            string responseString = "";

            List<MucLuc> lstMucLuc = new List<MucLuc>();
            try
            {
                if (manual || _context.MucLucs.Count() == 0 || _context.MucLucCons.Count() == 0 || _context.HoSoModels.Count() == 0)
                {
                    if (access_token == "")
                    {
                        if (string.IsNullOrEmpty(baseAddress) ||
                            string.IsNullOrEmpty(loginAddress) ||
                            string.IsNullOrEmpty(APIPostAddress) ||
                            string.IsNullOrEmpty(APIGetAddress))
                            GetDataFromConfig();

                        login();
                    }
                    if (access_token != "")
                    {
                        HttpClientHandler handler = new HttpClientHandler();

                        using (var client = new HttpClient(handler))
                        {
                            client.BaseAddress = new Uri(baseAddress);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            client.DefaultRequestHeaders.Authorization =
                                    new AuthenticationHeaderValue("Bearer", access_token);

                            var data = new
                            {
                                OrganId = dataRequest[0].Value,
                                Year = dataRequest[1].Value
                            };
                            ////#region appsetting
                            //data.OrganId = "000.00.37.H06";
                            ////#endregion
                            //data.Year = "2023"; //sửa

                            var json = JsonConvert.SerializeObject(data);
                            StringContent queryString = new StringContent(json, UnicodeEncoding.UTF8,
                                         "application/json");
                            response = client.PostAsync(APIGetAddress, queryString).GetAwaiter().GetResult();
                            StringBuilder Text = new StringBuilder();
                            if (response.IsSuccessStatusCode)
                            {
                                responseString = response.Content.ReadAsStringAsync().Result;
                                var dataObject = JsonConvert.DeserializeObject<List<MucLuc>>(responseString);
                                lstMucLuc.AddRange(dataObject);
                                List<MucLucCon> lstMucLucCon = new List<MucLucCon>();
                                List<HoSoModel> lstHoSo = new List<HoSoModel>();

                                lstMucLuc = lstMucLuc.Select(x => new MucLuc
                                {
                                    Id = x.Id,
                                    MaDV = x.MaDV,
                                    MaMucLuc = x.MaMucLuc,
                                    Nam = x.Nam,
                                    TenMucLuc = x.TenMucLuc,
                                    dsMucluccon = x.dsMucluccon != null ? x.dsMucluccon.Select(y => new MucLucCon()
                                    {
                                        Id = y.Id,
                                        MaDV = y.MaDV,
                                        MaMucLuc = y.MaMucLuc,
                                        MucLucId = x.Id,
                                        Nam = y.Nam,
                                        TenMucLuc = y.TenMucLuc,
                                        dsHoso = y.dsHoso != null ? y.dsHoso.Select(t => new HoSoModel
                                        {
                                            Id = t.Id,
                                            GhiChu = t.GhiChu,
                                            MaHoSo = t.MaHoSo,
                                            NguoiLap = t.NguoiLap,
                                            TenHoSo = t.TenHoSo,
                                            THBQ = t.THBQ,
                                            MucLucId = y.Id,
                                        }).ToList() : new List<HoSoModel>(),
                                        dsMucluccon = y.dsMucluccon != null ? y.dsMucluccon.Select(z => new MucLucCon()
                                        {
                                            Id = x.Id,
                                            MaDV = x.MaDV,
                                            MaMucLuc = x.MaMucLuc,
                                            Nam = x.Nam,
                                            TenMucLuc = x.TenMucLuc,
                                            dsMucluccon = x.dsMucluccon != null ? x.dsMucluccon.Select(y => new MucLucCon()
                                            {
                                                Id = y.Id,
                                                MaDV = y.MaDV,
                                                MaMucLuc = y.MaMucLuc,
                                                MucLucId = x.Id,
                                                Nam = y.Nam,
                                                TenMucLuc = y.TenMucLuc,
                                                dsHoso = y.dsHoso.Select(t => new HoSoModel
                                                {
                                                    Id = t.Id,
                                                    GhiChu = t.GhiChu,
                                                    MaHoSo = t.MaHoSo,
                                                    NguoiLap = t.NguoiLap,
                                                    TenHoSo = t.TenHoSo,
                                                    THBQ = t.THBQ,
                                                    MucLucId = y.Id,
                                                }).ToList(),
                                            }).ToList() : new List<MucLucCon>(),
                                        }).ToList() : new List<MucLucCon>(),
                                    }).ToList() : new List<MucLucCon>(),
                                    dsHoso = x.dsHoso != null ? x.dsHoso.Select(y => new HoSoModel
                                    {
                                        Id = y.Id,
                                        GhiChu = y.GhiChu,
                                        MaHoSo = y.MaHoSo,
                                        NguoiLap = y.NguoiLap,
                                        TenHoSo = y.TenHoSo,
                                        THBQ = y.THBQ,
                                        MucLucId = x.Id,
                                    }).ToList() : new List<HoSoModel>(),
                                }).ToList();
                                foreach (var item in lstMucLuc)
                                {
                                    if (item.dsMucluccon != null && item.dsMucluccon.Count() > 0)
                                    {
                                        lstMucLucCon.AddRange(item.dsMucluccon);
                                    }

                                    if (item.dsHoso != null && item.dsHoso.Count() > 0)
                                    {

                                        lstHoSo.AddRange(item.dsHoso);
                                    }

                                    foreach (var childItem in item.dsMucluccon)
                                    {
                                        if (childItem.dsMucluccon != null && childItem.dsMucluccon.Count() > 0)
                                        {
                                            lstMucLucCon.AddRange(childItem.dsMucluccon);
                                        }

                                        if (childItem.dsHoso != null && childItem.dsHoso.Count() > 0)
                                        {
                                            lstHoSo.AddRange(childItem.dsHoso);
                                        }
                                    }
                                }
                                SaveToDatabase(lstMucLuc, lstMucLucCon, lstHoSo);
                            }
                        }
                    }
                }
                return lstMucLuc;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public object? SendExternalData(HoSoCongViecSendDataRequest request)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("SeriLog/logger.txt").CreateLogger();

            GetDataFromConfig();
            edXMLLocal d1 = new edXMLLocal();
            d1.edXMLEnvelope = new EnvelopeLocal();
            d1.edXMLEnvelope.edXMLHeader = new MessageHeaderLocal();
            d1.edXMLEnvelope.edXMLBody = new edXMLBodyLocal();
            if (request.Type == "INSERT")
                d1.edXMLEnvelope.edXMLHeader.Process = ProcessLocal.INSERT_NEW;
            else
                d1.edXMLEnvelope.edXMLHeader.Process = ProcessLocal.UPDATE;
            #region thông tin hồ sơ
            //Mã hồ sơ bên QLVB
            string idFile = Guid.NewGuid().ToString();
            d1.edXMLEnvelope.edXMLHeader.FileID = request.Id == null ? idFile : request.Id.ToString();
            var MucLucId = _context.MucLucs.Where(x => x.MucLucId == request.DeMucId).FirstOrDefault();
            d1.edXMLEnvelope.edXMLHeader.FileCode = new FileFileCodeLocal()
            {
                OrganId = dataRequest[0].Value,// Mã đơn vị nộp lưu
                FileCatalog = MucLucId.Id.ToString(),//Mục lục
                FileNotation = request.MaTieuDeHoSo ?? "", // Mã hồ sơ
            };
            d1.edXMLEnvelope.edXMLHeader.Title = string.IsNullOrEmpty(request.TieuDeHoSo.ToString()) ? "Tiêu đề hồ sơ mẫu" : request.TieuDeHoSo.ToString(); //tiêu đề hồ sơ
            d1.edXMLEnvelope.edXMLHeader.Maintenance = string.IsNullOrEmpty(request.ThoiGianBaoQuan.ToString()) ? "Lâu dài" : request.ThoiGianBaoQuan.ToString(); //thời hạn bản quản
            d1.edXMLEnvelope.edXMLHeader.Rights = request.IsHanCheCheDoSuDung == true ? "Hạn chế" : "Không hạn chế";//chế độ sử dụng: "Hạn chế" hoặc ""
            d1.edXMLEnvelope.edXMLHeader.Language = string.IsNullOrEmpty(request.NgonNgu.ToString()) ? "Việt Nam" : request.NgonNgu.ToString(); // Ngôn ngữ
            d1.edXMLEnvelope.edXMLHeader.StartDate = string.IsNullOrEmpty(request.ThoiGianBatDau.ToString()) ? (DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year).ToString() : request.ThoiGianBatDau.ToString();//Thời gian bắt đầu hố sơ
            d1.edXMLEnvelope.edXMLHeader.EndDate = string.IsNullOrEmpty(request.ThoiGianKetThuc.ToString()) ? (DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year).ToString() : request.ThoiGianKetThuc.ToString();//Thời gian kết thúc hố sơ
            d1.edXMLEnvelope.edXMLHeader.DocTotal = int.Parse(string.IsNullOrEmpty(request.TongVanBan.ToString()) ? "1" : request.TongVanBan.ToString()); //tổng số văn bản trong hồ sơ
            d1.edXMLEnvelope.edXMLHeader.PageTotal = int.Parse(string.IsNullOrEmpty(request.TongTrang.ToString()) ? "1" : request.TongTrang.ToString());//tổng số trang văn bản
            d1.edXMLEnvelope.edXMLHeader.Description = string.IsNullOrEmpty(request.GhiChu.ToString()) ? "Ghi chú mẫu" : request.GhiChu.ToString(); // Ghi chú
            #endregion

            #region convert to XML
            SerializerXML xml = new SerializerXML();
            string t = xml.Serializeutf8(d1);
            #endregion

            #region dataPost
            dataPost data = new dataPost();
            data.checksum = "";
            var stringReader = new StringReader(t).ReadToEnd();
            UTF8Encoding ByteConverter = new UTF8Encoding();
            byte[] dataToXml = ByteConverter.GetBytes(stringReader.ToString());
            data.checksum = SerializerXML.CalculateChecksum(dataToXml);
            data.Content = dataToXml;
            var json = JsonConvert.SerializeObject(data);
            StringContent queryString = new StringContent(json, UnicodeEncoding.UTF8,
                         "application/json");
            #endregion

            string MessResult = "";
            if (access_token == "")
            {
                login();
            }

            if (access_token != "")
            {
                HttpClientHandler handler = new HttpClientHandler();

                using (var client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri(baseAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", access_token);

                    HttpResponseMessage response;
                    try
                    {
                        response = client.PostAsync(APIPostAddress, queryString).GetAwaiter().GetResult();
                        StringBuilder Text = new StringBuilder();
                        if (response.IsSuccessStatusCode)
                        {
                            MessResult = response.Content.ReadAsStringAsync().Result;
                        }
                        else
                        {
                            MessResult = "Có lỗi trong gói tin: " + response.Content.ReadAsStringAsync().Result;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessResult = "Post dữ liệu thất bại";
                        Log.Logger.Error("Result: " + ex.Message);
                    }
                }
            }
            return MessResult;
        }
        public object? SendMultipleExternalData(HoSoCongViecSendMultipleDataRequest request)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("SeriLog/logger.txt").CreateLogger();
            GetDataFromConfig();

            string MessResult = "";
            int index = 1;
            foreach (var itemRequest in request.HoSos)
            {
                Log.Logger.Information("Request: " + itemRequest.Id);
                #region
                edXMLLocal d1 = new edXMLLocal();
                d1.edXMLEnvelope = new EnvelopeLocal();
                d1.edXMLEnvelope.edXMLHeader = new MessageHeaderLocal();
                d1.edXMLEnvelope.edXMLBody = new edXMLBodyLocal();
                if (itemRequest.Type == "INSERT")
                    d1.edXMLEnvelope.edXMLHeader.Process = ProcessLocal.INSERT_NEW;
                else
                    d1.edXMLEnvelope.edXMLHeader.Process = ProcessLocal.UPDATE;
                #region thông tin hồ sơ
                //Mã hồ sơ bên QLVB
                string idFile = Guid.NewGuid().ToString();
                d1.edXMLEnvelope.edXMLHeader.FileID = itemRequest.Id == null ? idFile : itemRequest.Id.ToString();
                var HoSo = _context.HoSoCongViecs.Where(x => x.Id == itemRequest.Id).FirstOrDefault();
                var MucLucId = _context.MucLucs.Where(x => x.MucLucId == HoSo.DeMucId).FirstOrDefault();
                d1.edXMLEnvelope.edXMLHeader.FileCode = new FileFileCodeLocal()
                {
                    OrganId = dataRequest[0].Value,// Mã đơn vị nộp lưu
                    FileCatalog = MucLucId.Id.ToString(),//Mục lục
                    FileNotation = HoSo.SoKyHieu.Split('.')[0] ?? "", // Mã hồ sơ
                };
                d1.edXMLEnvelope.edXMLHeader.Title = string.IsNullOrEmpty(itemRequest.TieuDeHoSo.ToString()) ? "Tiêu đề hồ sơ mẫu" : itemRequest.TieuDeHoSo.ToString(); //tiêu đề hồ sơ
                d1.edXMLEnvelope.edXMLHeader.Maintenance = string.IsNullOrEmpty(itemRequest.ThoiGianBaoQuan.ToString()) ? "Lâu dài" : itemRequest.ThoiGianBaoQuan.ToString(); //thời hạn bản quản
                d1.edXMLEnvelope.edXMLHeader.Rights = HoSo.IsHanCheCheDoSuDung == true ? "Hạn chế" : "Không hạn chế";//chế độ sử dụng: "Hạn chế" hoặc ""
                d1.edXMLEnvelope.edXMLHeader.Language = string.IsNullOrEmpty(HoSo.NgonNgu.ToString()) ? "Việt Nam" : HoSo.NgonNgu.ToString(); // Ngôn ngữ
                d1.edXMLEnvelope.edXMLHeader.StartDate = string.IsNullOrEmpty(HoSo.ThoiGianBatDau.ToString()) ? (DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year).ToString() : HoSo.ThoiGianBatDau.Value.ToShortDateString();//Thời gian bắt đầu hố sơ
                d1.edXMLEnvelope.edXMLHeader.EndDate = string.IsNullOrEmpty(HoSo.ThoiGianKetThuc.ToString()) ? (DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year).ToString() : HoSo.ThoiGianKetThuc.Value.ToShortDateString();//Thời gian kết thúc hố sơ
                d1.edXMLEnvelope.edXMLHeader.DocTotal = int.Parse(string.IsNullOrEmpty(HoSo.TongVanBan.ToString()) ? "1" : HoSo.TongVanBan.ToString()); //tổng số văn bản trong hồ sơ
                d1.edXMLEnvelope.edXMLHeader.PageTotal = int.Parse(string.IsNullOrEmpty(HoSo.TongTrang.ToString()) ? "1" : HoSo.TongTrang.ToString());//tổng số trang văn bản
                d1.edXMLEnvelope.edXMLHeader.Description = string.IsNullOrEmpty(HoSo.GhiChu.ToString()) ? "Ghi chú mẫu" : HoSo.GhiChu.ToString(); // Ghi chú
                #endregion

                if (itemRequest.ListVanBanDen != null && itemRequest.ListVanBanDen.Count() > 0)
                {
                    d1.edXMLEnvelope.edXMLBody = new edXMLBodyLocal();
                    d1.edXMLEnvelope.edXMLBody.edXMLDocuments = new List<DocumentLocal>();
                    //d1.AttachmentEncoded = new List<AttachmentLocal>();
                    #region thêm văn bản 1
                    var vbIndex = 1;
                    foreach (var itemVanBan in itemRequest.ListVanBanDen)
                    {
                        try
                        {
                            string vbInfo = string.Empty;
                            string iddoc1 = Guid.NewGuid().ToString();

                            var loaiVB = _context.DmLoaivanbans.Where(x => x.LoaiVanBanId == itemVanBan.VanBan.LoaiVanBanID).FirstOrDefault();
                            if (loaiVB != null)
                            {
                                d1.edXMLEnvelope.edXMLBody.edXMLDocuments.Add(new DocumentLocal()
                                {
                                    Docid = iddoc1,
                                    DocOrdinal = vbIndex.ToString(),
                                    TypeName = loaiVB.TenLoaiVanBan,//Loai van ban
                                    CodeNumber = itemVanBan.VanBan.SoKyHieu.ToString(),//?soKyHieu
                                    CodeNotation = itemVanBan.VanBan.SoVanBan.ToString(),//? soVanBan
                                    IssuedDate = itemVanBan.VanBan.NgayVanBan.Value.ToShortDateString(),//ngayVanBan
                                    OrganName = itemVanBan.VanBan.TenDonViPhatHanh.ToString(),//tenDonViPhatHanh
                                    Subject = itemVanBan.VanBan.TrichYeu.ToString(),//TrichYeu
                                    Language = "Việt Nam",
                                    PageAmount = (int)itemVanBan.VanBan.TotalCount,
                                    Description = string.Empty,//ghiChu
                                    InfoSign = string.Empty,
                                    Keyword = string.Empty,
                                    Mode = string.Empty,
                                    ConfidenceLevel = string.Empty,
                                    Autograph = string.Empty,
                                    Format = string.Empty,
                                });
                                //Thêm file đính kèm
                                //AttachmentLocal file1 = new AttachmentLocal(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                //    "1.pdf"), iddoc1);
                                //d1.AttachmentEncoded.Add(file1);
                                vbIndex++;
                            }
                            else
                            {
                                Log.Logger.Information("Không tìm thấy loaiVB: " + itemVanBan.VanBan.LoaiVanBanID);
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Logger.Error("Error: " + ex.Message);
                            if (ex.InnerException != null)
                                Log.Logger.Error("Error: " + ex.InnerException.Message);
                            MessResult = ex.Message;
                        }
                    }
                    #endregion

                }
                #region convert to XML
                SerializerXML xml = new SerializerXML();
                string t = xml.Serializeutf8(d1);
                #endregion

                #region dataPost
                dataPost data = new dataPost();
                data.checksum = "";
                var stringReader = new StringReader(t).ReadToEnd();
                UTF8Encoding ByteConverter = new UTF8Encoding();
                byte[] dataToXml = ByteConverter.GetBytes(stringReader.ToString());
                data.checksum = SerializerXML.CalculateChecksum(dataToXml);
                data.Content = dataToXml;
                var json = JsonConvert.SerializeObject(data);
                StringContent queryString = new StringContent(json, UnicodeEncoding.UTF8,
                             "application/json");
                #endregion

                if (access_token == "")
                {
                    login();
                }

                if (access_token != "")
                {
                    HttpClientHandler handler = new HttpClientHandler();

                    using (var client = new HttpClient(handler))
                    {
                        client.BaseAddress = new Uri(baseAddress);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Bearer", access_token);

                        HttpResponseMessage response;
                        try
                        {
                            response = client.PostAsync(APIPostAddress, queryString).GetAwaiter().GetResult();
                            StringBuilder Text = new StringBuilder();
                            if (response.IsSuccessStatusCode)
                            {
                                MessResult = MessResult + "\n" + "Số gói tin gửi đi: " + index + "\n Thông báo: " + response.Content.ReadAsStringAsync().Result;
                            }
                            else
                            {
                                MessResult = "Có lỗi trong gói tin: " + response.Content.ReadAsStringAsync().Result;
                            }
                        }
                        catch (Exception ex)
                        {

                            MessResult = "Post dữ liệu thất bại";

                        }
                    }
                }
                index++;
            }
            #endregion

            return MessResult;
        }
        private void SaveToDatabase(List<MucLuc> lstMucLuc, List<MucLucCon> lstMucLucCon, List<HoSoModel> lstHoSo)
        {
            var existingMucLucData = _context.MucLucs.Count() > 0;
            var existingMucLucConData = _context.MucLucCons.Count() > 0;
            var existingHoSoData = _context.HoSoModels.Count() > 0;
            if (!existingMucLucData)
            {
                _context.MucLucs.AddRange(lstMucLuc);
            }
            if (!existingMucLucConData)
            {
                _context.MucLucCons.AddRange(lstMucLucCon);
            }
            if (!existingHoSoData)
            {
                _context.HoSoModels.AddRange(lstHoSo);
            }
            _context.SaveChanges();
        }
    }
}
