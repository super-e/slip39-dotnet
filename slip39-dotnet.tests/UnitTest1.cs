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

    //[Test]
    //public void DivisionByZeroThrowsAnException()
    //{
    //    const byte testValue1 = 35;
    //    var sat1 = new FiniteFieldElement(testValue1);

    //    Assert.Throws<ArgumentOutOfRangeException>(() => { var result = sat1 / 0; });
    //}
}