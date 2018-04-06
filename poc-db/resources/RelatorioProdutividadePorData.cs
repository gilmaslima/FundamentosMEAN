/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Redecard.PN.FMS.Servico.Modelo
{
    /// <summary>
    /// Este componente publica a classe RelatorioProdutividadePorData, que expõe propriedades para manipular dados do relatório de produtividade por data.
    /// </summary>
    [DataContract]
    public class RelatorioProdutividadePorData
    {
        [DataMember]
        public List<AgrupamentoProdutividadePorData> ProdutividadePorData { get; set; }
        [DataMember]
        public long QuantidadeCartoesAnalisados { get; set; }
        [DataMember]
        public decimal ValorTotalFraude { get; set; }
        [DataMember]
        public long QuantidadeTotalCartoesFraudulentos { get; set; }
        [DataMember]
        public long QuantidadeTotalTransacoesFraudulentas { get; set; }
        [DataMember]
        public decimal ValorTotalNaoFraude { get; set; }
        [DataMember]
        public long QuantidadeTotalCartoesNaoFraudulentos { get; set; }
        [DataMember]
        public long QuantidadeTotalTransacoesNaoFraudulentas { get; set; }
    }
}
