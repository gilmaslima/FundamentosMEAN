using System;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;
using System.ServiceModel;
using System.Collections.Generic;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais
{
    public partial class ModeloAlteracaoDados : ApplicationPageBaseAutenticada
    {
        /// <summary>
        /// Sessão do usuário
        /// </summary>
        private Sessao _sessao = null;

        /// <summary>
        /// Sessão atual do usuário
        /// </summary>
        private Sessao SessaoAtual
        {
            get 
            {
                if (_sessao != null && Sessao.Contem())
                    return _sessao;
                else
                {
                    if (Sessao.Contem())
                    {
                        _sessao = Sessao.Obtem();
                    }
                    return _sessao;
                }
            }
        }

        /// <summary>
        /// Inicialização da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                    CarregarDados();
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                pnlErro.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                pnlErro.Controls.Add(base.RetornarPainelExcecao(FONTE, CODIGO_ERRO));
            }
        }

        /// <summary>
        /// Carrega as informações em tela
        /// </summary>
        private void CarregarDados()
        {
            var entidadeCliente = new EntidadeServico.EntidadeServicoClient();
            CarregarDadosGerais(entidadeCliente);
            CarregarEstados(entidadeCliente);
            CarregarEnderecoEstabelecimento(entidadeCliente);
            CarregarEnderecoCorrespondencia(entidadeCliente);
        }

        /// <summary>
        /// Carregar lista de Estados na tela
        /// </summary>
        /// <param name="entidadeCliente"></param>
        private void CarregarEstados(EntidadeServico.EntidadeServicoClient entidadeCliente)
        {
            try
            {
                var listaEstados = entidadeCliente.ConsultarEstados();
                if (listaEstados != null)
                {
                    ddlEstado.DataSource = listaEstados;
                    ddlEstado.DataTextField = "CodigoUF";
                    ddlEstado.DataValueField = "NomeUF";
                    ddlEstado.DataBind();

                    ddlEstadoCorrespondecia.DataSource = listaEstados;
                    ddlEstadoCorrespondecia.DataTextField = "CodigoUF";
                    ddlEstadoCorrespondecia.DataValueField = "NomeUF";
                    ddlEstadoCorrespondecia.DataBind();
                }
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                pnlErro.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                pnlErro.Controls.Add(base.RetornarPainelExcecao(FONTE, CODIGO_ERRO));
            }
        }

        /// <summary>
        /// Carregar os Dados Gerais do Estabelecimento na tela
        /// </summary>
        /// <param name="entidadeCliente"></param>
        private void CarregarDadosGerais(EntidadeServico.EntidadeServicoClient entidadeCliente)
        {
            try
            {
                if (SessaoAtual != null)
                {
                    Int32 codigoRetorno;
                    var dadosGerais = entidadeCliente.ConsultarDadosGerais(out codigoRetorno, SessaoAtual.CodigoEntidade);

                    if (codigoRetorno > 0)
                    {
                        pnlErro.Controls.Add(base.RetornarPainelExcecao("EntidadeServico.ConsultarDadosGerais", codigoRetorno));
                    }
                    else
                    {
                        if (dadosGerais != null)
                        {
                            txtRazaoSocial.Text = dadosGerais.RazaoSocial.Trim().ToUpper();
                            txtNomeFantasia.Text = dadosGerais.NomeFantasia.Trim().ToUpper();
                            txtCPF.Text = dadosGerais.CPNJ.Trim().ToUpper();

                            txtDDD.Text = dadosGerais.DDD;
                            txtTelefone.Text = dadosGerais.Telefone;
                            txtRamal.Text = dadosGerais.Ramal;

                            lblNumeroEstabelecimento.Text = SessaoAtual.CodigoEntidade.ToString();
                            txtNumeroPlaqueta.Text = dadosGerais.Plaqueta;

                            txtNomeProprietario1.Text = dadosGerais.Proprietario1.Trim().ToUpper();
                            txtDataProprietario1.Text = dadosGerais.DataNascimentoProprietario1;
                            txtCPFProprietario1.Text = dadosGerais.CPFProprietario1;

                            txtNomeProprietario2.Text = dadosGerais.Proprietario2.Trim().ToUpper();
                            txtDataProprietario2.Text = dadosGerais.DataNascimentoProprietario2;
                            txtCPFProprietario2.Text = dadosGerais.CPFProprietario2;

                            txtNomeProprietario3.Text = dadosGerais.Proprietario3.Trim().ToUpper();
                            txtDataProprietario3.Text = dadosGerais.DataNascimentoProprietario3;
                            txtCPFProprietario3.Text = dadosGerais.CPFProprietario3;
                        }
                    }
                }
                else
                    this.ExibirErro("Sessão inválida.");
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                pnlErro.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                pnlErro.Controls.Add(base.RetornarPainelExcecao(FONTE, CODIGO_ERRO));
            }
        }

        /// <summary>
        /// Carregar o endereço de Estabelecimento na tela
        /// </summary>
        /// <param name="entidadeCliente"></param>
        private void CarregarEnderecoEstabelecimento(EntidadeServico.EntidadeServicoClient entidadeCliente)
        {
            try
            {
                if (SessaoAtual != null)
                {
                    Int32 codigoRetorno;

                    var enderecoEstabelecimento = entidadeCliente.ConsultarEndereco(out codigoRetorno, SessaoAtual.CodigoEntidade, "E")[0];

                    if (codigoRetorno > 0)
                    {
                        pnlErro.Controls.Add(base.RetornarPainelExcecao("EntidadeServico.ConsultarEndereco", codigoRetorno));
                    }
                    else
                    {
                        if (enderecoEstabelecimento != null)
                        {
                            txtEnderecoLoja.Text = enderecoEstabelecimento.EnderecoEstabelecimento;
                            txtNumeroLoja.Text = enderecoEstabelecimento.Numero;
                            txtComplementoLoja.Text = enderecoEstabelecimento.Complemento;
                            txtBairroLoja.Text = enderecoEstabelecimento.Bairro;
                            txtCEPLoja.Text = enderecoEstabelecimento.CEP;
                            txtCidadeLoja.Text = enderecoEstabelecimento.Cidade;
                            ddlEstado.SelectedValue = enderecoEstabelecimento.UF.Trim();
                        }
                    }
                }
                else
                    this.ExibirErro("Sessão inválida.");

            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                pnlErro.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                pnlErro.Controls.Add(base.RetornarPainelExcecao(FONTE, CODIGO_ERRO));
            }
        }

        /// <summary>
        /// Carregar o endereço de Correspondência na tela
        /// </summary>
        /// <param name="entidadeCliente"></param>
        private void CarregarEnderecoCorrespondencia(EntidadeServico.EntidadeServicoClient entidadeCliente)
        {
            try
            {
                if (SessaoAtual != null)
                {
                    Int32 codigoRetorno;
                    var enderecoCorrespondencia = entidadeCliente.ConsultarEndereco(out codigoRetorno, SessaoAtual.CodigoEntidade, "C")[0];

                    if (codigoRetorno > 0)
                    {
                        pnlErro.Controls.Add(base.RetornarPainelExcecao("EntidadeServico.ConsultarEndereco", codigoRetorno));
                    }
                    else
                    {
                        if (enderecoCorrespondencia != null)
                        {
                            txtEnderecoCorrespondencia.Text = enderecoCorrespondencia.EnderecoEstabelecimento;
                            txtNumeroCorrespondencia.Text = enderecoCorrespondencia.Numero;
                            txtComplementoCorrespondecia.Text = enderecoCorrespondencia.Complemento;
                            txtBairroCorrespondencia.Text = enderecoCorrespondencia.Bairro;
                            txtCEPCorrespondencia.Text = enderecoCorrespondencia.CEP;
                            txtCidadeCorrespondencia.Text = enderecoCorrespondencia.Cidade;
                            ddlEstadoCorrespondecia.SelectedValue = enderecoCorrespondencia.UF.Trim();
                        }
                    }
                }
                else
                    this.ExibirErro("Sessão inválida.");
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                pnlErro.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                pnlErro.Controls.Add(base.RetornarPainelExcecao(FONTE, CODIGO_ERRO));
            }
        }

        /// <summary>
        /// Exibe uma mensagem de erro/aviso
        /// </summary>
        /// <param name="erro">Mensagem</param>
        private void ExibirErro(String erro)
        {
            pnlErro.Controls.Add(base.RetornarPainelExcecao(erro));
            pnlErro.Visible = true;
            pnlCadastro.Visible = false;
        }
    }
}
