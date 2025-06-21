using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using ToDoList.Models;

namespace ToDoList.Infrastructure
{
    public class CustomDateTimeModelBinder : IModelBinder
    {
        private readonly IModelBinder fallbackBinder;

        public CustomDateTimeModelBinder(IModelBinder fallbackBinder)
        {
            this.fallbackBinder = fallbackBinder;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var idPartValue = bindingContext.ValueProvider.GetValue("Id").FirstValue;
            var lablePartValue = bindingContext.ValueProvider.GetValue("Lable").FirstValue;

            if (string.IsNullOrEmpty(lablePartValue))
                return Task.CompletedTask;

            var statusPartValue = bindingContext.ValueProvider.GetValue("Status").FirstValue;
            var priorityPartValue = bindingContext.ValueProvider.GetValue("Priority").FirstValue;
            var descriptionPartValue = bindingContext.ValueProvider.GetValue("Description").FirstValue;
            var userIdPartValue = bindingContext.ValueProvider.GetValue("UserId").FirstValue;

            var datePartValue = bindingContext.ValueProvider.GetValue("Date");
            var timePartValue = bindingContext.ValueProvider.GetValue("Time");
            var expiresDatePartValue = bindingContext.ValueProvider.GetValue("ExpiresDate");
            DateTime? dateTime = null;

            if (string.IsNullOrEmpty(datePartValue.FirstValue) == false)
            {
                string? date = datePartValue.FirstValue;
                string? time = timePartValue.FirstValue;

                DateTime.TryParse(date, out var parsedDateValue);
                DateTime.TryParse(time, out var parsedTimeValue);

                dateTime = new DateTime(parsedDateValue.Year,
                                parsedDateValue.Month,
                                parsedDateValue.Day,
                                parsedTimeValue.Hour,
                                parsedTimeValue.Minute,
                                parsedTimeValue.Second);
            }
            else if (string.IsNullOrEmpty(expiresDatePartValue.FirstValue) == false)
            {
                string? expiresDate = expiresDatePartValue.FirstValue;
                string pattern = "MM/dd/yyyy HH:mm:ss";
                var date = DateTime.ParseExact(expiresDate, pattern, CultureInfo.InvariantCulture);

                if (DateTime.TryParseExact(expiresDate, pattern, null, DateTimeStyles.None, out var parsedDateTimeValue))
                    Console.WriteLine("Converted '{0}' to {1} ({2}).", expiresDate,
                                      parsedDateTimeValue, parsedDateTimeValue.Kind);
                else
                    Console.WriteLine("Unable to parse '{0}'.", expiresDate);

                dateTime = parsedDateTimeValue;
            }

            var task = new TaskModel
            {
                Id = idPartValue,
                Lable = lablePartValue,
                ExpiresDate = dateTime,
                Status = Enum.Parse<Status>(statusPartValue),
                Priority = Enum.Parse<Priority>(priorityPartValue),
                Description = descriptionPartValue,
                UserId = userIdPartValue,
            };

            bindingContext.Result = ModelBindingResult.Success(task);
            return Task.CompletedTask;
        }
    }
}
