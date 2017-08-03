# TwitSentiAnalysis
Project is twitter sentiment analysis. The project is developed using Azure HDInsight storm cluster, Azure SQL Server and Visual Studio 2015. The project contains 1 spout and 2 bolts. Spout reads the twitter feeds and emits to a bolt. The first bolt takes a 10 second window and finds the top 10 tweets ordered by the Score(sum of number of retweets and favourite count) to emit tuple to second bolt. The second bolt will store those tweets with classfier column SentimentType using Sentiment140 in Azure SQL database. The Sentiment140 is used to classify the tweet data as positive, negative and neutral sentiment type.

## Building project

In order to build this project, use Microsoft Visual Studio 2015.
