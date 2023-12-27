using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SoR4_Studio.Modules.Windows.Helpers
{
    // source: https://gist.github.com/OlsonAndrewD/5623505
    internal static class DynamicStringFormatHelper
    {
        #region Value

        private static DependencyProperty valueProperty = DependencyProperty.RegisterAttached(
            "Value", typeof(object), typeof(DynamicStringFormatHelper), new System.Windows.PropertyMetadata(null, OnValueChanged));

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            RefreshFormattedValue(obj);
        }

        public static object GetValue(DependencyObject obj)
        {
            return obj.GetValue(valueProperty);
        }

        public static void SetValue(DependencyObject obj, object newValue)
        {
            obj.SetValue(valueProperty, newValue);
        }

        #endregion

        #region Format

        private static DependencyProperty formatProperty = DependencyProperty.RegisterAttached(
            "Format", typeof(string), typeof(DynamicStringFormatHelper), new System.Windows.PropertyMetadata(null, OnFormatChanged));

        private static void OnFormatChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            RefreshFormattedValue(obj);
        }

        public static string GetFormat(DependencyObject obj)
        {
            return (string)obj.GetValue(formatProperty);
        }

        public static void SetFormat(DependencyObject obj, string newFormat)
        {
            obj.SetValue(formatProperty, newFormat);
        }

        #endregion

        #region FormattedValue

        private static DependencyProperty formattedValueProperty = DependencyProperty.RegisterAttached(
            "FormattedValue", typeof(string), typeof(DynamicStringFormatHelper), new System.Windows.PropertyMetadata(null));

        public static string GetFormattedValue(DependencyObject obj)
        {
            return (string)obj.GetValue(formattedValueProperty);
        }

        public static void SetFormattedValue(DependencyObject obj, string newFormattedValue)
        {
            obj.SetValue(formattedValueProperty, newFormattedValue);
        }

        #endregion

        private static void RefreshFormattedValue(DependencyObject obj)
        {
            var value = GetValue(obj);
            var format = GetFormat(obj);

            if (format != null)
            {
                SetFormattedValue(obj, string.Format(format, value));
            }
            else
            {
                SetFormattedValue(obj, value == null ? string.Empty : (value.ToString() ?? string.Empty));
            }
        }
    }
}
