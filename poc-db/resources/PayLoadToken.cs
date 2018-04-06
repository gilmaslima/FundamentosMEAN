using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.ApiLogin.Sharepoint.ISAPI.Login.Models
{
    [DataContract]
    public class PayLoadToken
    {
        /// <summary>
        /// ISS do Token
        /// </summary>
        [DataMember(Name = "iss")]
        public String Iss { get; set; }

        /// <summary>
        /// usuário
        /// </summary>
        [DataMember(Name = "usuario")]
        public UsuarioPayload Usuario { get; set; }
    }
}
