using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.IO;

namespace EPocalipse.Json.Viewer
{
    class PluginsManager
    {
        List<IJsonViewerPlugin> plugins = new List<IJsonViewerPlugin>();
        List<IJsonTextVisualizer> textVisualizers = new List<IJsonTextVisualizer>();
        List<IJsonVisualizer> visualizers = new List<IJsonVisualizer>();
        IJsonVisualizer _defaultVisualizer;

        public PluginsManager()
        {
        }

        public void Initialize()
        {
            string myDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;

            Configuration config=ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            if (config != null)
            {
                ViewerConfiguration viewerConfig = (ViewerConfiguration)config.GetSection("jsonViewer");
                if (viewerConfig != null)
                {
                    foreach (KeyValueConfigurationElement keyValue in viewerConfig.Plugins)
                    {
                        string type = keyValue.Value;
                        Type pluginType = Type.GetType(type, false);
                        if (pluginType != null && typeof(IJsonViewerPlugin).IsAssignableFrom(pluginType))
                        {
                            try
                            {
                                IJsonViewerPlugin plugin = (IJsonViewerPlugin)Activator.CreateInstance(pluginType);
                                plugins.Add(plugin);
                                if (plugin is IJsonTextVisualizer)
                                    textVisualizers.Add((IJsonTextVisualizer)plugin);
                                if (plugin is IJsonVisualizer)
                                {
                                    if (_defaultVisualizer == null)
                                        _defaultVisualizer = (IJsonVisualizer)plugin;
                                    visualizers.Add((IJsonVisualizer)plugin);
                                }
                            }
                            catch
                            {
                                //Silently ignore any errors in plugin creation
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<IJsonTextVisualizer> TextVisualizers
        {
            get
            {
                return textVisualizers;
            }
        }

        public IEnumerable<IJsonVisualizer> Visualizers
        {
            get
            {
                return visualizers;
            }
        }

        public IJsonVisualizer DefaultVisualizer
        {
            get
            {
                return _defaultVisualizer;
            }
        }
    }
}
