using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.OutrasEntidades;
using Redecard.PN.Comum;
using Redecard.PN.OutrasEntidades.Modelo;
using Redecard.PN.OutrasEntidades.Dados;

namespace Redecard.PN.OutrasEntidades.Negocio
{
    public class NegocioWFPropostas : RegraDeNegocioBase
    {

        /// <summary>
        /// Consulta Proposta Por CNPJCPF
        /// </summary>
        /// <param name="codigoTipoPessoa">Codigo Tipo Pessoa</param>
        /// <param name="numeroCnpjCpf">Numero Cnpj Cpf</param>
        /// <param name="indicadorSequenciaProp">Indicador Sequencia Proposta</param>
        /// <returns></returns>
        public List<PropostaPorCNPJCPF> ConsultaPropostaPorCNPJCPF(Char codigoTipoPessoa, Int64 numeroCnpjCpf, Int32 indicadorSequenciaProp)
        {
            using (var log = Logger.IniciarLog("Consultar propriedades do NegocioWFProposta - Utilizado na Migração do IS"))
            {
                log.GravarLog(EventoLog.InicioNegocio);

                try
                {
                    return new DadosWFPropostas().ConsultaPropostaPorCNPJCPF(codigoTipoPessoa, numeroCnpjCpf, indicadorSequenciaProp);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, String.Concat(FONTE, ".OutrasEntidades"), ex);
                }
            }
        }
    }
}
