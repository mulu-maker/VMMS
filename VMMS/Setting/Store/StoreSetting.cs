using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace VMMS
{
    public class StoreSetting
    {
        private static StoreSetting _instance;
        private static readonly object Lock = new object();
        private readonly string configFilePath;
        public Dictionary<string, string> settings = new Dictionary<string, string>();
        private Dictionary<string, string> defaultSettings = new Dictionary<string, string>
    {
        {"StoreName", "XXX维修中心"},
        {"StoreThank", "XXX维修中心感谢您的支持及信任，祝您用车愉快，欢迎下次光临!"},
        {"StoreThank2", ""},
        {"StoreTelephone", "12345678901"},
        {"StoreAddress", "中国"},
        {"PrinterName", ""},
        {"PrinterNum", "3"},
        {"FontsName", ""}
        // 在这里添加更多默认配置项
    };
        //// 更新配置项示例
        //StoreSetting.Instance.SetSetting("StoreName", "新的维修中心名称");
        public static StoreSetting Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance == null)
                    {
                        _instance = new StoreSetting();
                    }
                    return _instance;
                }
            }
        }
        private StoreSetting()
        {
            configFilePath = @"config\configFile.config";
            InitializeConfig();
        }

        // 第一部分：设置默认配置项（已在defaultSettings字典中完成）

        // 第二部分：初始化配置文件
        private void InitializeConfig()
        {
            if (!File.Exists(configFilePath))
            {
                MessageBox.Show("配置文件不存在，已创建配置文件！");
                CreateConfigFile();
            }
            EnsureConfigIntegrity();
        }

        private void CreateConfigFile()
        {
            new XDocument(
                new XElement("configuration",
                    new XElement("appSettings",
                        defaultSettings.Select(kv => new XElement("add", new XAttribute("key", kv.Key), new XAttribute("value", kv.Value)))
                    )
                )
            ).Save(configFilePath);
        }

        private void EnsureConfigIntegrity()
        {
            var doc = XDocument.Load(configFilePath);
            var appSettingsElement = doc.Element("configuration")?.Element("appSettings");

            foreach (var defaultSetting in defaultSettings)
            {
                var setting = appSettingsElement?.Elements("add").FirstOrDefault(s => s.Attribute("key")?.Value == defaultSetting.Key);
                if (setting == null)
                {
                    MessageBox.Show(string.Format("配置项：“{0}”缺失，已初始化配置项！", defaultSetting.Key));
                    appSettingsElement?.Add(new XElement("add", new XAttribute("key", defaultSetting.Key), new XAttribute("value", defaultSetting.Value)));
                }
            }

            doc.Save(configFilePath);
            LoadSettingsIntoObject(doc);
        }

        // 第三部分：读取到对象中
        private void LoadSettingsIntoObject(XDocument doc)
        {
            settings = doc.Element("configuration")
                          ?.Element("appSettings")
                          ?.Elements("add")
                          .ToDictionary(s => s.Attribute("key")?.Value, s => s.Attribute("value")?.Value);
        }

        public string GetSetting(string key)
        {
            return settings.TryGetValue(key, out var value) ? value : null;
        }

        // 第四部分：写入和刷新对象中的配置项内容
        public void SetSetting(string key, string value)
        {
            if (!settings.ContainsKey(key))
            {
                throw new ArgumentException($"Setting key '{key}' not found.");
            }

            settings[key] = value;
            var doc = XDocument.Load(configFilePath);
            var settingElement = doc.Element("configuration")
                                    ?.Element("appSettings")
                                    ?.Elements("add")
                                    .FirstOrDefault(s => s.Attribute("key")?.Value == key);

            if (settingElement != null)
            {
                settingElement.SetAttributeValue("value", value);
                doc.Save(configFilePath);
            }
        }
        public void SaveSettings()
        {
            var doc = XDocument.Load(configFilePath);
            var appSettingsElement = doc.Element("configuration")?.Element("appSettings");

            // 确保appSettings元素存在
            if (appSettingsElement == null)
            {
                var configurationElement = doc.Element("configuration");
                if (configurationElement == null)
                {
                    configurationElement = new XElement("configuration");
                    doc.Add(configurationElement);
                }
                appSettingsElement = new XElement("appSettings");
                configurationElement.Add(appSettingsElement);
            }

            // 更新现有的设置项或添加新的设置项
            foreach (var setting in settings)
            {
                var settingElement = appSettingsElement.Elements("add").FirstOrDefault(s => s.Attribute("key")?.Value == setting.Key);
                if (settingElement != null)
                {
                    // 更新现有设置项的值
                    settingElement.Attribute("value").Value = setting.Value;
                }
                else
                {
                    // 添加新的设置项
                    appSettingsElement.Add(new XElement("add", new XAttribute("key", setting.Key), new XAttribute("value", setting.Value)));
                }
            }

            doc.Save(configFilePath);
            MessageBox.Show("配置文件保存成功！");
        }
    }
}
