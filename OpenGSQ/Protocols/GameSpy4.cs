namespace OpenGSQ.Protocols
{
    public class GameSpy4 : GameSpy3
    {
        /// <summary>
        /// Gamespy Query Protocol version 4
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        public GameSpy4(string address, int port, int timeout = 5000) : base(address, port, timeout)
        {
            _Challenge = true;
        }
    }
}
