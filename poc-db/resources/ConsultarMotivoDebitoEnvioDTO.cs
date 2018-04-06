using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA152 - Relatório de débitos e desagendamentos - Motivo débito.
    /// </summary>
    public class ConsultarMotivoDebitoEnvioDTO
    {
        public int NumeroEstabelecimento { get; set; }
        public DateTime DataPesquisa { get; set; }
        public string Timestamp { get; set; }
        public decimal NumeroDebito { get; set; }
        public string TipoPesquisa { get; set; }
        public String Versao { get; set; }
    }
}
