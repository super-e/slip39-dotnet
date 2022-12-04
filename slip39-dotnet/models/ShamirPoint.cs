using slip39_dotnet.helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace slip39_dotnet.models
{
    public sealed class ShamirPoint
    {
        List<FiniteFieldElement> Y { get; set; }
        public ShamirPoint(List<FiniteFieldElement> Y) { this.Y = Y; }
        public ShamirPoint(byte[] Y)
        {
            var result = new List<FiniteFieldElement>();
            foreach(byte bY in Y)
            {
                result.Add((FiniteFieldElement) bY);
            }
            this.Y = result;
        }
        public ShamirPoint() => throw new ArgumentException();

        public ShamirPoint(string Y) : this(StringToByteArray(Y)) { }

        public int N => Y.Count();


        public ShamirPoint GetChecksum()
        {
            if (N < 4) throw new Exception($"Cannot calculate checksum, secret is too short ({N * 8} bit): must be at least 32 bit");
            var rnd = RandomNumberGenerator.Create();
            var R = new byte[N - 4];
            rnd.GetBytes(R);

            var hmac = new HMACSHA256(R);
            var result = hmac.ComputeHash(this.byteArrayValue).Take(4).ToArray().Concat(R).ToArray();
         
            
            return new ShamirPoint(result);
        }

        public bool VerifyChecksum(ShamirPoint checksum)
        {
            if (checksum is null) return false;
            if (checksum.N != this.N) return false;

            var key = checksum.byteArrayValue.TakeLast(this.N - 4);
            var verification = checksum.byteArrayValue.Take<byte>(4);
            var hmac = new HMACSHA256(key);
            var computedHash = hmac.ComputeHash(this.byteArrayValue).Take<byte>(4) ;
            
            return Enumerable.SequenceEqual(verification, computedHash);
        }

        static public ShamirPoint GetRandomShamirPoint(int byteLength)
        {
            var rnd = RandomNumberGenerator.Create();
            var tempBytes = new byte[byteLength];
            rnd.GetBytes(tempBytes);
            return new ShamirPoint(tempBytes);
        }

        public override string ToString()
        {
            return Encoding.Default.GetString(this.byteArrayValue);
        }

        public byte[] byteArrayValue
        {
            get
            {
                return Y.Select(x => (byte) x).ToArray();
            }
        }

        public FiniteFieldElement this[int index]
        {
            get => Y[index];
            set => Y[index] = value;
        }

        static public ShamirPoint operator +(ShamirPoint a, ShamirPoint b)
        {
            if (a.N != b.N) throw new ArgumentException();
            var result = new ShamirPoint(new byte[a.N]);
            for(int i = 0; i < a.N; i++)
            {
                result[i] = a[i] + b[i];
            }
            return result;
        }

        static public ShamirPoint operator -(ShamirPoint a, ShamirPoint b)
        {
            if (a.N != b.N) throw new ArgumentException();
            var result = new ShamirPoint(new byte[a.N]);
            for (int i = 0; i < a.N; i++)
            {
                result[i] = a[i] - b[i];
            }
            return result;
        }
        static public ShamirPoint operator *(ShamirPoint a, ShamirPoint b)
        {
            if (a.N != b.N) throw new ArgumentException();
            var result = new ShamirPoint(new byte[a.N]);
            for (int i = 0; i < a.N; i++)
            {
                result[i] = a[i] * b[i];
            }
            return result;
        }

        static public ShamirPoint operator *(FiniteFieldElement a, ShamirPoint b)
        {
            var result = new ShamirPoint(new byte[b.N]);
            for (int i = 0; i < b.N; i++)
            {
                result[i] = a * b[i];
            }
            return result;
        }

        static public ShamirPoint operator /(ShamirPoint a, ShamirPoint b)
        {
            if (a.N != b.N) throw new ArgumentException();
            var result = new ShamirPoint(new byte[a.N]);
            for (int i = 0; i < a.N; i++)
            {
                result[i] = a[i] / b[i];
            }
            return result;
        }

        static public bool operator ==(ShamirPoint a, ShamirPoint b)
        {
            if (a.N != b.N) throw new ArgumentException();
            var result = true;  
            for (int i = 0; i < a.N; i++)
            {
                result &= a[i] == b[i];
            }
            return result;
        }

        static public bool operator !=(ShamirPoint a, ShamirPoint b)
        {
            if (a.N != b.N) return true;
            var result = false;
            for (int i = 0; i < a.N; i++)
            {
                result |= a[i] != b[i];
            }
            return result;
        }

               
        public override bool Equals(object obj)
        {
            return obj is ShamirPoint element && Equals(element);
        }

        public bool Equals(ShamirPoint other)
        {
            if (this.N != other.N) return false;
            var result = true;
            for (int i = 0; i < this.N; i++)
            {
                result &= this[i].Equals(other[i]);
            }
            return result;
        }

        

        public override int GetHashCode()
        {
            return -196543833  + Y.GetHashCode();
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

    }
}
