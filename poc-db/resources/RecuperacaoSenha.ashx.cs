using Microsoft.SharePoint;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;
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
    public class RecuperacaoSenha : IHttpHandler, IRequiresSessionState
    {
        /// <summary>
        /// Marca handler como reusável
        /// </summary>
        public bool IsReusable { get { return true; } }

        #region Generic Handler
        
        /// <summary>
        /// Serializer para retornar os dados em formato JSON para o front-end.
        /// </summary>
        private JavaScriptSerializer serializer = new JavaScriptSerializer();


        /// <summary>
        /// Método para processar o Request Ajax.
        /// </summary>
        /// <param name="context">Contexto HTTP</param>
        public void ProcessRequest(HttpContext context)
        {
            using (Logger log = Logger.IniciarLog("RecuperacaoSenha.ashx - ProcessRequest"))
            {
                ConsultaPvsHandlerResponse model = null;
                try
                {
                    //Prossegue para a recuperação de senha.
                    model = RecuperarSenha(context);

                    // verifica se deve exibir o captcha
                    model.ExibirRecaptcha = UsuarioNegocio.ExibirRecaptcha(context);
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    GravarParametrosLog(context, log);
                    log.GravarErro(ex);
                    model = UsuarioNegocio.ConstruirMensagemErro(500, ex.Detail.Fonte, ex.Detail.Codigo);

                    // verifica se deve exibir o captcha
                    model.ExibirRecaptcha = UsuarioNegocio.ExibirRecaptcha(context);
                }
                catch (HttpException ex)
                {
                    GravarParametrosLog(context, log);
                    log.GravarErro(ex);
                    model = UsuarioNegocio.ConstruirMensagemErro(500, ex.Message, ex.StackTrace);

                    // verifica se deve exibir o captcha
                    model.ExibirRecaptcha = UsuarioNegocio.ExibirRecaptcha(context);
                }
                catch (Exception ex)
                {
                    GravarParametrosLog(context, log);
                    log.GravarErro(ex);
                    model = UsuarioNegocio.ConstruirMensagemErro(500, "Sistema indisponível (-1).", String.Format("{0} - {1}", ex.Message, ex.StackTrace));

                    // verifica se deve exibir o captcha
                    model.ExibirRecaptcha = UsuarioNegocio.ExibirRecaptcha(context);
                }

                //Retorna o código de status HTTP e o objeto serializado.
                context.Response.StatusCode = model.StatusCode != 0 ? model.StatusCode : 200;
                context.Response.Write(serializer.Serialize(model));
            }
        }

        /// <summary>
        /// Método para Retornar os PV's no fluxo de recuperação de Senha.
        /// </summary>
        /// <param name="context">Contexto HTTP</param>
        private ConsultaPvsHandlerResponse RecuperarSenha(HttpContext context)
        {
            ConsultaPvsHandlerResponse model = null;
            
            //Indicador do passo atual.
            Int32 indicadorPasso = context.Request.Form["indicadorPasso"] != null ? Convert.ToInt32(context.Request.Form["indicadorPasso"]) : 0;
            
            //Parâmetros primeira chamada.
            String email = context.Request.Form["email"] != null ? context.Request.Form["email"] : String.Empty;
            
            //Response do captcha
            String captchaResponse = context.Request.Params["captchaResponse"];

            //Parâmetros segunda chamada.
            String arrayPvs = context.Request.Form["arrayPvs"];
            String arrayPvsSelecionados = context.Request.Form["arrayPvsSelecionados"];

            //Parâmetros terceira chamada
            Boolean rbEmailPrincipalChecked = context.Request.Form["rbEmailPrincipal"] != null ? Convert.ToBoolean(context.Request.Form["rbEmailPrincipal"]) : false;
            Boolean rbSMSChecked = context.Request.Form["rbSMS"] != null ? Convert.ToBoolean(context.Request.Form["rbSMS"]) : false;
            Boolean rbEmailSecundarioChecked = context.Request.Form["rbEmailSecundario"] != null ? Convert.ToBoolean(context.Request.Form["rbEmailSecundario"]) : false;

            switch (indicadorPasso)
            {
                //Na primeira chamada, é necessário ou trazer o grid com os pv's, ou redirecionar o usuário para a próxima tela.
                case 1:

                    // incrementa o contador do Recaptcha
                    ControlTemplates.RecaptchaGoogle.IncrementTentativas(context, true);

                    //Valida o captcha
                    if (!ControlTemplates.RecaptchaGoogle.ValidateCaptcha(context, captchaResponse, true))
                    {
                        model = UsuarioNegocio.ConstruirMensagemErro(500, "Código (captcha) inválido.");
                        return model;
                    }

                    if (String.IsNullOrEmpty(email))
                    {
                        model = UsuarioNegocio.ConstruirMensagemErro(500, "Parâmetro email não informado.");
                    }
                    else
                    {
                        model = ContinuarRecuperacaoSenha(email, context);
                    }
                    
                    break;
                //Após a seleção dos PV's, decide se o usuário deve ser redirecionado para o próximo passo, ou se deve ser trazido o painel de formas de envio.
                case 2:
                    if (!String.IsNullOrEmpty(arrayPvs) && !String.IsNullOrEmpty(arrayPvsSelecionados) && InformacaoUsuario.Existe())
                    {
                        //Desserializando a lista de PVs (Todos os pv's e os pv's selecionados) obtida na chamada.
                        var listTodosPvs = serializer.Deserialize<List<ConsultaPvsHandlerModel>>(arrayPvs);
                        var listPvsSelecionados = serializer.Deserialize<Int32[]>(arrayPvsSelecionados);
                        InformacaoUsuario info = InformacaoUsuario.Recuperar();

                        //Setando o ID do usuário relacionado ao PV selecionado.
                        info.IdUsuario = listTodosPvs.FirstOrDefault(x => listPvsSelecionados.Contains(x.NumeroPv)).IdUsuario;
                        InformacaoUsuario.Salvar(info);

                        //Envia para o próximo passo
                        model = DefinirProximoPasso(info, listTodosPvs, listPvsSelecionados);
                    }
                    else
                    {
                        model = UsuarioNegocio.ConstruirMensagemErro(500, "Erro ao prosseguir para o próximo passo.");
                    }
                    break;
                //No passo 3, verifica qual a forma de envio selecionada e redireciona para a próxima tela.
                case 3:
                    if (rbEmailPrincipalChecked || rbSMSChecked || rbEmailSecundarioChecked)
                    {
                        model = new ConsultaPvsHandlerResponse();
                        model.UrlRedirect = String.Format("{0}/Paginas/RecuperacaoSenhaFormaEnvio.aspx?rbEmailPrincipal={1}&rbEmailSecundario={2}&rbSMS={3}", SPContext.Current.Web.Url, rbEmailPrincipalChecked, rbEmailSecundarioChecked, rbSMSChecked);
                    }
                    else
                    {
                        model = UsuarioNegocio.ConstruirMensagemErro(500, "Nenhuma forma de envio selecionada.");
                    }
                    break;
                default:
                    model = UsuarioNegocio.ConstruirMensagemErro(500, "Erro ao prosseguir para o próximo passo.");
                    break;
            }
            return model;
        }

        /// <summary>
        /// Define qual o próximo passo do fluxo de recuperação.
        /// </summary>
        /// <param name="info">Informação do usuário</param>
        /// <param name="todosPvs">Lista de todos os Pv's</param>
        /// <param name="pvsSelecionados">Lista com os PV's selecionados</param>
        private ConsultaPvsHandlerResponse DefinirProximoPasso(InformacaoUsuario info, List<ConsultaPvsHandlerModel> todosPvs, Int32[] pvsSelecionados)
        {
            ConsultaPvsHandlerResponse model = new ConsultaPvsHandlerResponse();
            using (Logger log = Logger.IniciarLog("Recuperação de Senha - Definir se deve ser exibido o painel de formas de envio na mesma tela ou redirecionar para o passo 2"))
            {
                HashSet<String> emailDistinct;
                HashSet<Int32?> celularDistinct;
                HashSet<Int32> numeroPvs = new HashSet<int>(todosPvs.Select(y => y.NumeroPv));

                if (InformacaoUsuario.Existe())
                {
                    // Selecionando, dos pv's enviados pela chamada Ajax, somente os que constam na lista dos pv's do usuário (evitando que o processamento seja feito para PVS que não pertencem ao usr).
                    pvsSelecionados = pvsSelecionados.Where(x => numeroPvs.Contains(x)).ToArray();
                    info.PvsSelecionados = pvsSelecionados;
                    InformacaoUsuario.Salvar(info);

                    //Caso mais de um PV tenha sido selecionado no passo anterior e as informações sejam iguais, habilita os checkboxes.
                    if (pvsSelecionados != null && pvsSelecionados.Count() > 1)
                    {
                        //HashSet irá realizar um distinct nos emails
                        emailDistinct = new HashSet<string>(todosPvs.Select(x => x.EmailSecundario));
                        celularDistinct = new HashSet<int?>(todosPvs.Select(x => x.Celular));

                        model.ExibirRadioEmailSecundario = true;
                        model.ExibirRadioSMS = true;

                        //Caso alguem não tenha email secundario, não mostra a opção
                        if (todosPvs.Any(x => string.IsNullOrEmpty(x.EmailSecundario)))
                        {
                            model.ExibirRadioEmailSecundario = false;
                        }

                        //Caso alguem possua um email secundario diferente não mostra a opção
                        if (emailDistinct != null && emailDistinct.Count > 1)
                        {
                            model.ExibirRadioEmailSecundario = false;
                        }

                        //Caso alguem não possuam celular não mostra a opção
                        if (todosPvs.Any(x => x.Celular == null || x.Celular == 0))
                        {
                            model.ExibirRadioSMS = false;
                        }

                        //Caso alguem possua um celular diferente  não mostra a opção
                        if (celularDistinct != null && celularDistinct.Count > 1)
                        {
                            model.ExibirRadioSMS = false;
                        }

                    }
                    //Caso apenas um PV esteja selecionado habilita o(s) check(s) caso as informações não sejam nulas
                    else
                    {
                        log.GravarMensagem("Apenas um pv selecionado");

                        //Pega o PV selecionado.
                        var selecionado = todosPvs.FirstOrDefault(x => pvsSelecionados.Contains(x.NumeroPv));

                        if (selecionado != null)
                        {
                            model.ExibirRadioEmailSecundario = !String.IsNullOrEmpty(selecionado.EmailSecundario);

                            model.ExibirRadioSMS = (selecionado.DDDCelular > 0) && (selecionado.Celular > 0);
                        }
                    }

                    log.GravarMensagem("Opções ", new { OpcaoSms = model.ExibirRadioSMS, OpcaoEmailSecundario = model.ExibirRadioEmailSecundario });

                    model.ExibirPainelEnvioCodigo = model.ExibirRadioEmailSecundario || model.ExibirRadioSMS;

                    //Caso nenhum dos dois radios esteja visível (o que significa que somente o email principal está disponível), redireciona para a tela de confirmação do envio de e-mail.
                    if (!model.ExibirRadioEmailSecundario && !model.ExibirRadioSMS)
                    {
                        String urlProximoPasso = String.Format("{0}/Paginas/RecuperacaoSenhaFormaEnvio.aspx?rbEmailPrincipal=true&rbEmailSecundario=false&rbSMS=false", SPContext.Current.Web.Url);
                        model.StatusCode = 301;
                        model.MensagemRetorno = String.Format("Redirecionamento para: {0}", urlProximoPasso);
                        model.UrlRedirect = urlProximoPasso;
                    }

                    model.Pvs = todosPvs;

                    return model;
                }
                else
                {
                    return UsuarioNegocio.ConstruirMensagemErro(500, "SharePoint.SessaoUsuario", 1151);
                }
            }
        }

        /// <summary>
        /// Handler de clique do botão Continuar.
        /// </summary>
        /// <param name="email">Email do usuário</param>
        /// <param name="context">Contexto HTTP</param>
        protected ConsultaPvsHandlerResponse ContinuarRecuperacaoSenha(String email, HttpContext context)
        {
            ConsultaPvsHandlerResponse model = null;
            using (Logger log = Logger.IniciarLog("Continuar para o próximo Passo de Recuperação de Usuário/Senha"))
            {
                //A lista de PV's ainda não estará visível, então limpa os dados de informação do usuário.
                InformacaoUsuario.Limpar();

                //Obtém os PV's para exibir a lista.
                List<ConsultaPvsHandlerModel> pvs = UsuarioNegocio.ObterPvs(null, email);
                InformacaoUsuario info = InformacaoUsuario.Recuperar();

                //Se o status do usuário não for válido, retorna.
                if (!UsuarioNegocio.ValidarStatusUsuario(null, email, out model))
                    return model;

                //Caso não sejam retornados pv's, retorna.
                if (pvs == null || pvs.Count() == 0)
                {
                    return UsuarioNegocio.ConstruirMensagemErro(500, "Não foram encontrados estabelecimentos para o usuário informado", dispararTagGtm: true, labelGtm: "Não foram encontrados estabelecimentos para o usuário informado");
                }

                //Caso retorne somente um, prossegue para o próximo passo.
                if (pvs.Count() == 1 || info.PvSenhasIguais)
                {
                    info = null;

                    var usuario = UsuarioNegocio.ObterUsuario(null, email);

                    if (usuario == null)
                    {
                        return UsuarioNegocio.ConstruirMensagemAviso(500, "Usuário não encontrado", true, "Usuário não encontrado");
                    }

                    if (!UsuarioNegocio.ValidarUsuario(usuario, out model))
                        return model;
                    
                    info = InformacaoUsuario.Recuperar();

                    if (info != null)
                    {
                        info.PvsSelecionados = info.EstabelecimentosRelacinados.Select(p => p.NumeroPV).ToArray();

                        InformacaoUsuario.Salvar(info);
                        return DefinirProximoPasso(info, pvs, info.PvsSelecionados);
                    }
                }

                //Caso retorne mais de um, exibe a lista de PV's.
                else
                {
                    //Salvando no cache os estabelecimentos que foram trazidos na consulta, para filtrar posteriormente quando o usuário selecionar.
                    info.EstabelecimentosRelacinados = Array.ConvertAll<ConsultaPvsHandlerModel, EntidadeServicoModel>(pvs.ToArray(), x => new EntidadeServicoModel
                    {
                        Celular = x.Celular,
                        DDDCelular = x.DDDCelular,
                        Email = x.Email,
                        EmailSecundario = x.EmailSecundario,
                        RazaoSocial = x.NomeEstabelecimento,
                        NumeroPV = x.NumeroPv,
                    });
                    InformacaoUsuario.Salvar(info);

                    model = new ConsultaPvsHandlerResponse();
                    model.StatusCode = 200;
                    model.ExibirGridPvs = true;
                    model.Pvs = pvs;
                }
            }
            return model;
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

        #endregion
    }
}
