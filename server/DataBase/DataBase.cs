using System;
using System.Data;
using System.Globalization;
using Balancer.Common;

namespace server.DataBase
{
    /// <summary>
    ///     Работа с БД
    /// </summary>
    public static class Database
    {
        private static MySqlDb _mysqldb;

        private static Config.Data.DataBaseType _database;

        private static NumberFormatInfo _nfi;

        public static event Action ConnectionError;

        /// <summary>
        ///     Инициализация класса
        /// </summary>
        /// <returns>true в случае успеха</returns>
        public static bool Init()
        {
            _database = Config.ServerConfig.Instance.DataBase.DataBaseType;
            _nfi = new NumberFormatInfo {NumberDecimalSeparator = "."};
            switch (_database)
            {
                case Config.Data.DataBaseType.MySQL:
                    _mysqldb = new MySqlDb(
                        Config.ServerConfig.Instance.DataBase.UserName,
                        Config.ServerConfig.Instance.DataBase.Password,
                        Config.ServerConfig.Instance.DataBase.DataBaseName,
                        Config.ServerConfig.Instance.DataBase.Host,
                        Config.ServerConfig.Instance.DataBase.Port);
                    _mysqldb.ConnectionError += mysqldb_ConnectionError;
                    return _mysqldb.MySqlConnectionOpen();
            }
            return false;
        }

        private static void mysqldb_ConnectionError()
        {
            if (ConnectionError != null) ConnectionError();
        }

        /// <summary>
        ///     Проверка инициализации
        /// </summary>
        private static void CheckInit()
        {
            switch (_database)
            {
                case Config.Data.DataBaseType.MySQL:
                    if (_mysqldb == null) Init();
                    break;
            }
        }


        /// <summary>
        ///     Запрос Вставки
        /// </summary>
        /// <param name="query">Запрос</param>
        private static void InsertQuery(string query)
        {
            CheckInit();
            //Logger.Write("Запрос на запись к базе: " + query, 7);
            switch (_database)
            {
                case Config.Data.DataBaseType.MySQL:
                    _mysqldb.Insert(query);
                    break;
            }
        }

        /// <summary>
        ///     Запрос выборки из БД
        /// </summary>
        /// <param name="query">запрос</param>
        /// <returns>результаты запроса</returns>
        private static DataSet SelectQuery(string query)
        {
            CheckInit();
            //Logger.Write("Запрос на чтение к базе: " + query, 7);
            switch (_database)
            {
                case Config.Data.DataBaseType.MySQL:
                    return _mysqldb.Select(query);
            }
            return null;
        }

        /// <summary>
        ///     Запрос на обновление в БД
        /// </summary>
        /// <param name="query">запрос</param>
        private static void UpdateQuery(string query)
        {
            CheckInit();
            //Logger.Write("Запрос на обновление к базе: " + query, 7);
            int updatedRows = 0;
            switch (_database)
            {
                case Config.Data.DataBaseType.MySQL:
                    updatedRows = _mysqldb.Update(query);
                    break;
            }
            Logger.Write("В результате измененно " + updatedRows + " строк", 7);
        }

        /// <summary>
        ///     Завпрос удаления из БД
        /// </summary>
        /// <param name="query">запрос</param>
        private static void DeleteQuery(string query)
        {
            CheckInit();
            Logger.Write("Запрос на удаление к базе: " + query, 7);
            int deletedRows = 0;
            switch (_database)
            {
                case Config.Data.DataBaseType.MySQL:
                    deletedRows = _mysqldb.Delete(query);
                    break;
            }
            Logger.Write("В результате измененно " + deletedRows + " строк", 7);
        }

        /// <summary>
        ///     Произвольный запрос
        /// </summary>
        /// <param name="query">запрос</param>
        /// <returns>результат select или null</returns>
        public static DataTable CustomQuery(string query)
        {
            string str = query.Substring(0, 10);
            if (str.Contains("SELECT") || str.Contains("select") || str.Contains("Select"))
                return SelectQuery(query).Tables[0];
            InsertQuery(query);
            return null;
        }
    }
}