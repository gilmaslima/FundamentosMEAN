using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.Portal.Helper.Validacao;
using Redecard.Portal.Helper.DTO;
using Redecard.Portal.Helper.Web.Mails;
using System.Text.RegularExpressions;
using Redecard.Portal.Helper;

namespace Redecard.Portal.Aberto.WebParts.FormularioContatoComOuvidoria
{
    /// <summary>
    /// Autora: Adriana da Silva de Sena
    /// Data criação: 05/10/2010
    /// Descrição: classe responsável pela validação dos dados inseridos no formulário de contato com a ouvidoria
    /// </summary>
    public sealed class ValidadorFormularioContatoComOuvidoria :IValidacao<Contato>
    {
        /// <summary>
        /// Para cada violação de regra aqui implementada, adiciona um objeto Inconsistência ao objeto Sumário de erros e retorna
        /// </summary>
        /// <param name="contato"></param>
        /// <returns></returns>
        public SumarioValidacao Validar(Contato contato)
        {
            SumarioValidacao sumario = new SumarioValidacao();

            //Tipo de Pessoa - Campo selecionado?
            if (contato.TipoPessoa.Equals(RedecardHelper.ObterResource("formContato_SelecioneUmaOpcao")))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_TipoPessoa"), RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_TipoPessoa_Obrigatorio")));

            //Estabelecimento - campo vazio?
            //if (string.IsNullOrEmpty(contato.NumeroEstabelecimento))
            //    sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_Estabelecimento"), RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_Estabelecimento_Obrigatorio")));
            //else
            //{
            //    //É numérico?
            //    if (!Regex.IsMatch(contato.NumeroEstabelecimento.Trim(), @"^[0-9]{1,}$"))
            //        sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_Estabelecimento"), string.Format("validadorFormularioContatoOuvidoria_Campo_Estabelecimento_Invalido",contato.NumeroEstabelecimento)));
            //}
            // CNPJ/ CPF - campo vazio?
            if (string.IsNullOrEmpty(contato.CNPJCPF))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_CNPJCPF"), RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_CNPJCPF_Obrigatorio")));
            //else
            //{
            //    //É numérico?
            //    if (!Regex.IsMatch(contato.CNPJCPF.Trim(), @"^[0-9]{1,}$"))
            //        sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_CNPJCPF"), string.Format(RedcardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_CNPJCPF_Invalido"),contato.CNPJCPF)));
            //}

            //Nome - Campo vazio?
            if (string.IsNullOrEmpty(contato.Nome))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_Nome"), RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_Nome_Obrigatorio")));

            //Email - Campo vazio?
            if (string.IsNullOrEmpty(contato.Email))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_Email"), RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_Email_Obrigatorio")));
            else
            {
                //Email - Formato válido?
                if (!EmailUtils.EnderecoEmailValido(contato.Email))
                    sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_Email"), string.Format(RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_Email_Invalido"), contato.Email)));
            }

            //Telefone - Campo vazio?
            if (string.IsNullOrEmpty(contato.Telefone))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_Telefone"), RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_Telefone_Obrigatorio")));
            else
            {
                //Comprimento do campo maior que 15?
                if (contato.Telefone.Trim().Length > 15)
                    sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_Telefone"), string.Format(RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_Telefone_DeveConterNDigitos"), contato.Telefone, "15")));

                //É numérico?
                if (!Regex.IsMatch(contato.Telefone.Trim(), @"^[0-9]{1,15}$"))
                    sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_Telefone"), string.Format(RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_Telefone_Invalido"), contato.Telefone)));
            }

            //Motivo do contato - Campo selecionado?
            if (contato.Motivo.EmailDestinatario.Equals("-1"))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_MotivoContato"), RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_MotivoContato_Obrigatorio")));

            //Mensagem - Campo vazio?
            if (string.IsNullOrEmpty(contato.Mensagem))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_Mensagem"), RedecardHelper.ObterResource("validadorFormularioContatoOuvidoria_Campo_Mensagem_Obrigatorio")));

            return sumario;
        }
    }
}