/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 27/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.FMS.Sharepoint
{
    /// <summary>
    /// Classe de definição de constantes.
    /// </summary>
    public class Constantes
    {
        public const string FMS_Lista_SituacaoTransacao = "FMSListaSituacaoTransacao";
        public const int PosicaoInicialPrimeiroRegistro = 0;
        public const int QtdMaximaRegistros = 100;

        public const string ArquivoImagemRegistroBloqueado = "../../../_layouts/Redecard.PN.FMS.Sharepoint/imagens/bot_lock.gif";
        public const string ArquivoImagemRegistroDesbloqueado = "../../../_layouts/Redecard.PN.FMS.Sharepoint/imagens/bot_unlock.gif";
        public const string ArquivoImagemRegistroBloqueadoMesmoUsuario = "../../../_layouts/Redecard.PN.FMS.Sharepoint/imagens/bot_lock.gif";
    }
}
