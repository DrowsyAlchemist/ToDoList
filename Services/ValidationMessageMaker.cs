using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ToDoList.Services
{
    public class ValidationMessageMaker
    {
        public string GetValidationErrorMessage(ModelStateDictionary modelState)
        {
            string errorMessages = "";

            foreach (var item in modelState)
                if (item.Value.ValidationState == ModelValidationState.Invalid)
                    foreach (var error in item.Value.Errors)
                        errorMessages = $"{errorMessages}{error.ErrorMessage}\n";

            return errorMessages;
        }
    }
}
