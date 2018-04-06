using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rede.PN.ApiLogin.Modelo
{
    // <summary>
    /// Estrutura básica de um item de menu, usado para fazer a vinculação pai -> filho
    /// </summary>
    public struct MenuInfo
    {

        /// <summary>
        /// 
        /// </summary>
        public Int32 CodigoServico;

        /// <summary>
        /// 
        /// </summary>
        public Int32 CodigoServicoPai;

        /// <summary>
        /// 
        /// </summary>
        public String Observacoes;

        /// <summary>
        /// 
        /// </summary>
        public String NomeServico;

        /// <summary>
        /// 
        /// </summary>
        public List<PaginaInfo> Paginas;

        /// <summary>
        /// 
        /// </summary>
        public Boolean FlagMenu;

        /// <summary>
        /// Indica se o menu é um serviço basico
        /// </summary>
        public Boolean ServicoBasico;
    }
}
