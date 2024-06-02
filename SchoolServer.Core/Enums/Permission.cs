namespace SchoolServer.Core.Enums;

public enum Permission
{
    GrantRole = 1,
    RemoveRole,
    DeleteOwnAccount,
    DeleteAnyAccount,
    GetOwnAccount,
    GetAnyAccount,
    ChangeOwnPassword,
    ChangeAnyPassword,
    CreateResource,
    DeleteResource,
    CreateLesson,
    DeleteLesson,
    ModifyLesson,
    ViewFiles,
    CreateTest,
    DeleteTest,
    ModifyTest
}
