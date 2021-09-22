using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;

namespace SaveFileServices.Tests
{
    public class SaveFileServiceTests
    {
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
    }
}