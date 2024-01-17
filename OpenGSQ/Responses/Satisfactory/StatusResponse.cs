namespace OpenGSQ.Responses.Satisfactory
{
    /// <summary>
    /// Represents the status response.
    /// </summary>
    public class StatusResponse
    {
        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the beacon port.
        /// </summary>
        public short BeaconPort { get; set; }
    }

}