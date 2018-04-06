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
    public class ConsultarVencimentosResumoVendaRetorno
    {
        [DataMember]
        public DateTime DataVencimento { get; set; }
        [DataMember]
        public short NumeroPeca { get; set; }
        [DataMember]
        public short QuantidadePecas { get; set; }
        [DataMember]
        public short QuantidadeComprovanteVenda { get; set; }
        [DataMember]
        public string Descricao { get; set; }
        [DataMember]
        public decimal ValorApresentado { get; set; }
        [DataMember]
        public decimal ValorLiquido { get; set; }
        [DataMember]
        public int Banco { get; set; }
        [DataMember]
        public int Agencia { get; set; }
        [DataMember]
        public string Conta { get; set; }

    }
}