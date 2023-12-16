using System.Diagnostics;
using System.Reflection;

namespace Kanoe.Shared
{
    public static class Logger
    {
        
        public static void Log(string message)
        {
            if(!Directory.Exists(Directory.GetCurrentDirectory() + @$"\UserData\logs"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @$"\UserData\logs");
            }

            using StreamWriter writer = new(Directory.GetCurrentDirectory() + @$"\UserData\logs\{DateTime.Now.ToShortDateString()}.txt", true);
            writer.WriteLine($"[{DateTime.Now}]: {message}");
            Console.WriteLine($"[{DateTime.Now}]: {message}");
        }

        public static void Error(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @$"\UserData\logs"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @$"\UserData\logs");
            }

            using StreamWriter writer = new(Directory.GetCurrentDirectory() + @$"\UserData\logs\{DateTime.Now.ToShortDateString()}.txt", true);
            writer.WriteLine($"[{DateTime.Now}] {memberName} : {message}");
            Console.WriteLine($"[{DateTime.Now}] {memberName} : {message}");
        }
    }
}
