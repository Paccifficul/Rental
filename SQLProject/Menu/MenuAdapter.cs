using SQLProject.Enum;
using SQLProject.ModelDB;

namespace SQLProject.Menu;


public static class MenuAdapter
{

    public static AccessLevel AccessLvl { get; set; }


    public static void RentCar()
    {
        DbExecutive.GetDataForRentCar();
    }

    public static void RentCatalog()
    {
        DbExecutive.GetInfoAboutCarsFromDb();
    }

    public static void Exit()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("До свидания, приходите к нам еще!");
        Environment.Exit(0);
    }

    /// <summary>
    /// Запуск функций для разработчика
    /// </summary>
    public static void DeveloperOptions()
    {
        AccessLvl = AccessLevel.Developer;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("***ВЫ В РЕЖИМЕ РАЗРАБОТЧИКА***");
        Console.ForegroundColor = ConsoleColor.Cyan;
        
        Menu.ButtonsDictionary.Clear();
        Menu.ButtonsDictionary.Add("0", Exit);
        Menu.ButtonsDictionary.Add("1", DbExecutive.AddCarsToDb);
        Menu.ButtonsDictionary.Add("2", DbExecutive.ChangeInfoInDb);
        Menu.ButtonsDictionary.Add("3", DbExecutive.GetInfoAboutCarsFromDb);
        
        Console.WriteLine(MenuButtons());
    }

    /// <summary>
    /// Установка текста меню в зависимости от уровня доступа
    /// </summary>
    /// <returns>Строку со списком командами</returns>
    public static string MenuButtons()
    {
        return AccessLvl switch
        {
            (AccessLevel.User) => "0) Выйти из программы\n" +
                                  "1) Взять автомобиль в аренду\n" +
                                       "2) Каталог автомобилей\n" +
                                       "3) Настройки разработчика",
            (AccessLevel.Developer) => "0) Выйти из программы\n" +
                                       "1) Добавить в базу данных новые автомобили\n" +
                                  "2) Изменить значения в базе данных\n" + 
                                  "3) Вывести содержимое базы данных",
            _ => ""
        };
    }
}