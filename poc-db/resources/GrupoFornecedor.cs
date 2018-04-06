#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   :
- [18/06/2013] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Collections.Generic;

using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.Negocio
{
    /// <summary>
    /// Classe contendo as regras negócios do Grupo de Entidade
    /// </summary>
    public class GrupoFornecedor : RegraDeNegocioBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigoGrupo"></param>
        /// <param name="descricao"></param>
        /// <param name="funcional"></param>
        /// <returns></returns>
        public Int32 InserirGrupoFornecedor(Int32 codigoGrupo, String descricao, String funcional)
        {
            try
            {
                Dados.GrupoFornecedor dados = new Dados.GrupoFornecedor();
                return dados.InserirGrupoFornecedor(codigoGrupo, descricao, funcional);
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
        /// 
        /// </summary>
        /// <param name="codigoGrupo"></param>
        /// <param name="descricao"></param>
        /// <param name="funcional"></param>
        /// <returns></returns>
        public Int32 AlterarGrupoFornecedor(Int32 codigoGrupo, String descricao, String funcional)
        {
            try
            {
                Dados.GrupoFornecedor dados = new Dados.GrupoFornecedor();
                return dados.AlterarGrupoFornecedor(codigoGrupo, descricao, funcional);
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
        /// 
        /// </summary>
        /// <param name="codigoGrupo"></param>
        /// <returns></returns>
        public Int32 RemoverGrupoFornecedor(Int32 codigoGrupo)
        {
            try
            {
                Dados.GrupoFornecedor dados = new Dados.GrupoFornecedor();
                return dados.RemoverGrupoFornecedor(codigoGrupo);
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
        /// Consultar grupos de fornecedores do Portal Redecard
        /// </summary>
        /// <param name="codigoRetorno"></param>
        /// <param name="codigoGrupoFornecedor"></param>
        /// <returns></returns>
        public List<Modelo.GrupoFornecedor> Consultar(out Int32 codigoRetorno, Int32? codigoGrupoFornecedor)
        {
            try
            {
                Dados.GrupoFornecedor dados = new Dados.GrupoFornecedor();
                List<Modelo.GrupoFornecedor> lista = dados.Consultar(out codigoRetorno, codigoGrupoFornecedor);
                lista.Sort();
                return lista;
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
        /// Consultar Grupo Fornecedor x Ramo de Atividade
        /// </summary>
        /// <param name="codGrupoFornecedor">Código do Grupo de Fornecedor</param>
        /// <param name="codigoRetorno">Código de Retorno</param>
        /// <returns>Associações de Grupo Fornecedor x Ramos de Atividade</returns>
        public List<Modelo.GrupoFornecedorRamoAtividade> ConsultarGrupoFornecedorRamoAtividade(Int32 codGrupoFornecedor, out Int32 codigoRetorno)
        {
            try
            {
                Dados.GrupoFornecedor dados = new Dados.GrupoFornecedor();
                List<Modelo.GrupoFornecedorRamoAtividade> lista = dados.ConsultarGrupoFornecedorRamoAtividade(codGrupoFornecedor, out codigoRetorno);                
                return lista;
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
        /// Exclusão de associação Grupo Fornecedor x Ramo de Atividade
        /// </summary>
        /// <param name="codGrupoFornecedor">Código do grupo fornecedor</param>
        /// <param name="codGrupoRamoAtividade">Código do grupo ramo de atividade</param>
        /// <param name="codRamoAtividade">Código do ramo de atividade</param>
        /// <param name="localFlag">Flag de exclusão local/dual</param>
        /// <returns>Código de retorno.</returns>
        public Int32 ExcluirGrupoFornecedorRamoAtividade(
            Int32 codGrupoFornecedor, Int32? codGrupoRamoAtividade, Int32? codRamoAtividade, Int32? localFlag)
        {
            try
            {
                Dados.GrupoFornecedor dados = new Dados.GrupoFornecedor();
                return dados.ExcluirGrupoFornecedorRamoAtividade(codGrupoFornecedor, codGrupoRamoAtividade, codRamoAtividade, localFlag);
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
        /// Inserção de associação Grupo Fornecedor x Ramo de Atividade
        /// </summary>
        ///<param name="grupoFornecedorXRamo">Dados da associação Grupo Fornecedor x Ramo de Atividade</param>
        /// <param name="localFlag">Flag de exclusão local/dual</param>
        /// <returns>Código de retorno.</returns>
        public Int32 InserirGrupoFornecedorRamoAtividade(Modelo.GrupoFornecedorRamoAtividade grupoFornecedorXRamo, Int32? localFlag)
        {
            try
            {
                Dados.GrupoFornecedor dados = new Dados.GrupoFornecedor();
                return dados.InserirGrupoFornecedorRamoAtividade(grupoFornecedorXRamo, localFlag);
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