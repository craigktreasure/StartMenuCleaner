namespace StartMenuCleaner.Tests.Utils
{
    using FluentAssertions;
    using StartMenuCleaner.TestLibrary;
    using StartMenuCleaner.Utils;
    using System.IO;
    using Xunit;

    public class DefaultFileShortcutHandlerTests
    {
        [Fact]
        public void ResolveShortcutTargetPath()
        {
            EmbeddedResourceHelper embeddedResourceHelper = new EmbeddedResourceHelper(typeof(AssemblyToken).Assembly);

            string temporaryFilePath = embeddedResourceHelper.CopyToTempFile(@"EmbeddedContent\systeminfo.lnk");

            try
            {
                DefaultFileShortcutHandler shortcutHandler = new DefaultFileShortcutHandler();
                string actualTargetPath = shortcutHandler.ResolveTarget(temporaryFilePath);

                actualTargetPath.Should().Be(@"C:\Windows\System32\systeminfo.exe");
            }
            finally
            {
                File.Delete(temporaryFilePath);
            }
        }
    }
}
