using Rede.PN.ApiLogin.Agente.Core.Wcf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;

namespace Rede.PN.ApiLogin.Agente
{
    /// <summary>
    /// Classe Agente de acesso aos serviços 
    /// </summary>
    public class AgenteUsuario
    {
        /// <summary>
        /// Obter o modelo de dados do Usuário passado como parâmetro
        /// </summary>
        /// <param name="usuario">Usuário</param>
        /// <param name="codigoRetorno">Código de retorno do serviço</param>
        /// <returns></returns>
        public Modelo.Usuario ObterUsuarioLogin(Modelo.Usuario usuario, out Int32 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Obter o modelo de dados do Usuário passado como parâmetro"))
            {
                try
                {
                    log.GravarLog(EventoLog.ChamadaAgente, usuario);

                    Modelo.Usuario usuarioModelo = default(Modelo.Usuario);

                    //Chamando serviço WCF atual
                    using (var contexto = new ContextoWcf<UsuarioServico.UsuarioServicoClient>())
                    {
                        //codigoNomeUsuario
                        var usuarios = contexto.Cliente.ConsultarPorCodigoEntidade(out codigoRetorno, usuario.Email,
                            new UsuarioServico.Entidade()
                            {
                                Codigo = usuario.Entidade.Codigo,
                                GrupoEntidade = new UsuarioServico.GrupoEntidade() { Codigo = usuario.Entidade.GrupoEntidade }
                            }
                        );

                        if (usuarios != null && usuarios.Count > 0)
                        {
                            usuarioModelo = new Modelo.Usuario();
                            usuarioModelo.Email = usuarios[0].Email;
                            usuarioModelo.QuantidadeTentativaLoginIncorreta = usuarios[0].QuantidadeTentativaLoginIncorreta;
                            usuarioModelo.QuantidadeTentativaConfirmacaoPositiva = usuarios[0].QuantidadeTentativaConfirmacaoPositiva;
                            usuarioModelo.Nome = usuarios[0].Descricao;
                            usuarioModelo.Senha = usuarios[0].Senha;
                            usuarioModelo.TipoUsuario = usuarios[0].TipoUsuario;
                            usuarioModelo.CodigoIdUsuario = usuarios[0].CodigoIdUsuario;
                            usuarioModelo.Status = new Modelo.Status() { Codigo = usuarios[0].Status.Codigo };
                            usuarioModelo.DataUltimoAcesso = usuarios[0].DataUltimoAcesso;
                            usuarioModelo.Entidade = new Modelo.Entidade(usuario.Entidade.Codigo, usuario.Entidade.GrupoEntidade);
                        }

                    }

                    log.GravarLog(EventoLog.RetornoNegocio, new { usuarioModelo, codigoRetorno });

                    return usuarioModelo;
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
        /// Obtém o código de retorno da validação do login do usuário
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public Int32 ObterCodigoValidacaoLogin(Modelo.Usuario usuario)
        {
            using (Logger log = Logger.IniciarLog("Obtém o código de retorno da validação do login do usuário"))
            {
                try
                {
                    log.GravarLog(EventoLog.ChamadaAgente, usuario);

                    Int32 codigoValidacao = -1;

                    //Chamando serviço WCF atual
                    using (var contexto = new ContextoWcf<UsuarioServico.UsuarioServicoClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaAgente, new {
                            usuario.Entidade.GrupoEntidade,
                            usuario.Entidade.Codigo,
                            usuario.Email,
                            usuario.Senha,
                            usuario.Entidade.PossuiVendaOnline
                        });

                        codigoValidacao = contexto.Cliente.Validar(usuario.Entidade.GrupoEntidade, usuario.Entidade.Codigo, usuario.Email, usuario.Senha, usuario.Entidade.PossuiVendaOnline);
                        
                    }

                    log.GravarLog(EventoLog.RetornoAgente, codigoValidacao);

                    return codigoValidacao;

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
        /// Obter o usuário a partir do seu ID
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public Modelo.Usuario ObterUsuarioLogin(int idUsuario, out int codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Obter o usuário a partir do seu ID"))
            {
                try
                {
                    log.GravarLog(EventoLog.ChamadaAgente, idUsuario);

                    Modelo.Usuario usuarioModelo = default(Modelo.Usuario);

                    //Chamando serviço WCF atual
                    using (var contexto = new ContextoWcf<UsuarioServico.UsuarioServicoClient>())
                    {
                        //codigoNomeUsuario
                        var usuario = contexto.Cliente.ConsultarPorID(out codigoRetorno, idUsuario);

                        if (usuario != null)
                        {
                            usuarioModelo = new Modelo.Usuario();
                            usuarioModelo.Email = usuario.Email;
                            usuarioModelo.Nome = usuario.Descricao;
                            usuarioModelo.Senha = usuario.Senha;
                            usuarioModelo.CodigoIdUsuario = usuario.CodigoIdUsuario;
                            usuarioModelo.Entidade = null;
                        }

                    }

                    log.GravarLog(EventoLog.RetornoNegocio, new { usuarioModelo, codigoRetorno });

                    return usuarioModelo;
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
