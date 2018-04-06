using Redecard.PN.Request.SharePoint.XBChargebackServico;

namespace Redecard.PN.Request.SharePoint.Model
{
    /// <summary>
    /// Modelo estendido de XBChargebackServico.Comprovante para identificação do status e tipo da venda do comprovante
    /// </summary>
    public class ComprovanteModel : Comprovante
    {
        /// <summary>
        /// Status do comprovante (histórico/pendente)
        /// </summary>
        public StatusComprovante Status { get; set; }

        /// <summary>
        /// Tipo da venda (crédito/débito)
        /// </summary>
        public TipoVendaComprovante TipoVenda { get; set; }

        /// <summary>
        /// Converte o objeto base
        /// </summary>
        /// <param name="baseClass"></param>
        /// <returns></returns>
        public static ComprovanteModel Convert(Comprovante baseClass)
        {
            ComprovanteModel target = new ComprovanteModel();

            var baseProperties = baseClass.GetType().GetProperties();
            var targetType = target.GetType();

            foreach (var prop in baseProperties)
            {
                var propValue = prop.GetValue(baseClass);

                var targetProp = targetType.GetProperty(prop.Name);
                if (targetProp != null)
                    targetProp.SetValue(target, propValue);
            }

            return target;
        }
    }
}
