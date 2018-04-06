/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos.PlanoContas
{
    /// <summary>
    /// Classe representando uma Oferta do Plano de Contas
    /// </summary>
    [DataContract]
    public class Oferta
    {
        /// <summary>
        /// Data de Adesão (Aceite)
        /// </summary>
        [DataMember]
        public DateTime DataAdesao { get; set; }

        /// <summary>
        /// Período Inicial de Vigência
        /// </summary>
        [DataMember]
        public DateTime PeriodoInicialVigencia { get; set; }

        /// <summary>
        /// Período Final de Vigência
        /// </summary>
        [DataMember]
        public DateTime PeriodoFinalVigencia { get; set; }

        /// <summary>
        /// Código da Oferta
        /// </summary>
        [DataMember]
        public Int64 CodigoOferta { get; set; }

        /// <summary>
        /// Nome da Oferta
        /// </summary>
        [DataMember]
        public String NomeOferta { get; set; }

        /// <summary>
        /// Descrição da Oferta
        /// </summary>
        [DataMember]
        public String DescricaoOferta { get; set; }

        /// <summary>
        /// Agência Adesão (Aceite)
        /// </summary>
        [DataMember]
        public Int32 AgenciaAdesao { get; set; }

        /// <summary>
        /// Conta Adesão (Aceite)
        /// </summary>
        [DataMember]
        public String ContaAdesao { get; set; }

        /// <summary>
        /// Código da Proposta
        /// </summary>
        [DataMember]
        public Decimal CodigoProposta { get; set; }

        /// <summary>
        /// Indicador de Oferta Atual
        /// </summary>
        [DataMember]
        public Boolean IndicadorOfertaAtual { get; set; }

        /// <summary>
        /// Status da Oferta
        /// </summary>
        [DataMember]
        public StatusOferta Status { get; set; }

        /// <summary>
        /// Tipo da Oferta
        /// <para>MDR Regressivo</para>
        /// <para>Aluguel</para>
        /// <para>Bônus Celular</para>
        /// <para>Preço Único</para>
        /// </summary>
        [DataMember]
        public String TipoOferta { get; set; }

        /// <summary>
        /// Origem da Oferta
        /// <para>UK (MDR)</para>
        /// <para>ZP (outras ofertas)</para>
        /// </summary>
        [DataMember]
        public String Origem { get; set; }

        /// <summary>
        /// Data de Contracação da Oferta (Ofertas do ZP não possuem esta data)
        /// </summary>
        [DataMember]
        public DateTime? DataContratacao { get; set; }
    }
}