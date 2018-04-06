using Redecard.PN.Comum;
using Redecard.PN.Extrato.Core.Web.Controles.Portal;
using Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato.RelatorioCreditoSuspensosRetidosPenhorados;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Extrato.SharePoint.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.SharePoint.WebParts.CredSuspRetPen.CredSuspRetPen
{
    public partial class CredSuspRetPenUserControl : BaseUserControl, IRelatorioHandler
    {
        #region InnerClasses
        private class InfoPaginacao
        {
            public Int32 PaginaAtual { get; set; }
            public Int32 QtdRegsPagina { get; set; }
            public InfoPaginacao(Int32 paginaAtual, Int32 qtdRegsPagina)
            {
                this.PaginaAtual = paginaAtual;
                this.QtdRegsPagina = qtdRegsPagina;
            }
        }
        #endregion

        #region Event
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (ViewState["buscarRealizado"] != null)
            {
                BuscarDados buscarDados = (BuscarDados)ViewState["buscarRealizado"];

                InfoPaginacao outSave;

                this.InternalConsultar(buscarDados,
                                        null, null, null, null,
                                        out outSave, out outSave, out outSave, out outSave,
                                        false);
            }
        }

        protected String objBuscar_onObterConteudoHTMLDownload(BuscarDados buscarDados, Boolean recomporTela)
        {

            InfoPaginacao ipCreditosSave;
            InfoPaginacao ipDebitosSave;
            InfoPaginacao ipRetencoesSave;
            InfoPaginacao ipPenhorasSave;

            InfoPaginacao ipTudo = new InfoPaginacao(1, Constantes.MAX_LINHAS_DOWNLOAD);
            InternalConsultar(buscarDados,
                ipTudo, ipTudo, ipTudo, ipTudo,
                out ipCreditosSave, out ipDebitosSave, out ipRetencoesSave, out ipPenhorasSave,
                true
            );
            IControlesParaDownload creditoSuspenso = this.CreditoSuspenso1 as IControlesParaDownload;
            IControlesParaDownload debitoSuspenso = this.DebitoSuspenso1 as IControlesParaDownload;
            IControlesParaDownload retidos = this.Retidos1 as IControlesParaDownload;
            IControlesParaDownload penhorados = this.Penhorados1 as IControlesParaDownload;

            List<Control> ctrList = new List<Control>();
            ctrList.AddRange(creditoSuspenso.ObterControlesParaDownload());
            ctrList.AddRange(debitoSuspenso.ObterControlesParaDownload());
            ctrList.AddRange(retidos.ObterControlesParaDownload());
            ctrList.AddRange(penhorados.ObterControlesParaDownload());

            Control container = new PlaceHolder();
            foreach (Control ctr in ctrList)
            {
                container.Controls.Add(ctr);
            }
            String result = ObterHTMLControle(container); // outTabela
            if (recomporTela)
            {
                InternalConsultar(buscarDados,
                    ipCreditosSave, ipDebitosSave, ipRetencoesSave, ipPenhorasSave,
                    out ipCreditosSave, out ipDebitosSave, out ipRetencoesSave, out ipPenhorasSave,
                    true
                );
            }
            return result;
        }

        private void objBuscar_onBuscar(BuscarDados buscarDados, EventArgs e)
        {
            InfoPaginacao ipCreditosSave;
            InfoPaginacao ipDebitosSave;
            InfoPaginacao ipRetencoesSave;
            InfoPaginacao ipPenhorasSave;
            InternalConsultar(buscarDados,
                null, null, null, null,
                out ipCreditosSave, out ipDebitosSave, out ipRetencoesSave, out ipPenhorasSave,
                false
            );
        }


        /// <summary>
        /// Faz a consulta aos serviços e recupera as páginas atuais caso precise remontar a tela após download ou envio de emails
        /// </summary>
        /// <param name="buscarDados">Objeto Buscar Dados</param>
        /// <param name="ipCreditos">Pagina e Quantidade de linhas desejada do controle de creditos. null para trazer a pagina selecionada no controle de paginaçao e quantidade de linhas no controle de linhas</param>
        /// <param name="ipDebitos">Pagina e Quantidade de linhas desejada do controle de debitos. null para trazer a pagina selecionada no controle de paginaçao e quantidade de linhas no controle de linhas</param>
        /// <param name="ipRetencoes">Pagina e Quantidade de linhas desejada do controle de retencoes. null para trazer a pagina selecionada no controle de paginaçao e quantidade de linhas no controle de linhas</param>
        /// <param name="ipPenhorados">Pagina e Quantidade de linhas desejada do controle de penhoras. null para trazer a pagina selecionada no controle de paginaçao e quantidade de linhas no controle de linhas</param>
        /// <param name="ipCreditosSave">Retorna a pagina atual e a quantidade de linhas atuais do Creditos</param>
        /// <param name="ipDebitosSave">Retorna a pagina atual e a quantidade de linhas atuais do Debitos</param>
        /// <param name="ipRetidosSave">Retorna a pagina atual e a quantidade de linhas atuais do Retencoes</param>
        /// <param name="ipPenhoradosSave">Retorna a pagina atual e a quantidade de linhas atuais do Penhoras</param>
        /// <param name="lancarException">Lança exception em caso de erro ao invés de simplesmente exibir o box. Serve para processar o Download, que tem que sair com exceçao para que a mesma seja tratada por aquele método</param>
        private void InternalConsultar(BuscarDados buscarDados,
                                        InfoPaginacao ipCreditos,
                                        InfoPaginacao ipDebitos,
                                        InfoPaginacao ipRetidos,
                                        InfoPaginacao ipPenhorados,
                                        out InfoPaginacao ipCreditosSave,
                                        out InfoPaginacao ipDebitosSave,
                                        out InfoPaginacao ipRetidosSave,
                                        out InfoPaginacao ipPenhoradosSave,
                                        bool lancarException)
        {
            // Defaults de saida caso dê erro
            ipCreditosSave = new InfoPaginacao(-1, -1);
            ipDebitosSave = new InfoPaginacao(-1, -1);
            ipRetidosSave = new InfoPaginacao(-1, -1);
            ipPenhoradosSave = new InfoPaginacao(-1, -1);

            Int32 intPaginaSave;
            Int32 intQtdPaginasSave;

            // Defaults caso venha null (página atual)
            ipCreditos = ipCreditos ?? new InfoPaginacao(-1, -1);
            ipDebitos = ipDebitos ?? new InfoPaginacao(-1, -1);
            ipRetidos = ipRetidos ?? new InfoPaginacao(-1, -1);
            ipPenhorados = ipPenhorados ?? new InfoPaginacao(-1, -1);

            try
            {
                // Credito Suspensos
                CreditoSuspenso creditoSuspenso = (CreditoSuspenso)this.CreditoSuspenso1;

                creditoSuspenso.Consultar(buscarDados, ipCreditos.PaginaAtual, ipCreditos.QtdRegsPagina, out intPaginaSave, out intQtdPaginasSave);
                ipCreditosSave.PaginaAtual = intPaginaSave;
                ipCreditosSave.QtdRegsPagina = intQtdPaginasSave;

                var valorSuspensosCredito = String.Empty;
                if (creditoSuspenso.Totais != null)
                {
                    //Total de Valores Crédito
                    valorSuspensosCredito = creditoSuspenso.Totais.TotalValorSuspencao.ToString("N2");
                }
                else
                {
                    //Total de Valores Crédito
                    valorSuspensosCredito = 0.0.ToString("N2");
                }

                // Debitos Suspensos
                DebitoSuspenso debitoSuspenso = (DebitoSuspenso)this.DebitoSuspenso1;
                debitoSuspenso.Consultar(buscarDados, ipDebitos.PaginaAtual, ipDebitos.QtdRegsPagina, out intPaginaSave, out intQtdPaginasSave);
                ipDebitosSave.PaginaAtual = intPaginaSave;
                ipDebitosSave.QtdRegsPagina = intQtdPaginasSave;
                var valorSuspensosDebito = String.Empty;
                if (debitoSuspenso.Totais != null)
                {
                    //Total de Valores Débito
                    valorSuspensosDebito = debitoSuspenso.Totais.TotalValorSuspencao.ToString("N2");
                }
                else
                {
                    //Total de Valores Débito
                    valorSuspensosDebito = 0.0.ToString("N2");
                }

                this.qiValoresConsolidadosSuspensos.QuadroInformacaoItems.Clear();
                this.qiValoresConsolidadosSuspensos.QuadroInformacaoItems.Add(new QuadroInformacaoItem
                {
                    Descricao = "total de valores suspensos no crédito",
                    Valor = valorSuspensosCredito
                });
                this.qiValoresConsolidadosSuspensos.QuadroInformacaoItems.Add(new QuadroInformacaoItem
                {
                    Descricao = "total de valores suspensos no débito",
                    Valor = valorSuspensosDebito
                });
                divRelatorioValores.Visible = true;


                // Retidos
                Retidos retidos = (Retidos)this.Retidos1;
                retidos.Consultar(buscarDados, ipRetidos.PaginaAtual, ipRetidos.QtdRegsPagina, out intPaginaSave, out intQtdPaginasSave);
                ipRetidosSave.PaginaAtual = intPaginaSave;
                ipRetidosSave.QtdRegsPagina = intQtdPaginasSave;

                // Penhorados
                Penhorados penhorados = (Penhorados)this.Penhorados1;
                penhorados.Consultar(buscarDados, ipPenhorados.PaginaAtual, ipPenhorados.QtdRegsPagina, out intPaginaSave, out intQtdPaginasSave);
                ipPenhoradosSave.PaginaAtual = intPaginaSave;
                ipPenhoradosSave.QtdRegsPagina = intQtdPaginasSave;

                this.qiValoresConsolidadosRetidos.QuadroInformacaoItems.Clear();
                this.qiValoresConsolidadosRetidos.QuadroInformacaoItems.Add(new QuadroInformacaoItem
                {
                    Descricao = "total de valores a reter",
                    Valor = retidos.TotalValoresReter.ToString("C2")
                });
                this.qiValoresConsolidadosRetidos.QuadroInformacaoItems.Add(new QuadroInformacaoItem
                {
                    Descricao = "total de valores retidos",
                    Valor = retidos.TotalValoresRetidos.ToString("C2")
                });

                this.qiValoresConsolidadosPenhorados.QuadroInformacaoItems.Clear();
                this.qiValoresConsolidadosPenhorados.QuadroInformacaoItems.Add(new QuadroInformacaoItem
                {
                    Descricao = "total de valores a penhorar",
                    Valor = penhorados.TotalValoresPenhorar.ToString("C2")
                });
                this.qiValoresConsolidadosPenhorados.QuadroInformacaoItems.Add(new QuadroInformacaoItem
                {
                    Descricao = "total de valores penhorados",
                    Valor = penhorados.TotalValoresPenhorados.ToString("C2")
                });



                ViewState["buscarRealizado"] = buscarDados;

                //Verifica os controles que devem estar visíveis
                base.VerificaControlesVisiveis(creditoSuspenso.objPaginacao.QuantidadeTotalRegistros +
                                                debitoSuspenso.objPaginacao.QuantidadeTotalRegistros +
                                                retidos.objPaginacao.QuantidadeTotalRegistros +
                                                penhorados.objPaginacao.QuantidadeTotalRegistros, null, null);
            }
            catch (PortalRedecardException ex)
            {
                if (lancarException)
                {
                    throw ex;
                }
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                if (lancarException)
                {
                    throw ex;
                }
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }
        #endregion

        #region Method
        private Servico.CSP.ConsultarRetencaoEnvio TradudorEnvioSPParaServico(BuscarDados BuscarDados)
        {
            Servico.CSP.ConsultarRetencaoEnvio objEnvio = new Servico.CSP.ConsultarRetencaoEnvio();
            objEnvio.DataInicial = BuscarDados.DataInicial;
            objEnvio.DataFinal = BuscarDados.DataFinal;
            objEnvio.CodigoBandeira = BuscarDados.CodigoBandeira;
            objEnvio.Estabelecimentos = BuscarDados.Estabelecimentos;
            return objEnvio;
        }



        private Servico.CSP.ConsultarPenhoraEnvio TradudorEnvioPenhoradosSPParaServico(BuscarDados BuscarDados)
        {
            Servico.CSP.ConsultarPenhoraEnvio objEnvio = new Servico.CSP.ConsultarPenhoraEnvio();
            objEnvio.DataInicial = BuscarDados.DataInicial;
            objEnvio.DataFinal = BuscarDados.DataFinal;
            objEnvio.CodigoBandeira = BuscarDados.CodigoBandeira;
            objEnvio.Estabelecimentos = BuscarDados.Estabelecimentos;
            return objEnvio;
        }
        #endregion

        #region [ Implementações ]
        public void Pesquisar(BuscarDados dados)
        {
            objBuscar_onBuscar(dados, new EventArgs());
        }

        public string IdControl
        {
            get { return "CredSuspRetPenUserControl_ascx"; }
        }

        /// <summary>
        /// Retorna uma tabela HTML com os dados 
        /// </summary>
        public String ObterTabelaExcel(BuscarDados dados, Int32 quantidadeRegistros, Boolean incluirTotalizadores)
        {
            // Chamar método de consulta
            InfoPaginacao ipCreditosSave;
            InfoPaginacao ipDebitosSave;
            InfoPaginacao ipRetencoesSave;
            InfoPaginacao ipPenhorasSave;

            InfoPaginacao ipTudo = new InfoPaginacao(1, Constantes.MAX_LINHAS_DOWNLOAD);


            InternalConsultar(dados,
                ipTudo, ipTudo, ipTudo, ipTudo,
                out ipCreditosSave, out ipDebitosSave, out ipRetencoesSave, out ipPenhorasSave,
                false
            );

            //oculta controles para impressão
            new Control[] {
                this.CreditoSuspenso1.FindControl("divRegistrosPorPagina"),
                this.DebitoSuspenso1.FindControl("divRegistrosPorPagina"),
                this.Retidos1.FindControl("divRegistrosPorPagina"),
                this.Penhorados1.FindControl("divRegistrosPorPagina") 
            }.Where(controle => controle != null).ToList().ForEach(controle => controle.Visible = false);

            if (incluirTotalizadores)
                return base.RenderizarControles(true, divRelatorioValores, CreditoSuspenso1, DebitoSuspenso1, Retidos1, Penhorados1);
            else
                return base.RenderizarControles(true, CreditoSuspenso1, DebitoSuspenso1, Retidos1, Penhorados1);
        }
        #endregion

        protected void btnVoltar_Click(object sender, EventArgs e)
        {            
            Response.Redirect("pn_default.aspx");
        }
    }
}
