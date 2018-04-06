using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.Request.SharePoint.XBChargebackServico;
using Redecard.PN.Request.Core.Web.Controles.Portal;
using System.Web.UI.HtmlControls;
using System.Text;
using System.IO;

namespace Redecard.PN.Request.SharePoint.WebParts.RequestHistoricoFiltro
{
    public partial class RequestHistoricoFiltroUserControl : UserControlBase
    {
        #region [ Propriedades e Atributos ]

        private QueryStringSegura QS
        {
            get
            {
                if (Request.QueryString["dados"] != null)
                    return new QueryStringSegura(Request.QueryString["dados"]);
                else
                    return null;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            //Definição do botão default da página
            this.Page.Form.DefaultButton = btnBuscar.UniqueID;

            //Esconde a mensagem de erro
            //lblMensagem.Visible = false;

            //Datas não podem ser posteriores à data atual
            if (!Page.IsPostBack)
            {
                //Veio da tela Avisos de Débito se possuir QueryString
                if (QS != null)
                {
                    //Define que a origem foi a tela Avisos de Débito                    
                    Decimal codProcesso = QS["NumProcesso"].ToDecimal();
                    String tipoVenda = QS["TipoVenda"];

                    //Atualiza os campos de filtro com os dados passados pela querystring
                    ddlTipoVenda.SelectedValue = tipoVenda;
                    txtNumeroProcesso.Text = codProcesso.ToString();

                    //Atualiza os dados do repeater, informando que a 
                    CarregarHistorico("DEBITO");
                }
                else
                {
                    VerificarQueryString();
                }
            }
            if (this.SessaoAtual != null)
            {
                // dados do usuário no header do bloco de impressão
                this.lblHeaderImpressaoUsuario.Text = string.Concat(SessaoAtual.CodigoEntidade, " / ", SessaoAtual.LoginUsuario);
            }
            //else
            //{
            //    pnlErro.Visible = false;
            //}
        }

        #region [ Handlers de eventos ]

        /// <summary>Handler que exibe efetivamente as informações nas linhas da tabela</summary>        
        protected void rptHistorico_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    // Recupera objetos da linha
                    var ltlNumProcesso = (Literal)e.Item.FindControl("ltlNumProcesso");
                    var btnDebito = (HyperLink)e.Item.FindControl("btnDebito");
                    var ltlNumResumo = (Literal)e.Item.FindControl("ltlNumResumo");
                    var ltlCentralizadora = (Literal)e.Item.FindControl("ltlCentralizadora");
                    var ltlNumCartao = (Literal)e.Item.FindControl("ltlNumCartao");
                    var btnInformacao = (BotaoInformacao)e.Item.FindControl("btnInformacao");
                    var ltlDataVenda = (Literal)e.Item.FindControl("ltlDataVenda");
                    var ltlValorVenda = (Literal)e.Item.FindControl("ltlValorVenda");
                    var ltlEnvioNotificacao = (Literal)e.Item.FindControl("ltlEnvioNotificacao");
                    var ltlDocEnviado = (Literal)e.Item.FindControl("ltlDocEnviado");
                    var ltlQualidadeDoc = (Literal)e.Item.FindControl("ltlQualidadeDoc");

                    XBChargebackServico.Comprovante request = e.Item.DataItem as XBChargebackServico.Comprovante;

                    //setando os valores nos controles das linhas
                    ltlNumProcesso.Text = request.Processo.ToString();
                    ltlNumResumo.Text = request.ResumoVenda.ToString();
                    ltlCentralizadora.Text = request.Centralizadora.ToString();
                    ltlNumCartao.Text = request.NumeroCartao;
                    ltlDataVenda.Text = request.DataVenda.ToString("dd/MM/yy");
                    ltlValorVenda.Text = request.ValorVenda.ToString("N2");

                    String canalEnvio = ObtemDescricaoCanalEnvio((Int32?)request.CanalEnvio, null);
                    String dataEnvio = request.DataEnvio.HasValue && request.DataEnvio.Value > DateTime.MinValue ? (request.DataEnvio.Value.ToString("dd/MM/yy")) : String.Empty;
                    ltlEnvioNotificacao.Text = String.Format("{0} {1}", canalEnvio, dataEnvio);

                    ltlDocEnviado.Text = request.SolicitacaoAtendida ? "Sim" : "Não";
                    //lblDocEnviado.ForeColor = request.SolicitacaoAtendida ? Color.Green : Color.Red;
                    ltlQualidadeDoc.Text = request.QualidadeRecebimentoDocumentos;
                    //btnMotivo.Attributes.Add("mensagem", HttpUtility.HtmlEncode(request.Motivo));
                    //btnMotivo.Attributes.Add("titulo", "MOTIVO DO PROCESSO");
                    btnInformacao.Titulo = "motivo do processo";
                    btnInformacao.Mensagem = HttpUtility.HtmlEncode(request.Motivo);

                    QueryStringSegura queryString = new QueryStringSegura();
                    queryString["TipoVenda"] = ddlTipoVenda.SelectedValue;
                    queryString["NumProcesso"] = request.Processo.ToString();
                    btnDebito.NavigateUrl = String.Format(base.web.ServerRelativeUrl + "/Paginas/pn_AvisoDebito.aspx?dados={0}", queryString.ToString());
                    btnDebito.Visible = request.IndicadorDebito ?? false;// só mostra se tiver débitos
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante DataBind de dados de Histórico de Comprovantes", ex);
                ExibirHistorico(false, false);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Evento de click para download em excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDownloadExcel_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Histórico de Comprovantes - Gerar Excel"))
            {
                try
                {
                    Response.Clear();
                    Response.AddHeader("content-disposition", String.Format("attachment;filename=HistoricoComprovantes_{0}.xls", DateTime.Now.ToString("yyyyMMdd_hhmmss")));
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding(1252);
                    Response.ContentType = "application/ms-excel";

                    StringWriter stringWrite = new StringWriter();
                    HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);

                    htmlWrite.Write("<div>");
                    htmlWrite.Write("<strong>Historico de Comprovantes</strong>");
                    htmlWrite.Write("<br /><br />");
                    htmlWrite.Write(String.Format(@"Data da consulta: {0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
                    htmlWrite.Write("<br />");
                    htmlWrite.Write(String.Format(@"Usuário: {0}", SessaoAtual.LoginUsuario));
                    htmlWrite.Write("<br />");
                    htmlWrite.Write(String.Format(@"Nome do estabelecimento: {0}", SessaoAtual.NomeEntidade));
                    htmlWrite.Write("<br />");
                    htmlWrite.Write(String.Format(@"Nº do estabelcimento: {0}", SessaoAtual.CodigoEntidade));
                    htmlWrite.Write("</div>");
                    htmlWrite.Write("<br />");

                    List<Control> lstControlsRemove = new List<Control>();


                    foreach (Control item in rptHistorico.Controls)
                    {
                        if (item.GetType() == typeof(RepeaterItem))
                        {
                            //Obtendo o botão de informação da linha, caso haja
                            BotaoInformacao controleBotaoInformacao = item.FindControl("btnInformacao") != null ? (BotaoInformacao)item.FindControl("btnInformacao") : null;
                            if (controleBotaoInformacao != null)
                            {
                                //Extraindo o texto da mensagem do botão, e inserindo em um Literal
                                Literal textoBotao = new Literal();
                                textoBotao.Text = controleBotaoInformacao.Mensagem;

                                //Obtendo o índice do Botão de informação, removendo o mesmo e inserindo o Literal no lugar, para exportar somente o texto no Excel
                                Int32 itemIndex = item.Controls.IndexOf(controleBotaoInformacao);
                                item.Controls.RemoveAt(itemIndex);
                                item.Controls.AddAt(itemIndex, textoBotao);
                            }
                            RepeaterItem rptItem = (RepeaterItem)item;

                            //Removendo o footer, para não causar exception com o LinkButton da paginação.
                            bool remove = rptItem.ItemType == ListItemType.Footer;
                            if (remove)
                            {
                                lstControlsRemove.Add(item);
                            }
                        }
                    }

                    foreach (var item in lstControlsRemove)
                    {
                        rptHistorico.Controls.Remove(item);
                    }

                    rptHistorico.RenderControl(htmlWrite);

                    string resp = stringWrite.ToString();

                    Response.Write(resp);
                    Response.End();
                }
                catch (FaultException<GeneralFault> fe)
                {
                    Logger.GravarErro("Consultar Cancelamento - Gerar Excel", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Consultar Cancelamento - Gerar Excel", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>Handler do botão que aciona a busca e exibe o grid de resultados</summary>        
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        /// <summary>Handler do botão voltar, redireciona para a tela de Comprovantes Pendentes</summary>        
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect(base.web.ServerRelativeUrl + "/Paginas/pn_ComprovacaoVendas.aspx");
        }

        protected IEnumerable<Object> Paginacao_ObterDados(Guid IdPesquisa, int registroInicial, int quantidadeRegistros, int quantidadeRegistroBuffer, out int quantidadeTotalRegistrosEmCache, params object[] parametros)
        {
            String msgLog = String.Format("Histórico de Comprovantes - {0} ({1}-{2})", ddlTipoVenda.SelectedValue, registroInicial, registroInicial + quantidadeRegistros);

            using (Logger Log = Logger.IniciarLog(msgLog))
            {
                //Objetos de retorno
                IEnumerable<Object> retorno = new List<Object>();
                quantidadeTotalRegistrosEmCache = 0;

                try
                {
                    //Recupera valores do parâmetros para a consulta
                    String tipoVenda = ddlTipoVenda.Text;
                    Decimal codProcesso = txtNumeroProcesso.Text.ToDecimalNull(0).Value;
                    DateTime dtInicio = String.IsNullOrWhiteSpace(txtDtIniDatePicker.Text) ? DateTime.Now.AddDays(-60) : Convert.ToDateTime(txtDtIniDatePicker.Text);
                    // Requisito dos books enviar a data atual como data Fim
                    DateTime dtFim = String.IsNullOrWhiteSpace(txtDtFimDatePicker.Text) ? DateTime.Now : Convert.ToDateTime(txtDtFimDatePicker.Text);
                    String origemConsulta = parametros != null && parametros.Length > 0 ? parametros[0].ToString() : "REQUESTHISTORICO";

                    #region [ Crédito ]
                    if (tipoVenda.Equals("CREDITO", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Int32 codRetorno = default(Int32);
                        String sistemaOrigem = SessaoAtual.UsuarioAtendimento ? "IZ" : "IS";

                        Log.GravarMensagem("Instanciando serviço HISServicoXB - Crédito");

                        //Client para serviço XB
                        using (HISServicoXBChargebackClient client = new HISServicoXBChargebackClient())
                        {
                            Log.GravarLog(EventoLog.ChamadaServico, new
                            {
                                IdPesquisa,
                                registroInicial,
                                quantidadeRegistros,
                                quantidadeRegistroBuffer,
                                SessaoAtual.CodigoEntidade,
                                codProcesso,
                                origemConsulta,
                                dtInicio,
                                dtFim,
                                sistemaOrigem
                            });

                            XBChargebackServico.Comprovante[] historico = client.HistoricoRequest(
                                out quantidadeTotalRegistrosEmCache,
                                out codRetorno,
                                IdPesquisa,
                                registroInicial,
                                quantidadeRegistros,
                                quantidadeRegistroBuffer,
                                SessaoAtual.CodigoEntidade,
                                origemConsulta,
                                dtInicio,
                                dtFim,
                                codProcesso,
                                sistemaOrigem);

                            Log.GravarLog(EventoLog.RetornoServico, new { quantidadeTotalRegistrosEmCache, codRetorno, historico });

                            //caso o código de retorno seja != 0 ocorreu um erro.
                            //considerar que 10 ou 53 não é erro: DADOS NAO ENCONTRADOS NA TABELA
                            if (codRetorno > 0 && codRetorno != 10 && codRetorno != 53)
                                base.ExibirPainelExcecao("XBChargebackServico.HistoricoRequest", codRetorno);
                            else
                                retorno = historico;
                        }

                        //Se não retornou registros, exibe mensagem adequada
                        Boolean exibirHistorico = retorno != null && retorno.Count() > 0 && quantidadeTotalRegistrosEmCache > 0;
                        this.ExibirHistorico(exibirHistorico, !exibirHistorico);
                    }
                    #endregion
                    #region [ Débito ]
                    else
                    {
                        Int16 codRetorno = default(Int16);

                        Log.GravarMensagem("Instanciando serviço HISServicoXB - Débito");

                        //Client para serviço XB
                        using (HISServicoXBChargebackClient client = new HISServicoXBChargebackClient())
                        {
                            Log.GravarLog(EventoLog.ChamadaServico, new
                            {
                                IdPesquisa,
                                registroInicial,
                                quantidadeRegistros,
                                quantidadeRegistroBuffer,
                                codProcesso,
                                SessaoAtual.CodigoEntidade,
                                dtInicio,
                                dtFim,
                                origem = "IS",
                                transacao = "XB95"
                            });

                            XBChargebackServico.Comprovante[] historico = client.ConsultarHistoricoSolicitacoes(
                                out quantidadeTotalRegistrosEmCache,
                                out codRetorno,
                                IdPesquisa,
                                registroInicial,
                                quantidadeRegistros,
                                quantidadeRegistroBuffer,
                                codProcesso,
                                SessaoAtual.CodigoEntidade,
                                dtInicio,
                                dtFim,
                                "IS",
                                "XB95");

                            Log.GravarLog(EventoLog.RetornoServico, new { quantidadeTotalRegistrosEmCache, codRetorno, historico });

                            //caso o código de retorno seja != 0 ocorreu um erro.
                            //considerar que 10 ou 53 não é erro: DADOS NAO ENCONTRADOS NA TABELA
                            if (codRetorno > 0 && codRetorno != 10 && codRetorno != 53)
                                base.ExibirPainelExcecao("XBChargebackServico.ConsultarHistoricoSolicitacoes", codRetorno);
                            else
                                retorno = historico;
                        }

                        //Se não retornou registros, exibe mensagem adequada
                        Boolean exibirHistorico = retorno != null && retorno.Count() > 0 && quantidadeTotalRegistrosEmCache > 0;
                        this.ExibirHistorico(exibirHistorico, !exibirHistorico);
                    }
                    #endregion
                }
                catch (FaultException<XBChargebackServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirHistorico(false, false);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirHistorico(false, false);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }

                return retorno;
            }
        }

        #endregion

        #region [ Métodos privados ]

        /// <summary>
        /// verificação da query string, caso existam parâmetros
        /// informados via query string aberta, que podem ser:
        /// <para>"processo" (opcional): Int32, número do processo</para>
        /// <para>"tipoVenda" (opcional): String, tipo da venda ("Credito" ou "Debito")</para>
        /// <para>"de" (opcional): String, data início do período, formato dd/MM/yyyy</para>
        /// <para>"para" (opcional): String, data fim do período, formato dd/MM/yyyy</para>
        /// <para>"pesquisar" (opcional): apenas chave, se existir, executa pesquisa além de preencher os campos</para>
        /// </summary>
        private void VerificarQueryString()
        {
            //Recupera parâmetros provenientes de query string aberta
            Decimal? numeroProcesso = Request["processo"].ToDecimalNull();
            String tipoVenda = Request["tipoVenda"]; //"Credito" ou "Debito"
            DateTime? dataInicio = Request["de"].ToDateTimeNull("dd/MM/yyyy");
            DateTime? dataFim = Request["ate"].ToDateTimeNull("dd/MM/yyyy");
            Boolean pesquisar = Request.QueryString.AllKeys.Contains("pesquisar");

            //preenche controles caso parâmetros existam e sejam válidos
            if (numeroProcesso.HasValue)
                txtNumeroProcesso.Text = numeroProcesso.ToString();
            if (tipoVenda.EmptyToNull() != null)
                ddlTipoVenda.SelectedValue = tipoVenda;
            if (dataInicio.HasValue)
                txtDtIniDatePicker.Text = dataInicio.Value.ToString("dd/MM/yyyy");
            if (dataFim.HasValue)
                txtDtFimDatePicker.Text = dataFim.Value.ToString("dd/MM/yyyy");

            //executa pesquisa se recebido parâmetro pesquisar
            if (pesquisar)
                Buscar();
        }

        //efetua a busca com os dados preenchidos no filtro
        private void Buscar()
        {
            //Valida os campos digitados
            cvValidaData.Validate();

            if (cvValidaData.IsValid)
            {
                //Carrega os dados da pesquisa
                CarregarHistorico("REQUESTHISTORICO");
            }
        }

        private void ExibirHistorico(Boolean exibirHistorico, Boolean exibirSemComprovantes)
        {
            rptHistorico.Visible = exibirHistorico;
            pnlSemHistorico.Visible = exibirSemComprovantes;
            qdAvisoSemHistorico.TipoQuadro = TipoQuadroAviso.Aviso;

            //Exibe o menu de ações
            mnuAcoes.Visible = exibirHistorico;

            if (exibirSemComprovantes)
                qdAvisoSemHistorico.Mensagem = "não foram encontrados comprovantes.";

            //spnBotaoVoltarLeft.Attributes["style"] = !exibirHistorico ? "" : "display:none";
            //trBotaoVoltarRight.Attributes["style"] = exibirHistorico ? "" : "display:none";
        }

        /// <summary>Carrega o repeater carregando (paginado) os dados do mainframe</summary>
        /// <param name="origemConsulta">Origem da consulta: DEBITO (se proveniente da tela Aviso de Débito) ou REQUESTHISTORICO.</param>
        private void CarregarHistorico(String origemConsulta)
        {
            //Repeater visível
            rptHistorico.Visible = true;

            //Atualiza datasource do repeater            
            paginacao.Carregar(origemConsulta);
        }

        /// <summary>Valida os filtros informados. </summary>
        /// <returns>Mensagem de validação. Retorna String.Empty caso validação esteja OK.</returns>
        private String ValidarFiltros()
        {
            if (string.Compare(ddlTipoVenda.SelectedValue, "Selecione", true) == 0)
            {
                return "selecione o tipo de venda.";
            }

            //Se processo não foi informado, valida os campos de data
            if (String.IsNullOrEmpty(txtNumeroProcesso.Text.Trim()))
            {
                //Verifica se o período informado está dentro dos 60 dias
                DateTime dtInicio = DateTime.MinValue;
                DateTime dtFim = DateTime.MinValue;
                DateTime dtNow = DateTime.Now;
                TimeSpan dtDiff = default(TimeSpan);

                if (!DateTime.TryParse(txtDtIniDatePicker.Text, out dtInicio))
                {
                    return "data inicial do período não foi informada ou inválida.";
                }
                if (!DateTime.TryParse(txtDtFimDatePicker.Text, out dtFim))
                {
                    return "data final do período não foi informada ou inválida.";
                }

                //A data final não pode ser maior que hoje
                dtDiff = dtNow.Subtract(dtFim);
                if (dtDiff.TotalDays < 0)
                    return "o período contém data futura!";

                //Calcula a diferença de dias entre as datas informadas, limitando em 60 dias
                dtDiff = dtFim.Subtract(dtInicio);
                if (dtDiff.TotalDays > 60)
                    return "a consulta não deve ultrapassar 60 dias!";

                //A data final não pode ser menor que a data inicial
                dtDiff = dtFim.Subtract(dtInicio);
                if (dtDiff.TotalDays < 0)
                    return "período inválido! a data final não pode ser menor que a data inicial.";
            }

            return String.Empty;
        }

        private String ObtemDescricaoCanalEnvio(Int32? canalEnvio, String descricaoPadrao)
        {
            switch (canalEnvio)
            {
                case 1: return "correio";
                case 2: return "fax automático";
                case 3: return "não envia";
                case 4: return "internet";
                case 5: return "e-mail";
                case 6: return "arquivo e transmissão de request de chargeback";
                case 7: return "extrato eletrônico";
                default: return descricaoPadrao.EmptyToNull() ?? "indefinido";
            }
        }

        #endregion

        protected void cvValidaData_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string mensagem = ValidarFiltros();
            if (!string.IsNullOrEmpty(mensagem))
            {
                //Limpa os dados carregados
                this.ExibirHistorico(false, false);

                cvValidaData.Text = mensagem;
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

    }
}