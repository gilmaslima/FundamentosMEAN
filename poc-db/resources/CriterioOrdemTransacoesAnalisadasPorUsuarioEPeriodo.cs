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
    /// Enumeração utilizado para critérios de ordem de transações analisadas por usuário e período.
    /// </summary>
    [DataContract]
    public enum CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo
    {
        [EnumMember]
        SituacaoFraude = 0,
        [EnumMember]
        TipoAlarme = 1,
        [EnumMember]
        TipoResposta = 2,
        [EnumMember]
        NumeroCartao = 3,
        [EnumMember]
        Data = 4,
        [EnumMember]
        Valor = 5,
        [EnumMember]
        Score = 6,
        [EnumMember]
        Mcc = 7,
        [EnumMember]
        UF = 8,
        [EnumMember]
        TipoCartao = 9,
        [EnumMember]
        Bandeira = 10,
        [EnumMember]
        UsuarioLogin = 11,
        [EnumMember]
        DataAnalise = 12,
        [EnumMember]
        DataTratamento = 13
    }
}
