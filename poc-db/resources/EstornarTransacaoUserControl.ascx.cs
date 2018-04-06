using Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico;
using Rede.PN.Eadquirencia.Sharepoint.Helper;
using Redecard.PN.Comum;
using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Rede.PN.Eadquirencia.Sharepoint.Webparts.EstornarTransacao
{

    public partial class EstornarTransacaoUserControl : WebpartBase
    {
        /// <summary>
        /// ViewState que guarda o retorno da Query.
        /// </summary>
        public QueryResponse ResponseQueryViewState
        {
            get
            {
                return ViewState["responseQuery"] as QueryResponse ?? new QueryResponse();
            }
            set
            {
                ViewState["responseQuery"] = value;
            }
        }

        #region Eventos
        /// <summary>
        /// Evento de Click Voltar do multiview.
        /// </summary>
        protected void btnAnterior_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Estornar Transação - btnAnterior_Click", () =>
                {
                    btnProximo.Visible = true;
                    btnExportarPdf.Visible = false;
                    btnExportarExcel.Visible = false;
                    ucAssistente.Voltar();

                    switch (mvwPreAutorizacao.ActiveViewIndex)
                    {
                        case 0:
                            txtTID.Text = String.Empty;
                            break;
                        case 1:
                            mvwPreAutorizacao.SetActiveView(vwInicial);
                            btnAnterior.Text = "Limpar";
                            btnProximo.Text = "Continuar";
                            pnlCamposObrigatorios.Visible = true;
                            break;
                        case 2:
                            mvwPreAutorizacao.SetActiveView(vwVerificacao);
                            btnAnterior.Text = "Voltar";
                            btnProximo.Text = "Confirmar";
                            break;
                        default:
                            throw new Exception("Passo não implementado.");
                    }
                });
        }

        /// <summary>
        /// Evento de Click Próximo do multiview
        /// </summary>
        protected void btnProximo_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Estornar Transação - btnProximo_Click", () =>
                {
                    btnExportarPdf.Visible = false;
                    btnExportarExcel.Visible = false;
                    btnImprimir.Visible = false;

                    switch (mvwPreAutorizacao.ActiveViewIndex)
                    {
                        case 0:
                            TratarPassoInicialParaConfirmacao();
                            break;
                        case 1:
                            TratarPassoConfirmacaoParaComprovante();
                            break;
                        case 2:
                            TratarPassoComprovante();
                            break;
                        default:
                            throw new Exception("Passo não implementado.");
                    }
                });
        }

        /// <summary>
        /// Evento do botão Exportar PDF.
        /// </summary>
        protected void btnExportarPdf_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Estornar Transação - btnExportarPdf_Click", () =>
            {
                Byte[] pdf = null;
                DadosComprovanteEstornoTransacao dados = RetornarDadosComprovante();

                using (var ctx = new ContextoWCF<ServicoEAdquirenciaClient>())
                {
                    pdf = ctx.Cliente.GerarPDFComprovanteEstornoTransacao(dados);
                }

                String nomeArquivo = String.Format("attachment; filename=ComprovanteEstorno_{0}.pdf", DateTime.Now.ToString("ddMMyyyy_HHmmss"));

                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", nomeArquivo);
                Response.BinaryWrite(pdf);

                Response.End();
            });
        }

        /// <summary>
        /// Evento do Botão Exportar Excel.
        /// </summary>
        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Estornar Transação - btnExportarExcel_Click", () =>
                {
                    Byte[] csv = null;
                    DadosComprovanteEstornoTransacao dados = RetornarDadosComprovante();

                    using (var ctx = new ContextoWCF<ServicoEAdquirenciaClient>())
                    {
                        csv = ctx.Cliente.GerarCSVComprovanteEstornoTransacao(dados);
                    }

                    String nomeArquivo = String.Format("attachment; filename=ComprovanteEstorno_{0}.csv", DateTime.Now.ToString("ddMMyyyy_HHmmss"));

                    Response.ContentType = "text/csv";
                    Response.AddHeader("content-disposition", nomeArquivo);
                    Response.BinaryWrite(csv);

                    Response.End();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                });
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Método de apoio para preencher o objeto DadosComprovanteEstornoTransacao baseado na tela.
        /// </summary>
        /// <returns></returns>
        private DadosComprovanteEstornoTransacao RetornarDadosComprovante()
        {
            DadosComprovanteEstornoTransacao dados = new DadosComprovanteEstornoTransacao 
            {
                NumeroComprovante     = litComprovanteNSU.Text,
                Tid                   = litComprovanteTid.Text,
                NumeroEstabelecimento = litComprovanteNumeroEstab.Text,
                NomeEstabelecimento   = litComprovanteNomeEstab.Text,
                DataAutorizacao       = litComprovanteDataAutorizacao.Text,
                ValidadeAutorizacao   = litComprovanteValidadeAutorizacao.Text,
                DataCaptura           = litComprovanteDataCaptura.Text,
                ValorCaptura          = litComprovanteValorCaptura.Text,
                NumeroCartao          = litComprovanteUltCartao.Text,
                NumeroAutorizacao     = litComprovanteNumeroAutorizacao.Text,
                DataEstorno           = litComprovanteDataEstorno.Text,
                HoraEstorno           = litComprovanteHoraEstorno.Text,
                DataTransacao         = litComprovanteDataTransacao.Text,
                HoraTransacao         = litComprovanteHoraTransacao.Text,
                ValorTransacao        = litComprovanteValorTransacao.Text
            };

            dados.TipoEstorno = CalcularTipoEstorno();

            return dados;
        }

        /// <summary>
        /// Tratativa do primeiro passo
        /// </summary>
        private void TratarPassoInicialParaConfirmacao()
        {
            if (String.IsNullOrWhiteSpace(txtTID.Text))
            {
                base.ExibirPainelMensagem("Campos obrigatórios não preenchidos.");
                return;
            }

            Int64 valorTid;
            if (!Int64.TryParse(txtTID.Text, out valorTid))
            {
                base.ExibirPainelMensagem("TID incorreto.");
                return;
            }


            String numeroPv = String.Empty;
#if DEBUG
            numeroPv = "1250191";
#else
            if (!Sessao.Contem())
                throw new Exception("Falha ao obter sessão.");

             numeroPv = SessaoAtual.CodigoEntidade.ToString();
#endif
            QueryRequest requestQuery = new QueryRequest()
            {
                Filiacao = numeroPv,
                Tid = txtTID.Text
            };

            QueryResponse responseQuery = null;
            using (ContextoWCF<ServicoEAdquirenciaClient> adquirencia = new ContextoWCF<ServicoEAdquirenciaClient>())
            {
                responseQuery = adquirencia.Cliente.Query(requestQuery);
            }


            //Validação do retorno da query
            if (responseQuery == null)
            {
                base.ExibirPainelMensagem("Transação não encontrada.");
                return;
            }
            else if (String.Compare(responseQuery.REGISTRO.COD_RET, "99") == 0)
            {
                base.ExibirPainelMensagem("TID não encontrado. Por favor, verifique o número de transação inserido.");
                return;
            }
            else if (String.IsNullOrEmpty(responseQuery.REGISTRO.TID)
                    || String.IsNullOrWhiteSpace(responseQuery.REGISTRO.STATUS)
                    || String.IsNullOrWhiteSpace(responseQuery.REGISTRO.DATA))
            {
                base.ExibirPainelMensagem("Ocorreu uma falha no processamento. Por favor tente novamente.");
                return;
            }

            DateTime dataTransacao;
            DateTime.TryParseExact(responseQuery.REGISTRO.DATA, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataTransacao);


            //Validações de status
            if (String.Compare(responseQuery.REGISTRO.STATUS, "1") == 0) //status 1 = Aprovada.
            {

                if (DateTime.Today > dataTransacao)
                {
                    base.ExibirPainelHtml("A transação informada não foi capturada hoje. Para realizar o cancelamento desta venda, dirija-se ao menu Gerenciamento de vendas e selecione a opção Cancelamento de vendas.");
                    return;
                }
            }
            else if (String.Compare(responseQuery.REGISTRO.STATUS, "2") == 0) //status 2 = Negada.
            {
                base.ExibirPainelMensagem("A transação informada não foi aprovada no momento da autorização. Por favor, verifique o número de transação inserido.");
                return;
            }
            else if (String.Compare(responseQuery.REGISTRO.STATUS, "3") == 0) //status 3 = Estornada.
            {
                base.ExibirPainelMensagem("A transação informada já foi estornada. Por favor, verifique o número de transação inserido.");
                return;
            }
            else if (String.Compare(responseQuery.REGISTRO.STATUS, "4") == 0) //status 4 = pendente de confirmação
            {
                DateTime dataExpiracao;
                DateTime.TryParseExact(responseQuery.REGISTRO.DATA_EXP_PRE_AUT, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataExpiracao);

                if (DateTime.Today > dataExpiracao)
                {
                    base.ExibirPainelMensagem("A transação informada está fora do prazo de captura.");
                    return;
                }
            }

            ResponseQueryViewState = responseQuery;
            decimal valorTransacao = ConverterValorMonetario(responseQuery.REGISTRO.TOTAL);

            litConfirmaTid.Text = txtTID.Text;
            litConfirmaDataTransacao.Text = dataTransacao.ToString("dd/MM/yyyy");
            litConfirmaValorTransacao.Text = valorTransacao.ToString("C", new CultureInfo("pt-BR"));

            ucAssistente.Avancar();
            mvwPreAutorizacao.SetActiveView(vwVerificacao);
            btnAnterior.Text = "Voltar";
            btnProximo.Text = "Confirmar";

            switch (CalcularTipoEstorno())
            {
                case 1: 
                    ExibirCamposAutorizacaoNaoCaptura(); 
                    break;
                case 2: 
                    ExibirCamposAutorizacaoCaptura(); 
                    break;
                case 3: 
                    ExibirCamposAutorizacaoCapturaAutomatica(); 
                    break;
                default:
                    throw new Exception("Tipo de estorno não definido.");
            }

        }

        /// <summary>
        /// Método TratarPassoConfirmacaoParaComprovante.
        /// </summary>
        private void TratarPassoConfirmacaoParaComprovante()
        {
            String numeroPv = String.Empty;
            String nomeEntidade = String.Empty;

#if DEBUG
            numeroPv = "1250191";
            nomeEntidade = "TAM Linhas Areas";
#else
            if (!Sessao.Contem())
                throw new Exception("Falha ao obter sessão.");

             numeroPv = SessaoAtual.CodigoEntidade.ToString();
             nomeEntidade = SessaoAtual.NomeEntidade;
#endif

            VoidTransactionTID voidRequest = new VoidTransactionTID()
            {
                Tid = txtTID.Text,
                Filiacao = numeroPv,
            };


            Confirmation responseVoid = null;
            using (ContextoWCF<EadquirenciaServico.ServicoEAdquirenciaClient> servico = new ContextoWCF<ServicoEAdquirenciaClient>())
            {
                responseVoid = servico.Cliente.VoidTransactionTID(voidRequest);
            }

            if (String.Compare(responseVoid.CodRet, "00") != 0)
            {
                pnlNaoAprovada.Visible   = true;
                pnlAprovada.Visible      = false;

                btnExportarPdf.Visible   = false;
                btnExportarExcel.Visible = false;
                btnImprimir.Visible      = false;
                btnProximo.Visible       = false;

                litCodRet.Text = responseVoid.CodRet;
                litMsgRet.Text = responseVoid.MsgRet;
            }
            else
            {
                ExibirPainelMensagem("Sua Transação foi estornada com sucesso.");
                Decimal total = ConverterValorMonetario(ResponseQueryViewState.REGISTRO.TOTAL);

                litComprovanteNSU.Text                  = ResponseQueryViewState.REGISTRO.NUMSQN;
                litComprovanteTid.Text                  = responseVoid.Tid;
                litComprovanteNumeroEstab.Text          = numeroPv.ToString();
                litComprovanteNomeEstab.Text            = nomeEntidade;
                litComprovanteDataAutorizacao.Text      = ConverterData(ResponseQueryViewState.REGISTRO.DATA);
                litComprovanteHoraAutorizacao.Text      = ResponseQueryViewState.REGISTRO.HORA;
                litComprovanteValidadeAutorizacao.Text  = ConverterData(ResponseQueryViewState.REGISTRO.DATA_EXP_PRE_AUT);
                litComprovanteDataCaptura.Text          = ConverterData(ResponseQueryViewState.REGISTRO.DATA_CON_PRE_AUT);
                litComprovanteValorCaptura.Text         = total.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));

                litComprovanteDataTransacao.Text        = ConverterData(ResponseQueryViewState.REGISTRO.DATA_CON_PRE_AUT);
                litComprovanteHoraTransacao.Text        = ResponseQueryViewState.REGISTRO.HORA;
                litComprovanteValorTransacao.Text       = total.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));

                litComprovanteUltCartao.Text            = ResponseQueryViewState.REGISTRO.NR_CARTAO;
                litComprovanteNumeroAutorizacao.Text    = ResponseQueryViewState.REGISTRO.NUMAUTOR;
                litComprovanteDataEstorno.Text          = ConverterData(responseVoid.Data);
                litComprovanteHoraEstorno.Text          = responseVoid.Hora;

                btnExportarPdf.Visible        = true;
                btnExportarExcel.Visible      = true;
                btnImprimir.Visible           = true;
                pnlCamposObrigatorios.Visible = false;
            }

            mvwPreAutorizacao.SetActiveView(vwComprovante);
            ucAssistente.Avancar();
            btnProximo.Text       = "Concluir";
            btnAnterior.Visible   = false;
        }

        /// <summary>
        /// Método de apoio para converter data.
        /// </summary>
        /// <param name="data">Data no formato yyyyMMdd.</param>
        /// <returns>Data no formato dd/MM/yyyy</returns>
        private String ConverterData(String data)
        {
            if (String.IsNullOrWhiteSpace(data))
                return "-";

            return DateTime.ParseExact(data, "yyyyMMdd", new CultureInfo("pt-BR")).ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Método do passo 3.
        /// </summary>
        private void TratarPassoComprovante()
        {
            mvwPreAutorizacao.SetActiveView(vwInicial);
            btnAnterior.Text = " Limpar";
            btnProximo.Text = "Continuar";
            txtTID.Text = String.Empty;
            btnAnterior.Visible = true;
            ucAssistente.AtivarPasso(0);
            pnlCamposObrigatorios.Visible = true;
        }

        /// <summary>
        /// Converte uma String no formato "R$ 1200,99".
        /// </summary>
        /// <param name="valor">String que será convertida.</param>
        /// <returns>valor em decimal.</returns>
        private Decimal ConverterValorMonetario(String valor)
        {
            decimal check;
            decimal.TryParse(valor, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.InvariantCulture, out check);

            return check;
        }

        /// <summary>
        /// Exibe os campos da autorização não captura.
        /// </summary>
        private void ExibirCamposAutorizacaoNaoCaptura()
        {
            EsconderCamposDinamicosDoComprovante();
            divComprovanteNSU.Visible = true;
            divValidadeAutorizacao.Visible = true;
        }

        /// <summary>
        /// Exibe os campos da autorização captura.
        /// </summary>
        private void ExibirCamposAutorizacaoCaptura()
        {
            EsconderCamposDinamicosDoComprovante();
            divValidadeAutorizacao.Visible = true;
            divDataCaptura.Visible = true;
            divValorCaptura.Visible = true;
        }

        /// <summary>
        /// Exibe os campos da captura automática.
        /// </summary>
        private void ExibirCamposAutorizacaoCapturaAutomatica()
        {
            EsconderCamposDinamicosDoComprovante();
            divComprovanteNSU.Visible = true;
            divDataTrasancao.Visible = true;
            divHoraTransacao.Visible = true;
            divValorTransacao.Visible = true;
        }

        /// <summary>
        /// Método que esconde todos os campos. Dai só exibe os necessários depois.
        /// </summary>
        private void EsconderCamposDinamicosDoComprovante()
        {
            divComprovanteNSU.Visible = false;
            divValidadeAutorizacao.Visible = false;
            divDataCaptura.Visible = false;
            divValorCaptura.Visible = false;
            divDataTrasancao.Visible = false;
            divHoraTransacao.Visible = false;
            divValorTransacao.Visible = false;
        }

        /// <summary>
        /// Método de apoio que verifica qual é o tipo de Estorno:
        /// 1: Autorização Não Capturada.
        /// 2: Autorização Capturada.
        /// 3: Captura Automatica.
        /// </summary>
        /// <returns></returns>
        private Int32 CalcularTipoEstorno()
        {
            Int32 tipo;

            if (String.Compare(ResponseQueryViewState.REGISTRO.STATUS, "4") == 0)
                tipo = 1;
            else if (String.IsNullOrWhiteSpace(ResponseQueryViewState.REGISTRO.DATA_CON_PRE_AUT))
                tipo = 3;
            else
                tipo = 2;

            return tipo;
        }
        #endregion
    }
}
