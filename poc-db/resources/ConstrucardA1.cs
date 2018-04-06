/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Vendas
{
    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Vendas - Construcard.<br/>
    /// Representa os registros do tipo "A1" - Ajuste com Valor.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1315 / Programa WA1315 / TranID ISHF
    /// </remarks>
    [DataContract]
    public class ConstrucardA1 : Construcard
    {
        /// <summary>Área da chave do registro</summary>
        [DataMember]
        public String ChaveRegistro { get; set; }

        /// <summary>Descrição da Bandeira</summary>
        [DataMember]
        public String Bandeira { get; set; }

        /// <summary>Descrição do Total</summary>
        [DataMember]
        public String Descricao { get; set; }

        /// <summary>Valor Total Apresentado</summary>
        [DataMember]
        public Decimal ValorApresentado { get; set; }

        /// <summary>Valor Total Desconto</summary>
        [DataMember]
        public Decimal ValorDesconto { get; set; }

        /// <summary>Valor Total Líquido</summary>
        [DataMember]
        public Decimal ValorLiquido { get; set; }

        /// <summary>Indicador de Sinal do Valor</summary>
        [DataMember]
        public String DebitoCredito { get; set; }

        /// <summary>Banco de Crédito</summary>
        [DataMember]
        public Int32 BancoCredito { get; set; }

        /// <summary>Agência de Crédito</summary>
        [DataMember]
        public Int32 AgenciaCredito { get; set; }

        /// <summary>Conta de Crédito</summary>
        [DataMember]
        public String ContaCredito { get; set; }
    }
}