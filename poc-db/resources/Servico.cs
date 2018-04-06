#region Histórico do Arquivo
/*
(c) Copyright [2016] Redecard S.A.
Autor       : [Raphael Ivo]
Empresa     : [Iteris]
Histórico   :
- [19/09/2016] – [Raphael Ivo] – [Criação]
*/
#endregion

using System.Runtime.Serialization;

namespace Redecard.PN.Sustentacao.SharePoint
{
    [DataContract]
    public class Servico
    {
        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public string Status { get; set; }
    }
}
