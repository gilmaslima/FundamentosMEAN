using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.Portal.Helper.DTO;

namespace Redecard.Portal.Helper.Conversores
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Descrição: Classe conversora de string serializada com motivos de contato para uma lista de objetos MotivoContato
    /// </summary>
    public class TradutorDeStringParaMotivoContato : ITraducao<string, System.Collections.Generic.IList<MotivoContato>>
    {
        /// <summary>
        /// Converte uma string em uma lista de objetos MotivoContato
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IList<MotivoContato> Traduzir(string item)
        {
            if (string.IsNullOrEmpty(item))
                return new List<MotivoContato>();

            IList<MotivoContato> lstMotivosContato = new List<MotivoContato>();

            //Cada motivo com seu respectivo email(que é opcional) é demarcado com \r\n. (TECLA ENTER NO TEXTAREA)
            //Realiza a quebra par obter cada item
            string[] motivosContato = item.Split(new char[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries);

            foreach (string motivoContato in motivosContato)
            {
                //Cada motivo de contato vem no formato [descrição];[email]
                //Realiza a quebra de cada parte da informação
                string[] motivos_Contatos = motivoContato.Split(';');

                //Instancia e adiciona à lista de motivo de contato um novo objeto em que:
                //motivos_Contatos[0] = "descrição"
                //motivos_Contatos[1] = "email"
                lstMotivosContato.Add(new MotivoContato(motivos_Contatos[0].Trim(),motivos_Contatos[1].Trim()));
            }

            //Retorna a lista montada
            return lstMotivosContato;
        }

        public string Traduzir(IList<MotivoContato> item)
        {
            throw new NotImplementedException();
        }
    }
}