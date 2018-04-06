using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.Script.Serialization;

namespace Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum
{
    public partial class ConsultaCEP : UserControlBase
    {
        /// <summary>JS Serializer</summary>
        private static JavaScriptSerializer _jsSerializer;
        private static JavaScriptSerializer JsSerializer
        {
            get
            {
                return _jsSerializer ?? (_jsSerializer = new JavaScriptSerializer());
            }
        }

        #region [ Propriedades ]

        public String IDControleCEP { get; set; }
        public String IDControleEndereco { get; set; }
        public String IDControleCidade { get; set; }
        public String IDControleBairro { get; set; }
        public String IDControleUF { get; set; }
        public String IDControleContainer { get; set; }
        public String FuncaoCallback { get; set; }
        public Boolean Assincrono { get; set; }
        public Boolean BloquearControles { get; set; }
        public Boolean ExibirBlockUI { get; set; }
        public Boolean AutoLimparControles { get; set; }

        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            var config = new 
            {
                this.IDControleBairro,
                this.IDControleCEP,
                this.IDControleCidade,
                this.IDControleContainer,
                this.IDControleEndereco,
                this.IDControleUF,
                this.FuncaoCallback,
                Assincrono = this.Assincrono.ToString().ToLower(),
                BloquearControles = this.BloquearControles.ToString().ToLower(),
                ExibirBlockUI = this.ExibirBlockUI.ToString().ToLower(),
                AutoLimparControles = this.AutoLimparControles.ToString().ToLower()
            };

            hddConfig.Value = JsSerializer.Serialize(config);
        }
    }
}
