using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DataCash.Modelo
{
    [Serializable]
    public class DadosCliente
    {
        /// <summary>
        /// Título
        /// </summary>
        public String Titulo { get; set; }

        /// <summary>
        /// Nome
        /// </summary>
        public String Nome { get; set; }

        /// <summary>
        /// Sobrenome
        /// </summary>
        public String Sobrenome { get; set; }

        /// <summary>
        /// E-mail
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// DDD
        /// </summary>
        public String DDD { get; set; }

        /// <summary>
        /// Telefone
        /// </summary>
        public String Telefone { get; set; }

    }
}
