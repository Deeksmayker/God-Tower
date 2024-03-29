﻿public static class SettingsController
{
    public enum GrassQuality
    {
        High, 
        Medium,
        Low
    }

    public static int LastScene = 1;
    
    public static float Sensitivity = 0.1f;
    public static float SFXVolume = 0.5f;
    public static float AmbientVolume = 0.5f;
    public static GrassQuality GrassQualityValue = GrassQuality.High;
}
