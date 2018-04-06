using System;
using System.Collections.Generic;
using Rede.PN.AtendimentoDigital.Core.Padroes.Repositorio;
using Rede.PN.AtendimentoDigital.Modelo.Entidades;
using Rede.PN.AtendimentoDigital.Modelo.Structure;

namespace Rede.PN.AtendimentoDigital.Modelo.Repositorio
{
    /// <summary>
    /// Interface IRepositorioFavorito 
    /// </summary>
    public interface IRepositorioFavorito : IRepositorio<FavoritoEntidade, FavoritoChave>
    {
        #region [Read]        
        /// <summary>
        /// Listar os favoritos.
        /// </summary>
        /// <returns>Lista de favoritos.</returns>
        List<FavoritoEntidade> Listar(Int32? codUsrId);
        #endregion
        #region [Update]
        #endregion
        #region [Delete]
        /// <summary>
        /// Remover todos os favoritos do usúario.
        /// </summary>
        /// <param name="codUsrId">Código de identificação do usúario.</param>
        void Excluir(Int32 codUsrId);
        #endregion
    }
}
