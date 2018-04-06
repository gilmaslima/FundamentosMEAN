/*
© Copyright 2017 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Runtime.Serialization;

namespace Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoDados.Request
{
    /// <summary>
    /// Classe AtualizarFavoritosUsuarioRequest
    /// </summary>
    [DataContract]
    public class AtualizarFavoritosUsuarioRequest
    {
        /// <summary>
        /// Propriedade CodUsrId
        /// </summary>
        [DataMember]
        public Int32 CodUsrId { get; set; }

        /// <summary>
        /// Propriedade CodigosServico
        /// </summary>
        [DataMember]
        public Int32[] CodigosServico { get; set; }
    }
}