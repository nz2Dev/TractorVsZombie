using System.IO;
using NUnit.Framework;

public class FileCreatorTests {
    
    [SetUp]
    public void SetUp() {
        Directory.CreateDirectory(FileCreator.k_Directory);
    }

    [Test]
    public void CreateEmptyFile() {
        var fileCreator = new FileCreator();
        string expectedFileName = "fileName1";

        fileCreator.CreateEmptyFile(expectedFileName);

        var files = Directory.GetFiles(FileCreator.k_Directory);
        Assert.That(files.Length, Is.EqualTo(1), "Expected one file");
        Assert.That(files[0], Does.Contain(expectedFileName));
    }

    [TearDown]
    public void TearDown() {
        Directory.Delete(FileCreator.k_Directory, true);
    }

}
