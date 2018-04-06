/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;

namespace Redecard.PN.Extrato.Modelo.Vendas
{
    /// <summary>
    /// Registro do Relatório de Vendas - Recarga de Celular de PV Físico
    /// </summary>
    public class RecargaCelularPvFisico : RecargaCelular
    {
        /// <summary>
        /// Tipo do Registro
        /// </summary>
        public String TipoRegistro { get; set; }

        /// <summary>
        /// Número do Estabelecimento
        /// </summary>
        public Int32 NumeroEstabelecimento { get; set; }

        /// <summary>
        /// NSU (Número Sequencial Único) da Recarga
        /// </summary>
        public Int32 NsuRecarga { get; set; }

        /// <summary>
        /// Data e Hora da Recarga
        /// </summary>
        public DateTime DataHoraRecarga { get; set; }

        /// <summary>
        /// Número do Resumo de Vendas
        /// </summary>
        public String NumeroRV { get; set; }

        /// <summary>
        /// Tipo de Venda
        /// </summary>
        public String TipoVenda { get; set; }

        /// <summary>
        /// Nome da Operadora
        /// </summary>
        public String NomeOperadora { get; set; }

        /// <summary>
        /// Número do Celular (mascarado)
        /// </summary>
        public String NumeroCelular { get; set; }

        /// <summary>
        /// Valor Bruto da Recarga
        /// </summary>
        public Decimal ValorBrutoRecarga { get; set; }

        /// <summary>
        /// Valor Líquido da Comissão
        /// </summary>
        public Decimal ValorLiquidoComissao { get; set; }

        /// <summary>
        /// Status da Comissão
        /// </summary>
        public String StatusComissao { get; set; }
    }
}