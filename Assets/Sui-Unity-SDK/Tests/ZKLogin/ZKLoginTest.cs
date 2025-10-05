using NUnit.Framework;
using UnityEngine;

namespace Sui.Tests.ZkLogin
{
    [TestFixture]
    public class ZKLoginTest : MonoBehaviour
    {
        [Test]
        public void JwtToAddress_1()
        {
            string jwt = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ijg5Y2UzNTk4YzQ3M2FmMWJkYTRiZmY5NWU2Yzg3MzY0NTAyMDZmYmEiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiI1NzMxMjAwNzA4NzEtMGs3Z2E2bnM3OWllMGpwZzFlaTZpcDV2amUyb3N0dDYuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiI1NzMxMjAwNzA4NzEtMGs3Z2E2bnM3OWllMGpwZzFlaTZpcDV2amUyb3N0dDYuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMDYyODY5MzE5MDYzNjI2MDkyODYiLCJub25jZSI6InhzYnA0XzRZY3RPdkYxSzNuRVFORlhaYXlIayIsIm5iZiI6MTczNjU1NjE2MCwiaWF0IjoxNzM2NTU2NDYwLCJleHAiOjE3MzY1NjAwNjAsImp0aSI6ImU1NDliMWYxYjAzOWJhMWVmZmNlMjQxNTJiM2E0ODBmM2U0ZWY4OTAifQ.Lsou_uz3puLZxVs4mLRxn5RNpGfSVHMi7u5EX1jcEcVVq0v1SD8y03gnhkgNqBKozgU8H9WoGB9pYtWIRB7yxtWhipPJbbMlKmYAborsk_2GTASqy_hDJpYPOfLGHmFgZp4wXzcoi87PFWMBCJu1uzAN4zlZWUAv3-H6YV16TSkx2t-pjtzvfXtK0SJIfGhN5UgmgYHVbOvwTjpPI1ah7bm1aRZzo4DAWtBk9U6fcpHoLm6vSqjSabnvpmktdRzQK4Hk2uzCsfCSsAR0mf2q4WO0vmy_xlZpkjFVv3M3pBA63MuciMxCclXJXxx0RMx9D0TMWvO_-F-XHbKGlI_FXA";
            string userSalt = "254046730921541395157831213406089663029";
            string address = ZKLogin.SDK.Address.JwtToAddress(jwt, userSalt);

            string expected = "0x7a9545d3633d05df805b5b2d7821a2e28ba21765a7b5b8255d22d9857190ca89";
            Assert.AreEqual(expected, address);
        }
    }
}