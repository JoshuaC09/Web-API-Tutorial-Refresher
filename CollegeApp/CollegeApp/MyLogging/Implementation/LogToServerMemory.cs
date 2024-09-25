using CollegeApp.MyLogging.Interface;

namespace CollegeApp.MyLogging.Implementation
{
    public class LogToServerMemory : IMyLogger
    {
        public string Log(string message)
        {
            Console.WriteLine(message);
            return "This is for log to Server Memory";
        }
    }
}
