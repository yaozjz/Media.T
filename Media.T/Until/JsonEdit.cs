using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Media.T.Until
{
    internal class JsonEdit
    {
        public static data.SettingData JsonRead(string jsonFilePath)
        {
            string jsonString = File.ReadAllText(jsonFilePath);

            // 将 JSON 字符串反序列化为对象
            data.SettingData setting_data = JsonSerializer.Deserialize<data.SettingData>(jsonString);

            return setting_data;
        }

        public static void JsonWrite(string jsonFilePath, data.SettingData setting_value)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // 将对象序列化为 JSON 字符串
            string jsonString = JsonSerializer.Serialize(setting_value, options);

            // 将 JSON 字符串写入文件
            File.WriteAllText(jsonFilePath, jsonString);
        }
    }
}
