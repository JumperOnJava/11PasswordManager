namespace WinUi3Test.src.ViewModel.AccountDefault
{
    public interface PasswordFinishCallback
    {
        UiAccountModel Clone { get; }
        void Accept(UiAccountModel account);
        void Cancel();
    }
}