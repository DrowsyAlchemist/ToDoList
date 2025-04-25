using Microsoft.AspNetCore.Mvc.ModelBinding;
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
            var datePartValue = bindingContext.ValueProvider.GetValue("Date");
            var timePartValue = bindingContext.ValueProvider.GetValue("Time");
            var statusPartValue = bindingContext.ValueProvider.GetValue("Status").FirstValue;
            var priorityPartValue = bindingContext.ValueProvider.GetValue("Priority").FirstValue;
            var descriptionPartValue = bindingContext.ValueProvider.GetValue("Description").FirstValue;
            var userIdPartValue = bindingContext.ValueProvider.GetValue("UserId").FirstValue;

            DateTime dateTime;

            if (datePartValue == ValueProviderResult.None || timePartValue == ValueProviderResult.None)
            {
                datePartValue = bindingContext.ValueProvider.GetValue("ExpiresDate");
                dateTime = DateTime.Parse(datePartValue.FirstValue);
            }
            else
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
