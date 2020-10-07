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

namespace csmcrc
{
    /// <summary>
    /// PacketDisassembler class. Disassembles responses from the server. [VERSION 1.1]
    /// </summary>
    class PacketDisassembler
    {
        public static readonly byte PACKET_S_LENGTH = 0;
        public static readonly byte PACKET_S_REQUEST_ID = 1;
        public static readonly byte PACKET_S_TYPE = 2;
        public static readonly byte PACKET_S_PAYLOAD = 3;

        /// <summary>
        /// Disassembles a valid RCON response from the server
        /// </summary>
        /// <param name="packet">byte[], contains the raw response from the server</param>
        /// <param name="type">can be PACKET_S_LENGTH, PACKET_S_REQUEST_ID, PACKET_S_TYPE, PACKET_S_PAYLOAD depending on what you want to extract</param>
        /// <returns>The extracted data</returns>
        public static byte[] disassemblePacket(byte[] packet, byte type)
        {
            byte[] data = { };
            byte[] bRemainderLength = new byte[4];
            Array.Copy(packet, 0, bRemainderLength, 0, 4);
            int payloadLength = BitConverter.ToInt32(bRemainderLength, 0) - 8;
            try
            {
                switch (type)
                {
                    case 0: //PACKET_S_LENGTH
                        data = new byte[4];
                        Array.Copy(packet, 0, data, 0, 4);
                        break;
                    case 1: //PACKET_S_REQUEST_ID
                        data = new byte[4];
                        Array.Copy(packet, 4, data, 0, 4);
                        break;
                    case 2: //PACKET_S_TYPE
                        data = new byte[4];
                        Array.Copy(packet, 8, data, 0, 4);
                        break;
                    case 3: //PACKET_S_PAYLOAD
                        data = new byte[payloadLength];
                        Array.Copy(packet, 12, data, 0, payloadLength);
                        break;
                    default:
                        throw new InvalidPacketCsmcrcException();
                }
            }catch(Exception x)
            {
                throw new InvalidPacketCsmcrcException(x.Message);
            }
            return data;
        }
    }
}
