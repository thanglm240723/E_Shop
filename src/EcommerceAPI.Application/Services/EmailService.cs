using EcommerceAPI.Core.DTOs.Responses;
using EcommerceAPI.Core.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace EcommerceAPI.Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly SmtpSettings _smtpSettings;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _smtpSettings = new SmtpSettings
        {
            Host = _configuration["SmtpSettings:Host"] ?? "smtp.gmail.com",
            Port = int.Parse(_configuration["SmtpSettings:Port"] ?? "587"),
            Username = _configuration["SmtpSettings:Username"] ?? "",
            Password = _configuration["SmtpSettings:Password"] ?? "",
            From = _configuration["SmtpSettings:From"] ?? "",
            FromName = _configuration["SmtpSettings:FromName"] ?? "Ecommerce Shop"
        };
    }

    public async Task SendOrderConfirmationEmailAsync(OrderResponse order)
    {
        var subject = $"Xác nhận đơn hàng #{order.OrderCode}";

        var body = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 20px; background-color: #f9f9f9; }}
                    .order-info {{ background-color: white; padding: 15px; margin: 10px 0; border-radius: 5px; }}
                    .product-item {{ padding: 10px; border-bottom: 1px solid #ddd; }}
                    .total {{ font-size: 18px; font-weight: bold; color: #4CAF50; margin-top: 20px; }}
                    .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Cảm ơn bạn đã đặt hàng!</h1>
                    </div>
                    
                    <div class='content'>
                        <p>Xin chào <strong>{order.CustomerName}</strong>,</p>
                        <p>Chúng tôi đã nhận được đơn hàng của bạn và đang xử lý.</p>
                        
                        <div class='order-info'>
                            <h3>Thông tin đơn hàng</h3>
                            <p><strong>Mã đơn hàng:</strong> {order.OrderCode}</p>
                            <p><strong>Ngày đặt:</strong> {order.CreatedAt:dd/MM/yyyy HH:mm}</p>
                            <p><strong>Trạng thái:</strong> {order.StatusText}</p>
                        </div>
                        
                        <div class='order-info'>
                            <h3>Thông tin giao hàng</h3>
                            <p><strong>Người nhận:</strong> {order.CustomerName}</p>
                            <p><strong>Số điện thoại:</strong> {order.Phone}</p>
                            <p><strong>Địa chỉ:</strong> {order.Address}</p>
                            {(!string.IsNullOrEmpty(order.Note) ? $"<p><strong>Ghi chú:</strong> {order.Note}</p>" : "")}
                        </div>
                        
                        <div class='order-info'>
                            <h3>Chi tiết sản phẩm</h3>
                            {string.Join("", order.Items.Select(item => $@"
                                <div class='product-item'>
                                    <p><strong>{item.ProductName}</strong></p>
                                    <p>Số lượng: {item.Quantity} x {item.Price:N0}₫ = {item.SubTotal:N0}₫</p>
                                </div>
                            "))}
                            
                            <div class='total'>
                                <p>Tổng cộng: {order.TotalAmount:N0}₫</p>
                            </div>
                        </div>
                        
                        <p>Chúng tôi sẽ liên hệ với bạn sớm nhất để xác nhận đơn hàng.</p>
                        <p>Nếu có bất kỳ thắc mắc nào, vui lòng liên hệ với chúng tôi.</p>
                    </div>
                    
                    <div class='footer'>
                        <p>© 2024 Ecommerce Shop. All rights reserved.</p>
                        <p>Email này được gửi tự động, vui lòng không trả lời.</p>
                    </div>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(order.Email, subject, body);
    }

    public async Task SendOrderNotificationToAdminAsync(OrderResponse order)
    {
        // Email cho admin/người bán
        var adminEmail = _configuration["SmtpSettings:AdminEmail"] ?? _smtpSettings.From;

        var subject = $"[ĐơN HÀNG MỚI] #{order.OrderCode}";

        var body = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #FF5722; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 20px; background-color: #f9f9f9; }}
                    .order-info {{ background-color: white; padding: 15px; margin: 10px 0; border-radius: 5px; }}
                    .product-item {{ padding: 10px; border-bottom: 1px solid #ddd; }}
                    .total {{ font-size: 18px; font-weight: bold; color: #FF5722; margin-top: 20px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>🔔 ĐƠN HÀNG MỚI</h1>
                    </div>
                    
                    <div class='content'>
                        <p>Bạn có đơn hàng mới từ khách hàng <strong>{order.CustomerName}</strong></p>
                        
                        <div class='order-info'>
                            <h3>Thông tin đơn hàng</h3>
                            <p><strong>Mã đơn:</strong> {order.OrderCode}</p>
                            <p><strong>Ngày đặt:</strong> {order.CreatedAt:dd/MM/yyyy HH:mm}</p>
                            <p><strong>Tổng tiền:</strong> {order.TotalAmount:N0}₫</p>
                        </div>
                        
                        <div class='order-info'>
                            <h3>Thông tin khách hàng</h3>
                            <p><strong>Tên:</strong> {order.CustomerName}</p>
                            <p><strong>SĐT:</strong> {order.Phone}</p>
                            <p><strong>Email:</strong> {order.Email}</p>
                            <p><strong>Địa chỉ:</strong> {order.Address}</p>
                            {(!string.IsNullOrEmpty(order.Note) ? $"<p><strong>Ghi chú:</strong> {order.Note}</p>" : "")}
                        </div>
                        
                        <div class='order-info'>
                            <h3>Chi tiết sản phẩm</h3>
                            {string.Join("", order.Items.Select(item => $@"
                                <div class='product-item'>
                                    <p><strong>{item.ProductName}</strong></p>
                                    <p>SL: {item.Quantity} x {item.Price:N0}₫ = {item.SubTotal:N0}₫</p>
                                </div>
                            "))}
                            
                            <div class='total'>
                                <p>Tổng cộng: {order.TotalAmount:N0}₫</p>
                            </div>
                        </div>
                        
                        <p style='color: red; font-weight: bold;'>⚠️ Vui lòng liên hệ khách hàng để xác nhận đơn hàng!</p>
                    </div>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(adminEmail, subject, body);
    }

    private async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.From));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            Console.WriteLine($"✓ Email sent to {to}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Failed to send email to {to}: {ex.Message}");
            throw;
        }
    }
}

// Helper class for SMTP settings
public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}