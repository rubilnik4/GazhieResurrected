﻿using GadzhiApplicationCommon.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GadzhiApplicationCommon.Models.Interfaces.StampCollections
{
    /// <summary>
    /// Штамп. Базовый вариант
    /// </summary>
    public interface IStamp
    {
        /// <summary>
        /// Наименование
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Тип штампа
        /// </summary>
        StampType StampType { get; }

        /// <summary>
        /// Формат
        /// </summary>
        string PaperSize { get; }
    }
}
