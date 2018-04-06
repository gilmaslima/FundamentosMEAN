#region Used Namespaces
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Linq;
using System;
#endregion

namespace Redecard.Portal.Aberto.Model {
    /// <summary>
    ///   Adiciona a funcionalidade de LINQ to SharePoint para as colunas da lista "Cartões" que
    ///   utilizem custom fields.
    /// </summary>
    public partial class CartõesItem : ICustomMapping {
        #region Fields
        /// <summary>
        ///   Variável que armazena o tipo do cartão de benefício.
        /// </summary>
        private string _tipoDoCartãoDeBenefício;

        /// <summary>
        /// 
        /// </summary>
        private string _beneficios;
        #endregion

        #region Properties
        /// <summary>
        ///   Retorna ou armazena o tipo do cartão de benefício.
        /// </summary>
        public string TipoDoCartãoDeBenefício {
            get { return _tipoDoCartãoDeBenefício; }
            set {
                if ((value == _tipoDoCartãoDeBenefício)) return;

                OnPropertyChanging("TipoDoCartãoDeBenefício", _tipoDoCartãoDeBenefício);
                _tipoDoCartãoDeBenefício = value;
                OnPropertyChanged("TipoDoCartãoDeBenefício");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Beneficios {
            get {
                return _beneficios;
            }
            set {
                if ((value == _beneficios)) return;
                OnPropertyChanging("Benefícios", _beneficios);
                _beneficios = value;
                OnPropertyChanged("Benefícios");
            }
        }
        #endregion

        #region Implementation of ICustomMapping
        /// <summary>
        ///   Persiste um campo (coluna) para uma propriedade para que o LINQ to SharePoint possa carregar o conteúdo
        ///   do campo na base de dados para a propriedade que o representa.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        [CustomMapping(Columns = new[]
                                 {
                                     "Tipo_x0020_do_x0020_Cart_x00e3_o0",
                                     "_beneficios"
                                 })]
        public void MapFrom(object listItem) {
            var item = (SPListItem)listItem;
            var cartaoBeneficioValue = item[Constants.Cartoes.Fields.TipoDoCartaoDeBeneficio];
            var beneficiosValue = item[Constants.Cartoes.Fields.Beneficios];

            TipoDoCartãoDeBenefício = cartaoBeneficioValue != null ? cartaoBeneficioValue.ToString() : null;
            Beneficios = beneficiosValue != null ? beneficiosValue.ToString() : null;
        }

        /// <summary>
        ///   Persiste uma propriedade para um campo (coluna) para que o LINQ to SharePoint possa salvar o valor da propriedade
        ///   para o campo na base de dados.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        public void MapTo(object listItem) {
            var item = (SPListItem)listItem;
            item[Constants.Cartoes.Fields.TipoDoCartaoDeBeneficio] = TipoDoCartãoDeBenefício;
            item[Constants.Cartoes.Fields.Beneficios] = _beneficios;
        }

        /// <summary>
        ///   Resolve divergências nos valores de um ou mais campos no item da lista, respeitando o valor atual no cliente,
        ///   o valor atual na base de dados e o valor que foi originalmente retornado da base de dados.
        /// </summary>
        /// <param name = "mode">Um valor que especifica a regra para resolver as divergências.</param>
        /// <param name = "originalListItem">Um objeto que representa os valores do item da lista quando foi retornado da base de dados e pode ser convertido para SPListItem.</param>
        /// <param name = "databaseListItem">Um objeto que representa o valor atual do item da lista na base de dados e pode ser convertido para SPListItem.</param>
        public void Resolve(RefreshMode mode, object originalListItem, object databaseListItem) {
            this.ResolveTipoCartaoBeneficio(mode, originalListItem, databaseListItem);
            this.ResolveBeneficios(mode, originalListItem, databaseListItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="originalListItem"></param>
        /// <param name="databaseListItem"></param>
        private void ResolveBeneficios(RefreshMode mode, object originalListItem, object databaseListItem) {
            var originalItem = (SPListItem)originalListItem;
            var databaseItem = (SPListItem)databaseListItem;

            string originalValue = String.Empty;
            if (originalItem[Constants.Cartoes.Fields.Beneficios] != null) {
                originalValue = originalItem[Constants.Cartoes.Fields.Beneficios].ToString();
            }

            string dbValue = String.Empty;
            if (databaseItem[Constants.Cartoes.Fields.Beneficios] != null) {
                dbValue = databaseItem[Constants.Cartoes.Fields.Beneficios].ToString();
            }

            switch (mode) {
                case RefreshMode.OverwriteCurrentValues:
                    Beneficios = dbValue;
                    break;
                case RefreshMode.KeepCurrentValues:
                    databaseItem[Constants.Cartoes.Fields.Beneficios] = Beneficios;
                    break;
                case RefreshMode.KeepChanges:
                    if (TipoDoCartãoDeBenefício != originalValue) databaseItem[Constants.Cartoes.Fields.Beneficios] = Beneficios;
                    else if (Beneficios == originalValue && Beneficios != dbValue) Beneficios = dbValue;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="originalListItem"></param>
        /// <param name="databaseListItem"></param>
        private void ResolveTipoCartaoBeneficio(RefreshMode mode, object originalListItem, object databaseListItem) {
            var originalItem = (SPListItem)originalListItem;
            var databaseItem = (SPListItem)databaseListItem;

            string originalValue = String.Empty;
            if (originalItem[Constants.Cartoes.Fields.TipoDoCartaoDeBeneficio] != null) {
                originalValue = originalItem[Constants.Cartoes.Fields.TipoDoCartaoDeBeneficio].ToString();
            }

            string dbValue = String.Empty;
            if (databaseItem[Constants.Cartoes.Fields.TipoDoCartaoDeBeneficio] != null) {
                dbValue = databaseItem[Constants.Cartoes.Fields.TipoDoCartaoDeBeneficio].ToString();
            }

            switch (mode) {
                case RefreshMode.OverwriteCurrentValues:
                    TipoDoCartãoDeBenefício = dbValue;
                    break;
                case RefreshMode.KeepCurrentValues:
                    databaseItem[Constants.Cartoes.Fields.TipoDoCartaoDeBeneficio] = TipoDoCartãoDeBenefício;
                    break;
                case RefreshMode.KeepChanges:
                    if (TipoDoCartãoDeBenefício != originalValue) databaseItem[Constants.Cartoes.Fields.TipoDoCartaoDeBeneficio] = TipoDoCartãoDeBenefício;
                    else if (TipoDoCartãoDeBenefício == originalValue && TipoDoCartãoDeBenefício != dbValue) TipoDoCartãoDeBenefício = dbValue;
                    break;
            }
        }
        #endregion
    }

    /// <summary>
    ///   Adiciona a funcionalidade de LINQ to SharePoint para as colunas da lista "Depoimentos" que
    ///   utilizem custom fields.
    /// </summary>
    public partial class DepoimentosItem : ICustomMapping {
        #region Fields
        /// <summary>
        ///   Variável que armazena o produto ou serviço do depoimento.
        /// </summary>
        private string _produtoServico;

        /// <summary>
        ///   Variável que armazena o estado do depoimento.
        /// </summary>
        private string _estado;

        /// <summary>
        ///   Variável que armazena o ramo do depoimento.
        /// </summary>
        private string _ramo;
        #endregion

        #region Properties
        /// <summary>
        ///   Retorna ou armazena o ramo do depoimento.
        /// </summary>
        public string Ramo {
            get { return _ramo; }
            set {
                if ((value == _ramo)) return;

                OnPropertyChanging("Ramo", _ramo);
                _ramo = value;
                OnPropertyChanged("Ramo");
            }
        }

        /// <summary>
        ///   Retorna ou armazena o produto ou serviço do depoimento.
        /// </summary>
        public string ProdutoServico {
            get { return _produtoServico; }
            set {
                if ((value == _produtoServico)) return;

                OnPropertyChanging("ProdutoServico", _produtoServico);
                _produtoServico = value;
                OnPropertyChanged("ProdutoServico");
            }
        }

        /// <summary>
        ///   Retorna ou armazena o tipo do depoimento.
        /// </summary>
        public string Estado {
            get { return _estado; }
            set {
                if ((value == _estado)) return;

                OnPropertyChanging("Estado", _estado);
                _estado = value;
                OnPropertyChanged("Estado");
            }
        }
        #endregion

        #region Implementation of ICustomMapping
        /// <summary>
        ///   Persiste um campo (coluna) para uma propriedade para que o LINQ to SharePoint possa carregar o conteúdo
        ///   do campo na base de dados para a propriedade que o representa.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        [CustomMapping(Columns = new[]
                                 {
                                     "Ramo",
                                     "ProdutoServico",
                                     "Estado"
                                 })]
        public void MapFrom(object listItem) {
            var item = (SPListItem)listItem;

            Ramo = item[Constants.Depoimentos.Fields.Ramo] != null ? item[Constants.Depoimentos.Fields.Ramo].ToString() : null;
            ProdutoServico = item[Constants.Depoimentos.Fields.ProdutoServico] != null ? item[Constants.Depoimentos.Fields.ProdutoServico].ToString() : null;
            Estado = item[Constants.Depoimentos.Fields.Estado] != null
                                   ? item[Constants.Depoimentos.Fields.Estado].ToString()
                                   : null;
        }

        /// <summary>
        ///   Persiste uma propriedade para um campo (coluna) para que o LINQ to SharePoint possa salvar o valor da propriedade
        ///   para o campo na base de dados.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        public void MapTo(object listItem) {
            var item = (SPListItem)listItem;
            item[Constants.Depoimentos.Fields.Ramo] = Ramo;
            item[Constants.Depoimentos.Fields.ProdutoServico] = ProdutoServico;
            item[Constants.Depoimentos.Fields.Estado] = Estado;
        }

        /// <summary>
        ///   Resolve divergências nos valores de um ou mais campos no item da lista, respeitando o valor atual no cliente,
        ///   o valor atual na base de dados e o valor que foi originalmente retornado da base de dados.
        /// </summary>
        /// <param name = "mode">Um valor que especifica a regra para resolver as divergências.</param>
        /// <param name = "originalListItem">Um objeto que representa os valores do item da lista quando foi retornado da base de dados e pode ser convertido para SPListItem.</param>
        /// <param name = "databaseListItem">Um objeto que representa o valor atual do item da lista na base de dados e pode ser convertido para SPListItem.</param>
        public void Resolve(RefreshMode mode, object originalListItem, object databaseListItem) {
            var originalItem = (SPListItem)originalListItem;
            var databaseItem = (SPListItem)databaseListItem;

            var originalRamoValue = originalItem[Constants.Depoimentos.Fields.Ramo] != null
                                        ? originalItem[Constants.Depoimentos.Fields.Ramo].ToString()
                                        : null;
            var dbRamoValue = databaseItem[Constants.Depoimentos.Fields.Ramo] != null
                                  ? databaseItem[Constants.Depoimentos.Fields.Ramo].ToString()
                                  : null;

            var originalProdutoServicoValue = originalItem[Constants.Depoimentos.Fields.ProdutoServico] != null
                                           ? originalItem[Constants.Depoimentos.Fields.ProdutoServico].ToString()
                                           : null;
            var dbProdutoServicoValue = databaseItem[Constants.Depoimentos.Fields.ProdutoServico] != null
                                     ? databaseItem[Constants.Depoimentos.Fields.ProdutoServico].ToString()
                                     : null;

            var originalEstadoValue = originalItem[Constants.Depoimentos.Fields.Estado] != null
                                                    ? originalItem[Constants.Depoimentos.Fields.Estado].ToString()
                                                    : null;
            var dbEstadoValue = databaseItem[Constants.Depoimentos.Fields.Estado] != null
                                              ? databaseItem[Constants.Depoimentos.Fields.Estado].ToString()
                                              : null;

            switch (mode) {
                case RefreshMode.OverwriteCurrentValues:
                    Ramo = dbRamoValue;
                    ProdutoServico = dbProdutoServicoValue;
                    Estado = dbEstadoValue;
                    break;
                case RefreshMode.KeepCurrentValues:
                    databaseItem[Constants.Depoimentos.Fields.Ramo] = Ramo;
                    databaseItem[Constants.Depoimentos.Fields.ProdutoServico] = ProdutoServico;
                    databaseItem[Constants.Depoimentos.Fields.Estado] = Estado;
                    break;
                case RefreshMode.KeepChanges:
                    if (Ramo != originalRamoValue) databaseItem[Constants.Depoimentos.Fields.Ramo] = Ramo;
                    else if (Ramo == originalRamoValue && Ramo != dbRamoValue) Ramo = dbRamoValue;

                    if (ProdutoServico != originalProdutoServicoValue) databaseItem[Constants.Depoimentos.Fields.ProdutoServico] = ProdutoServico;
                    else if (ProdutoServico == originalProdutoServicoValue && ProdutoServico != dbProdutoServicoValue) ProdutoServico = dbProdutoServicoValue;

                    if (Estado != originalEstadoValue) databaseItem[Constants.Depoimentos.Fields.Estado] = Estado;
                    else if (Estado == originalEstadoValue && Estado != dbEstadoValue) Estado = dbEstadoValue;
                    break;
            }
        }
        #endregion
    }

    /// <summary>
    ///   Adiciona a funcionalidade de LINQ to SharePoint para as colunas da lista "Dicas" que
    ///   utilizem custom fields.
    /// </summary>
    public partial class DicasItem : ICustomMapping {
        #region Fields
        /// <summary>
        ///   Variável que armazena a categoria da dica.
        /// </summary>
        private string _categoria;

        /// <summary>
        /// Variável que armazena a data de publicação da dica
        /// </summary>
        private string _dataPublicacao;

        #endregion

        #region Properties
        /// <summary>
        ///   Retorna ou armazena a categoria da dica.
        /// </summary>
        public string Categoria {
            get { return _categoria; }
            set {
                if ((value == _categoria)) return;

                OnPropertyChanging("Categoria", _categoria);
                _categoria = value;
                OnPropertyChanged("Categoria");
            }
        }

        /// <summary>
        ///   Retorna ou armazena a categoria da dica.
        /// </summary>
        public string DataPublicacao {
            get { return _dataPublicacao; }
            set {
                if ((value == _dataPublicacao)) return;

                OnPropertyChanging("DataPublicacao", _dataPublicacao);
                _dataPublicacao = value;
                OnPropertyChanged("DataPublicacao");
            }
        }
        #endregion

        #region Implementation of ICustomMapping
        /// <summary>
        ///   Persiste um campo (coluna) para uma propriedade para que o LINQ to SharePoint possa carregar o conteúdo
        ///   do campo na base de dados para a propriedade que o representa.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        [CustomMapping(Columns = new[]
                                 {
                                     "Categoria",
                                     "DataPublicacao"
                                 })]
        public void MapFrom(object listItem) {
            var item = (SPListItem)listItem;
            var value = item[Constants.Dicas.Fields.Categoria];

            Categoria = value != null ? value.ToString() : null;

            value = item[Constants.Dicas.Fields.DataPublicacao];

            DataPublicacao = value != null ? value.ToString() : null;
        }

        /// <summary>
        ///   Persiste uma propriedade para um campo (coluna) para que o LINQ to SharePoint possa salvar o valor da propriedade
        ///   para o campo na base de dados.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        public void MapTo(object listItem) {
            var item = (SPListItem)listItem;
            item[Constants.Dicas.Fields.Categoria] = Categoria;
            //Não haverá alteração na data de publicação
        }

        /// <summary>
        ///   Resolve divergências nos valores de um ou mais campos no item da lista, respeitando o valor atual no cliente,
        ///   o valor atual na base de dados e o valor que foi originalmente retornado da base de dados.
        /// </summary>
        /// <param name = "mode">Um valor que especifica a regra para resolver as divergências.</param>
        /// <param name = "originalListItem">Um objeto que representa os valores do item da lista quando foi retornado da base de dados e pode ser convertido para SPListItem.</param>
        /// <param name = "databaseListItem">Um objeto que representa o valor atual do item da lista na base de dados e pode ser convertido para SPListItem.</param>
        public void Resolve(RefreshMode mode, object originalListItem, object databaseListItem) {
            var originalItem = (SPListItem)originalListItem;
            var databaseItem = (SPListItem)databaseListItem;

            var originalValue = originalItem[Constants.Dicas.Fields.Categoria] != null
                                    ? originalItem[Constants.Dicas.Fields.Categoria].ToString()
                                    : null;
            var dbValue = databaseItem[Constants.Dicas.Fields.Categoria] != null
                              ? databaseItem[Constants.Dicas.Fields.Categoria].ToString()
                              : null;

            //Não haverá alteração na data de publicação

            switch (mode) {
                case RefreshMode.OverwriteCurrentValues:
                    Categoria = dbValue;
                    break;
                case RefreshMode.KeepCurrentValues:
                    databaseItem[Constants.Dicas.Fields.Categoria] = Categoria;
                    break;
                case RefreshMode.KeepChanges:
                    if (Categoria != originalValue) databaseItem[Constants.Dicas.Fields.Categoria] = Categoria;
                    else if (Categoria == originalValue && Categoria != dbValue) Categoria = dbValue;
                    break;
            }
        }
        #endregion
    }

    /// <summary>
    ///   Adiciona a funcionalidade de LINQ to SharePoint para as colunas da lista "Downloads" que
    ///   utilizem custom fields.
    /// </summary>
    public partial class DownloadsItem : ICustomMapping {
        #region Fields
        /// <summary>
        ///   Variável que armazena os anexos do download.
        /// </summary>
        private string[] _anexos;

        /// <summary>
        ///   Variável que armazena o assunto do download.
        /// </summary>
        private string _assunto;

        /// <summary>
        ///   Variável que armazena a área do download.
        /// </summary>
        private string _área;
        #endregion

        #region Properties
        /// <summary>
        ///   Retorna ou armazena a área do download.
        /// </summary>
        public string Área {
            get { return _área; }
            set {
                if ((value == _área)) return;
                OnPropertyChanging("Área", _área);
                _área = value;
                OnPropertyChanged("Área");
            }
        }

        /// <summary>
        ///   Retorna ou armazena o assunto do download.
        /// </summary>
        public string Assunto {
            get { return _assunto; }
            set {
                if ((value == _assunto)) return;

                OnPropertyChanging("Assunto", _assunto);
                _assunto = value;
                OnPropertyChanged("Assunto");
            }
        }

        /// <summary>
        ///   Retorna ou armazena os anexos do download.
        /// </summary>
        public string[] Anexos {
            get { return _anexos; }
            set {
                if ((value == _anexos)) return;

                OnPropertyChanging("Anexos", _anexos);
                _anexos = value;
                OnPropertyChanged("Anexos");
            }
        }
        #endregion

        #region Implementation of ICustomMapping
        /// <summary>
        ///   Persiste um campo (coluna) para uma propriedade para que o LINQ to SharePoint possa carregar o conteúdo
        ///   do campo na base de dados para a propriedade que o representa.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        [CustomMapping(Columns = new[]
                                 {
                                     "_x00c1_rea",
                                     "Assunto",
                                     "Attachments"
                                 })]
        public void MapFrom(object listItem) {
            var item = (SPListItem)listItem;

            Área = item[Constants.Downloads.Fields.Area] != null ? item[Constants.Downloads.Fields.Area].ToString() : null;
            Assunto = item[Constants.Downloads.Fields.Assunto] != null ? item[Constants.Downloads.Fields.Assunto].ToString() : null;

            if (item.Attachments != null && item.Attachments.Count > 0) {
                var urlPrefix = item.Attachments.UrlPrefix;

                Anexos = (item.Attachments.Cast<object>().Select(attachment => string.Format("{0}{1}", urlPrefix, attachment))).ToArray();
            }
            else Anexos = null;
        }

        /// <summary>
        ///   Persiste uma propriedade para um campo (coluna) para que o LINQ to SharePoint possa salvar o valor da propriedade
        ///   para o campo na base de dados.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        public void MapTo(object listItem) {
            var item = (SPListItem)listItem;
            item[Constants.Downloads.Fields.Area] = Área;
            item[Constants.Downloads.Fields.Assunto] = Assunto;
            //Não será permitido modificar os anexos.
        }

        /// <summary>
        ///   Resolve divergências nos valores de um ou mais campos no item da lista, respeitando o valor atual no cliente,
        ///   o valor atual na base de dados e o valor que foi originalmente retornado da base de dados.
        /// </summary>
        /// <param name = "mode">Um valor que especifica a regra para resolver as divergências.</param>
        /// <param name = "originalListItem">Um objeto que representa os valores do item da lista quando foi retornado da base de dados e pode ser convertido para SPListItem.</param>
        /// <param name = "databaseListItem">Um objeto que representa o valor atual do item da lista na base de dados e pode ser convertido para SPListItem.</param>
        public void Resolve(RefreshMode mode, object originalListItem, object databaseListItem) {
            var originalItem = (SPListItem)originalListItem;
            var databaseItem = (SPListItem)databaseListItem;

            var originalAreaValue = originalItem[Constants.Downloads.Fields.Area] != null
                                        ? originalItem[Constants.Downloads.Fields.Area].ToString()
                                        : null;
            var dbAreaValue = databaseItem[Constants.Downloads.Fields.Area] != null
                                  ? databaseItem[Constants.Downloads.Fields.Area].ToString()
                                  : null;

            var originalAssuntoValue = originalItem[Constants.Downloads.Fields.Assunto] != null
                                           ? originalItem[Constants.Downloads.Fields.Assunto].ToString()
                                           : null;
            var dbAssuntoValue = databaseItem[Constants.Downloads.Fields.Assunto] != null
                                     ? databaseItem[Constants.Downloads.Fields.Assunto].ToString()
                                     : null;

            //Não será permitido modificar os Anexos.

            switch (mode) {
                case RefreshMode.OverwriteCurrentValues:
                    Área = dbAreaValue;
                    Assunto = dbAssuntoValue;
                    break;
                case RefreshMode.KeepCurrentValues:
                    databaseItem[Constants.Downloads.Fields.Area] = Área;
                    databaseItem[Constants.Downloads.Fields.Assunto] = Assunto;
                    break;
                case RefreshMode.KeepChanges:
                    if (Área != originalAreaValue) databaseItem[Constants.Downloads.Fields.Area] = Área;
                    else if (Área == originalAreaValue && Área != dbAreaValue) Área = dbAreaValue;

                    if (Assunto != originalAssuntoValue) databaseItem[Constants.Downloads.Fields.Assunto] = Assunto;
                    else if (Assunto == originalAssuntoValue && Assunto != dbAssuntoValue) Assunto = dbAssuntoValue;
                    break;
            }
        }
        #endregion
    }

    /// <summary>
    ///   Adiciona a funcionalidade de LINQ to SharePoint para as colunas da lista "Fotos" que
    ///   utilizem custom fields.
    /// </summary>
    public partial class FotosItem : ICustomMapping {
        #region Fields
        /// <summary>
        ///   Variável que armazena a galeria da foto.
        /// </summary>
        private string _galeria;
        #endregion

        #region Properties
        /// <summary>
        ///   Retorna ou armazena a galeria da foto.
        /// </summary>
        public string Galeria {
            get { return _galeria; }
            set {
                if ((value == _galeria)) return;

                OnPropertyChanging("Galeria", _galeria);
                _galeria = value;
                OnPropertyChanged("Galeria");
            }
        }
        #endregion

        #region Implementation of ICustomMapping
        /// <summary>
        ///   Persiste um campo (coluna) para uma propriedade para que o LINQ to SharePoint possa carregar o conteúdo
        ///   do campo na base de dados para a propriedade que o representa.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        [CustomMapping(Columns = new[]
                                 {
                                     "Galeria"
                                 })]
        public void MapFrom(object listItem) {
            var item = (SPListItem)listItem;
            var value = item[Constants.Fotos.Fields.Galeria];

            Galeria = value != null ? value.ToString() : null;
        }

        /// <summary>
        ///   Persiste uma propriedade para um campo (coluna) para que o LINQ to SharePoint possa salvar o valor da propriedade
        ///   para o campo na base de dados.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        public void MapTo(object listItem) {
            var item = (SPListItem)listItem;
            item[Constants.Fotos.Fields.Galeria] = Galeria;
        }

        /// <summary>
        ///   Resolve divergências nos valores de um ou mais campos no item da lista, respeitando o valor atual no cliente,
        ///   o valor atual na base de dados e o valor que foi originalmente retornado da base de dados.
        /// </summary>
        /// <param name = "mode">Um valor que especifica a regra para resolver as divergências.</param>
        /// <param name = "originalListItem">Um objeto que representa os valores do item da lista quando foi retornado da base de dados e pode ser convertido para SPListItem.</param>
        /// <param name = "databaseListItem">Um objeto que representa o valor atual do item da lista na base de dados e pode ser convertido para SPListItem.</param>
        public void Resolve(RefreshMode mode, object originalListItem, object databaseListItem) {
            var originalItem = (SPListItem)originalListItem;
            var databaseItem = (SPListItem)databaseListItem;

            var originalValue = originalItem[Constants.Fotos.Fields.Galeria] != null
                                    ? originalItem[Constants.Fotos.Fields.Galeria].ToString()
                                    : null;
            var dbValue = databaseItem[Constants.Fotos.Fields.Galeria] != null
                              ? databaseItem[Constants.Fotos.Fields.Galeria].ToString()
                              : null;

            switch (mode) {
                case RefreshMode.OverwriteCurrentValues:
                    Galeria = dbValue;
                    break;
                case RefreshMode.KeepCurrentValues:
                    databaseItem[Constants.Fotos.Fields.Galeria] = Galeria;
                    break;
                case RefreshMode.KeepChanges:
                    if (Galeria != originalValue) databaseItem[Constants.Fotos.Fields.Galeria] = Galeria;
                    else if (Galeria == originalValue && Galeria != dbValue) Galeria = dbValue;
                    break;
            }
        }
        #endregion
    }

    /// <summary>
    ///   Adiciona a funcionalidade de LINQ to SharePoint para as colunas da lista "Perguntas Frequentes" que
    ///   utilizem custom fields.
    /// </summary>
    public partial class PerguntasFrequentesItem : ICustomMapping {
        #region Fields
        /// <summary>
        ///   Variável que armazena o assunto da pergunta.
        /// </summary>
        private string _assunto;

        /// <summary>
        ///   Variável que armazena a área da pergunta.
        /// </summary>
        private string _área;
        #endregion

        #region Properties
        /// <summary>
        ///   Retorna ou armazena a área da pergunta.
        /// </summary>
        public string Área {
            get { return _área; }
            set {
                if ((value == _área)) return;

                OnPropertyChanging("Área", _área);
                _área = value;
                OnPropertyChanged("Área");
            }
        }

        /// <summary>
        ///   Retorna ou armazena o assunto da pergunta.
        /// </summary>
        public string Assunto {
            get { return _assunto; }
            set {
                if ((value == _assunto)) return;

                OnPropertyChanging("Assunto", _assunto);
                _assunto = value;
                OnPropertyChanged("Assunto");
            }
        }
        #endregion

        #region Implementation of ICustomMapping
        /// <summary>
        ///   Persiste um campo (coluna) para uma propriedade para que o LINQ to SharePoint possa carregar o conteúdo
        ///   do campo na base de dados para a propriedade que o representa.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        [CustomMapping(Columns = new[]
                                 {
                                     "_x00c1_rea",
                                     "Assunto"
                                 })]
        public void MapFrom(object listItem) {
            var item = (SPListItem)listItem;

            Área = item[Constants.PerguntasFrequentes.Fields.Area] != null ? item[Constants.PerguntasFrequentes.Fields.Area].ToString() : null;
            Assunto = item[Constants.PerguntasFrequentes.Fields.Assunto] != null
                          ? item[Constants.PerguntasFrequentes.Fields.Assunto].ToString()
                          : null;
        }

        /// <summary>
        ///   Persiste uma propriedade para um campo (coluna) para que o LINQ to SharePoint possa salvar o valor da propriedade
        ///   para o campo na base de dados.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        public void MapTo(object listItem) {
            var item = (SPListItem)listItem;
            item[Constants.PerguntasFrequentes.Fields.Area] = Área;
            item[Constants.PerguntasFrequentes.Fields.Assunto] = Assunto;
        }

        /// <summary>
        ///   Resolve divergências nos valores de um ou mais campos no item da lista, respeitando o valor atual no cliente,
        ///   o valor atual na base de dados e o valor que foi originalmente retornado da base de dados.
        /// </summary>
        /// <param name = "mode">Um valor que especifica a regra para resolver as divergências.</param>
        /// <param name = "originalListItem">Um objeto que representa os valores do item da lista quando foi retornado da base de dados e pode ser convertido para SPListItem.</param>
        /// <param name = "databaseListItem">Um objeto que representa o valor atual do item da lista na base de dados e pode ser convertido para SPListItem.</param>
        public void Resolve(RefreshMode mode, object originalListItem, object databaseListItem) {
            var originalItem = (SPListItem)originalListItem;
            var databaseItem = (SPListItem)databaseListItem;

            var originalAreaValue = originalItem[Constants.PerguntasFrequentes.Fields.Area] != null
                                        ? originalItem[Constants.PerguntasFrequentes.Fields.Area].ToString()
                                        : null;
            var dbAreaValue = databaseItem[Constants.PerguntasFrequentes.Fields.Area] != null
                                  ? databaseItem[Constants.PerguntasFrequentes.Fields.Area].ToString()
                                  : null;

            var originalAssuntoValue = originalItem[Constants.PerguntasFrequentes.Fields.Assunto] != null
                                           ? originalItem[Constants.PerguntasFrequentes.Fields.Assunto].ToString()
                                           : null;
            var dbAssuntoValue = databaseItem[Constants.PerguntasFrequentes.Fields.Assunto] != null
                                     ? databaseItem[Constants.PerguntasFrequentes.Fields.Assunto].ToString()
                                     : null;

            switch (mode) {
                case RefreshMode.OverwriteCurrentValues:
                    Área = dbAreaValue;
                    Assunto = dbAssuntoValue;
                    break;
                case RefreshMode.KeepCurrentValues:
                    databaseItem[Constants.PerguntasFrequentes.Fields.Area] = Área;
                    databaseItem[Constants.PerguntasFrequentes.Fields.Assunto] = Assunto;
                    break;
                case RefreshMode.KeepChanges:
                    if (Área != originalAreaValue) databaseItem[Constants.PerguntasFrequentes.Fields.Area] = Área;
                    else if (Área == originalAreaValue && Área != dbAreaValue) Área = dbAreaValue;

                    if (Assunto != originalAssuntoValue) databaseItem[Constants.PerguntasFrequentes.Fields.Assunto] = Assunto;
                    else if (Assunto == originalAssuntoValue && Assunto != dbAssuntoValue) Assunto = dbAssuntoValue;
                    break;
            }
        }
        #endregion
    }

    /// <summary>
    ///   Adiciona a funcionalidade de LINQ to SharePoint para as colunas da lista "Perguntas Frequentes Ecommerce" que
    ///   utilizem custom fields.
    /// </summary>
    public partial class PerguntasFrequentesEcommerceItem : ICustomMapping
    {
        #region Fields
        /// <summary>
        ///   Variável que armazena o assunto da pergunta.
        /// </summary>
        private string _assunto;

        /// <summary>
        ///   Variável que armazena a área da pergunta.
        /// </summary>
        private string _área;
        #endregion

        #region Properties
        /// <summary>
        ///   Retorna ou armazena a área da pergunta.
        /// </summary>
        public string Área
        {
            get { return _área; }
            set
            {
                if ((value == _área)) return;

                OnPropertyChanging("Área", _área);
                _área = value;
                OnPropertyChanged("Área");
            }
        }

        /// <summary>
        ///   Retorna ou armazena o assunto da pergunta.
        /// </summary>
        public string Assunto
        {
            get { return _assunto; }
            set
            {
                if ((value == _assunto)) return;

                OnPropertyChanging("Assunto", _assunto);
                _assunto = value;
                OnPropertyChanged("Assunto");
            }
        }
        #endregion

        #region Implementation of ICustomMapping
        /// <summary>
        ///   Persiste um campo (coluna) para uma propriedade para que o LINQ to SharePoint possa carregar o conteúdo
        ///   do campo na base de dados para a propriedade que o representa.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        [CustomMapping(Columns = new[]
                                 {
                                     "_x00c1_rea",
                                     "Assunto"
                                 })]
        public void MapFrom(object listItem)
        {
            var item = (SPListItem)listItem;

            Área = item[Constants.PerguntasFrequentesEcommerce.Fields.Area] != null ? item[Constants.PerguntasFrequentesEcommerce.Fields.Area].ToString() : null;
            Assunto = item[Constants.PerguntasFrequentesEcommerce.Fields.Assunto] != null
                          ? item[Constants.PerguntasFrequentesEcommerce.Fields.Assunto].ToString()
                          : null;
        }

        /// <summary>
        ///   Persiste uma propriedade para um campo (coluna) para que o LINQ to SharePoint possa salvar o valor da propriedade
        ///   para o campo na base de dados.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        public void MapTo(object listItem)
        {
            var item = (SPListItem)listItem;
            item[Constants.PerguntasFrequentesEcommerce.Fields.Area] = Área;
            item[Constants.PerguntasFrequentesEcommerce.Fields.Assunto] = Assunto;
        }

        /// <summary>
        ///   Resolve divergências nos valores de um ou mais campos no item da lista, respeitando o valor atual no cliente,
        ///   o valor atual na base de dados e o valor que foi originalmente retornado da base de dados.
        /// </summary>
        /// <param name = "mode">Um valor que especifica a regra para resolver as divergências.</param>
        /// <param name = "originalListItem">Um objeto que representa os valores do item da lista quando foi retornado da base de dados e pode ser convertido para SPListItem.</param>
        /// <param name = "databaseListItem">Um objeto que representa o valor atual do item da lista na base de dados e pode ser convertido para SPListItem.</param>
        public void Resolve(RefreshMode mode, object originalListItem, object databaseListItem)
        {
            var originalItem = (SPListItem)originalListItem;
            var databaseItem = (SPListItem)databaseListItem;

            var originalAreaValue = originalItem[Constants.PerguntasFrequentesEcommerce.Fields.Area] != null
                                        ? originalItem[Constants.PerguntasFrequentesEcommerce.Fields.Area].ToString()
                                        : null;
            var dbAreaValue = databaseItem[Constants.PerguntasFrequentesEcommerce.Fields.Area] != null
                                  ? databaseItem[Constants.PerguntasFrequentesEcommerce.Fields.Area].ToString()
                                  : null;

            var originalAssuntoValue = originalItem[Constants.PerguntasFrequentesEcommerce.Fields.Assunto] != null
                                           ? originalItem[Constants.PerguntasFrequentesEcommerce.Fields.Assunto].ToString()
                                           : null;
            var dbAssuntoValue = databaseItem[Constants.PerguntasFrequentesEcommerce.Fields.Assunto] != null
                                     ? databaseItem[Constants.PerguntasFrequentesEcommerce.Fields.Assunto].ToString()
                                     : null;

            switch (mode)
            {
                case RefreshMode.OverwriteCurrentValues:
                    Área = dbAreaValue;
                    Assunto = dbAssuntoValue;
                    break;
                case RefreshMode.KeepCurrentValues:
                    databaseItem[Constants.PerguntasFrequentesEcommerce.Fields.Area] = Área;
                    databaseItem[Constants.PerguntasFrequentesEcommerce.Fields.Assunto] = Assunto;
                    break;
                case RefreshMode.KeepChanges:
                    if (Área != originalAreaValue) databaseItem[Constants.PerguntasFrequentesEcommerce.Fields.Area] = Área;
                    else if (Área == originalAreaValue && Área != dbAreaValue) Área = dbAreaValue;

                    if (Assunto != originalAssuntoValue) databaseItem[Constants.PerguntasFrequentesEcommerce.Fields.Assunto] = Assunto;
                    else if (Assunto == originalAssuntoValue && Assunto != dbAssuntoValue) Assunto = dbAssuntoValue;
                    break;
            }
        }
        #endregion
    }

    /// <summary>
    ///   Adiciona a funcionalidade de LINQ to SharePoint para as colunas da lista "Prêmios" que
    ///   utilizem custom fields.
    /// </summary>
    public partial class PrêmiosItem : ICustomMapping {
        #region Fields
        /// <summary>
        ///   Variável que armazena a categoria do prêmio.
        /// </summary>
        private string _categoria;
        #endregion

        #region Properties
        /// <summary>
        ///   Retorna ou armazena a categoria do prêmio.
        /// </summary>
        public string Categoria {
            get { return _categoria; }
            set {
                if ((value == _categoria)) return;

                OnPropertyChanging("Categoria", _categoria);
                _categoria = value;
                OnPropertyChanged("Categoria");
            }
        }
        #endregion

        #region Implementation of ICustomMapping
        /// <summary>
        ///   Persiste um campo (coluna) para uma propriedade para que o LINQ to SharePoint possa carregar o conteúdo
        ///   do campo na base de dados para a propriedade que o representa.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        [CustomMapping(Columns = new[]
                                 {
                                     "Categoria"
                                 })]
        public void MapFrom(object listItem) {
            var item = (SPListItem)listItem;
            var value = item[Constants.Premios.Fields.Categoria];

            Categoria = value != null ? value.ToString() : null;
        }

        /// <summary>
        ///   Persiste uma propriedade para um campo (coluna) para que o LINQ to SharePoint possa salvar o valor da propriedade
        ///   para o campo na base de dados.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        public void MapTo(object listItem) {
            var item = (SPListItem)listItem;
            item[Constants.Premios.Fields.Categoria] = Categoria;
        }

        /// <summary>
        ///   Resolve divergências nos valores de um ou mais campos no item da lista, respeitando o valor atual no cliente,
        ///   o valor atual na base de dados e o valor que foi originalmente retornado da base de dados.
        /// </summary>
        /// <param name = "mode">Um valor que especifica a regra para resolver as divergências.</param>
        /// <param name = "originalListItem">Um objeto que representa os valores do item da lista quando foi retornado da base de dados e pode ser convertido para SPListItem.</param>
        /// <param name = "databaseListItem">Um objeto que representa o valor atual do item da lista na base de dados e pode ser convertido para SPListItem.</param>
        public void Resolve(RefreshMode mode, object originalListItem, object databaseListItem) {
            var originalItem = (SPListItem)originalListItem;
            var databaseItem = (SPListItem)databaseListItem;

            var originalValue = originalItem[Constants.Premios.Fields.Categoria] != null
                                    ? originalItem[Constants.Premios.Fields.Categoria].ToString()
                                    : null;
            var dbValue = databaseItem[Constants.Dicas.Fields.Categoria] != null
                              ? databaseItem[Constants.Premios.Fields.Categoria].ToString()
                              : null;

            switch (mode) {
                case RefreshMode.OverwriteCurrentValues:
                    Categoria = dbValue;
                    break;
                case RefreshMode.KeepCurrentValues:
                    databaseItem[Constants.Premios.Fields.Categoria] = Categoria;
                    break;
                case RefreshMode.KeepChanges:
                    if (Categoria != originalValue) databaseItem[Constants.Premios.Fields.Categoria] = Categoria;
                    else if (Categoria == originalValue && Categoria != dbValue) Categoria = dbValue;
                    break;
            }
        }
        #endregion
    }

    /// <summary>
    ///   Adiciona a funcionalidade de LINQ to SharePoint para as colunas da lista "Sons" que
    ///   utilizem custom fields.
    /// </summary>
    public partial class SonsItem : ICustomMapping {
        #region Fields
        /// <summary>
        ///   Variável que armazena o tipo do som.
        /// </summary>
        private string _tipoDoSom;

        /// <summary>
        ///   Variável que armazena os anexos do download.
        /// </summary>
        private string[] _anexos;
        #endregion

        #region Properties
        /// <summary>
        ///   Retorna ou armazena o tipo do som;
        /// </summary>
        public string TipoDoSom {
            get { return _tipoDoSom; }
            set {
                if ((value == _tipoDoSom)) return;

                OnPropertyChanging("TipoDoSom", _tipoDoSom);
                _tipoDoSom = value;
                OnPropertyChanged("TipoDoSom");
            }
        }

        /// <summary>
        ///   Retorna ou armazena os anexos do download.
        /// </summary>
        public string[] Anexos {
            get { return _anexos; }
            set {
                if ((value == _anexos)) return;

                OnPropertyChanging("Anexos", _anexos);
                _anexos = value;
                OnPropertyChanged("Anexos");
            }
        }
        #endregion

        #region Implementation of ICustomMapping
        /// <summary>
        ///   Persiste um campo (coluna) para uma propriedade para que o LINQ to SharePoint possa carregar o conteúdo
        ///   do campo na base de dados para a propriedade que o representa.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        [CustomMapping(Columns = new[]
                                 {
                                     "Tipo_x0020_do_x0020_Som",
                                     "Attachments"
                                 })]
        public void MapFrom(object listItem) {
            var item = (SPListItem)listItem;
            var value = item[Constants.Sons.Fields.TipoDoSom];

            TipoDoSom = value != null ? value.ToString() : null;

            if (item.Attachments != null && item.Attachments.Count > 0) {
                var urlPrefix = item.Attachments.UrlPrefix;

                Anexos = (item.Attachments.Cast<object>().Select(attachment => string.Format("{0}{1}", urlPrefix, attachment))).ToArray();
            }
            else Anexos = null;
        }

        /// <summary>
        ///   Persiste uma propriedade para um campo (coluna) para que o LINQ to SharePoint possa salvar o valor da propriedade
        ///   para o campo na base de dados.
        /// </summary>
        /// <param name = "listItem">Um objeto que representa um item de lista e que pode ser convertido para um SPListItem.</param>
        public void MapTo(object listItem) {
            var item = (SPListItem)listItem;
            item[Constants.Sons.Fields.TipoDoSom] = TipoDoSom;
        }

        /// <summary>
        ///   Resolve divergências nos valores de um ou mais campos no item da lista, respeitando o valor atual no cliente,
        ///   o valor atual na base de dados e o valor que foi originalmente retornado da base de dados.
        /// </summary>
        /// <param name = "mode">Um valor que especifica a regra para resolver as divergências.</param>
        /// <param name = "originalListItem">Um objeto que representa os valores do item da lista quando foi retornado da base de dados e pode ser convertido para SPListItem.</param>
        /// <param name = "databaseListItem">Um objeto que representa o valor atual do item da lista na base de dados e pode ser convertido para SPListItem.</param>
        public void Resolve(RefreshMode mode, object originalListItem, object databaseListItem) {
            var originalItem = (SPListItem)originalListItem;
            var databaseItem = (SPListItem)databaseListItem;

            var originalValue = originalItem[Constants.Sons.Fields.TipoDoSom] != null
                                    ? originalItem[Constants.Sons.Fields.TipoDoSom].ToString()
                                    : null;
            var dbValue = databaseItem[Constants.Sons.Fields.TipoDoSom] != null
                              ? databaseItem[Constants.Sons.Fields.TipoDoSom].ToString()
                              : null;

            switch (mode) {
                case RefreshMode.OverwriteCurrentValues:
                    TipoDoSom = dbValue;
                    break;
                case RefreshMode.KeepCurrentValues:
                    databaseItem[Constants.Sons.Fields.TipoDoSom] = TipoDoSom;
                    break;
                case RefreshMode.KeepChanges:
                    if (TipoDoSom != originalValue) databaseItem[Constants.Sons.Fields.TipoDoSom] = TipoDoSom;
                    else if (TipoDoSom == originalValue && TipoDoSom != dbValue) TipoDoSom = dbValue;
                    break;
            }
        }
        #endregion
    }
}