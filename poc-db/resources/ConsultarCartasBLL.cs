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
    /// WACA1116 - Consultar por transação - Carta.
    /// </summary>
    public class ConsultarCartasBLL : RegraDeNegocioBase
    {
        private static readonly ConsultarCartasBLL instance = new ConsultarCartasBLL();

        private ConsultarCartasBLL()
        {
        }

        public static ConsultarCartasBLL Instance
        {
            get
            {
                return instance;
            }
        }

        public List<ConsultarCartasRetornoDTO> Pesquisar(out StatusRetornoDTO statusRetornoDTO, ConsultarCartasEnvioDTO envio)
        {
            try
            {
                ConsultarCartasAG ag = new ConsultarCartasAG();

                List<ConsultarCartasRetornoDTO> result = ag.ConsultarCartas(out statusRetornoDTO, envio);

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
