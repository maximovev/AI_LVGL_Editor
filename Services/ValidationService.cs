using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LvglProjectEditor.Models;

namespace LvglProjectEditor.Services
{
    public interface IValidationService
    {
        Task<bool> ValidateProjectAsync(LvglProject project);
        Task<bool> ValidateAllProjectsAsync();
        Task<bool> ValidateFileAsync(string filePath);
        Task<bool> ValidateAllFilesAsync();
        Task<bool> ValidateElementAsync(LvglElement element);
        Task<bool> ValidateAllElementsAsync();
        Task<bool> ValidateStyleAsync(LvglStyle style);
        Task<bool> ValidateAllStylesAsync();
        Task<bool> ValidateThemeAsync(LvglTheme theme);
        Task<bool> ValidateAllThemesAsync();
        Task<List<string>> GetValidationErrorsAsync();
    }
    
    public class ValidationService : IValidationService
    {
        private readonly List<string> _errors = new();
        
        public ValidationService() { }
        
        public async Task<bool> ValidateProjectAsync(LvglProject project)
        {
            return await Task.Run(() =>
            {
                project.Validate();
                _errors.Clear();
                
                if (!project.IsValid)
                {
                    _errors.Add(project.ErrorMessage);
                    return false;
                }
                
                if (project.Files != null)
                {
                    foreach (var file in project.Files)
                    {
                        file.Validate();
                        if (!file.IsValid)
                        {
                            _errors.Add($"Файл {file.Name}: {file.ErrorMessage}");
                        }
                    }
                }
                
                return true;
            });
        }
        
        public async Task<bool> ValidateAllProjectsAsync()
        {
            return await Task.Run(() =>
            {
                var projects = GetProjectsAsync().Result;
                var allValid = true;
                
                foreach (var project in projects)
                {
                    var isValid = ValidateProjectAsync(project).Result;
                    if (!isValid)
                    {
                        allValid = false;
                    }
                }
                
                return allValid;
            });
        }
        
        public async Task<bool> ValidateFileAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(filePath))
                {
                    _errors.Add($"Файл не существует: {filePath}");
                    return false;
                }
                
                _errors.Clear();
                return true;
            });
        }
        
        public async Task<bool> ValidateAllFilesAsync()
        {
            return await Task.Run(() =>
            {
                var files = GetFilesAsync().Result;
                var allValid = true;
                
                foreach (var file in files)
                {
                    var isValid = ValidateFileAsync(file).Result;
                    if (!isValid)
                    {
                        allValid = false;
                    }
                }
                
                return allValid;
            });
        }
        
        public async Task<bool> ValidateElementAsync(LvglElement element)
        {
            return await Task.Run(() =>
            {
                element.Validate();
                _errors.Clear();
                
                if (!element.IsValid)
                {
                    _errors.Add(element.ErrorMessage);
                    return false;
                }
                
                return true;
            });
        }
        
        public async Task<bool> ValidateAllElementsAsync()
        {
            return await Task.Run(() =>
            {
                var elements = GetElementsAsync().Result;
                var allValid = true;
                
                foreach (var element in elements)
                {
                    var isValid = ValidateElementAsync(element).Result;
                    if (!isValid)
                    {
                        allValid = false;
                    }
                }
                
                return allValid;
            });
        }
        
        public async Task<bool> ValidateStyleAsync(LvglStyle style)
        {
            return await Task.Run(() =>
            {
                style.Validate();
                _errors.Clear();
                
                if (!style.IsValid)
                {
                    _errors.Add(style.ErrorMessage);
                    return false;
                }
                
                return true;
            });
        }
        
        public async Task<bool> ValidateAllStylesAsync()
        {
            return await Task.Run(() =>
            {
                var styles = GetStylesAsync().Result;
                var allValid = true;
                
                foreach (var style in styles)
                {
                    var isValid = ValidateStyleAsync(style).Result;
                    if (!isValid)
                    {
                        allValid = false;
                    }
                }
                
                return allValid;
            });
        }
        
        public async Task<bool> ValidateThemeAsync(LvglTheme theme)
        {
            return await Task.Run(() =>
            {
                theme.Validate();
                _errors.Clear();
                
                if (!theme.IsValid)
                {
                    _errors.Add(theme.ErrorMessage);
                    return false;
                }
                
                return true;
            });
        }
        
        public async Task<bool> ValidateAllThemesAsync()
        {
            return await Task.Run(() =>
            {
                var themes = GetThemesAsync().Result;
                var allValid = true;
                
                foreach (var theme in themes)
                {
                    var isValid = ValidateThemeAsync(theme).Result;
                    if (!isValid)
                    {
                        allValid = false;
                    }
                }
                
                return allValid;
            });
        }
        
        public async Task<List<string>> GetValidationErrorsAsync()
        {
            return await Task.Run(() => _errors.ToList());
        }
    }
    
    private List<LvglProject> GetProjectsAsync()
    {
        return new List<LvglProject>();
    }
    
    private List<string> GetFilesAsync()
    {
        return new List<string>();
    }
    
    private List<LvglElement> GetElementsAsync()
    {
        return new List<LvglElement>();
    }
    
    private List<LvglStyle> GetStylesAsync()
    {
        return new List<LvglStyle>();
    }
}