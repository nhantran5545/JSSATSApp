using JSSATSAPI.BussinessObjects.IService;
using ZXing.Common;
using ZXing;
using System.Drawing;
using System.Drawing.Imaging;
using IronBarCode;
using JSSATSAPI.DataAccess.IRepository;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class BarcodeService : IBarCodeService
    {
        private readonly IProductRepository _productRepository;

        public BarcodeService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<byte[]> GenerateBarcodeAsync(string productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new ArgumentException("Product ID does not exist.", nameof(productId));
            }

            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = 80,
                    Width = 250,
                    Margin = 10
                }
            };

            var pixelData = writer.Write(productId);
            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
            {
                using (var ms = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                                                     ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                    try
                    {
                        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }

                    bitmap.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        public string DecodeBarcode(Stream barcodeImageStream)
        {
            var result = BarcodeReader.QuicklyReadOneBarcode(barcodeImageStream);
            return result?.Text;
        }
    }

}
