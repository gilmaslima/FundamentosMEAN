using Microsoft.Practices.EnterpriseLibrary.Data;
using Redecard.PN.Comum;
using System;
using System.Data;

namespace Rede.PN.HealthCheck.DAL
{
    public class SistemaDAL : BancoDeDadosBase
    {
        /// <summary>
        /// Verificar status do GE
        /// </summary>
        /// <returns></returns>
        public Boolean StatusGE()
        {
            Boolean statusOk = default(Boolean);

            using (Logger log = Logger.IniciarLog("Verificar status do GE"))
            {
                try
                {
                    GenericDatabase db = base.SybaseGE();
                    using (var con = db.CreateConnection())
                    {
                        con.Open();
                        statusOk = true;
                    }
                }
                catch (DBConcurrencyException ex)
                {
                    log.GravarErro(ex);
                    statusOk = false;

                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    statusOk = false;

                    throw ex;
                }
            }
            return statusOk;
        }

        /// <summary>
        /// Verificar status do TG
        /// </summary>
        /// <returns></returns>
        public Boolean StatusTG()
        {
            Boolean statusOk = default(Boolean);

            using (Logger log = Logger.IniciarLog("Verificar status do TG"))
            {
                try
                {
                    GenericDatabase db = base.SybaseTG();
                    using (var con = db.CreateConnection())
                    {
                        con.Open();
                        statusOk = true;
                    }
                }
                catch (DBConcurrencyException ex)
                {
                    log.GravarErro(ex);
                    statusOk = false;

                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    statusOk = false;
                    
                    throw ex;
                }
            }
            return statusOk;
        }
    }
}