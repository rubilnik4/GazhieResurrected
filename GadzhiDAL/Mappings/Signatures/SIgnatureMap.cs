﻿using FluentNHibernate.Mapping;
using GadzhiDAL.Entities.Signatures;
using NHibernate.Type;

namespace GadzhiDAL.Mappings.Signatures
{
    /// <summary>
    /// Структура в БД для идентификатора личности с подписью
    /// </summary>
    public class SignatureMap : ClassMap<SignatureEntity>
    {
        public SignatureMap()
        {
            Id(x => x.Id).Not.Nullable();
            Component(x => x.PersonInformation, m =>
                {
                    m.Map(x => x.Surname).Not.Nullable();
                    m.Map(x => x.Name).Not.Nullable();
                    m.Map(x => x.Patronymic).Not.Nullable();
                    m.Map(x => x.Department).Not.Nullable();
                });
            Map(x => x.SignatureJpeg).CustomType<BinaryBlobType>().LazyLoad();
        }
    }
}