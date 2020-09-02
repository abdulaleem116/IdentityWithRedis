using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityWithRedis.Models
{
    [Serializable]
    public class SampleObject
    {
        public string senderUName { get; set; }
        public string receiverUName { get; set; }
        public string  textMsg { get; set; }
    }
}