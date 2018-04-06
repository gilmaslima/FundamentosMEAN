#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [04/06/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using Redecard.PN.Comum;
using Microsoft.SharePoint.IdentityModel;
using System.ServiceModel;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Utilities;
using System.Web;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Redecard.PN.DadosCadastrais.SharePoint
{
    public partial class AlteracaoSenha : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void TrocarSenhaClickHandle(object sender, Int32 codigo);

        /// <summary>
        /// 
        /// </summary>
        public event TrocarSenhaClickHandle SenhaTrocada;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (InformacaoUsuario.Existe())
                {
                    InformacaoUsuario dadosUsuarioCP = InformacaoUsuario.Recuperar();
                    ltNomeUsuario.Text = dadosUsuarioCP.Usuario;
                    CarregarValidacaoKomerci(dadosUsuarioCP);
                }
            }
        }

        private void CarregarValidacaoKomerci(InformacaoUsuario dadosUsuarioCP)
        {

            using (Logger Log = Logger.IniciarLog("Verifica se o PV do usuário é komerci"))
            {
                Int32 codigoRetorno = 0;
                Int32 codigoTecnologia = 0;
                try
                {
                    using (var servicoCliente = new EntidadeServico.EntidadeServicoClient())
                    {
                        codigoTecnologia = servicoCliente.ConsultarTecnologiaEstabelecimento(out codigoRetorno, dadosUsuarioCP.NumeroPV);

                        if (codigoTecnologia.Equals(25) || codigoTecnologia.Equals(26) || codigoTecnologia.Equals(23))
                        {
                            lblCriterioSenha.Text = "- Ter de 8 a 20 caracteres, composta por letras e números";

                            revTamanhoSenha.ValidationExpression = "[\\w\\s]{8,20}";
                            revTamanhoSenha.Text = "A senha deve conter, no mínimo, oito caracteres, e, no máximo, vinte caracteres e sem caracteres especiais.";
                            revLetraNumero.ValidationExpression = "^.*(?=.{8,20})(?=.*[a-zA-Z])(?=.*[0-9]).*$";
                            revLetraNumero.Text = "Senha deve conter pelo menos uma letra e um número.";
                        }
                        else
                        {
                            lblCriterioSenha.Text = "- Ter de 6 a 20 caracteres, composta por letras e números";

                            revTamanhoSenha.ValidationExpression = "[\\w\\s]{6,20}";
                            revTamanhoSenha.Text = "A senha deve conter, no mínimo, seis caracteres, e, no máximo, vinte caracteres e sem caracteres especiais.";
                            revLetraNumero.ValidationExpression = "^.*(?=.{6,20})(?=.*[a-zA-Z])(?=.*[0-9]).*$";
                            revLetraNumero.Text = "Senha deve conter pelo menos uma letra e um número.";
                        }
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO);
                }
            }

        }

        /// <summary>
        /// Voltar para a página de alteração de senha, os dados já foram previamente confirmados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Voltar(object sender, EventArgs e)
        {
            String url = String.Empty;
            url = Util.BuscarUrlRedirecionamento("/_layouts/DadosCadastrais/AlteracaoSenha.aspx", SPUrlZone.Internet);
            Response.Redirect(url, true);
        }

        /// <summary>
        /// Exibe painel de erro customizado
        /// </summary>
        /// <param name="erro">Mensagem de erro</param>
        private void ExibirErro(String erro)
        {
            //((QuadroAviso)quadroAviso).CarregarImagem("/_layouts/DadosCadastrais/IMAGES/CFP/Ico_Alerta.png");
            ((QuadroAviso)quadroAviso).CarregarMensagem("Atenção, dados inválidos.", erro);
            pnlErroPrincipal.Visible = true;
            pnlBotaoErro.Visible = true;
            pnlPrincipal.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TrocarSenha(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Troca de senha"))
            {
                try
                {
                    Int32 codigoRetorno;
                    if (InformacaoUsuario.Existe())
                    {
                        InformacaoUsuario dadosUsuarioCP = InformacaoUsuario.Recuperar();
                        using (UsuarioServico.UsuarioServicoClient servicoCliente = new UsuarioServico.UsuarioServicoClient())
                        {
                            // Cria objeto entidade do usuário logado
                            UsuarioServico.Entidade entidade = new UsuarioServico.Entidade()
                            {
                                Codigo = dadosUsuarioCP.NumeroPV,
                                GrupoEntidade = new UsuarioServico.GrupoEntidade() { Codigo = dadosUsuarioCP.GrupoEntidade }
                            };

                            // Consulta usuário
                            var usuarios = servicoCliente.ConsultarPorCodigoEntidade(out codigoRetorno,
                                dadosUsuarioCP.Usuario,
                                entidade);

                            if (codigoRetorno > 0)
                            {
                                base.ExibirPainelExcecao("UsuarioServico.Consultar", codigoRetorno);
                            }
                            else
                            {
                                UsuarioServico.Usuario UsuarioEditar = usuarios[0];

                                String _senhaAtual = null;

                                if (!String.IsNullOrEmpty(txtSenhaAtual.Text))
                                    _senhaAtual = EncriptadorSHA1.EncryptString(txtSenhaAtual.Text);

                                System.Diagnostics.Trace.WriteLine(String.Format("Criptografou a senha: {0}", _senhaAtual));
                                System.Diagnostics.Trace.WriteLine(UsuarioEditar.SenhaTemporaria);
                                System.Diagnostics.Trace.WriteLine(UsuarioEditar.Senha);
                                System.Diagnostics.Trace.WriteLine(dadosUsuarioCP.CodigoRetorno);

                                if (UsuarioEditar.SenhaTemporaria != _senhaAtual && UsuarioEditar.Senha != _senhaAtual)
                                {
                                    this.ExibirErro("Senha atual é inválida");
                                    return;
                                }

                                String _senhaNova = null;

                                if (!String.IsNullOrEmpty(txtSenha.Text))
                                    _senhaNova = EncriptadorSHA1.EncryptString(txtSenha.Text);

                                System.Diagnostics.Trace.WriteLine(String.Format("Nova Criptografia: {0}", _senhaNova));

                                if (_senhaNova == null ||
                                     UsuarioEditar.Senha.Trim().ToUpper() == _senhaNova.ToUpper() ||
                                     UsuarioEditar.SenhaTemporaria.Trim().ToUpper() == _senhaNova.ToUpper())
                                {
                                    System.Diagnostics.Trace.WriteLine("Ocorreu um erro ao verificar a validade da senha.");
                                    this.ExibirErro("Nova senha é inválida. Não é possível utilizar a senha atual (Temporária).");
                                }
                                else
                                {
                                    System.Diagnostics.Trace.WriteLine("Atualizar a Senha!!!");

                                    Int32 tecnologia = 0;

                                    using (EntidadeServico.EntidadeServicoClient servicoEntidade = new EntidadeServico.EntidadeServicoClient())
                                    {
                                        tecnologia = servicoEntidade.ConsultarTecnologiaEstabelecimento(out codigoRetorno, dadosUsuarioCP.NumeroPV);
                                    }

                                    if (codigoRetorno == 0)
                                    {
                                        codigoRetorno = servicoCliente.AtualizarSenha(UsuarioEditar, _senhaNova, VerificaKomerci(tecnologia), false);
                                        System.Diagnostics.Trace.WriteLine(codigoRetorno);

                                        if (codigoRetorno > 0)
                                            base.ExibirPainelExcecao("UsuarioServico.AtualizarSenha", codigoRetorno);
                                        else
                                            if (codigoRetorno == 0)
                                            {
                                                pnlPrincipal.Visible = false;
                                                pnlConfirmacao.Visible = true;

                                                dadosUsuarioCP.Senha = txtSenha.Text;
                                                InformacaoUsuario.Salvar(dadosUsuarioCP);
                                            }
                                    }
                                    else
                                        this.ExibirErro("Não foi possível recuperar o tipo de tecnologia do Estabelecimento.");
                                }
                            }
                        }
                        // chamar evento de senha alterada
                        InvocarSenhaAlterada(codigoRetorno);
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tecnologia"></param>
        protected Boolean VerificaKomerci(Int32 tecnologia)
        {
            return tecnologia == 25 || tecnologia == 26 || tecnologia == 23;
        }

        /// <summary>
        /// Cancelar o processo de Confirmação Positiva
        /// </summary>
        protected void Cancelar(object sender, EventArgs e)
        {
            String url = String.Empty;
            url = Util.BuscarUrlRedirecionamento("/", SPUrlZone.Default);
            Response.Redirect(url, true);
        }

        /// <summary>
        /// Exibe erro no painel
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        private void ExibirErro(String fonte, Int32 codigo)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);
            //((QuadroAviso)quadroAviso).CarregarImagem("/_layouts/DadosCadastrais/IMAGES/CFP/Ico_Alerta.png");
            ((QuadroAviso)quadroAviso).CarregarMensagem("Atenção, dados inválidos.", mensagem);
            pnlErro.Visible = true;
            pnlPrincipal.Visible = false;
        }

        /// <summary>
        /// Exibe painel customizado
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <returns>Painel customizado</returns>
        private Panel RetornarPainelExcecao(String fonte, Int32 codigo)
        {
            ApplicationBasePage page = new ApplicationBasePage();
            return page.RetornarPainelExcecao(fonte, codigo);
        }

        /// <summary>
        /// Exibe painel customizado
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <returns>Painel customizado</returns>
        private Panel RetornarPainelExcecao(string erro)
        {
            ApplicationBasePage page = new ApplicationBasePage();
            return page.RetornarPainelExcecao(erro);
        }

        /// <summary>
        /// Invoca o evento de validação, esse evento deve estar atachado na classe que hospeda o controle.
        /// </summary>
        protected void InvocarSenhaAlterada(Int32 codigoRetorno)
        {
            if (SenhaTrocada != null)
                SenhaTrocada(this, codigoRetorno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ctvCaracterRepetido_ServerValidate(object source, ServerValidateEventArgs args)
        {
            String senha = args.Value;
            Char caracter1 = args.Value.ToString()[0];
            Char caracter2 = args.Value.ToString()[1];

            for (int i = 2; i < args.Value.Length; i++)
            {
                if (senha[i].Equals(caracter1) && senha[i].Equals(caracter2))
                {
                    args.IsValid = false;
                    return;
                }
                else
                {
                    caracter1 = senha[i - 2];
                    caracter2 = senha[i - 1];
                }
            }
            args.IsValid = true;
        }
    }
}
