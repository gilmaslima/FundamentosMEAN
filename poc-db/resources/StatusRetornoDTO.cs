using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class StatusRetornoDTO
    {
        public short CodigoRetorno { get; private set; }
        public string MensagemRetorno { get; private set; }
        public string Fonte { get; private set; }

        public StatusRetornoDTO(short codigoRetorno, string mensagemRetorno, string fonte)
        {
            this.CodigoRetorno = codigoRetorno;
            this.MensagemRetorno = mensagemRetorno;
            this.Fonte = fonte;
        }
    }
}
