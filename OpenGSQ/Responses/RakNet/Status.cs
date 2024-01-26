namespace OpenGSQ.Responses.RakNet
{
    /// <summary>
    /// Represents the status response from a Minecraft server.
    /// </summary>
    public class Status
    {
        /// <summary>
        /// Gets or sets the edition of the server (MCPE or MCEE for Education Edition).
        /// </summary>
        public string Edition { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first line of the Message of the Day (MOTD).
        /// </summary>
        public string MotdLine1 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the protocol version of the server.
        /// </summary>
        public int ProtocolVersion { get; set; }

        /// <summary>
        /// Gets or sets the version name of the server.
        /// </summary>
        public string VersionName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of players currently on the server.
        /// </summary>
        public int NumPlayers { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of players that can join the server.
        /// </summary>
        public int MaxPlayers { get; set; }

        /// <summary>
        /// Gets or sets the unique ID of the server.
        /// </summary>
        public string ServerUniqueId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the second line of the Message of the Day (MOTD).
        /// </summary>
        public string MotdLine2 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the game mode of the server.
        /// </summary>
        public string GameMode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the numeric representation of the game mode.
        /// </summary>
        public int GameModeNumeric { get; set; }

        /// <summary>
        /// Gets or sets the IPv4 port of the server.
        /// </summary>
        public int PortIPv4 { get; set; }

        /// <summary>
        /// Gets or sets the IPv6 port of the server.
        /// </summary>
        public int PortIPv6 { get; set; }
    }
}