using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LvglProjectEditor.Models;
using LvglProjectEditor.Services;

namespace LvglProjectEditor.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly IProjectService _projectService;
        private readonly IFileService _fileService;
        private readonly IElementService _elementService;
        private readonly IStyleService _styleService;
        private readonly IThemeService _themeService;
        private readonly IParseService _parseService;
        private readonly IGeneratorService _generatorService;
        private readonly IValidationService _validationService;
        
        // Связанные данные
        public List<LvglProject> Projects { get; set; }
        public LvglProject SelectedProject { get; set; }
        public List<LvglFile> Files { get; set; }
        public List<LvglElement> Elements { get; set; }
        public List<LvglStyle> Styles { get; set; }
        public List<LvglTheme> Themes { get; set; }
        
        // Вводные данные
        public string NewProjectName { get; set; }
        public string NewProjectDescription { get; set; }
        
        // Состояние
        public bool IsProjectModified { get; set; }
        public string ErrorMessage { get; set; }
        
        public MainWindowViewModel()
        {
            _projectService = new ProjectService();
            _fileService = new FileService();
            _elementService = new ElementService();
            _styleService = new StyleService();
            _themeService = new ThemeService();
            _parseService = new ParseService();
            _generatorService = new GeneratorService();
            _validationService = new ValidationService();
            
            Projects = new List<LvglProject>();
            Files = new List<LvglFile>();
            Elements = new List<LvglElement>();
            Styles = new List<LvglStyle>();
            Themes = new List<LvglTheme>();
            
            LoadProjects();
        }
        
        public async void LoadProjects()
        {
            Projects = await _projectService.GetProjectsAsync();
            SelectedProject = Projects.FirstOrDefault();
        }
        
        public async Task<string> GetNewProjectName()
        {
            return NewProjectName;
        }
        
        public async Task<string> GetNewProjectDescription()
        {
            return NewProjectDescription;
        }
        
        public async Task<bool> CreateNewProjectAsync()
        {
            if (string.IsNullOrEmpty(NewProjectName))
            {
                ErrorMessage = "Пожалуйста, введите имя проекта";
                return false;
            }
            
            var project = new LvglProject
            {
                Name = NewProjectName,
                Description = NewProjectDescription,
                ProjectId = Guid.NewGuid().ToString("N"),
                Type = ProjectType.Default,
                Author = "User",
                CreatedDate = DateTime.Now,
                LastModified = DateTime.Now,
                IsModified = false,
                IsValid = true,
                ErrorMessage = null
            };
            
            var success = await _projectService.CreateProjectAsync(project);
            
            if (success)
            {
                Projects.Add(project);
                SelectedProject = project;
                NewProjectName = string.Empty;
                NewProjectDescription = string.Empty;
                IsProjectModified = false;
                ErrorMessage = null;
                
                return true;
            }
            
            ErrorMessage = "Не удалось создать проект";
            return false;
        }
        
        public async Task<bool> SaveProjectAsync()
        {
            if (SelectedProject == null)
            {
                ErrorMessage = "Нет открытого проекта";
                return false;
            }
            
            var isValid = await _validationService.ValidateProjectAsync(SelectedProject);
            
            if (!isValid)
            {
                ErrorMessage = "Пожалуйста, исправьте ошибки перед сохранением";
                return false;
            }
            
            var success = await _projectService.SaveProjectAsync(SelectedProject);
            
            if (success)
            {
                IsProjectModified = false;
                ErrorMessage = null;
                return true;
            }
            
            ErrorMessage = "Не удалось сохранить проект";
            return false;
        }
        
        public async Task<bool> OpenProjectAsync(string filePath)
        {
            var project = await _projectService.LoadProjectFromFileAsync(filePath);
            
            if (project != null)
            {
                SelectedProject = project;
                Projects.Add(project);
                Files = await _fileService.GetFilesAsync();
                Elements = await _elementService.GetElementsAsync();
                Styles = await _styleService.GetStylesAsync();
                Themes = await _themeService.GetThemesAsync();
                
                IsProjectModified = false;
                ErrorMessage = null;
                
                return true;
            }
            
            ErrorMessage = "Не удалось открыть проект";
            return false;
        }
        
        public async Task<bool> AddElementAsync(LvglElement element)
        {
            if (SelectedProject == null)
            {
                ErrorMessage = "Нет открытого проекта";
                return false;
            }
            
            element.Validate();
            if (!element.IsValid)
            {
                ErrorMessage = element.ErrorMessage;
                return false;
            }
            
            SelectedProject.Files?.Add(element);
            Elements.Add(element);
            IsProjectModified = true;
            
            return true;
        }
        
        public async Task<bool> UpdateElementAsync(LvglElement element)
        {
            if (SelectedProject == null)
            {
                ErrorMessage = "Нет открытого проекта";
                return false;
            }
            
            var index = Elements.FindIndex(e => e.Id == element.Id);
            if (index >= 0)
            {
                Elements[index] = element;
                IsProjectModified = true;
                return true;
            }
            
            ErrorMessage = "Элемент не найден";
            return false;
        }
        
        public async Task<bool> DeleteElementAsync(string elementId)
        {
            if (SelectedProject == null)
            {
                ErrorMessage = "Нет открытого проекта";
                return false;
            }
            
            var index = Elements.FindIndex(e => e.Id == elementId);
            if (index >= 0)
            {
                Elements.RemoveAt(index);
                IsProjectModified = true;
                return true;
            }
            
            ErrorMessage = "Элемент не найден";
            return false;
        }
        
        public async Task<bool> ApplyThemeAsync(LvglTheme theme)
        {
            if (SelectedProject == null)
            {
                ErrorMessage = "Нет открытого проекта";
                return false;
            }
            
            var success = await _themeService.ApplyThemeToProjectAsync(theme, SelectedProject);
            
            if (success)
            {
                IsProjectModified = true;
                ErrorMessage = null;
                return true;
            }
            
            ErrorMessage = "Не удалось применить тему";
            return false;
        }
        
        public async Task<string> GeneratePreviewAsync()
        {
            if (SelectedProject == null)
            {
                ErrorMessage = "Нет открытого проекта";
                return null;
            }
            
            var html = await _generatorService.GenerateHtmlAsync(SelectedProject);
            
            if (!string.IsNullOrEmpty(html))
            {
                IsProjectModified = true;
                return html;
            }
            
            ErrorMessage = "Не удалось сгенерировать предпросмотр";
            return null;
        }
        
        public async Task<string> GenerateCsvAsync()
        {
            if (SelectedProject == null)
            {
                ErrorMessage = "Нет открытого проекта";
                return null;
            }
            
            var csv = await _generatorService.GenerateCsvAsync(SelectedProject);
            
            if (!string.IsNullOrEmpty(csv))
            {
                IsProjectModified = true;
                return csv;
            }
            
            ErrorMessage = "Не удалось сгенерировать CSV";
            return null;
        }
        
        public bool HasChanges => IsProjectModified;
        
        public string ErrorText => ErrorMessage;
        
        public int ElementCount => Elements?.Count ?? 0;
        
        public int StyleCount => Styles?.Count ?? 0;
        
        public int ThemeCount => Themes?.Count ?? 0;
    }
}