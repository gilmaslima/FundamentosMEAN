/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Barbosa dos Santos]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/08/21 - Tiago Barbosa dos Santos - Versão Inicial
- 2012/08/29 - Guilherme Alves Brito / Lucas Nicoletto da Cunha - Anulação Cancelamento
*/
using System;
using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.Cancelamento.Modelo;

namespace Redecard.PN.Cancelamento.Servicos.Interfaces
{
    [ServiceContract]
    public interface IServicoPortalCancelamento
    {
        [OperationContract]
        ModConsultaResult ConsultaPorPeriodo(string numPdv, String DataInicial, String DataFinal);
        
        [OperationContract]
        ModConsultaResult ConsultaPorAvisoCancelamento(int numPdv, Decimal NumAvisoCancelamento);

        [OperationContract]
        List<ModComprovante> ConsultaAnulacao(int codEstabelecimento);

        [OperationContract]
        List<ModComprovante> ComprovanteCancelamento(int codEstabelecimento, long lonSession, string strServer,short CodCancelamento, string Continua, int NumAvisoCancel);

        [OperationContract]
        List<ModAnularCancelamento> RealizarAnulacaoCancelamento(string usuario, string ipUsuario, List<ModComprovante> registrosDesfazer);

        [OperationContract]
        List<ItemCancelamentoSaida> Cancelamento(List<ItemCancelamentoEntrada> input);

        [OperationContract]
        EstabelecimentoCancelamento RetornaDadosEstabelecimentoCancelamento(int codEstabelecimento);

        [OperationContract]
        List<ModConsultaDuplicado> ConsultaDuplicados(List<ItemCancelamentoEntrada> entrada);

        [OperationContract]
        List<ItemCancelamentoSaida> CancelamentoDuplicadas(List<ModConsultaDuplicado> input);
    }
}