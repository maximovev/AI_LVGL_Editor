using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LvglProjectEditor.Models;

namespace LvglProjectEditor.Services
{
    public interface IProjectService
    {
        Task<List<LvglProject>> GetProjectsAsync();
        Task<LvglProject> GetProjectAsync(string projectId);
        Task<LvglProject> GetProjectByNameAsync(string projectName);
        Task<bool> CreateProjectAsync(LvglProject project);
        Task<bool> UpdateProjectAsync(LvglProject project);
        Task<bool> DeleteProjectAsync(string projectId);
        Task<bool> SaveProjectAsync(LvglProject project);
        Task<bool> LoadProjectAsync(LvglProject project);
        Task<bool> ValidateProjectAsync(LvglProject project);
        Task<List<LvglProject>> SearchProjectsAsync(string query);
        Task<bool> ExportProjectAsync(LvglProject project, string format, string filePath);
        Task<bool> ImportProjectAsync(string filePath, out LvglProject project);
    }
    
    public class ProjectService : IProjectService
    {
        private readonly string _projectStoragePath;
        private readonly object _lock = new object();
        
        public ProjectService(string projectStoragePath = null)
        {
            _projectStoragePath = projectStoragePath ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".lvgl-projects");
            
            EnsureStoragePathExists();
        }
        
        private void EnsureStoragePathExists()
        {
            if (!Directory.Exists(_projectStoragePath))
            {
                Directory.CreateDirectory(_projectStoragePath);
            }
        }
        
        public async Task<List<LvglProject>> GetProjectsAsync()
        {
            var projects = new List<LvglProject>();
            
            if (!Directory.Exists(_projectStoragePath))
            {
                return projects;
            }
            
            try
            {
                var files = Directory.GetFiles(_projectStoragePath, "*.json");
                
                foreach (var file in files)
                {
                    var project = System.Text.Json.JsonSerializer.Deserialize<LvglProject>(File.ReadAllText(file));
                    if (project != null)
                    {
                        projects.Add(project);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке проектов: {ex.Message}");
            }
            
            return projects.OrderByDescending(p => p.LastModified).ToList();
        }
        
        public async Task<LvglProject> GetProjectAsync(string projectId)
        {
            var projects = await GetProjectsAsync();
            return projects.FirstOrDefault(p => p.ProjectId == projectId);
        }
        
        public async Task<LvglProject> GetProjectByNameAsync(string projectName)
        {
            var projects = await GetProjectsAsync();
            return projects.FirstOrDefault(p => 
                string.Equals(p.Name, projectName, StringComparison.OrdinalIgnoreCase));
        }
        
        public async Task<bool> CreateProjectAsync(LvglProject project)
        {
            try
            {
                project.Validate();
                
                if (!project.IsValid)
                {
                    return false;
                }
                
                project.ProjectId = Guid.NewGuid().ToString("N");
                project.CreatedDate = DateTime.Now;
                project.LastModified = DateTime.Now;
                
                var filePath = await SaveProjectToFileAsync(project, null);
                
                if (string.IsNullOrEmpty(filePath))
                {
                    return false;
                }
                
                project.Path = filePath;
                project.IsModified = false;
                project.IsValid = true;
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании проекта: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> UpdateProjectAsync(LvglProject project)
        {
            try
            {
                project.Validate();
                
                if (!project.IsValid)
                {
                    return false;
                }
                
                project.LastModified = DateTime.Now;
                
                await SaveProjectToFileAsync(project, project.Path);
                project.IsModified = false;
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении проекта: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> DeleteProjectAsync(string projectId)
        {
            var project = await GetProjectAsync(projectId);
            
            if (project == null)
            {
                return false;
            }
            
            try
            {
                if (File.Exists(project.Path))
                {
                    File.Delete(project.Path);
                }
                
                if (!string.IsNullOrEmpty(project.LvlFilePath) && File.Exists(project.LvlFilePath))
                {
                    File.Delete(project.LvlFilePath);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении проекта: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> SaveProjectAsync(LvglProject project)
        {
            if (string.IsNullOrEmpty(project.Path))
            {
                return await CreateProjectAsync(project);
            }
            
            return await UpdateProjectAsync(project);
        }
        
        public async Task<bool> LoadProjectAsync(LvglProject project)
        {
            if (string.IsNullOrEmpty(project.Path))
            {
                return false;
            }
            
            try
            {
                if (!File.Exists(project.Path))
                {
                    return false;
                }
                
                project = System.Text.Json.JsonSerializer.Deserialize<LvglProject>(File.ReadAllText(project.Path));
                
                if (project == null)
                {
                    return false;
                }
                
                project.Path = project.Path;
                project.IsModified = false;
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке проекта: {ex.Message}");
                return false;
            }
        }
        
        public async Task<string> SaveProjectToFileAsync(LvglProject project, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_projectStoragePath, 
                    $"{project.Name}_{project.ProjectId}.json");
            }
            
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                var jsonOptions = new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                };
                
                var json = System.Text.Json.JsonSerializer.Serialize(project, jsonOptions);
                
                File.WriteAllText(filePath, json);
                
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении проекта: {ex.Message}");
                return null;
            }
        }
        
        public async Task<List<LvglProject>> SearchProjectsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetProjectsAsync();
            }
            
            var projects = await GetProjectsAsync();
            var results = new List<LvglProject>();
            
            var normalizedQuery = query.ToLowerInvariant();
            
            foreach (var project in projects)
            {
                if (MatchProject(project, normalizedQuery))
                {
                    results.Add(project);
                }
            }
            
            return results;
        }
        
        private bool MatchProject(LvglProject project, string query)
        {
            return project.Name?.ToLowerInvariant().Contains(query) == true
                || project.Description?.ToLowerInvariant().Contains(query) == true
                || project.Author?.ToLowerInvariant().Contains(query) == true;
        }
        
        public async Task<bool> ValidateProjectAsync(LvglProject project)
        {
            try
            {
                project.Validate();
                
                if (!project.IsValid)
                {
                    return false;
                }
                
                foreach (var file in project.Files)
                {
                    file.Validate();
                    if (!file.IsModified)
                    {
                        project.IsValid = false;
                        project.ErrorMessage += $"\nФайл {file.Name}: {file.ErrorMessage}";
                    }
                }
                
                return project.IsValid;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при валидации проекта: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> ExportProjectAsync(LvglProject project, string format, string filePath)
        {
            try
            {
                switch (format.ToLowerInvariant())
                {
                    case "json":
                        return await SaveProjectToFileAsync(project, filePath);
                    case "html":
                        return await ExportToHtmlAsync(project, filePath);
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при экспорте проекта: {ex.Message}");
                return false;
            }
        }
        
        private async Task<bool> ExportToHtmlAsync(LvglProject project, string filePath)
        {
            // Генерация HTML представления
            return true;
        }
        
        public async Task<bool> ImportProjectAsync(string filePath, out LvglProject project)
        {
            project = System.Text.Json.JsonSerializer.Deserialize<LvglProject>(File.ReadAllText(filePath));
            
            if (project == null)
            {
                return false;
            }
            
            var isValid = await ValidateProjectAsync(project);
            
            if (!isValid)
            {
                return false;
            }
            
            return true;
        }
    }
}