/*
© Copyright 2017 Rede S.A.
Autor	: Francisco Mazali
Empresa	: Iteris Consultoria e Software LTDA
*/

using System;
using System.Runtime.Serialization;
using Status = Redecard.PN.Extrato.Servicos.Contrato.ContratoDados.Response.StatusRetorno;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// Classe para o response de relatório de extrato
    /// </summary>
    [DataContract]
    public abstract class RelatorioResponseBase
    {
        /// <summary>
        /// Construtor da classe com parametros de sucesso.
        /// </summary>
        public RelatorioResponseBase(): this (0, Status.OK, Status.OK, "Sucesso")
        {
        }

        /// <summary>
        /// Construtor da classe com parametros de sucesso.
        /// </summary>
        public RelatorioResponseBase(Int32 quantidade, Status statusTotalizador, Status statusRelatorio, String mensagem)
        {
            this.QuantidadeTotalRegistros = quantidade;
            this.StatusTotalizador = statusTotalizador;
            this.StatusRelatorio = statusRelatorio;
            this.Mensagem = mensagem;
        }

        /// <summary>
        /// Define a Quantidade Total Registros.
        /// </summary>
        [DataMember]
        public Int32 QuantidadeTotalRegistros { get; set; }

        /// <summary>
        /// Define o Status do Totalizador.
        /// </summary>
        [DataMember]
        public Status StatusTotalizador { get; set; }

        /// <summary>
        /// Define o Status do Relatorio.
        /// </summary>
        [DataMember]
        public Status StatusRelatorio { get; set; }

        /// <summary>
        /// Define a mensagem de retorno.
        /// </summary>
        [DataMember]
        public String Mensagem { get; set; }
    }
}