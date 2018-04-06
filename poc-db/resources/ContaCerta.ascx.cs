/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.OutrosServicos.SharePoint.EntidadeServico;
using Redecard.PN.OutrosServicos.SharePoint.ZPPlanoContasServico;

namespace Redecard.PN.OutrosServicos.SharePoint.ControlTemplates.OutrosServicos.Ofertas
{
    public partial class ContaCerta : UserControlBase
    {
        #region Propriedades/Atributos/Variáveis/Controles

        /// <summary>Nome do serviço utilizado pela tela</summary>
        private const String NomeServico = "ZPPlanoContasServico";

        /// <summary>CultureInfo pt-BR</summary>
        private CultureInfo ptBR = CultureInfo.CreateSpecificCulture("pt-BR");

        /// <summary>Data Início selecionada no Filtro</summary>
        private DateTime FiltroDataInicio
        {
            get { return txtDataInicio.Text.ToDate("dd/MM/yyyy", DateTime.MinValue); }
        }

        /// <summary>Data Fim selecionada no Filtro</summary>
        private DateTime FiltroDataFim
        {
            get { return txtDataFim.Text.ToDate("dd/MM/yyyy", DateTime.MinValue); }
        }

        /// <summary>Verifica se o PV é uma matriz ou filial</summary>
        private Boolean FiltroAcessoMatriz
        {
            get
            {
                if (ViewState["FiltroAcessoMatriz"] == null)
                    ViewState["FiltroAcessoMatriz"] = ConsultarAcessoMatriz();

                return (Boolean)ViewState["FiltroAcessoMatriz"];
            }
        }

        /// <summary>Tipo de Consulta da Oferta selecionado no filtro</summary>
        private TipoEstabelecimento FiltroTipoEstabelecimento
        {
            get
            {
                //Se está logado com PV Filial (não for matriz) ou for Matriz acessando como Filial,
                //tipo estabelecimento é Filial
                if (this.SessaoAtual.AcessoFilial || !this.FiltroAcessoMatriz)
                    return TipoEstabelecimento.Filial;
                //Cso contrário, é matriz
                else
                    return TipoEstabelecimento.Matriz;
            }
        }

        /// <summary>
        /// Número do PV utilizado para consulta
        /// </summary>
        private Int32 FiltroNumeroPV { get { return this.SessaoAtual.CodigoEntidade; } }

        /// <summary>"Cache" local (ViewState) da busca de ofertas (removido sempre que buscar é acionado)</summary>
        private Oferta[] Ofertas
        {
            get { return (Oferta[])ViewState["Ofertas"]; }
            set { ViewState["Ofertas"] = value; }
        }

        /// <summary>Oferta selecionada (ViewState)</summary>
        private Oferta OfertaSelecionada
        {
            get { return (Oferta)ViewState["OfertaSelecionada"]; }
            set { ViewState["OfertaSelecionada"] = value; }
        }

        /// <summary>Faturamentos de Oferta para todos os anos referentes</summary>
        private Faturamento[] FaturamentosOferta { get; set; }

        #endregion

        #region Métodos Públicos 

        /// <summary>
        /// Carregamento inicial do controle
        /// </summary>
        public void CarregarControle()
        {
            //Configurações da página
            this.ValidarPermissao = false;
            this.Page.Form.DefaultButton = btnBuscar.UniqueID;

            if (!IsPostBack)
            {
                //Valor inicial dos campos de período
                txtDataInicio.Text = DateTime.Today.ToString("dd/MM/yyyy");
                txtDataFim.Text = DateTime.Today.ToString("dd/MM/yyyy");
            }
        }

        #endregion

        #region Eventos Páginas / Botões

        /// <summary>Botão Buscar (filtro)</summary>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Buscando ofertas"))
            {
                Log.GravarMensagem("Dados da busca", new
                {
                    this.FiltroDataFim,
                    this.FiltroDataInicio,
                    this.FiltroNumeroPV,
                    this.FiltroTipoEstabelecimento,
                    this.FiltroAcessoMatriz
                });

                Oferta[] ofertas = ConsultarUltimasOfertas();

                if (ofertas != null && ofertas.Length > 0)
                {
                    //Se tem oferta, habilita visualização de ofertas
                    mvOfertas.SetActiveView(vwOfertas);

                    //ASH 10/09/13: comentado, pois na última especificação NÃO EXISTE MAIS a funcionalidade
                    //pnlVerContrato.Visible = true;

                    //Carrega os detalhes da primeira oferta
                    Oferta oferta = ofertas[0];
                    CarregarOferta(oferta);

                    //Carrega as abas das ofertas
                    CarregarUltimasOfertas(ofertas);
                }
                else //não existem ofertas
                {
                    mvOfertas.SetActiveView(vwSemOfertas);
                    (qaSemOfertas as QuadroAviso).CarregarMensagem();
                    pnlVerContrato.Visible = false;
                }
            }
        }

        /// <summary>Aba de Oferta selecionada</summary>
        protected void btnOferta_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Consultando oferta selecionada"))
            {
                try
                {
                    //Recupera dados para identificação da oferta correspondende do botão
                    LinkButton btnOferta = sender as LinkButton;
                    Int32 codigoOferta = Criptografia.Decrypt(btnOferta.Attributes["CodigoOferta"]).ToInt32(0);
                    Decimal codigoProposta = Criptografia.Decrypt(btnOferta.Attributes["CodigoProposta"]).ToDecimal();

                    Log.GravarMensagem("Dados da oferta selecionada", new { codigoOferta, codigoProposta });

                    //Identifica a oferta correspondente a partir das ofertas já consultadas anteriormente
                    Oferta ofertaSelecionada = this.Ofertas
                        .FirstOrDefault(oferta => oferta.CodigoOferta == codigoOferta
                            && oferta.CodigoProposta == codigoProposta);

                    //Carrega a oferta selecionada
                    CarregarOferta(ofertaSelecionada);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>Visualização de contrato</summary>
        protected void btnVerContrato_Click(object sender, EventArgs e)
        {
            // TODO ASH: implementar lógica de recuperação/visualização/geração de contrato
            // Regras abaixo foram extraídas do documento de especificação:
            // 1. O contrato aparecerá somente se houver alguma oferta contratada vigente no 
            //    período em que o usuário acessar o item de menu “Serviços > Ofertas”.
            // 2. O contrato deverá ser disponibilizado em arquivo PDF e deve ser a última 
            //    oferta vigente contratada.
            // 3. Para a fase 1 será contemplado somente 1 contrato para todos os PV’s 
            //    independente da oferta visualizada em tela.
            base.ExibirPainelMensagem("TODO");
        }

        #endregion

        #region Carregamento de Controles

        /// <summary>Carrega repeater de abas de ofertas</summary>
        private void CarregarUltimasOfertas(Oferta[] ofertas)
        {
            rptOfertas.DataSource = ofertas;
            rptOfertas.DataBind();
        }

        /// <summary>Carrega a oferta na tela</summary>
        /// <param name="oferta">Oferta a ser carregada</param>
        private void CarregarOferta(Oferta oferta)
        {
            //Guarda a oferta selecionada na ViewState
            this.OfertaSelecionada = oferta;

            //Preenche informações básicas da Oferta
            lblDataAceite.Text = oferta.DataAdesao.ToString("dd/MM/yyyy");
            lblInicioVigencia.Text = oferta.PeriodoInicialVigencia.ToString("dd/MM/yyyy");
            lblFimVigencia.Text = oferta.PeriodoFinalVigencia.ToString("dd/MM/yyyy");
            lblCodigoOferta.Text = oferta.CodigoOferta.ToString();
            lblAgencia.Text = oferta.AgenciaAdesao == 0 ? "-" : oferta.AgenciaAdesao.ToString();
            lblConta.Text = String.IsNullOrEmpty(oferta.ContaAdesao) ? "-" : oferta.ContaAdesao;
            lblDescricaoOferta.Text = oferta.DescricaoOferta;

            //Consulta e carrega as Metas da Oferta
            MetaOferta[] metasOferta = ConsultarMetasOferta(oferta.CodigoOferta, oferta.CodigoProposta);
            if (metasOferta != null && metasOferta.Length > 0)
            {
                rptMetas.DataSource = metasOferta;
                rptMetas.DataBind();
                pnlMetas.Visible = true;
            }
            else //não existem metas
            {
                pnlMetas.Visible = false;
            }

            //Consulta os faturamentos (armazena retorno em variável, para re-utilização no bind do repeater de anos)
            //Lógica criada para evitar N-chamadas de consulta de faturamento durante bind do repeater de anos
            this.FaturamentosOferta = ConsultarFaturamentos(oferta.CodigoOferta, oferta.CodigoProposta);

            //Identifica e carrega o repeater com os anos referência/número PV 
            //(do mais recente para menos recente, e alfabeticamente por PV)
            //criando array de classe anônima { (Int16) Ano, (Int32) NumeroPV }
            if (this.FaturamentosOferta != null && this.FaturamentosOferta.Length > 0)
            {
                pnlAnos.Visible = true;

                var anosPVs = this.FaturamentosOferta
                    .Select(faturamento => new
                    {
                        Ano = faturamento.AnoReferencia,
                        NumeroPV = faturamento.NumeroPV
                    }).Distinct()
                    .OrderByDescending(anoPV => anoPV.Ano)
                    .ThenBy(anoPV => anoPV.NumeroPV).ToArray();
                rptAnos.DataSource = anosPVs;
                rptAnos.DataBind();
            }
            else
            {
                //não existem faturamentos/anos
                pnlAnos.Visible = false;
            }

            //Recarrega ofertas, para atualização da Aba selecionada
            CarregarUltimasOfertas(this.Ofertas);
        }

        #endregion

        #region Bind de Controles

        protected void rptOfertas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //Recupera controles do repeater de ofertas
                HtmlControl liOferta = e.Item.FindControl("liOferta") as HtmlControl;
                LinkButton btnOferta = e.Item.FindControl("btnOferta") as LinkButton;
                Oferta oferta = e.Item.DataItem as Oferta;

                String cssClass = String.Empty;
                String tituloOferta = String.Empty;

                //Se for atual, título da oferta é "Atual"
                if (oferta.IndicadorOfertaAtual)
                    tituloOferta = "Atual";
                else
                {
                    //Se existir oferta atual (primeira do array), a segunda oferta do array 
                    //this.Ofertas é a Oferta Anterior 1, e assim por diante
                    if (this.Ofertas.Any(oft => oft.IndicadorOfertaAtual))
                        tituloOferta = String.Format("Oferta Anterior {0}", e.Item.ItemIndex);
                    else
                        //Se não possui oferta atual, a primeira oferta do array this.Ofertas
                        //é a Oferta Anterior 1
                        tituloOferta = String.Format("Oferta Anterior {0}", e.Item.ItemIndex + 1);
                }

                //Se a oferta for a oferta atualmente selecionada, aplica estilo de aba ativa
                if (oferta.CodigoProposta == this.OfertaSelecionada.CodigoProposta
                    && oferta.CodigoOferta == this.OfertaSelecionada.CodigoOferta)
                    cssClass = "ativo";
                else
                    cssClass = "desativado";

                //Aplica propriedades nos controles
                liOferta.Attributes["class"] = cssClass;
                btnOferta.Text = tituloOferta;

                //Inclui os campos de identificação da oferta no botão, para posterior recuperação no evento de clique                
                btnOferta.Attributes["CodigoOferta"] = Criptografia.Encrypt(oferta.CodigoOferta.ToString());
                btnOferta.Attributes["CodigoProposta"] = Criptografia.Encrypt(oferta.CodigoProposta.ToString());
            }
        }

        protected void rptMetas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //Recupera controles do repeater de metas
                Literal lblFaturamento = e.Item.FindControl("lblFaturamento") as Literal;
                Literal lblAluguelFaturamento = e.Item.FindControl("lblAluguelFaturamento") as Literal;
                Literal lblDesconto = e.Item.FindControl("lblDesconto") as Literal;
                Literal lblQtdeTerminais = e.Item.FindControl("lblQtdeTerminais") as Literal;
                MetaOferta metaOferta = e.Item.DataItem as MetaOferta;

                //Preenche controles com valores da Meta da Oferta                
                if (metaOferta.ValorInicial == 0)
                    lblFaturamento.Text = String.Format("Até {0}", metaOferta.ValorFinal.ToString("C2", ptBR));
                else if (metaOferta.ValorFinal == 0)
                    lblFaturamento.Text = String.Format("{0} em diante", metaOferta.ValorInicial.ToString("C2", ptBR));
                else
                    lblFaturamento.Text = String.Format("{0} a<br/>{1}",
                        metaOferta.ValorInicial.ToString("C2", ptBR), metaOferta.ValorFinal.ToString("C2", ptBR));

                lblAluguelFaturamento.Text = metaOferta.ValorOferta == 0 ? "-" : metaOferta.ValorOferta.ToString("C2", ptBR);
                lblQtdeTerminais.Text = metaOferta.QuantidadeTerminais == 0 ? "-" : metaOferta.QuantidadeTerminais.ToString();
                lblDesconto.Text = metaOferta.PercentualOferta == 0 ? "-" : (metaOferta.PercentualOferta / 100m).ToString("P0", ptBR);
            }
        }

        protected void rptAnos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //Recupera controles do repeater de anos referência
                Literal lblAno = e.Item.FindControl("lblAno") as Literal;
                Literal lblEstabelecimento = e.Item.FindControl("lblEstabelecimento") as Literal;
                Panel pnlFaturamento = e.Item.FindControl("pnlFaturamento") as Panel;
                Image imgExpandir = e.Item.FindControl("imgExpandir") as Image;
                Image imgOcultar = e.Item.FindControl("imgOcultar") as Image;
                Repeater rptFaturamento = e.Item.FindControl("rptFaturamento") as Repeater;

                //Recupera valores de objeto anônimo no DataItem
                Int16 ano = (Int16)DataBinder.Eval(e.Item.DataItem, "Ano");
                Int32 numeroPV = (Int32)DataBinder.Eval(e.Item.DataItem, "NumeroPV");

                //Preenche controles com valores do ano referência
                lblEstabelecimento.Text = numeroPV.ToString();
                lblAno.Text = ano.ToString();

                //Se for o primeiro ano (mais recente), deixa detalhamento já expandido. Caso contrário, deixa oculto
                if (e.Item.ItemIndex == 0)
                {
                    imgExpandir.Style.Add("display", "none");
                }
                else
                {
                    imgOcultar.Style.Add("display", "none");
                    pnlFaturamento.Style.Add("display", "none");
                }

                //Recupera os dados do faturamento (da variável) do ano referente e atribui no repeater
                rptFaturamento.DataSource = this.FaturamentosOferta
                    .Where(faturamento => faturamento.AnoReferencia == ano).ToArray();
                rptFaturamento.DataBind();
            }
        }

        protected void rptFaturamento_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //Recupera controles do repeater de Faturamento do Ano
                Literal lblMes = e.Item.FindControl("lblMes") as Literal;
                Literal lblFaturamento = e.Item.FindControl("lblFaturamento") as Literal;
                Literal lblAtendeuElegibilidade = e.Item.FindControl("lblAtendeuElegibilidade") as Literal;
                Literal lblValorBeneficio = e.Item.FindControl("lblValorBeneficio") as Literal;
                Literal lblBanco = e.Item.FindControl("lblBanco") as Literal;
                Literal lblAgencia = e.Item.FindControl("lblAgencia") as Literal;
                Literal lblConta = e.Item.FindControl("lblConta") as Literal;
                Faturamento faturamento = e.Item.DataItem as Faturamento;

                //Preenche controles com valores do faturamento para o ano referente
                lblMes.Text = ptBR.TextInfo.ToTitleCase(ptBR.DateTimeFormat.GetMonthName(faturamento.MesReferencia));
                lblFaturamento.Text = faturamento.ValorFaturamento.ToString("C2", ptBR);
                lblAtendeuElegibilidade.Text = ObterDescricao(faturamento.StatusElegibilidade, faturamento.StatusElegibilidadeInt);
                lblValorBeneficio.Text = faturamento.ValorAluguel.ToString("C2", ptBR);
                lblBanco.Text = faturamento.BancoRecebimento == 0 ? "-" : faturamento.BancoRecebimento.ToString();
                lblAgencia.Text = faturamento.AgenciaRecebimento == 0 ? "-" : faturamento.AgenciaRecebimento.ToString();
                lblConta.Text = String.IsNullOrEmpty(faturamento.ContaRecebimento) ? "-" : faturamento.ContaRecebimento;
            }
        }

        /// <summary>
        /// Obtém a descrição do Status da Elegibilidade
        /// </summary>
        /// <param name="statusElegibilidade">StatusElegibilidade</param>
        /// <param name="statusElegibilidadeInt">Inteiro do StatusElegibilidade, caso NAO_IDENTIFICADO</param>
        /// <returns>Descrição do Status da Elegibilidade</returns>
        private String ObterDescricao(StatusElegibilidade statusElegibilidade, Int16 statusElegibilidadeInt)
        {
            switch (statusElegibilidade)
            {
                case StatusElegibilidade.ATINGIU_OS_CRITERIOS_TECNICOS:
                    return "Sim, todos os critérios foram atendidos.";
                case StatusElegibilidade.NAO_ATINGIU_CRITERIO_CONCENTRACAO_DE_VENDAS:
                    return "Não, a concentração de vendas na Redecard não foi atendida.";
                case StatusElegibilidade.NAO_ATINGIU_TRAVA_E_CONCENTRACAO_DE_VENDAS:
                    return "Não, a concentração de vendas na Redecard e a trava de domicílio no Itaú não foram atendidas.";
                case StatusElegibilidade.NAO_POSSUI_TRAVA:
                    return "Não, a trava de domicílio no Itaú não foi atendida.";
                case StatusElegibilidade.NAO_ATINGIU_O_FATURAMENTO_NECESSARIO:
                    return "Não, o faturamento na Redecard não foi atingido.";
                case StatusElegibilidade.NAO_ATINGIU_FATURAMENTO_E_CONCENTRACAO_DE_VENDAS:
                    return "Não, o faturamento e a concentração de vendas na Redecard não foram atendidas.";
                case StatusElegibilidade.NAO_ATINGIU_FATURAMENTO_E_TRAVA:
                    return "Não, o faturamento na Redecard e a trava de domicílio no Itaú não foram atendidas.";
                case StatusElegibilidade.NAO_ATINGIU_FATURAMENTO_TRAVA_E_CONCENTRACAO_VENDAS:
                    return "Não atendeu nenhum dos critérios.";
                case StatusElegibilidade.NAO_IDENTIFICADO:
                default:
                    return statusElegibilidadeInt.ToString();
            }
        }

        #endregion

        #region Consultas

        /// <summary>Consulta ofertas para o período/PV selecionados no filtro</summary>
        private Oferta[] ConsultarUltimasOfertas()
        {
            //Variável de retorno
            Oferta[] ofertas = null;

            using (Logger Log = Logger.IniciarLog("Consulta Ofertas"))
            {
                try
                {
                    //Limpa ViewState de últimas ofertas
                    this.Ofertas = null;

                    //Obtém dados do filtro para consulta
                    Int32 numeroPV = this.FiltroNumeroPV;
                    DateTime dataInicio = this.FiltroDataInicio;
                    DateTime dataFim = this.FiltroDataFim;
                    TipoEstabelecimento tipoEstabelecimento = this.FiltroTipoEstabelecimento;

                    //Validação de parâmetros de entrada

                    //PV não informado
                    if (numeroPV == default(Int32))
                    {
                        base.ExibirPainelExcecao("Redecard.PN.OutrosServicos.PlanoContas", 1);
                        return null;
                    }

                    //Data início não informada
                    if (dataInicio.CompareTo(DateTime.MinValue) == 0)
                    {
                        base.ExibirPainelExcecao("Redecard.PN.OutrosServicos.PlanoContas", 2);
                        return null;
                    }

                    //Data início anterior a Setembro/2013
                    if (dataInicio < "01/09/2013".ToDate("dd/MM/yyyy"))
                    {
                        base.ExibirPainelExcecao("Redecard.PN.OutrosServicos.PlanoContas", 6);
                        return null;
                    }

                    //Data fim não informada
                    if (dataFim.CompareTo(DateTime.MinValue) == 0)
                    {
                        base.ExibirPainelExcecao("Redecard.PN.OutrosServicos.PlanoContas", 3);
                        return null;
                    }

                    //Data início maior que data fim
                    if (dataFim.CompareTo(dataInicio) < 0)
                    {
                        base.ExibirPainelExcecao("Redecard.PN.OutrosServicos.PlanoContas", 4);
                        return null;
                    }

                    //Período maior que 60 dias
                    if (dataFim.Subtract(dataInicio).TotalDays > 60)
                    {
                        base.ExibirPainelExcecao("Redecard.PN.OutrosServicos.PlanoContas", 5);
                        return null;
                    }

                    //Variáveis auxiliares para consulta                    
                    Int16 codigoRetorno = default(Int16);

                    //Consulta as ofertas
                    using (var ctx = new ContextoWCF<HISServicoZP_PlanoContasClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaServico,
                            new { numeroPV, dataInicio, dataFim, tipoEstabelecimento });
                        ofertas = ctx.Cliente.ConsultarOfertas(
                            out codigoRetorno, numeroPV, dataInicio, dataFim, tipoEstabelecimento);
                        Log.GravarLog(EventoLog.RetornoServico, new { codigoRetorno, ofertas });
                    }

                    //Valida código de retorno
                    if (codigoRetorno != 0)
                    {
                        ofertas = null;
                        base.ExibirPainelExcecao(NomeServico, codigoRetorno);
                    }

                    //Limita em 5 a quantidade de oferta disponíveis para visualização
                    if (ofertas != null && ofertas.Length > 0)
                        ofertas = ofertas.Take(5).ToArray();

                    //Coloca em "cache" local da página (ViewState)
                    this.Ofertas = ofertas;
                }
                catch (FaultException<ZPPlanoContasServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return ofertas;
        }

        /// <summary>Consulta metas da oferta</summary>        
        private MetaOferta[] ConsultarMetasOferta(Int32 codigoOferta, Decimal codigoProposta)
        {
            //Variável de retorno
            MetaOferta[] metasOferta = null;

            using (Logger Log = Logger.IniciarLog("Consulta Metas da Oferta"))
            {
                try
                {
                    //Obtém dados do filtro para consulta
                    Int32 numeroPV = this.FiltroNumeroPV;
                    TipoEstabelecimento tipoEstabelecimento = this.FiltroTipoEstabelecimento;

                    //Variáveis auxiliares para consulta                    
                    Int16 codigoRetorno = default(Int16);

                    //Consulta as metas
                    using (var ctx = new ContextoWCF<HISServicoZP_PlanoContasClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaServico,
                            new { codigoOferta, numeroPV, tipoEstabelecimento, codigoProposta });
                        metasOferta = ctx.Cliente.ConsultarMetasOferta(
                            out codigoRetorno, codigoOferta, numeroPV, tipoEstabelecimento, codigoProposta);
                        Log.GravarLog(EventoLog.RetornoServico, new { codigoRetorno, metasOferta });
                    }

                    //Valida código de retorno
                    if (codigoRetorno != 0)
                    {
                        metasOferta = null;
                        base.ExibirPainelExcecao(NomeServico, codigoRetorno);
                    }

                    //Limita em 6 a quantidade de metas de oferta disponíveis para exibição
                    if (metasOferta != null && metasOferta.Length > 0)
                        metasOferta = metasOferta.Take(6).ToArray();
                }
                catch (FaultException<ZPPlanoContasServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return metasOferta;
        }

        /// <summary>Consulta faturamentos da oferta (para todos os anos)</summary>
        private Faturamento[] ConsultarFaturamentos(Int32 codigoOferta, Decimal codigoProposta)
        {
            //Variável de retorno
            Faturamento[] faturamentos = null;

            using (Logger Log = Logger.IniciarLog("Consulta Faturamentos da Oferta"))
            {
                try
                {
                    //Obtém dados do filtro para consulta
                    Int32 numeroPV = this.FiltroNumeroPV;
                    TipoEstabelecimento tipoEstabelecimento = this.FiltroTipoEstabelecimento;

                    //Variáveis auxiliares para consulta            
                    Int16 codigoRetorno = default(Int16);

                    //Consulta dados do ano
                    using (var ctx = new ContextoWCF<HISServicoZP_PlanoContasClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaServico,
                            new { codigoOferta, numeroPV, tipoEstabelecimento, codigoProposta });
                        faturamentos = ctx.Cliente.ConsultarFaturamentos(
                            out codigoRetorno, codigoOferta, numeroPV, tipoEstabelecimento, codigoProposta);
                        Log.GravarLog(EventoLog.RetornoServico, new { codigoRetorno, faturamentos });
                    }

                    //Valida código de retorno
                    if (codigoRetorno != 0)
                    {
                        faturamentos = null;
                        base.ExibirPainelExcecao(NomeServico, codigoRetorno);
                    }
                }
                catch (FaultException<ZPPlanoContasServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return faturamentos;
        }

        /// <summary>Consulta se o PV logado é Matriz</summary>
        private Boolean ConsultarAcessoMatriz()
        {
            Boolean acessoMatriz = true;
            Int32 codigoRetorno = default(Int32);

            using (Logger Log = Logger.IniciarLog("Verifica se é Matriz"))
            {
                try
                {
                    //Consulta dados do PV
                    using (var ctx = new ContextoWCF<EntidadeServicoClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaServico, new { SessaoAtual.CodigoEntidade });
                        Entidade entidade = ctx.Cliente.ConsultarDadosCompletos(out codigoRetorno, SessaoAtual.CodigoEntidade, false);
                        Log.GravarLog(EventoLog.RetornoServico, new { entidade, codigoRetorno });

                        //Se 2, é Matriz
                        if (codigoRetorno == 0)
                            acessoMatriz = entidade.TipoEstabelecimento == 2;
                        else
                            base.ExibirPainelExcecao("EntidadeServico.ConsultarDadosCompletos", codigoRetorno);
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
            return acessoMatriz;
        }

        #endregion
    }
}