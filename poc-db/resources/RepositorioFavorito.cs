/*
© Copyright 2017 Rede S.A.
Autor : Lucas Akira Uehara
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Data;
using Rede.PN.AtendimentoDigital.Core.EntLib.Dados.Generico;
using Rede.PN.AtendimentoDigital.Modelo.Entidades;
using Rede.PN.AtendimentoDigital.Modelo.Excecao;
using Rede.PN.AtendimentoDigital.Modelo.Repositorio;
using Rede.PN.AtendimentoDigital.Modelo.Structure;

namespace Rede.PN.AtendimentoDigital.Dados.Repositorio
{
    /// <summary>
    /// Implementação do IRepositorioFavorito.
    /// </summary>
    public class RepositorioFavorito : RepositorioBase<FavoritoEntidade, FavoritoChave>, IRepositorioFavorito
    {
        #region [CRUD]
        /// <summary>
        /// Criar um Favorito na Lista.
        /// </summary>
        /// <param name="entidade">FavoritoEntidade.</param>
        /// <returns>Retorna nova chave gerado.</returns>
        public new FavoritoChave Inserir(FavoritoEntidade entidade)
        {
            FavoritoEntidade favorito = new FavoritoEntidade();
            try
            {
                
                this.ExecutarComando(
                    "sp_ins_favoritos",
                    TipoComando.StoredProcedure,
                    new ParametroComando[] 
                    {
                        new ParametroComandoEntrada("@CodUsrId", DbType.Int32, entidade.Chave.CodUsrId),
                        new ParametroComandoEntrada("@CodServ", DbType.Int32, entidade.Chave.CodServ)
                    }, 
                    ModoExecucao.Reader, 
                    (read) =>
                    {
                        IDataReader reader = read as IDataReader;
                        while (reader.Read())
                        {
                            favorito = MapearDadosFavoritos(reader);
                        }

                        if (favorito.DthInclusao == DateTime.MinValue)
                            throw new ExcecaoDados(-1, "Erro ao inserir.");
                    });

                return favorito.Chave;
            }
            catch (NullReferenceException ex) 
            {
                if (ex.Message.Contains("FOREIGN KEY"))
                    throw new ExcecaoDados(-1, "Não existe o usúario ou o código de serviço.");
                else if (ex.Message.Contains("PRIMARY KEY"))
                    throw new ExcecaoDados(-1, "Esse registro já existe no banco.");
                else
                    throw new ExcecaoDados(-1, ex.Message);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FOREIGN KEY"))
                    throw new ExcecaoDados(-1, "Não existe o usúario ou o código de serviço.");
                else if (ex.Message.Contains("PRIMARY KEY"))
                    throw new ExcecaoDados(-1, "Esse registro já existe no banco.");
                else
                    throw new ExcecaoDados(-1, ex.Message);
            }
        }

        /// <summary>
        /// Obtém um favorito.
        /// </summary>
        /// <param name="entidade">FavoritoEntidade.</param>
        /// <returns>Lista de Favoritos do Usúario.</returns>
        public new FavoritoEntidade Obter(FavoritoEntidade entidade)
        {
            try
            {
                FavoritoEntidade favorito = default(FavoritoEntidade);

                this.ExecutarReader(
                    "sp_cons_favoritos",
                    TipoComando.StoredProcedure,
                    new ParametroComando[]
                    {
                        new ParametroComandoEntrada("@CodUsrId", DbType.Int32, entidade.Chave.CodUsrId),
                        new ParametroComandoEntrada("@CodServ", DbType.Int32, entidade.Chave.CodServ)
                    },
                    (IDataReader reader) =>
                    {
                        while (reader.Read())
                        {
                            favorito = MapearDadosFavoritos(reader);
                        }
                    },
                    null);

                return favorito;
            }
            catch (NullReferenceException ex)
            {
                throw new ExcecaoDados(-1, ex.Message);
            }
            catch (Exception ex)
            {
                throw new ExcecaoDados(-1, ex.Message);
            }
        }

        /// <summary>
        /// Listar os favoritos.
        /// </summary>
        /// <param name="codUsrId">Código do usuário</param>
        /// <returns>Lista de favoritos.</returns>
        public List<FavoritoEntidade> Listar(Int32? codUsrId)
        {
            try
            {
                List<FavoritoEntidade> favoritos = new List<FavoritoEntidade>();

                this.ExecutarReader(
                    "sp_cons_favoritos",
                    TipoComando.StoredProcedure,
                    new ParametroComando[]
                    {
                        new ParametroComandoEntrada("@CodUsrId", DbType.Int32, codUsrId)
                    },
                    (IDataReader reader) =>
                    {
                        while (reader.Read())
                        {
                            favoritos.Add(MapearDadosFavoritos(reader));
                        }

                    }, null);

                return favoritos;
            }
            catch (NullReferenceException ex)
            {
                throw new ExcecaoDados(-1, ex.Message);
            }
            catch (Exception ex)
            {
                throw new ExcecaoDados(-1, ex.Message);
            }
        }

        /// <summary>
        /// Remover todos os favoritos do usúario.
        /// </summary>
        /// <param name="entidade">FavoritoEntidade.</param>
        public new void Excluir(FavoritoEntidade entidade)
        {
            try
            {
                this.ExecutarComando(
                    "sp_del_favoritos",
                    TipoComando.StoredProcedure,
                    new ParametroComando[]
                    {
                        new ParametroComandoEntrada("@CodUsrId", DbType.Int32, entidade.Chave.CodUsrId),
                        new ParametroComandoEntrada("@CodServ", DbType.Int32, entidade.Chave.CodServ)
                    },
                    ModoExecucao.NonQuery);
            }
            catch (NullReferenceException ex)
            {
                throw new ExcecaoDados(-1, ex.Message);
            }
            catch (Exception ex)
            {
                throw new ExcecaoDados(-1, ex.Message);
            }
        }

        /// <summary>
        /// Remover todos os favoritos do usúario.
        /// </summary>
        /// <param name="codUsrId">Código de identificação do usúario.</param>
        public void Excluir(Int32 codUsrId)
        {
            try
            {
                this.ExecutarComando(
                     "sp_del_favoritos",
                     TipoComando.StoredProcedure,
                     new ParametroComando[]
                    {
                        new ParametroComandoEntrada("@CodUsrId", DbType.Int32, codUsrId)
                    },
                    ModoExecucao.NonQuery);
            }
            catch (NullReferenceException ex)
            {
                throw new ExcecaoDados(-1, ex.Message);
            }
            catch (Exception ex)
            {
                throw new ExcecaoDados(-1, ex.Message);
            }
        }
        
        #endregion [CRUD]

        #region [Mapeadores]
        /// <summary>
        /// Mapeia o retorno do Reader.
        /// </summary>
        /// <param name="reader">Reader dos Favoritos.</param>
        /// <returns>Retorna um objeto Favorito.</returns>
        private FavoritoEntidade MapearDadosFavoritos(IDataReader reader)
        {
            try
            {
                return new FavoritoEntidade
                {
                    Chave = new FavoritoChave()
                    {
                        CodUsrId = reader.GetValueAsInt32("cod_usr_id"),
                        CodServ = reader.GetValueAsInt32("cod_serv")
                    },
                    DthInclusao = reader.GetValueAsDateTime("dth_incl_reg")
                };
            }
            catch (NullReferenceException ex)
            {
                throw new ExcecaoDados(-1, ex.Message);
            }
            catch (Exception ex)
            {
                throw new ExcecaoDados(-1, ex.Message);
            }
        }
        #endregion [Mapeadores]   
    }
}