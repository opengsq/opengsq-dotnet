using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenGSQ.Responses.Unreal2
{
    /// <summary>
    /// Represents the rules of the Unreal2 game.
    /// </summary>
    public class Rules
    {
        /// <summary>
        /// Gets or sets the list of mutators.
        /// </summary>
        [JsonPropertyName("Mutators")]
        public List<string> Mutators { get; set; } = new List<string>();

        /// <summary>
        /// A private dictionary to hold the data.
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value associated with the specified key.</returns>
        public string this[string key]
        {
            get { return Data[key].ToString(); }
            set { Data[key] = value.ToString(); }
        }
    }
}
