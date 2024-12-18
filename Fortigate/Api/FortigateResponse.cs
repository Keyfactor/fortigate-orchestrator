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
    public class FortigateResponse<ResultType>
    {
        public string http_method { get; set; }
        public ResultType results { get; set; }
        public string vdom { get; set; }
        public string path { get; set; }
        public string name { get; set; }
        public string action { get; set; }
        public string status { get; set; }
        public string serial { get; set; }
        public string version { get; set; }
        public int build { get; set; }
    }
}