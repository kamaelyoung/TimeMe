﻿using System.Windows;
using System.Windows.Controls;

namespace TimeMe
{
    /// <summary>
    /// Supports a PropertyChanged-Trigger for DataBindings
    /// in Silverlight. Works just for TextBoxes
    /// (C) Thomas Claudius Huber 2009
    /// http://www.thomasclaudiushuber.com
    /// </summary>
    public class TextBlockObservableBinder
    {
        public static bool GetUpdateSourceOnChange
          (DependencyObject obj)
        {
            return (bool)obj.GetValue(UpdateSourceOnChangeProperty);
        }

        public static void SetUpdateSourceOnChange
          (DependencyObject obj, bool value)
        {
            obj.SetValue(UpdateSourceOnChangeProperty, value);
        }

        // Using a DependencyProperty as the backing store for …
        public static readonly DependencyProperty
          UpdateSourceOnChangeProperty =
            DependencyProperty.RegisterAttached(
            "UpdateSourceOnChange",
            typeof(bool),
            typeof(TextBlockObservableBinder),
            new PropertyMetadata(false, OnPropertyChanged));

        private static void OnPropertyChanged
          (DependencyObject obj,
          DependencyPropertyChangedEventArgs e)
        {
            var txt = obj as TextBox;
            if (txt == null)
                return;
            if ((bool)e.NewValue)
            {
                txt.TextChanged += OnTextChanged;
            }
            else
            {
                txt.TextChanged -= OnTextChanged;
            }
        }
        static void OnTextChanged(object sender,
          TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt == null)
                return;
            var be = txt.GetBindingExpression(TextBox.TextProperty);
            if (be != null)
            {
                be.UpdateSource();
            }
        }
    }
}
