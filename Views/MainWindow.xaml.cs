using System;
using System.Threading.Tasks;
using LvglProjectEditor.Models;
using LvglProjectEditor.Services;

namespace LvglProjectEditor.Views
{
    public partial class MainWindow : ContentPage
    {
        private readonly IProjectService _projectService;
        private readonly IFileService _fileService;
        private readonly IElementService _elementService;
        private readonly IStyleService _styleService;
        private readonly IThemeService _themeService;
        private readonly IParseService _parseService;
        private readonly IGeneratorService _generatorService;
        private readonly IValidationService _validationService;
        
        private LvglProject _currentProject;
        
        public MainWindow()
        {
            InitializeComponent();
            
            _projectService = new ProjectService();
            _fileService = new FileService();
            _elementService = new ElementService();
            _styleService = new StyleService();
            _themeService = new ThemeService();
            _parseService = new ParseService();
            _generatorService = new GeneratorService();
            _validationService = new ValidationService();
        }
        
        private async void OnNewProjectClicked(object sender, EventArgs e)
        {
            var name = await DisplayAlert("Project Name", "Enter new project name:", "OK");
            if (!string.IsNullOrEmpty(name))
            {
                await CreateNewProjectAsync(name);
            }
        }
        
        private async void OnOpenProjectClicked(object sender, EventArgs e)
        {
            var filePath = await FilePicker.OpenPickerAsync(new PickOptions
            {
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".json", ".lv" } },
                    { DevicePlatform.Android, new[] { ".json", ".lv" } },
                    { DevicePlatform.iOS, new[] { ".json", ".lv" } }
                })
            });
            
            if (!string.IsNullOrEmpty(filePath))
            {
                await LoadProjectAsync(filePath);
            }
        }
        
        private async void OnSaveProjectClicked(object sender, EventArgs e)
        {
            if (_currentProject != null)
            {
                var isValid = await _validationService.ValidateProjectAsync(_currentProject);
                if (isValid)
                {
                    await _projectService.SaveProjectAsync(_currentProject);
                    await DisplayAlert("Success", "Project saved successfully!", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Please fix validation errors before saving.", "OK");
                }
            }
        }
        
        private async void OnCreateProjectClicked(object sender, EventArgs e)
        {
            var name = await DisplayAlert("Project Name", "Enter project name:", "OK");
            if (!string.IsNullOrEmpty(name))
            {
                await CreateNewProjectAsync(name);
            }
        }
        
        private async Task CreateNewProjectAsync(string name)
        {
            var project = new LvglProject
            {
                Name = name,
                ProjectId = Guid.NewGuid().ToString("N"),
                CreatedDate = DateTime.Now,
                LastModified = DateTime.Now,
                Type = ProjectType.Default,
                Author = "User"
            };
            
            var success = await _projectService.CreateProjectAsync(project);
            
            if (success)
            {
                _currentProject = project;
                await DisplayAlert("Success", $"Project '{name}' created successfully!", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Failed to create project.", "OK");
            }
        }
        
        private async Task LoadProjectAsync(string filePath)
        {
            var project = await _projectService.LoadProjectFromFileAsync(filePath);
            
            if (project != null)
            {
                _currentProject = project;
                await DisplayAlert("Success", $"Project '{project.Name}' loaded successfully!", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Failed to load project.", "OK");
            }
        }
    }
}