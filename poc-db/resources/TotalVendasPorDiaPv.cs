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
    /// Turquia - Total de vendas por dia PV
    /// </summary>
    [DataContract]
    public class TotalVendasPorDiaPv : TotalVendasBase
    {
        /// <summary>
        /// Número do estabelecimento
        /// </summary>
        [DataMember]
        public Int32 NumeroEstabelecimanto { get; set; }

        /// <summary>
        /// Data da realização da venda
        /// </summary>
        [DataMember]
        public DateTime? DataVenda { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static TotalVendasPorDiaPv FromDto(DTO.TotalVendasPorDiaPv dto)
        {
            Mapper.CreateMap<DTO.TotalVendasPorDiaPv, TotalVendasPorDiaPv>();
            return Mapper.Map<DTO.TotalVendasPorDiaPv, TotalVendasPorDiaPv>(dto);
        }
    }
}
