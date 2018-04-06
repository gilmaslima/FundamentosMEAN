using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA799 - Resumo de vendas - CDC.
    /// </summary>
    public class ConsultarTransacaoDebitoEnvioDTO
    {
        public int NumeroEstabelecimento { get; set; }
        public int NumeroResumoVenda { get; set; }
        public DateTime DataApresentacao { get; set; }
    }
}
