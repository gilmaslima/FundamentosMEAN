#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   :
- [11/05/2012] – [André Rentes] – [Criação]
 * 
(c) Copyright [2012] Redecard S.A.
Empresa     : [Iteris]
Histórico   :
- [06/06/2012] – [Agnaldo Costa] – [Inclusão do atributo senha para comparação durante alteração da senha]
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Modelo
{
    /// <summary>
    /// Classe de perguntas utilizada na Confirmação Positiva
    /// </summary>
    [DataContract]
    public class Pergunta
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Boolean PerguntaVariavel { get; set; }

        /// <summary>
        /// Código da Pergunta
        /// </summary>
        [DataMember]
        public Int32 CodigoPergunta { get; set; }

        /// <summary>
        /// Resposta da Pergunta
        /// </summary>
        [DataMember]
        public String Resposta { get; set; }
    }
}