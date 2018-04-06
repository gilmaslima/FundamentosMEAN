/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 28/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.FMS.Sharepoint.Modelo
{
    /// <summary>
    /// Contém todas as properiedades padrões dos registros contidos nas consultas de produtividade consolidada por data ou analista.
    /// </summary>
    public class ItemTabelaProdutividadeConsolidada : IComparable<ItemTabelaProdutividadeConsolidada>
    {
        public TipoItemTabela TipoItem { get; set; }
        public int IdentificadorAgrupamento { get; set; }
        public string UsuarioLogin { get; set; }
        public DateTime Data { get; set; }
        public long QuantidadeCartoesAnalisados { get; set; }
        public long QuantidadeCartoesFraudulentos { get; set; }
        public long QuantidadeTransacoesFraudulentas { get; set; }
        public decimal ValorFraude { get; set; }
        public long QuantidadeCartoesNaoFraudulentos { get; set; }
        public long QuantidadeTransacoesNaoFraudulentas { get; set; }
        public decimal ValorNaoFraude { get; set; }

        public string ObterDataFormatadaRegistro
        {
            get
            {
                if (TipoItem == TipoItemTabela.Total)
                    return "";
                else
                    return Data.ToString("dd/MM/yyyy");
            }

        }

        /// <summary>
        /// Enum contendo os tipos distintos de linha que serão exibidos no GridView.
        /// 
        /// O valor do item é diretamente proporcional a posição de sua ordem de aparição no GridView.
        /// </summary>
        public enum TipoItemTabela
        {
            Detalhe = 1,
            SubTotal = 2,
            Total = 3
        }

        public int RowSpan { get; set; }

        /// <summary>
        /// Implementa os critérios de ordenação deixando uma ordenação por grupo, em caso de serem 
        /// do mesmo grupo, será comparado por 
        /// tipo, fazendo com que ao ser listado, todos os itens de um grupo, sequencialmente 
        /// apareça seu respectivo subtotal, e após todos os itens 
        /// e subtotais, por fim, o total geral.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareTo(ItemTabelaProdutividadeConsolidada y)
        {
            if (this.IdentificadorAgrupamento == y.IdentificadorAgrupamento)
            {
                if (this.TipoItem == y.TipoItem)
                {
                    return y.RowSpan - this.RowSpan;
                }
                else
                {
                    return this.TipoItem - y.TipoItem;
                }
            }
            else
            {
                return (this.IdentificadorAgrupamento - y.IdentificadorAgrupamento);
            }
        }
    }
}
