using Rede.PN.AtendimentoDigital.Core;

namespace Rede.PN.AtendimentoDigital.Servicos
{
	public class Global
	{
		public static void AppInitialize()
		{
			// Registrando repositórios por convenção
			GestorInjecao.RegistrarObjetosConvencao();
		}
	}
}
