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
    /// WACA799 - Resumo de vendas - CDC.
    /// </summary>
    public class ConsultarTransacaoDebitoBLL : RegraDeNegocioBase
    {
        private static readonly ConsultarTransacaoDebitoBLL instance = new ConsultarTransacaoDebitoBLL();

        private ConsultarTransacaoDebitoBLL()
        {
        }

        public static ConsultarTransacaoDebitoBLL Instance
        {
            get
            {
                return instance;
            }
        }

        public ConsultarTransacaoDebitoRetornoDTO Pesquisar(out StatusRetornoDTO statusRetornoDTO, ConsultarTransacaoDebitoEnvioDTO envio)
        {
            try
            {
                ConsultarDebitoAG ag = new ConsultarDebitoAG();

                ConsultarTransacaoDebitoRetornoDTO result = ag.ConsultarTransacaoDebito(out statusRetornoDTO, envio);

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
