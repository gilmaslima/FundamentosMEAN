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
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Vendas - Construcard.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1315 / Programa WA1315 / TranID ISHF
    /// </remarks>
    [DataContract]
    public class Construcard : BasicContract
    {
        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<Construcard> FromDTO(List<DTO.Construcard> dto)
        {
            Mapper.CreateMap<DTO.Construcard, Construcard>()
                    .Include<DTO.ConstrucardDT, ConstrucardDT>()
                    .Include<DTO.ConstrucardA1, ConstrucardA1>()
                    .Include<DTO.ConstrucardA2, ConstrucardA2>();
            Mapper.CreateMap<DTO.ConstrucardDT, ConstrucardDT>();
            Mapper.CreateMap<DTO.ConstrucardA1, ConstrucardA1>();
            Mapper.CreateMap<DTO.ConstrucardA2, ConstrucardA2>();
            return Mapper.Map<List<DTO.Construcard>, List<Construcard>>(dto);
        }
    }
}