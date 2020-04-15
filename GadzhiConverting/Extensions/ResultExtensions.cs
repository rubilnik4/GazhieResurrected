﻿using ConvertingModels.Models.Interfaces.FilesConvert;
using GadzhiApplicationCommon.Models.Interfaces.Errors;
using GadzhiCommon.Extentions.Functional;
using GadzhiCommon.Models.Interfaces.Errors;
using GadzhiConverting.Models.Converters;
using GadzhiConverting.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GadzhiConverting.Extensions
{
    /// <summary>
    /// Преобразование результирующего ответа модуля конвертации в основной
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Преобразовать результирующий ответ модуля конвертации в основной
        /// </summary>      
        public static IResultError ToResultFromApplication(this IResultApplication resultApplication) =>
          ResultApplicationConverter.ToResult(resultApplication);

        /// <summary>
        /// Преобразовать результирующий ответ модуля со значением  конвертации в основной
        /// </summary>      
        public static GadzhiCommon.Models.Interfaces.Errors.IResultValue<TResult> ToResultValueFromApplication<TApplication, TResult>(this GadzhiApplicationCommon.Models.Interfaces.Errors.IResultValue<TApplication> resultApplication,
                                                                                                Func<TApplication, TResult> converterValue) =>
          ResultApplicationConverter.ToResultValue(resultApplication, converterValue);
    }
}
