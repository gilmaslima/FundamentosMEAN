using System;
using Microsoft.SharePoint;
using System.Web;
using System.Web.Script.Serialization;
using System.ServiceModel;
using Redecard.PN.Comum.SharePoint.EntidadeServico;
using System.Collections.Generic;
using System.Linq;
using System.Web.SessionState;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.Comum.SharePoint.DRConsultaCepServico;
using System.Text.RegularExpressions;

namespace Redecard.PN.Comum.SharePoint.Handlers
{
    public partial class ConsultarCEP : ApplicationPageBaseAnonima, IHttpHandler, IReadOnlySessionState
    {
        #region [ Propriedades ]

        /// <summary>JS Serializer</summary>
        private static JavaScriptSerializer _jsSerializer;
        private static JavaScriptSerializer JsSerializer
        {
            get
            {
                return _jsSerializer ?? (_jsSerializer = new JavaScriptSerializer());
            }
        }

        /// <summary>Valor padrão de propriedades</summary>
        public bool IsReusable { get { return false; } }

        #endregion

        public void ProcessRequest(HttpContext context)
        {
            using (Logger Log = Logger.IniciarLog("Handler de Consulta de CEP"))
            {
                try
                {
                    ////Valida se usuário está autenticado
                    //if (SessaoAtual == null)
                    //{
                    //    GerarRespostaJSON(context, -1, "Acesso negado. Usuário não autenticado.");
                    //    return;
                    //}

                    //Obtém os parâmetros para a consulta de CEP
                    String _cep = context.Request["cep"];

                    //Formata o CEP e valida
                    _cep = FormataCEP(_cep);
                    if (String.IsNullOrEmpty(_cep))
                    {
                        GerarRespostaJSON(context, -1, "CEP inválido.");
                        return;
                    }

                    //Gera resposta contendo dados do CEP
                    GerarRespostaJSON(context, BuscarCEP(_cep));                   
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    GerarRespostaJSON(context, -1, "Erro durante inicialização de Handler de consulta CEP.");
                }
            }
        }

        #region [ Operações ]

        /// <summary>
        /// Formata o CEP (Deve estar nos formatos DDDDD-DDD ou DDDDDDDD)
        /// </summary>
        /// <param name="cep">CEP (Deve estar nos formatos DDDDD-DDD ou DDDDDDDD)</param>
        /// <returns>CEP formatado, ou String.Empty caso CEP inválido</returns>
        private String FormataCEP(String cep)
        {
            cep = cep.Trim();
            Regex regexCEP_1 = new Regex(@"^\d{5}-\d{3}$");
            Regex regexCEP_2 = new Regex(@"^\d{8}$");

            Boolean cepValido = regexCEP_1.IsMatch(cep) || regexCEP_2.IsMatch(cep);
            if (cepValido)
            {
                cep = cep.Replace("-", "");
                return cep;
            }
            else
                return String.Empty;
        }

        /// <summary>
        /// Consulta o CEP.
        /// </summary>
        /// <param name="cep">CEP (Deve estar nos formatos DDDDD-DDD ou DDDDDDDD)</param>
        private Object BuscarCEP(String cep)
        {
            //Variáveis de retorno da chamada do serviço de consulta CEP
            String _endereco = String.Empty;
            String _bairro = String.Empty;
            String _cidade = String.Empty;
            String _uf = String.Empty;
            Int32 _codigoRetorno = default(Int32);
            String _mensagem = String.Empty;

            if (!String.IsNullOrEmpty(cep))
            {
                try
                {
                    using (DRCepServicoClient client = new DRCepServicoClient())
                    {
                        //Consulta CEP. Em caso de erro, retorna todos os dados do cep em branco (não gera exceção)                    
                        client.BuscaLogradouro(cep, ref _endereco, ref _bairro, ref _cidade, ref _uf);
                    }
                }
                catch (Exception ex)
                {
                    _codigoRetorno = -1;
                    _mensagem = "Erro Handler de Consulta de CEP: " + cep;
                    SharePointUlsLog.LogErro(ex);
                    Logger.GravarErro("Erro Handler de Consulta de CEP: " + cep, ex);
                }
            }

            //Prepara objeto de retorno que será serializado como resposta do handler
            return new
            {
                codigoRetorno = _codigoRetorno,
                mensagem = _mensagem,
                endereco = _endereco,
                cep = cep,
                uf = _uf,
                bairro = _bairro,
                cidade = _cidade
            };
        }

        #endregion

        #region [ Auxiliares ]

        private void GerarRespostaJSON(HttpContext context, Object dados)
        {
            context.Response.Write(JsSerializer.Serialize(dados));
            context.Response.ContentType = "application/json";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }

        private void GerarRespostaJSON(HttpContext context, Int32 codigoRetorno, String mensagem)
        {
            GerarRespostaJSON(context, new { codigoRetorno, mensagem });
        }

        #endregion
    }
}
