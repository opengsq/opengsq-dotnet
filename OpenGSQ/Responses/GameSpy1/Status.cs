using System.Collections.Generic;

namespace OpenGSQ.Responses.GameSpy1
{
    /// <summary>
    /// Server Status
    /// </summary>
    public class Status
    {
        /// <summary>
        /// Indicates whether the response is XServerQuery or old response.
        /// </summary>
        public bool IsXServerQuery { get => Info?.ContainsKey("XServerQuery") ?? false; }

        /// <summary>
        /// Server's Info
        /// <para>If <see cref="IsXServerQuery"/> is <see langword="true"/>, then it includes \info\xserverquery\rules\xserverquery, else \basic\\info\\rules\\</para>
        /// </summary>
        public Dictionary<string, string> Info { get; set; }

        /// <summary>
        /// Server's Players
        /// </summary>
        public List<Dictionary<string, string>> Players { get; set; }

        /// <summary>
        /// Server's Teams (Only when <see cref="IsXServerQuery"/> is <see langword="true"/>)
        /// </summary>
        public List<Dictionary<string, string>> Teams { get; set; }
    }
}