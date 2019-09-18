using Files.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Files.Tests
{
    [TestClass]
    public class FileTests
    {
        [TestMethod]
        public async Task add_file_to_repo_expect_storage()
        {
            var repository = new MockFileRepository(new MockFileReader());
            var file = new File
            {
                Name = "SomeUnprojectedData.GeoJson",
                Directory = Guid.NewGuid().ToString()
            };

            await repository.Add(file);

            var files = await repository.GetDirectory(file.Directory);
            var singleFile = files.Single();

            // The file is automatically embedded into a directory //
            // so we have to dig into the Files collection to get the original //
            Assert.AreEqual(singleFile.Files.Single(), file);
        }

        [TestMethod]
        public async Task get_file_from_repo_expect_match()
        {
            var repository = new MockFileRepository(new MockFileReader());
            var file = new File
            {
                Name = "SomeUnprojectedData.GeoJson",
                Directory = Guid.NewGuid().ToString()
            };
            var file2 = new File
            {
                Name = "SomeUnprojectedData2.GeoJson",
                Directory = Guid.NewGuid().ToString()
            };

            await repository.Add(file);
            await repository.Add(file2);

            var files = await repository.GetDirectory(file.Directory);
            var singleFile = files.Single();

            Assert.AreEqual(singleFile.Files.Single(f => f.Name == file.Name), file);


            var files2 = await repository.GetDirectory(file2.Directory);
            var singleFile2 = files2.Single();

            Assert.AreEqual(singleFile2.Files.Single(f => f.Name == file2.Name), file2);
        }

        [TestMethod]
        public async Task create_directory_in_repo_expect_directory_storage()
        {
            var repository = new MockFileRepository(new MockFileReader());

            var directory = Guid.NewGuid().ToString();

            var file = new File
            {
                Name = "SomeUnprojectedData.GeoJson",
                Directory = directory
            };
            var file2 = new File
            {
                Name = "SomeUnprojectedData2.GeoJson",
                Directory = directory
            };

            // Initial Add will generate the DIR if it doesnt exist //
            await repository.Add(file);
            await repository.Add(file2);

            var files = await repository.GetDirectory(file.Directory);
            var singleDirectory = files.Single();
            Assert.AreEqual(2, singleDirectory.Files.Count());
        }
    }
}
