﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GadzhiWord.Word.Interfaces.Elements
{
    /// <summary>
    /// Элемент строка. Базовый вариант
    /// </summary>
    public interface IRowElement
    {
        /// <summary>
        /// Список ячеек в строке
        /// </summary>
        IList<ICellElement> CellsElementWord { get; }
    }
}