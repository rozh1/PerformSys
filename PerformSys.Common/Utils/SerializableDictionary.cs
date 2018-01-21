#region Copyright
/*
 * Copyright 2013-2018 Roman Klassen
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 */
#endregion

﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PerformSys.Common.Utils
{
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        /// <summary>
        ///     The default XML tag name for an item.
        /// </summary>
        private const string Defaultitemtag = "item";

        /// <summary>
        ///     The default XML tag name for a key.
        /// </summary>
        private const string Defaultkeytag = "key";

        /// <summary>
        ///     The default XML tag name for a value.
        /// </summary>
        private const string Defaultvaluetag = "value";

        /// <summary>
        ///     The XML serializer for the key type.
        /// </summary>
        private static readonly XmlSerializer KeySerializer = new XmlSerializer(typeof (TKey));

        /// <summary>
        ///     The XML serializer for the value type.
        /// </summary>
        private static readonly XmlSerializer ValueSerializer = new XmlSerializer(typeof (TValue));

        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="SerializableDictionary&lt;TKey, TValue&gt;" /> class.
        /// </summary>
        public SerializableDictionary()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="SerializableDictionary&lt;TKey, TValue&gt;" /> class.
        /// </summary>
        /// <param name="info">
        ///     A
        ///     <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object
        ///     containing the information required to serialize the
        ///     <see cref="T:System.Collections.Generic.Dictionary`2" />.
        /// </param>
        /// <param name="context">
        ///     A
        ///     <see cref="T:System.Runtime.Serialization.StreamingContext" /> structure
        ///     containing the source and destination of the serialized stream
        ///     associated with the
        ///     <see cref="T:System.Collections.Generic.Dictionary`2" />.
        /// </param>
        protected SerializableDictionary(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        ///     Gets the XML tag name for an item.
        /// </summary>
        protected virtual string ItemTagName
        {
            get { return Defaultitemtag; }
        }

        /// <summary>
        ///     Gets the XML tag name for a key.
        /// </summary>
        protected virtual string KeyTagName
        {
            get { return Defaultkeytag; }
        }

        /// <summary>
        ///     Gets the XML tag name for a value.
        /// </summary>
        protected virtual string ValueTagName
        {
            get { return Defaultvaluetag; }
        }

        /// <summary>
        ///     Gets the XML schema for the XML serialization.
        /// </summary>
        /// <returns>An XML schema for the serialized object.</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        ///     Deserializes the object from XML.
        /// </summary>
        /// <param name="reader">The XML representation of the object.</param>
        public void ReadXml(XmlReader reader)
        {
            bool wasEmpty = reader.IsEmptyElement;

            reader.Read();

            if (wasEmpty)
            {
                return;
            }

            try
            {
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.ReadStartElement(ItemTagName);
                    try
                    {
                        TKey key;
                        TValue value;

                        reader.ReadStartElement(KeyTagName);
                        try
                        {
                            key = (TKey) KeySerializer.Deserialize(reader);
                        }
                        finally
                        {
                            reader.ReadEndElement();
                        }

                        reader.ReadStartElement(ValueTagName);
                        try
                        {
                            value = (TValue) ValueSerializer.Deserialize(reader);
                        }
                        finally
                        {
                            reader.ReadEndElement();
                        }

                        Add(key, value);
                    }
                    finally
                    {
                        reader.ReadEndElement();
                    }

                    reader.MoveToContent();
                }
            }
            finally
            {
                reader.ReadEndElement();
            }
        }

        /// <summary>
        ///     Serializes this instance to XML.
        /// </summary>
        /// <param name="writer">The writer to serialize to.</param>
        public void WriteXml(XmlWriter writer)
        {
            foreach (var keyValuePair in this)
            {
                writer.WriteStartElement(ItemTagName);
                try
                {
                    writer.WriteStartElement(KeyTagName);
                    try
                    {
                        KeySerializer.Serialize(writer, keyValuePair.Key);
                    }
                    finally
                    {
                        writer.WriteEndElement();
                    }

                    writer.WriteStartElement(ValueTagName);
                    try
                    {
                        ValueSerializer.Serialize(writer, keyValuePair.Value);
                    }
                    finally
                    {
                        writer.WriteEndElement();
                    }
                }
                finally
                {
                    writer.WriteEndElement();
                }
            }
        }
    }
}