using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Modelo
{
    public class DadosConsultaSaldosEmAbertoDTO
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<int> Estabelecimentos { get; set; }
        public String CodigoSolicitacao { get; set; }
    }
}