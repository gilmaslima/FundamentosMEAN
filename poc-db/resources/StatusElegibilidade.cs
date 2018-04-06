using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.OutrosServicos.Modelo.PlanoContas
{
    public enum StatusElegibilidade : short
    {
        /// <summary>Não identificado</summary>
        NAO_IDENTIFICADO = 0,

        /// <summary>Atingiu os critérios técnicos</summary>
        ATINGIU_OS_CRITERIOS_TECNICOS = 90,

        /// <summary>Não atingiu critério e concentração de vendas</summary>
        NAO_ATINGIU_CRITERIO_CONCENTRACAO_DE_VENDAS = 91,

        /// <summary>Não atingiu trava e concentração de vendas</summary>
        NAO_ATINGIU_TRAVA_E_CONCENTRACAO_DE_VENDAS = 92,

        /// <summary>Não possui trava</summary>
        NAO_POSSUI_TRAVA = 93,

        /// <summary>Não atingiu faturamento necessário</summary>
        NAO_ATINGIU_O_FATURAMENTO_NECESSARIO = 94,

        /// <summary>Não atingiu faturamento e concentração de vendas</summary>
        NAO_ATINGIU_FATURAMENTO_E_CONCENTRACAO_DE_VENDAS = 95,

        /// <summary>Não atingiu faturamento e trava</summary>
        NAO_ATINGIU_FATURAMENTO_E_TRAVA = 96,

        /// <summary>Não atingiu faturamento, trava e concentração de vendas</summary>
        NAO_ATINGIU_FATURAMENTO_TRAVA_E_CONCENTRACAO_VENDAS = 97     
    }
}
