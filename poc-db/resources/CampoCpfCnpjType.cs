
namespace Rede.PN.MultivanAlelo.Core.Web.Controles.Portal
{
    /// <summary>
    /// Enumerator para definir o tipo de campo a ser renderizado
    /// </summary>
    public enum CampoCpfCnpjType
    {
        /// <summary>
        /// Campo será usado como CPF e CNPJ, a ser tratado diretamente pela máscara em clientside
        /// </summary>
        Both = 0,

        /// <summary>
        /// Campo será tratado como CNPJ
        /// </summary>
        Cnpj = 1,

        /// <summary>
        /// Campo será tratado como CPF
        /// </summary>
        Cpf = 2
    }
}
