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

using Redecard.PN.DadosCadastrais.Modelo;
using Redecard.PN.DadosCadastrais.Dados;
using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.Negocio
{
    /// <summary>
    /// Classe contendo as regras negócios do Grupo de Entidade
    /// </summary>
    public class GrupoEntidade : RegraDeNegocioBase
    {
        /// <summary>
        /// Consultar um grupo de entidade pelo código
        /// </summary>
        /// <param name="codigo">Código do Grupo de Entidade</param>
        /// <returns>Objeto ModGrupoEntidade preenchido</returns>
        public List<Modelo.GrupoEntidade> Consultar(out Int32 codigoRetorno)
        {
            try
            {
                // Instancia classe de dados
                Dados.GrupoEntidade dadosGrupoEntidade = new Dados.GrupoEntidade();

                // Retorna objeto ModGrupoEntidade preenchido
                return dadosGrupoEntidade.Consultar(out codigoRetorno);
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
        /// Consultar um grupo de entidade pelo código
        /// </summary>
        /// <param name="codigo">Código do Grupo de Entidade</param>
        /// <returns>Objeto ModGrupoEntidade preenchido</returns>
        public List<Modelo.GrupoEntidade> Consultar(Int32? codigo, out Int32 codigoRetorno)
        {
            try
            {
                // Instancia classe de dados
                Dados.GrupoEntidade dadosGrupoEntidade = new Dados.GrupoEntidade();

                if (codigo == -1)
                    return dadosGrupoEntidade.Consultar(out codigoRetorno);
                else
                    return dadosGrupoEntidade.Consultar(codigo, out codigoRetorno);
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
        /// Altera um Grupo de Entidade
        /// </summary>
        /// <param name="grupoEntidade">Objeto Grupo de Entidade atualizado</param>
        /// <returns>Retorna identificador caso
        /// 0 - Altulização efetuada com sucesso
        /// 2 - Grupo não existe
        /// 99 - Erro de sistema não previsto
        /// </returns>
        public Int32 Atualizar(Modelo.GrupoEntidade grupoEntidade)
        {
            try
            {
                // Instancia classe de dados
                var GrupoEntidadeDados = new Dados.GrupoEntidade();

                // Retorna indicador da operação realizada
                return GrupoEntidadeDados.Atualizar(grupoEntidade);
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
        /// Inserir um Grupo de Entidade
        /// </summary>
        /// <param name="grupoEntidade">Objeto Grupo de Entidade</param>
        /// <returns>Retorna identificador caso
        /// 0 - Altulização efetuada com sucesso
        /// 1 - Grupo já existe
        /// 99 - Erro de sistema não previsto
        /// </returns>
        public Int32 Inserir(Modelo.GrupoEntidade grupoEntidade)
        {
            try
            {
                // Instancia classe de dados
                var GrupoEntidadeDados = new Dados.GrupoEntidade();

                // Retorna indicador da operação realizada
                return GrupoEntidadeDados.Inserir(grupoEntidade);
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
        /// Altera o Status de um Grupo de Entidade
        /// </summary>
        /// <param name="grupoEntidade">Objeto Grupo de Entidade atualizado</param>
        /// <returns>Retorna identificador caso
        /// 0 - Altulização efetuada com sucesso
        /// 2 - Grupo não existe
        /// 99 - Erro de sistema não previsto
        /// </returns>
        public void AtualizarStatus(Modelo.GrupoEntidade grupoEntidade)
        {
            try
            {
                // Instancia classe de dados
                var GrupoEntidadeDados = new Dados.GrupoEntidade();

                // Retorna indicador da operação realizada
                GrupoEntidadeDados.AtualizarStatus(grupoEntidade);
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
        /// Exclui o grupo de entidade
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public Int32 Excluir(Int32 codigo)
        {
            try
            {
                // Instancia classe de dados
                var GrupoEntidadeDados = new Dados.GrupoEntidade();

                // Retorna indicador da operação realizada
                return GrupoEntidadeDados.Excluir(codigo);
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
        /// Testa transações
        /// </summary>
        /// <param name="grupoEntidade">Objeto Grupo de Entidade</param>
        public void TestarTransacao(Modelo.GrupoEntidade grupoEntidade)
        {
            try
            {
                // Instancia classe de dados
                var GrupoEntidadeDados = new Dados.GrupoEntidade();

                // Retorna indicador da operação realizada
                GrupoEntidadeDados.TestarTransacao(grupoEntidade);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
