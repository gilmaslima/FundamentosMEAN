using System;

namespace Redecard.PN.DadosCadastrais.Modelo.InformacaoComercial
{
    /// <summary>
    /// Define os valores de resposta para o serviço 
    /// de Terminal Contratado.
    /// </summary>
    public class TerminalContratado
    {
        /// <summary>
        /// Define um Tipo de Equipamento.
        /// </summary>
        public String TipoEquipamento { get; set; }

        /// <summary>
        /// Define uma Quantidade de Equipamento.
        /// </summary>
        public Decimal QuantidadeEquipamento { get; set; }

        /// <summary>
        /// Define o Valor de Equipamento.
        /// </summary>
        public Decimal ValorEquipamento { get; set; }
    }
}