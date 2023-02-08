namespace Taskmony.Errors;

public static class UserErrors
{
    public static ErrorDetails WrongLoginOrPassword =>
        new("Wrong login or password", "WRONG_LOGIN_OR_PASSWORD");

    public static ErrorDetails LoginIsAlreadyInUse =>
        new("User with specified login already exists", "LOGIN_IS_ALREADY_IN_USE");

    public static ErrorDetails EmailIsAlreadyInUse =>
        new("User with specified email already exists", "EMAIL_IS_ALREADY_IN_USE");

    public static ErrorDetails NotFound => new("User not found", "USER_NOT_FOUND");
}