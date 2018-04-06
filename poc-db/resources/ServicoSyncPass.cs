/*
© Copyright 2017 Rede S.A.
Autor : Lucas Akira Uehara
Empresa : Iteris Consultoria e Software
*/
using Rede.PN.AtendimentoDigital.Core.EntLib.Dados;
using Rede.PN.AtendimentoDigital.Dados.SyncP2Service;
using Rede.PN.AtendimentoDigital.Modelo.Core.Wcf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rede.PN.AtendimentoDigital.Dados.SyncPass
{
    public class ServicoSyncPass : IServicoSyncPass
    {
        /// <summary>
        /// Obtem a senha por meio de chamada ao serviço do SyncPass
        /// </summary>
        /// <param name="arquivoSyncPass">Nome do arquivo, sem extenção.</param>
        /// <returns>Retorna uma string contendo a senha do banco especificado pelo parâmetro <paramref name="arquivoSyncPass"/>.</returns>
        public string ObterSenha(string arquivoSyncPass)
        {
            String retornoSyncPass = String.Empty;

            using (ContextoWcf<SyncP2ServiceClient> contexto = new ContextoWcf<SyncP2ServiceClient>())
            {
                retornoSyncPass = contexto.Cliente.RetornaSenha(arquivoSyncPass);
            }

            return retornoSyncPass;
        }
    }
}
