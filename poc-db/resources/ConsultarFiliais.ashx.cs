using System;
using Microsoft.SharePoint;
using System.Web;
using System.Web.Script.Serialization;
using System.ServiceModel;
using Redecard.PN.Comum.SharePoint.EntidadeServico;
using System.Collections.Generic;
using System.Linq;
using System.Web.SessionState;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;

namespace Redecard.PN.Comum.SharePoint.Handlers
{
    public partial class ConsultarFiliais : UserControlBase, IHttpHandler, IReadOnlySessionState
    {
        private class ConsultaFiliaisException : Exception
        {                        
            public String Mensagem { get; set; }

            public ConsultaFiliaisException(String mensagem)
            {                
                this.Mensagem = mensagem;
            }
        }

        public enum RetornosServicoConsultaFiliais
        {
            EstabelecimentoNaoCentralizador = 60201,
            EstabelecimentoNaoMatriz = 60200,
            EstabelecimentoNaoConsignador = 60202,
            EstabelecimentoNaoFilial = 60040,
            EstabelecimentoNaoEncontrado = 60006
        }
    
        private static JavaScriptSerializer _jsSerializer;
        private static JavaScriptSerializer JsSerializer
        {
            get
            {
                if (_jsSerializer == null)
                    _jsSerializer = new JavaScriptSerializer();
                return _jsSerializer;
            }
        }

        private new String FONTE = "Redecard.PN.Comum.SharePoint.Handlers.ConsultarFiliais";

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            using (Logger Log = Logger.IniciarLog("Consulta de Filiais"))
            {
                try
                {
                    //recupera parâmetros da consulta (se for -1, significa que conversão é inválida)
                    String strTipoAssociacao = context.Request["tipoAssociacao"];
                    Int32 tipoAssociacao = strTipoAssociacao.ToInt32(-1);
                    String origem = context.Request["origem"];

                    #region Validação dos parâmetros informados

                    //Valida se usuário está autenticado
                    if (SessaoAtual == null)
                        throw new ConsultaFiliaisException("Acesso negado. Usuário não autenticado.");

                    //Valida tipo de associação
                    if (tipoAssociacao == -1)
                        throw new ConsultaFiliaisException(String.Format("Tipo de Associação informado é inválido ({0}).", strTipoAssociacao));

                    #endregion

                    //Consulta filiais
                    List<Filial> filiais = this.ObterFiliais(tipoAssociacao);

                    //Se solicitação veio de GerencieExtrato, aplica filtro customizado
                    if (origem == ConsultaPV.OrigemSolicitacao.GerencieExtratoInibicao.ToString())
                        filiais = FiltroGerencieExtrato(filiais);

                    //Monta mensagem de retorno       
                    String mensagemRetorno = String.Empty;
                    if (tipoAssociacao != 0 && filiais.Count(filial => filial.PontoVenda != SessaoAtual.CodigoEntidade) == 0)
                    {
                        if (tipoAssociacao == 1)
                            mensagemRetorno = "Estabelecimento não possui Centralizados.";
                        else if (tipoAssociacao == 2)
                            mensagemRetorno = "Estabelecimento não possui Filiais.";
                        else if (tipoAssociacao == 3)
                            mensagemRetorno = "Estabelecimento não possui Consignados.";
                        else if (tipoAssociacao == 4)
                            mensagemRetorno = "Estabelecimento não possui outros PVs com mesmo CNPJ.";
                        else
                            mensagemRetorno = "Estabelecimentos não encontrados para o tipo de associação escolhido.";
                    }

                    var filiaisRetorno = filiais.Select(
                        filial => new
                        {
                            CT = filial.Categoria,
                            MD = filial.Moeda,
                            NC = filial.NomeComerc,
                            PV = filial.PontoVenda,
                            CH = Criptografia.CriptografarPV(filial.PontoVenda, SessaoAtual.CodigoEntidade)
                        }).ToList();

                    //Monta retorno contendo filiais
                    var retorno = new
                    {
                        CodigoRetorno = 0,
                        Fonte = "",
                        MensagemRetorno = mensagemRetorno,
                        Filiais = JsSerializer.Serialize(filiaisRetorno),
                        QuantidadeFiliais = filiaisRetorno.Count
                    };

                    context.Response.Write(JsSerializer.Serialize(retorno));
                    context.Response.ContentType = "application/json";
                    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                }
                catch (ConsultaFiliaisException ex)
                {
                    Log.GravarErro(ex);
                    GeraJSONExcecao(context, FONTE, -1, ex.Mensagem);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    GeraJSONExcecao(context, ex.Fonte, ex.Codigo, String.Empty);
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    GeraJSONExcecao(context, ex.Detail.Fonte, ex.Detail.Codigo, String.Empty);
                }
                catch (FaultException<GerencieExtratoServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    GeraJSONExcecao(context, ex.Detail.Fonte, ex.Detail.Codigo, String.Empty);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    GeraJSONExcecao(context, FONTE, CODIGO_ERRO, "Sistema indispoível.");
                }
            }
        }

        private void GeraJSONExcecao(HttpContext context, String fonte, Int32 codigo, String mensagem)
        {
            Int32 codigoErro = -1;
            String mensagemErro = "Sistema Indisponível";

            if (String.IsNullOrEmpty(mensagem))
            {
                var trataErro = this.VerificaErroServico(fonte, codigo);
                if (trataErro != null && trataErro.Codigo != 0)
                {
                    codigoErro = trataErro.Codigo;
                    mensagemErro = trataErro.Fonte;
                }
            }

            //Monta retorno de erro               
            var retorno = new
            {
                CodigoRetorno = codigoErro,
                Fonte = fonte,
                MensagemRetorno = mensagemErro
            };
            context.Response.Write(JsSerializer.Serialize(retorno));
            context.Response.ContentType = "application/json";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }

        /// <summary>Busca a mensagem de erro</summary>        
        private Redecard.PN.Comum.TrataErroServico.TrataErro VerificaErroServico(String fonte, Int32 codigo)
        {
            try
            {
                using (TrataErroServico.TrataErroServicoClient trataErroServico = new TrataErroServico.TrataErroServicoClient())
                {
                    return trataErroServico.Consultar(fonte, codigo);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Consulta as filiais do estabelecimento, de acordo com o tipo de associação.
        /// Exceções são encapsuladas no tipo ConsultaFiliaisException.
        /// </summary>
        /// <param name="tipoAssociacao">Tipo de associação</param>        
        /// <param name="fonte">Fonte em que ocorreu o erro, se ocorrido</param>
        /// <param name="codigoRetorno">Código de retorno. Se diferente de 0, erro</param>
        /// <returns>Lista de Filiais</returns>
        private List<Filial> ObterFiliais(Int32 tipoAssociacao)
        {
            //variável de retorno
            List<Filial> filiais = new List<Filial>();

            //Verifica qual o tipo de associação. Se for próprio, monta entidade fake sem necessidade de consulta
            if (tipoAssociacao == 0)
            {
                filiais.Add(Propria());
                return filiais;
            }

            //Instancia o serviço de entidade para consulta dos PVs associados à entidade
            using (EntidadeServicoClient servicoEntidade = new EntidadeServicoClient())
            {                
                    //Chama o serviço entidade, buscando as filiais
                    Int32 codigoRetorno;
                    List<Filial> entidades = servicoEntidade.ConsultarFiliais(
                        out codigoRetorno, SessaoAtual.CodigoEntidade, tipoAssociacao).ToList();

                    //COMENTADO: independente do código de retorno, não exibe exceção para o usuário
                    //Verifica código de retorno
                    //if (codigoRetorno != 0)
                    //{
                    //    Sai da rotina, em caso de erro
                    //    if (!Enum.IsDefined(typeof(RetornosServicoConsultaFiliais), codigoRetorno))
                    //    throw new PortalRedecardException(codigoRetorno, "EntidadeServico.ConsultarFiliais");
                    //}

                    if (entidades == null) entidades = new List<Filial>();
                    
                    //Reordena a lista de PVs
                    entidades.OrderBy(entidade => entidade.PontoVenda).ThenBy(entidade => entidade.NomeComerc).ToList();

                    // Insere o PV atual na lista caso não tenha sido retornado na consulta
                    if (!entidades.Any(entidade => entidade.PontoVenda == SessaoAtual.CodigoEntidade))
                        entidades.Insert(0, Propria());

                    //Atribui consulta para lista de filiais que serão retornadas
                    filiais = entidades;
                    codigoRetorno = 0;
            }

#if DEBUG            
            //gerando filiais fakes
            for (Int32 iFilial = 0, total = filiais.Count; iFilial < total; iFilial++)
                filiais.Add(filiais[iFilial].ObterCopia());
            for (Int32 iFilial = 0, total = filiais.Count; iFilial < total; iFilial++)
                filiais.Add(filiais[iFilial].ObterCopia());
                       
            //altera códigos PV para aleatórios (repetição de PVs em ambiente dev)
            int nextPV = filiais.Max(filial => filial.PontoVenda) + 1;
            var pvsDuplicados = filiais
                .GroupBy(filial => filial.PontoVenda) //agrupa por PV
                .Where(grupo => grupo.Count() > 1) //apenas grupos com repetição de PV
                .Select(g => g.Key).ToList(); //seleciona o código PV duplicado
            pvsDuplicados.ForEach(pvDuplicado => //para cada código PV duplicado
            {
                filiais.Where(filial => filial.PontoVenda == pvDuplicado) //busca as filiais do pv duplicado
                    .Skip(1).ToList() //a partir do segundo elemento
                    .ForEach(filial => filial.PontoVenda = nextPV++); //atribui um novo PV (+1 do maior PV)
            });
            //randomiza dados
            filiais.Where(filial => filial.PontoVenda != SessaoAtual.CodigoEntidade)
                .ToList().ForEach(filial =>
            {
                filial.Moeda = filial.PontoVenda.ToString().EndsWith("1") ? "D" : "R";
                filial.Categoria = filial.PontoVenda % 11 == 0 ? "X" : "A";
            });
#endif
            return filiais;
        }

        /// <summary>
        /// Filtro customizado utilizado pela tela GerencieExtrato
        /// </summary>        
        /// <returns>Lista de filiais filtrada</returns>
        private List<Comum.SharePoint.EntidadeServico.Filial> FiltroGerencieExtrato(List<Comum.SharePoint.EntidadeServico.Filial> filiais)
        {
            List<Comum.SharePoint.EntidadeServico.Filial> retorno = filiais ?? new List<Comum.SharePoint.EntidadeServico.Filial>();

                List<GerencieExtratoServico.StatusEmissao> lstSolicita = new List<GerencieExtratoServico.StatusEmissao>();
                GerencieExtratoServico.StatusEmissao item;
                Int16 codigoRetorno = 0;
                String mensagem = String.Empty;

                foreach (Comum.SharePoint.EntidadeServico.Filial filial in filiais)
                {
                    item = new GerencieExtratoServico.StatusEmissao();
                    item.PontoVenda = filial.PontoVenda.ToString();
                    item.SituacaoCobranca = " ";
                    item.Status = " ";
                    item.CodigoRetornoPV = 0;
                    lstSolicita.Add(item);
                }
                using (GerencieExtratoServico.GerencieExtratoClient client = new GerencieExtratoServico.GerencieExtratoClient())
                {
                    client.ObterExtratoPapel(ref lstSolicita, ref codigoRetorno, ref mensagem);
                    if (codigoRetorno > 0)
                        throw new PortalRedecardException(codigoRetorno, "GerencieExtratoServico.ObterExtratoPapel");
                    var pvFiltrado = lstSolicita.Where(x => x.Status == "E").Select(x => x.PontoVenda).ToList();
                    retorno = filiais.Where(x => pvFiltrado.Contains(x.PontoVenda.ToString())).ToList();
                    return retorno;
                }
        }

        /// <summary>Monta objeto "fake" representando a própria entidade.</summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <returns>Entidade própria</returns>        
        private Filial Propria()
        {
            return new Filial
            {
                PontoVenda = SessaoAtual.CodigoEntidade,
                NomeComerc = "PRÓPRIO",
                Categoria = "A",
                Moeda = SessaoAtual.TransacionaDolar ? "D" : "R"                
            };
        }
    }
}
