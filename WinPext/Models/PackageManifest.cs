
using System.Runtime.Serialization;

namespace frida_windows_package_manager.Models
{
    [DataContract]
    public class PackageManifest
    {
        [DataMember]
        public string Publisher { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string[] js_Files { get; set; }
        [DataMember]
        public string[] cs_Files { get; set; }
        [DataMember]
        public string csc_commandline { get; set; }
        [DataMember]
        public string Exe { get; set; }
        [DataMember]
        public string AppxPackageFamilyName { get; set; }
    }
}
