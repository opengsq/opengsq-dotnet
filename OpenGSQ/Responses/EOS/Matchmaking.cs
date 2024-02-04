using System.Collections.Generic;
using System.Text.Json;

namespace OpenGSQ.Responses.EOS
{
    /// <summary>
    /// Represents the response from a matchmaking request.
    /// </summary>
    public class Matchmaking
    {
        /// <summary>
        /// Gets or sets the list of sessions returned by the matchmaking request.
        /// Each session is represented as a dictionary where the keys are strings and the values are JsonElements.
        /// </summary>
        public List<Dictionary<string, JsonElement>> Sessions { get; set; } = new List<Dictionary<string, JsonElement>>();

        /// <summary>
        /// Gets or sets the count of sessions returned by the matchmaking request.
        /// </summary>
        public int Count { get; set; }
    }
}