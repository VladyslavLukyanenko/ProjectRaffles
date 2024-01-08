// ﻿using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Globalization;
// using System.Linq;
// using System.Reflection;
// using System.Windows.Data;
//
// using Resolff.Snkrs.Core.Model;
//
// namespace Resolff.Snkrs.WpfUI.Infra.Converters
// {
//     public class TaskStatusToDisplayValueConverter
//         : IValueConverter
//     {
//         public static readonly TaskStatusToDisplayValueConverter Instance = new TaskStatusToDisplayValueConverter();
//
//         private static readonly IDictionary<TaskStatus, string> DisplayNames = Enum.GetValues(typeof(TaskStatus))
//             .OfType<TaskStatus>()
//             .Select(e =>
//             {
//                 string displayName = GetAttribute<EnumDisplayValueAttribute>(e)?.Value;
//                 return new
//                 {
//                     Value = e,
//                     DisplayName = displayName
//                 };
//             })
//             .ToDictionary(_ => _.Value, _ => _.DisplayName);
//
//         public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//         {
//             if (value is TaskStatus status)
//             {
//                 return DisplayNames[status];
//             }
//
//             return null;
//         }
//
//         public static TAttribute GetAttribute<TAttribute>(Enum value)
//             where TAttribute : Attribute
//         {
//             var enumType = value.GetType();
//             var name = Enum.GetName(enumType, value);
//             return enumType.GetField(name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
//         }
//
//         public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//         {
//             throw new NotImplementedException();
//         }
//     }
// }