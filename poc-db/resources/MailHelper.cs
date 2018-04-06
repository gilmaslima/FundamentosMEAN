/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Redecard.PN.FMS.Comum.Log
{
    internal delegate void AsyncEnviarEmailAlerta(string remetente, string destinatarios, string assunto, string mensagem, MailPriority prioridade);
    /// <summary>
    /// Este componente publica a classe MailHelper, que expõe métodos para manipular as funções de ajuda.
    /// </summary>
    public class MailHelper
    {

        private const string AlertaEmailErroRemetente = "AlertaEmailErroRemetente";
        private const string AlertaEmailErroDesinatarios = "AlertaEmailErroDesinatarios";
        private const string AlertaEmailErroAssunto = "AlertaEmailErroAssunto";
        private const string AlertaEmailErroMensagem = "AlertaEmailErroMensagem";
        /// <summary>
        /// Este método é utilizado para enviar email de erros de processamento.
        /// </summary>
        /// <param name="idOperacao"></param>
        /// <param name="mensagem"></param>
        /// <param name="parametros"></param>
        /// <param name="dadosAdicionais"></param>
        public static void EnviarEmailErrosProcessamento(Guid idOperacao, string mensagem, object[] parametros, object dadosAdicionais)
        {

            string remetente = (string)ConfiguracaoHelper.ObterValorConfiguracao(AlertaEmailErroRemetente);
            string destinatarios = (string)ConfiguracaoHelper.ObterValorConfiguracao(AlertaEmailErroDesinatarios);
            string assunto = (string)ConfiguracaoHelper.ObterValorConfiguracao(AlertaEmailErroAssunto);
            string mensagemBase = (string)ConfiguracaoHelper.ObterValorConfiguracao(AlertaEmailErroMensagem);

            MailSender mailSender = new MailSender();

            AsyncEnviarEmailAlerta enviarMail = new AsyncEnviarEmailAlerta(mailSender.EnviarEmail);

            enviarMail.BeginInvoke(remetente, destinatarios, assunto, ObterMensagemFormatada(mensagemBase, mensagem, parametros, dadosAdicionais, idOperacao), MailPriority.High, null, null);

        }
        /// <summary>
        /// Este método é utilizado para  enviar email de alerta.
        /// </summary>
        /// <param name="idOperacao"></param>
        /// <param name="mensagem"></param>
        /// <param name="dadosAdicionais"></param>
        /// <param name="ex"></param>
        public static void EnviarEmailAlerta(Guid idOperacao, string mensagem, object dadosAdicionais, Exception ex)
        {

            string remetente = (string)ConfiguracaoHelper.ObterValorConfiguracao(AlertaEmailErroRemetente);
            string destinatarios = (string)ConfiguracaoHelper.ObterValorConfiguracao(AlertaEmailErroDesinatarios);
            string assunto = (string)ConfiguracaoHelper.ObterValorConfiguracao(AlertaEmailErroAssunto);
            string mensagemBase = (string)ConfiguracaoHelper.ObterValorConfiguracao(AlertaEmailErroMensagem);

            MailSender mailSender = new MailSender();

            AsyncEnviarEmailAlerta enviarMail = new AsyncEnviarEmailAlerta(mailSender.EnviarEmail);

            enviarMail.BeginInvoke(remetente, destinatarios, assunto, ObterMensagemFormatada(mensagemBase, mensagem, dadosAdicionais, ex, idOperacao), MailPriority.High, null, null);

        }
        /// <summary>
        /// Este método é utilizado para  obter mensagem de erro formatada.
        /// </summary>
        /// <param name="mensagemBase"></param>
        /// <param name="mensagemAdicional"></param>
        /// <param name="parametros"></param>
        /// <param name="dadosAdicionais"></param>
        /// <param name="idOperacao"></param>
        /// <returns></returns>
        private static string ObterMensagemFormatada(string mensagemBase, string mensagemAdicional, object[] parametros, object dadosAdicionais, Guid idOperacao)
        {

            StringBuilder sb = new StringBuilder();


            sb.Append(mensagemBase);
            sb.AppendLine("<br>");
            sb.AppendLine("Dados Adicionais:");
            sb.AppendLine("<br>");
            sb.AppendLine("-----------------------------------------------------");
            sb.AppendLine("<br>");
            sb.AppendLine("ID Operacao: " + idOperacao.ToString());
            sb.AppendLine("<br>");
            sb.AppendLine(mensagemAdicional);
            sb.AppendLine("<br>");
            if (dadosAdicionais != null)
                sb.Append(UtilHelper.SerializarDados(dadosAdicionais));
            
            return sb.ToString();
        }
        /// <summary>
        /// Este método é utilizado para  obter mensagem de erro formatada.
        /// </summary>
        /// <param name="mensagemBase"></param>
        /// <param name="mensagemAdicional"></param>
        /// <param name="dadosAdicionais"></param>
        /// <param name="ex"></param>
        /// <param name="idOperacao"></param>
        /// <returns></returns>
        private static string ObterMensagemFormatada(string mensagemBase, string mensagemAdicional, object dadosAdicionais, Exception ex, Guid idOperacao)
        {

            StringBuilder sb = new StringBuilder();
           

            sb.Append(mensagemBase.Replace("{0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
            sb.AppendLine("<br>");
            sb.AppendLine("Dados Adicionais:");
            sb.AppendLine("<br>");
            sb.AppendLine("-----------------------------------------------------");
            sb.AppendLine("<br>");
            sb.AppendLine("ID Operacao: " + idOperacao.ToString());
            sb.AppendLine("<br>");
            sb.AppendLine(mensagemAdicional);
            sb.AppendLine("<br>");
            if (dadosAdicionais != null)
                sb.Append(UtilHelper.SerializarDados(dadosAdicionais));
            sb.AppendLine("<br>");
            sb.AppendFormat("Exception: {0} - Stacktrace: {1}", ex.Message, ex.StackTrace);

            return sb.ToString();
        }

    }
}
