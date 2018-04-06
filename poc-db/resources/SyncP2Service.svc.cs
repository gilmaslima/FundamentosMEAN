#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   :
- [24/04/2012] – [André Rentes] – [Criação]
*/
#endregion
using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Redecard.PN.SyncPass
{
    /// <summary>
    /// Classe SyncP2Service
    /// </summary>
    public class SyncP2Service : ISyncP2Service
    {

        private SyncP2.DES32 syncPass = new SyncP2.DES32();

        /// <summary>
        /// Retorna a senha.
        /// </summary>
        /// <param name="nomeArquivoSenha">Nome do arquivo</param>
        /// <returns>Senha</returns>
        public String RetornaSenha(String nomeArquivoSenha)
        {
            String senha = null;

            try
            {
                String retorno = "";

                lock (syncPass)
                {
                    retorno = syncPass.GetSenha(nomeArquivoSenha);
                }

                String[] codigoSyncPassSenha = retorno.Split(",".ToCharArray());
                String codigoSyncPass = codigoSyncPassSenha[0];

                if (!codigoSyncPassSenha[0].Equals("0"))
                    throw new Exception("Erro ao consultar senha de banco de dados do arquivo" + nomeArquivoSenha + ". Retorno SyncP2:" + codigoSyncPassSenha[0] + "/" + codigoSyncPassSenha[1]);
                else
                    senha = codigoSyncPassSenha[1];
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex, ex.Message);
            }

            return senha;
        }

    }
}