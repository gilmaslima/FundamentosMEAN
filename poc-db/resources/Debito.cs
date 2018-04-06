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
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Vendas - Débito.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1313 / Programa WA1313 / TranID ISHD
    /// </remarks>
    [DataContract]
    public class Debito : BasicContract
    {
        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<Debito> FromDTO(List<DTO.Debito> dto)
        {
            Mapper.CreateMap<DTO.Debito, Debito>()
                    .Include<DTO.DebitoDT, DebitoDT>()
                    .Include<DTO.DebitoA1, DebitoA1>()
                    .Include<DTO.DebitoA2, DebitoA2>();
            Mapper.CreateMap<DTO.DebitoDT, DebitoDT>();
            Mapper.CreateMap<DTO.DebitoA1, DebitoA1>();
            Mapper.CreateMap<DTO.DebitoA2, DebitoA2>();
            return Mapper.Map<List<DTO.Debito>, List<Debito>>(dto);
        }
    }
}