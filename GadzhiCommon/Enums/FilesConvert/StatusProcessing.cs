﻿namespace GadzhiCommon.Enums.FilesConvert
{
    /// <summary>
    /// Статус обработки файлов
    /// </summary>
    public enum StatusProcessing
    {
        NotSend,
        Sending,
        InQueue,
        Converting,
        CompletedConverting,
        Writing,
        End,
    }
}
