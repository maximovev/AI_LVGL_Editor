using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LvglProjectEditor.Models;

namespace LvglProjectEditor.Services
{
    public interface ILvglParser
    {
        Task<bool> ParseLvglFileAsync(string filePath);
        Task<bool> ParseLvglContentAsync(string content);
        Task<List<LvglElement>> ParseElementsAsync(string content);
        Task<string> ExtractElementInfoAsync(string content);
        Task<bool> ValidateLvglSyntaxAsync(string content);
    }
    
    public class LvglParser : ILvglParser
    {
        public async Task<bool> ParseLvglFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return false;
                }
                
                var content = await File.ReadAllTextAsync(filePath);
                return await ParseLvglContentAsync(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при парсинге LVGL файла: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> ParseLvglContentAsync(string content)
        {
            try
            {
                if (string.IsNullOrEmpty(content))
                {
                    return false;
                }
                
                var elements = await ParseElementsAsync(content);
                return elements != null && elements.Count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при парсинге LVGL содержимого: {ex.Message}");
                return false;
            }
        }
        
        public async Task<List<LvglElement>> ParseElementsAsync(string content)
        {
            var elements = new List<LvglElement>();
            
            // Поиск элементов по шаблонам
            var elementPatterns = new[]
            {
                new (RegexPattern, ElementType) { ("button", "Button"), ("checkbox", "Checkbox"), ("label", "Label"), ("slider", "Slider") }
            };
            
            foreach (var pattern in elementPatterns)
            {
                var matches = Regex.Matches(content, $@"<{pattern.Pattern}[^>]*>.*?</{pattern.Pattern}>", RegexOptions.Multiline);
                
                foreach (var match in matches)
                {
                    var element = new LvglElement
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Name = $"Element {elements.Count + 1}",
                        Type = pattern.ElementType.ToString(),
                        IsVisible = true,
                        IsEnabled = true
                    };
                    
                    elements.Add(element);
                }
            }
            
            return elements;
        }
        
        public async Task<string> ExtractElementInfoAsync(string content)
        {
            try
            {
                var info = new StringBuilder();
                info.AppendLine("LVGL Project Information:");
                info.AppendLine("========================");
                info.AppendLine($"Content Length: {content.Length} characters");
                info.AppendLine($"Detected Elements: {await ParseElementsAsync(content).Count}");
                info.AppendLine();
                
                return info.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при извлечении информации: {ex.Message}");
                return null;
            }
        }
        
        public async Task<bool> ValidateLvglSyntaxAsync(string content)
        {
            try
            {
                if (string.IsNullOrEmpty(content))
                {
                    return false;
                }
                
                // Проверка на правильные скобки
                if (content.Count(c => c == '<') != content.Count(c => c == '/'))
                {
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при валидации синтаксиса: {ex.Message}");
                return false;
            }
        }
    }
    
    public class RegexPattern
    {
        public string Pattern { get; set; }
        public ElementType ElementType { get; set; }
    }
}