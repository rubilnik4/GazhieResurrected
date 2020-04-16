﻿using GadzhiCommon.Models.Implementations.Errors;
using GadzhiCommon.Models.Interfaces.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GadzhiCommon.Extentions.Functional
{
    /// <summary>
    /// Методы расширения для преобразования типов
    /// </summary>
    public static class MapAyncExtensions
    {
        /// <summary>
        /// Преобразование типов с помощью функции
        /// </summary>       
        public static Task<TResult> MapAsync<TSource, TResult>(this TSource @this, Func<TSource, Task<TResult>> func) =>
            func != null ?
            func(@this) :
            throw new ArgumentNullException(nameof(func));

        /// <summary>
        /// Преобразование типов с помощью функции
        /// </summary>       
        public static async Task<TResult> MapAsync<TSource, TResult>(this Task<TSource> @this, Func<TSource, TResult> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            var awaited = await @this;
            return func.Invoke(awaited);
        }

        /// <summary>
        /// Связывание типов с помощью функции
        /// </summary>       
        public static async Task<TResult> BindAsync<TSource, TResult>(this Task<TSource> @this, Func<TSource, Task<TResult>> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            var awaited = await @this;
            return await func.Invoke(awaited);
        }

        /// <summary>
        /// Выполнить действие, вернуть тот же тип
        /// </summary>       
        public static async Task<T> VoidAsync<T>(this Task<T> @this, Action<T> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            var awaited = await @this;
            action.Invoke(awaited);
            return awaited;
        }
    }
}
