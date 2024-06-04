using AutoMapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.ResponseModels;
using JSSATSAPI.DataAccess.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IOrderSellRepository _orderSellRepository;
        private readonly IMapper _mapper;

        public InvoiceService(IOrderSellRepository orderSellRepository, IMapper mapper)
        {
            _orderSellRepository = orderSellRepository;
            _mapper = mapper;
        }

        public async Task<byte[]> ExportSellInvoiceAsync(int orderSellId)
        {
            var orderSell = await _orderSellRepository.GetOrderSellWithDetailsAsync(orderSellId);
            if (orderSell == null)
            {
                throw new Exception("Order not found");
            }

            var sellInvoiceDto = _mapper.Map<SellInvoiceDTO>(orderSell);

            using (var ms = new MemoryStream())
            {
                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 10, 10, 10, 10);
                var writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                var html = new StringBuilder();
                html.Append("<html><head><title>Invoice</title>");
                html.Append("<style>");
                html.Append("body { font-family: Arial, sans-serif; font-size: 12px; }");
                html.Append(".invoice-box { width: 100%; padding: 10px; border: 1px solid #eee; }");
                html.Append(".invoice-box table { width: 100%; border-collapse: collapse; }");
                html.Append(".invoice-box table td { padding: 5px; vertical-align: top; }");
                html.Append(".invoice-box table tr td:nth-child(2) { text-align: right; }");
                html.Append(".invoice-box table tr.top table td { padding-bottom: 20px; }");
                html.Append(".invoice-box table tr.information table td { padding-bottom: 40px; }");
                html.Append(".invoice-box table tr.heading td { background: #eee; border-bottom: 1px solid #ddd; font-weight: bold; text-align: left; }");
                html.Append(".invoice-box table tr.details td { padding-bottom: 20px; }");
                html.Append(".invoice-box table tr.item td { border-bottom: 1px solid #eee; text-align: left; }");
                html.Append(".invoice-box table tr.item.last td { border-bottom: none; }");
                html.Append(".invoice-box table tr.total td:nth-child(2) { border-top: 2px solid #eee; font-weight: bold; }");
                html.Append("</style>");
                html.Append("</head><body>");
                html.Append("<div class='invoice-box'>");
                html.Append("<table cellpadding='0' cellspacing='0'>");
                html.Append("<tr class='top'>");
                html.Append("<td colspan='2'>");
                html.Append("<table>");
                html.Append("<tr>");
                html.Append("<td class='title'><img src='path/to/logo.png' style='width:100%; max-width:100px;' /></td>");
                html.Append("<td>");
                html.Append($"Invoice #: {sellInvoiceDto.OrderSellId}<br />");
                html.Append($"Order Date: {sellInvoiceDto.OrderDate:dd/MM/yyyy}<br />");
                html.Append($"Status: {sellInvoiceDto.Status}");
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("</table>");
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr class='information'>");
                html.Append("<td colspan='2'>");
                html.Append("<table>");
                html.Append("<tr>");
                html.Append("<td>");
                html.Append($"Customer Name: {sellInvoiceDto.CustomerName}<br />");
                html.Append("</td>");
                html.Append("<td>");
                html.Append($"Seller: {sellInvoiceDto.SellerFirstName} {sellInvoiceDto.SellerLastName}<br />");
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("</table>");
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr class='heading'>");
                html.Append("<td>Item</td>");
                html.Append("<td>Price</td>");
                html.Append("</tr>");

                foreach (var detail in sellInvoiceDto.OrderSellDetails)
                {
                    html.Append("<tr class='item'>");
                    html.Append($"<td>{detail.ProductName} (x{detail.Quantity})</td>");
                    html.Append($"<td>{detail.Price:C}</td>");
                    html.Append("</tr>");
                }

                html.Append("<tr class='total'>");
                html.Append("<td></td>");
                html.Append($"<td>Total: {sellInvoiceDto.TotalAmount:C}</td>");
                html.Append("</tr>");
                html.Append("<tr class='total'>");
                html.Append("<td></td>");
                html.Append($"<td>Promotion Discount: {sellInvoiceDto.PromotionDiscount:C}</td>");
                html.Append("</tr>");
                html.Append("<tr class='total'>");
                html.Append("<td></td>");
                html.Append($"<td>Membership Discount: {sellInvoiceDto.MemberShipDiscount:C}</td>");
                html.Append("</tr>");
                html.Append("<tr class='total'>");
                html.Append("<td></td>");
                html.Append($"<td>Final Amount: {sellInvoiceDto.FinalAmount:C}</td>");
                html.Append("</tr>");

                html.Append("<tr class='heading'>");
                html.Append("<td>Payment Type</td>");
                html.Append("<td>Amount</td>");
                html.Append("</tr>");

                foreach (var payment in sellInvoiceDto.Payments)
                {
                    html.Append("<tr class='item'>");
                    html.Append($"<td>{payment.PaymentTypeName}</td>");
                    html.Append($"<td>{payment.Amount:C}</td>");
                    html.Append("</tr>");
                }

                html.Append("</table>");
                html.Append("</div>");
                html.Append("</body></html>");

                using (var stringReader = new StringReader(html.ToString()))
                {
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, stringReader);
                }

                document.Close();
                return ms.ToArray();
            }
        }




        public async Task<string> GetSellInvoiceHtmlAsync(int orderSellId)
        {
            var orderSell = await _orderSellRepository.GetOrderSellWithDetailsAsync(orderSellId);
            if (orderSell == null)
            {
                throw new Exception("Order not found");
            }

            var sellInvoiceDto = _mapper.Map<SellInvoiceDTO>(orderSell);

            var html = new StringBuilder();
            html.Append("<html><head><title>Invoice</title>");
            html.Append("<style>");
            html.Append("body { font-family: Arial, sans-serif; margin: 20px; }");
            html.Append(".invoice-box { max-width: 800px; margin: auto; padding: 30px; border: 1px solid #eee; box-shadow: 0 0 10px rgba(0, 0, 0, 0.15); font-size: 16px; line-height: 24px; color: #555; }");
            html.Append(".invoice-box table { width: 100%; line-height: inherit; text-align: left; border-collapse: collapse; }");
            html.Append(".invoice-box table td { padding: 5px; vertical-align: top; }");
            html.Append(".invoice-box table tr td:nth-child(2) { text-align: right; }");
            html.Append(".invoice-box table tr.top table td { padding-bottom: 20px; }");
            html.Append(".invoice-box table tr.top table td.title { font-size: 45px; line-height: 45px; color: #333; }");
            html.Append(".invoice-box table tr.information table td { padding-bottom: 40px; }");
            html.Append(".invoice-box table tr.heading td { background: #eee; border-bottom: 1px solid #ddd; font-weight: bold; }");
            html.Append(".invoice-box table tr.details td { padding-bottom: 20px; }");
            html.Append(".invoice-box table tr.item td { border-bottom: 1px solid #eee; }");
            html.Append(".invoice-box table tr.item.last td { border-bottom: none; }");
            html.Append(".invoice-box table tr.total td:nth-child(2) { border-top: 2px solid #eee; font-weight: bold; }");
            html.Append("</style>");
            html.Append("</head><body>");
            html.Append("<div class='invoice-box'>");
            html.Append("<table cellpadding='0' cellspacing='0'>");
            html.Append("<tr class='top'>");
            html.Append("<td colspan='2'>");
            html.Append("<table>");
            html.Append("<tr>");
            html.Append("<td class='title'><img src='path/to/logo.png' style='width:100%; max-width:300px;'></td>");
            html.Append("<td>");
            html.Append($"Invoice #: {sellInvoiceDto.OrderSellId}<br>");
            html.Append($"Order Date: {sellInvoiceDto.OrderDate:dd/MM/yyyy}<br>");
            html.Append($"Status: {sellInvoiceDto.Status}");
            html.Append("</td>");
            html.Append("</tr>");
            html.Append("</table>");
            html.Append("</td>");
            html.Append("</tr>");
            html.Append("<tr class='information'>");
            html.Append("<td colspan='2'>");
            html.Append("<table>");
            html.Append("<tr>");
            html.Append("<td>");
            html.Append($"Customer Name: {sellInvoiceDto.CustomerName}<br>");
            html.Append("</td>");
            html.Append("<td>");
            html.Append($"Seller: {sellInvoiceDto.SellerFirstName} {sellInvoiceDto.SellerLastName}<br>");
            html.Append("</td>");
            html.Append("</tr>");
            html.Append("</table>");
            html.Append("</td>");
            html.Append("</tr>");
            html.Append("<tr class='heading'>");
            html.Append("<td>Item</td>");
            html.Append("<td>Price</td>");
            html.Append("</tr>");

            foreach (var detail in sellInvoiceDto.OrderSellDetails)
            {
                html.Append("<tr class='item'>");
                html.Append($"<td>{detail.ProductName} (x{detail.Quantity})</td>");
                html.Append($"<td>{detail.Price:C}</td>");
                html.Append("</tr>");
            }

            html.Append("<tr class='total'>");
            html.Append("<td></td>");
            html.Append($"<td>Total: {sellInvoiceDto.TotalAmount:C}</td>");
            html.Append("</tr>");
            html.Append("<tr class='total'>");
            html.Append("<td></td>");
            html.Append($"<td>Promotion Discount: {sellInvoiceDto.PromotionDiscount:C}</td>");
            html.Append("</tr>");
            html.Append("<tr class='total'>");
            html.Append("<td></td>");
            html.Append($"<td>Membership Discount: {sellInvoiceDto.MemberShipDiscount:C}</td>");
            html.Append("</tr>");
            html.Append("<tr class='total'>");
            html.Append("<td></td>");
            html.Append($"<td>Final Amount: {sellInvoiceDto.FinalAmount:C}</td>");
            html.Append("</tr>");

            html.Append("<tr class='heading'>");
            html.Append("<td>Payment Type</td>");
            html.Append("<td>Amount</td>");
            html.Append("</tr>");

            foreach (var payment in sellInvoiceDto.Payments)
            {
                html.Append("<tr class='item'>");
                html.Append($"<td>{payment.PaymentTypeName}</td>");
                html.Append($"<td>{payment.Amount:C}</td>");
                html.Append("</tr>");
            }

            html.Append("</table>");
            html.Append("</div>");
            html.Append("</body></html>");

            return html.ToString();
        }
    }
}
