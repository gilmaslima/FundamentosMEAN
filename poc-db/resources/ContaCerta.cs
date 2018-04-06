using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Negocio
{
    /// <summary>
    /// Negócio Conta Certa
    /// </summary>
    public class ContaCerta : RegraDeNegocioBase
    {

        /// <summary>
        /// Verifica se os terminais das filiais enviadas são do tipo Conta Certa
        /// </summary>
        /// <param name="filiais">Filiais a serem verificadas</param>
        /// <returns>Filiais atualizadas</returns>
        public List<Modelo.FilialTerminais> VerificaTerminalContaCerta(List<Modelo.FilialTerminais> filiais)
        {
            try
            {
                // Instancia classe agente
                var agenciaContaCerta = new Agentes.ContaCerta();

                // Retorna listagem do objeto Modelo.Terminal preenchido
                return agenciaContaCerta.VerificaTerminalContaCerta(filiais);
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
