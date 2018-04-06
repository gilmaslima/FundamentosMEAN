using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;

namespace Rede.PN.AtendimentoDigital.SharePoint.WebParts.AtendimentoDigital.FaleConosco
{
    [ToolboxItemAttribute(false)]
    public partial class FaleConosco : WebPart
    {
        #region [ Propriedades ]


        /// <summary>
        /// Destinatário padrão do Fale Conosco
        /// </summary>
        public String DestinatarioPadrao { get { return "faleconosco@userede.com.br"; } }

        //// <summary>
        /// Sessão atual do usuário autenticado
        /// </summary>
        public Sessao SessaoAtual { get { return Sessao.Obtem(); } }

        /// <summary>
        /// Nome do usuário da sessão atual
        /// </summary>
        public String NomeEntidade { get { return Sessao.Contem() ? SessaoAtual.NomeEntidade : String.Empty; } }

        /// <summary>
        /// E-mail do usuário da sessão atual
        /// </summary>
        public String Email { get { return Sessao.Contem() ? SessaoAtual.Email : String.Empty; } }
        /// <summary>
        /// Numero do estabelecimento
        /// </summary>
        public String CodigoEntidade { get { return Sessao.Contem() ? SessaoAtual.CodigoEntidade.ToString() : String.Empty;} }

        /// <summary>
        /// Nome do usuário da sessão atual
        /// </summary>
        public String NomeUsuario { get { return Sessao.Contem() ? SessaoAtual.NomeUsuario : String.Empty;} }

        /// <summary>
        /// Celular do usuário da sessão atual
        /// </summary>
        public String Celular { get { return Sessao.Contem() ? ((SessaoAtual.DDDCelular.HasValue && SessaoAtual.Celular.HasValue) ? String.Format("({0}) {1}", SessaoAtual.DDDCelular.Value, SessaoAtual.Celular.Value) : String.Empty) : String.Empty; } }

        /// <summary>
        /// Código do Segmento do usuário atual
        /// </summary>
        public String Segmento { get { return Sessao.Contem() ? SessaoAtual.CodigoSegmento.ToString() : String.Empty; } }

        /// <summary>
        /// E-mail para atendimento diferenciado de acordo com o Segmento do usuário
        /// </summary>
        public String DestinatarioDiferenciado
        { 
            get 
            {
                switch (Segmento) 
                {
                    //Cliente Top Varejo
                    case "E":
                    case "S":
                        return "resolve@userede.com.br";
                    //Email padrão
                    default:
                        return DestinatarioPadrao;
                }
                
            } 
        }
        #endregion

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