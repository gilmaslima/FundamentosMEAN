using Rede.PN.ApiLogin.Core.Wcf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rede.PN.ApiLogin.Negocio.Core.Padrao
{
    public class MensagemRetorno
    {
        /// <summary>
        /// Gera painel de erro padrão do sistema
        /// </summary>
        /// <param name="codigo">Código do erro</param>
        /// <returns>Painel de erro padrão</returns>
        public static String RetornarMensagemErro(String fonte, Int32 codigo)
        {
            using (var contexto = new ContextoWcf<TrataErroServico.TrataErroServicoClient>())
            {
                String erroFormato = "{0} ({1})";
                var trataErro = contexto.Cliente.Consultar(fonte, codigo);

                if (trataErro.Codigo != 0)
                    return String.Format(erroFormato, trataErro.Fonte, trataErro.Codigo);
                else
                    return String.Format(erroFormato, "Sistema Indisponível", "-1");
            }
        }
    }
}
