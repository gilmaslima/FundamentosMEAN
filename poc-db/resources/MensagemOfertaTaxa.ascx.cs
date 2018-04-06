using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Microsoft.SharePoint;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates
{
    /// <summary>
    /// Controle para consulta de mensagem e verificação de oferta para Estabelecimentos
    /// </summary>
    public partial class MensagemOfertaTaxa : UserControlBase
    {
        #region [ SP Listas ]

        /// <summary>
        /// Lista Período Migração
        /// </summary>
        private SPList ListaMensagens
        {
            get
            {
                //Recupera a lista de "Mensagem Oferta de Taxa" em sites/fechado/extrato
                using (SPSite spSite = SPContext.Current.Site.WebApplication.Sites["sites/fechado"])
                using (SPWeb spWeb = spSite.AllWebs["extrato"])
                    return spWeb.Lists.TryGetList("Mensagem Oferta de Taxa");
            }
        }

        #endregion

        #region [Propriedades]

        /// <summary>
        /// Indica se os PVs passados possuem Oferta
        /// </summary>
        public Boolean PossuiOferta
        {
            get
            {

                if (!Object.ReferenceEquals(ViewState["_PossuiOferta"], null))
                    return (Boolean)ViewState["_PossuiOferta"];
                else
                    return false;
            }
            set
            {
                ViewState["_PossuiOferta"] = value;
            }
        }

        #endregion

        /// <summary>
        /// Inicilização do Controle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Carregar a mensagem da taxa
        /// </summary>
        /// <param name=""></param>
        public void CarregarMensagem()
        {
			this.PossuiOferta = true;
			lblMensagemOferta.Text = this.ConsultarMensagem();
        }

        /// <summary>
        /// Consultar a mensagem a Exibir
        /// </summary>
        private String ConsultarMensagem()
        {
            String retorno = default(String);

            using (Logger log = Logger.IniciarLog("Consultar a mensagem a Exibir"))
            {
                try
                { 
                    if (this.ListaMensagens != null)
                    {
                        SPQuery query = new SPQuery();
                        String camelQuery = String.Concat(
                            "<Where>",
                                "<Eq>",
                                    "<FieldRef Name=\"Title\" />",
                                    "<Value Type=\"Text\">{0}</Value>",
                                "</Eq>",
                            "</Where>");

                        camelQuery = String.Format(camelQuery, "OfertaTaxaFixa");
                        
                        query.Query = camelQuery; 

                        SPListItemCollection mensagem = this.ListaMensagens.GetItems(query);
                        
                        log.GravarMensagem("Mensagem na lista", new { mensagem });

                        if (mensagem.Count > 0)
                        {
                            retorno = mensagem[0]["Mensagem"].ToString();
                        }
                    }

                }
                catch (SPException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return retorno;
        }
    }
}
