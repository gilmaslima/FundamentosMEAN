using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.Negocio
{
    public class Fornecedor : RegraDeNegocioBase
    {
        /// <summary>
        /// Exclusão de Fornecedor
        /// </summary>
        /// <param name="codigoEntidadeFornecedor">Código Entidade do Fornecedor</param>
        /// <param name="codigoGrupoEntidadeFornecedor">Código Grupo Entidade do Fornecedor</param>
        /// <returns>Código de retorno</returns>
        public Int32 Excluir(Int32 codigoGrupoEntidadeFornecedor, Int32 codigoEntidadeFornecedor)
        {
            try
            {
                Dados.Fornecedor dados = new Dados.Fornecedor();
                return dados.Excluir(codigoGrupoEntidadeFornecedor, codigoEntidadeFornecedor);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Inserção de Fornecedor.
        /// </summary>
        /// <param name="fornecedor">Fornecedor</param>
        /// <returns>Código de retorno</returns>
        public Int32 Inserir(Modelo.Fornecedor fornecedor)
        {
            try
            {
                Dados.Fornecedor dados = new Dados.Fornecedor();
                return dados.Inserir(fornecedor);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
    }
}
