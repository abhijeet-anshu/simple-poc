// ---------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

namespace EventGridHTTPSend
{
    using Azure.Core;
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    

    public class HttpSender : IDisposable
    {
        protected const string AuthorizationBearerTokenHeader = "Authorization";
        protected const string BearerTokenStringFormat = "Bearer {0}";

        readonly HttpClient client;
        
        bool disposed = false;


        bool useTokenCredentialBasedClient = false;
        TokenCredential tokenCredential;

        static TokenRequestContext GetTokenRequestContext()
        {
            return new TokenRequestContext(new[] { "https://eventgrid.azure.net/.default" });
        }

        public HttpSender(HttpClient client)
        {
            this.client = (client);
        }

        public HttpSender(HttpClient client, TokenCredential tokenCredential)
        {
            this.tokenCredential = tokenCredential;
            this.useTokenCredentialBasedClient = true;
            this.client = client;
        }

        public TimeSpan Timeout
        {
            set { this.client.Timeout = value; }
        }

        public async Task<HttpResponseMessage> PostAsync(Uri address, HttpContent content)
        {
            if(useTokenCredentialBasedClient)
            {
                var token = await this.tokenCredential.GetTokenAsync(GetTokenRequestContext(), new CancellationToken());
                this.client.DefaultRequestHeaders.Remove(AuthorizationBearerTokenHeader);
                this.client.DefaultRequestHeaders.Add(AuthorizationBearerTokenHeader, 
                    string.Format(BearerTokenStringFormat, token.Token));
            }

            return await this.client.PostAsync(address, content);
        }

        public async Task<HttpResponseMessage> GetAsync(Uri address)
        {
                var token = await this.tokenCredential.GetTokenAsync(new TokenRequestContext(), new CancellationToken());
                this.client.DefaultRequestHeaders.Remove(AuthorizationBearerTokenHeader);
                this.client.DefaultRequestHeaders.Add(AuthorizationBearerTokenHeader, 
                    string.Format(BearerTokenStringFormat, token.Token));
            

            return await this.client.GetAsync(address);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.client.Dispose();
                }

                this.disposed = true;
            }
        }
    }
}
