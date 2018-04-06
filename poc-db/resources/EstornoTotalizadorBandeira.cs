/*
© Copyright 2015 Rede S.A.
Autor : Dhouglas Lombello
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos totalizadores do Relatório de Estorno.<br/>
    /// Representa os Totais.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book BKWA2930 / Programa WAC293 / TranID WAAP
    /// </remarks>
    [DataContract]
    public class EstornoTotalizadorBandeira : BasicContract
    {
        /// <summary>Descrição da Bandeira</summary>
        [DataMember]
        public String DescricaoBandeira { get; set; }

        /// <summary>Valor da Bandeira</summary>
        [DataMember]
        public Decimal ValorBandeira { get; set; }
    }
}