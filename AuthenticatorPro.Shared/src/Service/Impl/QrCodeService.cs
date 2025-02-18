// Copyright (C) 2022 jmh
// SPDX-License-Identifier: GPL-3.0-only

using AuthenticatorPro.Shared.Data;
using AuthenticatorPro.Shared.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthenticatorPro.Shared.Service.Impl
{
    public class QrCodeService : IQrCodeService
    {
        private readonly IAuthenticatorService _authenticatorService;
        private readonly IIconResolver _iconResolver;

        public QrCodeService(IAuthenticatorService authenticatorService, IIconResolver iconResolver)
        {
            _authenticatorService = authenticatorService;
            _iconResolver = iconResolver;
        }

        public async Task<Authenticator> ParseOtpAuthUri(string uri)
        {
            var auth = Authenticator.FromOtpAuthUri(uri, _iconResolver);
            await _authenticatorService.AddAsync(auth);
            return auth;
        }

        public Task<int> ParseOtpMigrationUri(string uri)
        {
            var migration = OtpAuthMigration.FromOtpAuthMigrationUri(uri);
            var authenticators = new List<Authenticator>();

            foreach (var item in migration.Authenticators)
            {
                Authenticator auth;

                try
                {
                    auth = Authenticator.FromOtpAuthMigrationAuthenticator(item, _iconResolver);
                }
                catch (ArgumentException)
                {
                    continue;
                }

                authenticators.Add(auth);
            }

            return _authenticatorService.AddManyAsync(authenticators);
        }
    }
}