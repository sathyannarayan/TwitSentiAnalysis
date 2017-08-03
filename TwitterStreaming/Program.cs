        using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SCP;
using Microsoft.SCP.Topology;
using TwitterSentimentAnalysis.Bolt;
using TwitterSentimentAnalysis.Spout;

namespace TwitterSentimentAnalysis
{
[Active(true)]
class Program : TopologyDescriptor
{
        public ITopologyBuilder GetTopologyBuilder()
    {
        TopologyBuilder topoBuilder = new TopologyBuilder("TwitterSentimentAnalysis" + DateTime.Now.ToString("yyyyMMddHHmmss"));
        topoBuilder.SetSpout("TwitterReaderSpout",TwitterReaderSpout.Get,
            new Dictionary<string, List<string>>(){
                {Constants.DEFAULT_STREAM_ID, TwitterReaderSpout.OutputSchemaName}},1, true);
        // create a bolt with tick frequence of 10 secs to emit tuple
        var boltConfig = new StormConfig();
        boltConfig.Set("topology.tick.tuple.freq.secs", "10");
            topoBuilder.SetBolt("TweetRankBolt",TweetRankBolt.Get,
            new Dictionary<string, List<string>>(){{"TWEETRANK_STREAM", TweetRankBolt.OutputSchemaName}}, 1, true)
            .shuffleGrouping("TwitterReaderSpout")
            .addConfigurations(boltConfig);

        topoBuilder.SetBolt(
            "AzureSqlBolt",
            AzureSqlBolt.Get,
            new Dictionary<string, List<string>>(),
            1).shuffleGrouping("TweetRankBolt", "TWEETRANK_STREAM");

        return topoBuilder;
    }
}
}

