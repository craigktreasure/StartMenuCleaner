namespace StartMenuCleaner.Tests.Utils;

using System.IO;

using StartMenuCleaner.TestLibrary;
using StartMenuCleaner.Utils;

using Xunit;

public class CsWin32ShortcutHandlerTests
{
    [Fact]
    public void ResolveTarget()
    {
        EmbeddedResourceHelper embeddedResourceHelper = new(typeof(AssemblyToken).Assembly);

        string temporaryFilePath = embeddedResourceHelper.CopyToTempFile(@"EmbeddedContent\systeminfo.lnk");

        try
        {
            CsWin32ShortcutHandler shortcutHandler = new();
            string actualTargetPath = shortcutHandler.ResolveTarget(temporaryFilePath);

            Assert.Equal(@"C:\Windows\System32\systeminfo.exe", actualTargetPath);
        }
        finally
        {
            File.Delete(temporaryFilePath);
        }
    }
}
