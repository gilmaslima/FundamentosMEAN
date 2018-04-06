/*
© Copyright 2014 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using DTO = Redecard.PN.Extrato.Modelo.RelatorioValoresConsolidadosVendas;
using AutoMapper;

namespace Redecard.PN.Extrato.Servicos.Modelo.RelatorioValoresConsolidadosVendas
{
    /// <summary>
    /// Turquia - Resumo das vendas débito por dia.
    /// </summary>
    [DataContract]
    public class ResumoVendasDebitoPorDia : TotalVendasBase
    {
        /// <summary>
        /// Número do estabelecimento
        /// </summary>
        [DataMember]
        public Int32 NumeroEstabelecimento { get; set; }

        /// <summary>
        /// Data da venda
        /// </summary>
        [DataMember]
        public DateTime? DataVenda { get; set; }

        /// <summary>
        /// Data do pagamento
        /// </summary>
        [DataMember]
        public DateTime? DataPagamento { get; set; }

        /// <summary>
        /// Prazo de recebimento
        /// </summary>
        [DataMember]
        public Int32 PrazoRecebimento { get; set; }

        /// <summary>
        /// Número do resumo de venda
        /// </summary>
        [DataMember]
        public Int32 NumeroResumoVenda { get; set; }

        /// <summary>
        /// Quantidade de vendas
        /// </summary>
        [DataMember]
        public Int32 QuantidadeVendas { get; set; }

        /// <summary>
        /// Tipo de venda
        /// </summary>
        [DataMember]
        public String TipoVenda { get; set; }

        /// <summary>
        /// Código da bandeira
        /// </summary>
        [DataMember]
        public Int16 CodigoBandeira { get; set; }

        /// <summary>
        /// Descrição da bandeira
        /// </summary>
        [DataMember]
        public String DescricaoBandeira { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<ResumoVendasDebitoPorDia> FromDto(List<DTO.ResumoVendasDebitoPorDia> dto)
        {
            Mapper.CreateMap<DTO.ResumoVendasDebitoPorDia, ResumoVendasDebitoPorDia>();
            return Mapper.Map<List<DTO.ResumoVendasDebitoPorDia>, List<ResumoVendasDebitoPorDia>>(dto);
        }
    }
}
