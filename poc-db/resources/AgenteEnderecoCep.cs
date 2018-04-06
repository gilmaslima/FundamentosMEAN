/*
© Copyright 2017 Rede S.A.
Autor : Mário Neto
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Xml;
using Rede.PN.AtendimentoDigital.Modelo.Agente;
using Rede.PN.AtendimentoDigital.Modelo.Core.Wcf;
using Rede.PN.AtendimentoDigital.Modelo.Entidades;
using Rede.PN.AtendimentoDigital.Dados.DRWS;
using System.Collections.Generic;

namespace Rede.PN.AtendimentoDigital.Dados.Agente
{
    /// <summary>
    /// Agente similar ao da SIGLA DR para CEP, comunicação com SOAP Service DRWS.
    /// </summary>
    public class AgenteEnderecoCep : IAgenteEnderecoCep
    {
        /// <summary>
        /// Busca Endereço a partir do CEP informado
        /// </summary>
        public EntidadeEnderecoCep BuscarEnderecoPorCep(String cep)
        {
            using (ContextoWcf<DRWSSoapClient> contexto = new ContextoWcf<DRWSSoapClient>())
            {
                EntidadeEnderecoCep retorno;
                String strXML = String.Empty;
                String strAUX = String.Empty;

                Int32 codRetorno = contexto.Cliente.RecuperarLogradouro("", cep, 100, ref strXML, ref strAUX);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(strXML);

                XmlNode xNode = codRetorno == 38 || codRetorno == 1 ? xmlDoc.SelectSingleNode(@"Root/Logradouros") : xmlDoc.SelectSingleNode(@"Root");

                //Retorno Regular
                if (codRetorno == 38)
                {
                    retorno = new EntidadeEnderecoCep
                    {
                        CodigoRetorno = 0,
                        Mensagem = "Obtido com sucesso.",
                        Endereco = xNode.SelectSingleNode("Endereco").InnerText,
                        Bairro = xNode.SelectSingleNode("Bairro").InnerText,
                        Cidade = xNode.SelectSingleNode("Cidade").InnerText,
                        Uf = xNode.SelectSingleNode("UF").InnerText,
                        Cep = cep,
                        CepUnico = false
                    };
                }
                //Retorno para CEP único
                else if (codRetorno == 1)
                {
                    retorno = new EntidadeEnderecoCep
                    {
                        CodigoRetorno = 0,
                        Mensagem = "Obtido com sucesso. - CEP único por Cidade e/ou vários Logradouros.",
                        Endereco = String.Empty,
                        Bairro = String.Empty,
                        Cidade = xNode.SelectSingleNode("Cidade").InnerText,
                        Uf = xNode.SelectSingleNode("UF").InnerText,
                        Cep = cep,
                        CepUnico = true
                    };
                }
                else
                {
                    retorno = new EntidadeEnderecoCep
                    {
                        CodigoRetorno = codRetorno == 0 ? 304 : codRetorno,
                        Mensagem = xNode.SelectSingleNode("Mensagem").InnerText
                    };
                }

                return retorno;
            }
        }

        #region [Interface Members]
        /// <summary>
        /// Metodo Listar
        /// </summary>
        /// <param name="entidade">entidade</param>
        public List<EntidadeEnderecoCep> Listar(EntidadeEnderecoCep entidade)
        {
            return null;
        }

        /// <summary>
        /// Metodo Obter
        /// </summary>
        /// <param name="entidade">entidade</param>
        public EntidadeEnderecoCep Obter(EntidadeEnderecoCep entidade)
        {
            return null;
        }
        #endregion
    }
}




