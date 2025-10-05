using NUnit.Framework;
using OpenDive.Utils.Jwt;

namespace Sui.Tests.Jwt
{
    [TestFixture]
    public class JwDecoderTest
    {
        string jwt_sample_1
            = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE0ODUxNDA5ODQsImlhdCI6MTQ4NTEzNzM4NCwiaXNzIjoiYWNtZS5jb20iLCJzdWIiOiIyOWFjMGMxOC0wYjRhLTQyY2YtODJmYy0wM2Q1NzAzMThhMWQiLCJhcHBsaWNhdGlvbklkIjoiNzkxMDM3MzQtOTdhYi00ZDFhLWFmMzctZTAwNmQwNWQyOTUyIiwicm9sZXMiOltdfQ.Mp0Pcwsz5VECK11Kf2ZZNF_SMKu5CgBeLN9ZOP04kZo\n";
        [Test]
        public void DecodeJwtTest()
        {
            JWT jwtDecoded = JWTDecoder.DecodeJWT(jwt_sample_1);

            JWTHeader header = jwtDecoded.Header;
            string algo_expected = "HS256";
            string typ_expected = "JWT";
            Assert.AreEqual(algo_expected, header.alg);
            Assert.AreEqual(typ_expected, header.typ);

            JWTPayload payload = jwtDecoded.Payload;
            long exp_expected = 1485140984; // (long)payload.Exp;
            long iat_expected = 1485137384;
            string iss_expected = "acme.com";
            string sub_expected = "29ac0c18-0b4a-42cf-82fc-03d570318a1d";
            string application_id = "79103734-97ab-4d1a-af37-e006d05d2952";
            string[] roles_expected = { };
            Assert.AreEqual(exp_expected, payload.Exp);
            Assert.AreEqual(iat_expected, payload.Iat);
            Assert.AreEqual(iss_expected, payload.Iss);
            Assert.AreEqual(sub_expected, payload.Sub);
            //Assert.AreEqual(application_id, payload...)
        }
    }

}