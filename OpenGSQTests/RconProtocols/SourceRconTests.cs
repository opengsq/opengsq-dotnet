using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQ.Exceptions;
using OpenGSQTests;
using System;
using System.Threading.Tasks;

namespace OpenGSQ.RconProtocols.Tests
{
    [TestClass()]
    public class SourceRconTests : TestBase
    {
        public SourceRconTests() : base(typeof(SourceRconTests))
        {
            // EnableSave = true;
        }

        [TestMethod()]
        public async Task AuthenticateTest()
        {
            using var sourceRcon = new SourceRcon("", 27010);

            try
            {
                await sourceRcon.Authenticate("");

                string result = await sourceRcon.SendCommand("cvarlist");

                SaveResult(nameof(AuthenticateTest), result, isJson: false);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"{e.Message}");
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine($"{e.Message}");
            }
        }
    }
}