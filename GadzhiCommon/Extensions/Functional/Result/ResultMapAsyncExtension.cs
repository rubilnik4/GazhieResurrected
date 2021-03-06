﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GadzhiCommon.Models.Interfaces.Errors;

namespace GadzhiCommon.Extensions.Functional.Result
{
    /// <summary>
    /// Асинхронное преобразование внутреннего типа результирующего ответа
    /// </summary>
    public static class ResultMapAsyncExtension
    {
        /// <summary>
        /// Выполнить действие асинхронно, вернуть результирующий ответ
        /// </summary>      
        public static async Task<IResultValue<TValue>> ResultVoidAsync<TValue>(this Task<IResultValue<TValue>> @this, Action<TValue> action)
        {
            if (@this == null) throw new ArgumentNullException(nameof(@this));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var awaitedThis = await @this;
            action.Invoke(awaitedThis.Value);

            return awaitedThis;
        }

        /// <summary>
        /// Выполнить действие асинхронно при положительном значении, вернуть результирующий ответ
        /// </summary>      
        public static async Task<IResultValue<TValue>> ResultVoidOkAsync<TValue>(this Task<IResultValue<TValue>> @this, Action<TValue> action)
        {
            if (@this == null) throw new ArgumentNullException(nameof(@this));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var awaitedThis = await @this;
            if (awaitedThis.OkStatus) action.Invoke(awaitedThis.Value);

            return awaitedThis;
        }

        /// <summary>
        /// Выполнить действие асинхронно при отрицательном значении, вернуть результирующий ответ
        /// </summary>      
        public static async Task<IResultValue<TValue>> ResultVoidBadAsync<TValue>(this Task<IResultValue<TValue>> @this,
                                                                                  Action<IReadOnlyList<IErrorCommon>> action)
        {
            if (@this == null) throw new ArgumentNullException(nameof(@this));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var awaitedThis = await @this;
            if (awaitedThis.HasErrors) action.Invoke(awaitedThis.Errors);

            return awaitedThis;
        }

        /// <summary>
        /// Выполнить действие асинхронно, вернуть результирующий ответ
        /// </summary>      
        public static async Task<IResultValue<TValue>> ResultVoidAsyncBind<TValue>(this Task<IResultValue<TValue>> @this, Func<TValue, Task> actionAsync)
        {
            if (@this == null) throw new ArgumentNullException(nameof(@this));
            if (actionAsync == null) throw new ArgumentNullException(nameof(actionAsync));

            var awaitedThis = await @this;
            await actionAsync.Invoke(awaitedThis.Value);

            return awaitedThis;
        }

        /// <summary>
        /// Выполнить действие асинхронно, вернуть результирующий ответ
        /// </summary>      
        public static async Task<IResultValue<TValue>> ResultVoidAsyncBind<TValue>(this IResultValue<TValue> @this, Func<TValue, Task> actionAsync)
        {
            if (@this == null) throw new ArgumentNullException(nameof(@this));
            if (actionAsync == null) throw new ArgumentNullException(nameof(actionAsync));

            await actionAsync.Invoke(@this.Value);

            return @this;
        }

        /// <summary>
        /// Выполнить действие асинхронно при положительном значении, вернуть результирующий ответ
        /// </summary>      
        public static async Task<IResultValue<TValue>> ResultVoidOkBindAsync<TValue>(this Task<IResultValue<TValue>> @this, Func<TValue, Task> actionAsync)
        {
            if (@this == null) throw new ArgumentNullException(nameof(@this));
            if (actionAsync == null) throw new ArgumentNullException(nameof(actionAsync));

            var awaitedThis = await @this;
            if (awaitedThis.OkStatus) await actionAsync.Invoke(awaitedThis.Value);

            return awaitedThis;
        }

        /// <summary>
        /// Выполнить действие асинхронно при положительном значении, вернуть результирующий ответ
        /// </summary>      
        public static async Task<IResultValue<TValue>> ResultVoidBadBindAsync<TValue>(this Task<IResultValue<TValue>> @this, Func<IReadOnlyList<IErrorCommon>, Task> actionAsync)
        {
            if (@this == null) throw new ArgumentNullException(nameof(@this));
            if (actionAsync == null) throw new ArgumentNullException(nameof(actionAsync));

            var awaitedThis = await @this;
            if (awaitedThis.HasErrors) await actionAsync.Invoke(awaitedThis.Errors);

            return awaitedThis;
        }
    }
}
