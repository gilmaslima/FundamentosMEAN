/*
© Copyright 2017 Rede S.A.
Autor : Lucas Akira Uehara
Empresa : Iteris Consultoria e Software
*/
using Rede.PN.AtendimentoDigital.Core.Padroes.Repositorio;
using Rede.PN.AtendimentoDigital.Modelo.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rede.PN.AtendimentoDigital.Modelo.Entidades
{
    /// <summary>
    /// FavoritoEntidade
    /// </summary>
    public class FavoritoEntidade : IEntidade<FavoritoChave>
    {
        #region [Entrada]
        /// <summary>
        /// Define a chave.
        /// </summary>
        public FavoritoChave Chave { get; set; }
        #endregion

        #region [Saída]
        /// <summary>
        /// Define a Data de Inclusão no retorno.
        /// </summary>
        public DateTime DthInclusao { get; set; }
        #endregion
    }
}
