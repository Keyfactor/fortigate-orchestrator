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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Keyfactor.Extensions.Orchestrator.Fortigate.Api
{
    public class Certificate
    {
        public string name { get; set; }
        public string source { get; set; }
        public string comments { get; set; }
        public bool exists { get; set; }
        public string range { get; set; }
        public bool is_ssl_server_cert { get; set; }
        public bool is_ssl_client_cert { get; set; }
        public bool is_proxy_ssl_cert { get; set; }
        public bool is_general_allowable_cert { get; set; }
        public bool is_default_local { get; set; }
        public bool is_built_in { get; set; }
        public bool is_wifi_cert { get; set; }
        public bool is_deep_inspection_cert { get; set; }
        public bool has_valid_cert_key { get; set; }
        public string key_type { get; set; }
        public int key_size { get; set; }
        public bool is_local_ca_cert { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public long valid_from { get; set; }
        public string valid_from_raw { get; set; }
        public long valid_to { get; set; }
        public string valid_to_raw { get; set; }
        public string signature_algorithm { get; set; }
        public Subject subject { get; set; }
        public string subject_raw { get; set; }
        public Issuer issuer { get; set; }
        public string issuer_raw { get; set; }
        public string fingerprint { get; set; }
        public int version { get; set; }
        public bool is_ca { get; set; }
        public string serial_number { get; set; }
        public Ext[] ext { get; set; }
        public string q_path { get; set; }
        public string q_name { get; set; }
        public int q_ref { get; set; }
        public bool q_static { get; set; }
        public int q_type { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum KeyType
    {
        RSA, DSA, ECDSA
    }
}

