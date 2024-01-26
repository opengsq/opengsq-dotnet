namespace OpenGSQ.Responses.Minecraft
{
    /// <summary>
    /// Represents the status of a game for versions prior to 1.7.
    /// </summary>
    public class StatusPre17
    {
        /// <summary>
        /// Gets or sets the protocol of the game.
        /// </summary>
        public string Protocol { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the version of the game.
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the message of the day.
        /// </summary>
        public string Motd { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of players in the game.
        /// </summary>
        public int NumPlayers { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of players allowed in the game.
        /// </summary>
        public int MaxPlayers { get; set; }
    }
}