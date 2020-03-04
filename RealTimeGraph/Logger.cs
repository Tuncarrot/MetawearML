/* Program Name : SOA_A1_Client
 * Author       : Adam Tunkiewicz
 * Date         : 2019-12-05
 * Description  : This file holds the logging information
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace RealTimeGraph
{
    public static class Logger
    {
        public static string fileName;
        public static string desktopFilePath;
        public static string filePath;
        public static string folderName;
        public static string folderPath;

        public const string dividerShort = "---";

        //Constructor
        public static void StartLogger(string programName)
        {
            desktopFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            
            folderName = "Sensor_Data";
            fileName = "\\" + programName + "Log.csv";
            folderPath = desktopFilePath + "\\" + folderName;
            filePath = folderPath + fileName;

            try
            {
                if (Directory.Exists(folderPath))
                {
                    var folder = new DirectoryInfo(folderPath);
                    folder.Attributes &= ~FileAttributes.ReadOnly;

                    if (!File.Exists(filePath))
                    {
                        FileStream file = File.Create(filePath);
                        file.Close();
                    }

                }
                else
                {
                    var folder = Directory.CreateDirectory(folderPath);
                    folder.Attributes &= ~FileAttributes.ReadOnly;
                    FileStream file = File.Create(filePath);
                    file.Close();
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("FILE IO ERROR: " + Ex.Message);
            }
        }

        public static string GetDateTime(int levelCode)
        {
            string levelType = ""; ;
            switch (levelCode)
            {
                case Constants.DEBU:
                    levelType = "DEBU";
                    break;
                case Constants.INFO:
                    levelType = "INFO";
                    break;
                case Constants.WARN:
                    levelType = "WARN";
                    break;
                case Constants.ERRO:
                    levelType = "ERRO";
                    break;
                case Constants.FATA:
                    levelType = "FATA";
                    break;
            }

            var currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            var currentTime = DateTime.Now.ToString("HH:mm:ss");

            return levelType + "[" + currentDate + "T" + currentTime + "] ";
        }

        public static string StartServiceMessage(string teamName, string tagName, string serviceName)
        {
            string dividerLong = "=======================================================\n";
            string team = "Team     : " + teamName + "(Adam T, Mateus G, Jaime T, Maneul PR)\n";
            string tag = "Tag-name : " + tagName + "\n";
            string service = "Service  : " + serviceName + "\n";

            return (GetDateTime(Constants.INFO) + dividerLong) +
                   (GetDateTime(Constants.INFO) + team) +
                   (GetDateTime(Constants.INFO) + tag) +
                   (GetDateTime(Constants.INFO) + service) +
                   (GetDateTime(Constants.INFO) + dividerLong) +
                   (GetDateTime(Constants.INFO) + dividerShort);
        }

        public static string Action(string action, int status)
        {
            return GetDateTime(status) + action;
        }

        //public static string SendActionMsg(string )
        //{
        //    string startMsg = "Calling HL7 - Parser with content :\n";
        //    string contentMsg = "\t>> ";

        //}

        public static void LogMsg(string completeMsg)
        {
            File.AppendAllText(filePath, completeMsg + "\n");
        }

    }
}