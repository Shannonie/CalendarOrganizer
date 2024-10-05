using CalendarOrganizer.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CalendarOrganizer.UI.Wrapper
{
    public class ContactWrapper : ModelWrapper<Contact>
    {
        public ContactWrapper(Contact model) : base(model)
        { }

        public string ContactInfo
        {
            get
            {
                string name = GetValue<string>(nameof(Name)).Trim();
                string phoneNumber = GetValue<string>(nameof(PhoneNumber)).Trim();
                return !string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(phoneNumber) ?
                    $"{name},{phoneNumber}" : "";
            }
            set
            {
                string[] infos = value.Split(',');
                string phoneNumber = infos.Length == 2 ? infos[1] : "";
                SetValue(infos[0], nameof(Name));
                SetValue(phoneNumber, nameof(PhoneNumber));
            }
        }

        public string Name
        {
            get
            {
                return GetValue<string>();
            }
            set
            {
                SetValue(value);
            }
        }

        public string PhoneNumber
        {
            get
            {
                return GetValue<string>();
            }
            set
            {
                SetValue(value);
            }
        }

        protected override void ValidateDataAnnotations(string propertyName, object currentValue)
        {
            string proName = nameof(ContactInfo);
            if (propertyName == nameof(Name))
            {
                ClearErrors(proName);
            }
            ValidationContext context = new ValidationContext(Model) { MemberName = propertyName };
            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateProperty(currentValue, context, validationResults);
            foreach (ValidationResult validationResult in validationResults)
            {
                AddError(proName, validationResult.ErrorMessage);
            }
        }
    }
}
