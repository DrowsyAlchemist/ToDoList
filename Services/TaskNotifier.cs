using Microsoft.AspNetCore.SignalR;
using ToDoList.Models;
using ToDoList.SignalR;

namespace ToDoList.Services
{
    public class TaskNotifier
    {
        private readonly NotificationHub _hubContext;
        private readonly EmailService _emailService;

        public TaskNotifier(NotificationHub hubContext, EmailService emailService)
        {
            _hubContext = hubContext;
            _emailService = emailService;
        }

        public void AddNotification(TaskModel task)
        {
            ArgumentNullException.ThrowIfNull(task);
            var taskUser = task.User;

            if (taskUser == null)
                throw new ArgumentException("User of the task is null or empty.");

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

        private void SendEmailNotification(TaskModel task)
        {
            _emailService.SendEmail(
                to: task.User.LoginData.Email, 
                subject: task.Lable,
                body: $"Пора бы выполнить задачу: \"{task.Lable}\"" +
                $"\nОписание задачи: {task.Description}" +
                $"\nЗадача была запланирована на {task.ExpiresDate}");
        }
    }

}
