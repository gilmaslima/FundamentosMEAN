using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Agentes;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Negocio
{
    public class ConsultarDisponibilidadeVencimentosODCBLL : RegraDeNegocioBase
    {
        private static readonly ConsultarDisponibilidadeVencimentosODCBLL instance = new ConsultarDisponibilidadeVencimentosODCBLL();

        private ConsultarDisponibilidadeVencimentosODCBLL()
        {
        }

        public static ConsultarDisponibilidadeVencimentosODCBLL Instance
        {
            get
            {
                return instance;
            }
        }

        public List<ConsultarDisponibilidadeVencimentosODCRetornoDTO> Pesquisar(out StatusRetornoDTO statusRetornoDTO, ConsultarDisponibilidadeVencimentosODCEnvioDTO envio)
        {
            try
            {
                ConsultarOrdensCreditoAG ag = new ConsultarOrdensCreditoAG();

                List<ConsultarDisponibilidadeVencimentosODCRetornoDTO> result = ag.ConsultarDisponibilidadeVencimentosODC(out statusRetornoDTO, envio);

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
