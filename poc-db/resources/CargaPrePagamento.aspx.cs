using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Comum;

using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Data;
using System.ServiceModel;
using Redecard.PN.Emissores.Sharepoint.ServicoEmissores;
using System.Collections.Generic;
using System.Linq;

namespace Redecard.PN.Emissores.Sharepoint.Layouts.Emissores
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CargaPrePagamento : ApplicationPageBaseAutenticadaWindows
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly String[] nomesColunas = new String[11] {
            "DAT_PCM", 
            "DAT_VCTO", 
            "COD_BAC", 
            "COD_BNDR", 
            "COD_BAC_BNDR", 
            "VAL_SLDO_RCBR", 
            "VAL_SLDO_FEE", 
            "VAL_SLDO_LQDO", 
            "VAL_SLDO_ANTC",
            "MSG_ERRO",
            "NUM_LINHA"
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                _ifsResultado.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DataTable CriarColunas()
        {
            DataTable tabela = new DataTable("TabelaPrePagamentos");
            DataColumn coluna = new DataColumn();

            foreach (String nomeColuna in nomesColunas)
            {
                coluna = new DataColumn();
                coluna.DataType = Type.GetType("System.String");
                coluna.ColumnName = nomeColuna;

                tabela.Columns.Add(coluna);
            }

            return tabela;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dtGrid_OnPageIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {
            try
            {
                (sender as DataGrid).CurrentPageIndex = e.NewPageIndex;
                (sender as DataGrid).DataBind();
            }
            catch (Exception ex)
            {
                pnlErro.Visible = true;
                ltErro.Text = ex.Message;
            }
        }

        /// <summary>
        /// Limpeza das tabelas com dados de Pré-Pagamentos conforme selecionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDadosExcluir_OnClick(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Início Limpeza das tabelas com dados de Pré-Pagamentos conforme selecionado"))
            {
                try
                {
                    Boolean retorno = true;
                    
                    using (var context = new ContextoWCF<ServicoPortalEmissoresClient>())
                    {
                        if (ddlDadosExcluir.SelectedValue.Equals("C"))
                            retorno = context.Cliente.ExcluirPrePagamentosConfirmados();
                        else if (ddlDadosExcluir.SelectedValue.Equals("P"))
                            retorno = context.Cliente.ExcluirPrePagamentosParcelados();
                        else
                            retorno = context.Cliente.ExcluirPrePagamentosTemporarios();
                    }

                    if (retorno)
                    {
                        pnlAcoes.Visible = true;
                        ltResultadoAcoes.Text = String.Format("Exclusão dos dados {0} efetuada com sucesso!", ddlDadosExcluir.SelectedItem.Text);
                    }
                    else
                    {
                        pnlAcoes.Visible = true;
                        ltResultadoAcoes.Text = String.Format("Houver um erro na exclusão dos dados {0}!", ddlDadosExcluir.SelectedItem.Text);
                    }
                }
                catch (FaultException<ServicoEmissores.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    pnlAcoes.Visible = true;
                    ltResultadoAcoes.Text = String.Format("({0}) {1}", ex.Code, ex.Message);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    pnlAcoes.Visible = true;
                    ltResultadoAcoes.Text = String.Format("{0}", ex.Message);
                }
            }
        }

        /// <summary>
        /// Executa a procedure de ajuste de Pré-Pagamentos confirmados manualmente
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAjustarConfirmados_OnClick(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Início execução a procedure de ajuste de Pré-Pagamentos confirmados manualmente"))
            {
                try
                {
                    Boolean retorno = true;

                    using (var context = new ContextoWCF<ServicoPortalEmissoresClient>())
                    {
                        retorno = context.Cliente.AjustarCargaConfirmados();
                    }

                    if (retorno)
                    {
                        pnlAcoes.Visible = true;
                        ltResultadoAcoes.Text = "Execução do ajustes efetuado com sucesso!";
                    }
                    else
                    {
                        pnlAcoes.Visible = true;
                        ltResultadoAcoes.Text = "Houve um erro na execução do ajustes!";
                    }
                }
                catch (FaultException<ServicoEmissores.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    pnlAcoes.Visible = true;
                    ltResultadoAcoes.Text = String.Format("({0}) {1}", ex.Code, ex.Message);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    pnlAcoes.Visible = true;
                    ltResultadoAcoes.Text = String.Format("{0}", ex.Message);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ConfirmarCargar(object sender, EventArgs e)
        {
            try
            {
                _ifsResultado.Visible = true;
                pnlErro.Visible = false;
                pnlDadosErro.Visible = false;
                pnlAcoes.Visible = false;

                ServicoEmissores.PrePagamento prePagamentoCarregar;
                List<ServicoEmissores.PrePagamento> prePagamentosCarregar = new List<ServicoEmissores.PrePagamento>();
                List<ServicoEmissores.PrePagamento> prePagamentosErro = new List<ServicoEmissores.PrePagamento>();

                if (!fluCarga.HasFile)
                {
                    pnlErro.Visible = true;
                    ltErro.Text = "Nenhum arquivo selecionado";
                }
                else
                {
                    using (Stream arquivoCarga = fluCarga.PostedFile.InputStream)
                    {
                        if (arquivoCarga == null)
                        {
                            pnlErro.Visible = true;
                            ltErro.Text = "Nenhum arquivo selecionado";
                        }
                        else
                        {
                            using (StreamReader leitorArquivo = new StreamReader(arquivoCarga))
                            {
                                String prePagamento;
                                Int32 indexItem = 1;
                                while ((prePagamento = leitorArquivo.ReadLine()) != null)
                                {
                                    if (!String.IsNullOrEmpty(prePagamento.Trim()))
                                    {
                                        String[] infoPrepagamento = prePagamento.Split(Convert.ToChar(";"));
                                        prePagamentoCarregar = new ServicoEmissores.PrePagamento();
                                        prePagamentoCarregar.Banco = new ServicoEmissores.Emissor();
                                        prePagamentoCarregar.Bandeira = new ServicoEmissores.Bandeira();
                                        prePagamentoCarregar.Bandeira.EmissoresBandeiras = new List<ServicoEmissores.EmissorBandeira>();

                                        try
                                        {
                                            prePagamentoCarregar.NumeroLinha = indexItem;

                                            for (Int16 i = 0; i < infoPrepagamento.Length; i++)
                                            {
                                                //Removendo os caracteres \0 e SOH(\u0001)
                                                String valorLimpo = infoPrepagamento[i].Trim().Replace("\0", String.Empty);
                                                valorLimpo = valorLimpo.Replace(Char.ConvertFromUtf32(1).ToString(), String.Empty);
                                                valorLimpo = valorLimpo.Trim();

                                                if (nomesColunas[i].Contains("DAT"))
                                                {
                                                    DateTime data;
                                                    if (!DateTime.TryParse(valorLimpo, out data))
                                                    {
                                                        data = new DateTime();
                                                    }

                                                    if (nomesColunas[i].Equals("DAT_VCTO"))
                                                        prePagamentoCarregar.DataVencimento = data;
                                                    else
                                                        prePagamentoCarregar.DataProcessamento = data;
                                                }
                                                else if (nomesColunas[i].Contains("COD"))
                                                {
                                                    Int32 codigo = 0;

                                                    if (!Int32.TryParse(valorLimpo.Replace(".", String.Empty), out codigo))
                                                    {
                                                        codigo = 0;
                                                    }

                                                    if (nomesColunas[i].Equals("COD_BAC"))
                                                    {
                                                        prePagamentoCarregar.Banco.Codigo = codigo;
                                                    }
                                                    else if (nomesColunas[i].Equals("COD_BNDR"))
                                                    {
                                                        prePagamentoCarregar.Bandeira.Codigo = codigo;
                                                    }
                                                    else //"COD_BAC_BNDR", 
                                                    {
                                                        prePagamentoCarregar.Bandeira.EmissoresBandeiras.Add(
                                                                new ServicoEmissores.EmissorBandeira() { Codigo = codigo });
                                                    }
                                                }
                                                else if (nomesColunas[i].Contains("VAL"))
                                                {
                                                    Double valor = 0;

                                                    if (!Double.TryParse(valorLimpo.Replace('.', ','), out valor))
                                                    {
                                                        valor = 0;
                                                    }

                                                    if (nomesColunas[i].Equals("VAL_SLDO_RCBR"))
                                                    {
                                                        prePagamentoCarregar.ValorPagarReceber = valor;
                                                    }
                                                    else if (nomesColunas[i].Equals("VAL_SLDO_FEE"))
                                                    {
                                                        prePagamentoCarregar.ValorFEE = valor;
                                                    }
                                                    else if (nomesColunas[i].Equals("VAL_SLDO_LQDO"))
                                                    {
                                                        prePagamentoCarregar.SaldoEstoque = valor;
                                                    }
                                                    else //VAL_SLDO_ANTC
                                                    {
                                                        prePagamentoCarregar.SaldoAntecipado = valor;
                                                    }
                                                }
                                            }
                                            prePagamentosCarregar.Add(prePagamentoCarregar);
                                        }
                                        catch (ArgumentException ex)
                                        {
                                            prePagamentoCarregar.MensagemErroCarga = ex.Message;
                                            prePagamentosErro.Add(prePagamentoCarregar);
                                        }
                                        catch (Exception ex)
                                        {
                                            prePagamentoCarregar.MensagemErroCarga = ex.Message;
                                            prePagamentosErro.Add(prePagamentoCarregar);
                                        }

                                        //tabelaPrePagamentos.Rows.Add(linhaTabela);
                                        indexItem++;
                                    }
                                }
                            }
                        }
                    }

                    //Enviando para o Serviços os N registros a processar por commit
                    Int32 quantidadeProcessar = ddlQuantidadeProcessar.SelectedValue.ToInt32();
                    Int32 totalCarregados = prePagamentosCarregar.Count;
                    prePagamentosErro.AddRange(this.CarregarDados(quantidadeProcessar, prePagamentosCarregar, ddlCarga.SelectedValue.Equals("C") ? true : false));

                    pnlErro.Visible = true;
                    totalCarregados = (totalCarregados - prePagamentosErro.Count);
                    ltErro.Text = String.Format("{0} Pré-Pagamento{1} {2} carregados",
                                                totalCarregados,
                                                totalCarregados > 1 ? "s" : "",
                                                ddlCarga.SelectedItem.Text);

                    if (prePagamentosErro.Count > 0)
                    {
                        pnlDadosErro.Visible = true;
                        dtgErro.DataSource = this.MontarTabelaCarga(prePagamentosErro);
                        dtgErro.DataBind();
                    }
                }
            }
            catch (FaultException<ServicoEmissores.GeneralFault> ex)
            {
                pnlErro.Visible = true;
                ltErro.Text = ex.Message;
            }
            catch (IOException ex)
            {
                pnlErro.Visible = true;
                ltErro.Text = ex.Message;
            }
            catch (ArgumentException ex)
            {
                pnlErro.Visible = true;
                ltErro.Text = ex.Message;
            }
            catch (ObjectDisposedException ex)
            {
                pnlErro.Visible = true;
                ltErro.Text = ex.Message;
            }
            catch (Exception ex)
            {
                pnlErro.Visible = true;
                ltErro.Text = ex.Message;
            }
        }

        /// <summary>
        /// Carrega os dados na base de dados
        /// </summary>
        /// <param name="quantidadeProcessar"></param>
        /// <param name="prePagamentosCarregar"></param>
        /// <param name="confirmados"></param>
        /// <returns></returns>
        private List<ServicoEmissores.PrePagamento> CarregarDados(Int32 quantidadeProcessar, List<ServicoEmissores.PrePagamento> prePagamentosCarregar, Boolean confirmados)
        {
            List<ServicoEmissores.PrePagamento> prePagamentosErro = new List<ServicoEmissores.PrePagamento>();

            try
            {
                List<PrePagamento> carregar;
                if (quantidadeProcessar == 0)
                {
                    quantidadeProcessar = prePagamentosCarregar.Count;
                }

                carregar = prePagamentosCarregar.Take(quantidadeProcessar).ToList();

                using (var context = new ContextoWCF<ServicoPortalEmissoresClient>())
                {
                    if (confirmados)
                        context.Cliente.ExcluirPrePagamentosTemporarios();
                    else
                        context.Cliente.ExcluirPrePagamentosParcelados();

                    while (carregar.Count > 0)
                    {
                        prePagamentosErro.AddRange(context.Cliente.CarregarPrePagamentos(carregar, confirmados));

                        prePagamentosCarregar.RemoveRange(0, carregar.Count);
                        carregar = prePagamentosCarregar.Take(quantidadeProcessar).ToList();
                    }
                }

            }
            catch (FaultException<ServicoEmissores.GeneralFault> ex)
            {
                pnlErro.Visible = true;
                ltErro.Text = ex.Message;
            }
            catch (Exception ex)
            {
                pnlErro.Visible = true;
                ltErro.Text = ex.Message;
            }

            return prePagamentosErro;
        }

        private DataTable MontarTabelaCarga(List<PrePagamento> prePagamentos)
        {
            DataRow linhaTabela;
            DataTable tabelaPrePagamentos = CriarColunas();
            try
            {
                foreach (PrePagamento prePag in prePagamentos)
                {
                    linhaTabela = tabelaPrePagamentos.NewRow();

                    linhaTabela["DAT_PCM"] = prePag.DataProcessamento;
                    linhaTabela["DAT_VCTO"] = prePag.DataVencimento;
                    linhaTabela["COD_BAC"] = prePag.Banco.Codigo;
                    linhaTabela["COD_BNDR"] = prePag.Bandeira.Codigo;
                    linhaTabela["COD_BAC_BNDR"] = prePag.Bandeira.EmissoresBandeiras[0].Codigo;
                    linhaTabela["VAL_SLDO_RCBR"] = prePag.ValorPagarReceber;
                    linhaTabela["VAL_SLDO_FEE"] = prePag.ValorFEE;
                    linhaTabela["VAL_SLDO_LQDO"] = prePag.SaldoEstoque;
                    linhaTabela["VAL_SLDO_ANTC"] = prePag.SaldoAntecipado;
                    linhaTabela["MSG_ERRO"] = prePag.MensagemErroCarga;
                    linhaTabela["NUM_LINHA"] = prePag.NumeroLinha;

                    tabelaPrePagamentos.Rows.Add(linhaTabela);
                }
            }
            catch (ArgumentException ex)
            {
                pnlErro.Visible = true;
                ltErro.Text = ex.Message;
            }
            catch (InvalidCastException ex)
            {
                pnlErro.Visible = true;
                ltErro.Text = ex.Message;
            }
            catch (IndexOutOfRangeException ex)
            {
                pnlErro.Visible = true;
                ltErro.Text = ex.Message;
            }
            catch (Exception ex)
            {
                pnlErro.Visible = true;
                ltErro.Text = ex.Message;
            }

            return tabelaPrePagamentos;
        }

    }
}
