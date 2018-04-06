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
    /// Enumeração utilizado para critério de ordem de transações por numero cartão ou associada.
    /// </summary>
    public enum CriterioOrdemTransacoesPorNumeroCartaoOuAssociada
    {
        SituacaoFraude = 0,
        TipoAlarme = 1,
        TipoResposta = 2,
        DataTransacao = 3,
        Valor = 4,
        Score = 5,
        Autorizacao = 6,
        Estabelecimento = 7,
        Mcc = 8,
        UF = 9,
        TipoCartao = 10,
        EntryMode = 11,
        LoginUsuario = 12,
        DataAnalise = 13
    }
}
