using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;

namespace Redecard.Portal.Fechado.WebParts.RedecardMeusFavoritos
{
    [ToolboxItemAttribute(false)]
    public class RedecardMeusFavoritos : Microsoft.SharePoint.WebPartPages.WebPart
    {        
        #region Constantes____________________

        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Fechado.WebParts/RedecardMeusFavoritos/RedecardMeusFavoritosUserControl.ascx";

        #endregion

        #region Variáveis_____________________

        private string urlLink1 = string.Empty;
        private string urlLink2 = string.Empty;
        private string urlLink3 = string.Empty;
        private string urlLink4 = string.Empty;
        private string urlLink5 = string.Empty;

        #endregion

        #region Propriedades__________________

        /*public string URLLink1
        {
            get { return this.urlLink1; }
            set { this.urlLink1 = value; }
        }

        public string URLLink2
        {
            get { return this.urlLink2; }
            set { this.urlLink2 = value; }
        }

        public string URLLink3
        {
            get { return this.urlLink3; }
            set { this.urlLink3 = value; }
        }

        public string URLLink4
        {
            get { return this.urlLink4; }
            set { this.urlLink4 = value; }
        }

        public string URLLink5
        {
            get { return this.urlLink5; }
            set { this.urlLink5 = value; }
        }*/

        #endregion

        #region Eventos_______________________

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        public override ToolPart[] GetToolParts()
        {
            ToolPart[] allToolParts = new ToolPart[3];

            allToolParts[0] = new WebPartToolPart();
            allToolParts[1] = new CustomPropertyToolPart();
            allToolParts[2] = new RedecardMeusFavoritosToolPart();

            return allToolParts;
        }


        #endregion
    }

    public class RedecardMeusFavoritosToolPart : Microsoft.SharePoint.WebPartPages.ToolPart
    {

        #region Variáveis_____________________

        private LiteralControl ltlQuebra;

        private TextBox txtLink1;
        private TextBox txtLink2;
        private TextBox txtLink3;
        private TextBox txtLink4;
        private TextBox txtLink5;

        private Label labelInstrucao;

        private Label lblLink1;
        private Label lblLink2;
        private Label lblLink3;
        private Label lblLink4;
        private Label lblLink5;

        #endregion

        private RedecardMeusFavoritos WebPart
        {
            get
            {
                return this.ParentToolPane.SelectedWebPart as RedecardMeusFavoritos;
            }
        }

        public RedecardMeusFavoritosToolPart()
        {
        }

        protected override void OnInit(EventArgs e)
        {            
            this.Title = "Gerenciar Meus Links Favoritos";            
            
            base.OnInit(e);
        }
        
        public override void ApplyChanges()
        {
            /*
            //this.WebPart.URLLink1 = this.txtLink1.Text;
            //this.WebPart.URLLink2 = this.txtLink2.Text;
            //this.WebPart.URLLink3= this.txtLink3.Text;
            //this.WebPart.URLLink4= this.txtLink4.Text;
            //this.WebPart.URLLink5 = this.txtLink5.Text;            
             */ 
        }

        protected override void CreateChildControls()
        {
            /*labelInstrucao = new Label();
            labelInstrucao.Text = "Preencha os campos abaixo com o Nome do Link e URL no formato: <strong>Nome;URL</strong>. <br /><br /> Exemplo: <br /> Link Redecard;http://wwww.redecard.com.br <br />";            

            lblLink1 = new Label();
            lblLink1.Text = "Informe o 1º Nome e Link: <br />";

            lblLink2 = new Label();
            lblLink2.Text = "Informe o 2º Nome e Link: <br />";

            lblLink3 = new Label();
            lblLink3.Text = "Informe o 3º Nome e Link: <br />";

            lblLink4 = new Label();
            lblLink4.Text = "Informe o 4º Nome e Link: <br />";

            lblLink5 = new Label();
            lblLink5.Text = "Informe o 5º Nome e Link: <br />";

            txtLink1 = new TextBox();
            txtLink1.Width = Unit.Pixel(300);
            
            txtLink1.Text = this.WebPart.URLLink1;

            txtLink2 = new TextBox();
            txtLink2.Width = Unit.Pixel(300);
            txtLink2.Text = this.WebPart.URLLink2;

            txtLink3 = new TextBox();
            txtLink3.Width = Unit.Pixel(300);
            txtLink3.Text = this.WebPart.URLLink3;
            
            txtLink4 = new TextBox();
            txtLink4.Width = Unit.Pixel(300);
            txtLink4.Text = this.WebPart.URLLink4;
            
            txtLink5 = new TextBox();
            txtLink5.Width = Unit.Pixel(300);
            txtLink5.Text = this.WebPart.URLLink5;

            //Monta Tela (sequencia de controles)
            Controls.Add(labelInstrucao);

            Controls.Add(ltlQuebra = new LiteralControl() { Text = "<br />" });

            Controls.Add(lblLink1);            
            Controls.Add(txtLink1);

            Controls.Add(ltlQuebra = new LiteralControl() { Text = "<br /><br />" });

            Controls.Add(lblLink2);           
            Controls.Add(txtLink2);

            Controls.Add(ltlQuebra = new LiteralControl() { Text = "<br /><br />" });

            Controls.Add(lblLink3);            
            Controls.Add(txtLink3);

            Controls.Add(ltlQuebra = new LiteralControl() { Text = "<br /><br />" });

            Controls.Add(lblLink4);
            Controls.Add(txtLink4);

            Controls.Add(ltlQuebra = new LiteralControl() { Text = "<br /><br />" });

            Controls.Add(lblLink5);            
            Controls.Add(txtLink5);

            Controls.Add(ltlQuebra = new LiteralControl() { Text = "<br /><br />" });

            base.CreateChildControls();*/
        }
    }

}
