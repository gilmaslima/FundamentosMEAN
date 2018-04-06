using System;

namespace Redecard.PN.DadosCadastrais.Modelo
{
    /// <summary>
    /// Estrutura básica de um item de uma filial, usado para fazer a passagem
    /// de parametros dos dados para o negocio
    /// </summary>
    public class Filial
    {
        /// <summary>
        /// 
        /// </summary>
        public Int32 PontoVenda;

        /// <summary>
        /// 
        /// </summary>
        public String NomeComerc;

        /// <summary>
        /// 
        /// </summary>
        public String Categoria;

        /// <summary>
        /// 
        /// </summary>
        public String Moeda;

        /// <summary>
        /// 
        /// </summary>
        public Int32 TipoEstab;

        /// <summary>
        /// 
        /// </summary>
        public Int32 Matriz;

        /// <summary>
        /// 
        /// </summary>
        [Obsolete]
        public String Centralizador;
    }
}
