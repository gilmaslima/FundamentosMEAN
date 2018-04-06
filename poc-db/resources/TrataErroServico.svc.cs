using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using Redecard.PN.Negocio;
using AutoMapper;

namespace Redecard.PN.Servicos
{
    /// <summary>
    /// Serviço de tratamento de erros do sistema Redecard.PN
    /// </summary>
    public class TrataErroServico : ITrataErroServico
    {
        /// <summary>
        /// Consulta mensagem de erro
        /// </summary>
        /// <param name="fonte">Nome do serviço e do método que gerou o erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <returns>Mensagem de erro</returns>
        public Servicos.TrataErro Consultar(String fonte, Int32 codigo)
        {
            //try
            //{
                Negocio.TrataErro trataErroNegocio = new Negocio.TrataErro();

                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Modelo.TrataErro, Servicos.TrataErro>();

                // Retorna Servicos.TrataErro
                return Mapper.Map<Modelo.TrataErro, Servicos.TrataErro>(
                    trataErroNegocio.Consultar(fonte, codigo));

            //}
            //catch(Exception ex)
            //{
            //    throw new FaultException(ex.Message);
            //}
        }

        /// <summary>
        /// Consultar mensagem de erro
        /// </summary>
        /// <param name="codigo">Código do erro</param>
        /// <returns>Mensagem de erro</returns>
        public Servicos.TrataErro Consultar(Int32 codigo)
        {
            //try
            //{
            Negocio.TrataErro trataErroNegocio = new Negocio.TrataErro();

            // Cria mapeamento, caso já exista utilizará o existente
            Mapper.CreateMap<Modelo.TrataErro, Servicos.TrataErro>();
            
            // Retorna Servicos.TrataErro
            return Mapper.Map<Modelo.TrataErro, Servicos.TrataErro>(
                trataErroNegocio.Consultar(codigo));

            //}
            //catch(Exception ex)
            //{
            //    throw new FaultException(ex.Message);
            //}
        }

        /// <summary>
        /// Atualizar mensagem de erro
        /// </summary>
        /// <param name="mensagem">Objeto modelo com a nova mensagem e seu código de erro</param>
        /// <returns>0</returns>
        public Int16 AtualizarMensagem(Servicos.TrataErro mensagem)
        {
            try
            {
                Negocio.TrataErro trataErroNegocio = new Negocio.TrataErro();

                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Servicos.TrataErro, Modelo.TrataErro>();

                return trataErroNegocio.AtualizarMensagem(Mapper.Map<Servicos.TrataErro, Modelo.TrataErro>(mensagem));
            }
            catch(Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

    }
}
