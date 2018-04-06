#region Histórico do Arquivo
/*
(c) Copyright [2015] Redecard S.A.
Autor       : [Raphael Ivo]
Empresa     : [Iteris]
Histórico   :
    [12/02/2016] – [Raphael Ivo] – [Criação]
*/
#endregion

using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.HistoricoAtividadeServico;
using Redecard.PN.Sustentacao.SharePoint.SustentacaoAdministracaoServico;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;


namespace Redecard.PN.Sustentacao.SharePoint.CONTROLTEMPLATES.sustentacao
{
    public partial class ConsultaCancelamento : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtData.Text = DateTime.Today.ToString("dd/MM/yyyy");
            }
        }

        /// <summary>
        /// Evento de click do botão da tela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            ConsultarCancelamento();
        }

        /// <summary>
        /// Método central da tela, gerencia os outros métodos
        /// </summary>
        private void ConsultarCancelamento()
        {
            try
            {
                RetornoCancelamento retornoCancelamento = PopularRetornoCancelamento();

                List<RetornoCancelamento> listRetornoCancelamento = RelatorioCancelamento(retornoCancelamento);

                if (listRetornoCancelamento.Count > 0)
                {
                    lblResult.Text = "O relatório foi gerado com sucesso.";
                    DataTable tbListRetornoCancelamento = ToDataTable(listRetornoCancelamento);
                    SalvarArquivo(listRetornoCancelamento);
                }
                else
                {
                    lblResult.Text = "O relatório não retornou dados.";
                }
            }
            catch (Exception ex)
            {
                lblResult.Text = "Erro ao consultar relatório.";
            }
        }

        /// <summary>
        /// Retorna o objeto RetornoCancelamento populado com os campos da tela
        /// </summary>
        /// <returns></returns>
        private RetornoCancelamento PopularRetornoCancelamento()
        {
            RetornoCancelamento retornoCancelamento = new RetornoCancelamento();

            DateTime dataConsulta = Convert.ToDateTime(txtData.Text);
            int? idPv = null;
            int? idPvAcesso = null;
            string email = txtEmail.Text;
            string ip = txtIp.Text;

            if (!String.IsNullOrEmpty(txtPv.Text))
                idPv = Convert.ToInt32(txtPv.Text);

            if (!String.IsNullOrEmpty(txtPvAcesso.Text))
                idPvAcesso = Convert.ToInt32(txtPvAcesso.Text);

            retornoCancelamento.DataInclusao = dataConsulta;
            retornoCancelamento.CodigoPv = idPv;
            retornoCancelamento.CodigoPvAcesso = idPvAcesso;
            retornoCancelamento.Email = email;
            retornoCancelamento.Ip = ip;

            return retornoCancelamento;
        }

        /// <summary>
        /// Retorna o List<RetornoCancelamento> com os dados fornecidos
        /// </summary>
        /// <param name="retornoCancelamento"></param>
        /// <returns></returns>
        private List<RetornoCancelamento> RelatorioCancelamento(RetornoCancelamento retornoCancelamento)
        {
            using (SustentacaoAdministracaoServicoClient client = new SustentacaoAdministracaoServicoClient())
            {
                List<RetornoCancelamento> listRetornoCancelamento = new List<RetornoCancelamento>();
                RetornoCancelamento[] listaRetornoCancelamento = client.ConsultarRetornoCancelamento(retornoCancelamento.DataInclusao, retornoCancelamento.CodigoPv, retornoCancelamento.CodigoPvAcesso, retornoCancelamento.Email, retornoCancelamento.Ip);

                listRetornoCancelamento = listaRetornoCancelamento.ToList();

                return listRetornoCancelamento;
            }
        }

        /// <summary>
        /// Transforma a lista List<RetornoCancelamento> em DataTable
        /// </summary>
        /// <param name="listRetornoCancelamento"></param>
        /// <returns></returns>
        private DataTable ToDataTable(List<RetornoCancelamento> listRetornoCancelamento)
        {
            DataTable dt = new DataTable("Relatório Cancelamento");
            Type t = typeof(RetornoCancelamento);
            PropertyInfo[] pia = t.GetProperties();

            //Inspect the properties and create the columns in the DataTable
            foreach (PropertyInfo pi in pia)
            {
                Type ColumnType = pi.PropertyType;
                if ((ColumnType.IsGenericType))
                {
                    ColumnType = ColumnType.GetGenericArguments()[0];
                }
                if (pi.Name != "ExtensionData")
                    dt.Columns.Add(pi.Name, ColumnType);
            }

            //Populate the data table
            foreach (RetornoCancelamento item in listRetornoCancelamento)
            {
                DataRow dr = dt.NewRow();
                dr.BeginEdit();
                foreach (PropertyInfo pi in pia)
                {
                    if (pi.GetValue(item, null) != null)
                    {
                        if (pi.Name != "ExtensionData")
                            dr[pi.Name] = pi.GetValue(item, null);
                    }
                }
                dr.EndEdit();
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// Transforma o DataTable de dados em xls
        /// </summary>
        /// <param name="listRetornoCancelamento"></param>
        private void SalvarArquivo(List<RetornoCancelamento> listRetornoCancelamento)
        {

            DateTime? data = txtData.Text.ToDateTimeNull("dd/MM/yyyy");

            DataSet ds = new DataSet();

            ds.Tables.Add(ToDataTable(listRetornoCancelamento));

            StringBuilder csv = new StringBuilder();

            foreach (DataTable dt in ds.Tables)
            {
                csv.AppendLine(dt.TableName);

                String csvTabela = CSVExporter.GerarCSV(dt.Rows.Cast<DataRow>(),
                    dt.Columns.Cast<DataColumn>().Select(row => row.ColumnName).ToList(),
                    (row) => { return row.ItemArray.Select(item => Convert.ToString(item)).ToList(); }, "\t");

                csv.AppendLine(csvTabela);
            }

            Response.Clear();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.AppendHeader("Content-Disposition",
                String.Format("attachment;filename=Relatorio_Cancelamento{0}.xls", DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")));
            Response.ContentEncoding = System.Text.Encoding.GetEncoding(1252);
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.ContentType = "application/ms-excel";
            Response.AppendHeader("Content-Length", csv.Length.ToString());
            Response.Write(csv.ToString());
            Response.Flush();
            Response.End();
        }
    }
}
