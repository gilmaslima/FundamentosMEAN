#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   :
- [25/04/2012] – [André Rentes] – [Criação]
*/
#endregion
using System;
using System.Collections.Generic;
using System.ServiceModel;
using AutoMapper;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.Modelo;
using System.Linq;
using System.Reflection;
using Redecard.PN.DadosCadastrais.Modelo.Mensagens;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Serviço para gerenciamento de Entdiades
    /// </summary>
    public class EntidadeServico : ServicoBase, IEntidadeServico
    {
        /// <summary>
        /// 
        /// </summary>
        private const String grupoEntidadesCache = "__key__GrupoEntidadesCache";

        /// <summary>
        /// Validar se as Entidades existem no PN, caso não existam, incluir as mesmas na base
        /// </summary>
        /// <param name="entidades">Entidades separas por ';' para validação na base do PN
        /// <para>Cada elemento do conjunto de Entidades é separado por ';'</para>
        /// <para>Cada elemento de Entidades possui o Número do PV e o Nome da Entidade separados por ','</para>
        /// </param>
        /// <returns>
        /// <para>0 - Sucesso</para>
        /// <para>404 - Entidades não informadas</para>
        /// <para>425 - Erro de excessão no processamento das Entidades (rollback de todas as entidades inseridas)</para>
        /// </returns>
        public Int32 ValidarEntidadePn(String entidades)
        {
            using (Logger log = Logger.IniciarLog("Validar se as Entidades existem no PN, caso não existam, incluir as mesmas na base"))
            {
                log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Negocio.Entidade negocio = new Negocio.Entidade();
                    return negocio.ValidarEntidadePn(entidades);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Validar se as Entidades existem no PN, caso não existam, consulta os dados no GE e inclui as mesmas na base
        /// </summary>
        /// <param name="entidades">Número das entidades separados por '|' para validação na base do PN
        /// <para>Cada elemento do conjunto de Entidades é separado por '|'</para>
        /// </param>
        /// <returns>
        /// <para>0 - Sucesso</para>
        /// <para>404 - Entidades não informadas</para>
        /// <para>410 - Erro na procedure ao atualizar os dados de uma Entidade (rollback do processamento da entidade atual)</para>
        /// <para>411 - Erro na procedure ao inserir os dados de uma Entidade (rollback do processamento da entidade atual)</para>
        /// </returns>
        public Int32 ValidarPvsExistentes(String entidades)
        {
            using (Logger log = Logger.IniciarLog("Validar se as Entidades existem no PN, caso não existam, consulta no GE e incluii as mesmas na base"))
            {
                log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Negocio.Entidade negocio = new Negocio.Entidade();
                    return negocio.ValidarPvsExistentes(entidades);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta entidade
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="codigoRetornoIS"></param>
        /// <returns></returns>
        public List<Servicos.Entidade> Consultar(Int32? codigo, out Int32 codigoRetornoIS)
        {
            Int32 codigoRetornoGE = 0;
            // Enviar o Grupo de Entidade como 0 para pesquisar informações somente no IS/PN
            return this.Consultar(codigo, 0, out codigoRetornoIS, out codigoRetornoGE);
        }

        /// <summary>
        /// Consulta entidade
        /// </summary>
        /// <param name="id">Id da entidade</param>
        /// <returns>Entidade preenchida</returns>
        public List<Servicos.Entidade> Consultar(Int32? codigo, Int32? codigoGrupoEntidade, out Int32 codigoRetornoIS, out Int32 codigoRetornoGE)
        {
            List<Modelo.Entidade> modeloEntidades = null;
            var entidades = new List<Servicos.Entidade>();
            using (Logger Log = Logger.IniciarLog("Consulta entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioEntidade = new Negocio.Entidade();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigo, codigoGrupoEntidade });

                    modeloEntidades = negocioEntidade.Consultar(codigo, codigoGrupoEntidade, out codigoRetornoIS, out codigoRetornoGE);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetornoIS, codigoRetornoGE, modeloEntidades });

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                    foreach (var modeloEntidade in modeloEntidades)
                    {
                        // Converte Business Entity para Data Contract Entity
                        entidades.Add(Mapper.Map<Modelo.Entidade, Servicos.Entidade>(modeloEntidade));
                    }
                    Log.GravarLog(EventoLog.FimServico, new { entidades });

                    return entidades;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigoGrupo"></param>
        /// <param name="descricao"></param>
        /// <param name="funcional"></param>
        /// <returns></returns>
        public Int32 InserirGrupoFornecedor(Int32 codigoGrupo, String descricao, String funcional)
        {
            using (Logger Log = Logger.IniciarLog("Inserir um grupo fornecedor"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Negocio.GrupoFornecedor negocio = new Negocio.GrupoFornecedor();
                    return negocio.InserirGrupoFornecedor(codigoGrupo, descricao, funcional);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
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
            using (Logger Log = Logger.IniciarLog("Alterar grupo fornecedor"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Negocio.GrupoFornecedor negocio = new Negocio.GrupoFornecedor();
                    return negocio.AlterarGrupoFornecedor(codigoGrupo, descricao, funcional);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigoGrupo"></param>
        /// <returns></returns>
        public Int32 RemoverGrupoFornecedor(Int32 codigoGrupo)
        {
            using (Logger Log = Logger.IniciarLog("Remover grupo fornecedor"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Negocio.GrupoFornecedor negocio = new Negocio.GrupoFornecedor();
                    return negocio.RemoverGrupoFornecedor(codigoGrupo);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consultar grupos de fornecedores do Portal Redecard
        /// </summary>
        /// <param name="codigoRetorno"></param>
        /// <param name="codigoGrupoFornecedor"></param>
        /// <returns></returns>
        public List<Servicos.GrupoFornecedor> ConsultarGrupoFornecedor(out Int32 codigoRetorno, Int32? codigoGrupoFornecedor)
        {
            using (Logger Log = Logger.IniciarLog("Consulta grupo da entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                List<Servicos.GrupoFornecedor> grupos = new List<GrupoFornecedor>();

                try
                {

                    Negocio.GrupoFornecedor negocio = new Negocio.GrupoFornecedor();
                    List<Modelo.GrupoFornecedor> _grupos = negocio.Consultar(out codigoRetorno, codigoGrupoFornecedor);

                    Mapper.CreateMap<Modelo.GrupoFornecedor, Servicos.GrupoFornecedor>();
                    if (_grupos.Count > 0)
                    {
                        _grupos.ForEach(delegate(Modelo.GrupoFornecedor _grupo)
                        {
                            grupos.Add(Mapper.Map<Modelo.GrupoFornecedor, Servicos.GrupoFornecedor>(_grupo));
                        });
                    }

                    Log.GravarLog(EventoLog.FimServico, new { grupos });
                    return grupos;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta grupo da entidade
        /// </summary>
        /// <param name="id">Id do grupo da entidade</param>
        /// <returns>Grupo da Entidade preenchido</returns>
        public List<Servicos.GrupoEntidade> ConsultarGrupo(Int32? codigo, Boolean useCache, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta grupo da entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    codigoRetorno = 0;

                    List<Servicos.GrupoEntidade> grupoEntidade = new List<GrupoEntidade>();
                    List<Modelo.GrupoEntidade> modeloGrupoEntidade;

                    var negocioGrupoEntidade = new Negocio.GrupoEntidade();

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();

#if DEBUG

                    // Retorna objeto ModGrupoEntidade preenchido
                    modeloGrupoEntidade = negocioGrupoEntidade.Consultar(codigo, out codigoRetorno);

                    // Convert Business Entity para Data Contract Entity                        
                    grupoEntidade = Mapper.Map<List<Modelo.GrupoEntidade>, List<Servicos.GrupoEntidade>>(modeloGrupoEntidade);

#else
                    if (useCache)
                    {
                        if (CacheAdmin.Recuperar<List<GrupoEntidade>>(Cache.Geral, grupoEntidadesCache) == null)
                        {
                            // Retorna objeto ModGrupoEntidade preenchido
                            Log.GravarLog(EventoLog.ChamadaNegocio, new { codigo });
                            modeloGrupoEntidade = negocioGrupoEntidade.Consultar(codigo, out codigoRetorno);
                            Log.GravarLog(EventoLog.RetornoNegocio, new { modeloGrupoEntidade, codigoRetorno });

                            // Convert Business Entity para Data Contract Entity                        
                            grupoEntidade = Mapper.Map<List<Modelo.GrupoEntidade>, List<Servicos.GrupoEntidade>>(modeloGrupoEntidade);

                            // Adiciona listagem no cache
                            CacheAdmin.Adicionar<List<GrupoEntidade>>(Cache.Geral, grupoEntidadesCache, grupoEntidade);
                        }
                        else
                        {
                            // Retorna listagem do cache                        
                            grupoEntidade = CacheAdmin.Recuperar<List<GrupoEntidade>>(Cache.Geral, grupoEntidadesCache);
                            Log.GravarMensagem("Dados obtidos do Cache", new { grupoEntidade });
                        }
                    }
                    else
                    {                        
                        modeloGrupoEntidade = negocioGrupoEntidade.Consultar(codigo, out codigoRetorno);
                        grupoEntidade = Mapper.Map<List<Modelo.GrupoEntidade>, List<Servicos.GrupoEntidade>>(modeloGrupoEntidade);
                    }
#endif
                    Log.GravarLog(EventoLog.FimServico, new { grupoEntidade });

                    return grupoEntidade;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta grupo da entidade
        /// </summary>
        /// <param name="id">Id do grupo da entidade</param>
        /// <returns>Grupo da Entidade preenchido</returns>
        public List<Servicos.GrupoEntidade> ConsultarGrupo(Int32? codigo, out Int32 codigoRetorno)
        {
            return this.ConsultarGrupo(codigo, true, out codigoRetorno);
        }

        /// <summary>
        /// Consulta grupo da entidade (Mesmos inativos)
        /// </summary>
        /// <returns>Grupo da Entidade preenchido</returns>
        public List<Servicos.GrupoEntidade> ConsultarTodosGrupos(out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta grupo da entidade (Mesmos inativos)"))
            {
                Log.GravarLog(EventoLog.InicioServico);


                try
                {
                    codigoRetorno = 0;
                    List<Servicos.GrupoEntidade> grupoEntidade = new List<GrupoEntidade>();
                    List<Modelo.GrupoEntidade> modeloGrupoEntidade;
                    var negocioGrupoEntidade = new Negocio.GrupoEntidade();

                    Log.GravarLog(EventoLog.ChamadaNegocio);

                    // Retorna objeto ModGrupoEntidade preenchido
                    modeloGrupoEntidade = negocioGrupoEntidade.Consultar(out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, modeloGrupoEntidade });

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    foreach (var modeloEntidade in modeloGrupoEntidade)
                    {
                        // Converte Business Entity para Data Contract Entity
                        grupoEntidade.Add(Mapper.Map<Modelo.GrupoEntidade, Servicos.GrupoEntidade>(modeloEntidade));
                    }

                    // Convert Business Entity para Data Contract Entity
                    grupoEntidade = Mapper.Map<List<Modelo.GrupoEntidade>, List<Servicos.GrupoEntidade>>(modeloGrupoEntidade);
                    Log.GravarLog(EventoLog.FimServico, new { grupoEntidade });

                    return grupoEntidade;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        /// <summary>
        /// Altera o Status de um Grupo de Entidade
        /// </summary>
        /// <param name="grupoEntidade">Objeto Grupo de Entidade atualizado</param>
        public void AtualizarStatusGrupo(Servicos.GrupoEntidade grupoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Altera o Status de um Grupo de Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioGrupoEntidade = new Negocio.GrupoEntidade();
                    Modelo.GrupoEntidade modeloGrupoEntidade;

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                    // Convert Business Entity para Data Contract Entity
                    modeloGrupoEntidade = Mapper.Map<Servicos.GrupoEntidade, Modelo.GrupoEntidade>(grupoEntidade);

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { modeloGrupoEntidade });

                    // Altera Grupo de Entidade
                    negocioGrupoEntidade.AtualizarStatus(modeloGrupoEntidade);

                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico);

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Altera um Grupo de Entidade
        /// </summary>
        /// <param name="grupoEntidade">Objeto Grupo de Entidade atualizado</param>
        public void AtualizarGrupo(Servicos.GrupoEntidade grupoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Altera um Grupo de Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioGrupoEntidade = new Negocio.GrupoEntidade();
                    Modelo.GrupoEntidade modeloGrupoEntidade;

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                    // Convert Business Entity para Data Contract Entity
                    modeloGrupoEntidade = Mapper.Map<Servicos.GrupoEntidade, Modelo.GrupoEntidade>(grupoEntidade);

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { modeloGrupoEntidade });

                    // Altera Grupo de Entidade
                    negocioGrupoEntidade.Atualizar(modeloGrupoEntidade);
                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico);

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        /// <summary>
        /// Consultar endereços do Estabelecimento
        /// </summary>
        /// <param name="codigoEntidade">Código do Grupo Entidade</param>
        /// <param name="tipoEndereco">Tipo de endereço buscado. E - Estabelecimento; C - Correspondência</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Lista de estabelecimentos de acordo com o Tipo de Endereço</returns>
        public List<Endereco> ConsultarEndereco(Int32 codigoEntidade, String tipoEndereco, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar endereços do Estabelecimento"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {

                    List<Modelo.Endereco> modeloEnderecos = null;
                    var enderecos = new List<Servicos.Endereco>();

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.Endereco, Servicos.Endereco>();

                    var negocioEndereco = new Negocio.Endereco();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, tipoEndereco });

                    modeloEnderecos = negocioEndereco.Consultar(codigoEntidade, tipoEndereco, out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, modeloEnderecos });

                    foreach (var modeloEndereco in modeloEnderecos)
                        // Converte Business Entity para Data Contract Entity
                        enderecos.Add(Mapper.Map<Modelo.Endereco, Servicos.Endereco>(modeloEndereco));

                    Log.GravarLog(EventoLog.FimServico, new { enderecos });

                    return enderecos;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigoEntidade"></param>
        /// <returns></returns>
        public DadosGerais ConsultarDadosGerais(Int32 codigoEntidade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar dados gerais"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {

                    Modelo.DadosGerais dadosGerais = new Modelo.DadosGerais();
                    Servicos.DadosGerais dadosServico = new DadosGerais();

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.DadosGerais, Servicos.DadosGerais>();

                    var negocioDados = new Negocio.DadosGerais();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade });
                    dadosGerais = negocioDados.Consultar(codigoEntidade, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, dadosGerais });

                    var result = Mapper.Map<Modelo.DadosGerais, Servicos.DadosGerais>(dadosGerais);
                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Recupera todas entidades a serem processadas pelo ISRobô
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <param name="qtdMaximaEntidades">Quantidade máxima de registros de Entidades a serem retornadas</param>
        public List<Servicos.Entidade> Listar(out Int32 codigoRetorno, Int32 qtdMaximaEntidades)
        {
            List<Modelo.Entidade> modeloEntidades = null;
            var entidades = new List<Servicos.Entidade>();
            using (Logger Log = Logger.IniciarLog("Recupera todas entidades"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioEntidade = new Negocio.Entidade();

                    Log.GravarLog(EventoLog.ChamadaNegocio);
                    modeloEntidades = negocioEntidade.Listar(out codigoRetorno, qtdMaximaEntidades);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, modeloEntidades });
                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();

                    foreach (var modeloEntidade in modeloEntidades)
                    {
                        // Converte Business Entity para Data Contract Entity
                        entidades.Add(Mapper.Map<Modelo.Entidade, Servicos.Entidade>(modeloEntidade));
                    }

                    Log.GravarLog(EventoLog.FimServico, new { entidades });


                    return entidades;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Tipo de Tecnologia do Estabelecimento.
        /// </summary>
        /// <param name="codigoEntidade"></param>
        /// <returns>
        /// 25 ou 26 - Komerci
        /// 20 - Normal
        /// 0 - Erro
        /// </returns>
        public Int32 ConsultarTecnologiaEstabelecimento(Int32 codigoEntidade, out Int32 codigoRetorno)
        {
            Int32 codigoTecnologia = 0;
            using (Logger Log = Logger.IniciarLog("Tipo de Tecnologia do Estabelecimento."))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade });

                    var negocioDados = new Negocio.DadosGerais();
                    codigoTecnologia = negocioDados.ConsultarTecnologiaEstabelecimento(codigoEntidade, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, codigoTecnologia });

                    Log.GravarLog(EventoLog.FimServico, new { codigoTecnologia });
                    return codigoTecnologia;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Atualiza os Dados Gerais na base do IS Sybase e IS SQL Server
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="dados">Modelo de Dados Gerais com informações atualizadas</param>
        /// <returns>Código de Erro</returns>
        public Int32 AtualizarDadosGerais(Int32 codigoEntidade, Servicos.DadosGerais dados)
        {
            using (Logger Log = Logger.IniciarLog("Atualiza os Dados Gerais na base do IS Sybase e IS SQL Server"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Modelo.DadosGerais modeloDadosGerais = new Modelo.DadosGerais();
                    Negocio.DadosGerais negocioDadosGerais = new Negocio.DadosGerais();

                    Mapper.CreateMap<Servicos.DadosGerais, Modelo.DadosGerais>();
                    modeloDadosGerais = Mapper.Map<Servicos.DadosGerais, Modelo.DadosGerais>(dados);
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, modeloDadosGerais });
                    Int32 result = negocioDadosGerais.Atualizar(codigoEntidade, modeloDadosGerais);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Atualizar os Status de Acesso da Entidade no PN
        /// </summary>
        /// <param name="codigoGrupoEntidade">Código de Grupo Entidade</param>
        /// <param name="codigoEntidade">Códido do Estabelecimento</param>
        /// <param name="statusEntidade">Status da Entidade</param>
        /// <param name="nomeResponsavel">E-mail do responsável pela atualização</param>
        /// <returns>Código de Retorno</returns>
        public Int32 AtualizarStatusAcesso(
            Int32 codigoGrupoEntidade, 
            Int32 codigoEntidade, 
            Comum.Enumerador.Status statusEntidade,
            String nomeResponsavel
            )
        {
            using (Logger Log = Logger.IniciarLog("Atualizar os Status de Acesso da Entidade no PN"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.Entidade negocioEntidade = new Negocio.Entidade();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoGrupoEntidade, codigoEntidade, statusEntidade });

                    Int32 result = negocioEntidade.AtualizarStatusAcesso(codigoGrupoEntidade, codigoEntidade, statusEntidade, nomeResponsavel);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }


        /// <summary>
        /// Consulta os dados de IPs do Komerci
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <returns>Informações do Komerci</returns>
        public Servicos.URLBack ConsultarURLBack(Servicos.Entidade entidade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta os dados de IPs do Komerci"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Modelo.URLBack modeloURLBack = new Modelo.URLBack();

                    Modelo.Entidade modeloEntidade = null;
                    if (entidade != null)
                    {
                        Mapper.CreateMap<Servicos.Entidade, Modelo.Entidade>();
                        Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                        modeloEntidade = Mapper.Map<Servicos.Entidade, Modelo.Entidade>(entidade);
                    }

                    Mapper.CreateMap<Modelo.URLBack, Servicos.URLBack>();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { modeloEntidade });
                    var negocioDados = new Negocio.DadosGerais();
                    modeloURLBack = negocioDados.ConsultarURLBack(modeloEntidade, out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, modeloURLBack });

                    var result = Mapper.Map<Modelo.URLBack, Servicos.URLBack>(modeloURLBack);
                    Log.GravarLog(EventoLog.FimServico, new { result });
                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        /// <summary>
        /// Atualiza os IPs do Komerci
        /// </summary>
        /// <param name="entidade">Dados da Entidade do Komerci</param>
        /// <param name="dados">Dados gerais do URLBack para atualização</param>
        /// <param name="usuarioAlteracao">Usuário responsável pela atualização</param>
        /// <returns>Código de Erro da procedure</returns>
        public Int32 AtualizarURLBack(Int32 codigoEntidade, Servicos.URLBack dados, String usuarioAlteracao)
        {
            using (Logger Log = Logger.IniciarLog("Atualiza os IPs do Komerci"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Modelo.URLBack modeloURLBack = null;

                    Negocio.DadosGerais negocioDadosGerais = new Negocio.DadosGerais();

                    Mapper.CreateMap<Servicos.URLBack, Modelo.URLBack>();
                    modeloURLBack = Mapper.Map<Servicos.URLBack, Modelo.URLBack>(dados);

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, modeloURLBack, usuarioAlteracao });

                    var result = negocioDadosGerais.AtualizarURLBack(codigoEntidade, modeloURLBack, usuarioAlteracao);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });
                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consultar filiais do Estabelecimento por tipo de associação
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <param name="tipoAssociacao">ID do Tipo de Associação</param>
        /// <returns>Listagem de filiais do estabelecimento</returns>
        public List<Servicos.Filial> ConsultarFiliais(Int32 codigoEntidade, Int32 tipoAssociacao, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar filiais do Estabelecimento por tipo de associação"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    //Objeto de retorno
                    List<Servicos.Filial> entidadesServicos = null;

                    //gera chave da consulta no cache (composta pelo código da entidade e do tipo de associação)
                    String chaveCache = String.Format("ConsultarFiliais_{0}_{1}", codigoEntidade, tipoAssociacao);
#if !DEBUG                    
                    //Recupera consulta de filiais do cache
                    entidadesServicos = CacheAdmin.Recuperar<List<Servicos.Filial>>(Cache.Filiais, chaveCache);
#endif

                    //Se objeto é nulo (não está em cache), executa pesquisa
                    if (entidadesServicos == null)
                    {
                        Negocio.Entidade _dados = new Negocio.Entidade();

                        Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, tipoAssociacao });
                        List<Modelo.Filial> entidadesModelo = _dados.ConsultarFiliais(codigoEntidade, tipoAssociacao, out codigoRetorno);
                        Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, entidadesModelo });

                        entidadesServicos = new List<Filial>();

                        Mapper.CreateMap<Modelo.Filial, Servicos.Filial>();

                        foreach (Modelo.Filial entidade in entidadesModelo)
                            entidadesServicos.Add(Mapper.Map<Servicos.Filial>(entidade));
#if !DEBUG
                        //Adiciona objeto ao cache de filiais
                        CacheAdmin.Adicionar(Cache.Filiais, chaveCache, entidadesServicos);
#endif
                    }
                    else
                    {
                        //atribui código de retorno 0, pois se a consulta está em cache, ela foi executada
                        //com sucesso da última vez
                        codigoRetorno = 0;

                        Log.GravarMensagem("Dados obtidos do cache", new { chaveCache });
                    }

                    Log.GravarLog(EventoLog.FimServico, new { entidadesServicos });

                    return entidadesServicos;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);


                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);


                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consultar filiais do Estabelecimento pelo código da entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de filiais do estabelecimento</returns>
        public List<Servicos.Filial> ConsultarFiliaisEntidade(Int32 codigoEntidade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar filiais do Estabelecimento pelo código da entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    //Objeto de retorno
                    List<Servicos.Filial> entidadesServicos = null;

                    //gera chave da consulta no cache (composta pelo código da entidade)
                    String chaveCache = String.Format("ConsultarFiliais_CodEnt_{0}", codigoEntidade);
#if !DEBUG
                    //Recupera consulta de filiais do cache
                    entidadesServicos = CacheAdmin.Recuperar<List<Servicos.Filial>>(Cache.Filiais, chaveCache);
#endif

                    //Se objeto é nulo (não está em cache), executa pesquisa
                    if (entidadesServicos == null)
                    {
                        Negocio.Entidade dados = new Negocio.Entidade();

                        Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade });
                        List<Modelo.Filial> entidadesModelo = dados.ConsultarFiliais(codigoEntidade, out codigoRetorno);
                        Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, entidadesModelo });

                        entidadesServicos = new List<Filial>();

                        Mapper.CreateMap<Modelo.Filial, Servicos.Filial>();

                        foreach (Modelo.Filial entidade in entidadesModelo)
                            entidadesServicos.Add(Mapper.Map<Servicos.Filial>(entidade));
#if !DEBUG
                        //Adiciona objeto ao cache de filiais
                        CacheAdmin.Adicionar(Cache.Filiais, chaveCache, entidadesServicos);
#endif
                    }
                    else
                    {
                        //atribui código de retorno 0, pois se a consulta está em cache, ela foi executada
                        //com sucesso da última vez
                        codigoRetorno = 0;

                        Log.GravarMensagem("Dados obtidos do cache", new { chaveCache });
                    }

                    Log.GravarLog(EventoLog.FimServico, new { entidadesServicos });

                    return entidadesServicos;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }


        /// <summary>
        /// Retorna a relação de entidade por CNPJ informado
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="tipoAssociacao">Tipo de associação</param>
        /// <param name="CNPJ">CNPJ</param>
        /// <param name="codigoRetorno">Código de Retorno da procedure</param>
        /// <returns>List<Entidade></returns>
        public List<Servicos.Entidade> ConsultarPorCNPJ(Int32 codigoEntidade, Int32 tipoAssociacao, Double CNPJ, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Retorna a relação de entidade por CNPJ informado"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Negocio.Entidade negocioEntidade = new Negocio.Entidade();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, tipoAssociacao });
                    List<Modelo.Entidade> entidadesModelo = negocioEntidade.ConsultarPorCNPJ(codigoEntidade, tipoAssociacao, CNPJ, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, entidadesModelo });


                    List<Servicos.Entidade> entidadesServicos = new List<Entidade>();

                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();

                    foreach (Modelo.Entidade entidade in entidadesModelo)
                        entidadesServicos.Add(Mapper.Map<Servicos.Entidade>(entidade));

                    Log.GravarLog(EventoLog.FimServico, new { entidadesServicos });

                    return entidadesServicos;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        /// <summary>
        /// Retorna a relação de entidade por CNPJ informado
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="tipoAssociacao">Tipo de associação</param>
        /// <param name="CNPJ">CNPJ</param>
        /// <param name="codigoRetorno">Código de Retorno da procedure</param>
        /// <returns>List<Entidade></returns>
        public List<Servicos.InformacaoPV> ConsultarInformacoesPV(Int32 codigoEntidade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Retorna a relação de entidade por CNPJ informado"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.Entidade negocioEntidade = new Negocio.Entidade();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade });

                    List<Modelo.InformacaoPV> entidadesModelo = negocioEntidade.ConsultarInformacoesPV(codigoEntidade, out codigoRetorno);
                    List<Servicos.InformacaoPV> entidadesServicos = new List<Servicos.InformacaoPV>();
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, entidadesModelo });


                    Mapper.CreateMap<Modelo.InformacaoPV, Servicos.InformacaoPV>();

                    foreach (Modelo.InformacaoPV entidade in entidadesModelo)
                        entidadesServicos.Add(Mapper.Map<Servicos.InformacaoPV>(entidade));

                    Log.GravarLog(EventoLog.FimServico, new { entidadesServicos });

                    return entidadesServicos;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        /// <summary>
        /// Consulta a entidade pelo número do terminal
        /// </summary>
        /// <param name="numeroTerminal">Número do terminal</param>
        /// <param name="codigoRetorno">Código de retorno da procedure</param>
        /// <returns>Entidade</returns>
        public Servicos.Entidade ConsultarPorTerminal(String numeroTerminal, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta a entidade pelo número do terminal"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numeroTerminal });
                    Negocio.Entidade negocioEntidade = new Negocio.Entidade();
                    Modelo.Entidade entidadeModelo = negocioEntidade.ConsultarPorTerminal(numeroTerminal, out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, entidadeModelo });


                    Servicos.Entidade entidadeServico = new Entidade();

                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();

                    entidadeServico = (Mapper.Map<Servicos.Entidade>(entidadeModelo));

                    Log.GravarLog(EventoLog.FimServico, new { entidadeServico });

                    return entidadeServico;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta entidades associadas a um usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id usuário</param>
        /// <param name="codigoRetorno">Código de Retorno</param>
        /// <returns>Listagem das entidades</returns>
        public List<Modelo.Entidade> ConsultarPorUsuario(Int32 codigoIdUsuario, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta a entidade pelo código Id usuário"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoIdUsuario });

                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();

                    Negocio.Entidade negocioEntidade = new Negocio.Entidade();
                    List<Modelo.Entidade> entidades = negocioEntidade.ConsultarPorUsuario(codigoIdUsuario, out codigoRetorno);
                    
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno });

                    return entidades;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta todas as propriedades de um estabelecimento no banco do GE (spge6002) e
        /// </summary>
        /// <returns>Entidade</returns>
        public Servicos.Entidade Consultar(Int32 codigoEntidade, Boolean validarSenha, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta todas as propriedades de um estabelecimento no banco do GE (spge6002"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, validarSenha });
                    Negocio.Entidade negocioEntidade = new Negocio.Entidade();
                    Modelo.Entidade entidadeModelo = negocioEntidade.Consultar(codigoEntidade, validarSenha, out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, entidadeModelo });


                    Servicos.Entidade entidadeServico = new Entidade();

                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                    entidadeServico = (Mapper.Map<Servicos.Entidade>(entidadeModelo));

                    Log.GravarLog(EventoLog.FimServico, new { entidadeServico });

                    return entidadeServico;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta os dados bancários de Crédito ou Débito
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="tipoDado">Tipo de dados a ser consultado.
        /// C- Crédito
        /// D - Débito
        /// </param>
        /// <returns>Retorna a lista de Dados Bancários</returns>
        public List<Servicos.DadosBancarios> ConsultarDadosBancarios(Int32 codigoEntidade, String tipoDados, out Int32 codigoRetorno)
        {
            List<Modelo.DadosBancarios> listaDados = null;
            var dados = new List<Servicos.DadosBancarios>();
            using (Logger Log = Logger.IniciarLog("Consulta os dados bancários de Crédito ou Débito"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioDados = new Negocio.DadosBancarios();
                    Mapper.CreateMap<Modelo.DadosBancarios, Servicos.DadosBancarios>();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, tipoDados });

                    listaDados = negocioDados.Consultar(codigoEntidade, tipoDados, out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, listaDados });


                    foreach (var dado in listaDados)
                    {
                        dados.Add(Mapper.Map<Modelo.DadosBancarios, Servicos.DadosBancarios>(dado));
                    }
                    Log.GravarLog(EventoLog.FimServico, new { listaDados });

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                return dados;
            }
        }
        
        /// <summary>
        /// Consulta as tarifas de Transmissão
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="tipoDados">Tipo de dados a ser consultado.
        /// CR - Crédito
        /// DB - Débito
        /// </param>
        /// <returns></returns>
        public Servicos.Tarifas ConsultarTarifas(Int32 codigoEntidade, String tipoDados, out Int32 codigoRetorno)
        {
            Modelo.Tarifas tarifas = new Modelo.Tarifas();
            var dados = new Servicos.Tarifas();
            using (Logger Log = Logger.IniciarLog("Consulta as tarifas de Transmissão"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioDados = new Negocio.DadosBancarios();
                    Mapper.CreateMap<Modelo.Tarifas, Servicos.Tarifas>();


                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, tipoDados });
                    tarifas = negocioDados.ConsultarTarifas(codigoEntidade, tipoDados, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, tarifas });

                    dados = Mapper.Map<Modelo.Tarifas, Servicos.Tarifas>(tarifas);
                    Log.GravarLog(EventoLog.FimServico, new { dados });


                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                return dados;
            }
        }
        
        /// <summary>
        /// Consulta os dados de Domicílios Bancários da Entidade
        /// </summary>
        /// <param name="codigoEntidade"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns>Dados do Domicilio Bancário</returns>
        public List<Servicos.DadosDomiciolioBancario> ConsultarDadosDomiciliosBancarios(Int32 codigoEntidade, out Boolean permissaoAlteracao, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta os dados de Domicílio Bancário da Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var bancoNegocio = new Negocio.DadosBancarios();

                    //Servicos.DadosDomiciolioBancario domicilioServico = null;
                    //Modelo.DadosDomiciolioBancario domicilioModelo = null;

                    //Servicos.Bandeira bandeiraServico = null;
                    //Modelo.Bandeira bandeiraModelo = null;

                    List<Modelo.DadosDomiciolioBancario> listaDomicilioModelo = null;
                    List<Servicos.DadosDomiciolioBancario> listaDomicilioServico = new List<DadosDomiciolioBancario>();

                    //List<Servicos.Bandeira> listaBandeirasServico = null;
                    //List<Modelo.Bandeira> listaBandeirasModelo = null;

                    Mapper.CreateMap<Modelo.DadosDomiciolioBancario, Servicos.DadosDomiciolioBancario>();
                    Mapper.CreateMap<Modelo.Bandeira, Servicos.Bandeira>();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade });
                    listaDomicilioModelo = bancoNegocio.ConsultarDadosDomiciliosBancarios(codigoEntidade, out permissaoAlteracao, out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, listaDomicilioModelo });

                    foreach (Modelo.DadosDomiciolioBancario domicilio in listaDomicilioModelo)
                    {
                        //foreach (Modelo.Bandeira bandeira in domicilio.BandeirasCredito)
                        //{
                        //    domicilioServico.BandeirasCredito.Add(Mapper.Map<Modelo.Bandeira, Servicos.Bandeira>(bandeira));
                        //}

                        //foreach (Modelo.Bandeira bandeira in domicilio.BandeirasDebito)
                        //{
                        //    domicilioServico.BandeirasDebito.Add(Mapper.Map<Modelo.Bandeira, Servicos.Bandeira>(bandeira));
                        //}

                        listaDomicilioServico.Add(Mapper.Map<Modelo.DadosDomiciolioBancario, Servicos.DadosDomiciolioBancario>(domicilio));
                    }
                    //domicilioServico = Mapper.Map<Modelo.DadosDomiciolioBancario, Servicos.DadosDomiciolioBancario>(domicilioModelo);

                    Log.GravarLog(EventoLog.FimServico, new { listaDomicilioServico });

                    return listaDomicilioServico;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta bancos cadastradados na base DR
        /// </summary>
        /// <returns>Lista de Bancos</returns>
        public List<Servicos.Banco> ConsultarBancos()
        {
            using (Logger Log = Logger.IniciarLog("Consulta bancos cadastradados na base DR"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var bancoNegocio = new Negocio.DadosBancarios();

                    List<Servicos.Banco> listaBancosServico = new List<Servicos.Banco>();
                    List<Modelo.Banco> listaBancosModelo = new List<Modelo.Banco>();

                    Log.GravarLog(EventoLog.ChamadaNegocio);
                    Mapper.CreateMap<Modelo.Banco, Servicos.Banco>();
                    listaBancosModelo = bancoNegocio.ConsultarBancos();

                    Log.GravarLog(EventoLog.RetornoNegocio, new { listaBancosModelo });
                    foreach (Modelo.Banco banco in listaBancosModelo)
                    {
                        listaBancosServico.Add(Mapper.Map<Modelo.Banco, Servicos.Banco>(banco));
                    }


                    Log.GravarLog(EventoLog.FimServico, new { listaBancosServico });

                    return listaBancosServico;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        /// <summary>
        /// Consulta bancos cadastradados na base DR para confirmação positiva
        /// </summary>
        /// <returns>Lista de Bancos</returns>
        public List<Servicos.Banco> ConsultarBancosConfirmacaoPositiva()
        {
            using (Logger Log = Logger.IniciarLog("Consulta bancos cadastradados na base DR para confirmação positiva"))
            {
                Log.GravarLog(EventoLog.InicioServico);


                try
                {
                    var bancoNegocio = new Negocio.DadosBancarios();

                    List<Servicos.Banco> listaBancosServico = new List<Servicos.Banco>();
                    List<Modelo.Banco> listaBancosModelo = new List<Modelo.Banco>();

                    Log.GravarLog(EventoLog.ChamadaNegocio);
                    Mapper.CreateMap<Modelo.Banco, Servicos.Banco>();
                    listaBancosModelo = bancoNegocio.ConsultarBancosConfirmacaoPositiva();

                    Log.GravarLog(EventoLog.RetornoNegocio, new { listaBancosModelo });
                    foreach (Modelo.Banco banco in listaBancosModelo)
                    {
                        listaBancosServico.Add(Mapper.Map<Modelo.Banco, Servicos.Banco>(banco));
                    }


                    Log.GravarLog(EventoLog.FimServico, new { listaBancosServico });
                    return listaBancosServico;
                }
                catch (PortalRedecardException ex)
                {

                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {

                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta os bancos com Confirmação Eletrônica disponível
        /// </summary>
        /// <returns>Lista de bancos com confirmação eletrônica</returns>
        public List<Servicos.Banco> ConsultarBancosConfirmacaoEletronica()
        {
            using (Logger Log = Logger.IniciarLog("Consulta os bancos com Confirmação Eletrônica disponível"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var bancoNegocio = new Negocio.DadosBancarios();

                    List<Servicos.Banco> listaBancosServico = new List<Servicos.Banco>();
                    List<Modelo.Banco> listaBancosModelo = new List<Modelo.Banco>();
                    Log.GravarLog(EventoLog.ChamadaNegocio);

                    Mapper.CreateMap<Modelo.Banco, Servicos.Banco>();
                    listaBancosModelo = bancoNegocio.ConsultarBancosConfirmacaoEletronica();

                    Log.GravarLog(EventoLog.RetornoNegocio, new { listaBancosModelo });

                    foreach (Modelo.Banco banco in listaBancosModelo)
                    {
                        listaBancosServico.Add(Mapper.Map<Modelo.Banco, Servicos.Banco>(banco));
                    }

                    Log.GravarLog(EventoLog.FimServico, new { listaBancosServico });
                    return listaBancosServico;
                }
                catch (PortalRedecardException ex)
                {

                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {

                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Valida a Alteração dos dados do Domicílio Bancários da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="dadosBancarios">Dados do Domicílio Bancário</param>
        /// <param name="confirmacaoEletronica">Indica se há confirmação eletrônica
        /// S - Sim, há confirmação eletrônica
        /// N - Não, não há confirmação eletrônica
        /// </param>
        /// <param name="aguardarDocumento">Indica se ocorre espera de recebimento de Documento
        /// S - Sim. Não há Confirmação Eletrônica
        /// N - Não. Há Confirmação Eletrônica
        /// </param>
        /// <param name="codigoRetorno">Código de erro retornado pela procedure</param>
        /// <returns>
        /// True - Alteração Válida
        /// False - Alteração inválida
        /// </returns>
        public Boolean ValidarAlteracaoDomicilioBancario(Int32 codigoEntidade, Servicos.DadosDomiciolioBancario dadosBancarios, String confirmacaoEletronica, String aguardarDocumento, out Int32 codigoRetorno)
        {
            codigoRetorno = 0;
            using (Logger Log = Logger.IniciarLog("Valida a Alteração dos dados do Domicílio Bancários da Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);


                try
                {
                    var dadosNegocio = new Negocio.DadosBancarios();
                    Modelo.DadosDomiciolioBancario domicilioModelo = new Modelo.DadosDomiciolioBancario();

                    Mapper.CreateMap<Servicos.DadosDomiciolioBancario, Modelo.DadosDomiciolioBancario>();
                    Mapper.CreateMap<Servicos.Bandeira, Modelo.Bandeira>();

                    domicilioModelo = Mapper.Map<Servicos.DadosDomiciolioBancario, Modelo.DadosDomiciolioBancario>(dadosBancarios);
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, domicilioModelo, confirmacaoEletronica, aguardarDocumento });

                    bool result = dadosNegocio.ValidarAlteracaoDomicilioBancario(codigoEntidade, domicilioModelo, confirmacaoEletronica, aguardarDocumento, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno });

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {

                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {

                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Grava as alterações do Domicílio Bancário
        /// </summary>
        /// <param name="numeroRequisicao">Número de Requisição gerado pela proc SPWM0123</param>
        /// <param name="numeroSolicitacao">Número de Solicitação gerado pela proc SPWM0123</param>
        /// <param name="tipoOperacao">Tipo da operação bancária: CR - Crédito; DB - Débito; CDC - Construcard</param>
        /// <param name="codigoBanco">Código do Banco</param>
        /// <param name="agencia">Agência</param>
        /// <param name="conta">Número da conta</param>
        /// <param name="aguardaDocumento">Indica necessidade do envio de documento
        /// S - Sem confirmação eletrônica
        /// N - Com confirmação eletrônica
        /// </param>
        /// <param name="confirmacaoEletronica">Indica se há confirmação eletrônica
        /// S - Com confirmação eletrônica
        /// N - Sem confirmação eletrônica
        /// </param>
        /// <param name="canal">Canal de alteração</param>
        /// <param name="celula">Célula de alteração</param>
        /// <param name="tipoTransacao">Tipo da transação</param>
        /// <returns></returns>
        public Int32 InserirAlteracaoDomicilioBancario(Int32 numeroRequisicao, Int32 numeroSolicitacao, String tipoOperacao, String codigoBanco, String agencia,
            String conta, String aguardaDocumento, String confirmacaoEletronica, String canal, String celula, String tipoTransacao)
        {
            using (Logger Log = Logger.IniciarLog("Grava as alterações do Domicílio Bancário"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new
                    {
                        numeroRequisicao,
                        numeroSolicitacao,
                        tipoOperacao,
                        codigoBanco,
                        agencia,
                        conta,
                        aguardaDocumento,
                        confirmacaoEletronica,
                        canal,
                        celula,
                        tipoTransacao
                    });
                    var negocioDados = new Negocio.DadosBancarios();

                    Int32 result = negocioDados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, tipoOperacao, codigoBanco,
                             agencia, conta, aguardaDocumento, confirmacaoEletronica, canal, celula, tipoTransacao);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Grava uma nova solicitação de alteração de domicílio bancário
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="grupoSolicitacao">Grupo de Solicitação</param>
        /// <param name="cnpj">CNPJ</param>
        /// <param name="numeroRequisicao">Retorno com o número da Requisição</param>
        /// <param name="numeroSolicitacao">Retorno com o número da Solicitação</param>
        /// <returns></returns>
        public Int32 InserirSolicitacaoAlteracaoDomicilioBancario(Int32 codigoEntidade, String grupoSolicitacao, String cnpj,
            out Int32 numeroRequisicao, out Int32 numeroSolicitacao)
        {
            using (Logger Log = Logger.IniciarLog("Grava uma nova solicitação de alteração de domicílio bancário"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {

                    var negocioDados = new Negocio.DadosBancarios();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, grupoSolicitacao, cnpj });
                    Int32 result = negocioDados.InserirSolicitacaoAlteracaoDomicilioBancario(codigoEntidade, grupoSolicitacao, cnpj, out numeroRequisicao, out numeroSolicitacao);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { numeroRequisicao, numeroSolicitacao });

                    Log.GravarLog(EventoLog.FimServico, new { result });
                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        /// <summary>
        /// Insere um Grupo de Entidade
        /// </summary>
        /// <param name="grupoEntidade">Objeto Grupo de Entidade</param>
        public void InserirGrupo(Servicos.GrupoEntidade grupoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Insere um Grupo de Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioGrupoEntidade = new Negocio.GrupoEntidade();
                    Modelo.GrupoEntidade modeloGrupoEntidade;

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                    // Convert Business Entity para Data Contract Entity
                    modeloGrupoEntidade = Mapper.Map<Servicos.GrupoEntidade, Modelo.GrupoEntidade>(grupoEntidade);

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { modeloGrupoEntidade });
                    // Inserir Grupo de Entidade
                    negocioGrupoEntidade.Inserir(modeloGrupoEntidade);
                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico);


                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta as alterações de Domicílio Bancário solicitadas pela Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <returns>Última Solicitação de alteração de Domicílio Bancário</returns>
        public List<Servicos.DadosDomiciolioBancario> ConsultarDomiciliosAlterados(Int32 codigoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Consulta as alterações de Domicílio Bancário solicitadas pela Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    var bancoNegocio = new Negocio.DadosBancarios();

                    List<Servicos.DadosDomiciolioBancario> domiciliosServico = new List<DadosDomiciolioBancario>();
                    List<Modelo.DadosDomiciolioBancario> domiciliosModelo = new List<Modelo.DadosDomiciolioBancario>();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade });

                    domiciliosModelo = bancoNegocio.ConsultarDomiciliosAlterados(codigoEntidade);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { domicilioModelo = domiciliosModelo });

                    Mapper.CreateMap<Modelo.DadosDomiciolioBancario, Servicos.DadosDomiciolioBancario>();
                    Mapper.CreateMap<Modelo.Bandeira, Servicos.Bandeira>();

                    foreach (Modelo.DadosDomiciolioBancario dados in domiciliosModelo)
                    {
                        domiciliosServico.Add(Mapper.Map<Modelo.DadosDomiciolioBancario, Servicos.DadosDomiciolioBancario>(dados));
                    }

                    Log.GravarLog(EventoLog.FimServico, new { DomicilioServico = domiciliosServico });

                    return domiciliosServico;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        
        /// <summary>
        /// Lista das alterações de Domicílio Bancário solicitadas pela Entidade em status Pendente
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <returns>Lista de Solicitações de alteração de Domicílio Bancário</returns>
        public List<Servicos.DadosDomiciolioBancario> ListarDomiciliosAlterados(Int32 codigoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Lista as alterações de Domicílio Bancário solicitadas pela Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    var bancoNegocio = new Negocio.DadosBancarios();

                    List<Servicos.DadosDomiciolioBancario> domiciliosServico = new List<DadosDomiciolioBancario>();
                    List<Modelo.DadosDomiciolioBancario> domiciliosModelo = new List<Modelo.DadosDomiciolioBancario>();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade });

                    domiciliosModelo = bancoNegocio.ListarDomiciliosAlterados(codigoEntidade);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { domicilioModelo = domiciliosModelo });

                    Mapper.CreateMap<Modelo.DadosDomiciolioBancario, Servicos.DadosDomiciolioBancario>();
                    Mapper.CreateMap<Modelo.Bandeira, Servicos.Bandeira>();

                    foreach (Modelo.DadosDomiciolioBancario dados in domiciliosModelo)
                    {
                        domiciliosServico.Add(Mapper.Map<Modelo.DadosDomiciolioBancario, Servicos.DadosDomiciolioBancario>(dados));
                    }

                    Log.GravarLog(EventoLog.FimServico, new { DomicilioServico = domiciliosServico });

                    return domiciliosServico;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Cancela a alteração de Domicílio Bancária solicitada
        /// </summary>
        /// <param name="numeroSolicitacao">Código da Solicitação de Alteração</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoRetorno">Código de retorno de erro da procedure</param>
        /// <returns></returns>
        public Int16 CancelarAlteracao(Int32 numeroSolicitacao, Int32 codigoEntidade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Cancela a alteração de Domicílio Bancária solicitada"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numeroSolicitacao, codigoEntidade });
                    var dadosNegocio = new Negocio.DadosBancarios();
                    dadosNegocio.CancelarAlteracao(numeroSolicitacao, codigoEntidade, out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno });

                    Log.GravarLog(EventoLog.FimServico, new { codigoRetorno });
                    return 0;
                }
                catch (PortalRedecardException ex)
                {

                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {

                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Inserir Log sobre entidade
        /// </summary>
        /// <param name="entidade">Entidade</param>
        /// <param name="tipo">Tipo de Log</param>
        /// <param name="valor">Valor do Log</param>
        /// <returns>Código de retorno</returns>
        public Int32 InserirLog(Servicos.Entidade entidade, String tipo, String valor)
        {
            using (Logger Log = Logger.IniciarLog("Inserir Log sobre entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);


                try
                {
                    var negocioEntidade = new Negocio.Entidade();

                    Modelo.Entidade modeloEntidade = null;

                    // Convert Business Entity para Data Contract Entity
                    if (entidade != null)
                    {
                        Mapper.CreateMap<Servicos.Entidade, Modelo.Entidade>();
                        Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                        // Convert Data Contract Entity para Business Entity
                        modeloEntidade = Mapper.Map<Servicos.Entidade, Modelo.Entidade>(entidade);
                    }

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { modeloEntidade, tipo, valor });
                    var result = negocioEntidade.InserirLog(modeloEntidade, tipo, valor);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    // Inseri log
                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Alterar e-mail da entidade
        /// </summary>
        /// <param name="entidade">Entidade</param>
        /// <returns>Código de retorno</returns>
        public Int32 AlterarEmail(Servicos.Entidade entidade)
        {
            using (Logger Log = Logger.IniciarLog("Alterar e-mail da entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioEntidade = new Negocio.Entidade();

                    Modelo.Entidade modeloEntidade = null;

                    // Convert Business Entity para Data Contract Entity
                    if (entidade != null)
                    {
                        Mapper.CreateMap<Servicos.Entidade, Modelo.Entidade>();
                        Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                        // Convert Data Contract Entity para Business Entity
                        modeloEntidade = Mapper.Map<Servicos.Entidade, Modelo.Entidade>(entidade);
                    }

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { modeloEntidade });
                    Int32 result = negocioEntidade.AlterarEmail(modeloEntidade);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    // Alterar e-mail
                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        /// <summary>
        /// Consulta os Estados na base do SQL Server
        /// </summary>
        /// <returns>Lista de Estados na base SQL Server</returns>
        public List<Servicos.Estados> ConsultarEstados()
        {
            using (Logger Log = Logger.IniciarLog("Consulta os Estados na base do SQL Server"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var estadoNegocio = new Negocio.Estados();

                    List<Servicos.Estados> listaEstadosServico = new List<Servicos.Estados>();
                    List<Modelo.Estados> listaEstadosModelo = new List<Modelo.Estados>();

                    Log.GravarLog(EventoLog.ChamadaNegocio);
                    listaEstadosModelo = estadoNegocio.Consultar();
                    Log.GravarLog(EventoLog.RetornoNegocio, new { listaEstadosModelo });

                    Mapper.CreateMap<Modelo.Estados, Servicos.Estados>();
                    foreach (Modelo.Estados estado in listaEstadosModelo)
                    {
                        listaEstadosServico.Add(Mapper.Map<Modelo.Estados, Servicos.Estados>(estado));
                    }

                    Log.GravarLog(EventoLog.FimServico, new { listaEstadosServico });

                    return listaEstadosServico;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        /// <summary>
        /// Consulta as perguntas aleatórias disponíveis para o estabelecimento informado
        /// </summary>
        public List<Servicos.Pergunta> ConsultarPerguntasAleatorias(Int32 numeroPV)
        {
            using (Logger Log = Logger.IniciarLog("Consulta as perguntas aleatórias disponíveis para o estabelecimento informado"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocio = new Negocio.Entidade();
                    List<Servicos.Pergunta> perguntas = new List<Servicos.Pergunta>();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numeroPV });
                    List<Modelo.Pergunta> _perguntas = negocio.ConsultarPerguntasAleatorias(numeroPV);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { _perguntas });

                    Mapper.CreateMap<Modelo.Pergunta, Servicos.Pergunta>();
                    foreach (Modelo.Pergunta pergunta in _perguntas)
                    {
                        perguntas.Add(Mapper.Map<Servicos.Pergunta>(pergunta));
                    }
                    Log.GravarLog(EventoLog.FimServico, new { perguntas });

                    return perguntas;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        /// <summary>
        /// Consultar a lista de agências de um banco
        /// </summary>
        /// <returns></returns>
        public List<Servicos.Agencia> ConsultarAgencias(int codigoAgencia, int codigoBanco, out int codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar a lista de agências de um banco"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocio = new Negocio.DadosBancarios();
                    List<Servicos.Agencia> agencias = new List<Servicos.Agencia>();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoAgencia, codigoBanco });
                    List<Modelo.Agencia> _agencias = negocio.ConsultarAgencias(codigoAgencia, codigoBanco, out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, _agencias });

                    Mapper.CreateMap<Modelo.Agencia, Servicos.Agencia>();
                    foreach (Modelo.Agencia agencia in _agencias)
                    {
                        agencias.Add(Mapper.Map<Servicos.Agencia>(agencia));
                    }
                    Log.GravarLog(EventoLog.FimServico, new { agencias });

                    return agencias;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consultar dados do PV na base de dados do GE
        /// </summary>
        public Servicos.Entidade ConsultarDadosPV(out Int32 codigoRetorno, Int32 codigoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Consultar dados do PV na base de dados do GE"))
            {
                Log.GravarLog(EventoLog.InicioServico);


                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade });
                    var negocio = new Negocio.Entidade();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Modelo.Entidade entidade = negocio.ConsultarDadosPV(out codigoRetorno, codigoEntidade);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, entidade });

                    var result = Mapper.Map<Servicos.Entidade>(entidade);

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        
        #region Consulta pvs por email/cpf

        /// <summary>
        /// Consulta os pvs po CPF
        /// </summary>
        /// <param name="cpf">CPF</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <param name="pvsSelecionados">Filtro para pvs selecionados</param>
        /// <param name="filtroGenerico">Filtro generico</param>
        /// <returns></returns>
        public Modelo.EntidadeServicoModel[] ConsultarPvPorCpf(Int64 cpf, out int codigoRetorno, out int qtdEmailsPorCpf, string pvsSelecionados = null, string filtroGenerico = null)
        {
            using (Logger Log = Logger.IniciarLog("Consultar PV por CPF"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                Negocio.Entidade negocioEntidade = new Negocio.Entidade();

                try
                {
                    return negocioEntidade.ConsultarPv(cpf, out codigoRetorno, pvsSelecionados, filtroGenerico, out qtdEmailsPorCpf);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta os pvs por CPF com paginação no banco
        /// </summary>
        /// <param name="cpf">CPF</param>
        /// <param name="codigoRetorno">codigo de retorno</param>
        /// <param name="totalRows">Total de registros</param>
        /// <param name="pagina">Pagina</param>
        /// <param name="qtdRegistros">Quantidade de registros por pagina</param>
        /// <param name="qtdEmailsPorCpf">Quantidade de registros por pagina</param>
        /// <param name="pvsSelecionados">Pvs selecionados</param>
        /// <param name="filtroGenerico">Filtro generico</param>
        /// <returns></returns>
        public Modelo.EntidadeServicoModel[] ConsultarPvPorCpfComPaginacao(Int64 cpf, out int codigoRetorno, out int totalRows, int pagina, int qtdRegistros, out int qtdEmailsPorCpf, bool retornarEmail, string pvsSelecionados = null, string filtroGenerico = null)
        {
            using (Logger Log = Logger.IniciarLog("Consultar PV por CPF"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                Negocio.Entidade negocioEntidade = new Negocio.Entidade();

                try
                {
                    return negocioEntidade.ConsultarPv(cpf, out codigoRetorno, out totalRows, out qtdEmailsPorCpf, retornarEmail, pagina, qtdRegistros, pvsSelecionados, filtroGenerico);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        
        /// <summary>
        /// Consulta os pvs por email
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <param name="pvsSelecionados">Filtro pvs selecionados</param>
        /// <param name="filtroGenerico">Filtro generico</param>
        /// <returns></returns>
        public Modelo.EntidadeServicoModel[] ConsultarPvPorEmail(string email, out int codigoRetorno, out bool? pvSenhasIguais,  string pvsSelecionados = null, string filtroGenerico = null)
        {
            using (Logger Log = Logger.IniciarLog("Consultar PV por E-mail"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                Negocio.Entidade negocioEntidade = new Negocio.Entidade();

                try
                {
                    var retorno = negocioEntidade.ConsultarPv(email, out codigoRetorno, pvsSelecionados, filtroGenerico);
                    pvSenhasIguais = negocioEntidade.PvSenhasIguais(email, out codigoRetorno);
                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        
        /// <summary>
        /// Consulta os pvs por email com paginação
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="codigoRetorno">codigo de retorno</param>
        /// <param name="totalRows">Total de registros</param>
        /// <param name="pagina">Pagina</param>
        /// <param name="qtdRegistros">Quantidade de registros por pagina</param>
        /// <param name="pvSenhasIguais">Pv´s senhas iguais.</param>
        /// <param name="pvsSelecionados">Pvs selecionados</param>
        /// <param name="filtroGenerico">Filtro generico</param>
        /// <returns></returns>
        public Modelo.EntidadeServicoModel[] ConsultarPvPorEmailComPaginacao(string email, out int codigoRetorno, out int totalRows, int pagina, int qtdRegistros, string pvsSelecionados = null, string filtroGenerico = null)
        {
            using (Logger Log = Logger.IniciarLog("Consultar PV por E-mail"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                Negocio.Entidade negocioEntidade = new Negocio.Entidade();
                try
                {
                    return negocioEntidade.ConsultarPv(email, out codigoRetorno, out totalRows, pagina, qtdRegistros, pvsSelecionados, filtroGenerico);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region Consultas Passo 1 primeiro acesso

        /// <summary>
        /// Consulta os PVs relacionados ao CPF/CNPJ 
        /// </summary>
        /// <param name="codigoRetorno"></param>
        /// <param name="cnpj"></param>
        /// <param name="cpf"></param>
        /// <returns></returns>
        public Modelo.Entidade[] ConsultarPvGePorCpfCnpj(out int codigoRetorno, long? cnpj, long? cpf)
        {
            using (Logger Log = Logger.IniciarLog("Consultar PV no GE por Cnpj/CPF"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                Negocio.Entidade negocioEntidade = new Negocio.Entidade();

                try
                {
                    return negocioEntidade.ConsultarPvGePorCpfCnpj(out codigoRetorno, cnpj, cpf);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }


        /// <summary>
        /// Verifica se os pvs relacionados ao CPF\CNPJ são apenas de filiais
        /// </summary>
        /// <param name="codigoRetorno">código retorno</param>
        /// <param name="cnpj">CNPJ</param>
        /// <param name="cpf">CPF</param>
        /// <returns></returns>
        public bool PvsRelacionadosSaoFiliais(out int codigoRetorno, long cnpj)
        {
            using (Logger Log = Logger.IniciarLog("Consultar PV no GE por Cnpj/CPF PvsRelacionadosSaoFiliais"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                Negocio.Entidade negocioEntidade = new Negocio.Entidade();

                try
                {
                    return negocioEntidade.PvsRelacionadosSaoFiliais(out codigoRetorno, cnpj);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }


        /// <summary>
        /// Seleciona as entidades que existem no GE vinculadas ao CPF e CNPJ, cria essas entidades no PN caso não existam
        /// e retornas as entidades que estão ativas com informações de usuario
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <param name="cnpj">CNPJ</param>
        /// <param name="cpf">CPF</param>
        /// <returns></returns>
        public Modelo.EntidadeServicoModel[] ConsultarEntidadeGeCriarNoPN(out int codigoRetorno, long? cpf, long? cnpj)
        {
            using (Logger Log = Logger.IniciarLog("Consultar a entidade no PN"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                Negocio.Entidade negocioEntidade = new Negocio.Entidade();

                try
                {
                    return negocioEntidade.ConsultarEntidadeGeCriarNoPN(out codigoRetorno, cpf, cnpj);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta os e-mails dos PVs informados
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno caso ocorra algum erro na consulta</param>
        /// <param name="pvs">Arrau de PVs para consulta à base de dados</param>
        /// <returns>Dicionário contendo o e-mail para cada PV consultado</returns>
        public Dictionary<int, string> ConsultarEmailPVs(out int codigoRetorno, int[] pvs)
        {
            using (Logger Log = Logger.IniciarLog("Método ConsultarEmailPVs"))
            {
                codigoRetorno = 0;

                Log.GravarLog(EventoLog.InicioServico);
                Negocio.Entidade negocioEntidade = new Negocio.Entidade();
                try
                {
                    return negocioEntidade.ConsultarEmailPVs(out codigoRetorno, pvs);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region Consultas passo 2 primeiro acesso

        public Modelo.Mensagens.ConfirmacaoPositivaPrimeiroAcessoResponse ValidarConfirmacaoPositivaPrimeiroAcesso(
                    string emailUsuarioAlteracao,
                    Modelo.Mensagens.ConfirmacaoPositivaPrimeiroAcessoRequest request,
                    out EntidadeServicoModel[] entidadesPossuemUsuario,
                    out EntidadeServicoModel[] entidadesPossuemMaster)
        {
            using (Logger Log = Logger.IniciarLog("Consultar a emtidade no PN"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                Negocio.Entidade negocioEntidade = new Negocio.Entidade();

                try
                {
                    return negocioEntidade.ValidarConfirmacaoPositivaPrimeiroAcesso(emailUsuarioAlteracao,
                                                                                    request, 
                                                                                    out entidadesPossuemUsuario, 
                                                                                    out entidadesPossuemMaster);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Incrementa a quantidade de erros de Confirmação Positva das Entidades informadas
        /// </summary>
        /// <param name="codigoEntidades">Código das Entidades</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <returns>Quantidade de Tentativas já realizadas por entidade</returns>
        public List<Modelo.Entidade> IncrementarQuantidadeConfirmacaoPositivaMultiplasEntidades(out int codigoRetorno, int[] codigoEntidades, string emailUsuarioAlteracao)
        {
            using (Logger Log = Logger.IniciarLog("Incrementa a quantidade de erros de Confirmação Positva da Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidades});
                    Negocio.Entidade negocio = new Negocio.Entidade();
                    var result = negocio.IncrementarQuantidadeConfirmacaoPositivaMultiplasEntidades(out codigoRetorno, codigoEntidades, emailUsuarioAlteracao);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        
        #endregion

        /// <summary>
        /// Consultar Tipo Pessoa do PV na base de dados do GE
        /// </summary>
        public Char ConsultarTipoPessoaPV(out Int32 codigoRetorno, Int32 codigoEntidade) {
            using (Logger Log = Logger.IniciarLog("Consultar Tipo Pessoa  do PV na base de dados do GE")) {
                Log.GravarLog(EventoLog.InicioServico);

                try {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade });
                    var negocio = new Negocio.Entidade();

                    Char tipoPessoa = negocio.ConsultarTipoPessoaPV(out codigoRetorno, codigoEntidade);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, tipoPessoa });

                    return tipoPessoa;
                }
                catch (PortalRedecardException ex) {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex) {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Insere a entidade
        /// </summary>
        /// <param name="entidade"></param>
        /// <returns></returns>
        public int InserirEntidade(Entidade entidade)
        {
            using (Logger Log = Logger.IniciarLog("Inserir Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioEntidade = new Negocio.Entidade();
                    Modelo.Entidade modeloEntidade;

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Servicos.Entidade, Modelo.Entidade>();
                    Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();
                    Mapper.CreateMap<Servicos.TipoEntidade, Modelo.TipoEntidade>();

                    // Convert Business Entity para Data Contract Entity
                    modeloEntidade = Mapper.Map<Servicos.Entidade, Modelo.Entidade>(entidade);

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { modeloEntidade });

                    // Altera Entidade
                    Int32 result = negocioEntidade.InserirEntidade(modeloEntidade);
                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico);


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }

        }
        /// <summary>
        /// Insere entidade GE
        /// </summary>
        /// <param name="entidade"></param>
        /// <returns></returns>
        public int InserirEntidadeGE(Servicos.Entidade entidade)
        {
            using (Logger Log = Logger.IniciarLog("Inserir Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioEntidade = new Negocio.Entidade();
                    Modelo.Entidade modeloEntidade;

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Servicos.Entidade, Modelo.Entidade>();
                    Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                    // Convert Business Entity para Data Contract Entity
                    modeloEntidade = Mapper.Map<Servicos.Entidade, Modelo.Entidade>(entidade);

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { modeloEntidade });

                    // Insere entidade ge
                    Int32 result = negocioEntidade.InserirEntidadeGE(modeloEntidade);
                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico);


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Alerta a entidade
        /// </summary>
        /// <param name="entidade">Instrancia de entidade</param>
        /// <returns></returns>
        public int AlterarEntidade(Entidade entidade)
        {
            using (Logger Log = Logger.IniciarLog("Alterar Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioEntidade = new Negocio.Entidade();
                    Modelo.Entidade modeloEntidade;

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Servicos.Entidade, Modelo.Entidade>();
                    Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();
                    Mapper.CreateMap<Servicos.Status, Modelo.Status>();
                    // Convert Business Entity para Data Contract Entity
                    modeloEntidade = Mapper.Map<Servicos.Entidade, Modelo.Entidade>(entidade);

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { modeloEntidade });

                    // Altera Entidade
                    Int32 result = negocioEntidade.AlterarEntidade(modeloEntidade);
                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico);


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Exclui a entidade 
        /// </summary>
        /// <param name="codigoEntidade">Codigo da entidade </param>
        /// <param name="codigoGrupoEntidade">Codigo do grupo da entidade</param>
        /// <returns></returns>
        public int ExcluirEntidade(int codigoEntidade, int codigoGrupoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Excluir Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioEntidade = new Negocio.Entidade();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, codigoGrupoEntidade });

                    // Altera Entidade
                    Int32 result = negocioEntidade.ExcluirEntidade(codigoEntidade, codigoGrupoEntidade);
                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico);


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Exclui grupo entidade
        /// </summary>
        /// <param name="codigo"></param>
        public Int32 ExcluirGrupo(int codigo)
        {
            using (Logger Log = Logger.IniciarLog("Excluir Grupo Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioGrupoEntidade = new Negocio.GrupoEntidade();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigo });

                    // Exclui grupo Entidade
                    Int32 result = negocioGrupoEntidade.Excluir(codigo);
                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico);


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta EntidadeEC para IZ
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public EntidadeEC ConsultarEntidadeEC(int? codigo, out int codigoRetorno)
        {

            using (Logger Log = Logger.IniciarLog("Consultar EntidadeEC"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {

                    Modelo.EntidadeEC entidadeEC = new Modelo.EntidadeEC();

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.EntidadeEC, Servicos.EntidadeEC>();

                    var negocioDados = new Negocio.Entidade();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigo });
                    entidadeEC = negocioDados.ConsultarEntidadeEC(codigo, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, entidadeEC });

                    var result = Mapper.Map<Modelo.EntidadeEC, Servicos.EntidadeEC>(entidadeEC);
                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entidadeEC"></param>
        /// <returns></returns>
        public int AlterarEntidadeEC(EntidadeEC entidadeEC)
        {

            using (Logger Log = Logger.IniciarLog("Alterar EntidadeEC"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioEntidade = new Negocio.Entidade();
                    Modelo.EntidadeEC modeloEntidade;

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Servicos.EntidadeEC, Modelo.EntidadeEC>();
                    // Convert Business Entity para Data Contract Entity
                    modeloEntidade = Mapper.Map<Servicos.EntidadeEC, Modelo.EntidadeEC>(entidadeEC);

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { modeloEntidade });

                    // Altera Entidade
                    Int32 result = negocioEntidade.AlterarEntidadeEC(modeloEntidade);
                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico);


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Inserir entidadeC
        /// </summary>
        /// <param name="entidadeEC"></param>
        /// <returns></returns>
        public int InserirEntidadeEC(EntidadeEC entidadeEC)
        {
            using (Logger Log = Logger.IniciarLog("Inserir EntidadeEC"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioEntidade = new Negocio.Entidade();
                    Modelo.EntidadeEC modeloEntidade;

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Servicos.EntidadeEC, Modelo.EntidadeEC>();
                    // Convert Business Entity para Data Contract Entity
                    modeloEntidade = Mapper.Map<Servicos.EntidadeEC, Modelo.EntidadeEC>(entidadeEC);

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { modeloEntidade });

                    // Altera Entidade
                    Int32 result = negocioEntidade.InserirEntidadeEC(modeloEntidade);
                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico);


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta o Fornecedor Distribuidor da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="codigoRetorno">Código de Retorno</param>
        /// <returns>Modelo.DistribuidorFornecedor</returns>
        public List<Servicos.DistribuidorFornecedor> ConsultarDistribuidorFornecedor(Int32 codigoEntidade, Int32 codigoGrupoEntidade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Distribuidor Fornecedor"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    List<Modelo.DistribuidorFornecedor> listaFornecedorModelo = new List<Modelo.DistribuidorFornecedor>();
                    List<Servicos.DistribuidorFornecedor> listaFornecedorServico = new List<Servicos.DistribuidorFornecedor>();

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.DistribuidorFornecedor, Servicos.DistribuidorFornecedor>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();

                    var negocioDados = new Negocio.Entidade();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, codigoGrupoEntidade });

                    listaFornecedorModelo = negocioDados.ConsultarDistribuidorFornecedor(codigoEntidade, codigoGrupoEntidade, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, listaFornecedorModelo });

                    Servicos.DistribuidorFornecedor distribuidorServico = null;
                    foreach (Modelo.DistribuidorFornecedor dist in listaFornecedorModelo)
                    {
                        distribuidorServico = new DistribuidorFornecedor();
                        distribuidorServico = Mapper.Map<Modelo.DistribuidorFornecedor, Servicos.DistribuidorFornecedor>(dist);
                        listaFornecedorServico.Add(distribuidorServico);
                    }

                    Log.GravarLog(EventoLog.FimServico, new { listaFornecedorServico });

                    return listaFornecedorServico;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta o Fornecedor da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <returns>Servicos.DistribuidorFornecedor</returns>
        public List<Servicos.DistribuidorFornecedor> ConsultarFornecedor(Int32? codigoEntidade, Int32? codigoGrupoEntidade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Distribuidor Fornecedor"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.DistribuidorFornecedor, Servicos.DistribuidorFornecedor>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();

                    var negocioDados = new Negocio.Entidade();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, codigoGrupoEntidade });

                    List<Modelo.DistribuidorFornecedor> listaFornecedorModelo = negocioDados.ConsultarFornecedor(codigoEntidade, codigoGrupoEntidade, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, fornecedorModelo = listaFornecedorModelo });

                    List<Servicos.DistribuidorFornecedor> listaFornecedores = new List<Servicos.DistribuidorFornecedor>();
                    listaFornecedorModelo.ForEach(delegate(Modelo.DistribuidorFornecedor fornecedor)
                    {
                        listaFornecedores.Add(Mapper.Map<Modelo.DistribuidorFornecedor, Servicos.DistribuidorFornecedor>(fornecedor));
                    });

                    Log.GravarLog(EventoLog.FimServico, new { fornecedor = listaFornecedores });

                    return listaFornecedores;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta o Fornecedor Distribuidor da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="codigoRetorno">Código do Retorno</param>
        /// <returns>Servicos.DistribuidorFornecedor</returns>
        public List<Servicos.DistribuidorFornecedor> ConsultarFornecedorDistribuidor(Int32 codigoEntidade, Int32 codigoGrupoEntidade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Distribuidor Fornecedor"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.DistribuidorFornecedor, Servicos.DistribuidorFornecedor>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();

                    var negocioDados = new Negocio.Entidade();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, codigoGrupoEntidade });

                    List<Modelo.DistribuidorFornecedor> listaFornecedorModelo = null;
                    List<Servicos.DistribuidorFornecedor> listaFornecedorServico = new List<DistribuidorFornecedor>();

                    listaFornecedorModelo = negocioDados.ConsultarFornecedorDistribuidor(codigoEntidade, codigoGrupoEntidade, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, listaFornecedorModelo });

                    Servicos.DistribuidorFornecedor fornecedor = null;
                    foreach (Modelo.DistribuidorFornecedor distForn in listaFornecedorModelo)
                    {
                        fornecedor = new DistribuidorFornecedor();
                        fornecedor = Mapper.Map<Modelo.DistribuidorFornecedor, Servicos.DistribuidorFornecedor>(distForn);
                        listaFornecedorServico.Add(fornecedor);
                    }

                    Log.GravarLog(EventoLog.FimServico, new { listaFornecedorServico });

                    return listaFornecedorServico;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta distribuidores
        /// </summary>
        /// <param name="codigoGrupoEntidadeFornecedor">Código Grupo Entidade do Fornecedor</param>
        /// <param name="codigoEntidadeFornecedor">Código Entidade do Fornecedor</param>
        /// <param name="codigoGrupoEntidadeDistribuidor">Código Grupo Entidade Distribuidor</param>
        /// <param name="codigoEntidadeDistribuidor">Código Entidade Distribuidor</param>
        /// <param name="codigoGrupoFornecedor">Código Grupo Fornecedor</param>
        /// <param name="codigoRetorno">Código retorno</param>
        /// <returns>Lista de distribuidores</returns>
        public List<Servicos.Distribuidor> ConsultarDistribuidores(Int32? codigoGrupoEntidadeFornecedor, Int32? codigoEntidadeFornecedor,
            Int32? codigoGrupoEntidadeDistribuidor, Int32? codigoEntidadeDistribuidor, Int32? codigoGrupoFornecedor, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Distribuidores"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    codigoGrupoEntidadeFornecedor,
                    codigoEntidadeFornecedor,
                    codigoGrupoEntidadeDistribuidor,
                    codigoEntidadeDistribuidor,
                    codigoGrupoFornecedor
                });

                try
                {
                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.Distribuidor, Servicos.Distribuidor>();

                    var negocio = new Negocio.Distribuidor();

                    var listaFornecedorModelo = negocio.Consultar(codigoGrupoEntidadeFornecedor, codigoEntidadeFornecedor,
                        codigoGrupoEntidadeDistribuidor, codigoEntidadeDistribuidor, codigoGrupoFornecedor, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, listaFornecedorModelo });

                    var listaFornecedor = Mapper.Map<List<Modelo.Distribuidor>, List<Servicos.Distribuidor>>(listaFornecedorModelo);

                    Log.GravarLog(EventoLog.FimServico, new { listaFornecedor });

                    return listaFornecedor;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Exclusão de Distribuidor
        /// </summary>
        /// <param name="codigoGrupoEntidadeFornecedor">Código Grupo Entidade do Fornecedor</param>
        /// <param name="codigoEntidadeFornecedor">Código Entidade do Fornecedor</param>
        /// <param name="codigoGrupoEntidadeDistribuidor">Código Grupo Entidade do Distribuidor</param>
        /// <param name="codigoEntidadeDistribuidor">Código Entidade Distribuidor</param>
        /// <returns>Código de retorno</returns>
        public Int32 ExcluirDistribuidor(Int32 codigoGrupoEntidadeFornecedor, Int32 codigoEntidadeFornecedor,
            Int32 codigoGrupoEntidadeDistribuidor, Int32 codigoEntidadeDistribuidor)
        {
            using (Logger Log = Logger.IniciarLog("Excluir Distribuidor"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    codigoGrupoEntidadeFornecedor,
                    codigoEntidadeFornecedor,
                    codigoGrupoEntidadeDistribuidor,
                    codigoEntidadeDistribuidor
                });

                try
                {
                    var negocioDistribuidor = new Negocio.Distribuidor();

                    // Exclui Distribuidor
                    Int32 result = negocioDistribuidor.Excluir(codigoGrupoEntidadeFornecedor, codigoEntidadeFornecedor,
                        codigoGrupoEntidadeDistribuidor, codigoEntidadeDistribuidor);

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Inserção de Distribuidor.
        /// </summary>
        /// <param name="distribuidor">Distribuidor a ser inserido</param>
        /// <param name="codigoEntidadeFornecedor">Código Entidade do Fornecedor</param>
        /// <param name="codigoGrupoEntidadeFornecedor">Código Grupo Entidade do Fornecedor</param>
        /// <returns>Código de retorno</returns>
        public Int32 InserirDistribuidor(Int32 codigoGrupoEntidadeFornecedor, Int32 codigoEntidadeFornecedor,
            Servicos.Distribuidor distribuidor)
        {
            using (Logger Log = Logger.IniciarLog("Inserir Distribuidor"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoGrupoEntidadeFornecedor, codigoEntidadeFornecedor, distribuidor });

                try
                {
                    var negocio = new Negocio.Distribuidor();

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Servicos.Distribuidor, Modelo.Distribuidor>();

                    // Convert Business Entity para Data Contract Entity
                    var modeloDistribuidor = Mapper.Map<Servicos.Distribuidor, Modelo.Distribuidor>(distribuidor);

                    // Insere Distribuidor
                    Int32 result = negocio.Inserir(codigoGrupoEntidadeFornecedor, codigoEntidadeFornecedor, modeloDistribuidor);

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Exclusão de Fornecedor
        /// </summary>
        /// <param name="codigoEntidadeFornecedor">Código Entidade do Fornecedor</param>
        /// <param name="codigoGrupoEntidadeFornecedor">Código Grupo Entidade do Fornecedor</param>
        /// <returns>Código de retorno</returns>
        public Int32 ExcluirFornecedor(Int32 codigoGrupoEntidadeFornecedor, Int32 codigoEntidadeFornecedor)
        {
            using (Logger Log = Logger.IniciarLog("Excluir Fornecedor"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoGrupoEntidadeFornecedor, codigoEntidadeFornecedor });

                try
                {
                    var negocioFornecedor = new Negocio.Fornecedor();

                    // Exclui Fornecedor
                    Int32 result = negocioFornecedor.Excluir(codigoGrupoEntidadeFornecedor, codigoEntidadeFornecedor);

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Inserção de Fornecedor.
        /// </summary>
        /// <param name="fornecedor">Fornecedor</param>
        /// <returns>Código de retorno</returns>
        public Int32 InserirFornecedor(Servicos.Fornecedor fornecedor)
        {
            using (Logger Log = Logger.IniciarLog("Inserir Fornecedor"))
            {
                Log.GravarLog(EventoLog.InicioServico, fornecedor);

                try
                {
                    var negocio = new Negocio.Fornecedor();

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Servicos.Fornecedor, Modelo.Fornecedor>();

                    // Convert Business Entity para Data Contract Entity
                    var modeloFornecedor = Mapper.Map<Servicos.Fornecedor, Modelo.Fornecedor>(fornecedor);

                    // Insere Fornecedor
                    Int32 result = negocio.Inserir(modeloFornecedor);

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consultar Grupo Fornecedor x Ramo de Atividade
        /// </summary>
        /// <param name="codGrupoFornecedor">Código do Grupo de Fornecedor</param>
        /// <param name="codigoRetorno">Código de Retorno</param>
        /// <returns>Associações de Grupo Fornecedor x Ramos de Atividade</returns>
        public List<Servicos.GrupoFornecedorRamoAtividade> ConsultarGrupoFornecedorRamoAtividade(Int32 codGrupoFornecedor, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Associações de Grupo Fornecedor x Ramo"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codGrupoFornecedor });

                try
                {
                    var negocio = new Negocio.GrupoFornecedor();
                    var listaModelo = negocio.ConsultarGrupoFornecedorRamoAtividade(codGrupoFornecedor, out codigoRetorno);

                    Mapper.CreateMap<Modelo.GrupoFornecedorRamoAtividade, Servicos.GrupoFornecedorRamoAtividade>();
                    List<Servicos.GrupoFornecedorRamoAtividade> listaServico =
                        listaModelo == null ? new List<Servicos.GrupoFornecedorRamoAtividade>() :
                        Mapper.Map<List<Modelo.GrupoFornecedorRamoAtividade>, List<Servicos.GrupoFornecedorRamoAtividade>>(listaModelo);

                    Log.GravarLog(EventoLog.FimServico, new { listaServico });

                    return listaServico;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
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
            using (Logger Log = Logger.IniciarLog("Excluir uma associacao Grupo Fornecedor x Ramo de Atividade"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codGrupoFornecedor, codGrupoRamoAtividade, codRamoAtividade, localFlag });

                try
                {
                    var negocio = new Negocio.GrupoFornecedor();

                    // Excluir associações Grupo Fornecedor x Ramo de Atividade
                    Int32 result = negocio.ExcluirGrupoFornecedorRamoAtividade(
                        codGrupoFornecedor, codGrupoRamoAtividade, codRamoAtividade, localFlag);

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Inserção de associação Grupo Fornecedor x Ramo de Atividade
        /// </summary>
        ///<param name="grupoFornecedorXRamo">Dados da associação Grupo Fornecedor x Ramo de Atividade</param>
        /// <param name="localFlag">Flag de exclusão local/dual</param>
        /// <returns>Código de retorno.</returns>
        public Int32 InserirGrupoFornecedorRamoAtividade(Servicos.GrupoFornecedorRamoAtividade grupoFornecedorXRamo, Int32? localFlag)
        {
            using (Logger Log = Logger.IniciarLog("Inserir uma associacao Grupo Fornecedor x Ramo de Atividade"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { grupoFornecedorXRamo, localFlag });

                try
                {
                    var negocio = new Negocio.GrupoFornecedor();

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Servicos.GrupoFornecedorRamoAtividade, Modelo.GrupoFornecedorRamoAtividade>();

                    // Convert Business Entity para Data Contract Entity
                    var modelo = Mapper.Map<Servicos.GrupoFornecedorRamoAtividade, Modelo.GrupoFornecedorRamoAtividade>(grupoFornecedorXRamo);

                    // Insere associação Grupo Fornecedor x Ramo de Atividade
                    Int32 result = negocio.InserirGrupoFornecedorRamoAtividade(modelo, localFlag);

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta os Ramos de Atividade
        /// </summary>
        /// <param name="codGrupoRamo">Código do grupo ramo</param>
        /// <param name="codRamoAtividade">Código do ramo atividade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Ramos de Atividades</returns>
        public List<Servicos.RamoAtividade> ConsultarRamosAtividade(Int32? codGrupoRamo, Int32? codRamoAtividade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Ramos de Atividade"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codGrupoRamo, codRamoAtividade });

                //variável de lista de retorno
                List<RamoAtividade> listaServico = new List<RamoAtividade>();

                try
                {
                    var negocio = new Negocio.RamoAtividade();
                    var listaModelo = negocio.Consultar(codGrupoRamo, codRamoAtividade, out codigoRetorno);

                    if (codigoRetorno == 0)
                    {
                        Mapper.CreateMap<Modelo.RamoAtividade, RamoAtividade>();
                        listaServico = Mapper.Map<List<Modelo.RamoAtividade>, List<RamoAtividade>>(listaModelo);
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.FimServico, new { listaServico, codigoRetorno });
                return listaServico;
            }
        }

        /// <summary>
        /// Consulta as novas entidades credenciadas ou recadastradas num determinado período
        /// </summary>
        /// <param name="dataInicio"></param>
        /// <param name="dataFim"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns>Listagem de Modelo.Entidade com Entidades credenciadas</returns>
        public List<Servicos.Entidade> ConsultarCredenciamentos(DateTime dataInicio, DateTime dataFim, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta as novas entidades credenciadas ou recadastradas num determinado período"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { dataInicio, dataFim });
                Log.GravarLog(EventoLog.ChamadaServico, new { dataInicio, dataFim });

                List<Servicos.Entidade> entidadeServico = new List<Entidade>();

                try
                {
                    var negocio = new Negocio.Entidade();
                    var listaModelo = negocio.ConsultarCredenciamentos(dataInicio, dataFim, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoServico, new { codigoRetorno, listaModelo });

                    if (codigoRetorno == 0)
                    {
                        Mapper.CreateMap<Modelo.Entidade, Entidade>();
                        Mapper.CreateMap<Modelo.GrupoEntidade, GrupoEntidade>();
                        entidadeServico = Mapper.Map<List<Modelo.Entidade>, List<Entidade>>(listaModelo);
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.FimServico, new { codigoRetorno, entidadeServico });
                return entidadeServico;
            }
        }

        /// <summary>
        /// Consulta os Produtos Flex cadastrados para um estabelecimento
        /// </summary>
        /// <param name="codigoCCA">Código da CCA</param>
        /// <param name="codigoFeature">Código da Feature</param>
        /// <param name="codigoEntidade">Código do estabelecimento</param>
        /// <param name="codigoRetorno">Código de retorno da procedure</param>
        /// <returns>Retorna a lista dos Produtos Flex de um estabelecimento</returns>        
        public List<Servicos.ProdutoFlex> ConsultarProdutosFlex(Int32 codigoEntidade, Int32? codigoCCA, Int32? codigoFeature, out Int32 codigoRetorno)
        {
            var retorno = new List<Servicos.ProdutoFlex>();

            using (Logger Log = Logger.IniciarLog("Consultar Produtos Flex"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoEntidade, codigoCCA, codigoFeature });

                try
                {
                    //Consulta os produtos flex
                    List<Modelo.ProdutoFlex> listaDados = new Negocio.DadosBancarios()
                        .ConsultarProdutosFlex(codigoEntidade, codigoCCA, codigoFeature, out codigoRetorno);

                    //Mapeamento de saída
                    Mapper.CreateMap<Modelo.ProdutoFlex, Servicos.ProdutoFlex>();
                    retorno = Mapper.Map<List<Servicos.ProdutoFlex>>(listaDados);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.FimServico, new { retorno });

                return retorno;
            }
        }

        /// <summary>
        /// <para>Validar se o CPNJ/CPF e Código da Entidade existem na base do GE e do PN.</para>
        /// <para>Caso não exista na base do PN, cria a entidade no PN a partir dos dados do GE.</para>
        /// </summary>
        /// <param name="pvs">Códigos das Entidades</param>
        /// <param name="codigoGrupoEntidade"></param>
        /// <returns>Código de retorno
        /// <para>0 - Retorno OK</para>
        /// <para> != 0 - Retorno NOK</para></returns>
        public Int32 ValidarCriarEntidade(List<Int32> pvs, Int32 codigoGrupoEntidade)
        {
            Int32 codigoRetorno = 0;

            using (Logger Log = Logger.IniciarLog("Validar se o CPNJ/CPF e Código da Entidade existem na base do GE"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { pvs, codigoGrupoEntidade });

                try
                {
                    //Chama a validação
                    codigoRetorno = new Negocio.Entidade()
                        .ValidarCriarEntidade(pvs, codigoGrupoEntidade);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.FimServico, new { codigoRetorno });
            }

            return codigoRetorno;
        }

        /// <summary>
        /// Incrementa a quantidade de erros de Confirmação Positva da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <returns>Quantidade de Tentativas já realizadas</returns>
        public Int32 IncrementarQuantidadeConfirmacaoPositiva(Int32 codigoEntidade, Int32 codigoGrupoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Incrementa a quantidade de erros de Confirmação Positva da Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, codigoGrupoEntidade });
                    Negocio.Entidade negocio = new Negocio.Entidade();
                    var result = negocio.IncrementarQuantidadeConfirmacaoPositivaEntidade(codigoEntidade, codigoGrupoEntidade);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Atualiza a quantidade de confirmações positivas inválidas para 0 para a Entidade
        /// especificado nos paramêtros
        /// </summary>
        public Int32 ReiniciarQuantidadeConfirmacaoPositiva(int codigoEntidade, int codigoGrupoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Atualiza a quantidade de confirmações positivas inválidas para 0 para a Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, codigoGrupoEntidade });

                    Negocio.Entidade negocio = new Negocio.Entidade();
                    var result = negocio.ReiniciarQuantidadeConfirmacaoPositivaEntidade(codigoEntidade, codigoGrupoEntidade);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Atualiza a quantidade de confirmações positivas inválidas para 0 para múltiplas Entidades
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Listagem de Entidades</param>
        /// <returns>Retorna dicionário com o código de erro correspondente a cada entidade, ao reiniciar a qtd de tentativas</returns>
        public Dictionary<Int32, Int32> ReiniciarQuantidadeConfirmacaoPositiva(Int32[] codigoEntidades, Int32 codigoGrupoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Atualiza a quantidade de confirmações positivas inválidas para 0 para a Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidades, codigoGrupoEntidade });

                    Negocio.Entidade negocio = new Negocio.Entidade();
                    var result = negocio.ReiniciarQuantidadeConfirmacaoPositivaMultiplasEntidades(codigoEntidades, codigoGrupoEntidade);
                    
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });
                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta entidades associadas a um usuário por e-mail
        /// </summary>
        /// <param name="email">E-mail usuário</param>
        /// <param name="codigoRetorno">Código de Retorno</param>
        /// <returns>Listagem das entidades</returns>
        public List<Modelo.Entidade> ConsultarPorEmail(String email, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta a entidade pelo e-mail do usuário"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { email });

                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();

                    Negocio.Entidade negocioEntidade = new Negocio.Entidade();
                    List<Modelo.Entidade> entidades = negocioEntidade.ConsultarPorEmail(email, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno });

                    return entidades;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta entidades associadas a um usuário por e-mail e senha criptografada Hash
        /// </summary>
        /// <param name="email">E-mail usuário</param>
        /// <param name="senha">Senha Criptografada</param>
        /// <param name="codigoRetorno">Código de Retorno</param>
        /// <returns>Listagem das entidades</returns>
        public List<Modelo.Entidade> ConsultarPorEmailSenhaHash(String email, String senha, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta a entidade pelo e-mail do usuário"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { email });

                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();

                    Negocio.Entidade negocioEntidade = new Negocio.Entidade();
                    List<Modelo.Entidade> entidades = negocioEntidade.ConsultarPorEmailSenhaHash(email, senha, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno });

                    return entidades;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta entidades associadas a um usuário por CPF
        /// </summary>
        /// <param name="cpf">E-mail usuário</param>
        /// <param name="codigoRetorno">Código de Retorno</param>
        /// <returns>Listagem das entidades</returns>
        public List<Modelo.Entidade> ConsultarPorCpf(Int64 cpf, out Int32 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Consulta a entidade pelo CPF do usuário"))
            {
                log.GravarLog(EventoLog.InicioServico, cpf);

                try
                {
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();

                    Negocio.Entidade negocioEntidade = new Negocio.Entidade();
                    List<Modelo.Entidade> entidades = negocioEntidade.ConsultarPorCpf(cpf, out codigoRetorno);

                    return entidades;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Verifica, na listagem de PVs, se algum tem Komerci
        /// </summary>
        /// <param name="pvs">Lista de PVs</param>
        /// <returns>Se algum PV possui Komerci</returns>
        public Boolean PossuiKomerci(List<Int32> pvs)
        {
            Boolean possuiKomerci = false;

            using (Logger Log = Logger.IniciarLog("Verifica se PVs possuem Komerci"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, pvs);
                    possuiKomerci = new Negocio.Entidade().PossuiKomerci(pvs);
                    Log.GravarLog(EventoLog.RetornoNegocio, possuiKomerci);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }

            return possuiKomerci;
        }

        /// <summary>
        /// Valida se a Entidade já possui algum Usuário Master
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="possuiMaster">
        /// <para>True - Se a Entidade possuir Usuário Master</para>
        /// <para>False - Se a Entidade não possuir Usuário Master</para>
        /// </param>
        /// <param name="possuiUsuario">
        /// <para>True - Se a Entidade possuir Usuário</para>
        /// <para>False - Se a Entidade não possuir Usuário</para>
        /// </param>
        /// <param name="possuiSenhaTemporaria">
        /// <para>True - Se a Entidade possuir Usuário com senha Temporária</para>
        /// <para>False - Se a Entidade não possuir Usuário com senha Temporária</para>
        /// </param>
        /// <returns>
        /// Código de retorno da consulta
        /// </returns>
        public Int32 PossuiUsuario(Int32 codigoEntidade, Int32 codigoGrupoEntidade, 
                                    out Boolean possuiUsuario, 
                                    out Boolean possuiMaster, 
                                    out Boolean possuiSenhaTemporaria)
        {
            using (Logger log = Logger.IniciarLog("Valida se a Entidade já possui algum Usuário Master"))
            {
                log.GravarLog(EventoLog.InicioServico);

                try
                {
                    log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, codigoGrupoEntidade });

                    Negocio.Entidade negocio = new Negocio.Entidade();
                    Int32 codigoRetorno = negocio.PossuiUsuario(codigoEntidade, codigoGrupoEntidade, out possuiUsuario, out possuiMaster, out possuiSenhaTemporaria);

                    log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, possuiUsuario, possuiMaster, possuiSenhaTemporaria });

                    log.GravarLog(EventoLog.FimServico, new { codigoRetorno, possuiUsuario, possuiMaster, possuiSenhaTemporaria });
                    
                    return codigoRetorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Verificar se o PV informado possui usuários com a flag de Legado
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo Entidade</param>
        /// <returns>
        /// <para>True - Possui Usuário Legado</para>
        /// <para>False - Não possui Usuário Legado</para>
        /// </returns>
        public Boolean PossuiUsuarioLegado(Int32 codigoEntidade, Int32 codigoGrupoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Verificar se o PV informado possui usuários com a flag de Legado"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, codigoGrupoEntidade });

                    Negocio.Entidade negocio = new Negocio.Entidade();

                    Boolean possuiUsuarioLegado = negocio.PossuiUsuarioLegado(codigoEntidade, codigoGrupoEntidade);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoEntidade, codigoGrupoEntidade, possuiUsuarioLegado });

                    Log.GravarLog(EventoLog.FimServico, new { possuiUsuarioLegado });

                    return possuiUsuarioLegado;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta os usuários de um PV, filtrando-os pelo perfil
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código grupo entidade</param>
        /// <param name="tipoUsuario">
        /// <para>Tipo do Usuário:</para>
        /// <para> - "M": Master</para>
        /// <para> - "B": Básico</para>
        /// <para> - "P": Personalizado</para>
        /// </param>
        /// <param name="codigoRetorno">Código retorno da consulta</param>
        /// <returns>Usuários do PV com o Perfil solicitado</returns>
        public List<Servicos.Usuario> ConsultarUsuariosPorPerfil(
            Int32 codigoEntidade, Int32 codigoGrupoEntidade, Char tipoUsuario, out Int32 codigoRetorno)
        {
            var usuarios = default(List<Servicos.Usuario>);

            using (Logger Log = Logger.IniciarLog("Consulta os usuário do Estabelecimento com determinado perfil"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { codigoEntidade, codigoGrupoEntidade, tipoUsuario });
                    
                    var negocio = new Negocio.Entidade();

                    //Gera filtro por Perfil
                    Func<Modelo.Usuario, Boolean> filtro = 
                        (usuario) => String.Compare(usuario.TipoUsuario, tipoUsuario.ToString(), true) == 0;

                    //Consulta os usuários pelo Perfil
                    List<Modelo.Usuario> usuariosModelo = negocio.ConsultarUsuarios(
                        codigoEntidade, codigoGrupoEntidade, filtro, out codigoRetorno);

                    Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                    Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                    Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                    Mapper.CreateMap<Modelo.PaginaMenu, Servicos.PaginaMenu>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                    //Mapeamento do retorno
                    usuarios = Mapper.Map<List<Servicos.Usuario>>(usuariosModelo);

                    Log.GravarLog(EventoLog.FimServico, new { codigoRetorno, usuarios });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }            

            return usuarios;
        }

        /// <summary>
        /// Consulta os usuários de um PV, filtrando-os pelo Status
        /// </summary>
        /// <param name="status">Status dos usuários que serão retornados</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código grupo entidade</param>
        /// <param name="codigoRetorno">Código retorno da consulta</param>
        /// <returns>Usuários do PV com o Status solicitado</returns>
        public List<Servicos.Usuario> ConsultarUsuariosPorStatus(
            Int32 codigoEntidade, Int32 codigoGrupoEntidade, Comum.Enumerador.Status status, out Int32 codigoRetorno)
        {
            var usuarios = default(List<Servicos.Usuario>);

            using (Logger Log = Logger.IniciarLog("Consulta os usuário do Estabelecimento que estão com determinado status"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { codigoEntidade, codigoGrupoEntidade, status });

                    //Gera filtro por Status
                    Func<Modelo.Usuario, Boolean> filtro = 
                        (usuario) => usuario.Status != null && usuario.Status.Codigo == (Int32) status;

                    //Consulta os usuários pelo Perfil
                    var negocio = new Negocio.Entidade();
                    List<Modelo.Usuario> usuariosModelo = negocio.ConsultarUsuarios(
                        codigoEntidade, codigoGrupoEntidade, filtro, out codigoRetorno);

                    Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                    Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                    Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                    Mapper.CreateMap<Modelo.PaginaMenu, Servicos.PaginaMenu>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                    //Mapeamento do retorno
                    usuarios = Mapper.Map<List<Servicos.Usuario>>(usuariosModelo);

                    Log.GravarLog(EventoLog.FimServico, new { codigoRetorno, usuarios });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }

            return usuarios;
        }

        /// <summary>
        /// Consulta se o PV está no programa de fidelização
        /// </summary>
        /// <param name="numeroPv">Número do PV</param>
        /// <returns></returns>
        public String ConsultarPVFidelidade(Int32 numeroPv)
        {
            String retorno = String.Empty;

            using (Logger Log = Logger.IniciarLog("Consultar fidelidade PV"))
            {
                try
                {
                    Negocio.Entidade entidadeNegocio = new Negocio.Entidade();

                    retorno = entidadeNegocio.ConsultarPVFidelidade(numeroPv);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.FimServico, new { retorno });

                return retorno;
            }
        }

        #region .ConsultarDadosBancarios.
        /// <summary>
        /// Consulta os dados bancários de Crédito ou Débito
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <returns>Retorna a lista de Dados Bancários</returns>
        public Servicos.DadosBancarios ConsultarDadosBancariosCadastro(Int32 codigoEntidade, out Int32 codigoRetorno) {
            var dados = new Servicos.DadosBancarios();

            var listaDados = new Modelo.DadosBancarios();

            using (Logger Log = Logger.IniciarLog("Consulta os dados bancários de Crédito ou Débito")) {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioDados = new Negocio.DadosBancarios();

                    Mapper.CreateMap<Modelo.DadosBancarios, Servicos.DadosBancarios>();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade });

                    listaDados = negocioDados.ConsultarDadosBancarios(codigoEntidade, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno });

                    Mapper.Map<Modelo.DadosBancarios, Servicos.DadosBancarios>(listaDados, dados);

                    Log.GravarLog(EventoLog.FimServico, new { listaDados });

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                return dados;
            }
        }
        #endregion

        /// <summary>
        /// Verifica se os estabelecimentos estão filiados à Rede.
        /// </summary>
        /// <param name="numerosCgcCpf">Lista contendo o número de CNPJ/CPF a serem verificados</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Dicionário contendo o número do CNPJ/CPF como chave, e o Status do estabelecimento, como valor.
        /// C: NÃO - CANCELADO; S: SIM, N: NÃO
        /// </returns>
        public Dictionary<Int64, String> ListarEstabelecimentosFiliados(List<Int64> numerosCgcCpf, out Int32 codigoRetorno)
        {
            var retorno = default(Dictionary<Int64, String>);
            codigoRetorno = 0;

            using (Logger Log = Logger.IniciarLog("Consultar fidelidade PV"))
            {
                try
                {
                    Negocio.Entidade entidadeNegocio = new Negocio.Entidade();
                    retorno = entidadeNegocio.ListarEstabelecimentosFiliados(numerosCgcCpf, out codigoRetorno);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.FimServico, new { retorno, codigoRetorno });

                return retorno;
            }
        }

        /// <summary>
        /// Lista os métodos que 
        /// </summary>
        /// <param name="dllPath">Caminho</param>
        /// <param name="className">Nome do método</param>
        /// <returns></returns>
        public List<string> ListarMetodos(string dllPath, string className)
        {   
            string item;
            List<string> list = new List<string>();
            var assembly = Assembly.LoadFrom(dllPath);
            var type = assembly.GetType(className);

            foreach (var method in type.GetMethods())
            {
                var parameters = method.GetParameters();
                var parameterDescriptions = string.Join
                    (", ", method.GetParameters()
                                 .Select(x => x.ParameterType + " " + x.Name)
                                 .ToArray());

                item = string.Format("{0} {1} ({2})",
                                     method.ReturnType,
                                     method.Name,
                                     parameterDescriptions);

                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Obtem o numero da versão
        /// </summary>
        /// <returns></returns>
        public int GetNumVersion()
        {
            return 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigoEntidade"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public bool ValidarPossuiEAdquirencia(int codigoEntidade, out int codigoRetorno) {
            using (Logger log = Logger.IniciarLog(
                String.Format("Validar se o estabelecimento {0} possui o serviço de EAdquirência contratado", codigoEntidade))) {
                log.GravarLog(EventoLog.InicioServico);
                try {
                    Negocio.Entidade negocio = new Negocio.Entidade();
                    return negocio.ValidarPossuiEadquirencia(codigoEntidade, out codigoRetorno);
                }
                catch (PortalRedecardException ex) {
                    log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex) {
                    log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Retorna informações de condições comerciais caso não haja aceite realizado
        /// </summary>
        /// <param name="numeroPv"></param>
        /// <returns></returns>
        public Servicos.ResponseBaseItem<Modelo.InformacaoComercial.InformacaoComercial> ConsultarInformacaoComercial(Int64 numeroPv)
        {
            ResponseBaseItem<Modelo.InformacaoComercial.InformacaoComercial> retorno = new ResponseBaseItem<Modelo.InformacaoComercial.InformacaoComercial>();

            using (Logger Log = Logger.IniciarLog("Consultar Informacao Comercial"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    retorno.Item = new Negocio.InformacoesComerciais().Consultar(numeroPv);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }

                Log.GravarLog(EventoLog.FimServico, new { retorno });

                return retorno;
            }
        }

        /// <summary>
        /// Retorna informações de condições comerciais
        /// </summary>
        /// <param name="numeroPv"></param>
        /// <returns></returns>
        public Servicos.ResponseBaseItem<Modelo.InformacaoComercial.InformacaoComercial> RecuperarInformacaoComercial(Int64 numeroPv)
        {
            ResponseBaseItem<Modelo.InformacaoComercial.InformacaoComercial> retorno = new ResponseBaseItem<Modelo.InformacaoComercial.InformacaoComercial>();

            using (Logger Log = Logger.IniciarLog("Recuperar Informacao Comercial"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    retorno.Item = new Negocio.InformacoesComerciais().Recuperar(numeroPv);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }

                Log.GravarLog(EventoLog.FimServico, new { retorno });

                return retorno;
            }
        }

        /// <summary>
        /// Altera o status do aceite de condições comerciais
        /// </summary>
        /// <param name="codigoUsuario"></param>
        /// <param name="numeroPv"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public BaseResponse AlterarStatusInformacaoComercial(Int64 codigoUsuario, Int64 numeroPv, String status)
        {
            BaseResponse retorno = new BaseResponse();

            using (Logger Log = Logger.IniciarLog("Alterar Status Aceite de Informacao Comercial"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    new Negocio.InformacoesComerciais().AlterarStatus(codigoUsuario, numeroPv, status);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }

                Log.GravarLog(EventoLog.FimServico, new { retorno });

                return retorno;
            }
        }
    }
}
