using System.Collections.Generic;

namespace OpenGSQ.Responses.GameSpy2
{
    /// <summary>
    /// Represents the status of the server.
    /// </summary>
    public class Status
    {
        /// <summary>
        /// Gets or sets the server information.
        /// </summary>
        public Dictionary<string, string> Info { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the list of players.
        /// </summary>
        public List<Dictionary<string, string>> Players { get; set; } = new List<Dictionary<string, string>>();

        /// <summary>
        /// Gets or sets the list of teams.
        /// </summary>
        public List<Dictionary<string, string>> Teams { get; set; } = new List<Dictionary<string, string>>();
    }
}