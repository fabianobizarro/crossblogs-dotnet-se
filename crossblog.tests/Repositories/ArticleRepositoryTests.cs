using crossblog.Domain;
using crossblog.Repositories;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using crossblog.tests.Controllers;

namespace crossblog.tests.Repositories
{
    public class ArticleRepositoryTests
    {
        private Mock<ArticleRepository> _articleRepositoryMock = new Mock<ArticleRepository>();
        private readonly ArticleRepository _articleRepository;

        public ArticleRepositoryTests()
        {
            _articleRepositoryMock = new Mock<ArticleRepository>(new CrossBlogDbContext(CreateDbContextOptions()));
            _articleRepository = _articleRepositoryMock.Object;
        }

        private DbContextOptions<CrossBlogDbContext> CreateDbContextOptions()
        {
            var dbOptions = new DbContextOptionsBuilder<CrossBlogDbContext>();
            dbOptions.UseInMemoryDatabase(Guid.NewGuid().ToString());

            return dbOptions.Options;
        }

        [Fact]
        public async Task GetAsync()
        {
            var articleList = new List<Article>();

            var entityMock = Builder<Article>.CreateNew().Build();

            _articleRepositoryMock.Setup(p => p.GetAsync(1)).Returns(Task.FromResult(entityMock));

            var entity = await _articleRepository.GetAsync(1);
            Assert.NotNull(entity);

            Assert.Equal(entityMock, entity);
        }

        [Fact]
        public async Task AddAsync()
        {
            var articleList = new List<Article>();
            var article = Builder<Article>.CreateNew().Build();

            _articleRepositoryMock.Setup(p => p.InsertAsync(It.IsAny<Article>()))
                .Returns(Task.CompletedTask)
                .Callback(() => articleList.Add(article));

            _articleRepositoryMock.Setup(p => p.Query())
                .Returns(articleList.AsQueryable());

            int articlesCount = _articleRepository.Query().Count();

            Assert.Equal(0, articlesCount);

            await _articleRepository.InsertAsync(article);

            _articleRepositoryMock.Verify(i => i.InsertAsync(article));

            articlesCount = _articleRepository.Query().Count();

            Assert.Equal(1, articlesCount);

        }

        [Fact]
        public async Task DeleteAsync()
        {
            var article = Builder<Article>.CreateNew().Build();

            _articleRepositoryMock.Setup(p => p.DeleteAsync(1))
                .Returns(Task.CompletedTask);


            await _articleRepository.DeleteAsync(1);

            _articleRepositoryMock.Verify(r => r.DeleteAsync(1));
        }

        [Fact]
        public async Task UpdateAsync()
        {
            var articleList = new List<Article>();
            var article = Builder<Article>.CreateNew().Build();

            _articleRepositoryMock.Setup(p => p.GetAsync(1))
                .Returns(Task.FromResult(article));

            var entity = await _articleRepository.GetAsync(1);

            entity.Title = "updated title";

            await _articleRepository.UpdateAsync(entity);


            var modifiedEntity = await _articleRepository.GetAsync(1);

            Assert.NotNull(modifiedEntity);

            Assert.Equal(entity.Title, modifiedEntity.Title);

        }

        [Fact]
        public async Task Query()
        {
            var articleList = Builder<Article>.CreateListOfSize(3).Build().ToAsyncDbSetMock();

            _articleRepositoryMock.Setup(p => p.Query())
                .Returns(articleList.Object);

            var entity = await _articleRepository.Query()
                            .Where(p => p.Id == 1)
                            .FirstOrDefaultAsync();

            Assert.NotNull(entity);


        }


    }
}
