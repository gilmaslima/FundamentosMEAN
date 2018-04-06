using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA152 - Relatório de débitos e desagendamentos - Motivo débito.
    /// </summary>
    [DataContract]
    public class ConsultarMotivoDebitoEnvio
    {
        [DataMember]
        public int NumeroEstabelecimento { get; set; }
        [DataMember]
        public DateTime DataPesquisa { get; set; }
        [DataMember]
        public string Timestamp { get; set; }
        [DataMember]
        public decimal NumeroDebito { get; set; }
        [DataMember]
        public string TipoPesquisa { get; set; }
        [DataMember]
        public VersaoDebitoDesagendamento? Versao { get; set; }
    }
}