using System.Threading.Tasks;

namespace AbpCompanyName.AbpProjectName.Identity
{
    public interface ISmsSender
    {
        Task SendAsync(string number, string message);
    }
}
