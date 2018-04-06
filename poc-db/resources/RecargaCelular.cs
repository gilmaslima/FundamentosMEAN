/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using DTO = Redecard.PN.Extrato.Modelo.RecargaCelular;
using AutoMapper;

namespace Redecard.PN.Extrato.Servicos.Recarga
{
    #region [ Recarga de Celular - Detalhes - BKWA2420 / WA242 / ISIB ]

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Recarga de Celular - Detalhe
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book BKWA2420 / Programa WA242 / TranID ISIB
    /// </remarks>
    [DataContract]
    public class RecargaCelularDetalhe
    {
        /// <summary>Número do NSU</summary>
        [DataMember]
        public Decimal NumeroNsu { get; set; }

        /// <summary>Data e Hora da Transação</summary>
        [DataMember]
        public DateTime? DataHoraTransacao { get; set; }

        /// <summary>Nome da Operadora</summary>
        [DataMember]
        public String NomeOperadora { get; set; }

        /// <summary>Número do Celular</summary>
        [DataMember]
        public String NumeroCelular { get; set; }

        /// <summary>Valor da Transação</summary>
        [DataMember]
        public Decimal ValorTransacao { get; set; }

        /// <summary>Valor da Comissão</summary>
        [DataMember]
        public Decimal ValorComissao { get; set; }

        /// <summary>Descrição do Status da Transação</summary>
        [DataMember]
        public String StatusTransacao { get; set; }

        /// <summary>Descrição do Status da Comissão</summary>
        [DataMember]
        public String StatusComissao { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<RecargaCelularDetalhe> FromDTO(List<DTO.RecargaCelularDetalhe> dto)
        {
            Mapper.CreateMap<DTO.RecargaCelularDetalhe, RecargaCelularDetalhe>();
            return Mapper.Map<List<RecargaCelularDetalhe>>(dto);
        }
    }

    #endregion
}