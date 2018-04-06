using Redecard.PN.Comum;
using Redecard.PNCadastrais.Core.Web.Controles.Portal;
using System;

namespace Redecard.PN.DadosCadastrais.SharePoint.WPAberto.PassosNovo
{
    /// <summary>
    /// WebPart Comum de passos editáveis para cabeçalho de webparts
    /// </summary>
    public partial class PassosNovoUserControl : UserControlBase
    {
        /// <summary>
        /// Passos da Webpart
        /// </summary>
        public PassosNovo WebPartPassos { get; set; }

        /// <summary>
        /// Page Load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                using (var log = Logger.IniciarLog("Web Part Passos Novo - Page Load"))
                {
                    ctlPassos.Passos.Clear();
                    if (!String.IsNullOrEmpty(WebPartPassos.PassosDescricao))
                    {
                        var passos = WebPartPassos.PassosDescricao.Split(';');

                        foreach (var passo in passos)
                        {
                            var novoPasso = new Passo { Descricao = passo };
                            ctlPassos.Passos.Add(novoPasso);
                        }

                        ctlPassos.PassoAtivo = WebPartPassos.PassoAtivo.ToInt32();
                    }
                }
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Web Part Passos Novo - Page Load", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Web Part Passos Novo - Page Load", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }
    }
}
