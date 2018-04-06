#region Used Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.SharePoint.Linq;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using Microsoft.SharePoint;
#endregion

namespace Redecard.Portal.Aberto.Model.Repository
{
    /// <summary>
    ///   Responsável por qualquer ação com a lista "Cartões".
    /// </summary>
    public class RepositoryCartoes : RepositoryItem, IRepository<DTOCartao, CartõesItem>
    {
        #region Constructors
        /// <summary>
        ///   Inicializa o repositório da lista de "Cartões' utilizando um novo DataContext.
        /// </summary>
        public RepositoryCartoes() {}

        /// <summary>
        ///   Inicializa o repositório da lista de "Cartões' utilizando um DataContext já criado.
        /// </summary>
        /// <param name = "dataContext">
        ///   DataContext gerado pelo SPMetal baseado nas listas do Portal criadas pelo Model.
        ///   Permite a utilização de LINQ to SharePoint.
        /// </param>
        public RepositoryCartoes(GeneratedModelDataContext dataContext) : base(dataContext) {}
        #endregion

        #region Public Methods
        /// <summary>
        ///   Persiste o cartão na lista, atualizando ou inserindo.
        /// </summary>
        /// <param name = "cartao">Cartão a ser persistido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Persist(DTOCartao cartao)
        {
            return !cartao.ID.HasValue ? Add(DTOToModel(cartao)) : Update(DTOToModel(cartao));
        }

        /// <summary>
        ///   Deleta um cartão da lista.
        /// </summary>
        /// <param name = "cartao">Cartão a ser deletado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        public bool Delete(DTOCartao cartao)
        {
            try
            {
                var cartaoItem = DataContext.Cartões.Where(c => c.Id.Equals(cartao.ID)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser deletado.
                if (cartaoItem == null) return false;

                DataContext.Cartões.DeleteOnSubmit(cartaoItem);
                DataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);

                return !(DataContext.ChangeConflicts != null && DataContext.ChangeConflicts.Count == 0);
            }
            catch (Exception exception)
            {
                Log.WriteEntry(exception, EventLogEntryType.Error);
                //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
                //chamou este método.
                throw;
            }
        }

        /// <summary>
        ///   Retorna todos os itens da lista "Cartões".
        /// </summary>
        /// <returns>Todos os itens da lista "Cartões".</returns>
        public List<DTOCartao> GetAllItems()
        {
            try
            {
                var cartoes = new List<DTOCartao>();
                DataContext.Cartões.ToList().ForEach(cartao => cartoes.Add(ModelToDTO(cartao)));
                return cartoes;
            }
            catch (Exception exception)
            {
                Log.WriteEntry(exception, EventLogEntryType.Error);
                //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
                //chamou este método.
                throw;
            }
        }

        /// <summary>
        ///   Retorna determinados itens da lista "Cartões" baseado em um filtro.
        /// </summary>
        /// <param name = "filter">Filtro a ser utilizado na busca.</param>
        /// <returns>Lista com os itens encontrados.</returns>
        public List<DTOCartao> GetItems(Expression<Func<CartõesItem, bool>> filter)
        {
            try
            {
                var cartoes = new List<DTOCartao>();
                DataContext.Cartões.Where(filter).ToList().ForEach(cartao => cartoes.Add(ModelToDTO(cartao)));
                return cartoes;
            }
            catch (Exception exception)
            {
                Log.WriteEntry(exception, EventLogEntryType.Error);
                //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
                //chamou este método.
                throw;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        ///   Adiciona um novo cartão na lista.
        /// </summary>
        /// <param name = "cartao">Cartão a ser inserido.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Add(CartõesItem cartao)
        {
            try
            {
                DataContext.Cartões.InsertOnSubmit(cartao);
                DataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);

                return !(DataContext.ChangeConflicts != null && DataContext.ChangeConflicts.Count == 0);
            }
            catch (Exception exception)
            {
                Log.WriteEntry(exception, EventLogEntryType.Error);
                //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
                //chamou este método.
                throw;
            }
        }

        /// <summary>
        ///   Atualiza um cartão da lista.
        /// </summary>
        /// <param name = "cartao">Cartão a ser atualizado.</param>
        /// <returns>Boolean indicando se houve algum problema durante a operação.</returns>
        private bool Update(CartõesItem cartao)
        {
            try
            {
                var cartaoItem = DataContext.Cartões.Where(c => c.Id.Equals(cartao.Id)).FirstOrDefault();

                //Caso não tenha encontrado o item a ser atualizado
                if (cartaoItem == null) return false;

                //Atualiza os campos
                cartaoItem.Id = cartao.Id;
                cartaoItem.Title = cartao.Title;
                cartaoItem.Imagem = cartao.Imagem;
                cartaoItem.Descrição = cartao.Descrição;
                cartaoItem.TipoDoCartão = cartao.TipoDoCartão;
                cartaoItem.TipoDoCartãoDeBenefício = cartao.TipoDoCartãoDeBenefício;

                DataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);

                return !(DataContext.ChangeConflicts != null && DataContext.ChangeConflicts.Count == 0);
            }
            catch (Exception exception)
            {
                Log.WriteEntry(exception, EventLogEntryType.Error);
                //Dispara o erro para que a decisão da melhor maneira de exibir ao usuário seja de quem
                //chamou este método.
                throw;
            }
        }

        /// <summary>
        ///   Transforma um DTOCartao na entidade do modelo correspondente.
        /// </summary>
        /// <param name = "cartao">Cartão a ser transformado.</param>
        /// <returns>A entidade transformada para CartõesItem.</returns>
        private static CartõesItem DTOToModel(DTOCartao cartao)
        {
            var cartaoItem = new CartõesItem
                             {
                                 Id = cartao.ID,
                                 Title = cartao.Nome,
                                 Imagem = cartao.Imagem,
                                 Descrição = cartao.Descricao,
                                 TipoDoCartão = cartao.TipoDoCartao,
                                 TipoDoCartãoDeBenefício = cartao.TipoDoCartaoDeBeneficio,
                                 OrdemExibição = cartao.Ordem,
                                 Beneficios = cartao.Beneficios
                             };

            return cartaoItem;
        }

        /// <summary>
        ///   Transforma uma entidade do modelo CartõesItem na entidade DTO correspondente.
        /// </summary>
        /// <param name = "cartaoItem">Cartão a ser transformado.</param>
        /// <returns>A entidade do modelo transformada para DTOCartao.</returns>
        private static DTOCartao ModelToDTO(CartõesItem cartaoItem)
        {
            double dOrdem = 0;
            string sUrl = string.Empty;
            string sBeneficios = string.Empty;


            // verificar campo de URL, no formato padrão, o sharepoint retorna o valor no seguinte formato:
            // [URL], [Descrição]
            if (!String.IsNullOrEmpty(cartaoItem.Imagem)) {
                SPFieldUrlValue urlValue = new SPFieldUrlValue(cartaoItem.Imagem);
                sUrl = urlValue.Url;
            }
            else
                sUrl = cartaoItem.Imagem;

            if (cartaoItem.OrdemExibição.HasValue)
                dOrdem = cartaoItem.OrdemExibição.Value;

            if (!String.IsNullOrEmpty(cartaoItem.Beneficios)) {
                SPFieldMultiChoiceValue value = new SPFieldMultiChoiceValue(cartaoItem.Beneficios);
                for (int i = 0; i < value.Count; i++) {
                    if (i == 0)
                        sBeneficios += value[i];
                    else
                        sBeneficios += ";" + value[i];
                }
            }

            var cartao = new DTOCartao
                         {
                             ID = cartaoItem.Id,
                             Nome = cartaoItem.Title,
                             Imagem = sUrl,
                             Descricao = cartaoItem.Descrição,
                             TipoDoCartao = cartaoItem.TipoDoCartão,
                             TipoDoCartaoDeBeneficio = cartaoItem.TipoDoCartãoDeBenefício,
                             Ordem = dOrdem,
                             Beneficios = sBeneficios
                         };

            return cartao;
        }
        #endregion
    }
}