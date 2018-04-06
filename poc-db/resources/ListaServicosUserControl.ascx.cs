/*
 © Copyright 2017 Rede S.A.
   Autor : Rodrigo Coelho - rodrigo.oliveira@iteris.com.br
   Empresa : Iteris
 */

using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Linq;
using Rede.PN.ZeroDolar.Negocio;
using Rede.PN.ZeroDolar.Modelo;


namespace Rede.PN.ZeroDolar.SharePoint.WebParts.ListaServicos {
    /// <summary>
    /// WebPart para exibição de serviços disponíveis para contratação e cancelamento
    /// </summary>
    public partial class ListaServicosUserControl : Redecard.PN.Comum.UserControlBase {

        /// <summary>
        /// Instancia da classe de negócio
        /// </summary>
        private NegocioServicosDisponiveis objNegocioServicos = new NegocioServicosDisponiveis();

        /// <summary>
        /// Caminho para o arquivo de condicoes em html
        /// </summary>
        public string TermosCondicoesHtml { get; set; }

        /// <summary>
        /// Caminho para arquivo de condicoes em pdf
        /// </summary>
        public string TermosCondicoesPdf { get; set; }
        

        /// <summary>
        /// Load da Página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e) {

            using (var log = Logger.IniciarLog("CarregarListaServicosDisponiveis - Page_Load")) {
                try {

                    hdfTermosCondicoesHtml.Value = this.TermosCondicoesHtml;
                    lnkTermosCondicoesPdf.NavigateUrl = this.TermosCondicoesPdf;
                    btnImprimirTermo.NavigateUrl = this.TermosCondicoesPdf;

                    if (!Page.IsPostBack) {
                        CarregarListaServicosDisponiveis();
                    }
                } catch (PortalRedecardException ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                } catch (Exception ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento disparado para cada um dos check box de filtro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkFiltro_CheckedChanged(object sender, EventArgs e) {

            using (var log = Logger.IniciarLog("Filtro dos serviços exibidos - chkFiltro_CheckedChanged")) {
                try {

                    CarregarListaServicosDisponiveis();
                } catch (PortalRedecardException ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                } catch (Exception ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

        }

        private List<ServicoDisponivel> AplicarFiltros(List<ServicoDisponivel> listaServicos) {
            List<ServicoDisponivel> objListaFiltrados = new List<ServicoDisponivel>();

            if (listaServicos != null && listaServicos.Count > 0) {

                if (chkAtivados.Checked)
                    objListaFiltrados.AddRange(listaServicos.FindAll(obj => obj.Situacao.Equals('A') || obj.Situacao.Equals('R')));

                if (chkDesativados.Checked)
                    objListaFiltrados.AddRange(listaServicos.FindAll(obj => obj.Situacao.Equals('C')));

                if (!chkAtivados.Checked && !chkDesativados.Checked)
                    objListaFiltrados.AddRange(listaServicos);
            }            

            return objListaFiltrados;
        }


        /// <summary>
        /// Evento de confirmação da contratação de serviço
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnConfirmarContratacao_Click(object sender, EventArgs e) {
            using (var log = Logger.IniciarLog("Contratação de serviço - btnConfirmarContratacao_Click")) {
                try {
                    if (cbxAceite.Checked) {
                        cbxAceite.Checked = false;

                        ContratarServico();
                        CarregarListaServicosDisponiveis();

                        ExibirMensagemSucesso("Serviço contratado com sucesso!");
                    }
                } catch (PortalRedecardException ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                } catch (Exception ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

        }

        /// <summary>
        /// Realiza a contratação do serviço
        /// </summary>
        private void ContratarServico() {
            int intCodigoServico = Convert.ToInt32(hdfIdServicoContratar.Value);
            char chrSituacao = Convert.ToChar(hdfSituacaoServico.Value);

            objNegocioServicos.ContratarServico(
                SessaoAtual.CodigoEntidade,
                intCodigoServico,
                SessaoAtual.CodigoCanal,
                SessaoAtual.CodigoCelula,
                chrSituacao,
                SessaoAtual.NomeUsuario);
        }


        /// <summary>
        /// Evento para confirmação de cancelamento de serviço
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnConfirmaCancelamento_Click(object sender, EventArgs e) {
            using (var log = Logger.IniciarLog("Cancelamento de serviço - btnConfirmaCancelamento_Click")) {
                try {

                    CancelarServico();
                    CarregarListaServicosDisponiveis();

                    ExibirMensagemSucesso("Serviço cancelado com sucesso!");

                } catch (PortalRedecardException ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                } catch (Exception ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Exibe mensagem de sucesso
        /// </summary>
        /// <param name="mensagem"></param>
        private void ExibirMensagemSucesso(string mensagem) {
            string strScript = "ExecuteOrDelayUntilScriptLoaded(function () { window.abrirMensagemSucesso('" + mensagem + "') }, 'SP.UI.Dialog.js');";
            Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "Sucesso", strScript, true);
        }

        /// <summary>
        /// Realiza o cancelamento do serviço
        /// </summary>
        private void CancelarServico() {
            int intCodigoServico = Convert.ToInt32(hdfIdServicoCancelar.Value);
            objNegocioServicos.CancelarServico(SessaoAtual.CodigoEntidade, intCodigoServico, SessaoAtual.NomeUsuario);
        }

        /// <summary>
        /// Evento para item da lista de serviços
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void rptServicosDisponiveis_ItemDataBound(object sender, RepeaterItemEventArgs e) {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {

                var objServico = (e.Item.DataItem as ServicoDisponivel);

                ConfigurarEventoClick(e);

                ExibirValoresServico(e, objServico);
            }
        }

        /// <summary>
        /// Carrega os serviços disponíveis para o pv 
        /// </summary>
        private void CarregarListaServicosDisponiveis() {
            List<ServicoDisponivel> objLista = objNegocioServicos.ListarServicosDisponiveis(SessaoAtual.CodigoEntidade);

            AtualizarListaServicos(objLista);
        }

        /// <summary>
        /// Atualiza o grid da tela com a lista de serviço
        /// </summary>
        /// <param name="lista"></param>
        private void AtualizarListaServicos(List<ServicoDisponivel> listaServicos) {

            var listaFiltrada = AplicarFiltros(listaServicos);

            rptServicosDisponiveis.DataSource = listaFiltrada;
            rptServicosDisponiveis.DataBind();

            rptServicosDisponiveis.Visible = listaFiltrada.Count > 0;
            lblListaVazia.Visible = listaFiltrada.Count == 0;

        }

        /// <summary>
        /// Exibe as descrições do serviço no grid
        /// </summary>
        /// <param name="e"></param>
        /// <param name="servicoDisponivel"></param>
        private void ExibirValoresServico(RepeaterItemEventArgs e, ServicoDisponivel servicoDisponivel) {
            Label lblNomeServico = e.Item.FindControl("lblNomeServico") as Label;
            Label lblSituacao = e.Item.FindControl("lblSituacao") as Label;

            Button btnContratarServico = e.Item.FindControl("btnContratarServico") as Button;
            Button btnCancelarServico = e.Item.FindControl("btnCancelarServico") as Button;

            Label lblDetalhesInfoServico = e.Item.FindControl("lblDetalhesInfoServico") as Label;            

            Label lblContratadoEm = e.Item.FindControl("lblContratadoEm") as Label;
            HyperLink btnConsultarTermosUso = e.Item.FindControl("btnConsultarTermosUso") as HyperLink;

            btnConsultarTermosUso.Attributes.Add("onclick", "window.abrirTermosECondicoes(); return false;");

            btnCancelarServico.OnClientClick = string.Format("window.confirmarCancelamento({0}, 'Cancelamento do serviço: {1}'); return false;", servicoDisponivel.CodigoServico, servicoDisponivel.Nome);
            btnContratarServico.OnClientClick = string.Format("window.confirmarContratacao({0}, '{1}'); return false", servicoDisponivel.CodigoServico, servicoDisponivel.Situacao);

            lblNomeServico.Text = servicoDisponivel.Nome;
            lblSituacao.Text = servicoDisponivel.ObterDescricaoSituacao();

            lblContratadoEm.Text = servicoDisponivel.DataContratacao.HasValue ? servicoDisponivel.DataContratacao.Value.ToString("dd/MM/yyyy") : "--";

            btnContratarServico.Visible = servicoDisponivel.Situacao == 'D' || servicoDisponivel.Situacao == 'C';
            btnCancelarServico.Visible = servicoDisponivel.Situacao == 'A' || servicoDisponivel.Situacao == 'R';
        }

        /// <summary>
        /// Configura o evento click em cada linha para abrir e fechar os detalhes 
        /// </summary>
        /// <param name="e"></param>
        private void ConfigurarEventoClick(RepeaterItemEventArgs e) {
            HtmlTableRow trDetalhes = e.Item.FindControl("trDetalhes") as HtmlTableRow;
            HtmlTableRow trServico = e.Item.FindControl("trServico") as HtmlTableRow;

            HyperLink btnExibirDetalhes = e.Item.FindControl("btnExibirDetalhes") as HyperLink;

            trServico.Attributes.Add("onclick", string.Format("OpenDetalhes(this, '{0}', '{1}');", btnExibirDetalhes.ClientID, trDetalhes.ClientID));
        }




    }

}
