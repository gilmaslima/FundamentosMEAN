/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.OutrosServicos.Modelo
{
    /// <summary>
    /// Tipo do Estabelecimento
    /// </summary>
    public enum TipoEstabelecimentoHistoricoOferta : int
    {
        /// <summary>
        /// Nenhum Tipo
        /// </summary>
        Nenhum = 0,

        /// <summary>
        /// Estabelecimento Autonomo
        /// </summary>
        Autonomo = 1,

        /// <summary>
        /// Estabelecimento Filial
        /// </summary>
        Filial = 2,

        /// <summary>
        /// Estabelecimento Matriz
        /// </summary>
        Matriz = 3,

        /// <summary>
        /// Estabelecimento Grupo Comercial
        /// </summary>
        GrupoComercial = 4,

        /// <summary>
        /// Estabelecimento Grupo Gerencial
        /// </summary>
        GrupoGerencial = 5,
    }
}
