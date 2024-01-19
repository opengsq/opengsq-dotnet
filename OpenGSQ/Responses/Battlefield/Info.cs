using System.Collections.Generic;

namespace OpenGSQ.Responses.Battlefield
{
    /// <summary>
    /// Represents the info of a game.
    /// </summary>
    public class Info
    {
        /// <summary>
        /// Gets or sets the hostname of the game server.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets the number of players in the game.
        /// </summary>
        public int NumPlayers { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of players allowed in the game.
        /// </summary>
        public int MaxPlayers { get; set; }

        /// <summary>
        /// Gets or sets the type of the game.
        /// </summary>
        public string GameType { get; set; }

        /// <summary>
        /// Gets or sets the current map of the game.
        /// </summary>
        public string Map { get; set; }

        /// <summary>
        /// Gets or sets the number of rounds played.
        /// </summary>
        public int RoundsPlayed { get; set; }

        /// <summary>
        /// Gets or sets the total number of rounds.
        /// </summary>
        public int RoundsTotal { get; set; }

        /// <summary>
        /// Gets or sets the list of teams.
        /// </summary>
        public List<float> Teams { get; set; }

        /// <summary>
        /// Gets or sets the target score.
        /// </summary>
        public int TargetScore { get; set; }

        /// <summary>
        /// Gets or sets the status of the game.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the game is ranked.
        /// </summary>
        public bool Ranked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether PunkBuster is enabled.
        /// </summary>
        public bool PunkBuster { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a password is required.
        /// </summary>
        public bool Password { get; set; }

        /// <summary>
        /// Gets or sets the uptime of the game server.
        /// </summary>
        public int Uptime { get; set; }

        /// <summary>
        /// Gets or sets the round time.
        /// </summary>
        public int RoundTime { get; set; }

        // Additional properties based on your code
        public string Mod { get; set; }
        public string IpPort { get; set; }
        public string PunkBusterVersion { get; set; }
        public bool JoinQueue { get; set; }
        public string Region { get; set; }
        public string PingSite { get; set; }
        public string Country { get; set; }
        public int? BlazePlayerCount { get; set; }
        public string BlazeGameState { get; set; }
        public bool? QuickMatch { get; set; }
    }
}