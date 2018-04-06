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
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.FMS.Servico.Modelo.CriterioSelecao
{
    /// <summary>
    /// Este componente publica a classe PesquisarRangeBinPorEmissorRetorno, que expõe propriedades para manipular dados de retorno de pesquisa de range bin por emissor.
    /// </summary>
    [DataContract]
    public class PesquisarRangeBinPorEmissorRetorno
    {
        [DataMember]
        public List<FaixaBin> ListaFaixaBin { get; set; }

    }
}