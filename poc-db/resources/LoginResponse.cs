using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rede.PN.ApiLogin.Modelo.Response
{
    public class LoginResponse
    {
        /// <summary>
        /// Token de autenticação do login
        /// </summary>
        public String TokenLogin { get; set; }

        /// <summary>
        /// Tipo do Token de Autenticação
        /// </summary>
        public String TipoTokenLogin { get; set; }

        /// <summary>
        /// Dados do usuário da Autenticação
        /// </summary>
        public Usuario Usuario { get; set; }

        /// <summary>
        /// Informações de Estabelecimentos relacionados ao Login para retornar
        /// </summary>
        public List<Entidade> EstabelecimentosRelacionados { get; set; }

        /// <summary>
        /// Status de Retorno para resposta
        /// </summary>
        public StatusResponse StatusRetorno { get; set; }
    }
}
