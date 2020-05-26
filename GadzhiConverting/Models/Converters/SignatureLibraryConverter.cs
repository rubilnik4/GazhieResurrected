﻿using GadzhiApplicationCommon.Models.Implementation.LibraryData;
using GadzhiApplicationCommon.Models.Interfaces.LibraryData;
using GadzhiCommon.Models.Implementations.LibraryData;
using GadzhiCommon.Models.Interfaces.LibraryData;
using System;

namespace GadzhiConverting.Models.Converters
{
    public static class SignatureLibraryConverter
    {
        /// <summary>
        /// Преобразовать имя с идентификатором в класс модуля конвертации
        /// </summary>
        public static ISignatureLibraryApp ToSignatureLibraryApplication(ISignatureLibrary signatureLibrary) =>
            (signatureLibrary != null)
                ? new SignatureLibraryApp(signatureLibrary.PersonId, ToPersonInformationApp(signatureLibrary.PersonInformation))
                : throw new ArgumentNullException(nameof(signatureLibrary));

        /// <summary>
        /// Преобразовать имя с идентификатором и подписью в класс модуля конвертации
        /// </summary>
        public static ISignatureFileApp ToSignatureFileApplication(ISignatureFile signatureFile) =>
            (signatureFile != null)
                ? new SignatureFileApp(signatureFile.PersonId, ToPersonInformationApp(signatureFile.PersonInformation),
                                       signatureFile.SignatureFilePath)
                : throw new ArgumentNullException(nameof(signatureFile));

        /// <summary>
        /// Преобразовать информацию о пользователе в класс модуля конвертации
        /// </summary>
        public static PersonInformationApp ToPersonInformationApp(PersonInformation personInformation) =>
            new PersonInformationApp(personInformation.Surname, personInformation.Name,
                                     personInformation.Patronymic, personInformation.Department);
    }
}