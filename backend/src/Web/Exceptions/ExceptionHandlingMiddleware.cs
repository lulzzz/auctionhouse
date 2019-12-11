﻿using System;
using System.Net;
using System.Threading.Tasks;
using Core.Command.SignIn;
using Core.Command.SignUp;
using Core.Common.Auth;
using Core.Common.Domain.Users;
using Core.Common.Exceptions;
using Core.Common.Exceptions.Command;
using Core.Common.Exceptions.Query;
using Core.Common.RequestStatusService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using UnauthorizedAccessException = Core.Command.UpdateAuction.UnauthorizedAccessException;

namespace Web.Exceptions
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ApiException ex)
            {
                HandleException(ex, context);
            }
            catch (CommandException ex)
            {
                HandleException(ex, context);
            }
            catch (QueryException ex)
            {
                HandleException(ex, context);
            }
            catch (DomainException ex)
            {
                HandleException(ex, context);
            }
//            catch (Exception ex)
//            {
//                HandleException(ex, context);
//            }
        }

        private void HandleException(ApiException ex, HttpContext context)
        {
            context.Response.StatusCode = (int) ex.StatusCode;
        }

        private void HandleException(CommandException ex, HttpContext context)
        {
            ApiException apiException;
            switch (ex)
            {
                case UsernameConflictException e:
                    apiException = new ApiException(HttpStatusCode.Conflict, "User exists", e);
                    break;
                case UserNotFoundException e:
                    apiException = new ApiException(HttpStatusCode.Unauthorized, "Invalid credentials", e);
                    break;
                case InvalidPasswordException e:
                    apiException = new ApiException(HttpStatusCode.Unauthorized, "Invalid credentials", e);
                    break;
                case UnauthorizedAccessException e:
                    apiException = new ApiException(HttpStatusCode.Unauthorized, "Unauthorize access", e);
                    break;
                default:
                    apiException = new ApiException(HttpStatusCode.InternalServerError, "error", ex);
                    break;
            }
            HandleException(apiException, context);
        }

        private void HandleException(DomainException ex, HttpContext context)
        {
            ApiException apiException;
            switch (ex)
            {
                case InvalidUsernameException e:
                    apiException = new ApiException(HttpStatusCode.BadRequest, "Invalid username", e);
                    break;
                default:
                    apiException = new ApiException(HttpStatusCode.BadRequest, "error", ex);
                    break;
            }
            HandleException(apiException, context);
        }

        private void HandleException(QueryException ex, HttpContext context)
        {
            ApiException apiException;

            apiException = new ApiException(HttpStatusCode.InternalServerError, "Server error");
            HandleException(apiException, context);
        }

        private void HandleException(Exception ex, HttpContext context)
        {
            ApiException apiException;
            switch (ex)
            {
                case NotSignedInException e:
                    apiException = new ApiException(HttpStatusCode.Forbidden, "Not signed in", e);
                    break;
                default:
                    apiException = new ApiException(HttpStatusCode.InternalServerError, "Server error");
                    break;
            }
            HandleException(apiException, context);
        }
    }
}
