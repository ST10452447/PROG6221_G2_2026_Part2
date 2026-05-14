using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot_GUI
{
    // DELEGATE - Learning outcome requirement for Part 2
    public delegate string SentimentHandler(string sentiment);

    public class ChatbotEngine
    {
        // MEMORY - stores user information
        private string _userName = "";
        private string _favoriteTopic = "";
        private List<string> _conversationHistory = new List<string>();

        // CONVERSATION FLOW - tracks last topic for follow-ups
        private string _lastTopic = "";

        // 15+ RESPONSES PER TOPIC (75 total)
        private readonly Dictionary<string, List<string>> _responses;

        // SENTIMENT DETECTION keywords
        private readonly string[] _worriedWords = { "worried", "scared", "afraid", "nervous", "anxious", "concerned", "unsafe" };
        private readonly string[] _curiousWords = { "curious", "interesting", "tell me", "explain", "how", "why", "what", "learn" };
        private readonly string[] _frustratedWords = { "frustrated", "annoying", "confused", "doesn't work", "hate", "difficult" };

        // FOLLOW-UP triggers
        private readonly string[] _moreRequests = { "more", "another", "another tip", "tell me more", "explain more", "continue", "elaborate" };

        public ChatbotEngine()
        {
            _responses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["password"] = new List<string>
                {
                    "🔐 Use strong, unique passwords for every account. Never reuse passwords!",
                    "🛡️ A good password is at least 12 characters long - longer is better.",
                    "💡 Mix uppercase, lowercase, numbers, AND symbols for maximum strength.",
                    "⚠️ Avoid personal information like your name, birthday, or pet's name.",
                    "🔑 Consider a passphrase: 4-5 random words (e.g., 'Correct-Horse-Battery-Staple').",
                    "📱 Use a password manager like Bitwarden, LastPass, or 1Password.",
                    "🚫 Never write passwords on sticky notes or save them in unencrypted files.",
                    "🔄 Change passwords immediately if a service announces a data breach.",
                    "📧 Be wary of password reset emails - go directly to the website instead.",
                    "🔒 Two-factor authentication (2FA) adds another layer of security.",
                    "🎯 Avoid '123456', 'password', or 'qwerty' - they're hacked within seconds.",
                    "👨‍💻 Use different passwords for work, banking, social media, and shopping.",
                    "📱 Don't save passwords on shared or public computers.",
                    "🕵️ Check if your passwords are compromised at 'Have I Been Pwned'.",
                    "🔐 Biometric authentication (fingerprint, face ID) is best with strong passwords."
                },
                ["phishing"] = new List<string>
                {
                    "🎣 Phishing emails create urgency - 'Your account will be closed!' - verify first.",
                    "⚠️ Hover over links BEFORE clicking to see the real URL.",
                    "📧 Check sender's email address carefully - scammers use similar-looking addresses.",
                    "🔍 Look for spelling and grammar mistakes - legitimate companies rarely have errors.",
                    "📞 If an email asks you to call a number, verify it from the official website.",
                    "💸 Never send money, gift cards, or crypto in response to an email.",
                    "🏦 Banks will NEVER ask for your full password or PIN via email.",
                    "📎 Don't open attachments from unknown senders - they may contain malware.",
                    "🔗 Check for 'https://' and a padlock before entering login details.",
                    "📱 SMS phishing ('smishing') is common - treat texts with suspicion.",
                    "🎭 Scammers impersonate Amazon, PayPal, Netflix - go directly to the app.",
                    "📧 Forward suspicious emails to the company's fraud department.",
                    "🛡️ Use email filters and anti-phishing features in Gmail or Outlook.",
                    "🔐 Enable 2FA on your email account for extra protection.",
                    "👨‍👩‍👧 Discuss phishing with family - seniors and kids are often targeted."
                },
                ["safe browsing"] = new List<string>
                {
                    "🌐 Look for 'https://' and a padlock before entering personal info.",
                    "🛡️ Keep browser, extensions, and OS updated - updates fix vulnerabilities.",
                    "🔒 Use uBlock Origin ad-blocker - malicious ads are common attack vectors.",
                    "📱 Avoid public Wi-Fi for banking - use a VPN if you must.",
                    "🕵️ Clear browser cookies regularly to remove tracking data.",
                    "🔍 Check browser permissions - does that site need your location?",
                    "🚫 Never download software from pop-up ads - use official sources.",
                    "🔗 Use a link expander for shortened links (bit.ly, tinyurl).",
                    "📧 Be careful with 'browser lockers' - fake infection pop-ups.",
                    "🔐 Use separate browsers for banking vs general browsing.",
                    "🧹 Review and remove unnecessary browser extensions regularly.",
                    "🛡️ Enable 'Do Not Track' and enhanced tracking protection.",
                    "🔒 Use private/incognito mode on shared computers.",
                    "📱 On mobile, be cautious of apps requesting unnecessary permissions.",
                    "🎓 Learn to recognize 'typosquatting' - fake sites like 'faceb00k.com'."
                },
                ["privacy"] = new List<string>
                {
                    "👤 Review social media privacy settings - limit who sees your posts.",
                    "🔒 Use end-to-end encrypted messaging like Signal or WhatsApp.",
                    "🕵️ Be careful what you share online - it's nearly impossible to remove.",
                    "📧 Use disposable email addresses for newsletters and untrusted services.",
                    "🔐 Cover your webcam when not in use - simple but effective.",
                    "📱 Check app permissions regularly - revoke unnecessary access.",
                    "🔍 Search for yourself on Google to see what's publicly available.",
                    "🛡️ Use a VPN to hide your IP address and encrypt traffic.",
                    "📞 Opt out of data broker sites like Whitepages or Spokeo.",
                    "🔑 Don't use real birthday, address, or phone number unless necessary.",
                    "📧 Use 'Sign in with Apple' or 'hide my email' features when available.",
                    "🔒 Enable 2FA on all accounts - email, banking, social media.",
                    "🕵️ Use private browsing for searches - prevents local storage.",
                    "📱 On phones, review 'Privacy Dashboard' for app access history.",
                    "🎓 Understand that 'free' services often sell your data - consider paying."
                },
                ["scam"] = new List<string>
                {
                    "🚨 If it sounds too good to be true, it probably is - lottery wins, free money.",
                    "📞 Hang up on robocalls offering to lower credit cards or fix your computer.",
                    "💰 Never send money, gift cards, or crypto to someone you've only met online.",
                    "🏠 Be wary of rental scams - see property in person before paying deposit.",
                    "💼 Job interview scams ask for money for training - legitimate jobs pay YOU.",
                    "📦 Package delivery scams - check tracking directly on the carrier's site.",
                    "👮 Fake police, IRS, or tech support - real agencies never demand gift cards.",
                    "💸 'Accidental overpayment' scams - fake check, wire back, then check bounces.",
                    "📧 CEO fraud - scammers impersonate your boss asking for urgent wire transfers.",
                    "🏥 Medicare scams target seniors - never give Medicare number to unsolicited callers.",
                    "🎟️ Ticket scams sell fake tickets - use reputable resale sites with buyer protection.",
                    "📱 'SIM swapping' - use authenticator app instead of SMS 2FA.",
                    "🖥️ Tech support pop-ups - legitimate antivirus never uses scary full-screen alerts.",
                    "💵 Fake charity scams after disasters - donate directly through official websites.",
                    "📞 Hang up and call back using a trusted number from the company's website."
                }
            };
        }

        public string GetResponse(string userInput)
        {
            userInput = userInput.Trim();

            // Save conversation history
            _conversationHistory.Add(userInput);

            // SENTIMENT DETECTION using delegate
            SentimentHandler sentimentHandler = GetSentimentResponse;
            string sentiment = DetectSentiment(userInput);
            string sentimentPrefix = sentimentHandler(sentiment);

            // MEMORY: Get user name if not set
            if (string.IsNullOrEmpty(_userName) && !userInput.StartsWith("my name is", StringComparison.OrdinalIgnoreCase))
            {
                if (userInput.Length < 30 && !userInput.Contains("?") && !userInput.Contains("password") && !userInput.Contains("phishing"))
                {
                    _userName = userInput;
                    return $"Nice to meet you, {_userName}! I'll remember that. I have over 75 cybersecurity tips! Ask me about passwords, phishing, safe browsing, privacy, or scams.";
                }
                else
                {
                    return "Hello! What's your name? (You can say 'My name is John')";
                }
            }

            // Explicit name setting
            if (userInput.StartsWith("my name is", StringComparison.OrdinalIgnoreCase))
            {
                _userName = userInput.Substring(11).Trim();
                return $"Got it, {_userName}! I'll remember your name. Ask me about any cybersecurity topic - I have 15+ tips for each!";
            }

            // CONVERSATION FLOW: Handle follow-up requests
            if (IsFollowUpRequest(userInput) && !string.IsNullOrEmpty(_lastTopic))
            {
                if (_responses.ContainsKey(_lastTopic))
                {
                    string extraTip = GetRandomResponse(_lastTopic);
                    return $"{sentimentPrefix} Here's another tip about {_lastTopic}: {extraTip}";
                }
            }

            // KEYWORD RECOGNITION
            string matchedTopic = null;
            foreach (var key in _responses.Keys)
            {
                if (userInput.Contains(key, StringComparison.OrdinalIgnoreCase))
                {
                    matchedTopic = key;
                    break;
                }
            }

            if (matchedTopic != null)
            {
                _lastTopic = matchedTopic;
                string response = GetRandomResponse(matchedTopic);

                // MEMORY: Store favorite topic if user expresses interest
                if (userInput.Contains("interested in", StringComparison.OrdinalIgnoreCase) ||
                    userInput.Contains("love", StringComparison.OrdinalIgnoreCase))
                {
                    _favoriteTopic = matchedTopic;
                    response += $" I've noted that you're interested in {matchedTopic}. Just say 'tell me more' for another tip!";
                }

                return $"{sentimentPrefix} {response}";
            }

            // DEFAULT RESPONSE for unknown input (ERROR HANDLING)
            return $"{sentimentPrefix} I'm not sure I understand. You can ask me about passwords, phishing, safe browsing, privacy, or scams - I have 15+ tips for each! Try saying 'Tell me about passwords' or 'Give me a phishing tip'.";
        }

        private string GetRandomResponse(string topic)
        {
            var list = _responses[topic];
            Random rnd = new Random();
            return list[rnd.Next(list.Count)];
        }

        private string DetectSentiment(string input)
        {
            if (_worriedWords.Any(w => input.Contains(w, StringComparison.OrdinalIgnoreCase)))
                return "worried";
            if (_frustratedWords.Any(f => input.Contains(f, StringComparison.OrdinalIgnoreCase)))
                return "frustrated";
            if (_curiousWords.Any(c => input.Contains(c, StringComparison.OrdinalIgnoreCase)))
                return "curious";
            return "neutral";
        }

        // DELEGATE target method
        private string GetSentimentResponse(string sentiment)
        {
            switch (sentiment)
            {
                case "worried":
                    return "😟 I understand your concern. It's normal to feel worried about online safety.";
                case "frustrated":
                    return "😤 I hear you - cybersecurity can be frustrating. Let me help clearly.";
                case "curious":
                    return "🤔 Great question! It's smart to be curious about staying safe online.";
                default:
                    return "";
            }
        }

        private bool IsFollowUpRequest(string input)
        {
            return _moreRequests.Any(req => input.Contains(req, StringComparison.OrdinalIgnoreCase));
        }
    }
}