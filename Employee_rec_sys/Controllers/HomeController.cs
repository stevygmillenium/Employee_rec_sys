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
            string constr = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=emp_rec_sys;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
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
            string constr = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=emp_rec_sys;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
            SqlConnection sqlConnection = new SqlConnection(constr);
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand(constr, sqlConnection);
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "select * from";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            List<Applicant> applicants = new List<Applicant>();
            return View(applicants);
        }
        public IActionResult Rec_admin() 
        {
            string constr = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=emp_rec_sys;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
            SqlConnection sqlConnection = new SqlConnection(constr);
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand(constr, sqlConnection);
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "select * from";            
            Applicant applicant=new Applicant();
            sqlCommand.CommandText = "select * from";
            List<appl_files>appl_Files=new List<appl_files>();
            applicant.appl_files=appl_Files;
            return View(applicant);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
