using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class MinecraftTests : TestBase
    {
        public Minecraft minecraft = new("mc.goldcraft.ir", 25565);

        public MinecraftTests() : base(typeof(MinecraftTests))
        {
            // EnableSave = true;
        }

        [TestMethod()]
        public async Task GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), await minecraft.GetStatus());
        }

        [TestMethod()]
        public async Task GetStatusPre17Test()
        {
            SaveResult(nameof(GetStatusPre17Test), await minecraft.GetStatusPre17());
        }
    }
}