using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Doera.Application.Abstractions.Results;
using Doera.Application.DTOs.TodoItem;
using Doera.Application.DTOs.TodoItem.Requests;
using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Services;
using Doera.Core.Enums;
using Doera.Tests.TestSupport;
using Doera.Tests.TestSupport.Builders;
using Doera.Web.Features.TodoItem;
using Doera.Web.Features.TodoItem.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NToastNotify;

namespace Doera.Tests.MVCControllers {
    public class TodoItemControllerTests : BaseTest {
        private readonly Mock<ITodoItemService> _itemService = new();
        private readonly Mock<IQueryDispatcher> _dispatcher = new();

        private readonly IToastNotification _toast = Mock.Of<IToastNotification>();
        private readonly ILogger<TodoItemController> _logger = Mock.Of<ILogger<TodoItemController>>();

        private readonly Faker<CreateTodoItemVM> _createVmFaker = TodoItemBuilders.CreateVm();
        private readonly Faker<EditTodoItemVM> _editVmFaker = TodoItemBuilders.EditVm();
        private readonly Faker<TodoItemDto> _dtoFaker = TodoItemBuilders.ItemDto();

        private TodoItemController Sut() => new(_itemService.Object, _dispatcher.Object, _toast, _logger);

        [Fact]
        public void Create_Get_ListIdProvided_ReturnsViewWithPresetListId() {
            // Arrange
            var listId = Guid.NewGuid();
            var sut = Sut();

            // Act
            var result = sut.Create(listId);

            // Assert
            var view = result.Should().BeOfType<ViewResult>().Subject;
            view.Model.Should().BeOfType<CreateTodoItemVM>()
                .Which.TodoListId.Should().Be(listId);
        }

        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsCreateViewWithSameModel() {
            // Arrange
            var sut = Sut();
            var vm = _createVmFaker.Generate();
            sut.ModelState.AddModelError("Title", "Required");

            // Act
            var result = await sut.Create(vm);

            // Assert
            var view = result.Should().BeOfType<ViewResult>().Subject;
            view.ViewName.Should().Be(nameof(TodoItemController.Create));
            view.Model.Should().BeEquivalentTo(vm);
            sut.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToList() {
            var sut = Sut();
            var vm = _createVmFaker.Generate();
            var newId = Guid.NewGuid();

            _itemService.Setup(s => s.CreateAsync(It.IsAny<CreateTodoItemRequest>()))
                    .ReturnsAsync(newId);

            var result = await sut.Create(vm);

            _itemService.Verify(s => s.CreateAsync(It.Is<CreateTodoItemRequest>(r =>
                r.TodoListId == vm.TodoListId &&
                r.Title == vm.Title &&
                r.Description == vm.Description
            )), Times.Once);

            var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirect.ActionName.Should().Be("Index");
            redirect.ControllerName.Should().Be("TodoList");
            redirect.RouteValues!["id"].Should().Be(vm.TodoListId);
        }

        [Fact]
        public async Task Create_Post_ServiceFailure_ReturnsCreateViewWithErrors() {
            var sut = Sut();
            var vm = _createVmFaker.Generate();

            _itemService.Setup(s => s.CreateAsync(It.IsAny<CreateTodoItemRequest>()))
                    .ReturnsAsync(Errors.Common.Failed("ServiceFailure"));

            var result = await sut.Create(vm);

            var view = result.Should().BeOfType<ViewResult>().Subject;
            view.ViewName.Should().Be(nameof(TodoItemController.Create));
            view.ViewData.ModelState.IsValid.Should().BeFalse();
            view.Model.Should().BeEquivalentTo(vm);
        }

        [Fact]
        public async Task Edit_Get_ItemNotFound_ReturnsNotFound() {
            var sut = Sut();
            var id = Guid.NewGuid();

            _dispatcher.Setup(d => d.DispatchAsync<GetTodoItemByIdRequest, TodoItemDto>(
                    It.IsAny<GetTodoItemByIdRequest>(), default))
                .ReturnsAsync(Errors.TodoItem.NotFound());

            var result = await sut.Edit(id, CancellationToken.None);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_Get_ItemFound_ReturnsEditViewWithViewModel() {
            var sut = Sut();
            var dto = _dtoFaker.Generate();

            _dispatcher
                .Setup(d => d.DispatchAsync<GetTodoItemByIdRequest, TodoItemDto>(
                    It.IsAny<GetTodoItemByIdRequest>(), default))
                .ReturnsAsync(dto);

            var result = await sut.Edit(dto.Id, CancellationToken.None);

            _dispatcher.Verify(d => d.DispatchAsync<GetTodoItemByIdRequest, TodoItemDto>(
                It.Is<GetTodoItemByIdRequest>(r => r.Id == dto.Id),
                default
            ), Times.Once);

            var view = result.Should().BeOfType<ViewResult>().Subject;
            var model = view.Model.Should().BeOfType<EditTodoItemVM>().Subject;

            model.Id.Should().Be(dto.Id);
            model.Title.Should().Be(dto.Title);
            model.Description.Should().Be(dto.Description);
        }

        [Fact]
        public async Task Edit_Post_RouteIdMismatch_ReturnsBadRequest() {
            var sut = Sut();
            var vm = _editVmFaker.Generate();

            var result = await sut.Edit(Guid.NewGuid(), vm);

            _itemService.Verify(s => s.UpdateAsync(It.IsAny<UpdateTodoItemRequest>()), Times.Never);
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Edit_Post_InvalidModel_ReturnsEditView() {
            var sut = Sut();
            var vm = _editVmFaker.Generate();
            sut.ModelState.AddModelError("Title", "Required");

            var result = await sut.Edit(vm.Id, vm);

            _itemService.Verify(s => s.UpdateAsync(It.IsAny<UpdateTodoItemRequest>()), Times.Never);

            var view = result.Should().BeOfType<ViewResult>().Subject;
            view.ViewName.Should().Be(nameof(TodoItemController.Edit));
            view.ViewData.ModelState.IsValid.Should().BeFalse();
            view.Model.Should().Be(vm);
        }

        [Fact]
        public async Task Edit_Post_ServiceFailure_ReturnsEditViewWithErrors() {
            var sut = Sut();
            var vm = _editVmFaker.Generate();

            _itemService.Setup(s => s.UpdateAsync(It.IsAny<UpdateTodoItemRequest>()))
                    .ReturnsAsync(Errors.Common.Failed("ServiceFailure"));

            var result = await sut.Edit(vm.Id, vm);

            var view = result.Should().BeOfType<ViewResult>().Subject;
            view.ViewName.Should().Be(nameof(TodoItemController.Edit));
            view.ViewData.ModelState.IsValid.Should().BeFalse();
            view.Model.Should().Be(vm);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_MapsRequestAndCallsService() {
            var sut = Sut();
            var vm = _editVmFaker.Generate();
            _itemService.Setup(s => s.UpdateAsync(It.IsAny<UpdateTodoItemRequest>()))
                        .ReturnsAsync(Result.Success());

            var result = await sut.Edit(vm.Id, vm);

            _itemService.Verify(s => s.UpdateAsync(It.Is<UpdateTodoItemRequest>(r =>
                r.Id == vm.Id &&
                r.Title == vm.Title &&
                r.Description == vm.Description &&
                r.Status == vm.Status &&
                r.Priority == vm.Priority &&
                r.StartDate == vm.StartDate &&
                r.DueDate == vm.DueDate &&
                (r.TagNames ?? Array.Empty<string>()).SequenceEqual(SplitTags((vm.TagNames ?? string.Empty)))
            )), Times.Once);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_ServiceSuccess_RedirectsToList() {
            var sut = Sut();
            var vm = _editVmFaker.Generate();
            _itemService.Setup(s => s.UpdateAsync(It.IsAny<UpdateTodoItemRequest>()))
                        .ReturnsAsync(Result.Success());

            var result = await sut.Edit(vm.Id, vm);

            _itemService.Verify(s => s.UpdateAsync(It.IsAny<UpdateTodoItemRequest>()), Times.Once);

            var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirect.ControllerName.Should().Be("TodoList");
            redirect.ActionName.Should().Be("Index");
            redirect.RouteValues!["id"].Should().Be(vm.TodoListId);
        }

        [Fact]
        public async Task Delete_Post_ServiceSuccess_Redirects() {
            var sut = Sut();
            var listId = Guid.NewGuid();
            var itemId = Guid.NewGuid();

            _itemService.Setup(s => s.DeleteAsync(itemId))
                    .ReturnsAsync(Result.Success());

            var result = await sut.Delete(listId, itemId);

            _itemService.Verify(s => s.DeleteAsync(itemId), Times.Once);

            var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirect.ActionName.Should().Be("Index");
            redirect.ControllerName.Should().Be("TodoList");
            redirect.RouteValues!["id"].Should().Be(listId);

        }

        [Fact]
        public async Task Delete_Post_ServiceFailure_Redirects() {
            var sut = Sut();
            var listId = Guid.NewGuid();
            var itemId = Guid.NewGuid();

            _itemService.Setup(s => s.DeleteAsync(itemId))
                    .ReturnsAsync(Errors.Common.Failed("ServiceFailure"));

            var result = await sut.Delete(listId, itemId);

            _itemService.Verify(s => s.DeleteAsync(itemId), Times.Once);
            result.Should().BeOfType<RedirectToActionResult>();
        }
    }
}
