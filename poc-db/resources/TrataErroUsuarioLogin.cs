using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Modelo
{
    /// <summary>
    /// Classe modelo de tratamento de erro do login
    /// </summary>
    public class TrataErroUsuarioLogin
    {
        /// <summary>
        /// Usuário Chave - Formato Login
        /// </summary>
        public String Chave { get; set; }

        /// <summary>
        /// Código de retorno
        /// </summary>
        public Int32 Codigo { get; set; }
        
    }
}
