/*
© Copyright 2017 Rede S.A.
Autor : Lucas Akira Uehara
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.Script.Serialization;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Datasource;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content;
using Rede.PN.AtendimentoDigital.SharePoint.ServicoFavorito;
using Redecard.PN.Comum;

namespace Rede.PN.AtendimentoDigital.SharePoint.Handlers
{
    /// <summary>
    /// Classe Favorito.
    /// </summary>
    public class Favorito : HandlerBase
    {        
        #region [Propriedades]
        /// <summary>
        /// Código de identificação do Usúario.
        /// </summary>
        public Int32 CodUsrId { get { return base.Sessao.CodigoIdUsuario; } }

        /// <summary>
        /// Código do Serviço.
        /// </summary>
        public Int32 CodServ { get { return base.Request["codigoServico"].ToInt32(0); } }

        /// <summary>
        /// Lista de Códigos de Serviços.
        /// </summary>
        public Int32[] CodServs { get { return new JavaScriptSerializer().Deserialize<Int32[]>(base.Request["codigoServicos"]); } }

        /// <summary>
        /// Chave.
        /// </summary>
        public FavoritoChave Chave
        {
            get
            {
                return new FavoritoChave
                {
                    CodServ = this.CodServ,
                    CodUsrId = this.CodUsrId
                };
            }
        }

        /// <summary>
        /// Request.
        /// </summary>
        public FavoritoRequest FavoritoRequest { get { return new FavoritoRequest { Chave = this.Chave }; } }
        
        #endregion [Propriedades]

        #region [ Métodos - Consulta ]

        /// <summary>
        /// Obter Favorito por Código de Identificação do Usúario.
        /// </summary>
        /// <param name="CodUsrId">Int32</param>
        /// <returns>HandlerResponse</returns>
        [HttpGet]
        public HandlerResponse ListarFavoritos()
        {
            using (Logger log = Logger.IniciarLog("Obter Favorito por Código de Identificação do Usúario."))
            {
                var servicoDatasource = new ServicoDatasource(base.Sessao);

                try
                {
                    var favoritosUsuario = new FavoritoResponseList();

                    using (var contexto = new ContextoWCF<ServicoFavoritoClient>())
                    {
                        favoritosUsuario = contexto.Cliente.Listar(CodUsrId);
                    }

                    if (favoritosUsuario == null)
                        return new HandlerResponse(302, "O serviço retornou um objeto nulo.");

                    //obtém os serviços associados ao usuário
                    List<ServicoItem> servicos = servicoDatasource.GetItems().Cast<ServicoItem>().ToList();

                    Object response = favoritosUsuario.Itens
                        .Select(favoritoUsuario => new
                        {
                            CodigoServico = favoritoUsuario.Chave.CodServ,
                            DataInclusao = favoritoUsuario.DthInclusao,
                            Servico = servicos.FirstOrDefault(servico => servico.Codigo == favoritoUsuario.Chave.CodServ)
                        }).ToList();

                    return new HandlerResponse(response);
                }
                catch (FaultException<FalhaGenerica> ex)
                {
                    Logger.GravarErro("Erro ao obter os Favoritos para este usúario.", ex);
                    return new HandlerResponse(301, "Erro ao obter os Favoritos para este usúario.",
                        new
                        {
                            Codigo = ex.Detail.Codigo,
                            CodeName = ex.Code != null ? ex.Code.Name : null,
                            CodeSubcode = ex.Code != null ? ex.Code.SubCode : null
                        }, null);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Erro ao obter os Favoritos para este usúario.", ex);
                    return new HandlerResponse(HandlerBase.CodigoErro, "Erro ao obter os Favoritos para este usúario.");
                }
            }
        }

        #endregion

        /// <summary>
        /// Atualizar Favoritos.
        /// </summary>
        /// <returns>HandlerResponse</returns>
        [HttpPost]
        public HandlerResponse AtualizarFavoritos()
        {
            using (Logger log = Logger.IniciarLog("Atualizar Favoritos."))
            {
                var servicoDatasource = new ServicoDatasource(base.Sessao);

                try
                {
                    var favoritosUsuario = default(FavoritoResponseList);

                    using (var contexto = new ContextoWCF<ServicoFavoritoClient>())
                    {
                        favoritosUsuario = contexto.Cliente
                            .AtualizarFavoritosUsuario(new AtualizarFavoritosUsuarioRequest() {
                                CodUsrId = CodUsrId, 
                                CodigosServico = CodServs.ToList()
                        });
                    }

                    if (favoritosUsuario == null)
                        return new HandlerResponse(302, "O serviço retornou um objeto nulo.");

                    //obtém os serviços associados ao usuário
                    List<ServicoItem> servicos = servicoDatasource.GetItems().Cast<ServicoItem>().ToList();

                    Object response = favoritosUsuario.Itens
                        .Select(favoritoUsuario => new
                        {
                            CodigoServico = favoritoUsuario.Chave.CodServ,
                            DataInclusao = favoritoUsuario.DthInclusao,
                            Servico = servicos.FirstOrDefault(servico => servico.Codigo == favoritoUsuario.Chave.CodServ)
                        });

                    return new HandlerResponse(response);
                }
                catch (FaultException<FalhaGenerica> ex)
                {
                    Logger.GravarErro("Erro ao atualizar os Favoritos para este usúario.", ex);
                    return new HandlerResponse(
                        301,
                        "Erro ao atualizar os Favoritos para este usúario.",
                        new
                        {
                            Codigo = ex.Detail.Codigo,
                            CodeName = ex.Code != null ? ex.Code.Name : null,
                            CodeSubcode = ex.Code != null ? ex.Code.SubCode : null
                        }, null);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Erro ao atualizar os Favoritos para este usúario.", ex);
                    return new HandlerResponse(
                        HandlerBase.CodigoErro,
                        "Erro ao atualizar os Favoritos para este usúario.");
                }
            }
        }
    }
}