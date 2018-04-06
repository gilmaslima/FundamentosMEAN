using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public class DetalheCampanha
    {

        [DataMember]
        public String CodigoCampanha { get; set; }

        [DataMember]
        public String NomeCampanha { get; set; }

        [DataMember]
        public DateTime? DataInicioCampanha { get; set; }

        [DataMember]
        public DateTime? DataFimCampanha { get; set; }

        [DataMember]
        public Char? IndicadorSituacaoCampanha { get; set; }

        [DataMember]
        public Char? IndicadorBloqueioCampanha { get; set; }

        [DataMember]
        public Int32? QuantidadeCarencia { get; set; }

        [DataMember]
        public Int32? IndicadorCampanhaAtiva { get; set; }

        [DataMember]
        public String DescricaoCampanha { get; set; }

        [DataMember]
        public Char? IndicadorNaoEnviaCartaComercial { get; set; }

        [DataMember]
        public Char? IndicadorNaoEnvioFichaAdesao1 { get; set; }

        [DataMember]
        public Char? IndicadorNaoEnvioFichaAdesao2 { get; set; }

        [DataMember]
        public Char? IndicadorNaoEnvioOs { get; set; }

        [DataMember]
        public Char? IndicadorNaoEnvioInvestCadastral { get; set; }

        [DataMember]
        public Char? IndicadorNaoEnvioProposta { get; set; }

        [DataMember]
        public String UsuarioInclusao { get; set; }

        [DataMember]
        public DateTime? DataHoraInclusaoRegistro { get; set; }

        [DataMember]
        public String UsuarioUltimaAtualizacao { get; set; }

        [DataMember]
        public DateTime? DataHoraUltimaAtualizacao { get; set; }

        [DataMember]
        public Char? IndicadorTipoCampanha { get; set; }


    }
}
