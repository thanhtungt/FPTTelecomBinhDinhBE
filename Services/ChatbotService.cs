namespace FPTTelecomBE.Services;

public class ChatbotService : IChatbotService
{
    private readonly ILogger<ChatbotService> _logger;

    public ChatbotService(ILogger<ChatbotService> logger)
    {
        _logger = logger;
    }

    private readonly Dictionary<string, string> _faqResponses = new()
    {
        { "giá cước", "**Bảng giá các gói cước Internet FPT:**\n\n" +
                      "• Gói 50Mbps: **165.000đ/tháng**\n" +
                      "• Gói 100Mbps: **215.000đ/tháng**\n" +
                      "• Gói 150Mbps: **265.000đ/tháng**\n" +
                      "• Gói 200Mbps: **330.000đ/tháng**\n\n" +
                      "Tất cả các gói đều bao gồm:\n" +
                      "✓ Miễn phí lắp đặt\n" +
                      "✓ Tặng Modem WiFi 6\n" +
                      "✓ Hỗ trợ 24/7\n\n" +
                      "Nhập **'đăng ký'** để được tư vấn chi tiết hoặc **'nhân viên'** để chat trực tiếp!" },

        { "khuyến mãi", "🎁 **Khuyến mãi HOT tháng này:**\n\n" +
                        "🔥 Tặng 2 THÁNG CƯỚC khi đăng ký 12 tháng\n" +
                        "🔥 Miễn phí 100% chi phí lắp đặt\n" +
                        "🔥 Tặng Modem WiFi 6 chuẩn AX (trị giá 1.2 triệu)\n" +
                        "🔥 Tặng thêm 1 tháng khi giới thiệu bạn bè\n\n" +
                        "⏰ Chương trình có hạn!\n\n" +
                        "Nhập **'đăng ký'** ngay để được tư vấn!" },

        { "tốc độ", "⚡ **Tư vấn chọn gói theo nhu cầu:**\n\n" +
                    "📱 **50Mbps** - Phù hợp 2-3 người, lướt web, xem phim HD\n" +
                    "💻 **100Mbps** - Gia đình 4-6 người, làm việc online, học trực tuyến\n" +
                    "🎮 **150Mbps** - Gaming, streaming, nhiều thiết bị cùng lúc\n" +
                    "🚀 **200Mbps+** - Công ty, văn phòng, nhu cầu cao\n\n" +
                    "Bạn có bao nhiêu người sử dụng? Mình sẽ tư vấn gói phù hợp!" },

        { "lắp đặt", "🔧 **Quy trình lắp đặt nhanh chóng:**\n\n" +
                     "1️⃣ Đăng ký online (2 phút)\n" +
                     "2️⃣ Nhân viên liên hệ khảo sát (trong 2h)\n" +
                     "3️⃣ Kỹ thuật lắp đặt tận nhà (24-48h)\n" +
                     "4️⃣ Hoàn tất, sử dụng ngay!\n\n" +
                     "✓ Miễn phí khảo sát\n" +
                     "✓ Cam kết đúng hẹn\n" +
                     "✓ Bảo hành 12 tháng\n\n" +
                     "Nhập **'đăng ký'** để bắt đầu!" },

        { "đăng ký", "📝 **Đăng ký dịch vụ FPT Telecom:**\n\n" +
                     "Bạn vui lòng cung cấp:\n" +
                     "• Họ tên\n" +
                     "• Số điện thoại\n" +
                     "• Địa chỉ lắp đặt\n" +
                     "• Gói cước mong muốn\n\n" +
                     "Hoặc nhập **'nhân viên'** để được tư vấn viên hỗ trợ trực tiếp!" },

        { "thanh toán", "💳 **Các hình thức thanh toán:**\n\n" +
                        "✓ Chuyển khoản ngân hàng\n" +
                        "✓ Ví điện tử (Momo, ZaloPay, VNPay)\n" +
                        "✓ Thẻ tín dụng/ghi nợ\n" +
                        "✓ Tiền mặt (thu hộ tại nhà)\n\n" +
                        "Nhập **'nhân viên'** để được hướng dẫn chi tiết!" },

        { "hỗ trợ", "🆘 **Trung tâm hỗ trợ 24/7:**\n\n" +
                    "📞 Hotline: **1900 xxxx**\n" +
                    "📧 Email: support@fptbinhdinh.com\n" +
                    "💬 Chat: Bạn đang chat đây 😊\n\n" +
                    "Nhập **'nhân viên'** để kết nối với tư vấn viên!" }
    };

    public string GetWelcomeMessage(string? userName)
    {
        var name = !string.IsNullOrEmpty(userName) ? userName : "bạn";
        return $"👋 **Xin chào {name}!**\n\n" +
               "Mình là **FPT Bot** - trợ lý ảo của FPT Telecom Bình Định.\n" +
               "Mình có thể giúp bạn với:\n\n" +
               "💰 **Giá cước** - Xem bảng giá các gói\n" +
               "🎁 **Khuyến mãi** - Ưu đãi đặc biệt\n" +
               "⚡ **Tốc độ** - Tư vấn gói phù hợp\n" +
               "🔧 **Lắp đặt** - Quy trình & thời gian\n" +
               "📝 **Đăng ký** - Đăng ký dịch vụ\n\n" +
               "Bạn muốn tìm hiểu về vấn đề gì? Hoặc nhập **'nhân viên'** để chat trực tiếp với tư vấn viên! 😊";
    }

    public string? GetAutoReply(string userMessage)
    {
        var lowerMessage = userMessage.ToLower().Trim();

        _logger.LogInformation("Processing message: {Message}", lowerMessage);

        // Greeting
        if (lowerMessage.Contains("xin chào") ||
            lowerMessage.Contains("hello") ||
            lowerMessage.Contains("hi") ||
            lowerMessage.Contains("chào") ||
            lowerMessage == "alo")
        {
            return GetWelcomeMessage(null);
        }

        // Check FAQ responses
        foreach (var faq in _faqResponses)
        {
            if (lowerMessage.Contains(faq.Key))
            {
                return faq.Value;
            }
        }

        // Các từ khóa liên quan đến giá
        if (lowerMessage.Contains("giá") ||
            lowerMessage.Contains("bao nhiêu") ||
            lowerMessage.Contains("phí"))
        {
            return _faqResponses["giá cước"];
        }

        // Các từ khóa liên quan tốc độ
        if (lowerMessage.Contains("nhanh") ||
            lowerMessage.Contains("chậm") ||
            lowerMessage.Contains("mbps"))
        {
            return _faqResponses["tốc độ"];
        }

        // Không tìm thấy câu trả lời phù hợp
        return "🤔 Mình chưa hiểu rõ câu hỏi của bạn.\n\n" +
               "Bạn có thể hỏi về: **giá cước**, **khuyến mãi**, **tốc độ**, **lắp đặt**\n\n" +
               "Hoặc nhập **'nhân viên'** để được tư vấn viên hỗ trợ trực tiếp nhé! 😊";
    }

    public bool IsRequestingStaff(string message)
    {
        var lowerMessage = message.ToLower().Trim();

        var staffKeywords = new[]
        {
            "nhân viên",
            "nhan vien",
            "tư vấn",
            "tu van",
            "tư vấn viên",
            "tvv",
            "staff",
            "admin",
            "người",
            "người thật",
            "con người",
            "kết nối",
            "ket noi",
            "gặp nhân viên",
            "gặp tư vấn",
            "chat với",
            "nói chuyện với",
            "support"
        };

        var isRequesting = staffKeywords.Any(keyword => lowerMessage.Contains(keyword));

        if (isRequesting)
        {
            _logger.LogInformation("User is requesting staff connection");
        }

        return isRequesting;
    }
}