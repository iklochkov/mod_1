using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MyCRM.Application.DTOs;

namespace MyCRM.Application.Validators
{
    public class ClientRegisterDtoValidator : AbstractValidator<ClientRegisterDto>
    {

    public ClientRegisterDtoValidator()
        {
            RuleFor(i => i.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Некорректный формат email");

        RuleFor(i => i.Password)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(6).WithMessage("Пароль должен быть не менее 6 символов")
            .Matches("[A-Z]").WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
            .Matches("[0-9]").WithMessage("Пароль должен содержать хотя бы одну цифру");
        }
        
    }
}