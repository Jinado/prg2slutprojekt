using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FiaMedKnuff
{
    public static class FileHandler
    {
        /// <summary>
        /// Saves a user's data
        /// </summary>
        /// <param name="name">The name of the user</param>
        /// <param name="gamesWon">The amount of games won by the user</param>
        /// <param name="gamesLost">The amount of games lost by the user</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static void SaveUserData(string name, int gamesWon, int gamesLost)
        {
            if (name == null)
                throw new ArgumentNullException();

            if (gamesWon < 0 || gamesLost < 0)
                throw new ArgumentException("The \"gamesLost\" and \"gamesWon\" arguments cannot be less than 0");

            try
            {
                // Makes sure to create the FiaMedKnuff/users directory
                Directory.CreateDirectory(StringConstants.USER_PATH);

                FileStream fs = new FileStream(Path.Combine(StringConstants.USER_PATH, name + ".fmk"), FileMode.Create, FileAccess.Write);
                BinaryWriter bw = new BinaryWriter(fs);

                bw.Write(name);
                bw.Write(gamesWon);
                bw.Write(gamesLost);

                bw.Dispose();
                fs.Dispose();
            }
            catch (Exception err)
            {
                MessageBox.Show($"Fel vid nersparing av data.\n{err.Message}", "Fel", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Saves the last used IP into a file so that it may be read and automatically filled out
        /// </summary>
        /// <param name="ip">IP-address to store</param>
        /// <exception cref="ArgumentException"></exception>
        public static void SaveConfigData(string ip)
        {
            if (!IPAddress.TryParse(ip, out _))
                throw new ArgumentException("The provided argument was not correctly formatted");

            try
            {
                // Makes sure to create the FiaMedKnuff/config directory
                Directory.CreateDirectory(StringConstants.CONFIG_PATH);

                FileStream fs = new FileStream(Path.Combine(StringConstants.CONFIG_PATH, "config.fmkc"), FileMode.Create, FileAccess.Write);
                BinaryWriter bw = new BinaryWriter(fs);

                bw.Write(ip);

                bw.Dispose();
                fs.Dispose();
            }
            catch(Exception err)
            {
                MessageBox.Show($"Fel vid nersparing av data.\n{err.Message}", "Fel", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Reads the user data of the user with the specified name
        /// </summary>
        /// <param name="name">The name of the user</param>
        /// <param name="gamesWon">The amount of games the user has won</param>
        /// <param name="gamesLost">The amount of games the user has lost</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ReadUserData(string name, out int gamesWon, out int gamesLost)
        {
            if (name == null)
                throw new ArgumentNullException();

            try
            {
                // Makes sure to create the FiaMedKnuff/users directory
                Directory.CreateDirectory(StringConstants.USER_PATH);

                FileStream fs = new FileStream(Path.Combine(StringConstants.USER_PATH, name + ".fmk"), FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);

                br.ReadString();
                gamesWon = br.ReadInt32();
                gamesLost = br.ReadInt32();

                br.Dispose();
                fs.Dispose();
                return;
            }
            catch(Exception err)
            {
                MessageBox.Show($"Fel vid läsning av användardata.\n{err.Message}", "Fel", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            gamesWon = 0;
            gamesLost = 0;
        }

        /// <summary>
        /// Reads config data for the application
        /// </summary>
        /// <returns>The last used IP</returns>
        public static string ReadConfigData()
        {
            try
            {
                // Makes sure to create the FiaMedKnuff/config directory
                Directory.CreateDirectory(StringConstants.CONFIG_PATH);

                FileStream fs = new FileStream(Path.Combine(StringConstants.CONFIG_PATH, "config.fmkc"), FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);

                string ip = br.ReadString();

                br.Dispose();
                fs.Dispose();
                return ip;
            }
            catch (Exception err)
            {
                MessageBox.Show($"Fel vid läsning av användardata.\n{err.Message}", "Fel", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return "Error";
        }
    }
}
