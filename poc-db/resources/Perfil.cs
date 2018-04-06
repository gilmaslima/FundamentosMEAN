using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.Modelo
{
    /// <summary>
    /// Classe que relaciona funcional com grupo entidade
    /// </summary>
    [DataContract]
    public class Perfil
    {
        /// <summary>
        /// Funcional
        /// </summary>
        [DataMember]
        public String Funcional { get; set; }

        /// <summary>
        /// Grupo Entidade
        /// </summary>
        [DataMember]
        public Int32 GrupoEntidade { get; set; }
    }
}
