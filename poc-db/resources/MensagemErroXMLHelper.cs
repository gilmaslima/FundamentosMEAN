/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Redecard.PN.FMS.Comum
{
    /// <summary>
    /// Este componente publica a classe MensagemErroXMLHelper, que expõe métodos para manipular as mensgens de erro orieundo do arquivo XML.
    /// </summary>
    public class MensagemErroXMLHelper
    {
        private static XmlDocument _xmlDocument = ObterDocumentoXMLDeErros();

        /// <summary>
        /// Este método é utilizado para abrir o arquivo XML.
        /// </summary>
        /// <returns></returns>
        private static XmlDocument ObterDocumentoXMLDeErros()
        {
            XmlDocument result = new XmlDocument();
            try
            {
                Stream streamXML = Assembly.GetExecutingAssembly().GetManifestResourceStream("Redecard.PN.FMS.Comum.Exception.MensagensErro.xml");

                string[] a = Assembly.GetExecutingAssembly().GetManifestResourceNames();

                Console.Write(a);

                result.Load(streamXML);
            }
            catch (Exception ex)
            {
                LogHelper.GravarErrorLog(ex);
            }
            return result;
        }

        /// <summary>
        /// Obtém dados da mensagem do Erro por Código de Erro
        /// </summary>
        /// <param name="codigoErro"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ObterDadosErroPorCodigoErro(int codigoErro)
        {
            LogHelper.GravarTraceLog(string.Format("ObterDadosErroPorCodigoErro[Código erro: {0}]", codigoErro));

            string query = String.Format("/MensagensErroFMS/MensagemErroFMS[@codigoFMS=\"{0}\"]", codigoErro.ToString());

            Dictionary<string, string> retorno = new Dictionary<string, string>();

            XmlNode elemento = _xmlDocument.SelectSingleNode(query);

            if (elemento != null)
            {
                retorno.Add("mensagem", elemento.Attributes["mensagem"].Value);
                retorno.Add("ehExcecao", elemento.Attributes["ehExcecao"].Value);
            }
            return retorno;
        }


    }
}
