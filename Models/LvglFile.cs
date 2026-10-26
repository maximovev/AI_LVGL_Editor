using System;
using System.Collections.Generic;

namespace LvglProjectEditor.Models
{
    public class LvglFile
    {
        // Базовая информация
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Type { get; set; }
        public string Extension { get; set; }
        public string Description { get; set; }
        
        // Путь к файлу
        public string Path { get; set; }
        public string RelativePath { get; set; }
        
        // Размер и дата
        public long Size { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        
        // Типы файлов LVGL
        public FileTypes FileTypeCategory { get; set; }
        public bool IsMainFile { get; set; }
        public bool IsEditable { get; set; }
        public bool IsReadOnly { get; set; }
        
        // Зависимости
        public List<string> Dependencies { get; set; }
        public List<string> Includes { get; set; }
        
        // Метаданные
        public string Author { get; set; }
        public string Version { get; set; }
        public string License { get; set; }
        
        // Статус
        public bool IsModified { get; set; }
        public bool IsSaved { get; set; }
        public string ErrorMessage { get; set; }
        
        // Конструктор
        public LvglFile()
        {
            Dependencies = new List<string>();
            Includes = new List<string>();
            IsSaved = true;
        }
        
        // Конструктор с параметрами
        public LvglFile(string name, string type, string fullPath, FileTypes fileTypeCategory = FileTypes.Other)
        {
            Name = name;
            FullName = fullPath;
            Type = type;
            Extension = Path.GetExtension(fullPath)?.ToLowerInvariant() ?? string.Empty;
            FileTypeCategory = fileTypeCategory;
            IsSaved = true;
            
            Path = Path.GetDirectoryName(fullPath) ?? string.Empty;
            RelativePath = Path.GetRelativePath(Path, fullPath) ?? fullPath;
            
            Dependencies = new List<string>();
            Includes = new List<string>();
        }
        
        // Определение типа файла
        public static FileTypes GetFileTypeCategory(string extension)
        {
            return extension switch
            {
                ".lv" => FileTypes.Source,
                ".lv_c" => FileTypes.Source,
                ".lvh" => FileTypes.Header,
                ".h" => FileTypes.Header,
                ".css" => FileTypes.CascadingStyleSheets,
                ".c" => FileTypes.C,
                ".s" => FileTypes.Assembler,
                ".bin" => FileTypes.Binary,
                ".png" => FileTypes.Image,
                ".gif" => FileTypes.Image,
                ".jpg" => FileTypes.Image,
                ".bmp" => FileTypes.Image,
                ".ttf" => FileTypes.Font,
                ".otf" => FileTypes.Font,
                _ => FileTypes.Other
            };
        }
        
        // Сохранение файла
        public void Save()
        {
            IsModified = false;
            IsSaved = true;
            LastModified = DateTime.Now;
        }
        
        // Добавление зависимости
        public void AddDependency(string dependency)
        {
            if (!Dependencies.Contains(dependency))
            {
                Dependencies.Add(dependency);
                IsModified = true;
            }
        }
        
        // Добавление include
        public void AddInclude(string include)
        {
            if (!Includes.Contains(include))
            {
                Includes.Add(include);
                IsModified = true;
            }
        }
        
        // Удаление зависимости
        public bool RemoveDependency(string dependency)
        {
            var index = Dependencies.IndexOf(dependency);
            if (index >= 0)
            {
                Dependencies.RemoveAt(index);
                IsModified = true;
                return true;
            }
            return false;
        }
        
        // Удаление include
        public bool RemoveInclude(string include)
        {
            var index = Includes.IndexOf(include);
            if (index >= 0)
            {
                Includes.RemoveAt(index);
                IsModified = true;
                return true;
            }
            return false;
        }
        
        // Получение относительного пути к файлу
        public string GetRelativePath(string baseDir)
        {
            if (Path.IsPathRooted(baseDir))
            {
                return Path.GetRelativePath(baseDir, FullName);
            }
            return RelativePath;
        }
        
        // Получение имени файла без расширения
        public string GetFileNameWithoutExtension()
        {
            return Path.GetFileNameWithoutExtension(Name);
        }
        
        // Получение расширения файла
        public string GetFileExtension()
        {
            return Extension;
        }
        
        // Проверка валидности
        public void Validate()
        {
            IsModified = false;
            ErrorMessage = null;
            
            if (string.IsNullOrEmpty(Name))
            {
                IsModified = false;
                ErrorMessage = "Имя файла не задано";
                return;
            }
            
            if (!IsEditable && FileTypeCategory == FileTypes.Source)
            {
                IsModified = false;
                ErrorMessage = "Источник файлов должен быть редактируемым";
                return;
            }
        }
        
        // Форматирование размера
        public string FormatSize()
        {
            if (Size < 1024)
                return $"{Size} байт";
            else if (Size < 1024 * 1024)
                return $"{Size / 1024F:F2} КБ";
            else
                return $"{Size / (1024 * 1024):F2} МБ";
        }
    }
    
    public enum FileTypes
    {
        Source,         // .lv, .lv_c
        Header,         // .lvh, .h
        CascadingStyleSheets, // .css
        C,              // .c
        Assembler,      // .s
        Binary,         // .bin
        Image,          // .png, .jpg, .gif, .bmp
        Font,           // .ttf, .otf
        Other           // другие типы
    }
}