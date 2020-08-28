using System;
using System.Collections.Generic;
using System.Text;

namespace FolderComparator
{
    public class ComparsionResult
    {
        public Dictionary<DoubleHash, List<string>> SameFiles;
        public List<string> FirstFolderNoMatch;
        public List<string> SecondFolderNoMatch;

        public ComparsionResult()
        {
            SameFiles = new Dictionary<DoubleHash, List<string>>();
            FirstFolderNoMatch = new List<string>();
            SecondFolderNoMatch = new List<string>();
        }
    }
}
