﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadzhiDAL.Entities.Signatures;
using GadzhiDAL.Models.Implementations;
using GadzhiDTOBase.TransferModels.Signatures;
using GadzhiDTOServer.TransferModels.Signatures;

namespace GadzhiDAL.Infrastructure.Implementations.Converters.DataFile
{
    /// <summary>
    /// Преобразование идентификатора с подписью в модель базы банных и трансферную
    /// </summary>
    public static class ConverterDataFile
    {
        /// <summary>
        /// Преобразовать идентификаторы с подписью в модель базы банных
        /// </summary>
        public static IList<SignatureEntity> SignaturesFromDto(IList<SignatureDto> signaturesDto) =>
            signaturesDto?.Select(SignatureFromDto).ToList()
            ?? throw new ArgumentNullException(nameof(signaturesDto));

        /// <summary>
        /// Преобразовать идентификаторы с подписью в трансферную модель
        /// </summary>
        public static IList<SignatureDto> SignaturesToDto(IList<SignatureEntity> signatureEntities, bool signatureLoad)
        {
            if (signatureEntities == null) throw new ArgumentNullException(nameof(signatureEntities));
            return signatureEntities.AsQueryable().
                                     Select(signatire => SignatureToDto(signatire, signatureLoad)).
                                     ToList();
        }

        /// <summary>
        /// Преобразовать идентификатор с подписью Microstation в модель базы банных
        /// </summary>
        public static MicrostationDataFileEntity MicrostationDataFileFromDto(MicrostationDataFileDto microstationDataFileDto, string idDataFile)
        {
            if (microstationDataFileDto == null) throw new ArgumentNullException(nameof(microstationDataFileDto));
            if (String.IsNullOrEmpty(idDataFile)) throw new ArgumentNullException(nameof(idDataFile));

            var signatureMicrostationEntity = new MicrostationDataFileEntity()
            {
                NameDatabase = microstationDataFileDto.NameDatabase,
                MicrostationDataBase = microstationDataFileDto.MicrostationDataBase,
            };
            signatureMicrostationEntity.SetId(idDataFile);

            return signatureMicrostationEntity;
        }

        /// <summary>
        /// Преобразовать идентификатор с подписью Microstation в трансферную модель
        /// </summary>
        public static MicrostationDataFileDto SignatureMicrostationToDto(MicrostationDataFileEntity microstationDataFileEntity)
        {
            if (microstationDataFileEntity == null) throw new ArgumentNullException(nameof(microstationDataFileEntity));

            var signatureMicrostationDto = new MicrostationDataFileDto()
            {
                NameDatabase = microstationDataFileEntity.NameDatabase,
                MicrostationDataBase = microstationDataFileEntity.MicrostationDataBase.ToArray(),
            };

            return signatureMicrostationDto;
        }

        /// <summary>
        /// Преобразовать идентификатор с подписью в модель базы банных
        /// </summary>
        private static SignatureEntity SignatureFromDto(SignatureDto signatureDto)
        {
            if (signatureDto == null) throw new ArgumentNullException(nameof(signatureDto));

            var signatureEntity = new SignatureEntity()
            {
                PersonInformation = PersonInformationFromDto(signatureDto.PersonInformation),
                SignatureJpeg = signatureDto.SignatureJpeg,
            };
            signatureEntity.SetId(signatureDto.Id);

            return signatureEntity;
        }

        private static PersonInformationComponent PersonInformationFromDto(PersonInformationDto personInformation) =>
            (personInformation != null)
                ? new PersonInformationComponent()
                {
                    Surname = personInformation.Surname,
                    Name = personInformation.Name,
                    Patronymic = personInformation.Patronymic,
                    Department = personInformation.Department,
                }
                : throw new ArgumentNullException(nameof(personInformation));

        /// <summary>
        /// Преобразовать идентификатор с подписью в трансферную модель
        /// </summary>
        private static SignatureDto SignatureToDto(SignatureEntity signatureEntity, bool signatureLoad) =>
            (signatureEntity != null)
                ? new SignatureDto()
                {
                    Id = signatureEntity.Id,
                    PersonInformation = PersonInformationToDto(signatureEntity.PersonInformation),
                    SignatureJpeg = signatureLoad
                                ? signatureEntity.SignatureJpeg.AsQueryable().ToArray()
                                : null,
                }
                : throw new ArgumentNullException(nameof(signatureEntity));

        private static PersonInformationDto PersonInformationToDto(PersonInformationComponent personInformation) =>
            (personInformation != null)
                ? new PersonInformationDto()
                {
                    Surname = personInformation.Surname,
                    Name = personInformation.Name,
                    Patronymic = personInformation.Patronymic,
                    Department = personInformation.Department,
                }
                : throw new ArgumentNullException(nameof(personInformation));
    }
}