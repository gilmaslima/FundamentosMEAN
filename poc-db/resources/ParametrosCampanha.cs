using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public class ParametrosCampanha
    {
        [DataMember]
        public String CodigoCampanha { get; set; }
        [DataMember]
        public Char? CodigoTipoParametro { get; set; }
        [DataMember]
        public Int32? SequenciaParametro { get; set; }
        [DataMember]
        public Int32? CodigoCca { get; set; }
        [DataMember]
        public Int32? CodigoFeature { get; set; }
        [DataMember]
        public Char? IndicadorTipoOperacao { get; set; }
        [DataMember]
        public Int32? CodigoBancoCompensacao { get; set; }
        [DataMember]
        public String CodigoTipoEquipamento { get; set; }
        [DataMember]
        public Int32? CodigoServico { get; set; }
        [DataMember]
        public Int32? IndicadorParametroAtivo { get; set; }
        [DataMember]
        public String DescricaoParametro { get; set; }
        [DataMember]
        public Int32? CodigoRegimeParametro { get; set; }
        [DataMember]
        public Int32? PrazoParametro { get; set; }
        [DataMember]
        public Double? ValorTaxaParametro { get; set; }
        [DataMember]
        public String UsuarioInclusao { get; set; }
        [DataMember]
        public DateTime? DataHoraInclusaoRegistro { get; set; }
        [DataMember]
        public String UsuarioUltimaAtualizacao { get; set; }
        [DataMember]
        public DateTime? DataHoraUltimaAtualizacao { get; set; }
        [DataMember]
        public Int32? CodigoCenario { get; set; }
        [DataMember]
        public String CodigoEventoEspecial { get; set; }
        [DataMember]
        public Int32? ValorInicioCotaEquipamento { get; set; }
        [DataMember]
        public Int32? ValorSaldoCotaEquipamento { get; set; }

    }
}
