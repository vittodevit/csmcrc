/*
This software is distributed under the Apache License 2.0
Copyright 2020 Vittorio Lo Mele

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Net.Sockets;
using System.Text;

namespace csmcrc
{
    public class Csmcrc
    {
        private string chost;
        private int cport;
        private bool loggedIn;
        private int requestId;
        private TcpClient client;
        private NetworkStream stream;
        private byte[] dataBuffer;

        /// <summary>
        /// Creates a connection to the server and sends a login packet.
        /// </summary>
        /// <param name="host">Hostname or IP address of the Minecraft server</param>
        /// <param name="port">Tcp port where RCON is listening, usually 25575</param>
        /// <param name="password">RCON Password</param>
        /// <returns>boolean (true if login is successful)</returns>
        public bool init(string host, int port, string password)
        {
            if (isConnected())
            {
                throw new UnableToConnectCsmcrcException("You are already connected to a server!");
            }
            try
            {
                client = new TcpClient(host, port);
                stream = client.GetStream();
            }catch(Exception x)
            {
                throw new UnableToConnectCsmcrcException(x.Message);
            }
            //generate request id
            Random rand = new Random();
            requestId = rand.Next(2147483647);
            //assemble packet
            byte[] loginPacket = PacketAssembler.assemblePacket(requestId, PacketAssembler.TYPE_LOGIN, password);
            try
            {
                stream.Write(loginPacket, 0, loginPacket.Length);
            }catch(Exception x)
            {
                throw new UnableToConnectCsmcrcException("Socket error = " + x.Message);
            }
            //creating data buffer and reading
            dataBuffer = new byte[8192];
            try
            {
                stream.Read(dataBuffer, 0, dataBuffer.Length); //check this, possible exception
            }
            catch (Exception x)
            {
                throw new UnableToConnectCsmcrcException("Socket error = " + x.Message);
            }
            //disassemble resposse
            byte[] responseId = PacketDisassembler.disassemblePacket(dataBuffer, PacketDisassembler.PACKET_S_REQUEST_ID);
            byte[] bRequestId = BitConverter.GetBytes(requestId);
            //check login
            if(responseId[0] == bRequestId[0] && responseId[1] == bRequestId[1] && responseId[2] == bRequestId[2] && responseId[3] == bRequestId[3])
            {
                //login ok, set variables
                chost = host;
                cport = port;
                loggedIn = true;
                //clean buffer and return
                dataBuffer = new byte[8192];
                return true;
            }
            else
            {
                //login failed, clean buffer and return
                dataBuffer = new byte[8192];
                return false;
            }
            
        }

        /// <summary>
        /// Sends a command to the server after the connection has been initalized
        /// </summary>
        /// <param name="payload">Command to send (only ASCII allowed!)</param>
        /// <returns>string (response from the server)</returns>
        public string send(string payload)
        {
            if (!isConnected())
            {
                throw new NotConnectedCsmcrcException("You are not connected to any server");
            }
            //assemble packet
            byte[] requestPacket = PacketAssembler.assemblePacket(requestId, PacketAssembler.TYPE_COMMAND, payload);
            //write stream
            try
            {
                stream.Write(requestPacket, 0, requestPacket.Length);
            }
            catch (Exception x)
            {
                throw new UnableToConnectCsmcrcException("Socket error = " + x.Message);
            }
            //clean buffer and receive data
            dataBuffer = new byte[8192];
            try
            {
                stream.Read(dataBuffer, 0, dataBuffer.Length);
            }
            catch (Exception x)
            {
                throw new UnableToConnectCsmcrcException("Socket error = " + x.Message);
            }
            //clean buffer, disassemble packet and return response
            byte[] bResponse = PacketDisassembler.disassemblePacket(dataBuffer, PacketDisassembler.PACKET_S_PAYLOAD);
            dataBuffer = new byte[8192];
            try
            {
                return Encoding.ASCII.GetString(bResponse);
            }catch(Exception x)
            {
                throw new InvalidPacketCsmcrcException("The response payload contains invalid ASCII characters.");
            }
        }

        public void disconnect()
        {
            if (!isConnected())
            {
                throw new NotConnectedCsmcrcException();
            }
            //close everything and reset class vars
            stream.Close();
            client.Close();
            chost = "";
            cport = 0;
            loggedIn = false;
            dataBuffer = new byte[8192];
        }

        /// <summary>
        /// Gets the state of connection
        /// </summary>
        /// <returns>bool</returns>
        public bool isConnected()
        {
            return loggedIn;
        }
        
        /// <summary>
        /// Gets the current server address
        /// </summary>
        /// <returns>string</returns>
        public string getAddress()
        {
            return chost;
        }

        /// <summary>
        /// Gets the current server port
        /// </summary>
        /// <returns>int</returns>
        public int getPort()
        {
            return cport;
        }
    }
}
