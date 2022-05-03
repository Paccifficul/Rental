using System.Data.SQLite;
using SQLProject.Enum;
using SQLProject.Menu;

namespace SQLProject.ModelDB;

public static class DbExecutive
{
    private static readonly SQLiteConnection Connection = new("Data Source=database");
    private static SQLiteCommand? _command;
    private static SQLiteDataReader? _dataReader;
    
    /// <summary>
    /// Добавляет данные в базу по введённым параметрам
    /// </summary>
    public static void AddCarsToDb()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Введите марку и модель автомобиля:");
        var model = Console.ReadLine(); // Марка и модель автомобиля

        Console.WriteLine("Введите тип автомобиля (спорткар/электромобиль/обычный)");
        string? type; // Тип автомобиля
        
        // Проверка на корректность введённых данных 
        while (true)
        {
            type = Console.ReadLine();
            if (type?.ToLower() == "спорткар" || type?.ToLower() == "электромобиль" || type?.ToLower() == "обычный")
                break;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Вводимые данные некорректны. Попробуйте снова:");
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        Console.WriteLine("Введите уровень топлива:");
        double level; // Уровень топлива в автомобиле
        
        // Проверка на корректность введённых данных. 
        while (true)
        {
            try
            {
                level = Convert.ToDouble(Console.ReadLine());
                if (level is < 0 or > 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Такого уровня топлива быть не может. Введите данные повторно:");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else break;
            }
            catch 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Данные некоректны, повторите попытку.");
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
        }

        string? fuelType; // Вид топлива
        Console.WriteLine("Введите тип топлива (бензин/дизель/газ/вода/электричество):");
        
        // Проверка на корректность введённых данных. 
        while (true)
        {
            fuelType = Console.ReadLine();
            if (fuelType?.ToLower() == "бензин" || fuelType?.ToLower() == "дизель" || fuelType?.ToLower() == "газ"
                || fuelType?.ToLower() == "вода" || fuelType?.ToLower() == "электричество")
            {
                // Проверка вида топлива при введённом виде автомобиля 'электромобиль'
                if (fuelType.ToLower() != "электричество" && type.ToLower() == "электромобиль")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Электромобиль не может использовать {fuelType}. Введите данные повторно:");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else break;
            }
            
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Такого вида топлива база не знает. Введите данные повторно:");
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            
        }

        int price; // Цена аренды автомобиля
        Console.WriteLine("Введите целочисленное знаение цены (цена/мин):");
        
        // Проверка на корректность введённых данных. 
        while (true)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                price = Convert.ToInt32(Console.ReadLine());
                break;
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Введите цену в целом числе. Введите данные повторно:");
            }
        }

        // Подключение к файлу
        Connection.Open();
        
        // SQL-запрос на добавление данных в базу
        _command = new SQLiteCommand
        (
            "INSERT INTO rent_cars (model, type, fuel_level, fuel_type, price) " +
            "VALUES (@model, @type, @fuel_level, @fuel_type, @price)",
            Connection
        );

        // Установка значений у параметров
        _command.Parameters.AddWithValue("@model", model);
        _command.Parameters.AddWithValue("@type", type);
        _command.Parameters.AddWithValue("@fuel_level", level);
        _command.Parameters.AddWithValue("@fuel_type", fuelType);
        _command.Parameters.AddWithValue("@price", price);

        // Попытка загрузить в базу данные
        try
        {
            _command.Prepare();
            _command.ExecuteNonQuery();
            
            TextAnimation("Добавление данных");
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.CursorVisible = true;
            Console.WriteLine("\nДанные успешно добавлены.\n");
            Thread.Sleep(2000);
            Console.WriteLine(MenuAdapter.MenuButtons()); // Возращение меню
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ошибка добавления данных.");
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    ///  Вывод информации об автомобиле
    /// </summary>
    public static void GetInfoAboutCarsFromDb()
    {
        // Подключение к файлу
        Connection.Open();

        // SQL-запрос
        _command = new SQLiteCommand
        (
                "SELECT * FROM rent_cars"
                , Connection
        );

        Console.ForegroundColor = ConsoleColor.Magenta;
        _dataReader = _command.ExecuteReader();
        
        TextAnimation("Загрузка");

        Console.CursorVisible = true;
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        
        // Проверка на наличие строк в reader'e
        if (_dataReader.HasRows)
        {
            while (_dataReader.Read())
            {
                switch (MenuAdapter.AccessLvl)
                {
                    case AccessLevel.Developer:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Car ID: {_dataReader["id"]}," +
                                          $" МОДЕЛЬ: {_dataReader["model"]}," +
                                          $" ТИП АВТОМОБИЛЯ: {_dataReader["type"]}," +
                                          $" ТИП ТОПЛИВА: {_dataReader["fuel_type"]}," +
                                          $" УРОВЕНЬ ТОПЛИВА: {_dataReader["fuel_level"]}," +
                                          $" ЦЕНА (РУБ/МИН): {_dataReader["price"]}," +
                                          $" ID АРЕНДАТОРА: {_dataReader["renter_id"]}," +
                                          $" ИМЯ АРЕНДАТОРА: {_dataReader["renter_name"]};");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case AccessLevel.User:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"Car ID: {_dataReader["id"]}," +
                                          $" МОДЕЛЬ: {_dataReader["model"]}," +
                                          $" ТИП АВТОМОБИЛЯ: {_dataReader["type"]}," +
                                          $" ТИП ТОПЛИВА: {_dataReader["fuel_type"]}," +
                                          $" УРОВЕНЬ ТОПЛИВА: {_dataReader["fuel_level"]}," +
                                          $" ЦЕНА (РУБ/МИН): {_dataReader["price"]}" +
                                          $" ДОСТУПНОСТЬ: {_dataReader["is_available"]}");
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    default:
                        Console.WriteLine();
                        break;
                }
                
                // Задержка для красивого вывода
                Thread.Sleep(250);
            }
        }
        else
        {
            Console.WriteLine("К сожалению, у нас в каталоге нет машин.");
            _dataReader.Close();
            _dataReader.Close();
        }
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n" + MenuAdapter.MenuButtons()); // Вывод меню
        
        _dataReader.Close();
        Connection.Close();
    }

    /// <summary>
    /// Изменение информации об автомобиле
    /// </summary>
    public static void ChangeInfoInDb()
    {
        // Подключение к файлу
        Connection.Open();

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Информацию о какой машине вы хотите изменить? (Введите её ID)");
        var inputId = Console.ReadLine(); // ID автомобиля

        while (true)
        {
            // Проверка, что введено было число
            try
            {
                var id = Convert.ToInt32(inputId);
                Console.WriteLine("Сколько параметров вы хотите поменять?");
                var inputCount = Console.ReadLine(); // Количество изменений
                
                // Проверка, что введено было число
                try
                {
                    var count = Convert.ToInt32(inputCount);
                    
                    // Запуск цикла для ввода данных
                    for (var i = 0; i < count; i++)
                    {
                        Console.WriteLine("Введите имя поля, которое хотите поменять:");
                        var inputField = Console.ReadLine(); // Имя поля
                        string? inputNewData;
                        Console.WriteLine("Введите новые данные:");

                        while (true)
                        {
                            inputNewData = Console.ReadLine(); // Новое значение
                            if (inputField == "is_available" && (inputNewData == "YES" || inputNewData == "NO")) break;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Для данного поля введены некоректная. Повторите попытку:");
                        }
                        
                        // Попытка изменить дынные
                        try {
                            // SQL-запрос
                            _command = new SQLiteCommand
                            (
                                $"UPDATE rent_cars SET {inputField} = @newData WHERE id = {id}",
                                Connection
                            );

                            _command.Parameters.AddWithValue("@newData", inputNewData);
                            
                            _command.ExecuteNonQuery();

                            TextAnimation("Изменение данных");
                            
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Данные успешно изменены.");
                        }
                        catch
                        {
                            Console.ForegroundColor = ConsoleColor.Red; 
                            Console.WriteLine(
                                    "Хмммм... Произошла ошибка. Проверьте правильность написанных данных и " +
                                    "попробуйте снова.");
                            Console.ForegroundColor = ConsoleColor.Blue;
                        }
                    }

                    
                    Thread.Sleep(2000);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\n" + MenuAdapter.MenuButtons()); // Возращение меню
                    
                    Connection.Close();
                    return;
                }
                catch 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Введите данные корректно.");
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
            }
            catch 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Введите данные корректно.");
                Console.ForegroundColor = ConsoleColor.Blue;
            }
        }
    }

    /// <summary>
    /// Получение данных из базы о доступных для аренды автомобилей
    /// </summary>
    public static void GetDataForRentCar()
    {
        // Подключение к файлу
        Connection.Open();

        _command = new SQLiteCommand
        (
            "SELECT * FROM rent_cars WHERE is_available = 'YES'",
            Connection
        );

        _dataReader = _command.ExecuteReader();
        
        // Проверка на наличие доступных машин
        if (_dataReader.HasRows)
        {
            Console.WriteLine("Автомобили, которые доступны в данный момент:");
            
            TextAnimation("Загрузка");

            Console.CursorVisible = true;
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.ForegroundColor = ConsoleColor.Magenta;
            
            while (_dataReader.Read())
            {
                Console.WriteLine($"Car ID: {_dataReader["id"]}," +
                                  $" МОДЕЛЬ: {_dataReader["model"]}," +
                                  $" ТИП АВТОМОБИЛЯ: {_dataReader["type"]}," +
                                  $" ТИП ТОПЛИВА: {_dataReader["fuel_type"]}," +
                                  $" УРОВЕНЬ ТОПЛИВА: {_dataReader["fuel_level"]}," +
                                  $" ЦЕНА (РУБ/МИН): {_dataReader["price"]}"
                                  );
                
                // Задержка для красивого вывода
                Thread.Sleep(250);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Введите ID автомобиля, который хотите взять в аренду:");

            while (true)
            {
                var id = Console.ReadLine();
            
                // Проверка на корректность введённых данных
                try
                {
                    Console.WriteLine("Введите Ваше имя:");
                    var name = Console.ReadLine();
                    Console.WriteLine("Введите Ваш ID:");
                    var userId = Console.ReadLine();

                    _command = new SQLiteCommand
                    (
                        $"UPDATE rent_cars SET is_available = 'NO', renter_name = @name, renter_id = @renter_id WHERE id = {Convert.ToInt32(id)}",
                        Connection
                    );

                    _command.Parameters.AddWithValue("@name", name);
                    _command.Parameters.AddWithValue("@renter_id", Convert.ToInt32(userId));
                    
                    _command.ExecuteNonQueryAsync();
                    
                    TextAnimation("Загрузка");

                    Console.CursorVisible = true;
                    
                    _command = new SQLiteCommand
                    (
                        $"SELECT * FROM rent_cars WHERE id = {Convert.ToInt32(id)}",
                        Connection
                    );

                    _dataReader = _command.ExecuteReader();

                    while (_dataReader.Read())
                    {
                        Console.WriteLine($"Вы арендовали автомобиль {_dataReader["model"]} с ID {_dataReader["id"]} " + 
                                          $"стоимостью {_dataReader["price"]}руб/мин. Приятной поездки!\n");
                    }
                    
                    Console.WriteLine(MenuAdapter.MenuButtons());
                    
                    _dataReader.Close();
                    Connection.Close();
                    return;
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e);
                    Console.WriteLine("Хммм... Вы ввели некорректные данные, попробуйте еще раз");
                    Console.ForegroundColor = ConsoleColor.Green;
                }
            }
            
        }
        
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("К сожалению, в данный момент нет свободных автомобилей.");
        Console.ForegroundColor = ConsoleColor.Green;
    }

    /// <summary>
    /// Функция создания таблицы при запуске программы, если её нет в базе
    /// </summary>
    public static void CreateDb()
    {
        // Подключение к файлу
        var connection = new SQLiteConnection("Data Source=database");
        connection.Open();

        // SQL-запрос
        var createDb = new SQLiteCommand
        (
            @"create table if not exists rent_cars
                    (
                        id              integer
                        constraint rent_cars_pk
                        primary key autoincrement,
                        model           text not null,
                        type            text,
                        fuel_level      real,
                        fuel_type       text,
                        price           integer,
                        is_available    text default 'YES' not null,
                        renter_id       integer default 0,
                        renter_name     text    default 'None'
                    );", 
            connection
        );
        
        createDb.ExecuteNonQuery();
        connection.Close();
    }
    /// <summary>
    /// Вывод анимированного текста - изменение строки путё добавления точек в конце для анимации
    /// </summary>
    /// <param name="text">Текст, который нужно заанимировать</param>
    private static void TextAnimation(string text)
    {
        Console.WriteLine(text);
        Thread.Sleep(500);
        for (var j = 0; j < 3; j++) 
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine(text += '.');
            Thread.Sleep(500);
        }
        Console.CursorVisible = true;
    }
}