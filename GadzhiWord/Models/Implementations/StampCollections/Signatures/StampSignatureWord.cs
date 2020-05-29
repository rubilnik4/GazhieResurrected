﻿using System;
using GadzhiApplicationCommon.Extensions.Functional.Result;
using GadzhiApplicationCommon.Models.Enums;
using GadzhiApplicationCommon.Models.Implementation.Errors;
using GadzhiApplicationCommon.Models.Implementation.LibraryData;
using GadzhiApplicationCommon.Models.Implementation.StampCollections.Signatures;
using GadzhiApplicationCommon.Models.Interfaces.Errors;
using GadzhiApplicationCommon.Models.Interfaces.LibraryData;
using GadzhiApplicationCommon.Models.Interfaces.StampCollections.Fields;
using GadzhiApplicationCommon.Models.Interfaces.StampCollections.Signatures;
using GadzhiCommon.Extensions.Functional;
using GadzhiWord.Models.Interfaces.StampCollections;

namespace GadzhiWord.Models.Implementations.StampCollections.Signatures
{
    /// <summary>
    /// Базовая структура подписи Microstation
    /// </summary>
    public abstract class StampSignatureWord : StampSignature
    {
        /// <summary>
        /// Подпись. Элемент Word
        /// </summary>
        private readonly IResultAppValue<IStampFieldWord> _signature;

        protected StampSignatureWord(ISignatureLibraryApp signatureLibrary, IStampFieldWord signature )
            : base(signatureLibrary)
        {
            _signature = new ResultAppValue<IStampFieldWord>(signature, new ErrorApplication(ErrorApplicationType.SignatureNotFound,
                                                                                             "Подпись не инициализирована"));
        }

        /// <summary>
        /// Подпись
        /// </summary>
        public override IResultAppValue<IStampField> Signature => _signature;

        /// <summary>
        /// Установлена ли подпись
        /// </summary>
        public override bool IsSignatureValid() => _signature.OkStatus && _signature.Value.CellElementStamp.HasPicture;

        /// <summary>
        /// Вставить подпись
        /// </summary>
        public override IStampSignature InsertSignature(ISignatureFileApp signatureFile) =>
            _signature.
            ResultVoidOk(signature => signature.CellElementStamp.InsertPicture(signatureFile.SignatureFilePath)).
            Map(_ => this);

        /// <summary>
        /// Удалить текущую подпись
        /// </summary>
        public override IStampSignature DeleteSignature() =>
            _signature.
            ResultVoidOk(signature => signature.CellElementStamp.DeleteAllPictures()).
            Map(_ => this);
    }
}