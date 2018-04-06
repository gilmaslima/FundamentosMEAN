using System;

namespace Rede.PN.HealthCheck.Models
{
    /// <summary>
    /// Classe de camada de Negócio para consultas ao sistemas parceiros
    /// </summary>
    public class SistemaParceiroBL
    {
        /// <summary>
        /// Verificar os status do GE
        /// </summary>
        /// <returns></returns>
        public Boolean VerificarStatusGE()
        {
            return new DAL.SistemaDAL().StatusGE();
        }

        /// <summary>
        /// Verificar status do TG
        /// </summary>
        /// <returns></returns>
        public Boolean VerificarStatusTG()
        {
            return new DAL.SistemaDAL().StatusTG();
        }
    }
}