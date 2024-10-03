using Microsoft.AspNetCore.SignalR;

public class CustomUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        // Lấy UserId từ Query String "userId" khi client kết nối
        connection.GetHttpContext().Request.Query.TryGetValue("userId", out var userId);

        // Trả về UserId nếu có trong Query String, ngược lại trả về null hoặc rỗng
        return userId;
    }
}