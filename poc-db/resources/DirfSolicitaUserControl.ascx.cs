using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using System.ServiceModel;
using Redecard.PN.Extrato.SharePoint.Helper;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Redecard.PN.Extrato.SharePoint.Servico.Dirf;
using Microsoft.SharePoint;

namespace Redecard.PN.Extrato.SharePoint.DirfSolicita
{
    public partial class DirfSolicitaUserControl : BaseUserControl
    {
        public Boolean ModoImpressao { get; set; }
        private Decimal valorTotalEstabelecimentoRecebido;
        private Decimal valorTotalEstabelecimentoRecebidoIR;
        private Decimal valorTotalEmissores;
        private Decimal ValorTotalEstabelecimento;

        // para cada emissor
        private Decimal rendimentoTotalEmissor;
        private Decimal irRetidoTotalEmissor;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Int16[] anosBase = CarregarAnoBase();
                
                //se existe QueryStringSegura em "dados"
                if (QS != null)
                {
                    //procura parâmetro "anoBase" e traz DIRF deste ano já carregada, caso ano exista
                    Int16? anoBase = QS["anoBase"].ToInt16Null();
                    if (anoBase.HasValue && anosBase != null && anosBase.Contains(anoBase.Value))
                    {
                        ddlAnoDirf.SelectedValue = anoBase.Value.ToString();
                        Visualizar(anoBase.Value);
                    }
                }

                if (this.ModoImpressao)
                {
                    artConhecaDirf.Visible = false;
                    divFiltros.Visible = false;
                    mnuAcoes.Visible = false;
                    lbxModalAceite.Visible = false;
                    lbxConhecerDirf.Visible = false;
                    btnVoltarTopo.Visible = false;
                }
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Dirf Solicita - Carregar Ano Base"))
            {
                try
                {
                    string sRetorno = ValidarEscolha();
                    lblValidacoes.Text = String.Empty;
                    if (sRetorno != string.Empty)
                    {
                        lblValidacoes.Text = sRetorno;
                        return;
                    }

                    Visualizar(Convert.ToInt16(ddlAnoDirf.SelectedValue));
                }
                catch (FaultException<Servico.Dirf.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    if (Request.QueryString["mostrarErro"] != null)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                    Redecard.PN.Comum.SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    if (Request["mostrarErro"] != null)
                    {
                        throw ex;
                    }
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

        }
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            // registra o 'aceite' do usuario na lista SP e dispara o evento do buscar
            String nomeLista = "Dirf Usuario Aceite";

            Int32 pv = this.SessaoAtual.CodigoEntidade;
            Int32 codigoUsuario = this.SessaoAtual.CodigoIdUsuario;
            Int32 anoCorrente = ddlAnoDirf.SelectedValue.ToInt32();
            String colunaAnoCorrente = "AnoCalendario" + anoCorrente;

            // Rodando com privilegio elevado
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite site = new SPSite(SPContext.Current.Web.Url))
                using (SPWeb web = site.OpenWeb())
                {
                    SPList listaDirf = web.Lists[nomeLista];
                    
                    // Ativando a flag AllowUnsafeUpdates
                    bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
                    web.AllowUnsafeUpdates = true;

                    // verifica se existe a coluna do ano corrente
                    if (!listaDirf.Fields.ContainsField(colunaAnoCorrente))
                    {
                        //cria a coluna
                        listaDirf.Fields.Add(colunaAnoCorrente, SPFieldType.Boolean, true);
                        listaDirf.Update();
                        var viewDefault = listaDirf.DefaultView;
                        viewDefault.ViewFields.Add(colunaAnoCorrente);
                        viewDefault.Update();
                    }

                    //verifica se o usuario ja tem um registro
                    SPQuery query = new SPQuery();
                    query.Query = @"
                        <Where>
                              <And>
                                 <Eq>
                                    <FieldRef Name='Title' />
                                    <Value Type='Text'>" + codigoUsuario + @"</Value>
                                 </Eq>
                                 <Eq>
                                    <FieldRef Name='NumeroDoPV' />
                                    <Value Type='Text'>" + pv + @"</Value>
                                 </Eq>
                              </And>
                         </Where>";
                    
                    SPListItem itemUsuario = listaDirf.GetItems(query).Cast<SPListItem>().FirstOrDefault();
                    if (itemUsuario != null)
                    {
                        itemUsuario[colunaAnoCorrente] = true;
                        itemUsuario.Update();
                    }
                    else
                    {
                        //cria um novo registro
                        SPListItem item = listaDirf.Items.Add();
                        item["Title"] = codigoUsuario;
                        item["NumeroDoPV"] = pv;
                        // Retornando a flag AllowUnsafeUpdates
                        item[colunaAnoCorrente] = true;
                        item.Update();
                    }

                    web.AllowUnsafeUpdates = allowUnsafeUpdates;

                }
            });

            btnBuscar_Click(sender, e);
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Download Dirf Solicita"))
            {
                String idSender = ((Control)sender).ID;
                String extensao = String.Empty;
                if (String.Compare(idSender, "btnDownloadHtml", true) == 0)
                    extensao = "html";
                else if (String.Compare(idSender, "btnDownloadDoc", true) == 0)
                    extensao = "doc";
                else if (String.Compare(idSender, "btnDownloadTxt", true) == 0)
                    extensao = "txt";

                DownloadDirf(extensao);
            }
        }

        protected void grvDirfEstabelecimento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[1].Text = this.valorTotalEstabelecimentoRecebido.ToString("N2");
                e.Row.Cells[2].Text = this.valorTotalEstabelecimentoRecebidoIR.ToString("N2");
                e.Row.Cells[3].Text = this.valorTotalEmissores.ToString("N2");
                e.Row.Cells[4].Text = this.ValorTotalEstabelecimento.ToString("N2");
            }
        }

        protected void rptEmissoresNovo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Servico.Dirf.ConsultarExtratoDirfEmissorRetorno emissor = (Servico.Dirf.ConsultarExtratoDirfEmissorRetorno)e.Item.DataItem;
                String cnpj = emissor.Cnpj.ToString().PadLeft(14, '9');

                if (emissor.Nome.Equals("000000000000000000000000000000") && cnpj.Equals("99999999999999"))
                    e.Item.Visible = false;
                else
                {
                    Label lblNomeEstabelecimento = (Label)e.Item.FindControl("lblNomeEstabelecimento");
                    Label lblCnpjEstabelecimento = (Label)e.Item.FindControl("lblCnpjEstabelecimento");
                    Literal ltlDivisor = (Literal)e.Item.FindControl("ltlDivisor");
                    
                    Repeater rptEmissorRendimentos = (Repeater)e.Item.FindControl("rptEmissorRendimentos");
                    Repeater rptEmissorRetido = (Repeater)e.Item.FindControl("rptEmissorRetido");

                    lblNomeEstabelecimento.Text = emissor.Nome;
                    lblCnpjEstabelecimento.Text = Convert.ToUInt64(emissor.Cnpj.ToString().PadLeft(14, '0')).ToString(@"00\.000\.000\/0000\-00");

                    // AAR - Alteração solicita pelo Sérgio Andrade e Eneas conforme e-mail enviado
                    if (cnpj.Equals("99999999999999"))
                    {
                        lblNomeEstabelecimento.Text = "outros emissores sem retenção";
                        lblCnpjEstabelecimento.Visible = false;
                        ltlDivisor.Visible = false;
                        rptEmissorRetido.Visible = false;
                    }

                    this.rendimentoTotalEmissor = 0;
                    this.irRetidoTotalEmissor = 0;

                    List<dynamic> rendimentosEmissor = new List<dynamic>();
                    List<dynamic> irRetidoEmissor = new List<dynamic>();

                    for (int i = 1; i < 13; i++)
                    {
                        String mes = new CultureInfo("pt-BR").DateTimeFormat.GetMonthName(i);

                        Decimal valorRepassado = (Decimal)GetPropertyValue(emissor, String.Format("ValorRepassadoEmissor{0}", i));
                        Decimal valorIrRetido = (Decimal)GetPropertyValue(emissor, String.Format("ValorIrEmissor{0}", i));

                        this.rendimentoTotalEmissor += valorRepassado;
                        this.irRetidoTotalEmissor += valorIrRetido;

                        rendimentosEmissor.Add(new
                        {
                            Mes = mes,
                            Valor = valorRepassado.ToString("N2")
                        });

                        irRetidoEmissor.Add(new
                        {
                            Mes = mes,
                            Valor = valorIrRetido.ToString("N2")
                        });
                    }

                    rptEmissorRendimentos.DataSource = rendimentosEmissor;
                    rptEmissorRendimentos.DataBind();

                    rptEmissorRetido.DataSource = irRetidoEmissor;
                    rptEmissorRetido.DataBind();
                }
            }
        }

        protected void rptEmissorRendimentos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                Literal ltlRendimentoTotal = (Literal)e.Item.FindControl("ltlRendimentoTotal");
                ltlRendimentoTotal.Text = this.rendimentoTotalEmissor.ToString("C");
            }
        }

        protected void rptEmissorRetido_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                Literal ltlRendimentoTotal = (Literal)e.Item.FindControl("ltlRendimentoTotal");
                ltlRendimentoTotal.Text = this.irRetidoTotalEmissor.ToString("C");
            }
        }

        /// <summary>
        /// Carrega os anos base disponíveis para consulta de DIRF.
        /// </summary>
        /// <returns>Anos base disponíveis</returns>
        private Int16[] CarregarAnoBase()
        {
            Int16[] anosBase = default(Int16[]);

            using (Logger Log = Logger.IniciarLog("Dirf Solicita - Carregar Ano Base"))
            {
                try
                {
                    Servico.Dirf.StatusRetorno objStatusRetorno;
                    Servico.Dirf.ConsultarAnosBaseDirfRetorno objRetorno;

                    using (var contexto = new ContextoWCF<Servico.Dirf.DirfClient>())
                    {
                        objRetorno = contexto.Cliente.ConsultarAnosBaseDirf(out objStatusRetorno);
                    }

                    if (objStatusRetorno.CodigoRetorno != 0)
                    {
                        base.ExibirPainelExcecao(objStatusRetorno.Fonte, objStatusRetorno.CodigoRetorno);
                        return anosBase;
                    }

                    anosBase = objRetorno.AnosBase.Where(val => val != DateTime.Now.Year).ToArray();
                    ddlAnoDirf.DataSource = anosBase;
                    ddlAnoDirf.DataBind();
                    ddlAnoDirf.Items.Insert(0, new ListItem("selecione o ano", "selecione"));
                }
                catch (FaultException<Servico.Dirf.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    if (Request.QueryString["mostrarErro"] != null)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                    Redecard.PN.Comum.SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    if (Request["mostrarErro"] != null)
                    {
                        throw ex;
                    }
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return anosBase;
        }

        private string ValidarEscolha()
        {
            string sRetorno = string.Empty;
            //if (hdnChecked.Value == string.Empty)
            if (String.Compare(ddlAnoDirf.SelectedValue, "selecione", true) == 0)
            {
                sRetorno = "É obrigatório selecionar o ano";
            }
            return sRetorno;
        }

        /// <summary>Rotina principal para download da DIRF</summary>
        private void DownloadDirf(String extensao)
        {
            //String extensao = ObterOpcaoDownloadSelecionada();
            String nomeArquivo = "DIRFDownload_" + DateTime.Now.ToString("dd-MM-yyyy_HHmmssfff");

            String content = hdnDownload.Value;
            hdnDownload.Value = String.Empty;

            if (extensao == "doc")
                Utils.DownloadDOC(content, nomeArquivo);
            else if (extensao == "html")
                Utils.DownloadHTML(content, nomeArquivo);
            else if (extensao == "txt")
                Utils.DownloadTXT(content, nomeArquivo);
        }

        public void Visualizar(Int16 anoBase)
        {
            using (Logger Log = Logger.IniciarLog("Visualização de Dirf"))
            {
                try
                {
                    Servico.Dirf.StatusRetorno objStatusRetorno;
                    Servico.Dirf.ConsultarExtratoDirfRetorno objConsultarExtratoDirfRetorno;

                    using (var contexto = new ContextoWCF<Servico.Dirf.DirfClient>())
                    {
                        Servico.Dirf.ConsultarExtratoDirfEnvio objEnvio = new Servico.Dirf.ConsultarExtratoDirfEnvio
                        {
                            NumeroEstabelecimento = base.SessaoAtual.CodigoEntidade,
                            AnoBaseDirf = anoBase,
                            CnpjEstabelecimento = this.SessaoAtual.CNPJEntidade
                        };

                        objConsultarExtratoDirfRetorno = contexto.Cliente.ConsultarExtratoDirf(out objStatusRetorno, objEnvio);
                    }

                    if (objStatusRetorno.CodigoRetorno != 0)
                    {
                        base.ExibirPainelExcecao(objStatusRetorno.Fonte, objStatusRetorno.CodigoRetorno);
                        return;
                    }

                    //carrega os valores de visualizar
                    CarregaValoresVisualizar(objConsultarExtratoDirfRetorno, anoBase);
                    pnlVisualizarDirf.Visible = true;
                    mnuAcoes.Visible = true;
                }
                catch (FaultException<Servico.Dirf.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    if (Request.QueryString["mostrarErro"] != null)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                    Redecard.PN.Comum.SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    if (Request["mostrarErro"] != null)
                    {
                        throw new Exception(this.SessaoAtual.CNPJEntidade + "...." + this.SessaoAtual.CNPJEntidade.Length.ToString(), ex);
                    }
                    Redecard.PN.Comum.SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        private void CarregaValoresVisualizar(Servico.Dirf.ConsultarExtratoDirfRetorno pInformeEstabelecimentoDIRF, Int16 anoBase)
        {
            sttDirf.Descricao = String.Format("DIRF Anual - Calendário {0}", anoBase);
            spnVisualizarReferencia.InnerText = anoBase.ToString();
            spnVisualizarNEstabelecimento.InnerText = base.SessaoAtual.CodigoEntidade.ToString();
            spnVisualizarRazaoSocial.InnerText = pInformeEstabelecimentoDIRF.RazaoSocialEstabelecimento;
            spnVisualizarCnpj.InnerText = String.Format(@"{0:00\.000\.000\/0000\-00}", this.SessaoAtual.CNPJEntidade);
            spnVisualizarEndereco.InnerText = pInformeEstabelecimentoDIRF.EnderecoEstabelecimento;
            spnVisualizarCidadeEstado.InnerText = pInformeEstabelecimentoDIRF.CidadeEstabelecimento + "/" + pInformeEstabelecimentoDIRF.EstadoEstabelecimento;
            spnVisualizarCep.InnerText = pInformeEstabelecimentoDIRF.CepEstabelecimento.ToString().Length > 3 ? pInformeEstabelecimentoDIRF.CepEstabelecimento.ToString().Insert(pInformeEstabelecimentoDIRF.CepEstabelecimento.ToString().Length - 3, "-") : pInformeEstabelecimentoDIRF.CepEstabelecimento.ToString();

            Dictionary<Int32, Decimal> valorRendimentoAcumuladoPorMes = new Dictionary<Int32, Decimal>();
            decimal dValorIREmissorBancoRetidosAcumulado = 0;

            // Obtem os valores de cada mes de cada Banco para apresentar na tela
            for (int i = 0; i < pInformeEstabelecimentoDIRF.Emissores.Length; i++)
            {
                ConsultarExtratoDirfEmissorRetorno emissor = pInformeEstabelecimentoDIRF.Emissores[i];

                //acumular por mes
                for (int j = 1; j < 13; j++)
                {
                    Decimal valor = (Decimal)GetPropertyValue(emissor, String.Format("ValorRepassadoEmissor{0}", j));
                    if (valorRendimentoAcumuladoPorMes.ContainsKey(j))
                        valorRendimentoAcumuladoPorMes[j] += valor;
                    else
                        valorRendimentoAcumuladoPorMes[j] = valor;
                }

                //'BIP51361_1 - Somente para Emissor diferente de 99999999999999
                if (emissor.Cnpj.ToString().Replace(".", string.Empty) != "99999999999999")
                {
                    dValorIREmissorBancoRetidosAcumulado += emissor.ValorIrEmissor1 +
                                                            emissor.ValorIrEmissor2 +
                                                            emissor.ValorIrEmissor3 +
                                                            emissor.ValorIrEmissor4 +
                                                            emissor.ValorIrEmissor5 +
                                                            emissor.ValorIrEmissor6 +
                                                            emissor.ValorIrEmissor7 +
                                                            emissor.ValorIrEmissor8 +
                                                            emissor.ValorIrEmissor9 +
                                                            emissor.ValorIrEmissor10 +
                                                            emissor.ValorIrEmissor11 +
                                                            emissor.ValorIrEmissor12;
                }
            }

            //totalizador da primeira tabela
            this.valorTotalEstabelecimentoRecebido = 0;
            this.valorTotalEstabelecimentoRecebidoIR = 0;
            this.valorTotalEmissores = valorRendimentoAcumuladoPorMes.Sum(v => v.Value);
            this.ValorTotalEstabelecimento = 0;

            List<dynamic> dirfEstabelecimento = new List<dynamic>();

            // retornado 12 Estabelecimentos, cada um é um mes
            for (int i = 0; i < pInformeEstabelecimentoDIRF.Estabelecimentos.Length; i++)
            {
                ConsultarExtratoDirfEstabelecimentoRetorno mesServico = pInformeEstabelecimentoDIRF.Estabelecimentos[i];

                // Totalizador da tabela
                this.valorTotalEstabelecimentoRecebido += mesServico.ValorRecebido;
                this.valorTotalEstabelecimentoRecebidoIR += mesServico.ValorIrRecebido;
                this.ValorTotalEstabelecimento += mesServico.ValorCobrado;

                Decimal rendimentoEmissores = 0;
                // verifica se o registro existe
                if (valorRendimentoAcumuladoPorMes.ContainsKey(i + 1))
                {
                    rendimentoEmissores = valorRendimentoAcumuladoPorMes[i + 1];
                }

                dirfEstabelecimento.Add(new
                {
                    Mes = new CultureInfo("pt-BR").DateTimeFormat.GetMonthName(i + 1),
                    RendimentoRede = mesServico.ValorRecebido,
                    IrRetidoRede = mesServico.ValorIrRecebido,
                    RendimentoEmissores = rendimentoEmissores,
                    RendimentoTotalEstabelecimento = mesServico.ValorCobrado
                });
            }

            grvDirfEstabelecimento.DataSource = dirfEstabelecimento;
            grvDirfEstabelecimento.DataBind();

            if (grvDirfEstabelecimento.Rows.Count > 0)
            {
                grvDirfEstabelecimento.HeaderRow.TableSection = TableRowSection.TableHeader;
                grvDirfEstabelecimento.FooterRow.TableSection = TableRowSection.TableFooter;
            }

            //emissores
            if (pInformeEstabelecimentoDIRF.Emissores.Length > 0)
            {
                rptEmissoresNovo.DataSource = pInformeEstabelecimentoDIRF.Emissores;
                rptEmissoresNovo.DataBind();
            }
            else
            {
                rptEmissoresNovo.Visible = false;
                pnlSemEmissores.Visible = true;
                qdAvisoSemEmissores.Mensagem = "Nenhum emissor encontrado.";
            }

            // quadro de totalizadores
            spnTabelaTotais.InnerHtml = String.Format("totais {0}:", anoBase);
            spnValorTotalRedecard.InnerText = this.valorTotalEstabelecimentoRecebido.ToString("N2");
            spnValorTotalIRRetido.InnerText = this.valorTotalEstabelecimentoRecebidoIR.ToString("N2");
            spnValorTotalEmissores.InnerText = this.valorTotalEmissores.ToString("N2");
            spnValorTotalIREmissor.InnerText = dValorIREmissorBancoRetidosAcumulado.ToString("N2");
            spnValorTotalEstabelecimento.InnerText = this.ValorTotalEstabelecimento.ToString("N2");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "FecharDownload", "CallbackDownloadDirf();", true);
        }

        public static object GetPropertyValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

    }
}

