using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using Newtonsoft.Json;

namespace Elastic.Test
{
    [TestClass]
    public class ApiExploratoryTests
    {
        public const string TwitterIndexName = "tweetindex";
        public const string ReutersCorpusIndexName = "reuterscorpusindex";
        private IElasticClient _client;
        private IndexName _twitterIndex;
        private IndexName _reutersIndex;

        [TestInitialize]
        public void Setup()
        {
            var node = new Uri("http://localhost:9200");
            var settings = new ConnectionSettings(node);
            _client = new ElasticClient(settings);

            _twitterIndex = new IndexName();
            _twitterIndex.Name = TwitterIndexName;
            _reutersIndex = new IndexName();
            _reutersIndex.Name = ReutersCorpusIndexName;
            _client.CreateIndex(_twitterIndex);
            _client.CreateIndex(_reutersIndex);
        }

        [TestMethod]
        public void Should_Be_Able_To_Index()
        {
            Index().IsValid.Should().BeTrue();
        }

        private IResponse Index()
        {
            var tweet = new Tweet
            {
                Id = 1,
                User = "kimchy",
                PostDate = new DateTime(2009, 11, 15),
                Message = "Trying out NEST, so far so good?"
            };
            return _client.Index(tweet, idx => idx.Index(TwitterIndexName)); // can also use settings.DefaultIndex(IndexName);
        }

        [TestMethod]
        public void Should_Be_Able_To_Retrieve_Indexed_Document()
        {
            Index();

            var response = _client.Get<Tweet>(1, idx => idx.Index(TwitterIndexName));
            response.Source.Id.Should().Be(1); // the original document
        }

        [TestMethod]
        public void Should_Be_Able_To_Search_Indexed_Document_With_DSL_Api()
        {

            Index();
            Thread.Sleep(1000);
            var searchQuery = "kimchy";

            var response = _client.Search<Tweet>(s => s
                .Index(TwitterIndexName)
                .From(0)
                .Size(10)
                .Query(q =>
                    q.Term(t => t.User, searchQuery)
                    || q.Match(mq => mq.Field(f => f.User).Query("nest"))
                )
            );

            response.Documents.Count().Should().Be(1);
            response.Documents.First().User.Should().Be(searchQuery);
        }

        [TestMethod]
        public void Should_Be_Able_To_Search_Indexed_Document_With_ObjectInitializer_Api()
        {
            Index();
            Thread.Sleep(1000);
            var searchQuery = "kimchy";
            
            var request = new SearchRequest
            {
                
                From = 0,
                Size = 10,
                Query = new TermQuery { Field = "user", Value = searchQuery }
        || new MatchQuery { Field = "description", Query = "nest" }
            };

            var response = _client.Search<Tweet>(request);
            response.Documents.Count().Should().Be(1);
            response.Documents.First().User.Should().Be(searchQuery);

        }

        [TestMethod]
        public void Should_Be_Able_To_Deserialize_Corpus()
        {
            var articles = DeserializeCorpus();
            articles.Count.Should().Be(21578);
        }

        private static List<Article> DeserializeCorpus()
        {
            List<Article> articles = new List<Article>(21578);

            for (int i = 0; i < 22; i++)
            {
                var file = File.ReadAllText($@"..\..\Data\reuters-{i.ToString().PadLeft(3, '0')}.json");
                var objects = JsonConvert.DeserializeObject<List<Article>>(file);
                articles.AddRange(objects);
            }
            return articles;
        }

        [TestMethod]
        public void Should_Be_Able_To_Index_Corpus_Really_Really_Fast()
        {
            var articles = DeserializeCorpus();
            var bulkRequest = new BulkRequest(_reutersIndex);

            var operations = articles.Select(article => new BulkIndexOperation<Article>(article) {Id = article.Id, Index = _reutersIndex}).Cast<IBulkOperation>().ToList();

            bulkRequest.Operations = operations;
            var response = _client.Bulk(bulkRequest);
            Thread.Sleep(1000);
            var document = _client.Get<Article>(1, idx => idx.Index(ReutersCorpusIndexName));
            document.Should().NotBeNull();
            document.Id.Should().Be("1");
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.DeleteIndex(TwitterIndexName);
            //_client.DeleteIndex(ReutersCorpusIndexName);
        }
    }

    public class Tweet
    {
        public int Id { get; set; }
        public string User { get; set; }
        public DateTime PostDate { get; set; }
        public string Message { get; set; }
    }
}