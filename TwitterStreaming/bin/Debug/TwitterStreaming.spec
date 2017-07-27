{:name "TwitterSentimentAnalysis20170726162527"
 :topology
   (topology

     {
       "TwitterReaderSpout"
       (spout-spec 
         (scp-spout
           {
             "plugin.name" "SCPHost.exe"
             "plugin.args" ["TwitterStreaming.dll" "TwitterSentimentAnalysis.Spout.TwitterReaderSpout" "Get"]
             "output.schema" {"default" ["TwitterFeed"]}
             "nontransactional.ack.enabled" true
           })
         :p 1)
     }

     {
       "TweetRankBolt"
       (bolt-spec 
         {
           ["TwitterReaderSpout" "default"] :shuffle
         }
         (scp-bolt
           {
             "plugin.name" "SCPHost.exe"
             "plugin.args" ["TwitterStreaming.dll" "TwitterSentimentAnalysis.Bolt.TweetRankBolt" "Get"]
             "output.schema" {"TWEETRANK_STREAM" ["TwitterFeed"]}
             "nontransactional.ack.enabled" true
           })
         :p 1
         :conf {"topology.tick.tuple.freq.secs" 5})

       "AzureSqlBolt"
       (bolt-spec 
         {
           ["TweetRankBolt" "TWEETRANK_STREAM"] :shuffle
         }
         (scp-bolt
           {
             "plugin.name" "SCPHost.exe"
             "plugin.args" ["TwitterStreaming.dll" "TwitterSentimentAnalysis.Bolt.AzureSqlBolt" "Get"]
             "output.schema" {}
           })
         :p 1)
     }

   )

 :config {}

}
