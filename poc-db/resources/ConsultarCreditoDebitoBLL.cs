using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Negocio
{
    /// <summary>
    /// WACA1108 - Home - Lançamentos futuros.
    /// </summary>
    public class ConsultarCreditoDebitoBLL : RegraDeNegocioBase
    {
        private static readonly ConsultarCreditoDebitoBLL instance = new ConsultarCreditoDebitoBLL();

        private ConsultarCreditoDebitoBLL()
        {
        }

        public static ConsultarCreditoDebitoBLL Instance
        {
            get
            {
                return instance;
            }
        }

        public List<ConsultarCreditoDebitoRetornoDTO> Pesquisar(out StatusRetornoDTO statusRetornoDTO, ConsultarCreditoDebitoEnvioDTO envio)
        {
            try
            {
                ConsultarLancamentosFuturosAG ag = new ConsultarLancamentosFuturosAG();

                List<ConsultarCreditoDebitoRetornoDTO> result = ag.ConsultarCreditoDebito(out statusRetornoDTO, envio);

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
