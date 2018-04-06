/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace Redecard.PN.FMS.Modelo
{
    /// <summary>
    /// Este componente publica a classe RespostaAnalise, que expõe propriedades para manipular dados de resposta da análise.
    /// </summary>    
    public class RespostaAnalise
    {
        public Int32 NumeroEmissor { get; set; }
        public string Comentario { get; set; }
        public int GrupoEntidade { get; set; }
        public bool EhFraude { get; set; }
        public TipoResposta TipoResposta { get; set; }
        public long IdentificadorTransacao { get; set; }
        public string UsuarioLogin { get; set; }
        public TipoAlarme TipoAlarme { get; set; }
    }
}
