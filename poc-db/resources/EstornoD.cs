/*
© Copyright 2015 Rede S.A.
Autor : Dhouglas Lombello
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Estorno.<br/>
    /// Representa os registros do tipo "D".
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book BKWA2940 / Programa WAC294 / TranID WAAQ
    /// </remarks>
    [DataContract]
    public class EstornoD : Estorno
    {
        /// <summary>Número do Estabelecimento</summary>
        [DataMember]
        public Int32 NumeroPV { get; set; }

        /// <summary>Data e Hora da Venda</summary>
        [DataMember]
        public DateTime DataHoraVenda { get; set; }

        /// <summary>Data e Hora do Estorno</summary>
        [DataMember]
        public DateTime DataHoraEstorno { get; set; }

        /// <summary>Descrição do Tipo da Conta</summary>
        [DataMember]
        public String DescricaoTipoConta { get; set; }

        /// <summary>Descrição da Modalidade da Venda</summary>
        [DataMember]
        public String DescricaoModalidadeVenda { get; set; }

        /// <summary>Quantidade de Parcelas</summary>
        [DataMember]
        public Int16 Plano { get; set; }

        /// <summary>Descrição da Bandeira</summary>
        [DataMember]
        public String DescricaoBandeira { get; set; }

        /// <summary>NSU da Transação</summary>
        [DataMember]
        public Int64 NSU { get; set; }

        /// <summary>Código do Terminal</summary>
        [DataMember]
        public String CodigoTerminal { get; set; }

        /// <summary>Número do Cartão</summary>
        [DataMember]
        public String NumeroCartao { get; set; }

        /// <summary>Valor da Venda</summary>
        [DataMember]
        public Decimal ValorVenda { get; set; }

        /// <summary>Indicador de Tokenizacao</summary>
        [DataMember]
        public String IndicadorTokenizacao { get; set; }
    }
}