﻿using System;
using System.Threading.Tasks;


namespace GadzhiModules.Infrastructure.Interfaces.ApplicationGadzhi
{
    /// <summary>
    /// Слой приложения, инфраструктура
    /// </summary>
    public interface IApplicationGadzhi : IApplicationGadzhiFileData, IApplicationGadzhiServices, IDisposable
    {
        /// <summary>
        /// Закрыть приложение
        /// </summary>
        Task CloseApplication();
    }
}
