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
    /// Enumeração utilizado para criterios de ordem detransações agrupadas por estabelecimento.
    /// </summary>
    [DataContract]
    public enum CriterioOrdemTransacoesAgrupadasPorEstabelecimento
    {
        [EnumMember]
        Valor = 0,
        [EnumMember]
        Quantidade = 1,
        [EnumMember]
        NomeFantasiaEstabelecimento = 2,
        [EnumMember]
        NumeroEstabelecimento = 3,
        [EnumMember]
        ValorTransacoesSuspeitasAprovadas = 4,
        [EnumMember]
        ValorTransacoesSuspeitasNegadas = 6,
        [EnumMember]
        QuantidadeTransacoesSuspeitasAprovadas = 5,
        [EnumMember]
        QuantidadeTransacoesSuspeitasNegadas = 7,
        [EnumMember]
        TipoCartao = 8
    }
}


            
