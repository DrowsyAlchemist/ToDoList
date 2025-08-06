using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using ToDoList.DataBase;
using ToDoList.Logger;
using ToDoList.Models;
using ToDoList.SignalR;

namespace ToDoList.Services
{
    public class TaskNotifier : BackgroundService
    {
        private readonly NotificationHub _hubContext;
        private readonly EmailService _emailService;
        private readonly IServiceProvider _services;
        private readonly AppLogger _logger;


        private static List<TaskModel> _notificationsQueue = new List<TaskModel>();

        public TaskNotifier(NotificationHub hubContext,
            EmailService emailService,
            IServiceProvider services,
            AppLogger logger)
        {
            _hubContext = hubContext;
            _emailService = emailService;
            _logger = logger;
            _services = services;
            UpdateNotificationsQueue();
            _ = ExecuteAsync(new CancellationToken());
        }

        public void AddNotification(TaskModel task)
        {
            ArgumentNullException.ThrowIfNull(task);
            var taskUser = task.User;

            if (taskUser == null)
                throw new ArgumentException("User of the task is null or empty.");

            bool isInserted = false;

            foreach (var notification in _notificationsQueue)
            {
                if (task.ExpiresDate < notification.ExpiresDate)
                {
                    int index = _notificationsQueue.IndexOf(notification);
                    _notificationsQueue.Insert(index, task);
                    isInserted = true;
                    break;
                }
            }
            if (isInserted == false)
                _notificationsQueue.Add(task);

        }

        public void UpdateNotificationsQueue()
        {
            using (var scope = _services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                _notificationsQueue = dbContext.Tasks
                .Where(t => t.ExpiresDate > DateTime.Now)
                .Include(t => t.User)
                .OrderBy(t => t.ExpiresDate)
                .ToList();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var nextTaskForNotification = _notificationsQueue.FirstOrDefault();

                if (nextTaskForNotification != null && nextTaskForNotification.ExpiresDate <= DateTime.Now)
                {
                    try
                    {
                        _logger.LogInfo($"Try send <{nextTaskForNotification.Lable}>");
                        await SendNotification(nextTaskForNotification)
                            .ContinueWith((_) => _notificationsQueue.Remove(nextTaskForNotification));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                }
                else
                {
                    await Task.Delay(3000, stoppingToken);
                }
            }
        }

        private async Task SendNotification(TaskModel task)
        {
            ArgumentNullException.ThrowIfNull(task);
            var taskUser = task.User;

            if (taskUser == null)
                throw new ArgumentException("User of the task is null or empty.");

            if (_hubContext.IsUserOnline(task.User.LoginData.Email))
                await SendBrowserNotification(task);
            else
                await SendEmailNotification(task);
        }

        private async Task SendBrowserNotification(TaskModel task)
        {
            var user = _hubContext.Clients.User(task.User.LoginData.Email);
            await user
                .SendAsync("ReceiveNotification",
                $"Время выполнить задачу: {task.Lable}",
                task.Id);
            _logger.LogInfo($"BrowserNotification sent <{task.Lable}>");
        }

        private async Task SendEmailNotification(TaskModel task)
        {
            await _emailService.SendEmail(
                to: task.User.LoginData.Email,
                subject: task.Lable,
                body: $"Пора бы выполнить задачу: \"{task.Lable}\"" +
                $"\nОписание задачи: {task.Description}" +
                $"\nЗадача была запланирована на {task.ExpiresDate}");
            _logger.LogInfo($"EmailNotification sent <{task.Lable}>");
        }
    }

}
