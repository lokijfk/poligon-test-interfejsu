﻿using System.Globalization;
using System.Windows.Controls;

namespace poligon_inter.View.Converters;

public class NotEmptyValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        return string.IsNullOrWhiteSpace((value ?? "").ToString())
            ? new ValidationResult(false, "Pole jest wymagane.")
            : ValidationResult.ValidResult;
    }
}
