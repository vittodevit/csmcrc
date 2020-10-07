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
using System.Collections.Generic;
using System.Text;

namespace csmcrc
{
    [Serializable]
    class InvalidPayloadCsmcrcException : Exception
    {
        public static readonly string PAYLOAD_TOO_LONG = "is too long";
        public static readonly string PAYLOAD_ISNOT_ASCII = "contains non-ASCII characters";

        public InvalidPayloadCsmcrcException()
        {

        }

        public InvalidPayloadCsmcrcException(string m)
            : base (String.Format("The payload {0}", m))
        {

        }
    }
}
