using Redecard.PN.OutrosServicos.SharePoint.NkPlanoContasServico;
using System;
using System.Collections.Generic;

namespace Redecard.PN.OutrosServicos.SharePoint.Modelos
{
    /// <summary>
    /// Modelo para exibição dos dados do plano em tela
    /// </summary>
    [Serializable]
    public class PrecoUnicoView
    {
        /// <summary>
        /// Nome do Plano Preço Único contratado.
        /// </summary>
        public String NomePlano { get; set; }

        /// <summary>
        /// Valor da mensalidade referênte ao período.
        /// </summary>
        public String ValorMensalidade { get; set; }

        /// <summary>
        /// Valor máximo de vendas do plano contratado.
        /// </summary>
        public String ValorLimiteFaturamentoContratado { get; set; }

        /// <summary>
        /// Valor total das vendas crédito e débito do período.
        /// </summary>
        public String ValorFaturamentoApurado { get; set; }

        /// <summary>
        /// Valor de vendas acima do valor máximo contratado.
        /// </summary>
        public String ValorExcedenteContratado { get; set; }

        /// <summary>
        /// Valor do plano contratado.
        /// </summary>
        public String ValorMensalidadeCobrada { get; set; }

        /// <summary>
        /// Valor cobrado por vendas que excederam o valor máximo do plano.
        /// </summary>
        public String ValorCobradoPeloExcedente { get; set; }

        /// <summary>
        /// Data início de apuração do plano preço único.
        /// </summary>
        public String DataInicioApuracao { get; set; }

        /// <summary>
        /// Data fim da apuração do plano preço único.
        /// </summary>
        public String DataFimApuracao { get; set; }

        /// <summary>
        /// Valor total cobrado.
        /// </summary>
        public String Total { get; set; }

        /// <summary>
        /// Valor total de aluguel de terminais para o período.
        /// </summary>
        public String TotalAluguelEquipamento { get; set; }

        /// <summary>
        /// Valor Limite do Faturamento Contratado Pro-Rata
        /// </summary>
        public String ValorLimiteFaturamentoContratadoProRata { get; set; }

        /// <summary>
        /// Código do estabelecimento
        /// </summary>
        public String CodigoEntidade { get; set; }

        /// <summary>
        /// Ano/mês de referência da oferta
        /// </summary>
        public String AnoMesReferencia { get; set; }

        /// <summary>
        /// Lista com os equipamentos (tipos de terminais) e suas quantidades
        /// </summary>
        public String[] Equipamentos { get; set; }
    }
}
