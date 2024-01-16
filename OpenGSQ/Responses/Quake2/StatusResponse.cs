using System.Collections.Generic;

namespace OpenGSQ.Responses.Quake2
{
    /// <summary>
    /// Represents the status of the server.
    /// </summary>
    public class StatusResponse
    {
        /// <summary>
        /// Gets or sets the server information.
        /// </summary>
        public Dictionary<string, string> Info { get; set; }

        /// <summary>
        /// Gets or sets the list of players.
        /// </summary>
        public List<Player> Players { get; set; }
    }

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