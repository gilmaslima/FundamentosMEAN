using Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico;
using Rede.PN.Eadquirencia.Sharepoint.Helper;
using Redecard.PN.Comum;
using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Rede.PN.Eadquirencia.Sharepoint.Webparts.CapturarTransacao
{
    /// <summary>
    /// Classe de Capturar Transação.
    /// </summary>
    public partial class CapturarTransacaoUserControl : WebpartBase
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
        /// Evento de Page_Load.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Evento de Click Voltar do multiview.
        /// </summary>
        protected void btnAnterior_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Capturar Transação - btnAnterior_Click", () =>
            {
                btnProximo.Visible = true;
                btnExportarPdf.Visible = false;
                btnExportarExcel.Visible = false;
                ucAssistente.Voltar();

                switch (mvwPreAutorizacao.ActiveViewIndex)
                {
                    case 0:
                        txtTID.Text = String.Empty;
                        txtValorTransacao.Text = String.Empty;
                        break;
                    case 1:
                        mvwPreAutorizacao.SetActiveView(vwInicial);
                        btnAnterior.Text = " Limpar";
                        btnProximo.Text = "Continuar";
                        pnlCamposObrigatorios.Visible = true;
                        break;
                    case 2:
                        mvwPreAutorizacao.SetActiveView(vwVerificacao);
                        btnAnterior.Text = " Voltar";
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
            ExecucaoTratada("Capturar Transação - btnProximo_Click", () =>
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
            ExecucaoTratada("Capturar Transação - btnExportarPdf_Click", () =>
            {
                Byte[] pdf = null;
                DadosComprovanteCapturarTransacao dados = RetornarDadosComprovante();

                using (var ctx = new ContextoWCF<ServicoEAdquirenciaClient>())
                {
                    pdf = ctx.Cliente.GerarPDFComprovanteCapturaTransacao(dados);
                }

                String nomeArquivo = String.Format("attachment; filename=ComprovanteCaptura_{0}.pdf", DateTime.Now.ToString("ddMMyyyy_HHmmss"));

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
            ExecucaoTratada("Capturar Transação - btnExportarExcel_Click", () =>
            {
                Byte[] csv = null;
                DadosComprovanteCapturarTransacao dados = RetornarDadosComprovante();

                using (var ctx = new ContextoWCF<ServicoEAdquirenciaClient>())
                {
                    csv = ctx.Cliente.GerarCSVComprovanteCapturaTransacao(dados);
                }

                String nomeArquivo = String.Format("attachment; filename=ComprovanteCaptura_{0}.csv", DateTime.Now.ToString("ddMMyyyy_HHmmss"));

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
        /// Método de apoio para preencher o objeto DadosComprovanteCapturarTransacao baseado na tela.
        /// </summary>
        /// <returns></returns>
        private DadosComprovanteCapturarTransacao RetornarDadosComprovante()
        { 
            DadosComprovanteCapturarTransacao dados = new DadosComprovanteCapturarTransacao 
            { 
                DataConfirmacao         = litComprovanteDataConfirmacao.Text,
                DataPreAutorizacao      = litComprovanteDataAutor.Text,
                HoraConfirmacao         = litComprovanteHoraConfirmacao.Text,
                HoraPreAutorizacao      = litComprovanteHoraAutor.Text,
                NomeEstabelecimento     = litComprovanteNomeEstab.Text,
                NumeroAutorizacao       = litComprovanteNumeroAutorizacao.Text,
                NumeroCartao            = litComprovanteUltCartao.Text,
                NumeroComprovante       = litComprovanteNSU.Text,
                NumeroEstabelecimento   = litComprovanteNumeroEstab.Text,
                Tid                     = litComprovanteTid.Text,
                ValorConfirmacao        = litComprovanteValorConfirmacao.Text,
                ValorPreAutorizacao     = litComprovanteValorAutor.Text
            };
            return dados;
        }

        /// <summary>
        /// Método do Primeiro passo.
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

            Decimal valorConfirmacao = ConverterValorMonetario(txtValorTransacao.Text);
            String numeroPv = String.Empty;
            if (!Sessao.Contem())
                throw new Exception("Falha ao obter sessão.");

             numeroPv = SessaoAtual.CodigoEntidade.ToString();
            QueryRequest requestQuery = new QueryRequest()
            {
                Filiacao = numeroPv,
                Tid      = txtTID.Text
            };

            QueryResponse responseQuery = null;
            using (ContextoWCF<ServicoEAdquirenciaClient> adquirencia = new ContextoWCF<ServicoEAdquirenciaClient>())
            {
                responseQuery = adquirencia.Cliente.Query(requestQuery);
            }

            Decimal valorAutorizadoQuery;
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
            else if(  String.IsNullOrEmpty(responseQuery.REGISTRO.TID)
                   || String.IsNullOrWhiteSpace(responseQuery.REGISTRO.STATUS)
                   || !Decimal.TryParse(responseQuery.REGISTRO.TOTAL, NumberStyles.Number, CultureInfo.InvariantCulture, out valorAutorizadoQuery))
            {
                base.ExibirPainelMensagem("Ocorreu uma falha no processamento. Por favor tente novamente.");
                return;
            }

            if (String.Compare(responseQuery.REGISTRO.STATUS, "1") == 0) //status 1 = Aprovada.
            {
                base.ExibirPainelMensagem("A transação informada já foi capturada. Por favor, verifique o número de transação inserido");
                return;
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
                    base.ExibirPainelMensagem("Não foi possível realizar a confirmação. A autorização fora do prazo de captura.");
                    return;
                }
            }

            //validar o valor da transação
            if (valorConfirmacao > valorAutorizadoQuery)
            {
                base.ExibirPainelMensagem("Valor da transação maior que o permitido. Por favor, verifique o valor da transação inserido.");
                return;
            }

            // se não passar um valor na captura, pega o valor total.
            if (valorConfirmacao == 0)
                valorConfirmacao = valorAutorizadoQuery;

            DateTime data;
            DateTime.TryParseExact(responseQuery.REGISTRO.DATA, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out data);

            ResponseQueryViewState = responseQuery;

            litConfirmaTid.Text = responseQuery.REGISTRO.TID;
            litConfirmaValorTransacao.Text = String.Concat("R$ ", valorConfirmacao.ToString(new CultureInfo("pt-BR")));
            
            litConfirmaDataTransacao.Text = data.ToString("dd/MM/yyyy");
            mvwPreAutorizacao.SetActiveView(vwVerificacao);
            ucAssistente.Avancar();
            btnAnterior.Text = " Voltar";
            btnProximo.Text = "Confirmar";
            pnlCamposObrigatorios.Visible = false;
        }

        /// <summary>
        /// Método do passo 2.
        /// </summary>
        private void TratarPassoConfirmacaoParaComprovante()
        {
            Int32  numeroPv = 0;
            String nomeEntidade = String.Empty;
            Decimal valorTotal = String.IsNullOrWhiteSpace(txtValorTransacao.Text) ? ConverterValorMonetario(ResponseQueryViewState.REGISTRO.TOTAL) : ConverterValorMonetario(txtValorTransacao.Text);
            numeroPv = SessaoAtual.CodigoEntidade;
            nomeEntidade = SessaoAtual.NomeEntidade;

            ConfirmTxnTID requestConfirm = new ConfirmTxnTID()
            {
                Filiacao = numeroPv.ToString(),
                Tid = txtTID.Text,
                Total = valorTotal.ToString(CultureInfo.InvariantCulture),
            };

            Authorization responseConfirm;
            using (ContextoWCF<ServicoEAdquirenciaClient> adquirencia = new ContextoWCF<ServicoEAdquirenciaClient>())
            {
                responseConfirm = adquirencia.Cliente.ConfirmTxnTID(requestConfirm);
            }

            if (String.Compare(responseConfirm.CodRet, "00") != 0)
            {
                pnlNaoAprovada.Visible   = true;
                pnlAprovada.Visible      = false;

                btnExportarPdf.Visible   = false;
                btnExportarExcel.Visible = false;
                btnImprimir.Visible      = false;
                btnProximo.Visible       = false;

                litCodRet.Text = responseConfirm.CodRet;
                litMsgRet.Text = responseConfirm.Msgret;
            }
            else
            {
                ExibirPainelMensagem("Sua Transação foi capturada com sucesso.");
                Decimal total;
                Decimal.TryParse(ResponseQueryViewState.REGISTRO.TOTAL, NumberStyles.None, CultureInfo.InvariantCulture, out total);

                litComprovanteNSU.Text               = ResponseQueryViewState.REGISTRO.NUMSQN;
                litComprovanteTid.Text               = responseConfirm.Tid;
                litComprovanteNumeroEstab.Text       = numeroPv.ToString();
                litComprovanteNomeEstab.Text         = nomeEntidade;
                litComprovanteDataAutor.Text         = litConfirmaDataTransacao.Text; // data da transação já está no formato certo do passo 2
                litComprovanteHoraAutor.Text         = ResponseQueryViewState.REGISTRO.HORA;
                litComprovanteValorAutor.Text        = total.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));
                litComprovanteUltCartao.Text         = ResponseQueryViewState.REGISTRO.NR_CARTAO;
                litComprovanteNumeroAutorizacao.Text = responseConfirm.NumAutor;
                litComprovanteDataConfirmacao.Text   = responseConfirm.Data;
                litComprovanteHoraConfirmacao.Text   = responseConfirm.Hora;
                litComprovanteValorConfirmacao.Text  = txtValorTransacao.Text; //valor que o usuário digitou.
                
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
        /// Método do passo 3.
        /// </summary>
        private void TratarPassoComprovante()
        {
            mvwPreAutorizacao.SetActiveView(vwInicial);
            btnAnterior.Text       = " Limpar";
            btnProximo.Text        = "Continuar";
            txtTID.Text            = String.Empty;
            txtValorTransacao.Text = String.Empty;
            btnAnterior.Visible    = true;
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
            decimal.TryParse(valor, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, new CultureInfo("pt-BR"), out check);

            return check;
        }
        #endregion
    }
}