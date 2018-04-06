using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Comum;
using Redecard.PN.Extrato.Modelo.Comum;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Negocio
{
    public abstract class AbstractPesquisaSemTotalizadorBLL<TEnvioDTO, TRetornoDTO> : RegraDeNegocioBase
    {


        #region Delegates
        public delegate List<TRetornoDTO> PreFiltro<TParam>(List<TRetornoDTO> entrada, TParam parametros);
        #endregion
    
#if DEBUG
        /// <summary>
        /// Controla cache - Versão ambiente teste. Alterar para AppFabric para colocar em produção.
        /// </summary>
        private static Dictionary<String, CacheVO> cache = new Dictionary<String, CacheVO>();
#endif

        /// <summary>
        /// Método para ser implementado pelas subclasses com a chamada real ao mainframe.
        /// </summary>
        /// <param name="statusRetornoDTO">Retorna o status do resultado desta consulta (codigo e mensagemRetorno de erro)</param>
        /// <param name="envio">DTO que contém os dados de envio para o mainframe</param>
        /// <returns></returns>
        protected abstract RetornoPesquisaSemTotalizadorDTO<TRetornoDTO> ExecutarPesquisa(out StatusRetornoDTO statusRetornoDTO, TEnvioDTO envio);

        /// <summary>
        /// Realiza uma consulta ao mainframe e armazena em cache as respostas.
        /// </summary>
        /// <param name="statusRetornoDTO">Retorna o status do resultado desta consulta (codigo e mensagemRetorno de erro)</param>
        /// <param name="numeroPagina">Número da página desejada. Inicia em 1.</param>
        /// <param name="envio">DTO que contém os dados de envio para o mainframe</param>
        /// <param name="quantidadeRegistrosPorPagina">Quantidade de registros desejada por paginação</param>
        /// <param name="guidPesquisa">Guid de pesquisa que será utilizado para armazenar no cache. O Guid de Pesquisa serve para armazenar as informações por chamada.</param>
        /// <param name="guidUsuarioCacheExtrato">Guid de usuário que será utilizado para armazenar no cache. O Guid de Usuario serve para identificar unicamente o usuário no cache.</param>
        /// <returns></returns>
        public RetornoPesquisaSemTotalizadorDTO<TRetornoDTO> Pesquisar(out StatusRetornoDTO statusRetornoDTO, TEnvioDTO envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato)
        {
            return PesquisarComFiltro<Object>(out statusRetornoDTO, envio, null, null, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato);
        }

        /// <summary>
        /// Realiza uma consulta ao mainframe e armazena em cache as respostas.
        /// </summary>
        /// <param name="statusRetornoDTO">Retorna o status do resultado desta consulta (codigo e mensagem de erro)</param>
        /// <param name="numeroPagina">Número da página desejada. Inicia em 1.</param>
        /// <param name="envio">DTO que contém os dados de envio para o mainframe</param>
        /// <param name="parametrosPreFiltro">Parametros para execuçao do pré-filtro</param>
        /// <param name="preFiltro">Filtro a ser executado no retorno da consulta antes de aplicar as regras de paginaç|ao</param>
        /// <param name="quantidadeRegistrosPorPagina">Quantidade de registros desejada por paginação</param>
        /// <param name="guidPesquisa">Guid de pesquisa que será utilizado para armazenar no cache. O Guid de Pesquisa serve para armazenar as informações por chamada.</param>
        /// <param name="guidUsuarioCacheExtrato">Guid de usuário que será utilizado para armazenar no cache. O Guid de Usuario serve para identificar unicamente o usuário no cache.</param>
        /// <returns></returns>
        protected RetornoPesquisaSemTotalizadorDTO<TRetornoDTO> PesquisarComFiltro<TParam>(out StatusRetornoDTO statusRetornoDTO, TEnvioDTO envio, TParam parametrosPreFiltro, PreFiltro<TParam> preFiltro, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato)
        {
            try
            {
                RetornoPesquisaSemTotalizadorDTO<TRetornoDTO> retorno = BuscarDadosNoServico(envio, guidPesquisa, guidUsuarioCacheExtrato, out statusRetornoDTO);

                if (statusRetornoDTO.CodigoRetorno != 0)
                {
                    return null;
                }

                List<TRetornoDTO> registros;
                // Se houver um pre-filtro, este é o momento de executá-lo
                if (preFiltro != null)
                {
                    registros = preFiltro(retorno.Registros, parametrosPreFiltro);
                }
                else
                {
                    registros = retorno.Registros;
                }

                if (numeroPagina < 1)
                {
                    numeroPagina = 1;
                }
                int indiceUltimoRegistro = (numeroPagina - 1) * quantidadeRegistrosPorPagina;

                // Caso a quantidade de registros por página ultrapasse a quantidade de registros, modifica o parametro para não trazer coisas à mais
                if (quantidadeRegistrosPorPagina > registros.Count)
                {
                    quantidadeRegistrosPorPagina = registros.Count;
                }

                int quantidadeRegistroRestante = registros.Count - indiceUltimoRegistro;

                RetornoPesquisaSemTotalizadorDTO<TRetornoDTO> result = new RetornoPesquisaSemTotalizadorDTO<TRetornoDTO>();
                result.Registros = registros.GetRange(indiceUltimoRegistro, quantidadeRegistrosPorPagina < quantidadeRegistroRestante ? quantidadeRegistrosPorPagina : quantidadeRegistroRestante);

                // Sobrescreve quantidade total de registros, por conta do pre-filtro
                result.QuantidadeTotalRegistros = registros.Count;

                return result;
            }            
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

       
        /// <summary>
        /// Verifica se a consulta já está em cache. Se estiver retorna. Caso contrário executa novamente.
        /// </summary>
        /// <param name="envio"></param>
        /// <param name="guidPesquisa"></param>
        /// <param name="guidUsuarioCacheExtrato"></param>
        /// <param name="statusRetornoDTO"></param>
        /// <returns></returns>
        private RetornoPesquisaSemTotalizadorDTO<TRetornoDTO> BuscarDadosNoServico(TEnvioDTO envio, Guid guidPesquisa, Guid guidUsuarioCacheExtrato, out StatusRetornoDTO statusRetornoDTO)
        {
            // Cria um hash dos dados de pesquisa, para verificar posteriormente se os dados passados sao os mesmos armazenados em cache
            int hashDadosPesquisa = UtilHelper.SerializarDados(envio).GetHashCode();

            // Cria uma entrada no cache por serviço. Se utilizar o mesmo serviço duas vezes, limpa o primeiro cache
            String chaveCache = guidUsuarioCacheExtrato + "_" + this.GetType().Name;


            // Recupera do cache a entrada correta (caso exista)
            CacheVO cacheVO;
#if DEBUG
            cacheVO = null;
            cache.TryGetValue(chaveCache, out cacheVO);
#else
            try {
                cacheVO = Redecard.PN.Comum.CacheAdmin.Recuperar<CacheVO>(Cache.Extrato, chaveCache);
            } catch (NullReferenceException) {
                cacheVO = null;
            }
#endif
            Boolean estaEmCache = (cacheVO != null) && (cacheVO.HashDadosPesquisa == hashDadosPesquisa);

            //LogHelper.GravarTraceLog(this.GetType().Name + " - guidPesquisa: " + guidPesquisa + ", guidUsuarioCacheExtrato = " + guidUsuarioCacheExtrato + ", XML envio.GetHashCode()=" + hashDadosPesquisa + ", estaEmCache=" + estaEmCache);
            //LogHelper.GravarTraceLog(envio);


            if (estaEmCache)
            {
                    Logger.GravarLog("Dados obtidos do cache", new { chaveCache });


                    statusRetornoDTO = new StatusRetornoDTO(0, "", this.GetType().Name);

                    return (RetornoPesquisaSemTotalizadorDTO<TRetornoDTO>)cacheVO.RetornoPesquisa;
            }
            else
            {
                if (cacheVO != null)
                {
#if DEBUG
                    cache.Remove(chaveCache);
#else
                    Redecard.PN.Comum.CacheAdmin.Remover(Cache.Extrato, chaveCache);
#endif
                }

                RetornoPesquisaSemTotalizadorDTO<TRetornoDTO> retorno = ExecutarPesquisa(out statusRetornoDTO, envio);

                if (statusRetornoDTO.CodigoRetorno != 0)
                {
                    return null;
                }

                CacheVO cacheData = new CacheVO()
                {
                    HashDadosPesquisa = hashDadosPesquisa,
                    RetornoPesquisa = retorno
                };

#if DEBUG
                cache.Add(chaveCache, cacheData);
#else
                Redecard.PN.Comum.CacheAdmin.Alterar<CacheVO>(Cache.Extrato, chaveCache, cacheData);
#endif


                return retorno;
            }
        }

    }
}
