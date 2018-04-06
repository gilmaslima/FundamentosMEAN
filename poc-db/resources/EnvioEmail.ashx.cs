/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Resources;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Rede.PN.AtendimentoDigital.SharePoint.WebParts.AtendimentoDigital.FaleConosco;
using Redecard.PN.Comum;

namespace Rede.PN.AtendimentoDigital.SharePoint.Layouts.PN.AtendimentoDigital
{
    public partial class EnvioEmail : UserControlBase, IHttpHandler, IReadOnlySessionState
    {
        #region [ Propriedades ]

        /// <summary>
        /// Literal Nome
        /// </summary>
        public String LiteralNome { get { return this.ObterResource("formContato_CorpoEmail_Nome"); } }

        /// <summary>
        /// Literal E-mail
        /// </summary>
        public String LiteralEmail { get { return this.ObterResource("formContato_CorpoEmail_Email"); } }

        /// <summary>
        /// Literal telefone
        /// </summary>
        public String LiteralTelefone { get { return this.ObterResource("formContato_CorpoEmail_Telefone"); } }

        /// <summary>
        /// Literal PV
        /// </summary>
        public String LiteralPv { get { return this.ObterResource("formContato_CorpoEmail_NroEstabelecimento"); } }

        /// <summary>
        /// Literal mensagem
        /// </summary>
        public String LiteralMensagem { get { return this.ObterResource("formContato_CorpoEmail_Mensagem"); } }

        /// <summary>
        /// Literal não informado
        /// </summary>
        public String LiteralNaoInformado { get { return this.ObterResource("formContato_CorpoEmail_NaoInformado"); } }

        /// <summary>
        /// Literal razão social
        /// </summary>
        public String LiteralRazaoSocial { get { return this.ObterResource("formContatoOuvidoria_Campo_RazaoSocial"); } }

        /// <summary>
        /// Literal assunto
        /// </summary>
        public String LiteralAssunto { get { return this.ObterResource("formContato_CabecalhoEmail"); } }

        /// <summary>
        /// Literal motivo
        /// </summary>
        public String LiteralMotivo { get { return this.ObterResource("formContatoOuvidoria_Campo_MotivoContato"); } }

        /// <summary>
        /// Destinatário padrão do Fale Conosco
        /// </summary>
        public String DestinatarioPadrao { get { return "faleconosco@userede.com.br"; } }

        /// <summary>
        /// Código do Segmento do usuário atual
        /// </summary>
        public String Segmento { get { return Sessao.Contem() ? SessaoAtual.CodigoSegmento.ToString() : String.Empty; } }

        /// <summary>
        /// E-mail para atendimento diferenciado de acordo com o Segmento do usuário
        /// </summary>
        public String DestinatarioDiferenciado
        {
            get
            {
                switch (Segmento)
                {
                    //Cliente Top Varejo
                    case "E":
                    case "S":
                        return "resolve@userede.com.br";
                    //Email padrão
                    default:
                        return DestinatarioPadrao;
                }

            }
        }
        #endregion

        #region [ Implementação IHttpHandler ]

        /// <summary>
        /// IsReusable
        /// </summary>
        public Boolean IsReusable { get { return false; } }

        /// <summary>
        /// ProcessRequest.<br/>
        /// Recebe como parâmetro de entrada o JSON no formato:
        /// <code>
        ///  { 
        ///     'motivo': 'motivo do contato',
        ///     'assunto': 'assunto do contato',
        ///     'nome': 'nome do contato',
        ///     'telefone': 'telefone do contato',
        ///     'mensagem': 'mensagem'
        ///  }
        /// </code>
        /// Retorna como resposta:
        /// <code>
        /// {
        ///     'mensagem': 'mensagem de retorno',
        ///     'codigo': 0     -- 0 (sucesso) ou 300 (erro)
        /// }
        /// </code>
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            //Validação CSRF (validação de X-RequestDigest do Header da requisição) e sessão do usuário
            if (!Sessao.Contem() || !ValidateFormDigest())
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (Int32)HttpStatusCode.Forbidden;
                context.Response.Write(SerializarResposta(false, "acesso negado"));
                return;
            }

            //Apenas permite POST
            if (String.Compare(context.Request.RequestType, "POST", true) != 0)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (Int32)HttpStatusCode.MethodNotAllowed;
                context.Response.Write(SerializarResposta(false, "tipo de operação inválido"));
                return;
            }

            //Recupera os campos da requisição
            String motivo = context.Request["motivo"];
            String assunto = context.Request["assunto"];
            String nome = context.Request["nome"];
            String telefone = context.Request["telefone"];
            String mensagem = context.Request["mensagem"];

            //Envia e-mail
            Object retorno = EnviarEmail(motivo, assunto, nome, telefone, mensagem);
            context.Response.Write(retorno);            
        }

        #endregion

        /// <summary>
        /// ValidateFormDigest
        /// </summary>
        /// <returns>Validação</returns>
        private Boolean ValidateFormDigest()
        {
            Boolean valido = false;
            try
            {
                valido = SPUtility.ValidateFormDigest();
            }
            catch (SPException ex)
            {
                Logger.GravarErro("ValidateFormDigest=false", ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("ValidateFormDigest=false", ex);
            }
            return valido;
        }

        /// <summary>
        /// Método principal para envio de e-mail
        /// </summary>
        /// <param name="motivo">Motivo do contato</param>
        /// <param name="assunto">Assunto do contato</param>
        /// <param name="nome">Nome do contato</param>
        /// <param name="telefone">Telefone do contato</param>
        /// <param name="mensagem">Mensagem do contato</param>
        /// <returns>Objeto de retorno</returns>
        private Object EnviarEmail(
            String motivo,
            String assunto,
            String nome,
            String telefone,
            String mensagem)
        {
            String mensagemValidacao = default(String);

            //Apenas prepara e-mail se todos os campos forem válidos
            if (ValidarCampos(motivo, assunto, nome, telefone, mensagem, out mensagemValidacao))
            {
                //Recupera os dados da sessão
                String email = SessaoAtual.Email;
                String pv = SessaoAtual.CodigoEntidade.ToString();
                String razaoSocial = SessaoAtual.NomeEntidade;

                //Prepara corpo do e-mail com conteúdo do formulário e do usuário
                String corpoEmail = MontarCorpoEmailHtml(
                    nome, email, telefone,
                    pv, razaoSocial, motivo,
                    assunto, mensagem);

                //Montagem do e-mail para envio
                Email mensagemEmail = new Email(
                    from: this.DestinatarioDiferenciado,
                    to: this.DestinatarioDiferenciado,
                    subject: this.LiteralAssunto,
                    bodyContent: corpoEmail,
                    isBodyHtml: true);

                Boolean emailEnviado = mensagemEmail.EnviarEmail();

                return SerializarResposta(emailEnviado, emailEnviado ?
                    "mensagem enviada com sucesso" : "erro durante envio da mensagem");
            }
            else
            {
                return SerializarResposta(false, mensagemValidacao);
            }
        }

        #region [ Auxiliares Handler ]

        /// <summary>
        /// Serializa o retorno.
        /// </summary>
        /// <param name="sucesso">Sucesso ou não do envio do e-mail</param>
        /// <param name="mensagem">Mensagem de sucesso</param>
        /// <returns>String do objeto serializado em JSON</returns>
        private String SerializarResposta(Boolean sucesso, String mensagem)
        {
            var jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(new
            {
                mensagem = mensagem,
                codigo = sucesso ? 0 : CODIGO_ERRO
            });
        }


        #endregion

        #region [Validar > Formatar > Enviar e-mail]

        /// <summary>
        /// Valida os campos obrigatórios do formulário.
        /// </summary>
        /// <param name="motivo">Motivo do e-mail</param>
        /// <param name="assunto">Assunto do e-mail</param>
        /// <param name="nome">Nome do contato</param>
        /// <param name="telefone">Telefone do contato</param>
        /// <param name="mensagem">Mensagem do contato</param>
        /// <param name="mensagemValidacao">Mensagem de validação, em caso de campo inválido</param>
        /// <returns>Campos válidos</returns>
        private Boolean ValidarCampos(
            String motivo,
            String assunto,
            String nome,
            String telefone,
            String mensagem,
            out String mensagemValidacao)
        {
            if (String.IsNullOrWhiteSpace(motivo))
            {
                mensagemValidacao  = "motivo é obrigatório";
                return false;
            }

            if (String.IsNullOrWhiteSpace(assunto))
            {
                mensagemValidacao  = "assunto é obrigatório";
                return false;
            }

            if (String.IsNullOrWhiteSpace(nome))
            {
                mensagemValidacao = "nome é obrigatório";
                return false;
            }

            if (String.IsNullOrWhiteSpace(mensagem))
            {
                mensagemValidacao = "mensagem é obrigatório";
                return false;
            }

            if (String.IsNullOrWhiteSpace(telefone))
            {
                mensagemValidacao = "telefone é obrigatório";
                return false;
            }

            mensagemValidacao = String.Empty;
            return true;
        }

        /// <summary>
        /// Retorna um texto para uma determinada chave a partir do arquivo redecard.[idioma].resx
        /// (Este arquivo encontra-se no projeto Redecard.Portal.Aberto.SD, pasta RecursosApp)
        /// </summary>
        /// <param name="chave">Chave da propriedade</param>
        /// <returns>Valor da propriedade</returns>
        private String ObterResource(String chave)
        {
            String valor = String.Empty;

            try
            {
                valor = Convert.ToString(HttpContext.GetGlobalResourceObject("redecard", chave));
            }
            catch (MissingManifestResourceException ex)
            {
                Logger.GravarErro("Erro durante obtenção de recurso global: " + chave + ". " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante obtenção de recurso global: " + chave + ". " + ex.Message, ex);
            }

            return valor;
        }

        /// <summary>
        /// Monta corpo do do e-mail com os dados do contato.
        /// </summary>
        /// <param name="contato">Dados do e-mail</param>
        /// <returns>Corpo do e-mail</returns>
        private String MontarCorpoEmailHtml(
            String contatoNome,
            String contatoEmail,
            String contatoTelefone,
            String contatoPv,
            String contatoRazaoSocial,
            String contatoMotivo,
            String contatoAssunto,
            String contatoMensagem)
        {
            String formatacaoLinha = "{0} {1}";

            List<String> linhasEmail = new List<String>();

            linhasEmail.Add(String.Format(formatacaoLinha, this.LiteralNome, TratarCampo(contatoNome)));
            linhasEmail.Add(String.Format(formatacaoLinha, this.LiteralEmail, TratarCampo(contatoEmail)));
            linhasEmail.Add(String.Format(formatacaoLinha, this.LiteralTelefone, TratarCampo(contatoTelefone)));
            linhasEmail.Add(String.Format(formatacaoLinha, this.LiteralPv, TratarCampo(contatoPv)));
            linhasEmail.Add(String.Format(formatacaoLinha, this.LiteralRazaoSocial, TratarCampo(contatoRazaoSocial)));
            linhasEmail.Add(String.Format(formatacaoLinha, this.LiteralMotivo, TratarCampo(contatoMotivo) + " - " + TratarCampo(contatoAssunto)));
            linhasEmail.Add(String.Format(formatacaoLinha, this.LiteralMensagem, TratarCampo(contatoMensagem)));

            return String.Join("<br />", linhasEmail.ToArray());
        }

        /// <summary>
        /// Formata valor da resposta do formulário do e-mail. 
        /// Substitui por "campo não informado" quando o valor é vazio.
        /// </summary>
        /// <param name="campo">Campo</param>
        /// <returns>Valor formatado</returns>
        private String TratarCampo(String campo)
        {
            if (String.IsNullOrWhiteSpace(campo))
                return this.LiteralNaoInformado;
            else
                return campo.Trim();
        }

        #endregion
    }
}