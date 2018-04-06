/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Vendas
{
    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos totalizadores do Relatório de Vendas - Construcard.<br/>
    /// Representa os Totais por Bandeira.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1314 / Programa WA1314 / TranID ISHE
    /// </remarks>
    [DataContract]
    public class ConstrucardTotalizadorValor : BasicContract
    {
        /// <summary>Descrição da Bandeira</summary>
        [DataMember]
        public String Bandeira { get; set; }

        /// <summary>Valor Bruto</summary>
        [DataMember]
        public Decimal ValorBruto { get; set; }

        /// <summary>Valor Líquido</summary>
        [DataMember]
        public Decimal ValorLiquido { get; set; }

        /// <summary>Valor Descontado</summary>
        [DataMember]
        public Decimal ValorDescontado { get; set; }
    }
}