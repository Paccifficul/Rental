using SQLProject.Enum;

namespace SQLProject.Menu;

public static class Menu
{
    // Словарь для обработки ввода
    public static Dictionary<string, Action> ButtonsDictionary { get; set; } = new();

    /// <summary>
    /// Заполнение словаря в начале программы
    /// </summary>
    static Menu()
    {
        ButtonsDictionary.Add("0", MenuAdapter.Exit);
        ButtonsDictionary.Add("1", MenuAdapter.RentCar);
        ButtonsDictionary.Add("2", MenuAdapter.RentCatalog);
        ButtonsDictionary.Add("3", MenuAdapter.DeveloperOptions);
    }
    
    /// <summary>
    /// Вывод верхней панели
    /// </summary>
    public static void Bar()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Дорбо пожаловать в центр проката автомобилей 'Rental'!\n" +
                          "У нас огромный выбор автомобилей разного уровня.");
    }
    
    /// <summary>
    /// Вывод кнопок
    /// </summary>
    public static void Buttons()
    {
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(MenuAdapter.MenuButtons());
    }

    /// <summary>
    /// Обработчик команд
    /// </summary>
     public static void ExecuteCommands()
     { 
         try
        {
            ButtonsDictionary[Console.ReadLine()!].Invoke(); // Вызов команду по введённому значению
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Введены некорректные данные, попробуйте ещё раз");

            Console.ForegroundColor = MenuAdapter.AccessLvl switch
            {
                AccessLevel.User => ConsoleColor.Green,
                AccessLevel.Developer => ConsoleColor.Cyan,
                _ => ConsoleColor.Green
            };
        }
    }
}