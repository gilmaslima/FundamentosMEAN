using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Redecard.PN.Comum;

namespace Redecard.PN.DataCash.controles
{    
    [ParseChildren(true)]
    [PersistChildren(false)]
    public partial class HeaderDataCash : System.Web.UI.UserControl
    {
        private PassoCollection passos;

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public PassoCollection Passos 
        {
            get { return passos ?? (passos = new PassoCollection(this)); }
        }

        public Int32 PassoAtual
        {
            get { return Convert.ToString(ViewState["PassoAtual"]).ToInt32(0); }
            set { ViewState["PassoAtual"] = value; }
        }

        public String Titulo 
        {
            get { return lblTitulo.Text; }
            set { lblTitulo.Text = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.AtualizarPassos();
        }

        protected void rptPassos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var passo = e.Item.DataItem as Passo;
                var divPasso = e.Item.FindControl("divPasso") as HtmlGenericControl;
                var lblNomePasso = e.Item.FindControl("lblNomePasso") as Literal;

                lblNomePasso.Text = passo.Descricao;

                if (e.Item.ItemIndex < this.PassoAtual)
                    divPasso.Attributes["class"] = String.Concat("anterior", e.Item.ItemIndex + 1);
                else if (e.Item.ItemIndex == this.PassoAtual)
                    divPasso.Attributes["class"] = String.Concat("ativado", e.Item.ItemIndex + 1);
                else
                    divPasso.Attributes["class"] = String.Concat("proximo", e.Item.ItemIndex + 1);
            }
        }

        public void DefinirPassoAtual(Int32 passo)
        {
            if (passo >= 0 && passo < this.Passos.Count)
            {
                this.PassoAtual = passo;
                this.AtualizarPassos();
            }
        }

        public Int32 AvancarPasso()
        {
            this.DefinirPassoAtual(this.PassoAtual + 1);
            return this.PassoAtual;
        }

        public Int32 RetornarPasso()
        {
            this.DefinirPassoAtual(this.PassoAtual - 1);
            return this.PassoAtual;
        }

        private void AtualizarPassos()
        {
            rptPassos.DataSource = this.Passos.Cast<Passo>().OrderBy(passo => passo.Ordem).ToArray();
            rptPassos.DataBind();
        }
    }

    public class PassoCollection : ControlCollection
    { 
        public PassoCollection(Control owner) : base(owner) { }

        public override void Add(Control child)
        {
            if (child is HtmlGenericControl)
                base.Add(new Passo(child as HtmlGenericControl, this.Count));
        }
    }

    public class Passo : HtmlGenericControl
    {
        public String Descricao { get; set; }
        public Int32 Ordem { get; set; }

        public Passo(HtmlGenericControl genericControl, Int32 index) : base()
        {
            this.Descricao = genericControl.Attributes["Descricao"];
            this.Ordem = genericControl.Attributes["Ordem"].ToInt32(index);
        }

        public override String ToString()
        {
            return this.Descricao;
        }
    }
}