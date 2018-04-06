/*
© Copyright 2017 Rede S.A.
Autor : Lucas Akira Uehara
Empresa : Iteris Consultoria e Software
*/
using Rede.PN.AtendimentoDigital.Modelo.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoDados.Response.FavoritoResponse
{
	/// <summary>
    /// Classe FavoriteResponse
	/// </summary>
    [DataContract]
	public sealed class FavoritoResponse
	{
        /// <summary>
        /// Define Chave.
        /// </summary>
        [DataMember]
        public FavoritoChave Chave { get; set; }
        /// <summary>
        /// Define DthInclusao.
        /// </summary>
        [DataMember]
        public DateTime DthInclusao { get; set; }
	}
}