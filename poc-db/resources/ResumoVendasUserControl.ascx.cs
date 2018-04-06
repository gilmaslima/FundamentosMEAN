using System;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato.ResumoVendas;
using Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.ResumoVendas;

namespace Redecard.PN.Extrato.SharePoint.ResumoVendas
{
    public partial class ResumoVendasUserControl : UserControlBase
    {
        /// <summary>
        /// ResumoVendasSelecionarTipoVenda1 control.
        /// </summary>
        protected ResumoVendasSelecionarTipoVenda ResumoVendasSelecionarTipoVenda1 
        { 
            get { return (ResumoVendasSelecionarTipoVenda)resumoVendasSelecionarTipoVenda1; } 
        }

        /// <summary>
        /// ResumoVendasSelecionarEstabelecimento1 control.
        /// </summary>
        protected ResumoVendasSelecionarEstabelecimento ResumoVendasSelecionarEstabelecimento1
        {
            get { return (ResumoVendasSelecionarEstabelecimento)ResumoVendasSelecionarEstabelecimento1; }
        }

        /// <summary>
        /// ResumoVendasCredito control.
        /// </summary>
        protected Credito ResumoVendasCredito
        {
            get { return (Credito)resumoVendasCredito;}
        }

        /// <summary>
        /// ResumoVendasDebito control.
        /// </summary>
        protected Debito ResumoVendasDebito
        {
            get { return (Debito)resumoVendasDebito; }
        }

        /// <summary>
        /// ResumoVendasConstrucard control.
        /// </summary>
        protected Construcard ResumoVendasConstrucard
        {
            get { return (Construcard)resumoVendasConstrucard; }
        }

        /// <summary>
        /// ResumoVendasRecargaCelular control.
        /// </summary>
        protected RecargaCelular ResumoVendasRecargaCelular
        {
            get { return (RecargaCelular)resumoVendasRecargaCelular; }
        }

        /// <summary>
        /// Enumerador para as Views do Resumo de Vendas
        /// </summary>
        private enum ViewResumoVenda
        {
            TipoResumoVenda = 0,
            PV = 1,
            Credito = 2,
            Debito = 3,
            CDC = 4,
            RecargaCelular = 5
        }

        /// <summary>
        /// Evento de Page_Load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    String dados = Request.QueryString["dados"];
                    
                    if (!String.IsNullOrEmpty(dados))
                    {
                        QueryStringSegura queryString = new QueryStringSegura(dados);

                        String tipoRelatorio = queryString["tipoVenda"];
                        Int32 numeroResumoVenda = queryString["numeroResumoVenda"].ToInt32(0);
                        Int32 numeroEstabelecimento = queryString["numeroEstabelecimento"].ToInt32(0);
                        DateTime dataApresentacao = queryString["dataApresentacao"].ToDate("dd/MM/yyyy");

                        ResumoVendaDadosConsultaDTO dto = new ResumoVendaDadosConsultaDTO();
                        dto.NumeroEstabelecimento = numeroEstabelecimento;
                        dto.NumeroResumoVenda = numeroResumoVenda;
                        dto.DataApresentacao = dataApresentacao;

                        ViewState["TipoRelatorio"] = tipoRelatorio.ToUpper();

                        ConsultarResumoVenda(dto);
                    }
                    else
                    {
                        AlterarViewAtual(ViewResumoVenda.TipoResumoVenda);
                    }
                }
                else
                {
                    multiViewResumoVenda.ActiveViewIndex = (Int32)ObterViewAtual();
                }                                
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro Resumo de Vendas", ex);
                if (Request.QueryString["mostrarErro"] != null)
                {
                    throw;
                }
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Item selecionado
        /// </summary>
        protected void objResumoVendaDetalheUserControl_onItemSelecionado(String tipoRelatorio, EventArgs e)
        {
            ViewState["TipoRelatorio"] = tipoRelatorio;

            AlterarViewAtual(ViewResumoVenda.PV);
        }

        /// <summary>
        /// Item selecionado
        /// </summary>
        protected void objResumoVendaPVUserControl_onItemSelecionado(ResumoVendaDadosConsultaDTO dadosConsultaDTO, EventArgs e)
        {
            ConsultarResumoVenda(dadosConsultaDTO);
        }

        /// <summary>
        /// Preenche o controle com dados de acordo com o tipo do resumo de vendas
        /// </summary>
        private void ConsultarResumoVenda(ResumoVendaDadosConsultaDTO dadosConsultaDTO)
        {
            try
            {
                String tipoRelatorio = ViewState["TipoRelatorio"].ToString();

                if (String.Compare("C", tipoRelatorio, true) == 0)
                {
                    ResumoVendasCredito.ConsultarResumoVenda(dadosConsultaDTO);
                    AlterarViewAtual(ViewResumoVenda.Credito);
                }
                else if (String.Compare("D", tipoRelatorio, true) == 0)
                {
                    ResumoVendasDebito.ConsultarResumoVenda(dadosConsultaDTO);
                    AlterarViewAtual(ViewResumoVenda.Debito);
                }
                else if (String.Compare("CDC", tipoRelatorio, true) == 0)
                {
                    ResumoVendasConstrucard.ConsultarResumoVenda(dadosConsultaDTO);
                    AlterarViewAtual(ViewResumoVenda.CDC);
                }
                else if (String.Compare("RC", tipoRelatorio, true) == 0)
                {
                    ResumoVendasRecargaCelular.ConsultarResumoVenda(dadosConsultaDTO);
                    AlterarViewAtual(ViewResumoVenda.RecargaCelular);
                }
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Altera a view atual
        /// </summary>
        private void AlterarViewAtual(ViewResumoVenda view)
        {
            ViewState["ViewAtual"] = view;

            multiViewResumoVenda.ActiveViewIndex = (int)view;
        }

        /// <summary>
        /// Obtém a view atual
        /// </summary>
        private ViewResumoVenda ObterViewAtual()
        {
            ViewResumoVenda view;

            if (ViewState["ViewAtual"] == null)
                view = ViewResumoVenda.TipoResumoVenda;
            else
                view = (ViewResumoVenda)ViewState["ViewAtual"];

            return view;
        }
    }
}
