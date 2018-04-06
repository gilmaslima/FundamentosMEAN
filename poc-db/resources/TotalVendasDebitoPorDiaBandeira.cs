﻿/*
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
    /// Turquia - Total de vendas débito por dia separado por bandeiras.
    /// </summary>
    [DataContract]
    public class TotalVendasDebitoPorDiaBandeira : TotalVendasBase
    {
        /// <summary>
        /// Lista de totais de vendas por bandeira
        /// </summary>
        [DataMember]
        public List<TotalVendasPorBandeira> ListaTotalVendasDebitoPorBandeira { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static TotalVendasDebitoPorDiaBandeira FromDto(DTO.TotalVendasDebitoPorDiaBandeira dto)
        {
            Mapper.CreateMap<DTO.TotalVendasDebitoPorDiaBandeira, TotalVendasDebitoPorDiaBandeira>();
            Mapper.CreateMap<DTO.TotalVendasPorBandeira, TotalVendasPorBandeira>();
            return Mapper.Map<DTO.TotalVendasDebitoPorDiaBandeira, TotalVendasDebitoPorDiaBandeira>(dto);
        }
    }
}
