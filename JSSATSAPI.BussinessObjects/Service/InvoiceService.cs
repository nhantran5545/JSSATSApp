using AutoMapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.ResponseModels;
using JSSATSAPI.DataAccess.IRepository;
using System.Text;
using QRCoder;
using JSSATSAPI.DataAccess.Repository;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.Kernel.Pdf;
using iText.Layout.Font;
using System.Globalization;
using JSSATSAPI.DataAccess.Models;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IOrderSellRepository _orderSellRepository;
        private readonly IOrderBuyBackRepository _orderBuyBackRepository;
        private readonly IMapper _mapper;

        public InvoiceService(IOrderSellRepository orderSellRepository, IMapper mapper, IOrderBuyBackRepository orderBuyBackRepository)
        {
            _orderSellRepository = orderSellRepository;
            _mapper = mapper;
            _orderBuyBackRepository = orderBuyBackRepository;
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
                var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);
                document.Open();

                var html = new StringBuilder();
                html.Append("<html><head><title>Invoice</title>");
                html.Append("<style>");
                html.Append("body { font-family: 'Arial', sans-serif; font-size: 14px; color: #333; margin: 0; padding: 0; }");
                html.Append(".invoice-container { max-width: 800px; margin: auto; padding: 20px; border: 1px solid #ccc; border-radius: 10px; }");
                html.Append(".invoice-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }");
                html.Append(".invoice-header .qr-code { margin-top: 10px; }");
                html.Append(".invoice-title { text-align: center; margin-bottom: 20px; }"); 
                html.Append(".invoice-details { margin-bottom: 20px; }");
                html.Append(".invoice-details div { margin-bottom: 10px; }");
                html.Append(".invoice-table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }");
                html.Append(".invoice-table th, .invoice-table td { padding: 10px; border: 1px solid #ddd; }");
                html.Append(".invoice-table th { background-color: #efefef; }");
                html.Append(".invoice-summary { text-align: right; margin-bottom: 20px; }");
                html.Append(".invoice-summary div { margin-bottom: 5px; }");
                html.Append(".invoice-footer { display: flex; justify-content: space-between; align-items: center; margin-top: 50px; }");
                html.Append(".invoice-footer .signature { text-align: center; }");
                html.Append(".invoice-footer .signature .line { border-top: 1px solid #000; width: 200px; margin-top: 50px; }");
                html.Append("</style>");
                html.Append("</head><body>");
                html.Append("<div class='invoice-container'>");

                // Header Section
                html.Append("<div class='invoice-header'>");
                html.Append("<img src='https://bfrsserverupload.blob.core.windows.net/bfrsimage/JSSATS_logo.png' alt='Logo' style='width: 100%; max-width: 450px;' />");

                // Generate QR code
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(orderSellId.ToString(), QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCoder.QRCode(qrCodeData);
                using (var qrBitmap = qrCode.GetGraphic(20))
                {
                    using (var qrStream = new MemoryStream())
                    {
                        qrBitmap.Save(qrStream, System.Drawing.Imaging.ImageFormat.Png);
                        var qrImage = iTextSharp.text.Image.GetInstance(qrStream.ToArray());
                        qrImage.ScaleAbsolute(100f, 100f); // Set QR code image size
                        qrImage.SetAbsolutePosition(450, 700); // Adjusted position of QR code to the right and lower
                        document.Add(qrImage);
                    }
                }
                html.Append("</div>");

                // Title Section
                html.Append("<div>");

                html.Append("<h1 class='invoice-title'>Hóa Đơn Bán Hàng</h1>");

                html.Append("<div style='display: flex; justify-content: space-between; margin-bottom: 20px;'>");
                html.Append("<div>");
                html.Append($"<p><strong>Hóa đơn #:</strong> {sellInvoiceDto.OrderSellId}</p>");
                html.Append($"<p><strong>Ngày:</strong> {sellInvoiceDto.OrderDate:dd/MM/yyyy}</p>");
                html.Append("</div>");
                html.Append("<div>");
                html.Append($"<p><strong>Khách hàng:</strong> {sellInvoiceDto.CustomerName}</p>"); 
                html.Append($"<p><strong>Nhân viên:</strong> {sellInvoiceDto.SellerFirstName} {sellInvoiceDto.SellerLastName}</p>");
                html.Append("</div>");

                html.Append("</div>");

                html.Append("</div>");

                // Order Details
                html.Append("<div class='invoice-details'>");
                html.Append("<h2>Chi Tiết Đơn Hàng</h2>");
                html.Append("<table class='invoice-table'>");
                html.Append("<thead>");
                html.Append("<tr>");
                html.Append("<th>STT</th>");
                html.Append("<th>Tên Sản Phẩm</th>");
                html.Append("<th>Số Lượng</th>");
                html.Append("<th>Giá (VNĐ)</th>");
                html.Append("<th>Tổng (VNĐ)</th>");
                html.Append("</tr>");
                html.Append("</thead>");
                html.Append("<tbody>");

                int index = 1;
                foreach (var detail in sellInvoiceDto.OrderSellDetails)
                {
                    html.Append("<tr>");
                    html.Append($"<td>{index}</td>");
                    html.Append($"<td>{detail.ProductName}</td>");
                    html.Append($"<td>{detail.Quantity}</td>");
                    html.Append($"<td>{detail.Price.ToString("N0")} VNĐ</td>");
                    html.Append($"<td>{(detail.Price * detail.Quantity).ToString("N0")} VNĐ</td>");
                    html.Append("</tr>");

                    index++;
                }

                html.Append("</tbody>");
                html.Append("</table>");
                html.Append("</div>");

                // Summary
                html.Append("<div class='invoice-summary'>");
                html.Append($"<div><strong>Tổng tiền:</strong> {sellInvoiceDto.TotalAmount.ToString("N0")} VNĐ</div>");
                html.Append($"<div><strong>Giảm giá:</strong> {sellInvoiceDto.InvidualPromotionDiscount.ToString("N0")} VNĐ</div>");
                html.Append($"<div><strong>Giảm giá thành viên:</strong> {sellInvoiceDto.MemberShipDiscount.ToString("N0")} VNĐ</div>");
                html.Append($"<div><strong>Số tiền thanh toán:</strong> {sellInvoiceDto.FinalAmount.ToString("N0")} VNĐ</div>");
                html.Append("</div>");

                // Payment Information
                html.Append("<h2>Thanh Toán</h2>");
                html.Append("<table class='invoice-table'>");
                html.Append("<thead>");
                html.Append("<tr>");
                html.Append("<th>Hình thức thanh toán</th>");
                html.Append("<th>Số tiền (VNĐ)</th>");
                html.Append("<th>Ngày</th>");
                html.Append("</tr>");
                html.Append("</thead>");
                html.Append("<tbody>");

                foreach (var payment in sellInvoiceDto.Payments)
                {
                    DateTime createDate = payment.CreateDate;
                    string paymentDateString = createDate != DateTime.MinValue ? createDate.ToString("dd-MM-yyyy") : "N/A";
                    html.Append("<tr>");
                    html.Append($"<td>{payment.PaymentTypeName}</td>");
                    html.Append($"<td>{payment.Amount.ToString("N0") ?? "0"}</td>");
                    html.Append($"<td>{paymentDateString}</td>");
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

        public async Task<string> GetBuyBackInvoiceHtmlAsync(int orderBuyBackId )
        {
            var orderBuyBack = await _orderBuyBackRepository.GetByIdAsync(orderBuyBackId);
            if (orderBuyBack == null)
            {
                throw new Exception("Không tìm thấy đơn hàng mua lại");
            }


            var html = new StringBuilder();
            html.Append("<html><head><title>Invoice</title>");
            html.Append("<style>");
            html.Append("body { font-family: 'Arial', sans-serif; font-size: 14px; color: #333; margin: 0; padding: 0; }");
            html.Append(".invoice-container { max-width: 800px; margin: auto; padding: 20px; border: 1px solid #ccc; border-radius: 10px; }");
            html.Append(".invoice-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }");
            html.Append(".invoice-header .qr-code { margin-top: 10px; }");
            html.Append(".invoice-title { text-align: center; margin-bottom: 20px; }");
            html.Append(".invoice-info { text-align: left; margin-bottom: 10px; }");
            html.Append(".invoice-details { margin-bottom: 20px; }");
            html.Append(".invoice-details div { margin-bottom: 10px; }");
            html.Append(".invoice-table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }");
            html.Append(".invoice-table th, .invoice-table td { padding: 10px; border: 1px solid #ddd; }");
            html.Append(".invoice-table th { background-color: #efefef; }");
            html.Append(".invoice-summary { text-align: right; margin-bottom: 20px; }");
            html.Append(".invoice-summary div { margin-bottom: 5px; }");
            html.Append(".invoice-footer { display: flex; justify-content: space-between; align-items: center; margin-top: 50px; }");
            html.Append(".invoice-footer .signature { text-align: center; }");
            html.Append(".invoice-footer .signature .line { border-top: 1px solid #000; width: 200px; margin-top: 50px; }");
            html.Append("</style>");
            html.Append("</head><body>");
            html.Append("<div class='invoice-container'>");

            // Header Section
            html.Append("<div class='invoice-header'>");
            html.Append("<img src='https://bfrsserverupload.blob.core.windows.net/bfrsimage/JSSATS_logo.png' alt='Logo' style='width: 70%; max-width: 400px;' />");

            // Generate QR code
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(orderBuyBackId.ToString(), QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCoder.QRCode(qrCodeData);
            using (var qrBitmap = qrCode.GetGraphic(20))
            {
                using (var qrStream = new MemoryStream())
                {
                    qrBitmap.Save(qrStream, System.Drawing.Imaging.ImageFormat.Png);
                    var qrBase64 = Convert.ToBase64String(qrStream.ToArray());
                    html.Append($"<img src='data:image/png;base64,{qrBase64}' alt='QR Code' style='width: 100px; height: 100px;' />");
                }
            }

            html.Append("</div>");

            // Title Section
            html.Append("<div class='invoice-title'>");
            html.Append("<h1>Hóa Đơn Mua Lại</h1>");
            html.Append("</div>");
            html.Append("<div class='invoice-info'>");
            html.Append($"<p>Ngày: {orderBuyBack.DateBuyBack?.ToString("dd-MM-yyyy") ?? ""}</p>");
            html.Append($"<p>Khách hàng: {orderBuyBack.Customer.Name}</p>");
            html.Append($"<p>Số điện thoại: {orderBuyBack.Customer.Phone}</p>");
            html.Append("</div>"); 

            // Order Details
            html.Append("<div class='invoice-details'>");
            html.Append("<h2>Chi Tiết Đơn Hàng</h2>");
            html.Append("<table class='invoice-table'>");
            html.Append("<thead>");
            html.Append("<tr>");
            html.Append("<th>STT</th>");
            html.Append("<th>Tên Sản Phẩm</th>");
            html.Append("<th>Số Lượng</th>");
            html.Append("<th>Giá (VNĐ)</th>");
            html.Append("<th>Tổng (VNĐ)</th>");
            html.Append("</tr>");
            html.Append("</thead>");
            html.Append("<tbody>");

            int index = 1;
            foreach (var detail in orderBuyBack.OrderBuyBackDetails)
            {
                string productName = !string.IsNullOrEmpty(detail.ProductId) && !string.IsNullOrEmpty(detail.Product?.ProductName)
                    ? detail.Product.ProductName
                    : detail.BuyBackProductName;

                int quantity = detail.Quantity ?? 1;
                decimal price = detail.Price ?? 0;
                decimal total = quantity * price;

                html.Append("<tr>");
                html.Append($"<td>{index}</td>");
                html.Append($"<td>{productName}</td>");
                html.Append($"<td>{quantity}</td>");
                html.Append($"<td>{price.ToString("N0")}</td>");
                html.Append($"<td>{total.ToString("N0")}</td>");
                html.Append("</tr>");

                index++;
            }

            html.Append("</tbody>");
            html.Append("</table>");
            html.Append("</div>");

            // Summary
            html.Append("<div class='invoice-summary'>");
            html.Append($"<div><strong>Tổng Tiền:</strong> {orderBuyBack.TotalAmount?.ToString("N0") ?? "0"} VNĐ</div>");
            html.Append($"<div><strong>Số Tiền Thanh Toán:</strong> {orderBuyBack.FinalAmount?.ToString("N0") ?? "0"} VNĐ</div>");
            html.Append("</div>");

            // Payment Information
            html.Append("<h2>Thanh Toán</h2>");
            html.Append("<table class='invoice-table'>");
            html.Append("<thead>");
            html.Append("<tr>");
            html.Append("<th>Hình Thức Thanh Toán</th>");
            html.Append("<th>Số Tiền (VNĐ)</th>");
            html.Append("<th>Ngày</th>");
            html.Append("</tr>");
            html.Append("</thead>");
            html.Append("<tbody>");

            foreach (var payment in orderBuyBack.Payments)
            {
                string paymentDateString = payment.CreateDate.HasValue ? payment.CreateDate.Value.ToString("dd-MM-yyyy") : "N/A";
                html.Append("<tr>");
                html.Append($"<td>{payment.PaymentType.PaymentTypeName}</td>");
                html.Append($"<td>{payment.Amount.ToString("N0") ?? "0"}</td>");
                html.Append($"<td>{paymentDateString}</td>");
                html.Append("</tr>");
            }

            html.Append("</tbody>");
            html.Append("</table>");

            html.Append("</div>");
            html.Append("</body></html>"); 


            return html.ToString();
        }
        public async Task<string> GetSellInvoiceHtmlAsync(int orderSellId)
        {
            var orderSell = await _orderSellRepository.GetOrderSellWithDetailsAsync(orderSellId);
            if (orderSell == null)
            {
                throw new Exception("Order not found");
            }

            var sellInvoiceDto = _mapper.Map<SellInvoiceDTO>(orderSell);

            // Generate QR code
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(orderSellId.ToString(), QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCoder.QRCode(qrCodeData);
            string qrCodeBase64;
            using (var qrBitmap = qrCode.GetGraphic(20))
            {
                using (var qrStream = new MemoryStream())
                {
                    qrBitmap.Save(qrStream, System.Drawing.Imaging.ImageFormat.Png);
                    var qrBytes = qrStream.ToArray();
                    qrCodeBase64 = Convert.ToBase64String(qrBytes);
                }
            }

            var html = new StringBuilder();
            html.Append("<html><head><title>Invoice</title>");
            html.Append("<style>");
            html.Append("body { font-family: 'Arial', sans-serif; font-size: 14px; color: #333; margin: 0; padding: 0; }");
            html.Append(".invoice-container { max-width: 800px; margin: auto; padding: 20px; border: 1px solid #ccc; border-radius: 10px; }");
            html.Append(".invoice-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }");
            html.Append(".invoice-header .qr-code { margin-top: 10px; }");
            html.Append(".invoice-title { text-align: center; margin-bottom: 20px; }");
            html.Append(".invoice-details { margin-bottom: 20px; }");
            html.Append(".invoice-details div { margin-bottom: 10px; }");
            html.Append(".invoice-table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }");
            html.Append(".invoice-table th, .invoice-table td { padding: 10px; border: 1px solid #ddd; }");
            html.Append(".invoice-table th { background-color: #efefef; }");
            html.Append(".invoice-summary { text-align: right; margin-bottom: 20px; }");
            html.Append(".invoice-summary div { margin-bottom: 5px; }");
            html.Append(".invoice-footer { display: flex; justify-content: space-between; align-items: center; margin-top: 50px; }");
            html.Append(".invoice-footer .signature { text-align: center; }");
            html.Append(".invoice-footer .signature .line { border-top: 1px solid #000; width: 200px; margin-top: 50px; }");
            html.Append("</style>");
            html.Append("</head><body>");
            html.Append("<div class='invoice-container'>");

            // Header Section
            html.Append("<div class='invoice-header'>");
            html.Append("<img src='https://bfrsserverupload.blob.core.windows.net/bfrsimage/JSSATS_logo.png' alt='Logo' style='width: 100%; max-width: 450px;' />");
            html.Append("<div>");
            html.Append($"<p>Invoice #: {sellInvoiceDto.OrderSellId}</p>");
            html.Append($"<p>Order Date: {sellInvoiceDto.OrderDate:dd/MM/yyyy}</p>");
            html.Append("</div>");

            html.Append($"<div class='qr-code'><img src='data:image/png;base64,{qrCodeBase64}' alt='QR Code' style='width: 100px; height: 100px;' /></div>");

            html.Append("</div>");

            // Title Section
            html.Append("<div class='invoice-title'>");
            html.Append("<h1>Hóa Đơn Bán Hàng</h1>");
            html.Append("<p><strong>Khách hàng:</strong> " + sellInvoiceDto.CustomerName + "</p>");
            html.Append("<p><strong>Nhân viên:</strong> " + sellInvoiceDto.SellerFirstName + " " + sellInvoiceDto.SellerLastName + "</p>");
            html.Append("</div>");

            // Order Details
            html.Append("<div class='invoice-details'>");
            html.Append("<h2>Chi Tiết Đơn Hàng</h2>");
            html.Append("<table class='invoice-table'>");
            html.Append("<thead>");
            html.Append("<tr>");
            html.Append("<th>STT</th>");
            html.Append("<th>Tên Sản Phẩm</th>");
            html.Append("<th>Số Lượng</th>");
            html.Append("<th>Giá (VNĐ)</th>");
            html.Append("<th>Tổng (VNĐ)</th>");
            html.Append("</tr>");
            html.Append("</thead>");
            html.Append("<tbody>");

            int index = 1;
            foreach (var detail in sellInvoiceDto.OrderSellDetails)
            {
                html.Append("<tr>");
                html.Append($"<td>{index}</td>");
                html.Append($"<td>{detail.ProductName}</td>");
                html.Append($"<td>{detail.Quantity}</td>");
                html.Append($"<td>{detail.Price.ToString("N0")} VNĐ</td>");
                html.Append($"<td>{(detail.Price * detail.Quantity).ToString("N0")} VNĐ</td>");
                html.Append("</tr>");

                index++;
            }

            html.Append("</tbody>");
            html.Append("</table>");
            html.Append("</div>");

            // Summary
            html.Append("<div class='invoice-summary'>");
            html.Append($"<div><strong>Tổng tiền:</strong> {sellInvoiceDto.TotalAmount.ToString("N0")} VNĐ</div>");
            html.Append($"<div><strong>Giảm giá:</strong> {sellInvoiceDto.InvidualPromotionDiscount.ToString("N0")} VNĐ</div>");
            html.Append($"<div><strong>Giảm giá thành viên:</strong> {sellInvoiceDto.MemberShipDiscount.ToString("N0")} VNĐ</div>");
            html.Append($"<div><strong>Số tiền thanh toán:</strong> {sellInvoiceDto.FinalAmount.ToString("N0")} VNĐ</div>");
            html.Append("</div>");

            // Payment Information
            html.Append("<h2>Thanh Toán</h2>");
            html.Append("<table class='invoice-table'>");
            html.Append("<thead>");
            html.Append("<tr>");
            html.Append("<th>Hình thức thanh toán</th>");
            html.Append("<th>Số tiền (VNĐ)</th>");
            html.Append("<th>Ngày</th>");
            html.Append("</tr>");
            html.Append("</thead>");
            html.Append("<tbody>");

            foreach (var payment in sellInvoiceDto.Payments)
            {
                DateTime createDate = payment.CreateDate;
                string paymentDateString = createDate != DateTime.MinValue ? createDate.ToString("dd-MM-yyyy") : "N/A";
                html.Append("<tr>");
                html.Append($"<td>{payment.PaymentTypeName}</td>");
                html.Append($"<td>{payment.Amount.ToString("N0") ?? "0"}</td>");
                html.Append($"<td>{paymentDateString}</td>");
                html.Append("</tr>");
            }

            html.Append("</tbody>");
            html.Append("</table>");

            html.Append("</div>");
            html.Append("</body></html>");

            return html.ToString();
        }

        public async Task<byte[]> GenerateOrderBuyBackInvoicePdfAsync(int orderBuyBackId)
        {
            var orderBuyBack = await _orderBuyBackRepository.GetByIdAsync(orderBuyBackId);
            if (orderBuyBack == null)
            {
                throw new Exception("Không tìm thấy đơn hàng mua lại");
            }

            using (var ms = new MemoryStream())
            {
                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 10f, 10f, 10f, 0f);
                var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);
                document.Open();

                StringBuilder html = new StringBuilder();

                html.Append("<html><head><title>Invoice</title>");
                html.Append("<style>");
                html.Append("body { font-family: 'Arial', sans-serif; font-size: 14px; color: #333; margin: 0; padding: 0; }");
                html.Append(".invoice-container { max-width: 800px; margin: auto; padding: 20px; border: 1px solid #ccc; border-radius: 10px; }");
                html.Append(".invoice-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }");
                html.Append(".invoice-header .qr-code { margin-top: 10px; }");
                html.Append(".invoice-title { text-align: center; margin-bottom: 20px; }"); 
                html.Append(".invoice-info { text-align: left; margin-bottom: 10px; }");
                html.Append(".invoice-details { margin-bottom: 20px; }");
                html.Append(".invoice-details div { margin-bottom: 10px; }");
                html.Append(".invoice-table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }");
                html.Append(".invoice-table th, .invoice-table td { padding: 10px; border: 1px solid #ddd; }");
                html.Append(".invoice-table th { background-color: #efefef; }");
                html.Append(".invoice-summary { text-align: right; margin-bottom: 20px; }");
                html.Append(".invoice-summary div { margin-bottom: 5px; }");
                html.Append(".invoice-footer { display: flex; justify-content: space-between; align-items: center; margin-top: 50px; }");
                html.Append(".invoice-footer .signature { text-align: center; }");
                html.Append(".invoice-footer .signature .line { border-top: 1px solid #000; width: 200px; margin-top: 50px; }");
                html.Append("</style>");
                html.Append("</head><body>");
                html.Append("<div class='invoice-container'>");

                // Header Section
                html.Append("<div class='invoice-header'>");
                html.Append("<img src='https://bfrsserverupload.blob.core.windows.net/bfrsimage/JSSATS_logo.png' alt='Logo' style='width: 70%; max-width: 400px;' />");

                // Generate QR code
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(orderBuyBackId.ToString(), QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCoder.QRCode(qrCodeData);
                using (var qrBitmap = qrCode.GetGraphic(20))
                {
                    using (var qrStream = new MemoryStream())
                    {
                        qrBitmap.Save(qrStream, System.Drawing.Imaging.ImageFormat.Png);
                        var qrImage = iTextSharp.text.Image.GetInstance(qrStream.ToArray());
                        qrImage.ScaleAbsolute(100f, 100f); // Set QR code image size
                        qrImage.SetAbsolutePosition(document.PageSize.Width - 110, document.PageSize.Height - 140); // Adjusted position of QR code to the right
                        document.Add(qrImage);
                    }
                }

                html.Append("</div>");

                // Title Section
                html.Append("<div class='invoice-title'>");
                html.Append("<h1>Hóa Đơn Mua Lại</h1>");
                html.Append("</div>");
                html.Append("<div class='invoice-info'>");
                html.Append($"<p>Ngày: {orderBuyBack.DateBuyBack?.ToString("dd-MM-yyyy") ?? ""}</p>");
                html.Append($"<p>Khách hàng: {orderBuyBack.Customer.Name}</p>");
                html.Append($"<p>Số điện thoại: {orderBuyBack.Customer.Phone}</p>");
                html.Append("</div>");

                // Order Details
                html.Append("<div class='invoice-details'>");
                html.Append("<h2>Chi Tiết Đơn Hàng</h2>");
                html.Append("<table class='invoice-table'>");
                html.Append("<thead>");
                html.Append("<tr>");
                html.Append("<th>STT</th>");
                html.Append("<th>Tên Sản Phẩm</th>");
                html.Append("<th>Số Lượng</th>");
                html.Append("<th>Giá (VNĐ)</th>");
                html.Append("<th>Tổng (VNĐ)</th>");
                html.Append("</tr>");
                html.Append("</thead>");
                html.Append("<tbody>");

                int index = 1;
                foreach (var detail in orderBuyBack.OrderBuyBackDetails)
                {
                    string productName = !string.IsNullOrEmpty(detail.ProductId) && !string.IsNullOrEmpty(detail.Product?.ProductName)
                        ? detail.Product.ProductName
                        : detail.BuyBackProductName;

                    int quantity = detail.Quantity ?? 1;
                    decimal price = detail.Price ?? 0;
                    decimal total = quantity * price;

                    html.Append("<tr>");
                    html.Append($"<td>{index}</td>");
                    html.Append($"<td>{productName}</td>");
                    html.Append($"<td>{quantity}</td>");
                    html.Append($"<td>{price.ToString("N0")}</td>");
                    html.Append($"<td>{total.ToString("N0")}</td>");
                    html.Append("</tr>");

                    index++;
                }

                html.Append("</tbody>");
                html.Append("</table>");
                html.Append("</div>");

                // Summary
                html.Append("<div class='invoice-summary'>");
                html.Append($"<div><strong>Tổng Tiền:</strong> {orderBuyBack.TotalAmount?.ToString("N0") ?? "0"} VNĐ</div>");
                html.Append($"<div><strong>Số Tiền Thanh Toán:</strong> {orderBuyBack.FinalAmount?.ToString("N0") ?? "0"} VNĐ</div>");
                html.Append("</div>");

                // Payment Information
                html.Append("<h2>Thanh Toán</h2>");
                html.Append("<table class='invoice-table'>");
                html.Append("<thead>");
                html.Append("<tr>");
                html.Append("<th>Hình Thức Thanh Toán</th>");
                html.Append("<th>Số Tiền (VNĐ)</th>");
                html.Append("<th>Ngày</th>");
                html.Append("</tr>");
                html.Append("</thead>");
                html.Append("<tbody>");

                foreach (var payment in orderBuyBack.Payments)
                {
                    string paymentDateString = payment.CreateDate.HasValue ? payment.CreateDate.Value.ToString("dd-MM-yyyy") : "N/A";
                    html.Append("<tr>");
                    html.Append($"<td>{payment.PaymentType.PaymentTypeName}</td>");
                    html.Append($"<td>{payment.Amount.ToString("N0") ?? "0"}</td>");
                    html.Append($"<td>{paymentDateString}</td>");
                    html.Append("</tr>");
                }

                html.Append("</tbody>");
                html.Append("</table>");

                html.Append("</div>"); // Closing invoice-container
                html.Append("</body></html>"); // Closing body and html

                using (var stringReader = new StringReader(html.ToString()))
                {
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, stringReader);
                }

                document.Close();
                return ms.ToArray();
            }
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


