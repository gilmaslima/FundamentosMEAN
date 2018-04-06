using Redecard.PN.Comum;
using Redecard.PN.Request.SharePoint.Model;
using Redecard.PN.Request.SharePoint.XBChargebackServico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Redecard.PN.Request.SharePoint.Business
{
    /// <summary>
    /// Class para busca de comprovantes históricos do tipo crédito
    /// </summary>
    public class RequestComprovanteHistoricoCredito : RequestComprovante
    {
        /// <summary>
        /// Método customizado para consulta à camada de serviços
        /// </summary>
        /// <param name="request">Dados da requisição</param>
        /// <returns>Response customizado com a relação dos comprovantes ou conteúdo de exceção</returns>
        public override ComprovanteServiceResponse ConsultarServico(ComprovanteServiceRequest request)
        {
            // parâmetros
            String origemConsulta = request.Parametros != null && request.Parametros.Length > 0 
                ? request.Parametros[0].ToString() 
                : "REQUESTHISTORICO";
            String sistemaOrigem = request.SessaoUsuario.UsuarioAtendimento ? "IZ" : "IS";

            // trata os valores de entrada
            request.CodProcesso = request.CodProcesso ?? 0;
            request.DataInicio = request.DataInicio ?? DateTime.Now.AddDays(-60);
            request.DataFim = request.DataFim ?? DateTime.Now;

            // requisito dos books: enviar a data atual como data Fim
            if (DateTime.Compare(request.DataFim.Value, DateTime.Now) > 0)
                request.DataFim = DateTime.Now;

            using (Logger log = Logger.IniciarNovoLog("RequestReportUserControl.ConsultarHistoricosCredito - HISServicoXB Crédito"))
            using (HISServicoXBChargebackClient client = new HISServicoXBChargebackClient())
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    request.IdPesquisa,
                    request.RegistroInicial,
                    request.QuantidadeRegistros,
                    request.QuantidadeRegistroBuffer,
                    request.SessaoUsuario.CodigoEntidade,
                    request.CodProcesso,
                    origemConsulta,
                    request.DataInicio,
                    request.DataFim,
                    sistemaOrigem
                });

                Int32
                    quantidadeTotalRegistrosEmCache = 0,
                    codigoRetorno = 0;
                ComprovanteServiceResponse response = new ComprovanteServiceResponse();

                var ret = client.HistoricoRequest(
                    out quantidadeTotalRegistrosEmCache,
                    out codigoRetorno,
                    request.IdPesquisa,
                    request.RegistroInicial,
                    request.QuantidadeRegistros,
                    request.QuantidadeRegistroBuffer,
                    request.SessaoUsuario.CodigoEntidade,
                    origemConsulta,
                    request.DataInicio.Value,
                    request.DataFim.Value,
                    request.CodProcesso.GetValueOrDefault(0),
                    sistemaOrigem);
                
                if (ret != null)
                    response.Comprovantes = ret.Select(x =>
                    {
                        var model = ComprovanteModel.Convert(x);
                        model.Status = StatusComprovante.Historico;
                        model.TipoVenda = TipoVendaComprovante.Credito;
                        return model;
                    }).ToList();

                response.CodigoRetorno = codigoRetorno;
                response.QuantidadeTotalRegistrosEmCache = quantidadeTotalRegistrosEmCache;

                log.GravarLog(EventoLog.RetornoServico, new { 
                    quantidadeTotalRegistrosEmCache, 
                    codRetorno = codigoRetorno, 
                    response.Comprovantes 
                });

                //caso o código de retorno seja != 0 ocorreu um erro.
                //considerar que 10 ou 53 não é erro: DADOS NAO ENCONTRADOS NA TABELA
                if (codigoRetorno > 0 && codigoRetorno != 10 && codigoRetorno != 53)
                {
                    // response.FonteExcecao = "XBChargebackServico.HistoricoRequest";
                    throw new FaultException<GeneralFault>(new GeneralFault
                    {
                        Codigo = codigoRetorno,
                        Fonte = "XBChargebackServico.HistoricoRequest"
                    });
                }

                return response;
            }
        }
    }
}
