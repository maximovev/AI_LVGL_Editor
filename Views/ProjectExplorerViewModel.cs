using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LvglProjectEditor.Models;
using LvglProjectEditor.Services;

namespace LvglProjectEditor.ViewModels
{
    public class ProjectExplorerViewModel
    {
        private readonly IProjectService _projectService;
        private readonly IFileService _fileService;
        
        // Связанные данные
        public List<LvglProject> Projects { get; set; }
        public LvglProject SelectedProject { get; set; }
        
        // Вводные данные
        public string NewProjectName { get; set; }
        public string NewProjectDescription { get; set; }
        
        // Состояние
        public bool IsProjectModified { get; set; }
        public string ErrorMessage { get; set; }
        public int SelectedIndex { get; set; }
        
        public ProjectExplorerViewModel()
        {
            _projectService = new ProjectService();
            _fileService = new FileService();
            
            Projects = new List<LvglProject>();
            SelectedProject = null;
            
            LoadProjects();
        }
        
        public async void LoadProjects()
        {
            try
            {
                Projects = await _projectService.GetProjectsAsync();
                
                if (Projects.Count > 0)
                {
                    SelectedProject = Projects[0];
                    SelectedIndex = 0;
                }
                else
                {
                    SelectedProject = null;
                    SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при загрузке проектов: {ex.Message}";
            }
        }
        
        public async Task<string> GetNewProjectName()
        {
            return NewProjectName;
        }
        
        public async Task<string> GetNewProjectDescription()
        {
            return NewProjectDescription;
        }
        
        public async Task<bool> CreateNewProjectAsync(string name, string description = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                ErrorMessage = "Пожалуйста, введите имя проекта";
                return false;
            }
            
            try
            {
                var project = new LvglProject
                {
                    Name = name,
                    Description = description ?? string.Empty,
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
                    SelectedIndex = Projects.Count - 1;
                    NewProjectName = string.Empty;
                    NewProjectDescription = string.Empty;
                    IsProjectModified = false;
                    ErrorMessage = null;
                    
                    return true;
                }
                
                ErrorMessage = "Не удалось создать проект";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при создании проекта: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> SelectProjectAsync(string projectId)
        {
            var project = Projects.FirstOrDefault(p => p.ProjectId == projectId);
            
            if (project != null)
            {
                SelectedProject = project;
                SelectedIndex = Projects.IndexOf(project);
                ErrorMessage = null;
                return true;
            }
            
            ErrorMessage = "Проект не найден";
            return false;
        }
        
        public async Task<bool> SelectProjectByNameAsync(string projectName)
        {
            var project = Projects.FirstOrDefault(p => p.Name == projectName);
            
            if (project != null)
            {
                SelectedProject = project;
                SelectedIndex = Projects.IndexOf(project);
                ErrorMessage = null;
                return true;
            }
            
            ErrorMessage = "Проект не найден";
            return false;
        }
        
        public async Task<bool> SaveProjectAsync()
        {
            if (SelectedProject == null)
            {
                ErrorMessage = "Нет открытого проекта";
                return false;
            }
            
            try
            {
                var isValid = await _projectService.ValidateProjectAsync(SelectedProject);
                
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
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при сохранении проекта: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> OpenProjectAsync(string filePath)
        {
            try
            {
                var project = await _projectService.LoadProjectFromFileAsync(filePath);
                
                if (project != null)
                {
                    SelectedProject = project;
                    Projects.Add(project);
                    
                    Files = await _fileService.GetFilesAsync();
                    
                    IsProjectModified = false;
                    ErrorMessage = null;
                    
                    return true;
                }
                
                ErrorMessage = "Не удалось открыть проект";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при открытии проекта: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> DeleteProjectAsync(string projectId)
        {
            var index = Projects.FindIndex(p => p.ProjectId == projectId);
            
            if (index >= 0)
            {
                var project = Projects[index];
                
                try
                {
                    await _projectService.DeleteProjectAsync(projectId);
                    
                    Projects.RemoveAt(index);
                    
                    if (SelectedProject == project)
                    {
                        if (Projects.Count > 0)
                        {
                            SelectedProject = Projects[0];
                            SelectedIndex = 0;
                        }
                        else
                        {
                            SelectedProject = null;
                            SelectedIndex = -1;
                        }
                    }
                    
                    IsProjectModified = false;
                    ErrorMessage = null;
                    
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Ошибка при удалении проекта: {ex.Message}";
                    return false;
                }
            }
            
            ErrorMessage = "Проект не найден";
            return false;
        }
        
        public async Task<bool> ExportProjectAsync(LvglProject project, string format, string filePath)
        {
            try
            {
                var success = await _projectService.ExportProjectAsync(project, format, filePath);
                
                if (success)
                {
                    return true;
                }
                
                ErrorMessage = $"Не удалось экспортировать проект в формат {format}";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при экспорте проекта: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> ImportProjectAsync(string filePath)
        {
            try
            {
                var success = await _projectService.ImportProjectAsync(filePath, out var project);
                
                if (success && project != null)
                {
                    SelectedProject = project;
                    Projects.Add(project);
                    IsProjectModified = false;
                    ErrorMessage = null;
                    
                    return true;
                }
                
                ErrorMessage = "Не удалось импортировать проект";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при импорте проекта: {ex.Message}";
                return false;
            }
        }
        
        public int ProjectsCount => Projects?.Count ?? 0;
        
        public bool HasSelectedProject => SelectedProject != null;
        
        public string SelectedProjectName => SelectedProject?.Name ?? "Нет проекта";
        
        public string SelectedProjectDescription => SelectedProject?.Description ?? string.Empty;
        
        public string SelectedProjectType => SelectedProject?.Type?.ToString() ?? "Неизвестно";
        
        public DateTime SelectedProjectCreatedDate => SelectedProject?.CreatedDate ?? DateTime.MinValue;
        
        public DateTime SelectedProjectLastModified => SelectedProject?.LastModified ?? DateTime.MinValue;
    }
}