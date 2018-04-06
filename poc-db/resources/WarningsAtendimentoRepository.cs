/*
© Copyright 2017 Rede S.A.
Autor : Lucas Akira Uehara
Empresa : Iteris Consultoria e Software
*/
using Microsoft.SharePoint;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Constants;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Enums;
using Rede.PN.AtendimentoDigital.SharePoint.Repository;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rede.PN.AtendimentoDigital.SharePoint.Repository
{
    /// <summary>
    /// Repositório do WarningAtendimento.
    /// </summary>
    public class WarningsAtendimentoRepository : BaseEntityRepository<WarningsAtendimentoEntity>, IWarningsAtendimento
    {
        /// <summary>
        /// Define o nome da lista.
        /// </summary>
        protected override string ListName
        {
            get { return Lists.WarningsAtendimento; }
        }
        /// <summary>
        /// Obtém uma lista de warnings.
        /// </summary>
        /// <param name="web">SPWeb.</param>
        /// <returns>Lista de Warnings do Atendimento Digital.</returns>
        public List<WarningsAtendimentoEntity> GetAll(SPWeb web)
        {
            SPQuery SPQuery = new SPQuery();
            SPQuery.RecurrenceOrderBy = true;

            return GetListItems(SPQuery, web);
        }

        protected override Dictionary<string, object> GatherParameters(WarningsAtendimentoEntity entity, SPWeb web)
        {
            Dictionary<String, Object> fields = new Dictionary<String, Object>();
            fields.Add(Fields.Segmento, entity.Segmentos);
            fields.Add(Fields.NumeroPV, entity.NumerosPV);
            fields.Add(Fields.Texto, entity.Texto);
            fields.Add(Fields.TextoBotao, entity.TextoBotao);
            fields.Add(Fields.DataInicioExibicao, entity.DataInicioExibicao);
            fields.Add(Fields.DataFimExibicao, entity.DataFimExibicao);
            fields.Add(Fields.Tipo, entity.Tipo);
            fields.Add(Fields.URLExibicao, entity.URLExibicao);
            fields.Add(Fields.URLDestino, entity.URLDestino);

            return fields;
        }

        /// <summary>
        /// PopulateEntity 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override WarningsAtendimentoEntity PopulateEntity(SPListItem item)
        {
            WarningsAtendimentoEntity entity = new WarningsAtendimentoEntity();
            entity.Segmentos = TrataArrayString(Fields.Segmento, item);
            entity.NumerosPV = TrataArrayString(Fields.NumeroPV, item);
            entity.Texto = (String)item[Fields.Texto];
            entity.TextoBotao = (String)item[Fields.TextoBotao] ?? String.Empty;
            entity.DataInicioExibicao = (DateTime)item[Fields.DataInicioExibicao];
            entity.DataFimExibicao = (DateTime)item[Fields.DataFimExibicao];
            entity.Tipo = (String)item[Fields.Tipo];
            entity.URLExibicao = (String)item[Fields.URLExibicao] ?? String.Empty;
            entity.URLDestino = (String)item[Fields.URLDestino] ?? String.Empty;

            return entity;
        }
        /// <summary>
        /// Trata Array de String
        /// </summary>
        /// <param name="fields">Nome do campo.</param>
        /// <param name="input">String de entrada para ser tratado.</param>
        /// <param name="separador">Char utilizado como separador. Default: ';'</param>
        /// <returns>Array de string tratado.</returns>
        private String[] TrataArrayString(String fields, SPListItem input = null, Char separador = ';')
        {
            String[] output = new String[] { };

            if (input[fields] != null && !String.IsNullOrEmpty(fields))
            {
                output = input[fields].ToString().Split(separador);

                output = (from s in output
                          where !String.IsNullOrEmpty(s)
                          select s.RemoverEspacos().RemoverCaracteresEspeciais()).ToArray<String>();
            }

            return output;
        }
    }
}
