///    Copyright (C) 2022  Super-E-

using slip39_dotnet.helpers;
using slip39_dotnet.models;

namespace slip39_dotnet.tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void DefaultElementIsZero()
    {
        var sat = new FiniteFieldElement();
        Assert.That((byte)sat, Is.EqualTo(0));
    }

    [Test]
    public void ElementsAreEqualIIFHaveTheSameValue()
    {
        var sat1 = new FiniteFieldElement(35);
        var sat2 = new FiniteFieldElement(35);
        var sat3 = new FiniteFieldElement(42);
        Assert.Multiple(() =>
        {
            Assert.That(sat2, Is.EqualTo(sat1));
            Assert.That(sat3, Is.Not.EqualTo(sat1));
            Assert.That(sat1 == sat2);
            Assert.That(sat1 != sat3);
        });
    }

    [Test]
    public void ElementCanBeInitializedWithAByte()
    {
        const byte testValue = (byte)144;
        var sat = new FiniteFieldElement(testValue);
        Assert.That((byte)sat, Is.EqualTo(testValue));
    }

    [Test]
    public void ElementCanBeInitializedWithAnInteger()
    {
        const int testValue = 144;
        var sat = new FiniteFieldElement(testValue);
        Assert.That((byte)sat, Is.EqualTo(testValue));
    }

    [Test]
    public void SumOfElementsIsXOR()
    {
        const byte testValue1 = 11;
        const byte testValue2 = 22;
        var sat1 = new FiniteFieldElement(testValue1);
        var sat2 = new FiniteFieldElement(testValue2);

        Assert.That((byte)(sat1 + sat2), Is.EqualTo(testValue1 ^ testValue2));
    }

    [Test]
    public void SumOfElementsIsEqualToSubtraction()
    {
        const byte testValue1 = 11;
        const byte testValue2 = 22;
        var sat1 = new FiniteFieldElement(testValue1);
        var sat2 = new FiniteFieldElement(testValue2);

        Assert.That((byte)(sat1 + sat2), Is.EqualTo((byte)(sat1 - sat2)));
    }

    [Test]
    public void SumIsCommutative()
    {
        const byte testValue1 = 11;
        const byte testValue2 = 22;
        var sat1 = new FiniteFieldElement(testValue1);
        var sat2 = new FiniteFieldElement(testValue2);

        Assert.That((byte)(sat2 + sat1), Is.EqualTo((byte)(sat1 + sat2)));
    }

    [Test]
    public void MultiplicationIsCommutative()
    {
        const byte testValue1 = 125;
        const byte testValue2 = 222;
        var sat1 = new FiniteFieldElement(testValue1);
        var sat2 = new FiniteFieldElement(testValue2);

        Assert.That((byte)(sat2 * sat1), Is.EqualTo((byte)(sat1 * sat2)));
    }

    [Test]
    public void MultiplicationIsCorrect()
    {
        const byte testValue1 = 35;
        const byte testValue2 = 36;
        const byte expected = 128;
        var sat1 = new FiniteFieldElement(testValue1);
        var sat2 = new FiniteFieldElement(testValue2);

        Assert.That((byte)(sat2 * sat1), Is.EqualTo(expected));
    }

    [Test]
    public void ZeroIsAdditionNeutralElement()
    {
        const byte testValue1 = 35;
        const byte testValue2 = 0;
        const byte expected = 35;
        var sat1 = new FiniteFieldElement(testValue1);
        var sat2 = new FiniteFieldElement(testValue2);

        Assert.That((byte)(sat2 + sat1), Is.EqualTo(expected));
    }

    [Test]
    public void ElementSummedToOppositeIsZero()
    {
        const byte testValue1 = 35;
        const byte expected = 0;
        var sat1 = new FiniteFieldElement(testValue1);


        Assert.That((byte)(sat1 + (-sat1)), Is.EqualTo(expected));
    }

    [Test]
    public void OneIsMultiplicationNeutralElement()
    {
        const byte testValue1 = 35;
        const byte testValue2 = 1;
        const byte expected = 35;
        var sat1 = new FiniteFieldElement(testValue1);
        var sat2 = new FiniteFieldElement(testValue2);

        Assert.That((byte)(sat2 * sat1), Is.EqualTo(expected));
    }

    [Test]
    public void OneIsDivisionNeutralElement()
    {
        const byte testValue1 = 35;
        const byte testValue2 = 1;
        const byte expected = 35;
        var sat1 = new FiniteFieldElement(testValue1);
        var sat2 = new FiniteFieldElement(testValue2);

        Assert.That((byte)(sat1 / sat2), Is.EqualTo(expected));
    }

    [Test]
    public void DivisionIsInverseOfMultiplication()
    {
        const byte testValue1 = 35;
        const byte testValue2 = 222;
        const byte expected = 35;
        var sat1 = new FiniteFieldElement(testValue1);
        var sat2 = new FiniteFieldElement(testValue2);

        Assert.That((byte)((sat1 / sat2) * sat2), Is.EqualTo(expected));
    }

    [Test]
    public void InterpolationVectorHasSizeGreaterThan0()
    {
        Dictionary<FiniteFieldElement, ShamirPoint> testValue1 = new();

        var x = new FiniteFieldElement(byte.MinValue);

        Assert.Throws<ArgumentException>(() => { var result = Slip39.Interpolate(x, testValue1); ; });
    }

    [Test]
    public void InterpolationVectorCannotBeNull()
    {
        Dictionary<FiniteFieldElement, ShamirPoint> testValue1 = null;

        var x = new FiniteFieldElement(byte.MinValue);

        Assert.Throws<ArgumentNullException>(() => { var result = Slip39.Interpolate(x, testValue1); });
    }

    [Test]
    public void ShamirPointsAreTheSameIfAllBytesAreTheSame()
    {
        var sat1 = new ShamirPoint("123456789abcdeff");
        var sat2 = new ShamirPoint("123456789abcdeff");

        Assert.That(sat1, Is.EqualTo(sat2));
    }

    [Test]
    public void ShamirPointsAreTheDifferentIfAtLeastOneBitIsDifferent()
    {
        var sat1 = new ShamirPoint("123456789abcdeff");
        var sat2 = new ShamirPoint("123456789abcdefe");

        Assert.That(sat1, Is.Not.EqualTo(sat2));
    }

    [Test]
    public void ShamirPointsAreDifferentIfLengthIsDifferent()
    {
        var sat1 = new ShamirPoint("123456789abcdeff");
        var sat2 = new ShamirPoint("123456789abcde");

        Assert.That(sat1, Is.Not.EqualTo(sat2));
    }

    [Test]
    public void ShamirPointsAreEqualIfLengthIsZero()
    {
        var sat1 = new ShamirPoint("");
        var sat2 = new ShamirPoint("");

        Assert.That(sat1, Is.EqualTo(sat2));
    }

    [Test]
    public void InterpolationSample1()
    {
        var Y1 = new ShamirPoint("A3");
        var Y2 = new ShamirPoint("67");
        var Y7 = new ShamirPoint("E2");
        var X1 = new FiniteFieldElement(1);
        var X2 = new FiniteFieldElement(2);
        var X7 = new FiniteFieldElement(7);
        var XFF = new FiniteFieldElement(255);

        var dic = new Dictionary<FiniteFieldElement, ShamirPoint>()
        {
            { X1, Y1 }, {X2, Y2 }, {X7, Y7 },

        };

        var result = Slip39.Interpolate(XFF, dic);
        var expected = new ShamirPoint("A3");

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void InterpolationSample2()
    {
        var Y1 = new ShamirPoint("A338");
        var Y2 = new ShamirPoint("67F5");
        var Y7 = new ShamirPoint("E26F");
        var X1 = new FiniteFieldElement(1);
        var X2 = new FiniteFieldElement(2);
        var X7 = new FiniteFieldElement(7);
        var XFF = new FiniteFieldElement(255);

        var dic = new Dictionary<FiniteFieldElement, ShamirPoint>()
        {
            { X1, Y1 }, {X2, Y2 }, {X7, Y7 },

        };

        var result = Slip39.Interpolate(XFF, dic);
        var expected = new ShamirPoint("A394");

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void TestChecksum()
    {
        var sat = new ShamirPoint("123456789a");
        var checksum = sat.GetChecksum();

        Assert.That(sat.VerifyChecksum(checksum), Is.True);
    }

    [Test]
    public void TestShareGeneration()
    {
        var sat = new ShamirPoint("a33456789a123456789a123456789a123456789a");
        var shares = Slip39.SplitSecret(sat, 5, 3);
        var calculatedSecret = Slip39.RecoverSecret(3, shares.Take(3).ToDictionary(x => x.Key, x => x.Value));


        Assert.That(sat, Is.EqualTo(calculatedSecret));
    }

    [Test]
    public void TestShareGenerationFailingIfLessSharesThanThreshold()
    {
        var sat = new ShamirPoint("a33456789a123456789a123456789a123456789a");
        var shares = Slip39.SplitSecret(sat, 5, 3);
        Assert.That(() => Slip39.RecoverSecret(3, shares.Take(2).ToDictionary(x => x.Key, x => x.Value)), Throws.Exception.TypeOf<ArgumentException>());

    }

    [Test]
    public void TestShareGenerationFailingIfLessSharesThanNeeded()
    {
        var sat = new ShamirPoint("a33456789a123456789a123456789a123456789a");
        var shares = Slip39.SplitSecret(sat, 5, 3);

        Assert.That(() => Slip39.RecoverSecret(2, shares.Take(2).ToDictionary(x => x.Key, x => x.Value)), Throws.Exception.TypeOf<Exception>());

    }

    [Test]
    public void SharesAreAllEqualToTheSecretIfThresholdIsOne()
    {
        var sat = new ShamirPoint("a33456789a123456789a123456789a123456789a");
        var shares = Slip39.SplitSecret(secret: sat, totalShares: 5, threshold: 1);

        Assert.That(shares.All(x => x.Value == sat), Is.True);

    }

    [Test]
    public void TestEncryptionAndDecryption()
    {
        var sat = new ShamirPoint("a33456789a123456789a123456789a123456789a");
        var id = BitConverter.GetBytes((short)31407);
        if (BitConverter.IsLittleEndian) id = id.Reverse<byte>().ToArray<byte>();

        var encrypted = Slip39.Encrypt(sat, 2, id);
        var decrypted = Slip39.Dencrypt(encrypted, 2, id);


        Assert.That(decrypted, Is.EqualTo(sat));

    }

    [Test]
    public void TestEncryptionAndDecryptionWithPassword()
    {
        var sat = new ShamirPoint("a33456789a123456789a123456789a123456789a");
        var id = BitConverter.GetBytes((short)31407);
        var password = "test password";
        if (BitConverter.IsLittleEndian) id = id.Reverse<byte>().ToArray<byte>();

        var encrypted = Slip39.Encrypt(sat, 2, id, password);
        var decrypted = Slip39.Dencrypt(encrypted, 2, id, password);

        Assert.That(decrypted, Is.EqualTo(sat));
    }

    [Test]
    public void TestEncryptionAndDecryptionFailsWithDifferentPasswords()
    {
        var sat = new ShamirPoint("a33456789a123456789a123456789a123456789a");
        var id = BitConverter.GetBytes((short)31407);
        var password1 = "test password";
        var password2 = "different password";
        if (BitConverter.IsLittleEndian) id = id.Reverse<byte>().ToArray<byte>();

        var encrypted = Slip39.Encrypt(sat, 2, id, password1);
        var decrypted = Slip39.Dencrypt(encrypted, 2, id, password2);

        Assert.That(decrypted, Is.Not.EqualTo(sat));
    }

    [Test]
    public void ShamirShareChecksumSuccess()
    {
        var secret = new ShamirPoint("a33456789a123456789a123456789a123456789a");

        Assert.That(() => new ShamirShare(12344, 2, 0, 3, 5, 1, 3, secret), Throws.Nothing);
    }

    [Test]
    public void ShamirShareSecretTooShortShouldFail()
    {
        var secret = new ShamirPoint("a3345678");

        Assert.That(() => new ShamirShare(12344, 2, 0, 3, 5, 1, 3, secret), Throws.ArgumentException);
    }

    [Test]
    public void Sandbox()
    {
        var secret = new ShamirPoint("bb54aac4b89dc868ba37d9cc21b2cece");

        var a = new ShamirShare(12344, 0, 0, 0, 0, 0, 0, secret);
        var b = a.ToString();
    }
}