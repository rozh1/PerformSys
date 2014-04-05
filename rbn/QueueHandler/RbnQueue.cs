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

﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Balancer.Common;
using Balancer.Common.Packet;
using rbn.ServersHandler;

namespace rbn.QueueHandler
{
    static class RbnQueue
    {
        /// <summary>
        /// Список клиентов (очередь выполнения)
        /// </summary>
        private static readonly List<Client> Clients  = new List<Client>();

        private static int id = 1;

        /// <summary>
        /// Добавление клиента в конец
        /// </summary>
        /// <param name="client">клиент</param>
        static public void AddClient(Client client)
        {
            if (Clients.Contains(client))
            {
                Client tmpClient = Clients[Clients.IndexOf(client)];
                tmpClient.Query = client.Query;
                tmpClient.AnswerPacketData = "";
            }
            else
            {
                client.Id = id++;
                Clients.Add(client);
            }
        }

        /// <summary>
        /// Удаление клиента
        /// </summary>
        /// <param name="client">клиент</param>
        public static void RemoveClient(Client client)
        {
            if (Clients.Contains(client))
            {
                Clients.Remove(client);
            }
        }

        /// <summary>
        /// Ответ сервера
        /// </summary>
        /// <param name="clientId">номер клиента</param>
        /// <param name="answer">пакет ответа</param>
        public static void ServerAnswer(int clientId, string answer)
        {
            try
            {
                int count = Clients.Count;
                for (int i = 0; i < count; i++)
                {
                    if (Clients[i].Id == clientId)
                    {
                        SendAnswer(Clients[i], answer);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Исключение при передаче ответа клиенту" + ex.Message);
            }
        }

        /// <summary>
        /// отправка ответа клиенту
        /// </summary>
        /// <param name="client">клиент</param>
        /// <param name="answer">пакет ответа</param>
        static void SendAnswer(Client client, string answer)
        {
            var packet = new Packet(PacketType.Answer,answer);
            byte[] bytes = packet.ToBytes();
            client.AnswerPacketData = answer;
            if (client.Connection.Connected)
                client.Connection.GetStream().Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Выбор клиента и отправка его запроса на сервер
        /// </summary>
        static public void SendRequestToServer()
        {
            int count = Clients.Count;
            for (int i = 0; i < count; i++)
            {
                if (!Clients[i].QuerySended) 
                    if(!SendRequest(Clients[i])) break;
            }
        }

        /// <summary>
        /// Отправка запроса серверу
        /// </summary>
        /// <param name="client">клиент</param>
        static bool SendRequest(Client client)
        {
            ServersHandler.Server server = Servers.GetNextReadyServer();
            if (server != null)
            {
                Servers.SendRequest(server, client.Query, client.Id);
                client.QuerySended = true;
            }
            else return false;
            return true;
        }
    }
}
