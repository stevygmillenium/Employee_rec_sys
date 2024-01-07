using Employee_rec_sys.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Xml;
using System.Text;

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
        [HttpPost]
        public IActionResult add_prof(IFormCollection keyValues) 
        {
            string edu_h = keyValues["edu"], work_h = keyValues["work"];
            Applicant applicant = new Applicant();
            applicant.email = keyValues["p_em"];
            List<edu_hist> edu = new List<edu_hist>();
            string[] edu_hist = edu_h.Split('\n');
            for (int i = 0; i < edu_hist.Length; i++)
            {
                string[] edu_inst = edu_hist[i].Split(',');
                edu_hist e_h = new edu_hist();
                e_h.inst = edu_inst[0];
                e_h.area_of_study = edu_inst[1];
                e_h.start = Convert.ToDateTime(edu_inst[2]);
                e_h.fin = Convert.ToDateTime(edu_inst[3]);
                edu.Add(e_h);
            }
            List<work_hist> work = new List<work_hist>();
            string[] work_hist = work_h.Split('\n');
            for (int i = 0; i < work_hist.Length; i++)
            {
                string[] work_org = work_hist[i].Split(',');
                work_hist w_h = new work_hist();
                w_h.org = work_org[0];
                w_h.job_title = work_org[1];
                w_h.start = Convert.ToDateTime(work_org[2]);
                w_h.fin = Convert.ToDateTime(work_org[3]);
                work.Add(w_h);
            }
            applicant.edu_Hists = edu;
            applicant.work_Hists = work;
            XmlSerializer xmlSerializer = null;
            XmlDocument xmlDocument = null;
            MemoryStream memoryStream = null;
            StreamWriter streamWriter = null;
            var builder = WebApplication.CreateBuilder();
            string constr = builder.Configuration.GetConnectionString("Default");
            SqlConnection sqlConnection = new SqlConnection(constr);
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand(constr, sqlConnection);
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "insert into [dbo].[emp_prof] (email,edu,work) values (@email,@edu,@work)";
            sqlCommand.Parameters.AddWithValue("@email", applicant.email);
            sqlCommand.Parameters.AddWithValue("@edu", SqlDbType.Xml);
            xmlSerializer= new XmlSerializer(typeof(List<edu_hist>));
            xmlDocument = new XmlDocument();
            memoryStream = new MemoryStream();
            streamWriter = new StreamWriter(memoryStream, Encoding.GetEncoding("utf-16"));
            xmlSerializer.Serialize(streamWriter, applicant.edu_Hists);
            memoryStream.Position = 0;
            xmlDocument.Load(memoryStream);
            sqlCommand.Parameters["@edu"].Value = xmlDocument.InnerXml;
            sqlCommand.Parameters.AddWithValue("@work", SqlDbType.Xml);
            xmlSerializer= new XmlSerializer(typeof(List<work_hist>));
            xmlDocument = new XmlDocument();
            memoryStream = new MemoryStream();
            streamWriter = new StreamWriter(memoryStream, Encoding.GetEncoding("utf-16"));
            xmlSerializer.Serialize(streamWriter, applicant.work_Hists);
            memoryStream.Position = 0;
            xmlDocument.Load(memoryStream);
            sqlCommand.Parameters["@work"].Value= xmlDocument.InnerXml;
            sqlCommand.ExecuteNonQuery();
            return View(applicant);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
