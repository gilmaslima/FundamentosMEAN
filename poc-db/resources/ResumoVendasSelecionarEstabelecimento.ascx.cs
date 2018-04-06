using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using System.Linq;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato.ResumoVendas
{
    public partial class ResumoVendasSelecionarEstabelecimento : UserControlBase
    {        
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Form.DefaultButton = this.btnEnviar.UniqueID;
        }

        /// <summary>
        /// Definição dos parâmetros do evento onItemSelecionado, ocorre quando o usuário
        /// acessa um item de Resumo de Venda
        /// </summary>
        /// <param name="dadosConsultaDTO"></param>
        /// <param name="e"></param>
        public delegate void ItemSelecionado(ResumoVendaDadosConsultaDTO dadosConsultaDTO, EventArgs e);

        /// <summary>
        /// Disparado quando ocorrer a seleção de um item de resumo de vendas
        /// </summary>
        [Browsable(true)]
        public event ItemSelecionado onItemSelecionado;
                                           
        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            lblValidacaoNumeroRV.Text = "";
            lblValidacaoDataApresentacao.Visible = false;
            lblValidacaoPV.Visible = false;

            ConsultaPV consultaPV = ucConsultaPV as ConsultaPV;

            Boolean succeeded;
            Int32? numeroEstabelecimento = consultaPV.PVsSelecionados.FirstOrDefault();
                                               
            if (!numeroEstabelecimento.HasValue || numeroEstabelecimento == 0)
            {
                lblValidacaoPV.Text = "Favor selecionar um Ponto de Venda";                
                return;
            }

            Int32 numeroResumoVenda;
            succeeded = Int32.TryParse(txtNumeroResumoVenda.Text, out numeroResumoVenda);
            if (!succeeded)
            {
                lblValidacaoNumeroRV.Visible = true;
                lblValidacaoNumeroRV.Text = "Favor Informar o Número do Resumo de Venda";
                return;
            }

            DateTime dataApresentacao;

            succeeded = DateTime.TryParseExact(txtDataApresentacao.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataApresentacao);
            if (!succeeded)
            {
                lblValidacaoDataApresentacao.Visible = true;
                lblValidacaoDataApresentacao.Text = "Favor Informar uma Data de Apresentação válida";                
                return;
            }

            ResumoVendaDadosConsultaDTO dadosConsultaDTO = new ResumoVendaDadosConsultaDTO();
            dadosConsultaDTO.NumeroEstabelecimento = numeroEstabelecimento.Value;
            dadosConsultaDTO.NumeroResumoVenda = numeroResumoVenda;
            dadosConsultaDTO.DataApresentacao = dataApresentacao;
            
            if (!object.ReferenceEquals(this.onItemSelecionado, null))
                this.onItemSelecionado(dadosConsultaDTO, e);
        }

        /// <summary>
        /// Limpar os campos do controle
        /// </summary>
        private void Limpar()
        {
            //TODO
            trErro.Visible = false;            
        }
    }
}
