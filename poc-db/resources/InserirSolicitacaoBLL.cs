using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Agentes;

namespace Redecard.PN.Extrato.Negocio.RelatorioSaldosEmAberto
{
    public class InserirSolicitacaoBLL
    {
        /// <summary>
        /// Inclui solicitação de saldos em aberto, para períodos maiores que 1(hum) ano
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public static Int16 InserirSolicitacao(out StatusRetornoDTO statusRetornoDTO, DadosConsultaSaldosEmAbertoDTO envio)
        {

            ConsultarSaldosEmAbertoAG ag = new ConsultarSaldosEmAbertoAG();

            Int16 codigoRetorno = ag.IncluirSolicitacao(out statusRetornoDTO, envio);

            return codigoRetorno;
        }
    }
}
