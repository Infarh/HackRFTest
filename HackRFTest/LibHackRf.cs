using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace HackRFTest;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
internal class LibHackRf
{
    public const string DllName = @"hackrf.dll";
    public const int SamplesPerBlock = 8192;

    /// <summary>Перечисление кодов ошибок HackRF</summary>
    public enum HackRfError
    {
        Success = 0,
        True = 1,
        InvalidParam = -2,
        NotFound = -5,
        Busy = -6,
        NoMem = -11,
        LibUsb = -1000,
        Thread = -1001,
        StreamingThreadErr = -1002,
        StreamingStopped = -1003,
        StreamingExitCalled = -1004,
        UsbApiVersion = -1005,
        NotLastDevice = -2000,
        Other = -9999
    }

    /// <summary>Перечисление идентификаторов USB-плат HackRF</summary>
    public enum UsbBoardId
    {
        Jawbreaker = 0x604B,
        HackRfOne = 0x6089,
        Rad1O = 0xCC15,
        Invalid = 0xFFFF
    }

    /// <summary>Структура, представляющая устройство HackRF</summary>
    public readonly struct HackRfDevice
    {
        // Оставляем структуру пустой, так как это непрозрачный тип
    }

    /// <summary>Структура, представляющая передачу данных HackRF</summary>
    public readonly struct HackRfTransfer(
        IntPtr Device,
        IntPtr Buffer,
        int BufferLength,
        int ValidLength,
        IntPtr RxCtx,
        IntPtr TxCtx)
    {
        public IntPtr Device { get; } = Device;
        public IntPtr Buffer { get; } = Buffer;
        public int BufferLength { get; } = BufferLength;
        public int ValidLength { get; } = ValidLength;
        public IntPtr RxCtx { get; } = RxCtx;
        public IntPtr TxCtx { get; } = TxCtx;
    }

    /// <summary>Структура, представляющая список устройств HackRF</summary>
    public readonly struct DeviceList(
        IntPtr SerialNumbers,
        IntPtr UsbBoardIds,
        IntPtr UsbDeviceIndex,
        int DeviceCount,
        IntPtr UsbDevices,
        int UsbDeviceCount)
    {
        public IntPtr SerialNumbers { get; } = SerialNumbers; // Указатель на массив строк
        public IntPtr UsbBoardIds { get; } = UsbBoardIds; // Указатель на массив UsbBoardId
        public IntPtr UsbDeviceIndex { get; } = UsbDeviceIndex; // Указатель на массив int
        public int DeviceCount { get; } = DeviceCount;
        public IntPtr UsbDevices { get; } = UsbDevices; // Указатель на массив IntPtr
        public int UsbDeviceCount { get; } = UsbDeviceCount;
    }

    /// <summary>Инициализирует библиотеку HackRF</summary>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_init")]
    public static extern HackRfError Initialize();

    /// <summary>Завершает работу с библиотекой HackRF</summary>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_exit")]
    public static extern HackRfError Exit();

    /// <summary>Возвращает версию библиотеки HackRF</summary>
    /// <returns>Указатель на строку с версией</returns>
    [DllImport(DllName, EntryPoint = "hackrf_library_version")]
    public static extern IntPtr LibraryVersion();

    /// <summary>Возвращает релиз библиотеки HackRF</summary>
    /// <returns>Указатель на строку с релизом</returns>
    [DllImport(DllName, EntryPoint = "hackrf_library_release")]
    public static extern IntPtr GetReleaseLibrary();

    /// <summary>Открывает устройство из списка по индексу</summary>
    /// <param name="list">Указатель на список устройств</param>
    /// <param name="idx">Индекс устройства в списке</param>
    /// <param name="device">Указатель на устройство</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_device_list_open")]
    public static extern HackRfError OpenDeviceList(IntPtr list, int idx, out IntPtr device);

    /// <summary>Освобождает список устройств</summary>
    /// <param name="list">Указатель на список устройств</param>
    [DllImport(DllName, EntryPoint = "hackrf_device_list_free")]
    public static extern void FreeDeviceList(IntPtr list);

    /// <summary>Возвращает список устройств HackRF</summary>
    /// <returns>Указатель на список устройств</returns>
    [DllImport(DllName, EntryPoint = "hackrf_device_list")]
    public static extern IntPtr GetDeviceList();

    /// <summary>Открывает устройство по серийному номеру</summary>
    /// <param name="DesiredSerialNumber">Желаемый серийный номер</param>
    /// <param name="device">Указатель на устройство</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_open_by_serial")]
    public static extern HackRfError OpenBySerial(string DesiredSerialNumber, out IntPtr device);

    /// <summary>Закрывает устройство</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_close")]
    public static extern HackRfError Close(IntPtr device);

    /// <summary>Устанавливает полосу пропускания базового фильтра</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="BandwidthHz">Полоса пропускания в герцах</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_baseband_filter_bandwidth")]
    public static extern HackRfError SetBaseBandFilterBandwidth(IntPtr device, uint BandwidthHz);

    /// <summary>Устанавливает частоту</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="FreqHz">Частота в герцах</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_freq")]
    public static extern HackRfError SetFreq(IntPtr device, ulong FreqHz);

    /// <summary>Устанавливает частоту дискретизации</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="FreqHz">Частота в герцах</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_sample_rate")]
    public static extern HackRfError SetSampleRate(IntPtr device, double FreqHz);

    /// <summary>Включает или выключает усилитель</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="value">1 для включения, 0 для выключения</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_amp_enable")]
    public static extern HackRfError SetAmpEnable(IntPtr device, byte value);

    /// <summary>Устанавливает усиление LNA</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="value">Значение усиления</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_lna_gain")]
    public static extern HackRfError SetLnaGain(IntPtr device, uint value);

    /// <summary>Устанавливает усиление VGA</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="value">Значение усиления</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_vga_gain")]
    public static extern HackRfError SetVgaGain(IntPtr device, uint value);

    /// <summary>Устанавливает усиление TX VGA</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="value">Значение усиления</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_txvga_gain")]
    public static extern HackRfError SetTxVgaGain(IntPtr device, uint value);

    /// <summary>Включает или выключает антенну</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="value">1 для включения, 0 для выключения</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_antenna_enable")]
    public static extern HackRfError SetAntennaEnable(IntPtr device, byte value);

    /// <summary>Читает идентификатор платы</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="value">Идентификатор платы</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_board_id_read")]
    public static extern HackRfError BoardIdRead(IntPtr device, out byte value);

    /// <summary>Читает строку версии</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="version">Указатель на буфер для версии</param>
    /// <param name="length">Длина буфера</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_version_string_read")]
    public static extern HackRfError VersionStringRead(IntPtr device, IntPtr version, byte length);

    /// <summary>Читает версию USB API</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="version">Версия USB API</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_usb_api_version_read")]
    public static extern HackRfError UsbApiVersionRead(IntPtr device, out ushort version);

    /// <summary>Делегат для callback-функции HackRF</summary>
    /// <param name="transfer">Структура передачи данных</param>
    /// <returns>Код завершения</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int RxCallback(ref HackRfTransfer transfer);

    /// <summary>Запускает прием данных</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="callback">Callback-функция</param>
    /// <param name="RxCtx">Контекст приема</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_start_rx")]
    public static extern HackRfError StartRx(IntPtr device, RxCallback callback, IntPtr RxCtx);

    /// <summary>Останавливает прием данных</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_stop_rx")]
    public static extern HackRfError StopRx(IntPtr device);

    /// <summary>Запускает передачу данных</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="callback">Callback-функция</param>
    /// <param name="TxCtx">Контекст передачи</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_start_tx")]
    public static extern HackRfError StartTx(IntPtr device, RxCallback callback, IntPtr TxCtx);

    /// <summary>Останавливает передачу данных</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_stop_tx")]
    public static extern HackRfError StopTx(IntPtr device);

    /// <summary>Проверяет, выполняется ли потоковая передача</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_is_streaming")]
    public static extern HackRfError IsStreaming(IntPtr device);

    /// <summary>Сбрасывает устройство</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_reset")]
    public static extern HackRfError Reset(IntPtr device);

    /// <summary>Включает или выключает выходной тактовый сигнал</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="value">1 для включения, 0 для выключения</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_clkout_enable")]
    public static extern HackRfError SetClockOutEnable(IntPtr device, byte value);

    /// <summary>Читает регистр MAX2837</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="RegisterNumber">Номер регистра</param>
    /// <param name="value">Значение регистра</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_max2837_read")]
    public static extern HackRfError Max2837Read(IntPtr device, byte RegisterNumber, out ushort value);

    /// <summary>Записывает в регистр MAX2837</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="RegisterNumber">Номер регистра</param>
    /// <param name="value">Значение для записи</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_max2837_write")]
    public static extern HackRfError Max2837Write(IntPtr device, byte RegisterNumber, ushort value);

    /// <summary>Читает регистр SI5351C</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="RegisterNumber">Номер регистра</param>
    /// <param name="value">Значение регистра</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_si5351c_read")]
    public static extern HackRfError Si5351CRead(IntPtr device, ushort RegisterNumber, out ushort value);

    /// <summary>Записывает в регистр SI5351C</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="RegisterNumber">Номер регистра</param>
    /// <param name="value">Значение для записи</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_si5351c_write")]
    public static extern HackRfError Si5351CWrite(IntPtr device, ushort RegisterNumber, ushort value);

    /// <summary>Читает регистр RFFC5071</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="RegisterNumber">Номер регистра</param>
    /// <param name="value">Значение регистра</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_rffc5071_read")]
    public static extern HackRfError Rffc5071Read(IntPtr device, byte RegisterNumber, out ushort value);

    /// <summary>Записывает в регистр RFFC5071</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="RegisterNumber">Номер регистра</param>
    /// <param name="value">Значение для записи</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_rffc5071_write")]
    public static extern HackRfError Rffc5071Write(IntPtr device, byte RegisterNumber, ushort value);

    /// <summary>Стирает SPI флеш-память</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_spiflash_erase")]
    public static extern HackRfError SpiFlashErase(IntPtr device);

    /// <summary>Записывает данные в SPI флеш-память</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="address">Адрес для записи</param>
    /// <param name="length">Длина данных</param>
    /// <param name="data">Указатель на данные</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_spiflash_write")]
    public static extern HackRfError SpiFlashWrite(IntPtr device, uint address, ushort length, IntPtr data);

    /// <summary>Читает данные из SPI флеш-памяти</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="address">Адрес для чтения</param>
    /// <param name="length">Длина данных</param>
    /// <param name="data">Указатель на буфер для данных</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_spiflash_read")]
    public static extern HackRfError SpiFlashRead(IntPtr device, uint address, ushort length, IntPtr data);

    /// <summary>Читает статус SPI флеш-памяти</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="data">Указатель на буфер для статуса</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_spiflash_status")]
    public static extern HackRfError SpiFlashStatus(IntPtr device, IntPtr data);

    /// <summary>Очищает статус SPI флеш-памяти</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_spiflash_clear_status")]
    public static extern HackRfError SpiFlashClearStatus(IntPtr device);

    /// <summary>Записывает данные в CPLD</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="data">Указатель на данные</param>
    /// <param name="TotalLength">Общая длина данных</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_cpld_write")]
    public static extern HackRfError CpldWrite(IntPtr device, IntPtr data, uint TotalLength);

    /// <summary>Инициализирует развертку</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="FrequencyList">Указатель на список частот</param>
    /// <param name="NumRanges">Количество диапазонов</param>
    /// <param name="NumBytes">Количество байт</param>
    /// <param name="StepWidth">Ширина шага</param>
    /// <param name="offset">Смещение</param>
    /// <param name="style">Стиль развертки</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_init_sweep")]
    public static extern HackRfError InitSweep(IntPtr device, IntPtr FrequencyList, uint NumRanges, uint NumBytes, uint StepWidth, uint offset, uint style);

    /// <summary>Запускает развертку приема</summary>
    /// <param name="device">Указатель на устройство</param>
    /// <param name="callback">Callback-функция</param>
    /// <param name="RxCtx">Контекст приема</param>
    /// <returns>Код ошибки HackRF</returns>
    [DllImport(DllName, EntryPoint = "hackrf_start_rx_sweep")]
    public static extern HackRfError StartRxSweep(IntPtr device, RxCallback callback, IntPtr RxCtx);

    // Вспомогательные методы

    /// <summary>Получает версию библиотеки HackRF как строку</summary>
    /// <returns>Версия библиотеки</returns>
    public static string GetLibraryVersion()
    {
        var version_ptr = LibraryVersion();
        return Marshal.PtrToStringAnsi(version_ptr) ?? string.Empty;
    }

    /// <summary>Получает релиз библиотеки HackRF как строку</summary>
    /// <returns>Релиз библиотеки</returns>
    public static string GetLibraryRelease()
    {
        var release_ptr = GetReleaseLibrary();
        return Marshal.PtrToStringAnsi(release_ptr) ?? string.Empty;
    }

    /// <summary>Получает массив серийных номеров из списка устройств</summary>
    /// <param name="ListPtr">Указатель на список устройств</param>
    /// <returns>Массив серийных номеров</returns>
    public static string[] GetSerialNumbers(IntPtr ListPtr)
    {
        var list = Marshal.PtrToStructure<DeviceList>(ListPtr);
        if (list.DeviceCount <= 0)
            return [];

        var serials = new string[list.DeviceCount];
        for (var i = 0; i < list.DeviceCount; i++)
        {
            var str_ptr = Marshal.ReadIntPtr(list.SerialNumbers, i * IntPtr.Size);
            serials[i] = Marshal.PtrToStringAnsi(str_ptr) ?? string.Empty;
        }

        return serials;
    }

    /// <summary>Получает массив идентификаторов USB-плат из списка устройств</summary>
    /// <param name="ListPtr">Указатель на список устройств</param>
    /// <returns>Массив идентификаторов USB-плат</returns>
    public static UsbBoardId[] GetUsbBoardIds(IntPtr ListPtr)
    {
        var list = Marshal.PtrToStructure<DeviceList>(ListPtr);
        if (list.DeviceCount <= 0)
            return [];

        var board_ids = new UsbBoardId[list.DeviceCount];
        for (var i = 0; i < list.DeviceCount; i++)
        {
            var board_id_ptr = IntPtr.Add(list.UsbBoardIds, i * sizeof(int));
            board_ids[i] = (UsbBoardId)Marshal.ReadInt32(board_id_ptr);
        }

        return board_ids;
    }
}
