using HackRFTest;

try
{
    // Инициализация библиотеки HackRF
    var init_result = LibHackRf.HackRfInit();
    if (init_result != LibHackRf.HackRfError.Success)
    {
        Console.WriteLine($"Ошибка инициализации HackRF: {init_result}");
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

    // Извлечение серийных номеров и идентификаторов USB-плат
    var serial_numbers = LibHackRf.GetSerialNumbers(device_list_ptr);
    var board_ids = LibHackRf.GetUsbBoardIds(device_list_ptr);

    // Вывод информации об устройствах
    if (serial_numbers.Length == 0)
        Console.WriteLine("Устройства HackRF не найдены.");
    else
    {
        Console.WriteLine($"Найдено {serial_numbers.Length} устройств HackRF:");
        for (var i = 0; i < serial_numbers.Length; i++)
        {
            Console.WriteLine($"Устройство {i + 1}:");
            Console.WriteLine($"  Серийный номер: {serial_numbers[i]}");
            Console.WriteLine($"  Идентификатор платы: {board_ids[i]}");
        }
    }

    // Освобождение списка устройств
    LibHackRf.HackRfDeviceListFree(device_list_ptr);

    // Завершение работы с библиотекой
    var exit_result = LibHackRf.HackRfExit();
    if (exit_result != LibHackRf.HackRfError.Success)
        Console.WriteLine($"Ошибка завершения работы HackRF: {exit_result}");
}
catch (Exception ex)
{
    Console.WriteLine($"Произошла ошибка: {ex.Message}");
}
finally
{
    LibHackRf.HackRfExit();
}

Console.WriteLine("End.");
return;
