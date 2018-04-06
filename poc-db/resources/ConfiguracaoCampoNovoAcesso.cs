/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrasEntidades
{
    /// <summary>
    /// Classe de configuração do controle, para acesso via Javascript
    /// </summary>
    [DataContract]
    public class ConfiguracaoCampoNovoAcesso
    {
        /// <summary>
        /// Quando utilizado o tipo "ConfirmacaoEmail", é clientID do controle a ser comparado
        /// </summary>
        [DataMember]
        public String ControlToCompare { get; set; }

        /// <summary>
        /// Obrigatoriedade do campo
        /// </summary>
        [DataMember]
        public Boolean Obrigatorio { get; set; }
    }
}
