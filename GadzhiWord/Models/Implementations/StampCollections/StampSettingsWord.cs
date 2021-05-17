﻿using System;
using GadzhiApplicationCommon.Models.Enums;
using GadzhiApplicationCommon.Models.Enums.StampCollections;
using GadzhiApplicationCommon.Models.Implementation.StampCollections;

namespace GadzhiWord.Models.Implementations.StampCollections
{
    /// <summary>
    /// Параметры штампа Word
    /// </summary>
    public class StampSettingsWord: StampSettings
    {
        public StampSettingsWord(StampIdentifier id, string personId, PdfNamingTypeApplication pdfNamingType, 
                                 string paperSize, StampOrientationType orientationType, bool useDefaultSignature)
            :base(id, personId, pdfNamingType, useDefaultSignature)
        {
            PaperSize = !String.IsNullOrWhiteSpace(paperSize)
                      ? paperSize
                      : throw new ArgumentNullException(nameof(paperSize));
            Orientation = orientationType;
        }

        /// <summary>
        /// Высота строки таблицы
        /// </summary>
        public const decimal HEIGHT_TABLE_ROW = 0.3m;

        /// <summary>
        /// Формат
        /// </summary>
        public string PaperSize { get; }

        /// <summary>
        /// Тип расположения штампа
        /// </summary>
        public StampOrientationType Orientation { get; }
    }
}