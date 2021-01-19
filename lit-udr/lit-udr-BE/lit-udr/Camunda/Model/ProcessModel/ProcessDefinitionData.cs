using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace lit_udr.Camunda.Model.ProcessModel
{
    public class ProcessDefinitionData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("description")]
        public object Description { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("version")]
        public int Version { get; set; }

        [JsonPropertyName("resource")]
        public string Resource { get; set; }

        [JsonPropertyName("deploymentId")]
        public string DeploymentId { get; set; }

        [JsonPropertyName("diagram")]
        public object Diagram { get; set; }

        [JsonPropertyName("suspended")]
        public bool Suspended { get; set; }

        [JsonPropertyName("tenantId")]
        public object TenantId { get; set; }

        [JsonPropertyName("versionTag")]
        public object VersionTag { get; set; }

        [JsonPropertyName("historyTimeToLive")]
        public object HistoryTimeToLive { get; set; }

        [JsonPropertyName("startableInTasklist")]
        public bool StartableInTasklist { get; set; }
    }
}
