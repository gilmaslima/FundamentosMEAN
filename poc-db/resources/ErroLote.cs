using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Cancelamento.Sharepoint.Modelos
{
    /// <summary>
    /// Classe para relacionar a lista de erros do arquivo de lote.
    /// </summary>
    [Serializable]
    public class ErroLote
    {
        #region Construtores
        public ErroLote(int linha, string mensagemErro)
        {
            this.Linha = linha;
            this.MensagemErro = mensagemErro;
        }
        #endregion

        #region Atributos
        private int _linha;
        private string _mensagemErro;
        #endregion

        #region Propriedade
        public int Linha { get { return _linha; } set { _linha = value; } }

        public string MensagemErro { get { return _mensagemErro; } set { _mensagemErro = value; } }
        #endregion
    }
}
