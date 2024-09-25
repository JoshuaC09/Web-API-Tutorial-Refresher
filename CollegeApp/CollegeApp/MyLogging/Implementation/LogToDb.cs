using CollegeApp.MyLogging.Interface;

namespace CollegeApp.MyLogging.Implementation
{
    public class LogToDB : IMyLogger
    {
        public string Log(string message)
        {
            Console.WriteLine(message);
            return "This is for log to Db";
        }
    }
}
