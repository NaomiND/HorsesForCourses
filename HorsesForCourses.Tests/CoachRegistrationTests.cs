using HorsesForCourses.Core;

namespace HorsesForCourses.Tests
{
    public class EmailAddressTests
    {
        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user@test.example.com")]
        [InlineData("a@b.co")]
        [InlineData("test.example@uk.co")]
        public void IsValidEmail_ValidEmails_ReturnsTrue(string email)
        {
            Assert.True(EmailHelper.IsValidEmail(email));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("user")]
        [InlineData("user.com")]
        [InlineData("@user.com")]
        [InlineData("user@@example.com")]
        [InlineData("user test@example.com")]
        [InlineData("user,@example.com")]

        public void IsValidEmail_InvalidEmails_ReturnsFalse(string email)
        {
            Assert.False(EmailHelper.IsValidEmail(email));
        }

        [Fact]
        public void Create_ValidEmail_ReturnsEmailAddress()
        {
            var value = "valid@example.com";

            var email = EmailAddress.Create(value);

            Assert.Equal(value, email.Value);
            Assert.Equal(value, email.ToString());
        }

        [Theory]
        [InlineData("")]
        [InlineData("notanemail")]
        [InlineData("invalid@")]
        public void Create_InvalidEmail_ThrowsArgumentException(string email)
        {
            var ex = Assert.Throws<ArgumentException>(() => EmailAddress.Create(email));
            Assert.IsType<ArgumentException>(ex);
        }
    }
}
