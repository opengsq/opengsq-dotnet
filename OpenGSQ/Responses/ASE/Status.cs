using System.Collections.Generic;

namespace OpenGSQ.Responses.ASE
{
    /// <summary>
    /// Represents the status of a game.
    /// </summary>
    public class Status
    {
        /// <summary>
        /// Gets or sets the name of the game.
        /// </summary>
        public string GameName { get; set; }

        /// <summary>
        /// Gets or sets the port of the game.
        /// </summary>
        public int GamePort { get; set; }

        /// <summary>
        /// Gets or sets the host name of the game.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets the type of the game.
        /// </summary>
        public string GameType { get; set; }

        /// <summary>
        /// Gets or sets the map of the game.
        /// </summary>
        public string Map { get; set; }

        /// <summary>
        /// Gets or sets the version of the game.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a password is required.
        /// </summary>
        public bool Password { get; set; }

        /// <summary>
        /// Gets or sets the number of players in the game.
        /// </summary>
        public int NumPlayers { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of players allowed in the game.
        /// </summary>
        public int MaxPlayers { get; set; }

        /// <summary>
        /// Gets or sets the rules of the game.
        /// </summary>
        public Dictionary<string, string> Rules { get; set; }

        /// <summary>
        /// Gets or sets the players in the game.
        /// </summary>
        public List<Player> Players { get; set; }
    }
}