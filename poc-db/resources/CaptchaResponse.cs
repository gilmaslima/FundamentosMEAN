using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.DadosCadastrais.Servicos.Modelos
{
    [DataContract]
    public class CaptchaResponse
    {

        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "challenge_ts")]
        public string ChallengeTs { get; set; }

        [DataMember(Name = "hostname")]
        public string HostName { get; set; }
    }
}