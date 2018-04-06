using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using System.ServiceModel;

namespace Redecard.PN.OutrasEntidades.SharePoint.WebParts.FMS.ConsultarEstabelecimento
{
    public partial class ConsultarEstabelecimentoUserControl : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnviarEstabelecimento_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Enviar estabelecimento"))
            {
                try
                {
                    Int32 codigoRetorno = 0;
                    Int32 codigoEntidade = 0;
                    using (var entidadeCliente = new EntidadeServico.EntidadeServicoClient())
                    {
                        LimparResultado();

                        if (Int32.TryParse(txtNumeroEstabelecimento.Text, out codigoEntidade))
                        {
                            EntidadeServico.Entidade entidade = entidadeCliente.ConsultarDadosCompletos(out codigoRetorno, codigoEntidade, false);
                            if (codigoRetorno > 0)
                                base.ExibirPainelExcecao("EntidadeServico.ConsultarDadosCompletos", codigoRetorno);
                            else
                            {
                                CarregarDadosEntidade(entidade);
                                LimparPesquisa();
                                pnlVazio.Visible = !(pnlEstabelecimento.Visible = (entidade.Codigo > 0));
                                ((QuadroAviso)qdAvisoSemEstabelecimento).CarregarMensagem("Aviso", "Nenhum estabelecimento encontrado.");
                            }
                        }
                        else
                            base.Alert("Número do Estabelecimento inválido", false);
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnviarCNPJ_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Enviar CNPJ"))
            {
                try
                {
                    Double CNPJ = 0;
                    Int32 codigoRetorno = 0;
                    Int32 codigoEntidade = 0;
                    LimparResultado();
                    EntidadeServico.Entidade[] entidades;

                    if (Double.TryParse(txtCNPJ.Text, out CNPJ))
                    {
                        using (var entidadeCliente = new EntidadeServico.EntidadeServicoClient())
                        {
                            entidades = entidadeCliente.ConsultarPorCNPJ(out codigoRetorno, 0, 4, CNPJ);

                            if (codigoRetorno > 0)
                            {
                                base.ExibirPainelExcecao("EntidadeServico.ConsultarPorCNPJ", codigoRetorno);
                            }
                            else
                            {
                                if (entidades.Length > 0)
                                {
                                    if (entidades.Length > 1)
                                    {
                                        base.ExibirPainelExcecao("CNPJ cadastrado para mais de um estabelecimento por tratar-se de uma rede. Favor efetuar a pesquisa por número de Máquina ou Terminal.", "301");
                                    }
                                    else
                                    {
                                        codigoEntidade = entidades[0].Codigo;
                                        EntidadeServico.Entidade entidade = entidadeCliente.ConsultarDadosCompletos(out codigoRetorno, codigoEntidade, false);
                                        if (codigoRetorno > 0)
                                            base.ExibirPainelExcecao("EntidadeServico.ConsultarDadosCompletos", codigoRetorno);
                                        else
                                        {
                                            CarregarDadosEntidade(entidade);
                                            LimparPesquisa();
                                        }
                                        pnlVazio.Visible = !(pnlEstabelecimento.Visible = (codigoRetorno == 0));
                                        ((QuadroAviso)qdAvisoSemEstabelecimento).CarregarMensagem("Aviso", "Nenhum estabelecimento encontrado.");
                                    }
                                }
                                else
                                    pnlEstabelecimento.Visible = !(pnlVazio.Visible = false);
                            }
                        }
                    }
                    else
                        base.Alert("Número do CNPJ inválido.", false);
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        protected void btnEnviarTerminal_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Enviar terminal"))
            {
                try
                {
                    Int32 codigoRetorno = 0;
                    Int32 codigoEntidade = 0;
                    String numeroTerminal = "0";
                    EntidadeServico.Entidade entidade;
                    LimparResultado();

                    if (!String.IsNullOrEmpty(txtTerminal.Text.Trim()))
                    {
                        numeroTerminal = txtTerminal.Text.Trim();
                        using (var entidadeCliente = new EntidadeServico.EntidadeServicoClient())
                        {
                            entidade = entidadeCliente.ConsultarPorTerminal(out codigoRetorno, numeroTerminal);

                            if (codigoRetorno > 0)
                            {
                                base.ExibirPainelExcecao("EntidadeServico.ConsultarPorTerminal", codigoRetorno);
                            }
                            else
                            {
                                codigoEntidade = entidade.Codigo;
                                entidade = entidadeCliente.ConsultarDadosCompletos(out codigoRetorno, codigoEntidade, false);
                                if (codigoRetorno > 0)
                                    base.ExibirPainelExcecao("EntidadeServico.ConsultarDadosCompletos", codigoRetorno);
                                else
                                {
                                    txtTerminal.Text = numeroTerminal;
                                    CarregarDadosEntidade(entidade);
                                    LimparPesquisa();
                                }
                                pnlVazio.Visible = !(pnlEstabelecimento.Visible = (codigoRetorno == 0));
                                ((QuadroAviso)qdAvisoSemEstabelecimento).CarregarMensagem("Aviso", "Nenhum estabelecimento encontrado.");
                            }
                        }
                    }
                    else
                        base.Alert("Número do Terminal inválido.", false);
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }
        
        /// <summary>
        /// Preenche na tela a entidade encontrada
        /// </summary>
        /// <param name="entidade"></param>
        private void CarregarDadosEntidade(EntidadeServico.Entidade entidade)
        {
            using (Logger Log = Logger.IniciarLog("Carregar dados entidade"))
            {
                try
                {
                    if (entidade != null)
                    {
                        lblNumeroEntidade.Text = entidade.Codigo.ToString();
                        lblRazaoSocial.Text = entidade.RazaoSocial;
                        lblNomeFantasia.Text = entidade.NomeEntidade;
                        lblEndereco.Text = entidade.Endereco;
                        lblBairro.Text = entidade.Bairro;
                        lblCEP.Text = entidade.CEP;
                        lblEstado.Text = entidade.Estado;
                        lblCidade.Text = entidade.Cidade;
                        lblTelefone.Text = entidade.Telefone;
                        lblTelefone2.Text = entidade.Telefone2;
                        lblFAX.Text = entidade.FAX;
                        lblBairro.Text = entidade.Bairro;
                        lblRamoAtividade.Text = entidade.RamoAtividade;
                        lblCNPJ.Text = entidade.CNPJEntidade;
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Limpar campos de pesquisa
        /// </summary>
        private void LimparPesquisa()
        {
            using (Logger Log = Logger.IniciarLog("Limpando pesquisa"))
            {
                try
                {
                    txtTerminal.Text = String.Empty;
                    txtCNPJ.Text = String.Empty;
                    txtNumeroEstabelecimento.Text = String.Empty;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        private void LimparResultado()
        {
            try
            {
                lblNumeroEntidade.Text = String.Empty;
                lblRazaoSocial.Text = String.Empty;
                lblTerminal.Text = String.Empty;
                lblNomeFantasia.Text = String.Empty;
                lblEndereco.Text = String.Empty;
                lblBairro.Text = String.Empty;
                lblCEP.Text = String.Empty;
                lblEstado.Text = String.Empty;
                lblTelefone.Text = String.Empty;
                lblTelefone2.Text = String.Empty;
                lblFAX.Text = String.Empty;
                lblBairro.Text = String.Empty;
                lblBairro.Text = String.Empty;
                lblRamoAtividade.Text = String.Empty;
                lblCNPJ.Text = String.Empty;
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }
    }
}
