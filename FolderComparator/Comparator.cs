using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;

namespace FolderComparator
{
    public class Comparator
    {
        public string FirstPath { get; set; }
        public string SecondPath { get; set; }

        private ConcurrentDictionary<string, DoubleHash> firstFolder;
        private ConcurrentDictionary<string, DoubleHash> secondFolder;

        private Dictionary<string, bool> paired; //for second folder only

        public Comparator()
        {
            firstFolder = new ConcurrentDictionary<string, DoubleHash>();
            secondFolder = new ConcurrentDictionary<string,DoubleHash>();
            paired = new Dictionary<string, bool>();
        }

        private DoubleHash HashFile(string path)
        {
            MD5CryptoServiceProvider MD5Provider = new MD5CryptoServiceProvider();
            SHA256CryptoServiceProvider SHA256Provider = new SHA256CryptoServiceProvider();

            using (FileStream reader = new FileStream(path, FileMode.Open))
            {
                return new DoubleHash
                {
                    MD5Hash = MD5Provider.ComputeHash(reader),
                    SHA256Hash = SHA256Provider.ComputeHash(reader)
                };
            }          
        }

        private void AddFileToDict(int dictNumber, string filePath)
        {
            switch (dictNumber)
            {
                case 1:
                    firstFolder.TryAdd(filePath, HashFile(filePath));
                    break;
                case 2:
                    secondFolder.TryAdd(filePath, HashFile(filePath));
                    break;
            }
        }

        private bool FileHashEquals(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length)
            {
                return false;
            }
            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                {
                    return false;
                }
            }
            return true;
        }

        private bool FileContentEquals(string path1, string path2)
        {
            if (path1.Equals(path2))
            {
                return true;
            }
            using (FileStream reader1 = new FileStream(path1, FileMode.Open), reader2 = new FileStream(path2, FileMode.Open))
            {
                if (reader1.Length != reader2.Length)
                {
                    return false;
                }
                int byte1, byte2;
                while ((byte1 = reader1.ReadByte()) != -1)
                {
                    byte2 = reader2.ReadByte();
                    if (byte1 != byte2)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void FillDict(int dictNumber, string folderPath)
        {
            foreach (var filePath in Directory.GetFiles(folderPath))
            {
                Thread thread = new Thread(() => AddFileToDict(dictNumber, filePath));
                thread.Start();
                thread.Join();
            }
        }

        public ComparsionResult Compare()
        {
            ComparsionResult result = new ComparsionResult();
            bool pairedOnThisStep;

            foreach (var firstFile in firstFolder.Keys)
            {
                DoubleHash firstFileHash = firstFolder[firstFile];
                pairedOnThisStep = false;

                bool keyExists = false;

                foreach (var key in result.SameFiles.Keys)
                {
                    if (FileHashEquals(firstFileHash.MD5Hash, key.MD5Hash) &&
                        FileHashEquals(firstFileHash.SHA256Hash, key.SHA256Hash))
                    {
                        keyExists = true;
                        result.SameFiles[key].Add(firstFile);
                        break;
                    }
                }

                if (!keyExists)
                {
                    result.SameFiles.Add(firstFileHash, new List<string> { firstFile });
                }
                else
                {
                    continue;
                }

                foreach (var secondFile in secondFolder.Keys)
                {
                    if (paired.ContainsKey(secondFile) && paired[secondFile])
                    {
                        continue;
                    }

                    if (FileHashEquals(firstFolder[firstFile].MD5Hash, secondFolder[secondFile].MD5Hash) &&
                        FileHashEquals(firstFolder[firstFile].SHA256Hash, secondFolder[secondFile].SHA256Hash))
                    {
                        result.SameFiles[firstFileHash].Add(secondFile);
                        paired[secondFile] = true;
                        pairedOnThisStep = true;
                    }
                }

                if (!pairedOnThisStep)
                {
                    result.FirstFolderNoMatch.Add(firstFile);
                }
            }

            foreach (var key in secondFolder.Keys)
            {
                if (!paired.ContainsKey(key))
                {
                    result.SecondFolderNoMatch.Add(key);
                }
            }

            return result;
        }
    }
}
