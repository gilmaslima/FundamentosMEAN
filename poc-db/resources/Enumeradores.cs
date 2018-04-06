using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.Eadquirencia.Sharepoint.Helper
{
    /// <summary>
    /// Código do usuário que iniciou o processo de Homologação
    /// </summary>
    public enum SolicitanteHomologacao
    {
        /// <summary>
        /// Código quando o processo de homologação é iniciado pelo sistema (Exemplo: Serviço Re-homologação periódica)
        /// </summary>
         Sistema                = 0,

        /// <summary>
         /// Código quando o processo de homologação é iniciado pelo Estabelecimento através do PN
        /// </summary>
        Estabelecimento        = -1
    }

    /// <summary>
    /// Tipos possíveis de configuração da web part de redirecionamento
    /// </summary>
    public enum TipoRedirecionamento
    {
        /// <summary>
        /// Tipo default de redirecionamento
        /// </summary>
        Default                 =   0,

        /// <summary>
        /// Faça sua venda
        /// </summary>
        FacaSuaVenda            =   1,
    }
}
