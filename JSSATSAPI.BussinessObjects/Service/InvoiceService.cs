using AutoMapper;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.ResponseModels;
using JSSATSAPI.DataAccess.IRepository;
using System.Text;
using DinkToPdf.Contracts;

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

        /*        public async Task<byte[]> ExportSellInvoiceAsync(int orderSellId)
                {
                    var orderSell = await _orderSellRepository.GetOrderSellWithDetailsAsync(orderSellId);
                    if (orderSell == null)
                    {
                        throw new Exception("Order not found");
                    }

                    var sellInvoiceDto = _mapper.Map<SellInvoiceDTO>(orderSell);
                    var htmlContent = GenerateInvoiceHtml(sellInvoiceDto);

                    using (var ms = new MemoryStream())
                    {
                        var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 5, 5, 5, 5);
                        var writer = PdfWriter.GetInstance(document, ms);
                        document.Open();

                        using (var stringReader = new StringReader(htmlContent))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, stringReader);
                        }

                        document.Close();
                        return ms.ToArray();
                    }
                }*/
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
                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 10f, 10f, 10f, 0f);
                var writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                var html = new StringBuilder();
                html.Append("<html><head><title>Invoice</title>");
                html.Append("<style>");
                html.Append("body { font-family: 'Arial', sans-serif; font-size: 14px; color: #333; }");
                html.Append(".invoice-container { max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ccc; border-radius: 10px;  }");
                html.Append(".invoice-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }");
                html.Append(".invoice-header img { max-width: 100px; }");
                html.Append(".invoice-header div { text-align: right; }");
                html.Append(".invoice-header div h1 { margin: 0; font-size: 28px; color: #000; }");
                html.Append(".invoice-header div p { margin: 5px 0; }");
                html.Append(".invoice-details { margin-bottom: 20px; }");
                html.Append(".invoice-details div { margin-bottom: 10px; }");
                html.Append(".invoice-table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }");
                html.Append(".invoice-table th, .invoice-table td { padding: 10px; border: 1px solid #ddd; }");
                html.Append(".invoice-table th { background-color: #efefef; }");
                html.Append(".invoice-summary { text-align: right; margin-bottom: 20px; }");
                html.Append(".invoice-summary div { margin-bottom: 5px; }");
                html.Append(".invoice-footer { display: flex; justify-content: space-between; align-items: center; margin-top: 50px; }");
                html.Append(".invoice-footer .signature { text-align: center; }");
                html.Append(".invoice-footer .signature .line { border-top: 1px solid #000; width: 200px; margin-top: 100px; }");
                html.Append("</style>");
                html.Append("</head><body>");
                html.Append("<div class='invoice-container'>");

                // Header Section
                html.Append("<div class='invoice-header'>");
                html.Append("<img src='https://bfrsserverupload.blob.core.windows.net/bfrsimage/JSSATS_logo.png' alt='Logo' style=\"width: 100%; max-width: 450px;\" />");
                html.Append("<div>");
                html.Append("<h1>Invoice</h1>");
                html.Append($"<p>Invoice #: {sellInvoiceDto.OrderSellId}</p>");
                html.Append($"<p>Order Date: {sellInvoiceDto.OrderDate:dd/MM/yyyy}</p>");
                html.Append($"<p>Status: {sellInvoiceDto.Status}</p>");
                html.Append("</div>");
                html.Append("</div>");

                // Customer and Seller Information
                html.Append("<div class='invoice-details'>");
                html.Append("<div><strong>Customer Name:</strong> " + sellInvoiceDto.CustomerName + "</div>");
                html.Append("<div><strong>Seller:</strong> " + sellInvoiceDto.SellerFirstName + " " + sellInvoiceDto.SellerLastName + "</div>");
                html.Append("</div>");

                // Item Table
                html.Append("<table class='invoice-table'>");
                html.Append("<thead>");
                html.Append("<tr>");
                html.Append("<th>Item</th>");
                html.Append("<th>Quantity</th>");
                html.Append("<th>Price</th>");
                html.Append("<th>Total</th>");
                html.Append("</tr>");
                html.Append("</thead>");
                html.Append("<tbody>");

                foreach (var detail in sellInvoiceDto.OrderSellDetails)
                {
                    html.Append("<tr>");
                    html.Append($"<td>{detail.ProductName}</td>");
                    html.Append($"<td>{detail.Quantity}</td>");
                    html.Append($"<td>{detail.Price.ToString("N0")} ₫</td>");
                    html.Append($"<td>{(detail.Price * detail.Quantity).ToString("N0")} ₫</td>");
                    html.Append("</tr>");
                }

                html.Append("</tbody>");
                html.Append("</table>");

                // Summary
                html.Append("<div class='invoice-summary'>");
                html.Append($"<div><strong>Total Amount:</strong> {sellInvoiceDto.TotalAmount.ToString("N0")} ₫</div>");
                html.Append($"<div><strong>Promotion Discount:</strong> {sellInvoiceDto.InvidualPromotionDiscount.ToString("N0")} ₫</div>");
                html.Append($"<div><strong>Membership Discount:</strong> {sellInvoiceDto.MemberShipDiscount.ToString("N0")} ₫</div>");
                html.Append($"<div><strong>Final Amount:</strong> {sellInvoiceDto.FinalAmount.ToString("N0")} ₫</div>");
                html.Append("</div>");

                // Payment Information
                html.Append("<table class='invoice-table'>");
                html.Append("<thead>");
                html.Append("<tr>");
                html.Append("<th>Payment Type</th>");
                html.Append("<th>Amount</th>");
                html.Append("</tr>");
                html.Append("</thead>");
                html.Append("<tbody>");

                foreach (var payment in sellInvoiceDto.Payments)
                {
                    html.Append("<tr>");
                    html.Append($"<td>{payment.PaymentTypeName}</td>");
                    html.Append($"<td>{payment.Amount.ToString("N0")} ₫</td>");
                    html.Append("</tr>");
                }

                html.Append("</tbody>");
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






        /*        public async Task<string> GetSellInvoiceHtmlAsync(int orderSellId)
                {
                    var orderSell = await _orderSellRepository.GetOrderSellWithDetailsAsync(orderSellId);
                    if (orderSell == null)
                    {
                        throw new Exception("Order not found");
                    }

                    var sellInvoiceDto = _mapper.Map<SellInvoiceDTO>(orderSell);
                    return GenerateInvoiceHtml(sellInvoiceDto);
                }*/

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
            html.Append("body { font-family: 'Arial', sans-serif; font-size: 14px; color: #333; }");
            html.Append(".invoice-container { max-width: 800px; margin: auto; padding: 20px; border: 1px solid #ccc; border-radius: 10px; background: #f9f9f9; box-shadow: 0 0 15px rgba(0, 0, 0, 0.1); }");
            html.Append(".invoice-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }");
            html.Append(".invoice-header img { max-width: 100px; }");
            html.Append(".invoice-header div { text-align: right; }");
            html.Append(".invoice-header div h1 { margin: 0; font-size: 28px; color: #000; }");
            html.Append(".invoice-header div p { margin: 5px 0; }");
            html.Append(".invoice-details { margin-bottom: 40px; }");
            html.Append(".invoice-details div { margin-bottom: 10px; }");
            html.Append(".invoice-table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }");
            html.Append(".invoice-table th, .invoice-table td { padding: 10px; border: 1px solid #ddd; }");
            html.Append(".invoice-table th { background-color: #efefef; }");
            html.Append(".invoice-summary { text-align: right; margin-bottom: 20px; }");
            html.Append(".invoice-summary div { margin-bottom: 5px; }");
            html.Append(".invoice-footer { display: flex; flex-direction: row; justify-content: space-between; align-items: center; margin-top: 50px; }");
            html.Append(".invoice-footer .signature { text-align: center; }");
            html.Append(".invoice-footer .signature .line { border-top: 1px solid #000; width: 200px; }");
            html.Append("</style>");
            html.Append("</head><body>");
            html.Append("<div class='invoice-container'>");

            // Header Section
            html.Append("<div class='invoice-header'>");
            html.Append("<img src='https://bfrsserverupload.blob.core.windows.net/bfrsimage/JSSATS_logo.png' alt='Logo' style=\"width: 100%; max-width: 300px;\" />");
            html.Append("<div>");
            html.Append("<h1>Invoice</h1>");
            html.Append($"<p>Invoice #: {sellInvoiceDto.OrderSellId}</p>");
            html.Append($"<p>Order Date: {sellInvoiceDto.OrderDate:dd/MM/yyyy}</p>");
            html.Append($"<p>Status: {sellInvoiceDto.Status}</p>");
            html.Append("</div>");
            html.Append("</div>");

            // Customer and Seller Information
            html.Append("<div class='invoice-details'>");
            html.Append("<div><strong>Customer Name:</strong> " + sellInvoiceDto.CustomerName + "</div>");
            html.Append("<div><strong>Seller:</strong> " + sellInvoiceDto.SellerFirstName + " " + sellInvoiceDto.SellerLastName + "</div>");
            html.Append("</div>");

            // Item Table
            html.Append("<table class='invoice-table'>");
            html.Append("<thead>");
            html.Append("<tr>");
            html.Append("<th>Item</th>");
            html.Append("<th>Quantity</th>");
            html.Append("<th>Price</th>");
            html.Append("<th>Total</th>");
            html.Append("</tr>");
            html.Append("</thead>");
            html.Append("<tbody>");

            foreach (var detail in sellInvoiceDto.OrderSellDetails)
            {
                html.Append("<tr>");
                html.Append($"<td>{detail.ProductName}</td>");
                html.Append($"<td>{detail.Quantity}</td>");
                html.Append($"<td>{detail.Price.ToString("N0")} ₫</td>");
                html.Append($"<td>{(detail.Price * detail.Quantity).ToString("N0")} ₫</td>");
                html.Append("</tr>");
            }

            html.Append("</tbody>");
            html.Append("</table>");

            // Summary
            html.Append("<div class='invoice-summary'>");
            html.Append($"<div><strong>Total Amount:</strong> {sellInvoiceDto.TotalAmount.ToString("N0")} ₫</div>");
            html.Append($"<div><strong>Promotion Discount:</strong> {sellInvoiceDto.InvidualPromotionDiscount.ToString("N0")} ₫</div>");
            html.Append($"<div><strong>Membership Discount:</strong> {sellInvoiceDto.MemberShipDiscount.ToString("N0")} ₫</div>");
            html.Append($"<div><strong>Final Amount:</strong> {sellInvoiceDto.FinalAmount.ToString("N0")} ₫</div>");
            html.Append("</div>");

            // Payment Information
            html.Append("<table class='invoice-table'>");
            html.Append("<thead>");
            html.Append("<tr>");
            html.Append("<th>Payment Type</th>");
            html.Append("<th>Amount</th>");
            html.Append("</tr>");
            html.Append("</thead>");
            html.Append("<tbody>");

            foreach (var payment in sellInvoiceDto.Payments)
            {
                html.Append("<tr>");
                html.Append($"<td>{payment.PaymentTypeName}</td>");
                html.Append($"<td>{payment.Amount.ToString("N0")} ₫</td>");
                html.Append("</tr>");
            }

            html.Append("</tbody>");
            html.Append("</table>");


            html.Append("</div>");
            html.Append("</body></html>");

            return html.ToString();
        }


        private string GenerateInvoiceHtml(SellInvoiceDTO sellInvoiceDto)
        {
            var html = new StringBuilder();
            html.Append("<!DOCTYPE html>");
            html.Append("<html lang=\"vi\">");
            html.Append("<head>");
            html.Append("<meta charset=\"UTF-8\"/>");
            html.Append("<title>Hóa Đơn</title>");
            html.Append("<style>");
            html.Append("body { font-family: 'Arial', sans-serif; margin: 0; padding: 0; background-color: #f6f6f6; }");
            html.Append(".invoice-box { max-width: 800px; margin: auto; padding: 30px; border: 1px solid #ddd; background-color: #fff; box-shadow: 0 0 10px rgba(0, 0, 0, 0.15); font-size: 16px; line-height: 24px; color: #333; }");
            html.Append(".invoice-box table { width: 100%; line-height: inherit; text-align: left; border-collapse: collapse; }");
            html.Append(".invoice-box table td { padding: 8px; vertical-align: top; }");
            html.Append(".invoice-box table tr td:nth-child(2) { text-align: right; }");
            html.Append(".invoice-box table tr.top table td { padding-bottom: 20px; }");
            html.Append(".invoice-box table tr.top table td.title { font-size: 45px; line-height: 45px; color: #333; }");
            html.Append(".invoice-box table tr.information table td { padding-bottom: 30px; }");
            html.Append(".invoice-box table tr.heading td { background: #eee; border-bottom: 1px solid #ddd; font-weight: bold; }");
            html.Append(".invoice-box table tr.details td { padding-bottom: 20px; }");
            html.Append(".invoice-box table tr.item td { border-bottom: 1px solid #eee; }");
            html.Append(".invoice-box table tr.item.last td { border-bottom: none; }");
            html.Append(".invoice-box table tr.total td:nth-child(2) { border-top: 2px solid #eee; font-weight: bold; }");
            html.Append(".bold { font-weight: bold; }");
            html.Append(".signature-section { margin-top: 50px; }");
            html.Append(".signature-section .signature-box { width: 45%; display: inline-block; text-align: center; }");
            html.Append(".signature-section .signature-box .line { margin: 120px 0 60px; border-top: 1px solid #000; }");
            html.Append("</style>");
            html.Append("</head>");
            html.Append("<body>");
            html.Append("<div class=\"invoice-box\">");
            html.Append("<table cellpadding=\"0\" cellspacing=\"0\">");
            html.Append("<tr class=\"top\">");
            html.Append("<td colspan=\"2\">");
            html.Append("<table>");
            html.Append("<tr>");
            html.Append("<td class=\"title\">");
            html.Append("<img src=\"https://bfrsserverupload.blob.core.windows.net/bfrsimage/JSSATS_logo.png\" style=\"width: 100%; max-width: 300px;\" />");
            html.Append("</td>");
            html.Append("<td>");
            html.Append($"<span class=\"bold\">Hóa Đơn #:</span> {sellInvoiceDto.OrderSellId}<br/>");
            html.Append($"<span class=\"bold\">Ngày Đặt Hàng:</span> {sellInvoiceDto.OrderDate:dd/MM/yyyy}<br/>");
            html.Append($"<span class=\"bold\">Trạng Thái:</span> {sellInvoiceDto.Status}");
            html.Append("</td>");
            html.Append("</tr>");
            html.Append("</table>");
            html.Append("</td>");
            html.Append("</tr>");
            html.Append("<tr class=\"information\">");
            html.Append("<td colspan=\"2\">");
            html.Append("<table>");
            html.Append("<tr>");
            html.Append("<td>");
            html.Append($"<span class=\"bold\">Tên Khách Hàng:</span> {sellInvoiceDto.CustomerName}<br/>");
            html.Append("</td>");
            html.Append("<td>");
            html.Append($"<span class=\"bold\">Người Bán:</span> {sellInvoiceDto.SellerFirstName} {sellInvoiceDto.SellerLastName}<br/>");
            html.Append("</td>");
            html.Append("</tr>");
            html.Append("</table>");
            html.Append("</td>");
            html.Append("</tr>");
            html.Append("<tr class=\"heading\">");
            html.Append("<td>Mặt Hàng</td>");
            html.Append("<td>Giá (₫)</td>");
            html.Append("</tr>");

            foreach (var detail in sellInvoiceDto.OrderSellDetails)
            {
                html.Append("<tr class=\"item\">");
                html.Append($"<td>{detail.ProductName} (x{detail.Quantity})</td>");
                html.Append($"<td>{detail.Price.ToString("N0")}</td>");
                html.Append("</tr>");
            }

            html.Append("<tr class=\"total\">");
            html.Append("<td></td>");
            html.Append($"<td><span class=\"bold\">Tổng Cộng:</span> {sellInvoiceDto.TotalAmount.ToString("N0")} ₫</td>");
            html.Append("</tr>");
            html.Append("<tr class=\"total\">");
            html.Append("<td></td>");
            html.Append($"<td><span class=\"bold\">Khuyến Mãi:</span> {sellInvoiceDto.InvidualPromotionDiscount.ToString("N0")} ₫</td>");
            html.Append("</tr>");
            html.Append("<tr class=\"total\">");
            html.Append("<td></td>");
            html.Append($"<td><span class=\"bold\">Giảm Giá Thành Viên:</span> {sellInvoiceDto.MemberShipDiscount.ToString("N0")} ₫</td>");
            html.Append("</tr>");
            html.Append("<tr class=\"total\">");
            html.Append("<td></td>");
            html.Append($"<td><span class=\"bold\">Số Tiền Cuối Cùng:</span> {sellInvoiceDto.FinalAmount.ToString("N0")} ₫</td>");
            html.Append("</tr>");
            html.Append("<tr class=\"heading\">");
            html.Append("<td>Hình Thức Thanh Toán</td>");
            html.Append("<td>Số Tiền (₫)</td>");
            html.Append("</tr>");

            foreach (var payment in sellInvoiceDto.Payments)
            {
                html.Append("<tr class=\"item\">");
                html.Append($"<td>{payment.PaymentTypeName}</td>");
                html.Append($"<td>{payment.Amount.ToString("N0")} ₫</td>");
                html.Append("</tr>");
            }

            html.Append("</table>");

            html.Append("<div class=\"signature-section\">");
            html.Append("<div class=\"signature-box\">");
            html.Append("<span  class=\"bold\">Người Mua Hàng</span>");
            html.Append("<div class=\"line\"></div>");
            html.Append("</div>");

            html.Append("<div class=\"signature-box\" style=\"float: right;\">");
            html.Append("<span class=\"bold\">Người Bán Hàng</span>");
            html.Append("<div class=\"line\"></div>");
            html.Append("</div>");

            html.Append("</div>");

            html.Append("</div>");
            html.Append("</body>");
            html.Append("</html>");

            return html.ToString();
        }



    }
}


