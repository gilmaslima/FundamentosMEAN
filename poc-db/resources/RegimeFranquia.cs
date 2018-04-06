using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.OutrosServicos.Modelo
{
    /// <summary>
    /// Modelo de Franquia.
    /// Remodelagem da estrutura ListarRegime_FILLER do COMTI ListarRegime
    /// </summary>
    public class RegimeFranquia
    {
        /// <summary>
        /// Código de regime da Franquia
        /// </summary>
        public Int32 CodigoRegime { get; set; }

        /// <summary>
        /// Lista de Faixas de Valores por Consulta da Franquia
        /// </summary>
        public List<FaixaConsultaFranquia> FaixasConsultaFranquia { get; set; }

        /// <summary>
        /// Quantidade de Consultas da Franquia
        /// </summary>
        public Int16 QuantidadeConsulta { get; set; }

        /// <summary>
        /// Indicador de Rede.
        /// S = Sim. N = Não.
        /// </summary>
        public String Rede { get; set; }

        /// <summary>
        /// Valor da Franquia
        /// </summary>
        public Decimal ValorFranquia { get; set; }

        /// <summary>
        /// Valor da Consulta de Franquia
        /// </summary>
        public Decimal ValorConsulta { get; set; }
    }
}
