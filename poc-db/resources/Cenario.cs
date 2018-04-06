using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public class Cenario
    {

        [DataMember]
        public Int32 CodigoCanal { get; set; }

        [DataMember]
        public Int32 CodigoCenario { get; set; }

        [DataMember]
        public String DescricaoCenario { get; set; }

        [DataMember]
        public Char CodigoSituacaoCenario { get; set; }

        [DataMember]
        public Double ValorCenario { get; set; }

        [DataMember]
        public Char IndicadorEscalonamento { get; set; }

        [DataMember]
        public Double ValorEscalonamentoMes1 { get; set; }

        [DataMember]
        public Double ValorEscalonamentoMes2 { get; set; }

        [DataMember]
        public Double ValorEscalonamentoMes3 { get; set; }

        [DataMember]
        public Double ValorEscalonamentoMes4 { get; set; }

        [DataMember]
        public Double ValorEscalonamentoMes5 { get; set; }

        [DataMember]
        public Double ValorEscalonamentoMes6 { get; set; }

        [DataMember]
        public Double ValorEscalonamentoMes7 { get; set; }

        [DataMember]
        public Double ValorEscalonamentoMes8 { get; set; }

        [DataMember]
        public Double ValorEscalonamentoMes9 { get; set; }

        [DataMember]
        public Double ValorEscalonamentoMes10 { get; set; }

        [DataMember]
        public Double ValorEscalonamentoMes11 { get; set; }

        [DataMember]
        public Double ValorEscalonamentoMes12 { get; set; }

        [DataMember]
        public Double ValorDescontoMes1 { get; set; }

        [DataMember]
        public Double ValorDescontoMes2 { get; set; }

        [DataMember]
        public Double ValorDescontoMes3 { get; set; }

        [DataMember]
        public Double ValorDescontoMes4 { get; set; }

        [DataMember]
        public Double ValorDescontoMes5 { get; set; }

        [DataMember]
        public Double ValorDescontoMes6 { get; set; }

        [DataMember]
        public Double ValorDescontoMes7 { get; set; }

        [DataMember]
        public Double ValorDescontoMes8 { get; set; }

        [DataMember]
        public Double ValorDescontoMes9 { get; set; }

        [DataMember]
        public Double ValorDescontoMes10 { get; set; }

        [DataMember]
        public Double ValorDescontoMes11 { get; set; }

        [DataMember]
        public Double ValorDescontoMes12 { get; set; }

        [DataMember]
        public Char IndicadorSazonalidade { get; set; }

        [DataMember]
        public Double PercentualSazonalidadeJan { get; set; }

        [DataMember]
        public Double PercentualSazonalidadeFev { get; set; }

        [DataMember]
        public Double PercentualSazonalidadeMar { get; set; }

        [DataMember]
        public Double PercentualSazonalidadeAbr { get; set; }

        [DataMember]
        public Double PercentualSazonalidadeMai { get; set; }

        [DataMember]
        public Double PercentualSazonalidadeJun { get; set; }

        [DataMember]
        public Double PercentualSazonalidadeJul { get; set; }

        [DataMember]
        public Double PercentualSazonalidadeAgo { get; set; }

        [DataMember]
        public Double PercentualSazonalidadeSet { get; set; }

        [DataMember]
        public Double PercentualSazonalidadeOut { get; set; }

        [DataMember]
        public Double PercentualSazonalidadeNov { get; set; }

        [DataMember]
        public Double PercentualSazonalidadeDez { get; set; }

        [DataMember]
        public String CodigoTipoEquipamento { get; set; }

        [DataMember]
        public String UsuarioUltimaAtualizacao { get; set; }

        [DataMember]
        public DateTime DataHoraUltimaAtualizacao { get; set; }

        [DataMember]
        public Char CodigoSituacaoCenarioCanal { get; set; }

        [DataMember]
        public String Timestamp { get; set; }

    }
}
