using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.SCP;
using Microsoft.SCP.Rpc.Generated;
using Tweetinvi.Models;
using TwitterSentimentAnalysis.Data;
using TwitterSentimentAnalysis.Spout;

namespace TwitterSentimentAnalysis.Bolt
{
public class TweetRankBolt : ISCPBolt
{
    bool enableAck = false;
    private Context context;
    List<TwitterFeed> tweetCache = new List<TwitterFeed>();

    public static List<Type> OutputSchema = new List<Type>() { typeof(TwitterFeed) };
    public static List<string> OutputSchemaName = new List<string>() { "TwitterFeed" };

    public TweetRankBolt(Context ctx, Dictionary<string, Object> parms)
    {
        this.context = ctx;
        Dictionary<string, List<Type>> inSchema = new Dictionary<string, List<Type>>();
        inSchema.Add(Constants.DEFAULT_STREAM_ID, TwitterReaderSpout.OutputSchema);        
        inSchema.Add(Constants.SYSTEM_TICK_STREAM_ID, new List<Type> { typeof(long) });
        Dictionary<string, List<Type>> outSchema = new Dictionary<string, List<Type>>();
        outSchema.Add("TWEETRANK_STREAM", OutputSchema);
        this.context.DeclareComponentSchema(new ComponentStreamSchema(inSchema, outSchema));
        if (Context.Config.pluginConf.ContainsKey(Constants.NONTRANSACTIONAL_ENABLE_ACK))
        {
            enableAck = (bool)(Context.Config.pluginConf
                [Constants.NONTRANSACTIONAL_ENABLE_ACK]);
        }
        enableAck = true;
    }

    public static TweetRankBolt Get(Context ctx, Dictionary<string, Object> parms)
    {
        return new TweetRankBolt(ctx, parms);
    }

    int totalAck = 0;
    public void Execute(SCPTuple tuple)
    {
        var isTickTuple = tuple.GetSourceStreamId().Equals(Constants.SYSTEM_TICK_STREAM_ID);
        if (isTickTuple)
        {
            // Get top 10 higest forwards + retweets count from last time window of 5 seconds
            Context.Logger.Debug($"Total tweets in window: {tweetCache.Count}");
            var topNTweets = tweetCache.OrderByDescending(o => o.Score).Take(Math.Min(10, tweetCache.Count)).ToList();

            foreach (var tweet in topNTweets)
            {
               this.context.Emit("TWEETRANK_STREAM", new Values(tweet));
            }

            
            tweetCache.Clear();
        }
        else
        {
            try
            {
               TwitterFeed tweet = tuple.GetValue(0) as TwitterFeed;
                if (!tweetCache.Any(o => o.Id.Equals(tweet.Id)))
                    tweetCache.Add(tweet);
                Context.Logger.Info(tweet.ToString());
                if (enableAck)
                {
                    this.context.Ack(tuple);
                    Context.Logger.Info("Total Ack: " + ++totalAck);
                }
            }
            catch (Exception ex)
            {
                Context.Logger.Error("An error occured while executing Tuple Id: {0}. Exception Details:\r\n{1}",
                    tuple.GetTupleId(), ex.ToString());
                if (enableAck)
                {
                    this.context.Fail(tuple);
                }
            }
        }
    }
}
}