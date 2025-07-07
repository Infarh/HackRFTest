using System.Runtime.InteropServices;

namespace HackRFTest;

internal class LibHackRf
{
    public const string DllName = @"hackrf.dll";
    public const int SamplesPerBlock = 8192;

    /// <summary>
    /// Перечисление кодов ошибок HackRF.
    /// </summary>
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

    /// <summary>
    /// Перечисление идентификаторов USB-плат HackRF.
    /// </summary>
    public enum HackRfUsbBoardId
    {
        Jawbreaker = 0x604B,
        HackRfOne = 0x6089,
        Rad1O = 0xCC15,
        Invalid = 0xFFFF
    }

    /// <summary>
    /// Структура, представляющая устройство HackRF.
    /// </summary>
    public readonly struct HackRfDevice
    {
        // Оставляем структуру пустой, так как это непрозрачный тип
    }

    /// <summary>
    /// Структура, представляющая передачу данных HackRF.
    /// </summary>
    public readonly struct HackRfTransfer
    {
        public IntPtr Device { get; }
        public IntPtr Buffer { get; }
        public int BufferLength { get; }
        public int ValidLength { get; }
        public IntPtr RxCtx { get; }
        public IntPtr TxCtx { get; }

        public HackRfTransfer(IntPtr device, IntPtr buffer, int bufferLength, int validLength, IntPtr rxCtx, IntPtr txCtx)
        {
            Device = device;
            Buffer = buffer;
            BufferLength = bufferLength;
            ValidLength = validLength;
            RxCtx = rxCtx;
            TxCtx = txCtx;
        }
    }

    /// <summary>
    /// Структура, представляющая список устройств HackRF.
    /// </summary>
    public readonly struct HackRfDeviceList
    {
        public IntPtr SerialNumbers { get; } // Указатель на массив строк
        public IntPtr UsbBoardIds { get; }   // Указатель на массив HackRfUsbBoardId
        public IntPtr UsbDeviceIndex { get; } // Указатель на массив int
        public int DeviceCount { get; }
        public IntPtr UsbDevices { get; }     // Указатель на массив IntPtr
        public int UsbDeviceCount { get; }

        public HackRfDeviceList(IntPtr serialNumbers, IntPtr usbBoardIds, IntPtr usbDeviceIndex, int deviceCount, IntPtr usbDevices, int usbDeviceCount)
        {
            SerialNumbers = serialNumbers;
            UsbBoardIds = usbBoardIds;
            UsbDeviceIndex = usbDeviceIndex;
            DeviceCount = deviceCount;
            UsbDevices = usbDevices;
            UsbDeviceCount = usbDeviceCount;
        }
    }

    /// <summary>
    /// Инициализирует библиотеку HackRF.
    /// </summary>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_init")]
    public static extern HackRfError HackRfInit();

    /// <summary>
    /// Завершает работу с библиотекой HackRF.
    /// </summary>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_exit")]
    public static extern HackRfError HackRfExit();

    /// <summary>
    /// Возвращает версию библиотеки HackRF.
    /// </summary>
    /// <returns>Указатель на строку с версией.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_library_version")]
    public static extern IntPtr HackRfLibraryVersion();

    /// <summary>
    /// Возвращает релиз библиотеки HackRF.
    /// </summary>
    /// <returns>Указатель на строку с релизом.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_library_release")]
    public static extern IntPtr HackRfLibraryRelease();

    /// <summary>
    /// Открывает устройство из списка по индексу.
    /// </summary>
    /// <param name="list">Указатель на список устройств.</param>
    /// <param name="idx">Индекс устройства в списке.</param>
    /// <param name="device">Указатель на устройство.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_device_list_open")]
    public static extern HackRfError HackRfDeviceListOpen(IntPtr list, int idx, out IntPtr device);

    /// <summary>
    /// Освобождает список устройств.
    /// </summary>
    /// <param name="list">Указатель на список устройств.</param>
    [DllImport(DllName, EntryPoint = "hackrf_device_list_free")]
    public static extern void HackRfDeviceListFree(IntPtr list);

    /// <summary>
    /// Возвращает список устройств HackRF.
    /// </summary>
    /// <returns>Указатель на список устройств.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_device_list")]
    public static extern IntPtr GetHackRfDeviceList();

    /// <summary>
    /// Открывает устройство по серийному номеру.
    /// </summary>
    /// <param name="desiredSerialNumber">Желаемый серийный номер.</param>
    /// <param name="device">Указатель на устройство.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_open_by_serial")]
    public static extern HackRfError HackRfOpenBySerial(string desiredSerialNumber, out IntPtr device);

    /// <summary>
    /// Закрывает устройство.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_close")]
    public static extern HackRfError HackRfClose(IntPtr device);

    /// <summary>
    /// Устанавливает полосу пропускания базового фильтра.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="bandwidthHz">Полоса пропускания в герцах.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_baseband_filter_bandwidth")]
    public static extern HackRfError HackRfSetBasebandFilterBandwidth(IntPtr device, uint bandwidthHz);

    /// <summary>
    /// Устанавливает частоту.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="freqHz">Частота в герцах.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_freq")]
    public static extern HackRfError HackRfSetFreq(IntPtr device, ulong freqHz);

    /// <summary>
    /// Устанавливает частоту дискретизации.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="freqHz">Частота в герцах.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_sample_rate")]
    public static extern HackRfError HackRfSetSampleRate(IntPtr device, double freqHz);

    /// <summary>
    /// Включает или выключает усилитель.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="value">1 для включения, 0 для выключения.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_amp_enable")]
    public static extern HackRfError HackRfSetAmpEnable(IntPtr device, byte value);

    /// <summary>
    /// Устанавливает усиление LNA.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="value">Значение усиления.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_lna_gain")]
    public static extern HackRfError HackRfSetLnaGain(IntPtr device, uint value);

    /// <summary>
    /// Устанавливает усиление VGA.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="value">Значение усиления.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_vga_gain")]
    public static extern HackRfError HackRfSetVgaGain(IntPtr device, uint value);

    /// <summary>
    /// Устанавливает усиление TX VGA.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="value">Значение усиления.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_txvga_gain")]
    public static extern HackRfError HackRfSetTxVgaGain(IntPtr device, uint value);

    /// <summary>
    /// Включает или выключает антенну.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="value">1 для включения, 0 для выключения.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_antenna_enable")]
    public static extern HackRfError HackRfSetAntennaEnable(IntPtr device, byte value);

    /// <summary>
    /// Читает идентификатор платы.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="value">Идентификатор платы.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_board_id_read")]
    public static extern HackRfError HackRfBoardIdRead(IntPtr device, out byte value);

    /// <summary>
    /// Читает строку версии.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="version">Указатель на буфер для версии.</param>
    /// <param name="length">Длина буфера.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_version_string_read")]
    public static extern HackRfError HackRfVersionStringRead(IntPtr device, IntPtr version, byte length);

    /// <summary>
    /// Читает версию USB API.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="version">Версия USB API.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_usb_api_version_read")]
    public static extern HackRfError HackRfUsbApiVersionRead(IntPtr device, out ushort version);

    /// <summary>
    /// Делегат для callback-функции HackRF.
    /// </summary>
    /// <param name="transfer">Структура передачи данных.</param>
    /// <returns>Код завершения.</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int HackRfDelegate(ref HackRfTransfer transfer);

    /// <summary>
    /// Запускает прием данных.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="callback">Callback-функция.</param>
    /// <param name="rxCtx">Контекст приема.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_start_rx")]
    public static extern HackRfError HackRfStartRx(IntPtr device, HackRfDelegate callback, IntPtr rxCtx);

    /// <summary>
    /// Останавливает прием данных.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_stop_rx")]
    public static extern HackRfError HackRfStopRx(IntPtr device);

    /// <summary>
    /// Запускает передачу данных.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="callback">Callback-функция.</param>
    /// <param name="txCtx">Контекст передачи.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_start_tx")]
    public static extern HackRfError HackRfStartTx(IntPtr device, HackRfDelegate callback, IntPtr txCtx);

    /// <summary>
    /// Останавливает передачу данных.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_stop_tx")]
    public static extern HackRfError HackRfStopTx(IntPtr device);

    /// <summary>
    /// Проверяет, выполняется ли потоковая передача.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_is_streaming")]
    public static extern HackRfError HackRfIsStreaming(IntPtr device);

    /// <summary>
    /// Сбрасывает устройство.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_reset")]
    public static extern HackRfError HackRfReset(IntPtr device);

    /// <summary>
    /// Включает или выключает выходной тактовый сигнал.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="value">1 для включения, 0 для выключения.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_set_clkout_enable")]
    public static extern HackRfError HackRfSetClkoutEnable(IntPtr device, byte value);

    /// <summary>
    /// Читает регистр MAX2837.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="registerNumber">Номер регистра.</param>
    /// <param name="value">Значение регистра.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_max2837_read")]
    public static extern HackRfError HackRfMax2837Read(IntPtr device, byte registerNumber, out ushort value);

    /// <summary>
    /// Записывает в регистр MAX2837.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="registerNumber">Номер регистра.</param>
    /// <param name="value">Значение для записи.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_max2837_write")]
    public static extern HackRfError HackRfMax2837Write(IntPtr device, byte registerNumber, ushort value);

    /// <summary>
    /// Читает регистр SI5351C.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="registerNumber">Номер регистра.</param>
    /// <param name="value">Значение регистра.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_si5351c_read")]
    public static extern HackRfError HackRfSi5351CRead(IntPtr device, ushort registerNumber, out ushort value);

    /// <summary>
    /// Записывает в регистр SI5351C.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="registerNumber">Номер регистра.</param>
    /// <param name="value">Значение для записи.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_si5351c_write")]
    public static extern HackRfError HackRfSi5351CWrite(IntPtr device, ushort registerNumber, ushort value);

    /// <summary>
    /// Читает регистр RFFC5071.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="registerNumber">Номер регистра.</param>
    /// <param name="value">Значение регистра.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_rffc5071_read")]
    public static extern HackRfError HackRfRffc5071Read(IntPtr device, byte registerNumber, out ushort value);

    /// <summary>
    /// Записывает в регистр RFFC5071.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="registerNumber">Номер регистра.</param>
    /// <param name="value">Значение для записи.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_rffc5071_write")]
    public static extern HackRfError HackRfRffc5071Write(IntPtr device, byte registerNumber, ushort value);

    /// <summary>
    /// Стирает SPI флеш-память.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_spiflash_erase")]
    public static extern HackRfError HackRfSpiFlashErase(IntPtr device);

    /// <summary>
    /// Записывает данные в SPI флеш-память.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="address">Адрес для записи.</param>
    /// <param name="length">Длина данных.</param>
    /// <param name="data">Указатель на данные.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_spiflash_write")]
    public static extern HackRfError HackRfSpiFlashWrite(IntPtr device, uint address, ushort length, IntPtr data);

    /// <summary>
    /// Читает данные из SPI флеш-памяти.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="address">Адрес для чтения.</param>
    /// <param name="length">Длина данных.</param>
    /// <param name="data">Указатель на буфер для данных.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_spiflash_read")]
    public static extern HackRfError HackRfSpiFlashRead(IntPtr device, uint address, ushort length, IntPtr data);

    /// <summary>
    /// Читает статус SPI флеш-памяти.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="data">Указатель на буфер для статуса.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_spiflash_status")]
    public static extern HackRfError HackRfSpiFlashStatus(IntPtr device, IntPtr data);

    /// <summary>
    /// Очищает статус SPI флеш-памяти.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_spiflash_clear_status")]
    public static extern HackRfError HackRfSpiFlashClearStatus(IntPtr device);

    /// <summary>
    /// Записывает данные в CPLD.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="data">Указатель на данные.</param>
    /// <param name="totalLength">Общая длина данных.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_cpld_write")]
    public static extern HackRfError HackRfCpldWrite(IntPtr device, IntPtr data, uint totalLength);

    /// <summary>
    /// Инициализирует развертку.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="frequencyList">Указатель на список частот.</param>
    /// <param name="numRanges">Количество диапазонов.</param>
    /// <param name="numBytes">Количество байт.</param>
    /// <param name="stepWidth">Ширина шага.</param>
    /// <param name="offset">Смещение.</param>
    /// <param name="style">Стиль развертки.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_init_sweep")]
    public static extern HackRfError HackRfInitSweep(IntPtr device, IntPtr frequencyList, uint numRanges, uint numBytes, uint stepWidth, uint offset, uint style);

    /// <summary>
    /// Запускает развертку приема.
    /// </summary>
    /// <param name="device">Указатель на устройство.</param>
    /// <param name="callback">Callback-функция.</param>
    /// <param name="rxCtx">Контекст приема.</param>
    /// <returns>Код ошибки HackRF.</returns>
    [DllImport(DllName, EntryPoint = "hackrf_start_rx_sweep")]
    public static extern HackRfError HackRfStartRxSweep(IntPtr device, HackRfDelegate callback, IntPtr rxCtx);

    // Вспомогательные методы

    /// <summary>
    /// Получает версию библиотеки HackRF как строку.
    /// </summary>
    /// <returns>Версия библиотеки.</returns>
    public static string GetLibraryVersion()
    {
        var versionPtr = HackRfLibraryVersion();
        return Marshal.PtrToStringAnsi(versionPtr) ?? string.Empty;
    }

    /// <summary>
    /// Получает релиз библиотеки HackRF как строку.
    /// </summary>
    /// <returns>Релиз библиотеки.</returns>
    public static string GetLibraryRelease()
    {
        var releasePtr = HackRfLibraryRelease();
        return Marshal.PtrToStringAnsi(releasePtr) ?? string.Empty;
    }

    /// <summary>
    /// Получает массив серийных номеров из списка устройств.
    /// </summary>
    /// <param name="listPtr">Указатель на список устройств.</param>
    /// <returns>Массив серийных номеров.</returns>
    public static string[] GetSerialNumbers(IntPtr listPtr)
    {
        var list = Marshal.PtrToStructure<HackRfDeviceList>(listPtr);
        if (list.DeviceCount <= 0) return Array.Empty<string>();

        var serials = new string[list.DeviceCount];
        for (var i = 0; i < list.DeviceCount; i++)
        {
            var strPtr = Marshal.ReadIntPtr(list.SerialNumbers, i * IntPtr.Size);
            serials[i] = Marshal.PtrToStringAnsi(strPtr) ?? string.Empty;
        }
        return serials;
    }

    /// <summary>
    /// Получает массив идентификаторов USB-плат из списка устройств.
    /// </summary>
    /// <param name="listPtr">Указатель на список устройств.</param>
    /// <returns>Массив идентификаторов USB-плат.</returns>
    public static HackRfUsbBoardId[] GetUsbBoardIds(IntPtr listPtr)
    {
        var list = Marshal.PtrToStructure<HackRfDeviceList>(listPtr);
        if (list.DeviceCount <= 0) return Array.Empty<HackRfUsbBoardId>();

        var boardIds = new HackRfUsbBoardId[list.DeviceCount];
        for (var i = 0; i < list.DeviceCount; i++)
        {
            var boardIdPtr = IntPtr.Add(list.UsbBoardIds, i * sizeof(int));
            boardIds[i] = (HackRfUsbBoardId)Marshal.ReadInt32(boardIdPtr);
        }
        return boardIds;
    }
}