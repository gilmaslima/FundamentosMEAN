using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public class DomicilioBancarioMatriz
    {

        [DataMember]
        public Int32? CodigoErro { get; set; }

        [DataMember]
        public String DescricaoricaoErro { get; set; }

        [DataMember]
        public TipoOperacao CodigoTipoOperacao { get; set; }

        [DataMember]
        public String DescricaoTipoOperacao { get; set; }

        [DataMember]
        public String SiglaProduto { get; set; }

        [DataMember]
        public String DescricaoSiglaProduto { get; set; }

        [DataMember]
        public Int32? CodigoBancoAtual { get; set; }

        [DataMember]
        public String NomeBancoAtual { get; set; }

        [DataMember]
        public Int32? CodigoAgenciaAtual { get; set; }

        [DataMember]
        public String NomeAgenciaAtual { get; set; }

        [DataMember]
        public String DigitoAgencia { get; set; }

        [DataMember]
        public Int32? CodigoBancoInterno { get; set; }

        [DataMember]
        public String NumeroContaAtual { get; set; }

        [DataMember]
        public Int32? CodigoBancoAnterior { get; set; }

        [DataMember]
        public Int32? CodigoAgenciaAnterior { get; set; }

        [DataMember]
        public String DigitoAgenciaAnterior { get; set; }

        [DataMember]
        public String NumeroContaAnterior { get; set; }

        [DataMember]
        public Int32? CodigoBancoNovo { get; set; }

        [DataMember]
        public String NomeBancoNovo { get; set; }

        [DataMember]
        public Int32? CodigoAgenciaNovo { get; set; }

        [DataMember]
        public String NomeAgenciaNovo { get; set; }

        [DataMember]
        public String DigitoAgenciaNovo { get; set; }

        [DataMember]
        public String NumeroContaNovo { get; set; }

        [DataMember]
        public DateTime? DataAtualizacaoDomicilio { get; set; }

        [DataMember]
        public String UsuarioAtualizacaoDomicilio { get; set; }

        [DataMember]
        public DateTime? DataLiberacaoDomicilio { get; set; }

        [DataMember]
        public String UsuarioLiberacaoDomicilio { get; set; }

        [DataMember]
        public Int32? IndicadorFormaPagamento { get; set; }

        [DataMember]
        public DateTime? DataUltAtualizacao { get; set; }

        [DataMember]
        public String HoraUltAtualizacao { get; set; }

        [DataMember]
        public String UsuarioUltAtualizacao { get; set; }

        [DataMember]
        public String TimestampRegistro { get; set; }

        [DataMember]
        public Int32? CodigoCanalAtual { get; set; }

        [DataMember]
        public Int32? CodigoCelulaAtual { get; set; }

        [DataMember]
        public Int32? CodigoAgenciaCanalAtual { get; set; }

        [DataMember]
        public Int32? CodigoCanalAnterior { get; set; }

        [DataMember]
        public Int32? CodigoCelulaAnterior { get; set; }

        [DataMember]
        public Int32? CodigoAgenciaCanalAnterior { get; set; }

        [DataMember]
        public Int32? CodigoCanalNovo { get; set; }

        [DataMember]
        public Int32? CodigoCelulaNovo { get; set; }

        [DataMember]
        public Int32? CodigoAgenciaCanalNovo { get; set; }

        [DataMember]
        public Int32? NumeroSolicitacao { get; set; }

        [DataMember]
        public Int32? NumeroSolicitacaoAnterior { get; set; }

    }
}
