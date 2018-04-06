using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Agentes;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Negocio
{
    public class ConsultarRejeitadosBLL : RegraDeNegocioBase
    {
        private static readonly ConsultarRejeitadosBLL instance = new ConsultarRejeitadosBLL();

        private ConsultarRejeitadosBLL()
        {
        }

        public static ConsultarRejeitadosBLL Instance
        {
            get
            {
                return instance;
            }
        }

        public List<ConsultarRejeitadosRetornoDTO> Pesquisar(out StatusRetornoDTO statusRetornoDTO, ConsultarRejeitadosEnvioDTO envio)
        {
            try
            {
                ConsultarResumoVendasAG ag = new ConsultarResumoVendasAG();

                List<ConsultarRejeitadosRetornoDTO> result = ag.ConsultarRejeitados(out statusRetornoDTO, envio);

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
