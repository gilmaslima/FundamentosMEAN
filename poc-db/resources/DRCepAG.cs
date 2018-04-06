using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Redecard.PN.Comum;
using Redecard.PN.Agentes.DRWS;

namespace Redecard.PN.Agentes
{
    public class DRCepAG : AgentesBase
    {
        public Int32 BuscaLogradouro(String cep, ref String endereco, ref String bairro, ref String cidade, ref String uf)
        {
            using (DRWSSoapClient client = new DRWSSoapClient())
            {
                String strXML = String.Empty;
                String strAUX = String.Empty;

                Int32 codRetorno = client.RecuperarLogradouro("", cep, 100, ref strXML, ref strAUX);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(strXML);

                if (codRetorno == 38)
                {
                    XmlNode xNode = xmlDoc.SelectSingleNode(@"Root/Logradouros");

                    endereco = xNode.SelectSingleNode("Endereco").InnerText;
                    bairro = xNode.SelectSingleNode("Bairro").InnerText;
                    cidade = xNode.SelectSingleNode("Cidade").InnerText;
                    uf = xNode.SelectSingleNode("UF").InnerText;
                }
                else if (codRetorno == 1)
                {
                    XmlNode xNode = xmlDoc.SelectSingleNode(@"Root/Logradouros");

                    endereco = String.Empty;
                    bairro = String.Empty;
                    cidade = xNode.SelectSingleNode("Cidade").InnerText;
                    uf = xNode.SelectSingleNode("UF").InnerText;
                }
                else
                {
                    XmlNode xNode = xmlDoc.SelectSingleNode(@"Root");

                    String mensagem = xNode.SelectSingleNode("Mensagem").InnerText;
                    throw new PortalRedecardException(codRetorno, FONTE, mensagem, new Exception());
                }

                return codRetorno;
            }
        }
    }
}
