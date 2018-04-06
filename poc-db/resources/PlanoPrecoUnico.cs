/*
© Copyright 2014 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.OutrosServicos.Modelo.PlanoContas
{
    /// <summary>
    /// Turquia - Plano Preço Único
    /// </summary>
    public class PlanoPrecoUnico
    {
        /// <summary>
        /// Nome do Plano Preço Único contratado.
        /// </summary>
        public String NomePlano { get; set; }

        /// <summary>
        /// Descrição da oferta contratada.
        /// </summary>
        public String DescricaoOferta { get; set; }

        /// <summary>
        /// Valor da mensalidade referênte ao período.
        /// </summary>
        public Decimal ValorMensalidade { get; set; }

        /// <summary>
        /// Valor máximo de vendas do plano contratado.
        /// </summary>
        public Decimal ValorLimiteFaturamentoContratado { get; set; }

        /// <summary>
        /// Quantidade de Tecnologias Cadastradas no pacote.
        /// </summary>
        public short QtdTecnologiasCadastradasPacote { get; set; }

        /// <summary>
        /// Terminais cadastrados para o pacote.
        /// </summary>
        public List<Equipamento> Equipamentos { get; set; }

        /// <summary>
        /// Valor total das vendas crédito e débito do período.
        /// </summary>
        public Decimal ValorFaturamentoApurado { get; set; }

        /// <summary>
        /// Valor de vendas acima do valor máximo contratado.
        /// </summary>
        public Decimal ValorExcedenteContratado { get; set; }

        /// <summary>
        /// Valor do plano contratado.
        /// </summary>
        public Decimal ValorMensalidadeCobrada { get; set; }

        /// <summary>
        /// Valor cobrado por vendas que excederam o valor máximo do plano.
        /// </summary>
        public Decimal ValorCabradoPeloExcedente { get; set; }

        /// <summary>
        /// Data início de apuração do plano.
        /// </summary>
        public DateTime? DataInicioApuracao { get; set; }

        /// <summary>
        /// Data fim da apuração do plano.
        /// </summary>
        public DateTime? DataFimApuracao { get; set; }

        /// <summary>
        /// Ano e mês de referência da apuração.
        /// </summary>
        public DateTime AnoMesReferencia { get; set; }

        /// <summary>
        /// Valor total cobrado.
        /// </summary>
        public Decimal Total { get; set; }

        /// <summary>
        /// Valor total de aluguel de terminais para o período.
        /// </summary>
        public Decimal TotalAluguelEquipamento { get; set; }

        /// <summary>
        /// Indicador Flex - indica se houve condição de pagamento diferenciado contratado.
        /// </summary>
        public String IndicadorFlex { get; set; }

        /// <summary>
        /// Valor Limite do Faturamento Contratado Pro-Rata
        /// </summary>
        public Decimal ValorLimiteFaturamentoContratadoProRata { get; set; }
    }
}
