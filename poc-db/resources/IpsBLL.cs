/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.FMS.Modelo.CadastroIPs;
using System.Data.SqlClient;
using Redecard.PN.FMS.Data;

namespace Redecard.PN.FMS.Negocio
{
    /// <summary>
    /// Este componente publica a classe IpsBLL, consumida a partir da camada de serviços, e expõe métodos para consultar o webservice, via as classes de agentes, para expor os serviços de Ips autorizados para o FMS.
    /// </summary>
    public class IpsBLL
    {
        /// <summary>
        /// Este método é utilizado para inserir ips
        /// </summary>
        /// <param name="ips"></param>
        /// <param name="indicadorIpsAutorizados"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        public void InsereIps(List<IPsAutorizados> ips, bool indicadorIpsAutorizados, int numeroEmissor, int grupoEntidade)
        {
            CadastroIPDAL ipDAL = new CadastroIPDAL();

            ManutencaoRetorno retorno = ipDAL.IncluirIps(grupoEntidade, numeroEmissor, indicadorIpsAutorizados, ips);

            if (retorno.Codigo != 0)
            {
                throw new Exception(String.Format("{0} - {1}", retorno.Codigo, retorno.Mensagem));
            }
        }
        /// <summary>
        /// Este método é utilizado para buscar ips
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <returns></returns>
        public List<IPsAutorizados> BuscaIps(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            CadastroIPDAL ipDAL = new CadastroIPDAL();

            List<IPsAutorizados> ips = ipDAL.ConsultarIPs(grupoEntidade, numeroEmissor);

            return ips;
        }
    }
}
