using System;

namespace MathematicsBot
{
    public class Log
    {
        public static void Push(string message) =>
            Console.WriteLine($"[{DateTime.Now}]: {message}");
    }
}
