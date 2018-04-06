using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA615 - Resumo de vendas - Cartões de débito - Vencimentos.
    /// </summary>
    [DataContract]
    public class ConsultarVencimentosResumoVendaEnvio
    {
        [DataMember]
        public int NumeroEstabelecimento { get; set; }
        [DataMember]
        public int NumeroResumoVenda { get; set; }
        [DataMember]
        public DateTime DataApresentacao { get; set; }
    }
}