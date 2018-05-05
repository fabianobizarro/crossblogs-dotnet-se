using crossblog.Domain;
using crossblog.Repositories;
using crossblog.tests.Controllers;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace crossblog.tests.Repositories
{
    public class CommentRepositoryTests
    {
        private Mock<CommentRepository> _commentRepositoryMock;
        private readonly CommentRepository _commentRepository;

        public CommentRepositoryTests()
        {
            _commentRepositoryMock = new Mock<CommentRepository>(new CrossBlogDbContext(CreateDbContextOptions()));
            _commentRepository = _commentRepositoryMock.Object;
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
            var commentList = new List<Comment>();

            var entityMock = Builder<Comment>.CreateNew().Build();

            _commentRepositoryMock.Setup(p => p.GetAsync(1)).Returns(Task.FromResult(entityMock));

            var entity = await _commentRepository.GetAsync(1);
            Assert.NotNull(entity);

            Assert.Equal(entityMock, entity);
        }

        [Fact]
        public async Task AddAsync()
        {
            var commentList = new List<Comment>();
            var comment = Builder<Comment>.CreateNew().Build();

            _commentRepositoryMock.Setup(p => p.InsertAsync(It.IsAny<Comment>()))
                .Returns(Task.CompletedTask)
                .Callback(() => commentList.Add(comment));

            _commentRepositoryMock.Setup(p => p.Query())
                .Returns(commentList.AsQueryable());

            int commentsCount = _commentRepository.Query().Count();

            Assert.Equal(0, commentsCount);

            await _commentRepository.InsertAsync(comment);

            _commentRepositoryMock.Verify(i => i.InsertAsync(comment));

            commentsCount = _commentRepository.Query().Count();

            Assert.Equal(1, commentsCount);

        }

        [Fact]
        public async Task DeleteAsync()
        {
            var comment = Builder<Comment>.CreateNew().Build();

            _commentRepositoryMock.Setup(p => p.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            await _commentRepository.DeleteAsync(1);

            _commentRepositoryMock.Verify(r => r.DeleteAsync(1));
        }

        [Fact]
        public async Task UpdateAsync()
        {
            var commentList = new List<Comment>();
            var comment = Builder<Comment>.CreateNew().Build();

            _commentRepositoryMock.Setup(p => p.GetAsync(1))
                .Returns(Task.FromResult(comment));

            var entity = await _commentRepository.GetAsync(1);

            entity.Title = "updated title";

            await _commentRepository.UpdateAsync(entity);

            var modifiedEntity = await _commentRepository.GetAsync(1);

            Assert.NotNull(modifiedEntity);

            Assert.Equal(entity.Title, modifiedEntity.Title);

        }

        [Fact]
        public async Task Query()
        {
            var commentList = Builder<Comment>.CreateListOfSize(3).Build().ToAsyncDbSetMock();

            _commentRepositoryMock.Setup(p => p.Query())
                .Returns(commentList.Object);

            var entity = await _commentRepository.Query()
                            .Where(p => p.Id == 1)
                            .FirstOrDefaultAsync();

            Assert.NotNull(entity);


        }


    }
}
