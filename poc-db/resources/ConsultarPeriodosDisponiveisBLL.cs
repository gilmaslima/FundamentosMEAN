using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Agentes;

namespace Redecard.PN.Extrato.Negocio.RelatorioSaldosEmAberto
{
    /// <summary>
    /// Classe para consulta de períodos disponíveis
    /// </summary>
    public class ConsultarPeriodosDisponiveisBLL : AbstractPesquisaSemTotalizadorBLL<DadosConsultaSaldosEmAbertoDTO, PeriodoDisponivelDTO>
    {
        private static readonly ConsultarPeriodosDisponiveisBLL instance = new ConsultarPeriodosDisponiveisBLL();

        private ConsultarPeriodosDisponiveisBLL()
        {
        }

        public static ConsultarPeriodosDisponiveisBLL Instance
        {
            get
            {
                return instance;
            }
        }


        /// <summary>
        /// Efetua a pesquisa
        /// </summary>
        /// <param name="statusRetornoDTO">Classe status retorno que retorna o código e mensagem de retorno da consulta</param>
        /// <param name="envio">Dados para a consulta</param>
        /// <returns>Retorna classe com a lista de períodos disponíveis</returns>
        protected override RetornoPesquisaSemTotalizadorDTO<PeriodoDisponivelDTO> ExecutarPesquisa(out StatusRetornoDTO statusRetornoDTO, DadosConsultaSaldosEmAbertoDTO envio)
        {

            ConsultarSaldosEmAbertoAG ag = new ConsultarSaldosEmAbertoAG();

            List<PeriodoDisponivelDTO> registros = ag.ConsultarPeriodosDisponiveis(out statusRetornoDTO, envio);

            if (statusRetornoDTO.CodigoRetorno != 0)
            {
                return null;
            }

            RetornoPesquisaSemTotalizadorDTO<PeriodoDisponivelDTO> result = new RetornoPesquisaSemTotalizadorDTO<PeriodoDisponivelDTO>();
            result.Registros = registros;
            result.QuantidadeTotalRegistros = registros.Count;

            return result;
        }
    }
}
