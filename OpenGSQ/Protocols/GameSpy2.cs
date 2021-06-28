﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace OpenGSQ.Protocols
{
    public class GameSpy2 : ProtocolBase
    {
        /// <summary>
        /// Gamespy Query Protocol version 2
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        public GameSpy2(string address, int port, int timeout = 5000) : base(address, port, timeout)
        {

        }

        /// <summary>
        /// Retrieves information about the server including, Info, Players, and Teams.
        /// </summary>
        /// <exception cref="SocketException"></exception>
        /// <param name="request"></param>
        /// <returns></returns>
        public Response GetResponse(Request request = Request.Info | Request.Players | Request.Teams)
        {
            using (var udpClient = new UdpClient())
            {
                var responseData = ConnectAndSendPackets(udpClient, request);

                using (var br = new BinaryReader(new MemoryStream(responseData), Encoding.UTF8))
                {
                    var response = new Response();

                    // Save Response Info
                    if (request.HasFlag(Request.Info))
                    {
                        response.Info = GetInfo(br);
                    }

                    // Save Response Players
                    if (request.HasFlag(Request.Players))
                    {
                        response.Players = GetPlayers(br);
                    }

                    // Save Response Teams
                    if (request.HasFlag(Request.Teams))
                    {
                        response.Teams = GetTeams(br);
                    }

                    return response;
                }
            }
        }

        private byte[] ConnectAndSendPackets(UdpClient udpClient, Request request)
        {
            // Connect to remote host
            udpClient.Connect(EndPoint);
            udpClient.Client.SendTimeout = Timeout;
            udpClient.Client.ReceiveTimeout = Timeout;

            // Send Request
            var requestData = new byte[] { 0xFE, 0xFD, 0x00, 0x04, 0x05, 0x06, 0x07 }.Concat(GetRequestBytes(request)).ToArray();
            udpClient.Send(requestData, requestData.Length);

            // Server response
            var responseData = udpClient.Receive(ref EndPoint);

            // Remove the first 5 bytes { 0x00, 0x04, 0x05, 0x06, 0x07 }
            return responseData.Skip(5).ToArray();
        }

        private byte[] GetRequestBytes(Request request)
        {
            return new byte[] {
                (byte)(request.HasFlag(Request.Info) ? 0xFF : 0x00),
                (byte)(request.HasFlag(Request.Players) ? 0xFF : 0x00),
                (byte)(request.HasFlag(Request.Teams) ? 0xFF : 0x00),
            };
        }

        private Dictionary<string, string> GetInfo(BinaryReader br)
        {
            var info = new Dictionary<string, string>();

            // Read all key values
            while (br.TryReadStringEx(out var key))
            {
                info[key] = br.ReadStringEx().Trim();
            }

            return info;
        }

        private List<Dictionary<string, string>> GetPlayers(BinaryReader br)
        {
            var players = new List<Dictionary<string, string>>();

            // Skip a byte
            br.ReadByte();

            // Get player count
            var playerCount = br.ReadByte();

            // Get all keys
            var keys = new List<string>();

            while (br.TryReadStringEx(out var key))
            {
                keys.Add(key.TrimEnd('_'));
            }

            // Set all keys and values
            for (int i = 0; i < playerCount; i++)
            {
                players.Add(new Dictionary<string, string>());

                foreach (var key in keys)
                {
                    players[i][key] = br.ReadStringEx().Trim();
                }
            }

            return players;
        }

        private List<Dictionary<string, string>> GetTeams(BinaryReader br)
        {
            var teams = new List<Dictionary<string, string>>();

            // Skip a byte
            br.ReadByte();

            // Get team count
            var teamCount = br.ReadByte();

            // Get all keys
            var keys = new List<string>();

            while (br.TryReadStringEx(out var key))
            {
                keys.Add(key.TrimEnd('t').TrimEnd('_'));
            }

            // Set all keys and values
            for (int i = 0; i < teamCount; i++)
            {
                teams.Add(new Dictionary<string, string>());

                foreach (var key in keys)
                {
                    teams[i][key] = br.ReadStringEx().Trim();
                }
            }

            return teams;
        }

        /// <summary>
        /// Request Flag
        /// </summary>
        [Flags]
        public enum Request : short
        {
            Info = 1,
            Players = 2,
            Teams = 4,
        }

        /// <summary>
        /// Response object
        /// </summary>
        public class Response
        {
            /// <summary>
            /// Response Info
            /// </summary>
            public Dictionary<string, string> Info { get; set; }

            /// <summary>
            /// Response Players
            /// </summary>
            public List<Dictionary<string, string>> Players { get; set; }

            /// <summary>
            /// Response Teams
            /// </summary>
            public List<Dictionary<string, string>> Teams { get; set; }
        }
    }
}
