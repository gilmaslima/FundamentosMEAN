using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Redecard.PN.Comum
{
    public static class SMS
    {
        /// <summary>
        /// Envia mensagem de recuperação de senha para o celular informado
        /// </summary>
        /// <param name="usuarioSms">Usuário Criptografado</param>
        /// <param name="senhaSms">Senha Criptografada</param>
        /// <param name="codigoIdUsuario">Código ID do usuário do Portal</param>
        /// <param name="celular">Número do Celular</param>
        /// <param name="mensagem">Mensagem</param>
        /// <returns>OK, NOK, NOK2, NA</returns>
        public static String EnviaSMSRecuperacaoSenha(String usuarioSms, String senhaSms,
            Int32 codigoIdUsuario, String celular, String mensagem)
        {
            String retorno = String.Empty;
            EnumerableRowCollection<DataRow> rowCollection = null;

            using (var log = Logger.IniciarLog("Envia SMS para recuperação de senha"))
            {
                log.GravarLog(EventoLog.InicioServico,
                    new { usuarioSms, senhaSms, celular, mensagem, codigoIdUsuario });

                DateTime dataInicio = DateTime.Now.AddMinutes(-20);
                DateTime dataFim = DateTime.Now;
                //O id da mensagem é automaticamente truncado em 20 caracteres pelo serviço da TWW
                String idMensagemRecuperacao = String.Concat(codigoIdUsuario, "R", celular).Left(20);

                DataSet mensagensEnviadasDs = SMS.BuscaSMS(usuarioSms, senhaSms, dataInicio, dataFim);

                //Verifica se existe mensagem enviada para o celular nos últimos 20min
                
                if(mensagensEnviadasDs != null 
                    && mensagensEnviadasDs.Tables.Count > 0 
                    && mensagensEnviadasDs.Tables.Contains("BuscaSMS") 
                    && mensagensEnviadasDs.Tables["BuscaSMS"].Rows.Count > 0)
                    rowCollection = mensagensEnviadasDs.Tables["BuscaSMS"].AsEnumerable();

                if(rowCollection != null && rowCollection.Any())
                {
                    var mensagens = rowCollection.
                    Select(mensagemEnviada => new
                    {
                        Celular = mensagemEnviada.Field<String>("celular") != null ? mensagemEnviada.Field<String>("celular").Trim() : string.Empty,
                        DataEnvio = mensagemEnviada.Field<DateTime>("dataenv") != null ? mensagemEnviada.Field<DateTime>("dataenv") : DateTime.MinValue, 
                        IdMensagem = mensagemEnviada.Field<String>("seunum") != null ? mensagemEnviada.Field<String>("seunum").Trim() : string.Empty
                    }).ToList();

                    if (mensagens != null && mensagens.Any(m => m.Celular.Trim().Equals(celular)
                                                           && m.DataEnvio > dataInicio
                                                           && idMensagemRecuperacao.Equals(m.IdMensagem)))
                    // Não envia mensagem
                    {
                        retorno = "NOK2";
                        return retorno;
                    }
                }
                
                // Envia nova mensagem
                retorno = SMS.EnviaSMS(usuarioSms, senhaSms, celular, mensagem, idMensagemRecuperacao);
                
                log.GravarLog(EventoLog.FimServico, new { retorno, idMensagemRecuperacao });
            }

            return retorno;
        }

        /// <summary>
        /// Busca SMSs enviados em um período
        /// </summary>
        /// <param name="usuario">Usuário Criptografado</param>
        /// <param name="senha">Senha Criptografada</param>
        /// <param name="dataInicio">Data de Início</param>
        /// <param name="dataFim">Data Final</param>
        /// <returns></returns>
        public static DataSet BuscaSMS(String usuario, String senha, DateTime dataInicio, DateTime dataFim)
        {
            DataSet retorno = new DataSet();

            using (var log = Logger.IniciarLog("Busca SMS"))
            {
                log.GravarLog(EventoLog.InicioServico,
                    new { usuario, senha, dataInicio, dataFim });

                String usuarioDescriptografado = Criptografia.Decrypt(usuario);
                String senhaDescriptografada = Criptografia.Decrypt(senha);

                using (var contexto = new ContextoWCF<SMSService.ReluzCapWebServiceSoapClient>())
                {
                    retorno = contexto.Cliente.BuscaSMS(usuarioDescriptografado, senhaDescriptografada, dataInicio, dataFim);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Envia SMS para um celular
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="senha"></param>
        /// <param name="celular"></param>
        /// <param name="mensagem"></param>
        /// <param name="idMensagem">Id para identificação de mensagens de recupeção de acesso</param>
        /// <returns></returns>
        public static String EnviaSMS(String usuario, String senha, String celular, String mensagem, String idMensagem = "")
        {
            String retorno = String.Empty;

            using (var log = Logger.IniciarLog("Envia SMS"))
            {
                log.GravarLog(EventoLog.InicioServico,
                    new { usuario, senha, celular, mensagem, idMensagem });

                String usuarioDescriptografado = Criptografia.Decrypt(usuario);
                String senhaDescriptografada = Criptografia.Decrypt(senha);

                using (var contexto = new ContextoWCF<SMSService.ReluzCapWebServiceSoapClient>())
                {
                    retorno = contexto.Cliente.EnviaSMS(usuarioDescriptografado, senhaDescriptografada, idMensagem, celular, mensagem);
                }

                log.GravarLog(EventoLog.FimServico, retorno);
            }

            return retorno;
        }
    }
}
