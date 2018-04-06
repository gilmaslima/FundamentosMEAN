using System;
using System.ComponentModel;

namespace Redecard.PN.Extrato.SharePoint.Helper.ConsultarVendas
{
    [TypeConverter(typeof(ExpandableObjectConverter)), Serializable]
    public class ConsultarVendasSumarioItem
    {
        /// <summary>
        /// Construtor da classe para inicialização default
        /// </summary>
        public ConsultarVendasSumarioItem() { }

        /// <summary>
        /// Construtor da classe para inicialização com conteúdo predefinido
        /// </summary>
        /// <param name="descricao">Descrição do item de resumo</param>
        /// <param name="valor">Valor do item de resumo</param>
        public ConsultarVendasSumarioItem(String descricao, String valor)
        {
            this.Descricao = descricao;
            this.Valor = valor;
        }

        /// <summary>
        /// Descrição do item de resumo
        /// </summary>
        [NotifyParentProperty(true)]
        public String Descricao { get; set; }

        /// <summary>
        /// Valor do item de resumo
        /// </summary>
        [NotifyParentProperty(true)]
        public String Valor { get; set; }
    }
}
