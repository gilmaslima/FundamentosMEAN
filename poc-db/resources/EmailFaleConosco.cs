/*
© Copyright 2014 Rede S.A.
Autor : Lucas Uehara
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Bases;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Emails
{
    /// <summary>
    /// Classe para implementação do envio do Fale Conosco
    /// </summary>
    public sealed class EmailFaleConosco : BaseEmail
    {
        /// <summary>
        /// Dicionário de dados
        /// </summary>
        private Dictionary<String, String> dados;

        /// <summary>
        /// Construtor da classe
        /// </summary>        
        public EmailFaleConosco(String from, String to, String subject, String bodyContent, Dictionary<String, String> dictionary)
            : base(from, to, subject, bodyContent)
        {
            this.dados = dictionary;
        }

        /// <summary>
        /// Envio de e-mail.
        /// </summary>
        /// <param name="parametros">Parâmetros</param>
        /// <param name="templateHtml">Template HTML</param>
        /// <returns>Tratamento de retorno</returns>
        public Boolean EnviarEmail(Dictionary<String, String> parametros = null, String templateHtml = null)
        {
            var retorno = default(Boolean);

            if (BaseEmail.EnderecoEmailValido(To))
                retorno = EnviarEmailSmtpClient(parametros, templateHtml);
            else
                throw new Exception("E-mail inválido.", new Exception());

            return retorno;
        }
    }
}
