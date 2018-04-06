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
    public class RequestComprovantePendenteDebito : RequestComprovante
    {
        /// <summary>
        /// Método customizado para consulta à camada de serviços
        /// </summary>
        /// <param name="request">Dados da requisição</param>
        /// <returns>Response customizado com a relação dos comprovantes ou conteúdo de exceção</returns>
        public override ComprovanteServiceResponse ConsultarServico(ComprovanteServiceRequest request)
        {
            //parametros 
            Int32 codEstabelecimento = request.SessaoUsuario.CodigoEntidade;
            String sistemaOrigem = request.SessaoUsuario.UsuarioAtendimento ? "IZ" : "IS";
            DateTime dataInicio = new DateTime(1980, 1, 1, 0, 0, 0);
            DateTime dataFim = new DateTime(2999, 12, 1);
            String transacao = "XB94";

            using (Logger log = Logger.IniciarNovoLog("RequestReportUserControl.ConsultarPendentesDebito - HISServicoXB Débito"))
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
                    dataInicio,
                    dataFim,
                    sistemaOrigem,
                    transacao
                });

                short codigoRetorno = default(short);
                Int32 quantidadeTotalRegistrosEmCache = 0;
                ComprovanteServiceResponse response = new ComprovanteServiceResponse();

                //recuperando os requests pendentes
                var ret = client.ConsultaSolicitacaoPendente(
                    out quantidadeTotalRegistrosEmCache,
                    out codigoRetorno,
                    request.IdPesquisa,
                    request.RegistroInicial,
                    request.QuantidadeRegistros,
                    request.QuantidadeRegistroBuffer,
                    codEstabelecimento,
                    dataInicio,
                    dataFim,
                    sistemaOrigem,
                    transacao);

                if (ret != null)
                    response.Comprovantes = ret.Select(x =>
                    {
                        var model = ComprovanteModel.Convert(x);
                        model.Status = StatusComprovante.Pendente;
                        model.TipoVenda = TipoVendaComprovante.Debito;
                        return model;
                    }).ToList();

                response.CodigoRetorno = codigoRetorno;
                response.QuantidadeTotalRegistrosEmCache = quantidadeTotalRegistrosEmCache;

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    quantidadeTotalRegistrosEmCache,
                    codRetorno = codigoRetorno,
                    response.Comprovantes
                });

                //caso o código de retorno seja != 0 ocorreu um erro.
                //considerar que 10 ou 53 não é erro: DADOS NAO ENCONTRADOS NA TABELA
                if (codigoRetorno > 0 && codigoRetorno != 10 && codigoRetorno != 53)
                {
                    // response.FonteExcecao = "XBChargebackServico.ConsultarDebitoPendente";
                    throw new FaultException<GeneralFault>(new GeneralFault
                    {
                        Codigo = codigoRetorno,
                        Fonte = "XBChargebackServico.ConsultarDebitoPendente"
                    });
                }

                return response;
            }
        }
    }
}
