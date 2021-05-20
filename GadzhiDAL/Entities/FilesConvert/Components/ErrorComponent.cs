﻿using System;
using GadzhiCommon.Enums.FilesConvert;

namespace GadzhiDAL.Entities.FilesConvert.Base.Components
{
    /// <summary>
    /// Информация об ошибке
    /// </summary>
    public class ErrorComponent
    {
        /// <summary>
        /// Тип ошибки при конвертации файлов
        /// </summary>
        public virtual ErrorConvertingType ErrorConvertingType { get; set; } = ErrorConvertingType.NoError;

        /// <summary>
        /// Описание ошибки
        /// </summary>
        public virtual string ErrorDescription { get; set; } = String.Empty;
    }
}