using Microsoft.SharePoint;
using Microsoft.SharePoint.Publishing.Navigation;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.AdministracaoServico;
using Redecard.PN.DadosCadastrais.SharePoint.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace Redecard.PN.DadosCadastrais.SharePoint.Handlers
{
    public partial class Warnings : IHttpHandler, IRequiresSessionState
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
            //get
            //{
            //    if (HttpContext.Current.Session["ListaWarnings"] == null)
            //        HttpContext.Current.Session["ListaWarnings"] = ObterListaWarnings();
            //    return (List<Warning>)HttpContext.Current.Session["ListaWarnings"];
            //}

            //Consulta todos os warnings (utilizando cache appfabric)
            get
            {
                Boolean cacheUp = true;
                List<Warning> warnings = null;

                //verifica se cache esta UP
                try
                {
                    //var cacheWarnings = CacheAdmin.ObterObjetos(Cache.Warnings);
                    //cacheUp = cacheWarnings != null;
                    warnings = CacheAdmin.Recuperar<List<Warning>>(Cache.Warnings, chaveWarningAppFabric);
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

                if (cacheUp)
                {
                    //Int32 tempoCache = 8; // Tempo de expiracao em Horas - agora fica direto na region do appfabric
                    if (warnings == null)
                    {
                        warnings = ObterListaWarnings();
                        CacheAdmin.Adicionar(Cache.Warnings, chaveWarningAppFabric, warnings); //, DateTime.Now.AddHours(tempoCache).Subtract(DateTime.Now)
                    }
                }
                else
                {
                    warnings = ObterListaWarnings();
                }

                return warnings;
            }
        }

        /// <summary>
        /// Itens do warnings
        /// </summary>
        private List<WarningAtendimento> ListaWarningsAtendimento
        {
            get
            {
                return ObterListaWarningAtendimento();
            }
        }

        #endregion

        private String chaveWarningAppFabric = "WarningsPortal";
        private String nomeListaWarningsAtendimento = "WarningAtendimento";

        private JavaScriptSerializer jsSerializer;

        private List<Warning> ObterListaWarnings()
        {
            using (var log = Logger.IniciarLog("Warning - Handler - Leitura e atualização de status da lista 'Warnings' do Sharepoint"))
            {
                try
                {
                    //this.ListaWarnings = new List<Warning>();
                    var listaWarnings = new List<Warning>();
                    SPSite objSite = SPContext.Current.Site;

                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (SPSite sites = new SPSite(SPUtility.GetFullUrl(objSite, "/sites/fechado/")))
                        using (SPWeb web = sites.OpenWeb("minhaconta"))
                        {
                            String nomeLista = "Warnings";
                            SPList lista = web.Lists.TryGetList(nomeLista);

                            web.AllowUnsafeUpdates = true;

                            if (lista != null)
                            {
                                //SPListItemCollection itensSp = lista.GetItems();
                                String dataAtual = DateTime.Now.ToString("yyyy-MM-dd");

                                //Query retornando os itens dentro da data atual.
                                SPQuery query = new SPQuery();
                                query.Query =
                                String.Format(@"<Where>
                                        <And>
                                            <Neq>
                                                <FieldRef Name='Status' />
                                                <Value Type='Text'>Erro</Value>
                                            </Neq>
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
                                        </And>
                                    </Where>", dataAtual);

                                SPListItemCollection itensSp = lista.GetItems(query);

                                //Caso tenha sido encontrado algum item.
                                if (itensSp != null && itensSp.Count > 0)
                                {
                                    bool houveErro = false;
                                    foreach (SPListItem item in itensSp)
                                    {
                                        try
                                        {
                                            listaWarnings.Add(new Warning
                                            {
                                                Segmentos = item["Title"] != null ? item["Title"].ToString().ToLower().Split(';').ToList() : new List<String>(),
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

                                            item["Status"] = "OK";
                                            item["MensagemDeErro"] = "";
                                        }
                                        catch (Exception ex)
                                        {
                                            item["Status"] = "Erro";
                                            item["MensagemDeErro"] = ex.Message;

                                            houveErro = true;
                                        }
                                        finally
                                        {
                                            item.Update();
                                        }
                                        //DataInicial = item["DataInicioExibicao"] != null ? item["DataInicioExibicao"].ToString().Substring(0, 10).ToDate("dd/MM/yyyy", DateTime.MinValue) : DateTime.MinValue,
                                        //DataFinal = item["DataFimExibicao"] != null ? item["DataFimExibicao"].ToString().Substring(0, 10).ToDate("dd/MM/yyyy", DateTime.MaxValue) : DateTime.MaxValue,
                                    }

                                    if (houveErro)
                                        EnviaEmailErroWarning();
                                }
                            }
                        }
                    });
                    return listaWarnings.OrderBy(w => w.DataInicial).ToList();
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    return new List<Warning>();
                }
            }
        }

        /// <summary>
        /// Envia Email para as caixas configuradas na lista WarningEmail (Somente do dominio "userede.com.br")
        /// </summary>
        public void EnviaEmailErroWarning()
        {
            using (var log = Logger.IniciarLog("Warning - Handler - Envio de E-mail de Erro na criação de Cache de Warning"))
            {
                try
                {
                    SPSite objSite = SPContext.Current.Site;

                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (SPSite sites = new SPSite(SPUtility.GetFullUrl(objSite, "/sites/fechado/")))
                        using (SPWeb web = sites.OpenWeb("minhaconta"))
                        {
                            String nomeLista = "WarningEmail";
                            SPList lista = web.Lists.TryGetList(nomeLista);

                            String _htmlBody = "Um ou mais itens da lista de Warning estão gerando erro de leitura.\r\n" +
                                            "É preciso corrigir o problema e limpar o campo Status.";

                            foreach (SPListItem item in lista.Items)
                            {
                                String email = item.Title.Trim();

                                if (email.EndsWith("@userede.com.br"))
                                {
                                    SPUtility.SendEmail(SPContext.Current.Web,
                                        false,
                                        false,
                                        email,
                                        "PN - Warning - Erro na criação de Cache",
                                        _htmlBody
                                        );
                                }
                            }

                        }
                    });

                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw;
                }
            }
        }

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

            //sessao = new Sessao();
            //sessao.CodigoEntidade = 1250191;
            //sessao.UsuarioAtendimento = true;

            if (!SessaoAtual.UsuarioAtendimento)
            {
                //Obtem warning dos PVs
                #region warnings dos PVs
                using (var log = Logger.IniciarLog("Warning - Handler - Obtendo warnings do pv"))
                {
                    try
                    {
                        //Parâmetros de consulta.
                        Int32 pv = SessaoAtual.CodigoEntidade;
                        Char codigoSegmento = SessaoAtual.CodigoSegmento;

                        //Caso o tipo do warning seja AceiteOferta,
                        //Salvar o PV, a data atual e o identificador em outra lista do Sharepoint.
                        if (context.Request.Params["AceiteOferta"] == "true")
                        {
                            SalvarAceite(context, pv, DateTime.Now.ToString("yyyy-MM-dd"));
                            return;
                        };

                        //Caso tenha o parametro para limpara o appfabric
                        if (context.Request.Params["ClearAppFabric"] == "true")
                        {
                            CacheAdmin.Remover(Cache.Warnings, chaveWarningAppFabric);
                        };

                        //Url relativa (/sites/fechado...)
                        //String url = new Uri(context.Request.Params["urlExibicao"]).LocalPath;

                        //Deixando somente a url a partir do Paginas/ ou de algum subsite (minhaconta, extrato) .
                        //url = url.Replace("/sites/fechado/", String.Empty);

                        //Obtem os itens na lista de warnings que pertencem ao PV
                        var itens = this.ListaWarnings.Where(w =>
                            (w.Segmentos.Contains(codigoSegmento.ToString().ToLower()) || w.Segmentos.Count == 0) &&
                            (w.Pvs.Contains(pv) || w.Pvs.Count == 0) &&
                                //(w.UrlWarning.Contains(url) || String.IsNullOrWhiteSpace(w.UrlWarning)) &&
                            (w.DataInicial.Date <= DateTime.Now.Date && w.DataFinal.Date >= DateTime.Now.Date)
                        );

                        //Caso não tenha sido encontrado nenhum item.
                        if (itens == null)
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
                                Dados = itens.Select(item => new
                                {
                                    Tipo = item.Tipo,
                                    DataFimExibicao = item.DataFinal.ToString("dd/MM/yyyy"),
                                    Texto = item.Texto,
                                    Url = item.UrlWarning,
                                    UrlDestino = item.UrlDestino,
                                    TextoBotao = item.TextoBotao,
                                    IdentificadorAceite = item.IdentificadorAceite
                                }).ToList()
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
                #endregion
            }
            else
            {
                //Obtem warning de atendimento (exclusivo para central de atendimento)
                //Exibirá as atividades recentes do PV de acordo com os templates na lista minhaconta/WarningsAtendimento
                #region warning de atendimento
                using (var log = Logger.IniciarLog("Warning - Handler - Obtendo Warning de Atendimento"))
                {
                    try
                    {
                        //Parâmetros de consulta.
                        Int32 pv = SessaoAtual.CodigoEntidade;

                        //Obtem os itens na lista de WarningAtendimento
                        var itensTemplate = this.ListaWarningsAtendimento.ToList();

                        //Caso não tenha sido encontrado nenhum item.
                        if (itensTemplate == null)
                        {
                            context.Response.Write(jsSerializer.Serialize(new { Retorno = 2, Mensagem = "Não foram retornados dados." }));
                            return;
                        }

                        //Concatena os códigos de atividade e dias de histórico para consulta dos logs
                        String codigosDias = String.Join(
                            ";", itensTemplate.Select(item => String.Concat(item.CodigoTipoAtividade, ",", item.DiasHistorico))
                            );

                        //Obtem atividades recentes do PV selecionado e faz parse com as mensagens template
                        var itensWarning = RealizarParseAtividades(itensTemplate, pv, codigosDias);

                        //Serializando os dados em JSON e enviando para o front-end.
                        context.Response.Write(jsSerializer.Serialize(
                            new
                            {
                                Retorno = 0,
                                Mensagem = "OK",
                                Dados = itensWarning.Select(item => new
                                {
                                    Tipo = item.Tipo,
                                    DataFimExibicao = item.DataFinal.ToString("dd/MM/yyyy"),
                                    Texto = item.Texto,
                                    Url = item.UrlWarning,
                                    UrlDestino = item.UrlDestino,
                                    TextoBotao = item.TextoBotao,
                                    IdentificadorAceite = item.IdentificadorAceite
                                }).ToList()
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
                #endregion
            }
        }

        #region Métodos de tratamento do warning dos PVs

        /// <summary>
        /// Método para salvar os dados do Aceite do warning na lista WarningAceite
        /// </summary>
        /// <param name="context">Parâmetros HTTP.</param>
        /// <param name="pv">Número do PV</param>
        /// <param name="dataAtual">Data Atual</param>
        private void SalvarAceite(HttpContext context, Int32 pv, String dataAtual)
        {
            SPList listaAceite = null;
            String nomeListaAceite = "WarningAceite";

            //É necessário obter a Web e o Site fora do Elevated Privileges, do contrário é lançada uma exceção.
            SPSite site = SPContext.Current.Site;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite sites = new SPSite(SPUtility.GetFullUrl(site, "/sites/fechado/")))
                using (SPWeb newWeb = sites.OpenWeb("minhaconta"))
                {
                    // Flag necessária para alterar dados de uma lista através de um request GET.
                    bool allowUnsafeUpdates = newWeb.AllowUnsafeUpdates;
                    newWeb.AllowUnsafeUpdates = true;
                    listaAceite = newWeb.Lists.TryGetList(nomeListaAceite);

                    if (listaAceite != null)
                    {
                        //Criando um novo item e associando os dados.
                        SPListItem item = listaAceite.AddItem();
                        item["NumeroDoPv"] = pv;
                        item["Data"] = dataAtual;
                        item["IdentificadorAceite"] = context.Request.Params["IdentificadorAceite"];

                        //Atualizando a lista com o novo item.
                        item.Update();

                        context.Response.Write(jsSerializer.Serialize(
                            new
                            {
                                Retorno = 4,
                                Mensagem = "Dados Aceite salvos com sucesso.",
                            }
                        ));

                        //Setando a flag como false novamente.
                        newWeb.AllowUnsafeUpdates = allowUnsafeUpdates;
                        return;
                    }
                    else
                    {
                        context.Response.Write(jsSerializer.Serialize(new { Retorno = 3, Mensagem = "Lista de aceite dos warnings não encontrada" }));
                        newWeb.AllowUnsafeUpdates = allowUnsafeUpdates;
                        return;
                    }
                }
            });
        }

        #endregion

        #region Metodos de tratamento dos warnings de atendimento
        /// <summary>
        /// Realiza o parse dos logs de atendimento com os templates que devem ser exibidos
        /// </summary>
        /// <param name="itensTemplate"></param>
        /// <param name="pv"></param>
        /// <param name="codigosDias"></param>
        /// <returns></returns>
        private List<Warning> RealizarParseAtividades(List<WarningAtendimento> itensTemplate, Int32 pv, String codigosDias)
        {
            AdministracaoServicoClient client = new AdministracaoServicoClient();
            List<AtividadeRecente> atividades =
                client.ConsultarAtividadesRecentesPV(pv, codigosDias).ToList();

            List<Warning> itens = new List<Warning>();

            foreach (var atividade in atividades)
            {
                var itemTemplate = itensTemplate.Where(n => n.CodigoTipoAtividade == atividade.CodigoAtividade).FirstOrDefault();

                if (itemTemplate != null)
                {
                    itens.Add(new Warning()
                    {
                        Tipo = "Erro",
                        DataFinal = new DateTime(2099, 12, 31),
                        Texto = itemTemplate.Texto.Replace("##datahora##", atividade.Data.ToString("dd/MM/yyyy hh:mm"))
                            .Replace("##detalhevalor##", atividade.Detalhes)
                            .Replace("##detalhesemvalor##", Regex.Replace(atividade.Detalhes, @"\(.*?\)", "").Replace("  ", " ").Replace(" , ", ", ").Trim())
                            .Replace("##email##", atividade.Email)
                            .Replace("##IP##", atividade.IP)
                            .Replace("##nomeusuario##", atividade.NomeUsuario)
                            .Replace("##PV##", atividade.PV)
                        ,
                        UrlWarning = @"/homespa/index.html",
                        UrlDestino = itemTemplate.UrlDestino,
                        TextoBotao = itemTemplate.TextoBotao,
                        IdentificadorAceite = "WARNING_ATENDIMENTO"
                    });
                }
            }

            return itens;
        }
        /// <summary>
        /// Obtem os warnings de atendimento ativos na lista
        /// </summary>
        /// <returns></returns>
        private List<WarningAtendimento> ObterListaWarningAtendimento()
        {
            using (var log = Logger.IniciarLog("Warning - Handler - Leitura e atualização de status da lista 'WarningAtendimento' do Sharepoint"))
            {
                SPSite objSite = SPContext.Current.Site;

                try
                {
                    var listaWarningAtendimento = new List<WarningAtendimento>();

                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (SPSite sites = new SPSite(SPUtility.GetFullUrl(objSite, "/sites/fechado/")))
                        using (SPWeb web = sites.OpenWeb("minhaconta"))
                        {
                            SPList lista = web.Lists.TryGetList(nomeListaWarningsAtendimento);

                            if (lista != null)
                            {
                                //Obtem os itens de warning ativos - no SP2010 ou SP2013 migrado usa-se integer no campo boolean
                                SPQuery query = new SPQuery();
                                query.Query = @"<Where>
                                        <Eq>
                                            <FieldRef Name='Ativo' />
                                            <Value Type='Integer'>1</Value>
                                        </Eq>
                                    </Where>";

                                SPListItemCollection itensSp = lista.GetItems(query);

                                //Caso tenha sido encontrado algum item.
                                if (itensSp != null && itensSp.Count > 0)
                                {
                                    foreach (SPListItem item in itensSp)
                                    {
                                        listaWarningAtendimento.Add(new WarningAtendimento
                                        {
                                            CodigoTipoAtividade = item["Title"] != null ? Convert.ToInt32(item["Title"]) : 0,
                                            Texto = item["Texto"] != null ? item["Texto"].ToString() : String.Empty,
                                            TextoBotao = item["TextoBotao"] != null ? item["TextoBotao"].ToString() : null,
                                            UrlDestino = item["UrlDestino"] != null ? item["UrlDestino"].ToString() : null,
                                            DiasHistorico = item["DiasHistorico"] != null ? Convert.ToInt32(item["DiasHistorico"]) : 0,
                                            Ativo = item["Ativo"] != null ? Convert.ToBoolean(item["Ativo"]) : false,
                                            ExibirBotao = item["ExibirBotao"] != null ? Convert.ToBoolean(item["ExibirBotao"]) : false
                                        });
                                    }
                                }
                            }
                        }
                    });
                    return listaWarningAtendimento.ToList();
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    return new List<WarningAtendimento>();
                }
            }
        }
        #endregion

    }
}
