using CollegeApp.Model;

namespace CollegeApp.Data
{
    public class Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string DepartDescription { get; set; }
        public virtual ICollection<Student> Students { get;set; }
    }
}
