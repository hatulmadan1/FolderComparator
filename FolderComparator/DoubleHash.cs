using System;
using System.Collections.Generic;
using System.Text;

namespace FolderComparator
{
    public class DoubleHash
    {
        public byte[] MD5Hash { get; set; }
        public byte[] SHA256Hash { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is DoubleHash))
            {
                return false;
            }

            DoubleHash _obj = obj as DoubleHash;

            return ArrayEqual(this.MD5Hash, _obj.MD5Hash) && ArrayEqual(this.SHA256Hash, _obj.SHA256Hash);
        }

        private bool ArrayEqual(byte[] first, byte[] second)
        {
            if (first.Length != second.Length)
            {
                return false;
            }

            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
