using Redecard.PN.Extrato.SharePoint.Modelo;
using System;
using System.Web.UI;
using Redecard.PN.Comum;
using System.Web.Script.Serialization;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.ContaCorrente {
    /// <summary>
    /// 
    /// </summary>
    public partial class RelatorioContaCorrente : BaseUserControl, IRelatorioHandler, IRelatorioCSV {
        /// <summary>
        /// 
        /// </summary>
        private BuscarDados dadosPesquisa = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e) {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dados"></param>
        public void Pesquisar(BuscarDados dados) {
            dadosPesquisa = dados;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dados"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="incluirTotalizadores"></param>
        /// <returns></returns>
        public string ObterTabelaExcel(BuscarDados dados, int quantidadeRegistros, bool incluirTotalizadores) {
            //throw new NotImplementedException();
            return String.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        public string IdControl {
            get { return "RelatorioContaCorrente"; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dadosBusca"></param>
        /// <param name="funcaoOutput"></param>
        public void GerarConteudoRelatorio(BuscarDados dadosBusca, Action<string> funcaoOutput) {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e) {
            if (this.SessaoAtual != null && Util.UsuarioLogadoFBA()) {
                ClientScriptManager scriptManager = Page.ClientScript;
                if (scriptManager != null && !scriptManager.IsClientScriptBlockRegistered("__contaCorrenteScript__")) {
                    scriptManager.RegisterClientScriptBlock(typeof(Page), "__contaCorrenteScript__", this.GerarScriptsRelatoriosContaCorrente(), true);
                }
            }
            base.OnPreRender(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected String GerarScriptsRelatoriosContaCorrente() {
            // renderizar objeto de pesquisa
            var script = String.Empty;
            var dados = new {
                Estabelecimento = this.SessaoAtual.CodigoEntidade,
                Ativo = !this.SessaoAtual.StatusPVCancelado(),
                TransacionaDolar = this.SessaoAtual.TransacionaDolar
            };

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            script += String.Format("__conta__corrente__object__ = {{ \"search\": {0}, \"dados\": {1} }};", serializer.Serialize(dadosPesquisa), serializer.Serialize(dados));
            return script;
        }
    }
}