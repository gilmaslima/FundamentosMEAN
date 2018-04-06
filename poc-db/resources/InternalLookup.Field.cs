#region Used Namespaces
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.WebControls;
#endregion

namespace Redecard.Portal.Aberto.CustomFields
{
    /// <summary>
    ///   Campo que permite selecionar um valor ou criar um novo.
    /// </summary>
    public class InternalLookupField : SPFieldText
    {
        /// <summary>
        ///   Inicializa uma nova instância de InternalLookupField baseado em uma coleção de campos específicos e um nome.
        /// </summary>
        /// <param name = "fields">Uma coleção de campos que representa a coleção do pai.</param>
        /// <param name = "fieldName">Uma string que contém o nome do campo.</param>
        public InternalLookupField(SPFieldCollection fields, string fieldName) : base(fields, fieldName) {}

        /// <summary>
        ///   Inicializa uma nova instância de InternalLookupField baseado em uma coleção de campos específicos, nome do tipo e um nome de exibição.
        /// </summary>
        /// <param name = "fields">ma coleção de campos que representa a coleção do pai.</param>
        /// <param name = "typeName">Uma string que contém o nome do tipo.</param>
        /// <param name = "displayName">Uma string que contém o nome de exibição do campo.</param>
        public InternalLookupField(SPFieldCollection fields, string typeName, string displayName) : base(fields, typeName, displayName) {}

        /// <summary>
        ///   Retorna o controle que é usado na renderização do campo.
        /// </summary>
        /// <returns>
        ///   Um objeto que representa o controle de renderização.
        /// </returns>
        public override BaseFieldControl FieldRenderingControl
        {
            [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
            get
            {
                BaseFieldControl fieldControl = new InternalLookupFieldControl
                                                {
                                                    FieldName = InternalName
                                                };

                return fieldControl;
            }
        }
    }
}