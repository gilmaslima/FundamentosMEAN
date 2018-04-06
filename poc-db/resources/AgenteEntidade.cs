using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Rede.PN.ApiLogin.Agente.Core.Wcf;
using Redecard.PN.Comum;

namespace Rede.PN.ApiLogin.Agente
{
    public class AgenteEntidade
    {
        /// <summary>
        /// Listar os estabelecimentos relacionados ao usuário através do WCF atual do Portal
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public List<Modelo.Entidade> ListarEstabelecimentosLogin(Modelo.Usuario usuario, out Int32 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Listar os estabelecimentos relacionados ao usuário através do WCF atual do Portal"))
            {
                try
                {
                    log.GravarLog(EventoLog.ChamadaAgente, usuario);

                    List<Modelo.Entidade> entidadesModelo = new List<Modelo.Entidade>();

                    //Chamando serviço WCF atual
                    using (var contexto = new ContextoWcf<EntidadeServico.EntidadeServicoClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaAgente, new { usuario.Email, usuario.Senha });

                        var entidades = contexto.Cliente.ConsultarPorEmailSenhaHash(out codigoRetorno, usuario.Email, usuario.Senha);

                        log.GravarLog(EventoLog.RetornoAgente, new { entidades, codigoRetorno });

                        foreach (EntidadeServico.Entidade1 entidade in entidades)
                        {
                            entidadesModelo.Add(new Modelo.Entidade(entidade.Codigo, 1, entidade.Descricao));
                        }
                    }

                    log.GravarLog(EventoLog.RetornoAgente, entidadesModelo);

                    return entidadesModelo;
                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Consultar os dados do Estabelecimento no GE
        /// </summary>
        /// <param name="entidade"></param>
        /// <returns></returns>
        public Modelo.Entidade ObterEstabecimento(Modelo.Entidade entidade)
        {
            Modelo.Entidade estabelecimentoModelo = default(Modelo.Entidade);
            Int32 codigoRetorno = default(Int32);

            using (Logger log = Logger.IniciarLog("Consultar os dados do Estabelecimento no GE através do WCF do Portal"))
            {
                try
                {
                    log.GravarLog(EventoLog.ChamadaAgente, entidade);

                    //Chamando serviço WCF atual
                    using (var contexto = new ContextoWcf<EntidadeServico.EntidadeServicoClient>())
                    {
                        var estabelecimento = contexto.Cliente.ConsultarDadosPV(out codigoRetorno, entidade.Codigo);

                        if (estabelecimento != null && codigoRetorno == 0)
                        {
                            estabelecimentoModelo = new Modelo.Entidade();

                            estabelecimentoModelo.Codigo = estabelecimento.Codigo;
                            estabelecimentoModelo.NomeEntidade = estabelecimento.Descricao;
                            estabelecimentoModelo.IndicadorDataCash = String.IsNullOrEmpty(estabelecimento.IndicadorDataCash) ? String.Empty : estabelecimento.IndicadorDataCash;
                            estabelecimentoModelo.DataAtivacaoDataCash = estabelecimento.DataAtivacaoDataCash;
                            estabelecimentoModelo.GrupoEntidade = 1;
                            estabelecimentoModelo.CNPJEntidade = estabelecimento.CNPJEntidade;
                        }

                    }

                    log.GravarLog(EventoLog.ChamadaAgente, new { estabelecimentoModelo, codigoRetorno });

                    return estabelecimentoModelo;
                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Consultar a tecnologia do Estabelecimento no TG
        /// </summary>
        /// <param name="entidade"></param>
        /// <returns></returns>
        public Int32 ObterTecnologia(Modelo.Entidade entidade)
        {
            Int32 codigoTecnologia = default(Int32);
            Int32 codigoRetorno = default(Int32);

            using (Logger log = Logger.IniciarLog("Consultar a tecnologia do Estabelecimento no TG"))
            {
                try
                {
                    log.GravarLog(EventoLog.ChamadaAgente, entidade);

                    //Chamando serviço WCF atual
                    using (var contexto = new ContextoWcf<EntidadeServico.EntidadeServicoClient>())
                    {
                        codigoTecnologia = contexto.Cliente.ConsultarTecnologiaEstabelecimento(out codigoRetorno, entidade.Codigo);
                    }

                    log.GravarLog(EventoLog.RetornoAgente, codigoTecnologia);

                    return codigoTecnologia;
                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
            }
        }
    }
}
