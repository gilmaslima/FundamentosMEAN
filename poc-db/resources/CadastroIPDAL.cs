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
using Redecard.PN.Comum;
using Redecard.PN.FMS.Modelo.CadastroIPs;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Redecard.PN.FMS.Comum;

namespace Redecard.PN.FMS.Data
{
    /// <summary>
    /// Este componente publica a classe CadastroIPDAL, é estendida de BancoDeDadosBase, e expõe métodos para manipular o cadastro de IPs.
    /// </summary>
    public class CadastroIPDAL : BancoDeDadosBase
    {
        private const string DELIMITADOR = "$";
        /// <summary>
        /// Este método é utilizado para retornar string com delimitador.
        /// </summary>
        /// <param name="ips"></param>
        /// <returns></returns>
        private string RetornarStringComDelimitador(List<IPsAutorizados> ips)
        {
            string retorno = String.Empty;

            foreach (IPsAutorizados item in ips)
            {
                if (retorno == String.Empty)
                {
                    retorno += item.NumeroIP;
                }
                else
                {
                    retorno += DELIMITADOR + item.NumeroIP;
                }
            }

            return retorno;
        }
        /// <summary>
        /// Este método é utilizado para  incluir ips.
        /// </summary>
        /// <param name="codGruEtd"></param>
        /// <param name="codEtd"></param>
        /// <param name="ehPassivelValidacao"></param>
        /// <param name="ips"></param>
        /// <returns></returns>
        public ManutencaoRetorno IncluirIps(int codGruEtd, int codEtd, bool ehPassivelValidacao, List<IPsAutorizados> ips)
        {
            GenericDatabase db = base.SybaseIS();

            LogHelper.GravarLogIntegracao("[@@@SERVIÇO - LISTA IP AUTORIZADO]", ips);

            using (DbCommand comando = db.GetStoredProcCommand("sp_ins_ip_emissor"))
            {
                db.AddInParameter(comando, "@cod_gru_etd", DbType.Int32, codGruEtd);
                db.AddInParameter(comando, "@cod_etd", DbType.Int32, codEtd);
                db.AddInParameter(comando, "@ind_aces_rstt", DbType.AnsiString, ehPassivelValidacao ? "S" : "N");
                db.AddInParameter(comando, "@ips", DbType.AnsiString, RetornarStringComDelimitador(ips));
                db.AddOutParameter(comando, "@cod_ret", DbType.Int32, 2);
                db.AddOutParameter(comando, "@msg_ret", DbType.AnsiString, 50);

                db.ExecuteNonQuery(comando);

                int codRet = Convert.ToInt32(db.GetParameterValue(comando, "@cod_ret"));
                string msgRet = Convert.ToString(db.GetParameterValue(comando, "@msg_ret"));

                return new ManutencaoRetorno() { Codigo = codRet, Mensagem = msgRet };
            }
        }

        /// <summary>
        /// Consulta ips que estao habilitados para o FMS
        /// </summary>
        /// <param name="codGruEtd"></param>
        /// <param name="codEtd"></param>
        /// <returns></returns>
        public List<IPsAutorizados> ConsultarIPs(int codGruEtd, int codEtd)
        {
            List<IPsAutorizados> ips = new List<IPsAutorizados>();

            GenericDatabase db = base.SybaseIS();

            using (DbCommand comando = db.GetStoredProcCommand("sp_cons_ip_emissor"))
            {

                db.AddInParameter(comando, "@cod_gru_etd", DbType.Int32, codGruEtd);
                db.AddInParameter(comando, "@cod_etd", DbType.Int32, codEtd);
                db.AddOutParameter(comando, "@cod_ret", DbType.Int32, 2);
                db.AddOutParameter(comando, "@msg_ret", DbType.AnsiString, 50);

                try
                {
                    using (IDataReader reader = db.ExecuteReader(comando))
                    {

                        int codRet = Convert.ToInt32(db.GetParameterValue(comando, "@cod_ret"));
                        string msgRet = Convert.ToString(db.GetParameterValue(comando, "@msg_ret"));

                        if (codRet != 0)
                            throw new Exception(String.Format("{0} - {1}", codRet, msgRet));

                        while (reader.Read())
                        {
                            string ip = Convert.ToString(reader["num_ip"]);
                            bool ehPassivelValidacao = Convert.ToString(reader["ind_aces_rstt"]) == "S";

                            ips.Add(new IPsAutorizados() { NumeroIP = ip, EhPassivelValidacaoIP = ehPassivelValidacao });
                        }
                    }
                }
                catch { }
            }

            return ips;
        }
    }
}
