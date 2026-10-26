using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LvglProjectEditor.Models;

namespace LvglProjectEditor.Services
{
    public interface IFileService
    {
        Task<List<string>> GetFilesAsync();
        Task<List<string>> GetFilesByExtensionAsync(string extension);
        Task<string> GetFileContentAsync(string filePath);
        Task<bool> SaveFileAsync(string filePath, string content);
        Task<bool> DeleteFileAsync(string filePath);
        Task<bool> CopyFileAsync(string sourcePath, string destinationPath);
        Task<bool> MoveFileAsync(string sourcePath, string destinationPath);
        Task<List<LvglFile>> GetLvglFilesAsync();
        Task<bool> LoadLvglFileAsync(LvglFile file);
        Task<bool> SaveLvglFileAsync(LvglFile file);
        Task<bool> ValidateFileAsync(string filePath);
        Task<List<string>> SearchFilesAsync(string query);
    }
    
    public class FileService : IFileService
    {
        private readonly string _workspacePath;
        private readonly object _lock = new object();
        
        public FileService(string workspacePath = null)
        {
            _workspacePath = workspacePath ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Documents");
            
            EnsureWorkspacePathExists();
        }
        
        private void EnsureWorkspacePathExists()
        {
            if (!Directory.Exists(_workspacePath))
            {
                Directory.CreateDirectory(_workspacePath);
            }
        }
        
        public async Task<List<string>> GetFilesAsync()
        {
            var files = new List<string>();
            
            try
            {
                var allFiles = Directory.GetFiles(_workspacePath, "*", SearchOption.AllDirectories);
                files.AddRange(allFiles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении файлов: {ex.Message}");
            }
            
            return files;
        }
        
        public async Task<List<string>> GetFilesByExtensionAsync(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return await GetFilesAsync();
            }
            
            var files = new List<string>();
            extension = extension.TrimStart('.').ToLowerInvariant();
            
            try
            {
                files.AddRange(Directory.GetFiles(_workspacePath, $"*.{extension}", SearchOption.AllDirectories));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении файлов по расширению: {ex.Message}");
            }
            
            return files;
        }
        
        public async Task<string> GetFileContentAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return null;
                }
                
                return await File.ReadAllTextAsync(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
                return null;
            }
        }
        
        public async Task<bool> SaveFileAsync(string filePath, string content)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }
                
                await File.WriteAllTextAsync(filePath, content);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении файла: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> CopyFileAsync(string sourcePath, string destinationPath)
        {
            try
            {
                if (!File.Exists(sourcePath))
                {
                    return false;
                }
                
                if (!Directory.Exists(Path.GetDirectoryName(destinationPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                }
                
                File.Copy(sourcePath, destinationPath);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при копировании файла: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> MoveFileAsync(string sourcePath, string destinationPath)
        {
            try
            {
                if (!File.Exists(sourcePath))
                {
                    return false;
                }
                
                if (!Directory.Exists(Path.GetDirectoryName(destinationPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                }
                
                File.Move(sourcePath, destinationPath);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при перемещении файла: {ex.Message}");
                return false;
            }
        }
        
        public async Task<List<LvglFile>> GetLvglFilesAsync()
        {
            var files = new List<LvglFile>();
            
            try
            {
                var filePaths = await GetFilesByExtensionAsync("lv");
                
                foreach (var filePath in filePaths)
                {
                    var file = new LvglFile
                    {
                        Name = Path.GetFileName(filePath),
                        FullName = filePath,
                        Path = Path.GetDirectoryName(filePath),
                        Type = "lvgl source",
                        FileTypeCategory = FileTypes.Source,
                        IsEditable = true,
                        IsSaved = true,
                        Size = new FileInfo(filePath).Length,
                        CreatedDate = new FileInfo(filePath).Created,
                        LastModified = new FileInfo(filePath).LastWriteTime
                    };
                    
                    files.Add(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении LVGL файлов: {ex.Message}");
            }
            
            return files;
        }
        
        public async Task<bool> LoadLvglFileAsync(LvglFile file)
        {
            try
            {
                if (string.IsNullOrEmpty(file.FullName))
                {
                    return false;
                }
                
                if (!File.Exists(file.FullName))
                {
                    return false;
                }
                
                var content = await GetFileContentAsync(file.FullName);
                
                if (string.IsNullOrEmpty(content))
                {
                    return false;
                }
                
                file.Content = content;
                file.IsSaved = false;
                file.IsModified = true;
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке LVGL файла: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> SaveLvglFileAsync(LvglFile file)
        {
            try
            {
                if (string.IsNullOrEmpty(file.FullName))
                {
                    return false;
                }
                
                if (string.IsNullOrEmpty(file.Content))
                {
                    return false;
                }
                
                await SaveFileAsync(file.FullName, file.Content);
                
                file.IsSaved = true;
                file.IsModified = false;
                file.LastModified = DateTime.Now;
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении LVGL файла: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> ValidateFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return false;
                }
                
                var content = await GetFileContentAsync(filePath);
                
                if (string.IsNullOrEmpty(content))
                {
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при валидации файла: {ex.Message}");
                return false;
            }
        }
        
        public async Task<List<string>> SearchFilesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetFilesAsync();
            }
            
            var files = await GetFilesAsync();
            var results = new List<string>();
            
            var normalizedQuery = query.ToLowerInvariant();
            
            foreach (var file in files)
            {
                if (file.ToLowerInvariant().Contains(normalizedQuery))
                {
                    results.Add(file);
                }
            }
            
            return results;
        }
    }
}