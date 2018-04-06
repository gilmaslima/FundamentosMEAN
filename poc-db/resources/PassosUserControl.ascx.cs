using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.Web.Script.Serialization;
using System.Collections.Generic;

namespace Redecard.PN.DadosCadastrais.SharePoint.WPAberto.Passos
{
    /// <summary>
    /// WebPart Comum de passos editáveis para cabeçalho de webparts
    /// </summary>
    public partial class PassosUserControl : UserControlBase
    {
        /// <summary>
        /// Passos da Webpart
        /// </summary>
        public Passos WebPartPassos { get; set; }

        /// <summary>
        /// Page Load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                using (var log = Logger.IniciarLog("Web Part Passos - Page Load"))
                {
                    ctlPassos.Passos.Clear();
                    if (!String.IsNullOrEmpty(WebPartPassos.PassosDescricao))
                    {
                        var passos = WebPartPassos.PassosDescricao.Split(';');

                        foreach (var passo in passos)
                        {
                            var novoPasso = new Redecard.PN.Comum.Web.Passo { Descricao = passo };
                            ctlPassos.Passos.Add(novoPasso);
                        }

                        ctlPassos.PassoAtivo = WebPartPassos.PassoAtivo.ToInt32();
                    }
                }
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Web Part Passos - Page Load", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Web Part Passos - Page Load", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }
    }
}
