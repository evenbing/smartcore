using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace SmartCore.Validator
{
    public class CustomBadRequest:ValidationProblemDetails
    {
        public CustomBadRequest(ActionContext context)
        { 
        
        Title ="";
            Detail="";
            Status=400;
            ConstructErrorMessage(context);
            Type = context.HttpContext.TraceIdentifier;
        }

        private void ConstructErrorMessage(ActionContext context)
        {
            foreach (var keyModelStatePair in context.ModelState)
            {
                var key = keyModelStatePair.Key;
                var errors = keyModelStatePair.Value.Errors;
                if (errors != null && errors.Count > 0)
                {
                    if (errors.Count == 1)
                    {
                        var errorMessage = GetErrorMessage(errors[0]);
                        Errors.Add(key, new[] { errorMessage });
                    }
                    else
                    {
                        var errorMessages = new string[errors.Count];
                        for (int i = 0; i < errors.Count; i++)
                        {
                            errorMessages[i] = GetErrorMessage(errors[i]);
                        }
                        Errors.Add(key, errorMessages);
                    }
                }
            }
        }
        private string GetErrorMessage(ModelError error)
        {
            return string.IsNullOrEmpty(error.ErrorMessage) ? "The input was not valid." : error.ErrorMessage;
        }
    }
}
