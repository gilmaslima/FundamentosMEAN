using Redecard.PN.Comum;
using Redecard.PN.Request.SharePoint.Model;
using Redecard.PN.Request.SharePoint.XBChargebackServico;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Redecard.PN.Request.SharePoint.Business
{
    /// <summary>
    /// Classe de negócio para consulta dos comprovantes junto à camada de serviços
    /// </summary>
    public static class ComprovantesService
    {
        /// <summary>
        /// Consulta os comprovantes
        /// </summary>
        /// <param name="request">Dados para a requisição</param>
        /// <returns>Modelo consolidado de comprovantes históricos e pendentes</returns>
        public static ComprovanteServiceResponse Consultar(
            ComprovanteServiceRequest request,
            List<ComprovanteRequestIdPesquisa> listPesquisaId)
        {
            // utiliza o factory para definição do serviço a ser utilizado
            List<RequestComprovante> factoryResult = GetRequestService(request, listPesquisaId);

            // coleção thread-safe que conterá as exceções geradas
            ConcurrentQueue<Exception> exceptions = new ConcurrentQueue<Exception>();

            // cria as tasks para busca em paralelo
            Task<ComprovanteServiceResponse>[] tasks = factoryResult
                .Select(x =>
                {
                    return Task.Factory.StartNew<ComprovanteServiceResponse>(() =>
                    {
                        try
                        {
                            return x.ConsultarServico();
                        }
                        catch (FaultException<GeneralFault> ex)
                        {
                            exceptions.Enqueue(ex);
                        }
                        catch (Exception ex)
                        {
                            exceptions.Enqueue(ex);
                        }
                        return null;
                    });
                })
                .ToArray();

            // aguarda processamento de todas as tasks
            Task.WaitAll(tasks);

            // caso tenha ocorrido alguma exceção, lança a primeira exceção gerada
            if (exceptions.Count > 0)
            {
                // registra as exceptions para consulta posterior
                foreach (var ex in exceptions)
                {
                    Logger.GravarErro("Exception ComprovanteServiceResponse", ex);
                    SharePointUlsLog.LogErro(ex);
                }

                // lança a primeira exceção segundo ordem de prioridade
                var exception = exceptions
                    .OrderBy(x => x is FaultException || x is PortalRedecardException ? 0 : 1)
                    .First();
                throw exception;
            }

            // cria o response
            ComprovanteServiceResponse response = new ComprovanteServiceResponse();
            response.Comprovantes = new List<ComprovanteModel>();
            foreach (var task in tasks)
            {
                response.Comprovantes.AddRange(task.Result.Comprovantes);

                response.CodigoRetorno = response.CodigoRetorno == 0
                    ? task.Result.CodigoRetorno
                    : response.CodigoRetorno;
            }

            // filtra por número do processo se foi informado
            if (request.CodProcesso.GetValueOrDefault(0) > 0)
                response.Comprovantes = response.Comprovantes.Where(x => 
                    Decimal.Compare(request.CodProcesso.Value, x.Processo) == 0).ToList();

            // informa a quantidade total de registros disponíveis
            response.QuantidadeTotalRegistrosEmCache = response.Comprovantes.Count;

            // ordena e retorna os registros segundo a paginação
            var comprovantesRetorno = response.Comprovantes
                .OrderBy(x => x.Status == StatusComprovante.Pendente ? 0 : 1)
                .Skip(request.RegistroInicial);
            
            if (request.QuantidadeRegistros > 0)
                comprovantesRetorno = comprovantesRetorno.Take(request.QuantidadeRegistros);

            response.Comprovantes = comprovantesRetorno.ToList();

            return response;
        }

        /// <summary>
        /// Factory para identificação do serviço segundo o tipo de venda
        /// </summary>
        /// <param name="request">Dados da requisição</param>
        /// <param name="listPesquisaId">Lista com a identificação de pesquisa (guid) por cada tipo de relatório</param>
        /// <returns>Class de serviço para consulta à camada de serviços segundo o tipo de venda</returns>
        private static List<RequestComprovante> GetRequestService(
            ComprovanteServiceRequest request, 
            List<ComprovanteRequestIdPesquisa> listPesquisaId)
        {
            // retorno com a lista de request services
            List<RequestComprovante> retorno = new List<RequestComprovante>();

            // histórico + crédito
            AddServiceRequestToList(
                request.TipoVenda == TipoVendaComprovante.Credito &&
                (request.Status == StatusComprovante.Historico || request.Status == StatusComprovante.Todos),
                new RequestComprovanteHistoricoCredito
                {
                    RequestData = new ComprovanteServiceRequest
                    {
                        IdPesquisa = GetPesquisaId(listPesquisaId, StatusComprovante.Historico, TipoVendaComprovante.Credito),
                        QuantidadeRegistros = Int16.MaxValue,
                        QuantidadeRegistroBuffer = Int16.MaxValue,
                        Parametros = request.Parametros,
                        SessaoUsuario = request.SessaoUsuario,
                        TipoVenda = request.TipoVenda,
                        DataFim = request.DataFim,
                        DataInicio = request.DataInicio,
                        CodProcesso = request.CodProcesso
                    }
                },
                ref retorno);

            // histórico + débito
            AddServiceRequestToList(
                request.TipoVenda == TipoVendaComprovante.Debito &&
                (request.Status == StatusComprovante.Historico || request.Status == StatusComprovante.Todos),
                new RequestComprovanteHistoricoDebito
                {
                    RequestData = new ComprovanteServiceRequest
                    {
                        IdPesquisa = GetPesquisaId(listPesquisaId, StatusComprovante.Historico, TipoVendaComprovante.Debito),
                        QuantidadeRegistros = Int16.MaxValue,
                        QuantidadeRegistroBuffer = Int16.MaxValue,
                        Parametros = request.Parametros,
                        SessaoUsuario = request.SessaoUsuario,
                        TipoVenda = request.TipoVenda,
                        DataFim = request.DataFim,
                        DataInicio = request.DataInicio,
                        CodProcesso = request.CodProcesso
                    }
                },
                ref retorno);
            
            // pendente + crédito
            AddServiceRequestToList(
                request.TipoVenda == TipoVendaComprovante.Credito &&
                (request.Status == StatusComprovante.Pendente || request.Status == StatusComprovante.Todos),
                new RequestComprovantePendenteCredito
                {
                    RequestData = new ComprovanteServiceRequest
                    {
                        IdPesquisa = GetPesquisaId(listPesquisaId, StatusComprovante.Pendente, TipoVendaComprovante.Credito),
                        QuantidadeRegistros = Int16.MaxValue,
                        QuantidadeRegistroBuffer = Int16.MaxValue,
                        Parametros = request.Parametros,
                        SessaoUsuario = request.SessaoUsuario,
                        TipoVenda = request.TipoVenda
                    }
                },
                ref retorno);

            // pendente + débito
            AddServiceRequestToList(
                request.TipoVenda == TipoVendaComprovante.Debito &&
                (request.Status == StatusComprovante.Pendente || request.Status == StatusComprovante.Todos),
                new RequestComprovantePendenteDebito
                {
                    RequestData = new ComprovanteServiceRequest
                    {
                        IdPesquisa = GetPesquisaId(listPesquisaId, StatusComprovante.Pendente, TipoVendaComprovante.Debito),
                        QuantidadeRegistros = Int16.MaxValue,
                        QuantidadeRegistroBuffer = Int16.MaxValue,
                        Parametros = request.Parametros,
                        SessaoUsuario = request.SessaoUsuario,
                        TipoVenda = request.TipoVenda
                    }
                },
                ref retorno);

            return retorno;
        }

        /// <summary>
        /// Popula a lista de requests com determinado request segundo o critério informado
        /// </summary>
        /// <param name="criterio">Critério para inclusão do request à lista</param>
        /// <param name="request">Request a ser inserido à lista</param>
        /// <param name="listRequests">Lista de requests de comprovantes</param>
        private static void AddServiceRequestToList(
            Boolean criterio,
            RequestComprovante request,
            ref List<RequestComprovante> listRequests)
        {
            if (criterio)
                listRequests.Add(request);
        }

        /// <summary>
        /// Obtém o IdPesquisa usado para persistência dos dados em cache em server side
        /// </summary>
        /// <param name="listPesquisaId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private static Guid GetPesquisaId(
            List<ComprovanteRequestIdPesquisa> listPesquisaId, 
            StatusComprovante status,
            TipoVendaComprovante tipovenda)
        {
            var retorno = listPesquisaId.FirstOrDefault(x => x.Status == status && x.TipoVenda == tipovenda);
            if (retorno == null)
                return Guid.NewGuid();

            return retorno.IdPesquisa;
        }

        /// <summary>
        /// Obtém a descrição do canal de envio pelo código informado
        /// </summary>
        /// <param name="canalEnvio"></param>
        /// <param name="descricaoPadrao"></param>
        /// <returns></returns>
        public static String ObtemDescricaoCanalEnvio(Int32? canalEnvio, String descricaoPadrao)
        {
            switch (canalEnvio)
            {
                case 1: return "correio";
                case 2: return "fax automático";
                case 3: return "não envia";
                case 4: return "internet";
                case 5: return "e-mail";
                case 6: return "arquivo e transmissão de request de chargeback";
                case 7: return "extrato eletrônico";
                default: return descricaoPadrao.EmptyToNull() ?? "indefinido";
            }
        }
    }
}
