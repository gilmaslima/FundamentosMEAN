using System;
using System.Text.RegularExpressions;
using Redecard.Portal.Helper.DTO;
using Redecard.Portal.Helper.Validacao;
using Redecard.Portal.Helper.Web.Mails;
using Redecard.Portal.Helper;

namespace Redecard.Portal.Aberto.WebParts.FormularioContato
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Data criação: meados de 25/09/2010
    /// Descrição: Classe responsável pela validação dos dados inseridos no formulário de contato com a Redecard
    /// </summary>
    public sealed class ValidadorFormularioContato : IValidacao<Contato>
    {
        /// <summary>
        /// Para cada violação de regra aqui implementada, adiciona um objeto Inconsistência ao objeto Sumário de erros e retorna
        /// </summary>
        /// <param name="contato"></param>
        /// <returns></returns>
        public SumarioValidacao Validar(Contato contato)
        {
            SumarioValidacao sumario = new SumarioValidacao();

            // É Nosso Cliente - Campo Vazio
            if (String.IsNullOrEmpty(contato.ECliente))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContato_Campo_ENossoCliente"), RedecardHelper.ObterResource("validadorFormularioContato_Campo_ENossoCliente_Branco")));

            // É Nosso Cliente está preenchido com "Sim" e Número Estabelecimento em branco
            if (contato.ECliente.ToLowerInvariant() == "sim" && String.IsNullOrEmpty(contato.NumeroEstabelecimento))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContato_Campo_NumeroEstabelecimento"), RedecardHelper.ObterResource("validadorFormularioContato_Campo_NumeroEstabelecimento_Branco")));

            // CNPJ/CPF em branco
            if (String.IsNullOrEmpty(contato.CNPJCPF))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContato_Campo_CNPJ_CPF"), RedecardHelper.ObterResource("validadorFormularioContato_Campo_CNPJ_CPF_Branco")));

            //Nome - Campo vazio?
            if(string.IsNullOrEmpty(contato.Nome))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContato_Campo_Nome"), RedecardHelper.ObterResource("validadorFormularioContato_Campo_Nome_Obrigatorio")));

            //Email - Campo vazio?
            if (string.IsNullOrEmpty(contato.Email))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContato_Campo_Email"), RedecardHelper.ObterResource("validadorFormularioContato_Campo_Email_Obrigatorio")));
            else
            {
                //Email - Formato válido?
                if (!EmailUtils.EnderecoEmailValido(contato.Email))
                    sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContato_Campo_Email"), string.Format(RedecardHelper.ObterResource("validadorFormularioContato_Campo_Email_Invalido"),contato.Email)));
            }

            //DDD - Campo vazio?
            //if (string.IsNullOrEmpty(contato.DDD))
            //    sumario.AdicionarInconsistencia(new Inconsistencia("DDD", "Campo DDD obrigatório"));
            if(!string.IsNullOrEmpty(contato.DDD))
            {
                //Comprimento do campo maior que 5?
                if(contato.DDD.Trim().Length > 5)
                    sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContato_Campo_DDD"), string.Format(RedecardHelper.ObterResource("validadorFormularioContato_Campo_DDD_DeveConterNDigitos"),contato.DDD,"5")));

                //É numérico?
                if(!Regex.IsMatch(contato.DDD.Trim(),@"^[0-9]{1,5}$"))
                    sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContato_Campo_DDD"), string.Format(RedecardHelper.ObterResource("validadorFormularioContato_Campo_DDD_Invalido"),contato.DDD)));
            }

            //Telefone - Campo vazio?
            //if (string.IsNullOrEmpty(contato.Telefone))
            //    sumario.AdicionarInconsistencia(new Inconsistencia("Telefone", "Campo telefone obrigatório"));
            if(!string.IsNullOrEmpty(contato.Telefone))
            {
                //Comprimento do campo maior que 15?
                if(contato.Telefone.Trim().Length > 15)
                    sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContato_Campo_Telefone"), string.Format(RedecardHelper.ObterResource("validadorFormularioContato_Campo_Telefone_DeveConterNDigitos"),contato.Telefone,"15")));

                //É numérico?
                if(!Regex.IsMatch(contato.Telefone.Trim(),@"^[0-9]{1,15}$"))
                    sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContato_Campo_Telefone"), string.Format(RedecardHelper.ObterResource("validadorFormularioContato_Campo_Telefone_Invalido"),contato.Telefone)));
            }

            //Estabelecimento - se o campo não for vazio....
            //if (!string.IsNullOrEmpty(contato.NumeroEstabelecimento.Trim()))
            //{
            //    //É numérico?
            //    if (!Regex.IsMatch(contato.NumeroEstabelecimento.Trim(), @"^[0-9]{1,}$"))
            //        sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContato_Campo_Estabelecimento"),string.Format(RedecardHelper.ObterResource("validadorFormularioContato_Campo_Estabelecimento_Invalido"),contato.NumeroEstabelecimento)));
            //}

            //Motivo do contato - Campo selecionado?
            if(contato.Motivo.EmailDestinatario.Equals("-1"))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContato_Campo_MotivoContato"), RedecardHelper.ObterResource("validadorFormularioContato_Campo_MotivoContato_Obrigatorio")));

            //Mensagem - Campo vazio?
            if (string.IsNullOrEmpty(contato.Mensagem))
                sumario.AdicionarInconsistencia(new Inconsistencia(RedecardHelper.ObterResource("validadorFormularioContato_Campo_Mensagem"), RedecardHelper.ObterResource("validadorFormularioContato_Campo_Mensagem_Obrigatorio")));

            return sumario;
        }
    }
}