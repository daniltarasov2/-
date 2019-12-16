using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace MathematicsBot
{
    public class Account
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [NonSerialized] public VkApi vkApi;

        Random random { get; set; }

        public Account()
        {
            vkApi = new VkApi();
            random = new Random();
        }

        public void Auth()
        {
            ApiAuthParams api = new ApiAuthParams()
            {
                Login = Login,
                Password = Password,
                ApplicationId = 2685278
            };

            try
            {
                vkApi.Authorize(api);
                var get = vkApi.Users.Get(
                    new long[0], null, null).FirstOrDefault();

                if (vkApi.IsAuthorized)
                {
                    Log.Push("Успешная авторизация");
                    Console.Title = $"Вход: {get.FirstName} {get.LastName} | id{get.Id}";
                    Log.Push("Какие примеры будем решать?\n1) Обычные примеры\n2) Квадратные уравнения");

                    var selection = int.Parse(Console.ReadLine());

                    switch (selection)
                    {
                        case 1:
                            SolvingsExamples();
                            break;
                        case 2:
                            QuadraticEquation();
                            break;
                    }
                }
                else
                {
                    Log.Push("Не удалось авторизоваться — запуск невозможен");
                    Console.Title = $"Не удалось авторизоваться: {Login}";
                    vkApi = null;
                }

            }
            catch (Exception ex)
            {
                vkApi = null;
                Log.Push(ex.Message);
            }
        }
        public void SolvingsExamples()
        {
            try
            {
                Log.Push("Введите ID пользователя: ");
                var _userID = long.Parse(Console.ReadLine());
                Operations operations = Operations.None;
                double result = 0;

                Log.Push($"Цель: id{_userID}. Бот запущен.");
                Help();
                while (true)
                {
                    var history =
                        vkApi.Messages.GetHistory(new MessagesGetHistoryParams()
                    { Count = 1, UserId = _userID });

                    foreach (var message in history.Messages)
                    {
                        if (message.FromId != vkApi.UserId)
                        {
                            var text = message.Text.ToLower();
                            var getText = text.Split(' ');

                            if (!getText[1].Contains("из"))
                            {
                                var firstNumbers = double.Parse(getText[0]);
                                var secondNumbers = double.Parse(getText[2]);

                                if (getText[1].Contains("+"))
                                    operations = Operations.Add;
                                else if (getText[1].Contains("-"))
                                    operations = Operations.Difference;
                                else if (getText[1].Contains("/"))
                                    operations = Operations.Division;
                                else if (getText[1].Contains("*"))
                                    operations = Operations.Multiplier;

                                switch (operations)
                                {
                                    case Operations.Add:
                                        result = firstNumbers + secondNumbers;
                                        break;
                                    case Operations.Difference:
                                        result = firstNumbers - secondNumbers;
                                        break;
                                    case Operations.Division:
                                        result = firstNumbers / secondNumbers;
                                        break;
                                    case Operations.Multiplier:
                                        result = firstNumbers * secondNumbers;
                                        break;
                                }
                                vkApi.Messages.Send(new MessagesSendParams()
                                { UserId = _userID, Message = result.ToString(), RandomId = random.Next() });

                                Log.Push($"Пример решен успешно, ответ: {result.ToString()}");
                            }
                            else
                            {
                                var rootNumbers = double.Parse(getText[2]);
                                operations = Operations.Root;
                                switch (operations)
                                {
                                    case Operations.Root:
                                        result = Math.Sqrt(rootNumbers);
                                        break;
                                }
                                vkApi.Messages.Send(new MessagesSendParams()
                                { UserId = _userID, Message = result.ToString(), RandomId = random.Next() });

                                Log.Push($"Корень успешно извлечен, ответ: {result.ToString()}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Push(ex.Message);
            }
        }

        public void QuadraticEquation()
        {
            try
            {
                Log.Push("Введите ID пользователя");
                var _userID = long.Parse(Console.ReadLine());

                Log.Push($"Цель: {_userID}. Бот запущен.");
                while (true)
                {
                    var history =
                          vkApi.Messages.GetHistory(new MessagesGetHistoryParams()
                          { Count = 1, UserId = _userID });

                    foreach (var message in history.Messages)
                    {
                        if (message.FromId != vkApi.UserId)
                        {
                            var text = message.Text;
                            var getText = text.Split(' ');

                            var A = double.Parse(getText[0]);
                            var B = double.Parse(getText[2]);
                            var C = double.Parse(getText[4]);

                            double D = B * B - 4 * A * C;

                            if (D < 0)
                            {
                                var result = "Корней нет. Дискриминант меньше нуля.";
                                vkApi.Messages.Send(new MessagesSendParams()
                                { UserId = _userID, Message = result, RandomId = random.Next() });

                                Log.Push($"Уравнение успешно решено: {result}");
                            }
                            else if (D == 0)
                            {
                                double X = -B / (2 * A);
                                var result = $"Дискриминант равен нулю. Корень равен — {X.ToString()}";
                                vkApi.Messages.Send(new MessagesSendParams()
                                { UserId = _userID, Message = result, RandomId = random.Next() });

                                Log.Push($"Уравнение успешно решено: {result}");
                            }
                            else if (D > 0)
                            {
                                double x1 = -B - Math.Sqrt(D) / (2 * A);
                                double x2 = -B + Math.Sqrt(D) / (2 * A);
                                var result = $"Дискриминант равен {D.ToString()}\nПервый корень: {x1.ToString()}\nВторой корень: {x2.ToString()}";
                                vkApi.Messages.Send(new MessagesSendParams()
                                { UserId = _userID, Message = result, RandomId = random.Next() });

                                Log.Push($"\nУравнение успешно решено:\n{result}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Push(ex.Message);
            }
        }
        public enum Operations
        {
            None,
            Add,
            Difference,
            Division,
            Multiplier,
            Root
        }

        public void Help() =>
               Log.Push($"\nСправка:\n\nБот решает примеры со всеми действиями:\n1) Сложение: 1 + 1\n2) Разность: 1 - 1\n3) Деление: 1 / 1\n4) Умножение: 1 * 1");
    }
}
