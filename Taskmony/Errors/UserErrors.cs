namespace Taskmony.Errors;

public static class UserErrors
{
    public static ErrorDetails WrongLoginOrPassword =>
        new("Wrong login or password", "WRONG_LOGIN_OR_PASSWORD");

    public static ErrorDetails LoginIsAlreadyInUse =>
        new("User with specified login already exists", "LOGIN_IS_ALREADY_IN_USE");

    public static ErrorDetails EmailIsAlreadyInUse =>
        new("User with specified email already exists", "EMAIL_IS_ALREADY_IN_USE");

    public static ErrorDetails InvalidPasswordFormat => new(
        "Password must contain minimum eight characters, at least one uppercase letter, one lowercase letter and one number",
        "INVALID_PASSWORD_FORMAT");

    public static ErrorDetails InvalidLoginFormat => new(
        "Login must contain minimum four characters and only letters and digits",
        "INVALID_LOGIN_FORMAT");

    public static ErrorDetails InvalidEmailFormat =>
        new("Invalid email format", "INVALID_EMAIL_FORMAT");
}