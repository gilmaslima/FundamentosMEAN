using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Agentes;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Negocio
{
    /// <summary>
    /// WACA617 - Resumo de vendas - Cartões de débito.
    /// </summary>
    public class ConsultarPreDatadosBLL : RegraDeNegocioBase
    {
        private static readonly ConsultarPreDatadosBLL instance = new ConsultarPreDatadosBLL();

        private ConsultarPreDatadosBLL()
        {
        }

        public static ConsultarPreDatadosBLL Instance
        {
            get
            {
                return instance;
            }
        }

        public List<ConsultarPreDatadosRetornoDTO> Pesquisar(out StatusRetornoDTO statusRetornoDTO, ConsultarPreDatadosEnvioDTO envio)
        {
            try
            {
                ConsultarResumoVendasAG ag = new ConsultarResumoVendasAG();

                List<ConsultarPreDatadosRetornoDTO> result = ag.ConsultarPreDatados(out statusRetornoDTO, envio);

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
