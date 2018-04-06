using System.Web;
using System.Xml.Serialization;
using System.IO;
using System;
using Redecard.Portal.SharePoint.Client.WCF.SRVLoginLegado;

namespace Redecard.Portal.Helper
{

    //public enum CodigosRetornoLogin {
    //    [EnumMember]
    //    PVCanceladoPorFraude = 501,
    //    [EnumMember]
    //    PVCancelado = 502,
    //}
    
    /// <summary>
    /// Classe de namespaces e nomes comuns em todas as web parts/arquivos do portal Redecard
    /// </summary>
    public static class RedecardHelper
    {
        /// <summary>
        /// Retorna uma mensagem de erro customizada quando o acesso ao login é recusado
        /// pelo serviço de autenticação do legado
        /// </summary>
        public static string GetErrorMessage(CodigosRetornoLogin loginStatus) {
            string sMessage = string.Empty;
            switch (loginStatus) {
                case CodigosRetornoLogin.UsuarioNaoCadastrado:
                case CodigosRetornoLogin.UsuarioNaoCadastrado395:
                case CodigosRetornoLogin.SenhaIncorreta:
                case CodigosRetornoLogin.SenhaIncorretaMasTemSenhaTemporaria:
                    sMessage = "Usuário ou senha inválidos.";
                    break;
                case CodigosRetornoLogin.PVCanceladoPorFraude:
                case CodigosRetornoLogin.SenhaTemporariaIncorreta:
                    sMessage = "Seu usuário está bloqueado. Para solicitar o desbloqueio, entre em contato com nossa Central de Atendimento pelos telefones 4001-4433 (capitais e grandes cidades) e 0800-784433 (outras localidades).";
                    break;
                case CodigosRetornoLogin.AcessoBloqueadoMaisDeSeisTentativas:
                    sMessage = "Seu usuário foi bloqueado.";
                    break;
                case CodigosRetornoLogin.AcessoBloqueadoExcedeuTentativas:
                    sMessage = "Seu usuário está bloqueado.";
                    break;
            }
            return sMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginStatus"></param>
        /// <returns></returns>
        public static string GetErrorMessage(int loginNumber) {
            string sMsg = string.Empty;
            if (loginNumber < 0) {
                switch (loginNumber) {
                    case -1:
                        sMsg = "Não foi possível encontrar os paramêtros de login.";
                        break;
                    case -2:
                        sMsg = "Ocorreu uma falha ao instanciar os métodos de login.";
                        break;
                    case -99:
                        sMsg = "Sistema indisponível.";
                        break;
                    case -3:
                    default:
                        sMsg = "Logoff efetuado com sucesso.";
                        break;
                }
            }
            else {
                if (Enum.IsDefined(typeof(CodigosRetornoLogin), loginNumber)) {
                    CodigosRetornoLogin loginStatus = (CodigosRetornoLogin)Enum.Parse(typeof(CodigosRetornoLogin), loginNumber.ToString());
                    sMsg = RedecardHelper.GetErrorMessage(loginStatus);
                }
            }
            return sMsg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public static bool IsLoginSucess(CodigosRetornoLogin codigo) {
            // O PV Cancelado deve acessar o portal somente como leitura, tratamento deve ser realizado no legado.
            return codigo == CodigosRetornoLogin.LoginSucesso || codigo == CodigosRetornoLogin.SenhaTemporariaAntigaComAcessoOK || codigo == CodigosRetornoLogin.LoginRealizadoComSenhaTemporaria || codigo == CodigosRetornoLogin.PVCancelado || codigo == CodigosRetornoLogin.SenhaTemporariaNovaRequerValidacaoPositiva;
        }

        /// <summary>
        /// 
        /// </summary>
        //static RetornoLoginLegadoEstabelecimentoVO __loginData = null;

        /// <summary>
        /// Nome da categoria para as propriedades adicionais das web parts customizadas
        /// </summary>
        public const string _webPartsPropertiesConfigCategory = "Configurações";
        public const string _webPartsResourceFileName = "redecard";
        public const string _webPartsFechadoResourceFileName = "redecardFechado";

        /// <summary>
        /// Retorna um texto para uma determinada chave a partir do arquivo redecard.[idioma].resx
        /// (Este arquivo encontra-se no projeto Redecard.Portal.Aberto.SD, pasta RecursosApp)
        /// </summary>
        /// <param name="chave"></param>
        /// <returns></returns>
        public static string ObterResource(string chave)
        {
            return HttpContext.GetGlobalResourceObject(_webPartsResourceFileName, chave).ToString();
        }

        /// <summary>
        /// Retorna um texto para uma determinada chave a partir do arquivo redecardFechado.[idioma].resx
        /// (Este arquivo encontra-se no projeto Redecard.Portal.Fechado.SD, pasta RecursosApp)
        /// </summary>
        /// <param name="chave"></param>
        /// <returns></returns>
        public static string ObterResourceFechado(string chave)
        {
            return HttpContext.GetGlobalResourceObject(_webPartsFechadoResourceFileName, chave).ToString();
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public static RetornoLoginLegadoEstabelecimentoVO LoginData {
        //    get {
        //        if (object.ReferenceEquals(__loginData, null))
        //            __loginData = RedecardHelper.Deserializar(HttpContext.Current.Request.Cookies["LoginData"].Value, typeof(RetornoLoginLegadoEstabelecimentoVO)) as RetornoLoginLegadoEstabelecimentoVO;
        //        return __loginData;
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public static string Serializar(object obj) {
        //    XmlSerializer ser = new XmlSerializer(obj.GetType());
        //    StringWriter writer = new StringWriter();
        //    ser.Serialize(writer, obj);
        //    return writer.ToString();
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="_xml"></param>
        ///// <returns></returns>
        //public static object Deserializar(string _xml, Type objType) {
        //    XmlSerializer ser = new XmlSerializer(objType);
        //    StringReader reader = new StringReader(_xml);
        //    object obj = ser.Deserialize(reader);
        //    return obj;
        //}
    }
}