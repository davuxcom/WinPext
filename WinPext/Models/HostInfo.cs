
using System.Runtime.Serialization;

namespace frida_windows_package_manager.Models
{
    [DataContract]
    public class HostInfo
    {
        [DataMember]
        public string ProxyLibraryPath { get; set; }
        [DataMember]
        public string UserLibraryPath { get; set; }
        [DataMember]
        public string PackageRoot { get; set; }
        [DataMember]
        public string RuntimeRoot { get; set; }
    }
}
