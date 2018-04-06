/*
© Copyright 2014 Rede S.A.
Autor : Mário Neto
Empresa : Iteris Consultoria e Software
*/
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;

namespace Rede.PN.AtendimentoDigital.SharePoint.WebParts.AtendimentoDigital.VideoContainer
{
    /// <summary>
    /// WebPart genérica para carregamento do container de vídeo
    /// </summary>
    [ToolboxItemAttribute(false)]
    public partial class VideoContainer : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public VideoContainer()
        {
        }

        /// <summary>
        /// OnInit
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        /// <summary>
        /// Page_Load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}