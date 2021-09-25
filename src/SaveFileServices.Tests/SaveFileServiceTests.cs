using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LPSoft.SaveFileServices.Tests
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

        [TestCase(0)]
        [TestCase(-5)]
        public void OnInitialise_GivenNegativeNumber_ThrowsArgumentException(int slots)
            => Assert.That(() => { var saveService = new SaveService<string>(slots); }, Throws.ArgumentException.With.Message.EqualTo("Slots cannot be zero or negative."));

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

            Assert.That(actualData, Is.EquivalentTo(expectedData));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("        ")]
        public void Load_GivenInvalidFilePath_ThrowsArgumentException(string path)
        {
            var service = new SaveService<string>(5);
            Assert.That(() => service.Load(path), Throws.ArgumentException.With.Message.EqualTo("Path provided is invalid."));
        }

        [Test]
        public async Task Load_GivenValidFilePathWithSavedFile_LoadsAndDeserialisesData()
        {
            var service = new SaveService<string>(5);
            var testIndex = 2;
            var testData = "This is some test data.";


            await service.Set(testIndex, testData);
            await service.Save(_filePath);

            var newService = new SaveService<string>(5);
            await newService.Load(_filePath);

            var actualData = await newService.Get(testIndex);

            Assert.That(actualData, Is.EqualTo(testData));
        }

        [Test]
        public async Task Load_GivenFileWithDiffentSlotAmount_UpdatesSlotNUmber()
        {
            var service = new SaveService<string>(10);
            var testIndex = 2;
            var testData = "This is some test data.";


            await service.Set(testIndex, testData);
            await service.Save(_filePath);

            var newService = new SaveService<string>(5);
            await newService.Load(_filePath);


            Assert.That(newService.TotalSlots, Is.EqualTo(service.TotalSlots));
        }

    }
}