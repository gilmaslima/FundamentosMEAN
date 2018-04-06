using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Negocio
{
    public class ConsultarAnosBaseDirfBLL : RegraDeNegocioBase
    {
        private static readonly ConsultarAnosBaseDirfBLL instance = new ConsultarAnosBaseDirfBLL();

        private ConsultarAnosBaseDirfBLL()
        {
        }

        public static ConsultarAnosBaseDirfBLL Instance
        {
            get
            {
                return instance;
            }
        }

        public ConsultarAnosBaseDirfRetornoDTO Pesquisar(out StatusRetornoDTO statusRetornoDTO)
        {
            try
            {
                InformeIRAG ag = new InformeIRAG();

                ConsultarAnosBaseDirfRetornoDTO result = ag.ConsultarAnosBaseDirf(out statusRetornoDTO);

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
