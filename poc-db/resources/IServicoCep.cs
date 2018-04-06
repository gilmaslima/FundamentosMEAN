/*
© Copyright 2017 Rede S.A.
Autor : Mário Neto
Empresa : Iteris Consultoria e Software
*/

using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using Rede.PN.AtendimentoDigital.Servicos.Excecao;
using Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoDados.Response.EnderecoCepResponse;

namespace Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoServico
{
    /// <summary>
    /// Interface para implementação do serviço IServicoEstabelecimento.
    /// </summary>
    [ServiceContract]
    public interface IServicoCep
    {
        /// <summary>
        /// Obter Favoritos.
        /// </summary>
        /// <param name="request">FavoritoRequest.</param>
        /// <returns>FavoritoResponseList</returns>
        [OperationContract]
        [WebInvoke(
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "BuscarEnderecoPorCep/{cep}")]
        [FaultContract(typeof(FalhaGenerica))]
        EnderecoCepResponseItem BuscarEnderecoPorCep(String cep);
    }
}