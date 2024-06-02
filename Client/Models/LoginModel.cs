namespace Client.Models;

internal class LoginModel
{
    public string login {  get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
}
internal class LoginModelResponse
{
    public int code { get; set; }
}
