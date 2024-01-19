namespace OpenGSQ.Responses.ASE
{
    /// <summary>
    /// Represents a player in the game.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the team of the player.
        /// </summary>
        public string Team { get; set; }

        /// <summary>
        /// Gets or sets the skin of the player.
        /// </summary>
        public string Skin { get; set; }

        /// <summary>
        /// Gets or sets the score of the player.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the ping of the player.
        /// </summary>
        public int Ping { get; set; }

        /// <summary>
        /// Gets or sets the time of the player.
        /// </summary>
        public int Time { get; set; }
    }
}