/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Barbosa dos Santos]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/11/05 - Tiago Barbosa dos Santos - Versão Inicial
*/
using System.Runtime.Serialization;

namespace Redecard.PN.Emissores.Servicos
{
    [DataContract]
    public class DadosTelefone
    {

        [DataMember]
        public string DDD { get; set; }

        [DataMember]
        public string Telefone { get; set; }

        [DataMember]
        public string Ramal { get; set; }

    }
}
