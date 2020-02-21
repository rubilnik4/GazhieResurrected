﻿using GadzhiCommon.Enums.FilesConvert;
using System.Collections.Generic;
using System.Linq;

namespace GadzhiDAL.Entities.FilesConvert
{
    /// <summary>
    /// Класс содержащий данные о конвертируемых файлах в базе данных
    /// </summary>
    public class FileDataEntity : EntityBase<int>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FileDataEntity()
        {
            StatusProcessing = StatusProcessing.InQueue;
            IsCompleted = false;
            FileConvertErrorType = new List<FileConvertErrorType>();
        }

        /// <summary>
        /// Идентефикатор
        /// </summary>
        public override int Id { get; protected set; }

        /// <summary>
        /// Путь файла
        /// </summary>      
        public virtual string FilePath { get; set; }

        /// <summary>
        /// Цвет печати
        /// </summary>       
        public virtual ColorPrint ColorPrint { get; set; }

        /// <summary>
        /// Статус обработки файла
        /// </summary>     
        public virtual StatusProcessing StatusProcessing { get; set; }

        /// <summary>
        /// Завершена ли обработка файла
        /// </summary>
        public virtual bool IsCompleted { get; set; }

        /// <summary>
        /// Файл данных в формате zip GZipStream
        /// </summary>      
        public virtual byte[] FileDataSource { get; set; }

        /// <summary>
        /// Тип ошибки при конвертации файла
        /// </summary>
        public virtual IList<FileConvertErrorType> FileConvertErrorType { get; protected set; }

        /// <summary>
        /// Ссылка на родительский класс
        /// </summary>
        public virtual FilesDataEntity FilesDataEntity { get; set; }

        /// <summary>
        /// Отметить ошибки
        /// </summary>      
        public virtual void SetFileConvertErrorType(IEnumerable<FileConvertErrorType> fileConvertErrorType)
        {
            if (fileConvertErrorType?.Any() == true)
            {
                FileConvertErrorType = fileConvertErrorType.ToList();
            }
            else
            {
                FileConvertErrorType = new List<FileConvertErrorType>();
            }
        }
    }
}
