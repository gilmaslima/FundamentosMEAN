using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.Portal.Helper.DTO;

namespace Redecard.Portal.Helper.Conversores
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Descrição: Classe conversora de string serializada com promoções para uma lista de objetos Promocao
    /// </summary>
    public class TradutorDeStringParaPromocao : ITraducao<string, System.Collections.Generic.IList<Promocao>>
    {
        /// <summary>
        /// Converte uma string em uma lista de objetos Promocao
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IList<Promocao> Traduzir(string item)
        {
            if (string.IsNullOrEmpty(item))
                return new List<Promocao>();

            IList<Promocao> lstPromocoes = new List<Promocao>();

            //Quebra por linha cada item de promoção
            string[] promocoes  = item.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string promocao in promocoes)
            {
                //Quebra por ; a descrição e link
                string[] aPromocoes = promocao.Split(';');

                //Adiciona à lista de Promoções
                lstPromocoes.Add(new Promocao() { Perfil = aPromocoes[0].Trim(), Link = aPromocoes[1].Trim()});
            }

            //Retorna a lista
            return lstPromocoes;
        }

        public string Traduzir(IList<Promocao> item)
        {
            throw new NotImplementedException();
        }
    }
}
