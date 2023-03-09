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

    public static ErrorDetails WrongPassword => new("Wrong password", "WRONG_PASSWORD");

    public static ErrorDetails CouldNotCreateUser =>
        new("Could not create user", "COULD_NOT_CREATE_USER");

    public static ErrorDetails WrongConrimationLink =>
        new("Wrong confirmation link", "WRONG_CONFIRMATION_LINK");

    public static ErrorDetails UserIsNotActive =>
        new("User is not active", "USER_IS_NOT_ACTIVE");
}