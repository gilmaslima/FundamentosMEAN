#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [06/05/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.IO;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Net;
using Redecard.PN.Comum;
using System.ServiceModel;
using System.Web;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Microsoft.SharePoint.Administration;
using System.Configuration;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais {
    /// <summary>
    /// Página que realiza a confirmação positiva do usuário antes do login
    /// </summary>
    public class ConcluidoCarta : ApplicationPageBaseAnonima {
        #region Controles
        /// <summary>
        /// 
        /// </summary>
        protected Panel pnlErroPrincipal;
        /// <summary>
        /// 
        /// </summary>
        protected Literal ltErro;
        /// <summary>
        /// 
        /// </summary>
        protected SharePoint.ProcessoConcluidoCarta ctrlProcessoConcluido;
        /// <summary>
        /// 
        /// </summary>
        protected QuadroAviso quadroAviso;
        #endregion

        /// <summary>
        /// Voltar para o inicio do processo
        /// </summary>
        protected void Voltar(Object sender, EventArgs e) {
            String url = String.Empty;
            if (InformacaoUsuario.Existe()) {
                InformacaoUsuario usuario = InformacaoUsuario.Recuperar();
                if (usuario.EsqueciUsuario)
                    url = Util.BuscarUrlRedirecionamento("/_layouts/DadosCadastrais/EsqueciUsuario.aspx", SPUrlZone.Internet);
                else
                    url = Util.BuscarUrlRedirecionamento("/_layouts/DadosCadastrais/EsqueciSenha.aspx", SPUrlZone.Internet);
            }
            else {
                url = Util.BuscarUrlRedirecionamento("/", SPUrlZone.Default);
            }
            if (!String.IsNullOrEmpty(url)) {
                Response.Redirect(url, true);
            }
        }

        /// <summary>
        /// Verificar se o usuário realizou a confirmação positiva com sucesso.
        /// </summary>
        private bool ValidarConfirmacaoPositiva() {
            if (InformacaoUsuario.Existe()) {
                InformacaoUsuario dados = InformacaoUsuario.Recuperar();

                if (!dados.Confirmado) {
                    this.ExibirErro("Não é possível a abertura da tela de confirmação positiva manualmente, tente efetuar login novamente.");
                    return false;
                }
                return true;
            }
            else {
                this.ExibirErro("Não é possível a abertura da tela de confirmação positiva manualmente, tente efetuar login novamente.");
                return false;
            }
        }

        /// <summary>
        /// Exibe painel de erro customizado
        /// </summary>
        /// <param name="erro">Mensagem de erro</param>
        private void ExibirErro(String erro) {
            //quadroAviso.CarregarImagem("/_layouts/DadosCadastrais/IMAGES/CFP/Ico_Alerta.png");
            quadroAviso.CarregarMensagem("Atenção, dados inválidos.", erro);
            pnlErroPrincipal.Visible = true;
            ctrlProcessoConcluido.pnlPagina1.Visible = false;
        }

        /// <summary>
        /// Carregamento da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e) {
            //SetarMensagem("Os seus dados de acesso foram enviados para o endereço cadastrado em nosso sistema. Aguarde alguns <br />dias para o recebimento.");

            if (!Page.IsPostBack && this.ValidarConfirmacaoPositiva()) {
                this.EfetuarEnvioUsuario();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        String[] letras = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "x", "z" };

        /// <summary>
        /// 
        /// </summary>
        Int32[] numeros = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        /// <summary>
        /// Gera uma senha aleatória
        /// </summary>
        /// <returns></returns>
        public String GerarSenha() {
            String novaSenha = String.Empty;
            Random rLetras = new Random();
            Random rNumeros = new Random();

            for (int i = 0; i < 4; i++) {
                novaSenha += letras[rLetras.Next(letras.Length)];
            }
            for (int i = 0; i < 2; i++) {
                novaSenha += numeros[rNumeros.Next(numeros.Length)];
            }
            return novaSenha;
        }

        /// <summary>
        /// 
        /// </summary>
        private Boolean RenovarUsuario(String novaSenha) {
            InformacaoUsuario _dados = InformacaoUsuario.Recuperar();

            Int32 codigoTecnologia = 0;
            Int32 codigoRetorno = 0;
            Boolean pvKomerci = false;

            using (EntidadeServico.EntidadeServicoClient entidadeServico = new EntidadeServico.EntidadeServicoClient()) {
                codigoTecnologia = entidadeServico.ConsultarTecnologiaEstabelecimento(out codigoRetorno, _dados.NumeroPV);
                pvKomerci = (codigoTecnologia.Equals(26) || codigoTecnologia.Equals(25) || codigoTecnologia.Equals(23));
            }

            using (UsuarioServico.UsuarioServicoClient _client = new UsuarioServico.UsuarioServicoClient()) {
                int result = _client.Excluir(_dados.Usuario, _dados.NumeroPV, 1);
                if (result == 0) {
                    int result2 = _client.Inserir(_dados.Usuario,
                        _dados.NumeroPV,
                        1,
                        _dados.NomeCompleto,
                        "M", // Somente usuários master podem recuperar dados de acesso
                        EncriptadorSHA1.EncryptString(novaSenha),
                        "",
                        "IS",
                        pvKomerci);
                    if (result2 == 0)
                        return true;
                    else
                        return false;
                }
            }
            // chamar trata erro
            return false;
        }

        /// <summary>
        /// Faz o processamento de envio de dados para o usuário.
        /// </summary>
        private void EfetuarEnvioUsuario() {
            try {
                if (ctrlProcessoConcluido.Visible) {
                    Int32 codRetorno = 0;

                    String mensagemRetorno = String.Empty;
                    InformacaoUsuario info = InformacaoUsuario.Recuperar();

                    // definir nova senha
                    String novaSenha = this.GerarSenha();
                    this.RenovarUsuario(novaSenha);

                    using (EntidadeServico.EntidadeServicoClient entidadeClient = new EntidadeServico.EntidadeServicoClient()) {
                        EntidadeServico.Entidade entidade = entidadeClient.ConsultarDadosPV(out codRetorno, info.NumeroPV);
                        if (codRetorno == 0) {
                            codRetorno = this.GeraCarta(info, novaSenha, entidade);
                            if (codRetorno != 0) {
                                this.ExibirErro(mensagemRetorno);
                            }
                            else
                                SetarMensagem("Os seus dados de acesso foram enviados para o endereço de<br>correspondência cadastrado.");
                        }
                        else {
                            this.ExibirErro("Não foi possível consultar a entidade. Código de Retorno: " + codRetorno);
                        }
                    }
                }
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex) {
                ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
            }
            catch (FaultException<UsuarioServico.GeneralFault> ex) {
                ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
            }
            catch (Exception ex) {
                SharePointUlsLog.LogErro(ex);
                this.ExibirErro(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="novaSenha"></param>
        /// <returns></returns>
        private Int32 GeraCarta(InformacaoUsuario info, String novaSenha, EntidadeServico.Entidade entidade)
        {
            Int16 codigoCarta = (short)(String.IsNullOrEmpty(info.EmailEntidade) ? 6 : 5); // Código da Carta
            Int32 codRetorno = 0;

            WADadosCadastraisServico.HISServicoWA_DadosCadastraisClient
                waClient = new WADadosCadastraisServico.HISServicoWA_DadosCadastraisClient();

            codRetorno = waClient.GerarCarta(
                info.NumeroPV,
                info.Usuario,
                novaSenha,
                entidade.RazaoSocial,
                codigoCarta,
                "C",
                String.Empty);
            return codRetorno;
        }

        /// <summary>
        /// Exibe erro no painel
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        private void ExibirErro(String fonte, Int32 codigo) {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);
            //quadroAviso.CarregarImagem("/_layouts/DadosCadastrais/IMAGES/CFP/Ico_Alerta.png");
            quadroAviso.CarregarMensagem("Atenção, dados inválidos.", mensagem);
            pnlErroPrincipal.Visible = true;
            ctrlProcessoConcluido.Visible = false;
        }

        /// <summary>
        /// Seta a mensagem de aviso no controle de processo concluído
        /// </summary>
        private void SetarMensagem(String mensagem) {
            ctrlProcessoConcluido.Mensagem = mensagem;
            ctrlProcessoConcluido.ExibirTabelaPrazo = true;
        }
    }
}
