#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [Agnaldo Costa]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [16/07/2012] – [Agnaldo Costa] – [Criação]
*/
#endregion

using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Redecard.PN.OutrosServicos.Servicos
{
    [ServiceContract]
    public interface IRegimeFranquiaServico
    {
        /// <summary>
        /// Consultar Restrição de Serviço para a Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <returns>Dados da Restrição de Regime</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        RestricaoRegime ConsultarRestricaoRegime(Int32 codigoEntidade, Int32 codigoGrupoEntidade);

        /// <summary>
        /// Busca o conteúdo do contrato de acordo com o código da Versão de Regime atual
        /// </summary>
        /// <param name="codigoVersao">Código da versão do regime atual</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        RestricaoRegime ConsultarContratoRestricao(Int32 codigoVersao);

        /// <summary>
        /// Consulta Código de Regime atual da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoRetorno">Código de erro retornado pela proc</param>
        /// <returns>Código do Regime</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ConsultarCodigoRegime(Int32 codigoEntidade, out Int32 codigoRetorno);

        /// <summary>
        /// Atualiza o Regime de franquia da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoRegime">Código do regime</param>
        /// <param name="codigoCelula">Código da célula</param>
        /// <param name="codigoCanal">Código do canal</param>
        /// <param name="codigoUsuario">Usuário responsável pela atualização</param>
        /// <returns>Código de erro retornado da procedure spge0362</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 AtualizarFranquia(Int32 codigoEntidade, Int32 codigoRegime, Int32 codigoCelula, Int32 codigoCanal,
            String codigoUsuario);

        /// <summary>
        /// Inclui/Atualiza a confirmação do contrato Franquia
        /// </summary>
        /// <param name="codigoVersao">Código de versão</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoUsuario">Código do Usuário responsável pela inclusão/atualização</param>
        /// <returns>0</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int16 IncluirAceite(Int32 codigoVersao, Int32 codigoGrupoEntidade, Int32 codigoEntidade, String codigoUsuario);

        /// <summary>
        /// Inclui/Atualiza a confirmação do usuário
        /// </summary>
        /// <param name="codigoRestricao">Código de restrição</param>
        /// <param name="codigoVersao">Código de versão</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoUsuario">Código do Usuário responsável pela inclusão/atualização</param>
        /// <returns>0</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int16 IncluirAceiteUsuario(Int32 codigoRestricao, Int32 codigoVersao, Int32 codigoGrupoEntidade, Int32 codigoEntidade, String codigoUsuario);
    }
}
