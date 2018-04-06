using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Redecard.PN.DataCash.Negocio;
using Redecard.PN.Comum;
using System.Data;
using Redecard.PN.DataCash.BasePage;
using System.Globalization;
using Redecard.PN.DataCash.controles.comprovantes;
using Redecard.PN.DataCash.Modelos;
using System.ComponentModel;
using TipoDeTransacao = Redecard.PN.DataCash.Modelo.TipoTransacaoRecorrente;

namespace Redecard.PN.DataCash
{
    public partial class GerVendasTransacoesRecorrentes : PageBaseDataCash
    {
        #region [ Propriedades / Variáveis ]

        private static Negocio.Gerenciamento gerenciamento = new Negocio.Gerenciamento();

        protected global::Redecard.PN.DataCash.controles.comprovantes.QuadroAcoes ucAcoes;
        protected global::Redecard.PN.DataCash.controles.Paginacao paginacao;

        private List<RegistroTransacaoFireAndForget> TransacoesAgendado
        {
            get { return Util.DeserializarDado<List<RegistroTransacaoFireAndForget>>((byte[])Session["Transacoes"]); }
            set { Session["Transacoes"] = Util.SerializarDado(value); }
        }

        private List<RegistroTransacaoHistoricRecurring> TransacoesHistorico
        {
            get { return Util.DeserializarDado<List<RegistroTransacaoHistoricRecurring>>((byte[])Session["Transacoes"]); }
            set { Session["Transacoes"] = Util.SerializarDado(value); }
        }

        private TipoDeTransacao? TipoTransacao
        {
            get { return Util.DeserializarDado<TipoDeTransacao?>((byte[])Session["TipoTransacao"]); }
            set { Session["TipoTransacao"] = Util.SerializarDado(value); }
        }

        private List<RegistroTransacaoFireAndForget> TransacoesAgendadoSelecionadas
        {
            get { return Util.DeserializarDado<List<RegistroTransacaoFireAndForget>>((byte[])Session["TransacoesSelecionadas"]); }
            set { Session["TransacoesSelecionadas"] = Util.SerializarDado(value); }
        }

        private List<RegistroTransacaoHistoricRecurring> TransacoesHistoricoSelecionadas
        {
            get { return Util.DeserializarDado<List<RegistroTransacaoHistoricRecurring>>((byte[])Session["TransacoesSelecionadas"]); }
            set { Session["TransacoesSelecionadas"] = Util.SerializarDado(value); }
        }

        /// <summary>
        /// Filtro utilizado na última consulta:
        ///     - ParametrosRelatorioHistoricRecurring   
        ///     - ParametrosRelatorioFireAndForget
        /// </summary>
        private Object FiltroUltimaConsulta
        {
            get 
            { 
                if(this.TipoTransacao.Value == TipoDeTransacao.Agendado)
                    return Util.DeserializarDado<ParametrosRelatorioFireAndForget>((byte[])Session["FiltroUltimaConsulta"]); 
                else
                    return Util.DeserializarDado<ParametrosRelatorioHistoricRecurring>((byte[])Session["FiltroUltimaConsulta"]); 
            }
            set 
            { 
                if(value is ParametrosRelatorioFireAndForget)
                    Session["FiltroUltimaConsulta"] = Util.SerializarDado((ParametrosRelatorioFireAndForget)value); 
                else if(value is ParametrosRelatorioHistoricRecurring)
                    Session["FiltroUltimaConsulta"] = Util.SerializarDado((ParametrosRelatorioHistoricRecurring)value); 
            }
        }

        #endregion

        #region [ Eventos da Página ]

        /// <summary>
        /// Carrega a página
        /// </summary>        
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Page_Load - Faça sua Venda"))
            {
                try
                {

                    if (!IsPostBack)
                    {
                        this.CarregarComboStatus();
                        this.CarregarComboTipoTransacao();

                        if (!String.IsNullOrEmpty(Request["dados"]))
                        {
                            var qs = new QueryStringSegura(Request["dados"]);
                            if ("true".CompareTo(qs["BuscarRelatorio"]) == 0)
                                Buscar(default(Int32), this.FiltroUltimaConsulta);
                        }
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Redireciona para a tela de Relatório de Transações Recorrentes, 
        /// trazendo os dados da última consulta
        /// </summary>
        /// <param name="context">HttpContext</param>
        public static void RedirecionarParaRelatorio(HttpContext context)
        {
            //Gera QueryString para que no Page_Load o relatório seja carregado automaticamente
            //de acordo com a última consulta realizada
            var qs = new QueryStringSegura();
            qs.Add("BuscarRelatorio", "true");

            String url = String.Concat("GerVendasTransacoesRecorrentes.aspx?dados=", qs.ToString());
            context.Response.Redirect(url, false);
            context.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Em caso de postback da página e pelo menos um item na tabela, monta a paginação.
        /// </summary>        
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (rptTransacoesAgendado.Items.Count > 0 || rptTransacoesHistorico.Items.Count > 0)
                paginacao.CarregaPaginacao();
        }

        #endregion

        #region [ Carregamento Inicial de Combos ]

        /// <summary>
        /// Carrega o ComboBox tipo de transação
        /// </summary>
        private void CarregarComboTipoTransacao()
        {
            //Carrega os itens na combo Tipo Transação
            var itens = new Dictionary<TipoDeTransacao, String>();
            itens.Add(TipoDeTransacao.Agendado, "Agendado");
            itens.Add(TipoDeTransacao.Historico, "Histórico");

            this.ddlTipoTransacao.DataSource = itens;
            this.ddlTipoTransacao.DataBind();
        }

        /// <summary>
        /// Carrega o ComboBox status de acordo com o tipo de transação
        /// </summary>
        private void CarregarComboStatus()
        {
            //Carrega os itens na combo Status
            var itens = new Dictionary<EStatusTransacaoRecorrente, String>();
            itens.Add(EStatusTransacaoRecorrente.Todas, "Todas");
            itens.Add(EStatusTransacaoRecorrente.Ativa, "Ativa");
            itens.Add(EStatusTransacaoRecorrente.Cancelada, "Cancelada");

            ddlStatus.DataSource = itens;
            ddlStatus.DataBind();
        }

        /// <summary>
        /// Obtém o valor enum Tipado selecionado de uma DropDownList
        /// </summary>
        private T ValorSelecionado<T>(DropDownList dropDownList) where T : struct
        {
            return (T)Enum.Parse(typeof(T), dropDownList.SelectedValue);
        }

        #endregion

        #region [ Eventos Controles ]

        /// <summary>
        /// Busca as transações de acordo com os filtros selecionados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Transações Recorrentes - Busca"))
            {
                Buscar();
            }
        }

        protected void ddlItemsPorPagina_SelectedIndexChanged(object sender, EventArgs e)
        {
            Buscar(1);
        }

        /// <summary>
        /// Evento utilizado quando o usuário troca de página.
        /// </summary>
        protected void paginacao_PaginacaoChange(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog(String.Format("Transações Recorrentes - Página {0}", paginacao.PaginaAtual)))
            {
                Buscar(paginacao.PaginaAtual);
            }
        }

        protected void btnCancelarAgendamento_Click(object sender, EventArgs e)
        {
            this.TipoTransacao = TipoDeTransacao.Agendado;
            this.TransacoesAgendadoSelecionadas = this.ObterTransacoesSelecionadas<RegistroTransacaoFireAndForget>();
            base.Redirecionar("GerVendasTransacoesRecorrentesConfirmacao.aspx");
        }

        protected void btnCancelarConta_Click(object sender, EventArgs e)
        {
            this.TipoTransacao = TipoDeTransacao.Historico;
            this.TransacoesHistoricoSelecionadas = this.ObterTransacoesSelecionadas<RegistroTransacaoHistoricRecurring>();
            base.Redirecionar("GerVendasTransacoesRecorrentesConfirmacao.aspx");
        }

        protected void btnNovaAutorizacao_Click(object sender, EventArgs e)
        {
            this.TransacoesHistoricoSelecionadas = this.ObterTransacoesSelecionadas<RegistroTransacaoHistoricRecurring>();
            base.Redirecionar("GerVendasTransacoesRecorrentesNAConfirmar.aspx");
        }
      
        #endregion

        #region [ Métodos Auxiliares ]

        /// <summary>
        /// Busca as transações de acordo com os filtros selecionados.
        /// </summary>
        private void Buscar(Int32 pagina = 1, Object filtro = null)
        {            
            using (Logger Log = Logger.IniciarLog("Relatório de Transações Recorrentes - Buscar"))
            {
                try
                {
                    //Por padrão, oculta controles de exibição do relatório
                    ExibirDadosRelatorios(false, false);

                    if (filtro is ParametrosRelatorioFireAndForget)
                    {
                        var filtroAgendado = filtro as ParametrosRelatorioFireAndForget;
                        this.CarregarFiltroAgendado(filtroAgendado);
                        pagina = filtroAgendado.numerodapagina;                        
                    }
                    else if (filtro is ParametrosRelatorioHistoricRecurring)
                    {
                        var filtroHistorico = filtro as ParametrosRelatorioHistoricRecurring;
                        this.CarregarFiltroHistorico(filtroHistorico);
                        pagina = filtroHistorico.numerodapagina;
                    }

                    TipoDeTransacao tipoTransacao = TipoTransacaoFiltro();
                    Int32 tamanhoPagina = ddlItemsPorPagina.SelectedValue.ToInt32();
                    paginacao.PaginaAtual = pagina;

                    if (tipoTransacao == TipoDeTransacao.Agendado)
                    {
                        //Obtém dados do filtro para Transações Recorrentes - Agendado
                        ParametrosRelatorioFireAndForget parametros = this.ObterFiltroAgendado(pagina, tamanhoPagina);

                        //Verifica se filtros são válidos. Caso inválidos, cancela consulta
                        if (!ValidarPeriodo(parametros.DataInicio, parametros.DataFinal))
                            return;

                        this.FiltroUltimaConsulta = parametros;

                        //Efetua a consulta de Transações Recorrentes - Agendado
                        RetornoRelatorioFireAndForget retorno = gerenciamento.GetRelatorioFireAndForget(parametros);

                        if (retorno.CodigoRetorno != 1)
                        {
                            Log.GravarMensagem("Código Retorno != 1", retorno);
                            base.ExibirPainelExcecao("DataCash.Negocio.Gerenciamento.GetRelatorioFireAndForget", 
                                retorno.CodigoRetorno);
                            ExibirDadosRelatorios(false, false);
                        }
                        else if (retorno.CodigoRetorno == 1 && retorno.Transacoes != null && retorno.Transacoes.Count > 0)
                        {
                            this.TransacoesAgendado = retorno.Transacoes;
                            rptTransacoesAgendado.DataSource = retorno.Transacoes;
                            rptTransacoesAgendado.DataBind();
                            paginacao.NumeroPaginas = retorno.QuantidadePaginas;
                            ExibirDadosRelatorios(true, false);
                        }
                        else if (retorno.Transacoes.Count == 0)
                        {
                            ExibirDadosRelatorios(false, false);
                        }
                    }
                    else if (tipoTransacao == TipoDeTransacao.Historico)
                    {
                        //Obtém dados do filtro para Transações Recorrentes - Histórico
                        ParametrosRelatorioHistoricRecurring parametros = this.ObterFiltroHistorico(pagina, tamanhoPagina);

                        //Verifica se filtros são válidos. Caso inválidos, cancela consulta
                        if (!ValidarPeriodo(parametros.DataInicio, parametros.DataFinal))
                            return;

                        this.FiltroUltimaConsulta = parametros;

                        //Efetua a consulta de Transações Recorrentes - Histórico
                        RetornoRelatorioHistoricRecurring retorno = gerenciamento.GetRelatorioHistoricRecurring(parametros);

                        if (retorno.CodigoRetorno != 1)
                        {
                            Log.GravarMensagem("Código Retorno != 1", retorno);
                            base.ExibirPainelExcecao("DataCash.Negocio.Gerenciamento.GetRelatorioHistoricRecurring", 
                                retorno.CodigoRetorno);
                            ExibirDadosRelatorios(false, false);
                        }
                        else if (retorno.CodigoRetorno == 1 && retorno.Transacoes != null && retorno.Transacoes.Count > 0)
                        {
                            this.TransacoesHistorico = retorno.Transacoes;
                            rptTransacoesHistorico.DataSource = retorno.Transacoes;
                            rptTransacoesHistorico.DataBind();
                            paginacao.NumeroPaginas = retorno.QuantidadePaginas;
                            ExibirDadosRelatorios(false, true);
                        }
                        else if (retorno.Transacoes.Count == 0)
                        {
                            ExibirDadosRelatorios(false, false);
                        }
                    }                                     
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Valida os filtros
        /// </summary>
        /// <returns>Se filtro válido, retorna TRUE</returns>
        private Boolean ValidarPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            Int32 codigoErro = 0;

            Boolean dataInicialOK = dataInicial.ToString("dd/MM/yyyy").CompareTo("01/01/1800") != 0;
            Boolean dataFinalOK = dataFinal.ToString("dd/MM/yyyy").CompareTo("01/01/2500") != 0;

            if (dataInicialOK && !dataFinalOK)
                codigoErro = 3; //Data final não informada
            else if (!dataInicialOK && dataFinalOK)
                codigoErro = 4; //Data inicial não informada
            else if (dataFinalOK && dataInicialOK)
            {
                if (dataInicial > dataFinal)
                    codigoErro = 1; //Data final menor que data inicial
                else if (dataFinal.Subtract(dataInicial).TotalDays > 30)
                    codigoErro = 2; //Período maior que 30 dias
            }

            //Exibe mensagem de validação caso necessário
            if (codigoErro != 0)
            {
                base.ExibirPainelExcecao("DataCash.GerVendasRelatorios", codigoErro);
                return false;
            }
            else //filtro OK
                return true;
        }
        /// <summary>
        /// Exibe ou oculta controles dos relatórios
        /// </summary>
        /// <param name="exibirAgendado">Exibe ou não controles relacionados à Transações Recorrentes - Agendado</param>
        /// <param name="exibirHistorico">Exibe ou não controles relacionados à Transações Recorrentes - Histórico</param>
        private void ExibirDadosRelatorios(Boolean exibirAgendado, Boolean exibirHistorico)
        {
            pnlItemsPorPagina.Visible = exibirAgendado || exibirHistorico;
            rptTransacoesHistorico.Visible = exibirHistorico;
            paginacao.Visible = exibirHistorico || exibirAgendado;
            pnlDescricaoRelatorio.Visible = exibirHistorico || exibirAgendado;
            lblDescricaoAgendado.Visible = exibirAgendado;
            lblDescricaoHistorico.Visible = exibirHistorico;
            btnCancelarConta.Visible = exibirHistorico;
            btnNovaAutorizacao.Visible = exibirHistorico;
            rptTransacoesAgendado.Visible = exibirAgendado;
            btnCancelarAgendamento.Visible = exibirAgendado;
            ucAviso.Visible = !exibirAgendado && !exibirHistorico;
        }

        /// <summary>
        /// Monta objeto contendo dados do filtro do relatório de Transações Recorrentes - Agendado
        /// </summary>
        /// <param name="paginaAtual">Página solicitada</param>
        /// <param name="itensPorPagina">Quantidade de itens por página</param>
        /// <returns></returns>
        private ParametrosRelatorioFireAndForget ObterFiltroAgendado(Int32 paginaAtual, Int32 itensPorPagina)
        {
            return new ParametrosRelatorioFireAndForget
            {
                DataInicio = txtPeriodoInicio.Text.ToDate("dd/MM/yyyy", new DateTime(1800, 1, 1)),
                DataFinal = txtPeriodoFinal.Text.ToDate("dd/MM/yyyy", new DateTime(2500, 1, 1)),
                TID = txtTID.Text,
                StatusTransacaoRecorrente = ValorSelecionado<EStatusTransacaoRecorrente>(ddlStatus),
                NumPDV = SessaoAtual.CodigoEntidade,
                numerodapagina = paginaAtual,
                linhasporpagina = itensPorPagina
            };
        }

        /// <summary>
        /// Carrega os controles do filtro com os parâmetros existentes
        /// </summary>
        private void CarregarFiltroAgendado(ParametrosRelatorioFireAndForget filtro)
        {
            if(filtro.DataInicio.CompareTo(new DateTime(1800, 1, 1)) != 0)
                txtPeriodoInicio.Text = filtro.DataInicio.ToString("dd/MM/yyyy");
            if (filtro.DataFinal.CompareTo(new DateTime(2500, 1, 1)) != 0)
            txtPeriodoFinal.Text = filtro.DataFinal.ToString("dd/MM/yyyy");
            txtTID.Text = filtro.TID;
            ddlTipoTransacao.Items.FindByValue(TipoDeTransacao.Agendado.ToString()).Selected = true;
            ddlStatus.Items.FindByValue(filtro.StatusTransacaoRecorrente.ToString()).Selected = true;
            ddlItemsPorPagina.Items.FindByValue(filtro.linhasporpagina.ToString()).Selected = true;
        }

        /// <summary>
        /// Carrega os controles do filtro com os parâmetros existentes
        /// </summary>
        private void CarregarFiltroHistorico(ParametrosRelatorioHistoricRecurring filtro)
        {
            if (filtro.DataInicio.CompareTo(new DateTime(1800, 1, 1)) != 0)
                txtPeriodoInicio.Text = filtro.DataInicio.ToString("dd/MM/yyyy");
            if (filtro.DataFinal.CompareTo(new DateTime(2500, 1, 1)) != 0)
                txtPeriodoFinal.Text = filtro.DataFinal.ToString("dd/MM/yyyy");
            txtNumeroConta.Text = filtro.NumeroConta;
            ddlTipoTransacao.Items.FindByValue(TipoDeTransacao.Historico.ToString()).Selected = true;
            ddlStatus.Items.FindByValue(filtro.StatusTransacaoRecorrente.ToString()).Selected = true;
            ddlItemsPorPagina.Items.FindByValue(filtro.linhasporpagina.ToString()).Selected = true;
        }

        /// <summary>
        /// Monta objeto contendo dados do filtro do relatório de Transações Recorrentes - Histórico
        /// </summary>
        /// <param name="paginaAtual">Página solicitada</param>
        /// <param name="itensPorPagina">Quantidade de itens por página</param>
        /// <returns></returns>
        private ParametrosRelatorioHistoricRecurring ObterFiltroHistorico(Int32 paginaAtual, Int32 itensPorPagina)
        {
            return new ParametrosRelatorioHistoricRecurring
            {
                DataInicio = txtPeriodoInicio.Text.ToDate("dd/MM/yyyy", new DateTime(1800, 1, 1)),
                DataFinal = txtPeriodoFinal.Text.ToDate("dd/MM/yyyy", new DateTime(2500, 1, 1)),
                StatusTransacaoRecorrente = ValorSelecionado<EStatusTransacaoRecorrente>(ddlStatus),
                NumeroConta = txtNumeroConta.Text,
                NumPDV = SessaoAtual.CodigoEntidade,
                numerodapagina = paginaAtual,
                linhasporpagina = itensPorPagina
            };
        }

        /// <summary>
        /// Retorna o tipo de transação selecionado
        /// </summary>
        private TipoDeTransacao TipoTransacaoFiltro()
        {
            return ValorSelecionado<TipoDeTransacao>(ddlTipoTransacao);
        }

        /// <summary>
        /// Recupera as linhas selecionadas do repeater
        /// </summary>
        private List<T> ObterTransacoesSelecionadas<T>()
        {
            Repeater repeater = null;
            List<T> transacoes = null;

            if (typeof(T) == typeof(RegistroTransacaoFireAndForget))
            {
                repeater = rptTransacoesAgendado;
                transacoes = this.TransacoesAgendado.Cast<T>().ToList();
            }
            else if (typeof(T) == typeof(RegistroTransacaoHistoricRecurring))
            {
                repeater = rptTransacoesHistorico;
                transacoes = this.TransacoesHistorico.Cast<T>().ToList();
            }
            
            List<T> transacoesSelecionadas = new List<T>();

            if (repeater != null)
            {
                foreach (RepeaterItem item in repeater.Items)
                {
                    var ckbSelecao = (CheckBox)item.FindControl("ckbSelecao");
                    if (ckbSelecao.Checked)
                        transacoesSelecionadas.Add(transacoes.ElementAt(item.ItemIndex));
                }
            }

            return transacoesSelecionadas;
        }

        #endregion

        #region [ Bind dos Repeaters dos Relatórios ]

        /// <summary>
        /// Preenche o repeater de Transações Recorrentes - Agendado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptTransacoesAgendado_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = e.Item.DataItem as RegistroTransacaoFireAndForget;

                //Preenche dados da transação
                ((Literal)e.Item.FindControl("lblTID")).Text = item.TID.IfNullOrEmpty("-");
                ((Literal)e.Item.FindControl("lblDataVenda")).Text = item.DataVenda.ToString("dd/MM/yy");
                ((Literal)e.Item.FindControl("lblValorRecorrencia")).Text = item.ValorRecorente.ToString("N2", ptBR);
                ((Literal)e.Item.FindControl("lblFrequencia")).Text = item.Frequencia.IfNullOrEmpty("-");
                ((Literal)e.Item.FindControl("lblDataInicio")).Text = item.DataInicio.ToString("dd/MM/yy");
                ((Literal)e.Item.FindControl("lblQtdeRecorrencia")).Text = item.QuantidadeRecorrencias.ToString();
                ((Literal)e.Item.FindControl("lblNumeroCartao")).Text = item.NumeroCartao.IfNullOrEmpty("-").PadLeft(4, ' ').Right(4);
                ((Literal)e.Item.FindControl("lblNumeroPedido")).Text = item.NumeroPedido.IfNullOrEmpty("-");
                ((Literal)e.Item.FindControl("lblStatus")).Text = item.Status.IfNullOrEmpty("-");

                //Dados do box Dados do Cartão
                ((Literal)e.Item.FindControl("lblBandeira")).Text = item.Bandeira.IfNullOrEmpty("-");
                        
                //Dados do box Dados da Recorrência
                ((Literal)e.Item.FindControl("lblValorUltimaCobranca")).Text = item.ValorUltimaCobranca.ToString("C2", ptBR);
                ((Literal)e.Item.FindControl("lblDataUltimaCobranca")).Text = item.DataUltimaCobranca.ToString("dd/MM/yy");

                var ckbSelecao = (CheckBox)e.Item.FindControl("ckbSelecao");
                var imgExpansao = (Image)e.Item.FindControl("imgExpansao");

                ckbSelecao.Visible = item.Status.IfNullOrEmpty(String.Empty).CompareTo("Ativa") == 0;
            }
        }

        protected void rptTransacoesHistorico_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = e.Item.DataItem as RegistroTransacaoHistoricRecurring;

                //Preenche dados da transação
                ((Literal)e.Item.FindControl("lblNumeroConta")).Text = item.NumeroConta.IfNullOrEmpty("-");
                ((Literal)e.Item.FindControl("lblTID")).Text = item.TID.IfNullOrEmpty("-");                
                ((Literal)e.Item.FindControl("lblDataVenda")).Text = item.DataVenda.ToString("dd/MM/yy");
                ((Literal)e.Item.FindControl("lblValorVenda")).Text = item.ValorVenda.ToString("N2", ptBR);
                ((Literal)e.Item.FindControl("lblBandeira")).Text = item.Bandeira.IfNullOrEmpty("-");
                ((Literal)e.Item.FindControl("lblNumeroCartao")).Text = item.NumeroCartao.IfNullOrEmpty("-").PadLeft(4, ' ').Right(4);
                ((Literal)e.Item.FindControl("lblStatus")).Text = item.Status.IfNullOrEmpty("-");
                
                var ckbSelecao = (CheckBox)e.Item.FindControl("ckbSelecao");
                ckbSelecao.Visible = item.Status.IfNullOrEmpty(String.Empty).CompareTo("Ativa") == 0;
            }
        }

        #endregion

        #region [ Exportação Excel/PDF ]

        /// <summary>Consultar todos os registros do Relatório de Transações Recorrentes - Agendado</summary>        
        private List<RegistroTransacaoFireAndForget> ConsultarRecorrentesAgendadoCompleto()
        {
            using (Logger Log = Logger.IniciarLog("Relatório de Transações Recorrentes - Agendado - Exportação"))
            {
                try
                {
                    //Faz paginação de 40 em 40 itens                        
                    ParametrosRelatorioFireAndForget filtro = this.ObterFiltroAgendado(1, 40);
                    RetornoRelatorioFireAndForget paginaRelatorio = null;
                    var registros = new List<RegistroTransacaoFireAndForget>();

                    Log.GravarMensagem("Parâmetros da busca", filtro);

                    //Consulta todas as páginas do relatório
                    do
                    {
                        paginaRelatorio = gerenciamento.GetRelatorioFireAndForget(filtro);
                        if (paginaRelatorio.CodigoRetorno != 1)
                        {
                            base.ExibirPainelExcecao("DataCash.Negocio.Gerenciamento.GetRelatorioFireAndForget", 
                                paginaRelatorio.CodigoRetorno);
                            return registros;
                        }
                        else if (paginaRelatorio.Transacoes != null && paginaRelatorio.Transacoes.Count > 0)
                            registros.AddRange(paginaRelatorio.Transacoes);
                        filtro.numerodapagina++;
                    }
                    while (paginaRelatorio != null
                        && paginaRelatorio.CodigoRetorno == 1
                        && paginaRelatorio.Transacoes != null
                        && paginaRelatorio.Transacoes.Count > 0
                        && paginaRelatorio.QuantidadePaginas > filtro.numerodapagina);

                    return registros;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                    return null;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return null;
                }
            }
        }

        /// <summary>Consultar todos os registros do Relatório de Transações Recorrentes - Histórico</summary>        
        private List<RegistroTransacaoHistoricRecurring> ConsultarRecorrentesHistoricoCompleto()
        {
            using (Logger Log = Logger.IniciarLog("Relatório de Transações Recorrentes - Histórico - Exportação"))
            {
                try
                {
                    //Faz paginação de 40 em 40 itens                        
                    ParametrosRelatorioHistoricRecurring filtro = this.ObterFiltroHistorico(1, 40);
                    RetornoRelatorioHistoricRecurring paginaRelatorio = null;
                    var registros = new List<RegistroTransacaoHistoricRecurring>();

                    Log.GravarMensagem("Parâmetros da busca", filtro);

                    //Consulta todas as páginas do relatório
                    do
                    {
                        paginaRelatorio = gerenciamento.GetRelatorioHistoricRecurring(filtro);
                        if (paginaRelatorio.CodigoRetorno != 1)
                        {
                            base.ExibirPainelExcecao("DataCash.Negocio.Gerenciamento.GetRelatorioHistoricRecurring", 
                                paginaRelatorio.CodigoRetorno);
                            return registros;
                        }
                        else if (paginaRelatorio.Transacoes != null && paginaRelatorio.Transacoes.Count > 0)
                            registros.AddRange(paginaRelatorio.Transacoes);
                        filtro.numerodapagina++;
                    }
                    while (paginaRelatorio != null
                        && paginaRelatorio.CodigoRetorno == 1
                        && paginaRelatorio.Transacoes != null
                        && paginaRelatorio.Transacoes.Count > 0
                        && paginaRelatorio.QuantidadePaginas > filtro.numerodapagina);

                    return registros;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                    return null;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return null;
                }
            }
        }

        /// <summary>Obtenção dos dados da tabela para geração do PDF/Excel. 
        /// Realiza consulta completa do relatório, com todas as páginas</summary>
        /// <returns>Dados da tabela</returns>
        protected TabelaExportacao ucAcoes_ObterTabelaExportacao()
        {
            TipoDeTransacao tipoTransacao = TipoTransacaoFiltro();

            if (tipoTransacao == TipoDeTransacao.Agendado)
            {
                return new TabelaExportacao
                {
                    Titulo = String.Format("Transações Recorrentes - Agendado - {0} ({1})",
                        SessaoAtual.NomeEntidade, SessaoAtual.CodigoEntidade),
                    Colunas = new []
                { 
                    new Coluna("TID"), 
                    new Coluna("Data da Venda"),
                    new Coluna("Valor da Recorrência (R$)"),
                    new Coluna("Frequência"),
                    new Coluna("Data de início"), 
                    new Coluna("Qtde recorr."), 
                    new Coluna("Nº Cartão (Últimos 4 dig.)"),
                    new Coluna("Nº do Pedido"), 
                    new Coluna("Status")
                },
                    FuncaoValores = (registro) =>
                    {
                        var item = registro as RegistroTransacaoFireAndForget;
                        return new [] {
                            item.TID.IfNullOrEmpty("-"),
                            item.DataVenda.ToString("dd/MM/yy"),
                            item.ValorRecorente.ToString("N2", ptBR),
                            item.Frequencia.IfNullOrEmpty("-"),
                            item.DataInicio.ToString("dd/MM/yy"),
                            item.QuantidadeRecorrencias.ToString(),
                            item.NumeroCartao.IfNullOrEmpty("-").PadLeft(4, ' ').Right(4),
                            item.NumeroPedido.IfNullOrEmpty("-"),
                            item.Status.IfNullOrEmpty("-")
                        };
                    },
                    Registros = ConsultarRecorrentesAgendadoCompleto()
                };
            }
            else if (tipoTransacao == TipoDeTransacao.Historico)
            {
                return new TabelaExportacao
                {
                    Titulo = String.Format("Transações Recorrentes - Histórico - {0} ({1})",
                        SessaoAtual.NomeEntidade, SessaoAtual.CodigoEntidade),
                    Colunas = new [] 
                    { 
                        new Coluna("Nº da Conta"), 
                        new Coluna("TID"), 
                        new Coluna("Data da Venda"), 
                        new Coluna("Valor da Venda (R$)"), 
                        new Coluna("Bandeira"),
                        new Coluna("Nº Cartão (Últimos 4 dig.)"), 
                        new Coluna("Status")
                    },
                    FuncaoValores = (registro) =>
                    {
                        var item = registro as RegistroTransacaoHistoricRecurring;
                        return new[] {
                            item.NumeroConta.IfNullOrEmpty("-"),
                            item.TID.IfNullOrEmpty("-"),
                            item.DataVenda.ToString("dd/MM/yy"),
                            item.ValorVenda.ToString("N2", ptBR),
                            item.Bandeira.IfNullOrEmpty("-"),
                            item.NumeroCartao.IfNullOrEmpty("-").PadLeft(4, ' ').Right(4),
                            item.Status.IfNullOrEmpty("-")
                        };
                    },
                    Registros = ConsultarRecorrentesHistoricoCompleto()
                };
            }
            else //Condição inválida
                throw new ApplicationException();
        }

        #endregion
    }
}