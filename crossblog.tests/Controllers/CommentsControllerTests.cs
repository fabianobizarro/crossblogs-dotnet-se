using crossblog.Controllers;
using crossblog.Domain;
using crossblog.Repositories;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using crossblog.tests.Controllers;
using crossblog.Model;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading;

namespace crossblog.tests.Controllers
{
    public class CommentsControllerTests
    {
        private CommentsController _commentsController;

        private Mock<IArticleRepository> _articleRepositoryMock = new Mock<IArticleRepository>();
        private Mock<ICommentRepository> _commentRepositoryMock = new Mock<ICommentRepository>();

        public CommentsControllerTests()
        {
            _commentsController = new CommentsController(_articleRepositoryMock.Object, _commentRepositoryMock.Object);
        }

        [Fact]
        public async Task Get_NotFound()
        {

            var result = await _commentsController.Get(1);

            Assert.NotNull(result);

            var notFound = result as NotFoundResult;

            Assert.NotNull(notFound);

        }

        [Fact]
        public async Task Get_ListItems()
        {

            var articleMock = Builder<Article>.CreateNew().Build();
            var commentsMock = Builder<Comment>.CreateListOfSize(10).Build().ToAsyncDbSetMock();

            _articleRepositoryMock.Setup(p => p.GetAsync(1)).Returns(Task.FromResult(articleMock));
            _commentRepositoryMock.Setup(p => p.Query()).Returns(commentsMock.Object);


            var result = await _commentsController.Get(1);

            Assert.NotNull(result);

            var okResult = result as OkObjectResult;

            Assert.NotNull(okResult);

            var content = okResult.Value as CommentListModel;
            Assert.NotNull(content);

            Assert.Equal(10, content.Comments.Count());

        }

        [Fact]
        public async Task Get_Comment_ArticleNotFound()
        {

            var result = await _commentsController.Get(1, 3);

            Assert.NotNull(result);

            var notFound = result as NotFoundResult;

            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task Get_Comment_CommentNotFound()
        {
            var articleMock = Builder<Article>.CreateNew().Build();
            var commentMock = Builder<Comment>.CreateListOfSize(1).Build().ToAsyncDbSetMock();

            _articleRepositoryMock.Setup(p => p.GetAsync(1)).Returns(Task.FromResult(articleMock));
            _commentRepositoryMock.Setup(p => p.Query()).Returns(commentMock.Object);

            var result = await _commentsController.Get(1, 3);

            Assert.NotNull(result);

            var notFound = result as NotFoundResult;

            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task Get_Comment_Ok()
        {
            var articleMock = Builder<Article>.CreateNew().Build();
            var commentMock = Builder<Comment>.CreateListOfSize(5).Build().ToAsyncDbSetMock();


            _articleRepositoryMock.Setup(p => p.GetAsync(1)).Returns(Task.FromResult(articleMock));
            _commentRepositoryMock.Setup(p => p.Query()).Returns(commentMock.Object);


            var result = await _commentsController.Get(1, 1);

            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);


            var model = objectResult.Value as CommentModel;

            Assert.NotNull(model);

            Assert.Equal(1, model.Id);


        }

        [Fact]
        public async Task Post_BadRequest()
        {
            _commentsController.ModelState.AddModelError("mock", "Mock error");

            var result = await _commentsController.Post(1, new CommentModel());

            Assert.NotNull(result);

            var badRequest = result as BadRequestObjectResult;
            Assert.NotNull(badRequest);

        }


        [Fact]
        public async Task Post_NotFound()
        {
            var commentMock = Builder<CommentModel>.CreateNew().Build();

            var result = await _commentsController.Post(1, commentMock);

            Assert.NotNull(result);

            var notFoundResult = result as NotFoundResult;

            Assert.NotNull(notFoundResult);
        }


        [Fact]
        public async Task Post_Created() {

            var commentMock = Builder<CommentModel>.CreateNew().Build();
            var articleMock = Builder<Article>.CreateNew().Build();

            _articleRepositoryMock.Setup(p => p.GetAsync(1)).Returns(Task.FromResult(articleMock));

            var result = await _commentsController.Post(1, commentMock);

            Assert.NotNull(result);

            var objectResult = result as CreatedResult;
            Assert.NotNull(objectResult);

            var commentCreated = objectResult.Value as CommentModel;

            Assert.NotNull(commentCreated);

            Assert.Equal(commentMock.Title, commentCreated.Title);



        }
    }
}
