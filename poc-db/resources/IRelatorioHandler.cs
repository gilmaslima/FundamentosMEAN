
using System;
namespace Redecard.PN.Extrato.SharePoint.Modelo
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRelatorioHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dados"></param>
        void Pesquisar(BuscarDados dados);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        String ObterTabelaExcel(BuscarDados dados, Int32 quantidadeRegistros, Boolean incluirTotalizadores);

        /// <summary>
        /// 
        /// </summary>
        String IdControl { get; }
    }
}
