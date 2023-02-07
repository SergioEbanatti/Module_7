using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module_7
{
    /// <summary>
    /// Сотрудник
    /// </summary>
    public struct Worker
    {
        #region Конструктор

        /// <summary>
        /// Конструктор с ручным заполнением всех полей.
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="createDateTime">Дата создания записи</param>
        /// <param name="fullName">ФИО</param>
        /// <param name="age">Возраст</param>
        /// <param name="height">Рост</param>
        /// <param name="birthDate">Дата рождения</param>
        /// <param name="birthPlace">Место рождения</param>
        public Worker (int id, DateTime createDateTime, string fullName, int age, int height, DateTime birthDate, string birthPlace)
        {
            Id = id;
            CreateDateTime = createDateTime;
            FullName = fullName;
            Age = age;
            Height = height;
            BirthDate = birthDate;
            BirthPlace = birthPlace;
        }

        /// <summary>
        /// Конструктор без указания ID и даты создания.
        /// </summary>
        /// <param name="fullName">ФИО</param>
        /// <param name="age">Возраст</param>
        /// <param name="height">Рост</param>
        /// <param name="birthDate">Дата рождения</param>
        /// <param name="birthPlace">Место рождения</param>
        public Worker(string fullName, int age, int height, DateTime birthDate, string birthPlace) :
            this(0, new DateTime(1900, 1, 1, 0, 0, 0), fullName, age, height, birthDate, birthPlace)
        {

        }

        #endregion

        #region Свойства

        /// <summary>
        /// ID сотрудника
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Дата и время добавления записи
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// ФИО сотрудника
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Возраст сотрудника
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Рост сотрудника
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Дата рождения сотрудника
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Место рождения сотрудника
        /// </summary>
        public string BirthPlace { get; set; }

        #endregion

        #region Методы

        /// <summary>
        /// Формирует строку данных о сотруднике в формате хранения в файле на диске.
        /// </summary>
        /// <returns>Строка данных о сотруднике, в качестве разделителя используется #</returns>
        public string PrintToWriteFile()
        {
            return $"{Id}#{CreateDateTime}#{FullName}#{Age}#{Height}#{BirthDate.ToShortDateString()}#{BirthPlace}";
        }

        /// <summary>
        /// Формирует строку данных о сотруднике в формате вывода на экран.
        /// </summary>
        /// <returns>Строка данных о сотруднике для вывода в консольное окно, в качестве разделителя используется форматированные пробелы</returns>
        public string PrintToConsole()
        {
            return $"{Id}{CreateDateTime, 30}{FullName, 40}{Age, 20}{Height, 20}{BirthDate.ToShortDateString(),25}{BirthPlace, 25}";
        }

        /// <summary>
        /// Формирует массив заголовков таблицы справочника для вывода на экран.
        /// </summary>
        /// <returns>Форматированный массив строк с заголовками полей сотрудника</returns>
        public static string[] WorkerMetaDataTitlesInit()
        {
            return new string[]
            {
                $"{"ID"}",
                $"{"Дата создания", 26}",
                $"{"ФИО", 31}",
                $"{"Возраст", 34}",
                $"{"Рост", 18}",
                $"{"Дата рождения", 26}",
                $"{"Место рождения", 26}",
            };
        }

        #endregion
    }
}
