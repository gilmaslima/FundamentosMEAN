using System;

namespace Redecard.PN.OutrosServicos.Modelo
{
    /// <summary>
    /// Modelo de Faixa de Consultas da Franquia.
    /// Remodelagem da estrutura ListarRegime_FILLER2 do COMTI ListarRegime
    /// </summary>
    public class FaixaConsultaFranquia
    {
        /// <summary>
        /// Faixa Inicial de Consultas
        /// </summary>
        public Decimal FaixaInicial { get; set; }

        /// <summary>
        /// Faixa Final de Consultas
        /// </summary>
        public Decimal FaixaFinal { get; set; }

        /// <summary>
        /// Valor da Consulta
        /// </summary>
        public Decimal ValorConsulta { get; set; }
    }
}
