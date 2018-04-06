using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public class DomicilioBancario
    {

        [DataMember]
        public TipoDomicilioBancario TipoDomicilioBancario { get; set; }

        [DataMember]
        public Char CodigoTipoPessoa { get; set; }

        [DataMember]
        public Int64 NumeroCNPJ { get; set; }

        [DataMember]
        public Int32 NumeroSeqProp { get; set; }

        [DataMember]
        public Int32 IndicadorTipoOperacaoProd { get; set; }

        [DataMember]
        public Char IndicadorDomicilioDuplicado { get; set; }

        [DataMember]
        public Int32 CodigoBanco { get; set; }

        [DataMember]
        public String NomeBanco { get; set; }

        [DataMember]
        public Int32 CodigoAgencia { get; set; }

        [DataMember]
        public String NumeroContaCorrente { get; set; }

        [DataMember]
        public DateTime DataHoraUltimaAtualizacao { get; set; }

        [DataMember]
        public String UsuarioUltimaAtualizacao { get; set; }

        [DataMember]
        public Char IndConfirmacaoDomicilio { get; set; }

        [DataMember]
        public TipoAcaoBanco IndicadorTipoAcaoBanco { get; set; }
    }
}
