﻿using Newtonsoft.Json;
using PCLStorage;
using PCLStorage.Exceptions;
using Pollenalarm.Frontend.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Forms.Services
{
    public class FileSystemService : IFileSystemService
    {
        private readonly JsonSerializerSettings _JsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
        private readonly string _DefaultFolderName = "Pollenalarm";

        public async Task<T> ReadObjectFromFileAsync<T>(string fileName)
        {
            try
            {
                var rootFolder = FileSystem.Current.LocalStorage;
                var folder = await rootFolder.GetFolderAsync(_DefaultFolderName);
                var file = await folder.GetFileAsync(fileName);

                var json = await file.ReadAllTextAsync();
                var content = JsonConvert.DeserializeObject<T>(json, _JsonSerializerSettings);
                return content;
            }
            catch (Exception ex) when (ex is DirectoryNotFoundException || ex is FileNotFoundException)
            {
                return default(T);
            }
        }

        public async Task SaveObjectToFileAsync(string fileName, object content)
        {
            var rootFolder = FileSystem.Current.LocalStorage;
            var folder = await rootFolder.CreateFolderAsync(_DefaultFolderName, CreationCollisionOption.OpenIfExists);
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            var json = JsonConvert.SerializeObject(content, Formatting.Indented, _JsonSerializerSettings);
            await file.WriteAllTextAsync(json);
        }
    }
}