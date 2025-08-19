using FluentAssertions;
using KonturTestTask.Exceptions;
using KonturTestTask.Extensions;
using KonturTestTask.Helpers;

namespace KonturTests
{
    public class ValidateSchemaTest : BaseTest
    {
        [Theory]
        [InlineData("Data1.xml", false)]
        [InlineData("Data2.xml", false)]
        [InlineData("Data3.xml", false)]

        [InlineData("Data1ErrorInvalidMount.xml", true)]
        [InlineData("Data2ErrorInvalidMount.xml", true)]

        [InlineData("Data1ErrorEmptyName.xml", true)]
        [InlineData("Data1ErrorEmptySurname.xml", true)]
        [InlineData("Data1ErrorEmptyAmount.xml", true)]
        [InlineData("Data1ErrorEmptyMount.xml", true)]

        [InlineData("Data2ErrorEmptyName.xml", true)]
        [InlineData("Data2ErrorEmptySurname.xml", true)]
        [InlineData("Data2ErrorEmptyAmount.xml", true)]
        [InlineData("Data2EmptyMount.xml", false)]
        public void ValidateTest(string documentName, bool hasNoErrorExpected)
        {
            var doc = LoadDocumentFromTestData(documentName);

            var schemas = ResourceHelper.LoadSchemaSetFromResources();

            Action action = () => doc.ValidateDocument(schemas);

            if (!hasNoErrorExpected)
            {
                action.Should().NotThrow<Exception>();
            }
            else
            {
                action.Should().Throw<CustomException>().Where(e => e.Message.StartsWith("Ошибка валидации"));
            }
        }
    }
}
