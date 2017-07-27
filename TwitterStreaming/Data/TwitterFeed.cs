using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace TwitterSentimentAnalysis.Data
{
[Serializable]
public class TwitterFeed
{
    public string Text { get; set; }
    public long Id { get; set; }
    public int RetweetCount { get; set; }
    public int FavoriteCount { get; set; }
    public decimal Score
    {
        get
        {
            return (RetweetCount + FavoriteCount);
        }
    }

        public DateTime Createddate { get; private set; }

        public TwitterFeed()
    {
        // For searialization and deserialization
    }

    public TwitterFeed(ITweet tweet)
    {
        this.Text = tweet.FullText;
        this.Id = tweet.Id;
        this.RetweetCount = tweet.RetweetCount;
        this.FavoriteCount = tweet.FavoriteCount;
        this.Createddate = ParseTwitterDateTime(tweet.CreatedAt.ToString());
    }
        private DateTime ParseTwitterDateTime(string p)
        {
            if (p == null)
                return DateTime.Now;
            p = p.Replace("+0000 ", "");
            DateTimeOffset result;

            if (DateTimeOffset.TryParseExact(p, "ddd MMM dd HH:mm:ss yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-us").DateTimeFormat, System.Globalization.DateTimeStyles.AssumeUniversal, out result))
                return result.DateTime;
            else
                return DateTime.Now;
        }

        public override string ToString()
    {
        return $"{Id.ToString()}:{Text}:Retweet-{RetweetCount}:Likes-{FavoriteCount}:Score-{Score}:CreatedDate-{Createddate.ToString()}";
    }
}
}
