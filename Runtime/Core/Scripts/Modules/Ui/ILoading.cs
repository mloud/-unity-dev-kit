using Cysharp.Threading.Tasks;

namespace Core.Scripts.Modules.Ui
{
    public interface ILoading
    {
        UniTask Show();
        UniTask Hide();
        void SetProgress(int current, int total, string text);
    }
}