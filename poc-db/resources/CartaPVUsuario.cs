using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.Agentes
{
    public class CartaPVUsuario : AgentesBase
    {
        /// <summary>
        /// Gera carta para envio de uma nova senha para um usuário de um PV
        /// </summary>
        /// <param name="numeroPV">Número do PV</param>
        /// <param name="codigoUsuario">Código do Usuário</param>
        /// <param name="senha">Senha</param>
        /// <param name="nomeUsuario">Nome do usuário</param>
        /// <param name="codigoCarta">Código da Carta</param>
        /// <param name="tipoEnvio">Tipo de Envio</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        public void Gerar(Int32 numeroPV,
            String codigoUsuario,
            String senha,
            String nomeUsuario,
            Int16 codigoCarta,
            String tipoEnvio,
            String enderecoEmail,
            out Int16 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Gera carta para envio de uma nova senha para um usuário de um PV"))
            {
                try
                {
                    String mensagemErro = default(String);
                    codigoUsuario = (!String.IsNullOrEmpty(codigoUsuario) ? (codigoUsuario.Length > 20 ? codigoUsuario.Substring(0, 20).Trim() : codigoUsuario.Trim()) : "");
                    nomeUsuario = (!String.IsNullOrEmpty(nomeUsuario) ? (nomeUsuario.Length > 50 ? nomeUsuario.Substring(0, 50).Trim() : nomeUsuario.Trim()) : "");
                    Log.GravarLog(EventoLog.InicioAgente);
                    Log.GravarLog(EventoLog.ChamadaHIS, new
                    {
                        numeroPV,
                        codigoUsuario,
                        senha,
                        nomeUsuario,
                        codigoCarta,
                        tipoEnvio,
                        enderecoEmail
                    });



#if !DEBUG
                    using (COMTIWADadosCadastrais.COMTIWAClient cartaClient = new COMTIWADadosCadastrais.COMTIWAClient())
                    {

                        codigoRetorno = cartaClient.GerarCarta(
                            out mensagemErro,
                            numeroPV,
                            (!String.IsNullOrEmpty(codigoUsuario) ? (codigoUsuario.Length > 20 ? codigoUsuario.Substring(0, 20).Trim() : codigoUsuario.Trim()) : ""),
                            senha,
                            (!String.IsNullOrEmpty(nomeUsuario) ? (nomeUsuario.Length > 50 ? nomeUsuario.Substring(0, 50).Trim() : nomeUsuario.Trim()) : ""),
                            codigoCarta,
                            tipoEnvio,
                            enderecoEmail, // Colocar endereço de e-mail 
                            "");
                    }
#else
                codigoRetorno = 0;
#endif
                    Log.GravarLog(EventoLog.RetornoHIS, new { mensagemErro });
                    Log.GravarLog(EventoLog.FimAgente, new { codigoRetorno });
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
    }
}
