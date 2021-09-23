using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SaveFileServices.Tests
{
    public class SaveFileServiceTests
    {
        private string _filePath = "./file.test";

        [TearDown]
        public void TearDown()
        {
            File.Delete(_filePath);
        }

        [Test]
        public void Get_GivenNegativeIndex_ThrowsArgumentException()
        {
            var service = new SaveService<string>(5);
            Assert.That(() => service.Get(-5), Throws.ArgumentException.With.Message.EqualTo("Index cannot be negative."));
        }

        [Test]
        public void Get_GivenNumberGreaterThanMaxSlots_ThrowsArgumentException()
        {
            var service = new SaveService<string>(5);
            Assert.That(() => service.Get(10), Throws.ArgumentException.With.Message.EqualTo("Index provided is greater than maximum provided slots."));
        }

        [Test]
        public void Get_GivenSlotWithNoDataSet_ReturnsNull()
        {
            var service = new SaveService<string>(5);
            Assert.That(service.Get(4).Result, Is.Null);
        }

        [Test]
        public void Set_GivenNegativeIndex_ThrowsArgumentException()
        {
            var service = new SaveService<string>(5);
            Assert.That(() => service.Set(-5, "Data"), Throws.ArgumentException.With.Message.EqualTo("Index cannot be negative."));
        }

        [Test]
        public void Set_GivenNumberGreaterThanMaxSlots_ThrowsArgumentException()
        {
            var service = new SaveService<string>(5);
            Assert.That(() => service.Set(10, "Data"), Throws.ArgumentException.With.Message.EqualTo("Index provided is greater than maximum provided slots."));
        }

        [Test]
        public async Task Set_GivenValidIndex_SavesData()
        {
            var data = "Test Data";
            var service = new SaveService<string>(5);
            await service.Set(3, data);

            Assert.That(service.Get(3).Result, Is.EqualTo(data));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("        ")]
        public void Save_GivenInvalidFilePath_ThrowsArgumentException(string path)
        {
            var service = new SaveService<string>(5);
            Assert.That(() => service.Save(path), Throws.ArgumentException.With.Message.EqualTo("Path provided is invalid."));
        }

        [Test]
        public async Task Save_GivenValidFilePath_SavesFileToCorrectLocationAndCorrectlySavesData()
        {
            var service = new SaveService<string>(5);
            var testIndex = 2;
            var testData = "This is some test data.";

            var expectedData = new[]
            {
                null,
                null,
                testData,
                null,
                null
            };

            await service.Set(testIndex, testData);
            await service.Save(_filePath);

            Assert.That(File.Exists(_filePath), Is.True);

            var data = await File.ReadAllBytesAsync(_filePath);
            var memStream = new MemoryStream(data);
            var serializer = new XmlSerializer(typeof(string[]));
            var actualData = (string[])serializer.Deserialize(memStream);

            Assert.That(expectedData, Is.EquivalentTo(actualData));
        }
    }
}