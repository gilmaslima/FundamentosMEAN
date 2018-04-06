/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/

using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using System.Web;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.OutrosServicos.SharePoint.OfertaServico;
using System.Web.UI.WebControls;
using System.Web.UI;
using Redecard.PN.Outro.Core.Web.Controles.Portal;

namespace Redecard.PN.OutrosServicos.SharePoint.ControlTemplates.OutrosServicos.Mdr
{
    public partial class SelecionarOferta : UserControlBase
    {
        #region [Eventos da WebPart]

        /// <summary>
        /// Carregamento da Webpart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.CarregarOfertas();
            }
        }

        /// <summary>
        /// Preenche as linhas dentro do repeater
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void repOfertas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger log = Logger.IniciarNovoLog("Preencher as linhas dentro do repeater"))
            {
                Oferta oferta = default(Oferta);
                try
                {
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        if (!Object.ReferenceEquals(e.Item.DataItem, null))
                        {
                            oferta = (Oferta)e.Item.DataItem;
                            Literal ltrNomeOferta = e.Item.FindControl("ltrNomeOferta") as Literal;
                            Literal ltrStatus = e.Item.FindControl("ltrStatus") as Literal;
                            Literal ltrDataContratacao = e.Item.FindControl("ltrDataContratacao") as Literal;
                            HyperLink hlkDetalhes = e.Item.FindControl("hlkDetalhes") as HyperLink;
                            Literal ltrNomeOfertaTooltip = e.Item.FindControl("ltrNomeOfertaTooltip") as Literal;
                            Literal ltrTituloOfertaTooltip = e.Item.FindControl("ltrTituloOfertaTooltip") as Literal;
                            HiddenField hdnOfertaCancelada = e.Item.FindControl("hdnOfertaCancelada") as HiddenField;


                            ltrStatus.Text = oferta.Status.GetDescription().ToLowerInvariant();

                            hdnOfertaCancelada.Value = oferta.Status == StatusOferta.Cancelado ? "1" : "0";

                            ltrDataContratacao.Text = (oferta.DataContratacao.HasValue) ?
                                oferta.DataContratacao.Value.ToString("dd/MM/yyyy") : "-";

                            ltrNomeOfertaTooltip.Text = oferta.NomeOferta;

                            if (oferta.TipoOferta.ToUpper().Equals("MDR"))
                                ltrNomeOferta.Text = String.Format("{0} ({1})",
                                                                oferta.NomeOferta,
                                                                oferta.CodigoOferta.ToString());
                            else
                                ltrNomeOferta.Text = oferta.NomeOferta;

                            if (oferta.TipoOferta.ToUpper().Equals("MAQ"))
                            {
                                ltrTituloOfertaTooltip.Text = "Conta Certa Itaú";
                                ltrNomeOfertaTooltip.Text = "Oferta Itaú e Rede você pode contar com benefícios no Conta Certa Itaú e no terminal da Rede.";
                            }
                            else
                                ltrTituloOfertaTooltip.Text = "Oferta";

                            String urlOferta = this.ObterUrlOferta(oferta);
                            hlkDetalhes.NavigateUrl = urlOferta;
                            if (
                                (oferta.Status != StatusOferta.Contratado && (oferta.TipoOferta.ToUpper().Equals("BON") || oferta.TipoOferta.ToUpper().Equals("PRU")))
                                || String.IsNullOrWhiteSpace(urlOferta)
                                || this.SessaoAtual.StatusPVCancelado() //se o PV esta cancelado nao pode detalhar a oferta - pois o link esta abaixo de servicos
                                )
                            {
                                hlkDetalhes.Text = "-";
                                hlkDetalhes.Enabled = false;
                            }
                        }
                    }
                }
                catch (FaultException<NkPlanoContasServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        #endregion

        #region [Métodos da webpart]

        /// <summary>
        /// Obter a URL da Oferta de acordo com o Tipo de Oferta retornado pelo UK
        /// </summary>
        /// <param name="oferta">Oferta carregando</param>
        /// <returns></returns>
        private String ObterUrlOferta(Oferta oferta)
        {
            String url = String.Format("{0}/{1}", base.RecuperarEnderecoPortalFechado(), "servicos/Paginas/pn_ConsultarOfertas.aspx");

            QueryStringSegura qrOfertaTaxa = new QueryStringSegura();
            qrOfertaTaxa.Add("CodigoOferta", oferta.CodigoOferta.ToString());
            qrOfertaTaxa.Add("CodigoProposta", oferta.CodigoProposta.ToString());

            switch (oferta.TipoOferta)
            {
                case "MDR": // MDR Regressivo
                    url = String.Format("{0}/{1}?q={2}",
                                        base.RecuperarEnderecoPortalFechado(),
                                        "servicos/Paginas/DetalheOfertaTaxa.aspx",
                                        qrOfertaTaxa.ToString());
                    break;
                // AGA: Desabilitar link Maquininha Conta Certa
                case "MAQ": // Maquininha Conta Certa
                    QueryStringSegura qsOfertaContaCerta = new QueryStringSegura();
                    qsOfertaContaCerta.Add("codSitContrato", ((Int16)oferta.Status).ToString());
                    qsOfertaContaCerta.Add("dataFimVigencia", oferta.PeriodoFinalVigencia.ToString());
                    
                    // nova página Maquininha Conta Certa
                    url = String.Format("{0}/{1}?q={2}", base.RecuperarEnderecoPortalFechado(), "servicos/Paginas/pn_MaquininhaContaCerta.aspx", qsOfertaContaCerta.ToString());
                    // url = String.Format("{0}/{1}?q={2}", base.RecuperarEnderecoPortalFechado(), "servicos/Paginas/DetalheOfertaContaCerta.aspx", qsOfertaContaCerta.ToString());

                    break;
                case "BON": // Bônus Celular
                    url = String.Format("{0}/{1}", base.RecuperarEnderecoPortalFechado(), "servicos/Paginas/ConsultarOfertasBonus.aspx");
                    break;
                case "PRU": // Preço Único
                    url = String.Format("{0}/{1}", base.RecuperarEnderecoPortalFechado(), "servicos/Paginas/ConsultarOfertasPrecoUnico.aspx");
                    break;
                default:
                    url = "";
                    break;
            }

            return url;
        }

        #endregion

        #region [Consultas WCF]
        /// <summary>
        /// Carregar as Ofertas no Grid
        /// </summary>
        private void CarregarOfertas()
        {
            using (Logger log = Logger.IniciarLog("Consulta as Ofertas e carrega no grid"))
            {
                if (Sessao.Contem())
                {
                    Int32 numeroPv = SessaoAtual.CodigoEntidade;

                    //if (SessaoAtual.AcessoFilial)
                    //    numeroPv = SessaoAtual.CodigoEntidadeMatriz;
                    //else if (SessaoAtual.CodigoMatriz > 0)
                    //    numeroPv = SessaoAtual.CodigoMatriz;

                    var ofertas = default(List<Oferta>);

                    try
                    {
                        using (var ctx = new ContextoWCF<ServicoOfertaClient>())
                        {
                            ofertas = ctx.Cliente.ConsultarOfertas(numeroPv);
                        }

                        //Se oferta inexistente, exibe mensagem customizada
                        if (ofertas != null && ofertas.Count > 0)
                        {
                            qavSemDados.Visible = false;
                            hdnContemRegistrosOfertas.Value = "1";
                            pnlOfertas.Visible = true;
                            repOfertas.DataSource = ofertas;
                            repOfertas.DataBind();
                        }
                        else
                        {
                            qavSemDados.Visible = true;
                            hdnContemRegistrosOfertas.Value = "0";
                            qavSemDados.Mensagem = "Nenhuma oferta contratada.";
                            qavSemDados.TipoQuadro = TipoQuadroAviso.Aviso;
                            pnlOfertas.Visible = false;
                        }

                    }
                    catch (FaultException<OfertaServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                }
            }
        }

        #endregion
    }
}