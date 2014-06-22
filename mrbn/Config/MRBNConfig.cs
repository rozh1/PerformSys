using System;
using System.IO;
using System.Xml.Serialization;
using Balancer.Common;
using mrbn.Config.Data;

namespace mrbn.Config
{
    public class MRBNConfig
    {
        [XmlIgnore]
        public static MRBNConfig Instance;

        public MRBN MRBN { get; set; }

        /// <summary>
        ///     Конфиг серверов
        /// </summary>
        public RBN[] RBNs { get; set; }

        public void Save(string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                var serializer = new XmlSerializer(typeof(MRBNConfig));
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }

        public static MRBNConfig Load(string fileName)
        {
            using (FileStream stream = File.OpenRead(fileName))
            {
                return Load(stream);
            }
        }

        public static MRBNConfig Load(Stream fileStream)
        {
            var serializer = new XmlSerializer(typeof(MRBNConfig));
            try
            {
                Instance = serializer.Deserialize(fileStream) as MRBNConfig;
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
            }
            return Instance;
        }
    }
}