using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;

namespace Rede.PN.Credenciamento.Sharepoint.Servicos
{
    public static class ServicosDR
    {
        /// <summary>
        /// Consulta cep no DR.
        /// </summary>
        /// <param name="cep">cep</param>
        /// <param name="endereco">ref endereco</param>
        /// <param name="bairro">ref bairro</param>
        /// <param name="cidade">ref cidade</param>
        /// <param name="uf">ref uf</param>
        /// <returns>Retorna código do logradouro</returns>
        public static int BuscaLogradouro(string cep, ref string endereco, ref string bairro, ref string cidade, ref string uf)
        {
            Int32 codigoRetorno = 0;
            using (Logger log = Logger.IniciarLog("Carregar Dados Credenciamento - Carregar Endereço"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    cep
                });

                using (DRCepServico.DRCepServicoClient client = new DRCepServico.DRCepServicoClient())
                {
                    codigoRetorno = client.BuscaLogradouro(cep, ref endereco, ref bairro, ref cidade, ref uf);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    endereco,
                    bairro,
                    uf
                });
            }

            return codigoRetorno;
        }
    }
}
