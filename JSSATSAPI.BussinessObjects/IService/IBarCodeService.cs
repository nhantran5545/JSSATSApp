using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.IService
{
    public interface IBarCodeService
    {
        string DecodeBarcode(Stream barcodeImageStream);
        byte[] GenerateBarcode(string productId);
    }
}
