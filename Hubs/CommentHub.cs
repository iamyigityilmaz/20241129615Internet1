using Microsoft.AspNetCore.SignalR;

namespace HaberPortali2.Hubs
{
    public class CommentHub : Hub
    {
        public async Task SendComment(string userName, string text, int newsId)
        {
            await Clients.All.SendAsync("ReceiveComment", userName, text, newsId);
        }
    }
}
