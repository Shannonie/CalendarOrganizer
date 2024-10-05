using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace CalendarOrganizer.UI.Wrapper
{
    public class ModelWrapper<T> : NotifyDataErrorInfoBase
    {
        public T Model { get; }

        public ModelWrapper(T model)
        {
            Model = model;
        }

        protected virtual TValue GetValue<TValue>([CallerMemberName] string propertyName = null)
        {
            return (TValue)typeof(T).GetProperty(propertyName).GetValue(Model, null);
        }

        protected virtual void SetValue<TValue>(TValue value,
            [CallerMemberName] string propertyName = null)
        {
            typeof(T).GetProperty(propertyName).SetValue(Model, value, null);
            OnPropertyChanged(propertyName);
            ValidatePropertyInternal(propertyName, value);
        }

        protected virtual void ValidatePropertyInternal(
            string propertyName, object currentValue)
        {
            ClearErrors(propertyName);
            ValidateDataAnnotations(propertyName, currentValue);
            ValidateCustomErrors(propertyName);
        }

        protected virtual void ValidateDataAnnotations(string propertyName, object currentValue)
        {
            ValidationContext context = new ValidationContext(Model) { MemberName = propertyName };
            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateProperty(currentValue, context, validationResults);
            foreach (ValidationResult validationResult in validationResults)
            {
                AddError(propertyName, validationResult.ErrorMessage);
            }
        }

        private void ValidateCustomErrors(string propertyName)
        {
            IEnumerable<string> errors = ValidationProperty(propertyName);
            if (errors != null)
            {
                foreach (string error in errors)
                {
                    AddError(propertyName, error);
                }
            }
        }

        protected virtual IEnumerable<string> ValidationProperty(string propertyName)
        {
            return null;
        }
    }
}
