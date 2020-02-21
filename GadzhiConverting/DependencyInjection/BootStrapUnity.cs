﻿using ChannelAdam.ServiceModel;
using GadzhiCommon.Helpers.Wcf;
using GadzhiCommon.Infrastructure.Implementations;
using GadzhiCommon.Infrastructure.Interfaces;
using GadzhiConverting.Infrastructure.Implementations;
using GadzhiConverting.Infrastructure.Implementations.Converters;
using GadzhiConverting.Infrastructure.Interfaces;
using GadzhiConverting.Infrastructure.Interfaces.Converters;
using GadzhiDTOServer.Contracts.FilesConvert;
using GadzhiMicrostation.Infrastructure.Implementations;
using GadzhiMicrostation.Infrastructure.Interface;
using Unity;
using Unity.Lifetime;

namespace GadzhiConverting.DependencyInjection.GadzhiConverting
{
    /// <summary>
    /// Класс для регистрации зависимостей
    /// </summary>
    public static class BootStrapUnity
    {
        public static void Start(IUnityContainer container)
        {
            var clientEndpoints = new ClientEndpoints();
            string fileConvertingEndpoint = clientEndpoints.GetEndpointByInterfaceFullPath(typeof(IFileConvertingServerService));

            container.RegisterSingleton<IApplicationConverting, ApplicationConverting>();
            container.RegisterSingleton<IProjectSettings, ProjectSettings>();
            container.RegisterType<IConvertingService, ConvertingService>(new HierarchicalLifetimeManager());
            container.RegisterType<IConvertingFileData, ConvertingFileData>(new HierarchicalLifetimeManager());
            container.RegisterType<IMessageAndLoggingService, MessageAndLoggingService>(new HierarchicalLifetimeManager());
            container.RegisterFactory<IServiceConsumer<IFileConvertingServerService>>((unity) =>
                      ServiceConsumerFactory.Create<IFileConvertingServerService>(fileConvertingEndpoint), new ContainerControlledLifetimeManager());

            container.RegisterType<IFileSystemOperations, FileSystemOperations>();
            container.RegisterType<IExecuteAndCatchErrors, ExecuteAndCatchErrors>();
            container.RegisterType<IConverterServerFilesDataFromDTO, ConverterServerFilesDataFromDTO>();
            container.RegisterType<IConverterServerFilesDataToDTO, ConverterServerFilesDataToDTO>();

            //microstation
            container.RegisterType<IConvertingFileMicrostation, ConvertingFileMicrostation>(new HierarchicalLifetimeManager());
            //container.RegisterType<IApplicationMicrostation, ApplicationMicrostation>(new HierarchicalLifetimeManager());
            //container.RegisterType<IMicrostationProject, MicrostationProject>(new HierarchicalLifetimeManager());
            //container.RegisterType<IErrorMessagingMicrostation, ErrorMessagingMicrostation>(new HierarchicalLifetimeManager());
            //container.RegisterType<IProjectMicrostationSettings, ProjectMicrostationSettings>(new HierarchicalLifetimeManager());
            //container.RegisterType<ILoggerMicrostation, LoggerMicrostation>(new HierarchicalLifetimeManager());
            //container.RegisterType<IExecuteAndCatchErrorsMicrostation, ExecuteAndCatchErrorsMicrostation>();
            //container.RegisterType<IFileSystemOperationsMicrostation, FileSystemOperationsMicrostation>();         
        }
    }
}
