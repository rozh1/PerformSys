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
using System.Linq;
using System.Net.Sockets;
using Balancer.Common.Packet;

namespace rbn.QueueHandler
{
    static class RbnQueue
    {
        private static List<Client> clients  = new List<Client>();

        //static public void Init()
        //{
        //    clients
        //}

        static public void AddClient(Client client)
        {
            if (clients.Contains(client))
            {
                Client tmpClient = clients[clients.IndexOf(client)];
                tmpClient.Query = client.Query;
                tmpClient.AnswerPacketData = "";
            }
            else
            {
                clients.Add(client);
            }
        }

        public static void RemoveClient(Client client)
        {
            if (clients.Contains(client))
            {
                clients.Remove(client);
            }
        }

        public static Client GetClientByTcpConnection(TcpClient tcpClient)
        {
            int count = clients.Count;
            for (int i = 0; i < count; i++)
            {
                if (clients[i].Connection == tcpClient) return clients[i];
            }
            return null;
        }

        public static void ServerAnswer(int clientId, string answer)
        {
            SendAnswer(clients[clientId-1], answer);
        }

        static void SendAnswer(Client client, string answer)
        {
            var packet = new Packet(PacketType.Answer,answer);
            byte[] bytes = packet.ToBytes();
            client.AnswerPacketData = answer;
            if (client.Connection.Connected)
                client.Connection.GetStream().Write(bytes, 0, bytes.Length);
        }
    }
}
