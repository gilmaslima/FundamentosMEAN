using Rede.PN.AtendimentoDigital.Core.Padroes.Singleton;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rede.PN.AtendimentoDigital.Core.Log
{
	/// <summary>
	/// Classe que gerencia o serviço de log.
	/// </summary>
	public sealed class GestorLog : Singleton<GestorLog>
	{
		#region Metodos Estaticos

		/// <summary>
		/// Grava um registro de log.
		/// </summary>
		/// <param name="idEvento">Valor de identificação do evento de log</param>
		/// <param name="categoria">Especifica a categoria de log a ser utilizada</param>
		/// <param name="prioridade">Grau de importância da mensagem de log</param>
		/// <param name="titulo">Título do log</param>
		/// <param name="mensagem">Mensagem a ser gravada no log</param>
		/// <param name="severidade">Gravidade do log</param>
		/// <param name="propriedades"></param>
		public static void IncluirLog(Int32 idEvento, String categoria, Int32 prioridade, String titulo, String mensagem, TraceEventType severidade, IDictionary<String, Object> propriedades)
		{
			GestorLog.Instancia.IncluirLogInterno(
				idEvento,
				categoria,
				prioridade,
				titulo,
				mensagem,
				severidade,
				propriedades
			);
		}

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
		/// <param name="propriedades"></param>
		public static void IncluirAuditoria(String classe, String metodo, String parametros, Object retorno, String categoria, String titulo, String mensagem, TraceEventType severidade, IDictionary<String, Object> propriedades)
		{
			GestorLog.Instancia.IncluirAuditoriaInterno(
				classe,
				metodo,
				parametros,
				retorno,
				categoria,
				titulo,
				mensagem,
				severidade,
				propriedades
			);
		}

		#endregion

		#region Gestor Log

		private IServicoLog log;

		private GestorLog()
		{
			this.log = LocalizadorServico.ObterServico<IServicoLog>();
		}

		/// <summary>
		/// Grava um registro de log.
		/// </summary>
		/// <param name="idEvento">Valor de identificação do evento de log</param>
		/// <param name="categoria">Especifica a categoria de log a ser utilizada</param>
		/// <param name="prioridade">Grau de importância da mensagem de log</param>
		/// <param name="titulo">Título do log</param>
		/// <param name="mensagem">Mensagem a ser gravada no log</param>
		/// <param name="severidade">Gravidade do log</param>
		/// <param name="propriedades"></param>
		private void IncluirLogInterno(Int32 idEvento, String categoria, Int32 prioridade, String titulo, String mensagem, TraceEventType severidade, IDictionary<String, Object> propriedades)
		{
			this.log.IncluirLog(idEvento, categoria, prioridade, titulo, mensagem, severidade, propriedades);
		}

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
		/// <param name="propriedades"></param>
		private void IncluirAuditoriaInterno(String classe, String metodo, String parametros, Object retorno, String categoria, String titulo, String mensagem, TraceEventType severidade, IDictionary<String, Object> propriedades)
		{
			this.log.IncluirAuditoria(classe, metodo, parametros, retorno, categoria, titulo, mensagem, severidade, propriedades);
		}

		#endregion
	}
}
