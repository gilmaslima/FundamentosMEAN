/*
© Copyright 2014 Rede S.A.
Autor : Lucas Uehara
Empresa : Iteris Consultoria e Software
*/
using System;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.OrdemServico
{
    /// <summary>
    /// Classe modelo Contato OS
    /// </summary>
    public class ContatoOs
    {
        /// <summary>
        /// Nome do contato
        /// </summary>
        public String Nome { get; set; }

        /// <summary>
        /// E-mail do contato
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// Telefone do contato
        /// </summary>
        public String Telefone { get; set; }

        /// <summary>
        /// Celular do contato
        /// </summary>
        public String Celular { get; set; }
    }
}
