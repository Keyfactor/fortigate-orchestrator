namespace Keyfactor.Extensions.Orchestrator.Fortigate.Api
{
    public class cmdb_certificate_resource
    {
        public string type { get; set; }

        public string certname { get; set; }

        //< pass phrase used to encrypt key>
        //public string password { get; set; }

        //<base64-encoded certificate, without line breaks>
        public string key_file_content { get; set; }

        //<base64-encoded certificate, without line breaks>
        public string file_content { get; set; }

        public string scope { get; set; }
    }
}
