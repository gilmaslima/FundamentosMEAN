using Microsoft.SharePoint;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Emails;
using Rede.PN.AtendimentoDigital.SharePoint.Core.OrdemServico;
using Rede.PN.AtendimentoDigital.SharePoint.Repository;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;


namespace Rede.PN.AtendimentoDigital.SharePoint.Handlers
{
    /// <summary>
    /// Handler que trata envio de e-mail para suporte maquininha.
    /// </summary>
    public sealed class EmailHandler : HandlerBase
    {
        /// <summary>
        /// ParametrosConfiguracaoRepository
        /// </summary>
        private readonly ParametrosConfiguracaoRepository parametrosConfiguracaoRepository;

        /// <summary>
        /// EmailHandler
        /// </summary>
        public EmailHandler()
        {
            this.parametrosConfiguracaoRepository = new ParametrosConfiguracaoRepository();
        }


        public EmailHandler(SPContext sPContext, Sessao sessao): this()
        {
            base.CurrentSPContext = sPContext;
            base.Sessao = sessao;
            
        }
        #region [ Propriedades ]

        /// <summary>
        /// Retorna email faleConosco de acordo com o segmento.
        /// </summary>
        public String EmailFaleConosco
        {
            get
            {
                String name;
                SessaoUsuario sessaoUsuario = new SessaoUsuario();
                if (sessaoUsuario.EstabelecimentoTopVarejoEmail(base.CurrentSPContext.Web, base.Sessao))
                    name = "emailResolutivo";
                else
                    name = "gavetaEmail";

                ParametrosConfiguracao parametroConfiguracao = parametrosConfiguracaoRepository.Get(base.CurrentSPContext.Web, name);

                if (parametroConfiguracao != null && !String.IsNullOrWhiteSpace(parametroConfiguracao.Valor))
                {
                    return parametroConfiguracao.Valor;
                }
                else
                {
                    return "faleconosco@userede.com.br";
                }
            }
        }

        /// <summary>
        /// DisplayNameSupMaquininha
        /// </summary>
        public static String DisplayNameSupMaquininha { get { return "Rede"; } }

        /// <summary>
        /// E-mail suporteMaquininha.
        /// </summary>
        public String EmailSuporteMaquininha
        {
            get
            {
                ParametrosConfiguracao parametroConfiguracao = parametrosConfiguracaoRepository.Get(base.CurrentSPContext.Web, "emailTrocaMaquininha");
                if (parametroConfiguracao != null && !String.IsNullOrWhiteSpace(parametroConfiguracao.Valor))
                {
                    return parametroConfiguracao.Valor;
                }
                else
                {
                    return "rede@userede.com.br";
                }
            }
        }
        /// <summary>
        /// Assunto padrão suporteMaquinha.
        /// </summary>
        public static String AssuntoSuporteMaquinha { get { return @"Protocolo de Solicitação de Troca de Terminal #{0}"; } }
        /// <summary>
        /// assunto do fale conosco.
        /// </summary>
        public static String AssuntoFaleConosco { get { return "Portal Rede Logado - Contato realizado - Assunto: {0}"; } }
        /// <summary>
        /// assunto do fale conosco quando não tem assunto.
        /// </summary>
        public static String AssuntoFaleConoscoSemAssunto { get { return "Portal Rede Logado - Contato realizado"; } }
        /// <summary>
        /// E-mail do destinatário.
        /// </summary>
        public String Destinatario { get; set; }
        /// <summary>
        /// E-mail do remetente.
        /// </summary>
        public String Remetente { get; set; }
        /// <summary>
        /// Assunto do e-mail.
        /// </summary>
        public String Assunto { get; set; }
        #endregion

        #region [ Handler ]
        /// <summary>
        /// Handler para envio de protocolo do suporte a maquinha
        /// </summary>
        [HttpPost]
        public HandlerResponse EnviarEmailSuporteMaquininha()
        {
            //Recupera destinatário
            Destinatario = base.Request["email"];

            //Recupera remetente
            Remetente = EmailSuporteMaquininha;

            //Recupera previsão de atendimento
            String previsaoAtendimento = base.Request["dataPrevisao"];
            DateTime dataPrevisao = previsaoAtendimento.ToDate("dd/MM/yyyy");
            var culture = new System.Globalization.CultureInfo("pt-br");
            String diaPrevisao = culture.DateTimeFormat.GetDayName(dataPrevisao.DayOfWeek);

            //Recupera protocolo
            String protocolo = base.Request["numeroProtocolo"];

            //Concatena com assunto
            Assunto = String.Format(AssuntoSuporteMaquinha, protocolo);

            //Dados da maquininha
            String tipoMaquininha = base.Request["tipoMaquininha"];
            String numeroPV = base.Request["numeroPV"];
            String problemaEncontrado = base.Request["problemaEncontrado"];

            //Dados para atendimento
            String logradouro = base.Request["logradouro"];
            String numero = base.Request["numero"];
            String bairro = base.Request["bairro"];
            String cidade = base.Request["cidade"];
            String uf = base.Request["uf"];
            String cep = base.Request["cep"];

            //Dados pessoais
            String nomeCompleto = base.Request["nomeCompleto"];
            String[] nomes = nomeCompleto.Trim().Split(' ');
            String nome = nomes[0];
            String celular = base.Request["celular"];

            //Numero Estabelecimento
            String numeroEstabelecimento = base.Request["numeroEstabelecimento"];

            //Constrói dicionário
            var dicionario = new Dictionary<String, String>();
            dicionario.Add("{{nome}}", nome);
            dicionario.Add("{{dataPrevisao}}", dataPrevisao.FormatToDayMonthYear());
            dicionario.Add("{{diaPrevisao}}", diaPrevisao);
            dicionario.Add("{{numeroProtocolo}}", protocolo);
            dicionario.Add("{{tipoMaquininha}}", tipoMaquininha);
            dicionario.Add("{{numeroPV}}", numeroPV);
            dicionario.Add("{{problemaEncontrado}}", problemaEncontrado);
            dicionario.Add("{{logradouro}}", logradouro);
            dicionario.Add("{{numero}}", numero);
            dicionario.Add("{{bairro}}", bairro);
            dicionario.Add("{{cidade}}", cidade);
            dicionario.Add("{{uf}}", uf);
            dicionario.Add("{{cep}}", cep);
            dicionario.Add("{{email}}", Destinatario);
            dicionario.Add("{{celular}}", celular);
            dicionario.Add("{{numeroEstabelecimento}}", numeroEstabelecimento);
            dicionario.Add("{{nomeCompleto}}", nomeCompleto);

            //Recupera da lista template em html
            var adRepository = new Repository.AtendimentoDigitalRepository();
            var spFile = adRepository.Get(base.CurrentSPContext.Web, "AtendimentoDigital/Email/OrdemServico", "html").Select(q => q.File).FirstOrDefault();

            //converte o arquivo para string
            var templateHtml = String.Empty;
            using (var reader = new StreamReader(spFile.OpenBinaryStream()))
            {
                templateHtml = reader.ReadToEnd();
            }

            //Recupera as imagens
            var repository = adRepository.Get(base.CurrentSPContext.Web, "AtendimentoDigital/Email/OrdemServico", "png").ToList();

            var email = new EmailSuporteMaquininha(Remetente, Destinatario, Assunto).EnviarEmail(dicionario, templateHtml, repository);

            if (email)
            {
                return new HandlerResponse(true);

            }
            else
            {
                return new HandlerResponse(300, "Falha ao enviar o e-mail.");
            }

        }
        /// <summary>
        /// Handler para envio de email do fale conosco
        /// </summary>
        [HttpPost]
        public HandlerResponse EnviarEmailFaleConosco()
        {
            //Recupera destinatário
            Destinatario = EmailFaleConosco;

            //Recupera remetente
            Remetente = EmailFaleConosco;

            //Recupera dados para montar e-mail
            String pv = base.Sessao.CodigoEntidade.ToString();
            String razaoSocial = base.Sessao.NomeEntidade;
            String motivoContato = base.Request["motivoContato"];
            String assunto = base.Request["assunto"];
            String nome = base.Request["nome"];
            String telefone = base.Request["telefone"];
            String mensagem = base.Request["mensagem"];

            //Cria dicionário de dados
            var dicionario = new Dictionary<String, String>();
            dicionario.Add("{{email}}", base.Sessao.Email);
            dicionario.Add("{{numeroEstabelecimento}}", pv);
            dicionario.Add("{{motivoContato}}", motivoContato);
            dicionario.Add("{{nomeCompleto}}", nome);
            dicionario.Add("{{telefone}}", telefone);
            dicionario.Add("{{mensagem}}", mensagem);
            dicionario.Add("{{razaoSocial}}", razaoSocial);

            //Recupera da lista template em html
            var adRepository = new Repository.AtendimentoDigitalRepository();
            var spFile = adRepository.Get(base.CurrentSPContext.Web, "AtendimentoDigital/Email/FaleConosco", "html").Select(q => q.File).FirstOrDefault();

            //converte o arquivo para string
            var templateHtml = String.Empty;
            using (var reader = new StreamReader(spFile.OpenBinaryStream()))
            {
                templateHtml = reader.ReadToEnd();
            }

            //atualiza assunto do e-mail com o assunto do usuário
            String assuntoFaleConosco = String.IsNullOrWhiteSpace(assunto) ? AssuntoFaleConoscoSemAssunto : String.Format(AssuntoFaleConosco, assunto);

            var email = new EmailFaleConosco(Remetente, Destinatario, assuntoFaleConosco, templateHtml, dicionario);

            if (email.EnviarEmail(dicionario, templateHtml))
            {
                return new HandlerResponse(true);
            }
            else
            {
                return new HandlerResponse(300, "Falha ao enviar o e-mail.");
            }

        }
        #endregion
        /// <summary>
        /// Método Envio de email interno
        /// </summary>
        /// <param name="os">Rede.PN.AtendimentoDigital.SharePoint.MaximoServico.OSCriacao</param>
        /// <param name="dataPrevisaoAtendimento">String</param>
        /// <param name="numeroProtocolo">String</param>
        /// <returns>HandlerResponse</returns>
        public HandlerResponse EnviarEmailSuporteMaquininha(SPWeb SpContext, OrdemServico os, String dataPrevisaoAtendimento, String numeroProtocolo)
        {
            //Recupera destinatário
            Destinatario = os.Contato.Email;

            //Recupera remetente
            Remetente = EmailSuporteMaquininha;

            //Recupera previsão de atendimento
            String previsaoAtendimento = dataPrevisaoAtendimento;
            DateTime dataPrevisao = previsaoAtendimento.ToDate("dd/MM/yyyy");
            var culture = new CultureInfo("pt-br");
            String diaPrevisao = culture.DateTimeFormat.GetDayName(dataPrevisao.DayOfWeek);

            //Recupera protocolo
            String protocolo = numeroProtocolo;

            //Concatena com assunto
            Assunto = String.Format(AssuntoSuporteMaquinha, protocolo);

            //Dados da maquininha
            String tipoMaquininha = os.TipoEquipamento;
            String numeroPV = os.NumeroLogico;
            String problemaEncontrado = os.ProblemaEncontrado;

            //Dados para atendimento
            String logradouro = os.EnderecoAtendimento.Logradouro;
            String numero = os.EnderecoAtendimento.Numero;
            String bairro = os.EnderecoAtendimento.Bairro;
            String cidade = os.EnderecoAtendimento.Cidade;
            String uf = os.EnderecoAtendimento.Estado;
            String cep = os.EnderecoAtendimento.Cep;

            //Dados pessoais
            String nomeCompleto = os.Contato.Nome;
            String[] nomes = nomeCompleto.Trim().Split(' ');
            String nome = nomes[0];
            String celular = os.Contato.Celular;

            //Numero Estabelecimento
            String numeroEstabelecimento = Sessao.CodigoEntidade.ToString();

            //Constrói dicionário
            var dicionario = new Dictionary<String, String>();
            dicionario.Add("{{nome}}", nome);
            dicionario.Add("{{dataPrevisao}}", dataPrevisao.FormatToDayMonthYear());
            dicionario.Add("{{diaPrevisao}}", diaPrevisao);
            dicionario.Add("{{numeroProtocolo}}", protocolo);
            dicionario.Add("{{tipoMaquininha}}", tipoMaquininha);
            dicionario.Add("{{numeroPV}}", numeroPV);
            dicionario.Add("{{problemaEncontrado}}", problemaEncontrado);
            dicionario.Add("{{logradouro}}", logradouro);
            dicionario.Add("{{numero}}", numero);
            dicionario.Add("{{bairro}}", bairro);
            dicionario.Add("{{cidade}}", cidade);
            dicionario.Add("{{uf}}", uf);
            dicionario.Add("{{cep}}", cep.RemoverLetras());
            dicionario.Add("{{email}}", Destinatario);
            dicionario.Add("{{celular}}", celular);
            dicionario.Add("{{numeroEstabelecimento}}", numeroEstabelecimento);
            dicionario.Add("{{nomeCompleto}}", nomeCompleto);

            //Recupera da lista template em html
            var adRepository = new Repository.AtendimentoDigitalRepository();
            var spFile = adRepository.Get(SpContext, "AtendimentoDigital/Email/OrdemServico", "html").Select(q => q.File).FirstOrDefault();

            //converte o arquivo para string
            var templateHtml = String.Empty;
            using (var reader = new StreamReader(spFile.OpenBinaryStream()))
            {
                templateHtml = reader.ReadToEnd();
            }

            //Recupera as imagens
            var repository = adRepository.Get(SpContext, "AtendimentoDigital/Email/OrdemServico", "png").ToList();

            var email = new EmailSuporteMaquininha(Remetente, Destinatario, Assunto).EnviarEmail(dicionario, templateHtml, repository);

            if (email)
            {
                return new HandlerResponse(true);

            }
            else
            {
                return new HandlerResponse(300, "Falha ao enviar o e-mail.");
            }
        }
    }
}
