using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.Script.Serialization;
using Redecard.PN.Comum.SharePoint.EntidadeServico;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.ComponentModel;

namespace Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum
{
    public partial class ConsultaPV : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            this.ValidarPermissao = false;
            base.OnLoad(e);
        }

        #region [ Enumeradores da Classe ]

        public enum ETipoAssociacao
        {
            [Description("Próprio")]
            Proprio = 0,            
            [Description("Centralizados")]
            Centralizados = 1,
            [Description("Filiais")]
            Filiais = 2,
            [Description("Consignados")]
            Consignados = 3,
            [Description("Mesmo CNPJ")]
            MesmoCNPJ = 4
        }

        [Flags]
        public enum ETipoEntidade
        {
            [Description("0")]
            Proprio = 1,            
            [Description("1")]
            Centralizados = 2,
            [Description("2")]
            Filiais = 4,
            [Description("3")]
            Consignados = 8,
            [Description("4")]
            MesmoCNPJ = 16
        }

        public enum Modo
        {
            Unico = 1,
            Multiplo = 2
        }

        public enum OrigemSolicitacao
        {
            FiltroRelatorios = 1,
            GerencieExtratoInibicao = 2
        }

        public ETipoAssociacao TipoAssociacao
        {
            get
            {
                return (ETipoAssociacao)ddlTipoAssociacao.SelectedValue.ToInt32(0);
            }
        }

        #endregion

        #region [ Classes auxiliares para De-serialização ]

        public class FilialConsulta
        {
            public String Categoria { get; set; }
            public String Centralizador { get; set; }
            public Int32 Matriz { get; set; }
            public String Moeda { get; set; }
            public String NomeComerc { get; set; }
            public Int32 PontoVenda { get; set; }
            public Int32 TipoEstab { get; set; }
            public String Chave { get; set; }
        }

        #endregion

        #region [ Privados ]

        private static JavaScriptSerializer _jsSerializer;
        private static JavaScriptSerializer JsSerializer
        {
            get
            {
                if (_jsSerializer == null)
                    _jsSerializer = new JavaScriptSerializer();
                return _jsSerializer;
            }
        }
        
        #endregion

        #region [ Propriedades Públicas ]
        
        public List<Int32> PVsSelecionados
        {
            get
            {
                return hdnPVsSelecionados.Value.ToString()
                    .Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries)                    
                    .Select(pvCripto => Criptografia.DescriptografarPV(pvCripto, SessaoAtual.CodigoEntidade))
                    .ToList().Distinct().ToList();
            }

            set
            {
                hdnPVsSelecionados.Value = String.Join(";", value.Select(pv => Criptografia.CriptografarPV(pv, SessaoAtual.CodigoEntidade)).ToArray());
            }
        }
          
        public String OnClientSelectionChanged
        {
            get { return hdnOnClientSelectionChanged.Value.ToString(); }
            set { hdnOnClientSelectionChanged.Value = value; }
        }

        public String OnClientPopupClosed
        {
            get { return hdnOnClientPopupClosed.Value.ToString(); }
            set { hdnOnClientPopupClosed.Value = value; }
        }

        public String OnNoDataFound
        {
            get { return hdnOnNoDataFound.Value.ToString(); }
            set { hdnOnNoDataFound.Value = value; }
        }

        public OrigemSolicitacao Origem { 
            get
            {
                String origem = hdnOrigem.Value.ToString();
                if (!Enum.IsDefined(typeof(OrigemSolicitacao), origem))
                    return OrigemSolicitacao.FiltroRelatorios;
                else
                    return (OrigemSolicitacao)Enum.Parse(typeof(OrigemSolicitacao), origem);
            }
            set { hdnOrigem.Value = value.ToString(); }
        }

        public Modo ModoSelecao
        {
            get
            {
                String modo = hdnModo.Value.ToString();
                if (!Enum.IsDefined(typeof(Modo), modo))
                    return Modo.Multiplo;
                else
                    return (Modo)Enum.Parse(typeof(Modo), modo);
            }
            set { hdnModo.Value = value.ToString(); }
        }

        public DropDownList DropDownList { get { return ddlTipoAssociacao; } }

        public Unit Width 
        {
            get 
            {
                return ddlTipoAssociacao.Width;
            }
            set
            {
                ddlTipoAssociacao.Width = value;
            }
        }

        public Boolean UseSubmitBehavior
        {
            get { return btnSelecionar.UseSubmitBehavior; }
            set { btnSelecionar.UseSubmitBehavior = value; }
        }

        public Boolean PVObrigatorio
        {
            get
            {
                String pvObrigatorio = hdnPVObrigatorio.Value.ToString();
                return pvObrigatorio.ToLower() != "false";
            }
            set { hdnPVObrigatorio.Value = value.ToString().ToLower(); }
        }
       
        public String ButtonText
        {
            get { return btnSelecionar.Text; }
            set { btnSelecionar.Text = value; }
        }

        public event EventHandler Click;
                        
        public ETipoEntidade? TiposEntidade
        {
            set { ViewState["TiposEntidade"] = value; }
            get { return (ETipoEntidade?)ViewState["TiposEntidade"]; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!object.ReferenceEquals(this.SessaoAtual, null))
                {
                    //Serializa o objeto de sessão em JSON, para utilização pelo JS do controle
                    var sessao = new
                    {
                        CNPJEntidade = this.SessaoAtual.CNPJEntidade,
                        CodigoEntidade = this.SessaoAtual.CodigoEntidade,
                        CodigoEntidadeMatriz = this.SessaoAtual.CodigoEntidadeMatriz,
                        GrupoEntidade = this.SessaoAtual.GrupoEntidade,
                        NomeEntidade = this.SessaoAtual.NomeEntidade,
                        NomeEntidadeMatriz = this.SessaoAtual.NomeEntidadeMatriz,
                        MoedaEntidade = this.SessaoAtual.TransacionaDolar ? "D" : "R",
                        Chave = Criptografia.CriptografarPV(this.SessaoAtual.CodigoEntidade, this.SessaoAtual.CodigoEntidade)
                    };

                    hdnSessao.Value = JsSerializer.Serialize(sessao);
                    hdnModo.Value = ModoSelecao.ToString();
                    hdnOrigem.Value = Origem.ToString();
                    hdnPVObrigatorio.Value = PVObrigatorio.ToString().ToLower();
                    this.UseSubmitBehavior &= Click != null;
                }

                //Se modo seleção é Único, oculta checkboxes de Todos/Ativos/Cancelados
                if (ModoSelecao == Modo.Unico)
                {
                    chkAtivos.Visible = false;
                    chkCancelados.Visible = false;
                    chkTodos.Visible = false;
                }

                //Se foi definida uma coleção de opções de Tipos de Entidades, aplica coleção definida na combo
                if(this.TiposEntidade.HasValue)
                {                    
                    foreach (ETipoEntidade tipoEntidade in Enum.GetValues(typeof(ETipoEntidade)))
                    {
                        if (tipoEntidade != (tipoEntidade & this.TiposEntidade))
                        {
                            //mapeia enumerador de tipoEntidade para enum de ETipoAssociacao
                            ETipoAssociacao tipoAssociacao = (ETipoAssociacao)tipoEntidade.GetDescription().ToInt32(0);

                            //Desabilita aqueles que não estão na coleção
                            this.DropDownList.Items.Cast<ListItem>().Single(
                                item => item.Value.CompareTo(((Int32)tipoAssociacao).ToString()) == 0).Enabled = false;
                        }
                    }
                }
            }

            ScriptManager.RegisterStartupScript(this, Page.GetType(), "consultaPV_init_" + this.ClientID, 
                "try { " +
                "   var " + this.ID + " = ConsultaPV.get('#" + this.ClientID + "'); " +
                "   var " + this.ClientID + " = ConsultaPV.get('#" + this.ClientID + "'); " +
                    this.ClientID + ".alterouTipoAssociacao(false, false); " +                
                "} catch(e) { }", true);            
        }

        protected void btnSelecionar_Click(object sender, EventArgs e)
        {
            if (this.Click != null)
                this.Click(sender, e);
        }
    }
}
