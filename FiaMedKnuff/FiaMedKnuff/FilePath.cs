using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiaMedKnuff
{
    /// <summary>
    /// A class of constants useful for this application
    /// </summary>
    public static class FilePath
    {
        /// <summary>
        /// The main path to the folder for this application
        /// </summary>
        private static readonly string MAIN_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        /// <summary>
        /// The path to the user data folder
        /// </summary>
        public static readonly string USER_PATH = Path.Combine(MAIN_PATH, "FiaMedKnuff\\users");
        /// <summary>
        /// The path to the config data folder
        /// </summary>
        public static readonly string CONFIG_PATH = Path.Combine(MAIN_PATH, "FiaMedKnuff\\config");
        
    }
}
