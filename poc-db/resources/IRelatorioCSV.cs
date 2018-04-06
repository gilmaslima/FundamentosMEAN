using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.SharePoint.Modelo
{
    public interface IRelatorioCSV
    {
        /// <summary>
        /// ID do Controle
        /// </summary>
        String IdControl { get; }

        /// <summary>
        /// Obtém o conteúdo do CSV
        /// </summary>
        void GerarConteudoRelatorio(BuscarDados dadosBusca, Action<String> funcaoOutput);
    }
}
