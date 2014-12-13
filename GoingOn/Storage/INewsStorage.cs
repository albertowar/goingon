// ****************************************************************************
// <copyright file="INewsStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using System.Collections.Generic;
using Model.EntitiesBll;

public interface INewsStorage
{
    IEnumerable<NewsBll> GetNews();
    void AddNews(NewsBll newsBll);
    void UpdateNews(NewsBll newsBll);
    void DeleteNews(NewsBll newsBll);
    void DeleteAllNews();
    bool ContainsNews(NewsBll newsBll);
}