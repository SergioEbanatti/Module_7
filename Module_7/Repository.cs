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
        #region Поля

        private List<Worker> _workers;      // Основной список для хранения данных.

        #endregion

        #region Свойства

        /// <summary>
        /// Имя и путь файла с данными.
        /// </summary>
        public string RepositoryPathName { get; set; }      //Путь и имя файла на диске.
        public int LinesCount { get; set; }     //Текущее количество записей в файле на диске.
        public bool IsWorkerFound { get; set; }
        public bool IsWorkerDeleted { get; set; }

        #endregion

        #region Конструктор

        /// <summary>
        /// Создание репозиория с указанием пути и имени файла.
        /// </summary>
        /// <param name="repositoryPathName">Путь и имя файла</param>
        public Repository(string repositoryPathName)
        {
            RepositoryPathName = repositoryPathName;
            _workers = new List<Worker>();
            _workers = GetAllWorkers();     //Начальное заполнение списка данными.
        }

        #endregion

        #region Методы

        /// <summary>
        /// Считывает все записи из файла на диске и записывает их в список.
        /// </summary>
        /// <returns>Список экземпляров Worker</returns>
        public List<Worker> GetAllWorkers()
        {
            if (IsRepositoryExist())
            {
                using (StreamReader streamReader = new StreamReader(RepositoryPathName))
                {
                    LinesCount = 0;     //Сбрасываем счётчик строк т.к. будем считывать все записи из файла.
                    _workers.Clear();

                    while (!streamReader.EndOfStream)
                    {
                        string[] workerLine = streamReader.ReadLine().Split('#');       //Считываем одну строку и разбиваем её на элементы разделителем #.

                        LinesCount++;
                        _workers.Add(new Worker(
                            Convert.ToInt32(workerLine[0]),
                            Convert.ToDateTime(workerLine[1]),
                            workerLine[2],
                            Convert.ToInt32(workerLine[3]),
                            Convert.ToInt32(workerLine[4]),
                            Convert.ToDateTime(workerLine[5]),
                            workerLine[6]));        //Конвертируем массив строк в элемент списка Worker.
                    }
                }
            }
            return _workers;
        }

        /// <summary>
        /// Происходит чтение из файла, возвращается Worker с запрашиваемым ID.
        /// </summary>
        /// <param name="id">ID, который требуется найти</param>
        /// <returns>Искомый экземпляр Worker</returns>
        public Worker GetWorkerById(int id)
        {
            Worker searchedWorker;
            foreach (var item in _workers)      //Ищем сотрудника в списке.
            {
                if (item.Id == id)
                {
                    IsWorkerFound = true;
                    return searchedWorker = item;
                }
            }
            IsWorkerFound = false;
            return new Worker();        //Если сотрудник не найден, возвращаем пустую запись.
        }

        /// <summary>
        /// Добавление нового сотрудника.
        /// </summary>
        /// <param name="worker">Добавляемый сотрудник</param>
        public void AddWorker(Worker worker)
        {
            using (StreamWriter streamWriter = new StreamWriter(RepositoryPathName, true))
            {
                worker.Id = _workers.Last().Id + 1;   //Формируем ID на основе ID последнего записанного сотрудника.        
                worker.CreateDateTime = DateTime.Now;       //Дата создания формируется на основании текущей даты.
                _workers.Add(worker);       //Записываем сотрудника в список.
                LinesCount++;
                string note = worker.PrintToWriteFile();    //Конвертируем Worker в строку, для записи в файл.
                streamWriter.WriteLine(note);
            }
        }

        /// <summary>
        /// Удаление сотрудника по ID.
        /// </summary>
        /// <param name="id">ID удаляемого сотрудника</param>
        public void DeleteWorker(int id)
        {
            IsWorkerDeleted = false;

            for (int i = 0; i < LinesCount; i++)
            {
                if (_workers[i].Id == id)    //Обходим список и сравниваем с искомым ID.
                {
                    _workers.RemoveAt(i);    //Удаляем элемент.
                    LinesCount--;
                    IsWorkerDeleted = true;
                    break;
                }
            }

            if (IsWorkerDeleted)
            {
                string note = String.Empty;

                for (int i = 0; i < LinesCount; i++)
                    note += $"{_workers[i].PrintToWriteFile()}\n";   /*Обходим список и конвертируем его в строчный тип записи, согласно формату хранения на диске,
                                                                        в каждой строке данные по одному сотруднику.*/

                string[] resultString = note.Split('\n'); //    Разделяем записи построчно и записываем каждую строку в отдельный элемент массива.
                Array.Resize(ref resultString, resultString.Length - 1);    //Удаляем последний элемент массива т.к. там пустая строка.
                File.WriteAllLines(RepositoryPathName, resultString);   //Перезаписываем файл.
            }

        }

        /// <summary>
        /// Получет данные о сотрудниках за определённый период времени.
        /// </summary>
        /// <param name="dateFrom">С даты</param>
        /// <param name="dateTo">До даты</param>
        /// <returns>Массив Worker[] за искомый период</returns>
        public List<Worker> GetWorkersBetweenTwoDates(DateTime dateFrom, DateTime dateTo)
        {
            List<Worker> workersBetweenTwoDates = new List<Worker>();       //Создаем дополнительный список, в который будут помещены подходящие по условию сотрудники.
            IsWorkerFound = false;

            for (int i = 0; i < LinesCount; i++)   //Обходим основной список и ищем нужные элементы.
            {
                if (_workers[i].CreateDateTime >= dateFrom && _workers[i].CreateDateTime <= dateTo)
                {
                    workersBetweenTwoDates.Add(_workers[i]);        //Записываем в дополнительный список найденный элемент из основного списка.
                    IsWorkerFound = true;
                }
            }
            return workersBetweenTwoDates;
        }

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
