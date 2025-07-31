using Microsoft.AspNetCore.SignalR;
using ToDoList.Models;
using ToDoList.SignalR;

namespace ToDoList.Services
{
    public class TaskNotifier
    {
        private readonly NotificationHub _hubContext;
        
        public TaskNotifier(NotificationHub hubContext)
        {
            _hubContext = hubContext;
        }

        public void AddNotification(TaskModel task)
        {
            var delay = task.ExpiresDate - DateTime.Now;

            if (delay > TimeSpan.Zero)
            {
                _ = Task.Delay(delay).ContinueWith(async (_) =>
                {
                    if (_hubContext.IsUserOnline(task.User.LoginData.Email))
                        await SendBrowserNotification(task);
                    else
                        SendEmailNotification(task);
                });
            }
        }

        private async Task SendBrowserNotification(TaskModel task)
        {
            var user = _hubContext.Clients.User(task.User.LoginData.Email);
            await user
                .SendAsync("ReceiveNotification",
                $"Время выполнить задачу: {task.Lable}",
                task.Id);
        }

        private async Task SendEmailNotification(TaskModel task)
        {
            throw new NotImplementedException();
        }
    }

}
