/*
© Copyright 2013 Rede S.A.
Autor : Alexandre Osada
Empresa : Iteris Consultoria e Software
Histórico: [08/08/2016] Victor Alencar: Alteração Pesquisa Satisfação 2016
 *         [19/11/2014] Alexandre Osada: Criação do documento
 *         [11/11/2014] Alexandre Shiroma: Alteração Pesquisa Satisfação 2014
*/

using System;
using System.Web.UI;
using Microsoft.SharePoint;
using Redecard.PN.Comum;
using System.Text.RegularExpressions;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.PesquisaSatisfacao.Sharepoint.WebParts.PesquisaSatisfacao
{
    /// <summary>
    /// UserControl de Pesquisa de Satisfação
    /// </summary>
    public partial class PesquisaSatisfacaoUserControl : UserControlBase
    {

        public Boolean EstabelecimentoVarejo()
        {
            Boolean isVarejo = false;
            if (Sessao.Contem())
            {
                Char codigoSegmento = SessaoAtual.CodigoSegmento;
#if DEBUG
                if (!String.IsNullOrEmpty(Request["Segmento"]))
                    codigoSegmento = Request["Segmento"][0];
#endif
                switch (codigoSegmento)
                {
                    case 'V':
                    case 'v':
                        isVarejo = true;
                        break;
                    default:
                        isVarejo = false;
                        break;
                }
            }
            return isVarejo;
        }

        /// <summary>
        /// OnInit
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            //Desabilita a permissão da WebPart
            this.ValidarPermissao = false;

            base.OnInit(e);
        }
        
        /// <summary>
        /// Load da página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // verifica se a página está em modo edição
            if (SPContext.Current.FormContext.FormMode == SPControlMode.Edit)
                return;

            try
            {
                if (!Page.IsPostBack)
                {
                    this.Visible = false;

                    // só exibe lightbox se:
                    if (Sessao.Contem()                     // 1. usuário estiver autenticado (possuir sessão)
                        && !SessaoAtual.UsuarioAtendimento  // 2. não for central de atendimento
                        && !SessaoAtual.Legado              // 3. usuário migrado/novo acesso
                        && SessaoAtual.GrupoEntidade == 1)  // 4. usuário estabelecimento
                    {
                        SPWeb web = SPContext.Current.Web;
                        SPList lista = null;
                        if (EstabelecimentoVarejo())
                        {
                            lista = web.Lists.TryGetList("Pesquisa de Satisfação - Varejo");
                        }
                        else
                        {
                            lista = web.Lists.TryGetList("Pesquisa de Satisfação - IBBA");
                        }

                        // recupera a lista de pesquisa de satisfação
                        if (lista != null)
                        {
                            SPQuery query = new SPQuery();
                            query.Query = String.Concat(
                                "<Where><Eq><FieldRef Name='CodigoIdUsuario'/><Value Type='Double'>",
                                this.SessaoAtual.CodigoIdUsuario,
                                "</Value></Eq></Where>");

                            // se usuário ainda não respondeu, exibe formulário da pesquisa
                            SPListItemCollection items = lista.GetItems(query);
                            if (items.Count == 0 || items[0]["Não Quero Responder"] == null)
                            {
                                this.Visible = true;
                                if (EstabelecimentoVarejo())
                                {
                                    pnlPesquisaVarejo.Visible = true;
                                }
                                else
                                {
                                    pnlPesquisaIbba.Visible = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Logger.GravarErro("Pesquisa de Satisfação - Page Load - ArgumentException", ex);
                SharePointUlsLog.LogErro(ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Pesquisa de Satisfação - Page Load - Exception", ex);
                SharePointUlsLog.LogErro(ex);
            }
        }

        /// <summary>
        /// Evento do botão "Enviar"
        /// </summary>
        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate();
                if (Page.IsValid)
                {
                    SPWeb web = SPContext.Current.Web;
                    SPList lista = null;
                    if (EstabelecimentoVarejo())
                    {
                        lista = web.Lists.TryGetList("Pesquisa de Satisfação - Varejo");
                        if (lista != null)
                        {
                            SPListItem item = lista.Items.Add();

                            String resposta = String.Join("#", new[] {
                                "Atendimento Geral;", sqAtendimentoGeral.SelectedValue,
                                "Canais Digitais;", sqCanaisDigitais.SelectedValue,
                                "Indicação;", sqIndicacao.SelectedValue,
                            String.Empty });

                            item["Pesquisa de Satisfação - Varejo"] = resposta;
                            item["Comentários"] = txtComentarios.Text;
                            item["Não Quero Responder"] = "Falso";
                            item["CodigoIdUsuario"] = this.SessaoAtual.CodigoIdUsuario;
                            item.Update();

                            String script = String.Format("HidePesquisa(true);");
                            this.Page.ClientScript.RegisterStartupScript(this.GetType(), String.Format("Key_{0}", this.ClientID), script, true);
                        }
                    }
                    else
                    {
                        lista = web.Lists.TryGetList("Pesquisa de Satisfação - IBBA");
                        if (lista != null)
                        {
                            SPListItem item = lista.Items.Add();

                            String resposta = String.Join("#", new[] {
                                "Atendimento Geral;", sqAtendimentoGeralIbba.SelectedValue,
                                "Canais Digitais;", sqCanaisDigitaisIbba.SelectedValue,
                                "Executivo;", sqExecutivoIbba.SelectedValue,
                                "Indicação;", sqIndicacaoIbba.SelectedValue,
                            String.Empty });

                            item["Pesquisa de Satisfação - IBBA"] = resposta;
                            item["Comentários"] = txtComentariosIbba.Text;
                            item["Não Quero Responder"] = "Falso";
                            item["CodigoIdUsuario"] = this.SessaoAtual.CodigoIdUsuario;
                            item.Update();

                            String script = String.Format("HidePesquisa(true);");
                            this.Page.ClientScript.RegisterStartupScript(this.GetType(), String.Format("Key_{0}",this.ClientID), script, true);
                        }
                    }

                }
            }
            catch (ArgumentException ex)
            {
                Logger.GravarErro("Pesquisa de Satisfação - ArgumentException - Enviar", ex);
                SharePointUlsLog.LogErro(ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Pesquisa de Satisfação - Exception - Enviar", ex);
                SharePointUlsLog.LogErro(ex);
            }
        }

        ///// <summary>
        ///// Evento do botão "Não Responder"
        ///// </summary>
        //protected void btnNaoResponder_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        SPWeb web = SPContext.Current.Web;
        //        if (EstabelecimentoVarejo())
        //        {
        //            SPList lista = web.Lists.TryGetList("Pesquisa de Satisfação - Varejo");

        //            if (lista != null)
        //            {
        //                SPListItem item = lista.Items.Add();

        //                item["Não Quero Responder"] = "Verdadeiro";
        //                item["CodigoIdUsuario"] = this.SessaoAtual.CodigoIdUsuario;

        //                item.Update();

        //                String script = String.Format("HidePesquisa(false);");
        //                this.Page.ClientScript.RegisterStartupScript(this.GetType(), String.Format("Key_{0}", this.ClientID), script, true);
        //            }
        //        }
        //        else
        //        {
        //            SPList lista = web.Lists.TryGetList("Pesquisa de Satisfação - IBBA");

        //            if (lista != null)
        //            {
        //                SPListItem item = lista.Items.Add();

        //                item["Não Quero Responder"] = "Verdadeiro";
        //                item["CodigoIdUsuario"] = this.SessaoAtual.CodigoIdUsuario;

        //                item.Update();

        //                String script = String.Format("HidePesquisa(false);");
        //                this.Page.ClientScript.RegisterStartupScript(this.GetType(), String.Format("Key_{0}", this.ClientID), script, true);
        //            }
        //        }

        //    }
        //    catch (ArgumentException ex)
        //    {
        //        Logger.GravarErro("Pesquisa de Satisfação - ArgumentException - Enviar", ex);
        //        SharePointUlsLog.LogErro(ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.GravarErro("Pesquisa de Satisfação - Exception - Enviar", ex);
        //        SharePointUlsLog.LogErro(ex);
        //    }
        //}
    }
}