/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;
using AutoMapper;
using Redecard.PN.Extrato.Servicos.Modelo;
using DTO = Redecard.PN.Extrato.Modelo.Vendas;

namespace Redecard.PN.Extrato.Servicos.Vendas
{
    /// <summary>
    /// Classe modelo para Relatório de Vendas - Recarga de Celular.
    /// Reutilizado para os relatório de PV Físico e PV Lógico.
    /// </summary>
    [DataContract]
    public class RecargaCelularTotalizador : BasicContract
    {
        /// <summary>
        /// Total Valor Bruto de Recarga de Celular
        /// </summary>
        [DataMember]
        public Decimal TotalValorBrutoRecarga { get; set; }

        /// <summary>
        /// Total Valor Líquido de Comissão
        /// </summary>
        [DataMember]
        public Decimal TotalValorLiquidoComissao { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static RecargaCelularTotalizador FromDTO(DTO.RecargaCelularTotalizador dto)
        {
            Mapper.CreateMap<DTO.RecargaCelularTotalizador, RecargaCelularTotalizador>();
            return Mapper.Map<DTO.RecargaCelularTotalizador, RecargaCelularTotalizador>(dto);
        }
    }
}