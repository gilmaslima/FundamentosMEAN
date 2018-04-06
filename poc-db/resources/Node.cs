/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;

namespace Redecard.PN.Comum.SharePoint.LAYOUTS.Redecard.Comum
{
    /// <summary>
    /// Classe interna para montagem hierárquica dos dados de um objeto (utiliza Reflection)
    /// </summary>
    public class Node
    {
        #region [ Propriedades ]

        /// <summary>
        /// Objeto
        /// </summary>
        public Object Objeto { get; private set; }

        /// <summary>
        /// Nome do objeto
        /// </summary>
        public String Nome { get; private set; }

        /// <summary>
        /// Filhos do objeto
        /// </summary>
        public List<Node> Filhos { get; private set; }

        /// <summary>
        /// Flag indicando se o nó é folha
        /// </summary>
        public Boolean Folha { get; private set; }

        #endregion

        #region [ Construtores ]

        /// <summary>
        /// Construtor padrão
        /// </summary>
        /// <param name="nome">Nome do objeto</param>
        /// <param name="objeto">Objeto que será processado</param>
        public Node(String nome, Object objeto)
        {
            this.Nome = nome;
            this.Objeto = objeto;
            this.Filhos = new List<Node>();

            Processar();
        }

        #endregion

        /// <summary>
        /// Processa Objeto e converte em estrutura de Node.
        /// </summary>
        private void Processar()
        {
            //Se objeto é nulo, não precisa processar os filhos
            if (this.Objeto == null)
            {
                this.Folha = true;
                return;
            }
            else
            {
                Type t = this.Objeto.GetType();

                //Se for tipo primitivo, é folha, e não precisa expandir o objeto
                if (t == typeof(String) || t == typeof(Int16) || t == typeof(Int32) || t == typeof(Int64)
                    || t == typeof(Boolean) || t == typeof(Decimal) || t == typeof(DateTime))
                {
                    this.Folha = true;
                }
                else
                {
                    //Se for IEnumerable
                    if (t.GetProperty("Count") != null)
                    {
                        var count = (Int32)t.GetProperty("Count").GetValue(this.Objeto, null);
                        for (Int32 i = 0; i < count; i++)
                        {
                            Object objFilho = t.GetProperty("Item").GetValue(this.Objeto, new Object[] { i });
                            String nomeFilho = String.Format("{0}[{1}]", this.Nome, i);
                            this.Filhos.Add(new Node(nomeFilho, objFilho));
                        }
                        this.Nome = String.Concat(this.Nome, " (", count, ")");
                    }
                    //Se for array
                    else if (t.IsArray)
                    {
                        Array arr = this.Objeto as Array;
                        for (Int32 i = 0; i < arr.Length; i++)
                        {
                            Object objFilho = arr.GetValue(i);
                            String nomeFilho = String.Format("{0}[{1}]", this.Nome, i);
                            this.Filhos.Add(new Node(nomeFilho, objFilho));
                        }
                        this.Nome = String.Concat(this.Nome, " (", arr.Length, ")");
                    }
                    //Se for objeto complexo
                    else
                    {
                        var props = t.GetProperties();
                        if (props != null)
                        {
                            foreach (PropertyInfo prop in t.GetProperties().OrderBy(p => p.Name))
                            {
                                if (prop.GetIndexParameters().Length == 0 && prop.GetGetMethod() != null)
                                {
                                    Object objFilho = prop.GetValue(this.Objeto, null);
                                    this.Filhos.Add(new Node(prop.Name, objFilho));
                                }
                            }
                        }

                        var fields = t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                        if (fields != null)
                        {
                            foreach (FieldInfo field in fields.OrderBy(p => p.Name))
                            {
                                Object objFilho = field.GetValue(this.Objeto);
                                if (field.FieldType.IsClass)
                                    this.Filhos.Add(new Node(field.Name, objFilho));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Converte instância para TreeNode
        /// </summary>
        /// <returns>TreeNode</returns>
        public TreeNode ToTreeNode()
        {
            String nome = String.Concat(
                this.Nome,
                this.Folha ? ": " : String.Empty,
                this.Folha ? Convert.ToString(this.Objeto) : String.Empty);

            var node = new TreeNode(nome);
            node.SelectAction = TreeNodeSelectAction.None;

            foreach (Node filho in this.Filhos)
                node.ChildNodes.Add(filho.ToTreeNode());

            return node;
        }
    }
}