using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LvglProjectEditor.Models;
using LvglProjectEditor.Services;

namespace LvglProjectEditor.ViewModels
{
    public class ThemeEditorViewModel
    {
        private readonly IThemeService _themeService;
        private readonly IValidationService _validationService;
        
        // Связанные данные
        public List<LvglTheme> Themes { get; set; }
        public LvglTheme SelectedTheme { get; set; }
        
        // Вводные данные
        public string ThemeName { get; set; }
        public string ThemeDescription { get; set; }
        public ThemeType ThemeType { get; set; }
        public ThemeCategory Category { get; set; }
        public int Brightness { get; set; }
        public int Contrast { get; set; }
        public int Temperature { get; set; }
        public int PrimaryColor { get; set; }
        public int SecondaryColor { get; set; }
        public int AccentColor { get; set; }
        public int BackgroundColor { get; set; }
        public int SurfaceColor { get; set; }
        public int TextColor { get; set; }
        public int DefaultFontSize { get; set; }
        public int SmallFontSize { get; set; }
        public int LargeFontSize { get; set; }
        public int DefaultDuration { get; set; }
        public int DefaultDelay { get; set; }
        public bool EnableTransitions { get; set; }
        public bool EnableAnimations { get; set; }
        public bool IsDark { get; set; }
        public bool IsLight { get; set; }
        
        // Состояние
        public bool IsThemeModified { get; set; }
        public bool IsThemeSaved { get; set; }
        public string ErrorMessage { get; set; }
        
        public ThemeEditorViewModel()
        {
            _themeService = new ThemeService();
            _validationService = new ValidationService();
            
            Themes = new List<LvglTheme>();
            SelectedTheme = null;
            
            LoadThemes();
        }
        
        public async void LoadThemes()
        {
            try
            {
                Themes = await _themeService.GetThemesAsync();
                
                if (Themes.Count > 0)
                {
                    SelectedTheme = Themes[0];
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при загрузке тем: {ex.Message}";
            }
        }
        
        public async Task<string> GetThemeName()
        {
            return ThemeName;
        }
        
        public async Task<string> GetThemeDescription()
        {
            return ThemeDescription;
        }
        
        public async Task<ThemeType> GetThemeType()
        {
            return ThemeType;
        }
        
        public async Task<int> GetBrightness()
        {
            return Brightness;
        }
        
        public async Task<int> GetContrast()
        {
            return Contrast;
        }
        
        public async Task<int> GetTemperature()
        {
            return Temperature;
        }
        
        public async Task<int> GetPrimaryColor()
        {
            return PrimaryColor;
        }
        
        public async Task<int> GetSecondaryColor()
        {
            return SecondaryColor;
        }
        
        public async Task<int> GetAccentColor()
        {
            return AccentColor;
        }
        
        public async Task<int> GetBackgroundColor()
        {
            return BackgroundColor;
        }
        
        public async Task<int> GetSurfaceColor()
        {
            return SurfaceColor;
        }
        
        public async Task<int> GetTextColor()
        {
            return TextColor;
        }
        
        public async Task<int> GetDefaultFontSize()
        {
            return DefaultFontSize;
        }
        
        public async Task<int> GetSmallFontSize()
        {
            return SmallFontSize;
        }
        
        public async Task<int> GetLargeFontSize()
        {
            return LargeFontSize;
        }
        
        public async Task<int> GetDefaultDuration()
        {
            return DefaultDuration;
        }
        
        public async Task<int> GetDefaultDelay()
        {
            return DefaultDelay;
        }
        
        public async Task<bool> CreateNewThemeAsync()
        {
            try
            {
                var theme = new LvglTheme
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = ThemeName ?? "New Theme",
                    Description = ThemeDescription ?? string.Empty,
                    ThemeType = ThemeType,
                    Category = Category,
                    Brightness = Brightness,
                    Contrast = Contrast,
                    Temperature = Temperature,
                    PrimaryColor = PrimaryColor,
                    SecondaryColor = SecondaryColor,
                    AccentColor = AccentColor,
                    BackgroundColor = BackgroundColor,
                    SurfaceColor = SurfaceColor,
                    TextColor = TextColor,
                    DefaultFontSize = DefaultFontSize,
                    SmallFontSize = SmallFontSize,
                    LargeFontSize = LargeFontSize,
                    DefaultDuration = DefaultDuration,
                    DefaultDelay = DefaultDelay,
                    IsDark = IsDark,
                    IsLight = IsLight,
                    IsSaved = true,
                    CreatedDate = DateTime.Now,
                    LastModified = DateTime.Now
                };
                
                var success = await _themeService.CreateThemeAsync(theme);
                
                if (success)
                {
                    Themes.Add(theme);
                    SelectedTheme = theme;
                    ThemeName = theme.Name;
                    ThemeDescription = theme.Description;
                    ThemeType = theme.ThemeType;
                    IsThemeModified = false;
                    IsThemeSaved = true;
                    ErrorMessage = null;
                    
                    return true;
                }
                
                ErrorMessage = "Не удалось создать тему";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при создании темы: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> UpdateThemeAsync()
        {
            if (SelectedTheme == null)
            {
                ErrorMessage = "Нет выбранной темы";
                return false;
            }
            
            try
            {
                SelectedTheme.Name = ThemeName;
                SelectedTheme.Description = ThemeDescription;
                SelectedTheme.ThemeType = ThemeType;
                SelectedTheme.Category = Category;
                SelectedTheme.Brightness = Brightness;
                SelectedTheme.Contrast = Contrast;
                SelectedTheme.Temperature = Temperature;
                SelectedTheme.PrimaryColor = PrimaryColor;
                SelectedTheme.SecondaryColor = SecondaryColor;
                SelectedTheme.AccentColor = AccentColor;
                SelectedTheme.BackgroundColor = BackgroundColor;
                SelectedTheme.SurfaceColor = SurfaceColor;
                SelectedTheme.TextColor = TextColor;
                SelectedTheme.DefaultFontSize = DefaultFontSize;
                SelectedTheme.SmallFontSize = SmallFontSize;
                SelectedTheme.LargeFontSize = LargeFontSize;
                SelectedTheme.DefaultDuration = DefaultDuration;
                SelectedTheme.DefaultDelay = DefaultDelay;
                SelectedTheme.IsDark = IsDark;
                SelectedTheme.IsLight = IsLight;
                
                var success = await _themeService.UpdateThemeAsync(SelectedTheme);
                
                if (success)
                {
                    IsThemeModified = false;
                    IsThemeSaved = true;
                    ErrorMessage = null;
                    return true;
                }
                
                ErrorMessage = "Не удалось обновить тему";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при обновлении темы: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> CloneThemeAsync(string newName = null)
        {
            if (SelectedTheme == null)
            {
                ErrorMessage = "Нет выбранной темы";
                return false;
            }
            
            try
            {
                var clone = await _themeService.CloneThemeAsync(SelectedTheme, newName ?? $"Clone of {SelectedTheme.Name}");
                
                if (clone != null)
                {
                    Themes.Add(clone);
                    SelectedTheme = clone;
                    IsThemeModified = false;
                    IsThemeSaved = false;
                    ErrorMessage = null;
                    
                    return true;
                }
                
                ErrorMessage = "Не удалось клонировать тему";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при клонировании темы: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> DeleteThemeAsync()
        {
            if (SelectedTheme == null)
            {
                ErrorMessage = "Нет выбранной темы";
                return false;
            }
            
            try
            {
                var success = await _themeService.DeleteThemeAsync(SelectedTheme.Id);
                
                if (success)
                {
                    Themes.Remove(SelectedTheme);
                    SelectedTheme = Themes.Count > 0 ? Themes[0] : null;
                    
                    IsThemeModified = false;
                    IsThemeSaved = true;
                    ErrorMessage = null;
                    
                    return true;
                }
                
                ErrorMessage = "Не удалось удалить тему";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при удалении темы: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> ApplyThemeToProjectAsync(LvglProject project)
        {
            if (SelectedTheme == null)
            {
                ErrorMessage = "Нет выбранной темы";
                return false;
            }
            
            try
            {
                var success = await _themeService.ApplyThemeToProjectAsync(SelectedTheme, project);
                
                if (success)
                {
                    IsThemeModified = true;
                    IsThemeSaved = false;
                    ErrorMessage = null;
                    return true;
                }
                
                ErrorMessage = "Не удалось применить тему к проекту";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при применении темы: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> ValidateThemeAsync()
        {
            if (SelectedTheme == null)
            {
                ErrorMessage = "Нет выбранной темы";
                return false;
            }
            
            try
            {
                var isValid = await _validationService.ValidateThemeAsync(SelectedTheme);
                
                if (isValid)
                {
                    IsThemeModified = false;
                    IsThemeSaved = true;
                    ErrorMessage = null;
                    return true;
                }
                
                ErrorMessage = "Тема не валидна";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при валидации темы: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> SaveThemeAsync()
        {
            if (SelectedTheme == null)
            {
                ErrorMessage = "Нет выбранной темы";
                return false;
            }
            
            try
            {
                var isValid = await _validationService.ValidateThemeAsync(SelectedTheme);
                
                if (isValid)
                {
                    var success = await _themeService.UpdateThemeAsync(SelectedTheme);
                    
                    if (success)
                    {
                        IsThemeModified = false;
                        IsThemeSaved = true;
                        ErrorMessage = null;
                        return true;
                    }
                    
                    ErrorMessage = "Не удалось сохранить тему";
                    return false;
                }
                
                ErrorMessage = "Тема не валидна";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при сохранении темы: {ex.Message}";
                return false;
            }
        }
        
        public async Task<string> GetThemeInfoAsync()
        {
            if (SelectedTheme == null)
            {
                return "Нет выбранной темы";
            }
            
            var info = $"ID: {SelectedTheme.Id}\n";
            info += $"Name: {SelectedTheme.Name}\n";
            info += $"Description: {SelectedTheme.Description}\n";
            info += $"Type: {SelectedTheme.ThemeType}\n";
            info += $"Category: {SelectedTheme.Category}\n";
            info += $"Brightness: {SelectedTheme.Brightness}%\n";
            info += $"Contrast: {SelectedTheme.Contrast}%\n";
            info += $"Temperature: {SelectedTheme.Temperature}°\n";
            info += $"Primary Color: {SelectedTheme.PrimaryColor:X8}\n";
            info += $"Secondary Color: {SelectedTheme.SecondaryColor:X8}\n";
            info += $"Accent Color: {SelectedTheme.AccentColor:X8}\n";
            info += $"Background Color: {SelectedTheme.BackgroundColor:X8}\n";
            info += $"Surface Color: {SelectedTheme.SurfaceColor:X8}\n";
            info += $"Text Color: {SelectedTheme.TextColor:X8}\n";
            info += $"Default Font Size: {SelectedTheme.DefaultFontSize}px\n";
            info += $"Small Font Size: {SelectedTheme.SmallFontSize}px\n";
            info += $"Large Font Size: {SelectedTheme.LargeFontSize}px\n";
            info += $"Default Duration: {SelectedTheme.DefaultDuration}ms\n";
            info += $"Default Delay: {SelectedTheme.DefaultDelay}ms\n";
            info += $"Dark Mode: {SelectedTheme.IsDark}\n";
            info += $"Light Mode: {SelectedTheme.IsLight}";
            
            return info;
        }
        
        public bool HasChanges => IsThemeModified;
        
        public bool IsSaved => IsThemeSaved;
        
        public string ErrorText => ErrorMessage;
        
        public int ThemesCount => Themes?.Count ?? 0;
        
        public string ThemeTypeName => ThemeType?.ToString() ?? "Unknown";
        
        public string ThemeCategoryName => Category?.ToString() ?? "Unknown";
        
        public int BrightnessMin => 0;
        
        public int BrightnessMax => 100;
        
        public int ContrastMin => 0;
        
        public int ContrastMax => 200;
        
        public int TemperatureMin => 1000;
        
        public int TemperatureMax => 7000;
        
        public List<ThemeType> AvailableThemeTypes => new List<ThemeType>
        {
            ThemeType.Light,
            ThemeType.Dark,
            ThemeType.System,
            ThemeType.HighContrast,
            ThemeType.Accessibility
        };
        
        public List<ThemeCategory> AvailableCategories => new List<ThemeCategory>
        {
            ThemeCategory.Default,
            ThemeCategory.Professional,
            ThemeCategory.Creative,
            ThemeCategory.Gaming,
            ThemeCategory.Enterprise,
            ThemeCategory.Healthcare,
            ThemeCategory.Finance,
            ThemeCategory.Retail,
            ThemeCategory.Hospitality
        };
        
        public int DefaultFontSizeMin => 8;
        
        public int DefaultFontSizeMax => 32;
        
        public int SmallFontSizeMin => 8;
        
        public int SmallFontSizeMax => 24;
        
        public int LargeFontSizeMin => 24;
        
        public int LargeFontSizeMax => 48;
    }
}