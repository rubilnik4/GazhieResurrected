﻿using ChannelAdam.ServiceModel;
using GadzhiCommon.Helpers.Wcf;
using GadzhiDTOClient.Contracts.FilesConvert;
using GadzhiModules.Infrastructure.Implementations;
using GadzhiModules.Infrastructure.Implementations.ApplicationGadzhi;
using GadzhiModules.Infrastructure.Implementations.Converters;
using GadzhiModules.Infrastructure.Interfaces;
using GadzhiModules.Infrastructure.Interfaces.ApplicationGadzhi;
using GadzhiModules.Infrastructure.Interfaces.Converters;
using GadzhiModules.Modules.GadzhiConvertingModule.Models.Implementations.FileConverting;
using GadzhiModules.Modules.GadzhiConvertingModule.Models.Implementations.ProjectSettings;
using GadzhiModules.Modules.GadzhiConvertingModule.Models.Interfaces.FileConverting;
using GadzhiModules.Modules.GadzhiConvertingModule.Models.Interfaces.ProjectSettings;
using GadzhiModules.Modules.GadzhiConvertingModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using Unity;

namespace GadzhiModules.Modules.GadzhiConvertingModule
{
    public class GadzhiConvertingModule : IModule
    {
        /// <summary>
        /// Привязка модулей к View
        /// </summary>
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.GadzhiConvertingModule, typeof(GadzhiConvertingView));
        }

        /// <summary>
        /// Регистрация зависимостей
        /// </summary>        
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var unityContainer = containerRegistry.GetContainer();
            var clientEndpoints = new ClientEndpoints();
            string fileConvertingEndpoint = clientEndpoints.GetEndpointByInterfaceFullPath(typeof(IFileConvertingClientService));

            unityContainer.RegisterFactory<IServiceConsumer<IFileConvertingClientService>>(unity =>
                      ServiceConsumerFactory.Create<IFileConvertingClientService>(fileConvertingEndpoint), FactoryLifetime.Singleton);

            unityContainer.RegisterSingleton<IPackageData, PackageData>();
            unityContainer.RegisterSingleton<IStatusProcessingInformation, StatusProcessingInformation>();
            unityContainer.RegisterType<IFileDataProcessingStatusMark, FileDataProcessingStatusMark>();
            unityContainer.RegisterType<IConverterClientPackageDataToDto, ConverterClientPackageDataToDto>();
            unityContainer.RegisterType<IConverterClientPackageDataFromDto, ConverterClientPackageDataFromDto>();

            unityContainer.RegisterFactory<IProjectSettings>(unity =>
                new ProjectSettings(ApplicationGadzhi.GetConvertingSettingFromConfiguration()), FactoryLifetime.Singleton);

            var application = unityContainer.Resolve<IApplicationGadzhi>();
            unityContainer.RegisterFactory<IProjectResources>(unity => new ProjectResources(application.GetSignaturesNames()), FactoryLifetime.Singleton);
        }
    }
}
