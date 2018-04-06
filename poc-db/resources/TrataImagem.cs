#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [Alexandre Shiroma]
Empresa     : [Iteris]
Histórico   :
- [30/07/2012] – [Alexandre Shiroma] – [Criação]
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using Redecard.PN.Comum;

namespace Redecard.PN.Request.Negocio
{
    /// <summary>
    /// Classe para tratamento de imagens.
    /// </summary>
    internal class TrataImagem
    {
        #region [ Métodos não traduzidos ]

        #region [ Converte Escala Cinza ]
        //    ''' -----------------------------------------------------------------------------
        //    ''' <summary>
        //    '''     Converte a imagem para escala de cinza.
        //    ''' </summary>
        //    ''' <param name="imgOriginal">Objeto do tipo Bitmap com a imagem.</param>
        //    ''' <remarks>
        //    '''     Esta função altera o objeto bitmap passado no parâmetro pois ele é passado por referência.
        //    ''' </remarks>
        //    ''' <example>
        //    ''' 
        //    '''     Dim objImagem As Bitmap
        //    '''     objImagem = objImagem.FromFile("c:\image.jpg")
        //    ''' 
        //    '''     ConverteEscalaCinza(objImagem)
        //    '''     
        //    ''' </example>
        //    ''' <history>
        //    '''     [imorales]  30/5/2006   Created
        //    ''' </history>
        //    ''' -----------------------------------------------------------------------------
        //    Public Shared Sub ConverteEscalaCinza(ByRef imgOriginal As Bitmap)

        //        Dim btmNew As Bitmap

        //        Try
        //            btmNew = New Bitmap(imgOriginal.Width, imgOriginal.Height, PixelFormat.Format16bppRgb565)

        //            btmNew.SetResolution(imgOriginal.HorizontalResolution, imgOriginal.VerticalResolution)

        //            For y As Integer = 0 To btmNew.Height - 1
        //                For x As Integer = 0 To btmNew.Width - 1
        //                    Dim objColor As Color = imgOriginal.GetPixel(x, y)
        //                    Dim intColor As Integer = CInt(objColor.R * 0.3 + objColor.G * 0.59 + objColor.B * 0.11)
        //                    btmNew.SetPixel(x, y, Color.FromArgb(intColor, intColor, intColor))
        //                Next
        //            Next

        //            imgOriginal = btmNew

        //        Catch ex As Exception
        //            Throw ex
        //        End Try

        //    End Sub
        #endregion

        #region [ Converte Preto Branco ]
        //    ''' -----------------------------------------------------------------------------
        //    ''' <summary>
        //    '''     Ajusta a cor da imagem para Preto e Branco.
        //    ''' </summary>
        //    ''' <param name="imgOriginal">Objeto do tipo Bitmap com a imagem.</param>
        //    ''' <remarks>
        //    '''     Esta função altera o objeto bitmap passado no parâmetro pois ele é passado por referência.
        //    ''' </remarks>
        //    ''' <example>
        //    ''' 
        //    '''     Dim objImagem As Bitmap
        //    '''     objImagem = objImagem.FromFile("c:\image.jpg")
        //    ''' 
        //    '''     ConvertePretoBranco(objImagem)
        //    '''     
        //    ''' </example>
        //    ''' <history>
        //    '''     [imorales]  30/5/2006   Created
        //    ''' </history>
        //    ''' -----------------------------------------------------------------------------
        //    Public Shared Sub ConvertePretoBranco(ByRef imgOriginal As Bitmap)

        //        Dim intY As Integer
        //        Dim intX As Integer

        //        Try

        //            Dim btmBitmap As New Bitmap(imgOriginal.Width, imgOriginal.Height, PixelFormat.Format1bppIndexed)
        //            Dim btdBitmap As BitmapData = btmBitmap.LockBits(New Rectangle(0, 0, btmBitmap.Width, btmBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed)

        //            btmBitmap.SetResolution(imgOriginal.HorizontalResolution, imgOriginal.VerticalResolution)

        //            For intX = 0 To imgOriginal.Width - 1
        //                For intY = 0 To imgOriginal.Height - 1
        //                    If imgOriginal.GetPixel(intX, intY).GetBrightness() > 0.5F Then
        //                        SetIndexedPixel(intX, intY, btdBitmap, True)
        //                    End If
        //                Next
        //            Next

        //            btmBitmap.UnlockBits(btdBitmap)
        //            imgOriginal = btmBitmap

        //        Catch ex As Exception
        //            Throw ex
        //        End Try

        //    End Sub
        #endregion

        #region [ SetIndexPixel ]
        //    ''' -----------------------------------------------------------------------------
        //    ''' <summary>
        //    '''     Seta o pixel do objeto BitmapData.
        //    ''' </summary>
        //    ''' <param name="intX">Posição do pixel no eixo x.</param>
        //    ''' <param name="intY">Posição do pixel no eixo y.</param>
        //    ''' <param name="bmdBitmap">Objeto do tipo bitmap.</param>
        //    ''' <param name="blnPixel">Variável boleana para printar o novo pixel.</param>
        //    ''' <remarks>
        //    ''' </remarks>
        //    ''' <example>
        //    ''' 
        //    ''' Dim btmBitmap As New Bitmap(imgOriginal.Width, imgOriginal.Height, PixelFormat.Format1bppIndexed)
        //    ''' Dim btdBitmap As BitmapData = btmBitmap.LockBits(New Rectangle(0, 0, btmBitmap.Width, btmBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed)
        //    ''' 
        //    ''' For intX = 0 To imgOriginal.Width - 1
        //    '''     For intY = 0 To imgOriginal.Height - 1
        //    '''         If imgOriginal.GetPixel(intX, intY).GetBrightness() > 0.5F Then
        //    '''             SetIndexedPixel(intX, intY, btdBitmap, True)
        //    '''         End If
        //    '''     Next
        //    ''' Next
        //    ''' 
        //    ''' </example>
        //    ''' <history>
        //    '''     [imorales]  1/6/2006    Created
        //    ''' </history>
        //    ''' -----------------------------------------------------------------------------
        //    Private Shared Sub SetIndexedPixel(ByVal intX As Integer, _
        //                                       ByVal intY As Integer, _
        //                                       ByVal bmdBitmap As BitmapData, _
        //                                       ByVal blnPixel As Boolean)

        //        Dim intIndex As Integer
        //        Dim bytP As Byte
        //        Dim bytMask As Byte

        //        Try

        //            intIndex = intY * bmdBitmap.Stride + (intX >> 3)
        //            bytP = Marshal.ReadByte(bmdBitmap.Scan0, intIndex)
        //            bytMask = &H80 >> (intX And &H7)

        //            If blnPixel Then
        //                bytP = bytP Or bytMask
        //            Else
        //                bytP = bytP And CByte(bytMask ^ &HFF)
        //            End If

        //            Marshal.WriteByte(bmdBitmap.Scan0, intIndex, bytP)

        //        Catch ex As Exception
        //            Throw
        //        End Try

        //    End Sub
        #endregion

        #region [ Consolidar Imagem ]
        //    ''' -----------------------------------------------------------------------------
        //    ''' <summary>
        //    '''     Consolida duas imagens.
        //    ''' </summary>
        //    ''' <param name="imgOriginal">Imagem do tipo Bitmap.</param>
        //    ''' <param name="btmNova">Imagem do tipo Bitmap.</param>
        //    ''' <returns>Retorna a imagem bitmap.</returns>
        //    ''' <remarks>
        //    ''' </remarks>
        //    ''' <example>
        //    ''' 
        //    '''     Dim imgOriginal As Bitmap
        //    '''     Dim btpNovaImagem As Bitmap
        //    '''     Dim btpImagemConsolidada As Bitmap
        //    '''
        //    '''     imgOriginal = imgOriginal.FromFile("c:\image1.jpg")
        //    '''     btpNovaImagem = btpNew.FromFile("c:\image2.jpg")
        //    '''
        //    '''     btpImagemConsolidada = ConsolidarImagem(imgOriginal, btpNovaImagem)
        //    ''' 
        //    ''' </example>
        //    ''' <history>
        //    ''' 	[imorales]	1/6/2006	Created
        //    ''' </history>
        //    ''' -----------------------------------------------------------------------------
        //    Public Shared Function ConsolidarImagem(ByVal imgOriginal As Bitmap, _
        //                                            ByVal btmNova As Bitmap) As Bitmap

        //        Dim btmConsolidada As Bitmap
        //        Dim intX As Integer
        //        Dim intY As Integer

        //        Try

        //            'Verifica qual imagem tem a maior largura.
        //            If imgOriginal.Width > btmNova.Width Then
        //                btmConsolidada = New Bitmap(imgOriginal.Width, imgOriginal.Height + btmNova.Height)
        //            Else
        //                btmConsolidada = New Bitmap(btmNova.Width, imgOriginal.Height + btmNova.Height)
        //            End If

        //            'Copia a imagem original.
        //            For intY = 0 To imgOriginal.Height - 1
        //                For intX = 0 To imgOriginal.Width - 1
        //                    btmConsolidada.SetPixel(intX, intY, imgOriginal.GetPixel(intX, intY))
        //                Next
        //            Next

        //            'Copia a nova imagem.
        //            For intY = imgOriginal.Height To btmConsolidada.Height - 1
        //                For intX = 0 To btmNova.Width - 1
        //                    btmConsolidada.SetPixel(intX, intY, btmNova.GetPixel(intX, intY - imgOriginal.Height))
        //                Next
        //            Next

        //            Return btmConsolidada

        //        Catch ex As Exception
        //            Throw ex
        //        End Try

        //    End Function
        #endregion

        #region [ ConsolidarImagens ]

        //    ''' -----------------------------------------------------------------------------
        //    ''' <summary>
        //    '''     Consolida uma lista de imagens.
        //    ''' </summary>
        //    ''' <param name="strPath">Pasta onde estão as imagens.</param>
        //    ''' <param name="strListaImagens">Lista com o nome das imagens.</param>
        //    ''' <param name="strImagemTif">Nome que a imagem será salva.</param>
        //    ''' <remarks>
        //    ''' </remarks>
        //    ''' <example>
        //    ''' 
        //    '''     ConsolidarImagens("c:\", "img1.JPG,img2.JPG,img3.JPG,img4.JPG", "Imagem.tif")
        //    ''' 
        //    ''' </example>
        //    ''' <history>
        //    ''' 	[imorales]	14/6/2006	Created
        //    ''' </history>
        //    ''' -----------------------------------------------------------------------------
        //    Public Shared Function ConsolidarImagens(ByVal strPath As String, _
        //                                        ByVal strListaImagens As String, _
        //                                        ByVal strImagemTif As String) As Boolean

        //        Dim arrListaImagem As Array
        //        Dim intCont As Integer
        //        Dim btmConsolidada As Bitmap
        //        Dim btmTemporaria As Bitmap
        //        Dim objEncParam As System.Drawing.Imaging.EncoderParameter
        //        Dim objEncParams As System.Drawing.Imaging.EncoderParameters
        //        Dim objCodec As System.Drawing.Imaging.ImageCodecInfo

        //        Try

        //            objEncParams = New System.Drawing.Imaging.EncoderParameters(1)
        //            objEncParam = New System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Compression, System.Drawing.Imaging.EncoderValue.CompressionCCITT4)
        //            objEncParams.Param(0) = objEncParam

        //            objCodec = GetCodecInfo("image/tiff")

        //            'Cria um array de nome de arquivos.
        //            arrListaImagem = strListaImagens.Replace("/", "\").Split(",")

        //            If arrListaImagem.Length > 1 Then
        //                For intCont = 0 To arrListaImagem.Length - 1
        //                    If intCont = 0 Then
        //                        btmConsolidada = btmConsolidada.FromFile(strPath + arrListaImagem(intCont))
        //                        btmTemporaria = btmTemporaria.FromFile(strPath + arrListaImagem(intCont + 1))
        //                        btmConsolidada = ConsolidarImagem(btmConsolidada, btmTemporaria)
        //                    Else
        //                        If intCont = arrListaImagem.Length - 1 Then Exit For
        //                        btmTemporaria = btmTemporaria.FromFile(strPath + arrListaImagem(intCont + 1))
        //                        btmConsolidada = ConsolidarImagem(btmConsolidada, btmTemporaria)
        //                    End If
        //                Next

        //                'Salva o arquivo.

        //                Dim objTiffBitmap As Bitmap = ConvertTo1bpp(btmConsolidada)
        //                objTiffBitmap.Save(strPath + strImagemTif, objCodec, objEncParams)

        //            Else
        //                If arrListaImagem.Length = 1 Then
        //                    btmConsolidada = btmConsolidada.FromFile(strPath + arrListaImagem(0))
        //                    btmConsolidada.Save(strPath + strImagemTif, objCodec, objEncParams)
        //                Else
        //                    Return False
        //                End If
        //            End If

        //            Return True

        //        Catch ex As Exception
        //            Throw ex
        //        End Try

        //    End Function
        #endregion

        #region [ ConvertTo1bpp ]

        //    ''' -----------------------------------------------------------------------------
        //    ''' <summary>
        //    ''' Uma imagem bitmap contem mais de um bit por pixel, porem a compressao especificada
        //    ''' necessita de apenas um bit por pixel, esta funcao converte essa caracteristica
        //    ''' </summary>
        //    ''' <param name="objOriginBitmap"></param>
        //    ''' <returns></returns>
        //    ''' <remarks>
        //    ''' </remarks>
        //    ''' <history>
        //    '''     [cveber]    30/6/2006   Created
        //    ''' </history>
        //    ''' -----------------------------------------------------------------------------
        //    Private Shared Function ConvertTo1bpp(ByVal objOriginBitmap As Bitmap) As Bitmap

        //        Dim objMapOrigem As BitmapData
        //        Dim objReturn As Bitmap
        //        Dim objMapReturn As BitmapData

        //        Try

        //            '------<Trava os bits do bitmap original>--------------------------------
        //            objMapOrigem = objOriginBitmap.LockBits(New Rectangle(0, 0, objOriginBitmap.Width, objOriginBitmap.Height), ImageLockMode.ReadOnly, objOriginBitmap.PixelFormat)

        //            '------<Cria a nova imagem com 1 bit por pixel>--------------------------------
        //            objReturn = New Bitmap(objOriginBitmap.Width, objOriginBitmap.Height, PixelFormat.Format1bppIndexed)
        //            objMapReturn = objReturn.LockBits(New Rectangle(0, 0, objReturn.Width, objReturn.Height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed)

        //            For y As Integer = 0 To objOriginBitmap.Height - 1
        //                For x As Integer = 0 To objOriginBitmap.Width - 1

        //                    '------<Obtem o endereço do pixel colorido>--------------------------------
        //                    Dim index As Integer = y * objMapOrigem.Stride + x * 4

        //                    '------<Verifica a luminosidade>--------------------------------
        //                    If Color.FromArgb(Marshal.ReadByte(objMapOrigem.Scan0, index + 2), Marshal.ReadByte(objMapOrigem.Scan0, index + 1), Marshal.ReadByte(objMapOrigem.Scan0, index)).GetBrightness() > 0.5F Then

        //                        '------<Define o pixel na imagem de retorno>--------------------------------
        //                        SetIndexedPixel(x, y, objMapReturn, True)

        //                    End If
        //                Next
        //            Next

        //            '------<Libera os bits da imagem>--------------------------------
        //            objReturn.UnlockBits(objMapReturn)
        //            objOriginBitmap.UnlockBits(objMapOrigem)

        //            '------<Retorna a imagem pronta para compressao CompressionCCITT4>--------------------------------
        //            Return objReturn

        //        Catch ex As Exception
        //            Throw ex
        //        Finally
        //            objMapOrigem = Nothing
        //            objReturn = Nothing
        //            objMapReturn = Nothing
        //        End Try

        //    End Function

        #endregion

        #endregion

        #region [ Métodos Privados ]

        #region [ Carregar imagem física ]

        /// <summary>
        /// Método auxiliar para carregar arquivos de imagens em memória, liberando
        /// o recurso físico em disco.
        /// </summary>
        /// <param name="path">Caminho do arquivo</param>
        /// <returns>Bitmap contendo a imagem carregada</returns>
        private static Bitmap CarregarBitmap(string path)
        {

            //Open file in read only mode
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            //Get a binary reader for the file stream
            using (BinaryReader reader = new BinaryReader(stream))
            {
                //copy the content of the file into a memory stream
                var memoryStream = new MemoryStream(reader.ReadBytes((int)stream.Length));
                //make a new Bitmap object the owner of the MemoryStream
                return new Bitmap(memoryStream);
            }


        }

        #endregion

        #region [ Ajustar Resolução ]
        /// <summary>
        /// Ajusta a resolução da imagem de acordo com a altura e largura máxima 
        /// passadas como parâmetro. Mantendo as proporções da imagem.
        /// </summary>
        /// <param name="imgOriginal">Objeto do tipo Bitmap com a imagem.</param>
        /// <param name="intMaxHeight">Altura máxima que a imagem pode ter.</param>
        /// <param name="intMaxWidth">Largura máxima que a imagem pode ter.</param>
        /// <remarks>Esta função altera o objeto bitmap passado no parâmetro pois ele é passado por referência.</remarks>
        /// <example>
        /// Bitmao objImagem = Bitmap.FromFile("c:\image.jpg");
        /// AjustarResolucao(objImagem, 300, 200);
        /// </example>
        private static void AjustarResolucao(ref Bitmap imgOriginal, Int32 intMaxHeight, Int32 intMaxWidth)
        {
            Single intHeight;
            Single intWidth;

            Image.GetThumbnailImageAbort callback = new Image.GetThumbnailImageAbort(() => { return false; });

            if (imgOriginal.Width > intMaxWidth)
            {
                intWidth = intMaxWidth;
                intHeight = Convert.ToInt32(imgOriginal.Height * intMaxWidth / imgOriginal.Width); ;

                if (intHeight > intMaxHeight)
                {
                    intHeight = intMaxHeight;
                    intWidth = Convert.ToInt32(intWidth * intMaxHeight / intHeight);
                }
                imgOriginal = (Bitmap)imgOriginal.GetThumbnailImage((Int32)intWidth, (Int32)intHeight, callback, IntPtr.Zero);
            }
            else
            {
                if (imgOriginal.Height > intMaxHeight)
                {
                    intHeight = intMaxHeight;
                    intWidth = Convert.ToInt32(imgOriginal.Width * intMaxHeight / imgOriginal.Height);
                    if (intWidth > intMaxWidth)
                    {
                        intWidth = intMaxWidth;
                        intHeight = Convert.ToInt32(intHeight * intMaxWidth / intWidth);
                    }
                    imgOriginal = (Bitmap)imgOriginal.GetThumbnailImage((Int32)intWidth, (Int32)intHeight, callback, IntPtr.Zero);
                }
            }
        }
        #endregion

        #region [ Ajustar Dpi ]
        /// <summary>
        /// Ajusta a imagem caso ela tenha um dpi maior que o passado no parâmetro.
        /// </summary>
        /// <param name="imgOriginal">Objeto do tipo Bitmap com a imagem.</param>
        /// <param name="intMaxDpi">DPI máximo permitido para a imagem.</param>
        /// <remarks>
        /// Esta função altera o objeto bitmap passado no parâmetro pois ele é passado por referência.
        /// Para que as proporções da imagem se mantenham é necessário diminuir a resolução proporcionalmente a redução do Dpi.
        /// </remarks>
        /// <example>
        /// Bitmap objImagem = Bitmap.FromFile("c:\image.jpg");
        /// AjustarDpi(objImagem, 250);
        /// </example>
        private static void AjustaDpi(ref Bitmap imgOriginal, Single intMaxDpi)
        {
            if (imgOriginal.HorizontalResolution > intMaxDpi)
            {
                Single intHeight = (imgOriginal.Height * intMaxDpi) / imgOriginal.HorizontalResolution;
                Single intWidth = (imgOriginal.Width * intMaxDpi) / imgOriginal.HorizontalResolution;

                imgOriginal.SetResolution(intMaxDpi, intMaxDpi);
                AjustarResolucao(ref imgOriginal, (Int32)intHeight, (Int32)intWidth);
            }
        }
        #endregion

        #region [ Rotacionar Imagem ]
        /// <summary>
        /// Rotaciona a imagem caso a largura seja maior que a altura.
        /// </summary>
        /// <remarks>Esta função altera o objeto bitmap passado no parâmetro pois ele é passado por referência.</remarks>
        /// <param name="imgOriginal">>Recebe o objeto Imagem</param>
        /// <example>
        ///  Bitmap objImagem = Bitmap.FromFile("c:\image.jpg");
        ///  RotacionarImagem(ref objImagem);
        /// </example>
        private static void RotacionarImagem(ref Bitmap imgOriginal)
        {
            //Se a largura é maior que a altura então rotaciona a imagem.
            if (imgOriginal.Width > imgOriginal.Height)
                imgOriginal.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }

        #endregion

        #region [ GetCodecInfo ]

        /// <summary>
        /// Obtem as configurações de um codec
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        private static ImageCodecInfo GetCodecInfo(String mimeType)
        {
            foreach (ImageCodecInfo encoder in ImageCodecInfo.GetImageEncoders())
                if (encoder.MimeType.Equals(mimeType))
                    return encoder;
            return null;
        }

        #endregion

        #region [ Consolida Imagens em Páginas Arrays ]
        /// <summary>
        /// Consolida uma lista de imagens em páginas.
        /// </summary>
        /// <param name="path">Pasta onde estão as imagens.</param>
        /// <param name="imagemTif">Nome que a imagem será salva.</param>
        /// <param name="btmImagens">Lista com o nome das imagens.</param>
        /// <returns></returns>
        /// <example>
        /// Bitmap[] btmImagens = new Bitmap[2];
        /// btmImagens[0] = Bitmap.FromFile("c:\img1.jpg");
        /// btmImagens[1] = Bitmap.FromFile("c:\img2.jpg");
        /// ConsolidaImagensPaginas("c:\", btmImagens, "Imagem.tiff");
        /// </example>
        private static Boolean ConsolidarImagensPaginasArray(String path, String imagemTif, Bitmap[] btmImagens)
        {
            Encoder enc = Encoder.SaveFlag;

            //Pega informações sobre o codec.
            ImageCodecInfo info = GetCodecInfo("image/tiff");

            //Cria um encoder parameters
            EncoderParameters ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(enc, (Int64)EncoderValue.MultiFrame);

            Bitmap btmPages = null;

            for (Int32 intCont = 0; intCont < btmImagens.Length; intCont++)
            {
                if (intCont == 0)
                {
                    btmPages = btmImagens[intCont];
                    btmPages.Save(path + imagemTif, info, ep);
                }
                else
                {
                    ep.Param[0] = new EncoderParameter(enc, (Int64)EncoderValue.FrameDimensionPage);
                    Bitmap bm = btmImagens[intCont];
                    btmPages.SaveAdd(bm, ep);
                }
                if (intCont == btmImagens.Length - 1)
                {
                    ep.Param[0] = new EncoderParameter(enc, (Int64)EncoderValue.Flush);
                    btmPages.SaveAdd(ep);
                }

            }
            btmPages.Dispose();
            return true;
        }
        #endregion

        #region [ LerPaginasTif ]
        /// <summary>
        /// Função que lê um arquivo .Tif com multiplas páginas e devolve um array de bitmaps.
        /// </summary>
        /// <param name="path">Pasta do arquivo.</param>
        /// <param name="arquivo">Nome do arquivo.</param>
        /// <example>LerPaginasTif("c:\", "Imagem.tif")</example>
        /// <returns>Coleção de Bitmaps</returns>
        private static IEnumerable<Bitmap> LerPaginasTif(String path, String arquivo)
        {
            //Abre o arquivo tif de multipla página.
            Bitmap btmTemp = (Bitmap)CarregarBitmap(path + arquivo);

            //Percorre o tif alimentando o array de bitmap.
            for (Int32 intPageCount = 0; intPageCount < btmTemp.GetFrameCount(FrameDimension.Page); intPageCount++)
            {
                btmTemp.SelectActiveFrame(FrameDimension.Page, intPageCount);
                yield return (Bitmap)btmTemp.Clone();
            }
        }
        #endregion

        #endregion

        #region [ Consolida Imagens em Páginas ]

        /// <summary>
        /// Consolida uma lista de imagens em páginas.
        /// </summary>
        /// <param name="path">Pasta onde estão as imagens.</param>
        /// <param name="listaImagens">Lista com o nome das imagens.</param>
        /// <param name="imagemTif">Nome que a imagem será salva.</param>
        /// <example>ConsolidaImagensPaginas("c:\", new String[] {"img1.JPG", "img2.JPG", "img3.JPG", "img4.JPG" }, "Imagem.tif")</example>
        /// <returns></returns>
        public static Boolean ConsolidarImagensPaginas(String path, String[] listaImagens, String imagemTif)
        {
            if (listaImagens == null || listaImagens.Length == 0) return false;

            try
            {
                Image tiff = null;
                EncoderParameters encoderParams = null;
                imagemTif = Path.Combine(path, imagemTif);

                for (Int32 iImagem = 0; iImagem < listaImagens.Length; iImagem++)
                {
                    String imagem = Path.Combine(path, listaImagens[iImagem]);
                    if (File.Exists(imagem))
                    {
                        //Se for a primeira imagem
                        if (iImagem == 0)
                        {
                            //Inicia com o primeiro Bitmap, colocando-o em um objeto Image
                            Bitmap bitmap = (Bitmap)Image.FromFile(imagem);

                            //Salva o bitmap em memória como TIFF
                            MemoryStream byteStream = new MemoryStream();
                            bitmap.Save(byteStream, ImageFormat.Tiff);

                            //Coloca o TIFF em outro objeto imagem
                            tiff = Image.FromStream(byteStream);

                            //Prepara os encoders
                            ImageCodecInfo encoderInfo = GetCodecInfo("image/tiff");
                            encoderParams = new EncoderParameters(2);
                            encoderParams.Param[0] = new EncoderParameter(Encoder.Compression, (Int64)EncoderValue.CompressionLZW);
                            encoderParams.Param[1] = new EncoderParameter(Encoder.SaveFlag, (Int64)EncoderValue.MultiFrame);

                            //Salva o arquivo
                            tiff.Save(imagemTif, encoderInfo, encoderParams);

                            bitmap.Dispose();
                        }
                        else
                        {
                            Bitmap bitmap = (Bitmap)Image.FromFile(imagem);

                            //Salva o bitmap em memória como TIFF
                            MemoryStream byteStream = new MemoryStream();
                            bitmap.Save(byteStream, ImageFormat.Tiff);

                            //Coloca o TIFF em outro objeto imagem
                            Image tiffPage = Image.FromStream(byteStream);

                            //Para as próximas páginas, prepara os encoders
                            encoderParams = new EncoderParameters(2);
                            encoderParams.Param[0] = new EncoderParameter(Encoder.Compression, (Int64)EncoderValue.CompressionLZW);
                            encoderParams.Param[1] = new EncoderParameter(Encoder.SaveFlag, (Int64)EncoderValue.FrameDimensionPage);
                            tiff.SaveAdd(tiffPage, encoderParams);

                            bitmap.Dispose();
                        }
                    }
                }

                //Finaliza a gravação da imagem
                encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.Flush);
                tiff.SaveAdd(encoderParams);
                tiff.Dispose();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [Obsolete]
        private static Boolean ConsolidarImagensPaginas_OLD(String path, String[] listaImagens, String imagemTif)
        {
            //Pega informações sobre o codec
            ImageCodecInfo info = GetCodecInfo("image/tiff");

            //Parametros do Imaging
            EncoderParameters ep = new EncoderParameters(2);
            ep.Param[0] = new EncoderParameter(Encoder.SaveFlag, (Int64)EncoderValue.MultiFrame);
            ep.Param[1] = new EncoderParameter(Encoder.Compression, (Int64)EncoderValue.CompressionLZW);

            //Variáveis auxiliares para o loop            
            Bitmap btmTemp = null;
            String extension = String.Empty;
            Bitmap[] arrBitmap = new Bitmap[1];
            Int32 intFrame = 0;
            Bitmap btmPages = null;

            try
            {
                foreach (String fileName in listaImagens)
                {
                    //Abre imagem
                    if (File.Exists(Path.Combine(path, fileName)))
                    {
                        btmTemp = CarregarBitmap(Path.Combine(path, fileName));
                        extension = Path.GetExtension(fileName).ToLower();

                        if (extension == ".tiff" || extension == ".tif")
                        {
                            //Carrega várias páginas do TIFF
                            arrBitmap = new Bitmap[btmTemp.GetFrameCount(FrameDimension.Page)];
                            for (int intPageCount = 0; intPageCount < arrBitmap.Length; intPageCount++)
                            {
                                btmTemp.SelectActiveFrame(FrameDimension.Page, intPageCount);
                                arrBitmap[intPageCount] = (Bitmap)btmTemp.Clone();
                            }
                        }
                        else
                        {
                            //Carrega imagem dentro do array
                            arrBitmap = new Bitmap[1];
                            arrBitmap[0] = btmTemp;
                        }

                        for (Int32 intCont = 0; intCont < arrBitmap.Length; intCont++)
                        {
                            if (intFrame == 0)
                            {
                                btmPages = arrBitmap[intCont];
                                btmPages.Save(path + imagemTif, info, ep);
                            }
                            else
                            {
                                ep.Param[0] = new EncoderParameter(Encoder.SaveFlag, (Int64)EncoderValue.FrameDimensionPage);
                                btmPages.SaveAdd((Bitmap)arrBitmap[intCont].Clone(), ep);
                                btmTemp.Dispose();
                                GC.Collect();
                            }
                            intFrame++;
                        }
                    }

                    ep.Param[0] = new EncoderParameter(Encoder.SaveFlag, (Int64)EncoderValue.Flush);

                    if (btmPages != null)
                        btmPages.SaveAdd(ep);
                }
            }
            finally
            {
                if (btmPages != null)
                    btmPages.Dispose();
                if (btmTemp != null)
                    btmTemp.Dispose();
            }
            return true;
        }
        #endregion

        #region [ Converte Imagem ]
        /// <summary>
        /// Altera as caracteristicas da imagem para armazenamento no banco de dados.
        /// </summary>
        public static String ConverteImagem(String fileName, String pathLoad, Int32 maxHeight, Int32 maxWidth, Int32 maxDpi)
        {
            Bitmap[] arrBitmap;
            String pathSave = pathLoad;
            String fileNameSave = String.Empty;

            //Caso a extensao do arquivo original seja TIFF, altera o nome para não ser o mesmo do arquivo final
            fileNameSave = Path.GetFileNameWithoutExtension(fileName) + ".tiff";

            if (Path.GetExtension(fileName) == ".tiff")
            {
                String prefixo = "o_";
                while (File.Exists(Path.Combine(pathLoad, prefixo + fileName)))
                {
                    File.Delete(Path.Combine(pathLoad, prefixo + fileName));
                    prefixo = prefixo + "o_";
                }
                File.Move(Path.Combine(pathLoad + fileName), Path.Combine(pathLoad, prefixo + fileName));
                fileName = prefixo + fileName;
            }

            //Abre o arquivo em múltiplas páginas (se houver)
            if (Path.GetExtension(fileName).ToLower() == ".tiff" ||
                Path.GetExtension(fileName).ToLower() == ".tif")
            {
                arrBitmap = LerPaginasTif(pathLoad, fileName).ToArray();
            }
            else
            {
                arrBitmap = new Bitmap[1];
                arrBitmap[0] = (Bitmap)CarregarBitmap(Path.Combine(pathLoad, fileName));
            }

            for (Int32 intCont = 0; intCont < arrBitmap.Length; intCont++)
            {
                Bitmap imgOriginal = arrBitmap[intCont];

                //Rotaciona a imagem
                RotacionarImagem(ref imgOriginal);

                //Ajusta DPI
                AjustaDpi(ref imgOriginal, maxDpi);

                //Ajusta Resolução
                AjustarResolucao(ref imgOriginal, maxHeight, maxWidth);

                //Define Preto e branco
                //ConvertePretoBranco(imgOriginal)

                arrBitmap[intCont] = imgOriginal;
            }

            //Consolida as várias páginas do TIFF (se houver)
            ConsolidarImagensPaginasArray(pathSave, fileNameSave, arrBitmap);

            File.Delete(Path.Combine(pathLoad, fileName));

            return fileNameSave;
        }
        #endregion
    }
}
