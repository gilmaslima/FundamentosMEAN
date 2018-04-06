using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.Web.Configuration;
using System.Web;
using Redecard.PN.Boston.Sharepoint.Negocio;
using System.Web.Services;
using Microsoft.SharePoint;
using System.Linq;
using System.Globalization;
using System.ServiceModel;
using System.Collections.Generic;

namespace Redecard.PN.Boston.Sharepoint.WebParts.PedidoNovoEquipamento
{
    public partial class PedidoNovoEquipamentoUserControl : UserControlBase
    {
        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    //Se o PV está na lista de PVs Mobile Rede 2.0, redireciona para
                    //a nova página de Novo Leitor de Cartão
                    if (VerificarEstabelecimentoMobile20(SessaoAtual.CodigoEntidade))
                    {
                        Response.Redirect("NovoLeitorCartao.aspx", false);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        return;
                    }

                    CarregarDadosControles();
                }
            }
            catch (FaultException<GEPontoVen.ModelosErroServicos> fe)
            {
                Logger.GravarErro("MPos - Pedido Novo Equipamento", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.MsgErro, fe.Detail.CodErro ?? 600);
            }
            catch (FaultException<WFAdministracao.ModelosErroServicos> fe)
            {
                Logger.GravarErro("MPos - Pedido Novo Equipamento", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.MsgErro, fe.Detail.CodigoErro ?? 600);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("MPos - Pedido Novo Equipamento", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("MPos - Pedido Novo Equipamento", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("MPos - Pedido Novo Equipamento", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Carrega os valores dos controles da página
        /// </summary>
        private void CarregarDadosControles()
        {
            CarregarTaxaDeAtivacao();
            CarregarDadosEndereco();
            MudarImagemEquipamento();
        }

        /// <summary>
        /// Carrega os dados do endereço de instalação
        /// </summary>
        private void CarregarDadosEndereco()
        {
            var endereco = Servicos.GetEnderecoInstalacaoPorPV(SessaoAtual.CodigoEntidade);
            ltlCEP.Text = endereco.CEP;
            ltlLogradouro.Text = String.Format("{0} {1}, {2}", endereco.Logradouro, endereco.Numero, endereco.Complemento);
            ltlEstado.Text = endereco.Estado;
            ltlCidade.Text = endereco.Cidade;
            ltlBairro.Text = endereco.Bairro;
            ltlPais.Text = "Brasil";
        }

        /// <summary>
        /// Carrega o valor da taxa de ativação
        /// </summary>
        private void CarregarTaxaDeAtivacao()
        {
            ltlTaxaAtivacao.Text = Servicos.GetTaxaAtivacao(SessaoAtual.CodigoCanal, SessaoAtual.CodigoCelula, null, null, null, null, null, 30);
        }

        /// <summary>
        /// Muda Imagem do equipamento de acordo com o Canal e Célula
        /// </summary>
        private void MudarImagemEquipamento()
        {
            if (SessaoAtual.CodigoCanal == 26 && SessaoAtual.CodigoCelula == 503)
                imgEquipamento.ImageUrl = "/_layouts/MobileRede/imagens/Device_vivo.png";
        }

        /// <summary>
        /// Evento do clique no botão de continuar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            try
            {
                MontaIFramePagamento();
            }
            catch (Exception ex)
            {
                Logger.GravarErro("MPos - Pedido Novo Equipamento", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Evento do clique no botão de voltar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Monta e exibe IFrame de Pagamento
        /// </summary>
        private void MontaIFramePagamento()
        {
            String titulo = "Pagamento";
            String src = WebConfigurationManager.AppSettings["urlDataCash"].ToString();
#if !DEBUG
                    String token = GetToken();
                    src = String.Format(@"{0}?token={1}", src, token);
#endif

            String script = String.Format("exibirIFramePagamento('{0}', '{1}');",
                HttpUtility.HtmlEncode(titulo), HttpUtility.HtmlEncode(src));
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Key_" + this.ClientID, script, true);
        }

        /// <summary>
        /// Retorna token do datacash
        /// </summary>
        /// <returns></returns>
        private String GetToken()
        {
            /* -- Remover condicional (Solicitação feita pela José de Castro) -- AGA
            String numPdv = SessaoAtual.CodigoCanal == 26 && SessaoAtual.CodigoCelula == 503 ?
                WebConfigurationManager.AppSettings["numPdvEstabelecimento"].ToString() :
                WebConfigurationManager.AppSettings["numPdvEstabelecimentoVivo"].ToString();
            */
            String numPdvConfig = WebConfigurationManager.AppSettings["numPdvEstabelecimento"];
            String numPdv = (String.IsNullOrEmpty(numPdvConfig) ? String.Empty : numPdvConfig);

            Decimal valorTransacao = Decimal.Parse(ltlTaxaAtivacao.Text, NumberStyles.Currency);
            Random random = new Random();
            String numPedido = String.Format(@"{0}-{1}", base.SessaoAtual.CodigoEntidade, random.Next(100000, 999999));
            Int32 qtdParcela = WebConfigurationManager.AppSettings["qtdParcela"].ToInt32();
            String urlRetorno = String.Format("{0}/{1}", SPContext.Current.Web.Url, WebConfigurationManager.AppSettings["urlRetornoFechado"]);

            var enderecoComercial = Servicos.GetEnderecoComercialPorPV(SessaoAtual.CodigoEntidade);
            var enderecoInstalacao = Servicos.GetEnderecoInstalacaoPorPV(SessaoAtual.CodigoEntidade);

            String token = Servicos.GetToken(numPdv, valorTransacao, numPedido, qtdParcela, urlRetorno, new Int32[]{1});
            return token;
        }

        /// <summary>
        /// Verifica se o estabelecimento está na lista de PVs candidatos ao Mobile Rede 2.0.
        /// </summary>
        /// <param name="codigoEntidade">Número do estabelecimento</param>
        /// <returns>Se está na lista do Mobile Rede 2.0</returns>
        private static Boolean VerificarEstabelecimentoMobile20(Int32 codigoEntidade)
        {
            var codigoEstabelecimentos = new List<Int32>();

            try
            {
                //Recupera a lista
                SPList lista = SPContext.Current.Web.Lists.TryGetList("PVs Mobile 2.0");
                if (lista != null)
                {
                    Char[] separadores = new[] { ';', ',', ' ' };

                    //Recupera os itens
                    SPListItem[] itens = lista.Items.Cast<SPListItem>().ToArray();
                    if (itens != null && itens.Length > 0)
                    {
                        //Processa os itens, e recupera os PVs
                        String[] linhas = itens.Select(item => Convert.ToString(item["Title"])).ToArray();
                        foreach (String linha in linhas)
                        {
                            //Cada item da lista pode conter mais de um PV, separado por ",", ";" ou " "
                            String[] conteudoPvs = linha.Split(separadores, StringSplitOptions.RemoveEmptyEntries);
                            if (conteudoPvs != null && conteudoPvs.Length > 0)
                            {
                                Int32[] pvs = conteudoPvs
                                    .Select(pv => pv.ToInt32Null())
                                    .Where(pv => pv.HasValue)
                                    .Select(pv => pv.Value).ToArray();
                                if (pvs != null && pvs.Length > 0)
                                    codigoEstabelecimentos.AddRange(pvs);
                            }
                        }
                    }
                }
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Erro durante consulta da Lista 'PVs Mobile 2.0'", ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante consulta da Lista 'PVs Mobile 2.0'", ex);
            }

            //Se o PV está na lista de PVs candidatos
            return codigoEstabelecimentos.Contains(codigoEntidade);
        }
    }
}
