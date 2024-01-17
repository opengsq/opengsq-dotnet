namespace OpenGSQ.Responses.Scum
{
    /// <summary>
    /// Represents the response status of a server.
    /// </summary>
    public class StatusResponse
    {
        /// <summary>
        /// Gets or sets the IP address of the server.
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Gets or sets the port number of the server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of players currently connected to the server.
        /// </summary>
        public int NumPlayers { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of players that can connect to the server.
        /// </summary>
        public int MaxPlayers { get; set; }

        /// <summary>
        /// Gets or sets the server time.
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a password is required to connect to the server.
        /// </summary>
        public bool Password { get; set; }

        /// <summary>
        /// Gets or sets the version of the server.
        /// </summary>
        public string Version { get; set; }
    }
}