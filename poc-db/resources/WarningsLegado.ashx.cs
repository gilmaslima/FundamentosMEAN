using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace Redecard.PN.DadosCadastrais.SharePoint.Handlers
{
    public partial class WarningsLegado : IHttpHandler, IRequiresSessionState
    {
        #region [ Propriedades ]
        /// <summary>Valor padrão de propriedades</summary>
        public bool IsReusable { get { return false; } }

        private Sessao sessao = null;
        private Sessao SessaoAtual
        {
            get
            {
                if (sessao != null && Sessao.Contem())
                    return sessao;
                else
                {
                    if (Sessao.Contem())
                    {
                        sessao = Sessao.Obtem();
                    }
                    return sessao;
                }
            }
        }

        /// <summary>
        /// Itens do warnings
        /// </summary>
        private List<Warning> ListaWarnings
        {
            //Consulta todos os warnings (utilizando cache)
            get
            {
                //verifica se cache esta UP
                Boolean cacheUp = false;
                try
                {
                    var cacheGeral = CacheAdmin.ObterObjetos(Cache.Geral);
                    cacheUp = cacheGeral != null;
                }
                catch (PortalRedecardException ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    cacheUp = false;
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    cacheUp = false;
                }

                List<Warning> warnings = null;
                if (cacheUp)
                {
                    String chave = "WarningsPortal";
                    Int32 tempoCache = 8; // Tempo de expiracao em Horas
                    warnings = CacheAdmin.Recuperar<List<Warning>>(chave);

                    if (warnings == null)
                    {
                        warnings = ObterListaWarnings();
                        CacheAdmin.Adicionar(chave, warnings, DateTime.Now.AddHours(tempoCache).Subtract(DateTime.Now));
                    }
                }
                else
                {
                    warnings = ObterListaWarnings();
                }

                return warnings;
            }
        }

        private List<Warning> ObterListaWarnings()
        {
            var listaWarnings = new List<Warning>();

            using (var log = Logger.IniciarLog("Warning - Handler - Leitura da lista 'Warnings' do Sharepoint"))
            using (SPSite sites = new SPSite(SPUtility.GetFullUrl(SPContext.Current.Site, "/sites/fechado/")))
            using (SPWeb web = sites.OpenWeb("minhaconta"))
            {
                String nomeLista = "Warnings";
                SPList lista = web.Lists.TryGetList(nomeLista);

                if (lista != null)
                {
                    //SPListItemCollection itensSp = lista.GetItems();
                    String dataAtual = DateTime.Now.ToString("yyyy-MM-dd");

                    //Query retornando os itens dentro da data atual.
                    SPQuery query = new SPQuery();
                    query.Query =
                    String.Format(@"<Where>
                                        <And>
                                            <Leq>
                                                <FieldRef Name='DataInicioExibicao' />
                                                <Value Type='DateTime'>{0}</Value>
                                            </Leq>
                                            <Geq>
                                                <FieldRef Name='DataFimExibicao' />
                                                <Value Type='DateTime'>{0}</Value>
                                            </Geq>
                                        </And>
                                    </Where>", dataAtual);

                    SPListItemCollection itensSp = lista.GetItems(query);

                    //Caso tenha sido encontrado algum item.
                    if (itensSp != null && itensSp.Count > 0)
                    {
                        foreach (SPListItem item in itensSp)
                        {
                            listaWarnings.Add(new Warning
                            {
                                Segmentos = item["Title"] != null ? item["Title"].ToString().Split(';').ToList() : new List<String>(),
                                Pvs = item["NumeroDoPv"] != null ? item["NumeroDoPv"].ToString().Split(';').Select(Int32.Parse).ToList() : new List<Int32>(),
                                Texto = item["Texto"] != null ? item["Texto"].ToString() : String.Empty,
                                Tipo = item["Tipo"] != null ? item["Tipo"].ToString() : String.Empty,
                                UrlWarning = item["UrlExibicao"] != null ? item["UrlExibicao"].ToString() : String.Empty,
                                TextoBotao = item["TextoBotao"] != null ? item["TextoBotao"].ToString() : null,
                                UrlDestino = item["UrlDestino"] != null ? item["UrlDestino"].ToString() : null,
                                DataInicial = item["DataInicioExibicao"] != null ? item["DataInicioExibicao"].ToString().ToDate() : DateTime.MinValue,
                                DataFinal = item["DataFimExibicao"] != null ? item["DataFimExibicao"].ToString().ToDate() : DateTime.MaxValue,
                                IdentificadorAceite = item["IdentificadorAceite"] != null ? item["IdentificadorAceite"].ToString() : null
                            });
                        }
                    }
                }
            }
            return listaWarnings.OrderBy(w => w.DataInicial).ToList();
        }

        private JavaScriptSerializer jsSerializer;
        #endregion

        /// <summary>
        /// Executa o serviço que valida as transações
        /// </summary>
        /// <param name="context">Contexto Http</param>
        public void ProcessRequest(HttpContext context)
        {
            /*
            Retornos 
            0 = Retorno Warnings OK.
            1 = Lista de warnings não encontrada
            2 = Não foram retornados dados.
            3 = Lista de aceite dos warnings não encontrada.
            4 = Dados Aceite salvos com sucesso.
            99 = Exceção não tratada.
            */

            jsSerializer = new JavaScriptSerializer();
            context.Response.ContentType = "application/json";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            using (var log = Logger.IniciarLog("Warning - Handler - Obtendo warning especifico"))
            {
                try
                {
                    //Parâmetros de consulta.
                    Int32 pv = SessaoAtual.CodigoEntidade;
                    Char codigoSegmento = SessaoAtual.CodigoSegmento;

                    //Url relativa (/sites/fechado...)
                    String url = new Uri(context.Request.Params["urlExibicao"]).LocalPath;

                    //Deixando somente a url a partir do Paginas/ ou de algum subsite (minhaconta, extrato) .
                    url = url.Replace("/sites/fechado/", String.Empty);

                    //Obtem o item na lista de warnings
                    var item = this.ListaWarnings.FirstOrDefault(w =>
                        (w.Segmentos.Contains(codigoSegmento.ToString()) || w.Segmentos.Count == 0) &&
                        (w.Pvs.Contains(pv) || w.Pvs.Count == 0) &&
                        (w.UrlWarning.Contains(url) || String.IsNullOrWhiteSpace(w.UrlWarning)) &&
                        (w.DataInicial.Date <= DateTime.Now.Date && w.DataFinal.Date >= DateTime.Now.Date)
                    );

                    //Caso não tenha sido encontrado nenhum item.
                    if (item == null)
                    {
                        context.Response.Write(jsSerializer.Serialize(new { Retorno = 2, Mensagem = "Não foram retornados dados." }));
                        return;
                    }

                    //Serializando os dados em JSON e enviando para o front-end.
                    context.Response.Write(jsSerializer.Serialize(
                        new
                        {
                            Retorno = 0,
                            Mensagem = "OK",
                            Dados = new
                            {
                                Tipo = item.Tipo,
                                DataFimExibicao = item.DataFinal.ToString("dd/MM/yyyy"),
                                Texto = item.Texto,
                                UrlDestino = item.UrlDestino,
                                TextoBotao = item.TextoBotao,
                                IdentificadorAceite = item.IdentificadorAceite
                            }
                        }
                    ));
                    return;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);

                    context.Response.Write(jsSerializer.Serialize(new
                    {
                        Retorno = 99,
                        MensagemErro = ex.Message
                    }));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);

                    context.Response.Write(jsSerializer.Serialize(new
                    {
                        Retorno = 99,
                        MensagemErro = ex.Message
                    }));
                }
            }
        }
    }
}
