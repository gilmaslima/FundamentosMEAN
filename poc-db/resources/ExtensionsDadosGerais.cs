using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario
{
    /// <summary>
    /// Classe auxiliar para formatação de dados gerais (cpf/cnpj, telefone, cep, número, etc)
    /// </summary>
    public static class ExtensionsDadosGerais
    {
        /// <summary>
        /// Formata CPF/CNPJ com máscara
        /// </summary>
        /// <param name="cpfCnpj">CNPJ a ser formatado</param>
        /// <returns>CPF/CNPJ formatado</returns>
        public static String FormatarCnpjCnpj(this String cpfCnpj)
        {
            cpfCnpj = Regex.Replace(cpfCnpj, "[^0-9]", "");

            MaskedTextProvider maskProvider = null;
            if (cpfCnpj.Length <= 11)
            {
                maskProvider = new MaskedTextProvider(@"000\.000\.000-00");
                cpfCnpj = cpfCnpj.PadLeft(11, '0');
            }
            else
            {
                maskProvider = new MaskedTextProvider(@"00\.000\.000/0000-00");
                cpfCnpj = cpfCnpj.PadLeft(14, '0');
            }

            maskProvider.Set(cpfCnpj);
            return maskProvider.ToString();
        }

        /// <summary>
        /// Trata número com conteúdo nulo ("0" ou "") como "-"
        /// </summary>
        /// <param name="valor"></param>
        /// <returns>Se ver "0" ou em branco, trata como "-"</returns>
        public static string TratarValorNulo(this String valor)
        {
            return String.IsNullOrWhiteSpace(valor) || String.Compare(valor.Trim(), "0") == 0 ? "-" : valor;
        }

        /// <summary>
        /// Formata telefone com máscara
        /// </summary>
        /// <param name="telefone">Telefone a ser formatado com máscara</param>
        /// <param name="ddd">DDD acompanhando o telefone</param>
        /// <param name="ramal">Ramal acompanhando o telefone</param>
        /// <returns>Telefone formatado como (00) 0000[0]-0000</returns>
        public static string FormatarTelefone(this String telefone, String ddd, String ramal)
        {
            ddd = Regex.Replace(ddd, "[^0-9]", "");
            telefone = Regex.Replace(telefone, "[^0-9]", "");
            ramal = Regex.Replace(ramal, "[^0-9]", "");

            String retorno = String.Empty;

            if (!String.IsNullOrEmpty(ddd) && !String.Equals(ddd, "0"))
            {
                if (ddd.Length > 2)
                {
                    if (ddd.StartsWith("0"))
                        ddd = ddd.Substring(ddd.Length - 2, 2);
                    else
                        ddd = ddd.Substring(0, 2);
                }

                MaskedTextProvider maskDddProvider = new MaskedTextProvider("(00) ");
                maskDddProvider.Set(ddd);
                retorno = maskDddProvider.ToString();
            }

            if (telefone.Length >= 8)
            {
                MaskedTextProvider maskTelefoneProvider = null;
                if (telefone.Length >= 9)
                    maskTelefoneProvider = new MaskedTextProvider("00000-000099999");
                else
                    maskTelefoneProvider = new MaskedTextProvider("0000-0000");

                maskTelefoneProvider.Set(telefone);

                retorno = String.Format("{0}{1}", retorno, maskTelefoneProvider.ToString());
            }

            if (!String.IsNullOrEmpty(ramal) && String.Compare(ramal, "0") != 0)
                retorno = String.Format("{0} {1}", retorno, ramal);

            return retorno;
        }

        /// <summary>
        /// Formata telefone com máscara
        /// </summary>
        /// <param name="telefone">Telefone a ser formatado com máscara</param>
        /// <param name="ddd">DDD acompanhando o telefone</param>
        /// <returns>Telefone formatado como (00) 0000[0]-0000</returns>
        public static string FormatarTelefone(this String telefone, String ddd)
        {
            return FormatarTelefone(telefone, ddd, String.Empty);
        }

        /// <summary>
        /// Formata telefone com máscara
        /// </summary>
        /// <param name="telefoneDddRamal">Telefone com DDD e possivelmente ramal</param>
        /// <returns>Telefone formatado como (00) 0000[0]-0000</returns>
        public static string FormatarTelefone(this String telefoneDddRamal)
        {
            // prevê caracteres não numéricos
            telefoneDddRamal = Regex.Replace(telefoneDddRamal, "[^0-9 ]", "");

            // remove espaços duplicados
            telefoneDddRamal = Regex.Replace(telefoneDddRamal, @"\s\s+", " ");
            
            if (String.IsNullOrWhiteSpace(telefoneDddRamal))
                return String.Empty;

            // quebra os dados por espaço
            String[] dadosTelefone = telefoneDddRamal.Split(' ');
            String ddd = dadosTelefone[0];
            String telefone = dadosTelefone.Length >= 2 ? dadosTelefone[1] : String.Empty;
            String ramal = dadosTelefone.Length >= 3 ? dadosTelefone[2] : String.Empty;

            if (String.IsNullOrEmpty(telefone) && ddd.Length > 4)
            {
                telefone = ddd;
                ddd = String.Empty;
            }

            return FormatarTelefone(telefone, ddd, ramal);
        }
    }
}
