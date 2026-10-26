using System.Collections.ObjectModel;

namespace LVGLEditor.Models
{
    /// <summary>
    /// Модель LVGL проекта с файлами и ресурсами
    ///</summary>
    public class ProjectModel
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsModified { get; set; }
        public ObservableCollection<string> IncludedFiles { get; set; } = new();
        public ObservableCollection<ThemeModel> Themes { get; set; } = new();
        public ObservableCollection<FontModel> Fonts { get; set; } = new();
        public ObservableCollection<ScreenModel> Screens { get; set; } = new();
    }

    /// <summary>
    /// Модель темы
    ///</summary>
    public class ThemeModel
    {
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }

    /// <summary>
    /// Модель шрифта
    ///</summary>
    public class FontModel
    {
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int Size { get; set; }
        public bool IsCustom { get; set; }
    }

    /// <summary>
    /// Модель экрана
    ///</summary>
    public class ScreenModel
    {
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public ObservableCollection<string> Widgets { get; set; } = new();
    }
}