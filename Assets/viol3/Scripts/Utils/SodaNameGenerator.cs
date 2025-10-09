using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class SodaNameGenerator
{
    static string[] _sodaAdjectives = new string[] 
    {
        "Zesty","Fizz","Frost","Chill","Cool","Minty","Pop","Lush","Glow","Spark",
        "Lime","Neo","Fresh","Bubly","Crisp","Breez","Nova","Snap","Rush","Pure",
        "Lumo","Aero","Cloud","Zen","Tonic","Flash","Jolt","Pep","Quik","Zing",
        "Hype","Vibe","Frosty","Chilly","Sweer","Juicy","Soda","Gassy","Sparkz","Snaz",
        "Hyper","AeroX","Breezy","Fizzy","Sodie","Frot","Jucy","Zesti","Poppy","Minti",
        "Bubbly","Icy","Wavy","Lumen","Vapor","Prizm","Nitro","Glowz","Lix","Freshy",
        "Chillz","Zup","Zyp","Cooly","Soft","Bubz","Peppy","Vibra","Frozy","Zuno",
        "Zira","SodaX","Lumy","Popz","Zipz","Fiza","Noxy","Zeon","Froz","Fliq",
        "Joly","Juci","Mox","Pura","Clix","SodaY","Vex","Frizz","Fizzi","Roxy",
        "Lima","Zora","Popa","Zina","Bree","Wavi","SodaQ","Fizo","NovaX","Jupi"
    };

    static string[] _sodaNouns = new string[] 
    {
        "Pop","Wave","Rush","Drop","Can","Sip","Cup","Bomb","Shot","Flow",
        "Drip","Pulse","Zing","Fizz","Bite","Glow","Beam","Core","Byte","Jet",
        "Storm","Dash","Cloud","Burst","Tune","Buzz","Ring","Snap","Flash","Tank",
        "Fuel","Orb","Zap","GlowX","PopX","Cube","Pod","Puff","Stream","RushX",
        "FizzX","CanX","Soda","Bolt","Mint","Nova","DropX","Loop","Beat","Tube",
        "Bit","Blip","Spot","DashX","Mode","Chip","Phase","TuneX","RingX","TankX",
        "Zorb","ByteX","BoltX","Tone","Mix","SipX","RushQ","Bling","Gem","Kick",
        "Zone","PopQ","Bubb","Flux","WaveX","BeamX","Popz","FlowX","DropQ","Juice",
        "SipQ","BuzzX","Cubz","Riff","TuneQ","TubeX","PuffX","RingQ","Glint","BeamQ",
        "FizzQ","SodaQ","GlowQ","MintX","ByteQ","DripQ","DashQ","BitX","PopR","BlipQ"
    };

    public static string GenerateNickname(string walletAddress)
    {
        if (walletAddress.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            walletAddress = walletAddress.Substring(2);

        using (var sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(walletAddress));
            int prefixIndex = hash[0] % _sodaAdjectives.Length;
            int suffixIndex = hash[1] % _sodaNouns.Length;

            string nickname = _sodaAdjectives[prefixIndex] + _sodaNouns[suffixIndex];

            return nickname;
        }
    }
}
