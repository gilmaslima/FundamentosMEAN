/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.OutrosServicos.Modelo
{
    /// <summary>
    /// Classe modelo de Taxa da Meta
    /// </summary>
    public class TaxaMeta
    {
        /// <summary>
        /// Código da Bandeira
        /// </summary>
        public Int64 CodigoBandeira { get; set; }

        /// <summary>
        /// Código da Modalidade
        /// </summary>
        public Int64 CodigoModalidade { get; set; }

        /// <summary>
        /// Código da Precificação do Produto
        /// </summary>
        public Int64? CodigoPrecificacaoProduto { get; set; }

        /// <summary>
        /// Código do Tipo de Meta
        /// </summary>
        public Int64? CodigoTipoMeta { get; set; }

        /// <summary>
        /// Código do Tipo de Precificação
        /// </summary>
        public Int32? CodigoTipoPrecificacao { get; set; }

        /// <summary>
        /// Código do Tipo de Valor de Meta
        /// </summary>
        public Int64? CodigoTipoValorMeta { get; set; }

        /// <summary>
        /// Código do Tipo de Valor Precificação
        /// </summary>
        public Int32? CodigoTipoValorPrecificacao { get; set; }

        /// <summary>
        /// Nome da Bandeira
        /// </summary>
        public String DescricaoBandeira { get; set; }

        /// <summary>
        /// Nome da Modalidade
        /// </summary>
        public String DescricaoModalidade { get; set; }

        /// <summary>
        /// Nome do Tipo de Meta
        /// </summary>
        public String NomeTipoMeta { get; set; }

        /// <summary>
        /// Nome do Valor de Tipo de Meta
        /// </summary>
        public String NomeTipoValorMeta { get; set; }

        /// <summary>
        /// Número da Parcela Máxima Final da Taxa 
        /// </summary>
        public Int64? NumeroParcelaFinal { get; set; }

        /// <summary>
        /// Número da Parcela Máxima Inicial da Taxa
        /// </summary>
        public Int64? NumeroParcelaInicial { get; set; }

        /// <summary>
        /// Prazo máximo da Taxa
        /// </summary>
        public Int64 Prazo { get; set; }

        /// <summary>
        /// Tarifa da Taxa
        /// </summary>
        public Double? Tarifa { get; set; }

        /// <summary>
        /// Valor de Taxa do Produto
        /// </summary>
        public Double? Taxa { get; set; }
    }
}
