namespace StartMenuCleaner.Tests
{
    using StartMenuCleaner.TestLibrary;
    using StartMenuCleaner.Utils;
    using System;
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
        [InlineData(nameof(FileClassification.App), @"C:\StartMenu\MyApp\MyApp.lnk;C:\MyApp\MyApp.exe")]
        [InlineData(nameof(FileClassification.App), @"C:\StartMenu\MyApp\MyClickOnceApp.lnk;C:\MyApp\MyApp.appref-ms")]
        [InlineData(nameof(FileClassification.App), @"C:\StartMenu\MyApp\MyClickOnceApp.appref-ms")]
        [InlineData(nameof(FileClassification.Help), @"C:\StartMenu\MyApp\MyApp Help.lnk;C:\MyApp\Help.chm")]
        [InlineData(nameof(FileClassification.Other), @"C:\StartMenu\MyApp\MyApp.other")]
        [InlineData(nameof(FileClassification.Other), @"C:\StartMenu\MyApp\MyApp.lnk;C:\MyApp\MyApp.other")]
        [InlineData(nameof(FileClassification.OtherDeletable), @"C:\StartMenu\MyApp\MyApp.txt")]
        [InlineData(nameof(FileClassification.OtherDeletable), @"C:\StartMenu\MyApp\MyApp.lnk;C:\MyApp\MyApp.txt")]
        [InlineData(nameof(FileClassification.Uninstaller), @"C:\StartMenu\MyApp\Uninstall MyApp.lnk;C:\MyApp\Uninstall MyApp.exe")]
        [InlineData(nameof(FileClassification.Uninstaller), @"C:\StartMenu\MyApp\Uninstall MyApp.lnk;C:\MyApp\Uninstall MyApp.msi")]
        [InlineData(nameof(FileClassification.WebLink), @"C:\StartMenu\MyApp\MyApp Help.url")]
        [InlineData(nameof(FileClassification.WebLink), @"C:\StartMenu\MyApp\MyApp Help.lnk;C:\MyApp\Help.url")]
        public void ClassifyFile(string expectedClassificationValue, string filePath)
        {
            FileClassification expectedClassification = Enum.Parse<FileClassification>(expectedClassificationValue);
            string pathToClassify = filePath;

            if (FileShortcut.TryConvertFrom(filePath, out FileShortcut? fileShortcut))
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
