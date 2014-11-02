﻿using System.Xml.Serialization;

namespace client.Config.Data
{
    public class QuerySequence
    {
        [XmlAttribute]
        public QuerySequenceMode Mode { get; set; }

        [XmlArrayItem(ElementName = "Query")]
        public QueryConfig[] List { get; set; }
    }

    public enum QuerySequenceMode
    {
        Sequential,
        Random,
        FromList
    }
}