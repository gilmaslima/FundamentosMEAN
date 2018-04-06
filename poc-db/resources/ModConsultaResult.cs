using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Redecard.PN.Cancelamento.Modelo
{
    /// <summary>
    /// Classe para armazenar o retorno da consulta de cancelamentos
    /// </summary>
    [DataContract]    
    public class ModConsultaResult
    {
        #region Atributos
        private int _codErro = 0;

        private string _descErro = string.Empty;

        private List<ModCancelamentoConsulta> _listaRetorno = new List<ModCancelamentoConsulta>();
        #endregion

        #region Propriedades
        [DataMember]
        public int CodErro {
            get { return _codErro; }
            set { _codErro = value; }
        }

        [DataMember]
        public string DescErro
        {
            get { return _descErro; }
            set { _descErro = value; }
        }

        [DataMember]
        public List<ModCancelamentoConsulta> ListaRetorno
        {
            get { return _listaRetorno; }
            set { _listaRetorno = value; }
        }
        #endregion
    }
}
