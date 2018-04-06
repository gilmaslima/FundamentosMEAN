/*
© Copyright 2017 Rede S.A.
Autor : Lucas Akira Uehara
Empresa : Iteris Consultoria e Software
*/
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Rede.PN.AtendimentoDigital.Modelo.Entidades;
using Rede.PN.AtendimentoDigital.Negocio;
using Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoDados.Request;
using Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoDados.Response;
using Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoDados.Response.FavoritoResponse;
using Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoServico;

namespace Rede.PN.AtendimentoDigital.Servicos.Servicos
{
    /// <summary>
    /// Classe de serviço do Favorito.
    /// </summary>
    public class ServicoFavorito : ServicoBase, IServicoFavorito
    {
        #region [Inicialização]
        /// <summary>
        /// Define se o mapeamento foi realizado.
        /// </summary>
        private static readonly Boolean mapeamentoRealizado;
        /// <summary>
        /// Construtor.
        /// </summary>
        static ServicoFavorito()
        {
            if (!mapeamentoRealizado)
            {
                Mapper.CreateMap<FavoritoRequest, FavoritoEntidade>();
                Mapper.CreateMap<FavoritoEntidade, FavoritoResponse>();
                Mapper.CreateMap<FavoritoEntidade, FavoritoResponseItem>();
                mapeamentoRealizado = true;
            }
        }

        #endregion [Inicialização]

        /// <summary>
        /// ObterPorRequest
        /// </summary>
        /// <param name="request">request</param>
        public FavoritoResponseItem Obter(FavoritoRequest request)
        {
            return this.ExecucaoTratada<FavoritoResponseItem>(retorno =>
            {
                NegocioFavorito negocio = new NegocioFavorito();

                FavoritoEntidade item = negocio.Obter(Mapper.Map<FavoritoEntidade>(request));

                retorno.Item = Mapper.Map<FavoritoResponse>(item);
                retorno.Mensagem = "Obtido com sucesso.";
                retorno.StatusRetorno = StatusRetorno.OK;
            });
        }

        /// <summary>
        /// Listar
        /// </summary>
        public FavoritoResponseList Listar(Int32? codUsrId)
        {
            return this.ExecucaoTratada<FavoritoResponseList>(retorno =>
            {
                NegocioFavorito negocio = new NegocioFavorito();

                List<FavoritoEntidade> itens = negocio.Listar(codUsrId);

                retorno.Itens = Mapper.Map<FavoritoResponse[]>(itens);
                retorno.TotalItens = retorno.Itens.Length;
                retorno.Mensagem = "Listado com sucesso.";
                retorno.StatusRetorno = StatusRetorno.OK;
            });
        }

        /// <summary>
        /// InserirPorRequest
        /// </summary>
        /// <param name="request">request</param>
        public FavoritoChaveResponse Inserir(FavoritoRequest request)
        {
            return this.ExecucaoTratada<FavoritoChaveResponse>(retorno =>
            {
                NegocioFavorito negocio = new NegocioFavorito();
                retorno.Chave = negocio.Inserir(Mapper.Map<FavoritoEntidade>(request));
                retorno.Mensagem = "Inserido com sucesso.";
                retorno.StatusRetorno = StatusRetorno.OK;
            });
        }

        /// <summary>
        /// ExcluirPorRequest
        /// </summary>
        /// <param name="request">request</param>
        public ResponseBase Excluir(FavoritoRequest request)
        {
            return this.ExecucaoTratada<ResponseBase>(retorno =>
            {
                NegocioFavorito negocio = new NegocioFavorito();
                negocio.Excluir(Mapper.Map<FavoritoEntidade>(request));
                retorno.Mensagem = "Excluído com sucesso";
                retorno.StatusRetorno = StatusRetorno.OK;
            });
        }

        /// <summary>
        /// ExcluirPorCodUsrId
        /// </summary>
        /// <param name="codUsrId">Código id do usuário</param>
        public ResponseBase ExcluirPorUsuario(Int32 codUsrId)
        {
            return this.ExecucaoTratada<ResponseBase>(retorno =>
            {
                NegocioFavorito negocio = new NegocioFavorito();
                negocio.Excluir(codUsrId);
                retorno.Mensagem = "Excluído com sucesso";
                retorno.StatusRetorno = StatusRetorno.OK;
            });
        }

        /// <summary>
        /// Atualizar favoritos do usuário.
        /// </summary>
        /// <param name="codUsrId">Código id do usuário</param>
        /// <param name="codigosServico">Códigos dos serviços</param>
        public FavoritoResponseList AtualizarFavoritosUsuario(AtualizarFavoritosUsuarioRequest request)
        {
            return this.ExecucaoTratada<FavoritoResponseList>(retorno =>
            {
                NegocioFavorito negocio = new NegocioFavorito();
                List<FavoritoEntidade> itens = negocio.AtualizarFavoritosUsuario(request.CodUsrId, request.CodigosServico);
                retorno.Itens = Mapper.Map<FavoritoResponse[]>(itens);
                retorno.TotalItens = retorno.Itens.Length;
                retorno.Mensagem = "Atualizado com sucesso.";
                retorno.StatusRetorno = StatusRetorno.OK;
            });
        }
    }
}