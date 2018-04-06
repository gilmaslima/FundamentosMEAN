using System;
using Microsoft.SharePoint;
using System.Web;
using System.Web.Script.Serialization;
using Rede.PN.Credenciamento.Sharepoint.Servicos;

namespace Rede.PN.Credenciamento.Sharepoint.Layouts.Rede.Credenciamento.Handlers
{
    public partial class DadosIniciais : IHttpHandler
    {
        #region [ Propriedades ]

        /// <summary>JS Serializer</summary>
        private static JavaScriptSerializer _jsSerializer;
        private static JavaScriptSerializer JsSerializer
        {
            get
            {
                return _jsSerializer ?? (_jsSerializer = new JavaScriptSerializer());
            }
        }

        /// <summary>Valor padrão de propriedades</summary>
        public bool IsReusable { get { return false; } }

        #endregion

        public void ProcessRequest(HttpContext context)
        {
            
        }
    }
}
