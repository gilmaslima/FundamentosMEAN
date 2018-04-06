/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redecard.PN.Maximo.Agentes.Mapeadores;

namespace Redecard.PN.Maximo.Test
{
    [TestClass()]
    public class TestesMapeamento
    {
        #region [ Configuração de Mapeamentos ]

        /// <summary>
        /// Teste para verificar se o mapeamento bidirecional entre Modelos da Aplicação 
        /// para Modelos do Serviço Máximo estão corretamente configurados.
        /// </summary>
        [TestMethod, TestCategory("Configuração AutoMapper")]
        public void VerificarConfiguracaoMapeamentos()
        {
            try
            {
                Mapeador.Configurar();
                Mapper.AssertConfigurationIsValid();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion

        #region [ Mapeamento Ordem de Serviço ]

        [TestMethod, TestCategory("Mapeamento Ordem de Serviço")]
        public void Aluguel2t_aluguel()
        {
            Mapeador.Configurar();

            //Criação de dados para teste, com DataInicioCobranca não informada
            var aluguel = new Modelo.OrdemServico.Aluguel
            {
                DataInicioCobranca = null,
                Escalonamento = new List<Modelo.OrdemServico.MesValor>(new[] {  
                    new Modelo.OrdemServico.MesValor { Mes = Modelo.OrdemServico.TipoMeses.ABRIL, Valor = 123 },
                    new Modelo.OrdemServico.MesValor { Mes = Modelo.OrdemServico.TipoMeses.DEZEMBRO, Valor = 321 },
                }),
                Isento = true,
                Sazonalidade = new List<Modelo.OrdemServico.MesValor>(new[] {  
                    new Modelo.OrdemServico.MesValor { Mes = Modelo.OrdemServico.TipoMeses.FEVEREIRO, Valor = 999.99 },
                    new Modelo.OrdemServico.MesValor { Mes = Modelo.OrdemServico.TipoMeses.SETEMBRO, Valor = 555.42 },
                }),
                ValorUnitario = 123.456
            };

            //Mapeamento
            var aluguelOS = Mapper.Map<Agentes.TGAOrdemServico.t_aluguel>(aluguel);

            //Validação dos dados mapeados (sem a data início cobrança especificada)
            Assert.AreEqual(aluguelOS.data_inicio_cobrancaSpecified, false);
            Assert.AreEqual(aluguelOS.escalonamento.Length, aluguel.Escalonamento.Count);
            Assert.AreEqual(aluguelOS.escalonamento[0].mes, Agentes.TGAOrdemServico.d_meses.ABRIL);
            Assert.AreEqual(aluguelOS.escalonamento[0].valor, 123);
            Assert.AreEqual(aluguelOS.sazonalidade.Length, aluguel.Sazonalidade.Count);
            Assert.AreEqual(aluguelOS.sazonalidade[1].mes, Agentes.TGAOrdemServico.d_meses.SETEMBRO);
            Assert.AreEqual(aluguelOS.sazonalidade[1].valor, 555.42);
            Assert.AreEqual(aluguelOS.valor_unitario, aluguel.ValorUnitario);

            //Definindo uma data de início da cobrança e verificando correto mapeamento
            aluguel.DataInicioCobranca = DateTime.Now.AddDays(5);
            aluguelOS = Mapper.Map<Agentes.TGAOrdemServico.t_aluguel>(aluguel);
            Assert.AreEqual(aluguelOS.data_inicio_cobrancaSpecified, true);
            Assert.AreEqual(aluguelOS.data_inicio_cobranca, aluguel.DataInicioCobranca);

            //Converte o modelo TGAOrdemServico para modelo de Negócio, e compara com objeto original
            var aluguel2 = Mapper.Map<Modelo.OrdemServico.Aluguel>(aluguelOS);
            var aluguelOS2 = Mapper.Map<Agentes.TGAOrdemServico.t_aluguel>(aluguel2);

            Assert.IsTrue(AreObjectsEqual(aluguel, aluguel2));
            Assert.IsTrue(AreObjectsEqual(aluguelOS, aluguelOS2));
        }

        [TestMethod, TestCategory("Mapeamento Ordem de Serviço")]
        public void OSDetalhada2t_os_detalhada()
        {
            Mapeador.Configurar();

            var os = new Modelo.OrdemServico.OSDetalhada();
            var osOS = Mapper.Map<Agentes.TGAOrdemServico.t_os_detalhada>(os);

            //Valores nullables não informados
            Assert.AreEqual(osOS.data_atendimentoSpecified, false);
            Assert.AreEqual(osOS.data_programadaSpecified, false);

            //Atribução de valores às propriedades nullables
            os.DataAtendimento = DateTime.Now.AddDays(1);
            os.DataProgramada = DateTime.Now.AddDays(2);

            //Valores nullables informados
            osOS = Mapper.Map<Agentes.TGAOrdemServico.t_os_detalhada>(os);
            Assert.AreEqual(osOS.data_atendimentoSpecified, true);
            Assert.AreEqual(osOS.data_programadaSpecified, true);
            Assert.AreEqual(osOS.data_atendimento, os.DataAtendimento);
            Assert.AreEqual(osOS.data_programada, os.DataProgramada);

            var os2 = Mapper.Map<Modelo.OrdemServico.OSDetalhada>(osOS);
            var osOS2 = Mapper.Map<Agentes.TGAOrdemServico.t_os_detalhada>(os2);

            Assert.IsTrue(AreObjectsEqual(os, os2));
            Assert.IsTrue(AreObjectsEqual(osOS, osOS2));
        }

        [TestMethod, TestCategory("Mapeamento Ordem de Serviço")]
        public void VendaDigitadaTerminal2t_venda_digitada_terminal()
        {
            Mapeador.Configurar();

            var venda = new Modelo.OrdemServico.VendaDigitadaTerminal();
            var vendaOS = Mapper.Map<Agentes.TGAOrdemServico.t_venda_digitada_terminal>(venda);

            //Valores nullables não informados
            Assert.AreEqual(vendaOS.cvc2_obrigatorioSpecified, false);
            Assert.AreEqual(vendaOS.habilitada_receptivoSpecified, false);

            //Atribução de valores às propriedades nullables
            venda.Cvc2Obrigatorio = true;
            venda.HabilitadaReceptivo = false;

            //Valores nullables informados
            vendaOS = Mapper.Map<Agentes.TGAOrdemServico.t_venda_digitada_terminal>(venda);
            Assert.AreEqual(vendaOS.cvc2_obrigatorioSpecified, true);
            Assert.AreEqual(vendaOS.habilitada_receptivoSpecified, true);
            Assert.AreEqual(vendaOS.cvc2_obrigatorio, venda.Cvc2Obrigatorio);
            Assert.AreEqual(vendaOS.habilitada_receptivo, venda.HabilitadaReceptivo);

            var venda2 = Mapper.Map<Modelo.OrdemServico.VendaDigitadaTerminal>(vendaOS);
            var vendaOS2 = Mapper.Map<Agentes.TGAOrdemServico.t_venda_digitada_terminal>(venda2);

            Assert.IsTrue(AreObjectsEqual(venda, venda2));
            Assert.IsTrue(AreObjectsEqual(vendaOS, vendaOS2));
        }

        #endregion

        #region [ Performance ]

        //[TestMethod, TestCategory("Performance")]
        public void TestePerformanceInterceptor()
        {
            var autenticacao = new Modelo.OrdemServico.Autenticacao();
            var filtro = new Modelo.OrdemServico.FiltroOS();
            Int32 iteracoes = 1000;
            Stopwatch watch = new Stopwatch();

            Agentes.OrdemServico.Instancia.ConsultarOS(autenticacao, filtro);
            new Agentes.OrdemServico().ConsultarOS(autenticacao, filtro);

            watch.Start();
            for (Int32 i = 0; i < iteracoes; i++)
                Agentes.OrdemServico.Instancia.ConsultarOS(autenticacao, filtro);
            watch.Stop();

            Console.WriteLine("Com Interceptor: " + new DateTime().Add(watch.Elapsed).ToString("mm:ss.fff"));

            var watch2 = new Stopwatch();
            watch2.Start();
            for (Int32 i = 0; i < iteracoes; i++)
                new Agentes.OrdemServico().ConsultarOS(autenticacao, filtro);
            watch2.Stop();

            Console.WriteLine("Sem Interceptor: " + new DateTime().Add(watch2.Elapsed).ToString("mm:ss.fff"));
            Console.WriteLine("Diff: " + new DateTime().Add(watch.Elapsed.Subtract(watch2.Elapsed)).ToString("mm:ss.fff"));
            Console.WriteLine("Diff/iteração: " + new DateTime().Add(TimeSpan.FromMilliseconds(watch2.Elapsed.TotalMilliseconds / iteracoes)).ToString("mm:ss.fff"));
        }

        #endregion

        #region [ Verificação de Mapeamento de Valores de Enums ]

        /// <summary>
        /// Teste para verificação dos valores dos Enumeradores mapeados.
        /// Caso algum valor de Enum não exista entre os Enumeradores mapeados, acusa erro.
        /// </summary>
        [TestMethod]
        public void VerificaMapeamentoDeValoresDeEnumeradores()
        {
            //Configura o mapeador
            Mapeador.Configurar();
            StringBuilder validacoes = new StringBuilder();

            //Obtém todos os mapeamentos entre Enums configurados
            var enumMaps = Mapper.GetAllTypeMaps()
                .Where(map => map.SourceType.IsEnum || map.DestinationType.IsEnum).ToArray();                      

            foreach (TypeMap map in enumMaps)
            {
                var valuesSource = Enum.GetValues(map.SourceType).Cast<Enum>().Select(value => value.ToString()).ToArray();
                var valuesDestination = Enum.GetValues(map.DestinationType).Cast<Enum>().Select(value => value.ToString()).ToArray();
                
                Console.WriteLine(map.SourceType.FullName + " ==> " + map.DestinationType.FullName);
                Console.WriteLine();

                //Verifica se todos os valores existentes no Enumerador da Origem existem no Enumerador do Destino
                foreach (var source in valuesSource)
                {
                    if (!valuesDestination.Any(val => String.Compare(val, source) == 0))
                    {
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine(">>>>>>>>>>>-----=====> [NÃO OK] " + source + "<=====-----<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                        Console.WriteLine();
                        Console.WriteLine();
                        validacoes.AppendLine()
                            .Append("Valor " + source + " não encontrado no destino: ")
                            .AppendLine(map.SourceType.FullName + " => " + map.DestinationType.FullName);
                    }
                    else
                        Console.WriteLine("\t [OK] " + source);
                }

                Console.WriteLine();
            }

            Assert.IsTrue(validacoes.Length == 0, validacoes.ToString());
        }

        #endregion

        #region [ Métodos Auxiliares ]

        /// <summary>
        /// Compares the properties of two objects of the same type and returns if all properties are equal.
        /// </summary>
        /// <param name="objectA">The first object to compare.</param>
        /// <param name="objectB">The second object to compre.</param>
        /// <param name="ignoreList">A list of property names to ignore from the comparison.</param>
        /// <returns>true if all property values are equal, otherwise false</returns>
        public static bool AreObjectsEqual(object objectA, object objectB, params string[] ignoreList)
        {
            Boolean result;

            if (objectA != null && objectB != null)
            {
                Type objectType = objectA.GetType();
                Type objectTypeB = objectB.GetType();

                if (objectType != objectTypeB)
                {
                    Console.WriteLine(String.Format("Mismatch property type {0} vs. {1}",
                        objectType.FullName, objectTypeB.FullName));
                    return false;
                }

                result = true; // assume by default they are equal

                foreach (PropertyInfo propertyInfo in objectType.GetProperties(
                    BindingFlags.Public | BindingFlags.Instance).Where(
                    p => p.CanRead && !ignoreList.Contains(p.Name)))
                {
                    object valueA;
                    object valueB;

                    valueA = propertyInfo.GetValue(objectA, null);
                    valueB = propertyInfo.GetValue(objectB, null);

                    // if it is a primative type, value type or implements
                    // IComparable, just directly try and compare the value
                    if (CanDirectlyCompare(propertyInfo.PropertyType))
                    {
                        if (!AreValuesEqual(valueA, valueB))
                        {
                            Console.WriteLine("Mismatch with property '{0}.{1}' found. Values '{2}' != '{3}'.",
                                        objectType.FullName, propertyInfo.Name, valueA, valueB);
                            result = false;
                        }
                    }
                    // if it implements IEnumerable, then scan any items
                    else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        IEnumerable<object> collectionItems1;
                        IEnumerable<object> collectionItems2;
                        int collectionItemsCount1;
                        int collectionItemsCount2;

                        // null check
                        if (valueA == null && valueB != null || valueA != null && valueB == null)
                        {
                            Console.WriteLine("Mismatch with property '{0}.{1}' found.  Values '{2}' != '{3}'.",
                                objectType.FullName, propertyInfo.Name, valueA, valueB);
                            result = false;
                        }
                        else if (valueA != null && valueB != null)
                        {
                            collectionItems1 = ((IEnumerable)valueA).Cast<object>();
                            collectionItems2 = ((IEnumerable)valueB).Cast<object>();
                            collectionItemsCount1 = collectionItems1.Count();
                            collectionItemsCount2 = collectionItems2.Count();

                            // check the counts to ensure they match
                            if (collectionItemsCount1 != collectionItemsCount2)
                            {
                                Console.WriteLine("Collection counts for property '{0}.{1}' do not match.",
                                                    objectType.FullName, propertyInfo.Name);
                                result = false;
                            }
                            // and if they do, compare each item...
                            // this assumes both collections have the same order
                            else
                            {
                                for (int i = 0; i < collectionItemsCount1; i++)
                                {
                                    object collectionItem1;
                                    object collectionItem2;
                                    Type collectionItemType;

                                    collectionItem1 = collectionItems1.ElementAt(i);
                                    collectionItem2 = collectionItems2.ElementAt(i);
                                    collectionItemType = collectionItem1.GetType();

                                    if (CanDirectlyCompare(collectionItemType))
                                    {
                                        if (!AreValuesEqual(collectionItem1, collectionItem2))
                                        {
                                            Console.WriteLine("Item {0} in property collection '{1}.{2}' does not match.",
                                                       i, objectType.FullName, propertyInfo.Name);
                                            result = false;
                                        }
                                    }
                                    else if (!AreObjectsEqual(collectionItem1, collectionItem2, ignoreList))
                                    {
                                        Console.WriteLine("Item {0} in property collection '{1}.{2}' does not match.",
                                                            i, objectType.FullName, propertyInfo.Name);
                                        result = false;
                                    }
                                }
                            }
                        }
                    }
                    else if (propertyInfo.PropertyType == typeof(Object))
                    {
                        result = AreValuesEqual(valueA, valueB);
                    }
                    else if (propertyInfo.PropertyType.IsClass)
                    {
                        if (!AreObjectsEqual(propertyInfo.GetValue(objectA, null),
                                                 propertyInfo.GetValue(objectB, null), ignoreList))
                        {
                            Console.WriteLine("Mismatch with property '{0}.{1}' found.",
                                                    objectType.FullName, propertyInfo.Name);
                            result = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Cannot compare property '{0}.{1}'.",
                                                  objectType.FullName, propertyInfo.Name);
                        result = false;
                    }
                }
            }
            else
                result = object.Equals(objectA, objectB);

            return result;
        }

        /// <summary>
        /// Determines whether value instances of the specified type can be directly compared.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>true if this value instances of the specified type can be directly compared; otherwise, false</returns>
        private static bool CanDirectlyCompare(Type type)
        {
            return typeof(IComparable).IsAssignableFrom(type) || type.IsPrimitive || type.IsValueType;
        }

        /// <summary>
        /// Compares two values and returns if they are the same.
        /// </summary>
        /// <param name="valueA">The first value to compare.</param>
        /// <param name="valueB">The second value to compare.</param>
        /// <returns><c>true</c> if both values match,
        /// otherwise <c>false</c>.</returns>
        private static bool AreValuesEqual(object valueA, object valueB)
        {
            bool result;
            IComparable selfValueComparer;

            selfValueComparer = valueA as IComparable;

            if (valueA == null && valueB != null || valueA != null && valueB == null)
                result = false; // one of the values is null
            else if (selfValueComparer != null && selfValueComparer.CompareTo(valueB) != 0)
                result = false; // the comparison using IComparable failed
            else if (!object.Equals(valueA, valueB))
                result = false; // the comparison using Equals failed
            else
                result = true; // match

            return result;
        }

        #endregion
    }
}