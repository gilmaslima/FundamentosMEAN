using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Agentes;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Negocio
{
    public class ConsultarConsolidadoDebitosEDesagendamentoBLL : RegraDeNegocioBase
    {
        private static readonly ConsultarConsolidadoDebitosEDesagendamentoBLL instance = new ConsultarConsolidadoDebitosEDesagendamentoBLL();

        private ConsultarConsolidadoDebitosEDesagendamentoBLL()
        {
        }

        public static ConsultarConsolidadoDebitosEDesagendamentoBLL Instance
        {
            get
            {
                return instance;
            }
        }

        public ConsultarConsolidadoDebitosEDesagendamentoRetornoDTO Pesquisar(out StatusRetornoDTO statusRetornoDTO, ConsultarConsolidadoDebitosEDesagendamentoEnvioDTO envio)
        {
            try
            {
                ConsultarExtratosAG ag = new ConsultarExtratosAG();
                ConsultarConsolidadoDebitosEDesagendamentoRetornoDTO result = null;

                //Verifica qual versão de programa no mainframe deve ser chamada
                if ("ISF".CompareTo(envio.Versao) == 0)
                    result = ag.ConsultarConsolidadoDebitosEDesagendamento(out statusRetornoDTO, envio);
                else
                    result = ag.ConsultarConsolidadoDebitosEDesagendamentoISD1(out statusRetornoDTO, envio);

                if (statusRetornoDTO.CodigoRetorno != 0)
                {
                    return null;
                }

                return result;
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
    }
}
