using System;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace client.Config.Data
{
    public class ScenarioStep
    {
        [XmlIgnore]
        public TimeSpan Duration { get; set; }

        // XmlSerializer does not support TimeSpan, so use this property for 
        // serialization instead.
        [Browsable(false)]
        [XmlAttribute(DataType = "duration", AttributeName = "Duration")]
        public string TimeSinceLastEventString
        {
            get { return XmlConvert.ToString(Duration); }
            set
            {
                Duration = string.IsNullOrEmpty(value)
                    ? TimeSpan.Zero
                    : XmlConvert.ToTimeSpan(value);
            }
        }

        [XmlAttribute]
        public ScenarioActions Action { get; set; }
    }
}