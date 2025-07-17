using System.Runtime.InteropServices;

namespace HackRFTest;

internal static class ReceiveData
{
    private static LibHackRf.HackRfDelegate __Callback;
    private static GCHandle __ContextHandle;

    public static void Run()
    {
        try
        {
            // Инициализация библиотеки
            if (LibHackRf.HackRfInit() != LibHackRf.HackRfError.Success)
            {
                Console.WriteLine("Ошибка инициализации HackRF.");
                return;
            }

            // Получение списка устройств
            var device_list_ptr = LibHackRf.GetHackRfDeviceList();
            if (device_list_ptr == nint.Zero)
            {
                Console.WriteLine("Не удалось получить список устройств.");
                LibHackRf.HackRfExit();
                return;
            }

            var deviceList = Marshal.PtrToStructure<LibHackRf.HackRfDeviceList>(device_list_ptr);
            if (deviceList.DeviceCount == 0)
            {
                Console.WriteLine("Устройства HackRF не найдены.");
                LibHackRf.HackRfDeviceListFree(device_list_ptr);
                LibHackRf.HackRfExit();
                return;
            }

            // Открытие первого устройства
            if (LibHackRf.HackRfDeviceListOpen(device_list_ptr, 0, out var device) != LibHackRf.HackRfError.Success)
            {
                Console.WriteLine("Не удалось открыть устройство.");
                LibHackRf.HackRfDeviceListFree(device_list_ptr);
                LibHackRf.HackRfExit();
                return;
            }

            LibHackRf.HackRfDeviceListFree(device_list_ptr);

            // Настройка параметров
            LibHackRf.HackRfSetFreq(device, 433_000_000); // 433 МГц
            LibHackRf.HackRfSetSampleRate(device, 2_000_000.0); // 2 МГц
            LibHackRf.HackRfSetLnaGain(device, 16); // 16 дБ
            LibHackRf.HackRfSetVgaGain(device, 20); // 20 дБ

            // Подготовка контекста для callback
            var rxContext = new RxContext();
            __ContextHandle = GCHandle.Alloc(rxContext, GCHandleType.Normal);
            __Callback = RxCallback;

            // Запуск приёма
            if (LibHackRf.HackRfStartRx(device, __Callback, GCHandle.ToIntPtr(__ContextHandle)) != LibHackRf.HackRfError.Success)
            {
                Console.WriteLine("Ошибка запуска приёма.");
                __ContextHandle.Free();
                LibHackRf.HackRfClose(device);
                LibHackRf.HackRfExit();
                return;
            }

            // Ожидание завершения захвата
            rxContext.Done.WaitOne();

            // Остановка приёма
            LibHackRf.HackRfStopRx(device);

            // Сохранение данных в файл
            File.WriteAllBytes("capture.bin", rxContext.Data);

            Console.WriteLine("Захват завершён. Данные сохранены в capture.bin.");

            // Очистка
            __ContextHandle.Free();
            LibHackRf.HackRfClose(device);
            LibHackRf.HackRfExit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }

    private static int RxCallback(ref LibHackRf.HackRfTransfer transfer)
    {
        var handle = GCHandle.FromIntPtr(transfer.RxCtx);
        var context = (RxContext)handle.Target!;

        var bytes_to_copy = Math.Min(transfer.ValidLength, context.Data.Length - context.Index);
        if (bytes_to_copy > 0)
        {
            Marshal.Copy(transfer.Buffer, context.Data, context.Index, bytes_to_copy);
            context.Index += bytes_to_copy;
        }

        if (context.Index >= context.Data.Length)
        {
            context.Done.Set();
            return 1; // Остановить приём
        }

        return 0; // Продолжить приём
    }

    private class RxContext
    {
        public byte[] Data { get; } = new byte[2_000_000]; // 1M IQ-сэмплов = 2M байт

        public Span<ushort> Samples => MemoryMarshal.Cast<byte, ushort>(Data);

        public Span<Sample> SampleData => MemoryMarshal.Cast<byte, Sample>(Data);

        public int Index { get; set; }

        public ManualResetEvent Done { get; } = new ManualResetEvent(false);
    }
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct Sample
{
    public sbyte I { get; init; }

    public sbyte Q { get; init; }
}
