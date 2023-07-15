using System;

public static class LanguageManager
{
    public static string CurrentLanguage = "RU";

    public static event Action OnChangeLanguage;

    public static void SetLanguage(string language)
    {
        CurrentLanguage = language;
        OnChangeLanguage?.Invoke();
    }
}