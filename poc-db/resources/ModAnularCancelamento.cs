/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Barbosa dos Santos]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/08/29 - Guilherme Alves Brito / Lucas Nicoletto da Cunha - Anulação Cancelamento
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Redecard.PN.Cancelamento.Modelo
{
    [DataContract]
    public class ModAnularCancelamento
    {

        #region Atributos

        private Int32 _LngCodEstab;

        private String _arrNumCno;

        private Int32 _arrCodMaquineta;

        private short _codcanalcan;

        private string _codUsuario;

        private string _codIPUser;

        private String _CodRetorno;

        private String _DesfazimentosFalhos;

        private Int32 _lngSession;

        private String _strEtapa;

        private String _strTecnolog;

        private String _numCartao;

        private String _numNsu;

        private DateTime _dataTrans;
      
        private Decimal _valorTrans;
       
        private Decimal _valorCancel;

        private String _tipoCancel;

        private String _tipoTrans;

        private bool _verificaCheck;

        #endregion

        #region Propriedades

        [DataMember]
        public Int32 LngCodEstab
        {
            get { return _LngCodEstab; }
            set { if (_LngCodEstab != value)_LngCodEstab = value; }
        }

        [DataMember]
        public String arrNumCno
        {
            get { return _arrNumCno; }
            set { if (_arrNumCno != value)_arrNumCno = value; }
        }

        [DataMember]
        public Int32 arrCodMaquineta
        {
            get { return _arrCodMaquineta; }
            set { if (_arrCodMaquineta != value)_arrCodMaquineta = value; }
        }

        [DataMember]
        public short codcanalcan
        {
            get { return _codcanalcan; }
            set { if (_codcanalcan != value)_codcanalcan = value; }
        }

        [DataMember]
        public String codUsuario
        {
            get { return _codUsuario; }
            set { if (_codUsuario != value)_codUsuario = value; }
        }

        [DataMember]
        public String codIPUser
        {
            get { return _codIPUser; }
            set { if (_codIPUser != value)_codIPUser = value; }
        }

        [DataMember]
        public String DesfazimentosFalhos
        {
            get { return _DesfazimentosFalhos; }
            set { if (_DesfazimentosFalhos != value)_DesfazimentosFalhos = value; }
        }

        [DataMember]
        public String CodRetorno
        {
            get { return _CodRetorno; }
            set { if (_CodRetorno != value)_CodRetorno = value; }
        }

        [DataMember]
        public Int32 lngSession
        {
            get { return _lngSession; }
            set { if (_lngSession != value)_lngSession = value; }
        }

        [DataMember]
        public String strEtapa
        {
            get { return _strEtapa; }
            set { if (_strEtapa != value)_strEtapa = value; }
        }

        [DataMember]
        public String strTecnolog
        {
            get { return _strTecnolog; }
            set { if (_strTecnolog != value)_strTecnolog = value; }
        }

        [DataMember]
        public String numCartao
        {
            get { return _numCartao; }
            set { if (_numCartao != value)_numCartao = value; }
        }

        [DataMember]
        public String numNsu
        {
            get { return _numNsu; }
            set { if (_numNsu != value)_numNsu = value; }
        }

        [DataMember]
        public DateTime dataTrans
        {
            get { return _dataTrans; }
            set { if (_dataTrans != value)_dataTrans = value; }
        }

        [DataMember]
        public Decimal valorTrans
        {
            get { return _valorTrans; }
            set { if (_valorTrans != value)_valorTrans = value; }
        }

        [DataMember]
        public Decimal valorCancel
        {
            get { return _valorCancel; }
            set { if (_valorCancel != value)_valorCancel = value; }
        }

        [DataMember]
        public String tipoCancel
        {
            get { return _tipoCancel; }
            set { if (_tipoCancel != value)_tipoCancel = value; }
        }

        [DataMember]
        public String tipoTrans
        {
            get { return _tipoTrans; }
            set { if (_tipoTrans != value)_tipoTrans = value; }
        }

        [DataMember]
        public Boolean verificaCheck
        {
            get { return _verificaCheck; }
            set { if (_verificaCheck != value)_verificaCheck = value; }
        }



        #endregion
    }
}
