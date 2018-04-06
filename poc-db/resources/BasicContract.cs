using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    // WACA1106 - Relatório de pagamentos ajustados.
    [KnownType(typeof(ConsultarOrdensCreditoEnviadosAoBancoDetalheRetorno))]
    [KnownType(typeof(ConsultarOrdensCreditoEnviadosAoBancoAjusteRetorno))]

    // WACA1111 - Relatório de créditos suspensos, retidos e penhorados - Créditos/Débitos suspensos.
    [KnownType(typeof(ConsultarSuspensaoDetalheRetorno))]
    [KnownType(typeof(ConsultarSuspensaoTotalBandeiraDiaRetorno))]
    [KnownType(typeof(ConsultarSuspensaoTotalBandeiraRetorno))]
    [KnownType(typeof(ConsultarSuspensaoTotalDiaRetorno))]
    [KnownType(typeof(ConsultarSuspensaoTotalPeriodoRetorno))]

    // WACA1112 - Relatório de créditos suspensos, retidos e penhorados - Créditos penhorados.
    [KnownType(typeof(ConsultarPenhoraNumeroProcessoRetorno))]
    [KnownType(typeof(ConsultarPenhoraDetalheProcessoCreditoRetorno))]
    [KnownType(typeof(ConsultarPenhoraTotalBandeiraRetorno))]
    [KnownType(typeof(ConsultarPenhoraTotalSemBandeiraRetorno))]

    // WACA1113 - Relatório de créditos suspensos, retidos e penhorados - Créditos retidos.
    [KnownType(typeof(ConsultarRetencaoNumeroProcessoRetorno))]
    [KnownType(typeof(ConsultarRetencaoDetalheProcessoCreditoRetorno))]
    [KnownType(typeof(ConsultarRetencaoDetalheProcessoDebitoRetorno))]
    [KnownType(typeof(ConsultarRetencaoDescricaoComValorRetorno))]
    [KnownType(typeof(ConsultarRetencaoDescricaoSemValorRetorno))]

    [DataContract]
    public class BasicContract
    {
        /// <summary>Tipo do Registro</summary>
        [DataMember]
        public string TipoRegistro { get; set; }
    }
}