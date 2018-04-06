using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.Modelo;
using Redecard.PN.DadosCadastrais.Dados;

namespace Redecard.PN.DadosCadastrais.Negocio
{
    /// <summary>
    /// Negócio Terminal Bancário
    /// </summary>
    public class TerminalBancario : RegraDeNegocioBase
    {
        /// <summary>
        /// Obtém Teconologia
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="dataPesquisa">Data da pesquisa no formato AAAA/MM</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de terminais </returns>
        public List<Modelo.TerminalBancario> ObterTecnologia(Int32 codigoEntidade, Int32 dataPesquisa, out Int16 codigoRetorno)
        {
            try
            {
                // Instancia classe agente
                var dadosTerminal = new Agentes.TerminalBancario();

                // Retorna listagem do objeto Modelo.Terminal preenchido
                return dadosTerminal.ObterTecnologia(codigoEntidade, dataPesquisa, out codigoRetorno);
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
