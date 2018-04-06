using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.OutrasEntidades;
using Redecard.PN.Comum;
using Redecard.PN.OutrasEntidades.Modelo;
using Redecard.PN.OutrasEntidades.Dados;

namespace Redecard.PN.OutrasEntidades.Negocio
{
    public class NegocioISLogin : RegraDeNegocioBase
    {

        /// <summary>
        /// Consulta login KO usuários do sistema Komerci
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="senhaUsuario">Senha do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Código de retorno</returns>
        public void AutenticarUsuarioKO(Int32 codigoEntidade, String codigoUsuario, String senhaUsuario, out Int32 codigoRetorno, out String mensagemRetorno, bool senhaCriptografada)
        {
            using (var log = Logger.IniciarLog("Autenticar Usuário KO (Komerci) - Utilizado na Migração do IS"))
            {
                log.GravarLog(EventoLog.InicioNegocio);
                try
                {

                    if (codigoEntidade.Equals(0))
                    {
                        codigoRetorno = 9;
                        mensagemRetorno = "Estabelecimento nao preenchido.";
                    }
                    else if (String.IsNullOrEmpty(codigoUsuario))
                    {
                        codigoRetorno = 10;
                        mensagemRetorno = "Codigo do usuario nao preenchido.";
                    }
                    else if (String.IsNullOrEmpty(senhaUsuario))
                    {
                        codigoRetorno = 11;
                        mensagemRetorno = "Senha nao preenchida.";
                    }
                    else
                    {
                        //Se senha não criptografada, será criptografada agora usando SHA1 (Padrão do wsLogin também - verificado)
                        if (!senhaCriptografada)
                            senhaUsuario = EncriptadorSHA1.EncryptString(senhaUsuario);

                        #if DEBUG
                            codigoRetorno = 0;
                            mensagemRetorno = "OK";
                        #else

                        Dados.DadosISLogin usuario = new Dados.DadosISLogin();
                        usuario.AutenticarUsuarioKO(codigoEntidade, codigoUsuario, senhaUsuario, out codigoRetorno, out mensagemRetorno);

                        #endif

                        switch (codigoRetorno)
                        {
                            case 0:
                                mensagemRetorno = "OK";
                                break;

                            case 1:
                                mensagemRetorno = "Por favor, tente novamente ou entre em contato com um usuário Master.";
                                break;

                            case 2:
                            case 3:
                                mensagemRetorno = "Os dados não conferem. Verifique  novamente o número do estabelecimento, usuário e código de acesso. Verifique se a tecla CAPS LOCK está acionada pois o sistema diferencia letras maiúsculas de minúsculas.";
                                break;

                            case 4:
                                mensagemRetorno = "Os dados não conferem. Verifique  novamente o número do estabelecimento, usuário e código de acesso. Verifique se a tecla CAPS LOCK está acionada pois o sistema diferencia letras maiúsculas de minúsculas.";
                                break;

                            case 5:
                            case 6:
                                mensagemRetorno = "Por favor, tente novamente ou entre em contato com um usuário Master.";
                                break;

                            case 7:
                                mensagemRetorno = "Seu estabelecimento já está cadastrado.";
                                break;

                            case 8:
                                mensagemRetorno = "Problemas no processamento.";
                                break;

                            default:
                                mensagemRetorno = "Sistema Inválido";
                                break;
                        }
                    }

                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, String.Concat(FONTE, ".OutrasEntidades"), ex);
                }
            }
        }


    }
}
