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
    /// Turquia - Total de vendas débito por período.
    /// </summary>
    [DataContract]
    public class TotalVendasDebitoPorPeriodo : TotalVendasBase
    {
        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static TotalVendasDebitoPorPeriodo FromDto(DTO.TotalVendasDebitoPorPeriodo dto)
        {
            Mapper.CreateMap<DTO.TotalVendasDebitoPorPeriodo, TotalVendasDebitoPorPeriodo>();
            return Mapper.Map<DTO.TotalVendasDebitoPorPeriodo, TotalVendasDebitoPorPeriodo>(dto);
        }
    }
}
