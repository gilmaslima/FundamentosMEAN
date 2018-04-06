/*
© Copyright 2015 Rede S.A.
Autor : Dhouglas Lombello
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using AutoMapper;
using Redecard.PN.Extrato.Servicos.Modelo;
using DTO = Redecard.PN.Extrato.Modelo.Estornos;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos totalizadores do Relatório de Estorno.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book BKWA2930 / Programa WAC293 / TranID WAAP
    /// </remarks>
    [DataContract]
    public class EstornoTotalizador : BasicContract
    {
        /// <summary>Quantidade de Registros</summary>
        [DataMember]
        public Int32 QuantidadeRegistros { get; set; }

        /// <summary>Lista de Bandeiras</summary>
        [DataMember]
        public List<EstornoTotalizadorBandeira> Bandeiras { get; set; }

        /// <summary>Valor Total de Estornos</summary>
        [DataMember]
        public Decimal ValorTotalEstornos { get; set; }

        /// <summary>
        /// Tipo de Venda de Estornos
        /// </summary>
        [DataMember]
        public Int16 CodigoTipoVenda { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static EstornoTotalizador FromDTO(DTO.EstornoTotalizador dto)
        {
            Mapper.CreateMap<DTO.EstornoTotalizador, EstornoTotalizador>();
            Mapper.CreateMap<DTO.EstornoTotalizadorBandeira, EstornoTotalizadorBandeira>();
            return Mapper.Map<DTO.EstornoTotalizador, EstornoTotalizador>(dto);
        }
    }
}