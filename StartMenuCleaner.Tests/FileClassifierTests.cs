namespace StartMenuCleaner.Tests
{
    using StartMenuCleaner.TestLibrary;
    using Xunit;

    public class FileClassifierTests
    {
        private readonly FileClassifier classifier;

        private readonly MockFileSystemComposer fileSystemComposer = new MockFileSystemComposer();

        public FileClassifierTests()
        {
            this.classifier = new FileClassifier(this.fileSystemComposer.FileSystem, this.fileSystemComposer.ShortcutHandler);
        }

        [Theory]
        [InlineData(FileClassification.App, @"C:\StartMenu\MyApp\MyApp.lnk;C:\MyApp\MyApp.exe")]
        [InlineData(FileClassification.App, @"C:\StartMenu\MyApp\MyClickOnceApp.lnk;C:\MyApp\MyApp.appref-ms")]
        [InlineData(FileClassification.App, @"C:\StartMenu\MyApp\MyClickOnceApp.appref-ms")]
        [InlineData(FileClassification.Help, @"C:\StartMenu\MyApp\MyApp Help.lnk;C:\MyApp\Help.chm")]
        [InlineData(FileClassification.Other, @"C:\StartMenu\MyApp\MyApp.other")]
        [InlineData(FileClassification.Other, @"C:\StartMenu\MyApp\MyApp.lnk;C:\MyApp\MyApp.other")]
        [InlineData(FileClassification.OtherDeletable, @"C:\StartMenu\MyApp\MyApp.txt")]
        [InlineData(FileClassification.OtherDeletable, @"C:\StartMenu\MyApp\MyApp.lnk;C:\MyApp\MyApp.txt")]
        [InlineData(FileClassification.Uninstaller, @"C:\StartMenu\MyApp\Uninstall MyApp.lnk;C:\MyApp\Uninstall MyApp.exe")]
        [InlineData(FileClassification.Uninstaller, @"C:\StartMenu\MyApp\Uninstall MyApp.lnk;C:\MyApp\Uninstall MyApp.msi")]
        [InlineData(FileClassification.WebLink, @"C:\StartMenu\MyApp\MyApp Help.url")]
        [InlineData(FileClassification.WebLink, @"C:\StartMenu\MyApp\MyApp Help.lnk;C:\MyApp\Help.url")]
        public void ClassifyFile(FileClassification expectedClassification, string filePath)
        {
            string pathToClassify = filePath;

            if (FileShortcutSyntax.TryConvertFrom(filePath, out FileShortcutSyntax? fileShortcut))
            {
                pathToClassify = fileShortcut.FilePath;
                this.fileSystemComposer.Add(fileShortcut);
            }
            else
            {
                this.fileSystemComposer.AddFile(filePath);
            }

            FileClassification actual = this.classifier.ClassifyFile(pathToClassify);

            Assert.Equal(expectedClassification, actual);
        }
    }
}
