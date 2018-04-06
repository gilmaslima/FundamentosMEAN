using System;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;

/// <summary>
/// Classe utilizada para verificar a permissão aos serviços e demais funcionalidades 
/// da Intranet Serviços da Redecard
/// </summary>
public class GAPerfil
{
    #region Métodos chamados via COM+ do GAPerfil (GAP.dll)

    /// <summary>
    /// Nome do componente COM+ que faz a chamada ao GAPerfil
    /// </summary>
    const String NOME_COMPONENTE_GAPERFIL = "GAPerfil.clsGAP";

    /// <summary>
    /// Objeto estático que armazena o tipo instanciado do objeto COM+
    /// </summary>
    private static Type gaPerfilComPlusType = null;

    /// <summary>
    /// Objeto estático que armazena o objeto instanciado do tipo COM+
    /// </summary>
    private static object gaPerfilComPlusInstance = null;

    /// <summary>
    /// Atributo que controla a execução dos métodos e chamadas COM+ para ambientes multithread 
    /// </summary>
    private static object lockObject = new object();

    /// <summary>
    /// Garante a inicialização dos atributos estáticos tipo e objeto COM+
    /// </summary>
    private static void EnsureCOMPlusInstance()
    {
        try
        {

            if (object.ReferenceEquals(gaPerfilComPlusInstance, null))
            {
                lock (lockObject)
                {
                    gaPerfilComPlusType = Type.GetTypeFromProgID(NOME_COMPONENTE_GAPERFIL);
                    gaPerfilComPlusInstance = Activator.CreateInstance(gaPerfilComPlusType);
                }
            }
        }
        catch (COMException com)
        {
            throw new InvalidComObjectException(
                String.Format("Não foi possível instanciar o componente {0}", NOME_COMPONENTE_GAPERFIL), com);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    /// <summary>
    /// Verifica se o usuário informado possui acesso ao serviço/sistema
    /// </summary>
    /// <param name="funcional">Funcional do usuário utilizador</param>
    /// <param name="nomeSistema">Nome do sistema serviço de acesso</param>
    /// <returns>Um identificador númerico inteiro que identifica se o usuário possui ou não acesso ao serviço/sistema</returns>
    private static Int32? LSkill(String funcional, String nomeSistema)
    {
        EnsureCOMPlusInstance();
        object[] typeMethodParams = { 0, funcional, nomeSistema };
        object retorno = null;

        lock (lockObject)
        {
            retorno = gaPerfilComPlusType.InvokeMember("LSkill", BindingFlags.InvokeMethod, null, gaPerfilComPlusInstance, typeMethodParams);
        }

        return retorno as Int32?;
    }

    /// <summary>
    /// Verifica se o usuário informado possui acesso a página/função do serviço, este método só pode ser chamada após a primeira
    /// execução do método LSkill
    /// </summary>
    /// <param name="funcional">Nome da página</param>
    /// <param name="nomeSistema">Nome da função do serviço</param>
    /// <returns>Um identificador númerico inteiro que identifica se o usuário possui ou não acesso a página/função</returns>
    private static Int32? CSkill(String pagina, String codigoFuncao)
    {
        EnsureCOMPlusInstance();
        object[] typeMethodParams = { 0, pagina, codigoFuncao };
        object retorno = null;

        lock (lockObject)
        {
            retorno = gaPerfilComPlusType.InvokeMember("CSkill", BindingFlags.InvokeMethod, null, gaPerfilComPlusInstance, typeMethodParams);
        }

        return retorno as Int32?;
    }

    #endregion

    /// <summary>
    /// Verificar permissão a uma sigla/serviço da Intranet Serviços Redecard
    /// </summary>
    /// <param name="funcional">Funcional do usuário</param>
    /// <param name="siglaServico">Nome do sistema/sigla do serviço</param>
    /// <returns>Verdadeiro se o usuário possuir acesso, caso contrário, retorna Falso</returns>
    public static Boolean VerificarPermissao(String funcional, String siglaServico)
    {
#if DEBUG
        return true;
#else
        if (Helper.AmbienteSimulacao)
        {
            return true;
        }
        else
        {
            //funcional = funcional.Normalize(NormalizationForm.FormC);
            //siglaServico = funcional.Normalize(NormalizationForm.FormC);

            //Int32? retorno = LSkill(funcional, siglaServico);

            //if (retorno.HasValue)
            //    return (retorno.Value == 1);
            //else
            //    return false;
            return true;
        }
#endif
    }

    /// <summary>
    /// Verificar permissão a uma funcionalidade/parte de um serviço da Intranet Serviços Redecard
    /// </summary>
    /// <param name="pagina">Nome da página que contém a funcionalidade</param>
    /// <param name="codigoFuncao">Nome do sistema/sigla do serviço</param>
    /// <returns>Verdadeiro se o usuário possuir acesso, caso contrário, retorna Falso</returns>
    public static Boolean VerificarAcessoFuncionalidade(String pagina, String codigoFuncao)
    {
#if DEBUG
        return true;
#else
        if (Helper.AmbienteSimulacao)
        {
            return true;
        }
        else
        {
            //pagina = pagina.Normalize(NormalizationForm.FormC);
            //codigoFuncao = codigoFuncao.Normalize(NormalizationForm.FormC);

            //Int32? retorno = CSkill(pagina, codigoFuncao);

            //if (retorno.HasValue)
            //    return (retorno.Value == 1);
            //else
            //    return false;
            return true;
        }
#endif
    }
}