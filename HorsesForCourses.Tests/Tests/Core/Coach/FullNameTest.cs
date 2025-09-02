using HorsesForCourses.Core;

namespace HorsesForCourses.Tests
{
    public class FullNameTests
    {
        [Fact]
        public void Constructor_SetsFirstAndLastName_WithValidInput()
        {
            var fullName = new FullName("Ine", "De Wit");

            Assert.Equal("Ine", fullName.FirstName);
            Assert.Equal("De Wit", fullName.LastName);
            Assert.Equal("Ine De Wit", fullName.DisplayName);
            Assert.Equal("Ine De Wit", fullName.ToString());
        }

        [Theory]
        [InlineData(null, "De Wit", "Voornaam verplicht")]
        [InlineData("Ine", null, "Achternaam verplicht")]

        public void Constructor_ShouldThrow_WhenNamePartsAreNull(string firstName, string lastName, string expectedMessagePart)
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new FullName(firstName, lastName));

            Assert.Contains(expectedMessagePart, ex.Message);
        }

        [Fact]
        public void From_ShouldCreateFullName_WhenInputHasFirstAndLastName()
        {
            var input = "Ine Van Den Broeck";

            var fullName = FullName.From(input);

            Assert.Equal("Ine", fullName.FirstName);
            Assert.Equal("Van Den Broeck", fullName.LastName);
        }

        [Theory]
        [InlineData(null, "Volledige naam verplicht.")]
        [InlineData("", "Volledige naam verplicht.")]
        [InlineData("   ", "Volledige naam verplicht.")]
        [InlineData("Ine", "Naam moet een voor- en achternaam bevatten.")]
        public void From_ShouldThrowArgumentException_WhenInputIsInvalid(string input, string expectedMessage)
        {
            var ex = Assert.Throws<ArgumentException>(() => FullName.From(input));

            Assert.Contains(expectedMessage, ex.Message);
        }
    }
}