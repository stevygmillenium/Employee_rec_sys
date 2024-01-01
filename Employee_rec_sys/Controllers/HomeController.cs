using Employee_rec_sys.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Employee_rec_sys.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Apply() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult Confirmation([Bind(include:"Name,email,phone,job_pos,des_sal,Files")]Applicant appl) 
        {
            var builder = WebApplication.CreateBuilder();
            string constr = builder.Configuration.GetConnectionString("Default");
            SqlConnection sqlConnection = new SqlConnection(constr);
            sqlConnection.Open();
            SqlCommand sqlCommand=new SqlCommand(constr,sqlConnection);
            sqlCommand.Connection=sqlConnection;
            sqlCommand.CommandText = "insert into [dbo].[Table] (name,email,phone,job_pos,des_sal,dateTime) values (@name,@email,@phone,@job_pos,@des_sal,@dateTime)";
            appl.dateTime=DateTime.Now;
            sqlCommand.Parameters.AddWithValue("@name", appl.Name);
            sqlCommand.Parameters.AddWithValue("@email", appl.email);
            sqlCommand.Parameters.AddWithValue("@phone", appl.phone);
            sqlCommand.Parameters.AddWithValue("@job_pos", appl.job_pos);
            sqlCommand.Parameters.AddWithValue("@des_sal", (float)appl.des_sal);
            sqlCommand.Parameters.AddWithValue("@dateTime", appl.dateTime);
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(nameof(appl_files.filename),typeof(string));
            dataTable.Columns.Add(nameof(appl_files.filetype),typeof(string));
            dataTable.Columns.Add(nameof(appl_files.data), typeof(byte[]));
            dataTable.Columns.Add(nameof(Applicant.email), typeof(string));
            foreach(IFormFile formFile in appl.Files) 
            {
                MemoryStream stream = new MemoryStream();
                formFile.CopyTo(stream);
                byte[] data = stream.ToArray();
                dataTable.Rows.Add(formFile.FileName,formFile.ContentType,data,appl.email);
            }
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlConnection);
            sqlBulkCopy.DestinationTableName = "sel_File";
            sqlBulkCopy.ColumnMappings.Add("filename", "filename");
            sqlBulkCopy.ColumnMappings.Add("filetype", "filetype");
            sqlBulkCopy.ColumnMappings.Add("data", "data");
            sqlBulkCopy.ColumnMappings.Add("email", "email");
            sqlConnection.Open();
            sqlBulkCopy.WriteToServer(dataTable);
            sqlConnection.Close();
            return View(appl);
        }
        public IActionResult appl_searc() 
        {            
            return View();
        }
        [HttpPost]
        public IActionResult Rec_admin(IFormCollection keyValues) 
        {
            int id = Convert.ToInt16(keyValues["hf"]);
            var builder = WebApplication.CreateBuilder();
            string constr = builder.Configuration.GetConnectionString("Default");
            SqlConnection sqlConnection = new SqlConnection(constr);
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand(constr, sqlConnection);
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "select * from [dbo].[Table] where id="+id;
            SqlDataReader sqlDataReader; 
            sqlDataReader = sqlCommand.ExecuteReader();
            sqlDataReader.Read();
            Applicant applicant=new Applicant();
            applicant.Id = (int)sqlDataReader["id"];
            applicant.Name = (string)sqlDataReader["name"];
            applicant.email = (string)sqlDataReader["email"];
            applicant.phone = (string)sqlDataReader["phone"];
            applicant.job_pos = (string)sqlDataReader["job_pos"];
            applicant.des_sal = (double)sqlDataReader["des_sal"];
            applicant.dateTime = (DateTime)sqlDataReader["dateTime"];
            sqlConnection.Close();
            sqlConnection.Open();
            sqlCommand.CommandText = "select * from sel_File where email='"+applicant.email+"'";
            sqlDataReader = sqlCommand.ExecuteReader();
            List<appl_files>appl_Files=new List<appl_files>();
            while (sqlDataReader.Read()) 
            {
                appl_files appl_File = new appl_files();
                appl_File.filename =(string) sqlDataReader["filename"];
                appl_File.filetype =(string) sqlDataReader["filetype"];
                appl_File.data =(byte[]) sqlDataReader["data"];
                appl_Files.Add(appl_File);
            }
            applicant.appl_files=appl_Files;
            sqlConnection.Close();
            return View(applicant);
        }
        [HttpPost]
        public IActionResult ext_file(IFormCollection keyValuePairs)
        {
            appl_files appl_Files = new appl_files();
            appl_Files.filename = keyValuePairs["hf"];
            string em = keyValuePairs["hf_em"];
            var builder = WebApplication.CreateBuilder();
            string constr = builder.Configuration.GetConnectionString("Default");
            SqlConnection sqlConnection = new SqlConnection(constr);
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand(constr, sqlConnection);
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "select * from sel_File where email='"+em+"' and filename='"+appl_Files.filename+"'";
            SqlDataReader sqlDataReader= sqlCommand.ExecuteReader();
            sqlDataReader.Read();
            appl_Files.data =(byte[]) sqlDataReader["data"];
            return View(appl_Files);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
