﻿using System.Collections.Generic;

namespace NetworkService.Assets
{
    public class ValidationErrors : BindableBase
    {
        private readonly Dictionary<string, string> validationErrors = new Dictionary<string, string>();

        public bool IsValid
        {
            get
            {
                return this.validationErrors.Count < 1;
            }
        }

        public string this[string fieldName]
        {
            get
            {
                return this.validationErrors.ContainsKey(fieldName) ?
                    this.validationErrors[fieldName] : string.Empty;
            }

            set
            {
                if (this.validationErrors.ContainsKey(fieldName))
                {

                        this.validationErrors[fieldName] = value;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        this.validationErrors.Add(fieldName, value);
                    }
                }
                this.OnPropertyChanged("IsValid");
            }
        }

        public void Clear()
        {
            validationErrors.Clear();
        }
    }
}
