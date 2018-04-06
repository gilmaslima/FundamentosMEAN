/*
© Copyright 2015 Rede S.A.
Autor : Dhouglas Lombello
Empresa : Iteris Consultoria e Software
*/

using System.Collections.Generic;
using System.Runtime.Serialization;
using AutoMapper;
using Redecard.PN.Extrato.Servicos.Modelo;
using DTO = Redecard.PN.Extrato.Modelo.Estornos;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Vendas - Crédito.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book BKWA2940 / Programa WAC294 / TranID WAAQ
    /// </remarks>
    [DataContract]
    public class Estorno : BasicContract
    {
        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<Estorno> FromDTO(List<DTO.Estorno> dto)
        {
            Mapper.CreateMap<DTO.Estorno, Estorno>()
                    .Include<DTO.EstornoD, EstornoD>();
            Mapper.CreateMap<DTO.EstornoD, EstornoD>();
            return Mapper.Map<List<DTO.Estorno>, List<Estorno>>(dto);
        }
    }
}