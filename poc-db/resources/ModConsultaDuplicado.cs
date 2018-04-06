using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Cancelamento.Modelo
{
    [DataContract]
    public class ModConsultaDuplicado
    {
        #region Atributos
        private String _cd_aut_bnd;
        private String _cd_aut_inf;
        private String _desc_tp_tec;
        private String _ind_can;
        private String _nu_mes;
        private String _nu_trx_tcc;
        private String _qtd_pca;
        private String _tp_prod;
        private String _tp_trans;
        private string _codIPCan;
        private string _codUsrCan;
        private int _numPdvCan;
        private decimal _vlCan;
        private string _numNSUCartao;
        #endregion

        #region Propriedades
        [DataMember]
        public string NumNSUCartao
        {
            get { return _numNSUCartao; }
            set { _numNSUCartao = value; }
        }

        [DataMember]
        public string CodIPCan {
            get { return _codIPCan; }
            set { _codIPCan = value; }
        }

        [DataMember]
        public string CodUsrCan
        {
            get { return _codUsrCan; }
            set { _codUsrCan = value; }
        }

        [DataMember]
        public int NumPdvCan
        {
            get { return _numPdvCan; }
            set { _numPdvCan = value; }
        }

        [DataMember]
        public decimal VlCan
        {
            get { return _vlCan; }
            set { _vlCan = value; }
        }

        [DataMember]
        public String Cd_aut_bnd
        {
            get { return _cd_aut_bnd; }
            set { _cd_aut_bnd = value; }
        }

        [DataMember]
        public String Cd_aut_inf
        {
            get { return _cd_aut_inf; }
            set { _cd_aut_inf = value; }
        }

        [DataMember]
        public String Desc_tp_tec
        {
            get { return _desc_tp_tec; }
            set { _desc_tp_tec = value; }
        }

        [DataMember]
        public String Ind_can
        {
            get { return _ind_can; }
            set { _ind_can = value; }
        }

        [DataMember]
        public String Nu_mes
        {
            get { return _nu_mes; }
            set { _nu_mes = value; }
        }

        [DataMember]
        public String Nu_trx_tcc
        {
            get { return _nu_trx_tcc; }
            set { _nu_trx_tcc = value; }
        }

        [DataMember]
        public String Qtd_pca
        {
            get { return _qtd_pca; }
            set { _qtd_pca = value; }
        }

        [DataMember]
        public String Tp_prod
        {
            get { return _tp_prod; }
            set { _tp_prod = value; }
        }

        [DataMember]
        public String Tp_trans
        {
            get { return _tp_trans; }
            set { _tp_trans = value; }
        }
        #endregion
    }
}