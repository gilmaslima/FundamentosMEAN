/*
© Copyright 2014 Rede S.A.
Autor : Juliano Marcon
Empresa : Rede
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Redecard.PN.Comum;
using System.IO;
using System.Linq;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.Util
{
    /// <summary>
    /// Classe auxiliar para centralização de Envio de E-mail para as telas do projeto Novo Acesso
    /// </summary>
    public class EmailNovoAcessoMobile
    {
        #region [ Constantes ]

        /// <summary>Substitui a tag ##USEREDE## dos templates de E-mail</summary>
        private const String TAG_USEREDE = "userede.com.br";
        /// <summary>Substitui a tag ##COMO_ACESSAR_URL## dos templates de E-mail</summary>
        private const String TAG_COMO_ACESSAR_URL = "{0}/comoacessar";
        /// <summary>Substitui a tag ##COMO_ACESSAR## dos templates de E-mail</summary>
        private const String TAG_COMO_ACESSAR = "userede.com.br/comoacessar";
        /// <summary>Substitui a tag ##CENTRAL_ATENDIMENTO_URL## dos templates de E-mail</summary>
        private const String TAG_CENTRAL_ATENDIMENTO_URL = "{0}/pt-BR/atendimento/Paginas/telefonesuteis.aspx";

        #endregion

        #region [ Propriedades ]

        /// <summary>
        /// Endereço de e-mail do Remetente dos e-mails do Novo Acesso
        /// </summary>
        public static String Remetente 
        {
            get
            {
                return String.IsNullOrEmpty(SPAdministrationWebApplication.Local.OutboundMailSenderAddress) ?
                    "faleconosco@userede.com.br" : SPAdministrationWebApplication.Local.OutboundMailSenderAddress;
            }
        }

        #endregion

        /// <summary>
        /// Envio de e-mail para o usuário Master para Aprovação de uma Solicitação
        /// de Acesso de um novo usuário.
        /// </summary>
        /// <param name="emailMaster">E-mail do destinatário</param>
        /// <param name="nomeUsuario">Nome do Usuário que solicitou o acesso</param>
        /// <param name="emailUsuario">E-mail do Usuário que solicitou o acesso</param>
        /// <param name="codigoIdUsuario">Código ID do usuário que solicitou o acesso</param>
        /// <param name="numeroPv">Número do estabelecimento para o qual o usuário solicitou acesso</param>
        /// <param name="perfilUsuario">Perfil do usuário que solicitou o acesso</param>
        /// <param name="funcionalUsuario">Número da funcional do usuário responsável</param>
        public static void EnviarEmailAprovacaoAcesso(
            String emailMaster, 
            String nomeUsuario, 
            String emailUsuario,
            Int32 codigoIdUsuario,
            String perfilUsuario,
            Int32 numeroPv,
            String funcionalUsuario)
        {            
            //Arquivo do template e assunto do e-mail
            String arquivoTemplate = "AprovacaoAcesso/e-mail.txt";
            String assuntoEmail = "Aprovação de Acesso no Portal Rede";

            //Mapeamento de tags específicas do e-mail
            var tagValor = new Dictionary<String,String>();
            tagValor["##NOME_USUARIO##"] = nomeUsuario;
            tagValor["##EMAIL_USUARIO##"] = emailUsuario;
            tagValor["##PV##"] = numeroPv.ToString();

            //Prepara conteúdo do e-mail (obtém template e substitui tags)
            String conteudoEmail = PrepararConteudoEmail(arquivoTemplate, tagValor);

            //Envio do e-mail
            EnviarEmail(emailMaster, assuntoEmail, conteudoEmail);

            //Registra no histórico/log de atividades
            Historico.EnvioEmail(codigoIdUsuario, nomeUsuario, emailUsuario, perfilUsuario, 
                numeroPv, funcionalUsuario, "Solicitação de aprovação de acesso", emailMaster);
        }

        /// <summary>
        /// Envio de e-mail para o usuário que bloqueou o seu acesso.
        /// </summary>
        /// <param name="nomeUsuario">Nome do Usuário que bloqueou seu acesso</param>
        /// <param name="emailUsuario">E-mail do Usuário que bloqueou seu acesso (e-mail do destinatário)</param>
        /// <param name="codigoIdUsuario">Código ID do usuário que bloqueou seu acesso</param>
        /// <param name="numeroPv">Número do estabelecimento com o qual o usuário bloqueou seu acesso</param>
        /// <param name="perfilUsuario">Perfil do usuário que bloqueou seu acesso</param>
        /// <param name="funcionalUsuario">Funcional do usuário</param>
        public static void EnviarEmailAcessoBloqueado(
            String nomeUsuario,
            String emailUsuario,
            Int32 codigoIdUsuario,
            String perfilUsuario,
            Int32 numeroPv,
            String funcionalUsuario)
        {
            //Arquivo do template e assunto do e-mail
            String arquivoTemplate = "AcessoBloqueado/e-mail.txt";
            String assuntoEmail = "Acesso Bloqueado";

            //Mapeamento de tags específicas do e-mail
            var tagValor = new Dictionary<String, String>();
            tagValor["##PV##"] = numeroPv.ToString();

            //Prepara conteúdo do e-mail (obtém template e substitui tags)
            String conteudoEmail = PrepararConteudoEmail(arquivoTemplate, tagValor);

            //Envio do e-mail
            EnviarEmail(emailUsuario, assuntoEmail, conteudoEmail);

            //Registra no histórico/log de atividades
            Historico.EnvioEmail(codigoIdUsuario, nomeUsuario, emailUsuario, perfilUsuario,
                numeroPv, funcionalUsuario, "Acesso bloqueado", emailUsuario);
        }

        /// <summary>
        /// Envio de e-mail informativo para o e-mail alternativo do usuário que iniciou o processo de Recuperação de Senha.        
        /// </summary>
        /// <param name="emailUsuario">E-mail do destinatário/usuario que solicitou a recuperação da senha</param>
        /// <param name="formaRecuperacao">Forma de envio das informações de cadastramento da nova senha.</param>
        /// <param name="codigoIdUsuario">Código ID do usuário que solicitou a recuperação da senha</param>
        /// <param name="nomeUsuario">Nome do usuário que solicitou a recuperação da senha</param>
        /// <param name="numeroPvUsuario">Número PV do usuário que solicitou a recuperação da senha</param>
        /// <param name="perfilUsuario">Perfil do usuário que solicitou a recuperação da senha</param>
        /// <param name="funcionalUsuario">Número da funcional do usuário responsável</param>
        public static void EnviarEmailInformativoRecuperacaoSenha(
            String emailUsuario, 
            FormaRecuperacao formaRecuperacao,
            Int32 codigoIdUsuario,
            String nomeUsuario,
            String perfilUsuario,
            Int32 numeroPvUsuario,
            String funcionalUsuario)
        {
            //Arquivo do template e assunto do e-mail
            String arquivoTemplate = "InformativoRecuperacaoSenha/e-mail.txt";
            String assuntoEmail = "Aviso de Solicitação de Senha no Portal Rede";

            //Mapeamento de tags específicas do e-mail
            var tagValor = new Dictionary<String,String>();
            tagValor["##PV##"] = numeroPvUsuario.ToString();

            switch(formaRecuperacao)
            {
                case FormaRecuperacao.EmailPrincipal: 
                    tagValor["##FORMA_ENVIO##"] = "ao seu e-mail principal"; break;
                case FormaRecuperacao.EmailSecundario: 
                    tagValor["##FORMA_ENVIO##"] = "ao seu e-mail secundário"; break;
                case FormaRecuperacao.Sms: 
                    tagValor["##FORMA_ENVIO##"] = "ao seu celular"; break;
                default: 
                    tagValor["##FORMA_ENVIO##"] = String.Empty; break;
            }            
            
            //Prepara conteúdo do e-mail (obtém template e substitui tags)
            String conteudoEmail = PrepararConteudoEmail(arquivoTemplate, tagValor);

            //Envio do e-mail
            EnviarEmail(emailUsuario, assuntoEmail, conteudoEmail);

            //Registra no log/histórico de atividade
            Historico.EnvioEmail(codigoIdUsuario, nomeUsuario, emailUsuario, perfilUsuario,
                numeroPvUsuario, funcionalUsuario, "Informativo de recuperação de senha", emailUsuario);            
        }

        /// <summary>
        /// Envio de e-mail de rejeição para o usuário que solicitou acesso.
        /// </summary>
        /// <param name="emailUsuarioRejeitado">
        /// E-mail do destinatário/usuário que está tendo sua solicitação de acesso rejeitada.</param>
        /// <param name="mensagemMotivoRejeicao">
        /// Mensagem enviada pelo usuário master do motivo da rejeição da aprovação de acesso.</param>
        /// <param name="sessaoUsuarioResponsavel">Sessão do usuário responsável pela rejeição do acesso do usuário</param>
        public static void EnviarEmailSolicitacaoAcessoRejeitada(
            Sessao sessaoUsuarioResponsavel,
            String emailUsuarioRejeitado,            
            String mensagemMotivoRejeicao)
        {
            //Arquivo do template e assunto do e-mail
            String arquivoTemplate = "SolicitacaoAcessoRejeitada/e-mail.txt";
            String assuntoEmail = "Resposta da Solicitação de Acesso para o Portal Rede";

            //Mapeamento de tags específicas do e-mail
            var tagValor = new Dictionary<String,String>();
            tagValor["##MENSAGEM##"] = mensagemMotivoRejeicao;
            tagValor["##PV##"] = sessaoUsuarioResponsavel.CodigoEntidade.ToString();

            //Prepara conteúdo do e-mail (obtém template e substitui tags)
            String conteudoEmail = PrepararConteudoEmail(arquivoTemplate, tagValor);

            //Envio do e-mail
            EnviarEmail(emailUsuarioRejeitado, assuntoEmail, conteudoEmail);

            //Histórico (Envio de e-mail Rejeição de Usuário)            
            Historico.EnvioEmail(sessaoUsuarioResponsavel, "Rejeição de acesso", emailUsuarioRejeitado, 
                new List<Int32>(new[] { sessaoUsuarioResponsavel.CodigoEntidade }));
        }

        /// <summary>
        /// Envio de e-mail para o usuário que solicitou uma nova senha de acesso.
        /// Possui prazo de expiração (48h)
        /// </summary>
        /// <param name="emailDestinatario">E-mail do destinatário</param>
        /// <param name="codigoIdUsuario">Código ID do usuário que solicitou a nova senha de acesso</param>
        /// <param name="hashConfirmacao">Hash para confirmação do e-mail</param>
        /// <param name="formaRecuperacao">Forma de envio das informações de cadastramento da nova senha.</param>
        /// <param name="codigoIdUsuarioResponsavel">Código ID do usuário responsável pelo (re)envio de e-mail</param>
        /// <param name="emailUsuarioResponsavel">E-mail do usuário responsável pelo (re)envio de e-mail</param>
        /// <param name="nomeUsuarioResponsavel">Nome do usuário responsável pelo (re)envio de e-mail</param>
        /// <param name="numeroPvUsuarioResponsavel">Número do PV do usuário responsável pelo (re)envio de e-mail</param>
        /// <param name="perfilUsuarioResponsavel">Perfil do usuário responsável pelo (re)envio de e-mail</param>
        /// <param name="funcionalUsuarioResponsavel">Número da funcional do usuário responsável</param>
        public static void EnviarEmailRecuperacaoSenha(
            String emailDestinatario,
            Int32 codigoIdUsuario,
            Guid hashConfirmacao,
            FormaRecuperacao formaRecuperacao,
            Int32 codigoIdUsuarioResponsavel,
            String emailUsuarioResponsavel,
            String nomeUsuarioResponsavel,
            String perfilUsuarioResponsavel,
            Int32 numeroPvUsuarioResponsavel,
            String funcionalUsuarioResponsavel)
        {
            //Arquivo do template e assunto do e-mail
            String arquivoTemplate = "RecuperacaoSenha/e-mail.txt";
            String assuntoEmail = "Senha de Acesso para o Portal Rede";

            //Mapeamento de tags específicas do e-mail
            var tagValor = new Dictionary<String,String>();
            tagValor["##PV##"] = numeroPvUsuarioResponsavel.ToString();

            //QueryString com informações adicionais sobre a forma de recuperação da senha
            var qs = new QueryStringSegura();
            qs["FormaRecuperacao"] = ((Int32) formaRecuperacao).ToString();

            //Link de confirmação
            String urlConfirmacao = MontarUrlConfirmacao(codigoIdUsuario, hashConfirmacao, qs, false);
            Uri linkConfirmacao = new Uri(urlConfirmacao);
            String descricaoLinkConfirmacao = PrepararDescricaoUrlConfirmacao(urlConfirmacao);
            
            tagValor["##CONFIRMACAO_URL##"] = linkConfirmacao.GetLeftPart(UriPartial.Query);
            tagValor["##CONFIRMACAO##"] = descricaoLinkConfirmacao;
            
            //Prepara conteúdo do e-mail (obtém template e substitui tags)
            String conteudoEmail = PrepararConteudoEmail(arquivoTemplate, tagValor);

            //Envio do e-mail
            EnviarEmail(emailDestinatario, assuntoEmail, conteudoEmail);

            //Armazena no histórico/log de atividades
            Historico.EnvioEmail(codigoIdUsuarioResponsavel, nomeUsuarioResponsavel,
                emailUsuarioResponsavel, perfilUsuarioResponsavel, numeroPvUsuarioResponsavel,
                funcionalUsuarioResponsavel, "Recuperação de senha", emailDestinatario);
        }

        /// <summary>
        /// Envio de e-mail de Confirmação de Cadastro para o usuário cadastrado pelo Master/Central de Atendimento.
        /// </summary>
        /// <param name="emailDestinatario">E-mail do destinatário/usuário sendo criado</param>
        /// <param name="codigoIdUsuario">Código ID do usuário para confirmação do e-mail</param>
        /// <param name="pvsDestinatario">PVs do usuário sendo criado</param>
        /// <param name="hashConfirmacao">Hash de confirmação do e-mail</param>
        /// <param name="sessaoUsuario">Sessão do usuário Master/Central</param>
        public static void EnviarEmailConfirmacaoCadastro(
            Sessao sessaoUsuario,
            String emailDestinatario,
            List<Int32> pvsDestinatario,
            Int32 codigoIdUsuario,
            Guid hashConfirmacao)
        {
            //Arquivo do template e assunto do e-mail
            //TODO: Rever Template
            String arquivoTemplate = "ConfirmacaoCadastro/e-mail.txt";
            
            /* Alterado por causa do Mobile */
            //TODO: Mudar no Redecard.PN.Comun

            String assuntoEmail = "Confirmação de Cadastro - Rede";

            //Mapeamento de tags específicas do e-mail
            var tagValor = new Dictionary<String,String>();

            //Link de confirmação
            String urlConfirmacao = MontarUrlConfirmacao(codigoIdUsuario, hashConfirmacao);
            String urlConfirmacaoMobile = MontarUrlConfirmacao(codigoIdUsuario, hashConfirmacao, true);
            Uri linkConfirmacao = new Uri(urlConfirmacao);
            Uri linkConfirmacaoMobile = new Uri(urlConfirmacaoMobile);
            String descricaoLinkConfirmacao = PrepararDescricaoUrlConfirmacao(urlConfirmacao);
            String descricaoLinkConfirmacaoMobile = PrepararDescricaoUrlConfirmacao(urlConfirmacaoMobile);

            tagValor["##CONFIRMACAO_URL##"] = linkConfirmacao.GetLeftPart(UriPartial.Query);
            tagValor["##CONFIRMACAO##"] = descricaoLinkConfirmacao;
            tagValor["##CONFIRMACAO_URL_MOBILE##"] = linkConfirmacaoMobile.GetLeftPart(UriPartial.Query);
            tagValor["##CONFIRMACAO_MOBILE##"] = descricaoLinkConfirmacao;

            //Prepara conteúdo do e-mail (obtém template e substitui tags)
            String conteudoEmail = PrepararConteudoEmail(arquivoTemplate, tagValor);

            //Envio do e-mail
            EnviarEmail(emailDestinatario, assuntoEmail, conteudoEmail);

            //Armazena no histórico/log de atividade
            Historico.EnvioEmail(sessaoUsuario, "Confirmação de acesso", emailDestinatario, pvsDestinatario);
        }

        /// <summary>
        /// Envio de e-mail de Confirmação de Migração para o usuário legado.
        /// O e-mail de confirmação possui prazo de validade até o final do período de migração.
        /// </summary>
        /// <param name="emailDestinatario">E-mail do destinatário/usuário que migrou seu acesso ao Portal</param>
        /// <param name="codigoIdUsuario">Código ID do usuário, que migrou seu acesso ao Portal</param>
        /// <param name="hashConfirmacao">Hash de confirmação do e-mail</param>
        /// <param name="nomeUsuario">Nome do usuário</param>
        /// <param name="numeroPv">Número do PV</param>
        /// <param name="perfilUsuario">Perfil do usuário</param>
        /// <param name="funcionalUsuario">Número da funcional do usuário responsável</param>
        public static void EnviarEmailConfirmacaoMigracao(
            String emailDestinatario,
            Int32 codigoIdUsuario,
            Guid hashConfirmacao,
            String nomeUsuario,
            String perfilUsuario,
            Int32 numeroPv,
            String funcionalUsuario)
        {
            //Arquivo do template e assunto do e-mail
            String arquivoTemplate = "ConfirmacaoMigracao/e-mail.txt";
            String assuntoEmail = "Solicitação de Acesso para o Portal Rede";

            //Mapeamento de tags específicas do e-mail
            var tagValor = new Dictionary<String, String>();

            //Link de confirmação
            String urlConfirmacao = MontarUrlConfirmacao(codigoIdUsuario, hashConfirmacao);
            Uri linkConfirmacao = new Uri(urlConfirmacao);
            String descricaoLinkConfirmacao = PrepararDescricaoUrlConfirmacao(urlConfirmacao);

            tagValor["##CONFIRMACAO_URL##"] = linkConfirmacao.GetLeftPart(UriPartial.Query);
            tagValor["##CONFIRMACAO##"] = descricaoLinkConfirmacao;
            tagValor["##PV##"] = numeroPv.ToString();

            //Prepara conteúdo do e-mail (obtém template e substitui tags)
            String conteudoEmail = PrepararConteudoEmail(arquivoTemplate, tagValor);

            //Envio do e-mail
            EnviarEmail(emailDestinatario, assuntoEmail, conteudoEmail);

            //Armazena no histórico/log de atividades
            Historico.EnvioEmail(codigoIdUsuario, nomeUsuario, emailDestinatario, 
                perfilUsuario, numeroPv, funcionalUsuario, "Confirmação de Migração", emailDestinatario);
        }

        /// <summary>
        /// Envio de e-mail de Confirmação de Cadastro para o usuário que solicitou o acesso.
        /// O e-mail de confirmação possui 48h no prazo de validade.
        /// </summary>
        /// <param name="emailDestinatario">E-mail do destinatário/usuário que solicitou o acesso ao Portal</param>
        /// <param name="codigoIdUsuario">Código ID do usuário recém criado, que solicitou acesso ao Portal</param>
        /// <param name="hashConfirmacao">Hash de confirmação do e-mail</param>
        /// <param name="codigoIdUsuarioResponsavel">Código ID do usuário responsável</param>
        /// <param name="emailUsuarioResponsavel">E-mail do usuário responsável</param>
        /// <param name="nomeUsuarioResponsavel">Nome do usuário responsável</param>
        /// <param name="numeroPvUsuarioResponsavel">Número do PV do usuário responsável</param>
        /// <param name="perfilUsuarioResponsavel">Perfil do usuário responsável</param>
        /// <param name="funcionalUsuarioResponsavel">Número da funcional do usuário responsável</param>
        public static void EnviarEmailConfirmacaoCadastro48h(
            String emailDestinatario,
            Int32 codigoIdUsuario,
            Guid hashConfirmacao,
            Int32 codigoIdUsuarioResponsavel,
            String emailUsuarioResponsavel,
            String nomeUsuarioResponsavel,
            String perfilUsuarioResponsavel,
            Int32 numeroPvUsuarioResponsavel,
            String funcionalUsuarioResponsavel)
        {
            //Arquivo do template e assunto do e-mail
            //TODO: Rever Template
            String arquivoTemplate = "ConfirmacaoCadastro48h/e-mail.txt";
            String assuntoEmail = "Confirmação de cadastro do Acesso Rede";
            
            //Mapeamento de tags específicas do e-mail
            var tagValor = new Dictionary<String,String>();

            //Link de confirmação
            String urlConfirmacao = MontarUrlConfirmacao(codigoIdUsuario, hashConfirmacao);
            String urlConfirmacaoMobile = MontarUrlConfirmacao(codigoIdUsuario, hashConfirmacao, true);
            Uri linkConfirmacao = new Uri(urlConfirmacao);
            Uri linkConfirmacaoMobile = new Uri(urlConfirmacaoMobile);
            String descricaoLinkConfirmacao = PrepararDescricaoUrlConfirmacao(urlConfirmacao);
            String descricaoLinkConfirmacaoMobile = PrepararDescricaoUrlConfirmacao(urlConfirmacaoMobile);

            tagValor["##CONFIRMACAO_URL##"] = linkConfirmacao.GetLeftPart(UriPartial.Query);
            tagValor["##CONFIRMACAO##"] = descricaoLinkConfirmacao;
            tagValor["##CONFIRMACAO_URL_MOBILE##"] = linkConfirmacaoMobile.GetLeftPart(UriPartial.Query);
            tagValor["##CONFIRMACAO_MOBILE##"] = descricaoLinkConfirmacao;
            tagValor["##PV##"] = numeroPvUsuarioResponsavel.ToString();

            //Prepara conteúdo do e-mail (obtém template e substitui tags)
            String conteudoEmail = PrepararConteudoEmail(arquivoTemplate, tagValor);

            //Envio do e-mail
            EnviarEmail(emailDestinatario, assuntoEmail, conteudoEmail);

            //Registra no log/histórico de atividades
            Historico.EnvioEmail(codigoIdUsuarioResponsavel, nomeUsuarioResponsavel, 
                emailUsuarioResponsavel, perfilUsuarioResponsavel, numeroPvUsuarioResponsavel, 
                funcionalUsuarioResponsavel, "Confirmação de acesso", emailDestinatario);
        }

        /// <summary>
        /// Envio de e-mail para o usuário Master, informando-o sobre 
        /// o bloqueio do formulário de criação de novos usuários.
        /// </summary>
        /// <param name="emailMaster">E-mail do destinatório/usuário Master</param>
        /// <param name="emailUsuario">E-mail do usuário que tentou/insistiu no cadastro no Portal.</param>
        /// <param name="codigoEntidadeBloqueada">Código da entidade</param>
        public static void EnviarEmailSolicitacoesAcessoBloqueada(
            String emailMaster,           
            String emailUsuario,
            Int32 codigoEntidadeBloqueada)
        {
            //Arquivo do template e assunto do e-mail
            String arquivoTemplate = "SolicitacoesAcessoBloqueada/e-mail.txt";
            String assuntoEmail = "Solicitação de Acesso Bloqueada no Portal Rede";

            //Mapeamento de tags específicas do e-mail
            var tagValor = new Dictionary<String,String>();
            tagValor["##EMAIL_USUARIO##"] = emailUsuario;
            tagValor["##PV##"] = codigoEntidadeBloqueada.ToString();

            //Prepara conteúdo do e-mail (obtém template e substitui tags)
            String conteudoEmail = PrepararConteudoEmail(arquivoTemplate, tagValor);

            //Envio do e-mail
            EnviarEmail(emailMaster, assuntoEmail, conteudoEmail);

            //Armazena no histórico/log de atividades
            Historico.EnvioEmail(null, null, emailUsuario, null, codigoEntidadeBloqueada, 
                null, "Formulário da parte aberta bloqueado", emailMaster);
        }

       

        /// <summary>
        /// Monta a URL para a criação do link de confirmação/verificação por e-mail.
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <param name="hashConfirmacao">Hash de confirmação do e-mail</param>
        /// <param name="qs">QueryString com informações adicionais</param>
        /// <param name="numeroPv">Número do Estabelecimento (PV)</param>
        /// <param name="mobile">Identifica se a url é mobile ou não</param>
        private static String MontarUrlConfirmacao(
             Int32 codigoIdUsuario,
            Guid hashConfirmacao,
            Int32 numeroPv,
            QueryStringSegura qs,
            Boolean mobile
            )
        {
            if (qs == null)
                qs = new QueryStringSegura();

            qs["CodigoIdUsuario"] = codigoIdUsuario.ToString();
            qs["Hash"] = hashConfirmacao.ToString("N");
            qs["PV"] = numeroPv.ToString();

            String url = new Uri(SPContext.Current.Site.Url).GetLeftPart(UriPartial.Authority);
            //18/10/2014: Conforme solicitação do André Fernandes e Cicotti, se for Produção, chumba o portal.userede.com.br
            url = url.Contains("172.16.7.44") || url.Contains("userede.com.br") || url.Contains("hom.userede.com.br") ? "https://portal.userede.com.br" : url;

            return String.Format("{0}/pt-br/novoacesso/paginas/{2}confirmaracesso.aspx?dados={1}", url, qs.ToString(), mobile ? @"Mobile/" : "");
        }

        /// <summary>
        /// Monta a URL para a criação do link de confirmação/verificação por e-mail.
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <param name="hashConfirmacao">Hash de confirmação do e-mail</param>
        private static String MontarUrlConfirmacao(
            Int32 codigoIdUsuario,
            Guid hashConfirmacao)
        {
            return MontarUrlConfirmacao(codigoIdUsuario, hashConfirmacao, null, false);
        }

        /// <summary>
        /// Monta a URL para a criação do link de confirmação/verificação por e-mail.
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <param name="hashConfirmacao">Hash de confirmação do e-mail</param>
        /// <param name="qs">QueryString com informações adicionais</param>
        private static String MontarUrlConfirmacao(
            Int32 codigoIdUsuario,
            Guid hashConfirmacao,
            QueryStringSegura qs,
            bool mobile)
        {
            return MontarUrlConfirmacao(codigoIdUsuario, hashConfirmacao, 0, qs, mobile);
        }


        private static String MontarUrlConfirmacao(
            Int32 codigoIdUsuario,
            Guid hashConfirmacao,
            bool mobile)
        {
            return MontarUrlConfirmacao(codigoIdUsuario, hashConfirmacao, null, mobile);
        }

        /// <summary>
        /// Prepara a descrição da URL, inserindo o elemento HTML "Word Break Opportunity"
        /// a cada bloco de string
        /// </summary>
        /// <param name="urlConfirmacao">URL a ser preparada</param>
        /// <returns>URL tratada</returns>
        private static String PrepararDescricaoUrlConfirmacao(
            String urlConfirmacao)
        {
            //Elemento <wbr>
            String elementoWbr = "<wbr>";

            //Divide a URL em blocos de 50 caracteres, separados pelo elemento HTML <wbr>
            return String.Join(elementoWbr, urlConfirmacao.SplitByLength(8).ToArray());
        }

        /// <summary>
        /// Prepara conteúdo do e-mail, obtendo o template e substituindo as tags
        /// </summary>
        /// <param name="arquivoTemplate">Nome do arquivo do template do e-mail</param>
        /// <param name="tagValor">Mapeamento entre TAG e Valor</param>
        /// <returns>Conteúdo do e-mail, com as tags substituídas pelos seus valores</returns>
        private static String PrepararConteudoEmail(
            String arquivoTemplate,
            Dictionary<String, String> tagValor)
        {
            String css = String.Empty;
            String conteudoEmail = ObterTemplateEmail(arquivoTemplate, out css);

            CSSInlinerConfig config = new CSSInlinerConfig();
            config.TextoCSS = css;
            config.RemoverReferenciasCss = true;

            conteudoEmail = new CSSInliner(config).Processar(conteudoEmail);

            conteudoEmail = SubstituirTags(conteudoEmail, tagValor);
            return conteudoEmail;
        }

        /// <summary>
        /// Substitui as tags (keys) com os valores preenchidos no dicionário.
        /// </summary>
        /// <param name="conteudoEmail">Template/Conteúdo do e-mail com as tags ##TAG## para substituição</param>
        /// <param name="tagValor">Mapeamento entre TAG e Valor</param>
        /// <returns>Conteudo do e-mail, com as tags substituídas pelos seus valores</returns>
        private static String SubstituirTags(
            String conteudoEmail, 
            Dictionary<String, String> tagValor)
        {
            if(tagValor == null)
                tagValor = new Dictionary<String,String>();
            if (String.IsNullOrEmpty(conteudoEmail))
                conteudoEmail = String.Empty;
            
            //Tags padrão para todos os templates
            String url = new Uri(SPContext.Current.Site.Url).GetLeftPart(UriPartial.Authority);
            tagValor["##URL##"] = url;
            tagValor["##USEREDE##"] = TAG_USEREDE;
            tagValor["##COMO_ACESSAR_URL##"] = String.Format(TAG_COMO_ACESSAR_URL, url);
            tagValor["##COMO_ACESSAR##"] = TAG_COMO_ACESSAR;
            tagValor["##CENTRAL_ATENDIMENTO_URL##"] = String.Format(TAG_CENTRAL_ATENDIMENTO_URL, url);

            //Substituição de tags
            foreach(String tag in tagValor.Keys)
                conteudoEmail = conteudoEmail.Replace(tag, tagValor[tag]);

            return conteudoEmail;
        }
        
        /// <summary>
        /// Obtém o conteúdo do arquivo de template a ser utilizado nos e-mails.
        /// </summary>
        /// <param name="arquivoTemplate">
        /// Nome do arquivo do template, com extensão.<br/>
        /// Busca o arquivo na pasta "/_layouts/DadosCadastrais/MODELOS/"
        /// </param>
        /// <param name="conteudoCss">Conteúdo do CSS dos links internos do HTML</param>
        /// <returns>Conteúdo do arquivo de Template</returns>
        private static String ObterTemplateEmail(String arquivoTemplate, out String conteudoCss)
        {
            StringBuilder conteudosCss = new StringBuilder();

            String arquivoModelo = String.Format(@"\DadosCadastrais\MODELOS\{0}", arquivoTemplate);
            String html = CSSInliner.ObterConteudoLayouts(arquivoModelo);

            foreach (String arquivoCss in CSSInliner.RecuperarEstilosCss(html))
            {
                if(arquivoCss.StartsWith("http"))
                    continue;

                String pastaModelo = Path.GetDirectoryName(arquivoModelo);

                String css = CSSInliner.ObterConteudoLayouts(Path.Combine(pastaModelo, arquivoCss));
                conteudosCss.AppendLine(css);
            }

            conteudoCss = conteudosCss.ToString();
            return html;
        }

        /// <summary>
        /// Envia e-mail
        /// </summary>
        /// <param name="emailDestinatario">E-mail do destinatário</param>
        /// <param name="conteudoEmail">Conteúdo do email</param>
        /// <param name="assunto">Assunto do e-mail</param>
        private static Boolean EnviarEmail(
            String emailDestinatario, 
            String assunto, 
            String conteudoEmail)
        {
            //Se e-mail do destinatário não existe, não envia e-mail
            if (String.IsNullOrEmpty(emailDestinatario))
                return false;

            //Cria o objeto para envio de e-mail (Buscando da configuração do Sharepoint)
            string smtpServer = SPAdministrationWebApplication.Local.OutboundMailServiceInstance != null ?
                                SPAdministrationWebApplication.Local.OutboundMailServiceInstance.Server.Address
                                : "";
            
#if !DEBUG
            //Verifica se retornou o servidor para envio de e-mail
            if (string.IsNullOrEmpty(smtpServer))
                throw new Exception("SMTP para envio de e-mail não configurado no servidor do Sharepoint.");

#else
            if (string.IsNullOrEmpty(smtpServer))
                smtpServer = "smtp@smtp.com.br";
#endif

            //Cria o objeto para envio do e-mail
            SmtpClient smtpClient = new SmtpClient(smtpServer);
            
            //Cria a mensagem e adiciona o anexo
            MailMessage mensagemEmail = new MailMessage();
            mensagemEmail.From = new MailAddress(Remetente, "Portal Rede");
            mensagemEmail.To.Add(emailDestinatario);
            mensagemEmail.Subject = assunto;
            mensagemEmail.Body = conteudoEmail;
            mensagemEmail.IsBodyHtml = true;

            //Anexa as imagens (content ID)
            CSSInliner.EmbutirImagens(mensagemEmail);

            //Envia o e-mail
            smtpClient.Send(mensagemEmail);

            return true;
        }
    }
}