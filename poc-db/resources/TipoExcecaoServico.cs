using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.FMS.Comum
{
    /// <summary>
    /// Enumeração utilizado para tipos de exceção.
    /// </summary>
    public enum TipoExcecaoServico
    {
        RequiredFieldNotFoundException = 0,
        InvalidParameterException = 100,
        InfrastructureException = 200,
        PaginationParameterException = 201,
        CardIssuingAgentNotFoundException = 202,
        LockException = 203,
        SecurityException = 204,
        MerchantAlreadyRegisteredException = 205,
        Outros = 300
    }
}
