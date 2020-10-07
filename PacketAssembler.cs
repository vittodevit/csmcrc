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
using System.Linq;
using System.Text;

namespace csmcrc
{
    /// <summary>
    /// PacketAssembler class. Assembles valid packets for RCON protocol [V1.0 C#]
    /// </summary>
    class PacketAssembler
    {
        //constants
        private static readonly byte[] padding = { 0x00, 0x00 };
        const int Ansi = 255;
        public static readonly int TYPE_LOGIN = 3;
        public static readonly int TYPE_COMMAND = 2;

        /// <summary>
        /// Assembles a packet valid for the RCON protocol
        /// </summary>
        /// <param name="requestId">Request identifier</param>
        /// <param name="requestType">Request type; TYPE_LOGIN for login and TYPE_COMMAND for command</param>
        /// <param name="payload">ASCII payload, content of the request, can be the password or the command</param>
        /// <returns>The assembled packet ready to be sent on tcp</returns>
        public static byte[] assemblePacket(int requestId, int requestType, String payload)
        {
            //checks if payload is pure ascii
            if (isNotAscii(payload))
            {
                throw new InvalidPayloadCsmcrcException(InvalidPayloadCsmcrcException.PAYLOAD_ISNOT_ASCII);
            }
            //calculates the total length of the packet, remainder and creates data array
            byte[] bPayload = Encoding.ASCII.GetBytes(payload);
            int totLength = 14 + bPayload.Length;
            if(totLength >= 1446)
            {
                throw new InvalidPayloadCsmcrcException(InvalidPayloadCsmcrcException.PAYLOAD_TOO_LONG);
            }
            int remainderLength = 10 + bPayload.Length;
            byte[] data = new byte[totLength];
            //converting parameters to byte[]
            byte[] bRequestId = BitConverter.GetBytes(requestId);
            byte[] bRequestType = BitConverter.GetBytes(requestType);
            byte[] bRemainderLength = BitConverter.GetBytes(remainderLength);
            //if the system is not little endian flip the byte array (RCON prot. is little endian)
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bRemainderLength);
                Array.Reverse(bRequestId);
                Array.Reverse(bRequestType);
            }
            //assembling packet
            Array.Copy(bRemainderLength, 0, data, 0, 4);
            Array.Copy(bRequestId, 0, data, 4, 4);
            Array.Copy(bRequestType, 0, data, 8, 4);
            Array.Copy(bPayload, 0, data, 12, bPayload.Length);
            Array.Copy(padding, 0, data, 12 + bPayload.Length, 2);
            return data;
        }

        /// <summary>
        /// Checks if the string is not ASCII
        /// </summary>
        /// <param name="s">String to check</param>
        /// <returns>If the string contains unicode characters returns true</returns>
        private static bool isNotAscii(string s)
        {
            return s.Any(c => c > Ansi);
        }
    }
}
