namespace StartMenuCleaner.Tests
{
    using StartMenuCleaner.TestLibrary;
    using System.IO.Abstractions.TestingHelpers;
    using Xunit;

    public class FileClassifierTests
    {
        private readonly FileClassifier classifier;

        private readonly MockFileSystem mockFileSystem = new MockFileSystem();

        private readonly TestFileShortcutHandler shortcutHandler = new TestFileShortcutHandler();

        public FileClassifierTests()
        {
            this.classifier = new FileClassifier(this.mockFileSystem, this.shortcutHandler);
        }

        [Theory]
        [InlineData(FileClassification.App, @"C:\StartMenu\MyApp\MyApp.lnk", @"C:\MyApp\MyApp.exe")]
        [InlineData(FileClassification.App, @"C:\StartMenu\MyApp\MyClickOnceApp.lnk", @"C:\MyApp\MyApp.appref-ms")]
        [InlineData(FileClassification.App, @"C:\StartMenu\MyApp\MyClickOnceApp.appref-ms", null)]
        [InlineData(FileClassification.Help, @"C:\StartMenu\MyApp\MyApp Help.lnk", @"C:\MyApp\Help.chm")]
        [InlineData(FileClassification.Other, @"C:\StartMenu\MyApp\MyApp.other", null)]
        [InlineData(FileClassification.Other, @"C:\StartMenu\MyApp\MyApp.lnk", @"C:\MyApp\MyApp.other")]
        [InlineData(FileClassification.OtherDeletable, @"C:\StartMenu\MyApp\MyApp.txt", null)]
        [InlineData(FileClassification.OtherDeletable, @"C:\StartMenu\MyApp\MyApp.lnk", @"C:\MyApp\MyApp.txt")]
        [InlineData(FileClassification.Uninstaller, @"C:\StartMenu\MyApp\Uninstall MyApp.lnk", @"C:\MyApp\Uninstall MyApp.exe")]
        [InlineData(FileClassification.Uninstaller, @"C:\StartMenu\MyApp\Uninstall MyApp.lnk", @"C:\MyApp\Uninstall MyApp.msi")]
        [InlineData(FileClassification.WebLink, @"C:\StartMenu\MyApp\MyApp Help.url", null)]
        [InlineData(FileClassification.WebLink, @"C:\StartMenu\MyApp\MyApp Help.lnk", @"C:\MyApp\Help.url")]
        public void ClassifyFile(FileClassification expectedClassification, string filePath, string? shortcutTarget)
        {
            this.mockFileSystem.AddFile(filePath, new MockFileData(string.Empty));

            if (shortcutTarget is not null)
            {
                this.shortcutHandler.AddShortcutMapping(filePath, shortcutTarget);
            }

            FileClassification actual = this.classifier.ClassifyFile(filePath);

            Assert.Equal(expectedClassification, actual);
        }
    }
}
