namespace StartMenuCleaner.Tests
{
    using StartMenuCleaner.TestLibrary;
    using System;
    using System.IO.Abstractions.TestingHelpers;
    using Xunit;

    public class CleanupRulesEngineTests
    {
        private readonly CleanupRulesEngine cleanupEngine;

        private readonly MockFileSystem mockFileSystem = new MockFileSystem();

        private readonly TestFileShortcutHandler shortcutHandler = new TestFileShortcutHandler();

        public CleanupRulesEngineTests()
        {
            FileClassifier classifier = new FileClassifier(this.mockFileSystem, this.shortcutHandler);
            this.cleanupEngine = new CleanupRulesEngine(this.mockFileSystem, classifier);
        }

        [Fact]
        public void TestEmpty()
        {
            const string directoryPath = @"C:\StartMenu\MyApp";
            this.mockFileSystem.AddDirectory(directoryPath);

            CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

            Assert.Equal(CleanReason.Empty, actual);
        }

        [Fact]
        public void TestFewAppsWithCruft()
        {
            string directoryPath = this.ConfigureForSingleApp();
            this.AddFile(
                this.mockFileSystem.Path.Combine(directoryPath, "MyApp2.lnk"),
                @"C:\Programs\MyApp\MyApp2.exe");
            this.AddFile(
                this.mockFileSystem.Path.Combine(directoryPath, "MyApp Help.lnk"),
                @"C:\Programs\MyApp\MyApp Help.chm");
            this.AddFile(
                this.mockFileSystem.Path.Combine(directoryPath, "MyApp Help.txt"),
                @"C:\Programs\MyApp\MyApp Help.txt");

            CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

            Assert.Equal(CleanReason.FewAppsWithCruft, actual);
        }

        [Fact]
        public void TestFewAppsWithCruftWithDirectory()
        {
            string directoryPath = this.ConfigureForSingleApp();
            this.AddFile(
                this.mockFileSystem.Path.Combine(directoryPath, "MyApp2.lnk"),
                @"C:\Programs\MyApp\MyApp2.exe");
            this.AddFile(
                this.mockFileSystem.Path.Combine(directoryPath, "MyApp Help.lnk"),
                @"C:\Programs\MyApp\MyApp Help.chm");
            this.AddFile(
                this.mockFileSystem.Path.Combine(directoryPath, "MyApp Help.txt"),
                @"C:\Programs\MyApp\MyApp Help.txt");
            this.mockFileSystem.AddDirectory(this.mockFileSystem.Path.Combine(directoryPath, "Foo"));

            CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

            Assert.Equal(CleanReason.None, actual);
        }

        [Fact]
        public void TestFewAppsWithCruftWithOtherFile()
        {
            string directoryPath = this.ConfigureForSingleApp();
            this.AddFile(
                this.mockFileSystem.Path.Combine(directoryPath, "MyApp2.lnk"),
                @"C:\Programs\MyApp\MyApp2.exe");
            this.AddFile(
                this.mockFileSystem.Path.Combine(directoryPath, "MyApp Help.lnk"),
                @"C:\Programs\MyApp\MyApp Help.chm");
            this.AddFile(
                this.mockFileSystem.Path.Combine(directoryPath, "MyApp Help.txt"),
                @"C:\Programs\MyApp\MyApp Help.txt");
            this.AddFile(this.mockFileSystem.Path.Combine(directoryPath, "Foo.other"));

            CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

            Assert.Equal(CleanReason.None, actual);
        }

        [Fact]
        public void TestIgnoreFolderName()
        {
            string directoryPath = this.AddFile(@"C:\StartMenu\Maintenance\MyApp.lnk");

            CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

            Assert.Equal(CleanReason.None, actual);
        }

        [Fact]
        public void TestSingleApp()
        {
            string directoryPath = this.ConfigureForSingleApp();

            CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

            Assert.Equal(CleanReason.SingleApp, actual);
        }

        [Fact]
        public void TestSingleAppWithExtraDirectory()
        {
            string directoryPath = this.ConfigureForSingleApp();
            this.mockFileSystem.AddDirectory(this.mockFileSystem.Path.Combine(directoryPath, "Foo"));

            CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

            Assert.Equal(CleanReason.None, actual);
        }

        [Fact]
        public void TestThreeApps()
        {
            string directoryPath = this.ConfigureForSingleApp();
            this.AddFile(
                this.mockFileSystem.Path.Combine(directoryPath, "MyApp2.lnk"),
                @"C:\Programs\MyApp\MyApp2.exe");
            this.AddFile(
                this.mockFileSystem.Path.Combine(directoryPath, "MyApp3.lnk"),
                @"C:\Programs\MyApp\MyApp3.exe");

            CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

            Assert.Equal(CleanReason.None, actual);
        }

        [Fact]
        public void TestTwoApps()
        {
            string directoryPath = this.ConfigureForSingleApp();
            this.AddFile(
                this.mockFileSystem.Path.Combine(directoryPath, "MyApp2.lnk"),
                @"C:\Programs\MyApp\MyApp2.exe");

            CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

            Assert.Equal(CleanReason.FewAppsWithCruft, actual);
        }

        private string AddFile(string itemPath)
        {
            string fileDirectoryPath = this.mockFileSystem.Path.GetDirectoryName(itemPath);
            this.mockFileSystem.AddDirectory(fileDirectoryPath);
            this.mockFileSystem.AddFile(itemPath, new MockFileData(String.Empty));

            return fileDirectoryPath;
        }

        private string AddFile(string itemPath, string shortcutPath)
        {
            string directoryPath = this.AddFile(itemPath);
            this.shortcutHandler.AddShortcutMapping(itemPath, shortcutPath);

            return directoryPath;
        }

        private string ConfigureForSingleApp()
        {
            const string itemPath = @"C:\StartMenu\MyApp\MyApp.lnk";
            const string shortcutPath = @"C:\Programs\MyApp\MyApp.exe";

            return AddFile(itemPath, shortcutPath);
        }
    }
}
