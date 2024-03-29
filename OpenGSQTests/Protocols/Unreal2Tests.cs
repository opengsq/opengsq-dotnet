﻿using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class Unreal2Tests : TestBase
    {
        public Unreal2 unreal2 = new("80.4.151.145", 7778);

        public Unreal2Tests() : base(typeof(Unreal2Tests))
        {
            // EnableSave = true;
        }

        [TestMethod()]
        public async Task GetDetailsTest()
        {
            SaveResult(nameof(GetDetailsTest), await unreal2.GetDetails());
        }

        [TestMethod()]
        public async Task GetRulesTest()
        {
            SaveResult(nameof(GetRulesTest), await unreal2.GetRules());
        }

        [TestMethod()]
        public async Task GetPlayersTest()
        {
            SaveResult(nameof(GetPlayersTest), await unreal2.GetPlayers());
        }
    }
}