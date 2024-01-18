namespace OpenGSQ.Responses.Quake2
{
    /// <summary>
    /// Represents a player in the game.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Gets or sets the player's frags.
        /// </summary>
        public int Frags { get; set; }

        /// <summary>
        /// Gets or sets the player's ping.
        /// </summary>
        public int Ping { get; set; }

        /// <summary>
        /// Gets or sets the player's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the player's address.
        /// </summary>
        public string Address { get; set; }
    }
}