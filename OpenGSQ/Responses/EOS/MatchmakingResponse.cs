using System.Collections.Generic;

namespace OpenGSQ.Responses.EOS
{
    /// <summary>
    /// Represents the response from a matchmaking request.
    /// </summary>
    public class MatchmakingResponse
    {
        /// <summary>
        /// Gets or sets the list of sessions returned by the matchmaking request.
        /// Each session is represented as a dictionary of string keys and object values.
        /// </summary>
        public List<Dictionary<string, object>> Sessions { get; set; }

        /// <summary>
        /// Gets or sets the count of sessions returned by the matchmaking request.
        /// </summary>
        public int Count { get; set; }
    }
}