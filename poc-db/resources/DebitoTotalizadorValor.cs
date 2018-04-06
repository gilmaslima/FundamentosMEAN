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
    /// Classe utilizada no módulo Extrato para consulta dos totalizadores do Relatório de Vendas - Débito.<br/>
    /// Representa os Totais por Bandeira.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1312 / Programa WA1312 / TranID ISHC
    /// </remarks>
    [DataContract]
    public class DebitoTotalizadorValor : BasicContract
    {
        /// <summary>Descrição da Bandeira</summary>
        [DataMember]
        public String Bandeira { get; set; }

        /// <summary>Valor Apresentado</summary>
        [DataMember]
        public Decimal ValorApresentado { get; set; }

        /// <summary>Valor Líquido</summary>
        [DataMember]
        public Decimal ValorLiquido { get; set; }

        /// <summary>Valor Descontado</summary>
        [DataMember]
        public Decimal ValorDescontado { get; set; }
    }
}