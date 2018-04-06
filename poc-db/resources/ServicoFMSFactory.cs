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
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Net.Security;
using Redecard.PN.FMS.Comum;
using Redecard.PN.FMS.Comum.Log;
using Redecard.PN.FMS.Agente.ServicoFMS;

namespace Redecard.PN.FMS.Agente
{
    /// <summary>
    /// Este componente publica a classe ServicoFMSFactory, que expõe métodos para a factory do serviço FMS.
    /// </summary>
    public static class ServicoFMSFactory
    {
        /// <summary>
        /// Este método é utilizado para obter a instancia do agente correto
        /// </summary>
        /// <returns></returns>
        public static IServicosFMS RetornaClient()
        {
#if DEBUG
            return new ServicosFMSAgStub();
#else
            return new ServicosFMSAg();
#endif
        }
        
        private static string FMSUsuarioWebService = "FMS_WebService_Usuario";
        private static string FMSSenhaWebService = "FMS_WebService_Senha";
        private static string FMSArquivoCertificadoCliente = "FMS_WebService_ArqCertificado";
        /// <summary>
        /// Este método é utilizado para obter a instancia.
        /// </summary>
        /// <returns></returns>
        public static ServicoFMS.CardIssuingAgentFacadeClient ObterInstancia()
        {

            CardIssuingAgentFacadeClient clienteFMS = new CardIssuingAgentFacadeClient();

            try
            {

                string usuarioAutenticacao = (string)ConfiguracaoHelper.ObterValorConfiguracao(FMSUsuarioWebService);
                string senhaAutenticacao = (string)ConfiguracaoHelper.ObterValorConfiguracao(FMSSenhaWebService);
                string arquivoCertificado = (string)ConfiguracaoHelper.ObterValorConfiguracao(FMSArquivoCertificadoCliente);

                if (!string.IsNullOrEmpty(usuarioAutenticacao))
                {
                    clienteFMS.ClientCredentials.UserName.UserName = usuarioAutenticacao;
                    clienteFMS.ClientCredentials.UserName.Password = senhaAutenticacao;
                }
                if (!string.IsNullOrEmpty(arquivoCertificado))
                {
                    clienteFMS.ClientCredentials.ClientCertificate.Certificate = new X509Certificate2(arquivoCertificado);
                    SetCertificatePolicy();
                }


            }
            catch (Exception ex)
            {
                LogHelper.GravarErrorLog(ex);
                throw new FMSException(TipoExcecaoServico.Outros, ex.Message);
            }

            return clienteFMS;
        }
        /// <summary>
        /// Este método é utilizado para definir o certificado.
        /// </summary>
        private static void SetCertificatePolicy()
        {
            ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;
        }
        /// <summary>
        /// Este método é utilizado para validar o certificado remoto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cert"></param>
        /// <param name="chain"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static bool RemoteCertificateValidate(object sender,
            X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            return true;
        }


    }
}
