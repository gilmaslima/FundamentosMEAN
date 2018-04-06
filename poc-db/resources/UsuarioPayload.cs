using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.ApiLogin.Sharepoint.ISAPI.Login.Models
{
    [DataContract(Name = "usuario")]
    public class UsuarioPayload
    {
        [DataMember(Name = "Email")]
        public String Email { get; set; }

        [DataMember(Name = "Id")]
        public String Id { get; set; }

        [DataMember(Name = "Nome")]
        public String Nome { get; set; }
    }
}
