using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario;
using Redecard.PNCadastrais.Core.Web.Controles.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using ControlTemplates = Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais.Handlers.Usuario
{
    /// <summary>
    /// Generic handler para recuperacao de usuario
    /// </summary>
    public class RecuperacaoUsuario : IHttpHandler, IRequiresSessionState
    {
        /// <summary>
        /// Marca handler como reusável
        /// </summary>
        public bool IsReusable { get { return true; } }

        /// <summary>
        /// Método para processar o Request Ajax.
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ConsultaPvsHandlerResponse model;

            using (Logger log = Logger.IniciarLog("RecuperacaoUsuario.ashx - ProcessRequest"))
            {
                try
                {
                    String 
                        metodoConsulta = context.Request.Params["metodoConsulta"],
                        captchaResponse = context.Request.Params["captchaResponse"];

                    // incrementa o contador do Recaptcha
                    ControlTemplates.RecaptchaGoogle.IncrementTentativas(context, true);

                    // valida o captcha response
                    if (!ControlTemplates.RecaptchaGoogle.ValidateCaptcha(context, captchaResponse, true))
                    {
                        model = UsuarioNegocio.ConstruirMensagemErro(500, "Código (captcha) inválido.");
                    }
                    else if (String.Compare(metodoConsulta, "ObterEmailUsuario", true) == 0)
                    {
                        log.GravarMensagem("recuperando dados do usuario");
                        model = RecuperarUsuario(context);
                    }
                    else
                    {
                        model = UsuarioNegocio.ConstruirMensagemAviso(500, "Método de consulta inválido. Utilizar 'ObterEmailUsuario'");
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    GravarParametrosLog(context, log);
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    model = UsuarioNegocio.ConstruirMensagemErro(500, ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (HttpException ex)
                {
                    GravarParametrosLog(context, log);
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    model = UsuarioNegocio.ConstruirMensagemErro(500, "RecuperacaoUsuario.ashx", ex.ErrorCode);
                }
                catch (Exception ex)
                {
                    GravarParametrosLog(context, log);
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    model = UsuarioNegocio.ConstruirMensagemErro(500, "Sistema indisponível (-1).", String.Format("{0} - {1}", ex.Message, ex.StackTrace));
                }
            }

            model.ExibirRecaptcha = UsuarioNegocio.ExibirRecaptcha(context);
            context.Response.StatusCode = model.StatusCode != 0 ? model.StatusCode : 200;
            context.Response.Write(serializer.Serialize(model));
        }

        /// <summary>
        /// Caso ocorra uma excecao, salva os dados no LOG
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        private void GravarParametrosLog(HttpContext context, Logger log)
        {
            StringBuilder parametros = new StringBuilder();
            foreach (String qsItem in context.Request.Params.AllKeys)
            {
                parametros.AppendFormat("{0}: {1} {2}", qsItem, context.Request.Params[qsItem], Environment.NewLine);
            }

            log.GravarErro(new Exception(parametros.ToString()));
        }

        /// <summary>
        /// Método para Retornar os PV's no fluxo de recuperação de Usuário.
        /// </summary>
        /// <param name="context">Contexto HTTP</param>
        private ConsultaPvsHandlerResponse RecuperarUsuario(HttpContext context)
        {
            ConsultaPvsHandlerResponse model;

            String cpf = context.Request.Params["cpf"];

            //Se preenchido e número inválido, erro
            if (!CampoCpfCnpjValidator.ValidateCpf(cpf))
            {
                return UsuarioNegocio.ConstruirMensagemAviso(500, "CPF inválido ou não informado.", true, "CPF inválido ou não informado");
            }

            Int64 cpfNumerico = new String(cpf.Where(c => Char.IsNumber(c)).ToArray()).ToInt64();

            //Se o status do usuário não for válido, retorna.
            if (!UsuarioNegocio.ValidarStatusUsuario(cpfNumerico, String.Empty, out model))
            {
                return model;
            }

            var usuario = UsuarioNegocio.ObterUsuario(cpfNumerico, String.Empty);
            if (usuario == null)
            {
                model = UsuarioNegocio.ConstruirMensagemAviso(500, "Usuário não encontrado", true, "Usuário não encontrado");
                return model;
            }

            if (UsuarioNegocio.ValidarUsuario(usuario, out model))
            {
                //A lista de PV's ainda não estará visível, então limpa os dados de informação do usuário.
                InformacaoUsuario.Limpar();

                //Obtém os PV's para exibir a lista.
                List<ConsultaPvsHandlerModel> pvs = UsuarioNegocio.ObterPvs(usuario.CPF.Value, usuario.Email);

                //Caso não sejam retornados pv's, retorne.
                if (pvs == null || pvs.Count == 0)
                {
                    return UsuarioNegocio.ConstruirMensagemAviso(500, "Não existe usuário para o CPF informado.",true, "Usuário inexistente para o CPF informado");
                }

                //Salva os pvs no cache
                InformacaoUsuario usuarioCache = InformacaoUsuario.Recuperar();
                usuarioCache.PvsSelecionados = pvs.Select(p => p.NumeroPv).ToArray();

                InformacaoUsuario.Salvar(usuarioCache);

                //Registra no histórico/log de atividades
                UsuarioNegocio.HistoricoRecuperacaoUsuario(usuarioCache.IdUsuario,
                                            usuarioCache.NomeCompleto,
                                            usuarioCache.EmailUsuario,
                                            usuarioCache.TipoUsuario,
                                            usuarioCache.PvsSelecionados);

                // Atualiza na tabela tbpn003 o status do usuario para 11
                UsuarioNegocio.AtualizarStatusPosPendente(usuarioCache);

                foreach (var pv in pvs)
                {
                    pv.Email = Util.TruncarEmailUsuario(pv.Email.Trim());
                    pv.NumeroPvMascarado = Util.TruncarNumeroPV(pv.NumeroPv.ToString());
                    pv.NumeroPv = 0;
                }

//#if DEBUG
//                if (pvs.Count > 1)
//                {
//                    for (int i = 1; i < 100; i++)
//                    {
//                        pvs.Add(new ConsultaPvsHandlerModel
//                        {
//                            Email = "teste " + i,
//                            NumeroPvMascarado = i + "",
//                            NomeEstabelecimento = "nome " + i
//                        });
//                    }
//                }
//#endif

                model = new ConsultaPvsHandlerResponse();
                model.Pvs = pvs;
                model.StatusCode = 200;
                model.ExibirGridPvs = true;
            }

            model.ExibirRecaptcha = UsuarioNegocio.ExibirRecaptcha(context);
            return model;
        }
    }
}
