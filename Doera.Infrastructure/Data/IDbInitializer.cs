using System.Threading.Tasks;

namespace Doera.Infrastructure.Data {
    public interface IDbInitializer {
        Task Initialize();
    }
}
