/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 27/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.ComponentModel;

namespace Redecard.PN.FMS.Sharepoint.ControlTemplates
{
    public partial class TabelaAcoes : UserControl
    {
        #region Listeners
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// A acao desse botao sera delegada para o RelatorioGridUserControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnExportar_Click(object sender, EventArgs e)
        {
            if (onExportarClick != null)
            {
                onExportarClick(e);
            }
        }

        #endregion

        #region Delegates e Eventos
        public delegate void ExportarClick(EventArgs e);
        [Browsable(true)]
        public event ExportarClick onExportarClick;
        #endregion Delegates e Eventos
    }
}
