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
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Modelo.RelatorioValoresConsolidadosVendas
{
    /// <summary>
    /// Turquia - Total de vendas Base
    /// </summary>
    [DataContract]
    public class TotalVendasBase : BasicContract
    {
        /// <summary>
        /// Valor total bruto de vendas
        /// </summary>
        [DataMember]
        public Decimal TotalBruto { get; set; }

        /// <summary>
        /// Valor total liquido de vendas
        /// </summary>
        [DataMember]
        public Decimal TotalLiquido { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static TotalVendasBase FromDto(DTO.TotalVendasBase dto)
        {
            Mapper.CreateMap<DTO.TotalVendasBase, TotalVendasBase>();
            return Mapper.Map<DTO.TotalVendasBase, TotalVendasBase>(dto);
        }
    }
}
