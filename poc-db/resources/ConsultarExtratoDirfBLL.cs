using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Agentes;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Negocio
{
    public class ConsultarExtratoDirfBLL : RegraDeNegocioBase
    {
        private static readonly ConsultarExtratoDirfBLL instance = new ConsultarExtratoDirfBLL();

        private ConsultarExtratoDirfBLL()
        {
        }

        public static ConsultarExtratoDirfBLL Instance
        {
            get
            {
                return instance;
            }
        }

        public ConsultarExtratoDirfRetornoDTO Pesquisar(out StatusRetornoDTO statusRetornoDTO, ConsultarExtratoDirfEnvioDTO envio)
        {
            try
            {
                InformeIRAG ag = new InformeIRAG();

                ConsultarExtratoDirfRetornoDTO result = ag.ConsultarExtratoDirf(out statusRetornoDTO, envio);

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
