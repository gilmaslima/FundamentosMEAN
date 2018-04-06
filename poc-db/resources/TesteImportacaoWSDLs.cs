/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redecard.PN.Comum;

namespace Redecard.PN.Maximo.Test
{
    [TestClass]
    public class TesteImportacaoWSDLs
    {
        /// <summary>
        /// Teste para validação dos enumeradores importados na camada Agentes dos serviços externos.
        /// Verifica se possui a ocorrência do caractere '?' na classe 'Reference.cs' gerada.
        /// </summary>
        [TestMethod]                    
        public void TesteDeVerificacaoDeDescricaoDeEnumeradores()
        {            
            //Obtém o Assembly da camada de Agentes
            Assembly assembly = Assembly.GetAssembly(typeof(Agentes.OrdemServico));
          
            if(assembly == null) 
                throw new Exception("'Assembly Redecard.PN.Maximo.Agentes não encontrado.'");

            StringBuilder validacoes = new StringBuilder();

            //Percorre todos os Enums, vericando o conteúdo do atributo do tipo XmlEnumAttribute de cada valor do Enum
            foreach (Type typeEnum in assembly.GetTypes().Where(t => t.IsEnum))
            {
                Console.WriteLine("========== Verificando Enum '" + typeEnum.FullName + "' ==========");
                Console.WriteLine();
                var values = Enum.GetValues(typeEnum);

                //Percorre os valores do Enum
                foreach (var value in values)
                {
                    String descricao = (value as Enum).GetDescription<XmlEnumAttribute, String>(attr => attr.Name);

                    //Se não possui o atributo XmlEnumAttribute, valor do Enum não possui caracteres especiais: OK
                    if (String.IsNullOrEmpty(descricao))
                        Console.WriteLine("\t[OK] " + value.ToString());
                    else
                    {
                        //Se valor do atributo possui o caractere ?, valor inválido de Enum
                        if (descricao.Contains('?'))
                        {
                            Console.WriteLine("\t>>>>>-----=====> [INVÁLIDO] " + descricao + "<=====-----<<<<<");
                            validacoes.AppendLine().AppendLine("Enumerador inválido: " + typeEnum.FullName + "." +
                                value.ToString() + " ('" + descricao + "')");
                        }
                        else
                            Console.WriteLine("\t[OK] " + descricao);
                    }
                }
                Console.WriteLine();
            }

            //Se foi encontrado algum valor de Enum inválido, acusa erro
            Assert.IsTrue(validacoes.Length == 0, validacoes.ToString());
        }
    }
}