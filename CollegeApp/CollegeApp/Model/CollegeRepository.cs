namespace CollegeApp.Model
{
    public static class CollegeRepository
    {

        public static List<Student> Students { get; set; } = new List<Student>()
        {
            new Student()
            {
                Id = 1,
                Name = "Test",
                Email = "Test",
                Allowance = 100
            }      ,
            new Student()
            {
                Id = 1,
                Name = "Test",
                Email = "Test",
                Allowance = 100
            }

        };
    }
}
