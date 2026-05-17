using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace EvaluaTeach
{
    public static class CommentClassifier
    {
        // ── Severe: slurs + extreme profanity (EN / PH) ───────────────────────
        private static readonly string[] SevereWords =
        {
            // English racial / identity slurs
            "nigger", "nigga", "chink", "spic", "kike", "wetback", "gook",
            "faggot", "fag", "dyke", "tranny", "retard", "cripple",
            // English sexual / violent
            "cunt", "motherfucker", "motherfucking", "cocksucker",
            "whore", "slut", "rape", "rapist",
            // English death / self-harm threats
            "kill yourself", "kys", "go die", "die bitch",
            "i hope you die", "i will kill",
            // English extreme combos
            "piece of shit", "fucking idiot", "fucking stupid",
            "fucking useless", "fucking worthless", "fucking hate",
            "i hate you", "dumb bitch", "stupid bitch", "ugly bitch",
            "ugly whore", "subhuman", "you are nothing",
            // Leet / obfuscated English (normalised by NormalizeLeet before matching)
            "fck", "fuk", "fvck", "phuck", "fucc",
            "sh1t", "sh!t", "sh!t", "s.h.i.t",
            "b1tch", "b!tch", "bytch",
            "a$$", "a55", "@ss",
            "c0ck", "d1ck", "d!ck",
            "n1gger", "n!gger",
            // Tagalog severe
            "putang ina", "putangina", "putang ina mo", "putangina mo",
            "puta ka", "pakyu", "pakyo", "puta",
            "leche ka", "letcheng", "hayop ka", "gago kang",
            "ulol na", "tangina mo", "tangina", "tang ina",
            "tarantado", "bobo kang", "tanga kang",
            "siraulo", "sinungaling na puta",
            "walang kwentang guro", "walang silbi",
            "patay ka", "mamatay ka na",
            // Cebuano / Bisaya severe
            "pisting yawa", "pisting yawa ka", "yawa ka",
            "anak sa puta", "anak ng puta",
            "buang ka", "buang gyud", "buang kaayo",
            "punyeta ka", "bogo ka", "amaw",
            "animal ka", "unggoy ka", "iro ka",
            "mamatay ka", "patay ka na",
            "mingaw kag ulo", "wa kay pulos",
            "pangit ka", "bastos kaayo",
            "bilat", "pisot", "otot mo"
        };

        // ── Moderate: clear profanity / aggressive (EN / PH) ─────────────────
        private static readonly string[] ModerateWords =
        {
            // English
            "fuck", "fucker", "fucking", "fucked",
            "shit", "shitty", "bullshit",
            "asshole", "bastard", "bitch", "bitchy",
            "damn", "goddamn", "crap", "piss", "pissed",
            "jackass", "dipshit", "dumbass", "jackoff",
            "screw you", "sucks ass",
            "terrible teacher", "worst teacher",
            "incompetent", "pathetic", "disgusting",
            "cock", "dick", "penis", "vagina",
            // Tagalog moderate
            "gago", "gaga", "ulol", "tanga", "bobo", "boba",
            "leche", "letch", "lintik", "bwisit", "bwiset",
            "punyeta", "putcha", "pota", "potah",
            "salot", "peste", "salbaheng",
            "mukha kang", "pangit mo",
            "walang kwenta", "walang silbi",
            "hunghang", "inutil",
            // Cebuano / Bisaya moderate
            "yawa", "buang", "bogo", "inutil",
            "sugarol", "bastos", "supak",
            "pastilan", "atay", "atay ka",
            "maldita", "maldito",
            "bongga", "bungol",
            "wa gyud ka", "di ka tinuod",
            "lintod", "bilatan"
        };

        // ── Mild: borderline / slight aggression (EN / PH) ───────────────────
        private static readonly string[] MildWords =
        {
            // English
            "stupid", "dumb", "idiot", "loser", "jerk", "sucks",
            "awful", "horrible", "useless", "lazy", "boring",
            "waste of time", "doesn't care", "never helps",
            "always late", "doesn't teach", "bad teacher",
            "unfair", "unprofessional", "rude", "annoying", "irritating",
            "clueless", "not helpful", "terrible",
            // Tagalog mild
            "hilo", "tonto", "wala kang kwenta",
            "sobrang dumb", "sobrang bobo",
            "tamad", "tamad na guro",
            "makulit", "walang alam", "walang magawa",
            "hindi marunong", "hindi maayos",
            "pabaya", "pabayaan",
            "di marunong magturo", "di magaling",
            // Cebuano / Bisaya mild
            "tapulan", "tapulan kaayo",
            "dili maayo", "dili marunong",
            "wa nagtutudlo", "wa nahibalo",
            "dugay kaayo", "dugay mag-explain",
            "kanunay absent", "di mo klaro",
            "dili klaro ang leksyon"
        };

        public static CommentLevel Classify(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return CommentLevel.Normal;

            string lower      = text.ToLowerInvariant();
            string deSpaced   = Regex.Replace(lower, @"\s+", " ").Trim();
            string normalized = NormalizeLeet(deSpaced);

            if (ContainsAny(normalized, SevereWords) || ContainsAny(deSpaced, SevereWords))
                return CommentLevel.Severe;

            if (ContainsAny(normalized, ModerateWords) || ContainsAny(deSpaced, ModerateWords))
                return CommentLevel.Moderate;

            if (ContainsAny(normalized, MildWords) || ContainsAny(deSpaced, MildWords))
                return CommentLevel.Mild;

            return CommentLevel.Normal;
        }

        private static string NormalizeLeet(string text)
        {
            return text
                .Replace("0", "o")
                .Replace("1", "i")
                .Replace("3", "e")
                .Replace("4", "a")
                .Replace("5", "s")
                .Replace("7", "t")
                .Replace("8", "b")
                .Replace("@", "a")
                .Replace("$", "s")
                .Replace("!", "i")
                .Replace("+", "t")
                .Replace("(", "c")
                .Replace("ph", "f")
                .Replace("ck", "ck");
        }

        private static bool ContainsAny(string text, string[] words)
        {
            foreach (var word in words)
            {
                if (text.Contains(word, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public static string GetLevelDescription(CommentLevel level) => level switch
        {
            CommentLevel.Normal   => "Normal — no inappropriate content detected.",
            CommentLevel.Mild     => "Mild — slight aggression or borderline language detected.",
            CommentLevel.Moderate => "Moderate — profanity or notably aggressive language detected.",
            CommentLevel.Severe   => "Severe — extreme profanity or slurs detected. Student should be contacted.",
            _ => string.Empty
        };
    }
}
