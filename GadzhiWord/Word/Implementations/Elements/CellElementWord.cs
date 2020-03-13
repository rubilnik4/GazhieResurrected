﻿using GadzhiWord.Word.Interfaces.Elements;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GadzhiWord.Word.Implementations.Elements
{
    /// <summary>
    /// Элемент ячейка
    /// </summary>
    public class CellElementWord : ICellElement
    {
        /// <summary>
        /// Элемент ячейка Word
        /// </summary>
        private readonly Cell _cellElement;

        /// <summary>
        /// Родительская таблица
        /// </summary>
        private readonly ITableElement _tableElementWord;

        public CellElementWord(Cell cellElement, ITableElement tableElementWord)
        {
            if (cellElement != null)
            {
                _cellElement = cellElement;
                _tableElementWord = tableElementWord;
            }
            else
            {
                throw new ArgumentNullException(nameof(cellElement));
            }
        }

        /// <summary>
        /// Текст ячейки
        /// </summary>
        public string Text => _cellElement.Range.Text;

        /// <summary>
        /// Родительский элемент строка
        /// </summary>
        public IRowElement RowElementWord => _tableElementWord?.RowsElementWord[_cellElement.RowIndex - 1];

        /// <summary>
        /// Номер строки
        /// </summary>
        public int RowIndex => _cellElement.RowIndex - 1;

        /// <summary>
        /// Вставить картинку
        /// </summary>
        public void InsertPicture(string filePath)
        {
            if (!String.IsNullOrWhiteSpace(filePath))
            {
                _cellElement.Range.InlineShapes.AddPicture(filePath, false, true);
            }           
        }

        /// <summary>
        /// Удалить все картинки
        /// </summary>
        public void DeleteAllPictures()
        {
            foreach (InlineShape shape in _cellElement.Range.InlineShapes)
            {
                shape.Delete();
            }
        }
    }
}
