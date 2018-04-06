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
    /// Enumeração utilizado para critério de ordem de produtividade.
    /// </summary>
    public enum CriterioOrdemProdutividade
    {
        LoginUsuario = 0,
        Data = 1,
        QuantidadeCartoesAnalisados = 2,
        QuantidadeCartoesFraude = 3,
        QuantidadeTransacoesFraude = 4,
        ValorTotalFraude = 5,
        QuantidadeCartoesNaoFraude = 6,
        QuantidadeTransacoesNaoFraude = 7,
        ValorTotalNaoFraude = 8
        
        
    }
}
