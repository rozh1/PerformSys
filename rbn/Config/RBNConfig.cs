using System;
using System.IO;
using System.Xml.Serialization;
using rbn.Config.Data;

namespace rbn.Config
{
    public class RBNConfig
    {
        [XmlIgnore]
        public static RBNConfig Instance;

        public Data.RBN RBN { get; set; }

        public Data.MRBN MRBN { get; set; }

        public Log Log { get; set; }

        /// <summary>
        ///     Конфиг серверов
        /// </summary>
        public Data.Server Server { get; set; }

        public void Save(string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                var serializer = new XmlSerializer(typeof(RBNConfig));
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }

        public static RBNConfig Load(string fileName)
        {
            using (FileStream stream = File.OpenRead(fileName))
            {
                return Load(stream);
            }
        }

        public static RBNConfig Load(Stream fileStream)
        {
            var serializer = new XmlSerializer(typeof(RBNConfig));
            try
            {
                Instance = serializer.Deserialize(fileStream) as RBNConfig;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Instance;
        }
    }
}