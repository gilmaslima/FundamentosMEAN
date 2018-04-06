/*
(c) Copyright [2012] Redecard S.A.
Autor : [Daniel Coelho]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/07/30 - Daniel Coelho - Versão Inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.RAV.Servicos
{
    /// <summary>
    /// Classe de entidade do RAV Avulso - Entrada.
    /// </summary>
    [DataContract]
    public class ModRAVAvulsoEntrada
    {
        [DataMember]
        public int NumeroPDV { get; set; }

        [DataMember]
        public EFuncaoEntrada Funcao { get; set; }

        [DataMember]
        public int DiasCredito { get; set; }

        [DataMember]
        public ECanal Canal { get; set; }

        [DataMember]
        public decimal ValorAntecipado { get; set; }

        [DataMember]
        public ModRAVAntecipa DadosAntecipacao { get; set; }
    }

    /// <summary>
    /// Classe para passar todos os parâmetros do MA30
    /// </summary>
    [DataContract]
    public class MA30
    {
        [DataMember]
        public int MA030_NUM_PDV { get; set; }
        [DataMember]
        public short MA030_FUNCAO { get; set; }
        [DataMember]
        public short MA030_TIP_CREDITO { get; set; }
        [DataMember]
        public short MA030_CANAL { get; set; }
        [DataMember]
        public decimal MA030_VALOR_A_ANTECIPAR { get; set; }
        [DataMember]
        public string MA030_CA_IND_ANTEC { get; set; }
        [DataMember]
        public string MA030_CA_VAL_ANTEC { get; set; }
        [DataMember]
        public string MA030_CA_IND_DATA_ANTEC { get; set; }
        [DataMember]
        public string MA030_CA_PER_DATA_DE { get; set; }
        [DataMember]
        public string MA030_CA_PER_DATA_ATE { get; set; }
        [DataMember]
        public string MA030_DAT_RESTRICAO { get; set; }
        [DataMember]
        public string MA030_CA_IND_PRODUTO { get; set; }
        [DataMember]
        public string MA030_DAT_PROCESSAMENTO {get;set;}
        [DataMember]
        public string MA030_HOR_PROCESSAMENTO{get;set;}
        [DataMember]
        public short MA030_BANCO{get;set;}
        [DataMember]
        public int MA030_AGENCIA{get;set;}
        [DataMember]
        public decimal MA030_CONTA{get;set;}
        [DataMember]
        public decimal MA030_VALOR_MINIMO{get;set;}
        [DataMember]
        public short MA030_HORA_INI_D0{get;set;}
        [DataMember]
        public short MA030_HORA_FIM_D0{get;set;}
        [DataMember]
        public short MA030_HORA_INI_DN{get;set;}
        [DataMember]
        public short MA030_HORA_FIM_DN{get;set;}
        [DataMember]
        public decimal MA030_PCT_DESCONTO{get;set;}
        [DataMember]
        public decimal MA030_VALOR_BRUTO{get;set;}
        [DataMember]
        public decimal MA030_VALOR_ORIG{get;set;}
        [DataMember]
        public string MA030_DAT_PERIODO_DE{get;set;}
        [DataMember]
        public string MA030_DAT_PERIODO_ATE{get;set;}
        [DataMember]
        public string MA030_MSGERRO{get;set;}
        [DataMember]
        public string MA030_DATA_FIM_CARENCIA{get;set;}
        [DataMember]
        public decimal MA030_VALOR_ANTEC_D0{get;set;}
        [DataMember]
        public decimal MA030_VALOR_ANTEC_D1{get;set;}
        [DataMember]
        public decimal MA030_VALOR_DISP_ANTEC{get;set;}
        [DataMember]
        public int MA030_RV_QTD_RV{get;set;}
        [DataMember]
        public List<FILLER1> filler1 { get; set; }
        [DataMember]
        public List<FILLER> filler { get; set; }
    }

    [DataContract]
    public class FILLER
    {
        [DataMember]
        public string MA030_DAT_CREDITO { set; get; }
        [DataMember]
        public decimal MA030_PCT_EFETIVA { set; get; }
        [DataMember]
        public decimal MA030_PCT_PERIODO { set; get; }
        [DataMember]
        public decimal MA030_VALOR_LIQUIDO { set; get; }
        [DataMember]
        public decimal MA030_VALOR_PARCELADO { set; get; }
        [DataMember]
        public decimal MA030_VALOR_ROTATIVO { set; get; }
    }

    [DataContract]
    public class FILLER1
    {
        [DataMember]
        public string MA030_RV_DAT_APRS { set; get; }
        [DataMember]
        public int MA030_RV_NUM_RV { set; get; }
        [DataMember]
        public int MA030_RV_QTD_OC { set; get; }
        [DataMember]
        public decimal MA030_RV_VAL_BRTO { set; get; }
        [DataMember]
        public decimal MA030_RV_VAL_LQDO { set; get; }
    }
    
    public enum EFuncaoEntrada
    { Consulta, Antecipacao, Extrato, ConsultaD0D1, Sonda, ValorHomePage, ConsultaDisponivelAntecipacao, ConsultarAntecipadosDia }

    public enum ECanal
    { RAVAutomatico, VendaPR, Mesa, Antendimento, IVR, Internet, POS }
}