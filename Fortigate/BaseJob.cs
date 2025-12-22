using Microsoft.Extensions.Logging;
using Keyfactor.Logging;
using Newtonsoft.Json;

namespace Keyfactor.Extensions.Orchestrator.Fortigate
{
    public class BaseJob
    {
        internal string VDOM { get; set; }

        internal void SetProperties(string properties, ILogger logger)
        {
            {
                logger.MethodEntry(LogLevel.Debug);
                logger.LogDebug($"Job Properties: {properties}");

                dynamic propertiesJSON = JsonConvert.DeserializeObject(properties);

                VDOM = propertiesJSON.VDOM == null || string.IsNullOrEmpty(propertiesJSON.VDOM.Value) ? "root" : propertiesJSON.VDOM.Value;
            }
        }
    }
}
