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

namespace client
{
    internal class Program
    {
        private static DateTime _startTime;

        private static void Main(string[] args)
        {
            int count = 1;
            int port = 3409;
            int.TryParse(args[0], out count);
            int.TryParse(args[2], out port);

            //Random random = new Random();

            var clients = new Client[count];

            _startTime = DateTime.Now;

            for (int i = 0; i < count; i++)
            {
                clients[i] = new Client(args[1], port, i, (i%14) + 1); //random.Next(1,5));
                clients[i].EndWork += EndWorkClient;
            }
        }

        private static void EndWorkClient()
        {
            Console.WriteLine(@"Время работы: " + (DateTime.Now - _startTime));
        }
    }
}