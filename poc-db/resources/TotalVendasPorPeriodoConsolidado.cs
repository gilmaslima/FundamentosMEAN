/*
© Copyright 2014 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using DTO = Redecard.PN.Extrato.Modelo.RelatorioValoresConsolidadosVendas;
using AutoMapper;

namespace Redecard.PN.Extrato.Servicos.Modelo.RelatorioValoresConsolidadosVendas
{
    /// <summary>
    /// Turquia - Total vendas por período consolidado
    /// </summary>
    [DataContract]
    public class TotalVendasPorPeriodoConsolidado
    {
        /// <summary>
        /// Total de vendas bruto á crédito
        /// </summary>
        [DataMember]
        public Decimal TotalVendasCredito { get; set; }

        /// <summary>
        /// Total de vendas bruto á débito
        /// </summary>
        [DataMember]
        public Decimal TotalVendasDebito { get; set; }

        /// <summary>
        /// Soma dos totais de vendas crédito e débito
        /// </summary>
        [DataMember]
        public Decimal TotalVendasPeriodo { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static TotalVendasPorPeriodoConsolidado FromDto(DTO.TotalVendasPorPeriodoConsolidado dto)
        {
            Mapper.CreateMap<DTO.TotalVendasPorPeriodoConsolidado, TotalVendasPorPeriodoConsolidado>();
            return Mapper.Map<DTO.TotalVendasPorPeriodoConsolidado, TotalVendasPorPeriodoConsolidado>(dto);
        }
    }
}
