using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Doera.Application.DTOs.TodoItem.Requests;
using Doera.Application.Services;
using Doera.Core.Entities;
using Doera.Core.Enums;
using Doera.Core.Interfaces;
using Doera.Tests.TestSupport;
using Doera.Tests.TestSupport.Builders;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Doera.Tests.Services {
    public class TodoItemServiceTests : BaseServiceTest {
        private readonly Mock<IValidator<CreateTodoItemRequest>> _createValidator = new();
        private readonly ILogger<TodoItemService> _logger = Mock.Of<ILogger<TodoItemService>>();

        public TodoItemServiceTests() {
            // returning new ValidationResult is success cuz 0 ERRORS lmao
            _createValidator
                .Setup(v => v.ValidateAsync(It.IsAny<CreateTodoItemRequest>(), default))
                .ReturnsAsync(new ValidationResult());
        }

        private TodoItemService Sut() =>
            new(UnitOfWork.Object, Current.Object, _createValidator.Object, _logger);

        [Fact]
        public async Task CreateAsync_ValidationFails_ReturnsErrors() {
            var req = TodoItemBuilders.CreateReq().Generate();

            _createValidator
                .Setup(v => v.ValidateAsync(It.IsAny<CreateTodoItemRequest>(), default))
                .ReturnsAsync(new ValidationResult([new ValidationFailure("Title", "Required")]));

            var result = await Sut().CreateAsync(req);

            result.Succeeded.Should().BeFalse();
            Lists.Verify(l => l.FindByIdAsync(It.IsAny<Guid>()), Times.Never);
            Items.Verify(i => i.AddAsync(It.IsAny<TodoItem>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ListNotFound_ReturnsNotFound() {
            var req = TodoItemBuilders.CreateReq().Generate();

            Lists.Setup(l => l.FindByIdAsync(req.TodoListId)).ReturnsAsync((TodoList?)null);

            var result = await Sut().CreateAsync(req);

            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task CreateAsync_Success_AddsItem_AndReturnsId() {
            var req = TodoItemBuilders.CreateReq().Generate();
            var newId = Guid.NewGuid();

            Lists.Setup(l => l.FindByIdAsync(req.TodoListId))
                 .ReturnsAsync(new TodoList { Id = req.TodoListId, Name = "Random", UserId = UserId, TodoItems = [] });

            Items.Setup(r => r.GetCountForListAsync(req.TodoListId)).ReturnsAsync(3);
            Tags.Setup(r => r.ResolveTagsAsync(req.TagNames)).ReturnsAsync([]);

            Items.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
                 .Callback<TodoItem>(e => e.Id = newId)
                 .Returns(Task.CompletedTask);

            SetCompleteAsync(1);

            var result = await Sut().CreateAsync(req);

            result.Succeeded.Should().BeTrue();
            result.Value.Should().Be(newId);
        }
    }
}
