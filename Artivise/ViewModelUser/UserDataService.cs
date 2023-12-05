﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows.Resources;
using System;
using Artivise.Model;
using System.Windows;
using System.Reflection;

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
            var directory = Path.GetDirectoryName(localFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(localFilePath))
            {
                // Проверяем, является ли путь ресурса валидным для существующего в сборке ресурса
                var resourceUri = new Uri($"pack://application:,,,/Artivise;component/{resourcePath}", UriKind.Absolute);
                StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);

                if (streamInfo != null)
                {
                    // Если ресурс найден в сборке, копируем его содержимое
                    using (var reader = new StreamReader(streamInfo.Stream))
                    using (var writer = new StreamWriter(localFilePath))
                    {
                        writer.Write(reader.ReadToEnd());
                    }
                }
                else
                {
                    // Если ресурса нет, создаем файл с начальным содержимым (например, пустым JSON-массивом)
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