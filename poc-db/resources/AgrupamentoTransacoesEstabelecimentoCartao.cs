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

namespace Redecard.PN.FMS.Modelo
{
    /// <summary>
    /// Este componente publica a classe AgrupamentoTransacoesEstabelecimentoCartao, que expõe propriedades para manipular dados de agrupamento de transações de estabelecimento por cartão.
    /// </summary>
    public class AgrupamentoTransacoesEstabelecimentoCartao
    {
        public long QuantidadeTransacoesAprovadas { get; set; }
        public decimal ValorTransacoesAprovadas { get; set; }
        public string NumeroCartao { get; set; }
        public long QuantidadeTransacoesSuspeitas { get; set; }
        public decimal ValorTransacoesSuspeitas { get; set; }
        public IndicadorTipoCartao TipoCartao { get; set; }
        public long QuantidadeTotalTransacoes { get; set; }
        public decimal ValorTotalTransacoes { get; set; }
        public SituacaoCartao Situacao { get; set; }
    }
}
