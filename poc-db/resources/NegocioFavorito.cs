/*
© Copyright 2017 Rede S.A.
Autor : Lucas Akira Uehara
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Rede.PN.AtendimentoDigital.Core;
using Rede.PN.AtendimentoDigital.Modelo.Entidades;
using Rede.PN.AtendimentoDigital.Modelo.Excecao;
using Rede.PN.AtendimentoDigital.Modelo.Repositorio;
using Rede.PN.AtendimentoDigital.Modelo.Structure;

namespace Rede.PN.AtendimentoDigital.Negocio
{
    /// <summary>
    /// Define o repositório pela classe de negócio.
    /// </summary>
    public class NegocioFavorito
    {
        /// <summary>
        /// Define o repositório utilizado pela classe de negócio.
        /// </summary>
        private readonly IRepositorioFavorito repositorioFavorito;
                
        /// <summary>
        /// Define quantidade máxima de favoritos permitidos por usúario.
        /// </summary>
        private Int32 quantidadeMaximaFavoritosPorUsuario = Configuracao.QuantidadeMaximaFavoritos;

        #region [Inicialização]
        /// <summary>
        /// Inicializa uma instância da classe de negócio.
        /// Obtem o repositório por meio do <seealso cref="Redecard.Infraestrutura.GeradorObjeto"/>.
        /// </summary>
        public NegocioFavorito() : this(GeradorObjeto.Obter<IRepositorioFavorito>()) { }
        /// <summary>
        /// Inicializa uma instância da classe de negócio.
        /// </summary>
        /// <param name="repositorioFavorito">O repositório de Favoritos previamente carregado.</param>
        public NegocioFavorito(IRepositorioFavorito repositorioFavorito)
        {
            this.repositorioFavorito = repositorioFavorito;
        }
        #endregion [Inicialização]

        #region [CRUD]

        /// <summary>
        /// Criar um Favorito na Lista.
        /// </summary>
        /// <param name="entidade">FavoritoEntidade.</param>
        /// <returns>Retorna nova chave gerado.</returns>
        public FavoritoChave Inserir(FavoritoEntidade entidade)
        {
            StringBuilder erros = new StringBuilder();

            if (entidade.Chave.CodUsrId <= 0 && entidade.Chave.CodServ <= 0)
                erros.AppendLine("Código de identificação do usúario ou código do serviço incorreto.");

            //Obtém todos os favoritos do usáurio
            List<FavoritoEntidade> listaFavoritoEntidade = Listar(entidade.Chave.CodUsrId);

            if (listaFavoritoEntidade.Count >= quantidadeMaximaFavoritosPorUsuario)
                erros.AppendLine("Excedeu a quantidade permitida de favoritos por usúario.");

            if (erros.Length > 0)
                throw new ExcecaoNegocio(erros.ToString());

            return this.repositorioFavorito.Inserir(entidade);
        }

        /// <summary>
        /// Listar os favoritos.
        /// </summary>
        /// <returns>Lista de favoritos.</returns>
        public List<FavoritoEntidade> Listar(Int32? codUsrId)
        {
            return this.repositorioFavorito.Listar(codUsrId);
        }

        /// <summary>
        /// Listar os favoritos.
        /// </summary>
        /// <param name="entidade">FavoritoEntidade.</param>
        /// <returns>Lista de Favoritos do Usúario.</returns>
        public FavoritoEntidade Obter(FavoritoEntidade entidade)
        {
            StringBuilder erros = new StringBuilder();

            if (entidade == null)
                erros.AppendLine("Chave da entidade não foi informada.");
            else if (entidade.Chave.CodUsrId <= 0)
                erros.AppendLine("Código de identificação do usúario está incorreto.");
            else if (entidade.Chave.CodServ <= 0)
                erros.AppendLine("Código do serviço está incorreto.");

            if (erros.Length > 0)
                throw new ExcecaoNegocio(erros.ToString());

            return this.repositorioFavorito.Obter(entidade);
        }

        /// <summary>
        /// Remover todos os favoritos do usúario.
        /// </summary>
        /// <param name="codUsrId">Código de identificação do usúario.</param>
        public void Excluir(Int32 codUsrId)
        {
            StringBuilder erros = new StringBuilder();

            if (codUsrId <= 0)
                erros.AppendLine("Código de identificação do usúario ou código do serviço incorreto.");

            if (erros.Length > 0)
                throw new ExcecaoNegocio(erros.ToString());

            this.repositorioFavorito.Excluir(codUsrId);
        }

        /// <summary>
        /// Remover todos os favoritos do usúario.
        /// </summary>
        /// <param name="entidade">FavoritoEntidade.</param>
        public void Excluir(FavoritoEntidade entidade)
        {
            StringBuilder erros = new StringBuilder();

            if (entidade.Chave.CodUsrId <= 0 && entidade.Chave.CodServ <= 0)
                erros.AppendLine("Código de identificação do usúario ou código do serviço incorreto.");

            if (erros.Length > 0)
                throw new ExcecaoNegocio(erros.ToString());

            this.repositorioFavorito.Excluir(entidade);
        }

        /// <summary>
        /// Atualizar o favorito.
        /// </summary>
        /// <param name="entidade"></param>
        public void Atualizar(FavoritoEntidade entidade)
        {
            StringBuilder erros = new StringBuilder();

            if (entidade.Chave.CodUsrId <= 0 && entidade.Chave.CodServ <= 0)
                erros.AppendLine("Código de identificação do usúario ou código do serviço incorreto.");

            if (erros.Length > 0)
                throw new ExcecaoNegocio(erros.ToString());

            this.repositorioFavorito.Excluir(entidade);
            this.repositorioFavorito.Inserir(entidade);
        }

        /// <summary>
        /// Atualiza os serviços favoritados.
        /// </summary>
        /// <param name="codigosServico">Int32[]</param>
        public List<FavoritoEntidade> AtualizarFavoritosUsuario(Int32 codUsrId, Int32[] codigosServico) 
        {
            try
            {
                //Excluí todos os favoritos.
                this.Excluir(codUsrId);

                List<FavoritoEntidade> favoritos = codigosServico
                    .Select(codigoServico => new FavoritoEntidade() {
                        Chave = new FavoritoChave()
                        {
                            CodUsrId = codUsrId,
                            CodServ = codigoServico
                        }
                    }).ToList();

                //Insere favoritos atualizados
                foreach (FavoritoEntidade favorito in favoritos)
                {                    
                    this.Inserir(favorito);
                }

                //retorna lista de favoritos atualizados.
                favoritos = this.Listar(codUsrId);                
                return favoritos;
            }
            catch (NullReferenceException ex)
            {
                throw new ExcecaoNegocio("Erro ao atualizar os favoritos. " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new ExcecaoNegocio("Erro ao atualizar os favoritos. " + ex.Message);
            }
        }
        #endregion [CRUD]
    }
}