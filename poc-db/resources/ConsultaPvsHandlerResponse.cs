using System;
using System.Collections.Generic;

namespace Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario
{
    /// <summary>
    /// Model para comunicação usando Ajax
    /// </summary>
    [Serializable]
    public class ConsultaPvsHandlerResponse
    {
        /// <summary>
        /// Listagem de PVs retornadas
        /// </summary>
        public List<ConsultaPvsHandlerModel> Pvs { get; set; }

        /// <summary>
        /// Título da modal.
        /// </summary>
        public String TituloModal { get; set; }

        /// <summary>
        /// Verifica se o retorno exigirá a exibição da modal;
        /// </summary>
        public Boolean FlagModal { get; set; }

        /// <summary>
        /// Define o tipo da modal que irá aparecer (ex: Warning, Error, Success)
        /// </summary>
        public TipoModal TipoModal { get; set; }

        /// <summary>
        /// Verifica se é necessário disparar uma tag para o GTM.
        /// </summary>
        public Boolean DispararTagGtm { get; set; }

        /// <summary>
        /// Label a ser enviado para o Gtm.
        /// </summary>
        public String LabelGtm { get; set; }

        /// <summary>
        /// Mensagem a ser exibida ao usuário
        /// </summary>
        public string MensagemRetorno { get; set; }

        /// <summary>
        /// Código de status do response HTTP
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Detalhe da exception a ser repassada para o clientside
        /// </summary>
        public string DetalheExcecao { get; set; }

        /// <summary>
        /// Url para qual a página deverá ser redirecionada (caso houver).
        /// </summary>
        public String UrlRedirect { get; set; }

        /// <summary>
        /// Campo que receberá a validação
        /// </summary>
        public String FieldValidationReturn { get; set; }

        #region Exibicao dos componentes

        /// <summary>
        /// Verifica se o grid de envio do código de recuperação deve ser exibido.
        /// </summary>
        public Boolean ExibirPainelEnvioCodigo { get; set; }

        /// <summary>
        /// Verifica se o radio de email secundario deve ser exibido.
        /// </summary>
        public Boolean ExibirRadioEmailSecundario { get; set; }

        /// <summary>
        /// Verifica se o radio de SMS deve ser exibido.
        /// </summary>
        public Boolean ExibirRadioSMS { get; set; }

        /// <summary>
        /// Verifica se o grid de PV's deve ser exibido.
        /// </summary>
        public Boolean ExibirGridPvs { get; set; }

        /// <summary>
        /// Determina se deve exibir o Google Recaptcha na página
        /// </summary>
        public Boolean ExibirRecaptcha { get; set; }

        #endregion
    }

    /// <summary>
    /// Enumerador para identificar o tipo de mensageria para exibir via js
    /// </summary>
    public enum TipoModal
    {
        None = 0,
        Success = 1,
        Warning = 2,
        Error = 3,
    }
}
