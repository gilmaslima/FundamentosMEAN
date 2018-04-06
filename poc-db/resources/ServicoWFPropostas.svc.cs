using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.OutrasEntidades.Modelo;
using Redecard.PN.OutrasEntidades.Negocio;

namespace Redecard.PN.OutrasEntidades.Servicos
{
    /// <summary>
    /// Serviço para acesso ao serviço PB do módulo Outras Entidades
    /// </summary>
    public class ServicoWFPropostas : ServicoBase, IServicoWFPropostas
    {

        public List<PropostaPorCNPJCPF> ConsultaPropostaPorCNPJCPF(Char codigoTipoPessoa, Int64 numeroCnpjCpf, Int32 indicadorSequenciaProp)
        {
            using (Logger Log = Logger.IniciarLog("Consultar propriedades do ServicoWFProposta - Utilizado na Migração do IS"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {

                    List<Modelo.PropostaPorCNPJCPF> lst = new NegocioWFPropostas().ConsultaPropostaPorCNPJCPF(codigoTipoPessoa, numeroCnpjCpf, indicadorSequenciaProp);

                    List<Servicos.PropostaPorCNPJCPF> result = PreencherModelo(lst).ToList();

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Preenche modelo de dados para o padrão do projeto
        /// </summary>
        /// <param name="lstProposta">Recebe lista proposta fora do padrão</param>
        /// <returns>Retorna lista porposta no padrão</returns>
        private List<Servicos.PropostaPorCNPJCPF> PreencherModelo(List<Modelo.PropostaPorCNPJCPF> lstProposta)
        {
            List<Servicos.PropostaPorCNPJCPF> lstResult = new List<PropostaPorCNPJCPF>();

            foreach (Modelo.PropostaPorCNPJCPF objItem in lstProposta)
            {
                Servicos.PropostaPorCNPJCPF objProposta = new Servicos.PropostaPorCNPJCPF();

                objProposta.CodigoRetorno = objItem.CodigoRetorno;
                objProposta.DescricaoRetorno = objItem.DescricaoRetorno;
                objProposta.Data = objItem.Data;
                objProposta.Hora = objItem.Hora;
                objProposta.NomeRazaoSocial = objItem.NomeRazaoSocial;
                objProposta.CodigoCelula = objItem.CodigoCelula;
                objProposta.CodigoAgenciaCanalFiliacao = objItem.CodigoAgenciaCanalFiliacao;

                lstResult.Add(objProposta);
            }

            return lstResult;
        }

    }
}
