using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Module_7
{
    /// <summary>
    /// Класс для работы со справочником "Сотрудники". Взаимодействует с экземплярами Worker и файлом на диске.
    /// </summary>
    public class Repository
    {
        private static string[] _workerTitles = Worker.WorkerMetaDataTitlesInit(); //Массив заголовков таблицы справочника.

        private Worker[] _workers; // Основной массив для хранения данных.
        private int _linesCount = 0; //Текущее количество записей в файле на диске.
        private bool _isWorkerFound;
        private bool _isWorkerDeleted;

        #region Свойства
        /// <summary>
        /// Имя и путь файла с данными.
        /// </summary>
        public string RepositoryPathName { get; set; }
        #endregion

        #region Конструктор
        /// <summary>
        /// Создание репозиория с указанием пути и имени файла.
        /// </summary>
        /// <param name="repositoryPathName">Путь и имя файла</param>
        public Repository(string repositoryPathName)
        {
            RepositoryPathName = repositoryPathName;
            _workers = new Worker[1];
            _workers = GetAllWorkers(); //Начальное заполнение массива данными.
        }
        #endregion

        #region Методы

        /// <summary>
        /// Выводит на экран таблицу данных обо всех сотрудниках.
        /// </summary>
        public void PrintAllWorkersToConsole()
        {
            Console.WriteLine($"\n{_workerTitles[0]}" +
                $" {_workerTitles[1]}" +
                $" {_workerTitles[2]}" +
                $" {_workerTitles[3]}" +
                $" {_workerTitles[4]}" +
                $" {_workerTitles[5]}" +
                $" {_workerTitles[6]}\n");  //Вывод заголовков.

            Worker[] workersToPrint = GetAllWorkers(); //Формируем массив сотрудников для вывода.
            Array.Resize(ref workersToPrint, _linesCount);

            var list = workersToPrint.ToList();
            //var orderedElem = workersToPrint.OrderBy(w => w.FullName);
            list.Sort(delegate (Worker one, Worker two)
            {
                if (one.FullName == null && two.FullName == null) return 0;
                else if (one.FullName == null) return -1;
                else if (two.FullName == null) return 1;
                else return one.FullName.CompareTo(two.FullName);
            });

            //workersToPrint = list.ToArray();


            for (int i = 0; i < _linesCount; i++)
            {
                string resultToPrint = list[i].PrintToConsole();  //Конвертируем экземпляр Worker в строку для вывода на экран.
                Console.WriteLine(resultToPrint);
            }

            for (int i = 0; i < _linesCount; i++)
            {
                
            }

        }

        /// <summary>
        /// Выводит на экран искомого сотрудника.
        /// </summary>
        /// <param name="id">ID, который требуется найти</param>
        public void PrintSearchedWorkerToConsole(int id)
        {
            Worker searchedWorker = GetWorkerById(id); //Выполняем поиск сотрудника.
            string resultToPrint;

            if (_isWorkerFound)
            {
                Console.WriteLine($"\n{_workerTitles[0]}" +
                    $" {_workerTitles[1]}" +
                    $" {_workerTitles[2]}" +
                    $" {_workerTitles[3]}" +
                    $" {_workerTitles[4]}" +
                    $" {_workerTitles[5]}" +
                    $" {_workerTitles[6]}\n");  //Вывод заголовков.

                resultToPrint = searchedWorker.PrintToConsole();
            }
            else
            {
                resultToPrint = $"Сотрудник с ID {id} не найден";
            }

            Console.WriteLine(resultToPrint);

        }

        /// <summary>
        /// Выводит на экран записи за заданный период времени.
        /// </summary>
        /// <param name="dateFrom">С даты</param>
        /// <param name="dateTo">До даты</param>
        public void PrintWorkersBetweenTwoDates(DateTime dateFrom, DateTime dateTo)
        {
            Worker[] workersBetweenTwoDates = GetWorkersBetweenTwoDates(dateFrom, dateTo);  //Формируем массив данных.
            string resultToPrint;

            if (_isWorkerFound)
            {
                Console.WriteLine($"{_workerTitles[0]}" +
                    $" {_workerTitles[1],25}" +
                    $" {_workerTitles[2],25}" +
                    $" {_workerTitles[3],25}" +
                    $" {_workerTitles[4],15}" +
                    $" {_workerTitles[5],25}" +
                    $" {_workerTitles[6],25}\n");   //Вывод заголовков.

                for (int i = 0; i < workersBetweenTwoDates.Length; i++)
                {
                    resultToPrint = workersBetweenTwoDates[i].PrintToConsole(); //Конвертируем массив Worker[] в строку для вывода на экран.
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
        /// Добавление нового сотрудника.
        /// </summary>
        /// <param name="worker">Добавляемый сотрудник</param>
        public void AddWorker(Worker worker)
        {
            using (StreamWriter streamWriter = new StreamWriter(RepositoryPathName, true))
            {
                worker.Id = _workers[_linesCount - 1].Id + 1;   /*Формируем ID на основе ID последнего записанного сотрудника.
                                                                _linesCount - 1 т.к. счётчик больше индекса на 1.*/
                worker.CreateDateTime = DateTime.Now;
                string note = worker.PrintToWriteFile();    //Конвертируем Worker в строку, для записи в файл.
                streamWriter.WriteLine(note);
                _linesCount++;
            }
        }

        /// <summary>
        /// Удаление сотрудника по ID.
        /// </summary>
        /// <param name="id">ID удаляемого сотрудника</param>
        public void DeleteWorker(int id)
        {
            var workersList = _workers.ToList();    //Конвертируем массив в список для удобства.

            for (int i = 0; i < _linesCount; i++)
            {
                if (workersList[i].Id == id)    //Обходим список и сравниваем с искомым ID.
                {
                    workersList.RemoveAt(i);    //Удаляем элемент.
                    _workers = workersList.ToArray();  //действие под вопросом??
                    _linesCount--;
                    _isWorkerDeleted = true;
                    break;

                }
            }


            if (_isWorkerDeleted)
            {
                string note = String.Empty;

                for (int i = 0; i < _linesCount; i++)
                    note += $"{workersList[i].PrintToWriteFile()}\n";   /*Обходим список и конвертируем его в строчный тип записи, согласно формату хранения на диске,
                                                                        в каждой строке данные по одному сотруднику.*/

                string[] resultString = note.Split('\n'); //    Разделяем записи построчно и записываем каждую строку в отдельный элемент массива.
                Array.Resize(ref resultString, resultString.Length - 1);    //Удаляем последний элемент массива т.к. там пустая строка.
                File.WriteAllLines(RepositoryPathName, resultString);   //Перезаписываем файл.
                Console.WriteLine($"Сотрудник с ID {id} удалён");
            }
            else
            {
                Console.WriteLine($"Сотрудник с ID {id} не найден");
            }

        }

        /// <summary>
        /// Считывает все записи из файла на диске и записывает их в массив.
        /// </summary>
        /// <returns>массив экземпляров Worker</returns>
        private Worker[] GetAllWorkers()
        {

            if (IsRepositoryExist())
            {
                using (StreamReader streamReader = new StreamReader(RepositoryPathName))
                {
                    _linesCount = 0; //Сбрасываем счётчик строк т.к. будем считывать все записи из файла.

                    while (!streamReader.EndOfStream)
                    {
                        string[] workerLine = streamReader.ReadLine().Split('#'); //Считываем одну строку и разбиваем её на элементы разделителем #.

                        _linesCount++;
                        Resize(_linesCount >= _workers.Length);

                        _workers[_linesCount - 1] = new Worker(
                            Convert.ToInt32(workerLine[0]),
                            Convert.ToDateTime(workerLine[1]),
                            workerLine[2],
                            Convert.ToInt32(workerLine[3]),
                            Convert.ToInt32(workerLine[4]),
                            Convert.ToDateTime(workerLine[5]),
                            workerLine[6]); //Конвертируем массив строк в элемент массива Worker[].

                        //_linesCount++;

                    }
                    //_linesCount--;
                }


            }

            return _workers;

        }

        /// <summary>
        /// Происходит чтение из файла, возвращается Worker с запрашиваемым ID.
        /// </summary>
        /// <param name="id">ID, который требуется найти</param>
        /// <returns>Искомый экземпляр Worker</returns>
        private Worker GetWorkerById(int id)
        {
            Worker searchedWorker;

            foreach (var item in _workers)  //Ищем сотрудника в хранилище.
            {
                if (item.Id == id)
                {
                    _isWorkerFound = true;
                    return searchedWorker = item;
                }
            }

            _isWorkerFound = false;
            return new Worker();    //Если сотрудник не найден, возвращаем пустую запись.


        }

        /// <summary>
        /// Получет данные о сотрудниках за определённый период времени.
        /// </summary>
        /// <param name="dateFrom">С даты</param>
        /// <param name="dateTo">До даты</param>
        /// <returns>Массив Worker[] за искомый период</returns>
        private Worker[] GetWorkersBetweenTwoDates(DateTime dateFrom, DateTime dateTo)
        {

            Worker[] workersBetweenTwoDates = new Worker[_linesCount];  /*Инициализируем дополнительный массив элементов в который будут записаны искомые данные
                                                                        Длинну массива пока указываем по счётчику записей из файла*/

            int additionalIndex = 0;    //Счётчик дополнительного массива
            _isWorkerFound = false;

            for (int mainIndex = 0; mainIndex < _linesCount; mainIndex++)   //Обходим основной массив и ищем нужные элементы.
            {
                if (_workers[mainIndex].CreateDateTime >= dateFrom && _workers[mainIndex].CreateDateTime <= dateTo)
                {
                    workersBetweenTwoDates[additionalIndex] = _workers[mainIndex];  //Записываем в дополнительный массив найденный элемент из основного массива.
                    _isWorkerFound = true;
                    additionalIndex++;
                }
            }

            Array.Resize(ref workersBetweenTwoDates, additionalIndex);  //Меняем размер массива по счётчику дополнительного массива.

            return workersBetweenTwoDates;
        }

        /// <summary>
        /// Увеличиваем, если требуется, размер массива данных.
        /// </summary>
        /// <param name="Flag">Условие увеличения</param>
        private void Resize(bool Flag)
        {
            if (Flag)
            {
                Array.Resize(ref _workers, _workers.Length * 2); //Увеличиваем размер массива в 2 раза.
            }
        }

        /// <summary>
        /// Создан ли файл?
        /// </summary>
        /// <returns></returns>
        private bool IsRepositoryExist()
        {
            if (File.Exists(RepositoryPathName))
            {
                return true;
            }

            Console.WriteLine("Сотрудники отсутствуют");
            return false;
        }
        #endregion
    }
}
