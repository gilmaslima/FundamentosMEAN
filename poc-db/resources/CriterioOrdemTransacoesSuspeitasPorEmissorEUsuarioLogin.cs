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
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.FMS.Servico.Modelo
{
    /// <summary>
    /// Enumeração utilizado para critérios de ordem de transações suspeitas por emissor e usuário logado.
    /// </summary>
    [DataContract]
    public enum CriterioOrdemTransacoesSuspeitasPorEmissorEUsuarioLogin
    {
        [EnumMember]
        Valor = 0,
        [EnumMember]
        Data = 1,
        [EnumMember]
        Score = 2,
        [EnumMember]
        TipoAlarme = 3,
        [EnumMember]
        NumeroCartao = 4,
        [EnumMember]
        Mcc = 5,
        [EnumMember]
        TipoCartao = 6,
        [EnumMember]
        UF = 7,
        [EnumMember]
        Bandeira = 8,
        [EnumMember]
        SituacaoFraude = 9,
        [EnumMember]
        DataTratamento = 10
    }
}
