using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Modelo
{
    /// <summary>
    /// Classe de Dados Gerais
    /// </summary>
    public class DadosGerais
    {
        /// <summary>
        /// Razão Social da Entidade
        /// </summary>
        public String RazaoSocial { get; set; }

        /// <summary>
        /// Nome Fantasia da Entidade
        /// </summary>
        public String NomeFantasia { get; set; }

        /// <summary>
        /// CPNJ da Entidade
        /// </summary>
        public String CPNJ { get; set; }

        /// <summary>
        /// Plaqueta da Entidade
        /// </summary>
        public String Plaqueta { get; set; }

        /// <summary>
        /// Nome do Primeiro Proprietário
        /// </summary>
        public String Proprietario1 { get; set; }

        /// <summary>
        /// CPF do Primeiro Proprietário
        /// </summary>
        public String CPFProprietario1 { get; set; }

        /// <summary>
        /// Data de Nascimento do Primeiro Proprietário
        /// </summary>
        public String DataNascimentoProprietario1 { get; set; }

        /// <summary>
        /// Nome do Segundo Proprietário
        /// </summary>
        public String Proprietario2 { get; set; }

        /// <summary>
        /// CPF do Segundo Proprietário
        /// </summary>
        public String CPFProprietario2 { get; set; }

        /// <summary>
        /// Data de Nascimento do Segundo Proprietário
        /// </summary>
        public String DataNascimentoProprietario2 { get; set; }

        /// <summary>
        /// Nome do Terceiro Proprietário
        /// </summary>
        public String Proprietario3 { get; set; }

        /// <summary>
        /// CPF do Terceiro Proprietário
        /// </summary>
        public String CPFProprietario3 { get; set; }

        /// <summary>
        /// Data de Nascimento do Terceiro Proprietário
        /// </summary>
        public String DataNascimentoProprietario3 { get; set; }

        /// <summary>
        /// Ramo do Estabelecimento
        /// </summary>
        public String Ramo { get; set; }

        /// <summary>
        /// Local de Pagamento
        /// </summary>
        public String LocalPagamento { get; set; }

        /// <summary>
        /// Tipo de Comercialização da Entidade
        /// </summary>
        public String Comercializacao { get; set; }

        /// <summary>
        /// Nome do Contato
        /// </summary>
        public String Nome { get; set; }

        /// <summary>
        /// Primeiro Telefone - DDD
        /// </summary>
        public String DDD { get; set; }

        /// <summary>
        /// Primeiro Telefone - Número
        /// </summary>
        public String Telefone { get; set; }

        /// <summary>
        /// Primeiro Telefone - Ramal
        /// </summary>
        public String Ramal { get; set; }

        /// <summary>
        /// Segundo Telefone - DDD
        /// </summary>
        public String DDD2 { get; set; }

        /// <summary>
        /// Segundo Telefone - Número
        /// </summary>
        public String Telefone2 { get; set; }

        /// <summary>
        /// Segundo Ramal - DDD
        /// </summary>
        public String Ramal2 { get; set; }

        /// <summary>
        /// FAX - DDD
        /// </summary>
        public String DDDFax { get; set; }

        /// <summary>
        /// FAX - Número
        /// </summary>
        public String FAX { get; set; }

        /// <summary>
        /// Primeiro e-mail
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// E-mail comprovante [Poynt]
        /// </summary>
        public String EmailComprovante { get; set; }

        /// <summary>
        /// Site da Entidade
        /// </summary>
        public String Site { get; set; }

        /// <summary>
        /// Data da última atualização
        /// </summary>
        public String DataAtualizacao { get; set; }

    }

    /// <summary>
    /// Classe para guardar as informações gerais dos IPs Komerci
    /// </summary>
    public class URLBack
    {

        /// <summary>
        /// Lista de IPs do Komerci
        /// </summary>
        public List<String> ListaIPs { get; set; }

        /// <summary>
        /// URL de retorno
        /// </summary>
        public String URLBackKomerci { get; set; }

        /// <summary>
        /// Nome da Entidade cadastrada no EC
        /// </summary>
        public String DescricaoEntidade { get; set; }

        /// <summary>
        /// Tipo de Ace da Entidade
        /// </summary>
        public String TipoAceEntidade { get; set; }

        /// <summary>
        /// Tipo do Serviço WS da Entidade
        /// </summary>
        public String TipoServicoWS { get; set; }
    }
}
