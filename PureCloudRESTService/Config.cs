using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PureCloudRESTService
{
    //JSON Formatted configuration file
    [DataContract]
    internal class Config
    {
        [DataMember(EmitDefaultValue = false)]
        internal string URL { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal string connectionString { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal string getContactByPhoneNumber { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal string getAccountByPhoneNumber { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal string getAccountByAccountNumber { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal string getAccountByContactId { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal string getMostRecentOpenCaseByContactId { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal ContactConfig contactConfig {get; set;}
        [DataMember(EmitDefaultValue = false)]
        internal AccountConfig accountConfig { get; set; }
    }

    [DataContract]
    internal class ContactConfig
    {
        [DataMember(EmitDefaultValue = false)]
        internal List<string> fields { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal List<string> addressFields { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal int phoneCount { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal List<string> phoneMappings { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal int emailCount { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal List<string> emailMappings { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal bool getAddress { get; set; }
    }

    [DataContract]
    internal class AccountConfig
    {
        [DataMember(EmitDefaultValue = false)]
        internal List<string> fields { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal List<string> addressFields { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal int phoneCount { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal List<string> phoneMappings { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal int emailCount { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal List<string> emailMappings { get; set; }
        [DataMember(EmitDefaultValue = false)]
        internal bool getAddress { get; set; }
    }
}
