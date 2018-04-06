using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

namespace Redecard.PN.DataCash.controles
{
    public partial class ControleEndereco : System.Web.UI.UserControl
    {
        #region Propriedades

        Boolean _exibirPais = true;
        public Boolean ExibirPais
        {
            get
            {
                return _exibirPais;
            }
            set
            {
                _exibirPais = value;
            }
        }

        Boolean _camposRequeridos = true;
        public Boolean CamposRequeridos
        {
            get 
            {
                return _camposRequeridos;
            }
            set
            {
                _camposRequeridos = value;
            }
        }

        public String Asterisco
        {
            get;
            set;
        }

        public String EstadoSelecionado
        {
            get
            {
                if (Session[ddlEstado.ClientID] == null)
                    Session[ddlEstado.ClientID] = "";

                return Convert.ToString(Session[ddlEstado.ClientID]);
            }
            set
            {
                if (value == "")
                    Session.Remove(ddlEstado.ClientID);
                else
                    Session[ddlEstado.ClientID] = value;
            }
        }

        public String PaisSelecionado
        {
            get
            {
                if (Session["PaisSelecionado"] == null)
                    Session["PaisSelecionado"] = "";

                return Convert.ToString(Session["PaisSelecionado"]);
            }
            set
            {
                if (value == "")
                    Session.Remove("PaisSelecionado");
                else
                    Session["PaisSelecionado"] = value;
            }
        }

        String _nomeSessao = "";
        public String NomeSessao
        {
            get
            {
                return _nomeSessao;
            }
            set
            {
                _nomeSessao = value;
            }
        }

        public String CidadeSelecionada
        {
            get
            {
                if (HttpContext.Current.Session[ddlCidade.ClientID] == null)
                    HttpContext.Current.Session[ddlCidade.ClientID] = "";

                return Convert.ToString(HttpContext.Current.Session[ddlCidade.ClientID]);
            }
        }

        #endregion

        public enum ETipoGrupoEndereco
        {
            Geral = 0,
            Entrega = 1,
            Cobranca = 2,
            EnderecoCliente = 3
        }

        public Modelo.Endereco ObterEndereco()
        {
            Modelo.Endereco endereco = new Modelo.Endereco()
            {
                CEP = txtCEP.Text,
                Logradouro = txtEndereco.Text,
                Numero = txtNumero.Text,
                Complemento = txtComplemento.Text,
                Cidade = CidadeSelecionada,
                Estado = ddlEstado.SelectedValue,
                Pais = (this.TipoGrupoEndereco == ETipoGrupoEndereco.Geral
                || this.TipoGrupoEndereco == ETipoGrupoEndereco.EnderecoCliente) ? "BR" : ddlPais.SelectedValue
            };

            return endereco;
        }
        public Modelo.Endereco ObterEndereco(Boolean requerInstalacao)
        {
            Modelo.Endereco endereco = null;
            if (requerInstalacao)
            {
                endereco = new Modelo.Endereco()
                {
                    CEP = txtCEP.Text,
                    Logradouro = txtEndereco.Text,
                    Numero = txtNumero.Text,
                    Complemento = txtComplemento.Text,
                    Cidade = CidadeSelecionada,
                    Estado = ddlEstado.SelectedValue,
                    Pais = (this.TipoGrupoEndereco == ETipoGrupoEndereco.Geral
                    || this.TipoGrupoEndereco == ETipoGrupoEndereco.EnderecoCliente) ? "BR" : ddlPais.SelectedValue
                };
            }
            else
            {
                endereco = new Modelo.Endereco()
                {
                    CEP = "",
                    Logradouro = "",
                    Numero = "",
                    Complemento = "",
                    Cidade = "",
                    Estado = "",
                    Pais = ""
                };
            }

            return endereco;
        }

        public void CarregarEndereco(Modelo.Endereco endereco)
        {
            if (endereco != null)
            {
                txtCEP.Text = endereco.CEP;
                txtEndereco.Text = endereco.Logradouro;
                txtNumero.Text = endereco.Numero;
                txtComplemento.Text = endereco.Complemento;
                //CidadeSelecionada = endereco.Cidade;
                this.EstadoSelecionado = endereco.Estado;
                this.PaisSelecionado = endereco.Pais;
            }
        }

        public ETipoGrupoEndereco TipoGrupoEndereco { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetarValidationGroup();
                VerificaTipoEndereco();
                CarregaPais();
                CarregarEstado();
                SetarCamposRequeridos();

                trPais.Attributes.Add("class", this.NomeSessao);
            }
            //ddlEstado.SelectedIndexChanged += new EventHandler(ddlEstado_SelectedIndexChanged);
        }

        private void SetarCamposRequeridos()
        {
            Asterisco = "*";
            if (CamposRequeridos)
                return;

            Asterisco = "";
            List<RequiredFieldValidator> validators = new List<RequiredFieldValidator>();
            GetControlList<RequiredFieldValidator>(this.Controls, validators);
            
            foreach (var validator in validators)
            {
                validator.Enabled = false;
            }
        }

        private void GetControlList<T>(ControlCollection controlCollection, List<T> resultCollection) where T : Control
        {
            foreach (Control control in controlCollection)
            {
                if (control is T)
                    resultCollection.Add((T)control);

                if (control.HasControls())
                    GetControlList(control.Controls, resultCollection);
            }
        }

        void VerificaTipoEndereco()
        {
            trPais.Visible = (TipoGrupoEndereco != ETipoGrupoEndereco.Geral);
        }

        protected void ddlEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregaCidade(ddlEstado.SelectedValue);
        }

        private void SetarValidationGroup()
        {
            String validationGroup = string.Empty;
            String classeCss = string.Empty;
            switch (TipoGrupoEndereco)
            {
                case ETipoGrupoEndereco.Geral:
                    validationGroup = "vlgDadosGerais";
                    classeCss = " vldDadosGerais";
                    break;
                case ETipoGrupoEndereco.Entrega:
                    validationGroup = "vlgDadosEntrega";
                    classeCss = " vldDadosEntrega";
                    break;
                case ETipoGrupoEndereco.Cobranca:
                    validationGroup = "vlgDadosCobranca";
                    classeCss = " vldDadosCobranca";
                    break;
                case ETipoGrupoEndereco.EnderecoCliente:
                    validationGroup = "vlgEnderecoCliente";
                    classeCss = " vldEnderecoCliente";
                    break;
            }

            rfvCEP.ValidationGroup = validationGroup;
            revCEP.ValidationGroup = validationGroup;
            rfvEndereco.ValidationGroup = validationGroup;
            rfvEstado.ValidationGroup = validationGroup;
            rfvCidade.ValidationGroup = validationGroup;
            rfvNumero.ValidationGroup = validationGroup;

            rfvCEP.CssClass += classeCss;
            revCEP.CssClass += classeCss;
            rfvEndereco.CssClass += classeCss;
            rfvEstado.CssClass += classeCss;
            rfvCidade.CssClass += classeCss;
            rfvNumero.CssClass += classeCss;

        }

        void CarregarEstado()
        {
            ddlEstado.DataSource = ProcessaListas.ObterEstados();
            ddlEstado.DataTextField = "Text";
            ddlEstado.DataValueField = "Value";
            ddlEstado.DataBind();
            ddlEstado.SelectedValue = this.EstadoSelecionado == "" ? "SP" : this.EstadoSelecionado;
            CarregaCidade(ddlEstado.SelectedValue);
            
        }
        void CarregaCidade(String estado)
        {

            ddlCidade.DataSource = ProcessaListas.ObterCidadesEstado(estado);
            ddlCidade.DataTextField = "Text";
            ddlCidade.DataValueField = "Value";
            ddlCidade.DataBind();
            ddlCidade.SelectedValue = this.CidadeSelecionada == "" ? "SP" : this.CidadeSelecionada;

        }
        void CarregaPais()
        {
            trPais.Visible = this.ExibirPais;

            if (trPais.Visible)
            {
                ddlPais.DataSource = ProcessaListas.ObterPaises();
                ddlPais.DataTextField = "Text";
                ddlPais.DataValueField = "Value";
                ddlPais.DataBind();
            }
            else
            {
                ddlPais.Items.Add(new ListItem("Brasil", "BR"));
            }
            ddlPais.SelectedValue = this.PaisSelecionado == "" ? "BR" : this.PaisSelecionado;
        }
    }
}