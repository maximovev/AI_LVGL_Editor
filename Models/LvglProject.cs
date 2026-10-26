using System;
using System.Collections.Generic;
using System.IO;

namespace LvglProjectEditor.Models
{
    public class LvglProject
    {
        // Базовая информация
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        
        // Версия проекта
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public int PatchVersion { get; set; }
        
        // Идентификаторы
        public string ProjectId { get; set; }
        public string LvlFilePath { get; set; }
        public string CfgFilePath { get; set; }
        public string CssFilePath { get; set; }
        
        // Настройки проекта
        public string TargetDevice { get; set; }
        public string ScreenResolution { get; set; }
        public int RefreshRate { get; set; }
        public string Microcontroller { get; set; }
        
        // Зависимости
        public List<string> Dependencies { get; set; }
        public List<string> Includes { get; set; }
        
        // Ссылки на файлы
        public List<LvglFile> Files { get; set; }
        
        // Статус
        public bool IsModified { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        
        // Конструктор
        public LvglProject()
        {
            Files = new List<LvglFile>();
            Dependencies = new List<string>();
            Includes = new List<string>();
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
        }
        
        // Конструктор с параметрами
        public LvglProject(string name, string path, string lvglFilePath)
        {
            Name = name;
            Path = path;
            LvlFilePath = lvglFilePath;
            Files = new List<LvglFile>();
            Dependencies = new List<string>();
            Includes = new List<string>();
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
        }
        
        // Сохранение проекта в файл
        public void SaveToFile(string filePath)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(this, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
            
            File.WriteAllText(filePath, json);
            LastModified = DateTime.Now;
            IsModified = false;
        }
        
        // Загрузка проекта из файла
        public static LvglProject LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                return null;
            
            var json = File.ReadAllText(filePath);
            return System.Text.Json.JsonSerializer.Deserialize<LvglProject>(json);
        }
        
        // Добавление файла
        public void AddFile(LvglFile file)
        {
            Files.Add(file);
            IsModified = true;
        }
        
        // Удаление файла
        public bool RemoveFile(string fileName)
        {
            var file = Files.FirstOrDefault(f => f.Name == fileName);
            if (file != null)
            {
                Files.Remove(file);
                IsModified = true;
                return true;
            }
            return false;
        }
        
        // Проверка валидности
        public void Validate()
        {
            IsValid = true;
            ErrorMessage = null;
            
            if (string.IsNullOrEmpty(Name))
            {
                IsValid = false;
                ErrorMessage = "Имя проекта не задано";
            }
            
            if (string.IsNullOrEmpty(LvlFilePath) && !Files.Any())
            {
                IsValid = false;
                ErrorMessage = "Нет файлов LVGL проекта";
            }
        }
        
        // Получение версии строки
        public string GetVersionString()
        {
            return $"{MajorVersion}.{MinorVersion}.{PatchVersion}";
        }
    }
}