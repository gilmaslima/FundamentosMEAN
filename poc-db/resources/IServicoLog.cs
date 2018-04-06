using Rede.PN.AtendimentoDigital.Core.Padroes.ServiceLocator;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rede.PN.AtendimentoDigital.Core.Log
{
	/// <summary>
	/// Interface generica de log.
	/// Esta interface é utilizada para ser implementada pelos serviços de log.
	/// </summary>
	public interface IServicoLog : IServico
	{
		/// <summary>
		/// Grava um registro de log.
		/// </summary>
		/// <param name="idEvento">Valor de identificação do evento de log</param>
		/// <param name="categoria">Especifica a categoria de log a ser utilizada</param>
		/// <param name="prioridade">Grau de importância da mensagem de log</param>
		/// <param name="titulo">Título do log</param>
		/// <param name="mensagem">Mensagem a ser gravada no log</param>
		/// <param name="severidade">Gravidade do log</param>
		/// <param name="propriedadesExtendidas"></param>
		void IncluirLog(Int32 idEvento, String categoria, Int32 prioridade, String titulo, String mensagem, TraceEventType severidade, IDictionary<String, Object> propriedadesExtendidas);

		/// <summary>
		/// Grava um registro de auditoria.
		/// </summary>
		/// <param name="classe">Nome da classe que será auditada</param>
		/// <param name="metodo">Nome do método que será auditado</param>
		/// <param name="parametros">Parâmetros que serão auditados</param>
		/// <param name="retorno">Retorno do método auditado</param>
		/// <param name="categoria">Especifica a categoria de registro de auditoria a ser utilizada</param>
		/// <param name="titulo">Título do registro de auditoria</param>
		/// <param name="mensagem">Mensagem a ser gravada no registro de auditoria</param>
		/// <param name="severidade">Gravidade do registro de auditoria</param>
		/// <param name="propriedadesExtendidas"></param>
		void IncluirAuditoria(String classe, String metodo, String parametros, Object retorno, String categoria, String titulo, String mensagem, TraceEventType severidade, IDictionary<String, Object> propriedadesExtendidas);
	}
}
