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
    /// Enumeração utilizado para critério de ordem de transações agrupadas por cartão.
    /// </summary>
    public enum CriterioOrdemTransacoesAgrupadasPorCartao
    {
        Valor = 0,
        Data = 1,
        Score = 2,
        Cartao = 3,
        ValorTransacoesSuspeitasAprovadas = 4,
        QuantidadeTransacoesSuspeitasAprovadas = 5,
        ValorTransacoesSuspeitasNegadas = 6,
        QuantidadeTransacoesSuspeitasNegadas = 7,
        TipoCartao = 8

    }
}
