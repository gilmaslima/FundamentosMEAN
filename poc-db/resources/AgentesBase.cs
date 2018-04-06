using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Classe base de Agentes
    /// </summary>
    public class AgentesBase
    {
        /// <summary>
        /// Constante com o código genérico de erro das classes de agentes
        /// </summary>
        /// <value>50000</value>
        public const Int32 CODIGO_ERRO = 50000;

        /// <summary>
        /// Constante com o nome completo da classe
        /// </summary>
        /// <value>Redecard.PN.Agentes</value>
        public const String FONTE = "Redecard.PN.Agentes";
    }

    /// <summary>
    /// Classe base de Agentes
    /// </summary>
    public class AgentesBase<Interface, Class> : AgentesBase where Class : Interface
    {
        /// <summary>
        /// Obtém instância da classe
        /// </summary>
        public static Interface Instancia { get { return ClassFactory.GetInstance<Interface, Class>(); } }
    }
}
