namespace CollegeApp.Model
{
    public static class CollegeRepository
    {

        public static List<Student> Students { get; set; } = new List<Student>()
        {
            new Student()
            {
                Id = 1,
                Name = "Test1",
                Email = "Test1",
                Allowance = 100
            }      ,
            new Student()
            {
                Id = 2,
                Name = "Test2",
                Email = "Test2",
                Allowance = 100
            }

        };
    }
}
