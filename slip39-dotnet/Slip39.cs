using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using slip39_dotnet.helpers;
using slip39_dotnet.models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

///    Copyright (C) 2022  Super-E-

namespace slip39_dotnet
{
    public class Slip39
    {
        private const int maxTotalShares = 16;
        private const int minThreshold = 1;
        private const int minSecretLength = 16;
        private const int maxIdBytes = 2;
        private const int maxIdBits = 15;
        private const int maxIdValue = (1 << maxIdBits) - 1;
        private const string salt = "shamir";
        private const int iterationBase = 2500;
        private const int maxIterations = 31;

        public  static ShamirPoint Interpolate(FiniteFieldElement desiredIndex, Dictionary<FiniteFieldElement, ShamirPoint> pointsToInterpolate)
        {
            if (pointsToInterpolate == null)  throw new ArgumentNullException(nameof(pointsToInterpolate), "The list of points to interpolate is null."); 
            if (pointsToInterpolate.Count == 0) throw new ArgumentException(nameof(pointsToInterpolate), "The list of points to interpolate must have at least one element.");
            if (pointsToInterpolate.All(p => p.Value.N == pointsToInterpolate.First().Value.N) == false) throw new ArgumentException(nameof(pointsToInterpolate), "All interpolation points must have the same length");


            var result = new ShamirPoint(new byte[pointsToInterpolate.First().Value.N]);
            

            foreach(var currentPoint in pointsToInterpolate)
            {
                var coefficient = FiniteFieldElement.MultiplicationNeutral;
                foreach (var index in pointsToInterpolate.Where(op => op.Key != currentPoint.Key))
                {
                    coefficient *= (desiredIndex - index.Key) / (currentPoint.Key - index.Key);
                }
                result += coefficient * currentPoint.Value;
            }

            return result;
        }

        public static Dictionary<FiniteFieldElement, ShamirPoint> SplitSecret(ShamirPoint secret, int totalShares, int threshold)
        {
            if (totalShares > maxTotalShares) throw new ArgumentOutOfRangeException(nameof(totalShares), $"{nameof(totalShares)} should not be greater than {maxTotalShares}");
            if (threshold > totalShares) throw new ArgumentOutOfRangeException(nameof(threshold), $"{nameof(threshold)} parameter is {threshold}, but should be not be greater than {nameof(totalShares)} = {totalShares}");
            if (threshold < minThreshold) throw new ArgumentOutOfRangeException(nameof(threshold), $"{nameof(threshold)} cannot be less than {minThreshold}.");
            if (secret.N < minSecretLength) throw new ArgumentException(nameof(secret), $"{nameof(secret)} must have a length greater than {minSecretLength * 8} bit.");

            var result = new Dictionary<FiniteFieldElement, ShamirPoint>();
            var pointsToBeInterpolated = new Dictionary<FiniteFieldElement, ShamirPoint>();

            if (threshold == 1)
            {
                for (int i = 0; i < totalShares; i++)
                {
                    result.Add((byte)i, secret);
                }
            }

            for (byte i = 0; i < threshold - 2; i++)
            {
                pointsToBeInterpolated.Add(i, ShamirPoint.GetRandomShamirPoint(secret.N));
            }
            pointsToBeInterpolated.Add((byte)254, secret.GetChecksum());
            pointsToBeInterpolated.Add((byte)255, secret);

            for (byte j = (byte)(threshold - 2); j < totalShares; j++)
            {
                result.Add(j, Interpolate(j, pointsToBeInterpolated));
            }

            pointsToBeInterpolated.Remove((byte)254);
            pointsToBeInterpolated.Remove((byte)255);

            

            return result.MergeLeft(pointsToBeInterpolated); 
        }

        public static ShamirPoint RecoverSecret(int threshold, Dictionary<FiniteFieldElement, ShamirPoint> shares)
        {
            if (shares == null) throw new ArgumentNullException(nameof(shares));
            if (threshold < 1) throw new ArgumentOutOfRangeException(nameof(threshold), $"{nameof(threshold)} must be at least equal to {minThreshold}");
            if (shares.Count < threshold) throw new ArgumentException(nameof(shares), $"Not enough secret shares ({shares.Count}), need at least {threshold}");
            if (threshold == 1) return shares.First().Value;

            var secret = Interpolate((byte)255, shares);
            var checksum = Interpolate((byte)254, shares);
            if (!secret.VerifyChecksum(checksum)) throw new Exception($"Secret checksum does not match");
            return secret; 
        }

        public static ShamirPoint Encrypt(ShamirPoint masterSecret, byte e, byte[] bigEndianId, string P = "")
        {
            if (masterSecret is null) throw new ArgumentNullException(nameof(masterSecret), $"{masterSecret} parameter cannot be null");
            if (e > maxIterations) throw new ArgumentException(nameof(e), $"{nameof(e)} parameter should be at most {maxIterations}, instead has {e}");
            if (bigEndianId.Length > maxIdBytes) throw new ArgumentException(nameof(bigEndianId), $"{nameof(bigEndianId)} parameter has {bigEndianId.Length} bytes, should have at most {maxIdBytes} bytes");

            

            int idValue;
            if (BitConverter.IsLittleEndian)
            {
                idValue = BitConverter.ToInt16(bigEndianId.Reverse<byte>().ToArray<byte>(), 0);
            }
            else
            {
                idValue = BitConverter.ToInt16(bigEndianId, 0);
            }
                
            
            if (idValue > maxIdValue) throw new ArgumentOutOfRangeException(nameof(bigEndianId), $"{nameof(bigEndianId)} parameter has value {idValue}, should be at most {maxIdValue}.");

            var left = masterSecret.byteArrayValue.Take<byte>(masterSecret.N / 2).ToArray<byte>();
            var right = masterSecret.byteArrayValue.TakeLast<byte>(masterSecret.N / 2).ToArray<byte>();

            for (int i = 0; i < 4; i++)
            {
                (left, right) = (right, left.XOR(F(i, right, Encoding.UTF8.GetBytes(P), bigEndianId, e)));
            }

            return new ShamirPoint(right.Append(left));
        }

        public static ShamirPoint Dencrypt(ShamirPoint encryptedMasterSecret, byte e, byte[] id, string P = "")
        {
            if (encryptedMasterSecret is null) throw new ArgumentNullException(nameof(encryptedMasterSecret), $"{encryptedMasterSecret} parameter cannot be null");
            if (e > maxIterations) throw new ArgumentException(nameof(e), $"{nameof(e)} parameter should be at most {maxIterations}, instead has {e}");
            if (id.Length > maxIdBytes) throw new ArgumentException(nameof(id), $"{nameof(id)} parameter has {id.Length} bytes, should have at most {maxIdBytes} bytes");

            var idValue = BitConverter.ToInt16(id, 0);
            if (idValue > maxIdValue) throw new ArgumentOutOfRangeException(nameof(id), $"{nameof(id)} parameter has value {idValue}, should be at most {maxIdValue}.");

            var left = encryptedMasterSecret.byteArrayValue.Take<byte>(encryptedMasterSecret.N / 2).ToArray<byte>();
            var right = encryptedMasterSecret.byteArrayValue.TakeLast<byte>(encryptedMasterSecret.N / 2).ToArray<byte>();

            for (int i = 3; i >= 0; i--)
            {
                (left, right) = (right, left.XOR(F(i, right, Encoding.UTF8.GetBytes(P), id, e)));
            }

            return new ShamirPoint(right.Append(left));
        }

        private static byte[] F(int i, byte[] R, byte[] passphrase, byte[] bigEndianId, int iterationExponent)
        {

            var password = new List<byte>();
            password.Add((byte)i);
            password.AddRange(passphrase);


            var _salt = new List<byte>();
            _salt.AddRange(Encoding.UTF8.GetBytes(salt));
            _salt.AddRange(bigEndianId);
            _salt.AddRange(R);

            return KeyDerivation.Pbkdf2(Encoding.UTF8.GetString(password.ToArray<byte>()), _salt.ToArray<byte>(), KeyDerivationPrf.HMACSHA256, iterationBase << iterationExponent, R.Length);
        }

 
    }
}
