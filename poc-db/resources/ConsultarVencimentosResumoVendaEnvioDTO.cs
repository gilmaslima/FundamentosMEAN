using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA615 - Resumo de vendas - Cartões de débito - Vencimentos.
    /// </summary>
    public class ConsultarVencimentosResumoVendaEnvioDTO
    {
        public int NumeroEstabelecimento { get; set; }
        public int NumeroResumoVenda { get; set; }
        public DateTime DataApresentacao { get; set; }
    }
}
