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

﻿using Balancer.Common.Packet.Packets;

namespace rbn.QueueHandler.Data
{
    public class QueueEntity
    {
        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        public int ClientId {
            get { return (int)RequestPacket.ClientId; } 
        }

        /// <summary>
        /// Пакет запроса к БД
        /// </summary>
        public DbRequestPacket RequestPacket { get; set; }

        /// <summary>
        /// Объем отношений для обработки запроса
        /// </summary>
        public double RelationVolume { get; set; }
    }
}