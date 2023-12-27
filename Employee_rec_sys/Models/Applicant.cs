namespace Employee_rec_sys.Models
{
    public class Applicant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string job_pos { get; set; }
        public double des_sal {  get; set; }
        public DateTime dateTime {  get; set; }
        public List<IFormFile> Files { get; set; }
        public List<appl_files> appl_files { get; set; }
    }
    public class appl_files 
    {
        public string filename { get; set; }
        public string filetype { get; set; }
        public byte[] data { get; set; }
    }
}
