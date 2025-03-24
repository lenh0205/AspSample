========================================================================
# SAP Crystal Reports
* -> is a **`business intelligence (BI)` reporting tool** developed by SAP
* -> that lets users **build and customize reports** by analyzing data from several data sources
* => for trend visualization, data analysis, and progress monitoring

## BI - Business Intelligence
* -> refers to **technologies, applications, and practices** used for **`collecting, analyzing, and presenting business data`**
* -> to help in **`decision-making`**
* -> it includes **reporting**, **dashboards**, **data visualization**, and **analytics**

## Data source
* -> **Relational Databases** - MySQL, Microsoft SQL Server, Oracle Database, IBM DB2, Google BigQuery, Amazon Redshift,...
* -> **ERP Systems** - SAP Business One, SAP ERP, Oracle E-Business Suite, Microsoft Dynamics, NetSuite
* -> **Spreadsheets** - Microsoft Excel, OpenDocument Spreadsheet (ODS), Comma-Separated Values (CSV)
* -> **Text Files** - Plain Text (TXT), XML
* -> other - ODBC and JDBC data sources, Web Services, Cloud-based data sources

## Element
* -> **`Report Designer`** - có 1 interface để user tạo và sửa report; thêm item (field, graph, chart,...) bằng việc "drag-and-drop"; change formatting, styles, filter data
* -> **`Fields`** - **unprocessed data items/characteristics** lấy từ data source, dùng cho việc create tables, charts, and other elements
* -> **`Formulas`** - **expression** để tính toán, chỉnh sửa dựa trên data (averages, percentages, and intricate conditions, ...)
* -> **`Groups and Totals`** - **Grouping** là để sắp xếp data theo một tiêu chuẩn nào đó; **Totals** cung cấp aggregate values for grouped data (sums, averages, or counts, ...)
* -> **`Graphs and charts`** - a **visual representation** of the data (scatter plots, pie charts, bar charts, and line charts, ...)
* -> **`Sub Reports`** - a smaller nested reports that is embedded a complex report to **enhance the structure and readability of reports**
* -> **`Parameters`** - for **dynamically** adjust report behavior and content based on user input, filter data, or alter the appearance of reports
* -> **`Distribution and Scheduling`** - users of Crystal Reports may program reports to **run automatically at certain periods**; **export reports to other formats** (PDF, Excel, or HTML) and **distribute** them via email, file systems, CMS

## Install
* -> cài Crystal Reports Developer Edition for Visual Studio (Ex: CR for Visual Studio SP30 install package; lưu ý cần có sự tương thích giữa phiên bản của Crystal Report Service Pack và Visual Studio)
* -> cài Crystal Reports Runtime (thường thì khi cài cái trên thì nó sẽ có option cài Runtime luôn)
* -> chạy setup wizard cho tệp tin vừa tải

========================================================================
# Example1: hiển thị Crystal Report in WinForms
* -> tạo project Window Forms mới 
* -> tạo Crystal Report: right-click on project, chọn "Add New Item" -> Reporting Template Group -> Crystal Report Template -> Add -> chuyển hướng đến màn hình "Crystal Report Gallery"
* -> chọn **Report Wizard** và **Standard Expert** -> Next -> chuyển sang màn hình "Standard Report Creation Wizard"
* -> chọn data source: Project Data (kết nối có sẵn trong project); My Connection (kết nối gần đây); New Connection
* -> chọn **OLE DB (ADO)** để kết nối đến data source -> chọn **OLE/DB Provider** phù hợp để kết nối vào DB của ta (_Ex: `SQL Server Native Client 11.0` để kết nối `SQL Server Database`_)
* -> điền các thông tin cho Connect String
* -> sau khi "Finish" quay về màn hình "Standard Report Creation Wizard", ta phải thấy tree của DB ta vừa kết nối thì mới là kết nối thành công
* -> chọn bảng ta muốn thêm vào Report -> chọn các cột ta muốn hiển thị trên report  
* -> giờ để hiển thị Crystal Report trong Windows Form của chúng ta: mở Form lên, vào Toolbox kéo **CrystalReportViewer** bỏ vào Form và update lại code

### hiển thị Crystal Report in ASP.NET WebForms
* -> tạo project ASP.NET Web mới
* -> right-click on the project -> select Add New Item -> select Crystal Reports -> đặt tên là "MyReport.rpt" -> Add
* -> các bước còn lại như trên
* -> ta kéo **CrystalReportViewer** vào file Default.aspx
* -> update code và run application
```cs
using CrystalDecisions.CrystalReports.Engine;

public partial class _Default : System.Web.UI.Page  
{  
    protected void Page_Load(object sender, EventArgs e)  
    {  
        ReportDocument cryRpt = new ReportDocument();  
        cryRpt.Load(Server.MapPath("EmployeeCrystalReport.rpt"));  
        CrystalReportViewer1.ReportSource = cryRpt;           
    }  
}  
``` 

# Example2: create a PDF file from the data and export it using Crystal Reports in MVC 5 (.NET Framework 4.5.2)

## Set up project
* -> tạo Database -> tạo Table -> add some Record
* -> Visual Studio ->  ASP.NET Web Application (.NET Framework) -> MVC -> OK
* -> adding ADO.NET Entity data model: Add New Item -> ADO.NET Entity Data Model -> đặt tên cho DbContext -> Add
* -> chọn "EF Designer from the database" -> Next -> chọn Server Name và Database Name -> chọn "Table" (VD: Customers) -> Finish -> ta sẽ thấy EDMX model generates a Customer class

## Create Crystal Report
* -> install Crystal Report for Visual Studio (nếu chưa có sẵn)
* -> tạo "CrystalReports" folder ->  Add New Item -> Reporting -> CrystalReports -> đặt tên "ReportCustomer.rpt" -> Add -> As a Blank Report -> OK
* -> nó sẽ hiện **Designer** của "ReportCustomer.rpt" và **Field Explorer**
* -> right-click "Database Fields" from the Field Explorer -> select the "Database Expert" option -> chọn Table ta muốn -> OK
* -> bây giờ ta sẽ thấy selected tables trong "Database Fields" -> giờ ta sẽ drag fields của nó (VD: "Employee" table có 2 field "Id" và "Name") to the report (designer) that we want to display in the PDF file

## Controller
```cs
using CrystalDecisions.CrystalReports.Engine;

namespace CrystalReportMVC.Controllers
{
    public class CustomerController : Controller
    {
        private CustomerDBEntities context = new CustomerDBEntities(); // DbContext

        // GET: Customer
        public ActionResult Index()
        {
            var customerList = context.Customers.ToList();
            return View(customerList);
        }
        public ActionResult ExportCustomers()
        {
            List<Customer> allCustomer = context.Customers.ToList();

            ReportDocument rd = new ReportDocument(); // create a Crystal Report's object
            rd.Load(Path.Combine(Server.MapPath("~/CrystalReports"), "ReportCustomer.rpt")); // load file .rpt theo path trong project
            rd.SetDataSource(allCustomer);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "CustomerList.pdf");
        }
    }
}
```

## View 
* -> in CustomerController -> right-click on the Index() action -> Add View -> select "List" as template, "Customer" as model class
* -> khi chạy project lên -> ta có thể click "Report PDF" để xuất file pdf
 
```cs
@model IEnumerable<CrystalReportMVC.Customer>
@{
    ViewBag.Title = "Index";
}
<h2>Customers List</h2>
<p>
    @Html.ActionLink("Create New", "Create")

    <div><a href="@Url.Action("ExportCustomers")"> Report PDF </a></div>
</p>
<table class="table">
    <tr>
        <th>
            @Html.DisplayNameFor(model => model.CustomerName)
        </th>
        <th>
            @Html.DisplayNameFor(model => model.CustomerEmail)
        </th>
        <th>
            @Html.DisplayNameFor(model => model.CustomerZipCode)
        </th>
        <th>
            @Html.DisplayNameFor(model => model.CustomerCountry)
        </th>
        <th>
            @Html.DisplayNameFor(model => model.CustomerCity)
        </th>
        <th></th>
    </tr>
    @foreach (var item in Model) {
        <tr>
            <td>
                @Html.DisplayFor(modelItem => item.CustomerName)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.CustomerEmail)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.CustomerZipCode)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.CustomerCountry)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.CustomerCity)
            </td>
            <td>
                @Html.ActionLink("Edit", "Edit", new { id=item.CustomerID }) |
                @Html.ActionLink("Details", "Details", new { id=item.CustomerID }) |
                @Html.ActionLink("Delete", "Delete", new { id=item.CustomerID })
            </td>
        </tr>
    }
</table>
```
