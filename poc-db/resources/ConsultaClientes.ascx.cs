/*
(c) Copyright 2012 Rede S.A.
Autor       : Alexandre Shiroma
Empresa     : Iteris Consultoria e Software
Histórico   :
- [15/06/2015] – Alexandre Shiroma: Criação da WebPart
*/

using Redecard.Portal.Aberto.WebParts.EntidadeServico;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Linq;

namespace Redecard.Portal.Aberto.WebParts.ConsultaClientes
{
    /// <summary>
    /// Classe utilizada para montagem do formulário de verificação de clientes credenciados
    /// </summary>
    [ToolboxItemAttribute(false)]
    public partial class ConsultaClientes : WebPart
    {
        /// <summary>
        /// Construtor
        /// </summary>
        public ConsultaClientes()
        {
        }

        /// <summary>
        /// OnInit
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        /// <summary>
        /// Page Load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //Preenche o repeater com 10 repetições
                repCnpjCpf.DataSource = new Object[10];
                repCnpjCpf.DataBind();
            }
        }

        /// <summary>
        /// Executar a pesquisa dos CGCs/CPFs
        /// </summary>
        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            var cgcCnpjs = new List<Int64>();
            var codigoRetorno = default(Int32);
            var status = default(Dictionary<Int64, String>);

            //Recupera os valores dos CNPJs
            foreach (RepeaterItem item in repCnpjCpf.Items)
            {
                var txtCnpjCpf = item.FindControl("txtCnpjCpf") as TextBox;
                var cnpjCpf = default(Int64);
                if (Int64.TryParse(txtCnpjCpf.Text, out cnpjCpf))
                    cgcCnpjs.Add(cnpjCpf);
            }

            //Chama serviço responsável por verificação dos CNPJs/CPFs
            using (EntidadeServicoClient client = new EntidadeServicoClient())
                status = client.ListarEstabelecimentosFiliados(cgcCnpjs, out codigoRetorno);
            
            //Exibe os dados em caso de sucesso
            if(codigoRetorno == 0)
                ExibirDadosEstabelecimentos(status);   
        }

        /// <summary>
        /// Exibe os dados dos estabelecimentos encontrados
        /// </summary>
        /// <param name="status">Status de cada CGC/CPF</param>
        private void ExibirDadosEstabelecimentos(Dictionary<Int64, String> status)
        {
            //Ativa a View
            mvwConsultaClientes.SetActiveView(vwResposta);

            //Exibe os dados no Repeater
            repResultado.DataSource = status.ToArray();
            repResultado.DataBind();
        }

        /// <summary>
        /// Repeater ItemDataBound
        /// </summary>
        protected void repResultado_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                //Recupera os controles e o item atual
                var item = (KeyValuePair<Int64, String>)e.Item.DataItem;
                var ltrCnpj = e.Item.FindControl("ltrCnpj") as Literal;
                var ltrStatus = e.Item.FindControl("ltrStatus") as Literal;

                //Exibição do resultado da consulta dos CNPJs/CPFs
                ltrCnpj.Text = item.Key.ToString();
                switch (item.Value)
                {
                    case "C": 
                        ltrStatus.Text = "Não - Cancelado";
                        break;

                    case "S": 
                        ltrStatus.Text = "Sim";
                        break;

                    case "N": 
                        ltrStatus.Text = "Não";
                        break;

                    default:
                        ltrStatus.Text = item.Value;
                        break;
                }
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                //Define o botão Enviar 
                var btnEnviar = e.Item.FindControl("btnEnviar") as LinkButton;
                if (btnEnviar != null)
                {
                    this.Page.Form.DefaultButton = btnEnviar.UniqueID;
                }
            }
        }
    }
}