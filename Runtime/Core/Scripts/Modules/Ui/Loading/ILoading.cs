using Cysharp.Threading.Tasks;

namespace Core.Modules.Ui.Loading
{
    public interface ILoading
    {
        UniTask Show();
        UniTask Hide();
        void SetProgress(int current, int total, string text);
    }
}