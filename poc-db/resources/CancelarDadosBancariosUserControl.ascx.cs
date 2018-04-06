using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Redecard.PN.Comum;
using System.ServiceModel;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;

using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.CancelarDadosBancarios
{
    public partial class CancelarDadosBancariosUserControl : UserControlBase
    {
        /// <summary>
        /// Dados de Domicílios Bancários da Entidade
        /// </summary>
        private List<EntidadeServico.DadosDomiciolioBancario> ListaDomiciliosBancario = new List<EntidadeServico.DadosDomiciolioBancario>();
        private EntidadeServico.DadosDomiciolioBancario DomicilioBancario = new DadosDomiciolioBancario();

        /// <summary>
        /// Inicializa a página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (Logger Log = Logger.IniciarLog("Cancelar dados bancários - Carregando página"))
                {
                    if (CarregarDomiciliosBancarios())
                    {
                        ExibeDadosCadastrais();
                        ExibeDomicilioNovo();
                    }
                }
            }
        }

        /// <summary>
        /// Limpar os checkbox selecionados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLimpar_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in rptDomiciolioNovo.Items)
            {
                CheckBox chkCancelar = (CheckBox)item.FindControl("chkCancelar");
                if (chkCancelar != null)
                    chkCancelar.Checked = false;

                btnEnviar.Enabled = btnLimpar.Enabled = false;
            }
        }

        /// <summary>
        /// Enviar as Solicitações para cancelamento de alteração
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Botão Enviar Solicitações para cancelamento de alteração"))
            {
                try
                {
                    CheckBox chkCancelar = new CheckBox();
                    Int32 codigoRetorno;
                    using (var entidadeCliente = new EntidadeServico.EntidadeServicoClient())
                    {
                        foreach (RepeaterItem item in rptDomiciolioNovo.Items)
                        {
                            codigoRetorno = 0;
                            chkCancelar = (CheckBox)item.FindControl("chkCancelar");
                            if (chkCancelar != null)
                            {
                                if (chkCancelar.Checked)
                                {
                                    chkCancelar.Checked = false;
                                    HiddenField hdnCodigoSolicitacao = (HiddenField)item.FindControl("hdnCodigoSolicitacao");

                                    if (hdnCodigoSolicitacao != null)
                                        entidadeCliente.CancelarAlteracao(out codigoRetorno, hdnCodigoSolicitacao.Value.ToInt32(), SessaoAtual.CodigoEntidade);

                                    Panel[] paineisCancelamento = new Panel[1]{
                                        pnlCancelarAlteracao
                                };

                                    if (codigoRetorno > 0)
                                    {
                                        base.ExibirPainelExcecao("EntidadeServico.CancelarAlteracao", codigoRetorno);
                                        return;
                                    }
                                    else
                                        base.ExibirPainelConfirmacaoAcao("Cancelamento Dados Bancários", "Cancelamento concluído.", SPUtility.GetPageUrlPath(HttpContext.Current), paineisCancelamento);
                                }
                            }
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao("Redecard.PN.SharePoint", 300);
                }
            }
        }

        /// <summary>
        /// Exibe os dados cadastrais da Entidade
        /// </summary>
        private void ExibeDadosCadastrais()
        {
            //using (Logger Log = Logger.IniciarLog("Exibindo dados cadastrais da entidade"))
            //{
            //    try
            //    {
            //        lblRazaoSocial.Text = DomicilioBancario.RazaoSocial.Trim().ToUpper();
            //        lblNomeFantasia.Text = DomicilioBancario.NomeFantasia.Trim().ToUpper();
            //        lblCPFCNPJ.Text = DomicilioBancario.CNPJ.Trim();

            //        if (DomicilioBancario.Trava)
            //        {
            //            if (DomicilioBancario.TipoTrava.Equals("C"))
            //            {
            //                lblTrava.Text = "Sim   Tipo Trava: C-Crédito";
            //            }
            //            else if (DomicilioBancario.TipoTrava.Equals("D"))
            //            {
            //                lblTrava.Text = "Sim   Tipo Trava: D-Débito";
            //            }
            //            else if (DomicilioBancario.TipoTrava.Equals("A"))
            //            {
            //                lblTrava.Text = "Sim   Ambos";
            //            }
            //        }

            //        lblCentralizado.Text = DomicilioBancario.Centralizado ? "Sim" : "Não";

            //        if (DomicilioBancario.Centralizado)
            //        {
            //            pnlCentralidora.Visible = true;
            //            lblCentralizadora.Text = DomicilioBancario.Centralizadora;
            //        }
            //        else
            //        {
            //            pnlCentralidora.Visible = false;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.GravarErro(ex);
            //        SharePointUlsLog.LogErro(ex);
            //        base.ExibirPainelExcecao("Redecard.PN.SharePoint", 300);
            //    }
            //}
        }

        /// <summary>
        /// Carrega no grid de Domicílios alterados a lista de solicitaçõesS
        /// </summary>
        private void ExibeDomicilioNovo()
        {
            rptDomiciolioNovo.DataSource = ListaDomiciliosBancario;
            rptDomiciolioNovo.DataBind();
            pnlMensagemSemDomicilio.Visible = (ListaDomiciliosBancario.Count == 0);
            if (pnlMensagemSemDomicilio.Visible)
            {
                rptDomiciolioNovo.Visible = false;
                QuadroAviso aviso = (QuadroAviso)qdAvisoSemDomicilio;
                aviso.ClasseImagem = "icone-aviso";
                aviso.CarregarMensagem("Aviso", "Não há dados de domicílio bancário pendentes para esse estabelecimento");
            }
        }

        /// <summary>
        /// Verifica se há Domicílios Bancários para a Entidade
        /// </summary>
        /// <returns></returns>
        private Boolean CarregarDomiciliosBancarios()
        {
            //using (Logger Log = Logger.IniciarLog("Carregando domicílios bancários para Entidade"))
            //{
            //    try
            //    {
            //        Int32 codigoRetorno;
            //        using (var entidadeServico = new EntidadeServico.EntidadeServicoClient())
            //        {
            //            DomicilioBancario = entidadeServico.ConsultarDomiciliosBancario(out codigoRetorno, SessaoAtual.CodigoEntidade);

            //            if (codigoRetorno > 0)
            //            {
            //                base.ExibirPainelExcecao("EntidadeServico.ConsultarDomiciliosBancario", codigoRetorno);
            //                return false;
            //            }
            //            else if (DomicilioBancario == null)
            //            {
            //                base.ExibirPainelExcecao("Redecard.PN.SharePoint", 300);
            //                return false;
            //            }
            //            else
            //            {
            //                var domiciliosAlterados = entidadeServico.ConsultarDomiciliosAlterados(SessaoAtual.CodigoEntidade);

            //                foreach (EntidadeServico.DadosDomiciolioBancario domicilio in domiciliosAlterados)
            //                    ListaDomiciliosBancario.Add(domicilio);

            //                return (ListaDomiciliosBancario != null);
            //            }
            //        }
            //    }
            //    catch (FaultException<EntidadeServico.GeneralFault> ex)
            //    {
            //        Log.GravarErro(ex);
            //        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
            //        return false;
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.GravarErro(ex);
            //        SharePointUlsLog.LogErro(ex);
            //        base.ExibirPainelExcecao("Redecard.PN.SharePoint", 300);
            //        return false;
            //    }
            //}
            return false;
        }

        /// <summary>
        /// Preenche dinamicamente o grid com as Solicitações de Alteração
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptDomiciolioNovo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
        //    try
        //    {
        //        if (e.Item.ItemType == ListItemType.Item)
        //        {
        //            EntidadeServico.DadosDomiciolioBancario dadosDomicilio = (EntidadeServico.DadosDomiciolioBancario)e.Item.DataItem;
        //            Label lblOperacao = (Label)e.Item.FindControl("lblOperacao");
        //            Label lblOcorrencia = (Label)e.Item.FindControl("lblOcorrencia");
        //            Label lblBanco = (Label)e.Item.FindControl("lblBanco");
        //            Label lblAgencia = (Label)e.Item.FindControl("lblAgencia");
        //            Label lblConta = (Label)e.Item.FindControl("lblConta");
        //            Label lblDataUltimaAlteracao = (Label)e.Item.FindControl("lblDataUltimaAlteracao");
        //            Label lblHoraUltimaAlteracao = (Label)e.Item.FindControl("lblHoraUltimaAlteracao");
        //            HiddenField hdnCodigoSolicitacao = (HiddenField)e.Item.FindControl("hdnCodigoSolicitacao");

        //            if (hdnCodigoSolicitacao != null)
        //                hdnCodigoSolicitacao.Value = dadosDomicilio.CodigoSolicitacao;

        //            if (lblOcorrencia != null)
        //                lblOcorrencia.Text = dadosDomicilio.CodigoSolicitacao;

        //            if (dadosDomicilio.TipoProduto.Equals("CR"))
        //            {
        //                if (lblOperacao != null)
        //                {
        //                    lblOperacao.Text = "Crédito";
        //                }

        //                if (lblBanco != null)
        //                {
        //                    lblBanco.Text = dadosDomicilio.NomeBancoCredido.ToString();
        //                }

        //                if (lblAgencia != null)
        //                {
        //                    lblAgencia.Text = dadosDomicilio.CodigoAgenciaCredito;
        //                }

        //                if (lblConta != null)
        //                {
        //                    lblConta.Text = dadosDomicilio.ContaCredito;
        //                }

        //                if (lblDataUltimaAlteracao != null)
        //                {
        //                    lblDataUltimaAlteracao.Text = dadosDomicilio.DataSolicitacao;
        //                }

        //                if (lblHoraUltimaAlteracao != null)
        //                {
        //                    lblHoraUltimaAlteracao.Text = dadosDomicilio.HoraSolicitacao;
        //                }
        //            }
        //            else if (dadosDomicilio.TipoProduto.Equals("DB"))
        //            {
        //                if (lblOperacao != null)
        //                {
        //                    lblOperacao.Text = "Débito";
        //                }

        //                if (lblBanco != null)
        //                {
        //                    lblBanco.Text = dadosDomicilio.NomeBancoDebito.ToString();
        //                }

        //                if (lblAgencia != null)
        //                {
        //                    lblAgencia.Text = dadosDomicilio.CodigoAgenciaDebito;
        //                }

        //                if (lblConta != null)
        //                {
        //                    lblConta.Text = dadosDomicilio.ContaDebito;
        //                }

        //                if (lblDataUltimaAlteracao != null)
        //                {
        //                    lblDataUltimaAlteracao.Text = dadosDomicilio.DataSolicitacao;
        //                }

        //                if (lblHoraUltimaAlteracao != null)
        //                {
        //                    lblHoraUltimaAlteracao.Text = dadosDomicilio.HoraSolicitacao;
        //                }
        //            }
        //            else if (dadosDomicilio.TipoProduto.Equals("CC"))
        //            {
        //                if (lblOperacao != null)
        //                {
        //                    lblOperacao.Text = "Construcard";
        //                }

        //                if (lblBanco != null)
        //                {
        //                    lblBanco.Text = dadosDomicilio.NomeBancoConstrucard.ToString();
        //                }

        //                if (lblAgencia != null)
        //                {
        //                    lblAgencia.Text = dadosDomicilio.CodigoAgenciaConstrucard;
        //                }

        //                if (lblConta != null)
        //                {
        //                    lblConta.Text = dadosDomicilio.ContaConstrucard;
        //                }

        //                if (lblDataUltimaAlteracao != null)
        //                {
        //                    lblDataUltimaAlteracao.Text = dadosDomicilio.DataSolicitacao;
        //                }

        //                if (lblHoraUltimaAlteracao != null)
        //                {
        //                    lblHoraUltimaAlteracao.Text = dadosDomicilio.HoraSolicitacao;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.GravarErro("Erro durante bind de grid de Solicitações de Alteração", ex);
        //        SharePointUlsLog.LogErro(ex);
        //        base.ExibirPainelExcecao("Redecard.PN.SharePoint", 300);
        //    }
        }
    }
}
