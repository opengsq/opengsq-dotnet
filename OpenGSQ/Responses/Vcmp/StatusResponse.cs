namespace OpenGSQ.Responses.Vcmp
{
    /// <summary>
    /// Represents the status response from a server.
    /// </summary>
    public class StatusResponse
    {
        /// <summary>
        /// Gets or sets the version of the server.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a password is required to connect to the server.
        /// </summary>
        public bool Password { get; set; }

        /// <summary>
        /// Gets or sets the number of players currently connected to the server.
        /// </summary>
        public short NumPlayers { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of players that can connect to the server.
        /// </summary>
        public short MaxPlayers { get; set; }

        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Gets or sets the type of game being played on the server.
        /// </summary>
        public string GameType { get; set; }

        /// <summary>
        /// Gets or sets the language of the server.
        /// </summary>
        public string Language { get; set; }
    }

    /// <summary>
    /// Represents a player in the game.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Gets or sets the player's name.
        /// </summary>
        public string Name { get; set; }
    }
}