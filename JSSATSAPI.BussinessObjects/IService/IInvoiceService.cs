﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.IService
{
    public interface IInvoiceService
    {
        Task<byte[]> ExportSellInvoiceAsync(int orderSellId);
        Task<string> GetSellInvoiceHtmlAsync(int orderSellId);
    }
}