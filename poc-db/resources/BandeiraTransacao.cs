using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.OutrosServicos.Modelo
{
    /// <summary>
    /// Classe modelo de bandeiras de transação Corban
    /// </summary>
    public class BandeiraTransacao
    {
        /// <summary>
        /// Código da Bandeira
        /// </summary>
        public Int16 Codigo { get; set; }

        /// <summary>
        /// Descrição da Bandeira
        /// </summary>
        public String Descricao { get; set; }

        /// <summary>
        /// Valor da bandeira
        /// </summary>
        public Decimal Valor { get; set; }
    }
}
