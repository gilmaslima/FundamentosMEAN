using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Credenciamento.Negocio;

namespace Redecard.PN.Credenciamento.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DRCepServico" in code, svc and config file together.
    public class DRCepServico : ServicoBase, IDRCepServico
    {

        /// <summary>
        /// Consulta dados do logradouro de um determinado CEP
        /// </summary>
        /// <param name="cep"></param>
        /// <param name="endereco"></param>
        /// <param name="bairro"></param>
        /// <param name="cidade"></param>
        /// <param name="uf"></param>
        /// <param name="mensagem"></param>
        /// <returns></returns>
        public Int32 BuscaLogradouro(String cep, ref String endereco, ref String bairro, ref String cidade, ref String uf)
        {
            Int32 retorno;

            using (Logger Log = Logger.IniciarLog("Serviço Consulta Logradouro"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { cep, endereco, bairro, cidade, uf });

                try
                {
                    DRCepBLL cepBLL = new DRCepBLL();
                    retorno = cepBLL.BuscaLogradouro(cep, ref endereco, ref bairro, ref cidade, ref uf);
                }
                catch (PortalRedecardException ex)
                {
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), ex.Message);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.FimServico, new { retorno, endereco, bairro, cidade, uf });
            }

            return retorno;
        }
    }
}
