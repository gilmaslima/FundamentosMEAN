using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Comum;
using System.ServiceModel;

namespace Redecard.PN.OutrosServicos.SharePoint.Layouts.OutrosServicos
{
    public partial class AceiteRestricao : ApplicationPageBaseAutenticada
    {
        /// <summary>
        /// Sessão do usuário
        /// </summary>
        private Sessao _sessao = null;

        /// <summary>
        /// Sessão atual do usuário
        /// </summary>
        private Sessao SessaoAtual
        {
            get
            {
                if (_sessao != null && Sessao.Contem())
                    return _sessao;
                else
                {
                    if (Sessao.Contem())
                    {
                        _sessao = Sessao.Obtem();
                    }
                    return _sessao;
                }
            }
        }

        /// <summary>
        /// Inicialização da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (SessaoAtual != null)
                {
                    if (!Page.IsPostBack)
                    {
                        if (Request.QueryString["dados"] != null)
                        {
                            QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);

                            if (queryString["Aceite"] != null)
                            {
                                phdComAceiteBottom.Visible = !(phdSemAceiteBottom.Visible = queryString["Aceite"].ToString().Equals("0"));
                                phdSemAceiteTop.Visible = queryString["Aceite"].ToString().Equals("0");

                                if (queryString["CodigoVersao"] != null)
                                {
                                    Int32 codigoVersao = (Int32)queryString["CodigoVersao"].ToString().ToInt32Null(0);

                                    using (var regimeServicoCliente = new RegimeFranquiaServico.RegimeFranquiaServicoClient())
                                    {
                                        var regimeContrato = regimeServicoCliente.ConsultarContratoRestricao(codigoVersao);
                                        if (!Object.ReferenceEquals(regimeContrato, null))
                                            txtContrato.Text = regimeContrato.CorpoContrato;
                                        else
                                        {
                                            dvPainel.Controls.Add(base.RetornarPainelExcecao("Contrato não encontrado."));
                                            pnlContrato.Visible = false;
                                        }

                                    }
                                }
                                else
                                {
                                    dvPainel.Controls.Add(base.RetornarPainelExcecao("Dados do contrato inválidos."));
                                    pnlContrato.Visible = false;
                                }
                            }
                            else
                            {
                                dvPainel.Controls.Add(base.RetornarPainelExcecao("Dados do contrato inválidos."));
                                pnlContrato.Visible = false;
                            }
                        }
                        else
                        {
                            dvPainel.Controls.Add(base.RetornarPainelExcecao("Dados do contrato inválidos."));
                            pnlContrato.Visible = false;
                        }

                        // O usuario do tipo atendimento tem permissao apenas para visualizar a pagina
                        if (SessaoAtual.UsuarioAtendimento)
                        {
                            phdSemAceiteTop.Visible = false;
                            phdSemAceiteBottom.Visible = false;
                            phdComAceiteBottom.Visible = false;
                        }
                    }
                }
                else
                {
                    dvPainel.Controls.Add(base.RetornarPainelExcecao("Sessão inválida."));
                }
            }
            catch (FaultException<RegimeFranquiaServico.GeneralFault> ex)
            {
                dvPainel.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
                pnlContrato.Visible = false;
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                dvPainel.Controls.Add(base.RetornarPainelExcecao(FONTE, CODIGO_ERRO));
                pnlContrato.Visible = false;
            }
        }
    }
}
