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
using System.IO;
using System.Linq;
using System.Text;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;

using Keyfactor.Orchestrators.Extensions;
using Keyfactor.Orchestrators.Common.Enums;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;
using Keyfactor.Orchestrators.Extensions.Interfaces;

namespace Keyfactor.Extensions.Orchestrator.Fortigate
{
    public class Management : IManagementJobExtension
    {
        public IPAMSecretResolver _resolver;
        public string ExtensionName => string.Empty;
        
        ILogger logger;

        public Management(IPAMSecretResolver resolver)
        {
            _resolver = resolver;
        }

        //Job Entry Point
        public JobResult ProcessJob(ManagementJobConfiguration config)
        {
            logger = LogHandler.GetClassLogger(this.GetType());

            logger.LogDebug($"Begin {config.Capability} for job id {config.JobId}...");
            logger.LogDebug($"Client Machine: {config.CertificateStoreDetails.ClientMachine}");
            logger.LogDebug($"Store Path: {config.CertificateStoreDetails.StorePath}");

            string vdom = string.IsNullOrEmpty(config.CertificateStoreDetails.StorePath) ? "root" : config.CertificateStoreDetails.StorePath;

            FortigateStore store = new FortigateStore(config.CertificateStoreDetails.ClientMachine, PAMUtilities.ResolvePAMField(_resolver, logger, "Fortigate Access Key", config.CertificateStoreDetails.StorePassword), vdom);

            try
            {
                //Management jobs, unlike Discovery, Inventory, and Reenrollment jobs can have 3 different purposes:
                switch (config.OperationType)
                {
                    case CertStoreOperationType.Add:
                        logger.LogDebug($"BEGIN add operation for {config.CertificateStoreDetails.ClientMachine}, alias {config.JobCertificate.Alias}.");
                        byte[] pfxBytes = Convert.FromBase64String(config.JobCertificate.Contents);
                        (byte[] certPem, byte[] privateKey) = GetPemFromPFX(pfxBytes, config.JobCertificate.PrivateKeyPassword.ToCharArray());

                        store.Insert(config.JobCertificate.Alias, Convert.ToBase64String(certPem), Convert.ToBase64String(privateKey), config.Overwrite);
                        break;
                    case CertStoreOperationType.Remove:
                        //delete will fail if certificate is in use
                        logger.LogDebug($"BEGIN remove operation for {config.CertificateStoreDetails.ClientMachine}, alias {config.JobCertificate.Alias}.");
                        store.Delete(config.JobCertificate.Alias);
                        break;
                    default:
                        //Invalid OperationType.  Return error.  Should never happen though
                        throw new FortigateException($"Unsupported operation: {config.OperationType.ToString()} ");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception for {config.Capability}: {FortigateException.FlattenExceptionMessages(ex, string.Empty)} for job id {config.JobId}");
                return new JobResult() { Result = OrchestratorJobStatusJobResult.Failure, JobHistoryId = config.JobHistoryId, FailureMessage = FortigateException.FlattenExceptionMessages(ex, $"Site {config.CertificateStoreDetails.ClientMachine}:") };
            }

            logger.LogDebug($"...End {config.Capability} job for job id {config.JobId}");
            return new JobResult() { Result = OrchestratorJobStatusJobResult.Success, JobHistoryId = config.JobHistoryId };
        }

        private (byte[], byte[]) GetPemFromPFX(byte[] pfxBytes, char[] pfxPassword)
        {
            logger.MethodEntry(LogLevel.Debug);

            Pkcs12StoreBuilder storeBuilder = new Pkcs12StoreBuilder();
            Pkcs12Store p = storeBuilder.Build();
            p.Load(new MemoryStream(pfxBytes), pfxPassword);

            // Extract private key
            MemoryStream memoryStream = new MemoryStream();
            TextWriter streamWriter = new StreamWriter(memoryStream);
            PemWriter pemWriter = new PemWriter(streamWriter);

            String alias = (p.Aliases.Cast<String>()).SingleOrDefault(a => p.IsKeyEntry(a));
            AsymmetricKeyParameter publicKey = p.GetCertificate(alias).Certificate.GetPublicKey();
            if (p.GetKey(alias) == null) { throw new Exception($"Unable to get the key for alias: {alias}"); }
            AsymmetricKeyParameter privateKey = p.GetKey(alias).Key;
            AsymmetricCipherKeyPair keyPair = new AsymmetricCipherKeyPair(publicKey, privateKey);

            pemWriter.WriteObject(keyPair.Private);
            streamWriter.Flush();
            String privateKeyString = Encoding.ASCII.GetString(memoryStream.GetBuffer()).Trim().Replace("\r", "").Replace("\0", "");
            memoryStream.Close();
            streamWriter.Close();

            // Extract server certificate
            String certStart = "-----BEGIN CERTIFICATE-----\n";
            String certEnd = "\n-----END CERTIFICATE-----";
            Func<String, String> pemify = null;
            pemify = (ss => ss.Length <= 64 ? ss : ss.Substring(0, 64) + "\n" + pemify(ss.Substring(64)));
            String certPem = certStart + pemify(Convert.ToBase64String(p.GetCertificate(alias).Certificate.GetEncoded())) + certEnd;

            logger.MethodExit(LogLevel.Debug);
            return (Encoding.ASCII.GetBytes(certPem), Encoding.ASCII.GetBytes(privateKeyString));
        }
    }
}