using System;
using System.IO;

namespace MathematicsBot
{
    class Program
    {
        private const string Name = 
            "MathematicsBot by spyfast | (vk.com/fusts)";
        static void Main(string[] args)
        {
            Console.Title = Name;

            if (!File.Exists("data.txt"))
            {
                File.Create("data.txt").Close();
                Log.Push("Введите логин и пароль: (+79000000000:passwords)");
                var ParseData = Console.ReadLine().Split(':');

                File.WriteAllText(
                    "data.txt", $"{ParseData[0]}:{ParseData[1]}");

                new Account()
                {
                    Login = ParseData[0],
                    Password = ParseData[1]
                }.Auth();

            }
            else
            {
                var data = 
                    File.ReadAllText("data.txt").Split(':');

                new Account()
                {
                    Login = data[0],
                    Password = data[1]
                }.Auth();
            }

            Console.ReadKey(true);
        }
    }
}
