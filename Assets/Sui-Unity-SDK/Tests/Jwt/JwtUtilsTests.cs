using System;
using System.Text;
using NUnit.Framework;
using Sui.ZKLogin;

namespace Sui.Tests.Jwt
{
    [TestFixture]
    public class JwtUtilsTests
    {
        [Test]
        public void TestEncode_StringInput()
        {
            string input = "Hello, World!";
            string expected = "SGVsbG8sIFdvcmxkIQ";
            string result = JwtUtils.Base64UrlEncode(input);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestEncode_ByteArrayInput()
        {
            byte[] input = Encoding.UTF8.GetBytes("Hello, ByteArray!");
            string expected = "SGVsbG8sIEJ5dGVBcnJheSE";
            string result = JwtUtils.Base64UrlEncode(input);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestEncode_EmptyStringInput()
        {
            string input = "";
            string expected = "";
            string result = JwtUtils.Base64UrlEncode(input);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestEncode_EmptyByteArrayInput()
        {
            byte[] input = new byte[] { };
            string expected = "";
            string result = JwtUtils.Base64UrlEncode(input);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ExtractClaimValue_ValidClaim_ReturnsCorrectValue()
        {
            // Arrange
            var claim = new Claim
            {
                value = "eyJuYW1lIjoiSm9obiJ9",  // Base64URL encoded '{"name":"John"}'
                indexMod4 = 0
            };

            // Act
            string result = JwtUtils.ExtractClaimValue<string>(claim, "name");

            // Assert
            Assert.AreEqual("John", result);
        }

        [Test]
        public void ExtractClaimValue_InvalidIndex_ThrowsException()
        {
            var claim = new Claim
            {
                value = "eyJuYW1lIjoiSm9obiJ9",
                indexMod4 = 3  // Invalid index
            };

            Assert.Throws<ArgumentException>(() =>
                JwtUtils.ExtractClaimValue<string>(claim, "name"));
        }

        [Test]
        public void ExtractClaimValue_WrongClaimName_ThrowsException()
        {
            var claim = new Claim
            {
                value = "eyJuYW1lIjoiSm9obiJ9",
                indexMod4 = 0
            };

            Assert.Throws<ArgumentException>(() =>
                JwtUtils.ExtractClaimValue<string>(claim, "wrongName"));
        }

        [Test]
        public void ExtractClaimValue_ComplexObject_DeserializesCorrectly()
        {
            var claim = new Claim
            {
                value = "eyJ1c2VyIjp7Im5hbWUiOiJKb2huIiwiYWdlIjozMH19",
                indexMod4 = 0
            };

            var result = JwtUtils.ExtractClaimValue<UserData>(claim, "user");

            Assert.AreEqual("John", result.name);
            Assert.AreEqual(30, result.age);
        }

        [Test]
        public void ExtractClaimValue_ShortInput_ThrowsException()
        {
            var claim = new Claim
            {
                value = "a",  // Too short
                indexMod4 = 0
            };

            Assert.Throws<ArgumentException>(() =>
                JwtUtils.ExtractClaimValue<string>(claim, "test"));
        }

        [Test]
        public void ExtractClaimValue_InvalidBase64Url_ThrowsException()
        {
            var claim = new Claim
            {
                value = "!@#$%^",  // Invalid characters
                indexMod4 = 0
            };

            Assert.Throws<ArgumentException>(() =>
                JwtUtils.ExtractClaimValue<string>(claim, "test"));
        }

        [Test]
        public void ExtractClaimValue_MultipleKeysInJson_ThrowsException()
        {
            var claim = new Claim
            {
                value = "eyJrZXkxIjoidmFsdWUxIiwia2V5MiI6InZhbHVlMiJ9",
                indexMod4 = 0
            };

            Assert.Throws<ArgumentException>(() =>
                JwtUtils.ExtractClaimValue<string>(claim, "key1"));
        }
    }

    [Serializable]
    public class UserData
    {
        public string name;
        public int age;
    }
}