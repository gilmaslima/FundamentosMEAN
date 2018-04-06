using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using System.ServiceModel;
using System.Collections.Generic;

namespace Redecard.PN.OutrosServicos.SharePoint.WebParts.ConsultarStatusSolicitacao
{
    public partial class ConsultarStatusSolicitacaoUserControl : UserControlBase
    {
        /// <summary>
        /// Inicialização da tela 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    PreencherOcorrencias();
                }
            }
            catch (PortalRedecardException ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Preenche o dropdownlist com os occorências
        /// </summary>
        private void PreencherOcorrencias()
        {
            using (Logger log = Logger.IniciarLog("Preenchendo ocorrências"))
            {
                try
                {
                    using (var solicitacaoServico = new SolicitacaoServico.SolicitacaoServicoClient())
                    {
                        SolicitacaoServico.Ocorrencia[] listaOcorrencias;
                        listaOcorrencias = solicitacaoServico.ConsultarOcorrencias();

                        if (listaOcorrencias.Length > 0)
                        {
                            ddlOcorrencia.DataSource = listaOcorrencias;

                            ddlOcorrencia.DataTextField = "NomeOcorrencia";
                            ddlOcorrencia.DataValueField = "CodigoTipoOcorrencia";

                            ddlOcorrencia.DataBind();
                        }
                    }
                }
                catch (FaultException<SolicitacaoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Habilitar a tela para inserir parâmetros de consulta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltarConsulta_Click(object sender, EventArgs e)
        {
            try
            {
                pnlConsultaSolicitacao.Visible = !(pnlResultado.Visible = false);
                pnlVoltar.Visible = false;
            }
            catch (PortalRedecardException ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Realizar pesquisa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnConsultar_Click(object sender, EventArgs e)
        {
            CarregarConsulta();
        }

        /// <summary>
        /// Realiza a busca e carrega a tela de acordo com os parâmetros da tela
        /// </summary>
        private void CarregarConsulta()
        {
            using (Logger log = Logger.IniciarLog("Carregando consulta"))
            {
                try
                {
                    using (var solicitacaoServico = new SolicitacaoServico.SolicitacaoServicoClient())
                    {
                        SolicitacaoServico.Solicitacao[] listaSolicitacoes = null;

                        Int32 numeroSolicitacao = 0;

                        if (!String.IsNullOrEmpty(txtNumeroSolicitacao.Text))
                        {
                            if (!Int32.TryParse(txtNumeroSolicitacao.Text, out numeroSolicitacao))
                                numeroSolicitacao = 0;
                        }

                        DateTime dtInicio = DateTime.MinValue;
                        DateTime dtTermino = DateTime.MinValue;
                        if (!String.IsNullOrEmpty(txtDataInicio.Text) && !String.IsNullOrEmpty(txtDataTermino.Text))
                        {
                            dtInicio = txtDataInicio.Text.ToDate();
                            dtTermino = txtDataTermino.Text.ToDate();
                        }

                        listaSolicitacoes = solicitacaoServico.Consultar(numeroSolicitacao, dtInicio, dtTermino,
                                ddlOcorrencia.SelectedValue, SessaoAtual.CodigoEntidade);

                        pnlConsultaSolicitacao.Visible = !(pnlResultado.Visible = true);

                        pnlSemSolicitacoes.Visible = !(rptResultados.Visible = (listaSolicitacoes.Length > 0));
                        pnlVoltar.Visible = !(pnlSemSolicitacoes.Visible);
                        rptResultados.DataSource = listaSolicitacoes;
                        rptResultados.DataBind();
                        if (listaSolicitacoes.Length > 0)
                            CarregarPaginacao();
                    }
                }
                catch (FaultException<SolicitacaoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Montar a mensagem de detalhamento da Solicitação
        /// </summary>
        /// <param name="numeroSolicitacao">Número da Solicitação</param>
        /// <param name="descricaoEnvioCarta">Status de envio de carta</param>
        /// <param name="situacaoSolicitacao">Situação da solicitação</param>
        /// <param name="usuarioMonitoria">Usuário responsável pela Monitoria</param>
        /// <param name="canalResposta">Canal de resposta</param>
        /// <returns>Mensagem de detalhamento</returns>
        private String MensagemDetalhe(Int32 numeroSolicitacao, String descricaoEnvioCarta, String situacaoSolicitacao, String usuarioMonitoria, String canalResposta)
        {
            using (Logger log = Logger.IniciarLog("Montando mensagem de detalhamento da solicitação"))
            {
                try
                {
                    String mensagem = "";

                    if (situacaoSolicitacao.Equals("Encerrada") && !canalResposta.Equals("Z1")
                            && (!descricaoEnvioCarta.Equals("F") && !descricaoEnvioCarta.Equals("C") && !descricaoEnvioCarta.Equals("E")))
                        mensagem = String.Format("Solicitação {0} respondida pela REDECARD. Em caso de dúvidas, entre em contato com Central de Atendimento.", numeroSolicitacao);
                    else if (situacaoSolicitacao.Equals("Encerrada") && !canalResposta.Equals("Z1")
                            && (!descricaoEnvioCarta.Equals("F") && !descricaoEnvioCarta.Equals("C") && !descricaoEnvioCarta.Equals("E")))
                        mensagem = "Ocorrência em análise pela REDECARD. Por favor, aguarde.";
                    else
                        mensagem = "";

                    return mensagem;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return "Erro";
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return "Erro";
                }
            }
        }

        /// <summary>
        /// Voltar a tela para exibição dos resultados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltarResultados_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Voltando para tela de exibição dos resultados"))
            {
                try
                {
                    CarregarConsulta();
                    pnlVoltar.Visible = pnlResultado.Visible = !(pnlDetalheSolicitacao.Visible = false);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Preenche as solicitações encontradas no repeater
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptResultados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Bind das solicitações no repeater"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
                    {
                        Label lblSolicitacao = (Label)e.Item.FindControl("lblSolicitacao");
                        Label lblTipoOcorrencia = (Label)e.Item.FindControl("lblTipoOcorrencia");
                        Label lblNumeroEntidade = (Label)e.Item.FindControl("lblNumeroEntidade");
                        Label lblAbertura = (Label)e.Item.FindControl("lblAbertura");
                        Label lblSolucao = (Label)e.Item.FindControl("lblSolucao");
                        Label lblSituacao = (Label)e.Item.FindControl("lblSituacao");
                        Label lblCanalResponsavel = (Label)e.Item.FindControl("lblCanalResponsavel");
                        LinkButton lnkDetalhe = (LinkButton)e.Item.FindControl("lnkDetalhe");

                        SolicitacaoServico.Solicitacao solicitacao = (SolicitacaoServico.Solicitacao)e.Item.DataItem;
                        if (solicitacao != null)
                        {
                            if (!String.IsNullOrEmpty(solicitacao.SituacaoSolicitacao))
                            {
                                lblSituacao.Text = SituacaoSolicitacao(solicitacao.SituacaoSolicitacao, solicitacao.UsuarioMonitoria);
                            }

                            lblSolicitacao.Text = solicitacao.NumeroSolicitacao.ToString();
                            lblTipoOcorrencia.Text = solicitacao.TipoOcorrencia.ToString();
                            lblNumeroEntidade.Text = solicitacao.CodigoEntidade.ToString();
                            lblAbertura.Text = solicitacao.DataSolicitacao.ToShortDateString();
                            lblSolucao.Text = solicitacao.DataEncerramento.Date == new DateTime(1900, 1, 1).Date ? "" : solicitacao.DataEncerramento.ToShortDateString();
                            lblCanalResponsavel.Text = solicitacao.CanalResposta;

                            String descricaoEnvioCarta = "";

                            switch (solicitacao.EnvioCarta)
                            {
                                case 99:
                                    descricaoEnvioCarta = "--";
                                    break;
                                case 1:
                                    descricaoEnvioCarta = "NÃO";
                                    break;
                                case 0:
                                    descricaoEnvioCarta = "P";
                                    break;
                                case 2:
                                    descricaoEnvioCarta = "F";
                                    break;
                                case 3:
                                    descricaoEnvioCarta = "E";
                                    break;
                                case 4:
                                    descricaoEnvioCarta = "C";
                                    break;
                                default:
                                    descricaoEnvioCarta = "--";
                                    break;
                            }

                            String situacao = SituacaoSolicitacao(solicitacao.SituacaoSolicitacao, solicitacao.UsuarioMonitoria);
                            String textoMensagem = MensagemDetalhe(solicitacao.NumeroSolicitacao, descricaoEnvioCarta, situacao, solicitacao.UsuarioMonitoria, solicitacao.CanalResposta);

                            //lnkDetalhe.CommandArgument = solicitacao.NumeroSolicitacao.ToString();
                            lnkDetalhe.CommandArgument = textoMensagem;

                            //showDet('<%=strNumSol%>','<%=strCartaColuna%>','<%=strSitSol%>','<%=strRazaoSocial%>','<%=strMatrAbert%>','<%=strCanalResp%>')"
                        }
                    }
                }
                catch (FaultException<SolicitacaoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Exibe as solicitações encontradas
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void rptResultados_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Comando de uma solicitação no repeater"))
            {
                try
                {
                    if (e.CommandName.Equals("DetalheSolicitacao"))
                    {
                        LinkButton lnkDetalhe = (LinkButton)e.Item.FindControl("lnkDetalhe");

                        Label lblSolicitacao = (Label)e.Item.FindControl("lblSolicitacao");
                        Label lblCanalResponsavel = (Label)e.Item.FindControl("lblCanalResponsavel");

                        if (!String.IsNullOrEmpty(lnkDetalhe.CommandArgument))
                        {
                            CarregarResposta(lblSolicitacao.Text, lnkDetalhe.CommandArgument);
                            pnlVoltar.Visible = pnlResultado.Visible = !(pnlResposta.Visible = true);
                        }
                        else
                        {
                            CarregarDetalhesSolicitacao(lblSolicitacao.Text.Trim().ToInt32(), lblCanalResponsavel.Text);
                            pnlVoltar.Visible = pnlResultado.Visible = !(pnlDetalheSolicitacao.Visible = true);
                        }
                    }
                }
                catch (FaultException<SolicitacaoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        private void CarregarResposta(String numeroSolicitacao, String textoMensagem)
        {
            try
            {
                lblNumeroSolicitacao.Text = numeroSolicitacao;
                lblRespostaSolicitacao.Text = textoMensagem;
            }
            catch (PortalRedecardException ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Carrega as informações de detalhe da solicitação
        /// </summary>
        /// <param name="numeroSolicitacao"></param>
        private void CarregarDetalhesSolicitacao(Int32 numeroSolicitacao, String canalResponsavel)
        {
            using (Logger log = Logger.IniciarLog("Carregando informações de detalhe da solicitação"))
            {
                try
                {
                    using (var solicitacaoServico = new SolicitacaoServico.SolicitacaoServicoClient())
                    {
                        //Código do Caso fixo em 0
                        SolicitacaoServico.Solicitacao[] solicitacoes = solicitacaoServico.ConsultarOcorrenciasSolicitacao(numeroSolicitacao, 0);
                        SolicitacaoServico.Solicitacao solicitacao = null;
                        if (solicitacoes.Length > 0)
                        {
                            solicitacao = solicitacoes[0];

                            LimparCamposDetalhamento();

                            //Carregar Descrição do Caso. Código do Caso fixo em 1 nessa consulta
                            txtDescricaoCaso.Text = solicitacaoServico.ConsultarDescricaoCaso(numeroSolicitacao, 1);
                            lblCodigoEntidade.Text = String.Format("{0} - {1}", SessaoAtual.CodigoEntidade, SessaoAtual.NomeEntidade);
                            lblSolicitacao.Text = numeroSolicitacao.ToString();

                            lblSituacaoSolicitacao.Text = SituacaoSolicitacao(solicitacao.SituacaoSolicitacao, solicitacao.UsuarioMonitoria);
                            lblOcorrenciaSolicitacao.Text = solicitacao.NomeOcorrencia;

                            if (solicitacao.SituacaoSolicitacao.Equals("P"))
                                lblDataSolucao.Text = "";
                            else
                                lblDataSolucao.Text = solicitacao.DataEncerramento.ToShortDateString();

                            btnConsultarResposta.Visible = !(btnSolicitacaoRespondida.Visible = ((lblSituacaoSolicitacao.Text.Equals("Encerrada") && canalResponsavel.Equals("Z1"))
                                                                                                    || ((lblSituacaoSolicitacao.Text.Equals("Pendente") || lblSituacaoSolicitacao.Text.Equals("Encerrada")) && canalResponsavel.Equals("Z1"))));

                            PreencherParametrosCarta();

                            //Código do caso fixo em 1 para consulta de pré-requisitos
                            SolicitacaoServico.PreRequisitoSolicitacao[] preRequisitos = solicitacaoServico.ConsultarPreRequisitosSolicitacao(numeroSolicitacao, 1);
                            if (preRequisitos.Length > 0)
                            {
                                rptPreRequisitos.DataSource = preRequisitos;
                                rptPreRequisitos.DataBind();
                                rptPreRequisitos.Visible = !(pnlSemPreRequisitos.Visible = false);
                            }
                            else
                            {
                                rptPreRequisitos.Visible = !(pnlSemPreRequisitos.Visible = true);
                                ((QuadroAviso)qdSemRequisitos).ClasseImagem = "icone-aviso";
                                ((QuadroAviso)qdSemRequisitos).CarregarMensagem("Aviso", "Não foram encontrados pré-requisitos para a solicitação selecionada!");
                            }
                        }
                    }
                }
                catch (FaultException<SolicitacaoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (FaultException<WMOutrosServicos.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Limpa os campos de Pré-requisitos da Solicitação
        /// </summary>
        private void LimparCamposDetalhamento()
        {
            try
            {
                lblSolicitacao.Text = String.Empty;
                lblSituacaoSolicitacao.Text = String.Empty;
                lblDataSolucao.Text = String.Empty;
                lblCodigoEntidade.Text = String.Empty;
                lblOcorrenciaSolicitacao.Text = String.Empty;
                txtDescricaoCaso.Text = String.Empty;
                rptPreRequisitos.Visible = false;
            }
            catch (PortalRedecardException ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Descrição de situação da Solicitação
        /// </summary>
        /// <param name="situacao">Código do Tipo de Situação</param>
        /// <param name="usuario">Usuário resposável pela monitoria</param>
        /// <returns>Retorna a descrição da Situação</returns>
        private String SituacaoSolicitacao(String situacao, String usuario)
        {
            try
            {
                String descricao = "";
                switch (situacao)
                {
                    case "E":
                        descricao = "Encerrada";
                        break;
                    case "P":
                        if (usuario.Equals("ISUSER"))
                            descricao = "Pendente";
                        else
                            descricao = "Em análise";
                        break;
                    case "C":
                        descricao = "Cancelada";
                        break;
                    default:
                        descricao = "Não identificado";
                        break;
                }

                return descricao;
            }
            catch (PortalRedecardException ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                return "Erro";
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                return "Erro";
            }
        }

        /// <summary>
        /// Exibe os detalhes da solicitação selecionada 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptPreRequisitos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Bind dos pré-requisitos"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        SolicitacaoServico.PreRequisitoSolicitacao preRequisito =
                                (SolicitacaoServico.PreRequisitoSolicitacao)e.Item.DataItem;

                        if (!object.ReferenceEquals(preRequisito, null))
                        {
                            Label lblDescricaoCampo = (Label)e.Item.FindControl("lblDescricaoCampo");
                            Label lblConteudoCampo = (Label)e.Item.FindControl("lblConteudoCampo");

                            if (!object.ReferenceEquals(lblDescricaoCampo, null) && !object.ReferenceEquals(lblConteudoCampo, null))
                            {
                                lblDescricaoCampo.Text = preRequisito.NomeCampo;
                                lblConteudoCampo.Text = preRequisito.ValorCampo;
                            }
                        }
                    }
                }
                catch (FaultException<SolicitacaoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Preencher os parâmetros do popup de Seleção de carta
        /// </summary>
        public void PreencherParametrosCarta()
        {
            String urlParametros = "";
            try
            {
                QueryStringSegura queryString = new QueryStringSegura();
                queryString["NumeroSolicitacao"] = lblSolicitacao.Text;

                urlParametros = String.Format("version={0}&dados={1}",
                        System.Guid.NewGuid().ToString("N"), queryString.ToString());

                parametrosCarta.Text = urlParametros;
            }
            catch (PortalRedecardException ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                parametrosCarta.Text = "";
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                parametrosCarta.Text = "";
            }
        }

        /// <summary>
        /// Carrega a função para paginação da tabela de resultados da pesquisa
        /// </summary>
        protected void CarregarPaginacao()
        {
            //Adiciona a função de JS de paginação do grid para as funções a serem executadas após renderização
            //pageResultTable(idTabela, idDivPai, pageIndex, pageSize, pagesPerView)
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Paginacao", "pageResultTable('tblSolicitacoes', 1, 10, 5);", true);
        }
    }
}
