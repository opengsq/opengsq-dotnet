using System.Text;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// World Opponent Network (WON) Query Protocol
    /// </summary>
    public class WON : Source
    {
        /// <inheritdoc/>
        public override string FullName => "World Opponent Network (WON) Query Protocol";

        /// <summary>
        /// Initializes a new instance of the WON class.
        /// </summary>
        /// <param name="host">The host address of the server.</param>
        /// <param name="port">The port to connect to. Default is 27015.</param>
        /// <param name="timeout">The connection timeout in milliseconds. Default is 5 seconds.</param>
        public WON(string host, int port = 27015, int timeout = 5000) : base(host, port, timeout)
        {
            A2S_INFO = Encoding.ASCII.GetBytes("details\0");
            A2S_PLAYER = Encoding.ASCII.GetBytes("players");
            A2S_RULES = Encoding.ASCII.GetBytes("rules");
        }
    }
}
