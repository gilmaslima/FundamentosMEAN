/*
© Copyright 2017 Rede S.A.
Autor : Lucas Akira Uehara
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoDados.Response.FavoritoResponse
{
    /// <summary>
    /// Classe FavoritoResponseList.
    /// </summary>
    [DataContract]
    public sealed class FavoritoResponseList : ResponseBaseList<FavoritoResponse>
    {
        /// <summary>
        /// Total de Itens.
        /// </summary>
        [DataMember]
        public Int32 TotalItens { get; set; }
    }
}