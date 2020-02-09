﻿using FluentNHibernate.Mapping;
using GadzhiCommon.Enums.FilesConvert;
using GadzhiDAL.Entities.FilesConvert;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GadzhiDAL.Mappings.FilesConvert
{
    public class FilesDataMap : ClassMap<FilesDataEntity>
    {
        public FilesDataMap()
        {
            Id(x => x.Id);
            Map(x => x.IsCompleted);
            //Map(x => x.IdGuid).CustomType<GuidType>();
            //Map(x => x.StatusProcessingProject).CustomType<StatusProcessingProject>();
            HasMany(x => x.FilesData)
                    .Inverse()
                    .Cascade.All();
        }        
    }
}
