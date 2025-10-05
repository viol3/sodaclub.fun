using System.Numerics;
using NUnit.Framework;
using OpenDive.Crypto.PoseidonLite;
using Sui.Cryptography.Ed25519;
using Sui.ZKLogin.SDK;

namespace Sui.Tests.PoseidonHash
{
    [TestFixture]
    public class PoseidonTest
    {
        [Test]
        public void PoseidonHasherTest_1()
        {
            PrivateKey pk = new PrivateKey(new byte[32]);
            PublicKey publicKey = (PublicKey)pk.PublicKey();

            byte[] publicKeyBytes = publicKey.ToSuiBytes();
            BigInteger publicKeyBigInt = NonceGenerator.ToBigIntBE(publicKeyBytes);

            BigInteger eph_public_key_0 = publicKeyBigInt / BigInteger.Pow(2, 128);
            BigInteger eph_public_key_1 = publicKeyBigInt % BigInteger.Pow(2, 128);

            BigInteger bigNum = PoseidonHasher.PoseidonHash(new[] {
                eph_public_key_0,
                eph_public_key_1,
                new BigInteger(0),
                new BigInteger(91593735651025872)
            });

            BigInteger expected = BigInteger.Parse(
                "11528381754461914166874197622133763632947811144082871006931258895171226799250"
            );

            Assert.AreEqual(expected, bigNum, "OUTPUT: " + bigNum);
        }

        [Test]
        public void PoseidonHasherTest_2()
        {
            PrivateKey pk = new PrivateKey(new byte[32]);
            PublicKey publicKey = (PublicKey)pk.PublicKey();

            byte[] publicKeyBytes = publicKey.ToSuiBytes();
            BigInteger publicKeyBigInt = NonceGenerator.ToBigIntBE(publicKeyBytes);

            BigInteger eph_public_key_0 = publicKeyBigInt / BigInteger.Pow(2, 128);
            BigInteger eph_public_key_1 = publicKeyBigInt % BigInteger.Pow(2, 128);

            BigInteger bigNum = PoseidonHasher.PoseidonHash(new[] {
                eph_public_key_0,
                eph_public_key_1,
                new BigInteger(0),
                new BigInteger(915937356510258724)
            });

            BigInteger expected = BigInteger.Parse(
                "1251557835947443048337993173967560552083820691731823376970830811540577977572"
            );

            Assert.AreEqual(expected, bigNum, "OUTPUT: " + bigNum);
        }

        [Test]
        public void PoseidonHasherTest_3()
        {
            PrivateKey pk = new PrivateKey(new byte[32]);
            PublicKey publicKey = (PublicKey)pk.PublicKey();

            byte[] publicKeyBytes = publicKey.ToSuiBytes();
            BigInteger publicKeyBigInt = NonceGenerator.ToBigIntBE(publicKeyBytes);

            BigInteger eph_public_key_0 = publicKeyBigInt / BigInteger.Pow(2, 128);
            BigInteger eph_public_key_1 = publicKeyBigInt % BigInteger.Pow(2, 128);

            BigInteger bigNum = PoseidonHasher.PoseidonHash(new[] {
                eph_public_key_0,
                eph_public_key_1,
                new BigInteger(0),
                BigInteger.Parse("144441570523660387698699922682251371601")
            });

            BigInteger expected = BigInteger.Parse(
                "17193722240089020688281705003166560172091221092476445933947167090340422628583"
            );

            Assert.AreEqual(expected, bigNum, "OUTPUT: " + bigNum);
        }

        [Test]
        public void Pow5_Small()
        {
            BigInteger expected = BigInteger.Parse("100000");
            BigInteger output = Poseidon.Pow5(
                new BigInteger(10)
            );
            Assert.AreEqual(expected, output, "OUT: " + output);
        }


        [Test]
        public void Pow5_EdgeCase()
        {
            BigInteger expected = BigInteger.Parse(
                "11010837150789914634450318524941536756949983930089334391384279603582433620699"
            );
            BigInteger output = Poseidon.Pow5(
                new BigInteger(91593735651025872)
            );
            Assert.AreEqual(expected, output, "OUT: " + output);
        }

        [Test]
        public void Pow5_LargeBigInt()
        {
            BigInteger expected = BigInteger.Parse(
                "17559730436681389016774801990945372508707837448216484012798694610221969001056"
            );
            BigInteger output = Poseidon.Pow5(
                BigInteger.Parse("915937356510258724")
            );
            Assert.AreEqual(expected, output, "OUT: " + output);
        }

        [Test]
        public void Pow5_VeryLargeBigInt()
        {
            BigInteger expected = BigInteger.Parse(
                "15185921239258314663131978835340256258828200357426086918741255628450751304834"
            );
            BigInteger output = Poseidon.Pow5(
                BigInteger.Parse("202306697704578242338925299207034965818")
            );
            Assert.AreEqual(expected, output, "OUT: " + output);
        }

        [Test]
        public void Pow5_VeryLargeBigInt_2()
        {
            BigInteger expected = BigInteger.Parse(
                "5980946075033925263865594708745312481464042940133774844061775902450749620354"
            );
            BigInteger output = Poseidon.Pow5(
                BigInteger.Parse("144441570523660387698699922682251371601")
            );
            Assert.AreEqual(expected, output, "OUT: " + output);
        }

        [Test]
        public void PoseidonHash1Test()
        {
            string[] inputs = { "0x5b1f0533dd" };
            string input = "05b1f0533dd"; //TODO: Look into why need to remove `x` from the hex string in C#.

            BigInteger bigIntInput = BigInteger.Parse(input, System.Globalization.NumberStyles.HexNumber);
            BigInteger[] bigIntArrInput = { bigIntInput };

            string expectedOutput = "9318308185879295164714774571585163653334223568644020086992052983287926868898";
            BigInteger expectedOuputBigInt = BigInteger.Parse(expectedOutput);

            BigInteger actual = Poseidon1.Hash(bigIntArrInput);
            Assert.AreEqual(expectedOuputBigInt, actual, "ACTUAL: " + actual.ToString());
        }

        [Test]
        public void PoseidonHash2Test()
        {
            BigInteger[] bigIntArrInput = {
                BigInteger.Parse("0568570a0c4",     System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0fa742d6517434f", System.Globalization.NumberStyles.HexNumber)
            };

            BigInteger expectedOuputBigInt = BigInteger.Parse("12886590615193023928990309918723719693793770204591928268188166210615666393999");

            BigInteger actual = Poseidon2.Hash(bigIntArrInput);
            Assert.AreEqual(expectedOuputBigInt, actual, "ACTUAL: " + actual.ToString());
        }

        [Test]
        public void PoseidonHash3Test()
        {
            BigInteger[] bigIntArrInput = {
                BigInteger.Parse("0d96def96651128", System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("01f7d",           System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0bf557e2e",       System.Globalization.NumberStyles.HexNumber)
            };

            BigInteger expectedOuputBigInt = BigInteger.Parse("7722661377568892714328861529862730512085148820504565133363463965621597421200");

            BigInteger actual = Poseidon3.Hash(bigIntArrInput);
            Assert.AreEqual(expectedOuputBigInt, actual, "ACTUAL: " + actual.ToString());
        }

        [Test]
        public void PoseidonHash4Test()
        {
            BigInteger[] bigIntArrInput = {
                BigInteger.Parse("0cc6d20772509",       System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0c3e08d080def6ce2",   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("04622f595",           System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("072",                 System.Globalization.NumberStyles.HexNumber)
            };

            BigInteger expectedOuputBigInt = BigInteger.Parse("15102662994204263113152726795771945095413709771678878395298412054401617168492");

            BigInteger actual = Poseidon4.Hash(bigIntArrInput);
            Assert.AreEqual(expectedOuputBigInt, actual, "ACTUAL: " + actual.ToString());
        }

        [Test]
        public void PoseidonHash5Test()
        {
            BigInteger[] bigIntArrInput = {
                BigInteger.Parse("0119cc70bda751b46214b",   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0d5ebd4812a",             System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("06cdf678ca608fc",         System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0819643836601fc501172",   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0fb9507a26a",             System.Globalization.NumberStyles.HexNumber)
            };

            BigInteger expectedOuputBigInt = BigInteger.Parse("475355438621302339608817371644128544331343530654767674429160180520836681578");

            BigInteger actual = Poseidon5.Hash(bigIntArrInput);
            Assert.AreEqual(expectedOuputBigInt, actual, "ACTUAL: " + actual.ToString());
        }

        [Test]
        public void PoseidonHash6Test()
        {
            BigInteger[] bigIntArrInput = {
                BigInteger.Parse("0873e8fdae701b8",         System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0bb4bf78a3ecb46d436e7",   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0cf627275bfe27e1869",     System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("00ddd89",                 System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("079910b8008",             System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0fc04af87e25434",         System.Globalization.NumberStyles.HexNumber)
            };

            BigInteger expectedOuputBigInt = BigInteger.Parse("19515825097958319939544949050353165672719977680261864201249067521141962147653");

            BigInteger actual = Poseidon6.Hash(bigIntArrInput);
            Assert.AreEqual(expectedOuputBigInt, actual, "ACTUAL: " + actual.ToString());
        }

        [Test]
        public void PoseidonHash7Test()
        {
            BigInteger[] bigIntArrInput = {
                BigInteger.Parse("0e483",                   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("006080a36f1",             System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("06c50",                   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("06f3ffad52d91ffbaccde",   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("07073e0c4",               System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0c6569f88",               System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("00abf7cc128",             System.Globalization.NumberStyles.HexNumber)
            };

            BigInteger expectedOuputBigInt = BigInteger.Parse("18883844781657687536511490093240957793997696769902810748263588204588181956695");

            BigInteger actual = Poseidon7.Hash(bigIntArrInput);
            Assert.AreEqual(expectedOuputBigInt, actual, "ACTUAL: " + actual.ToString());
        }

        [Test]
        public void PoseidonHash8Test()
        {
            BigInteger[] bigIntArrInput = {
                BigInteger.Parse("067b4",                   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("02557de9b6b",             System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0d5d64a1c17f707328a",     System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0941b0a016bfe",           System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("092825b69",               System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("007cc",                   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("064",                     System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("099",                     System.Globalization.NumberStyles.HexNumber)
            };

            BigInteger expectedOuputBigInt = BigInteger.Parse("10143860969547922827253848297127318079710852163153966500688330829827496701236");

            BigInteger actual = Poseidon8.Hash(bigIntArrInput);
            Assert.AreEqual(expectedOuputBigInt, actual, "ACTUAL: " + actual.ToString());
        }

        [Test]
        public void PoseidonHash9Test()
        {
            BigInteger[] bigIntArrInput = {
                BigInteger.Parse("06d4bf06ab0663290faf0",   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0faf495b0",               System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("02d",                     System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("06b7a5359f50986c1c7d6",   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0e36e57",                 System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0d9a4b9d1536e6673",       System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0b405c6ffeb",             System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0b7",                     System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0b51d56",                 System.Globalization.NumberStyles.HexNumber)
            };

            BigInteger expectedOuputBigInt = BigInteger.Parse("16731762789090838755987643190727158951458237596073457316914485312399930282313");

            BigInteger actual = Poseidon9.Hash(bigIntArrInput);
            Assert.AreEqual(expectedOuputBigInt, actual, "ACTUAL: " + actual.ToString());
        }

        [Test]
        public void PoseidonHash10Test()
        {
            BigInteger[] bigIntArrInput = {
                BigInteger.Parse("01933601db0c9",           System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0684c01d5",               System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0d57d",                   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("054b101a7224d76cb566f",   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0a8900b013b19d6",         System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("09a7567f1cdd3778b8989",   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("02816e8667502",           System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0157bdc",                 System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("032",                     System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0be458870",               System.Globalization.NumberStyles.HexNumber)
            };

            BigInteger expectedOuputBigInt = BigInteger.Parse("15038161413431544737497761490498978898561410766140972342361706870934945395547");

            BigInteger actual = Poseidon10.Hash(bigIntArrInput);
            Assert.AreEqual(expectedOuputBigInt, actual, "ACTUAL: " + actual.ToString());
        }

        [Test]
        public void PoseidonHash11Test()
        {
            BigInteger[] bigIntArrInput = {
                BigInteger.Parse("0983b4516fdd69c8af0",    System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0a54106681887d19a15a6",  System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("00b",                    System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("023cef2c82775",          System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0e3c6760f",              System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("047707418",              System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0cefe627ddd02cddb",      System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("06295dc5339",            System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0fdb1de",                System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("08ed95cbbe1d9e1222b",    System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("05b4ed4bd1c",            System.Globalization.NumberStyles.HexNumber)
            };

            BigInteger expectedOuputBigInt = BigInteger.Parse("2463557499146816809570446691579708789958646750384079533177754937949237857066");

            BigInteger actual = Poseidon11.Hash(bigIntArrInput);
            Assert.AreEqual(expectedOuputBigInt, actual, "ACTUAL: " + actual.ToString());
        }

        [Test]
        public void PoseidonHash12Test()
        {
            BigInteger[] bigIntArrInput = {
                BigInteger.Parse("052347ab7eed1f93c15fa",   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0c4",                     System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("02adced3de306e34512",     System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0923af51b25a6fefd710a",   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0fd00af",                 System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("00ba2777692497a",         System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("06ad2fa34e9e1",           System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0de66d7d8b7e549d8",       System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0d2",                     System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0934b",                   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("07b1aa989740111c2f8",     System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0b95f02b2",               System.Globalization.NumberStyles.HexNumber)
            };

            BigInteger expectedOuputBigInt = BigInteger.Parse("2802846600172134279623635550209587765135563973649964657179667373619793718849");

            BigInteger actual = Poseidon12.Hash(bigIntArrInput);
            Assert.AreEqual(expectedOuputBigInt, actual, "ACTUAL: " + actual.ToString());
        }

        [Test]
        public void PoseidonHash13Test()
        {
            BigInteger[] bigIntArrInput = {
                BigInteger.Parse("0ce651f",                 System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("03e91e0a2",               System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("093f0",                   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("02f009c9c5a",             System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0eefd5b",                 System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("03f7cfdb213",             System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("04ecc751db2",             System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("006dc2b468e24460a4993",   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0b2af47d3242ca0",         System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("05048fa6661a293b4",       System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0f420",                   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("08aa3a22ce286cbc4ac",     System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("08fd16e107952fd6d7a",     System.Globalization.NumberStyles.HexNumber)
            };

            BigInteger expectedOuputBigInt = BigInteger.Parse("9614402968776910330953449582868143222184345026768856485958566523382463841666");

            BigInteger actual = Poseidon13.Hash(bigIntArrInput);
            Assert.AreEqual(expectedOuputBigInt, actual, "ACTUAL: " + actual.ToString());
        }

        [Test]
        public void PoseidonHash14Test()
        {
            BigInteger[] bigIntArrInput = {
                BigInteger.Parse("00c8b6097f2675f6d",  System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0e850",              System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("039fc8a",            System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0bc66e8076291f2",    System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("053",                System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("041d0a3e6d2ba03e7",  System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("08670",              System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("09bec6a3a3a",        System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("00e286562bfd59b",    System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("05dc01bfd",          System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("029ec5955",          System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("000",                System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("002094fe729ea3043",  System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0219eff8a67",        System.Globalization.NumberStyles.HexNumber)
            };

            BigInteger expectedOuputBigInt = BigInteger.Parse("18116471261214629133520309724723205026319825312404545343380600481412204991295");

            BigInteger actual = Poseidon14.Hash(bigIntArrInput);
            Assert.AreEqual(expectedOuputBigInt, actual, "ACTUAL: " + actual.ToString());
        }

        [Test]
        public void PoseidonHash15Test()
        {
            BigInteger[] bigIntArrInput = {
                BigInteger.Parse("0e3570f5ad2a1",          System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0162638a98d",            System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0863b5a2b54f8",          System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0ec8f8b7338",            System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("010ca4e9963",            System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0e71f",                  System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0d07196cd",              System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("06ae9",                  System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0f2",                    System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0bec3c275b0a12866bb",    System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0fee7cb613997ff43",      System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0fc",                    System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0eacfa455472a",          System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0153a53ef95fc",          System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("011ee673de80a83",        System.Globalization.NumberStyles.HexNumber)
            };

            BigInteger expectedOuputBigInt = BigInteger.Parse("21747296707329791688018054607984230574141820767309547014135175791233380215443");

            BigInteger actual = Poseidon15.Hash(bigIntArrInput);
            Assert.AreEqual(expectedOuputBigInt, actual, "ACTUAL: " + actual.ToString());
        }

        [Test]
        public void PoseidonHash16Test()
        {
            BigInteger[] bigIntArrInput = {
                BigInteger.Parse("0dc9ebdf1be32335505",     System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0010cac726c97e11ff9f9",   System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("063eead1f",               System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0aa53ffd5b1",             System.Globalization.NumberStyles.HexNumber),

                BigInteger.Parse("0f35069a25c",             System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("069a3e9",                 System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0ad8cfd203f91f6eae3",     System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("04129a8ff2dd1b3",         System.Globalization.NumberStyles.HexNumber),

                BigInteger.Parse("072",                     System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("00cb2bc69e1e7ed",         System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("00deadf",                 System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("061ddd0",                 System.Globalization.NumberStyles.HexNumber),

                BigInteger.Parse("0b8a2bf",                 System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("090c5ecdef4608ca30b",     System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0b95b1bd9acb3514905",     System.Globalization.NumberStyles.HexNumber),
                BigInteger.Parse("0d8c05c11d4fc9451",       System.Globalization.NumberStyles.HexNumber)
            };

            BigInteger expectedOuputBigInt = BigInteger.Parse("17659853214326367823052875610415715805694234135492096018138758656568336777027");

            BigInteger actual = Poseidon16.Hash(bigIntArrInput);
            Assert.AreEqual(expectedOuputBigInt, actual, "ACTUAL: " + actual.ToString());
        }
    }
}