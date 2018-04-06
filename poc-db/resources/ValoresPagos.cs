/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using AutoMapper;
using DTO = Redecard.PN.Extrato.Modelo.HomePage;

namespace Redecard.PN.Extrato.Servicos.Modelo.HomePage
{
    /// <summary>
    /// Valores pagos
    /// </summary>
    [DataContract]
    public class ValoresPagos
    {
        /// <summary>Total bruto</summary>
        [DataMember]
        public Decimal TotalBruto { get; set; }

        /// <summary>total líquido</summary>
        [DataMember]
        public Decimal TotalLiquido { get; set; }

        /// <summary>Totais por data de recebimento</summary>
        [DataMember]
        public List<Resumo> Resumos { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static ValoresPagos FromDTO(DTO.ValoresPagos dto)
        {
            Mapper.CreateMap<DTO.ValoresPagos, ValoresPagos>();
            Mapper.CreateMap<DTO.Resumo, Resumo>();
            return Mapper.Map<ValoresPagos>(dto);
        }
    }
}