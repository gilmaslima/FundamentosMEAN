/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Comum;
using Redecard.PN.OutrosServicos.SharePoint.NkPlanoContasServico;
using Redecard.PN.OutrosServicos.SharePoint.ZPPlanoContasServico;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Redecard.PN.OutrosServicos.SharePoint.ControlTemplates.OutrosServicos.Ofertas
{
    /// <summary>
    /// UserControl da WebPart da Consulta de Bônus Rede / Projeto Japão
    /// </summary>
    public partial class JapaoBonusRede : UserControlBase
    {
      
        #region [ Propriedades / Variáveis / Atributos ]

        /// <summary>CultureInfo pt-BR</summary>
        private readonly CultureInfo ptBR = CultureInfo.CreateSpecificCulture("pt-BR");
        /// <summary>TextInfo</summary>
        private readonly TextInfo textInfo = new CultureInfo("pt-BR", false).TextInfo;

        /// <summary>Condições contratadas</summary>
        private FaixaOfertaNoAceite[] condicoesContratadas;
        /// <summary>Condições contratadas</summary>
        private FaixaOfertaNoAceite[] CondicoesContratadas
        {
            get
            {
                return condicoesContratadas ?? (condicoesContratadas = ConsultarCondicoesContratadas());
            }
        }

        /// <summary>Celulares para recebimento Bônus</summary>
        private OfertaCelular[] celulares;
        /// <summary>Celulares para recebimento Bônus</summary>
        private OfertaCelular[] Celulares
        {
            get
            {
                return celulares ?? (celulares = ConsultarCelulares());
            }
        }

        /// <summary>Histórico da oferta contratada</summary>
        private OfertaDadosApuracao[] historicoOferta;
        /// <summary>Histórico da oferta contratada</summary>
        private OfertaDadosApuracao[] HistoricoOferta
        {
            get
            {
                return historicoOferta ?? (historicoOferta = ConsultarHistoricoOferta());
            }
        }
        #endregion

        #region [ Métodos Públicos ]

        /// <summary>
        /// Carregamento inicial do controle
        /// </summary>
        public void CarregarControle()
        {
            if (!IsPostBack)
            {
                // dados do usuário no header do bloco de impressão
                if (this.SessaoAtual != null)
                    this.ltrHeaderImpressaoUsuario.Text = string.Concat(SessaoAtual.CodigoEntidade, " / ", SessaoAtual.LoginUsuario);

                //Carregamento das tabelas
                this.CarregarCondicoesContratadas();
                this.CarregarCelulares();
                this.CarregarHistoricoOferta();
            }
        }

        #endregion

        #region [ Eventos Página ]

        /// <summary>
        /// Page_Load
        /// </summary>
        public void Page_Load(object sender, EventArgs e)
        {
            //Registra botões como assíncronos
            RegistrarBotoesHistoricoAsync();

            //this.Page.MaintainScrollPositionOnPostBack = true;
        }

        #endregion

        #region [ Eventos Controles - Botões ]

        /// <summary>
        /// btnValorCompensado_Click
        /// </summary>
        protected void btnValorCompensado_Click(object sender, EventArgs e)
        {
            var btnValorCompensado = (LinkButton)sender;

            //Recuperação  de parâmetros do botão
            DateTime mesAnoReferencia = btnValorCompensado.CommandArgument.ToDate("dd/MM/yyyy");

            //Carrega dados e exibe lighbox
            this.CarregarAluguelCompensado(mesAnoReferencia);
            upnlAluguelCompensado.Update();

            String javaScript = "ExecuteOrDelayUntilScriptLoaded(function () { OfertaBonusOpenModal('#lbxModalAluguelCompensado'); }, 'SP.UI.Dialog.js');";
            ScriptManager.RegisterStartupScript(upnlAluguelCompensado, upnlAluguelCompensado.GetType(), "AbrirModalDialog", javaScript, true);
        }

        /// <summary>
        /// btnBonusCreditado_Click
        /// </summary>
        protected void btnBonusCreditado_Click(object sender, EventArgs e)
        {
            var btnBonusCreditado = (LinkButton)sender;

            //Recuperação de parâmetros do botão
            String[] argumentos = btnBonusCreditado.CommandArgument.Split(';');
            DateTime mesAnoReferencia = argumentos[0].ToDate("dd/MM/yyyy");
            DateTime? dataInicio = argumentos[1].ToDateTimeNull("dd/MM/yyyy");
            DateTime? dataFim = argumentos[2].ToDateTimeNull("dd/MM/yyyy");

            //Carrega dados e exibe lightbox
            this.CarregarBeneficio(mesAnoReferencia, dataInicio, dataFim);
            upnlBeneficio.Update();

            String javaScript = "ExecuteOrDelayUntilScriptLoaded(function () { OfertaBonusOpenModal('#lightboxBeneficio'); }, 'SP.UI.Dialog.js');";
            ScriptManager.RegisterStartupScript(upnlBeneficio, upnlBeneficio.GetType(), "AbrirModalDialog", javaScript, true);
        }

        #endregion

        #region [ Métodos Privados ]

        /// <summary>
        /// Registra os botões do repeater de histórico de ofertas como assíncronos
        /// para exibição dos lightboxes (que estão em UpdatePanel)
        /// </summary>
        private void RegistrarBotoesHistoricoAsync()
        {
            if (rptHistorico.Items.Count > 0)
            {
                foreach (RepeaterItem item in rptHistorico.Items)
                {
                    var btnValorCompensado = (LinkButton)item.FindControl("btnValorCompensado");
                    var btnBonusCreditado = (LinkButton)item.FindControl("btnBonusCreditado");

                    ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(btnValorCompensado);
                    ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(btnBonusCreditado);
                }
            }
        }

        #endregion

        #region [ Métodos Privados - Carregamento de Controles ]

        /// <summary>
        /// Carrega informações das condições contratadas
        /// </summary>
        private void CarregarCondicoesContratadas()
        {
            if (this.CondicoesContratadas != null && this.CondicoesContratadas.Length > 0)
            {
                rptCondicoesContratadas.DataSource = this.CondicoesContratadas;
                rptCondicoesContratadas.DataBind();
            }
        }

        /// <summary>
        /// Carrega informações de celulares
        /// </summary>
        private void CarregarCelulares()
        {
            if (this.Celulares != null && this.Celulares.Length > 0)
            {
                rptCelulares.DataSource = this.Celulares;
                rptCelulares.DataBind();
            }
        }

        /// <summary>
        /// Carrega informações do histórico da oferta
        /// </summary>
        private void CarregarHistoricoOferta()
        {
            if (this.HistoricoOferta != null && this.HistoricoOferta.Length > 0)
            {
                FaixaOfertaNoAceite meta = CondicoesContratadas.FirstOrDefault();

                var historicoJson = this.HistoricoOferta.Where(h => h.DataInicioApuracao.HasValue)
                .OrderByDescending("DataInicioApuracao")
                .GroupBy(d => d.DataInicioApuracao.Value.Year)
                .Select(g =>
                new
                {
                    Ano = g.Key,
                    Meses = g.Select(it =>
                    {
                        Decimal valorMeta = it.ValorInicial != 0 || it.ValorFinal != 0 ? it.ValorInicial : meta != null ? meta.ValorInicial : 0; // mesma regra que tem no databound do historico para preencher o valor da Meta
                        String mensagem = String.Empty;

                        if (valorMeta == 0)
                        {
                            mensagem = "não se aplica";
                        }
                        else
                        {
                            Decimal porcentagem = 0;
                            porcentagem = (it.ValorRealizado / valorMeta) * 100;
                            porcentagem = Math.Floor(porcentagem); // Porcentagem nao deve ter casa decimal, e deve estar arredondado para baixo.


                            if (porcentagem > 100)
                            {
                                mensagem = String.Format("<span class=\"bold\">{0}%</span><span> acima da meta :)</span>", porcentagem - 100);
                            }
                            else if (porcentagem < 100)
                            {
                                mensagem = String.Format("<span class=\"bold\">{0}%</span><span> abaixo da meta :(</span>", 100 - porcentagem);
                            }
                            else
                            {
                                mensagem = "Você bateu sua meta :)";
                            }
                        }

                        return new
                        {
                            Mes = it.DataInicioApuracao.Value.ToString("MMM", ptBR),
                            MesNumerico = it.DataInicioApuracao.Value.Month,
                            Meta = valorMeta,
                            Realizado = it.ValorRealizado,
                            Mensagem = mensagem
                        };
                    })
                });

                JavaScriptSerializer serialize = new JavaScriptSerializer();
                hdfAnosMeses.Value = serialize.Serialize(historicoJson);

                mvwHistoricoOferta.SetActiveView(pnlHistoricoOferta);
                rptHistorico.DataSource = this.HistoricoOferta;
                rptHistorico.DataBind();
            }
            else
            {
                //Só carregou o quadro de aviso (Sem apuração) quando o código do retorno é 60 
                mvwHistoricoOferta.SetActiveView(pnlSemApuracao);
            }
        }

        /// <summary>
        /// Carrega informações do aluguel compensado
        /// </summary>
        /// <param name="mesAnoReferencia">Mês/Ano referência</param>
        private void CarregarAluguelCompensado(DateTime mesAnoReferencia)
        {
            //Consulta as resumos compensados
            List<CompensacaoDebitoAluguel> resumos = ConsultarAluguelCompensado(mesAnoReferencia);

            //Se possui resumos compensados, carrega lightbox com os dados dos resumos
            if (resumos != null && resumos.Count > 0)
            {
                rptAluguelCompensado.Visible = true;
                qdAvisoAluguelCompensadoPendente.Visible = false;
                rptAluguelCompensado.DataSource = resumos;
                rptAluguelCompensado.DataBind();

                //Busca controle no header de um Repeater
                var ltrAluguelCompensadoMesReferencia = (Literal)rptAluguelCompensado.Controls[0].Controls[0].FindControl("ltrAluguelCompensadoMesReferencia");
                ltrAluguelCompensadoMesReferencia.Text = String.Format("{0}/{1}",
                    textInfo.ToTitleCase(mesAnoReferencia.ToString("MMM", ptBR)),
                    mesAnoReferencia.ToString("yy"));
            }
            //Se não possui resumos compensados, exibe mensagem de aviso
            else
            {
                rptAluguelCompensado.Visible = false;
                qdAvisoAluguelCompensadoPendente.Visible = true;
            }
        }

        /// <summary>
        /// Carrega informações do benefício
        /// </summary>
        /// <param name="mesAnoReferencia">Mês/Ano referência</param>
        /// <param name="dataInicio">Data início de referência</param>
        /// <param name="dataFim">Data fim de referência</param>
        private void CarregarBeneficio(DateTime mesAnoReferencia, DateTime? dataInicio, DateTime? dataFim)
        {
            rptBeneficio.DataSource = ConsultarBeneficio(mesAnoReferencia);
            rptBeneficio.DataBind();

            //Busca controle no header de um Repeater
            var ltrMesRefencia = (Literal)rptBeneficio.Controls[0].Controls[0].FindControl("ltrMesReferencia");
            ltrMesRefencia.Text = String.Format("{0} a {1}",
                dataInicio.HasValue ? dataInicio.Value.ToString("dd/MM/yyyy") : "-",
                dataFim.HasValue ? dataFim.Value.ToString("dd/MM/yyyy") : "-");
        }

        #endregion

        #region [ Bind de Repeaters ]

        /// <summary>
        /// ItemDataBound do repeater de Condições Contratadas
        /// </summary>
        protected void rptCondicoesContratadas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var faixa = (FaixaOfertaNoAceite)e.Item.DataItem;

                var ltrFaturamentoMeta = (Literal)e.Item.FindControl("ltrFaturamentoMeta");
                var rptEquipamentos = (Repeater)e.Item.FindControl("rptEquipamentos");
                var ltrValorBonus = (Literal)e.Item.FindControl("ltrValorBonus");
                var divChartValue = (HtmlGenericControl)e.Item.FindControl("divChartValue");

                //Tratamento de descrição caso Valor Final da Faixa = 0
                if (faixa.ValorInicial == 0 && faixa.ValorFinal == 0)
                    ltrFaturamentoMeta.Text = "Não se aplica";
                else if (faixa.ValorFinal == 0)
                    ltrFaturamentoMeta.Text = String.Format(ptBR, "A partir de {0:F2}", faixa.ValorInicial);
                else
                    ltrFaturamentoMeta.Text = String.Format(ptBR, "{0:F2} a {1:F2}", faixa.ValorInicial, faixa.ValorFinal);

                ltrValorBonus.Text = faixa.ValorBonus.ToString("F2", ptBR);
                rptEquipamentos.DataSource = faixa.Equipamentos;
                rptEquipamentos.DataBind();

                Decimal totalCondicoes = this.CondicoesContratadas.Count();
                Decimal condicaoAtual = e.Item.ItemIndex + 1;
                Decimal porcentagem = Math.Ceiling((100 / totalCondicoes) * condicaoAtual);

                if (porcentagem > 100)
                    porcentagem = 100;

                divChartValue.Style.Add("width", String.Format("{0}%", porcentagem));
            }
        }

        /// <summary>
        /// ItemDataBound do repeater interno de Equipamentos
        /// </summary>
        protected void rptEquipamentos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var equipamento = (FaixaOfertaNoAceiteEquipamento)e.Item.DataItem;

                var ltrEquipamento = (Literal)e.Item.FindControl("ltrEquipamento");
                var ltrEquipamentoValor = (Literal)e.Item.FindControl("ltrEquipamentoValor");

                String descricao = equipamento.Codigo;
                switch (equipamento.Codigo)
                {
                    case "POO":
                        descricao = String.Concat(descricao, " (sem fio)");
                        break;
                    case "POS":
                        descricao = String.Concat(descricao, " (com fio)");
                        break;
                    default:
                        descricao = equipamento.Codigo;
                        break;
                }
                ltrEquipamento.Text = descricao;

                ltrEquipamentoValor.Text = equipamento.ValorAluguel.ToString("F2", ptBR);
            }
        }

        /// <summary>
        /// ItemDataBound do repeater de celulares
        /// </summary>
        protected void rptCelulares_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var celular = (OfertaCelular)e.Item.DataItem;

                var ltrNumeroCelular = (Literal)e.Item.FindControl("ltrNumeroCelular");
                var ltrValorBonus = (Literal)e.Item.FindControl("ltrValorBonus");
                var ltrDistribuicaoBonus = (Literal)e.Item.FindControl("ltrDistribuicaoBonus");

                ltrValorBonus.Text = celular.ValorBonus.ToString("F2", ptBR);
                ltrDistribuicaoBonus.Text = (celular.PercentualBonus / 100).ToString("N0", ptBR);

                String numeroCel = celular.NumeroCelular.ToString();
                IEnumerable<Char> celParte1 = numeroCel.Take(numeroCel.Length >= 9 ? 5 : 4);
                IEnumerable<Char> celParte2 = numeroCel.Skip(celParte1.Count());
                ltrNumeroCelular.Text = String.Format("({0}) {1}-{2}",
                    celular.DddCelular, new String(celParte1.ToArray()), new String(celParte2.ToArray()));
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                var ltrTotalValorBonus = (Literal)e.Item.FindControl("ltrTotalValorBonus");
                var ltrTotalDistribuicaoBonus = (Literal)e.Item.FindControl("ltrTotalDistribuicaoBonus");

                ltrTotalValorBonus.Text =
                    this.celulares.Sum(celular => celular.ValorBonus).ToString("F2", ptBR);
                ltrTotalDistribuicaoBonus.Text =
                    this.Celulares.Sum(celular => celular.PercentualBonus / 100).ToString("N0", ptBR);
            }
        }

        /// <summary>
        /// ItemDataBound do repeater do histórico da oferta
        /// </summary>
        protected void rptHistorico_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var historico = (OfertaDadosApuracao)e.Item.DataItem;

                var divHistoricoItem = (HtmlGenericControl)e.Item.FindControl("divHistoricoItem");
                var ltrTitulo = (Literal)e.Item.FindControl("ltrTitulo");
                var ltrMesApuracao = (Literal)e.Item.FindControl("ltrMesApuracao");
                var ltrMeta = (Literal)e.Item.FindControl("ltrMeta");
                var ltrRealizado = (Literal)e.Item.FindControl("ltrRealizado");
                var lblRealizadoConfirmacao = (Label)e.Item.FindControl("lblRealizadoConfirmacao");
                var ltrQuantidade = (Literal)e.Item.FindControl("ltrQuantidade");
                var ltrValorAluguel = (Literal)e.Item.FindControl("ltrValorAluguel");
                var ltrValorCompensado = (Literal)e.Item.FindControl("ltrValorCompensado");
                var lblCompensadoBateu = (Label)e.Item.FindControl("lblCompensadoBateu");
                var btnValorCompensado = (LinkButton)e.Item.FindControl("btnValorCompensado");
                var btnBonusCreditado = (LinkButton)e.Item.FindControl("btnBonusCreditado");
                var ltrBonusCreditado = (Literal)e.Item.FindControl("ltrBonusCreditado");

                if (historico.DataInicioApuracao.HasValue)
                {
                    divHistoricoItem.Attributes.Add("data-mes-ano", String.Format(ptBR, "{0:yyyy}-{0:MMM}", historico.DataInicioApuracao));
                    ltrTitulo.Text = String.Format(ptBR, "{0:MMMM} {0:yyyy}", historico.DataInicioApuracao);
                }

                //Mês de Apuração
                if (historico.DataInicioApuracao.HasValue && historico.DataFimApuracao.HasValue)
                    ltrMesApuracao.Text = String.Format("{0} a {1}",
                        historico.DataInicioApuracao.Value.ToString("dd/MM/yyyy"),
                        historico.DataFimApuracao.Value.ToString("dd/MM/yyyy"));
                else
                    ltrMesApuracao.Text = "-";

                //Meta
                Boolean bateuMeta = historico.ValorInicial != 0 || historico.ValorFinal != 0;
                if (bateuMeta)
                {
                    if (historico.ValorFinal == 0)
                        ltrMeta.Text = String.Format(ptBR, "A partir de {0:C2}", historico.ValorInicial);
                    else
                        ltrMeta.Text = String.Format(ptBR, "{0:C2} a {1:C2}", historico.ValorInicial, historico.ValorFinal);
                }
                else
                {
                    //Quando não bateu Meta, exibe o valor da primeira Meta Contratada
                    FaixaOfertaNoAceite meta = CondicoesContratadas.FirstOrDefault();
                    if (meta != null)
                    {
                        if (meta.ValorInicial == 0 && meta.ValorFinal == 0)
                            ltrMeta.Text = "não se aplica";
                        else if (meta.ValorFinal == 0)
                            ltrMeta.Text = String.Format(ptBR, "A partir de {0:C2}", meta.ValorInicial);
                        else
                            ltrMeta.Text = String.Format(ptBR, "{0:C2} a {1:C2}", meta.ValorInicial, meta.ValorFinal);
                    }
                    else
                        ltrMeta.Text = "não se aplica";
                }

                //Realizado
                ltrRealizado.Text = historico.ValorRealizado.ToString("C2", ptBR);

                lblRealizadoConfirmacao.Text = "não";
                lblRealizadoConfirmacao.CssClass = "red-text";

                if (bateuMeta && historico.IndicadorMeta)
                {
                    lblRealizadoConfirmacao.Text = "sim";
                    lblRealizadoConfirmacao.CssClass = "jade-text";
                }

                //Qtd. Terminais
                ltrQuantidade.Text = historico.QuantidadeTerminais.ToString();

                //Valor de Aluguel
                ltrValorAluguel.Text = historico.ValorTotalAluguel.ToString("C2", ptBR);

                //Valor Compensado
                Boolean valorFoiCompensado = historico.ValorCompensado > 0 && historico.ValorCompensado == historico.ValorTotalAluguel;
                ltrValorCompensado.Text = btnValorCompensado.Text = historico.ValorCompensado.ToString("C2", ptBR);
                btnValorCompensado.Visible = valorFoiCompensado;
                ltrValorCompensado.Visible = !valorFoiCompensado;
                if (historico.IndicadorPagamento)
                {
                    lblCompensadoBateu.Text = "sim";
                    lblCompensadoBateu.CssClass = "jade-text";
                }
                else
                {
                    lblCompensadoBateu.Text = "não";
                    lblCompensadoBateu.CssClass = "red-text";
                }

                btnValorCompensado.CommandArgument = historico.MesReferencia.ToString("dd/MM/yyyy");

                //Bônus Creditado
                Boolean bonusFoiCreditado = historico.ValorBonusCreditado > 0;
                ltrBonusCreditado.Text = btnBonusCreditado.Text = historico.ValorBonusCreditado.ToString("C2", ptBR);
                btnBonusCreditado.Visible = bonusFoiCreditado;
                ltrBonusCreditado.Visible = !bonusFoiCreditado;
                btnBonusCreditado.CommandArgument = String.Format("{0};{1};{2}",
                    historico.MesReferencia.ToString("dd/MM/yyyy"),
                    historico.DataInicioApuracao.HasValue ? historico.DataInicioApuracao.Value.ToString("dd/MM/yyyy") : "-",
                    historico.DataFimApuracao.HasValue ? historico.DataFimApuracao.Value.ToString("dd/MM/yyyy") : "-");
            }
        }

        /// <summary>
        /// ItemDataBound do repeater de aluguel compensado do lightbox
        /// </summary>
        protected void rptAluguelCompensado_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var compensacao = (CompensacaoDebitoAluguel)e.Item.DataItem;

                var ltrResumo = (Literal)e.Item.FindControl("ltrResumo");
                var hlkResumo = (HyperLink)e.Item.FindControl("hlkResumo");
                var ltrVencimento = (Literal)e.Item.FindControl("ltrVencimento");
                var ltrValor = (Literal)e.Item.FindControl("ltrValor");

                switch (compensacao.IndicadorNet)
                {
                    case "C":
                    case "D":
                        var qs = new QueryStringSegura();
                        qs["tipoVenda"] = compensacao.IndicadorNet;
                        qs["numeroResumoVenda"] = compensacao.NumeroResumoVenda.ToString();
                        qs["numeroEstabelecimento"] = SessaoAtual.CodigoEntidade.ToString();
                        qs["dataApresentacao"] = (compensacao.DataCompensacao.HasValue ?
                            compensacao.DataCompensacao.Value : DateTime.MinValue).ToString("dd/MM/yyyy");

                        //String url = String.Format("/sites/fechado/extrato/paginas/pn_ResumoVendas.aspx?dados={0}", qs.ToString());
                        //hlkResumo.Text = compensacao.NumeroResumoVenda.ToString();
                        //hlkResumo.NavigateUrl = url;
                        //hlkResumo.Attributes.Add("onclick", "blockUI();");
                        //ltrResumo.Visible = false;

                        ltrResumo.Text = compensacao.NumeroResumoVenda.ToString();
                        hlkResumo.Visible = false;

                        break;
                    case "O":
                    default:
                        ltrResumo.Text = hlkResumo.Text = compensacao.DescricaoTipoPagamento;
                        hlkResumo.Visible = false;
                        break;
                }
                ltrVencimento.Text = compensacao.DataCompensacao.HasValue ?
                    compensacao.DataCompensacao.Value.ToString("dd/MM/yyyy") : "-";
                ltrValor.Text = compensacao.ValorCompensado.ToString("C2", ptBR);
            }
        }

        /// <summary>
        /// ItemDataBound do repeater de benefício do lightbox
        /// </summary>
        protected void rptBeneficio_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var beneficio = (OfertaDadosApuracaoDetalhe)e.Item.DataItem;

                var ltrNumeroCelular = (Literal)e.Item.FindControl("ltrNumeroCelular");
                var ltrValorBonus = (Literal)e.Item.FindControl("ltrValorBonus");
                var ltrPrevisaoCredito = (Literal)e.Item.FindControl("ltrPrevisaoCredito");
                var ltrDataCredito = (Literal)e.Item.FindControl("ltrDataCredito");
                var ltrObservacoes = (Literal)e.Item.FindControl("ltrObservacoes");

                String numeroCel = beneficio.NumeroCelular.ToString();
                IEnumerable<Char> celParte1 = numeroCel.Take(numeroCel.Length >= 9 ? 5 : 4);
                IEnumerable<Char> celParte2 = numeroCel.Skip(celParte1.Count());
                ltrNumeroCelular.Text = String.Format("({0}) {1}-{2}",
                    beneficio.DddCelular, new String(celParte1.ToArray()), new String(celParte2.ToArray()));
                ltrValorBonus.Text = beneficio.ValorBonusCreditado.ToString("C2", ptBR);
                ltrPrevisaoCredito.Text = beneficio.DataPrevisaoCredito.HasValue ?
                    beneficio.DataPrevisaoCredito.Value.ToString("dd/MM/yyyy") : "-";
                ltrDataCredito.Text = beneficio.DataCredito.HasValue ?
                    beneficio.DataCredito.Value.ToString("dd/MM/yyyy") : "-";
                ltrObservacoes.Text = beneficio.Observacao;
            }
        }

        #endregion

        #region [ Consultas WCF ]

        /// <summary>
        /// Consulta condições contratadas
        /// </summary>
        /// <returns>Condições contratadas</returns>
        private FaixaOfertaNoAceite[] ConsultarCondicoesContratadas()
        {
            using (Logger log = Logger.IniciarLog("Consulta dados da oferta no aceite"))
            {
                var codigoRetorno = default(Int16);
                var numeroPv = SessaoAtual.CodigoEntidade;
                var faixas = default(FaixaOfertaNoAceite[]);

                try
                {
                    using (var ctx = new ContextoWCF<HISServicoZP_PlanoContasClient>())
                        faixas = ctx.Cliente.ConsultarDadosOfertaAceite(out codigoRetorno, numeroPv);

                    if (codigoRetorno != 0)
                        base.ExibirPainelExcecao("ZPPlanoContasServico.ConsultarDadosOfertaAceite", codigoRetorno);
                }
                catch (FaultException<ZPPlanoContasServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }

                return faixas;
            }
        }

        /// <summary>
        /// Consulta celulares
        /// </summary>
        /// <returns>Celulares</returns>
        private OfertaCelular[] ConsultarCelulares()
        {
            using (Logger log = Logger.IniciarLog("Consulta celulares"))
            {
                var codigoRetorno = default(Int16);
                var numeroPv = SessaoAtual.CodigoEntidade;
                var celulares = default(OfertaCelular[]);

                try
                {
                    using (var ctx = new ContextoWCF<HISServicoZP_PlanoContasClient>())
                        celulares = ctx.Cliente.ConsultarDadosCelularBonus(out codigoRetorno, numeroPv);

                    if (codigoRetorno != 0)
                        base.ExibirPainelExcecao("ZPPlanoContasServico.ConsultarDadosCelularBonus", codigoRetorno);
                }
                catch (FaultException<ZPPlanoContasServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }

                return celulares;
            }
        }

        /// <summary>
        /// Consulta histórico da oferta
        /// </summary>
        /// <returns>Histórico da oferta</returns>
        private OfertaDadosApuracao[] ConsultarHistoricoOferta()
        {
            using (Logger log = Logger.IniciarLog("Consulta dados de apuração"))
            {
                var codigoRetorno = default(Int16);
                var numeroPv = SessaoAtual.CodigoEntidade;
                var dadosApuracao = default(OfertaDadosApuracao[]);

                try
                {
                    using (var ctx = new ContextoWCF<HISServicoZP_PlanoContasClient>())
                        dadosApuracao = ctx.Cliente.ConsultarDadosApuracao(out codigoRetorno, numeroPv);

                    //60: sem apuração, carrega quadro de aviso correspondente
                    if (codigoRetorno == 60)
                    {
                        dadosApuracao = null;
                    }
                    else if (codigoRetorno != 0)
                        base.ExibirPainelExcecao("ZPPlanoContasServico.ConsultarDadosApuracao", codigoRetorno);
                }
                catch (FaultException<ZPPlanoContasServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }

                return dadosApuracao;
            }
        }

        /// <summary>
        /// Consulta aluguel compensado
        /// </summary>
        /// <param name="mesAnoDebito">Mês/Ano de referência/débito</param>
        /// <returns>Aluguel compensado</returns>
        private List<CompensacaoDebitoAluguel> ConsultarAluguelCompensado(DateTime mesAnoDebito)
        {
            using (Logger log = Logger.IniciarLog("Consulta aluguel compensado"))
            {
                var codigoRetorno = default(Int16);
                var numeroPv = SessaoAtual.CodigoEntidade;
                var aluguelCompensado = default(List<CompensacaoDebitoAluguel>);

                try
                {
                    using (var ctx = new ContextoWCF<HisServicoNkPlanoContasClient>())
                        aluguelCompensado = ctx.Cliente.ConsultarCompensacoesDebitos(
                            out codigoRetorno, numeroPv, mesAnoDebito);

                    if (codigoRetorno != 0)
                        base.ExibirPainelExcecao("NKPlanoContasServico.ConsultarCompensacoesDebitos", codigoRetorno);
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

                return aluguelCompensado;
            }
        }

        /// <summary>
        /// Consulta benefício
        /// </summary>
        /// <param name="mesAnoReferencia">Mês/Ano de referência</param>
        /// <returns>Benefício</returns>
        private OfertaDadosApuracaoDetalhe[] ConsultarBeneficio(DateTime mesAnoReferencia)
        {
            using (Logger log = Logger.IniciarLog("Consulta aluguel compensado"))
            {
                var codigoRetorno = default(Int16);
                var numeroPv = SessaoAtual.CodigoEntidade;
                var beneficio = default(OfertaDadosApuracaoDetalhe[]);

                try
                {
                    using (var ctx = new ContextoWCF<HISServicoZP_PlanoContasClient>())
                        beneficio = ctx.Cliente.ConsultarDadosApuracaoDetalhes(
                            out codigoRetorno, numeroPv, mesAnoReferencia);

                    if (codigoRetorno != 0)
                        base.ExibirPainelExcecao("ZPPlanoContasServico.ConsultarDadosApuracaoDetalhes", codigoRetorno);
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

                return beneficio;
            }
        }

        #endregion

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
           Response.Redirect("/sites/fechado/servicos/Paginas/taxas-e-ofertas-contratadas.aspx");
        }
    }
}