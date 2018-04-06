#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [14/05/2012] – [André Garcia] – [Criação]
 */
#endregion

using System;
using System.Collections.Generic;

namespace Redecard.PN.DadosCadastrais.Negocio {

    /// <summary>
    /// 
    /// </summary>
    public class MensagensLogin {

        /// <summary>
        /// Códigos de Erro e Mensagens do sistema de Login
        /// </summary>
        private static Dictionary<Int32, String> _mensagensErro = new Dictionary<Int32, String> { 
            { 390, "Usuário Não Cadastrado" },
            { 389, "Senha Temporária Incorreta" },
            { 391, "Senha Incorreta" },
            { 393, "Acesso Bloqueado (Excesso de Tentativas)" },
            { 395, "Usuário Não Cadastrado" },
            { 396, "Senha Temporária Incorreta" },
            { 398, "Acesso Bloqueado (Excesso de Tentativas)" },
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static string ConsultarMensagem(int errorCode) {
            if (_mensagensErro.ContainsKey(errorCode))
                return _mensagensErro[errorCode];
            return String.Empty;
        }
    }
}
