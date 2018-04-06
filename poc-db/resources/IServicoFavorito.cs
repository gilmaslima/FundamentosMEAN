/*
© Copyright 2017 Rede S.A.
Autor : Lucas Akira Uehara
Empresa : Iteris Consultoria e Software
*/
using Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoDados.Request;
using Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoDados.Response;
using Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoDados.Response.FavoritoResponse;
using Rede.PN.AtendimentoDigital.Servicos.Excecao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoServico
{
    /// <summary>
    /// Interface para implementação do serviço IServicoEstabelecimento.
    /// </summary>
    [ServiceContract]
    public interface IServicoFavorito
    {
        /// <summary>
        /// Obter Favoritos.
        /// </summary>
        /// <param name="request">FavoritoRequest.</param>
        /// <returns>FavoritoResponseList</returns>
        [OperationContract]
        [WebInvoke]
        [FaultContract(typeof(FalhaGenerica))]
        FavoritoResponseItem Obter(FavoritoRequest request);

        /// <summary>
        /// Obter Favoritos.
        /// </summary>
        /// <returns>FavoritoResponseList</returns>
        [OperationContract]
        [WebInvoke]
        [FaultContract(typeof(FalhaGenerica))]
        FavoritoResponseList Listar(Int32? codUsrId);
     
        /// <summary>
        /// Inserir Favorito.
        /// </summary>
        /// <param name="request">FavoritoRequest</param>
        /// <returns>FavoritoChaveResponse</returns>
        [OperationContract]
        [WebInvoke]
        [FaultContract(typeof(FalhaGenerica))]
        FavoritoChaveResponse Inserir(FavoritoRequest request);

        /// <summary>
        /// Excluir Favorito.
        /// </summary>
        /// <param name="request">FavoritoRequest</param>
        [OperationContract]
        [WebInvoke]
        [FaultContract(typeof(FalhaGenerica))]
        ResponseBase Excluir(FavoritoRequest request);

        /// <summary>
        /// Excluir Favorito.
        /// </summary>
        /// <param name="codUsrId">Código de identificação do usúario.</param>
        [OperationContract]
        [WebInvoke]
        [FaultContract(typeof(FalhaGenerica))]
        ResponseBase ExcluirPorUsuario(Int32 codUsrId);

        /// <summary>
        /// Atualizar Favoritos de um usuário.
        /// </summary>
        /// <param name="request">Request</param>
        [OperationContract]
        [WebInvoke]
        [FaultContract(typeof(FalhaGenerica))]
        FavoritoResponseList AtualizarFavoritosUsuario(AtualizarFavoritosUsuarioRequest request);
    }
}