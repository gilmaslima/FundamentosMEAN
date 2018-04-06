#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [Agnaldo Costa]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [19/07/2012] – [Agnaldo Costa de Almeida] – [Criação]
*/
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Modelo de Faixa de Consultas da Franquia.
    /// Remodelagem da estrutura ListarRegime_FILLER2 do COMTI ListarRegime
    /// </summary>
    [DataContract]
    public class FaixaConsultaFranquia
    {
        /// <summary>
        /// Faixa Inicial de Consultas
        /// </summary>
        [DataMember]
        public Decimal FaixaInicial { get; set; }

        /// <summary>
        /// Faixa Final de Consultas
        /// </summary>
        [DataMember]
        public Decimal FaixaFinal { get; set; }

        /// <summary>
        /// Valor da Consulta
        /// </summary>
        [DataMember]
        public Decimal ValorConsulta { get; set; }
    }

    /// <summary>
    /// Modelo de Franquia.
    /// Remodelagem da estrutura ListarRegime_FILLER do COMTI ListarRegime
    /// </summary>
    [DataContract]
    public class RegimeFranquia
    {
        /// <summary>
        /// Código de regime da Franquia
        /// </summary>
        [DataMember]
        public Int32 CodigoRegime { get; set; }

        /// <summary>
        /// Lista de Faixas de Valores por Consulta da Franquia
        /// </summary>
        [DataMember]
        public List<FaixaConsultaFranquia> FaixasConsultaFranquia { get; set; }

        /// <summary>
        /// Quantidade de Consultas da Franquia
        /// </summary>
        [DataMember]
        public Int16 QuantidadeConsulta { get; set; }

        /// <summary>
        /// Indicador de Rede.
        /// S = Sim. N = Não.
        /// </summary>
        [DataMember]
        public String Rede { get; set; }

        /// <summary>
        /// Valor da Franquia
        /// </summary>
        [DataMember]
        public Decimal ValorFranquia { get; set; }

        /// <summary>
        /// Valor da Consulta de Franquia
        /// </summary>
        [DataMember]
        public Decimal ValorConsulta { get; set; }

    }
}