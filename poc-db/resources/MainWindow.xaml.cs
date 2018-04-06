using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Redecard.PN.Cancelamento.Tests.ServicoCancelamento;

namespace Redecard.PN.Cancelamento.Tests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            using (ServicoCancelamento.ServicoPortalCancelamentoClient client = new ServicoCancelamento.ServicoPortalCancelamentoClient()) { 
                client.RetornaDadosEstabelecimentoCancelamento(1457);
            }
        }

        private void CancelarVendaHIS()
        {
            using (ServicoCancelamento.ServicoPortalCancelamentoClient client = new ServicoCancelamento.ServicoPortalCancelamentoClient()) {
                List<ItemCancelamentoEntrada> input = new List<ItemCancelamentoEntrada>();

                input.Add(new ItemCancelamentoEntrada()
                {
                    CodUserCanc = "teste1",
                    DtTransf = new DateTime(2012, 09, 03),
                    DtTransfInt = 03092012,  
                    IPCanc = "172.168.4.86",
                    NSU = "000768874662",
                    NumEstabelecimento = 35343,
                    NumPDVCanc = 35343,
                    VlCanc = decimal.Parse("100"),
                    VlCancStr = "00000000000001000",
                    VlTrans = decimal.Parse("4465,64"),
                    VlTransStr = "00000000000446564",
                    NumCartao = string.Empty,
                    TpVenda = "RO"
                });

                List<ItemCancelamentoSaida> resultado = client.Cancelamento(input);
                this.lblResultadoCancelar.Content = string.Format("{0} - Numero: {1}", resultado[0].MsgErro, resultado[0].NumAvisoCanc);
            }

            
        }

        private void ConsultaDiaCancelamento() {
            using (ServicoCancelamento.ServicoPortalCancelamentoClient client = new ServicoCancelamento.ServicoPortalCancelamentoClient()) {
                List<ModComprovante> anulacao = client.ConsultaAnulacao(35343);

                this.lstAnulacao.ItemsSource = anulacao;
            }
        }

        private void ExecutarAnulacao() {
            using (ServicoCancelamento.ServicoPortalCancelamentoClient client = new ServicoCancelamento.ServicoPortalCancelamentoClient())
            {
                List<ModComprovante> anulacao = client.ConsultaAnulacao(35343);

                List<ModAnularCancelamento> result = client.RealizarAnulacaoCancelamento("teste1", "172.16.4.86", anulacao);

                this.lstAnulacaoResult.ItemsSource = result;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ObjectDataProvider provider = (ObjectDataProvider)this.Resources["provider"];
            provider.ObjectInstance = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CancelarVendaHIS();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ConsultaDiaCancelamento();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ExecutarAnulacao();
        }
    }
}
