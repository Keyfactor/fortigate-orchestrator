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

using System;
using System.Collections.Generic;

using Keyfactor.Orchestrators.Extensions;
using Keyfactor.Orchestrators.Common.Enums;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;
using Keyfactor.Orchestrators.Extensions.Interfaces;
using Org.BouncyCastle.Tls;

namespace Keyfactor.Extensions.Orchestrator.Fortigate
{
    public class Inventory : IInventoryJobExtension
    {
        public IPAMSecretResolver _resolver;
        public string ExtensionName => string.Empty;

        public Inventory(IPAMSecretResolver resolver)
        {
            _resolver = resolver;
        }

        public JobResult ProcessJob(InventoryJobConfiguration config, SubmitInventoryUpdate submitInventory)
        {
            ILogger logger = LogHandler.GetClassLogger(this.GetType());
            logger.LogDebug($"Begin {config.Capability} for job id {config.JobId}...");
            logger.LogDebug($"Client Machine: {config.CertificateStoreDetails.ClientMachine}");
            logger.LogDebug($"Store Path: {config.CertificateStoreDetails.StorePath}");

            List<CurrentInventoryItem> inventoryItems = new List<CurrentInventoryItem>();
            bool atLeastOneError = false;

            try
            {
                FortigateStore store = new FortigateStore(config.CertificateStoreDetails.ClientMachine, PAMUtilities.ResolvePAMField(_resolver, logger, "Fortigate Access Key", config.CertificateStoreDetails.StorePassword), config.CertificateStoreDetails.StorePath);

                Api.Certificate[] certificates = store.List(null);

                bool isError;

                foreach (var cert in certificates)
                {
                    if (cert.type == "local-cer")
                    {
                        var certFile = store.DownloadFileAsString(cert.name, cert.type, out isError);
                        if (isError) atLeastOneError = true;

                        var item = new CurrentInventoryItem()
                        {
                            Alias = cert.name,
                            Certificates = new string[] { certFile },
                            ItemStatus = OrchestratorInventoryItemStatus.Unknown,
                            PrivateKeyEntry = true,
                            UseChainLevel = false
                        };

                        inventoryItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception for {config.Capability}: {FortigateException.FlattenExceptionMessages(ex, string.Empty)} for job id {config.JobId} ");
                return new JobResult() { Result = OrchestratorJobStatusJobResult.Failure, JobHistoryId = config.JobHistoryId, FailureMessage = FortigateException.FlattenExceptionMessages(ex, $"Site {config.CertificateStoreDetails.ClientMachine} ") };
            }

            try
            {
                logger.LogDebug("Sending certificates back to Command:" + inventoryItems.Count);
                submitInventory.Invoke(inventoryItems);
                logger.LogDebug($"...End {config.Capability} job for job id {config.JobId}");

                if (atLeastOneError)
                    return new JobResult() { Result = OrchestratorJobStatusJobResult.Warning, JobHistoryId = config.JobHistoryId, FailureMessage = "At least one certificate was unable to be retrieved, Please check the log for more information." };
                else
                    return new JobResult() { Result = OrchestratorJobStatusJobResult.Success, JobHistoryId = config.JobHistoryId };
            }
            catch (Exception ex)
            {
                string errorMessage = FortigateException.FlattenExceptionMessages(ex, $"Site {config.CertificateStoreDetails.ClientMachine}: ");
                logger.LogError($"Exception returning certificates for {config.Capability}: {errorMessage} for job id {config.JobId}");
                return new JobResult() { Result = OrchestratorJobStatusJobResult.Failure, JobHistoryId = config.JobHistoryId, FailureMessage = errorMessage };
            }
        }
    }
}