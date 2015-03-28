﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Sharpknife.Common
{
	/// <summary>
	/// Represents a value converter to convert a null value to a <see cref="Visibility" />.
	/// </summary>
	public class NullVisibilityConverter : IValueConverter
	{
		/// <summary>
		/// Converts a value to a visibility.
		/// </summary>
		/// <param name="value">the value</param>
		/// <param name="targetType">the target type</param>
		/// <param name="parameter">the parameter</param>
		/// <param name="culture">the culture</param>
		/// <returns>the result</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value == null ? Visibility.Collapsed : Visibility.Visible;
		}

		/// <summary>
		/// Converts a visibility back to a value.
		/// </summary>
		/// <param name="value">the value</param>
		/// <param name="targetType">the target type</param>
		/// <param name="parameter">the parameter</param>
		/// <param name="culture">the culture</param>
		/// <returns>the result</returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
