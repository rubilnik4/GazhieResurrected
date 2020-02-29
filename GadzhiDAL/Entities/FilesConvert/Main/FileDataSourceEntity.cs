﻿using GadzhiDAL.Entities.FilesConvert.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GadzhiDAL.Entities.FilesConvert.Main
{
    /// <summary>
    /// Информация об отконвертированных файлах в базе данных
    /// </summary>
    public class FileDataSourceEntity: FileDataSourceEntityBase
    {    
        /// <summary>
        /// Файл данных в формате zip GZipStream
        /// </summary>       
        public virtual IList<byte> FileDataSource { get; set; }

        /// <summary>
        /// Ссылка на родительский класс
        /// </summary>
        public virtual FileDataEntity FileDataEntity { get; set; }
    }
}