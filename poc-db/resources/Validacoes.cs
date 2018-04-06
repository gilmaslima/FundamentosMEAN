/*
© Copyright 2015 Rede S.A.
Autor : Juliano Marcon
Empresa : Rede
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ServiceModel;
using Redecard.PN.Comum;
using Rede.PN.DadosCadastraisMobile.SharePoint.UsuarioServico;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Web.UI;
using Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile.DadosIniciaisMob;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.Util
{
    /// <summary>
    /// Classe que gera um campo novo acesso genérico.
    /// </summary>
    public class ControleCampos
    {
        public String Text
        {
            get { return Campo.Text; }
            set { Campo.Text = value; }
        }

        public int MaxLength { get { return Campo.MaxLength; } set { Campo.MaxLength = value; } }

        public TextBox Campo { get; set; }

        public HtmlGenericControl Div { get; set; }

        public HtmlGenericControl Texto { get; set; }

        public ControleCampos(TextBox campo, HtmlGenericControl div, HtmlGenericControl texto)
        {
            this.Campo = campo;
            this.Div = div;
            this.Texto = texto;

        }

        public ControleCampos(TextBox campo)
        {
            this.Campo = campo;
        }

        public void ExibirMensagemAviso(String msg)
        {
            Texto.InnerHtml = msg;
            Div.Attributes["class"] = "has-feedback atencao";

        }

        public void ExibirMensagemAviso(LinkButton botao, String texto)
        {
            Texto.InnerHtml = "Usuário aguardando confirmação ";
            Texto.Controls.Add(botao);
            Literal mensagem = new Literal();
            mensagem.Text = "<span><font color='black'>" + texto + "</font></span>";
            Texto.Controls.Add(mensagem);
            Div.Attributes["class"] = "has-feedback atencao";
        }

        public void ExibirMensagemErro(String msg)
        {
            Texto.InnerHtml = msg;
            Div.Attributes["class"] = "has-feedback erro";
        }

        public void ExibirMensagemSucesso(String msg)
        {
            Texto.InnerHtml = msg;
            Div.Attributes["class"] = "has-feedback ok";
        }

    }

    /// <summary>
    /// Classe de validações Genéricas
    /// </summary>
    public class Validacoes
    {

        /// <summary>
        /// Valida se uma string de User Agent é Mobile ou Não
        /// </summary>
        /// <param name="userAgent">String de User Agent: Request.ServerVariables["HTTP_USER_AGENT"]</param>
        /// <returns>Verdadeiro se o user agent representar um aplicativo mobile (SmartPhone ou Tablet)</returns>
        public static bool IsMobile(string userAgent)
        {
            Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
            Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

            if (b.IsMatch(userAgent) || v.IsMatch(userAgent.Substring(0, 4)))
                return true;

            return false;
        }


    }

    /// <summary>
    /// Classe de Validações de Senha - Classe Estática
    /// </summary>
    public static class ValidarSenha
    {
        /// <summary>
        /// Valida conteúdo do controle, se Texto é válido
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se conteúdo é válido</returns>
        public static Boolean ConfirmarSenha(Boolean exibirMensagem, ControleCampos Senha, ControleCampos ConfirmarSenha)
        {
            
            Boolean preenchido = !String.IsNullOrEmpty(ConfirmarSenha.Text);
            Boolean preenchido2 = !String.IsNullOrEmpty(Senha.Text);
            Boolean valido = (ConfirmarSenha.Text != String.Empty && Senha.Text != String.Empty) && String.Compare(ConfirmarSenha.Text, Senha.Text) == 0;

            //Se campo Confirmar Senha obrigatório
            if (!preenchido)
            {
                if (exibirMensagem)
                    ConfirmarSenha.ExibirMensagemErro("*Campo Obrigatório");
                return false;
            }

            //Se campo Senha obrigatório
            if (!preenchido2)
            {
                if (exibirMensagem)
                    ConfirmarSenha.ExibirMensagemErro("*Campo Obrigatório");
                return false;
            }

            //Se preenchido e número inválido, erro
            if ((preenchido || preenchido2) && !valido)
            {
                if (exibirMensagem)
                    ConfirmarSenha.ExibirMensagemErro("Senhas não confirmadas");

                if (exibirMensagem)
                    Senha.ExibirMensagemErro("Senhas não confirmadas");

                return false;
            }

            return true;
        }


        /// <summary>
        /// Validação da senha, de acordo com os critérios de segurança
        /// </summary>
        /// <param name="senha">Senha</param>
        /// <returns>Se a senha é válida ou não</returns>
        public static Boolean Validar(Boolean exibirMensagem, ControleCampos Senha)
        {
            if (Senha.Text.Count() < 8 || Senha.Text.Count() > 15) //Entre 8 e 15 caracteres;
            {
                if (exibirMensagem)
                {
                    Senha.ExibirMensagemErro("Senha inválida");
                }
                return false;
            }

            if (Senha.Text.Count(c => Char.IsLower(c)) == 0) //Nenhuma letra minuscula;
            {
                if (exibirMensagem)
                {
                    Senha.ExibirMensagemErro("Senha inválida");
                }
                return false;
            }

            if (Senha.Text.Count(c => Char.IsUpper(c)) == 0) //Nenhuma letra maiúscula;
            {
                if (exibirMensagem)
                {
                    Senha.ExibirMensagemErro("Senha inválida");
                }
                return false;
            }

            if (Senha.Text.Count(c => Char.IsNumber(c)) == 0) //Nenhum número;
            {
                if (exibirMensagem)
                {
                    Senha.ExibirMensagemErro("Senha inválida");
                }
                return false;
            }

            if (Senha.Text.Count(c => Char.IsWhiteSpace(c)) > 0) //Tem espaços;
            {
                if (exibirMensagem)
                {
                    Senha.ExibirMensagemErro("Senha inválida");
                }
                return false;
            }

            if (Senha.Text.Count(c => Char.IsLetterOrDigit(c)) != Senha.Text.Length) //Contém caracteres especiais (Diferente de letras e números)
            {
                if (exibirMensagem)
                {
                    Senha.ExibirMensagemErro("Senha inválida");
                }
                return false;
            }

            return true;
        }


    }

    /// <summary>
    /// Classe de Validações de Email - Classe Instânciavel
    /// </summary>
    public class ValidarCamposEmail
    {
        #region [ Constantes / Var. Estáticas ]
        /// <summary>
        /// Regular expression para E-mails
        /// </summary>
        public static Regex RegexEmail
        {
            get { return new Regex(@"^(?<Conta>[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&''*+/=?^_`{|}~-]+)*)@((?<Dominio>(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?))\.)+(?<Dominio>[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9]))?$", RegexOptions.IgnoreCase); }
        }


        ControleCampos Controle;

        #endregion

        #region [ Construtores ]
        public ValidarCamposEmail(TextBox campo, HtmlGenericControl div, HtmlGenericControl texto)
        {
            Controle = new ControleCampos(campo, div, texto);
        }

        public ValidarCamposEmail(String email)
        {
            TextBox txtEmail = new TextBox();
            txtEmail.Text = email;
            Controle = new ControleCampos(txtEmail);

        }
        #endregion

        #region [ Propriedades ]

        /// <summary>
        /// E-mail (completo)
        /// </summary>
        public String Email
        {
            get { return this.Controle.Text; }
            set { this.Controle.Text = value.ToString(); }
        }

        /// <summary>
        /// SubDomínios do e-mail (o que sucede o '@').
        /// Exemplo: email@dominio.com.br retorna new String[] { "dominio", "com", "br" }
        /// </summary>
        public String[] SubDominios
        {
            get
            {
                Match match = RegexEmail.Match(this.Email);
                if (match.Success)
                    return match.Groups["Dominio"].Captures.Cast<Capture>().Select(capture => capture.Value).ToArray();
                else
                    return null;
            }
        }

        /// <summary>
        /// Retorna o que sucede o '@'.
        /// Exemplo: email@dominio.com.br retorna "dominio.com.br".
        /// </summary>
        public String Dominios { get { return String.Join(".", this.SubDominios); } }

        /// <summary>
        /// Conta do e-mail (precede o @).
        /// Exemplo: email@dominio.com.br retorna "email"
        /// </summary>
        public String Conta
        {
            get
            {
                Match match = RegexEmail.Match(this.Email);
                if (match.Success)
                    return match.Groups["Conta"].Value;
                else
                    return null;
            }
        }

        /// <summary>
        /// Domínios bloqueados/não permitidos para cadastro de usuários
        /// </summary>
        private static String[] DominiosBloqueados
        {
            get
            {
                try
                {
                    //Query para recuperar apenas os registros ativos
                    var query = new SPQuery();
                    query.Query = String.Concat(
                        "<Where>",
                            "<Eq>",
                                "<FieldRef Name=\"Ativo\" />",
                                "<Value Type=\"Boolean\">1</Value>",
                            "</Eq>",
                        "</Where>");

                    //Preparação do objeto de retorno contendo a lista de domínios bloqueados
                    if (ListaDominiosBloqueados != null)
                        return ListaDominiosBloqueados.GetItems(query)
                            .Cast<SPListItem>()
                            .Select(spItem => (String)spItem["Dominio"]).ToArray();
                    else
                        return new String[0];
                }
                catch (SPException ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    return new String[0];
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    return new String[0];
                }
            }
        }

        #endregion

        #region [ Propriedades - Delegates Postbacks ]

        /// <summary>
        /// Evento chamado após o Reenvio de Solicitação de Aprovação para o Master com sucesso
        /// </summary>
        //public event EventHandler SolicitacaoAprovacaoReenviada;

        #endregion

        #region [ Listas ]

        /// <summary>
        /// Lista de domínios bloqueados
        /// </summary>
        private static SPList ListaDominiosBloqueados
        {
            get
            {
                SPList lista = null;

                SPUtility.ValidateFormDigest();

                //Recupera a lista de "Domínios Bloqueados" em sites/fechado/minhaconta
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite spSite = SPContext.Current.Site.WebApplication.Sites["sites/fechado"])
                    using (SPWeb spWeb = spSite.AllWebs["minhaconta"])
                    {
                        //spWeb.AllowUnsafeUpdates = true;
                        lista = spWeb.Lists.TryGetList("Domínios Bloqueados");
                    }
                });

                return lista;
            }
        }

        #endregion

        #region [ Métodos - Overrides ]

        /// <summary>
        /// Inicialização do controle.
        /// Atribuição de configurações específicas (máscara, textMode, etc)
        /// </summary>
        public void InicializarControle()
        {
            this.Controle.MaxLength = 50;
        }

        #endregion

        #region [ Métodos - Customizados ]

        /// <summary>
        /// Validação de E-mail pela Parte Aberta
        /// <para>Valida se E-mail é válido e atende negócio (se está disponível para utilização no PV).</para>
        /// <para>1. Verifica se domínio do e-mail é permitido</para>
        /// <para>2. Verifica se usuário/e-mail já existe para o PV</para>
        /// <para>3. Verifica se usuário/e-mail temporário já existe para o PV</para>
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="usuario">Usuário, caso já exista para o PV</param>
        /// <returns>Se conteúdo é válido</returns>
        public Boolean Validar(Boolean exibirMensagem, Int32 codigoEntidade, out Usuario usuario, DadosIniciaisMobUserControl dadosIniciais)
        {
            return Validar(exibirMensagem, new[] { codigoEntidade }, true, out usuario, dadosIniciais);
        }

        /// <summary>
        /// Validação de E-mail pela Parte Fechada (Administração de Usuários)
        /// <para>Valida se E-mail é válido e atende negócio (se está disponível para utilização para determinados PVs).</para>
        /// <para>1. Verifica se domínio do e-mail é permitido</para>
        /// <para>2. Verifica se usuário já existe para o PV</para>
        /// <para>3. Verifica se usuário/e-mail temporário já existe para o PV</para>
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <param name="codigoEntidades">Código das Entidades</param>
        /// <returns>Se conteúdo é válido</returns>
        public Boolean Validar(Boolean exibirMensagem, Int32[] codigoEntidades, DadosIniciaisMobUserControl dadosIniciais)
        {
            Usuario usuario = null;
            return Validar(exibirMensagem, codigoEntidades, false, out usuario, dadosIniciais);
        }

        /// <summary>
        /// Validação de E-mail pela Parte Fechada (Administração de Usuários)
        /// <para>Valida se E-mail é válido e atende negócio (se está disponível para utilização para determinados PVs).</para>
        /// <para>1. Verifica se domínio do e-mail é permitido</para>
        /// <para>2. Verifica se usuário já existe para o PV</para>
        /// <para>3. Verifica se usuário/e-mail temporário já existe para o PV</para>
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <param name="codigoEntidades">Código das Entidades</param>
        /// <returns>Se conteúdo é válido</returns>
        public Boolean Validar(Boolean exibirMensagem, Int32[] codigoEntidades,Boolean origemAberta, DadosIniciaisMobUserControl dadosIniciais)
        {
            Usuario usuario = null;
            return Validar(exibirMensagem, codigoEntidades, origemAberta, out usuario, dadosIniciais);
        }


        /// <summary>
        /// <para>Valida se E-mail é válido e atende negócio (se está disponível para utilização para determinados PVs).</para>
        /// <para>1. Verifica se domínio do e-mail é permitido</para>
        /// <para>2. Verifica se usuário já existe para o PV</para>
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <param name="codigoEntidades">Código das Entidades</param>
        /// <param name="origemAberta">TRUE: controle está na Parte Aberta</param>
        /// <param name="usuario">Usuário, caso já exista para o PV</param>
        /// <returns>Se conteúdo é válido</returns>
        private Boolean Validar(Boolean exibirMensagem, Int32[] codigoEntidades, Boolean origemAberta, out Usuario usuario, DadosIniciaisMobUserControl dadosIniciais)
        {
            usuario = null;
            Guid hash = Guid.Empty;

            //Valida se domínio do e-mail é permitido
            if (!ValidarDominio(exibirMensagem))
                return false;

            foreach (Int32 codigoEntidade in codigoEntidades)
            {
                //Verifica se já existe usuário com o mesmo e-mail para o PV           
                Boolean statusConsulta = false;
                usuario = ConsultarUsuarioPorEmail(this.Email, codigoEntidade, out statusConsulta, out hash);

                if (usuario == null)
                {
                    //Não existe usuário com o mesmo e-mail, então tenta verificar se existe usuário com e-mail temporário igual para o PV
                    usuario = ConsultarUsuarioPorEmailTemporario(this.Email, codigoEntidade, out statusConsulta);
                }

                if (usuario != null)
                {
                    switch ((Status1)usuario.Status.Codigo.Value)
                    {
                        //Se e-mail é de usuário Aguardando Aprovação do Master, informa que está aguardando aprovação
                        //Se está na Parte Aberta, exibe link para reenvio da solicitação de aprovação para o Master
                        case Status1.UsuarioAguardandoConfirmacaoMaster:
                            if (exibirMensagem)
                            {
                                var msg = new StringBuilder("Usuário aguardando confirmação ");
                                if (origemAberta)
                                {
                                    String reference = String.Concat("ReenviarSolicitacaoAprovacao", "|", codigoEntidade);
                                    msg.Append(String.Format(
                                        "<br/><span><a href=\"#\" onclick=\"{0}\">Clique aqui</a> para solicitar confirmação ao usuário Master</span>",
                                        dadosIniciais.GerarPostBackEventReference(reference)));

                                }
                                    this.Controle.ExibirMensagemAviso(msg.ToString());

                            }
                            break;
                        case Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario:
                            if (exibirMensagem)
                            {
                                var msg = new StringBuilder("Usuário aguardando confirmação");
                                if (origemAberta)
                                {
                                    String reference = String.Concat("ReenviarEmailConfirmacao", "|", codigoEntidade);
                                    msg.Append(String.Format(
                                        "<br/><span><a href=\"#\" onclick=\"{0}\">Clique aqui</a> para reenviar o e-mail de confirmação</span>",
                                        dadosIniciais.GerarPostBackEventReference(reference)));
                                }
                                this.Controle.ExibirMensagemAviso(msg.ToString());
                            }
                            break;
                        //Se usuário Ativo, informa que usuário já existe
                        //Se está na Parte Aberta, exibe link para recuperação de senha
                        case Status1.UsuarioAtivo:
                        case Status1.UsuarioAtivoLiberacaoAcessoCompletoBloqueada:
                            if (exibirMensagem)
                            {
                                var msg = new StringBuilder("Usuário já existente<br/>");
                                if (origemAberta)
                                    msg.Append(String.Format(
                                        "<span><font color='black'>Utilize a <a href=\"{0}\">recuperação de senha</a></font></span>",
                                        "/pt-br/novoacesso/Paginas/Mobile/RecuperacaoSenhaIdentificacao.aspx"));
                                this.Controle.ExibirMensagemErro(msg.ToString());
                            }
                            break;
                        //Para qualquer outro status, informa que usuário já existe
                        default:
                            if (exibirMensagem)
                            {
                                String msg = "Usuário já existente";
                                this.Controle.ExibirMensagemErro(msg);
                            }
                            break;
                    }
                    return false;
                }
            }

            if (exibirMensagem)
            {
                //Caso passe por todas as validações, o e-mail está disponível
                this.Controle.ExibirMensagemSucesso("Usuário disponível");
            }
            return true;
        }

        /// <summary>
        /// Valida se o e-mail informado é válido e se o domínio do e-mail é permitido.
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se e-mail é válido e permitido</returns>
        public Boolean ValidarDominio(Boolean exibirMensagem)
        {

            //Verifica se o domínio do e-mail é permitido
            Boolean dominioValido = VerificarDominioEmailPermitido(this.Email);

            if (!dominioValido)
            {
                if (exibirMensagem)
                {
                    String msgDominioInvalido = String.Format(
                        "Domínio @{0} inválido<br/>Por favor, insira outro e-mail", this.Dominios);
                    this.Controle.ExibirMensagemErro(msgDominioInvalido);
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validação da sintaxe do e-mail
        /// </summary>
        /// <param name="email">E-mail</param>
        /// <returns>Se válido</returns>
        public static Boolean ValidarFormatoEmail(String email)
        {
            return RegexEmail.Match(email).Success;
        }

        /// <summary>
        /// Verifica se o domínio do e-mail é válido
        /// </summary>
        /// <param name="email">E-mail</param>
        /// <returns></returns>
        public static Boolean VerificarDominioEmailPermitido(String email)
        {
            try
            {
                String[] dominiosBloqueados = DominiosBloqueados;

                //Verifica se o domínio do e-mail está na banlist dos domínios não permitidos
                foreach (String dominio in dominiosBloqueados)
                {
                    Boolean dominioBloqueado = VerificarDominioEmail(dominio, email);
                    if (dominioBloqueado)
                        return false;
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro ao validar domínio do e-mail", ex);
            }

            return true;
        }

        /// <summary>
        /// Verifica se determinado domínio do e-mail é aceito por determinada expressão de busca.<br/>
        /// O parâmetro "pattern" aceita os wildcards "*" e "?".<br/>
        /// Apenas o que sucede o '@' no e-mail é considerado.<br/>               
        /// </summary>
        /// <param name="pattern">Pattern</param>
        /// <param name="email">E-mail a ser verificado</param>
        /// <returns>TRUE: se e-mail atende o padrão informado</returns>
        private static Boolean VerificarDominioEmail(String pattern, String email)
        {
            Match match = RegexEmail.Match(email);
            if (match.Success)
            {
                //Extrai o domínio e subdomínios do e-mail
                String[] subdominios = match.Groups["Dominio"].Captures.Cast<Capture>()
                    .Select(capture => capture.Value).ToArray();
                String dominio = String.Join(".", subdominios);

                //Converte para Regex
                pattern = String.Concat("^", Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", "."), "$");
                return new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(dominio);
            }
            else
                return false;
        }

        
        #endregion

        #region [ Consultas Serviços ]

        /// <summary>
        /// Consulta usuário através do e-mail, para determinado estabelecimento
        /// </summary>
        /// <param name="email">E-mail</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="sucesso">Se a verificação foi realizada com sucesso</param>
        /// <returns>Usuário encontrado (deve ser apenas 1)</returns>        
        public static UsuarioServico.Usuario ConsultarUsuarioPorEmail(String email, Int32 codigoEntidade, out Boolean sucesso, out Guid hash)
        {
            hash = Guid.Empty;

            using (Logger log = Logger.IniciarLog("Consulta usuário através do e-mail"))
            {
                var codigoRetorno = default(Int32);
                var entidade = new Entidade
                {
                    Codigo = codigoEntidade,
                    GrupoEntidade = new GrupoEntidade { Codigo = 1 }
                };

                Usuario usuario = null;
                
                try
                {
                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                    {
                        var usuarios = ctx.Cliente.ConsultarPorEmailPrincipalPorStatus(out codigoRetorno, email, 0, codigoEntidade, null);

                        if (usuarios != null && usuarios.Any())
                        {
                            usuario = usuarios.FirstOrDefault();

                            var hashUsuarios = ctx.Cliente.ConsultarHash(out codigoRetorno, usuario.CodigoIdUsuario, null, null);

                            if (hashUsuarios != null || hashUsuarios.Any())
                            {
                                hash = hashUsuarios.FirstOrDefault().Hash;
                            }
                        }
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }

                if (usuario != null)
                {
                    sucesso = true;
                    return usuario;
                }
                else
                {
                    sucesso = false;
                    return null;
                }
            }
        }

        /// <summary>
        /// Consulta usuário através do e-mail temporário, para determinado estabelecimento
        /// </summary>
        /// <param name="emailTemporario">E-mail</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="sucesso">Se a verificação foi realizada com sucesso</param>
        /// <returns>Usuário encontrado (deve ser apenas 1)</returns>        
        private static Usuario ConsultarUsuarioPorEmailTemporario(String emailTemporario, Int32 codigoEntidade, out Boolean sucesso)
        {
            using (Logger log = Logger.IniciarLog("Consulta usuário através do e-mail temporário"))
            {
                var codigoRetorno = default(Int32);
                var usuario = default(Usuario);

                try
                {
                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                        usuario = ctx.Cliente.ConsultarPorEmailTemporario(out codigoRetorno, emailTemporario, 1, codigoEntidade);
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }

                if (usuario != null)
                {
                    sucesso = true;
                    return usuario;
                }
                else
                {
                    sucesso = false;
                    return null;
                }
            }
        }

        /// <summary>
        /// Consulta usuários Master do estabelecimento
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        public static EntidadeServico.Usuario[] ConsultarUsuariosMaster(Int32 codigoEntidade)
        {
            var codigoRetorno = default(Int32);
            var usuarios = default(EntidadeServico.Usuario[]);

            using (Logger log = Logger.IniciarLog("Consulta usuários de Perfil Master do Estabelecimento"))
            {
                try
                {
                    using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        usuarios = ctx.Cliente.ConsultarUsuariosPorPerfil(out codigoRetorno, codigoEntidade, 1, 'M');
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }
            }

            return usuarios;
        }

        #endregion


    }

    /// <summary>
    /// Classe de Validações de Confirmar Senha - Classe Estática
    /// </summary>
    public static class ValidarConfirmacaoEmail
    {
        /// <summary>
        /// Valida conteúdo do controle, se E-mail Secundário é válido
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se conteúdo é válido</returns>
        public static Boolean Validar(Boolean exibirMensagem, ControleCampos ConfEmail, Boolean obrigatorio)
        {
            Boolean preenchido = !String.IsNullOrEmpty(ConfEmail.Text);
            Boolean valido = ValidarFormatoEmail(ConfEmail.Text);

            //Se campo obrigatório
            if (!preenchido && obrigatorio)
            {
                if (exibirMensagem)
                    ConfEmail.ExibirMensagemErro("*Campo Obrigatório");
                return false;
            }

            //Se preenchido e e-mail inválido, erro
            if (preenchido && !valido)
            {
                if (exibirMensagem)
                    ConfEmail.ExibirMensagemErro("Email inválido");
                return false;
            }

            //Validações se e-mail foi preenchido
            if (preenchido && valido)
            {
                String msgDominioInvalido = String.Concat(
                    "Domínio @{0} inválido<br/>",
                    "Por favor, insira outro e-mail");
                ConfEmail.ExibirMensagemErro(msgDominioInvalido);

                ConfEmail.ExibirMensagemSucesso("Confirmação de email inválido!");
                return true;
            }

            return true;
        }

        /// <summary>
        /// Validação da sintaxe do e-mail
        /// </summary>
        /// <param name="email">E-mail</param>
        /// <returns>Se válido</returns>
        public static Boolean ValidarFormatoEmail(String email)
        {
            return RegexEmail.Match(email).Success;
        }

        public static Regex RegexEmail
        {
            get { return new Regex(@"^(?<Conta>[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&''*+/=?^_`{|}~-]+)*)@((?<Dominio>(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?))\.)+(?<Dominio>[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9]))?$", RegexOptions.IgnoreCase); }
        }
    }

    /// <summary>
    /// Classe de Validações de Celular - Classe Estática
    /// </summary>
    public static class ValidarCelular
    {
        private static Regex RegexCelular
        {
            get { return new Regex(@"^\((?<DDD>[0-9]{2})\)\s(?<Parte1>[0-9]{4})\-(?<Parte2>[0-9]{4,5})$"); }
        }



        /// <summary>
        /// DDD do celular
        /// </summary>
        public static Int32? GetDDD(String numero)
        {

            Match match = RegexCelular.Match(numero);
            if (match.Success)
                return match.Groups["DDD"].Value.ToInt32Null();
            else
                return null;

        }

        /// <summary>
        /// Número do celular (sem DDD)
        /// </summary>
        public static Int32? GetNumero(String numero)
        {

            Match match = RegexCelular.Match(numero);
            if (match.Success)
                return String.Concat(match.Groups["Parte1"], match.Groups["Parte2"]).ToInt32Null();
            else
                return null;

        }



        /// <summary>
        /// Aplica máscara de celular (99) 99999-9999
        /// </summary>
        public static String AplicarMascara(Int32? ddd, Int32? numero)
        {
            if (ddd.HasValue && numero.HasValue)
            {
                String numeroCelular = numero.Value.ToString();

                //Normaliza para quantidade mínima de 8 números, com zeros à esquerda
                if (numeroCelular.Length < 8)
                    numeroCelular = numeroCelular.PadLeft(8, '0');

                //Obtém a primeira parte do número (pré-hífen)
                String parte1 = numeroCelular.Substring(0, numeroCelular.Length > 8 ? 5 : 4);

                //Obtém a segunda parte do número (pós-hífen)
                String parte2 = numeroCelular.Substring(numeroCelular.Length - 4, 4);

                return String.Format("({0}) {1}-{2}", ddd.Value, parte1, parte2);
            }
            else
                return String.Empty;
        }
    }

    /// <summary>
    /// Classe de Validações de Telefone - Classe Estática
    /// </summary>
    public static class ValidarTelefone
    {
        /// <summary>
        /// Valida conteúdo do controle, se telefone é válido
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se conteúdo é válido</returns>
        public static Boolean Validar(Boolean exibirMensagem, ControleCampos Telefone, Boolean obrigatorio)
        {
            Boolean preenchido = !String.IsNullOrEmpty(Telefone.Text);
            Boolean valido = Validar(Telefone.Text);

            //Se campo obrigatório
            if (!preenchido && obrigatorio)
            {
                if (exibirMensagem)
                    Telefone.ExibirMensagemErro("*Campo Obrigatório");
                return false;
            }

            //Se preenchido e número inválido, erro
            if (preenchido && !valido)
            {
                if (exibirMensagem)
                    Telefone.ExibirMensagemErro("Telefone inválido");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida número do telefone com máscara
        /// </summary>
        /// <param name="celular">Telefone</param>
        /// <returns>Se válido</returns>
        public static Boolean Validar(String telefone)
        {
            return RegexTelefone.Match(telefone).Success;
        }

        /// <summary>
        /// Regular Expression para validação de celulares nos formatos
        /// 8 dígitos "(99) 9999-9999" ou 9 dígitos "(99) 99999-9999".
        /// Considera máscaras
        /// </summary>
        private static Regex RegexTelefone
        {
            get { return new Regex(@"^\((?<DDD>[0-9]{2})\)\s(?<Parte1>[0-9]{4})\-(?<Parte2>[0-9]{4})$"); }
        }
    }

    /// <summary>
    /// Classe de Validações de CPF - Classe Estática
    /// </summary>
    public static class ValidarCpf
    {
        /// <summary>
        /// Valida conteúdo do controle, se CPF é válido
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se conteúdo é válido</returns>
        public static Boolean Validar(Boolean exibirMensagem, ControleCampos CPF, Boolean obrigatorio)
        {

            Boolean preenchido = !String.IsNullOrEmpty(CPF.Text);
            Boolean valido = Cpf(CPF.Text);

            //Se campo obrigatório
            if (!preenchido && obrigatorio)
            {
                if (exibirMensagem)
                    CPF.ExibirMensagemErro("*Campo Obrigatório");
                return false;
            }

            //Se preenchido e número inválido, erro
            if (preenchido && !valido)
            {
                if (exibirMensagem)
                    CPF.ExibirMensagemErro("CPF inválido");
                return false;
            }

            if (exibirMensagem)
                CPF.ExibirMensagemSucesso("CPF válido");
            return true;
        }

        
        /// <summary>
        /// Validação do CPF com máscara 000.000.000-00
        /// </summary>
        /// <param name="cpf">CPF</param>
        /// <returns>Se válido</returns>
        public static Boolean Cpf(String cpf)
        {
            Int32[] multiplicador1 = new Int32[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            Int32[] multiplicador2 = new Int32[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            String tempCpf = default(String);
            String digito = default(String);
            Int32 soma = default(Int32);
            Int32 resto = default(Int32);

            //CPF com máscara de ter exatamente 14 caracteres
            if (cpf.Length != 14)
                return false;

            //Verifica separadores
            if (cpf[3] != '.' || cpf[7] != '.' || cpf[11] != '-')
                return false;

            //Remove caracteres especiais
            cpf = cpf.Trim().Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;

            //Verifica se sobraram apenas números
            for (Int32 i = 0; i < cpf.Length; i++)
                if (!Char.IsNumber(cpf[i]))
                    return false;

            //Valida dígito verificador
            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (Int32 i = 0; i < 9; i++)
                soma += tempCpf[i].ToString().ToInt32() * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (Int32 i = 0; i < 10; i++)
                soma += tempCpf[i].ToString().ToInt32() * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }


        /// <summary>
        /// Validação de CPF de Usuário
        /// <para>1. Verifica se CPF está preenchido</para>
        /// <para>2. Verifica se CPF é algoritmicamente válido</para>
        /// <para>3. Verifica se CPF já existe para o PV</para>
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <param name="codigoEntidades">Código dos PVs</param>
        /// <param name="codigoIdUsuario">ID do Usuário a validar</param>
        /// <returns>Se CPF é válido</returns>
        public static Boolean Validar(Boolean exibirMensagem, Int32[] codigoEntidades, Int32? codigoIdUsuario, ControleCampos CPF, Boolean obrigatorio)
        {
            using (Logger log = Logger.IniciarLog("Validação de CPF de Usuário"))
            {
                Boolean logicamenteValido = Validar(exibirMensagem, CPF, true);
                if (!logicamenteValido)
                    return false;

                Boolean cpfDisponivel = true;
                var usuarios = default(UsuarioServico.Usuario[]);

                foreach (Int32 codigoEntidade in codigoEntidades)
                {
                    try
                    {
                        using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                            usuarios = ctx.Cliente.ConsultarPorCpf(Convert.ToInt64(CPF.Text.Replace(".","").Replace("-","")), codigoEntidade, 1);
                    }
                    catch (FaultException<UsuarioServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                    }

                    if (usuarios != null)
                    {
                        var usuario = usuarios.FirstOrDefault();

                        if (usuario != null)
                            if (usuario.CodigoIdUsuario != codigoIdUsuario)
                                cpfDisponivel = false;
                    }
                }

                if (!cpfDisponivel)
                {
                    if (exibirMensagem)
                        CPF.ExibirMensagemErro("Este CPF já possui usuário cadastrado");
                    return false;
                }

                if (cpfDisponivel)
                    CPF.ExibirMensagemSucesso("CPF válido");
            }

            return true;
        }

    }

    /// <summary>
    /// Classe de Validações de CNPJ - Classe Estática
    /// </summary>
    public static class ValidarCnpj
    {
        /// <summary>
        /// Valida conteúdo do controle, se CNPJ é válido
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se conteúdo é válido</returns>
        public static Boolean Validar(Boolean exibirMensagem, ControleCampos CNPJ, Boolean obrigatorio)
        {
            Boolean preenchido = !String.IsNullOrEmpty(CNPJ.Text);
            Boolean valido = Cnpj(CNPJ.Text);

            //Se campo obrigatório
            if (!preenchido && obrigatorio)
            {
                if (exibirMensagem)
                    CNPJ.ExibirMensagemErro("*Campo Obrigatório");
                return false;
            }

            //Se preenchido e número inválido, erro
            if (preenchido && !valido)
            {
                if (exibirMensagem)
                    CNPJ.ExibirMensagemErro("CNPJ inválido");
                return false;
            }

            if (exibirMensagem)
                CNPJ.ExibirMensagemSucesso("CNPJ válido");
            return true;
        }

        /// <summary>
        /// Validação do CNPJ com máscara 00.000.000/0000-00
        /// </summary>
        /// <param name="cnpj">CNPJ</param>
        /// <returns>Se válido</returns>
        public static Boolean Cnpj(String cnpj)
        {
            Int32[] multiplicador1 = new Int32[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            Int32[] multiplicador2 = new Int32[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            Int32 soma = default(Int32);
            Int32 resto = default(Int32);
            String digito = default(String);
            String tempCnpj = default(String);

            //CNPJ com máscara deve ter exatamente 18 caracteres
            if (cnpj.Length != 18)
                return false;

            //Verifica separadores
            if (cnpj[2] != '.' || cnpj[6] != '.' || cnpj[10] != '/' || cnpj[15] != '-')
                return false;

            //Remove caracteres especiais
            cnpj = cnpj.Trim().Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;

            //Verifica se sobraram apenas números
            for (Int32 i = 0; i < cnpj.Length; i++)
                if (!Char.IsNumber(cnpj[i]))
                    return false;

            //Valida dígito verificador
            tempCnpj = cnpj.Substring(0, 12);
            for (Int32 i = 0; i < 12; i++)
                soma += tempCnpj[i].ToString().ToInt32() * multiplicador1[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;

            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();

            return cnpj.EndsWith(digito);
        }
    }

}
