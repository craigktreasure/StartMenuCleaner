namespace StartMenuCleaner.Tests
{
    using StartMenuCleaner.TestLibrary;
    using StartMenuCleaner.Utils;
    using Xunit;

    public class CleanupRulesEngineTests
    {
        private readonly CleanupRulesEngine cleanupEngine;

        private readonly MockFileSystemComposer fileSystemComposer = new MockFileSystemComposer();

        public CleanupRulesEngineTests()
        {
            FileClassifier classifier = new FileClassifier(this.fileSystemComposer.FileSystem, this.fileSystemComposer.ShortcutHandler);
            this.cleanupEngine = new CleanupRulesEngine(this.fileSystemComposer.FileSystem, classifier);
        }

        [Fact]
        public void TestEmpty()
        {
            const string directoryPath = @"C:\StartMenu\MyApp";
            this.fileSystemComposer.AddDirectory(directoryPath);

            CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

            Assert.Equal(CleanReason.Empty, actual);
        }

        [Fact]
        public void TestFewAppsWithCruft()
        {
            string directoryPath = this.ConfigureForSingleApp();
            this.AddFile(
                System.IO.Path.Combine(directoryPath, "MyApp2.lnk"),
                @"C:\Programs\MyApp\MyApp2.exe");
            this.AddFile(
                System.IO.Path.Combine(directoryPath, "MyApp Help.lnk"),
                @"C:\Programs\MyApp\MyApp Help.chm");
            this.AddFile(
                System.IO.Path.Combine(directoryPath, "MyApp Help.txt"),
                @"C:\Programs\MyApp\MyApp Help.txt");

            CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

            Assert.Equal(CleanReason.FewAppsWithCruft, actual);
        }

        [Fact]
        public void TestFewAppsWithCruftWithDirectory()
        {
            string directoryPath = this.ConfigureForSingleApp();
            this.AddFile(
                System.IO.Path.Combine(directoryPath, "MyApp2.lnk"),
                @"C:\Programs\MyApp\MyApp2.exe");
            this.AddFile(
                System.IO.Path.Combine(directoryPath, "MyApp Help.lnk"),
                @"C:\Programs\MyApp\MyApp Help.chm");
            this.AddFile(
                System.IO.Path.Combine(directoryPath, "MyApp Help.txt"),
                @"C:\Programs\MyApp\MyApp Help.txt");
            this.fileSystemComposer.AddDirectory(System.IO.Path.Combine(directoryPath, "Foo"));

            CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

            Assert.Equal(CleanReason.None, actual);
        }

        [Fact]
        public void TestFewAppsWithCruftWithOtherFile()
        {
            string directoryPath = this.ConfigureForSingleApp();
            this.AddFile(
                System.IO.Path.Combine(directoryPath, "MyApp2.lnk"),
                @"C:\Programs\MyApp\MyApp2.exe");
            this.AddFile(
                System.IO.Path.Combine(directoryPath, "MyApp Help.lnk"),
                @"C:\Programs\MyApp\MyApp Help.chm");
            this.AddFile(
                System.IO.Path.Combine(directoryPath, "MyApp Help.txt"),
                @"C:\Programs\MyApp\MyApp Help.txt");
            this.fileSystemComposer.AddFile(System.IO.Path.Combine(directoryPath, "Foo.other"));

            CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

            Assert.Equal(CleanReason.None, actual);
        }

        [Fact]
        public void TestIgnoreFolderName()
        {
            const string filePath = @"C:\StartMenu\Maintenance\MyApp.lnk";
            this.fileSystemComposer.AddFile(filePath);
            string directoryPath = System.IO.Path.GetDirectoryName(filePath)!;

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
            this.fileSystemComposer.AddDirectory(System.IO.Path.Combine(directoryPath, "Foo"));

            CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

            Assert.Equal(CleanReason.None, actual);
        }

        [Fact]
        public void TestThreeApps()
        {
            string directoryPath = this.ConfigureForSingleApp();
            this.AddFile(
                System.IO.Path.Combine(directoryPath, "MyApp2.lnk"),
                @"C:\Programs\MyApp\MyApp2.exe");
            this.AddFile(
                System.IO.Path.Combine(directoryPath, "MyApp3.lnk"),
                @"C:\Programs\MyApp\MyApp3.exe");

            CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

            Assert.Equal(CleanReason.None, actual);
        }

        [Fact]
        public void TestTwoApps()
        {
            string directoryPath = this.ConfigureForSingleApp();
            this.AddFile(
                System.IO.Path.Combine(directoryPath, "MyApp2.lnk"),
                @"C:\Programs\MyApp\MyApp2.exe");

            CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

            Assert.Equal(CleanReason.FewAppsWithCruft, actual);
        }

        private void AddFile(string filePath, string targetPath)
        {
            this.fileSystemComposer.Add(new FileShortcut(filePath, targetPath));
        }

        private string ConfigureForSingleApp()
        {
            FileShortcutSyntax fileShortcut = (FileShortcutSyntax)@"C:\StartMenu\MyApp\MyApp.lnk;C:\Programs\MyApp\MyApp.exe";
            this.fileSystemComposer.Add(fileShortcut);

            return System.IO.Path.GetDirectoryName(fileShortcut.FilePath)!;
        }
    }
}
