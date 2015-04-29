// ****************************************************************************
// <copyright file="NewsFinderWorkerRole.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.NewsFinderWorkerRole
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using GoingOn.GuardianClient.API.Entities;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public class NewsFinderWorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private NewsPusher newsPusher;

        public NewsFinderWorkerRole()
        {
            this.newsPusher = new NewsPusher();
        }

        public override void Run()
        {
            Trace.TraceInformation("NewsFinderWorkerRole is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("NewsFinderWorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("NewsFinderWorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("NewsFinderWorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");

                List<GuardianSectionArticle> articles = await NewsFinder.FindNews();

                await this.newsPusher.PushNews(articles);

                await Task.Delay(new TimeSpan(1, 0, 0, 0));
            }
        }
    }
}
