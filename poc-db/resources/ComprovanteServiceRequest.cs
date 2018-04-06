using Redecard.PN.Comum;
using System;

namespace Redecard.PN.Request.SharePoint.Model
{
    /// <summary>
    /// Modelo com os dados de requisição aos serviços de comprovantes
    /// </summary>
    public class ComprovanteServiceRequest
    {
        #region Dados técnicos

        /// <summary>
        /// Identificador da pesquisa para persistência em cache
        /// </summary>
        public Guid IdPesquisa { get; set; }

        /// <summary>
        /// Índice do registro inicial (usado pelo componente Comum.Paginacao)
        /// </summary>
        public Int32 RegistroInicial { get; set; }

        /// <summary>
        /// Quantidade de registros máxima por página
        /// </summary>
        public Int32 QuantidadeRegistros { get; set; }

        /// <summary>
        /// Quantidade de registros total por buffer
        /// </summary>
        public Int32 QuantidadeRegistroBuffer { get; set; }

        /// <summary>
        /// Parâmetros repassados pelo componente Comum.Paginacao
        /// </summary>
        public Object[] Parametros { get; set; }

        /// <summary>
        /// Objeto com os dados da sessão do usuário
        /// </summary>
        public Sessao SessaoUsuario { get; set; }

        #endregion

        #region Dados de filtro

        /// <summary>
        /// Identificador do tipo de comprovante (histórico/pendente/todos)
        /// </summary>
        public StatusComprovante Status { get; set; }

        /// <summary>
        /// Tipo da venda (crédito/débito)
        /// </summary>
        public TipoVendaComprovante TipoVenda { get; set; }

        /// <summary>
        /// Data inicial para busca
        /// </summary>
        public DateTime? DataInicio { get; set; }

        /// <summary>
        /// Data final para busca
        /// </summary>
        public DateTime? DataFim { get; set; }

        /// <summary>
        /// Número do processo
        /// </summary>
        public Decimal? CodProcesso { get; set; }

        #endregion
    }
}
