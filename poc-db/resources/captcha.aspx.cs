using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Timers;
using Redecard.PN.Comum;

namespace Redecard.PN.Boston.Sharepoint.Layouts.Redecard.PN.Boston.Sharepoint
{
    public partial class captcha : ApplicationPageBaseAnonima
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Gera o código quando a página é carregada

            Int32 QuantCh = 5;   //Número de caracteres no código. Pode alterar.
            Int32 i; //Controle do Loop        
            String Codigo = "";    //Variável que irá conter o código gerado
            //Randomize(Timer);             //Randomiza a execução

            Random rndPalavra = new Random();

            for (i = 1; i <= QuantCh; i++)
            {
                Codigo = Codigo + ((char)(rndPalavra.Next(65, 88)));   //Gera um caracter entre A-Z e adiciona à variável
            }


            Session.Add("cookie", Codigo);
            Application.Add("cookie", Codigo);
            //Grava o código em um cookie (com uma criptografia bem simples, o HashCode)
            Response.Cookies["codseg"]["hash"] = Codigo.GetHashCode().ToString();

            //O cookie expira em 5 minutos. Se quiser pode alterar.
            //Response.Cookies("codseg").Expires = Now().AddMinutes(5)
            //Agora que o código aleatório está definida, 
            //chama a função que irá criar a imagem.
            GerarImg(Codigo);
        }
        //Função para gerar uma imagem à partir de um texto    
        private void GerarImg(String texto)
        {
            string[] fontesPossiveis = { "Arial", "Tahoma", "Verdana", "Times New Roman" };
            string fonte = fontesPossiveis[new Random().Next(0, fontesPossiveis.Length - 1)];

            //Sorteia um tamanho entre 25 e 35
            int tamanho = new Random().Next(35, 40);

            //Sorteia um estilo (Normal, Negrito, Italico)
            FontStyle Estilo = new FontStyle();


            //Sorteia um fundo para a imagem. Os fundos devem estar na pasta //fundo//
            string[] arquivosFundo = System.IO.Directory.GetFiles(Server.MapPath("~/_layouts/images/fundoCaptcha/"));
            Random rndArquivo = new Random();

            string fundo = arquivosFundo[rndArquivo.Next(0, arquivosFundo.Length - 1)];

            //Define um objeto de fonte, com os dados sorteados
            Font oFonte = new Font(fonte, tamanho, Estilo, GraphicsUnit.Point, 0);

            //Cria uma imagem com base no fundo sorteado
            Image imgInicial = Image.FromFile(fundo);

            //Cria um objeto gráfico que irá conter a imagem final
            Graphics graf = Graphics.FromImage(imgInicial);

            //Define a qualidade da imagem criada. Usei qualidade baixa para aumentar a velocidade
            graf.InterpolationMode = InterpolationMode.Low;
            graf.SmoothingMode = SmoothingMode.HighSpeed;

            Random rndRotate = new Random();
            //E, para piorar, rotaciona a imagem levemente
            graf.RotateTransform(rndRotate.Next(-5, 5));

            //Escreve o texto na imagem, em uma posição sorteada
            graf.DrawString(texto, oFonte, Brushes.Black, rndRotate.Next(10, 20), rndRotate.Next(5, 15));

            //Retorna a imagem diretamente ao cliente
            Response.ContentType = "image/jpeg";
            imgInicial.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);

        }
    }
}
