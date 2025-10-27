using Bogus;
using Doera.Application.DTOs.TodoItem;
using Doera.Application.DTOs.TodoItem.Requests;
using Doera.Core.Entities;
using Doera.Core.Enums;
using Doera.Web.Features.TodoItem.ViewModels;
using System;

namespace Doera.Tests.TestSupport.Builders {
    public static class TodoItemBuilders {
        // Controller
        public static Faker<CreateTodoItemVM> CreateVm() =>
            new Faker<CreateTodoItemVM>()
                .RuleFor(x => x.Title, f => f.Random.String2(5, 60))
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.Status, _ => TodoStatus.ToDo)
                .RuleFor(x => x.Priority, _ => TodoPriority.Medium)
                .RuleFor(x => x.StartDate, _ => null)
                .RuleFor(x => x.DueDate, _ => null)
                .RuleFor(x => x.TodoListId, f => f.Random.Guid())
                .RuleFor(x => x.TagNames, _ => "work, home");

        public static Faker<EditTodoItemVM> EditVm() =>
            new Faker<EditTodoItemVM>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Title, f => f.Random.String2(5, 60))
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.Status, _ => TodoStatus.ToDo)
                .RuleFor(x => x.Priority, _ => TodoPriority.Medium)
                .RuleFor(x => x.StartDate, _ => null)
                .RuleFor(x => x.DueDate, _ => null)
                .RuleFor(x => x.TodoListId, f => f.Random.Guid())
                .RuleFor(x => x.TagNames, _ => "work, home");

        public static Faker<TodoItemDto> ItemDto() =>
            new Faker<TodoItemDto>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.TodoListId, f => f.Random.Guid())
                .RuleFor(x => x.Title, f => f.Random.String2(5, 60))
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.Status, _ => TodoStatus.ToDo)
                .RuleFor(x => x.Priority, _ => TodoPriority.Medium)
                .RuleFor(x => x.StartDate, _ => null)
                .RuleFor(x => x.DueDate, _ => null)
                .RuleFor(x => x.ArchivedAt, _ => null);

        // Service
        public static Faker<CreateTodoItemRequest> CreateReq() =>
        new Faker<CreateTodoItemRequest>()
            .RuleFor(x => x.TodoListId, f => f.Random.Guid())
            .RuleFor(x => x.Title, f => f.Random.String2(5, 60))
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .RuleFor(x => x.Status, _ => TodoStatus.ToDo)
            .RuleFor(x => x.Priority, _ => TodoPriority.Medium)
            .RuleFor(x => x.StartDate, _ => null)
            .RuleFor(x => x.DueDate, _ => null)
            .RuleFor(x => x.TagNames, _ => new[] { "work", "home" });

        public static Faker<UpdateTodoItemRequest> UpdateReq() =>
            new Faker<UpdateTodoItemRequest>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Title, f => f.Random.String2(5, 60))
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.Status, _ => TodoStatus.InProgress)
                .RuleFor(x => x.Priority, _ => TodoPriority.High)
                .RuleFor(x => x.StartDate, _ => null)
                .RuleFor(x => x.DueDate, _ => null)
                .RuleFor(x => x.TagNames, _ => new[] { "work", "home" });

        public static Faker<TodoItem> ItemEntity(Guid? userId = null) =>
            new Faker<TodoItem>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.UserId, _ => userId ?? Guid.NewGuid())
                .RuleFor(x => x.TodoListId, f => f.Random.Guid())
                .RuleFor(x => x.Title, f => f.Random.String2(5, 60))
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.Status, _ => TodoStatus.ToDo)
                .RuleFor(x => x.Priority, _ => TodoPriority.Medium)
                .RuleFor(x => x.StartDate, _ => null)
                .RuleFor(x => x.DueDate, _ => null);
    }
}