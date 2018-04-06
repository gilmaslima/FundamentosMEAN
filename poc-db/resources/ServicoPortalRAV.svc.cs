/*
(c) Copyright [2012] Redecard S.A.
Autor : [Daniel Coelho]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/07/30 - Daniel Coelho - Versão Inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.RAV.Servicos;
using Redecard.PN.RAV.Negocios;
using Redecard.PN.RAV.Modelos;
using AutoMapper;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.WCF;
using Redecard.PN.Comum;

namespace Redecard.PN.RAV.Servicos
{
    [ExceptionShielding("RedecardPortalException")]
    public class ServicoPortalRAV : IServicoPortalRAV
    {
        #region RAV Avulso
        /// <summary>
        /// Verifica o RAV Avulso disponível de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <returns>Classe de dados de um RAV Avulso.</returns>
        public Servicos.ModRAVAvulsoSaida VerificarRAVDisponivel(int numeroPDV)
        {
            try
            {
                Servicos.ModRAVAvulsoSaida svcSaidaRAV = new Servicos.ModRAVAvulsoSaida();
                Modelos.ModRAVAvulsoSaida modSaidaRAV = new NegPortalRAV().VerificarRAVDisponivel(numeroPDV);

                Mapper.CreateMap<Modelos.ModRAVAntecipa, Servicos.ModRAVAntecipa>();
                svcSaidaRAV.DadosAntecipacao = Mapper.Map<Modelos.ModRAVAntecipa, Servicos.ModRAVAntecipa>(modSaidaRAV.DadosAntecipado);

                Mapper.CreateMap<Modelos.ModRAVAvulsoSaida, Servicos.ModRAVAvulsoSaida>();
                svcSaidaRAV = Mapper.Map<Modelos.ModRAVAvulsoSaida, Servicos.ModRAVAvulsoSaida>(modSaidaRAV);

                return svcSaidaRAV;
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<ServicoRAVException>(new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte, Mensagem = ex.GetBaseException().Message });
            }
        }

        /// <summary>
        /// Consulta o RAV Avulso disponível de um PDV específico.
        /// </summary>
        /// <param name="entradaRAV">Os dados de entrada da pesquisa. Consultar documentação para informações.</param>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <param name="tipoCredito">O tipo de crédito.</param>
        /// <param name="valorAntecipado">O valor a ser antecipado.</param>
        /// <returns>Classe de dados de um RAV Avulso.</returns>
        public Servicos.ModRAVAvulsoSaida ConsultarRAVAvulso(Servicos.ModRAVAvulsoEntrada entradaRAV, int numeroPDV, int tipoCredito, decimal valorAntecipado)
        {
            try
            {
                Modelos.ModRAVAvulsoEntrada modEntradaRAV = new Modelos.ModRAVAvulsoEntrada();

                Mapper.CreateMap<Servicos.ModRAVAvulsoEntrada, Modelos.ModRAVAvulsoEntrada>();
                modEntradaRAV = Mapper.Map<Servicos.ModRAVAvulsoEntrada, Modelos.ModRAVAvulsoEntrada>(entradaRAV);

                Modelos.ModRAVAvulsoSaida modSaidaRAV = new NegPortalRAV().ConsultarRAVAvulso(modEntradaRAV, numeroPDV, tipoCredito, valorAntecipado);

                Mapper.CreateMap<Modelos.ModRAVAvulsoSaida, Servicos.ModRAVAvulsoSaida>();

                return Mapper.Map<Modelos.ModRAVAvulsoSaida, Servicos.ModRAVAvulsoSaida>(modSaidaRAV);
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<ServicoRAVException>(new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte, Mensagem = ex.GetBaseException().Message });
            }
        }

        /// <summary>
        /// Efetua o RAV Avulso de um PDV específico.
        /// </summary>
        /// <param name="entradaRAV">Os dados de entrada da efetuação. Consultar documentação para informações.</param>
        /// <param name="numeroPDV">O número do PDV a ser efetuado.</param>
        /// <param name="tipoCredito">O tipo de crédito.</param>
        /// <param name="valorAntecipado">O valor a ser antecipado.</param>
        /// <returns>O código de retorno da transação. Consultar documentação para lista de códigos de retorno.</returns>
        public int EfetuarRAVAvulso(Servicos.ModRAVAvulsoEntrada entradaRAV, int numeroPDV, int tipoCredito, decimal valorAntecipado)
        {
            try
            {
                Modelos.ModRAVAvulsoEntrada modEntradaRAV = new Modelos.ModRAVAvulsoEntrada();

                Mapper.CreateMap<Servicos.ModRAVAvulsoEntrada, Modelos.ModRAVAvulsoEntrada>();
                modEntradaRAV = Mapper.Map<Servicos.ModRAVAvulsoEntrada, Modelos.ModRAVAvulsoEntrada>(entradaRAV);

                return new NegPortalRAV().EfetuarRAVAvulso(modEntradaRAV, numeroPDV, tipoCredito, valorAntecipado);
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<ServicoRAVException>(new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte, Mensagem = ex.GetBaseException().Message });
            }
        }
        #endregion

        #region RAV Automático
        /// <summary>
        /// Consulta o RAV Automático de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <returns>Classe de dados de um RAV Automático.</returns>
        public Servicos.ModRAVAutomatico ConsultarRAVAutomatico(int numeroPDV)
        {
            try
            {
                Servicos.ModRAVAutomatico svcSaidaRAV = new Servicos.ModRAVAutomatico();
                Modelos.ModRAVAutomatico modSaidaRAV = new NegPortalRAV().ConsultarRAVAutomatico(numeroPDV);

                Mapper.CreateMap<Modelos.ModRAVAutomatico, Servicos.ModRAVAutomatico>();
                svcSaidaRAV = Mapper.Map<Modelos.ModRAVAutomatico, Servicos.ModRAVAutomatico>(modSaidaRAV);

                return svcSaidaRAV;
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<ServicoRAVException>(new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte, Mensagem = ex.GetBaseException().Message });
            }
        }

        /// <summary>
        /// Efetua o RAV Automático de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser efetuado.</param>
        /// <returns>True se foi realizado com sucesso, False se não foi.</returns>
        public bool EfetuarRAVAutomatico(int numeroPDV)
        {
            try
            {
                return new NegPortalRAV().EfetuarRAVAutomatico(numeroPDV);
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<ServicoRAVException>(new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte, Mensagem = ex.GetBaseException().Message });
            }
        }

        /// <summary>
        /// Simula o RAV Automático de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser simulado.</param>
        /// <returns>Classe de dados de um RAV Automático.</returns>
        public Servicos.ModRAVAutomatico SimularRAVAutomatico(int numeroPDV)
        {
            try
            {
                Servicos.ModRAVAutomatico svcSaidaRAV = new Servicos.ModRAVAutomatico();
                Modelos.ModRAVAutomatico modSaidaRAV = new NegPortalRAV().SimularRAVAutomatico(numeroPDV);

                Mapper.CreateMap<Modelos.ModRAVAutomatico, Servicos.ModRAVAutomatico>();
                svcSaidaRAV = Mapper.Map<Modelos.ModRAVAutomatico, Servicos.ModRAVAutomatico>(modSaidaRAV);

                return svcSaidaRAV;
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<ServicoRAVException>(new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte, Mensagem = ex.GetBaseException().Message });
            }
        }
        #endregion

        #region RAV Email
        /// <summary>
        /// Salva os emails para as notificações de um RAV.
        /// </summary>
        /// <param name="dadosEmail">Classe de dados preenchida. Consultar documentação para informações.</param>
        /// <returns>True se foi realizado com sucesso, False se não foi.</returns>
        public bool SalvarEmails(Servicos.ModRAVEmailEntradaSaida dadosemail)
        {
            try
            {
                Modelos.ModRAVEmailEntradaSaida modDadosEmail = new Modelos.ModRAVEmailEntradaSaida();

                Mapper.CreateMap<Servicos.ModRAVEmailEntradaSaida, Modelos.ModRAVEmailEntradaSaida>();
                modDadosEmail = Mapper.Map<Servicos.ModRAVEmailEntradaSaida, Modelos.ModRAVEmailEntradaSaida>(dadosemail);

                return new NegPortalRAV().SalvarEmails(modDadosEmail);
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<ServicoRAVException>(new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte, Mensagem = ex.GetBaseException().Message });
            }
        }

        /// <summary>
        /// Consulta os emails registrados no RAV de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <returns>Classe de dados com a lista de emails no RAV.</returns>
        public Servicos.ModRAVEmailEntradaSaida ConsultarEmails(int numeroPDV)
        {
            try
            {
                Modelos.ModRAVEmailEntradaSaida modEntradaSaida = new NegPortalRAV().ConsultarEmails(numeroPDV);

                Servicos.ModRAVEmailEntradaSaida sevEmailEntradaSaida = new ModRAVEmailEntradaSaida();

                Mapper.CreateMap<Modelos.ModRAVEmailEntradaSaida, Servicos.ModRAVEmailEntradaSaida>();
                sevEmailEntradaSaida = Mapper.Map<Modelos.ModRAVEmailEntradaSaida, Servicos.ModRAVEmailEntradaSaida>(modEntradaSaida);

                return sevEmailEntradaSaida;
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<ServicoRAVException>(new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte, Mensagem = ex.GetBaseException().Message });
            }
        }
        #endregion

        #region RAV Senha
        /// <summary>
        /// Valida a senha de RAV de um PDV específico.
        /// </summary>
        /// <param name="senha">A senha.</param>
        /// <param name="numeroPDV">O número do PDV.</param>
        /// <returns>True se foi realizado com sucesso, False se não foi.</returns>
        public bool ValidarSenha(string senha, int numeroPDV)
        {
            try
            {
                return new NegPortalRAV().ValidarSenha(senha, numeroPDV);
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<ServicoRAVException>(new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte, Mensagem = ex.GetBaseException().Message });
            }
        }

        /// <summary>
        /// Verifica o acesso ao RAV um número PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV.</param>
        /// <returns>True se foi realizado com sucesso, False se não foi.</returns>
        public bool VerificarAcesso(int numeroPDV)
        {
            try
            {
                return new NegPortalRAV().VerificarAcesso(numeroPDV);
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<ServicoRAVException>(new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte, Mensagem = ex.GetBaseException().Message });
            }
        }
        #endregion
    }
}
