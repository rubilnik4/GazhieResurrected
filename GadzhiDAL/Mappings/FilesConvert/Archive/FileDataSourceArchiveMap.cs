﻿using FluentNHibernate.Mapping;
using GadzhiDAL.Entities.FilesConvert.Archive;

namespace GadzhiDAL.Mappings.FilesConvert.Archive
{
    /// <summary>
    /// Структура в БД для конвертируемого файла
    /// </summary>
    public class FileDataSourceArchiveMap : ClassMap<FileDataSourceArchiveEntity>
    {
        public FileDataSourceArchiveMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.FileName).Not.Nullable().Default("");
            Map(x => x.PaperSize).Not.Nullable().Default("");
            Map(x => x.PrinterName).Not.Nullable().Default("");           
            References(x => x.FileDataArchiveEntity);
        }
    }
}
