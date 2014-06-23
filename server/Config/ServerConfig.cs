using System;
using System.IO;
using System.Xml.Serialization;
using Balancer.Common;

namespace server.Config
{
    public class ServerConfig
    {
        [XmlIgnore] public static ServerConfig Instance;

        /// <summary>
        ///     Конфиг базы данных
        /// </summary>
        public Data.DataBase DataBase { get; set; }

        /// <summary>
        ///     Конфиг сервера
        /// </summary>
        public Data.Server Server { get; set; }
        
        /// <summary>
        ///     Конфиг времен выполения запросов для режима симуляции
        /// </summary>
        public Data.SimulationParams SimulationParams { get; set; }

        public void Save(string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                var serializer = new XmlSerializer(typeof (ServerConfig));
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }

        public static ServerConfig Load(string fileName)
        {
            using (FileStream stream = File.OpenRead(fileName))
            {
                return Load(stream);
            }
        }

        public static ServerConfig Load(Stream fileStream)
        {
            var serializer = new XmlSerializer(typeof (ServerConfig));
            try
            {
                Instance = serializer.Deserialize(fileStream) as ServerConfig;
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
            }
            return Instance;
        }
    }
}