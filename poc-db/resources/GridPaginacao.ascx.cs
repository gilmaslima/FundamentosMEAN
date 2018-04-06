using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Linq;
using System.Web;
using System.ComponentModel;
namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais
{
    /// <summary>
    /// 
    /// </summary>
    public partial class GridPaginacao : UserControlBase
    {
        public Boolean SingleSelect { get; set; }

        /// <summary>
        /// Conteúdo digitado no campo de busca
        /// </summary>
        public string Busca
        {
            get
            {
                return txtBusca.Text;
            }
        }

        /// <summary>
        /// Evento que é disparado quando a paginação é obtida.
        /// </summary>
        public event Redecard.PN.Comum.Paginacao.ObterDadosEventHandler ObterPaginacao;

        public event EventHandler OnBuscarDados;
        /// <summary>
        /// Atribui uma fonte de dados.
        /// </summary>
        [Bindable(true)]
        public IEnumerable<EntidadeServico.EntidadeServicoModel> DataSource
        {
            get
            {
                if (ViewState["ObjetoDataSourcePaginacao"] != null)
                    return (IEnumerable<EntidadeServico.EntidadeServicoModel>)ViewState["ObjetoDataSourcePaginacao"];
                return null;
            }
            set
            {
                ViewState["ObjetoDataSourcePaginacao"] = value;
            }
        }

        /// <summary>
        /// Faz o Bind dos dados no Grid com a paginação.
        /// </summary>
        public override void DataBind()
        {
            if (DataSource == null)
                return;
            this.paginacaoSelecaoPvs.Carregar();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdPesquisa"></param>
        /// <param name="registroInicial"></param>
        /// <param name="quantidadeRegistrosRetornar"></param>
        /// <param name="quantidadeRegistrosPesquisar"></param>
        /// <param name="quantidadeTotalRegistrosEmCache"></param>
        /// <param name="parametros"></param>
        /// <returns></returns>
        protected IEnumerable<object> paginacaoSelecaoPvs_ObterDados(Guid IdPesquisa, int registroInicial, int quantidadeRegistrosRetornar, int quantidadeRegistrosPesquisar, out int quantidadeTotalRegistrosEmCache, params object[] parametros)
        {
            quantidadeTotalRegistrosEmCache = 0;
            if (this.ObterPaginacao != null)
                return this.ObterPaginacao(IdPesquisa, registroInicial, quantidadeRegistrosRetornar, quantidadeRegistrosPesquisar, out quantidadeTotalRegistrosEmCache, parametros);

            IEnumerable<object> retorno = new List<object>();
            if (DataSource != null && DataSource.Count() > 0)
            {
                int pagina = 1;
                if (registroInicial > 0)
                {
                    pagina = (registroInicial + quantidadeRegistrosRetornar) / quantidadeRegistrosRetornar;
                }

                var entidades = DataSource;

                retorno = entidades.Skip((pagina - 1) * quantidadeRegistrosRetornar).Take(quantidadeRegistrosRetornar);

                quantidadeTotalRegistrosEmCache = entidades.Count();
            }
            return retorno;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptSelecaoPvs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                bool chkTodos = Convert.ToBoolean(string.IsNullOrEmpty(hdnTodosSelecionados.Value) ? "false" : hdnTodosSelecionados.Value);

                if (e.Item.ItemType == ListItemType.Header)
                {
                    CheckBox chkCodigoTotos = (CheckBox)e.Item.FindControl("chkCodigoTotos");
                    Label lblTodos = (Label)e.Item.FindControl("lblTodos");
                    if (chkCodigoTotos != null)
                    {
                        chkCodigoTotos.Visible = !this.SingleSelect;
                    }
                }
                else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    var entidade = (EntidadeServico.EntidadeServicoModel)e.Item.DataItem;

                    CheckBox chkCodigo = (CheckBox)e.Item.FindControl("chkCodigo");
                    RadioButton rdoCodigo = (RadioButton)e.Item.FindControl("rdoCodigo");
                    Label lblCodigo = (Label)e.Item.FindControl("lblCodigo");
                    Label lblDescricao = (Label)e.Item.FindControl("lblDescricao");

                    lblCodigo.Text = Util.TruncarNumeroPV(entidade.NumeroPV.ToString());
                    lblDescricao.Text = entidade.RazaoSocial.Trim();
                    chkCodigo.Attributes.Add("data-value", entidade.NumeroPV.ToString());
                    chkCodigo.Attributes.Add("onclick", "chkCodigo_click(this);");
                    chkCodigo.Checked = chkTodos ? true : PvIsCheked(entidade.NumeroPV.ToString());

                    rdoCodigo.Attributes.Add("data-value", entidade.NumeroPV.ToString());
                    rdoCodigo.Attributes.Add("onclick", "rdoCodigo_click(this);");
                    rdoCodigo.Checked = PvIsCheked(entidade.NumeroPV.ToString());

                    if (this.SingleSelect)
                    {
                        rdoCodigo.Visible = true;
                        chkCodigo.Visible = false;
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }

        }


        /// <summary>
        /// Verifica se o Pv informado está na lsta de pvs selecionados
        /// </summary>
        /// <param name="pv">Pv</param>
        /// <returns></returns>
        private bool PvIsCheked(string pv)
        {
            List<Int32> pvsSelecionados = this.GetPvsSelecionados();

            if (pvsSelecionados != null && pvsSelecionados.Any())
                return pvsSelecionados.Any(x => x == Convert.ToInt32(pv));

            return false;
        }

        /// <summary>
        /// Retorna pvs selecinados na pagina
        /// </summary>
        /// <returns></returns>
        public List<Int32> GetPvsSelecionados()
        {
            bool chkTodos;
            List<int> pvs = new List<int>();

            chkTodos = Convert.ToBoolean(string.IsNullOrEmpty(hdnTodosSelecionados.Value) ? "false" : hdnTodosSelecionados.Value);

            //Pega os pvs selecionados
            if (!string.IsNullOrEmpty(hdnPvsSelecionados.Value))
            {
                pvs = hdnPvsSelecionados.Value.Split('|')
                                              .Select(x => Convert.ToInt32(x))
                                              .ToList();
            }

            // se os PVs não foram tratados em clientside...
            if (chkTodos && pvs.Count == 0 && 
                this.DataSource is EntidadeServico.EntidadeServicoModel[])
            {
                // carrega todos os PVs passados como datasource
                var pvsDataSource = (this.DataSource as EntidadeServico.EntidadeServicoModel[]);
                if (pvsDataSource != null)
                {
                    pvs = pvsDataSource.Select(x => x.NumeroPV).ToList();
                }
            }

            return pvs;
        }

        protected void btnBusca_Click(object sender, EventArgs e)
        {
            if (OnBuscarDados != null)
            {
                this.OnBuscarDados(sender, e);
            }
        }
        
    }
}
