using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Quake3 Query Protocol
    /// </summary>
    public class Quake3 : Quake2
    {
        /// <summary>
        /// Quake3 Query Protocol
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        public Quake3(string address, int port, int timeout = 5000) : base(address, port, timeout)
        {
            _RequestHeader = "getstatus";
            _ResponseHeader = "statusResponse\n";
        }

        /// <summary>
        /// This returns server information only. if <c>stripColor</c>
        /// <para>If you want to get players too. See: <seealso cref="GetStatus(bool)"/></para>
        /// </summary>
        /// <param name="stripColor"></param>
        /// <returns></returns>
        /// <exception cref="SocketException"></exception>
        public Dictionary<string, string> GetInfo(bool stripColor = true)
        {
            using (var udpClient = new UdpClient())
            {
                var responseData = ConnectAndSend(udpClient, "getinfo");

                using (var br = new BinaryReader(new MemoryStream(responseData), Encoding.UTF8))
                {
                    var header = br.ReadStringEx(_Delimiter1);
                    string infoResponseHeader = "infoResponse\n";

                    if (header != infoResponseHeader)
                    {
                        throw new Exception($"Packet header mismatch. Received: {header}. Expected: {infoResponseHeader}.");
                    }

                    var info = ParseInfo(br);

                    if (!stripColor)
                    {
                        return info;
                    }

                    if (info.ContainsKey("hostname"))
                    {
                        info["hostname"] = StripColors(info["hostname"]);
                    }

                    return info;
                }
            }
        }

        /// <summary>
        /// This returns server information and players.
        /// </summary>
        /// <param name="stripColor"></param>
        /// <returns></returns>
        /// <exception cref="SocketException"></exception>
        public Status GetStatus(bool stripColor = true)
        {
            using (var br = GetResponseBinaryReader())
            {
                var status = new Status
                {
                    Info = ParseInfo(br),
                    Players = ParsePlayers(br),
                };

                if (!stripColor)
                {
                    return status;
                }

                if (status.Info.ContainsKey("sv_hostname"))
                {
                    status.Info["sv_hostname"] = StripColors(status.Info["sv_hostname"]);
                }

                foreach (var player in status.Players)
                {
                    if (player.Name != null)
                    {
                        player.Name = StripColors(player.Name);
                    }
                }

                return status;
            }
        }

        /// <summary>
        /// Strip color codes
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string StripColors(string text)
        {
            return new Regex("\\^(X.{6}|.)").Replace(text, string.Empty);
        }
    }
}
