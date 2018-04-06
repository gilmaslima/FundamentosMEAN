/*
© Copyright 2014 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software
Histórico: [19/01/2015] Change Request Turquia - Alexandre Shiroma
*/

using Redecard.PN.Comum;
using Redecard.PN.GerencieExtrato.Core.Web.Controles.Portal;
using Redecard.PN.GerencieExtrato.SharePoint.GerencieExtratoServico;
using Redecard.PN.GerencieExtrato.SharePoint.NkPlanoContasServico;
using Redecard.PN.GerencieExtrato.SharePoint.ZPPlanoContasServico;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace Redecard.PN.GerencieExtrato.SharePoint.WebParts.SolicitarRelatorioPrecoUnico
{
    /// <summary>
    /// Solicitação de Relatório Preço Único
    /// </summary>
    public partial class SolicitarRelatorioPrecoUnicoUserControl : UserControlBase
    {
        #region [ Propriedades ]
        
        /// <summary>
        /// Lista de planos preço único contratados armazenada na ViewState.
        /// </summary>
        public List<PlanoPrecoUnico> PlanosPrecoUnicoContratados
        {
            get 
            { 
                if(ViewState["PlanosPrecoUnico"] == null)
                    ViewState["PlanosPrecoUnico"] = ConsultaPlanosPrecoUnicoContratados();
                return (List<PlanoPrecoUnico>)ViewState["PlanosPrecoUnico"];
            }
            set { ViewState["PlanosPrecoUnico"] = value; }
        }

        /// <summary>
        /// Lista de períodos de relatórios preço único disponíveis para consulta, armazenada na ViewState.
        /// </summary>
        public List<DateTime> PeriodosRelatoriosDisponiveis
        {
            get
            {
                if (ViewState["RelatoriosPrecoUnicoDisponiveis"] == null)
                    ViewState["RelatoriosPrecoUnicoDisponiveis"] = ConsultarPeriodosRelatoriosPrecoUnicoDisponiveis();
                return (List<DateTime>)ViewState["RelatoriosPrecoUnicoDisponiveis"];
            }
            set { ViewState["RelatoriosPrecoUnicoDisponiveis"] = value; }
        }

        /// <summary>CultureInfo pt-BR</summary>
        private readonly CultureInfo ptBr = CultureInfo.CreateSpecificCulture("pt-BR");

        #endregion

        #region [ Eventos ]

        /// <summary>
        /// Page_Load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Sessao.Contem())
                {
#if DEBUG
                    CarregarPlanosPrecoUnico();
                    return;
#endif

                    //Somente carrega a tela para quem possui oferta turquia
                    if (ConsultarTipoOfertaAtiva() == TipoOferta.OfertaTurquia)
                        CarregarPlanosPrecoUnico();
                    else
                    {
                        //Redireciona para a tela de Segunda Via Extrato (Extratos para Download)
                        Response.Redirect("pn_SegundaViaExtrato.aspx", false);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                }
            }
        }

        /// <summary>
        /// Evento click do botão Buscar.
        /// </summary>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Buscar Relatório Preço Único."))
            {
                try
                {
                    //Recupera o plano contratado selecionado pelo usuário
                    Int32 indicePlano = ddlPeriodo.SelectedValue.ToInt32();
                    PlanoPrecoUnico plano = this.PlanosPrecoUnicoContratados[indicePlano];

                    //Se o relatório já está disponível para consulta para o mês/ano selecionado,
                    //redireciona para a tela de Extratos para Download
                    if (VerificarDisponibilidadeRelatorio(plano.AnoMesReferencia))
                    {
                        Response.Redirect("pn_SegundaViaExtrato.aspx", false);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                    //Caso contrário, inclui a solicitação do relatório
                    else
                    {
                        var sbMensagem = new StringBuilder();

                        if (SolicitarRelatorioPrecoUnico(plano))
                            sbMensagem.Append("O período solicitado não está disponível para visualização.<br />")
                                .Append("O relatório estará disponível para download no prazo de <b>48 horas</b><br />")
                                .Append("em Minhas vendas > Extratos > Emitir 2ª via de extrato");
                        else
                            sbMensagem.Append("Não há relatório para o período informado!");

                        ExibirAviso(sbMensagem.ToString(), TipoQuadroAviso.Aviso);
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Fonte, ex.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }           
        }

        #endregion

        #region [Métodos]

        /// <summary>
        /// Carrega os planos preço único (disponíveis para consulta, ou apenas contratados)
        /// </summary>
        private void CarregarPlanosPrecoUnico()
        {
            if (this.PlanosPrecoUnicoContratados != null)
            {
                ddlPeriodo.Items.Clear();
                ddlPeriodo.Items.Add("selecione o período");

                for(Int32 indicePlano = 0; indicePlano < this.PlanosPrecoUnicoContratados.Count; indicePlano++)
                {
                    PlanoPrecoUnico plano = this.PlanosPrecoUnicoContratados[indicePlano];
                    String chave = indicePlano.ToString();
                    String descricao = plano.AnoMesReferencia.ToString("MMMM/yyyy", ptBr);
                    
                    //Primeira letra maiúscula
                    descricao = String.Concat(Char.ToUpper(descricao[0]), descricao.Substring(1)).ToLower();

                    ddlPeriodo.Items.Add(new ListItem(descricao, chave));
                }
            }
        }

        /// <summary>
        /// Verifica se o período selecionado (mês/ano) já possui o relatório
        /// preço único disponível para consulta
        /// </summary>
        /// <param name="anoMesReferencia">Ano/Mês de referência do período solicitado</param>
        /// <returns></returns>
        private Boolean VerificarDisponibilidadeRelatorio(DateTime anoMesReferencia)
        {
#if DEBUG
            return anoMesReferencia.Month == DateTime.Now.Month;
#endif

            if (this.PeriodosRelatoriosDisponiveis != null)
                return this.PeriodosRelatoriosDisponiveis.Any(anoMes =>
                    anoMes.Month == anoMesReferencia.Month && anoMes.Year == anoMesReferencia.Year);
            else
                return false;
        }

        /// <summary>
        /// Solicita Relatório Preço Único por período.
        /// </summary>
        /// <param name="plano">Plano Preço Único selecionado</param>
        /// <returns></returns>
        private Boolean SolicitarRelatorioPrecoUnico(PlanoPrecoUnico plano)
        {
            using (Logger log = Logger.IniciarLog("Solicitando Relatório Preço Único."))
            {
                try
                {
                    //Parâmetros do serviço
                    Int32 numeroPv = base.SessaoAtual.CodigoEntidade;
                    var mesAnoDe = default(DateTime);
                    var mesAnoAte = default(DateTime);

                    if (plano.DataInicioApuracao.HasValue)
                        mesAnoDe = plano.DataInicioApuracao.Value;
                    else
                        mesAnoDe = new DateTime(plano.AnoMesReferencia.Year, plano.AnoMesReferencia.Month, 1);

                    if (plano.DataFimApuracao.HasValue)
                        mesAnoAte = plano.DataFimApuracao.Value;
                    else
                        mesAnoAte = new DateTime(plano.AnoMesReferencia.Year, plano.AnoMesReferencia.Month,
                            DateTime.DaysInMonth(plano.AnoMesReferencia.Year, plano.AnoMesReferencia.Month));

                    Int16 codigoRetorno = 0;

                    log.GravarLog(EventoLog.ChamadaServico, new { numeroPv, mesAnoDe, mesAnoAte });

                    //Solicita o relatório preço único.
                    using (var ctx = new ContextoWCF<GerencieExtratoClient>())
                        codigoRetorno = ctx.Cliente.SolicitarRelatorioPrecoUnico(numeroPv, mesAnoDe, mesAnoAte);

                    log.GravarLog(EventoLog.RetornoServico, new { numeroPv, codigoRetorno });

                    // 00 - INCLUSAO EFETUADA COM SUCESSO
                    // 01 - INCLUSAO NAO EFETUADA - SOLICITACAO JA EXISTE - 1
                    // 02 - INCLUSAO NAO EFETUADA - SOLICITACAO JA EXISTE - 2
                    // 03 - INCLUSAO NAO EFETUADA - SOLICITACAO JA EXISTE - 3
                    // 99 - ERRO NA PESQUISA - CONSULTAR O RETURN CODE
                    
                    //Se código 0, 1, 2 ou 3 (inclusão efetuada ou solicitação já existe)
                    if (new[] { 0, 1, 2, 3 }.Contains(codigoRetorno))
                        return true;
                    else
                    {
                        base.ExibirPainelExcecao("GerencieExtrato.SolicitarRelatorioPrecoUnico", codigoRetorno);
                        return false;
                    }
                }
                catch (FaultException<GerencieExtratoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    return false;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return false;
                }     
            }
        }

        /// <summary>
        /// Exibe a mensagem do quadro de aviso
        /// </summary>
        /// <param name="titulo">Título do aviso.</param>
        /// <param name="mensagem">Mensagem do corpo do aviso.</param>
        private void ExibirAviso(string mensagem, TipoQuadroAviso tipoAviso = TipoQuadroAviso.Aviso)
        {
            //Mostra o quadro de aviso
            pnlQuadroAviso.Visible = true;

            //Define a mensagem do Quadro de aviso
            this.qdoAviso.Visible = true;
            this.qdoAviso.Mensagem = mensagem;
            this.qdoAviso.TipoQuadro = tipoAviso;
        }

        #endregion

        #region [ Consultas WCF ]

        /// <summary>
        /// Consulta o tipo de oferta Ativa que deve ser exibida para o usuário
        /// Japão, Plano de Contas, Turquia, Sem Oferta, ...
        /// </summary>
        /// <returns>Tipo de oferta ativa</returns>
        private TipoOferta ConsultarTipoOfertaAtiva()
        {
            var codigoRetorno = default(Int16);
            var tipoOferta = default(TipoOferta);
            var numeroPv = SessaoAtual.CodigoEntidade;

            using (Logger log = Logger.IniciarLog("Consulta tipo de oferta ativa"))
            {
                try
                {
                    log.GravarLog(EventoLog.ChamadaServico, numeroPv);

                    using (var ctx = new ContextoWCF<HISServicoZP_PlanoContasClient>())
                        codigoRetorno = ctx.Cliente.ConsultarTipoOfertaAtiva(out tipoOferta, numeroPv);

                    log.GravarLog(EventoLog.RetornoServico, new { codigoRetorno, tipoOferta });
                }
                catch (FaultException<ZPPlanoContasServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }
            }

            return codigoRetorno == 0 ? tipoOferta : ZPPlanoContasServico.TipoOferta.SemOferta;
        }

        /// <summary>
        /// Consulta os planos Preço Único contratados para o PV (Ponto de Venda) da sessão atual.
        /// </summary>
        /// <returns>Lista de planos de preço único contratados para o estabelecimento</returns>
        private List<PlanoPrecoUnico> ConsultaPlanosPrecoUnicoContratados()
        {
#if DEBUG
            var r = new Random();

            List<PlanoPrecoUnico> ret = new List<PlanoPrecoUnico>();
            for (int i = 0; i < 12; i++)
            {
                ret.Add(new PlanoPrecoUnico
                {
                    AnoMesReferencia = DateTime.Now.AddMonths(i * -1),
                    DataFimApuracao = DateTime.Now.AddMonths(i * -1),
                    DataInicioApuracao = DateTime.Now.AddMonths(i * -1),
                    DescricaoOferta = "descrição",
                    Equipamentos = new List<Equipamento>
                    {
                        new Equipamento
                        {
                            QtdTerminais = 1,
                            Tipo = "poo",
                            Valor = (decimal)r.NextDouble()
                        }
                    },
                    IndicadorFlex = "flex teste",
                    NomePlano = "preço único teste",
                    QtdTecnologiasCadastradasPacote = 1,
                    Total = (decimal)r.NextDouble(),
                    TotalAluguelEquipamento = (decimal)r.NextDouble(),
                    ValorCabradoPeloExcedente = (decimal)r.NextDouble(),
                    ValorExcedenteContratado = (decimal)r.NextDouble(),
                    ValorFaturamentoApurado = (decimal)r.NextDouble(),
                    ValorLimiteFaturamentoContratado = (decimal)r.NextDouble(),
                    ValorLimiteFaturamentoContratadoProRata = (decimal)r.NextDouble(),
                    ValorMensalidade = (decimal)r.NextDouble(),
                    ValorMensalidadeCobrada = (decimal)r.NextDouble()
                });
            }

            return ret;
#endif

            using (Logger log = Logger.IniciarLog("Consulta planos Preço Único contratados"))
            {
                var codigoRetorno = default(Int16);
                var planosPrecoUnico = default(List<PlanoPrecoUnico>);
                var planosPrecoUnicoTratados = new List<PlanoPrecoUnico>();

                try
                {
                    log.GravarLog(EventoLog.ChamadaServico, SessaoAtual.CodigoEntidade);

                    //Efetua a chamada do seviço somente no caso de não existir o objeto na ViewState.
                    using (var ctx = new ContextoWCF<HisServicoNkPlanoContasClient>())
                        planosPrecoUnico = ctx.Cliente.ConsultarPlanosPrecoUnicoContratados(
                            out codigoRetorno, SessaoAtual.CodigoEntidade);

                    log.GravarLog(EventoLog.RetornoServico, new { planosPrecoUnico, codigoRetorno });

                    //Se código 60, exibe mensagem customizada de Sem Apuração Realizada (CR Turquia)
                    if (codigoRetorno != 60 && codigoRetorno != 0)
                        base.ExibirPainelExcecao("NKPlanoContasServico.ConsultaPlanosPrecoUnicoContratados", codigoRetorno);

                    //Remove os duplicados por mês/ano
                    if (planosPrecoUnico != null)
                    {
                        foreach (PlanoPrecoUnico planoPrecoUnico in planosPrecoUnico)
                        {
                            if (!planosPrecoUnicoTratados.Any(plano =>
                                plano.AnoMesReferencia.Month == planoPrecoUnico.AnoMesReferencia.Month
                                && plano.AnoMesReferencia.Year == planoPrecoUnico.AnoMesReferencia.Year))
                                planosPrecoUnicoTratados.Add(planoPrecoUnico);
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

                return planosPrecoUnicoTratados;
            }
        }

        /// <summary>
        /// Consulta os relatórios de preço único disponíveis.
        /// </summary>
        /// <returns>Períodos dos relatórios preço único já disponíveis para consulta</returns>
        private List<DateTime> ConsultarPeriodosRelatoriosPrecoUnicoDisponiveis()
        {
            using (Logger log = Logger.IniciarLog("Consulta de Extrato Relatório Preço Único"))
            {
                var periodosDisponiveis = new List<DateTime>();

                //Parâmetros do serviço
                var relatoriosPrecoUnico = default(List<ExtratoRelatorioPrecoUnico>);
                Int32 numeroPv = base.SessaoAtual.CodigoEntidade;
                Int16 codigoRetorno = 0;

                try
                {
                    log.GravarLog(EventoLog.ChamadaServico, new { numeroPv, codigoRetorno });

                    //Busca os dados do extrato
                    using (var ctx = new ContextoWCF<GerencieExtratoClient>())
                        relatoriosPrecoUnico = ctx.Cliente.ConsultarRelatorioPrecoUnico(out codigoRetorno, numeroPv);

                    log.GravarLog(EventoLog.RetornoServico, new { relatoriosPrecoUnico, numeroPv, codigoRetorno });

                    //Caso tenha ocorrido erro na chamada, dispara uma exceção
                    if (codigoRetorno > 0)
                        base.ExibirPainelExcecao("GerencieExtrato.ExtratoRelatorioPrecoUnico", codigoRetorno);
                }
                catch (FaultException<GerencieExtratoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }

                //Retorna a lista
                if (relatoriosPrecoUnico != null)
                    periodosDisponiveis = relatoriosPrecoUnico
                        .Select(relatorio => relatorio.DataInicial).ToList();
                return periodosDisponiveis;
            }

        }

        #endregion
    }
}