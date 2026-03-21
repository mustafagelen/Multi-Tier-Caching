namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string Create(string username);
}
