using Microsoft.SharePoint;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;
using Redecard.PN.DadosCadastrais.SharePoint.Login.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Redecard.PN.DadosCadastrais.SharePoint.Login
{
    public enum TipoOrigem
    {
        Cache,
        Lista
    }

    /// <summary>
    /// Classe responsável por gerenciar os dados de configuração do tipo de login
    /// </summary>
    public class ConfiguracaoLogin
    {
        /// <summary>
        /// Chave de cache para configuração do login
        /// </summary>
        private const string chaveCache = "__configAPILogin";

        /// <summary>
        /// Verifica se o servidor validando o login deve utilizar a nova API ou o WCF
        /// </summary>
        /// <returns></returns>
        public static bool UtilizaApiLogin()
        {
            return true;
        }

        /// <summary>
        /// Adiciona a configuração de login no cache de Autosservicos
        /// </summary>
        /// <param name="dadosConfiguracao"></param>
        public static void AdicionaConfiguracaoCache(List<ConfiguracaoLoginRetorno> dadosConfiguracao)
        {
            CacheAdmin.Remover(Comum.Cache.AutosServicosLogin, chaveCache);

            CacheAdmin.Adicionar<List<ConfiguracaoLoginRetorno>>(Comum.Cache.AutosServicosLogin, chaveCache, dadosConfiguracao);
        }

        /// <summary>
        /// Consulta os dados da configuração de login na lista do Sharepoint ou em cache
        /// </summary>
        /// <param name="origem"></param>
        /// <returns></returns>
        public static List<ConfiguracaoLoginRetorno> ConsultarConfiguracaoLogin(TipoOrigem origem)
        {
            List<ConfiguracaoLoginRetorno> dadosConfiguracao = new List<ConfiguracaoLoginRetorno>();

            if (origem == TipoOrigem.Lista)
            {
                SPList listaConfiguracaoLogin = SPContext.Current.Web.Lists.TryGetList("Configuracao Login");

                if (listaConfiguracaoLogin != null)
                {
                    dadosConfiguracao = listaConfiguracaoLogin.Items.Cast<SPListItem>().Select(item => new ConfiguracaoLoginRetorno
                    {
                        Servidor = item.Title,
                        UtilizaApiLogin = Convert.ToBoolean(item["UtilizaLoginApi"])
                    }).ToList();
                }
            }
            else
            {
                dadosConfiguracao = CacheAdmin.Recuperar<List<ConfiguracaoLoginRetorno>>(Comum.Cache.AutosServicosLogin, chaveCache);
            }

            return dadosConfiguracao;
        }

        /// <summary>
        /// Converte o modelo de retorno da entidade retornada pelo WCF para o modelo de entidade da API
        /// </summary>
        /// <param name="entidadesWcf"></param>
        /// <returns></returns>
        public static List<PortalApi.Modelo.EntidadeRetorno> ConverteParaEntidadeApi(List<Entidade1> entidadesWcf)
        {
            List<PortalApi.Modelo.EntidadeRetorno> entidadesApi = new List<PortalApi.Modelo.EntidadeRetorno>();

            entidadesWcf.ForEach(ent =>
            {
                entidadesApi.Add(new PortalApi.Modelo.EntidadeRetorno
                {
                    Codigo = ent.Codigo,
                    CodigoGrupoEntidade = 1,
                    Nome = ent.Descricao,
                    DataAlteracao = ent.DataAlteracao.ToString(),
                    DataAlteracaoDt = ent.DataAlteracao
                });
            });

            return entidadesApi;
        }
    }
}
