﻿using System;
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
        /// Saves a <see cref="Player"/>'s data
        /// </summary>
        /// <param name="name">The name of the <see cref="Player"/></param>
        /// <param name="gamesWon">The amount of games won by the <see cref="Player"/></param>
        /// <param name="gamesLost">The amount of games lost by the <see cref="Player"/></param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        public static void SaveUserData(string name, int gamesWon, int gamesLost)
        {
            if (name == null)
                throw new ArgumentNullException();

            if (gamesWon < 0 || gamesLost < 0)
                throw new ArgumentException("The \"gamesLost\" and \"gamesWon\" arguments cannot be less than 0");

            try
            {
                // Makes sure to create the FiaMedKnuff/users directory if it does not exist
                Directory.CreateDirectory(FilePath.USER_PATH);

                FileStream fs = new FileStream(Path.Combine(FilePath.USER_PATH, name + ".fmk"), FileMode.Create, FileAccess.Write);
                BinaryWriter bw = new BinaryWriter(fs);

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
        /// Saves the last, successfully, used IP-address into a file so that it may be read and automatically filled out
        /// </summary>
        /// <param name="ip">IP-address to store</param>
        public static void SaveConfigData(string ip)
        {
            try
            {
                // Makes sure to create the FiaMedKnuff/config directory if it does not exist
                Directory.CreateDirectory(FilePath.CONFIG_PATH);

                FileStream fs = new FileStream(Path.Combine(FilePath.CONFIG_PATH, "config.fmkc"), FileMode.Create, FileAccess.Write);
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
        /// Reads the user data of the <see cref="Player"/> with the specified name
        /// </summary>
        /// <param name="name">The name of the <see cref="Player"/></param>
        /// <param name="gamesWon">The amount of games the <see cref="Player"/> has won</param>
        /// <param name="gamesLost">The amount of games the <see cref="Player"/> has lost</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FileNotFoundException"/>
        public static void ReadUserData(string name, ref int gamesWon, ref int gamesLost)
        {
            if (name == null)
                throw new ArgumentNullException();

            try
            {
                // Makes sure to create the FiaMedKnuff/users directory if it does not exist
                Directory.CreateDirectory(FilePath.USER_PATH);

                FileStream fs = new FileStream(Path.Combine(FilePath.USER_PATH, name + ".fmk"), FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);

                gamesWon = br.ReadInt32();
                gamesLost = br.ReadInt32();

                br.Dispose();
                fs.Dispose();
                return;
            }
            catch(Exception err)
            {
                if (err is FileNotFoundException) throw err;
                MessageBox.Show($"Fel vid läsning av användardata.\n{err.Message}", "Fel", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Reads config data for the application
        /// </summary>
        /// <returns>The last used IP</returns>
        /// <exception cref="FileNotFoundException"/>
        public static string ReadConfigData()
        {
            try
            {
                // Makes sure to create the FiaMedKnuff/config directory
                Directory.CreateDirectory(FilePath.CONFIG_PATH);

                FileStream fs = new FileStream(Path.Combine(FilePath.CONFIG_PATH, "config.fmkc"), FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);

                string ip = br.ReadString();

                br.Dispose();
                fs.Dispose();
                return ip;
            }
            catch (Exception err)
            {
                if (err is FileNotFoundException)
                    throw err;
                MessageBox.Show($"Fel vid läsning av användardata.\n{err.Message}", "Fel", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            throw new Exception("Something wennt wrong and we're not sure what");
        }
    }
}
