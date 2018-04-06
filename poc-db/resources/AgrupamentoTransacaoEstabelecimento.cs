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

namespace Redecard.PN.FMS.Servico.Modelo.Transacoes
{
    /// <summary>
    /// Este componente publica a classe AgrupamentoTransacaoEstabelecimento, que expõe propriedades para manipular dados de agrupamento de transação por estabelecimento. 
    /// </summary>
    [DataContract]
    public class AgrupamentoTransacaoEstabelecimento
    {
        [DataMember]
        public long QuantidadeTransacoesSuspeitasAprovadas { get; set; }
        [DataMember]
        public decimal ValorTransacoesSuspeitasAprovadas { get; set; }
        [DataMember]
        public long QuantidadeTransacoesSuspeitasNegadas { get; set; }
        [DataMember]
        public decimal ValorTransacoesSuspeitasNegadas { get; set; }
        [DataMember]
        public IndicadorTipoCartao TipoCartao { get; set; }
        [DataMember]
        public long NumeroEstabelecimento { get; set; }
        [DataMember]
        public string NomeFantasiaEstabelecimento { get; set; }
        [DataMember]
        public long QuantidadeTotalTransacoes { get; set; }
        [DataMember]
        public decimal ValorTotalTransacoes { get; set; }
    }
}
