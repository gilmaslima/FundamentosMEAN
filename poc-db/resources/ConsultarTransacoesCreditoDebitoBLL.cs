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
    /// WACA1107 - Home - Últimas Vendas.
    /// </summary>
    public class ConsultarTransacoesCreditoDebitoBLL : RegraDeNegocioBase
    {
        private static readonly ConsultarTransacoesCreditoDebitoBLL instance = new ConsultarTransacoesCreditoDebitoBLL();

        private ConsultarTransacoesCreditoDebitoBLL()
        {
        }

        public static ConsultarTransacoesCreditoDebitoBLL Instance
        {
            get
            {
                return instance;
            }
        }

        public List<ConsultarTransacoesCreditoDebitoRetornoDTO> Pesquisar(out StatusRetornoDTO statusRetornoDTO, ConsultarTransacoesCreditoDebitoEnvioDTO envio)
        {
            try
            {
                ConsultaHomepageAG ag = new ConsultaHomepageAG();

                List<ConsultarTransacoesCreditoDebitoRetornoDTO> result = ag.ConsultarTransacoesCreditoDebito(out statusRetornoDTO, envio);

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
