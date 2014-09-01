using System;
using System.IO;
using System.Xml.Serialization;

namespace Balancer.Common.Config
{
    /// <summary>
    /// Базовый класс конфигурации
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConfigBase<T>
    {
        [XmlIgnore]
        public static T Instance;

        public void Save(string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }

        public static T Load(string fileName)
        {
            using (FileStream stream = File.OpenRead(fileName))
            {
                return Load(stream);
            }
        }

        public static T Load(Stream fileStream)
        {
            var serializer = new XmlSerializer(typeof(T));
            try
            {
                Instance = (T)serializer.Deserialize(fileStream);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Instance;
        }
    }
}
