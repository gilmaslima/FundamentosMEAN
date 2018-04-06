#region Histórico do Arquivo
/*
(c) Copyright [2012] BRQ IT Solutions.
Autor       : [- 2012/08/21 - Lucas Nicoletto da Cunha]
Empresa     : [BRQ IT Solutions]
Histórico   : Criação da Classe
- [08/06/2012] – [Lucas Nicoletto da Cunha] – [Criação]
 *
*/
#endregion

using System.Runtime.Serialization;
using System;

namespace Redecard.PN.Cancelamento.Modelo
{
    [DataContract]
    public class ItemCancelamentoSaida
    {
        private String _codRetorno;
        private String _codErro;
        private string _msgErro;
        private String _numAvisoCanc;
        private String _vlSaldoAtual;

        [DataMember]
        public String VlSaldoAtual
        {
            get { return _vlSaldoAtual; }
            set { _vlSaldoAtual = value; }
        }

        [DataMember]
        public String NumAvisoCanc
        {
            get { return _numAvisoCanc; }
            set { _numAvisoCanc = value; }
        }

        [DataMember]
        public string MsgErro
        {
            get { return _msgErro; }
            set { _msgErro = value; }
        }

        [DataMember]
        public String CodErro
        {
            get { return _codErro; }
            set { _codErro = value; }
        }

        [DataMember]
        public String CodRetorno
        {
            get { return _codRetorno; }
            set { _codRetorno = value; }
        }
    }
}
