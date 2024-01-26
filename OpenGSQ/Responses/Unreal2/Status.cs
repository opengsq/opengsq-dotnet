namespace OpenGSQ.Responses.Unreal2
{
    /// <summary>
    /// Represents the status of a server.
    /// </summary>
    public class Status
    {
        /// <summary>
        /// Gets or sets the server ID.
        /// </summary>
        public int ServerId { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the server.
        /// </summary>
        public string ServerIP { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the game port of the server.
        /// </summary>
        public int GamePort { get; set; }

        /// <summary>
        /// Gets or sets the query port of the server.
        /// </summary>
        public int QueryPort { get; set; }

        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        public string ServerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the map.
        /// </summary>
        public string MapName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of the game.
        /// </summary>
        public string GameType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of players.
        /// </summary>
        public int NumPlayers { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of players.
        /// </summary>
        public int MaxPlayers { get; set; }

        /// <summary>
        /// Gets or sets the ping.
        /// </summary>
        public int Ping { get; set; }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// Gets or sets the skill level.
        /// </summary>
        public string Skill { get; set; } = string.Empty;
    }
}