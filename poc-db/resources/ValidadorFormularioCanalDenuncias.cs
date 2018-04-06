using System;
using System.Text.RegularExpressions;
using Redecard.Portal.Helper.DTO;
using Redecard.Portal.Helper.Validacao;
using Redecard.Portal.Helper.Web.Mails;
using Redecard.Portal.Helper;

namespace Redecard.Portal.Aberto.WebParts.FormularioCanalDenuncias
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Data criação: 06/11/2010
    /// Descrição: Classe responsável pela validação dos dados inseridos no formulário de canal de denúncias
    /// </summary>
    public sealed class ValidadorFormularioCanalDenuncias : IValidacao<Contato>
    {
        /// <summary>
        /// Para cada violação de regra aqui implementada, adiciona um objeto Inconsistência ao objeto Sumário de erros e retorna
        /// </summary>
        /// <param name="contato"></param>
        /// <returns></returns>
        public SumarioValidacao Validar(Contato contato)
        {
            SumarioValidacao sumario = new SumarioValidacao();

            //Nome - Campo vazio?
            if(string.IsNullOrEmpty(contato.Nome))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_Nome"), RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_Nome_Obrigatorio")));

            //Email - Campo vazio?
            if (string.IsNullOrEmpty(contato.Email))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_Email"), RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_Email_Obrigatorio")));
            else
            {
                //Email - Formato válido?
                if (!EmailUtils.EnderecoEmailValido(contato.Email))
                    sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_Email"), string.Format(RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_Email_Invalido"), contato.Email)));
            }

            //DDD - Campo vazio?
            //if (string.IsNullOrEmpty(contato.DDD))
            //    sumario.AdicionarInconsistencia(new Inconsistencia("DDD", "Campo DDD obrigatório"));
            if(!string.IsNullOrEmpty(contato.DDD))
            {
                //Comprimento do campo maior que 5?
                if(contato.DDD.Trim().Length > 5)
                    sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_DDD"), string.Format(RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_DDD_DeveConterNDigitos"), contato.DDD, "5")));

                //É numérico?
                if(!Regex.IsMatch(contato.DDD.Trim(),@"^[0-9]{1,5}$"))
                    sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_DDD"), string.Format(RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_DDD_Invalido"), contato.DDD)));
            }

            //Telefone - Campo vazio?
            //if (string.IsNullOrEmpty(contato.Telefone))
            //    sumario.AdicionarInconsistencia(new Inconsistencia("Telefone", "Campo telefone obrigatório"));
            if(!string.IsNullOrEmpty(contato.Telefone))
            {
                //Comprimento do campo maior que 15?
                if(contato.Telefone.Trim().Length > 15)
                    sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_Telefone"), string.Format(RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_Telefone_DeveConterNDigitos"), contato.Telefone, "15")));

                //É numérico?
                if(!Regex.IsMatch(contato.Telefone.Trim(),@"^[0-9]{1,15}$"))
                    sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_Telefone"), string.Format(RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_Telefone_Invalido"), contato.Telefone)));
            }

            //Motivo do contato - Campo selecionado?
            if(contato.Motivo.EmailDestinatario.Equals("-1"))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_MotivoContato"), RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_MotivoContato_Obrigatorio")));

            //Mensagem - Campo vazio?
            if (string.IsNullOrEmpty(contato.Mensagem))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_Mensagem"), RedecardHelper.ObterResource("validadorFormularioCanalDenuncias_Campo_Mensagem_Obrigatorio")));

            return sumario;
        }
    }
}