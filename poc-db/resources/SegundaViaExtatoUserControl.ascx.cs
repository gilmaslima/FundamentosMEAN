using Microsoft.SharePoint.Administration;
using Redecard.PN.Comum;
using Redecard.PN.GerencieExtrato.Core.Web.Controles.Portal;
using Redecard.PN.GerencieExtrato.SharePoint.GerencieExtratoServico;
using Redecard.PN.GerencieExtrato.SharePoint.Helper;
using Redecard.PN.GerencieExtrato.SharePoint.ZPPlanoContasServico;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.GerencieExtrato.SharePoint.SegundaViaExtato
{
    public partial class SegundaViaExtatoUserControl : UserControlBase
    {
        #region [ Constantes / Propriedades da página ]

        /// <summary>CultureInfo pt-BR</summary>
        private readonly CultureInfo ptBr = CultureInfo.CreateSpecificCulture("pt-BR");

        /// <summary>Página padrão de extrato onde usuário pode selecionar qual funcionalidade quer acessar</summary>
        private const String _cPaginaGerenciaExtrato = "/Paginas/pn_GerencieExtratoDefault.aspx";

        #endregion

        #region [ Eventos da Página ]
        protected void Page_Load(object sender, EventArgs e)
        {
            //Define o timeout da página de 10min
            Server.ScriptTimeout = 600;

            if (!Page.IsPostBack)
            {
                //Carrega os dados do Repeater
                CarregarRepeater();
            }
        }

        /// <summary>Handler do botão voltar, redireciona para a tela de Gerênciar Extratos</summary>        
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            //Redireciona para a página de escolha de funcionalidade
            Response.Redirect(string.Concat(base.web.ServerRelativeUrl, _cPaginaGerenciaExtrato));
        }

        /// <summary>Handler do botão de download, Gera os arquivos para os usuários</summary>        
        protected void btnDownloadHtml_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Download HTML da Segunda Via Extrato"))
            {
                try
                {
                    String nomeArquivo = default(String);
                    List<RelatorioDetalhadoPrecoUnico> listaRelatorioDetalhadoPrecoUnico;
                    List<String> linhasExtrato = ObterExtratosParaDownload(out nomeArquivo, out listaRelatorioDetalhadoPrecoUnico);

                    if (listaRelatorioDetalhadoPrecoUnico != null)
                        linhasExtrato = MontarLinhasRelatorioDetalhadoPrecoUnico(listaRelatorioDetalhadoPrecoUnico);

                    //Gera o arquivo para download
                    GeraArquivoDownload(linhasExtrato, "htm", nomeArquivo);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>Handler do botão de download, Gera os arquivos para os usuários</summary>        
        protected void btnDownloadDoc_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Download Doc da Segunda Via Extrato"))
            {
                try
                {
                    String nomeArquivo = default(String);
                    List<RelatorioDetalhadoPrecoUnico> listaRelatorioDetalhadoPrecoUnico;
                    List<String> linhasExtrato = ObterExtratosParaDownload(out nomeArquivo, out listaRelatorioDetalhadoPrecoUnico);

                    if (listaRelatorioDetalhadoPrecoUnico != null)
                        linhasExtrato = MontarLinhasRelatorioDetalhadoPrecoUnico(listaRelatorioDetalhadoPrecoUnico);

                    //Gera o arquivo para download
                    GeraArquivoDownload(linhasExtrato, "doc", nomeArquivo);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>Handler do botão de download, Gera os arquivos para os usuários</summary>        
        protected void btnDownloadTxt_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Download Txt da Segunda Via Extrato"))
            {
                try
                {
                    String nomeArquivo = default(String);
                    List<RelatorioDetalhadoPrecoUnico> listaRelatorioDetalhadoPrecoUnico;
                    List<String> linhasExtrato = ObterExtratosParaDownload(out nomeArquivo, out listaRelatorioDetalhadoPrecoUnico);

                    if (listaRelatorioDetalhadoPrecoUnico != null)
                        linhasExtrato = MontarLinhasRelatorioDetalhadoPrecoUnico(listaRelatorioDetalhadoPrecoUnico);

                    //Gera o arquivo para download
                    GeraArquivoDownload(linhasExtrato, "txt", nomeArquivo);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>Handler do botão de download, Gera os arquivos para os usuários</summary>        
        protected void btnDownloadExcel_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Download Excel da Segunda Via Extrato"))
            {
                try
                {
                    String nomeArquivo = default(String);
                    List<RelatorioDetalhadoPrecoUnico> listaRelatorioDetalhadoPrecoUnico;
                    List<String> linhasExtrato = ObterExtratosParaDownload(out nomeArquivo, out listaRelatorioDetalhadoPrecoUnico);

                    Dictionary<String, String> colunas = new Dictionary<String, String>();
                    List<Object> registros = new List<Object>();
                    StringBuilder sbCabecalho = new StringBuilder();

                    if (listaRelatorioDetalhadoPrecoUnico != null)
                    {
                        colunas = MontarColunasRelatorioDetalhadoPrecoUnico();
                        registros = MontarRegistrosRelatorioDetalhadoPrecoUnico(listaRelatorioDetalhadoPrecoUnico, true);

                        sbCabecalho.Append("<p style='font-weight:bold'>Relatório Acompanhamento Turquia</p>");
                        sbCabecalho.Append(String.Format("<p>Período: {0}</p>", hdPeriodo.Value));
                    }

                    //Gera o arquivo no formato xls para download.
                    GerarArquivoDownloadExcel(registros, colunas, nomeArquivo, "xls", sbCabecalho.ToString());
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        private List<String> ObterExtratosParaDownload(out String nomeArquivo, out List<RelatorioDetalhadoPrecoUnico> listaRelatorioDetalhadoPrecoUnico)
        {
            List<String> linhasExtrato = new List<String>();
            listaRelatorioDetalhadoPrecoUnico = default(List<RelatorioDetalhadoPrecoUnico>);

            if (Type.GetType(hdTipoExtratoSelecionado.Value) == typeof(ExtratoRelatorioPrecoUnico))
            {
                nomeArquivo = "ExtratoRelatorioPrecoUnico";
                //Busca os extratos relatório preço único.
                listaRelatorioDetalhadoPrecoUnico = DetalharRelatorioPrecoUnico();
            }
            else
            {
                nomeArquivo = "ExtratoPapel";
                //Busca os extratos
                List<Extrato> listaExtratos = ConsultarExtrato();

                if (listaExtratos != null)
                    linhasExtrato = listaExtratos.Select(extrato => extrato.LinhaExtrato).ToList();
            }

            return linhasExtrato;
        }
        /*
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Download da Segunda Via Extrato"))
            {
                try
                {
                    var linhasExtrato = new List<String>();
                    var listaRelatorioDetalhadoPrecoUnico = default(List<RelatorioDetalhadoPrecoUnico>);
                    String nomeArquivo = default(String);

                    if (Type.GetType(hdTipoExtratoSelecionado.Value) == typeof(ExtratoRelatorioPrecoUnico))
                    {
                        nomeArquivo = "ExtratoRelatorioPrecoUnico";
                        //Busca os extratos relatório preço único.
                        listaRelatorioDetalhadoPrecoUnico = DetalharRelatorioPrecoUnico();
                    }
                    else
                    {
                        nomeArquivo = "ExtratoPapel";
                        //Busca os extratos
                        List<Extrato> listaExtratos = ConsultarExtrato();

                        if (listaExtratos != null)
                            linhasExtrato = listaExtratos.Select(extrato => extrato.LinhaExtrato).ToList();
                    }

                    //Trata o export de Excel de maneira específica.
                    if (rboExtensaoXls.Checked == true)
                    {
                        Dictionary<String, String> colunas = new Dictionary<String, String>();
                        List<Object> registros = new List<Object>();
                        StringBuilder sbCabecalho = new StringBuilder();

                        if (listaRelatorioDetalhadoPrecoUnico != null)
                        {
                            colunas = MontarColunasRelatorioDetalhadoPrecoUnico();
                            registros = MontarRegistrosRelatorioDetalhadoPrecoUnico(listaRelatorioDetalhadoPrecoUnico, true);

                            sbCabecalho.Append("<p style='font-weight:bold'>Relatório Acompanhamento Turquia</p>");
                            sbCabecalho.Append(String.Format("<p>Período: {0}</p>", hdPeriodo.Value));
                        }

                        //Gera o arquivo no formato xls para download.
                        GerarArquivoDownloadExcel(registros, colunas, nomeArquivo, RetornaExtensao(), sbCabecalho.ToString());
                    }
                    else
                    {
                        if (listaRelatorioDetalhadoPrecoUnico != null)
                            linhasExtrato = MontarLinhasRelatorioDetalhadoPrecoUnico(listaRelatorioDetalhadoPrecoUnico);

                        //Gera o arquivo para download
                        GeraArquivoDownload(linhasExtrato, RetornaExtensao(), nomeArquivo);
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }
 */

        /// <summary>Envia o e-mail com os dados do Fax</summary>        
        protected void btnEnviarFax_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Envio E-mail com dados do Fax"))
            {
                //Envia o e-mail para fax
                EnviarEmailFax();
            }
        }
        #endregion

        #region [ Métodos Auxiliares ]
        // <summary>Exibe / Esconde os controles da página</summary>
        /// <param name="exibir"></param>
        private void ExibirExtratos(Boolean exibirExtrato)
        {
            //Carrega o Repeater
            rptExtratos.Visible = exibirExtrato;
            //tblExtrato.Visible  = exibirExtrato;
            //btnDownload.Visible = exibirExtrato;

            //Somente exibe se atendimento
#if DEBUG
            btnEnviar.Visible = true;
#else            
            btnEnviar.Visible = exibirExtrato && SessaoAtual != null && SessaoAtual.UsuarioAtendimento;
#endif
        }

        /// <summary>Exibe a mensagem do quadro de aviso</summary>
        /// <param name="titulo">Título</param>
        /// <param name="mensagem">Mensagem</param>
        private void ExibirAviso(string titulo, string mensagem, bool esconderExtratos, TipoQuadroAviso tipoQuadro, String buttonText = null, String clientClick = null)
        {
            //Mostra o quadro de aviso
            pnlSemExtratos.Visible = true;

            //Define a mensagem do Quadro de aviso
            //QuadroAviso qdAviso = qdAvisoSemExtrato as QuadroAviso;

            qdAvisoSemExtrato.Titulo = titulo ?? "Aviso";
            qdAvisoSemExtrato.Mensagem = mensagem;
            qdAvisoSemExtrato.TipoQuadro = tipoQuadro;
            qdAvisoSemExtrato.ClientClick = clientClick;
            qdAvisoSemExtrato.ButtonText = buttonText;

            //Esconde os dados do extrato
            if (esconderExtratos)
            {
                //Mostra o Repeater
                ExibirExtratos(false);

                //Exconde o painel de conteúdo
                pnlConteudo.Visible = false;
                //tdBotoesBottom.Visible = false;
            }
        }

        /// <summary>Retorna a extensão do arquivo selecionado pelo usuário</summary>
        /// <returns>extensão</returns>
        //private String RetornaExtensao()
        //{
        //    //if (rbExtensaoHTML.Checked)
        //    //    return "htm";
        //    //else if (rbExtensaoDOC.Checked)
        //    //    return "doc";
        //    //else if (rbExtensaoTXT.Checked)
        //    //    return "txt";
        //    //else 
        //    if (rboExtensaoXls.Checked)
        //        return "xls";
        //    else
        //        return "";
        //}

        /// <summary>Carrega os dados do Repeater</summary>
        private void CarregarRepeater()
        {
            using (Logger log = Logger.IniciarLog("Carregando Repeater"))
            {
                try
                {
                    //Parâmetros do serviço
                    List<ExtratoEmitido> listaExtratos = ListaExtratos();
                    List<ExtratoRelatorioPrecoUnico> listaExtatosRelatorioPrecoUnico = ConsultarRelatorioPrecoUnico();
                    List<ExtratoBase> listaExtratosDataSource = new List<ExtratoBase>();

                    listaExtratosDataSource.AddRange(listaExtratos.Cast<ExtratoBase>());
                    listaExtratosDataSource.AddRange(listaExtatosRelatorioPrecoUnico.Cast<ExtratoBase>());

                    //Controla exibição do link de solicitação de relatório
                    pnlSolicitarRelatorio.Visible = ConsultarTipoOfertaAtiva() == TipoOferta.OfertaTurquia;

#if DEBUG
                    pnlSolicitarRelatorio.Visible = true;
#endif

                    //Verifica se há extratos no período
                    if (listaExtratosDataSource != null && listaExtratosDataSource.Count > 0)
                    {
                        //Mostra o Repeater
                        ExibirExtratos(true);

                        //Esconde o quadro de aviso
                        pnlSemExtratos.Visible = false;

                        //Carrega o Repeater
                        rptExtratos.DataSource = listaExtratosDataSource;
                        rptExtratos.DataBind();
                    }
                    else
                    {
                        log.GravarMensagem("Não há extratos para este estabelecimento.");
                        //Exibe mensagem de aviso
                        ExibirAviso("Aviso", "Não há extratos para este estabelecimento.", true, TipoQuadroAviso.Aviso);
                    }
                }
                catch (ExcecaoWs ex)
                {
                    log.GravarErro(ex);
                    ExibirExtratos(false);
                    base.ExibirPainelExcecao(ex.Fonte, ex.CodigoErro);
                }
                catch (FaultException<GerencieExtratoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    ExibirExtratos(false);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    ExibirExtratos(false);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>Busca os dados do Extrato</summary>
        /// <returns>Lista com os extratos</returns>
        private List<Extrato> ConsultarExtrato()
        {
            using (Logger Log = Logger.IniciarLog("Consulta de Extrato"))
            {
                //Parâmetros do serviço
                List<Extrato> listaExtratos = new List<Extrato>();
                Int32 codigoEstabelecimento = base.SessaoAtual.CodigoEntidade;
                Int32 numeroExtrato = hdNumeroSelecionado.Value.ToInt32();
                String tipoAcesso = "AVANCA";
                Int32 sequencia = 0;
                String mensagem = "";
                Int16 codigoRetorno = 0;

                Log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoEstabelecimento,
                    numeroExtrato,
                    tipoAcesso,
                    codigoRetorno,
                    mensagem,
                    sequencia
                });

                //Busca os dados do extrato
                using (var ctx = new ContextoWCF<GerencieExtratoClient>())
                {
                    listaExtratos = ctx.Cliente.ConsultarExtrato(ref codigoEstabelecimento,
                                                                 ref numeroExtrato,
                                                                 ref tipoAcesso,
                                                                 ref codigoRetorno,
                                                                 ref mensagem,
                                                                 ref sequencia);
                }

                Log.GravarLog(EventoLog.RetornoServico, new
                {
                    listaExtratos,
                    codigoEstabelecimento,
                    numeroExtrato,
                    tipoAcesso,
                    codigoRetorno,
                    mensagem,
                    sequencia
                });

                //Caso tenha ocorrido erro na chamada, dispara uma exceção
                if (codigoRetorno > 0)
                    throw new ExcecaoWs() { CodigoErro = codigoRetorno, Fonte = "GerencieExtrato.ClientConsultarExtrato" };

                //Retorna a lista
                return listaExtratos;
            }
        }

        /// <summary>
        /// Gera arquivo de download no formato de Excel.
        /// </summary>
        /// <typeparam name="T">Tipo de registros que se deseja gerar o arquivo</typeparam>
        /// <param name="registros">Lista de registros de tipo genérico para gerar o arquivo.</param>
        /// <param name="colunas">Dicionário com nome da propriedade de T e nome de exibição da coluna.</param>
        private void GerarArquivoDownloadExcel<T>(List<T> registros, Dictionary<String, String> colunas, String nomeArquivo, String extensao, String cabecalho)
        {
            using (Logger log = Logger.IniciarLog("Gera arquivo Excel para download"))
            {
                try
                {
                    String dadosRelatorio = MontarRelatorioExcel(registros, colunas);

                    Response.Clear();
                    Response.Buffer = true;

                    //Define o Enconding para Ansi
                    Response.ContentEncoding = Encoding.GetEncoding(1252);

                    Response.AddHeader("content-disposition", String.Concat("attachment;filename=", nomeArquivo, ".", extensao));
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.ms-excel";

                    //style to format numbers to string
                    string style = @"<style> .textmode { mso-number-format:\@; } </style>";
                    Response.Write(cabecalho);
                    Response.Write(style);
                    Response.Output.Write(dadosRelatorio);
                    Response.Flush();
                    Response.End();

                }
                catch (ExcecaoWs ex)
                {
                    log.GravarErro(ex);
                    ExibirExtratos(false);
                    base.ExibirPainelExcecao(ex.Fonte, ex.CodigoErro);
                }
                catch (FaultException<GerencieExtratoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    ExibirExtratos(false);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    ExibirExtratos(false);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>Gera arquivo para download</summary>
        private void GeraArquivoDownload(List<String> linhasExtrato, String extensao, String nomeArquivo)
        {
            using (Logger log = Logger.IniciarLog("Gera arquivo para download"))
            {
                try
                {

                    //Define o Enconding para Ansi
                    Response.ContentEncoding = Encoding.GetEncoding(1252);

                    //Retorna
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);  //Define para não ter cache
                    Response.AppendHeader("Content-Disposition", String.Concat("attachment;filename=", nomeArquivo, ".", extensao));    //Define o nome do arquivo

                    //Caso seja um TXT
                    //if (rbExtensaoTXT.Checked)
                    if (String.Compare(extensao, "txt", true) == 0)
                    {
                        Response.ContentType = "text/plain";

                        foreach (String linhaExtrato in linhasExtrato)
                        {
                            if (!String.IsNullOrEmpty(linhaExtrato))
                                Response.Write(linhaExtrato + "\r\n");
                        }
                    }
                    else //Para DOC e HTML (Sempre exporta como html)
                    {
                        string fontStyle = "font-size:13px;font-family:Courier New;white-space:nowrap;";

                        //if (rbExtensaoHTML.Checked)
                        if (String.Compare(extensao, "htm", true) == 0)
                        {
                            Response.ContentType = "text/html; charset=UTF-8";
                        }
                        //else if (rbExtensaoDOC.Checked)
                        else if (String.Compare(extensao, "doc", true) == 0)
                        {
                            Response.ContentType = "application/msword";
                            if (nomeArquivo.Contains("ExtratoRelatorioPrecoUnico")) //Se Preço Único, diminui a fonte
                                fontStyle = "font-size:12px;font-family:Courier New;white-space:nowrap;";
                        }

                        //Inicia o html
                        Response.Write("<html>");

                        //Adiciona as linhas (com cabeçalho)
                        for (int i = 0; i < linhasExtrato.Count; i++)
                        {
                            if (i == 0)
                            {
                                Response.Write(String.Format("<img src='{0}'><br/>",
                                    String.Concat(base.web.Url, "/_layouts/Redecard.Comum/IMAGES/LogoRedeLaranja.png")));
                            }

                            if (!String.IsNullOrEmpty(linhasExtrato[i]) && linhasExtrato[i].Trim() != "")
                                Response.Write(String.Format("<span style='{0}'>{1}</span><br/>", fontStyle,
                                   HttpUtility.HtmlEncode(linhasExtrato[i]).Replace(" ", "&nbsp;")));
                        }

                        //Finaliza o html
                        Response.Write("</html>");
                    }

                    //Retorna o arquivo
                    Response.Flush();
                    Response.End();

                }
                catch (ExcecaoWs ex)
                {
                    log.GravarErro(ex);
                    ExibirExtratos(false);
                    base.ExibirPainelExcecao(ex.Fonte, ex.CodigoErro);
                }
                catch (FaultException<GerencieExtratoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    ExibirExtratos(false);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    ExibirExtratos(false);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

        }

        /// <summary>Monta o corpo do e-mail para envio de Fax</summary>
        private Boolean MontaConteudoAnexo(out String conteudo, Boolean email, Boolean extensaoTxt, Boolean extensaoXls)
        {
            using (Logger log = Logger.IniciarLog("Montando conteúdo anexo"))
            {
                //Inicia o Corpo de e-mail para retorno
                StringBuilder sbRetorno = new StringBuilder();
                conteudo = "";

                try
                {
                    var listaExtratos = default(List<Extrato>);
                    var linhasExtrato = new List<String>();
                    var listaRelatorioDetalhadoPrecoUnico = default(List<RelatorioDetalhadoPrecoUnico>);

                    //Busca os extratos
                    if (Type.GetType(hdTipoExtratoSelecionado.Value) == typeof(ExtratoRelatorioPrecoUnico))
                    {
                        listaRelatorioDetalhadoPrecoUnico = DetalharRelatorioPrecoUnico();

                        //if (!rboExtensaoXls.Checked && listaRelatorioDetalhadoPrecoUnico != null)
                        if (!extensaoXls && listaRelatorioDetalhadoPrecoUnico != null)
                            linhasExtrato = MontarLinhasRelatorioDetalhadoPrecoUnico(listaRelatorioDetalhadoPrecoUnico);
                    }
                    else
                    {
                        listaExtratos = ConsultarExtrato();

                        if (listaExtratos != null)
                            linhasExtrato = MontarLinhasExtratoPapel(listaExtratos);
                    }
                    if (!email)
                    {
                        //Caso tenha mais de 800  linhas, não permite o envio via Fax
                        if (linhasExtrato.Count > 800 || (listaRelatorioDetalhadoPrecoUnico != null && listaRelatorioDetalhadoPrecoUnico.Count > 800))
                        {
                            //Exibi a mensagem de aviso
                            ExibirAviso(null, "Não será possível enviar esse extrato por fax. <br/>Envie por correio ou peça para o cliente fazer o download no Portal.", true, TipoQuadroAviso.Erro, "Ok", String.Format(@"location.href=""{0}""; return false;", Request.Url.AbsolutePath));

                            //Retorna que não foi possível gerar
                            return false;
                        }
                    }
                    else
                    {
                        //Caso tenha mais de 20000 linhas, não permite o envio por e-mail
                        if (linhasExtrato.Count > 20000 || (listaRelatorioDetalhadoPrecoUnico != null && listaRelatorioDetalhadoPrecoUnico.Count > 20000))
                        {
                            //Exibi a mensagem de aviso
                            ExibirAviso(null, "Não será possível enviar esse extrato por e-mail. <br/>Envie por correio ou peça para o cliente fazer o download no Portal.<br/><br/>", true, TipoQuadroAviso.Erro, "Ok", String.Format(@"location.href=""{0}""; return false;", Request.Url.AbsolutePath));
                            //Retorna que não foi possível gerar
                            return false;
                        }
                    }

                    //Caso seja um TXT
                    //if (rbExtensaoTXT.Checked)
                    if (extensaoTxt)
                    {
                        foreach (var linhaExtrato in linhasExtrato)
                            sbRetorno.Append(String.Concat(linhaExtrato, "\r\n"));
                    }
                    //else if (rboExtensaoXls.Checked && listaRelatorioDetalhadoPrecoUnico != null) //Para XLS gera uma tabela em HTML com os dados.
                    else if (extensaoXls && listaRelatorioDetalhadoPrecoUnico != null) //Para XLS gera uma tabela em HTML com os dados.
                    {
                        Dictionary<String, String> colunas = MontarColunasRelatorioDetalhadoPrecoUnico();
                        List<Object> registros = MontarRegistrosRelatorioDetalhadoPrecoUnico(listaRelatorioDetalhadoPrecoUnico, false);
                        String relatorioExcel = MontarRelatorioExcel(registros, colunas);

                        sbRetorno.Append("<p style='font-weight:bold'>Relatório Acompanhamento Turquia</p>");
                        sbRetorno.Append(String.Format("<p>Período: {0}</p>", hdPeriodo.Value));
                        relatorioExcel.Replace("\"", " ").Replace("=", "");
                        sbRetorno.Append(relatorioExcel);
                    }
                    else //Para DOC e HTML (Sempre exporta como html)
                    {
                        //Inicia o html
                        string fontStyle = "font-family: Courier New; font-size: 8px;";
                        sbRetorno.Append("<html><body>");
                        sbRetorno.AppendFormat("<table width='100%'><tr><td><img src='{0}'></td></tr>", String.Concat(base.web.Url, "/_layouts/images/Redecard.PN.GerencieExtrato.SharePoint/logoExtrato.png"));

                        //Adiciona as linhas (com cabeçalho)
                        for (int i = 0; i < linhasExtrato.Count; i++)
                        {
                            if (!String.IsNullOrEmpty(linhasExtrato[i]) && linhasExtrato[i].Trim() != "")
                                sbRetorno.AppendFormat("<tr><td style='{0}'>{1}</td></tr>", fontStyle, linhasExtrato[i].Replace(" ", "&nbsp;"));
                        }
                        sbRetorno.Append("</table></body></html>");
                    }
                }
                catch (ExcecaoWs ex)
                {
                    log.GravarErro(ex);
                    ExibirExtratos(false);
                    base.ExibirPainelExcecao(ex.Fonte, ex.CodigoErro);
                }
                catch (FaultException<GerencieExtratoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    ExibirExtratos(false);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    return false;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    ExibirExtratos(false);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return false;
                }

                //Finaliza a geração do corpo
                conteudo = sbRetorno.ToString();

                Logger.GravarLog("Corpo do e-mail", conteudo);

                return true;
            }
        }

        /// <summary>Efetua o envio do e-mail para fax</summary>
        private void EnviarEmailFax()
        {
            using (Logger log = Logger.IniciarLog("Envio do e-mail para fax"))
            {
                //Fecha o Dialog
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "ProtecaoExibir", "$('#bgProtecao').hide();", true);
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "SelecaoPVExibir", "$('#dvEnvioFax').hide();", true);

                //Dados para o envio do e-mail                
                String origem = "faleconosco@userede.com.br";
                String conteudoAnexo = "";
                String nomeArquivo = String.Concat(base.SessaoAtual.CodigoEntidade.ToString(), "_", DateTime.Now.ToString("yyyyMMdd"), "SVEP.html");
                String assuntoEmail = null;
                String destino = null;
                Boolean extensaoTXT = String.Compare(ddlTipoArquivo.SelectedValue, "Txt", true) == 0;
                Boolean extensaoXls = String.Compare(ddlTipoArquivo.SelectedValue, "Excel", true) == 0;
                //Boolean extensaoTXT = rbExtensaoTXT.Checked;

                //Opção e-mail selecionada
                if (rdbtnEmail.Checked)
                {
                    assuntoEmail = "Segunda Via Extrato Papel";
                    destino = txtEmailFax.Text;
                }
                //Opção Fax selecionada
                else if (rdbtnFax.Checked)
                {
                    assuntoEmail = txtDDDFax.Text.Trim() != "11" ?
                            String.Concat("021", txtDDDFax.Text.Trim(), txtNumeroFax.Text.Trim()) :
                            txtNumeroFax.Text.Trim();

                    //Ajusta o título do e-mail
                    assuntoEmail = "#" + assuntoEmail + " NOCOVER";

                    destino = "rcfaxsrv@userede.com.br";
                }
                //Nenhuma opção de envio selecionada
                else
                {
                    ExibirExtratos(false);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return;
                }

                //Gera o corpo do e-mail, caso não tenha gerado, não continua
                if (!MontaConteudoAnexo(out conteudoAnexo, rdbtnEmail.Checked, extensaoTXT, extensaoXls))
                    return;

                try
                {
                    //Cria o objeto para envio de e-mail (Buscando da configuração do Sharepoint)
                    String smtpServer = SPAdministrationWebApplication.Local.OutboundMailServiceInstance != null ?
                                        SPAdministrationWebApplication.Local.OutboundMailServiceInstance.Server.Address
                                        : "";

#if !DEBUG
                    //Verifica se retornou o servidor para envio de e-mail
                    if (string.IsNullOrEmpty(smtpServer))
                        throw new Exception("SMTP para envio de e-mail não configurado no servidor do Sharepoint.");
#endif

                    //Cria o objeto para envio do e-mail
                    SmtpClient smtpClient = new SmtpClient(smtpServer);

                    //Cria a mensagem e adiciona o anexo
                    MailMessage mensagemEmail;

                    if (rdbtnFax.Checked)
                    {
                        mensagemEmail = new MailMessage(origem, destino, assuntoEmail, String.Empty);
                        mensagemEmail.Attachments.Add(new System.Net.Mail.Attachment(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(conteudoAnexo)),
                                                        nomeArquivo));
                    }
                    else
                    {
                        mensagemEmail = new MailMessage(origem, destino, assuntoEmail, conteudoAnexo);
                    }

                    mensagemEmail.IsBodyHtml = !extensaoTXT;

                    //Envia o e-mail
                    smtpClient.Send(mensagemEmail); 


                    //Se estiver OK, Exibe a mensagem de confirmação
                    //base.ExibirPainelConfirmacaoAcao("Segunda via de extrato", "Fax enviado com sucesso.", Request.Url.AbsoluteUri, new Panel[0]);
                    String msgSucesso = String.Format("{0} enviado com sucesso.", rdbtnFax.Checked ? "Fax" : "E-mail");
                    String clientClick = String.Format(@"location.href=""{0}""; return false;", Request.Url.AbsolutePath);
                    ExibirAviso(null, msgSucesso, true, TipoQuadroAviso.Sucesso, "Ok", clientClick);
                }
                catch (FaultException<GerencieExtratoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    ExibirExtratos(false);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    ExibirExtratos(false);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Consulta os relatórios de preço único disponíveis.
        /// </summary>
        /// <returns></returns>
        private List<ExtratoRelatorioPrecoUnico> ConsultarRelatorioPrecoUnico()
        {
            using (Logger log = Logger.IniciarLog("Consulta de Extrato Reltório Preço Único"))
            {
                //Parâmetros do serviço
                var listaExtratoRelatorioPrecoUnico = default(List<ExtratoRelatorioPrecoUnico>);
                Int32 numeroPv = base.SessaoAtual.CodigoEntidade;
                Int16 codigoRetorno = 0;

                log.GravarLog(EventoLog.ChamadaServico, new { numeroPv, codigoRetorno });

                //Busca os dados do extrato
                using (var ctx = new ContextoWCF<GerencieExtratoClient>())
                    listaExtratoRelatorioPrecoUnico = ctx.Cliente.ConsultarRelatorioPrecoUnico(out codigoRetorno, numeroPv);

                log.GravarLog(EventoLog.RetornoServico, new { listaExtratoRelatorioPrecoUnico, numeroPv, codigoRetorno });

                //Caso tenha ocorrido erro na chamada, dispara uma exceção
                if (codigoRetorno > 0)
                    throw new ExcecaoWs() { CodigoErro = codigoRetorno, Fonte = "GerencieExtrato.ExtratoRelatorioPrecoUnico" };

                //Retorna a lista
                return listaExtratoRelatorioPrecoUnico;
            }

        }

        /// <summary>
        /// Lista extratos disponíveis
        /// </summary>
        /// <returns></returns>
        private List<ExtratoEmitido> ListaExtratos()
        {
            using (Logger log = Logger.IniciarLog("Lista Extratos"))
            {
                //Parâmetros do serviço
                List<ExtratoEmitido> listaExtratos = null;
                Int32 codEstabelecimento = base.SessaoAtual.CodigoEntidade;
                Int16 numeroExtrato = 0;
                Int16 totalRegistros = 0;
                Int16 tsReg = 0;
                Int16 qtdOcorrencias = 0;
                Int16 codigoRetorno = 0;
                String mensagem = "";

                log.GravarLog(EventoLog.ChamadaServico, new { codEstabelecimento, numeroExtrato });

                //Busca os extratos
                using (var ctx = new ContextoWCF<GerencieExtratoClient>())
                {
                    listaExtratos = ctx.Cliente.ListaExtratos(codEstabelecimento,
                                                            numeroExtrato,
                                                            ref totalRegistros,
                                                            ref tsReg,
                                                            ref mensagem,
                                                            ref qtdOcorrencias,
                                                            ref codigoRetorno);
                }

                log.GravarLog(EventoLog.RetornoServico,
                    new { listaExtratos, totalRegistros, tsReg, mensagem, qtdOcorrencias, codigoRetorno });

                //Caso tenha ocorrido erro na chamada, dispara uma exceção
                if (codigoRetorno > 0)
                    throw new ExcecaoWs() { CodigoErro = codigoRetorno, Fonte = "GerencieExtratoServico.ListaExtratos" };

                return listaExtratos;
            }
        }

        /// <summary>
        /// Detalhamento do relatório Preço Único.
        /// </summary>
        /// <returns></returns>
        public List<RelatorioDetalhadoPrecoUnico> DetalharRelatorioPrecoUnico()
        {
            using (Logger log = Logger.IniciarLog("Consulta de Extrato Reltório Preço Único"))
            {
                //Parâmetros do serviço
                var listaRelatorioDetalhadoPrecoUnico = default(List<RelatorioDetalhadoPrecoUnico>);
                Int32 numeroPv = base.SessaoAtual.CodigoEntidade;
                DateTime mesAnoRelatorio = hdNumeroSelecionado.Value.ToString().ToDate("yyyyMM", DateTime.MinValue);
                Int16 flagVsam = Convert.ToInt16(hdFlagVsam.Value);
                Int16 codigoRetorno = 0;

                log.GravarLog(EventoLog.ChamadaServico, new { numeroPv, codigoRetorno });

                //Busca os dados do extrato
                using (var ctx = new ContextoWCF<GerencieExtratoClient>())
                    listaRelatorioDetalhadoPrecoUnico = ctx.Cliente.DetalharRelatorioPrecoUnico(
                        out codigoRetorno, numeroPv, mesAnoRelatorio, flagVsam);

                log.GravarLog(EventoLog.RetornoServico, new { listaRelatorioDetalhadoPrecoUnico, numeroPv, codigoRetorno });

                //Caso tenha ocorrido erro na chamada, dispara uma exceção
                if (codigoRetorno > 0)
                    throw new ExcecaoWs() { CodigoErro = codigoRetorno, Fonte = "GerencieExtrato.ExtratoRelatorioPrecoUnico" };

                //Retorna a lista
                return listaRelatorioDetalhadoPrecoUnico;
            }
        }

        /// <summary>
        /// Retorna a linha de cabeçalho do relatório detalhado Preço Único.
        /// </summary>
        /// <returns></returns>
        private String MontarLinhaNomeCamposRelatorioDetalhadoPrecoUnico()
        {
            return String.Concat(
                "\r\n\r\nComprovante de venda ".PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.NumeroSequencialUnico, ' '),
                "TID".PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.NumeroTid, ' '),
                "Nº do Cartão".PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.NumeroCartaoMascarado, ' '),
                "Tipo de venda".PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.TipoVenda, ' '),
                "País de Origem".PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.PaisOrigem, ' '),
                "Hora".PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.HoraVenda, ' '),
                "Tipo de Captura".PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.TipoCaptura, ' '),
                "Data da venda".PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.DataVenda, ' '),
                "Valor da venda".PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.ValorVenda, ' '),
                "Valor do desconto".PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.ValorDesconto, ' '),
                "Valor Liquido".PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.ValorLiquido, ' '),
                "Bandeira".PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.DescricaoBandeira, ' '),
                "Resumo de Vendas".PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.NumeroResumoVenda, ' '),

                "Qtde parcelas".PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.NumeroParcelaDe +
                    (Int32)Constantes.TipoTamCamposPrecoUnico.NumeroParcelaAte, ' '),

                "Vencimento".PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.DataVencimento, ' '),
                "Prazo de recebimento".PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.PrazoPagamento, ' '),
                "Venda Cancelada".PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.StatusVenda, ' '));
        }

        /// <summary>
        /// Monta as linhas do relatório preço único.
        /// </summary>
        /// <param name="listaRelatorioDetalhadoPrecoUnico">Lista de objetos com as informações para gerar as linhas.</param>
        /// <returns></returns>
        private List<String> MontarLinhasRelatorioDetalhadoPrecoUnico(List<RelatorioDetalhadoPrecoUnico> listaRelatorioDetalhadoPrecoUnico)
        {
            var linhasRelatorio = default(List<String>);

            linhasRelatorio = listaRelatorioDetalhadoPrecoUnico.Select(extrato => String.Concat(
                extrato.NumeroSequencialUnico.ToString().PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.NumeroSequencialUnico, ' '),
                extrato.NumeroTid.PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.NumeroTid, ' '),
                extrato.NumeroCartaoMascarado.PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.NumeroCartaoMascarado, ' '),
                extrato.TipoVenda.PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.TipoVenda, ' '),
                extrato.PaisOrigem.PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.PaisOrigem, ' '),

                extrato.HoraVenda.HasValue ? extrato.HoraVenda.Value.ToString("hh:mm:ss", ptBr)
                    .PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.HoraVenda, ' ') : String.Empty,

                extrato.TipoCaptura.PadRight(Convert.ToInt32(Constantes.TipoTamCamposPrecoUnico.TipoCaptura), ' '),
                extrato.DataVenda.HasValue ? extrato.DataVenda.Value.ToString("dd/MM/yyyy", ptBr)
                    .PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.DataVenda, ' ') : String.Empty,

                extrato.ValorVenda.ToString("N2", ptBr).PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.ValorVenda, ' '),
                extrato.ValorDesconto.ToString("N2", ptBr).PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.ValorDesconto, ' '),
                extrato.ValorLiquido.ToString("N2", ptBr).PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.ValorLiquido, ' '),
                extrato.DescricaoBandeira.ToString().PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.DescricaoBandeira, ' '),
                extrato.NumeroResumoVenda.ToString().PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.NumeroResumoVenda, ' '),

                String.Concat(extrato.NumeroParcelaDe, "/", extrato.NumeroParcelaAte)
                    .PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.NumeroParcelaDe +
                    (Int32)Constantes.TipoTamCamposPrecoUnico.NumeroParcelaAte, ' '),

                extrato.DataVencimento.HasValue ? extrato.DataVencimento.Value.ToString("dd/MM/yyyy", ptBr)
                    .PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.DataVencimento, ' ') : String.Empty,

                extrato.PrazoPagamento.ToString().PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.PrazoPagamento, ' '),
                extrato.StatusVenda.PadRight((Int32)Constantes.TipoTamCamposPrecoUnico.StatusVenda, ' '))).ToList();

            linhasRelatorio.Insert(0, MontarLinhaNomeCamposRelatorioDetalhadoPrecoUnico());
            linhasRelatorio.Insert(0, String.Format("\r\n\r\nPeríodo: {0}", hdPeriodo.Value));
            linhasRelatorio.Insert(0, "Relatório Acompanhamento Turquia");

            return linhasRelatorio;
        }

        /// <summary>
        /// Monta uma lista de objetos com as informações de relatório Preço Único.
        /// </summary>
        /// <param name="listaRelatorioDetalhadoPrecoUnico">Lista de objetos "Relatório Preço Único" para montar os registros.</param>
        /// <param name="download">Indica se os registros serão utilizados para download.</param>
        /// <returns></returns>
        private List<Object> MontarRegistrosRelatorioDetalhadoPrecoUnico(List<RelatorioDetalhadoPrecoUnico> listaRelatorioDetalhadoPrecoUnico, Boolean download)
        {
            List<Object> registros = new List<Object>();

            listaRelatorioDetalhadoPrecoUnico.ForEach(item =>
                        registros.Add(new
                        {
                            NumeroSequencialUnico = item.NumeroSequencialUnico,
                            NumeroTid = item.NumeroTid,
                            NumeroCartaoMascarado = item.NumeroCartaoMascarado,
                            TipoVenda = item.TipoVenda,
                            PaisOrigem = item.PaisOrigem,
                            HoraVenda = item.HoraVenda.HasValue ? item.HoraVenda.Value.ToString("hh:mm:ss", ptBr) : String.Empty,
                            TipoCaptura = item.TipoCaptura,
                            DataVenda = item.DataVenda.HasValue ? item.DataVenda.Value.ToString("dd/MM/yyyy", ptBr) : String.Empty,
                            ValorVenda = item.ValorVenda.ToString("N2", ptBr),
                            ValorDesconto = item.ValorDesconto.ToString("N2", ptBr),
                            ValorLiquido = item.ValorLiquido.ToString("N2", ptBr),
                            DescricaoBandeira = item.DescricaoBandeira,
                            NumeroResumoVenda = item.NumeroResumoVenda,
                            QtdParcelas = download ? String.Concat("=\"", item.NumeroParcelaDe.ToString(), "/", item.NumeroParcelaAte.ToString(), "\"") :
                                String.Concat(item.NumeroParcelaDe.ToString(), "/", item.NumeroParcelaAte.ToString()),
                            DataVencimento = item.DataVencimento.HasValue ? item.DataVencimento.Value.ToString("dd/MM/yyyy", ptBr) : String.Empty,
                            PrazoPagamento = item.PrazoPagamento,
                            StatusVenda = item.StatusVenda
                        }));

            return registros;
        }

        /// <summary>
        /// Monta dicionário com informações de nome e nome de exibição das colunas, para relatório Excel.
        /// </summary>
        /// <returns></returns>
        private Dictionary<String, String> MontarColunasRelatorioDetalhadoPrecoUnico()
        {
            Dictionary<String, String> colunas = new Dictionary<String, String>();

            colunas.Add("NumeroSequencialUnico", "Comprovante de venda");
            colunas.Add("NumeroTid", "TID");
            colunas.Add("NumeroCartaoMascarado", "N° do Cartão");
            colunas.Add("TipoVenda", "Tipo de venda");
            colunas.Add("PaisOrigem", "País de Origem");
            colunas.Add("HoraVenda", "Hora");
            colunas.Add("TipoCaptura", "Tipo de Captura");
            colunas.Add("DataVenda", "Data da venda");
            colunas.Add("ValorVenda", "Valor da venda");
            colunas.Add("ValorDesconto", "Valor do desconto");
            colunas.Add("ValorLiquido", "Valor Liquido");
            colunas.Add("DescricaoBandeira", "Bandeira");
            colunas.Add("NumeroResumoVenda", "Resumo de Vendas");
            colunas.Add("QtdParcelas", "Qtde parcelas");
            colunas.Add("DataVencimento", "Vencimento");
            colunas.Add("PrazoPagamento", "Prazo de recebimento");
            colunas.Add("StatusVenda", "Venda Cancelada");

            return colunas;
        }

        /// <summary>
        /// Monta as linhas do Extrato Papel.
        /// </summary>
        /// <param name="listaExtrato">Lista de objetos com as informações para gerar as linhas.</param>
        /// <returns></returns>
        private List<String> MontarLinhasExtratoPapel(List<Extrato> listaExtrato)
        {
            var linhasExtratoRetorno = new List<String>();

            foreach (var item in listaExtrato)
            {
                //linhasExtratoRetorno.Add(string.Concat("TESTE BOX ", item.NumeroBox));
                linhasExtratoRetorno.Add("");
                linhasExtratoRetorno.Add(item.LinhaExtrato);
            }

            return linhasExtratoRetorno;
        }

        /// <summary>
        /// Montar dados para geração de relatório Excel.
        /// </summary>
        /// <typeparam name="T">Tipo de dados apartir do qual que se deseja montar o relatório</typeparam>
        /// <param name="registros">Lista de registros de tipo genérico para montar dados do relatório</param>
        /// <param name="colunas">Dicionário com nome da propriedade de T e nome de exibição da coluna.</param>
        /// <returns></returns>
        private String MontarRelatorioExcel<T>(List<T> registros, Dictionary<String, String> colunas)
        {
            GridView gdvDonwload = new GridView();
            gdvDonwload.DataSource = registros;
            gdvDonwload.AutoGenerateColumns = false;
            String retornoDadosRelatorio;
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            //Define os nomes de exibição das colunas.
            foreach (var coluna in colunas)
                gdvDonwload.Columns.Add(new BoundField { DataField = coluna.Key, HeaderText = coluna.Value });

            gdvDonwload.AllowPaging = false;
            gdvDonwload.DataBind();
            gdvDonwload.RenderControl(hw);

            retornoDadosRelatorio = sw.ToString();

            return retornoDadosRelatorio;
        }

        /// <summary>
        /// Consulta o tipo de oferta Ativa que deve ser exibida para o usuário
        /// Japão, Plano de Contas, Turquia, Sem Oferta, ...
        /// </summary>
        /// <returns>Tipo de oferta ativa</returns>
        private TipoOferta ConsultarTipoOfertaAtiva()
        {
            var codigoRetorno = default(Int16);
            var tipoOferta = default(TipoOferta);
            var numeroPv = SessaoAtual.CodigoEntidade;

            using (Logger log = Logger.IniciarLog("Consulta tipo de oferta ativa"))
            {
                try
                {
                    using (var ctx = new ContextoWCF<HISServicoZP_PlanoContasClient>())
                        codigoRetorno = ctx.Cliente.ConsultarTipoOfertaAtiva(out tipoOferta, numeroPv);
                }
                catch (FaultException<ZPPlanoContasServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    //base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    //base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return codigoRetorno == 0 ? tipoOferta : ZPPlanoContasServico.TipoOferta.SemOferta;
        }

        /// <summary>
        /// Obtém a identificação do relatório para exibição na tabela
        /// </summary>
        /// <param name="valor">Valor da identificação</param>
        /// <param name="dataItem">Objeto DataItem</param>
        /// <returns>Valor a ser exibido na tabela</returns>
        protected static String ObterIdentificacao(Object valor, Object dataItem)
        {
            String identificacao = valor.ToString();
            if (String.Compare("*****", identificacao, true) != 0)
            {
                if (dataItem.GetType() == typeof(ExtratoRelatorioPrecoUnico))
                    return identificacao.ToInt32().ToString("D6").ToDate("yyyyMM").ToString("MMyyyy");
                else
                    return identificacao;
            }
            else
                return "<b>Não existe extrato para o Período</b>";
        }

        #endregion
    }
}