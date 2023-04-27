// Copyright 2023 Keyfactor
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Keyfactor.Logging;

using Keyfactor.Extensions.Orchestrator.Fortigate.Api;

using System.Net.Http;
//using System.Net.Http.Json;

using System.Linq;
using System.Web;
using System.Text;
using System.Net.Http.Headers;
using Keyfactor.Orchestrators.Extensions;
using Microsoft.Extensions.Logging;
using Keyfactor.Orchestrators.Common.Enums;
using System.Reflection.Metadata;

namespace Keyfactor.Extensions.Orchestrator.Fortigate
{
    public class FortigateStore
    {
        private ILogger logger { get; set; }
        private string FortigateHost { get; set; }
        private string AccessToken { get; set; }


        private static readonly string available_certificates = "/api/v2/monitor/system/available-certificates";

        private static readonly string download_certificate = "/api/v2/monitor/system/certificate/download";

        //private static readonly string certificate_api = "/api/v2/cmdb/certificate/local";
        private static readonly string import_certificate_api = "/api/v2/monitor/vpn-certificate/local/import";

        private static readonly string get_certificate_api = "/api/v2/cmdb/certificate/local/";

        private static readonly string update_certificate_api = "/api/v2/cmdb/certificate/local/";

        //api/v2/cmdb/vpn.certificate/local/test?vdom=root
        private static readonly string delete_certificate_api = "/api/v2/cmdb/vpn.certificate/local/";

        private static readonly string cert_usage_api = "/api/v2/monitor/system/object/usage";

        static readonly HttpClientHandler handler = new HttpClientHandler()
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, policyErrors) => { return true; }
        };
        static readonly HttpClient client = new HttpClient(handler);

        public FortigateStore(string fortigateHost, string accessToken)
        {
            logger = LogHandler.GetClassLogger(this.GetType());

            logger.MethodEntry(LogLevel.Debug);

            FortigateHost = fortigateHost;
            AccessToken = accessToken;

            logger.MethodExit(LogLevel.Debug);
        }

        public void Delete(string alias)
        {
            logger = LogHandler.GetClassLogger(this.GetType());

            var result = DeleteResource(delete_certificate_api + alias);

            logger.MethodExit(LogLevel.Debug);
        }

        public bool Exists(string alias)
        {
            logger = LogHandler.GetClassLogger(this.GetType());

            try
            {
                var result = GetResource(get_certificate_api + alias);
                var response = JsonConvert.DeserializeObject<FortigateResponse<Certificate[]>>(result);

                return (response.results != null && response.results.Length > 0);
            } 
            catch(Exception)
            {
                return false;
            }
            finally
            {
                logger.MethodExit(LogLevel.Debug);
            }

        }

        public void UpdateUsage(string alias, string path, string name, string attribute)
        {
            logger = LogHandler.GetClassLogger(this.GetType());

            var attributeValue = new Dictionary<String, String>();
            attributeValue.Add("q_origin_key", alias);
            var main = new Dictionary<String, Object>();
            main.Add(attribute, attributeValue);

            var endpoint = "/api/v2/cmdb/" + path + "/" + name;

            var parameters = new Dictionary<String, String>();
            parameters.Add("vdom", "root");
            var result = PutAsJson(endpoint, main, parameters);
            logger.LogDebug(result);

            logger.MethodExit(LogLevel.Debug);
        }

        public Usage Usage(string alias)
        {
            logger = LogHandler.GetClassLogger(this.GetType());

            var parameters = new Dictionary<String, String>();
            parameters.Add("vdom", "root");
            parameters.Add("scope", "global");
            parameters.Add("mkey", alias);
            parameters.Add("qtypes", "[160]");
            var result = GetResource(cert_usage_api, parameters);
            var response = JsonConvert.DeserializeObject<FortigateResponse<Usage>>(result);

            logger.MethodExit(LogLevel.Debug);
            return response.results;
        }

        public void Insert(string alias, string cert, string privateKey, bool overwrite, string password = null)
        {
            logger = LogHandler.GetClassLogger(this.GetType());

            if (overwrite)
            {
                var tmpAlias = alias + "_kftmp";
                var existing = Exists(alias);
                var tmpExisting = Exists(tmpAlias);

                //if there is an existing record
                if (existing)
                {
                    //check to see if it's in use
                    var existingUsage = Usage(alias);

                    //if it's currently in use
                    if (existingUsage.currently_using.Length > 0)
                    {
                        //if we don't have a tmp create a temp
                        if (!tmpExisting)
                        {
                            //create tmp
                            Insert(tmpAlias, cert, privateKey);

                            tmpExisting = true;
                        }

                        foreach (var existingUsing in existingUsage.currently_using)
                        {
                            UpdateUsage(tmpAlias, existingUsing.path, existingUsing.name, existingUsing.attribute);
                        }
                    }

                    logger.LogDebug("Deleting alias:" + alias);
                    Delete(alias);
                }

                logger.LogDebug("Inserting alias:" + alias);
                Insert(alias, cert, privateKey, password);

                //if we have an existing temp record
                if (tmpExisting)
                {
                    //check to see if it has any binds
                    var tmpUsage = Usage(tmpAlias);
                    if (tmpUsage.currently_using.Length > 0)
                    {
                        //transfer binds back to original
                        foreach (var tmpUsing in tmpUsage.currently_using)
                        {
                            UpdateUsage(alias, tmpUsing.path, tmpUsing.name, tmpUsing.attribute);
                        }
                    }
                    logger.LogDebug("Deleting alias:" + tmpExisting);
                    Delete(tmpAlias);
                }
            }
            else
            {
                //no overwrite so we just try to insert
                logger.LogDebug("Inserting certificate with alias: " + alias);
                Insert(alias, cert, privateKey, password);
            }

            logger.MethodExit(LogLevel.Debug);
        }

        private void Insert(string alias, string cert, string privateKey, string password = null)
        {
            logger = LogHandler.GetClassLogger(this.GetType());

            var cert_resource = new cmdb_certificate_resource()
            {
                certname = alias,
                key_file_content = privateKey,
                file_content = cert,
                scope = "global",
                //password = password,
                type = "regular"
            };

            logger.LogDebug(alias);
            logger.LogDebug("key_file_content:" + privateKey);
            logger.LogDebug("file_content:" + cert);
            
            Insert(cert_resource);

            logger.MethodExit(LogLevel.Debug);
        }

        private void Insert(cmdb_certificate_resource cert)
        {
            logger = LogHandler.GetClassLogger(this.GetType());

            var parameters = new Dictionary<String, String>();
            parameters.Add("vdom", "root");
            var result = PostAsJson(import_certificate_api, cert, parameters);
            logger.LogDebug(result);

            logger.MethodExit(LogLevel.Debug);
        }

        public List<CurrentInventoryItem> List()
        {
            logger = LogHandler.GetClassLogger(this.GetType());

            List<CurrentInventoryItem> items = new List<CurrentInventoryItem>();

            var result = GetResource(available_certificates);
            var response = JsonConvert.DeserializeObject<FortigateResponse<Certificate[]>>(result);

            foreach( var cert in response.results)
            {
                if (cert.type == "local-cer")
                {
                    var certFile = DownloadFileAsString(cert.name, cert.type);

                    var item = new CurrentInventoryItem()
                    {
                        Alias = cert.name,
                        Certificates = new string[] { certFile },
                        ItemStatus = OrchestratorInventoryItemStatus.Unknown,
                        PrivateKeyEntry = true,
                        UseChainLevel = false
                    };

                    items.Add(item);
                }
            }

            logger.MethodExit(LogLevel.Debug);

            return items;
        }

        private string DownloadFileAsString(string mkey, string type)
        {
            logger = LogHandler.GetClassLogger(this.GetType());

            var parameters = new Dictionary<String, String>();
            parameters.Add("mkey", mkey);
            parameters.Add("type", type);

            var response = client.GetAsync(GetUrl(download_certificate, parameters)).GetAwaiter().GetResult();
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error retrieving certificate {mkey}: {content}");

            logger.MethodExit(LogLevel.Debug);
            return content;
        }

        private String PostAsJson(string endpoint, cmdb_certificate_resource obj, Dictionary<String, String> additionalParams = null)
        {
            logger = LogHandler.GetClassLogger(this.GetType());

            string content = "";
            try
            {
                var url = GetUrl(endpoint, additionalParams);
                logger.LogDebug("postAsJson to url:" + url);
                var stringContent = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage responseMessage = client.PostAsync(url, stringContent).GetAwaiter().GetResult();
                logger.LogDebug("response message received");
                content = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                logger.LogDebug("Ensuring status code..");
                if (!responseMessage.IsSuccessStatusCode)
                    throw new Exception($"Error adding certificate {obj.certname}: {content}");

                logger.MethodExit(LogLevel.Debug);
                return responseMessage.StatusCode.ToString();
            }
            catch (HttpRequestException e)
            {
                logger.LogError("Error performing post resource: " + e.Message);
                throw e;
            }
        }

        private String DeleteResource(string endpoint, Dictionary<String, String> additionalParams = null)
        {
            logger = LogHandler.GetClassLogger(this.GetType());

            try
            {
                HttpResponseMessage responseMessage = client.DeleteAsync(GetUrl(endpoint, additionalParams)).GetAwaiter().GetResult();
                string content = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (!responseMessage.IsSuccessStatusCode)
                    throw new Exception($"Error removing certificate: {content}");

                logger.MethodExit(LogLevel.Debug);
                return responseMessage.StatusCode.ToString();
            }
            catch (HttpRequestException e)
            {
                logger.LogError("Error performing deleting resource: " + e.Message);
                throw e;
            }
        }

        private String PutAsJson(string endpoint, Object obj, Dictionary<String, String> additionalParams = null)
        {
            logger = LogHandler.GetClassLogger(this.GetType());

            try
            {
                var stringContent = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage responseMessage = client.PutAsync(GetUrl(endpoint, additionalParams), stringContent).GetAwaiter().GetResult();
                string content = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (!responseMessage.IsSuccessStatusCode)
                    throw new Exception(content);

                logger.MethodExit(LogLevel.Debug);
                return responseMessage.StatusCode.ToString();
            }
            catch (HttpRequestException e)
            {
                logger.LogError("Error performing put resource: " + e.Message);
                throw e;
            }
        }

        private String GetUrl(string endpoint, Dictionary<String, String> additionalParams = null)
        {
            logger = LogHandler.GetClassLogger(this.GetType());

            return AddQueryParams("https://" + FortigateHost + endpoint, additionalParams);

            logger.MethodExit(LogLevel.Debug);
        }

        private String AddQueryParams(string endpoint, Dictionary<String, String> additionalParams = null)
        {
            logger = LogHandler.GetClassLogger(this.GetType());

            var parameters = new Dictionary<String, String>();
            parameters.Add("access_token", AccessToken);
            if (additionalParams != null)
            {
                foreach (var additionalParam in additionalParams)
                {
                    parameters.Add(additionalParam.Key, additionalParam.Value);
                }
            }

            try
            {
                var queryString = endpoint + "?" + string.Join("&", parameters.Select(kvp => $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));

                logger.LogDebug("QueryString:" + queryString);

                logger.MethodExit(LogLevel.Debug);
                return queryString;
            } 
            catch (Exception e)
            {
                logger.LogDebug("Exception occured while creating query string", e);
                throw e;
            }
        }

        private string GetResource(string endpoint, Dictionary<String, String> additionalParams = null)
        {
            logger = LogHandler.GetClassLogger(this.GetType());

            try
            {
                logger.MethodExit(LogLevel.Debug);

                return client.GetStringAsync(GetUrl(endpoint, additionalParams)).GetAwaiter().GetResult();
            }
            catch(HttpRequestException e)
            {
                logger.LogError("Error performing get resource: " + e.Message);
                throw e;
            }
        }
    }
}
