using System.Net;
using System.Net.Mail;

namespace FPTTelecomBE.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendRegistrationNotificationToAdminAsync(
        string registrationId,
        string customerName,
        string phone,
        string address,
        string packageName)
    {
        try
        {
            var adminEmail = _configuration["Email:RegistrationAdminEmail"];

            if (string.IsNullOrEmpty(adminEmail))
            {
                _logger.LogWarning("Email Admin đăng ký chưa được cấu hình");
                return false;
            }

            var subject = $"Đơn đăng ký mới #{registrationId} - FPT Telecom";
            var body = $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Đơn đăng ký dịch vụ mới</title>
    <style type='text/css'>
        /* Reset styles */
        body {{ margin: 0; padding: 0; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; }}
        table {{ border-collapse: collapse; }}
        img {{ border: 0; max-width: 100%; height: auto; }}

        /* Main styles */
        .body {{ background-color: #f4f4f9; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; }}
        .container {{ max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.08); }}
        .header {{ background: linear-gradient(135deg, #e63946, #d00000); color: white; padding: 40px 30px 30px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 28px; font-weight: 600; letter-spacing: 1px; }}
        .content {{ padding: 35px 30px; }}
        .info-table {{ width: 100%; margin: 20px 0; }}
        .info-table td {{ padding: 14px 0; border-bottom: 1px solid #eee; }}
        .label {{ width: 140px; font-weight: 600; color: #e63946; vertical-align: top; }}
        .value {{ color: #333; }}
        .alert-box {{ background: #fff8e1; border-left: 5px solid #ffb300; padding: 20px; margin: 25px 0; border-radius: 6px; font-size: 15px; color: #5c3c00; }}
        .button {{ display: inline-block; background: #e63946; color: white !important; padding: 14px 32px; text-decoration: none; border-radius: 50px; font-weight: 600; margin: 20px 0; }}
        .button:hover {{ background: #d00000; }}
        .footer {{ background: #f8f9fa; padding: 25px 30px; text-align: center; color: #666; font-size: 13px; border-top: 1px solid #eee; }}
        .footer a {{ color: #e63946; text-decoration: none; }}

        /* Responsive */
        @media only screen and (max-width: 600px) {{
            .container {{ width: 100% !important; border-radius: 0; }}
            .content, .header, .footer {{ padding-left: 20px !important; padding-right: 20px !important; }}
            .label {{ width: 100% !important; display: block; margin-bottom: 5px; }}
            .info-table td {{ display: block !important; padding: 12px 0 !important; }}
            .header h1 {{ font-size: 24px !important; }}
        }}
    </style>
</head>
<body class='body'>
    <table width='100%' cellpadding='0' cellspacing='0' role='presentation'>
        <tr>
            <td align='center' style='padding: 30px 10px; background: #f4f4f9;'>
                <table class='container' cellpadding='0' cellspacing='0' role='presentation'>
                    <tr>
                        <td class='header'>
                            <h1>ĐƠN ĐĂNG KÝ DỊCH VỤ MỚI</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class='content'>
                            <p style='font-size: 16px; margin: 0 0 25px; color: #444;'>
                                Có một đơn đăng ký dịch vụ mới vừa được gửi. Vui lòng xử lý sớm.
                            </p>

                            <table class='info-table' cellpadding='0' cellspacing='0' role='presentation'>
                                <tr>
                                    <td class='label'>Mã đơn:</td>
                                    <td class='value'><strong>#{registrationId}</strong></td>
                                </tr>
                                <tr>
                                    <td class='label'>Khách hàng:</td>
                                    <td class='value'>{customerName}</td>
                                </tr>
                                <tr>
                                    <td class='label'>Số điện thoại:</td>
                                    <td class='value'>{phone}</td>
                                </tr>
                                <tr>
                                    <td class='label'>Địa chỉ:</td>
                                    <td class='value'>{address}</td>
                                </tr>
                                <tr>
                                    <td class='label'>Gói cước:</td>
                                    <td class='value'><strong>{packageName}</strong></td>
                                </tr>
                                <tr>
                                    <td class='label'>Thời gian:</td>
                                    <td class='value'>{DateTime.Now:dd/MM/yyyy HH:mm:ss}</td>
                                </tr>
                            </table>

                            <div class='alert-box'>
                                <strong>Lưu ý:</strong> Vui lòng liên hệ khách hàng trong vòng <strong>24 giờ</strong> để xác nhận thông tin và tư vấn chi tiết.
                            </div>

                            <div style='text-align: center;'>
                                <a href='tel:{phone}' class='button'>Liên hệ khách hàng ngay</a>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class='footer'>
                            © {DateTime.Now.Year} FPT Telecom Bình Định<br>
                            Email được gửi tự động từ hệ thống • Không trả lời email này<br>
                            <a href='#'>Xem chính sách bảo mật</a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>
";

            return await SendEmailAsync(adminEmail, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi gửi email thông báo đăng ký cho Admin");
            return false;
        }
    }

    public async Task<bool> SendJobApplicationNotificationToAdminAsync(
        string applicationId,
        string candidateName,
        string email,
        string phone,
        string position,
        string jobTitle)
    {
        try
        {
            var adminEmail = _configuration["Email:RecruitmentAdminEmail"];

            if (string.IsNullOrEmpty(adminEmail))
            {
                _logger.LogWarning("Email Admin tuyển dụng chưa được cấu hình");
                return false;
            }
            // LẤY URL DASHBOARD
            var dashboardUrl = _configuration["AppSettings:AdminDashboardUrl"] ?? "http://localhost:3000/admin";
            var applicationDetailUrl = $"{dashboardUrl}/job-applications/{applicationId}";
            var subject = $"Hồ sơ ứng tuyển mới #{applicationId} - {position}";
            var body = $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Hồ sơ ứng tuyển mới</title>
    <style type='text/css'>
        /* Reset */
        body {{ margin: 0; padding: 0; -webkit-font-smoothing: antialiased; background-color: #f5f7fa; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; }}
        table {{ border-collapse: collapse; }}
        img {{ border: 0; max-width: 100%; height: auto; }}

        /* Layout */
        .body {{ background-color: #f5f7fa; }}
        .container {{ max-width: 620px; margin: 20px auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 6px 18px rgba(0,0,0,0.08); }}
        .header {{ background: linear-gradient(135deg, #1e88e5, #1565c0); color: white; padding: 45px 30px 35px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 28px; font-weight: 600; letter-spacing: 0.5px; }}
        .content {{ padding: 40px 35px 30px; }}
        .info-table {{ width: 100%; margin: 25px 0 30px; }}
        .info-table td {{ padding: 16px 0; border-bottom: 1px solid #e8ecef; vertical-align: top; }}
        .label {{ width: 160px; font-weight: 600; color: #1e88e5; padding-right: 15px; }}
        .value {{ color: #2c3e50; }}
        .highlight {{ background: #e3f2fd; border-left: 5px solid #1e88e5; padding: 20px; margin: 30px 0; border-radius: 8px; font-size: 15px; color: #1a3c6d; }}
        .button {{ display: inline-block; background: #1e88e5; color: white !important; padding: 14px 38px; text-decoration: none; border-radius: 50px; font-size: 16px; font-weight: 600; margin: 25px 0; transition: background 0.3s; }}
        .button:hover {{ background: #1565c0; }}
        .footer {{ background: #f8f9fa; padding: 30px 35px; text-align: center; color: #6c757d; font-size: 13px; border-top: 1px solid #e9ecef; }}
        .footer a {{ color: #1e88e5; text-decoration: none; }}

        /* Responsive */
        @media only screen and (max-width: 620px) {{
            .container {{ width: 100% !important; border-radius: 0; margin: 0; }}
            .header, .content, .footer {{ padding-left: 25px !important; padding-right: 25px !important; }}
            .label {{ width: 100% !important; display: block; margin-bottom: 6px; }}
            .info-table td {{ display: block !important; padding: 14px 0 !important; }}
            .header h1 {{ font-size: 24px !important; }}
            .button {{ width: 100% !important; text-align: center; box-sizing: border-box; }}
        }}
    </style>
</head>
<body class='body'>
    <table width='100%' cellpadding='0' cellspacing='0' role='presentation'>
        <tr>
            <td align='center' style='padding: 20px 10px; background: #f5f7fa;'>
                <table class='container' cellpadding='0' cellspacing='0' role='presentation'>
                    <tr>
                        <td class='header'>
                            <h1>HỒ SƠ ỨNG TUYỂN MỚI</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class='content'>
                            <p style='font-size: 16px; margin: 0 0 25px; color: #444; line-height: 1.6;'>
                                Một hồ sơ ứng tuyển mới vừa được nộp. Vui lòng xem xét và xử lý sớm.
                            </p>

                            <table class='info-table' cellpadding='0' cellspacing='0' role='presentation'>
                                <tr>
                                    <td class='label'>Mã hồ sơ:</td>
                                    <td class='value'><strong>#{applicationId}</strong></td>
                                </tr>
                                <tr>
                                    <td class='label'>Tin tuyển dụng:</td>
                                    <td class='value'>{jobTitle}</td>
                                </tr>
                                <tr>
                                    <td class='label'>Vị trí:</td>
                                    <td class='value'><strong>{position}</strong></td>
                                </tr>
                                <tr>
                                    <td class='label'>Ứng viên:</td>
                                    <td class='value'>{candidateName}</td>
                                </tr>
                                <tr>
                                    <td class='label'>Email:</td>
                                    <td class='value'><a href='mailto:{email}' style='color: #1e88e5; text-decoration: none;'>{email}</a></td>
                                </tr>
                                <tr>
                                    <td class='label'>Số điện thoại:</td>
                                    <td class='value'><a href='tel:{phone}' style='color: #1e88e5; text-decoration: none;'>{phone}</a></td>
                                </tr>
                                <tr>
                                    <td class='label'>Thời gian nộp:</td>
                                    <td class='value'>{DateTime.Now:dd/MM/yyyy HH:mm:ss}</td>
                                </tr>
                            </table>

                            <div class='highlight'>
                                <strong>Hành động cần thực hiện:</strong> Vui lòng truy cập hệ thống để xem chi tiết hồ sơ, CV và các tài liệu đính kèm của ứng viên.
                            </div>

                            <div style='text-align: center;'>
                                <a href='{applicationDetailUrl}' class='button'>Xem hồ sơ ngay</a>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class='footer'>
                            © {DateTime.Now.Year} FPT Telecom Bình Định - Phòng Nhân Sự<br>
                            Email được gửi tự động từ hệ thống • Không trả lời trực tiếp email này<br>
                            <a href='#'>Quản lý thông báo</a> • <a href='#'>Chính sách bảo mật</a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>
";

            return await SendEmailAsync(adminEmail, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi gửi email thông báo ứng tuyển cho Admin");
            return false;
        }
    }

    private async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var senderEmail = _configuration["Email:SenderEmail"];
            var senderPassword = _configuration["Email:SenderPassword"];
            var senderName = _configuration["Email:SenderName"] ?? "FPT Telecom Bình Định";

            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
            {
                _logger.LogWarning("Email SMTP chưa được cấu hình. Email không được gửi.");
                _logger.LogInformation("[MOCK] Gửi email đến {Email} - Subject: {Subject}", toEmail, subject);
                return true; // Mock mode
            }

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                Timeout = 30000
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                Priority = MailPriority.High
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);

            _logger.LogInformation("Email gửi thành công đến {Email}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi gửi email đến {Email}", toEmail);
            return false;
        }
    }
}