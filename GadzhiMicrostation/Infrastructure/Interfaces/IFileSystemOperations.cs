﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GadzhiMicrostation.Infrastructure.Interface
{
    /// <summary>
    /// Проверка состояния папок и файлов
    /// </summary>
    public interface IFileSystemOperationsMicrostation
    {
        /// <summary>
        /// Является ли путь папкой
        /// </summary>       
        bool IsDirectory(string directoryPath);

        /// <summary>
        /// Является ли путь файлом
        /// </summary>       
        bool IsFile(string filePath);

        /// <summary>
        /// Существует ли папка
        /// </summary>       
        bool IsDirectoryExist(string directoryPath);

        /// <summary>
        /// Существует ли файл
        /// </summary>       
        bool IsFileExist(string filePath);

        /// <summary>
        /// Получить вложенные папки
        /// </summary>        
        IEnumerable<string> GetDirectories(string directoryPath);        

        /// <summary>
        /// Получить вложенные файлы
        /// </summary>        
        IEnumerable<string> GetFiles(string filePath);
       
        /// <summary>
        /// Получить полное имя файла по директории, имени и расширению
        /// </summary>       
        string CombineFilePath(string directoryPath, string fileNameWithoutExtension, string extension);      
    }
}