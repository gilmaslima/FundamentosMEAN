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
    /// Implementação do envio de e-mail de Abertura de OS para Suporte à Maquininha
    /// </summary>
    public sealed class EmailSuporteMaquininha : BaseEmail
    {
        /// <summary>
        /// Construtor padrão
        /// </summary>        
        public EmailSuporteMaquininha(String from, String to, String subject)
            : base(from, to, subject, String.Empty) { }

        /// <summary>
        /// Envio de e-mail
        /// </summary>
        /// <param name="parametros">Parâmetros do e-mail</param>
        /// <param name="templateHtml">Template HTML</param>
        /// <param name="repository">Repositório</param>
        /// <returns>Resultado do envio do e-mail</returns>
        public Boolean EnviarEmail(Dictionary<String, String> parametros, String templateHtml, List<Repository.AtendimentoDigital> repository)
        {
            var retorno = default(Boolean);

            if (BaseEmail.EnderecoEmailValido(To))
                retorno = EnviarEmailSmtpClient(parametros, templateHtml, repository);
            else
                throw new Exception("E-mail inválido.", new Exception());

            return retorno;
        }
    }
}
