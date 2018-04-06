/*
© Copyright 2017 Rede S.A.
Autor : Mário Neto
Empresa : Iteris Consultoria e Software
*/

using Rede.PN.AtendimentoDigital.Modelo.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoDados.Response.EnderecoCepResponse
{
    /// <summary>
    /// Classe ConsultaFavoritoResponse.
    /// </summary>
    [DataContract]
    public sealed class EnderecoCepResponseItem : ResponseBaseItem<EnderecoCepResponse> { }
}