namespace OpenGSQ.Responses.Source
{
    /// <summary>
    /// Indicates the type of server
    /// </summary>
    public enum ServerType : byte
    {
        /// <summary>
        /// Dedicated server
        /// </summary>
        Dedicated = 0x64,

        /// <summary>
        /// Listen server
        /// </summary>
        Listen = 0x6C,

        /// <summary>
        /// SourceTV relay (proxy)
        /// </summary>
        Proxy = 0x70,
    }
}