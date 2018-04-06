using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Comum
{ 
    /// <summary>
    /// Classe base de Negócio
    /// </summary>
    public class RegraDeNegocioBase
    {
        /// <summary>
        /// Constante com o código genérico de erro das classes de dados
        /// </summary>
        /// <value>400</value>
        public const Int32 CODIGO_ERRO = 400;

        /// <summary>
        /// Constante com o nome completo da classe
        /// </summary>
        /// <value>Redecard.PN.Negocio</value>
        public const String FONTE = "Redecard.PN.Negocio";
    }

    /// <summary>
    /// Classe base de Negócio
    /// </summary>
    public class RegraDeNegocioBase<Interface, Class> : RegraDeNegocioBase where Class : Interface
    {
        /// <summary>
        /// Obtém instância da classe
        /// </summary>
        public static Interface Instancia { get { return ClassFactory.GetInstance<Interface, Class>(); } }
    }
}
