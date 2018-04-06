using Redecard.PN.Comum;
using Redecard.PN.Request.SharePoint.Model;
using Redecard.PN.Request.SharePoint.XBChargebackServico;
using System;
using System.Linq;
using System.ServiceModel;

namespace Redecard.PN.Request.SharePoint.Business
{
    /// <summary>
    /// Class para busca de comprovantes pendentes do tipo crédito
    /// </summary>
    public class RequestComprovantePendenteCredito : RequestComprovante
    {
        /// <summary>
        /// Método customizado para consulta à camada de serviços
        /// </summary>
        /// <param name="request">Dados da requisição</param>
        /// <returns>Response customizado com a relação dos comprovantes ou conteúdo de exceção</returns>
        public override ComprovanteServiceResponse ConsultarServico(ComprovanteServiceRequest request)
        {
            // parâmetros
            Int32 codEstabelecimento = request.SessaoUsuario.CodigoEntidade;
            String sistemaOrigem = request.SessaoUsuario.UsuarioAtendimento ? "IZ" : "IS";
            Decimal codProcesso = 0;
            String programa = "REQUESTPENDENTE";

            using (Logger log = Logger.IniciarNovoLog("RequestReportUserControl.ConsultarPendetesCredito - HISServicoXB Crédito"))
            using (HISServicoXBChargebackClient client = new HISServicoXBChargebackClient())
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    TipoVenda = request.TipoVenda,
                    request.IdPesquisa,
                    request.RegistroInicial,
                    request.QuantidadeRegistros,
                    request.QuantidadeRegistroBuffer,
                    codEstabelecimento,
                    codProcesso,
                    programa,
                    sistemaOrigem
                });

                Int32
                    quantidadeTotalRegistrosEmCache = 0,
                    codigoRetorno = 0;
                ComprovanteServiceResponse response = new ComprovanteServiceResponse();

                //recuperando os requests pendentes
                var ret = client.ConsultarRequestPendente(
                    out quantidadeTotalRegistrosEmCache,
                    out codigoRetorno,
                    request.IdPesquisa,
                    request.RegistroInicial,
                    request.QuantidadeRegistros,
                    request.QuantidadeRegistroBuffer,
                    codEstabelecimento,
                    programa,
                    sistemaOrigem);

                if (ret != null)
                    response.Comprovantes = ret.Select(x =>
                    {
                        var model = ComprovanteModel.Convert(x);
                        model.Status = StatusComprovante.Pendente;
                        model.TipoVenda = TipoVendaComprovante.Credito;
                        return model;
                    }).ToList();

                response.CodigoRetorno = codigoRetorno;
                response.QuantidadeTotalRegistrosEmCache = quantidadeTotalRegistrosEmCache;

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    quantidadeTotalRegistrosEmCache,
                    codigoRetorno,
                    response.Comprovantes
                });

                //caso o código de retorno seja != 0 ocorreu um erro.
                //considerar que 10 ou 53 não é erro: DADOS NAO ENCONTRADOS NA TABELA
                if (codigoRetorno > 0 && codigoRetorno != 10 && codigoRetorno != 53)
                {
                    // response.FonteExcecao = "XBChargebackServico.ConsultarRequestPendente";
                    throw new FaultException<GeneralFault>(new GeneralFault
                    {
                        Codigo = codigoRetorno,
                        Fonte = "XBChargebackServico.ConsultarRequestPendente"
                    });
                }

                return response;
            }
        }
    }
}
