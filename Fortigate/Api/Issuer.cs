//Copyright 2023 Keyfactor
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

namespace Keyfactor.Extensions.Orchestrator.Fortigate.Api
{
    public class Issuer
    {
        public string C { get; set; }
        public string ST { get; set; }
        public string L { get; set; }
        public string O { get; set; }
        public string OU { get; set; }
        public string CN { get; set; }
        public string emailAddress { get; set; }
    }
}
