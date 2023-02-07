using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Module_7
{
    internal class Program
    {
        private static string[] _workerTitles = Worker.WorkerMetaDataTitlesInit();      //Массив заголовков таблицы справочника.
        private static Repository repository = new Repository("employeesDB.csv");       //Создание экземпляра класса репозиторий с указанием пути и имени файла.

        private static void Main()
        {
            Console.SetWindowSize(200, 50);     //Задание размера консольного окна, чтобы записи помещались без переносов строк.
            MainMenu();
            Console.ReadKey();

        }

        private static void MainMenu()
        {
            Console.WriteLine("Справочник \"Сотрудники\"\n");
            Console.WriteLine(
                "Нажмите \"1\", чтобы вывести данные по всем сотрудникам на экран\n" +
                "Нажмите \"2\", чтобы выполнить поиск сотрудника по ID\n" +
                "Нажмите \"3\", чтобы добавить нового сотрудника\n" +
                "Нажмите \"4\", чтобы удалить сотрудника\n" +
                "Нажмите \"5\", чтобы вывести сотрудников в заданном диапазоне дат\n");

            char key = Console.ReadKey(true).KeyChar;

            if (char.ToLower(key) == '1')
            {
                PrintAllWorkers();      //Вывод всех сотрудников.
            }
            else if (char.ToLower(key) == '2')
            {
                SearchWorkerById();     //Поиск сотрудника по ID.
            }
            else if (char.ToLower(key) == '3')
            {
                AddNewWorker();     //Добавление нового сотрудника.
            }
            else if (char.ToLower(key) == '4')
            {
                DeleteWorker();     //Удаление сотрудника.
            }
            else if (char.ToLower(key) == '5')
            {
               WorkersBetweenTwoDates();       //Вывод списка сотрудников в заданном диапазоне дат.
            }
        }

        /// <summary>
        /// Выводит данные обо всех сотрудниках на экран.
        /// </summary>
        private static void PrintAllWorkers()
        {
            PrintTitles();
            var workersToPrint = repository.GetAllWorkers();        //Формируем список сотрудников для вывода.

            //СОРТИРОВКА!
            /*workersToPrint.Sort(delegate (Worker one, Worker two)
            {
                if (one.FullName == null && two.FullName == null) return 0;
                else if (one.FullName == null) return -1;
                else if (two.FullName == null) return 1;
                else return one.FullName.CompareTo(two.FullName);
            });*/


            for (int i = 0; i < repository.LinesCount; i++)     //Обходим список и конвертируем экземпляр Worker в строку для вывода на экран.
            {
                string resultToPrint = workersToPrint[i].PrintToConsole();
                Console.WriteLine(resultToPrint);
            }

            Console.WriteLine(repository.LinesCount);       //УДАЛИТЬ ПОТОМ. счётчик строк

        }

        /// <summary>
        /// Поиск сотрудника по ID.
        /// </summary>
        private static void SearchWorkerById()
        {
            char key = 'н';

            do
            {
                Console.Write("Введите ID сотрудника для поиска: ");
                var stringId = Console.ReadLine();
                int id;

                if (int.TryParse(stringId, out id))     //Проверяем, что введено число.
                {
                    Worker searchedWorker = repository.GetWorkerById(id);       //Выполняем поиск сотрудника.
                    string resultToPrint;

                    if (repository.IsWorkerFound)
                    {
                        PrintTitles();
                        resultToPrint = searchedWorker.PrintToConsole();
                    }
                    else
                        resultToPrint = $"Сотрудник с ID {id} не найден";


                    Console.WriteLine(resultToPrint);
                }
                else
                {
                    Console.Write("Вы ввели некорректные данные, попробовать снова? н/д\n");
                    key = Console.ReadKey(true).KeyChar;
                }

            } while (char.ToLower(key) == 'д');



        }

        /// <summary>
        /// Добавление нового сотрудника.
        /// </summary>
        private static void AddNewWorker()
        {
            char key = 'н';

            do      //Цикл do для создания нескольких сотрудников подряд
            {
                string note = String.Empty;

                int age;
                int height;
                DateTime birthDate;

                for (int index = 2; index < _workerTitles.Length; index++)      /*Цикл для формирования записи о сотруднике.
                                                                                Первые два индекса будут заполняться автоматически, поэтому начинается с 2.*/
                {
                    Console.Write($"{_workerTitles[index].TrimStart(' ')}: ");      //Вывод текущего заголовка, чтобы пользователь понимал, какие данные сейчас вводить.
                    note += $"{Console.ReadLine()}#";       //Считывание строки + добавление # в качестве знака разделителя.
                }

                string[] noteSplited = note.Split('#');     //Преобразование записи в массив с разделением по #.

                if (!int.TryParse(noteSplited[1], out age) ||
                    !int.TryParse(noteSplited[2], out height) ||
                    !DateTime.TryParse(noteSplited[3], out birthDate))      /*Минимальная проверка на корректность введённых данных.
                                                                            Если что-то не так, то данные не запишутся.*/
                {
                    Console.Write("Введены некорректные данные. Ещё раз? н/д\n"); key = Console.ReadKey(true).KeyChar;
                }
                else
                {
                    Worker newWorker = new Worker(noteSplited[0], age, height, birthDate, noteSplited[4]);      //Выполяется создание экземпляра структуры с заполнением данных.
                    repository.AddWorker(newWorker);
                    Console.Write("Сотрудник добавлен. Продожить н/д\n");
                    key = Console.ReadKey(true).KeyChar;
                }


            } while (char.ToLower(key) == 'д');
        }

        /// <summary>
        /// Удаление сотрудника.
        /// </summary>
        private static void DeleteWorker()
        {
            char key = 'н';

            do
            {
                Console.Write("Введите ID сотрудника для удаления: ");
                var stringId = Console.ReadLine();
                int id;

                if (int.TryParse(stringId, out id)) //Проверяем, что введено число.
                {
                    repository.DeleteWorker(id); //Удаление сотрудника по ID.
                }
                else
                {
                    Console.Write("Вы ввели некорректные данные, попробовать снова? н/д\n");
                    key = Console.ReadKey(true).KeyChar;
                }

            } while (char.ToLower(key) == 'д');


        }

        /// <summary>
        /// Вывод списка сотрудников в заданном диапазоне дат.
        /// </summary>
        private static void WorkersBetweenTwoDates()
        {
            DateTime dateFrom;
            DateTime dateTo;

            Console.Write("Введите дату с: ");
            DateTime.TryParse(Console.ReadLine(), out dateFrom);

            Console.Write("Введите дату до: ");
            DateTime.TryParse(Console.ReadLine(), out dateTo);

            List<Worker> workersBetweenTwoDates = repository.GetWorkersBetweenTwoDates(dateFrom, dateTo);       //Формируем список сотрудников, подходящих по условию.
            string resultToPrint;

            if (repository.IsWorkerFound)
            {
                PrintTitles();
                for (int i = 0; i < workersBetweenTwoDates.Count; i++)
                {
                    resultToPrint = workersBetweenTwoDates[i].PrintToConsole(); //Конвертируем список сотрудников в строку для вывода на экран.
                    Console.WriteLine(resultToPrint);
                }
            }
            else
            {
                resultToPrint = $"Не найдено сотрудников, созданных между {dateFrom.ToShortDateString()} и {dateTo.ToShortDateString()}";
                Console.WriteLine(resultToPrint);
            }
        }

        /// <summary>
        /// Выводит на экран заголовки.
        /// </summary>
        private static void PrintTitles()
        {
            foreach (var title in _workerTitles)
            {
                Console.Write(title);
            }

            Console.WriteLine("\n");
        }



    }
}
