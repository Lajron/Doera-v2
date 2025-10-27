using System;
using System.Threading.Tasks;
using Doera.Application.Interfaces.Identity;
using Doera.Core.Interfaces;
using Doera.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Doera.Tests.TestSupport {
    public abstract class BaseServiceTest : BaseTest {
        protected readonly Mock<IUnitOfWork> UnitOfWork = new();
        protected readonly Mock<ITodoItemRepository> Items = new();
        protected readonly Mock<ITodoListRepository> Lists = new();
        protected readonly Mock<ITagRepository> Tags = new();
        protected readonly Mock<ICurrentUser> Current = new();
        protected Guid UserId = Guid.NewGuid();

        protected BaseServiceTest() {
            UnitOfWork.SetupGet(u => u.TodoItems).Returns(Items.Object);
            UnitOfWork.SetupGet(u => u.TodoLists).Returns(Lists.Object);
            UnitOfWork.SetupGet(u => u.Tags).Returns(Tags.Object);

            Current.Setup(c => c.RequireUserId()).Returns(() => UserId);
        }

        protected void SetCompleteAsync(int result) =>
            UnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(result);
    }
}