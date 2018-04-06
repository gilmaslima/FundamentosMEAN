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
using System.Web;
using Redecard.PN.FMS.Comum;
using System.ServiceModel;
using Redecard.PN.Comum;

namespace Redecard.PN.FMS.Servico.Helpers
{
    /// <summary>
    /// Este componente publica a classe ManipuladorExcecaoCamadaNegocio, que expõe métodos para manipular as exceçõo na camada de negocio.
    /// </summary>
    public class ManipuladorExcecaoCamadaNegocio
    {
        public const int CODIGO_ERRO = 600;
        public const string FONTE = "Redecard.PN.FMS.Servico";

        /// <summary>
        /// Este método é utilizado para tratar exceções.
        /// </summary>
        /// <param name="ex"></param>
        public static void TrataExcecao(System.Exception ex)
        {
            LogHelper.GravarErrorLog(ex);

            if ((ex.GetType() == typeof(Redecard.PN.FMS.Comum.FMSException)) || (ex.GetType() == typeof(Redecard.PN.FMS.Comum.FMSComCampoException  )))
            {
                Redecard.PN.FMS.Comum.FMSException e = (Redecard.PN.FMS.Comum.FMSException)ex;
                
                LogHelper.GravarErrorLog(string.Format("Código da FMSException: {0}", e.ObterCodigoOcorrencia()), ex);

                throw new FaultException<BusinessFault>(new BusinessFault(e.ObterCodigoOcorrencia(), e.Mensagem), e.GetBaseException().Message);
            }
            else
            {
                throw new FaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, FONTE), ex.GetBaseException().Message);
            }
        }
    }
}