/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 18/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Redecard.PN.FMS.Agente.ServicoFMS;
using Redecard.PN.FMS.Comum;

namespace Redecard.PN.FMS.Agente.Helpers
{
    /// <summary>
    /// Este componente publica a classe ManipuladorExcecaoCamadaServico, e expõe métodos para manipular as exceções da camada de serviço (WCF).
    /// </summary>
    public class ManipuladorExcecaoCamadaServico
    {
        /// <summary>
        /// Este método é utilizado para tratar as exceções da camada de serviço (WCF).
        /// </summary>
        /// <param name="ex"></param>
        public static void TratarExcecao(System.Exception ex)
        {
            LogHelper.GravarErrorLog(ex);

            if (ex.GetType() == typeof(FaultException<InfrastructureException>))
            {
                throw new FMSException(TipoExcecaoServico.InfrastructureException, ex.Message);
            }
            else if (ex.GetType() == typeof(FaultException<RequiredFieldNotFoundException>))
            {
                throw new FMSComCampoException(TipoExcecaoServico.RequiredFieldNotFoundException, ex.Message, ExcecaoHelper.ObterCodigoCampoErro(ex.Message));
            }
            else if (ex.GetType() == typeof(FaultException<PaginationParameterException>))
            {
                throw new FMSException(TipoExcecaoServico.PaginationParameterException, ex.Message);
            }
            else if (ex.GetType() == typeof(FaultException<LockException>))
            {
                throw new FMSException(TipoExcecaoServico.LockException, ex.Message);
            }
            else if (ex.GetType() == typeof(FaultException<InvalidParameterException>))
            {
                throw new FMSComCampoException(TipoExcecaoServico.RequiredFieldNotFoundException, ex.Message, ExcecaoHelper.ObterCodigoCampoErro(ex.Message));
            }
            else if (ex.GetType() == typeof(FaultException<CardIssuingAgentNotFoundException>))
            {
                throw new FMSException(TipoExcecaoServico.CardIssuingAgentNotFoundException, ex.Message);
            }
            else if (ex.GetType() == typeof(FaultException<MerchantAlreadyRegisteredException>))
            {
                throw new FMSException(TipoExcecaoServico.MerchantAlreadyRegisteredException, ex.Message);
            }
            else if (ex.GetType() == typeof(FaultException<SecurityException>))
            {
                throw new FMSException(TipoExcecaoServico.SecurityException, ex.Message);
            }
            else
            {
                throw new FMSException(TipoExcecaoServico.Outros, ex.Message + " - " + ex.StackTrace);
            }
        }
    }
}
