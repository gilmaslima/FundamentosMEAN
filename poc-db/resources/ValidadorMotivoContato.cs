using System;
using System.Linq;
using System.Text.RegularExpressions;
using Redecard.Portal.Helper.Validacao;
using Redecard.Portal.Helper.Web.Mails;

namespace Redecard.Portal.Aberto.WebParts.FormularioContato
{
    /// <summary>
    /// Autora: Adriana da Silva de Sena
    /// Data da criação: 23/09/2010
    /// Descrição: Classe responsável pela validação dos itens de Motivo de Contato que aparecem na combo
    /// "Motivo do Contato" nos formulários de Contato do site
    /// </summary>
    public class ValidadorMotivoContato : IValidacao<string>
    {
        /// <summary>
        /// Cria um objeto SumarioValidacao resultante da lógica de validação
        /// Os dados do motivo deverão ser retornados no formato [motivo];[email](opcional)\r\n em que \r\n = ENTER inserido no campo            
        /// Exemplo de dois motivos inseridos:
        ///    Esqueci minha senha;esquecisenha@redecard.com.br\r\nDúvidas;duvidas@redecard.com.br
        /// A lógica abaixo realiza a quebra de cada motivo usando split e valida estes itens considerando o seguinte:
        ///  - Obrigatoriedade do campo [motivo]
        ///  - Restrição de caracteres no campo [motivo]
        ///  - Validação do campo [email] (importante: ele valida se o e-mail está num formato aceitável; não valida se o endereço existe)
        /// Quanto há dados inválidos, é adicionado ao objeto SumarioValidacao a descrição do erro (objeto Inconsistencia)
        /// Última atualização: 10/01/2010
        /// Autor atualização: Cristiano Dias
        /// </summary>
        /// <param name="motivos"></param>
        /// <returns></returns>
        public SumarioValidacao Validar(string motivos)
        {
            SumarioValidacao sumario = new SumarioValidacao();

            #region Comentado
            /*
            //Obrigatoriedade do campo Motivo
            if (string.IsNullOrEmpty(motivos))
            {
                sumario.AdicionarInconsistencia(new Inconsistencia("Motivo", "Informe no mínimo um motivo de contato"));
                return sumario;
            }

            //Conjunto de caracteres inválidos para descrição dos motivos
            string caracteresInvalidos = @"@#$%*(){}[]/\";

            //Quebra cada item de motivo e atribui ao array
            string[] entrada = motivos.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            //Avalia cada item de motivo informado
            foreach (string motivo in entrada)
            {
                string[] descricoesMotivoEmail = motivo.Split(';');

                //Verifica se no motivo há letras inválidas com base na lista de caracteres inválidos
                if (descricoesMotivoEmail[0].IndexOfAny(caracteresInvalidos.ToCharArray()) != -1)
                {
                    Inconsistencia inconsistencia = new Inconsistencia("Motivo", "O motivo " + descricoesMotivoEmail[0] + " é inválido");

                    sumario.AdicionarInconsistencia(inconsistencia);
                }

                //Verifica se a descrição é vazia: não pode!
                if (descricoesMotivoEmail[0].Trim() == string.Empty)
                {
                    Inconsistencia inconsistencia = new Inconsistencia("Motivo", "Motivo deve ser informado");

                    sumario.AdicionarInconsistencia(inconsistencia);
                }

                //Caso ele informou um email, valida
                if (descricoesMotivoEmail[1] == null)
                {
                    Inconsistencia inconsistencia = new Inconsistencia("Email", "E-mail deve ser informado");

                    sumario.AdicionarInconsistencia(inconsistencia);
                }

                if (descricoesMotivoEmail[1].Trim() != string.Empty)
                {
                    //Se o email informado não estiver de acordo com a regra, é inválido
                    if (!EmailUtils.EnderecoEmailValido(descricoesMotivoEmail[1]))
                    {
                        Inconsistencia inconsistencia = new Inconsistencia("Email", "O e-mail" + descricoesMotivoEmail[1] + " é inválido");

                        sumario.AdicionarInconsistencia(inconsistencia);
                    }
                }
            }
            */
            #endregion

            /*
            Um breve:
            Os dados do motivo deverão ser retornados no formato [motivo];[email](opcional)\r\n em que \r\n = ENTER inserido no campo
            
            Exemplo de dois motivos inseridos:
             Esqueci minha senha;esquecisenha@redecard.com.br\r\nDúvidas;duvidas@redecard.com.br
            
            A lógica abaixo realiza a quebra de cada motivo usando split e valida estes itens considerando o seguinte:
            - Obrigatoriedade do campo [motivo]
            - Restrição de caracteres no campo [motivo]
            - Validação do campo [email] (importante: ele valida se o e-mail está num formato aceitável; não valida se o endereço existe)
            
            Quanto há dados inválidos, é adicionado ao objeto SumarioValidacao a descrição do erro (objeto Inconsistencia)
            */

            //Obrigatoriedade do campo Motivo
            if (string.IsNullOrEmpty(motivos) || motivos.Trim().Equals(string.Empty))
            {
                sumario.AdicionarInconsistencia(new Inconsistencia("Motivo", "Informe no mínimo um motivo de contato"));
                return sumario;
            }

            //Conjunto de caracteres considerados inválidos para descrição dos motivos
            string caracteresInvalidos = @"@#$%*(){}[]/\|:;<>";

            string[] entrada = motivos.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            //ANalisa cada entrada de MotivoDescricao...
            foreach (string descricao_email in entrada)
            {
                string[] descricoesMotivoEmail = descricao_email.Split(';'); // quebra em duas strings: [motivo] e [email]
                string textoDescricao = descricoesMotivoEmail[0]; //Aqui, não corre perigo de retornar array com zero posições pois anteriormente é verificado se a string é nula ou vazia
                string textoEmail; //E-mail poderá ou não ser informado

                //Cada item de motivo com email NÃO PODERÁ APRESENTAR MAIS DE DUAS POSIÇÕES(pensando na quebra por ;). Se não, é inválido
                if (descricoesMotivoEmail.GetLength(0) > 2)
                {
                    sumario.AdicionarInconsistencia(new Inconsistencia("Motivo", "Insira cada item de motivo no formato [motivo];[email] e pressione ENTER para adicionar mais motivos"));
                    return sumario;
                }

                //Verifica se a descrição é vazia: não pode!
                if (textoDescricao.Trim() == string.Empty)
                    sumario.AdicionarInconsistencia(new Inconsistencia("Motivo", "Motivo deve ser informado"));

                //Verifica se no motivo há letras inválidas com base na lista de caracteres inválidos
                if (textoDescricao.IndexOfAny(caracteresInvalidos.ToCharArray()) != -1)
                    sumario.AdicionarInconsistencia(new Inconsistencia("Motivo", "O motivo " + descricoesMotivoEmail[0] + " é inválido"));

                //Para evitar estouro de índice, verifica se a quebra retornou exatamente dois itens de array
                if (descricoesMotivoEmail.GetLength(0).Equals(2))
                {
                    textoEmail = descricoesMotivoEmail[1].Trim(); //Aqui, o item na posição 1(zero-based index) vai ser considerado como e-mail quando não for vazio

                    //Caso um email foi informado, valida
                    if (!string.IsNullOrEmpty(textoEmail))
                    {
                        //Se o email informado não estiver de acordo com a regra, é inválido
                        if (!EmailUtils.EnderecoEmailValido(descricoesMotivoEmail[1]))
                            sumario.AdicionarInconsistencia(new Inconsistencia("Email", "O e-mail " + descricoesMotivoEmail[1] + " é inválido"));
                    }
                }
            }

            return sumario;
        }
    }
}