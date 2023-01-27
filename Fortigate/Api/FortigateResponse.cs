

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