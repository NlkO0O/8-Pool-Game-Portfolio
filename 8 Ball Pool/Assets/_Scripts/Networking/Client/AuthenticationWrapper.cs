using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuthAsync(int maxTries = 5)
    {
        if (AuthState == AuthState.Authenticated) return AuthState;

        if (AuthState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already Authenticating...");
            await Authenticating();
            return AuthState;
        }

        await SignInAnonymouslyAsync(maxTries);

        return AuthState;
    }

    private static async Task Authenticating()
    {
        while (AuthState == AuthState.Authenticating || AuthState== AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }
    }
    private static async Task SignInAnonymouslyAsync(int maxTries)
    {
        AuthState = AuthState.Authenticating;

        int tried = 0;

        while (AuthState == AuthState.Authenticating && tried < maxTries)
        {
            try
            {
                var authService = AuthenticationService.Instance;

                await authService.SignInAnonymouslyAsync();

                if (authService.IsSignedIn && authService.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException authException)
            {
                Debug.LogError(authException);
                AuthState = AuthState.Error;
            }
            catch (RequestFailedException reqException)
            {
                Debug.LogError(reqException);
                AuthState = AuthState.Error;
            }

            tried++;
            await Task.Delay(1000);
        }

        if (AuthState != AuthState.Authenticated)
        {
            Debug.LogWarning($"Player was not signed in successfully after {tried} tries");
            AuthState = AuthState.TimeOut;
        }
    }
}

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}