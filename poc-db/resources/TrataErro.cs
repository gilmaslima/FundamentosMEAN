using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Redecard.PN.Data;
using Redecard.PN.Comum;

namespace Redecard.PN.Negocio
{
    /// <summary>
    /// Classe de negócio para tratamento de erros
    /// </summary>
    public class TrataErro
    {
        /// <summary>
        /// Consulta mensagem de erro
        /// </summary>
        /// <param name="fonte">Nome do serviço e do método que gerou o erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <returns>Mensagem de erro</returns>
        public Modelo.TrataErro Consultar(String fonte, Int32 codigo)
        {
            try
            {
                // Instancia classe de dados
                Data.TrataErro trataErroDados = new Data.TrataErro();

                // Executa e retorna mensagem
                return trataErroDados.Consultar(fonte, codigo);
            }
            catch(PortalRedecardException)
            {
                throw;
            }
        }

        /// <summary>
        /// Consulta mensagem de erro
        /// </summary>
        /// <param name="codigo">Código do erro</param>
        /// <returns>Mensagem de erro</returns>
        public Modelo.TrataErro Consultar(Int32 codigo)
        {
            try
            {
                // Instancia classe de dados
                Data.TrataErro trataErroDados = new Data.TrataErro();

                // Executa e retorna mensagem
                return trataErroDados.Consultar(codigo);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
        }

        /// <summary>
        /// Atualizar mensagem de erro
        /// </summary>
        /// <param name="mensagem">Objeto modelo com a nova mensagem e seu código de erro</param>
        /// <returns>0</returns>
        public Int16 AtualizarMensagem(Modelo.TrataErro mensagem)
        {
            try
            {
                Data.TrataErro trataErro = new Data.TrataErro();
                return trataErro.AtualizarMensagem(mensagem);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
        }
    }
}
