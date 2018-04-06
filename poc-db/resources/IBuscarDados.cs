using System;
namespace Redecard.PN.Extrato.SharePoint.Modelo
{
    public interface IBuscarDados
    {
        /// <summary>
        /// 
        /// </summary>
        Int16 CodigoBandeira { get; set; }

        /// <summary>
        /// 
        /// </summary>
        DateTime DataFinal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        DateTime DataInicial { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Int32[] Estabelecimentos { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Int32 IDRelatorio { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Int32 IDTipoVenda { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Int32 TipoEstabelecimento { get; set; }
    }
}