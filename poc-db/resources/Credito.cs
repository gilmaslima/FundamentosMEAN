/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.Collections.Generic;
using System.Runtime.Serialization;
using AutoMapper;
using Redecard.PN.Extrato.Servicos.Modelo;
using DTO = Redecard.PN.Extrato.Modelo.Vendas;

namespace Redecard.PN.Extrato.Servicos.Vendas
{
    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Vendas - Crédito.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1311 / Programa WA1311 / TranID ISHB
    /// </remarks>
    [DataContract]
    public class Credito : BasicContract
    {
        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<Credito> FromDTO(List<DTO.Credito> dto)
        {
            Mapper.CreateMap<DTO.Credito, Credito>()
                    .Include<DTO.CreditoD, CreditoD>();
            Mapper.CreateMap<DTO.CreditoD, CreditoD>();
            return Mapper.Map<List<DTO.Credito>, List<Credito>>(dto);
        }
    }
}