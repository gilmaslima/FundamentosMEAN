/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Redecard.PN.Comum;
using Redecard.PN.DataCash.BasePage;
using Redecard.PN.DataCash.controles;
using Redecard.PN.DataCash.controles.comprovantes;
using Redecard.PN.DataCash.Modelo;
using Redecard.PN.DataCash.Modelo.Util;

namespace Redecard.PN.DataCash
{
    /// <summary>
    /// Faça Sua Venda Mobile Rede - Comprovante Crédito
    /// Mobile Rede 2.0 - Novo Leitor de Cartão
    /// </summary>
    public partial class FacaSuaVendaMobileRedeComprovanteCredito : PageBaseDataCash
    {
        #region [ Propriedades/Variáveis ]

        /// <summary>
        /// Chave da QueryString que contém o GUID do objeto de integração com o Portal
        /// para a página NovoLeitorCartaoComprovanteCredito.aspx do projeto Redecard.PN.Boston.
        /// </summary>
        private static String ChaveIntegracaoComprovante { get { return "NovoLeitorCartaoComprovanteCredito"; } }

        /// <summary>
        /// PT-BR
        /// </summary>
        private static CultureInfo PtBr { get { return new CultureInfo("pt-BR"); } }

        /// <summary>JS Serializer</summary>
        private static JavaScriptSerializer jsSerializer;
        private static JavaScriptSerializer JsSerializer
        {
            get { return jsSerializer ?? (jsSerializer = new JavaScriptSerializer()); }
        }

        /// <summary>
        /// Query String
        /// </summary>
        private QueryStringSegura QueryString
        {
            get
            {
                var qs = default(QueryStringSegura);

                try
                {
                    String dados = this.Request.QueryString["dados"];
                    if (!String.IsNullOrEmpty(dados))
                        qs = new QueryStringSegura(dados);
                }
                catch (QueryStringExpiradaException ex)
                {
                    Logger.GravarErro("QueryStringSegura expirada", ex);
                }
                catch (QueryStringInvalidaException ex)
                {
                    Logger.GravarErro("QueryStringSegura inválida", ex);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Erro durante leitura de QueryStringSegura", ex);
                }

                return qs;
            }
        }

        /// <summary>
        /// Parâmetros de Retorno, ficam armazenados em Sessão
        /// Contém os valores de ativação/contratação dos leitores CCM, CPA, CPC, e resultado
        /// da Transação DataCash
        /// </summary>
        private static PedidoNovoLeitorRetorno ParametrosRetorno
        {
            get { return (PedidoNovoLeitorRetorno)HttpContext.Current.Session["ParametrosRetorno"]; }
            set { HttpContext.Current.Session["ParametrosRetorno"] = value; }
        }

        #endregion

        #region [ Controles ]

        /// <summary>
        /// UserControl do Quadro de Aviso
        /// </summary>
        private QuadroAviso UcQuadroAviso { get { return (QuadroAviso)ucQuadroAviso; } }

        #endregion

        /// <summary>
        /// Page Load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Faça Sua Venda Mobile Rede - Comprovante de Crédito"))
            {
                if (!IsPostBack)
                {
                    ParametrosRetorno = RecuperarParametrosRetornoDataCash();
                    
                    log.GravarMensagem("Parâmetros para montagem do comprovante", ParametrosRetorno);

                    if (ParametrosRetorno != null)
                    {
                        //Verifica se a transação DataCash foi realizada com sucesso
                        //Se não houve cobrança, o código retorno foi previamente setado em 1
                        Boolean sucessoDatacash = ParametrosRetorno.CodigoRetorno == 1;

                        //Verifica se a inclusão no WF foi realizada com sucesso
                        Boolean sucessoWf = ParametrosRetorno.CodigoRetornoWf == 0;

                        if (sucessoDatacash)
                        {
                            //TODO: Não estão sendo considerados eventuais erros na
                            //inclusão do pedido no WF. Utilizar a flag "sucessoWF"
                            //caso haja necessidade de customização da mensagem de retorno
                            mvwPrincipal.SetActiveView(viwComprovante);
                            ExibirEnderecoEntrega();
                            ExibirComprovante();
                            ExibirResumoPedido();
                        }
                        else
                        {
                            UcQuadroAviso.CarregarMensagem("DataCashService.TransactionXMLPortal",
                                ParametrosRetorno.CodigoRetorno, "Atenção, transação não aprovada!",
                                String.Empty, QuadroAviso.TipoIcone.Erro);
                            UcQuadroAviso.ExibirVoltarPagina = false;
                            UcQuadroAviso.ExibirVoltar = false;
                            mvwPrincipal.SetActiveView(viwQuadroAviso);
                        }
                    }
                    else
                    {
                        UcQuadroAviso.CarregarMensagem(FONTE, CODIGO_ERRO, "Atenção!",
                                String.Empty, QuadroAviso.TipoIcone.Erro);
                        UcQuadroAviso.ExibirVoltarPagina = false;
                        UcQuadroAviso.ExibirVoltar = false;
                        mvwPrincipal.SetActiveView(viwQuadroAviso);
                    }
                }
            }
        }

        /// <summary>
        /// Tentar Novamente
        /// </summary>
        protected void btnTentarNovamente_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Recupera (do Cache) o objeto contendo os dados de retorno 
        /// da transação do DataCash, através do Guid existente na QueryString
        /// </summary>
        /// <returns>Dados de retorno da transação do DataCash</returns>
        private PedidoNovoLeitorRetorno RecuperarParametrosRetornoDataCash()
        {
            var retorno = default(PedidoNovoLeitorRetorno);
            if (QueryString != null)
            {
                String guid = QueryString[ChaveIntegracaoComprovante];
                if (guid.EmptyToNull() != null)
                {
                    String dadosCache = CacheAdmin.Recuperar<String>(Comum.Cache.DataCashIntegracao, guid);
                    if (dadosCache.EmptyToNull() != null)
                    {
                        CacheAdmin.Remover(Comum.Cache.DataCashIntegracao, guid);
                        retorno = JsSerializer.Deserialize<PedidoNovoLeitorRetorno>(dadosCache);
                    }
                }
            }
            return retorno;
        }

        /// <summary>
        /// Exibe os dados do Comprovante da Venda DataCash
        /// </summary>
        private void ExibirComprovante()
        {
            //Se foi solicitado apenas o produto com Taxa de Aluguel (CPC), não foi realizada transação DataCash
            //Sendo assim, só exibe comprovante se houver venda de leitor (transação DataCash - venda de leitor CCM/CPA)
            Boolean exibirComprovante = ParametrosRetorno.QuantidadeCCM + ParametrosRetorno.QuantidadeCPA > 0;
            
            //Configura qual mensagem inicial será exibida (depende se houve compra ou não de leitor)
            ltrMensagemCompraLeitor.Visible = exibirComprovante;
            ltrMensagemTaxaManutencao.Visible = !exibirComprovante;

            //Só há necessidade de preencher os dados da transação se houve compra de leitor
            pnlComprovanteVenda.Visible = exibirComprovante;
            if (exibirComprovante)
            {
                ltrNSU.Text = ParametrosRetorno.NSU;
                ltrTID.Text = ParametrosRetorno.TID;
                ltrNumeroEstabelecimento.Text = SessaoAtual.CodigoEntidade.ToString();
                ltrNomeEstabelecimento.Text = SessaoAtual.NomeEntidade;
                ltrDataVenda.Text = ParametrosRetorno.DataTransacao.ToString("dd/MM/yyyy");
                ltrHoraVenda.Text = ParametrosRetorno.HoraTransacao.ToString("HH:mm");
                ltrNumeroAutorizacao.Text = ParametrosRetorno.NumeroAutorizacao;
                ltrTipoTransacao.Text = ParametrosRetorno.TipoTransacao;
                ltrFormaPagamento.Text = ParametrosRetorno.FormaPagamento;
                ltrBandeira.Text = ParametrosRetorno.CartaoBandeira;
                ltrNomePortador.Text = ParametrosRetorno.CartaoNomePortador;
                ltrNumeroCartao.Text = ParametrosRetorno.CartaoNumeroCriptografado;
                ltrValor.Text = ParametrosRetorno.ValorFormatado;
                ltrParcelas.Text = ParametrosRetorno.CartaoParcelas;
                ltrNumeroPedido.Text = ParametrosRetorno.NumeroPedido;

                //Só exibe Parcelas, se pagamento não for À Vista
                Boolean aVista = String.Compare(ParametrosRetorno.FormaPagamento, enFormaPagamento.Avista.GetTitle(), true) == 0;
                trParcelas.Visible = !aVista;
            }
        }

        /// <summary>
        /// Exibe os dados do Resumo do Pedido
        /// </summary>
        private void ExibirResumoPedido()
        {
            //Verifica se houve compra de leitor, e se houve contratação de aluguel
            Boolean compraLeitor = ParametrosRetorno.QuantidadeCCM + ParametrosRetorno.QuantidadeCPA > 0;
            Boolean taxaManutencao = ParametrosRetorno.QuantidadeCPC > 0;

            //Configura o layout do Resumo do Pedido (depende se houve compra ou não de leitor)
            pnlResumoCompraLeitor.Visible = compraLeitor;
            pnlResumoTaxaManutencao.Visible = taxaManutencao;

            PedidoNovoLeitor Parametros = ParametrosRetorno.Parametros;

            //Valor total da compra (apenas CCM e CPA, pois CPC é aluguel)
            Decimal valorTotal = CalcularValorTotal(ParametrosRetorno.QuantidadeCCM, 
                ParametrosRetorno.QuantidadeCPA, ParametrosRetorno.QuantidadeCPC);

            //Prepara objeto de retorno para o Resumo do Pedido - Modelo Taxa de Manutenção
            var resumoManutencao = new
            {
                CPC = new
                {
                    Quantidade = ParametrosRetorno.QuantidadeCPC.ToString("D3"),
                    Valor = ParametrosRetorno.QuantidadeCPC > 1 ?
                        String.Format("{0} x {1}", Parametros.TaxaAtivacaoCPC.ToString("N2", PtBr), ParametrosRetorno.QuantidadeCPC) :
                        (Parametros.TaxaAtivacaoCPC * ParametrosRetorno.QuantidadeCPC).ToString("N2", PtBr)
                },
                ValorTotal = (ParametrosRetorno.QuantidadeCPC * Parametros.TaxaAtivacaoCPC).ToString("N2", PtBr)
            };

            //Prepara objeto de retorno para o Resumo do Pedido - Modelo Pagamento Único
            var resumoPagamentoUnico = new
            {
                CCM = new
                {
                    Quantidade = ParametrosRetorno.QuantidadeCCM.ToString("D3"),
                    Valor = ParametrosRetorno.QuantidadeCCM > 1 ?
                        String.Format("{0} x {1}", Parametros.TaxaAtivacaoCCM.ToString("N2", PtBr), ParametrosRetorno.QuantidadeCCM) :
                        (Parametros.TaxaAtivacaoCCM * ParametrosRetorno.QuantidadeCCM).ToString("N2", PtBr)
                },
                CPA = new
                {
                    Quantidade = ParametrosRetorno.QuantidadeCPA.ToString("D3"),
                    Valor = ParametrosRetorno.QuantidadeCPA > 1 ?
                        String.Format("{0} x {1}", Parametros.TaxaAtivacaoCPA.ToString("N2", PtBr), ParametrosRetorno.QuantidadeCPA) :
                        (Parametros.TaxaAtivacaoCPA * ParametrosRetorno.QuantidadeCPA).ToString("N2", PtBr)
                },
                ValorTotal = (valorTotal).ToString("N2", PtBr)
            };

            ltrResumoCPCQtd.Text = resumoManutencao.CPC.Quantidade;
            ltrResumoCPCValor.Text = resumoManutencao.CPC.Valor;

            ltrResumoCPAQtd.Text = resumoPagamentoUnico.CPA.Quantidade;
            ltrResumoCPAValor.Text = resumoPagamentoUnico.CPA.Valor;

            ltrResumoCCMQtd.Text = resumoPagamentoUnico.CCM.Quantidade;
            ltrResumoCCMValor.Text = resumoPagamentoUnico.CCM.Valor;

            ltrResumoManutencaoTotal.Text = resumoManutencao.ValorTotal;
            ltrResumoPagamentoUnicoTotal.Text = resumoPagamentoUnico.ValorTotal;
        }

        /// <summary>
        /// Carrega os dados do Endereço de Entrega
        /// </summary>
        private void ExibirEnderecoEntrega()
        {
            PedidoNovoLeitor parametros = ParametrosRetorno.Parametros;

            StringBuilder linha1 = new StringBuilder(parametros.EnderecoLogradouro);
            if (parametros.EnderecoNumero.EmptyToNull() != null)
                linha1.Append(", ").Append(parametros.EnderecoNumero);
            if (parametros.EnderecoComplemento.EmptyToNull() != null)
                linha1.Append(" - ").Append(parametros.EnderecoComplemento);

            ltrEnderecoLinha1.Text = linha1.ToString();
            ltrEnderecoLinha2.Text = String.Format("{0}, {1}, {2} - {3}",
                parametros.EnderecoBairro, parametros.EnderecoCidade, parametros.EnderecoEstado, parametros.EnderecoCEP);
            ltrEnderecoLinha3.Text = "Brasil";
        }

        #region [ Exportação PDF ]

        /// <summary>
        /// Prepara conteúdo de exportação PDF/Excel
        /// </summary>
        protected TabelaExportacao ObterTabelaExportacao()
        {
            if (ParametrosRetorno != null)
            {
                //Se foi solicitado apenas o produto com Taxa de Aluguel (CPC), não foi realizada transação DataCash
                //Sendo assim, só exibe comprovante se houver venda de leitor (transação DataCash - venda de leitor CCM/CPA)
                Boolean exibirComprovante = ParametrosRetorno.QuantidadeCCM + ParametrosRetorno.QuantidadeCPA > 0;

                TabelaExportacao tabela = new TabelaExportacao();
                tabela.Titulo = String.Format("Novo Leitor de Cartão - Comprovante ({0} - {1})",
                        SessaoAtual.NomeEntidade, SessaoAtual.CodigoEntidade);
                tabela.ModoRetrato = true;
                tabela.LarguraTabela = 60;
                tabela.Colunas = new[] {
                            new Coluna("Descrição").SetAlinhamento(HorizontalAlign.Right),
                            new Coluna("Valor").SetAlinhamento(HorizontalAlign.Left).SetBordaInterna(false) };
                tabela.FuncaoValores = (registro) => {
                    var item = registro as Tuple<String, String>;
                    return new[] { item.Item1, item.Item2 };
                };
                tabela.ExibirTituloColunas = false;

                if (exibirComprovante)
                    tabela.Registros = ObtemItensComprovante();
                else
                    tabela.Registros = new List<Tuple<String, String>>();


                return tabela;
            }
            else
                return null;
        }

        /// <summary>
        /// Gera itens para exportação do Comprovante
        /// </summary>
        private IEnumerable<Object> ObtemItensComprovante()
        {
            var registros = new List<Tuple<String, String>>(new[] {
                new Tuple<String, String>("Nº do Comprovante de vendas (NSU)", ltrNSU.Text),
                new Tuple<String, String>("TID", ltrTID.Text),
                new Tuple<String, String>("N° Estabelecimento", ltrNumeroEstabelecimento.Text),
                new Tuple<String, String>("Nome Estabelecimento", ltrNomeEstabelecimento.Text),
                new Tuple<String, String>("Data do Pagamento", ltrDataVenda.Text),
                new Tuple<String, String>("Hora do Pagamento", ltrHoraVenda.Text),
                new Tuple<String, String>("N° Autorização", ltrNumeroAutorizacao.Text),
                new Tuple<String, String>("Tipo de Transação", ltrTipoTransacao.Text),
                new Tuple<String, String>("Forma de Pagamento", ltrFormaPagamento.Text),
                new Tuple<String, String>("Bandeira", ltrBandeira.Text),
                //new Tuple<String, String>("Nome do Portador", ltrNomePortador.Text),
                new Tuple<String, String>("N° Cartão (Últimos 4 díg.)", ltrNumeroCartao.Text),
                new Tuple<String, String>("Valor", ltrValor.Text),
            });

            //Só exibe Parcelas, se pagamento não for À Vista
            if(trParcelas.Visible)
                registros.Add(new Tuple<String, String>("N° Parcelas", ltrParcelas.Text));

            registros.Add(new Tuple<String, String>("N° Pedido", ltrNumeroPedido.Text));

            return registros;
        }

        /// <summary>
        /// Geração de conteúdo para Exportação
        /// </summary>
        /// <returns>PdfPTable</returns>
        public List<PdfPTable> ExportarConteudoCustomizado()
        {
            PdfPTable tabelaResumo = new PdfPTable(1) {
                WidthPercentage = 28,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            tabelaResumo.DefaultCell.Border = Rectangle.NO_BORDER;

            String logoPath = Server.MapPath(@"images\carrinho_de_compras.png");
            iTextSharp.text.Image imageHeader = iTextSharp.text.Image.GetInstance(File.ReadAllBytes(logoPath));
            imageHeader.Alignment = iTextSharp.text.Image.LEFT_ALIGN;
            imageHeader.Border = 0;
            tabelaResumo.AddCell(new PdfPCell(imageHeader, true) {
                Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER,
                PaddingTop = 5
            });

            //Modelo Taxa de Manutenção
            {
                if (pnlResumoTaxaManutencao.Visible)
                {
                    PdfPTable tableTaxaManutencao = new PdfPTable(3);
                    tabelaResumo.AddCell(new PdfPCell(tableTaxaManutencao) {
                        Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER,
                        Padding = 5
                    });

                    tableTaxaManutencao.AddCell(new PdfPCell(new Phrase("Modelo Taxa de Manutenção",
                            new Font(Font.FontFamily.HELVETICA, 6, 1, new BaseColor(8, 80, 146)))) { Border = 0, Colspan = 3 });

                    tableTaxaManutencao.AddCell(new PdfPCell(new Phrase("Qtde.",
                            new Font(Font.FontFamily.HELVETICA, 6, 1, new BaseColor(64, 64, 64)))) { Border = 0 });
                    tableTaxaManutencao.AddCell(new PdfPCell(new Phrase("Dispositivo",
                            new Font(Font.FontFamily.HELVETICA, 6, 1, new BaseColor(64, 64, 64)))) { Border = 0 });
                    tableTaxaManutencao.AddCell(new PdfPCell(new Phrase("Valor Total",
                            new Font(Font.FontFamily.HELVETICA, 6, 1, new BaseColor(64, 64, 64)))) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });

                    tableTaxaManutencao.AddCell(new PdfPCell(new Phrase(ltrResumoCPCQtd.Text,
                            new Font(Font.FontFamily.HELVETICA, 6, 0, new BaseColor(64, 64, 64)))) { Border = 0, NoWrap = true });
                    tableTaxaManutencao.AddCell(new PdfPCell(new Phrase("Leitor de Chip",
                            new Font(Font.FontFamily.HELVETICA, 6, 0, new BaseColor(64, 64, 64)))) { Border = 0, NoWrap = true });
                    tableTaxaManutencao.AddCell(new PdfPCell(new Phrase(ltrResumoCPCValor.Text ,
                            new Font(Font.FontFamily.HELVETICA, 6, 0, new BaseColor(64, 64, 64)))) { Border = 0, NoWrap = true, HorizontalAlignment = Element.ALIGN_RIGHT });

                    tableTaxaManutencao.AddCell(new PdfPCell(new Phrase(String.Empty,
                            new Font(Font.FontFamily.HELVETICA, 6, 1, new BaseColor(64, 64, 64)))) { Border = 0 });
                    tableTaxaManutencao.AddCell(new PdfPCell(new Phrase("Total por mês",
                            new Font(Font.FontFamily.HELVETICA, 6, 1, new BaseColor(64, 64, 64)))) { Border = 0, NoWrap = true });
                    tableTaxaManutencao.AddCell(new PdfPCell(new Phrase(ltrResumoManutencaoTotal.Text,
                            new Font(Font.FontFamily.HELVETICA, 6, 1, new BaseColor(64, 64, 64)))) { Border = 0, NoWrap = true, HorizontalAlignment = Element.ALIGN_RIGHT });
                }
            }

            //Modelo Pagamento único
            {
                if (pnlResumoCompraLeitor.Visible)
                {
                    PdfPTable tablePagamentoUnico = new PdfPTable(3);
                    tabelaResumo.AddCell(new PdfPCell(tablePagamentoUnico) {
                        Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER,
                        Padding = 5 });

                    tablePagamentoUnico.AddCell(new PdfPCell(new Phrase("Modelo Pagamento Único",
                            new Font(Font.FontFamily.HELVETICA, 6, 1, new BaseColor(8, 80, 146)))) { Border = 0, Colspan = 3 });
                    tablePagamentoUnico.AddCell(new PdfPCell(new Phrase("Qtde.",
                            new Font(Font.FontFamily.HELVETICA, 6, 1, new BaseColor(64, 64, 64)))) { Border = 0 });
                    tablePagamentoUnico.AddCell(new PdfPCell(new Phrase("Dispositivo",
                            new Font(Font.FontFamily.HELVETICA, 6, 1, new BaseColor(64, 64, 64)))) { Border = 0 });
                    tablePagamentoUnico.AddCell(new PdfPCell(new Phrase("Valor Total",
                            new Font(Font.FontFamily.HELVETICA, 6, 1, new BaseColor(64, 64, 64)))) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });

                    tablePagamentoUnico.AddCell(new PdfPCell(new Phrase(ltrResumoCCMQtd.Text,
                            new Font(Font.FontFamily.HELVETICA, 6, 0, new BaseColor(64, 64, 64)))) { Border = 0, NoWrap = true });
                    tablePagamentoUnico.AddCell(new PdfPCell(new Phrase("Leitor de Tarja",
                            new Font(Font.FontFamily.HELVETICA, 6, 0, new BaseColor(64, 64, 64)))) { Border = 0, NoWrap = true });
                    tablePagamentoUnico.AddCell(new PdfPCell(new Phrase(ltrResumoCCMValor.Text,
                            new Font(Font.FontFamily.HELVETICA, 6, 0, new BaseColor(64, 64, 64)))) { Border = 0, NoWrap = true, HorizontalAlignment = Element.ALIGN_RIGHT });

                    tablePagamentoUnico.AddCell(new PdfPCell(new Phrase(ltrResumoCPAQtd.Text,
                            new Font(Font.FontFamily.HELVETICA, 6, 0, new BaseColor(64, 64, 64)))) { Border = 0, NoWrap = true });
                    tablePagamentoUnico.AddCell(new PdfPCell(new Phrase("Leitor de Chip",
                            new Font(Font.FontFamily.HELVETICA, 6, 0, new BaseColor(64, 64, 64)))) { Border = 0, NoWrap = true });
                    tablePagamentoUnico.AddCell(new PdfPCell(new Phrase(ltrResumoCPAValor.Text,
                            new Font(Font.FontFamily.HELVETICA, 6, 0, new BaseColor(64, 64, 64)))) { Border = 0, NoWrap = true, HorizontalAlignment = Element.ALIGN_RIGHT });

                    tablePagamentoUnico.AddCell(new PdfPCell(new Phrase(String.Empty,
                            new Font(Font.FontFamily.HELVETICA, 6, 1, new BaseColor(64, 64, 64)))) { Border = 0 });
                    tablePagamentoUnico.AddCell(new PdfPCell(new Phrase("Total pago agora",
                            new Font(Font.FontFamily.HELVETICA, 6, 1, new BaseColor(64, 64, 64)))) { Border = 0, NoWrap = true });
                    tablePagamentoUnico.AddCell(new PdfPCell(new Phrase(ltrResumoPagamentoUnicoTotal.Text,
                            new Font(Font.FontFamily.HELVETICA, 6, 1, new BaseColor(64, 64, 64)))) { Border = 0, NoWrap = true, HorizontalAlignment = Element.ALIGN_RIGHT });
                }
            }

            //Endereço de entrega
            {
                PdfPTable tableEndereco = new PdfPTable(1);
                tabelaResumo.AddCell(new PdfPCell(tableEndereco) {
                    Padding = 5,
                    Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER });

                tableEndereco.AddCell(new PdfPCell(new Phrase("Endereço de Entrega",
                        new Font(Font.FontFamily.HELVETICA, 6, 1, new BaseColor(8, 80, 146)))) { Border = 0 });
                tableEndereco.AddCell(new PdfPCell(new Phrase(ltrEnderecoLinha1.Text,
                        new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL, new BaseColor(64, 64, 64)))) { Border = 0 });
                tableEndereco.AddCell(new PdfPCell(new Phrase(ltrEnderecoLinha2.Text,
                        new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL, new BaseColor(64, 64, 64)))) { Border = 0 });
                tableEndereco.AddCell(new PdfPCell(new Phrase(ltrEnderecoLinha3.Text,
                        new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL, new BaseColor(64, 64, 64)))) { Border = 0 });
            }

            return new[] { tabelaResumo }.ToList();
        }

        #endregion

        /// <summary>
        /// Calcula o valor total da compra.
        /// </summary>
        /// <param name="quantidadeCCM">Quantidade de leitores CCM</param>
        /// <param name="quantidadeCPA">Quantidade de leitores CPA</param>
        /// <param name="quantidadeCPC">Quantidade de leitors CPC</param>
        /// <returns>Valor total do pedido a ser cobrado no E-Rede</returns>
        private static Decimal CalcularValorTotal(Int32 quantidadeCCM, Int32 quantidadeCPA, Int32 quantidadeCPC)
        {
            //Valor total da compra (apenas CCM e CPA, pois CPC é aluguel)
            return quantidadeCCM * ParametrosRetorno.Parametros.TaxaAtivacaoCCM + 
                quantidadeCPA * ParametrosRetorno.Parametros.TaxaAtivacaoCPA;
        }
    }
}