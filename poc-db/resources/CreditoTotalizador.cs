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
    /// Classe utilizada no módulo Extrato para consulta dos totalizadores do Relatório de Vendas - Crédito.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1310 / Programa WA1310 / TranID ISHA
    /// </remarks>
    [DataContract]
    public class CreditoTotalizador : BasicContract
    {
        /// <summary>Quantidade de Registros</summary>
        [DataMember]
        public Int32 QuantidadeRegistros { get; set; }

        /// <summary>Lista de Registros</summary>
        [DataMember]
        public List<CreditoTotalizadorValor> Valores { get; set; }

        /// <summary>Valor Apresentado</summary>
        [DataMember]
        public Decimal ValorApresentado { get; set; }

        /// <summary>Valor Líquido</summary>
        [DataMember]
        public Decimal ValorLiquido { get; set; }

        /// <summary>Valor Descontado</summary>
        [DataMember]
        public Decimal ValorDescontado { get; set; }

        /// <summary>Valor Correção</summary>
        [DataMember]
        public Decimal ValorCorrecao { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static CreditoTotalizador FromDTO(DTO.CreditoTotalizador dto)
        {
            Mapper.CreateMap<DTO.CreditoTotalizador, CreditoTotalizador>();
            Mapper.CreateMap<DTO.CreditoTotalizadorValor, CreditoTotalizadorValor>();
            return Mapper.Map<DTO.CreditoTotalizador, CreditoTotalizador>(dto);
        }
    }
}