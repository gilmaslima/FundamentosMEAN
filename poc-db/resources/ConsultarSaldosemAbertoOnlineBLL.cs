using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Agentes;

namespace Redecard.PN.Extrato.Negocio.RelatorioSaldosEmAberto
{
    public class ConsultarSaldosemAbertoOnlineBLL : AbstractPesquisaComTotalizadorBLL<DadosConsultaSaldosEmAbertoDTO, BasicDTO, TotalizadorPorBandeiraSaldosEmAbertoDTO>
    {
        private static readonly ConsultarSaldosemAbertoOnlineBLL instance = new ConsultarSaldosemAbertoOnlineBLL();

        private ConsultarSaldosemAbertoOnlineBLL()
        {
        }

        public static ConsultarSaldosemAbertoOnlineBLL Instance
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
        /// <returns></returns>
        protected override RetornoPesquisaComTotalizadorDTO<BasicDTO, TotalizadorPorBandeiraSaldosEmAbertoDTO> ExecutarPesquisa(out StatusRetornoDTO statusRetornoDTO, DadosConsultaSaldosEmAbertoDTO envio)
        {

            ConsultarSaldosEmAbertoAG ag = new ConsultarSaldosEmAbertoAG();

            ConsultarSaldosEmAbertoDTO itens = ag.ConsultarSaldosemAbertoOnline(out statusRetornoDTO, envio);

            if (statusRetornoDTO.CodigoRetorno != 0)
            {
                return null;
            }

            RetornoPesquisaComTotalizadorDTO<BasicDTO, TotalizadorPorBandeiraSaldosEmAbertoDTO> result = new RetornoPesquisaComTotalizadorDTO<BasicDTO, TotalizadorPorBandeiraSaldosEmAbertoDTO>();

            result.Registros = itens.Registro;
            result.Totalizador = itens.Totais;
            result.QuantidadeTotalRegistros = result.Registros.Count;

            return result;
        }
    }
}
