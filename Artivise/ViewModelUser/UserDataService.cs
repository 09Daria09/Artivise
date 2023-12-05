using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows.Resources;
using System;
using Artivise.Model;
using System.Windows;
using System.Reflection;
using System.Linq;

namespace Artivise.Services
{
    public class UserDataService
    {
        private readonly string _localFilePath;

        public UserDataService(string localFilePath, string resourcePath)
        {
            _localFilePath = localFilePath;
            CopyResourceToFile(resourcePath, _localFilePath);
        }

        private void CopyResourceToFile(string resourcePath, string localFilePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();
            var resourceFullName = resourceNames.FirstOrDefault(rn => rn.EndsWith(resourcePath, StringComparison.OrdinalIgnoreCase));

            var directory = Path.GetDirectoryName(localFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(localFilePath))
            {
                if (resourceFullName != null)
                {
                    // Если ресурс найден в сборке, копируем его содержимое
                    using (var stream = assembly.GetManifestResourceStream(resourceFullName))
                    using (var reader = new StreamReader(stream))
                    using (var writer = new StreamWriter(localFilePath))
                    {
                        writer.Write(reader.ReadToEnd());
                    }
                }
                else
                {
                    // Если ресурса нет, создаем файл с начальным содержимым
                    File.WriteAllText(localFilePath, "[]");
                }
            }
        }


        public List<UserData> ReadUsers()
        {
            if (!File.Exists(_localFilePath))
            {
                return new List<UserData>();
            }

            var json = File.ReadAllText(_localFilePath);
            return JsonConvert.DeserializeObject<List<UserData>>(json);
        }

        public void WriteUsers(List<UserData> users)
        {
            var json = JsonConvert.SerializeObject(users);
            File.WriteAllText(_localFilePath, json);
        }
    }
}
