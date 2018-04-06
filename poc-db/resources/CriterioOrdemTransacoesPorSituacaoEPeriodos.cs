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
    /// Enumeração utilizado para critério de ordem de transações por situacao e período.
    /// </summary>
    public enum CriterioOrdemTransacoesPorSituacaoEPeriodo
    {
        SituacaoFraude = 0,
        TipoAlarme = 1,
        TipoResposta = 2,
        NumeroCartao = 3,
        Data = 4,
        Valor = 5,
        Score = 6,
        Mcc = 7,
        UF = 8,
        TipoCartao = 9,
        Bandeira = 10,
        LoginUsuario = 11,
        DataAnalise = 12,
        DataTratamento = 13
    }
}
