#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   :
- [26/04/2012] – [André Rentes] – [Criação]
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.Modelo;
using Redecard.PN.DadosCadastrais.Dados;

namespace Redecard.PN.DadosCadastrais.Negocio
{
    /// <summary>
    /// Classe contendo as regras negócios do tipo da Entidade
    /// </summary>
    public class TipoEntidade : RegraDeNegocioBase
    {
        /// <summary>
        /// Consultar o tipo da entidade pelo código
        /// </summary>
        /// <param name="codigo">Código do Tipo da Entidade</param>
        /// <returns>Objeto ModTipoEntidade preenchido</returns>
        public List<Modelo.TipoEntidade> Consultar(Int32 codigo, out Int32 codigoRetorno)
        {
            try
            {
                // Instancia classe de dados
                var dadosTipoEntidade = new Dados.TipoEntidade();
                
                // Retorna objeto ModGrupoEntidade preenchido
                return dadosTipoEntidade.Consultar(codigo, out codigoRetorno);
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
