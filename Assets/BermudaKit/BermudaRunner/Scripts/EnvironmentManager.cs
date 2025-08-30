using Ali.Helper;

public class EnvironmentManager : LocalSingleton<EnvironmentManager>
{
    public static void ChangeColors(int i)
    {
        switch (i)
        {
            case 1:
                ThemeChanger.Instance.ChangeTheme(Theme.Normal);
                break;
            case 2:
                ThemeChanger.Instance.ChangeTheme(Theme.Orange);
                break;
            case 3:
                ThemeChanger.Instance.ChangeTheme(Theme.Purple);
                break;
            case 4:
                ThemeChanger.Instance.ChangeTheme(Theme.DarkBlue);
                break;
            case 5:
                ThemeChanger.Instance.ChangeTheme(Theme.Red);
                break;
        }
    }

}
