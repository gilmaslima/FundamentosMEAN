using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using Rede.PN.DadosCadastraisMobile.SharePoint.Util;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Rede.PN.DadosCadastraisMobile.SharePoint.UsuarioServico;
using System.Text;
using System.ServiceModel;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.Util
{
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
            Texto.InnerText = msg;
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
            Texto.InnerText = msg;
            Div.Attributes["class"] = "has-feedback ok";
        }

    }

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
        public Boolean Validar(Boolean exibirMensagem, Int32 codigoEntidade, out Usuario usuario, EventHandler linkHandler)
        {
            return Validar(exibirMensagem, new[] { codigoEntidade }, true, out usuario, linkHandler);
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
        public Boolean Validar(Boolean exibirMensagem, Int32[] codigoEntidades, EventHandler linkHandler)
        {
            Usuario usuario = null;
            return Validar(exibirMensagem, codigoEntidades, false, out usuario, linkHandler);
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
        private Boolean Validar(Boolean exibirMensagem, Int32[] codigoEntidades, Boolean origemAberta, out Usuario usuario, EventHandler linkHandler)
        {
            usuario = null;

            //Valida se domínio do e-mail é permitido
            if (!ValidarDominio(exibirMensagem))
                return false;

            foreach (Int32 codigoEntidade in codigoEntidades)
            {
                //Verifica se já existe usuário com o mesmo e-mail para o PV           
                Boolean statusConsulta = false;
                usuario = ConsultarUsuarioPorEmail(this.Email, codigoEntidade, out statusConsulta);

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
                                var msg = new StringBuilder("Usuário aguardando confirmação");
                                if (origemAberta)
                                {
                                    String reference = String.Concat("ReenviarSolicitacaoAprovacao", "|", codigoEntidade);
                                    LinkButton ReEmail = new LinkButton();
                                    ReEmail.Click += linkHandler;
                                    ReEmail.Text = "Clique Aqui";
                                    ReEmail.CommandArgument = reference;

                                    String texto = " para solicitar confirmação ao usuário Master";

                                    this.Controle.ExibirMensagemAviso(ReEmail, texto);
                                }
                                else
                                {
                                    this.Controle.ExibirMensagemAviso(msg.ToString());
                                }
                            }
                            break;

                        //Se usuário Ativo, informa que usuário já existe
                        //Se está na Parte Aberta, exibe link para recuperação de senha
                        case Status1.UsuarioAtivo:
                        case Status1.UsuarioAtivoLiberacaoAcessoCompletoBloqueada:
                            if (exibirMensagem)
                            {
                                var msg = new StringBuilder("Usuário já existente");
                                if (origemAberta)
                                    msg.Append(String.Format(
                                        "<span><font color='black'>Utilize a <a href=\"{0}\">recuperação de senha</a></font></span>",
                                        "/pt-br/novoacesso/Paginas/Mobile/RecuperacaoSenhaIdentificacao.aspx"));
                                this.Controle.ExibirMensagemErro(msg.ToString());
                            }
                            break;
                        case Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario:
                            if (exibirMensagem)
                            {
                                String msg = "Usuário aguardando confirmação";
                                this.Controle.ExibirMensagemAviso(msg);
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
        public static Usuario ConsultarUsuarioPorEmail(String email, Int32 codigoEntidade, out Boolean sucesso)
        {
            using (Logger log = Logger.IniciarLog("Consulta usuário através do e-mail"))
            {
                var codigoRetorno = default(Int32);
                var entidade = new Entidade
                {
                    Codigo = codigoEntidade,
                    GrupoEntidade = new GrupoEntidade { Codigo = 1 }
                };
                var usuarios = default(Usuario[]);

                try
                {
                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                        usuarios = ctx.Cliente.ConsultarPorCodigoEntidade(out codigoRetorno, email, entidade);
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }

                if (usuarios != null)
                {
                    sucesso = true;
                    return usuarios.FirstOrDefault();
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

        #region [ Métodos Privados ]

        #endregion

        #region [ Postbacks ]

        /// <summary>
        /// Tratamento do PostBack do controle
        //// </summary>
        //public override void RaisePostBackEvent(String argument)
        //{
        //    //Reenvio de solicitação
        //    if (String.Compare(argument, "ReenviarSolicitacaoAprovacao", true) >= 0)
        //    {
        //        //Recupera o Código da Entidade que foi validada
        //        Int32 codigoEntidade = argument.Split('|')[1].ToInt32();
        //        Boolean sucesso = false;

        //        //Obtém os dados do usuário do e-mail
        //        Usuario usuario = ConsultarUsuarioPorEmail(this.Email, codigoEntidade, out sucesso);
        //        //Consulta os usuários Master da entidade
        //        EntidadeServico.Usuario[] usuariosMaster = ConsultarUsuariosMaster(codigoEntidade);

        //        if (usuario != null && usuariosMaster != null && usuariosMaster.Length > 0)
        //        {
        //            String[] emails = usuariosMaster.Select(master => master.Email)
        //                .Where(email => !String.IsNullOrEmpty(email)).ToArray();

        //            //Envia e-mail de aprovação
        //            EmailNovoAcessoMobile.EnviarEmailAprovacaoAcesso(String.Join(",", emails), usuario.Descricao, usuario.Email,
        //                usuario.CodigoIdUsuario, usuario.TipoUsuario, codigoEntidade, null);

        //            //Se definido, invoca handler para tratamento pós reenvio do e-mail 
        //            if (SolicitacaoAprovacaoReenviada != null)
        //                SolicitacaoAprovacaoReenviada(this, new EventArgs());
        //        }
        //    }
        //}

        //public String GerarPostBackEventReference(String postBackArgument)
        //{
        //    return this.Page.ClientScript.GetPostBackEventReference(this, postBackArgument);
        //}

        #endregion


    }
}