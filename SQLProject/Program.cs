using SQLProject.Enum;
using SQLProject.Menu;
using SQLProject.ModelDB;

namespace SQLProject;

internal static class Program
{
    public static void Main()
    {
        DbExecutive.CreateDb();
        MenuAdapter.AccessLvl = AccessLevel.User; // Доступ к программе как пользователь
        Menu.Menu.Bar();
        Menu.Menu.Buttons();
        while (true)
        {
            Menu.Menu.ExecuteCommands();
        }
    }
}