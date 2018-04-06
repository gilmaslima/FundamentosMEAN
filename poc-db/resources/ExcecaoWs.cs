using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.GerencieExtrato.SharePoint.Helper
{
    /// <summary>
    /// Classe interna para controlar a exceção gerada na chamada do WebServi
    /// </summary>
    public class ExcecaoWs : Exception
    {
        #region [Propriedades]

        /// <summary>
        /// Código do erro.
        /// </summary>
        public Int32 CodigoErro { get; set; }

        /// <summary>
        /// Fonte do erro.
        /// </summary>
        public String Fonte { get; set; }

        #endregion

        #region [Construtores]

        public ExcecaoWs(Int32 codigoErro, String fonte, String mensagem)
            : base(mensagem)
        {
            this.CodigoErro = codigoErro;
            this.Fonte = fonte;
        }

        public ExcecaoWs(Int32 codigoErro, String fonte)
        {
            this.CodigoErro = codigoErro;
            this.Fonte = fonte;
        }

        public ExcecaoWs()
        {
        }

        #endregion
    }
}
