using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Credenciamento.Modelo
{
    /// <summary>
    /// Classe de Sócios
    /// </summary>
    public class Socio
    {
        /// <summary>
        /// Nome do Sócio
        /// </summary>
        public String Nome { get; set; }

        /// <summary>
        /// CPF/CNPJ do Sócio
        /// </summary>
        public String CPF_CNPJ { get; set; }

        /// <summary>
        /// Particípação do Sócio
        /// </summary>
        public String Participacao { get; set; }

        /// <summary>
        /// Tipo de Pessoa
        /// </summary>
        public String TipoPessoa { get; set; }
    }
}
