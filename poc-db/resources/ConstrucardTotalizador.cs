/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using AutoMapper;
using Redecard.PN.Extrato.Servicos.Modelo;
using DTO = Redecard.PN.Extrato.Modelo.Vendas;

namespace Redecard.PN.Extrato.Servicos.Vendas
{

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos totalizadores do Relatório de Vendas - Construcard.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1314 / Programa WA1314 / TranID ISHE
    /// </remarks>
    [DataContract]
    public class ConstrucardTotalizador : BasicContract
    {
        /// <summary>Quantidade de Registros</summary>
        [DataMember]
        public Int32 QuantidadeRegistros { get; set; }

        /// <summary>Lista de Totais por Bandeira</summary>
        [DataMember]
        public List<ConstrucardTotalizadorValor> Valores { get; set; }

        /// <summary>Total Valor Bruto</summary>
        [DataMember]
        public Decimal ValorBruto { get; set; }

        /// <summary>Total Valor Líquido</summary>
        [DataMember]
        public Decimal ValorLiquido { get; set; }

        /// <summary>Total Valor Descontado</summary>
        [DataMember]
        public Decimal ValorDescontado { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static ConstrucardTotalizador FromDTO(DTO.ConstrucardTotalizador dto)
        {
            Mapper.CreateMap<DTO.ConstrucardTotalizador, ConstrucardTotalizador>();
            Mapper.CreateMap<DTO.ConstrucardTotalizadorValor, ConstrucardTotalizadorValor>();
            return Mapper.Map<DTO.ConstrucardTotalizador, ConstrucardTotalizador>(dto);
        }
    }
}