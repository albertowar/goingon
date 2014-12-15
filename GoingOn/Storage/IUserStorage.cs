// ****************************************************************************
// <copyright file="IUserStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using Model.EntitiesBll;

public interface IUserStorage
{
    void AddUser(UserBll userBll);
    UserBll GetUser(string nickname);
    bool ContainsUser(UserBll userBll);
    void DeleteUser(UserBll userBll);
    void DeleteAllUser();
}