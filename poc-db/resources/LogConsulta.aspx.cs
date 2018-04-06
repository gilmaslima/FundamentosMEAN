using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using Redecard.PN.Comum.SharePoint.LogServico;
using ServicoLogClient = Redecard.PN.Comum.SharePoint.LogServico.LogServicoClient;
using Microsoft.SharePoint;
using Redecard.PN.Comum;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    public partial class LogConsulta : SustentacaoApplicationPageBase
    {
        #region [ Propriedades ]

        /// <summary>
        /// ActivityID
        /// </summary>
        public Guid? ActivityID { get { return txtActivityID.Text.ToGuidNull(); } }

        /// <summary>
        /// CodigoEntidade
        /// </summary>
        public Int32? CodigoEntidade { get { return txtEntidade.Text.ToInt32Null(); } }

        /// <summary>
        /// Classe
        /// </summary>
        public String Classe { get { return txtClasse.Text.EmptyToNull(); } }

        /// <summary>
        /// Metodo
        /// </summary>
        public String Metodo { get { return txtMetodo.Text.EmptyToNull(); } }

        /// <summary>
        /// FiltrarPor
        /// </summary>
        public String FiltrarPor { get { return txtFiltro.Text.EmptyToNull(); } }

        /// <summary>
        /// SeverityID
        /// </summary>
        public Int32? SeverityID { get { return ddlSeveridade.SelectedIndex > 0 ? ddlSeveridade.SelectedItem.Value.ToInt32Null() : null; } }

        /// <summary>
        /// AssemblyID
        /// </summary>
        public Int32? AssemblyID { get { return ddlAssembly.SelectedIndex > 0 ? ddlAssembly.SelectedItem.Value.ToInt32Null() : null; } }

        /// <summary>
        /// Servidor
        /// </summary>
        public String Servidor { get { return txtServidor.Text.EmptyToNull(); } }

        /// <summary>
        /// DataInicio
        /// </summary>
        public DateTime? DataInicio { get { return txtDtInicio.Text.ToDateTimeNull("dd/MM/yyyy HH:mm:ss"); } }

        /// <summary>
        /// DataFim
        /// </summary>
        public DateTime? DataFim { get { return txtDtFim.Text.ToDateTimeNull("dd/MM/yyyy HH:mm:ss"); } }

        /// <summary>
        /// QtdRegistros
        /// </summary>
        public Int32 QtdRegistros { get { return ddlQtdRegistros.SelectedItem.Value.ToInt32(Int32.MaxValue); } }

        /// <summary>
        /// Agrupar
        /// </summary>
        public Boolean Agrupar { get { return "S".CompareTo(rblAgrupar.SelectedValue) == 0; } }

        /// <summary>
        /// Relatorio
        /// </summary>
        public String Relatorio { get { return (ddlRelatorios.SelectedIndex > 0 ? ddlRelatorios.SelectedItem.Value : null).EmptyToNull(); } }

        /// <summary>
        /// CSV
        /// </summary>
        public String CSV {
            get { return (String) ViewState["CSV"]; } 
            set { ViewState["CSV"] = value; }
        }

        /// <summary>
        /// RegexModulo
        /// </summary>
        private static Regex RegexModulo { get { return new Regex("(Rede|Redecard)[.](?<Sigla>[^.]*)[.](?<Modulo>[^.]*)[.]*"); } }

        #endregion

        /// <summary>
        /// Page_Load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.DefaultButton = btnPesquisar.UniqueID;

            if (!IsPostBack)
            {
                txtDtInicio.Text = DateTime.Now.ToString("dd/MM/yyyy '00:00:00'");
                txtDtFim.Text = DateTime.Now.ToString("dd/MM/yyyy '23:59:59'");
                mvwLog.SetActiveView(vewOperacoesAgrupadas);
                CarregarDropDownLists();
            }
        }
        
        private void CarregarDropDownLists()
        {
            try
            {
                var dadosBooks = default(Dictionary<String, String>);
                var dadosSeveridade = default(Dictionary<int, String>);
                var dadosAssemblies = default(Dictionary<int, String>);

                using (var contexto = new ContextoWCF<ServicoLogClient>())
                {
                    dadosBooks = contexto.Cliente.ConsultarExtratoBooks();
                    dadosSeveridade = contexto.Cliente.ConsultarSeveridades();
                    dadosAssemblies = contexto.Cliente.ConsultarAssemblies();
                }

                ddlSeveridade.DataSource = dadosSeveridade;
                ddlSeveridade.DataBind();
                ddlRelatorios.DataSource = dadosBooks;
                ddlRelatorios.DataBind();
                ddlAssembly.DataSource = dadosAssemblies;
                ddlAssembly.DataBind();

                txtMetodo.Text = "";
                txtClasse.Text = "";
                txtServidor.Text = "";
            }
            catch (Exception ex)
            {
                pnlFiltro.Visible = false;
                vewAcessoNegado.Controls.Clear();
                StringBuilder mensagem = new StringBuilder();
                mensagem.Append("Erro durante montagem de filtros.<br/>")
                    .Append(ex.Message).Append("<br/>").Append(ex.StackTrace);
                vewAcessoNegado.Controls.Add(base.RetornarPainelExcecao(mensagem.ToString()));
                mvwLog.SetActiveView(vewAcessoNegado);
                SharePointUlsLog.LogErro(ex);
            }
        }

        private void CarregarDropDownAssembly()
        {
            try
            {
                var dadosAssembly = default(Dictionary<int, String>);

                using (var contexto = new ContextoWCF<ServicoLogClient>())
                {
                    dadosAssembly = contexto.Cliente.ConsultarAssemblies();
                }

                ddlAssembly.DataSource = dadosAssembly;
                ddlAssembly.DataBind();
            }
            catch (Exception ex)
            {
                pnlFiltro.Visible = false;
                vewAcessoNegado.Controls.Clear();
                StringBuilder mensagem = new StringBuilder();
                mensagem.Append("Erro durante montagem do filtro de assembly.<br/>")
                    .Append(ex.Message).Append("<br/>").Append(ex.StackTrace);
                vewAcessoNegado.Controls.Add(base.RetornarPainelExcecao(mensagem.ToString()));
                mvwLog.SetActiveView(vewAcessoNegado);
                SharePointUlsLog.LogErro(ex);
            }
        }

        private void CarregarDropDownExtrato()
        {
            try
            {
                var dadosBooks = default(Dictionary<String, String>);

                using (var contexto = new ContextoWCF<ServicoLogClient>())
                {
                    dadosBooks = contexto.Cliente.ConsultarExtratoBooks();
                }

                ddlRelatorios.DataSource = dadosBooks;
                ddlRelatorios.DataBind();
            }
            catch (Exception ex)
            {
                pnlFiltro.Visible = false;
                vewAcessoNegado.Controls.Clear();
                StringBuilder mensagem = new StringBuilder();
                mensagem.Append("Erro durante montagem do filtro de relatório extrato.<br/>")
                    .Append(ex.Message).Append("<br/>").Append(ex.StackTrace);
                vewAcessoNegado.Controls.Add(base.RetornarPainelExcecao(mensagem.ToString()));
                mvwLog.SetActiveView(vewAcessoNegado);
                SharePointUlsLog.LogErro(ex);
            }
        }

        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            if (Relatorio != null)
                CarregarLogExtrato();
            else if (Agrupar)
                CarregarOperacoesAgrupadas(0);
            else
                CarregarOperacoes(0);
        }

        private List<LogOperacao> ConsultarOperacoesAgrupadas(Int32? registroInicial)
        {
            List<LogOperacao> retorno = null;
            using (var contexto = new ContextoWCF<ServicoLogClient>())
                retorno = contexto.Cliente.ConsultarLogOperacoes(
                    ActivityID, AssemblyID, Classe, Metodo, CodigoEntidade,
                    null, null, SeverityID, DataInicio, DataFim, FiltrarPor, QtdRegistros,
                    Servidor, registroInicial);
            return retorno;
        }

        private List<LogItem> ConsultarOperacoes(Int32? registroInicial)
        {
            List<LogItem> retorno = null;
            using (var contexto = new ContextoWCF<ServicoLogClient>())
                retorno = contexto.Cliente.ConsultarLogRegistros(
                        ActivityID, AssemblyID, Classe, Metodo, CodigoEntidade, null, null, 
                        SeverityID, DataInicio, DataFim, FiltrarPor, QtdRegistros + 1,
                        Servidor, registroInicial);
            return retorno;
        }

        private DataSet ConsultarLogExtrato()
        {
            DataSet retorno = null;
            using (var contexto = new ContextoWCF<ServicoLogClient>())
                retorno = contexto.Cliente.ConsultarExtrato(
                    ActivityID, Relatorio, DataInicio, DataFim, Servidor, FiltrarPor);
            return retorno;
        }

        private void CarregarOperacao(Guid activityId, GridView grdEventos)
        {
            if (grdEventos != null)
            {
                var eventos = new List<LogItem>();
                using (var contexto = new ContextoWCF<ServicoLogClient>())
                    eventos = contexto.Cliente.ConsultarLog(activityId);

                grdEventos.DataSource = eventos;
                grdEventos.DataBind();
            }
        }

        private void CarregarOperacoes(Int32 pagina)
        {
            mvwLog.SetActiveView(vewOperacoes);

            Int32 registroInicial = pagina * QtdRegistros;

            repOperacoesAgrupadas.Visible = false;
            
            var dados = ConsultarOperacoes(registroInicial);

            dados.InsertRange(0, new LogItem[registroInicial]);
                       
            grvOperacoes.DataSource = dados;
            grvOperacoes.Visible = true;
            grvOperacoes.PageSize = QtdRegistros;
            grvOperacoes.PageIndex = pagina;
            grvOperacoes.DataBind();
        }

        private void CarregarOperacoesAgrupadas(Int32 pagina)
        {
            mvwLog.SetActiveView(vewOperacoesAgrupadas);

            Int32 registroInicial = pagina * QtdRegistros;

            grvOperacoes.Visible = false;

            var dados = ConsultarOperacoesAgrupadas(registroInicial);

            repOperacoesAgrupadas.DataSource = dados;
            repOperacoesAgrupadas.Visible = true;
            repOperacoesAgrupadas.DataBind();
        }

        protected void grvOperacoes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            CarregarOperacoes(e.NewPageIndex);
        }

        private void CarregarLogExtrato()
        {
            mvwLog.SetActiveView(vwLogExtrato);

            var dados = ConsultarLogExtrato();
            
            //Registros tipo Chamada Mainframe
            var dadosChamada = dados.Tables[0].Rows.Cast<DataRow>().Select(row => new 
            {            
                Tipo = "Chamada",
                LogID = Convert.ToString(row["LogID"]).ToInt32(0),
                Timestamp = Convert.ToString(row["Timestamp"]).ToDateTimeNull(),
                Servidor = Convert.ToString(row["Servidor"]),
                ActivityID= Convert.ToString(row["ActivityID"]).ToGuidNull(),
                Parametros = XElement.Parse(Convert.ToString(row["Parametros"]))
            });

            //Registros tipo Retorno Mainframe
            var dadosRetorno = dados.Tables[1].Rows.Cast<DataRow>().Select(row => new
            {
                Tipo = "Retorno",
                LogID = Convert.ToString(row["LogID"]).ToInt32(0),
                Timestamp = Convert.ToString(row["Timestamp"]).ToDateTimeNull(),
                Servidor = Convert.ToString(row["Servidor"]),
                ActivityID = Convert.ToString(row["ActivityID"]).ToGuidNull(),
                Parametros = XElement.Parse(Convert.ToString(row["Parametros"]))
            });

            //Configuração de parâmetros
            var parametros = dados.Tables[2].Rows.Cast<DataRow>().Select(row => new
            {
                NomeXML = Convert.ToString(row["NomeXML"]),
                Nome = Convert.ToString(row["Nome"])
            });

            //Prepara objeto para exibir em tela
            DataTable dt = new DataTable();
            dt.Columns.Add("Tipo");
            dt.Columns.Add("LogID");
            dt.Columns.Add("Timestamp");
            dt.Columns.Add("Servidor");
            dt.Columns.Add("ActivityID");            
            foreach (var parametro in parametros)
                dt.Columns.Add(parametro.Nome);

            foreach(var dado in dadosChamada)
            {
                var row = dt.NewRow();
                row["Tipo"] = dado.Tipo;
                row["LogID"] = dado.LogID;
                row["Timestamp"] = dado.Timestamp;
                row["Servidor"] = dado.Servidor;
                row["ActivityID"] = dado.ActivityID;

                foreach (var parametro in parametros)
                    row[parametro.Nome] = dado.Parametros.Element(parametro.NomeXML).Value;

                dt.Rows.Add(row);
            }

            foreach (var dado in dadosRetorno)
            {
                var row = dt.NewRow();
                row["Tipo"] = dado.Tipo;
                row["LogID"] = dado.LogID;
                row["Timestamp"] = dado.Timestamp;
                row["Servidor"] = dado.Servidor;
                row["ActivityID"] = dado.ActivityID;

                foreach (var parametro in parametros)
                    row[parametro.Nome] = dado.Parametros.Element(parametro.NomeXML).Value;

                dt.Rows.Add(row);
            }
            
            dt.DefaultView.Sort = "Timestamp";
            dt = dt.DefaultView.ToTable();

            grvLogExtrato.DataSource = dt;
            grvLogExtrato.DataBind();

            this.CSV = CSVExporter.GerarCSV(dt.Rows.Cast<DataRow>(),
                dt.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToList(),
                (row) => {
                    return dt.Columns.Cast<DataColumn>().Select(col => Convert.ToString(row[col])).ToList();
                }, "\t");
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            String nomeArquivo = String.Format("LogExtrato_{0}.xls", DateTime.Now.ToString("ddMMyyyyHHmmss"));

            Response.Clear();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + nomeArquivo);
            Response.ContentEncoding = System.Text.Encoding.GetEncoding(1252);
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.ContentType = "application/ms-excel";

            Response.AppendHeader("Content-Length", this.CSV.Length.ToString());
            Response.Write(this.CSV);
            Response.Flush();
        }

        #region [ Grid Eventos/Operações ]

        protected void grvEventos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = e.Row.DataItem as LogItem;
                
                HyperLink hlkParametros = e.Row.FindControl("hlkParametros") as HyperLink;
                HyperLink hlkExcecao = e.Row.FindControl("hlkExcecao") as HyperLink;
                HyperLink hlkActivityID = e.Row.FindControl("hlkActivityID") as HyperLink;
                Label lblClasse = e.Row.FindControl("lblClasse") as Label;
                Label lblMetodo = e.Row.FindControl("lblMetodo") as Label;
                Label lblAssembly = e.Row.FindControl("lblAssembly") as Label;
                HiddenField hdnLogID = e.Row.FindControl("hdnLogID") as HiddenField;

                hdnLogID.Value = item.LogID.ToString();

                hlkParametros.Visible = !String.IsNullOrEmpty(item.Parametros);
                hlkExcecao.Visible = !String.IsNullOrEmpty(item.Excecao);
                lblClasse.Text = item.Classe;
                lblMetodo.Text = item.Metodo;

                if (hlkActivityID != null)
                {
                    hlkActivityID.ToolTip = item.ActivityID.ToString();
                    hlkActivityID.Attributes["onclick"] = 
                        String.Format("window.open(window.location.href.split('?')[0] + '?ActivityID={0}', '_blank'); return false;", 
                        item.ActivityID.ToString());
                }

                var tokensAssembly = item.Assembly.Split('.');
                lblAssembly.Text = tokensAssembly.Length > 2 ?
                    String.Join(".", tokensAssembly.Take(2).ToArray()) + "<br/>" +
                    String.Join(".", tokensAssembly.Skip(2).ToArray()) : item.Assembly;

                if (item.Severidade == "Error")
                    e.Row.ForeColor = hlkExcecao.ForeColor = Color.Red;
                else
                    e.Row.ForeColor = Color.Black;
            }
        }

        #endregion

        #region [ Processamento XML ]

        private static String ProcessaXml(String xml)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                return "<ul>" + ProcessaNode(xmlDoc.ChildNodes[0]) + "</ul>";
            }
            catch (Exception e)
            {
                return "Message: " + e.Message + "<br/>Source: " + e.Source + "<br/>Stack Trace: " + e.StackTrace;
            }
        }

        private static String ProcessaNode(XmlNode node)
        {
            StringBuilder output = new StringBuilder();

            if (node.ChildNodes.Count > 0)
            {                
                output.Append("<li>").Append(node.Name).Append(":");

                if (node.ChildNodes.Count == 1 && node.ChildNodes[0] is XmlText)
                    output.Append(ProcessaNode(node.ChildNodes[0]));
                else
                {
                    output.Append("<ul style='list-style-type:decimal;'>");
                    foreach (XmlNode childNode in node.ChildNodes)
                        output.Append(ProcessaNode(childNode));
                    output.Append("</ul>");
                }
                output.Append("</li>");
            }
            else if (node is XmlText)
            {
                XmlDocument nodeXml = XmlValido(node.Value);
                if (nodeXml == null)
                    output.Append("&nbsp;[")
                        .Append(HttpUtility.HtmlEncode(node.Value ?? ""))
                        .Append("]");
                else
                {
                    output.Append("<ul style='list-style-type:decimal;'>")
                        .Append(ProcessaNode(nodeXml.ChildNodes[0].ChildNodes[0]))
                        .Append("</ul>");
                }
            }
            else
            {
                output.Append("<li>").Append(node.Name).Append(": [");
                output.Append(node.Value).Append("]");
            }

            return output.ToString();
        }

        /// <summary>
        /// Verifica se uma string é um XML válido
        /// </summary>
        /// <param name="strXml">String a ser verificada</param>
        /// <returns>XmlDocument</returns>
        private static XmlDocument XmlValido(String strXml)
        {
            if (String.IsNullOrEmpty(strXml)) return null;
            strXml = strXml.Trim();

            if (!strXml.StartsWith("<") && !strXml.EndsWith(">"))
                return null;

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(String.Format("<Root>{0}</Root>", strXml));
                return xmlDoc;
            }
            catch(System.Xml.XmlException)
            {
                //Retorna null, pois não é um XML válido
                return null;
            }
        }

        #endregion

        #region [ Repeater Operações Agrupadas ]
      
        protected void repOperacoesAgrupadas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                LogOperacao item = e.Item.DataItem as LogOperacao;

                LinkButton lkbActivity = (LinkButton)e.Item.FindControl("lkbActivity");
                Label lblDuracaoNumero = e.Item.FindControl("lblDuracaoNumero") as Label;
                HtmlTableRow trOperacao = e.Item.FindControl("trOperacao") as HtmlTableRow;

                if (e.Item.ItemIndex % 2 != 0)
                    trOperacao.Attributes["class"] += " alternate";

                if (item.PossuiErro)
                    trOperacao.Attributes["style"] += ";color:red;";

                lblDuracaoNumero.Text = String.Format("{0} ({1})",
                    new DateTime().Add(item.Termino - item.Inicio).ToString("mm:ss.fff"),
                    item.Quantidade);

                if (item.CodigoEntidade.HasValue)
                {
                    Label lblEntidade = e.Item.FindControl("lblEntidade") as Label;
                    lblEntidade.Text = String.Format("{0}<br/>{1}/{2}",
                        (Convert.ToString(item.LoginUsuario) ?? "-").Trim(),
                        (Convert.ToString(item.CodigoEntidade) ?? "-").Trim(),
                        (Convert.ToString(item.GrupoEntidade) ?? "-").Trim());
                }

                Label lblModulo = e.Item.FindControl("lblModulo") as Label;
                lblModulo.Text = GetModulo(item.Classe);

                Label lblSigla = e.Item.FindControl("lblSigla") as Label;
                lblSigla.Text = GetSigla(item.Classe);
            }
        }

        protected void repOperacoesAgrupadas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            GridView grvEventos = (GridView)e.Item.FindControl("grvEventos");
            LinkButton lkbActivity = (LinkButton)e.Item.FindControl("lkbActivity");
            HtmlTableRow trEvento = e.Item.FindControl("trEvento") as HtmlTableRow;

            if (e.CommandName == "mostrar")
            {
                Guid activityID = new Guid(e.CommandArgument.ToString());

                lkbActivity.CommandName = "esconder";

                CarregarOperacao(activityID, grvEventos);
                trEvento.Visible = true;
            }
            else
            {
                lkbActivity.CommandName = "mostrar";
                trEvento.Visible = false;
            }
        }

        #endregion

        #region [ Métodos Auxiliares ]

        private Boolean ChecarUsuarioAdministrador()
        {
#if DEBUG
            return true;
#else
            try
            {
                Boolean permissao = SPContext.Current.Web.UserIsWebAdmin;
                if (!permissao)
                {
                    mvwLog.SetActiveView(vewAcessoNegado);
                    vewAcessoNegado.Controls.Clear();
                    vewAcessoNegado.Controls.Add(base.RetornarPainelExcecao("Somente usuários administradores podem acessar a página de Consulta de Log."));
                }
                return permissao;
            }
            catch
            {
                mvwLog.SetActiveView(vewAcessoNegado);
                vewAcessoNegado.Controls.Clear();
                vewAcessoNegado.Controls.Add(base.RetornarPainelExcecao(FONTE, CODIGO_ERRO));
                return false;
            }
#endif
        }

        private static String GetModulo(String assembly)
        {
            var match = RegexModulo.Match(assembly);
            return match.Success ? match.Groups["Modulo"].Value : assembly;
        }
        private static String GetSigla(String assembly)
        {
            var match = RegexModulo.Match(assembly);
            return match.Success ? match.Groups["Sigla"].Value : assembly;
        }

        #endregion

        #region [ WebMethod ]

        private static String ConsultarParametros(Int32 logID, Boolean formatar)
        {
            using (var contexto = new ContextoWCF<ServicoLogClient>())
            {
                LogItem registro = contexto.Cliente.ConsultarLogRegistro(logID);
                if (registro != null)
                    return formatar ? ProcessaXml(registro.Parametros) : HttpUtility.HtmlDecode(registro.Parametros);
                else
                    return String.Empty;
            }
        }

        private static String ConsultarExcecao(Int32 logID)
        {
            using (var contexto = new ContextoWCF<ServicoLogClient>())
            {
                LogItem registro = contexto.Cliente.ConsultarLogRegistro(logID);
                if (registro != null)
                    return HttpUtility.HtmlEncode(registro.Excecao).Replace("\n", "<br/>");
                else
                    return String.Empty;
            }
        }

        [WebMethod]
        public static String ConsultarRegistro(String tipo, String logID)
        {
            switch (tipo)
            {
                case "parametrosFormatados": return ConsultarParametros(logID.ToInt32(), true);
                case "parametros": return ConsultarParametros(logID.ToInt32(), false);
                case "excecao": return ConsultarExcecao(logID.ToInt32());
                default: return String.Format("Consulta inválida: {0}", tipo);
            }
        }

        #endregion
    }
}