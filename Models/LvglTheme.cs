using System;
using System.Collections.Generic;

namespace LvglProjectEditor.Models
{
    public class LvglTheme
    {
        // Уникальный идентификатор
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        // Родительская тема
        public string ParentThemeId { get; set; }
        public LvglTheme ParentTheme { get; set; }
        
        // Типы тем
        public ThemeType ThemeType { get; set; }
        public ThemeCategory Category { get; set; }
        
        // Базовые стили
        public List<LvglStyle> Styles { get; set; }
        
        // Настройки темы
        public int Brightness { get; set; }
        public int Contrast { get; set; }
        public int Temperature { get; set; }
        
        // Настройки цветов
        public ColorScheme ColorScheme { get; set; }
        
        // Настройки шрифтов
        public FontConfiguration FontConfig { get; set; }
        
        // Настройки анимации
        public AnimationConfiguration AnimationConfig { get; set; }
        
        // Метаданные
        public string Author { get; set; }
        public string Version { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        
        // Статус
        public bool IsModified { get; set; }
        public bool IsSaved { get; set; }
        public string ErrorMessage { get; set; }
        
        // Конструктор
        public LvglTheme()
        {
            Id = Guid.NewGuid().ToString("N");
            Styles = new List<LvglStyle>();
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
            IsSaved = true;
        }
        
        // Конструктор с параметрами
        public LvglTheme(string name, ThemeType themeType, ThemeCategory category)
        {
            Id = Guid.NewGuid().ToString("N");
            Name = name ?? string.Empty;
            ThemeType = themeType;
            Category = category;
            Styles = new List<LvglStyle>();
            
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
            IsSaved = true;
        }
        
        // Клонирование темы
        public LvglTheme Clone()
        {
            var clone = new LvglTheme
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = Name,
                Description = Description,
                ParentThemeId = ParentThemeId,
                ParentTheme = ParentTheme?.Clone(),
                ThemeType = ThemeType,
                Category = Category,
                Brightness = Brightness,
                Contrast = Contrast,
                Temperature = Temperature,
                ColorScheme = ColorScheme?.Clone(),
                FontConfig = FontConfig?.Clone(),
                AnimationConfig = AnimationConfig?.Clone(),
                Author = Author,
                Version = Version,
                CreatedDate = DateTime.Now,
                LastModified = DateTime.Now,
                IsModified = true,
                IsSaved = false
            };
            
            foreach (var style in Styles)
            {
                clone.Styles.Add(style.Clone());
            }
            
            return clone;
        }
        
        // Сохранение темы
        public void Save()
        {
            foreach (var style in Styles)
            {
                style.Save();
            }
            IsModified = false;
            IsSaved = true;
            LastModified = DateTime.Now;
        }
        
        // Применение темы к проекту
        public void ApplyToProject(LvglProject project)
        {
            foreach (var file in project.Files)
            {
                foreach (var element in file.Elements)
                {
                    foreach (var style in Styles)
                    {
                        if (style.TargetElementType == element.Type)
                        {
                            style.ApplyToElement(element);
                        }
                    }
                }
            }
        }
        
        // Получение всех стилей в иерархии
        public List<LvglStyle> GetAllStyles()
        {
            var styles = new List<LvglStyle>();
            styles.AddRange(Styles);
            
            if (ParentTheme != null)
            {
                styles.AddRange(ParentTheme.GetAllStyles());
            }
            
            return styles;
        }
        
        // Поиск стиля по типу элемента
        public LvglStyle GetStyleForElement(ElementType element)
        {
            foreach (var style in Styles)
            {
                if (style.TargetElementType == element)
                {
                    return style;
                }
            }
            return null;
        }
        
        // Валидация темы
        public void Validate()
        {
            IsModified = false;
            ErrorMessage = null;
            
            if (string.IsNullOrEmpty(Name))
            {
                IsModified = false;
                ErrorMessage = "Имя темы не задано";
                return;
            }
            
            if (Styles == null || Styles.Count == 0)
            {
                IsModified = false;
                ErrorMessage = "Тема не содержит стилей";
                return;
            }
            
            foreach (var style in Styles)
            {
                style.Validate();
            }
        }
        
        // Создание стиля для элемента
        public void CreateStyleForElement(ElementType element, string name = null)
        {
            var style = new LvglStyle
            {
                Name = name ?? $"{element.ToString()} Style",
                Type = element.ToString(),
                TargetElementType = element,
                IsModified = true
            };
            
            Styles.Add(style);
            IsModified = true;
        }
        
        // Удаление стиля
        public bool RemoveStyle(string styleId)
        {
            var index = Styles.FindIndex(s => s.Id == styleId);
            if (index >= 0)
            {
                Styles.RemoveAt(index);
                IsModified = true;
                return true;
            }
            return false;
        }
        
        // Обновление яркости
        public void UpdateBrightness(int value)
        {
            Brightness = value;
            IsModified = true;
        }
        
        // Обновление контраста
        public void UpdateContrast(int value)
        {
            Contrast = value;
            IsModified = true;
        }
        
        // Обновление температуры
        public void UpdateTemperature(int value)
        {
            Temperature = value;
            IsModified = true;
        }
        
        // Получение всех категорий темы
        public List<ThemeCategory> GetAllCategories()
        {
            var categories = new List<ThemeCategory>();
            categories.Add(Category);
            
            if (ParentTheme != null)
            {
                categories.AddRange(ParentTheme.GetAllCategories());
            }
            
            return categories;
        }
        
        // Форматирование информации о теме
        public string FormatInfo()
        {
            return $"Тема: {Name}\nТип: {ThemeType}\nКатегория: {Category}\nСтили: {Styles.Count}";
        }
    }
    
    public class ColorScheme
    {
        // Основные цвета
        public int PrimaryColor { get; set; }
        public int SecondaryColor { get; set; }
        public int AccentColor { get; set; }
        
        // Цвета интерфейса
        public int BackgroundColor { get; set; }
        public int SurfaceColor { get; set; }
        public int SurfaceVariantColor { get; set; }
        
        // Цвета текста
        public int PrimaryTextColor { get; set; }
        public int SecondaryTextColor { get; set; }
        public int DisabledTextColor { get; set; }
        
        // Цвета состояний
        public int SuccessColor { get; set; }
        public int WarningColor { get; set; }
        public int ErrorColor { get; set; }
        public int InfoColor { get; set; }
        
        // Цвета границ
        public int OutlineColor { get; set; }
        public int OutlineVariantColor { get; set; }
        
        // Настройки
        public bool IsDark { get; set; }
        public bool IsLight { get; set; }
        public string ThemeMode { get; set; }
        
        public ColorScheme()
        {
            IsDark = true;
            IsLight = false;
            ThemeMode = "dark";
        }
        
        public ColorScheme Clone()
        {
            return new ColorScheme
            {
                PrimaryColor = PrimaryColor,
                SecondaryColor = SecondaryColor,
                AccentColor = AccentColor,
                BackgroundColor = BackgroundColor,
                SurfaceColor = SurfaceColor,
                SurfaceVariantColor = SurfaceVariantColor,
                PrimaryTextColor = PrimaryTextColor,
                SecondaryTextColor = SecondaryTextColor,
                DisabledTextColor = DisabledTextColor,
                SuccessColor = SuccessColor,
                WarningColor = WarningColor,
                ErrorColor = ErrorColor,
                InfoColor = InfoColor,
                OutlineColor = OutlineColor,
                OutlineVariantColor = OutlineVariantColor,
                IsDark = IsDark,
                IsLight = IsLight,
                ThemeMode = ThemeMode
            };
        }
    }
    
    public class FontConfiguration
    {
        public int DefaultFontSize { get; set; }
        public int SmallFontSize { get; set; }
        public int LargeFontSize { get; set; }
        public int ExtraSmallFontSize { get; set; }
        public int ExtraLargeFontSize { get; set; }
        
        public int LineHeight { get; set; }
        public int LetterSpacing { get; set; }
        public int WordSpacing { get; set; }
        
        public int MaxTextWidth { get; set; }
        public int MinTextWidth { get; set; }
        
        public bool UseCustomFonts { get; set; }
        public List<string> CustomFontPaths { get; set; }
        
        public FontConfiguration()
        {
            DefaultFontSize = 16;
            SmallFontSize = 12;
            LargeFontSize = 24;
            ExtraSmallFontSize = 8;
            ExtraLargeFontSize = 32;
            LineHeight = 1.5;
            LetterSpacing = 0;
            WordSpacing = 0;
            MaxTextWidth = 400;
            MinTextWidth = 40;
        }
        
        public FontConfiguration Clone()
        {
            return new FontConfiguration
            {
                DefaultFontSize = DefaultFontSize,
                SmallFontSize = SmallFontSize,
                LargeFontSize = LargeFontSize,
                ExtraSmallFontSize = ExtraSmallFontSize,
                ExtraLargeFontSize = ExtraLargeFontSize,
                LineHeight = LineHeight,
                LetterSpacing = LetterSpacing,
                WordSpacing = WordSpacing,
                MaxTextWidth = MaxTextWidth,
                MinTextWidth = MinTextWidth,
                UseCustomFonts = UseCustomFonts,
                CustomFontPaths = CustomFontPaths?.Clone()
            };
        }
    }
    
    public class AnimationConfiguration
    {
        public int DefaultDuration { get; set; }
        public int DefaultDelay { get; set; }
        public int DefaultIterationCount { get; set; }
        
        public AnimationEasing DefaultEasing { get; set; }
        public AnimationEasing EasingEnter { get; set; }
        public AnimationEasing EasingExit { get; set; }
        
        public bool EnableTransitions { get; set; }
        public bool EnableAnimations { get; set; }
        public bool EnableMicroAnimations { get; set; }
        
        public int AnimationQuality { get; set; }
        public int AnimationSmoothness { get; set; }
        
        public AnimationConfiguration()
        {
            DefaultDuration = 300;
            DefaultDelay = 0;
            DefaultIterationCount = 1;
            EnableTransitions = true;
            EnableAnimations = true;
            EnableMicroAnimations = true;
            AnimationQuality = 1;
            AnimationSmoothness = 1;
        }
        
        public AnimationConfiguration Clone()
        {
            return new AnimationConfiguration
            {
                DefaultDuration = DefaultDuration,
                DefaultDelay = DefaultDelay,
                DefaultIterationCount = DefaultIterationCount,
                EnableTransitions = EnableTransitions,
                EnableAnimations = EnableAnimations,
                EnableMicroAnimations = EnableMicroAnimations,
                AnimationQuality = AnimationQuality,
                AnimationSmoothness = AnimationSmoothness
            };
        }
    }
    
    public enum ThemeType
    {
        Light,
        Dark,
        System,
        Custom,
        HighContrast,
        Accessibility
    }
    
    public enum ThemeCategory
    {
        Default,
        Professional,
        Creative,
        Gaming,
        Enterprise,
        Medical,
        Industrial,
        Consumer,
        Automotive,
        Aviation,
        Marine,
        Military,
        Educational,
        Healthcare,
        Finance,
        Retail,
        Hospitality,
        Entertainment,
        Sports,
        Weather,
        Navigation,
        Communication,
        Security,
        Utility,
        Other
    }
}