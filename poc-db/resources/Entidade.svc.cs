using Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Response;
using Rede.PN.Conciliador.SharePoint.EntidadeServico;
using Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Contratos;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Rede.PN.Conciliador.SharePoint.ConciliadorServicos
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Entidade : IEntidade
    {
        /// <summary>
        /// 
        /// </summary>
        public Int32 CodigoEntidade
        {
            get
            {
                Sessao sessaoUsuario = System.Web.HttpContext.Current.Session[Sessao.ChaveSessao] as Sessao;
                if (sessaoUsuario != null)
                {
                    return sessaoUsuario.CodigoEntidade;
                }
                throw new Exception("Código da entidade inválido");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String NomeEntidade
        {
            get
            {
                Sessao sessaoUsuario = System.Web.HttpContext.Current.Session[Sessao.ChaveSessao] as Sessao;
                if (sessaoUsuario != null)
                {
                    return sessaoUsuario.NomeEntidade;
                }
                throw new Exception("Nome da entidade inválido");
            }
        }

        /// <summary>
        /// Serviço que recupera a lista de entidades
        /// </summary>
        /// <returns></returns>
        public ListaEntidadesResponse ConsultarFiliais()
        {
            using (Logger log = Logger.IniciarLog("ConciliadorServicos - ConsultaFiliais - REST"))
            {
                log.GravarLog(EventoLog.InicioServico);

                ListaEntidadesResponse entidadeResponse = null;

                try
                {
                    using (EntidadeServico.EntidadeServicoClient client = new EntidadeServico.EntidadeServicoClient())
                    {
                        int codigoRetorno = 0;
                        entidadeResponse = new ListaEntidadesResponse();

                        Filial[] filiais = client.ConsultarFiliais(this.CodigoEntidade, 2, out codigoRetorno);
                        if (filiais.Length > 0)
                        {
                            List<ItemEntidadeResponse> entidades = filiais.Select(filial => new ItemEntidadeResponse
                                {
                                    Categoria = filial.Categoria,
                                    Centralizador = filial.Centralizador,
                                    Matriz = filial.Matriz,
                                    Moeda = filial.Moeda,
                                    NomeComercial = filial.NomeComerc,
                                    PontoVenda = filial.PontoVenda,
                                    TipoEstabelecimento = filial.TipoEstab
                                }).ToList();
                            entidades.Add(new ItemEntidadeResponse() { NomeComercial = this.NomeEntidade, PontoVenda = this.CodigoEntidade });

                            entidadeResponse.TotalItens = filiais.Length + 1;
                            entidadeResponse.Itens = entidades.ToArray();
                        }
                        else
                        {
                            entidadeResponse.TotalItens = 1;
                            entidadeResponse.Itens = new ItemEntidadeResponse[1] { new ItemEntidadeResponse() { NomeComercial = this.NomeEntidade, PontoVenda = this.CodigoEntidade } };
                        }
                    }

                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    throw ex;
                }

                log.GravarLog(EventoLog.FimServico);

                return entidadeResponse;
            }
        }
    }
}