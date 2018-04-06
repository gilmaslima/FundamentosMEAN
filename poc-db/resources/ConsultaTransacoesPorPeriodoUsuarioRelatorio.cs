using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.FMS.Sharepoint.Modelo
{
    public class ConsultaTransacoesPorPeriodoUsuarioRelatorio
    {
        public string TipoAlarme { get; set; }
        public string TipoResposta { get; set; }
        public string NumeroCartao { get; set; }
        public string DataHoraTransacao { get; set; }
        public string Valor { get; set; }
        public string Score { get; set; }
        public string Mcc { get; set; }
        public string Uf { get; set; }
        public string TipoCartao { get; set; }
        public string Bandeira { get; set; }
        public string Usuario { get; set; }
        public string DataHoraAnalise { get; set; }
        public string Comentario { get; set; }
    }
}
