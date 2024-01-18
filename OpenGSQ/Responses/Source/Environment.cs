namespace OpenGSQ.Responses.Source
{
    /// <summary>
    /// Indicates the operating system of the server
    /// </summary>
    public enum Environment : byte
    {
        /// <summary>
        /// Linux
        /// </summary>
        Linux = 0x6C,

        /// <summary>
        /// Windows
        /// </summary>
        Windows = 0x77,

        /// <summary>
        /// Mac
        /// </summary>
        Mac = 0x6D,
    }
}