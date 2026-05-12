using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace EvaluaTeach
{
    public static class CommentClassifier
    {
        // ── Severe: slurs + extreme profanity ──────────────────────────────────
        private static readonly string[] SevereWords =
        {
            "nigger", "nigga", "chink", "spic", "kike", "wetback", "gook",
            "faggot", "fag", "dyke", "tranny", "retard", "cripple",
            "cunt", "motherfucker", "motherfucking", "cock", "cocksucker",
            "whore", "slut", "rape", "rapist", "kill yourself", "kys",
            "go die", "die bitch", "piece of shit", "you piece",
            "fucking idiot", "fucking stupid", "fucking useless",
            "fucking worthless", "fucking hate", "i hate you", "hate you",
            "dumb bitch", "stupid bitch", "ugly bitch", "ugly whore",
            "imbecile", "moron", "subhuman"
        };

        // ── Moderate: clear profanity / aggressive ────────────────────────────
        private static readonly string[] ModerateWords =
        {
            "fuck", "fucker", "fucking", "fucked", "shit", "shitty",
            "bullshit", "asshole", "bastard", "bitch", "bitchy",
            "damn", "goddamn", "crap", "piss", "pissed",
            "jackass", "dipshit", "dumbass", "jackoff", "screw you",
            "sucks ass", "terrible teacher", "worst teacher",
            "incompetent", "pathetic", "disgusting"
        };

        // ── Mild: borderline / slight aggression ──────────────────────────────
        private static readonly string[] MildWords =
        {
            "stupid", "dumb", "idiot", "loser", "jerk", "sucks",
            "awful", "horrible", "useless", "lazy", "boring",
            "waste of time", "doesn't care", "never helps",
            "always late", "doesn't teach", "bad teacher",
            "unfair", "unprofessional", "rude", "annoying", "irritating",
            "clueless", "incompetent-ish", "not helpful"
        };

        /// <summary>
        /// Classifies a comment into one of four levels.
        /// Checks severe first; if matched, returns Severe regardless of context.
        /// </summary>
        public static CommentLevel Classify(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return CommentLevel.Normal;

            string normalized = Regex.Replace(text.ToLowerInvariant(), @"\s+", " ").Trim();

            if (ContainsAny(normalized, SevereWords))
                return CommentLevel.Severe;

            if (ContainsAny(normalized, ModerateWords))
                return CommentLevel.Moderate;

            if (ContainsAny(normalized, MildWords))
                return CommentLevel.Mild;

            return CommentLevel.Normal;
        }

        private static bool ContainsAny(string text, string[] words)
        {
            foreach (var word in words)
            {
                // Match whole-word or phrase occurrences
                string pattern = Regex.Escape(word);
                if (Regex.IsMatch(text, $@"(^|\s|[^a-z]){pattern}($|\s|[^a-z])", RegexOptions.IgnoreCase)
                    || text.Contains(word, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a human-readable description of each level.
        /// </summary>
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
