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
using System.Collections.Specialized;

namespace Redecard.PN.DataCash
{
    public partial class GerVendasRelatorioTransacoes : PageBaseDataCash
    {
        #region Propriedades / Variáveis
        
        private static Negocio.Gerenciamento gerenciamento = new Negocio.Gerenciamento();

        protected global::Redecard.PN.DataCash.controles.comprovantes.QuadroAcoes ucAcoes;
        protected global::Redecard.PN.DataCash.controles.Paginacao paginacao;

        #endregion

        #region Eventos da Página

        /// <summary>
        /// Carrega a página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    this.CarregarComboBandeiras();
                    this.CarregarComboTipoTransacao();

                    if (Request.QueryString["dados"] != null)
                    {
                        QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);

                        String dataInicio = queryString["DataInicio"];
                        String dataFinal = queryString["DataFinal"];
                        Boolean statusAprovada = (queryString["Aprovada"] ?? String.Empty).CompareTo("S") == 0; //se false: Não Aprovada

                        if (!(String.IsNullOrEmpty(dataInicio) || String.IsNullOrEmpty(dataFinal)))
                        {
                            txtPeriodoInicio.Text = dataInicio;
                            txtPeriodoFinal.Text = dataFinal;

                            ddlStatus.ClearSelection();

                            if (statusAprovada)
                                ddlStatus.Items.FindByValue(EStatus.Aprovada.ToString()).Selected = true;
                            else
                                ddlStatus.Items.FindByValue(EStatus.NaoAprovada.ToString()).Selected = true;                           

                            Buscar(1);
                        }
                    }
                }
                catch (PortalRedecardException ex)
                {
                    base.ExibirPainelExcecao(ex);
                    Logger.GravarErro("Erro durante Page_Load", ex);                    
                }
                catch (Exception ex)
                {
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    Logger.GravarErro("Erro durante Page_Load", ex);                    
                }
            }
        }

        /// <summary>
        /// Em caso de postback da página e pelo menos um item na tabela, monta a paginação.
        /// </summary>
        protected void Page_PreRender(object sender, EventArgs e)
        {            
            if (rptTransacoes.Items.Count > 0)
                paginacao.CarregaPaginacao();
        }

        #endregion

        #region Carregamento de Combos

        /// <summary>
        /// Carrega o ComboBox bandeiras
        /// </summary>
        private void CarregarComboBandeiras()
        {            
            // Obtem as bandeiras disponíveis
            Negocio.Configuracao configuracoes = new Negocio.Configuracao();
            Dictionary<Int32, String> bandeiras = configuracoes.ObtemBandeirasFiltro();

            // Preenche combo
            foreach (KeyValuePair<Int32, String> bandeira in bandeiras)
                ddlBandeiras.Items.Add(new ListItem(bandeira.Value, bandeira.Key.ToString()));

            // ARE - Conforme e-mail do Vinicius Mano não será verificado as bandeiras no filtro do relatório -- 09102014

            //Se PV possui serviço de Bandeiras Adicionais, inclui também bandeiras AMEX e ELO
            //Int32 pv = this.SessaoAtual.CodigoEntidade;
            //Boolean bandeirasAdicionais = configuracoes.VerificaBandeirasAdicionais(pv);
            //if (bandeirasAdicionais)
            //{
            //    ddlBandeiras.Items.Add(new ListItem("American Express", "American Express"));
            //    ddlBandeiras.Items.Add(new ListItem("ELO", "ELO"));
            //}    
        }

        /// <summary>
        /// Carrega o ComboBox tipo de transação
        /// </summary>
        private void CarregarComboTipoTransacao()
        {
            //Carrega os itens na combo Tipo Transação
            var itens = new Dictionary<ETipoTransacao, String>();
            itens.Add(ETipoTransacao.Todos, "Todos");
            itens.Add(ETipoTransacao.Credito, "Crédito");
            itens.Add(ETipoTransacao.Debito, "Débito");
            itens.Add(ETipoTransacao.Boleto, "Boleto");

            this.CarregarCombo(ddlTipoTransacao, itens, ETipoTransacao.Todos);

            //Carrega os combos relacionados/dependentes
            this.CarregarComboModalidade();
            this.CarregarComboFormaPagamento();
            this.CarregarComboStatus();
        }

        /// <summary>
        /// Carrega o ComboBox modalidade de acordo com o tipo de transação
        /// </summary>
        private void CarregarComboModalidade()
        {            
            //Carrega os itens na combo Modalidade
            var itens = new Dictionary<EModalidade, String>();
            switch (ValorSelecionado<ETipoTransacao>(ddlTipoTransacao, ETipoTransacao.Todos))
            {
                case ETipoTransacao.Credito:
                    ddlModalidade.Enabled = true;
                    itens.Add(EModalidade.Todas, "Todas");
                    itens.Add(EModalidade.SomenteCredito, "Somente crédito");
                    itens.Add(EModalidade.AVS, "Com AVS");
                    itens.Add(EModalidade.Com3ds, "Com 3DS");
                    itens.Add(EModalidade.IATA, "IATA");
                    itens.Add(EModalidade.PreAutorizacao, "Pré-Autorização");
                    itens.Add(EModalidade.Agendado, "Recorrente Agendado");
                    itens.Add(EModalidade.Historico, "Recorrente por Histórico");
                    break;
                case ETipoTransacao.Debito:
                    ddlModalidade.Enabled = false;
                    itens.Add(EModalidade.Com3ds, "Com 3DS");
                    break;
                case ETipoTransacao.Boleto:
                    ddlModalidade.Enabled = false;
                    itens.Add(EModalidade.Todas, "Todas");
                    break;                                    
                case ETipoTransacao.Todos:                
                    ddlModalidade.Enabled = true;
                    itens.Add(EModalidade.Todas, "Todas");
                    itens.Add(EModalidade.SomenteCredito, "Somente crédito");
                    itens.Add(EModalidade.AVS, "Com AVS");
                    itens.Add(EModalidade.Com3ds, "Com 3DS");
                    itens.Add(EModalidade.IATA, "IATA");
                    itens.Add(EModalidade.PreAutorizacao, "Pré-Autorização");
                    itens.Add(EModalidade.Agendado, "Recorrente Agendado");
                    itens.Add(EModalidade.Historico, "Recorrente por Histórico");
                    break;
                default:
                    break;
            }

            this.CarregarCombo(ddlModalidade, itens, EModalidade.Todas);
        }

        /// <summary>
        /// Carrega o ComboBox forma de pagamento de acordo com o tipo de transação.
        /// </summary>
        private void CarregarComboFormaPagamento()
        {
            //Carrega os itens na combo Forma Pagamento
            var itens = new Dictionary<EFormaPagamento, String>();
            
            switch (ValorSelecionado<ETipoTransacao>(ddlTipoTransacao, ETipoTransacao.Todos))
            {                
                case ETipoTransacao.Credito:
                    ddlFormaPagamento.Enabled = true;

                    itens.Add(EFormaPagamento.Todos, "Todos");

                    switch (ValorSelecionado<EModalidade>(ddlModalidade, EModalidade.Todas))
                    {
                        case EModalidade.IATA:
                        case EModalidade.PreAutorizacao:
                            itens.Add(EFormaPagamento.Avista, "À Vista");
                            itens.Add(EFormaPagamento.ParceladoEstabelecimento, "Parcelado Estabelecimento");
                            break;
                        case EModalidade.Historico:
                        case EModalidade.Agendado:
                            itens.Add(EFormaPagamento.Avista, "À Vista");
                            break;
                        default:
                            itens.Add(EFormaPagamento.Avista, "À Vista");
                            itens.Add(EFormaPagamento.ParceladoEstabelecimento, "Parcelado Estabelecimento");
                            itens.Add(EFormaPagamento.ParceladoEmissor, "Parcelado Emissor");
                            break;
                    }                    
                break;
                case ETipoTransacao.Debito:
                case ETipoTransacao.Boleto:
                    ddlFormaPagamento.Enabled = false;
                    itens.Add(EFormaPagamento.Todos, "Todos");
                    break;
                case ETipoTransacao.Todos:
                default:
                    ddlFormaPagamento.Enabled = true;
                    itens.Add(EFormaPagamento.Todos, "Todos");
                    switch (ValorSelecionado<EModalidade>(ddlModalidade, EModalidade.Todas))
                    {
                        case EModalidade.IATA:
                        case EModalidade.PreAutorizacao:
                            itens.Add(EFormaPagamento.Avista, "À Vista");
                            itens.Add(EFormaPagamento.ParceladoEstabelecimento, "Parcelado Estabelecimento");
                            break;
                        case EModalidade.Historico:
                        case EModalidade.Agendado:
                            itens.Add(EFormaPagamento.Avista, "À Vista");
                            break;
                        default:
                            itens.Add(EFormaPagamento.Avista, "À Vista");
                            itens.Add(EFormaPagamento.ParceladoEstabelecimento, "Parcelado Estabelecimento");
                            itens.Add(EFormaPagamento.ParceladoEmissor, "Parcelado Emissor");
                            break;
                    }     
                    break;
            }

            this.CarregarCombo(ddlFormaPagamento, itens, EFormaPagamento.Todos);
        }

        /// <summary>
        /// Carrega o ComboBox status de acordo com o tipo de transação
        /// </summary>
        private void CarregarComboStatus()
        {
            //Carrega os itens na combo Status
            var itens = new Dictionary<EStatus, String>();
            switch (ValorSelecionado<ETipoTransacao>(ddlTipoTransacao, ETipoTransacao.Todos))
            {
                case ETipoTransacao.Credito:
                case ETipoTransacao.Debito:
                    itens.Add(EStatus.Todas, "Todas");
                    itens.Add(EStatus.Aprovada, "Aprovada");
                    itens.Add(EStatus.Estornada, "Estornada");
                    itens.Add(EStatus.Pendente, "Pendente");
                    itens.Add(EStatus.PendenteHPS, "Pendente HPS");
                    if(ValorSelecionado<EModalidade>(ddlModalidade, EModalidade.Todas) == EModalidade.Com3ds
                        || ValorSelecionado<EModalidade>(ddlModalidade, EModalidade.Todas) == EModalidade.Todas)
                        itens.Add(EStatus.Pendente3DS, "Pendente 3DS");
                    itens.Add(EStatus.NaoAprovada, "Não Aprovada");
                    itens.Add(EStatus.RevisaoDeRisco, "Revisão de Risco");
                    break;
                case ETipoTransacao.Boleto:
                    itens.Add(EStatus.Todas, "Todas");
                    itens.Add(EStatus.Pendente, "Pendente");
                    itens.Add(EStatus.ParcPago, "Parc. Pago");
                    itens.Add(EStatus.Pago, "Pago");
                    itens.Add(EStatus.PagoAcimaValor, "Pago acima Valor");
                    break;
                case ETipoTransacao.Todos:
                    itens.Add(EStatus.Todas, "Todas");
                    itens.Add(EStatus.Aprovada, "Aprovada");
                    itens.Add(EStatus.Estornada, "Estornada");
                    itens.Add(EStatus.Pendente, "Pendente");
                    itens.Add(EStatus.PendenteHPS, "Pendente HPS");
                    itens.Add(EStatus.Pendente3DS, "Pendente 3DS");
                    itens.Add(EStatus.NaoAprovada, "Não Aprovada");
                    itens.Add(EStatus.RevisaoDeRisco, "Revisão de Risco");
                    itens.Add(EStatus.ParcPago, "Parc. Pago");
                    itens.Add(EStatus.Pago, "Pago");
                    itens.Add(EStatus.PagoAcimaValor, "Pago acima Valor");
                    break;
                default:
                    break;
            }

            this.CarregarCombo(ddlStatus, itens, EStatus.Todas);
        }

        /// <summary>
        /// Retorna o valor Enum selecionado de uma combo
        /// </summary>
        /// <typeparam name="T">Enum da combo</typeparam>
        /// <param name="dropDownList">DropDownList</param>
        /// <param name="valorDefault">Valor padrão, em caso de valor inválido selecionado</param>
        /// <returns>Valor selecionado</returns>
        private T ValorSelecionado<T>(DropDownList dropDownList, T valorDefault) where T : struct
        {
            T valor = valorDefault;
            Enum.TryParse<T>(dropDownList.SelectedValue, out valor);
            return valor;
        }

        /// <summary>
        /// Recarrega combo, mantendo selecionada a opção previamente selecionada,
        /// caso ainda exista.
        /// </summary>
        private void CarregarCombo<T>(DropDownList dropDown, Dictionary<T, String> valores, T valorPadrao) where T : struct
        {
            var valorSelecionado = ValorSelecionado<T>(dropDown, valorPadrao);
            dropDown.DataSource = valores;
            dropDown.DataBind();
            var item = dropDown.Items.FindByValue(valorSelecionado.ToString());
            if (item != null)
                item.Selected = true;
        }

        protected void ddlModalidade_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CarregarComboFormaPagamento();
            this.CarregarComboStatus();
        }
     
        #endregion

        #region Eventos de Controles para Busca do Relatório

        /// <summary>
        /// Busca as transações de acordo com os filtros selecionados.
        /// </summary>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Relatório de Transações - Busca"))
            {
                Buscar(1);
            }
        }

        /// <summary>
        /// Evento utilizado quando o usuário troca de página.
        /// </summary>
        protected void paginacao_PaginacaoChange(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog(String.Format("Relatório de Transações - Página {0}", paginacao.PaginaAtual)))
            {
                Buscar(paginacao.PaginaAtual);
            }
        }

        /// <summary>
        /// Alteração de quantidade de itens exibidos por página
        /// </summary>
        protected void ddlItemsPorPagina_SelectedIndexChanged(object sender, EventArgs e)
        {
            Buscar(1);
        }

        /// <summary>
        /// Busca as transações de acordo com os filtros selecionados.
        /// </summary>
        protected void Buscar(Int32 pagina)
        {
            using (Logger Log = Logger.IniciarLog("Relatório de transações - Buscar"))
            {
                try
                {
                    paginacao.PaginaAtual = pagina;
                    
                    //Obtém dados dos filtros                
                    ParametrosRelatorioTransacoes parametros =
                        this.ObterFiltro(paginacao.PaginaAtual, ddlItemsPorPagina.SelectedValue.ToInt32());

                    //Verifica se filtros são válidos. Caso inválidos, cancela consulta
                    if (!ValidarFiltros(parametros))
                        return;

                    //Efetua consulta
                    Log.GravarLog(EventoLog.ChamadaNegocio, parametros);
                    RetornoRelatorioTransacoes transacoes = gerenciamento.GetRelatorioTransacoes(parametros);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { transacoes.CodigoRetorno, transacoes.MensagemRetorno });

                    if (transacoes.CodigoRetorno != 1)
                    {
                        paginacao.NumeroPaginas = 0;
                        this.ExibirDadosRelatorio(false);
                        Log.GravarMensagem("Código Retorno != 1", transacoes);
                        base.ExibirPainelExcecao("DataCash.Negocio.Gerenciamento.GetRelatorioTransacoes", transacoes.CodigoRetorno);
                    }
                    else if (transacoes.Transacoes.Count == 0)
                    {
                        Log.GravarMensagem("Sem transações", transacoes);
                        paginacao.NumeroPaginas = 0;
                        this.ExibirDadosRelatorio(false);
                    }
                    else
                    {
                        Log.GravarMensagem("Transações encontradas", new { transacoes.QuantidadePaginas, transacoes.Transacoes.Count });
                        rptTransacoes.DataSource = transacoes.Transacoes;
                        rptTransacoes.DataBind();
                        this.ExibirDadosRelatorio(true);
                        paginacao.NumeroPaginas = transacoes.QuantidadePaginas;
                    }
                }
                catch (PortalRedecardException ex)
                {
                    this.ExibirDadosRelatorio(false);
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }         
                catch (Exception ex)
                {
                    this.ExibirDadosRelatorio(false);
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Valida os filtros
        /// </summary>
        /// <returns>Se filtro válido, retorna TRUE</returns>
        private Boolean ValidarFiltros(ParametrosRelatorioTransacoes filtro)
        {
            Int32 codigoErro = 0;

            Boolean dataInicialOK = filtro.DataInicio.ToString("dd/MM/yyyy").CompareTo("01/01/1800") != 0;
            Boolean dataFinalOK = filtro.DataFinal.ToString("dd/MM/yyyy").CompareTo("01/01/2500") != 0;

            if (dataInicialOK && !dataFinalOK)
                codigoErro = 3; //Data final não informada
            else if (!dataInicialOK && dataFinalOK)
                codigoErro = 4; //Data inicial não informada
            else if (dataFinalOK && dataInicialOK)
            {
                if (filtro.DataInicio > filtro.DataFinal)
                    codigoErro = 1; //Data final menor que data inicial
                else if (filtro.DataFinal.Subtract(filtro.DataInicio).TotalDays > 30)
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
        /// Monta objeto contendo dados do filtro
        /// </summary>
        /// <param name="paginaAtual">Página solicitada</param>
        /// <param name="itensPorPagina">Quantidade de itens por página</param>
        private ParametrosRelatorioTransacoes ObterFiltro(Int32 paginaAtual, Int32 itensPorPagina)
        {
            //this.GeraPainelMensagem("ALERTA",
            //    string.Join(",", this.ddlBandeiras.SelectedItems.Select(b => b.Value).ToArray()));

            return new ParametrosRelatorioTransacoes
            {
                DataInicio = txtPeriodoInicio.Text.ToDate("dd/MM/yyyy", new DateTime(1800, 1, 1)),
                DataFinal = txtPeriodoFinal.Text.ToDate("dd/MM/yyyy", new DateTime(2500, 1, 1)),
                TID = txtTID.Text,
                FormaPagamento = ValorSelecionado<EFormaPagamento>(ddlFormaPagamento, EFormaPagamento.Todos),
                NSU = txtNSU.Text.ToInt64Null(),
                Status = ValorSelecionado<EStatus>(ddlStatus, EStatus.Todas),
                TipoTransacao = ValorSelecionado<ETipoTransacao>(ddlTipoTransacao, ETipoTransacao.Todos),
                NumPDV = SessaoAtual.CodigoEntidade,
                Modalidade = ValorSelecionado<EModalidade>(ddlModalidade, EModalidade.Todas),
                numerodapagina = paginaAtual,
                linhasporpagina = itensPorPagina,
                Bandeiras = string.Join(",", this.ddlBandeiras.SelectedItems.Select(b => b.Value).ToArray())
            };
        }

        /// <summary>
        /// Oculta ou exibe os controles do relatório
        /// </summary>
        private void ExibirDadosRelatorio(Boolean exibir)
        {
            rptTransacoes.Visible = exibir;
            paginacao.Visible = exibir;
            pnlItemsPorPagina.Visible = exibir;
            ucAviso.Visible = !exibir;
            tbLegendas.Visible = exibir;
        }

        #endregion

        #region Eventos de Controles

        /// <summary>
        /// Preenche o repeater de transações.
        /// </summary>
        protected void rptTransacoes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    var item = e.Item.DataItem as RegistroTransacao;
                    //Preenche dados da transação
                    Literal ltNSU = ((Literal)e.Item.FindControl("ltNSU"));
                    if (String.IsNullOrEmpty(item.NSU) || item.NSU.Trim().CompareTo("0") == 0)
                        ltNSU.Text = "-";
                    else
                        ltNSU.Text = item.NSU;
                    ((Literal)e.Item.FindControl("ltTID")).Text = item.TID.IfNullOrEmpty("-");
                    ((Literal)e.Item.FindControl("ltTipoTransacao")).Text = item.TipoTransacao.IfNullOrEmpty("-");
                    ((Literal)e.Item.FindControl("ltFormaPagamento")).Text = item.FormaPagamento.IfNullOrEmpty("-");
                    ((Literal)e.Item.FindControl("ltDataTransacao")).Text = String.IsNullOrEmpty(item.DataTransacao) ? "-" : item.DataTransacao.ToDate("dd/MM/yyyy").ToString("dd/MM/yy");
                    ((Literal)e.Item.FindControl("ltNomeDistribuidor")).Text = item.NomeDistribuidor.IfNullOrEmpty("-");
                    ((Literal)e.Item.FindControl("ltlBandeira")).Text = item.Bandeira.IfNullOrEmpty("-");
                    ((Literal)e.Item.FindControl("ltValor")).Text = item.Valor.ToString("N2", ptBR);                    
                    ((Literal)e.Item.FindControl("ltParcelas")).Text = item.Parcelas == 0 ? "-" : item.Parcelas.ToString();
                    ((Literal)e.Item.FindControl("ltPedido")).Text = item.NumeroPedido.IfNullOrEmpty("-");
                    ((Literal)e.Item.FindControl("ltStatus")).Text = item.Status.IfNullOrEmpty("-");
                    Image imgExpansao = (Image)e.Item.FindControl("imgExpansao");
                    Image imgLiquidacao = (Image)e.Item.FindControl("imgLiquidacao");
                    var rptBoxes = e.Item.FindControl("rptBoxes") as Repeater;

                    //Verifica qual ícone de liquidação deve ser exibido
                    //Se ELO ou AMEX, é Não liquidada
                    if (String.Compare(item.Bandeira, "American Express", true) == 0 || String.Compare(item.Bandeira, "Elo", true) == 0)
                        imgLiquidacao.ImageUrl = "images/Ico_NAOLiquidada.png";
                    //Caso contrário, é liquidada pela Rede
                    else if (!String.IsNullOrWhiteSpace(item.Bandeira))
                        imgLiquidacao.ImageUrl = "images/Ico_Liquidada.png";
                    else
                        imgLiquidacao.Visible = false;

                    DataSet dadosBoxes = this.MontarDadosBoxes(item);
                    DataTable[] dados = dadosBoxes.Tables.Cast<DataTable>().Where(dt => dt.Rows.Count > 0).ToArray();

                    //Se não possui Boxes a serem exibidos, oculta botão de expansão
                    if (dados.Length == 0)
                        imgExpansao.Visible = false;
                    else
                    {
                        rptBoxes.DataSource = dados;
                        rptBoxes.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante bind de dados na tabela", ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }                        
        }

        /// <summary>
        /// Preenche o repeater de Boxes de cada transação
        /// </summary>
        protected void rptBoxes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dtDadosBox = e.Item.DataItem as DataTable;
                var lblNomeBox = e.Item.FindControl("lblNomeBox") as Literal;
                var rptDadosBox = e.Item.FindControl("rptDadosBox") as Repeater;

                lblNomeBox.Text = dtDadosBox.TableName;
                rptDadosBox.DataSource = dtDadosBox.Rows;
                rptDadosBox.DataBind();
            }
        }

        /// <summary>
        /// Preenche os boxes do repeater de boxes de cada transação
        /// </summary>
        protected void rptDadosBox_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataRow = e.Item.DataItem as DataRow;
                var lbNome = e.Item.FindControl("lbNome") as Literal;
                var ltDescricao = e.Item.FindControl("ltDescricao") as Literal;
                lbNome.Text = Convert.ToString(dataRow["Nome"]);
                ltDescricao.Text = Convert.ToString(dataRow["Descricao"]).IfNullOrEmpty("-");
            }
        }
        
        /// <summary>
        /// Monta os dados dos boxes de uma transação
        /// </summary>
        private DataSet MontarDadosBoxes(RegistroTransacao registro)
        {            
            //Instancia os Dicionários para pré-preparação dos dados dos boxes
            var dadosVenda = new OrderedDictionary();
            var dadosCartao = new OrderedDictionary();            
            var dadosPassageiro = new OrderedDictionary();
            var dadosBoleto = new OrderedDictionary();
            var dadosEnderecoCobranca = new OrderedDictionary();

            //Variável auxiliar para centralização do nome dos labels dos boxes
            var labels = new
            {
                Venda = new {
                    HoraDaVenda = "Hora da Venda",
                    NroAutorizacao = "Nº da Autorização",
                    Programa = "Programa",
                    NroConta = "Nº da Conta",
                    Score = "Score",
                    ValorParcelas = "Valor das Parcelas",
                    Encargos = "Encargos",
                    ValorTotalPagar = "Valor Total a Pagar",
                    TaxaEmbarque = "Taxa de Embarque",
                    CodigoIATA = "Código IATA",
                    MotivoNegativa = "Motivo da Negativa"
                },
                Cartao = new {
                    NroCartao = "Nº Cartão (Últimos 4 dig.)"
                },
                Passageiro = new {
                    Passageiro = "Passageiro {0}",
                    NroBilhete = "Nº do Bilhete {0}"
                },
                Boleto = new {
                    Emissor = "Emissor",
                    DataEmissao = "Data da Emissão",
                    DataVencimento = "Data de Vencimento",
                    DataPagamento = "Data do Pagamento",
                    MultaAtraso = "Multa de Atraso",
                    JurosDia = "Juros por dia"
                },
                EnderecoCobranca = new {
                    CEP = "CEP",
                    Endereco = "Endereço",
                    Numero = "Número",
                    Complemento = "Complemento",
                    Cidade = "Cidade",
                    Pais = "País"
                }
            };

            //Montagem do conteúdo dos boxes

            //Status: Não Aprovada
            if (registro.Status.CompareTo("Não Aprovada") == 0)
            {
                dadosVenda[labels.Venda.MotivoNegativa] = registro.MotivoDaNegativa;
            }
            else
            {                
                //Tipo Transação: Crédito
                if (registro.TipoTransacao.CompareTo("Crédito") == 0)
                {
                    //Modalidade: Com 3DS
                    if (!String.IsNullOrEmpty(registro.ModalidadeCom3ds))
                    {
                        dadosVenda[labels.Venda.NroConta] = registro.NumeroConta;
                        dadosVenda[labels.Venda.HoraDaVenda] = registro.HoraVenda;
                        dadosVenda[labels.Venda.NroAutorizacao] = registro.NumeroAutorizacao;
                        dadosVenda[labels.Venda.Programa] = registro.ProgramaTransacao;

                        dadosCartao[labels.Cartao.NroCartao] = registro.NumeroCartao.PadLeft(4, ' ').Right(4);
                    }

                    //Modalidade: Recorrente por Histórico
                    if (!String.IsNullOrEmpty(registro.ModalidadeHistorico))
                    {
                        dadosVenda[labels.Venda.NroConta] = registro.NumeroConta;
                        dadosVenda[labels.Venda.HoraDaVenda] = registro.HoraVenda;
                        dadosVenda[labels.Venda.NroAutorizacao] = registro.NumeroAutorizacao;
                        dadosVenda[labels.Venda.Score] = registro.Score;

                        dadosCartao[labels.Cartao.NroCartao] = registro.NumeroCartao.PadLeft(4, ' ').Right(4);
                    }

                    //Modalidade: Somente Crédito | AVS | Pré-Autorização | Recorrente Agendado
                    if (!String.IsNullOrEmpty(registro.ModalidadeCredito)
                        || !String.IsNullOrEmpty(registro.ModalidadeComAvs)
                        || !String.IsNullOrEmpty(registro.ModalidadePre)
                        || !String.IsNullOrEmpty(registro.ModalidadeAgendado))
                    {
                        dadosVenda[labels.Venda.HoraDaVenda] = registro.HoraVenda;
                        dadosVenda[labels.Venda.NroAutorizacao] = registro.NumeroAutorizacao;

                        dadosCartao[labels.Cartao.NroCartao] = registro.NumeroCartao.PadLeft(4, ' ').Right(4);
                    }

                    //Modalidade: Somente Crédito e com AVS, 
                    //Forma de Pagamento: Parcelado Emissor
                    if((!String.IsNullOrEmpty(registro.ModalidadeCredito)
                        || !String.IsNullOrEmpty(registro.ModalidadeComAvs))
                        && registro.FormaPagamento.IfNullOrEmpty(String.Empty).CompareTo("Parcelado Emissor") == 0)
                    {
                        dadosVenda[labels.Venda.HoraDaVenda] = registro.HoraVenda;
                        dadosVenda[labels.Venda.NroAutorizacao] = registro.NumeroAutorizacao;
                        dadosVenda[labels.Venda.ValorParcelas] = registro.ValorDasParcelas.ToString("C2", ptBR);
                        dadosVenda[labels.Venda.Encargos] = registro.Encargos.ToString("N0", ptBR);
                        dadosVenda[labels.Venda.ValorTotalPagar] = registro.ValorTotalAPagarParcelas.ToString("C2", ptBR);

                        dadosCartao[labels.Cartao.NroCartao] = registro.NumeroCartao.PadLeft(4, ' ').Right(4);
                    }

                    //Modalidade: com 3DS
                    //Forma de Pagamento: Parcelado Emissor
                    if (!String.IsNullOrEmpty(registro.ModalidadeCom3ds)
                        && registro.FormaPagamento.IfNullOrEmpty(String.Empty).CompareTo("Parcelado Emissor") == 0)
                    {
                        dadosVenda[labels.Venda.HoraDaVenda] = registro.HoraVenda;
                        dadosVenda[labels.Venda.NroAutorizacao] = registro.NumeroAutorizacao;
                        dadosVenda[labels.Venda.ValorParcelas] = registro.ValorDasParcelas.ToString("C2", ptBR);
                        dadosVenda[labels.Venda.Encargos] = registro.Encargos.ToString("N0", ptBR);
                        dadosVenda[labels.Venda.ValorTotalPagar] = registro.ValorTotalAPagarParcelas.ToString("C2", ptBR);
                        dadosVenda[labels.Venda.Programa] = registro.ProgramaTransacao;

                        dadosCartao[labels.Cartao.NroCartao] = registro.NumeroCartao.PadLeft(4, ' ').Right(4);
                    }

                    //Modalidade: IATA
                    if (!String.IsNullOrEmpty(registro.ModalidadeIATA))
                    {
                        dadosVenda[labels.Venda.HoraDaVenda] = registro.HoraVenda;
                        dadosVenda[labels.Venda.NroAutorizacao] = registro.NumeroAutorizacao;
                        dadosVenda[labels.Venda.TaxaEmbarque] = registro.TaxaEmbarque.ToString("N2", ptBR);
                        dadosVenda[labels.Venda.ValorTotalPagar] = registro.ValorTotalAPagarIATA.ToString("N2", ptBR);
                        dadosVenda[labels.Venda.CodigoIATA] = registro.CodigoIATA;
                        dadosVenda[labels.Venda.Score] = registro.Score;

                        dadosCartao[labels.Cartao.NroCartao] = registro.NumeroCartao.PadLeft(4, ' ').Right(4);

                        List<Passageiro> passageiros = registro.Passageiros;
                        for (Int32 iPassageiro = 1; iPassageiro <= passageiros.Count; iPassageiro++)
                        {
                            Passageiro passageiro = passageiros[iPassageiro - 1];
                            dadosPassageiro[String.Format(labels.Passageiro.Passageiro, iPassageiro)] = passageiro.NomePassagerio;
                            dadosPassageiro[String.Format(labels.Passageiro.NroBilhete, iPassageiro)] = 
                                String.Concat(passageiro.NumeroBilhete, iPassageiro == registro.Passageiros.Count ? String.Empty : "<br/><br/>");
                        }
                    }
                }
                //Tipo Transação: Débito
                else if (registro.TipoTransacao.CompareTo("Débito") == 0)
                {
                    dadosVenda[labels.Venda.HoraDaVenda] = registro.HoraVenda;
                    dadosVenda[labels.Venda.NroAutorizacao] = registro.NumeroAutorizacao;
                    dadosVenda[labels.Venda.Programa] = registro.ProgramaTransacao;

                    dadosCartao[labels.Cartao.NroCartao] = registro.NumeroCartao;
                }   
                //Tipo Transação: Boleto
                else if (registro.TipoTransacao.CompareTo("Boleto") == 0)
                {                    
                    dadosBoleto[labels.Boleto.Emissor] = registro.BoletoEmissor;
                    dadosBoleto[labels.Boleto.DataEmissao] = registro.BoletoDataEmissao;
                    dadosBoleto[labels.Boleto.DataVencimento] = registro.BoletoDataVencimento;
                    dadosBoleto[labels.Boleto.DataPagamento] = registro.BoletoDataPagamento;
                    dadosBoleto[labels.Boleto.MultaAtraso] = registro.BoletoMultaDeAtraso.ToString("C2", ptBR);
                    dadosBoleto[labels.Boleto.JurosDia] = String.Concat(registro.BoletoJurosPorDia.ToString("N2", ptBR), "%");

                    dadosEnderecoCobranca[labels.EnderecoCobranca.CEP] = registro.BoletoCEP;
                    dadosEnderecoCobranca[labels.EnderecoCobranca.Endereco] = registro.BoletoEndereco;
                    dadosEnderecoCobranca[labels.EnderecoCobranca.Complemento] = registro.BoletoComplementoEndereco;
                    dadosEnderecoCobranca[labels.EnderecoCobranca.Cidade] = registro.BoletoCidade;
                    dadosEnderecoCobranca[labels.EnderecoCobranca.Pais] = registro.BoletoPais;
                }
            }

            //Preparação dos DataSets para retorno das informações

            //DataSet, contendo coleção de DataTables, onde cada DataTable representa um Box
            DataSet ds = new DataSet("Boxes");

            //DataTable template auxiliar
            DataTable dtTemplate = new DataTable();
            dtTemplate.Columns.Add("Nome");
            dtTemplate.Columns.Add("Descricao");

            //Instancia os DataTables para alimentação dos dados preenchidos nos boxes
            DataTable dtVenda, dtCartao, dtPassageiro, dtBoleto, dtEnderecoCobranca;
            (dtVenda = dtTemplate.Clone()).TableName = "Dados da Venda";
            (dtCartao = dtTemplate.Clone()).TableName = "Dados do Cartão";
            (dtPassageiro = dtTemplate.Clone()).TableName = "Dados do Passageiro";
            (dtBoleto = dtTemplate.Clone()).TableName = "Dados do Boleto";
            (dtEnderecoCobranca = dtTemplate.Clone()).TableName = "Dados Endereço Cobrança";
            ds.Tables.AddRange(new[] { dtVenda, dtCartao, dtPassageiro, dtBoleto, dtEnderecoCobranca });

            //Transfere as informações dos dicionários para os DataSets
            this.PreencherDataTable(dadosVenda, ref dtVenda);
            this.PreencherDataTable(dadosCartao, ref dtCartao);
            this.PreencherDataTable(dadosPassageiro, ref dtPassageiro);
            this.PreencherDataTable(dadosBoleto, ref dtBoleto);
            this.PreencherDataTable(dadosEnderecoCobranca, ref dtEnderecoCobranca);

            return ds;
        }

        private void PreencherDataTable(OrderedDictionary dados, ref DataTable dt)
        {
            foreach (String chave in dados.Keys)
                dt.Rows.Add(chave, dados[chave]);
        }

        /// <summary>
        /// Muda o ComboBox forma de pagamento ao selecionar um tipo de transação
        /// </summary>
        protected void ddlTipoTransacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CarregarComboFormaPagamento();
            this.CarregarComboModalidade();
            this.CarregarComboStatus();
        }

        #endregion

        #region Métodos Autilixares

        /// <summary>
        /// Consultar todos os registros do relatório
        /// </summary>
        private List<RegistroTransacao> ConsultarRelatorioCompleto()
        {
            using (Logger Log = Logger.IniciarLog("Exportação do Relatório de Transações"))
            {
                try
                {
                    //Faz paginação de 40 em 40 itens                        
                    ParametrosRelatorioTransacoes filtro = this.ObterFiltro(1, 40);
                    RetornoRelatorioTransacoes paginaRelatorio = null;
                    var registros = new List<RegistroTransacao>();

                    Log.GravarMensagem("Parâmetros da busca", filtro);

                    //Consulta todas as páginas do relatório
                    do
                    {
                        paginaRelatorio = gerenciamento.GetRelatorioTransacoes(filtro);
                        if (paginaRelatorio.CodigoRetorno != 1)
                        {
                            base.ExibirPainelExcecao("DataCash.Negocio.Gerenciamento.GetRelatorioTransacoes", paginaRelatorio.CodigoRetorno);
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

        /// <summary>
        /// Obtenção dos dados da tabela para geração do PDF/Excel. 
        /// Realiza consulta completa do relatório, com todas as páginas
        /// </summary>
        /// <returns>Dados da tabela</returns>
        protected TabelaExportacao ucAcoes_ObterTabelaExportacao()
        {
            return new TabelaExportacao
            {
                Titulo = String.Format("Relatório de Transações - {0} ({1})",
                    SessaoAtual.NomeEntidade, SessaoAtual.CodigoEntidade),
                Colunas = new[] 
                { 
                    new Coluna("NSU"), 
                    new Coluna("TID"), 
                    new Coluna("Tipo de Transação"), 
                    new Coluna("Forma de Pagamento"), 
                    new Coluna("Data da Transação"),
                    new Coluna("Nome do Distribuidor"), 
                    new Coluna("Bandeira"),
                    new Coluna("Valor (R$)"), 
                    new Coluna("Nº Parc."), 
                    new Coluna("Nº Pedido"), 
                    new Coluna("Status"),
                    new Coluna(String.Empty).SetLargura(Convert.ToSingle(0.2))
                },
                FuncaoValores = (registro) =>
                {
                    RegistroTransacao item = registro as RegistroTransacao;

                    var imgLiquidada = default(System.Drawing.Image);
                    if (!String.IsNullOrWhiteSpace(item.Bandeira))
                    {
                        Boolean liquidadaRede = String.Compare(item.Bandeira, "Amex", true) != 0
                            && String.Compare(item.Bandeira, "ELO", true) != 0;
                        String urlLiquidada = liquidadaRede ? "images/Ico_Liquidada.png" : "images/Ico_NAOLiquidada.png";
                        imgLiquidada = System.Drawing.Image.FromFile(Server.MapPath(urlLiquidada));
                    }

                    return new Object[] {
                        (String.IsNullOrEmpty(item.NSU) || item.NSU.Trim().CompareTo("0") == 0) ? "-" : item.NSU,
                        item.TID.IfNullOrEmpty("-"),
                        item.TipoTransacao.IfNullOrEmpty("-"),
                        item.FormaPagamento.IfNullOrEmpty("-"),
                        String.IsNullOrEmpty(item.DataTransacao) ?
                            "-" : item.DataTransacao.ToDate("dd/MM/yyyy").ToString("dd/MM/yy"),
                        item.NomeDistribuidor.IfNullOrEmpty("-"),
                        item.Bandeira.IfNullOrEmpty("-"),
                        item.Valor.ToString("N2", ptBR),
                        item.Parcelas == 0 ? "-" : item.Parcelas.ToString(),
                        item.NumeroPedido.IfNullOrEmpty("-"),
                        item.Status.IfNullOrEmpty("-"),
                        imgLiquidada
                    };
                },
                Registros = ConsultarRelatorioCompleto()
            };
        }

        #endregion
    }
}