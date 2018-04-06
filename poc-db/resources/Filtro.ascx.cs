/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.ConsultaTransacao
{
    public partial class Filtro : UserControlBase
    {
        public delegate void ItemSelecionadoEventHandler(TransacaoDadosConsultaDTO dadosConsultaDTO, EventArgs e);

        [Browsable(true)]
        public event ItemSelecionadoEventHandler ItemSelecionado;


        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.DefaultButton = btnEnviar.UniqueID;
        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            using (Logger.IniciarLog("Consulta por Transação - Filtro"))
            {
                this.lblValidacaoPV.Text = "";
                this.lblValidacaoID.Visible = false;
                this.lblValidacaoDatas.Visible = false;
                this.lblValidacaoTipoVenda.Visible = false;

                ConsultaPV consultaPV = ucConsultaPV2 as ConsultaPV;

                //Obtém e valida número do estabelecimento
                List<Int32> pvsSelecionados = consultaPV.PVsSelecionados;
                Int32? numeroEstabelecimento = pvsSelecionados.FirstOrDefault();

                if (pvsSelecionados.Count == 0 || numeroEstabelecimento.Value == 0)
                {
                    lblValidacaoPV.Text = "Favor selecionar um Ponto de Venda";
                    return;
                }

                //Valida o tipo de venda            
                if (!ValidarTipoVenda())
                    return;
                
                //Valida cartão/NSU/TID
                if (!ValidarID())
                    return;

                //Valida data e período            
                if (!ValidarDatas())
                    return;

                TransacaoDadosConsultaDTO dadosConsultaDTO = new TransacaoDadosConsultaDTO();
                dadosConsultaDTO.DataInicial = txtDataInicial.Text.ToDate("dd/MM/yyyy");
                dadosConsultaDTO.DataFinal = txtDataFinal.Text.ToDate("dd/MM/yyyy");
                if ("C".Equals(rbCartaoNSU.SelectedValue))
                    dadosConsultaDTO.NumeroCartao = txtID.Text;
                else if ("N".Equals(rbCartaoNSU.SelectedValue))
                    dadosConsultaDTO.Nsu = txtID.Text.ToInt32(0);
                else if ("T".Equals(rbCartaoNSU.SelectedValue))
                    dadosConsultaDTO.TID = txtID.Text;
                dadosConsultaDTO.NumeroEstabelecimento = numeroEstabelecimento.Value;
                dadosConsultaDTO.TipoVenda = ddlTipoVenda.SelectedValue;

                if (this.ItemSelecionado != null)
                    this.ItemSelecionado(dadosConsultaDTO, e);
            }
        }

        #region [ Validações e Conversões ]        

        private Boolean ValidarTipoVenda()
        {
            String msgValidacao = null;
            if (String.IsNullOrEmpty(ddlTipoVenda.SelectedValue))
                msgValidacao = "Indique se a transação é de crédito ou débito.";

            Boolean validacaoOK = msgValidacao == null;
            lblValidacaoTipoVenda.Text = msgValidacao;
            lblValidacaoTipoVenda.Visible = !validacaoOK;
            return validacaoOK;
        }

        private Boolean ValidarID()
        {
            String msgValidacao = null;

            if (String.IsNullOrEmpty(txtID.Text.Trim()))
            {
                if ("N".Equals(rbCartaoNSU.SelectedValue))
                    msgValidacao = "NSU obrigatório.";
                else if ("C".Equals(rbCartaoNSU.SelectedValue))
                    msgValidacao = "Número do Cartão obrigatório.";
                else if ("T".Equals(rbCartaoNSU.SelectedValue))
                    msgValidacao = "Número TID obrigatório.";
                else
                    msgValidacao = "Cartão / NSU / TID obrigatório.";
            }

            Boolean validacaoOK = msgValidacao == null;
            lblValidacaoID.Text = msgValidacao;
            lblValidacaoID.Visible = !validacaoOK;
            return validacaoOK;
        }

        private Boolean ValidarDatas()
        {            
            String msgValidacao = null;

            //para TID, não há validação de data (não é parâmetro de consulta)
            if (!"T".Equals(rbCartaoNSU.SelectedValue, StringComparison.InvariantCultureIgnoreCase))
            {
                DateTime? dataInicial = this.ParseDateNull(txtDataInicial.Text, "dd/MM/yyyy");
                DateTime? dataFinal = this.ParseDateNull(txtDataFinal.Text, "dd/MM/yyyy");

                //Validação da data inicial
                if (String.IsNullOrEmpty(txtDataInicial.Text.Trim()))
                    msgValidacao = "Data Inicial é Campo Obrigatório.";
                else if (!dataInicial.HasValue)
                    msgValidacao = "Data Inicial não é uma data válida.";

                //Validação da data final
                else if (String.IsNullOrEmpty(txtDataFinal.Text.Trim()))
                    msgValidacao = "Data Final é Campo Obrigatório.";
                else if (!dataFinal.HasValue)
                    msgValidacao = "Data Final não é uma data válida.";

                //Validação do período
                else if (dataInicial.Value > dataFinal.Value)
                    msgValidacao = "Data Inicial não pode ser maior que a data final.";
                else if (dataFinal.Value.Subtract(dataInicial.Value).TotalDays > 30)
                    msgValidacao = "Período máximo de pesquisa é de 1 mês.";

                //Tipo de Venda: Débito selecionado
                else if ("D".Equals(ddlTipoVenda.SelectedValue) && dataFinal.Value.Subtract(dataInicial.Value).TotalDays > 180)
                    msgValidacao = "Período máximo de pesquisa para venda de débito é de 6 meses.";
            }

            Boolean validacaoOK = msgValidacao == null;
            lblValidacaoDatas.Text = msgValidacao;
            lblValidacaoDatas.Visible = !validacaoOK;
            return validacaoOK;
        }

        private DateTime? ParseDateNull(object value, string format)
        {
            DateTime date;
            var provider = CultureInfo.InvariantCulture;
            if (DateTime.TryParseExact(value.ToString(), format, provider, System.Globalization.DateTimeStyles.None, out date))
                return date;
            else return null;
        }
        #endregion
    }
}
