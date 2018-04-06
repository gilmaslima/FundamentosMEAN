/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 28/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.FMS.Sharepoint.Exceptions;
using Redecard.PN.FMS.Sharepoint.Helpers;
using Redecard.PN.FMS.Sharepoint.Interfaces;
using Redecard.PN.FMS.Sharepoint.Modelo;
using Redecard.PN.FMS.Sharepoint.Servico.FMS;
using Microsoft.SharePoint.Utilities;

namespace Redecard.PN.FMS.Sharepoint.WebParts.AnalisaTransacoesSuspeitasPorCartao
{
    public partial class AnalisaTransacoesSuspeitasPorCartaoUserControl : RelatorioGridBaseUserControl<CriterioOrdemTransacoesPorNumeroCartaoOuAssociada>, IPossuiExportacao
    {
        #region Atributos
        private List<TransacaoSuspeita> TransacoesSuspeitas
        {
            get
            {
                if (ViewState["TransacoesSuspeitas"] == null)
                {
                    ViewState["TransacoesSuspeitas"] = new List<TransacaoEmissor>();
                }

                return (List<TransacaoSuspeita>)ViewState["TransacoesSuspeitas"];
            }
            set { ViewState["TransacoesSuspeitas"] = value; }
        }

        private List<TipoResposta> TiposResposta
        {
            get
            {
                if (ViewState["TiposResposta"] == null)
                {
                    ViewState["TiposResposta"] = new List<TipoResposta>();
                }

                return (List<TipoResposta>)ViewState["TiposResposta"];
            }
            set { ViewState["TiposResposta"] = value; }
        }

        private long IdentificadorTransacao
        {
            get
            {
                return (long)ViewState["IdentificadorTransacao"];
            }
            set
            {
                ViewState["IdentificadorTransacao"] = value;
            }
        }

        private long NumeroEstabelecimento
        {
            get
            {
                if (ViewState["NumeroEstabelecimento"] == null)
                    return 0;
                else
                    return (long)ViewState["NumeroEstabelecimento"];
            }
            set
            {
                ViewState["NumeroEstabelecimento"] = value;
            }
        }

        private string RedirectedPage
        {
            get 
            {
                if (Request.QueryString["redirectedPage"] != null && Request.QueryString["redirectedPage"].ToString() != "")
                    return Request.QueryString["redirectedPage"].ToString();

                return null;
            }
        }

        #endregion

        #region Eventos
        /// <summary>
        /// Evento que irá abrir a página.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Análise de transações suspeitas por cartão] - Page_Load.");

                ObterValoresNavegacao();

                if (Request.QueryString["release"] != null)
                {
                    DesbloquearAnaliseCartao();
                    return;
                }

                if (!IsPostBack)
                {
                    
                    PreencheComboTipoResposta();

                    if (this.NumeroEstabelecimento != 0)
                    {
                        this.txtCodigoEstabelecimento.Text = this.NumeroEstabelecimento.ToString();
                    }
                    else
                    {
                        this.txtCodigoEstabelecimento.Visible = false;
                        this.lblEstabelecimento.Visible = false;
                    }

                    List<Servico.FMS.TransacaoEmissor> transacoesEmissor = null;

                    if (Session["FMS_transacoes"] != null)
                        transacoesEmissor = (List<Servico.FMS.TransacaoEmissor>)base.RemoverAtributoSessao("FMS_transacoes");
                        
                    btnBuscar.Enabled = false;
                    txtNumeroCartao.ReadOnly = true;

                    if (transacoesEmissor == null && GetSessaoAtual.UsuarioMaster())
                    {
                        btnBuscar.Enabled = true;
                        txtNumeroCartao.ReadOnly = false;
                        txtNumeroCartao.ReadOnly = false;
                    }
                    else
                    {
                        this.TransacoesSuspeitas = ConverterParaListaTransacoesSuspeitas(transacoesEmissor);
                        CarregarGrid(this.TransacoesSuspeitas);
                        RegistrarJSAbandono();
                    }

                    txtNumeroCartao.CssClass += " numerico";
                    txtCodigoEstabelecimento.CssClass += " numerico";
                    
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }
        /// <summary>
        /// Evento que irá ocorrer ao clicar no botão.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            decimal numeroCartao;
            long numeroEstabelecimento;

            if (!decimal.TryParse(txtNumeroCartao.Text, out numeroCartao))
            {
                if (string.IsNullOrEmpty(txtNumeroCartao.Text))
                {
                    base.ExibirMensagem("Informe o número do cartão.");
                    return;
                }
                else
                {
                    base.ExibirMensagem("Número do cartão inválido.");
                    return;
                }
            }

            if (!long.TryParse(txtCodigoEstabelecimento.Text, out numeroEstabelecimento))
            {
                if (string.IsNullOrEmpty(txtCodigoEstabelecimento.Text))
                {
                    numeroEstabelecimento = 0;
                }
                else
                {
                    base.ExibirMensagem("Número do Estabelecimento inválido.");
                    return;
                }
            }


            Redecard.PN.Comum.SharePointUlsLog.LogMensagem(string.Format("[Análise de transações suspeitas por cartão] - numeroCartao [{0}]", numeroCartao));

            try
            {
                PesquisarTransacoesPorNumeroCartaoEEstabelecimentoEnvio envio = new PesquisarTransacoesPorNumeroCartaoEEstabelecimentoEnvio()
                {
                    NumeroCartao = numeroCartao.ToString(),
                    NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                    GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                    UsuarioLogin = GetSessaoAtual.LoginUsuario,
                    TempoBloqueio = base.ParametrosSistema.TempoExpiracaoBloqueio,
                    TipoCartao = base.Tipocartao,
                    NumeroEstabelecimento = numeroEstabelecimento
                };

                using (Servico.FMS.FMSClient client = new Servico.FMS.FMSClient())
                {
                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Análise de transações suspeitas por cartão] - início da execução do serviço 'PesquisarTransacoesPorNumeroCartao'.");

                    Servico.FMS.PesquisarTransacoesPorNumeroCartaoEEstabelecimentoRetorno retorno = client.PesquisarTransacoesPorNumeroCartaoEEstabelecimento(envio);

                    Redecard.PN.Comum.SharePointUlsLog.LogMensagem("[Análise de transações suspeitas por cartão] - fim da execução do serviço 'PesquisarTransacoesPorNumeroCartao'.");

                    ValidarRetornoPesquisaPorCartao(retorno);

                    if (retorno.ListaTransacoesEmissor == null || retorno.ListaTransacoesEmissor.Length == 0)
                    {
                        base.ExibirMensagem("Não existem transações para esta consulta.");
                        return;
                    }

                    this.IdentificadorTransacao = retorno.ListaTransacoesEmissor[0].IdentificadorTransacao;

                    this.TransacoesSuspeitas = ConverterParaListaTransacoesSuspeitas(new List<Servico.FMS.TransacaoEmissor>(retorno.ListaTransacoesEmissor));

                    CarregarGrid(this.TransacoesSuspeitas);

                    RegistrarJSAbandono();
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }
        /// <summary>
        /// Evento que irá ocorrer ao gravar os dados do data-row.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grvDados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            CheckBox chkTransacaoSelecionada = (CheckBox)e.Row.FindControl("chkTransacaoSelecionada");

            TransacaoSuspeita transacao = (TransacaoSuspeita)e.Row.DataItem;

            chkTransacaoSelecionada.Checked = transacao.TransacaoSelecionada;

            if (transacao.DataAnalise != DateTime.MinValue &&
                transacao.DataAnalise.ToShortDateString() != DateTime.Now.ToShortDateString())
            {
                chkTransacaoSelecionada.Enabled = false;
            }

            SPListaPadrao lista = base.ObterPadraoASerAplicadoPorSituacaoFraude(transacao.TipoResposta.SituacaoFraude);

            if (lista != null)
            {
                e.Row.Cells[1].Style.Add("color", lista.CorDeLetra);
                e.Row.Cells[1].Style.Add("background-color", lista.CorDeFundo);

                e.Row.Cells[1].Text = lista.Titulo;
            }

            e.Row.Cells[e.Row.Cells.Count - 1].CssClass += " last";
        }
        /// <summary>
        /// Evento que irá ocorrer ao clicar no botão atualizar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAtualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!TodasTransacoesRespondidas())
                {
                    base.ExibirMensagem("Por favor, informe o tipo de resposta para as transações alarmadas.");
                    return;
                }

                List<RespostaAnaliseItem> items = new List<RespostaAnaliseItem>();

                foreach (GridViewRow linha in grvDados.Rows)
                {
                    CheckBox chkTransacaoSelecionada = (CheckBox)linha.FindControl("chkTransacaoSelecionada");

                    TransacaoSuspeita t = this.TransacoesSuspeitas[linha.RowIndex];

                    if (t.SituacaoTransacao != SituacaoFraude.NaoAplicavel && t.TransacaoAlterada)
                    {
                        RespostaAnaliseItem item = new RespostaAnaliseItem()
                        {
                            IdentificadorTransacao = t.IdentificadorTransacao,
                            Comentario = t.ComentarioAnalise,
                            TipoResposta = new TipoResposta()
                            {
                                CodigoResposta = t.TipoResposta.CodigoResposta,
                                DescricaoResposta = t.TipoResposta.DescricaoResposta,
                                NomeResposta = t.TipoResposta.NomeResposta,
                                SituacaoFraude = t.SituacaoTransacao
                            },
                            TipoAlarme = t.TipoAlarme,
                            EhFraude = t.SituacaoTransacao == SituacaoFraude.Fraude,
                            NumeroEmissor = GetSessaoAtual.CodigoEntidade.ToString(),
                            GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                            UsuarioLogin = GetSessaoAtual.LoginUsuario
                        };

                        items.Add(item);
                    }
                }

                if (items.Count > 0)
                {
                    using (Servico.FMS.FMSClient client = new Servico.FMS.FMSClient())
                    {
                        AtualizarResultadoAnaliseEnvio envio = new AtualizarResultadoAnaliseEnvio()
                        {
                            ListaRespostaAnalise = items.ToArray(),
                            TipoCartao = base.Tipocartao
                        };

                        client.AtualizarResultadoAnalise(envio);
                    }

                    base.ExibirMensagem("Operação realizada com sucesso.");

                    return;
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }
        /// <summary>
        /// Evento que irá ocorrer ao clicar no botão associar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAssosciar_Click(object sender, EventArgs e)
        {
            try
            {
                int tipoResposta;

                int.TryParse(cboTipoResposta.SelectedValue, out tipoResposta);
                
                if (tipoResposta == 0)
                {
                    base.ExibirMensagem("Por favor, informe o tipo de resposta.");
                    return;
                }

                Servico.FMS.TipoResposta objTipoResposta = (Servico.FMS.TipoResposta)TiposResposta.Find(v => v.CodigoResposta == tipoResposta);

                foreach (GridViewRow linha in grvDados.Rows)
                {
                    CheckBox chkTransacaoSelecionada = (CheckBox)linha.FindControl("chkTransacaoSelecionada");

                    if (chkTransacaoSelecionada.Checked)
                    {
                        TransacaoSuspeita t = this.TransacoesSuspeitas[linha.RowIndex];

                        t.TransacaoAlterada = true;
                        t.TransacaoSelecionada = true;
                        t.ComentarioAnalise = SPHttpUtility.HtmlEncode(txtComentarios.Text);

                        if (tipoResposta > 0)
                        {
                            t.SituacaoTransacao = objTipoResposta.SituacaoFraude;
                            t.TipoResposta = new TipoResposta()
                            {
                                CodigoResposta = objTipoResposta.CodigoResposta,
                                DescricaoResposta = objTipoResposta.DescricaoResposta,
                                NomeResposta = objTipoResposta.NomeResposta,
                                SituacaoFraude = (SituacaoFraude)Enum.ToObject(typeof(SituacaoFraude), objTipoResposta.SituacaoFraude)
                            };
                        }
                        else
                        {
                            t.SituacaoTransacao = SituacaoFraude.NaoFraude;
                            t.TipoResposta = null;
                        }
                        chkTransacaoSelecionada.Checked = false;
                    }
                }

                CarregarGrid(this.TransacoesSuspeitas);
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }

        #endregion

        #region Métodos
        /// <summary>
        /// Este método é utilizado para carregar os grids.
        /// </summary>
        /// <param name="transacoes"></param>
        private void CarregarGrid(List<TransacaoSuspeita> transacoes)
        {
            if (transacoes.Count > 0)
            {
                txtNumeroCartao.Text = transacoes[0].NumeroCartao;
            }

            grvDados.DataSource = transacoes;
            grvDados.DataBind();
        }
        /// <summary>
        /// Este método é utilizado para preenche combo tipo resposta.
        /// </summary>
        private void PreencheComboTipoResposta()
        {
            try
            {
                using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
                {
                    Servico.FMS.PesquisaTipoRespostaEnvio envio = new Servico.FMS.PesquisaTipoRespostaEnvio()
                    {
                        NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                        GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                        UsuarioLogin = GetSessaoAtual.LoginUsuario
                    };

                    PesquisaTipoRespostaRetorno objRetorno = objClient.PesquisarTiposResposta(envio);
                    cboTipoResposta.Items.Add(new ListItem("Selecione", "0"));

                    TiposResposta = new List<Servico.FMS.TipoResposta>(objRetorno.ListaTipoResposta);

                    foreach (Servico.FMS.TipoResposta tipoResposta in objRetorno.ListaTipoResposta)
                    {
                        cboTipoResposta.Items.Add(new ListItem()
                        {
                            Text = tipoResposta.DescricaoResposta,
                            Value = tipoResposta.CodigoResposta.ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }
        /// <summary>
        /// Este método é utilizado para desbloquear cartão.
        /// </summary>
        protected void DesbloquearCartao()
        {
            try
            {
                using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
                {
                    DesbloquearCartaoEnvio envio = new DesbloquearCartaoEnvio();
                    envio.IdentificadorTransacao = IdentificadorTransacao;
                    envio.NumeroEmissor = GetSessaoAtual.CodigoEntidade;
                    envio.GrupoEntidade = GetSessaoAtual.GrupoEntidade;
                    envio.UsuarioLogin = GetSessaoAtual.LoginUsuario;
                    objClient.DesbloquearCartao(envio);
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }
        /// <summary>
        /// Este método é utilizado para todas transações respondidas.
        /// </summary>
        /// <returns></returns>
        private bool TodasTransacoesRespondidas()
        {
            int i = 0;

            foreach (GridViewRow linha in grvDados.Rows)
            {
                CheckBox chkTransacaoSelecionada = (CheckBox)linha.FindControl("chkTransacaoSelecionada");

                TransacaoSuspeita t = this.TransacoesSuspeitas[linha.RowIndex];

                if (t.SituacaoTransacao == SituacaoFraude.NaoAplicavel && (t.TipoAlarme == TipoAlarme.POC || t.TipoAlarme == TipoAlarme.UTL) && chkTransacaoSelecionada.Checked)
                {
                    return false;
                }

                if (t.SituacaoTransacao != SituacaoFraude.NaoAplicavel && chkTransacaoSelecionada.Checked)
                {
                    i++;
                }
            }

            if (i == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// Este método é utilizado para definir o critério da ordem de transações por numero cartão ou associada.
        /// </summary>
        /// <returns></returns>
        protected override CriterioOrdemTransacoesPorNumeroCartaoOuAssociada ObterCriterioOrdemInicial()
        {
            return CriterioOrdemTransacoesPorNumeroCartaoOuAssociada.Autorizacao;
        }
        /// <summary>
        /// Este método é utilizado para montar o grid.
        /// </summary>
        /// <param name="objClient"></param>
        /// <param name="criterioOrdem"></param>
        /// <param name="ordemClassificacao"></param>
        /// <param name="primeiroRegistro"></param>
        /// <param name="quantidadeRegistroPagina"></param>
        /// <returns></returns>
        protected override long MontaGrid(FMSClient objClient, CriterioOrdemTransacoesPorNumeroCartaoOuAssociada criterioOrdem, OrdemClassificacao ordemClassificacao, int primeiroRegistro, int quantidadeRegistroPagina)
        {
            Servico.FMS.PesquisarTransacoesPorTransacaoAssociadaEnvio envio = new Servico.FMS.PesquisarTransacoesPorTransacaoAssociadaEnvio()
            {
                IdentificadorTransacao = this.IdentificadorTransacao,
                NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                UsuarioLogin = GetSessaoAtual.LoginUsuario,
                TipoCartao = base.Tipocartao,
                TempoBloqueio = base.ParametrosSistema.TempoExpiracaoBloqueio,
                Criterio = base.CriterioOrdem,
                Ordem = base.OrdemClassificacao
            };

            Servico.FMS.TransacaoEmissor[] retorno = objClient.PesquisarTransacoesPorTransacaoAssociada(envio);

            this.TransacoesSuspeitas = ConverterParaListaTransacoesSuspeitas(new List<Servico.FMS.TransacaoEmissor>(retorno));

            CarregarGrid(this.TransacoesSuspeitas);

            return retorno.Length;
        }

        /// <summary>
        /// Este método é utilizado para converter para lista transações suspeitas.
        /// </summary>
        /// <param name="transacoesEmissor"></param>
        /// <returns></returns>
        private List<TransacaoSuspeita> ConverterParaListaTransacoesSuspeitas(List<Servico.FMS.TransacaoEmissor> transacoesEmissor)
        {
            List<TransacaoSuspeita> transacoesSuspeitas = new List<TransacaoSuspeita>();

            foreach (TransacaoEmissor transacaoEmissor in transacoesEmissor)
            {
                TransacaoSuspeita transacaoSuspeita = ConverterParaTransacoesSuspeitas(transacaoEmissor);

                transacoesSuspeitas.Add(transacaoSuspeita);
            }

            return transacoesSuspeitas;
        }
        /// <summary>
        /// Este método é utilizado para converter para transações suspeitas.
        /// </summary>
        /// <param name="transacaoEmissor"></param>
        /// <returns></returns>
        private TransacaoSuspeita ConverterParaTransacoesSuspeitas(Servico.FMS.TransacaoEmissor transacaoEmissor)
        {
            TransacaoSuspeita transacaoSuspeita = new TransacaoSuspeita();

            transacaoSuspeita.TransacaoSelecionada = false;
            transacaoSuspeita.TransacaoAlterada = false;

            transacaoSuspeita.Bandeira = transacaoEmissor.Bandeira;
            transacaoSuspeita.CodigoEstabelecimento = transacaoEmissor.CodigoEstabelecimento;
            transacaoSuspeita.CodigoMCC = transacaoEmissor.CodigoMCC;
            transacaoSuspeita.ComentarioAnalise = transacaoEmissor.ComentarioAnalise;
            transacaoSuspeita.DataAnalise = transacaoEmissor.DataAnalise;
            transacaoSuspeita.DataEnvioAnalise = transacaoEmissor.DataEnvioAnalise;
            transacaoSuspeita.DataTransacao = transacaoEmissor.DataTransacao;
            transacaoSuspeita.DescricaoEntryMode = transacaoEmissor.DescricaoEntryMode;
            transacaoSuspeita.DescricaoMCC = transacaoEmissor.DescricaoMCC;
            transacaoSuspeita.EntryMode = transacaoEmissor.EntryMode;
            transacaoSuspeita.IdentificadorTransacao = transacaoEmissor.IdentificadorTransacao;
            transacaoSuspeita.NomeEstabelecimento = transacaoEmissor.NomeEstabelecimento;
            transacaoSuspeita.NumeroCartao = transacaoEmissor.NumeroCartao;
            transacaoSuspeita.ResultadoAutorizacao = transacaoEmissor.ResultadoAutorizacao;
            transacaoSuspeita.Score = transacaoEmissor.Score;
            transacaoSuspeita.SituacaoBloqueio = transacaoEmissor.SituacaoBloqueio;
            transacaoSuspeita.SituacaoTransacao = transacaoEmissor.SituacaoTransacao;
            transacaoSuspeita.TipoAlarme = transacaoEmissor.TipoAlarme;
            transacaoSuspeita.TipoCartao = transacaoEmissor.TipoCartao;
            transacaoSuspeita.TipoResposta = transacaoEmissor.TipoResposta;
            transacaoSuspeita.UnidadeFederacao = transacaoEmissor.UnidadeFederacao;
            transacaoSuspeita.UsuarioAnalise = transacaoEmissor.UsuarioAnalise;
            transacaoSuspeita.Valor = transacaoEmissor.Valor;

            return transacaoSuspeita;
        }
        /// <summary>
        /// Este método é utilizado para definir se os parâmetros de sistema estão carregados.
        /// </summary>
        /// <returns></returns>
        protected override bool CarregarParametrosSistema()
        {
            return true;
        }
        #endregion

        public void Exportar()
        {
            try
            {
                if (this.TransacoesSuspeitas == null || this.TransacoesSuspeitas.Count == 0)
                {
                    base.ExibirMensagem("Não foram encontrados dados para a exportação.");
                    return;
                }

                using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
                {
                    string[] campos = new string[] { "Tipo de alarme", "Tipo resposta", "Data/hora da transação", "Valor", "Score", "Aut.", "Estabelecimento", "MCC", "UF", "C/D", "EM", "Usuário", "Data/hora da análise" };

                    ExportadorHelper.GerarExcel(ExportadorHelper.ObterAnalisaTransacoesSuspeitasPorCartaoRelatorio(this.TransacoesSuspeitas), Response, campos);
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }

        protected override Control GetControleOndeColocarPaginacao()
        {
            return null;
        }

        private void RegistrarJSAbandono()
        {
            QueryStringSegura query = new QueryStringSegura();
            query.Add("identificadorTransacao", this.IdentificadorTransacao.ToString());

            
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), "abandonoPagina",
                    @"<script type=""text/javascript"">$(window).unload(function(){
                    $.ajax({
                        type: 'GET',
                        url: 'pn_AnalisaTransacoesSuspeitasPorCartao.aspx?release=1&dados={0}',
                        async:false,
                        data: {}
                                });
                            });</script>".Replace("{0}", query.ToString()));

        }

        private void DesbloquearAnaliseCartao()
        {
            using (Servico.FMS.FMSClient client = new Servico.FMS.FMSClient())
            {
                DesbloquearCartaoEnvio requisicao = new DesbloquearCartaoEnvio()
                {
                    GrupoEntidade = SessaoAtual.GrupoEntidade,
                    NumeroEmissor = SessaoAtual.CodigoEntidade,
                    UsuarioLogin = SessaoAtual.LoginUsuario,
                    IdentificadorTransacao = this.IdentificadorTransacao
                };

                client.DesbloquearCartao(requisicao);
            }

        }

        private void ObterValoresNavegacao()
        {
            string dadosQS = Request.QueryString["dados"];

            if (!string.IsNullOrEmpty(dadosQS))
            {
                QueryStringSegura query = new QueryStringSegura(dadosQS);

                if (query["identificadorTransacao"] != null)
                    this.IdentificadorTransacao = Int64.Parse(query["identificadorTransacao"]);

                if (query["numeroEstabelecimento"] != null)
                    this.NumeroEstabelecimento = Int64.Parse(query["numeroEstabelecimento"]);
                else
                    this.NumeroEstabelecimento = 0;
                
            }
        }

       
    }
}
