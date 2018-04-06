using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.Modelo;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato.SaldosEmAberto
{
    public partial class CartadeCircularizacao : BaseUserControl
    {
        const String TEXTO_ENDERECO = "ENDREÇO: {0}";
        const String TEXTO_BAIRRO = "BAIRRO: {0} CIDADE: {1} UF: {2}";
        const String TEXTO_CEP = "CEP: {0}";

        protected void Page_Load(object sender, EventArgs e)
        {
            //CarregarCarta();
        }

        /// <summary>
        /// Visualiza a carta
        /// </summary>
        /// <param name="codigoEntidade">Código do Estabelecimento</param>
        /// <param name="dataSaldo">Data inicial do período solicitado</param>
        /// <param name="valorLiquido">Valor Líquido, este valor é obtido do valor líquido do quadro totalizador de bandeiras </param>
        public void VisualizarCarta(Int32 codigoEntidade, DateTime dataSaldo, Decimal valorLiquido)
        {
            using (Logger Log = Logger.IniciarLog("Carta de Circularização - Visualizar Carta"))
            {
                try
                {
                    DateTime now = DateTime.Now;

                    lblDia.Text = now.Day.ToString();

                    switch (now.Month)
                    {
                        case 1: lblMes.Text = "Janeiro"; break;
                        case 2: lblMes.Text = "Fevereiro"; break;
                        case 3: lblMes.Text = "Março"; break;
                        case 4: lblMes.Text = "Abril"; break;
                        case 5: lblMes.Text = "Maio"; break;
                        case 6: lblMes.Text = "Junho"; break;
                        case 7: lblMes.Text = "Julho"; break;
                        case 8: lblMes.Text = "Agosto"; break;
                        case 9: lblMes.Text = "Setembro"; break;
                        case 10: lblMes.Text = "Outubro"; break;
                        case 11: lblMes.Text = "Novembro"; break;
                        case 12: lblMes.Text = "Dezembro"; break;
                    }

                    lblAno.Text = now.Year.ToString();

                    String razaoSocial = string.Empty;
                    String endereco = string.Empty;
                    String bairro = string.Empty;
                    String cep = string.Empty;
                    Int32 codRetorno = default(Int32);

                    // consulta dados do estabelecimento
                    using (var contexto = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                    {
                        Redecard.PN.Comum.SharePoint.EntidadeServico.Entidade entidade = contexto.Cliente.ConsultarDadosCompletos(out codRetorno, codigoEntidade, false);

                        razaoSocial = entidade.RazaoSocial;
                        endereco = String.Format(TEXTO_ENDERECO, entidade.Endereco);
                        bairro = String.Format(TEXTO_BAIRRO, entidade.Bairro, entidade.Cidade, entidade.UF); ;
                        cep = String.Format(TEXTO_CEP, entidade.CEP);
                    }

                    lblRazaoSocial.Text = razaoSocial.ToUpper();
                    lblEndereco.Text = endereco.ToUpper();
                    lblBairro.Text = bairro.ToUpper();
                    lblCep.Text = cep;
                    lblDataSaldoAberto.Text = dataSaldo.ToString("dd/MM/yyyy");

                    lblEstabelecimento.Text = SessaoAtual.CodigoEntidade.ToString();
                    lblValorCartao.Text = valorLiquido.ToString("N2");
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                }
            }
        }
    }
}
