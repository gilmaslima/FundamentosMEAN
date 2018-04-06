using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Request.Servicos
{
    /// <summary>
    /// Classe auxiliar utilizada nas telas do módulo Request para consultas ao Mainframe.
    /// </summary>    
    [DataContract]
    public class ProcessosInicioFim
    {
        /// <summary>Número do Processo inicial</summary>
        [DataMember]
        public Decimal NumeroProcessoInicio;

        /// <summary>Tipo do Processo inicial</summary>
        [DataMember]
        public String TipoProcessoInicio;

        /// <summary>Ciclo do Processo inicial</summary>
        [DataMember]
        public String CicloProcessoInicio;

        /// <summary>Data de Emissão do Processo inicial</summary>
        [DataMember]
        public String DataEmissaoInicio;

        /// <summary>Número do Ponto de Venda do Processo inicial</summary>
        [DataMember]
        public Int32 NumeroPVInicio;

        /// <summary>Número do Processo final</summary>
        [DataMember]
        public Decimal NumeroProcessoFim;

        /// <summary>Tipo do Processo final</summary>
        [DataMember]
        public String TipoProcessoFim;

        /// <summary>Ciclo do Processo final</summary>
        [DataMember]
        public String CicloProcessoFim;

        /// <summary>Data de Emissão do Processo final</summary>
        [DataMember]
        public String DataEmissaoFim;

        /// <summary>Número do Ponto de Venda do Processo final</summary>
        [DataMember]
        public Int32 NumeroPVFim;
    }
}