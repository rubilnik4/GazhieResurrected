﻿using System;
using ChannelAdam.ServiceModel;
using GadzhiCommon.Infrastructure.Implementations.Services;
using GadzhiDTOServer.Contracts.Signatures;

namespace GadzhiConverting.Infrastructure.Implementations.Services
{
    /// <summary>
    /// Фабрика для создания подключения к WCF сервису подписей для сервера
    /// </summary>
    public class SignatureServerServiceFactory : WcfServiceFactory<IServiceConsumer<ISignatureServerService>>
    {
        public SignatureServerServiceFactory(Func<IServiceConsumer<ISignatureServerService>> getSignatureService)
            : base(getSignatureService)
        { }

        /// <summary>
        /// Необходимость инициализации сервиса при вызове метода
        /// </summary>
        protected override bool IsInitMethodService(string methodName) => false;

        /// <summary>
        /// Необходимость освобождения сервиса при вызове метода
        /// </summary>
        protected override bool IsDisposeMethodService(string methodName) => false;
    }
}