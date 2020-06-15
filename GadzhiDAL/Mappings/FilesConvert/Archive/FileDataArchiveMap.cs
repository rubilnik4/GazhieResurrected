﻿using FluentNHibernate.Mapping;
using GadzhiCommon.Enums.FilesConvert;
using GadzhiDAL.Entities.FilesConvert.Archive;

namespace GadzhiDAL.Mappings.FilesConvert.Archive
{
    /// <summary>
    /// Структура в БД для конвертируемого файла
    /// </summary>
    public class FileDataArchiveMap : ClassMap<FileDataArchiveEntity>
    {
        public FileDataArchiveMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.FilePath).Not.Nullable().Default("");         
            Map(x => x.ColorPrint).CustomType<ColorPrint>().Not.Nullable();
            HasMany(x => x.FileErrorsArchive).Component(x =>
                                              {
                                                  x.Map(e => e.FileConvertErrorType);
                                                  x.Map(e => e.ErrorDescription);
                                               });
            HasMany(x => x.FileDataSourceServerArchiveEntities).Inverse().Cascade.All();
            References(x => x.PackageDataArchiveEntity);
        }
    }
}
