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
using client.ComandLineParamsParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace client.Test.ComandLineParamsParser
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void FullParamsParserTest()
        {
            client.ComandLineParamsParser.Parser parser = new Parser(
                "--clients 10 --host localhost --port 3409 --queries 1".Split(' '));
            
            Config.Config config = parser.GetConfig();

            Assert.AreEqual(config.BalancerHost, "localhost");
            Assert.AreEqual(config.BalancerPort, 3409);
        }

        [TestMethod]
        public void LiteParamsParserTest()
        {
            client.ComandLineParamsParser.Parser parser = new Parser(
                "-c 10 -h localhost -p 3409 -q 1".Split(' '));

            Config.Config config = parser.GetConfig();

            Assert.AreEqual(config.BalancerHost, "localhost");
            Assert.AreEqual(config.BalancerPort, 3409);
        }

        [TestMethod]
        public void NoParamsParserTest()
        {
            client.ComandLineParamsParser.Parser parser = new Parser(
                new string[1]);

            Config.Config config = parser.GetConfig();

            Assert.IsNull(config.BalancerHost);
            Assert.IsNull(config.BalancerPort);
        }

        [TestMethod]
        public void SemiParamsParserTest()
        {
            client.ComandLineParamsParser.Parser parser = new Parser(
                   "-c 10 -p 3409".Split(' '));

            Config.Config config = parser.GetConfig();

            Assert.AreEqual(config.BalancerPort, 3409);
            Assert.IsNull(config.BalancerHost);
        }
    }
}
