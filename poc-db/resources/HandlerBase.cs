/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Web;
using Microsoft.SharePoint;
using Redecard.PN.Comum;

namespace Rede.PN.AtendimentoDigital.SharePoint.Handlers
{
    /// <summary>
    /// Handler Base
    /// </summary>
    public class HandlerBase
    {
        /// <summary>
        /// Código de erro padrão
        /// </summary>
        public static Int32 CodigoErro { get { return 300; } }

        /// <summary>
        /// Fonte padrão
        /// </summary>
        public static String Fonte { get { return "Rede.PN.AtendimentoDigital.SharePoint.Handlers.HandlerBase"; } }

        /// <summary>
        /// Request
        /// </summary>
        public HttpRequest Request { get; set; }

        /// <summary>
        /// Sessão atual do usuário
        /// </summary>
        public Sessao Sessao { get; set; }

        /// <summary>
        /// SharePoint Context
        /// </summary>
        public SPContext CurrentSPContext { get; set; }
    }
}