using System;
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
            Assert.AreEqual(config.ClientCount, 10);
            Assert.AreEqual(config.QueryCount, 1);
        }

        [TestMethod]
        public void LiteParamsParserTest()
        {
            client.ComandLineParamsParser.Parser parser = new Parser(
                "-c 10 -h localhost -p 3409 -q 1".Split(' '));

            Config.Config config = parser.GetConfig();

            Assert.AreEqual(config.BalancerHost, "localhost");
            Assert.AreEqual(config.BalancerPort, 3409);
            Assert.AreEqual(config.ClientCount, 10);
            Assert.AreEqual(config.QueryCount, 1);
        }

        [TestMethod]
        public void NoParamsParserTest()
        {
            client.ComandLineParamsParser.Parser parser = new Parser(
                new string[1]);

            Config.Config config = parser.GetConfig();

            Assert.IsNull(config.BalancerHost);
            Assert.IsNull(config.BalancerPort);
            Assert.IsNull(config.ClientCount);
            Assert.IsNull(config.QueryCount);
        }

        [TestMethod]
        public void SemiParamsParserTest()
        {
            client.ComandLineParamsParser.Parser parser = new Parser(
                   "-c 10 -p 3409".Split(' '));

            Config.Config config = parser.GetConfig();

            Assert.AreEqual(config.BalancerPort, 3409);
            Assert.AreEqual(config.ClientCount, 10);
            Assert.IsNull(config.BalancerHost);
            Assert.IsNull(config.QueryCount);
        }
    }
}
