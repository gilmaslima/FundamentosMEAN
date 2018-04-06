using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Modelo
{
    /// <summary>
    /// Classe de com dados dos Bancos
    /// </summary>
    public class Banco : Base
    {
        /// <summary>
        /// Indica se o banco possui opção de Crédito habilitada
        /// S - Sim
        /// N - Não
        /// </summary>
        public String SituacaoCredito { get; set; }

        /// <summary>
        /// Indica se o banco possui opção de Débito habilitada
        /// S - Sim
        /// N - Não
        /// </summary>
        public String SituacaoDebito { get; set; }

        /// <summary>
        /// Nome reduzido do banco
        /// </summary>
        public String NomeReduzido { get; set; }

        /// <summary>
        /// Sigla do banco
        /// </summary>
        public String Sigla { get; set; }

    }
}