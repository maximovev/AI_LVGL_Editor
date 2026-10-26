using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LvglProjectEditor.Models;

namespace LvglProjectEditor.Services
{
    public interface ILvglGenerator
    {
        Task<string> GenerateLvglCodeAsync(LvglProject project);
        Task<string> GenerateElementCodeAsync(LvglElement element);
        Task<string> GenerateStyleCodeAsync(LvglStyle style);
        Task<string> GenerateThemeCodeAsync(LvglTheme theme);
        Task<bool> ExportToHtmlAsync(LvglProject project, string filePath);
        Task<bool> ExportToCsvAsync(LvglProject project, string filePath);
    }
    
    public class LvglGenerator : ILvglGenerator
    {
        public async Task<string> GenerateLvglCodeAsync(LvglProject project)
        {
            var code = new StringBuilder();
            code.AppendLine("// LVGL Project: " + project.Name);
            code.AppendLine("// Generated " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            code.AppendLine();
            
            // Генерация кода для каждого элемента
            if (project.Elements != null)
            {
                foreach (var element in project.Elements)
                {
                    code.AppendLine("// Element: " + element.Name);
                    code.AppendLine(await GenerateElementCodeAsync(element));
                    code.AppendLine();
                }
            }
            
            return code.ToString();
        }
        
        public async Task<string> GenerateElementCodeAsync(LvglElement element)
        {
            var code = new StringBuilder();
            code.AppendLine($"// Element: {element.Name}");
            code.AppendLine($"// Type: {element.Type}");
            code.AppendLine($"// Position: X={element.X}, Y={element.Y}");
            code.AppendLine($"// Size: Width={element.Width}, Height={element.Height}");
            code.AppendLine();
            
            code.AppendLine($"lv_obj_t* {element.Name.ToLower()} = lv_obj_create(NULL);");
            code.AppendLine($"lv_obj_set_pos({element.Name.ToLower()}, {element.X}, {element.Y});");
            code.AppendLine($"lv_obj_set_size({element.Name.ToLower()}, {element.Width}, {element.Height});");
            
            return code.ToString();
        }
        
        public async Task<string> GenerateStyleCodeAsync(LvglStyle style)
        {
            var code = new StringBuilder();
            code.AppendLine($"// Style: {style.Name}");
            code.AppendLine($"// Type: {style.Type}");
            code.AppendLine();
            
            code.AppendLine($"lv_style_t {style.Name.ToLower()}_style = {{ 0 }};");
            code.AppendLine($"lv_style_set_bg_color(&{style.Name.ToLower()}_style, lv_color_hex(0xFFFFFF));");
            
            return code.ToString();
        }
        
        public async Task<string> GenerateThemeCodeAsync(LvglTheme theme)
        {
            var code = new StringBuilder();
            code.AppendLine($"// Theme: {theme.Name}");
            code.AppendLine($"// Description: {theme.Description}");
            code.AppendLine();
            
            code.AppendLine($"void apply_{theme.Name.ToLower()}_theme(void) {{");
            code.AppendLine($"    // Apply theme styles");
            code.AppendLine($"}}");
            
            return code.ToString();
        }
        
        public async Task<bool> ExportToHtmlAsync(LvglProject project, string filePath)
        {
            try
            {
                var html = $@"<!DOCTYPE html>
<html>
<head>
    <title>LVGL Project: {project.Name}</title>
</head>
<body>
    <h1>LVGL Project Preview</h1>
    <p>Project: {project.Name}</p>
    <p>Author: {project.Author}</p>
    <p>Elements: {project.Elements?.Count ?? 0}</p>
</body>
</html>";
                
                await File.WriteAllTextAsync(filePath, html);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при экспорте в HTML: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> ExportToCsvAsync(LvglProject project, string filePath)
        {
            try
            {
                var csv = new StringBuilder();
                csv.AppendLine("Name,Type,Author,LastModified");
                csv.AppendLine($"{project.Name},{project.Type},{project.Author},{project.LastModified:yyyy-MM-dd}");
                
                await File.WriteAllTextAsync(filePath, csv.ToString());
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при экспорте в CSV: {ex.Message}");
                return false;
            }
        }
    }
}