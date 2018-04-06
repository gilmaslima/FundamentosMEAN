using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Modelo
{
    /// <summary>
    /// Classe de hash para envio de e-mail de um usuário
    /// </summary>
    public class UsuarioHash
    {
        /// <summary>
        /// Código Id do Usuário
        /// </summary>
        public Int32 CodigoIdUsuario { get; set; }

        /// <summary>
        /// Status do usuário - Novo Acesso
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Hash para envio de e-mail
        /// </summary>
        public Guid Hash { get; set; }

        /// <summary>
        /// Data de geração do Hash
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Data de expiraçao do Hash
        /// </summary>
        public DateTime? DataExpiracao { get; set; }
    }
}
