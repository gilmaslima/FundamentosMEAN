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
    /// Enumeração utilizado para critério de classificação de estabelecimento.
    /// </summary>
    public enum CriterioClassificacaoEstabelecimento
    {
        Valor = 0,
        Quantidade = 1,
        NomeFantasiaEstabelecimento = 2,
        NumeroEstabelecimento = 3,
        ValorTransacoesSuspeitasAprovadas = 4,
        QuantidadeTransacoesSuspeitasAprovadas = 5,
        ValorTransacoesSuspeitasNegadas = 6,
        QuantidadeTransacoesSuspeitasNegadas = 7,
        TipoCartao = 8
    }
}
