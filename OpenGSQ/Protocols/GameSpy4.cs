namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Gamespy Query Protocol version 4
    /// </summary>
    public class GameSpy4 : GameSpy3
    {
        /// <inheritdoc/>
        public override string FullName => "GameSpy Protocol version 4";

        /// <summary>
        /// Initializes a new instance of the GameSpy4 class.
        /// </summary>
        /// <param name="host">The IP address of the server.</param>
        /// <param name="port">The port number of the server.</param>
        /// <param name="timeout">The timeout for the connection in milliseconds.</param>
        public GameSpy4(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
            _Challenge = true;
        }
    }
}
