/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Resources;
using System.Web;
using Rede.PN.AtendimentoDigital.SharePoint.Core;
using Redecard.PN.Comum;

namespace Rede.PN.AtendimentoDigital.SharePoint.Handlers
{
    /// <summary>
    /// Implementação de handler para envio de e-mail
    /// </summary>
    public class EnvioEmail : HandlerBase
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
        /// E-mail para atendimento diferenciado de acordo com o Segmento do usuário
        /// </summary>
        public String DestinatarioDiferenciado
        {
            get
            {
                switch (Sessao.CodigoSegmento)
                {
                    //Cliente Top Varejo
                    case 'E':
                    case 'S':
                    //case 'N': removido por solicitação do Gerci Filho (14/09/2016) - Release 3
                        return "resolve@userede.com.br";
                    //Email padrão
                    default:
                        return DestinatarioPadrao;
                }

            }
        }
        #endregion

        /// <summary>
        /// Envio de e-mail
        /// </summary>
        /// <param name="request">HttpRequest</param>
        /// <returns>Resultado do envio do e-mail</returns>
        [HttpPost]
        public HandlerResponse EnviarEmail()
        {
            HandlerResponse response = new HandlerResponse();

            //Recupera os campos da requisição
            String motivo = base.Request["motivo"];
            String assunto = base.Request["assunto"];
            String nome = base.Request["nome"];
            String telefone = base.Request["telefone"];
            String mensagem = base.Request["mensagem"];
            String mensagemValidacao = default(String);

            //Apenas prepara e-mail se todos os campos forem válidos
            if (ValidarCampos(motivo, assunto, nome, telefone, mensagem, out mensagemValidacao))
            {
                //Recupera os dados da sessão
                String email = base.Sessao.Email;
                String pv = base.Sessao.CodigoEntidade.ToString();
                String razaoSocial = base.Sessao.NomeEntidade;

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

                Boolean emailEnviado = mensagemEmail.EnviarEmail(base.CurrentSPContext.Web);

                response.Codigo = emailEnviado ? 0 : CodigoErro;
                response.Mensagem = emailEnviado ? "mensagem enviada com sucesso" : "erro durante envio da mensagem";
            }
            else
            {
                response.Codigo = CodigoErro;                
                response.Mensagem = mensagemValidacao;
            }

            return response;
        }

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
                mensagemValidacao = "motivo é obrigatório";
                return false;
            }

            if (String.IsNullOrWhiteSpace(assunto))
            {
                mensagemValidacao = "assunto é obrigatório";
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