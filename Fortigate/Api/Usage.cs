using System.Text.Json.Serialization;

namespace Keyfactor.Extensions.Orchestrator.Fortigate.Api
{
    public class Usage
    {
        public CanUse[] can_use { get; set; }

        public CurrentlyUsing[] currently_using { get; set; }

        public int[] q_types { get; set; }

        public class CanUse
        {
            public string name { get; set; }
            public string path { get; set; }
            public string range { get; set; }
        }

        public class CurrentlyUsing
        {
            public string attribute { get; set; }
            public string name { get; set; }
            public string path { get; set; }
            public string range { get; set; }
            public int reference_count { get; set; }
            public bool static_ { get; set; }
            [JsonPropertyName("static")]
            public string table_type { get; set; }
            public string vdom { get; set; }
        }
        //[attribute, mkey, name, path, range, reference_count,static,table_type,vdom]
    }
}
