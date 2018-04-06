using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Redecard.PN.Comum
{
    public enum EventoLog
    {
        [Description("Início do serviço '{0}'")]
        InicioServico,
        [Description("Fim do serviço '{0}'")]
        FimServico,
        [Description("{0}: Chamando serviço")]
        ChamadaServico,
        [Description("{0}: Retorno do serviço")]
        RetornoServico,

        [Description("Início do agente '{0}'")]
        InicioAgente,
        [Description("Fim do agente '{0}'")]
        FimAgente,
        [Description("{0}: Chamando agente")]
        ChamadaAgente,
        [Description("{0}: Retorno do agente")]
        RetornoAgente,

        [Description("{0}: Chamando serviço HIS")]
        ChamadaHIS,
        [Description("{0}: Retorno do serviço HIS")]
        RetornoHIS,

        [Description("Início do dados '{0}'")]
        InicioDados,
        [Description("Fim do dados '{0}'")]
        FimDados,
        [Description("{0}: Chamando dados")]
        ChamadaDados,
        [Description("{0}: Retorno dados")]
        RetornoDados,

        [Description("Início do negócio '{0}'")]
        InicioNegocio,
        [Description("Fim do negócio '{0}'")]
        FimNegocio,
        [Description("{0}: Chamando negócio")]
        ChamadaNegocio,
        [Description("{0}: Retorno negócio")]
        RetornoNegocio
    }
}
