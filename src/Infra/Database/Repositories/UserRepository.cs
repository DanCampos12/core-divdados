using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Domain.UserContext.Services;
using Core.Divdados.Infra.SQL.DataContext;
using Core.Divdados.Shared.Uow;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Divdados.Infra.SQL.Repositories;

public record DefaultCategory(string Name, string Color);

public class UserRepository : IUserRepository
{
    public UserDataContext _context;
    private IUow _uow;

    public object ToAddresses { get; private set; }

    public UserRepository(UserDataContext context, IUow uow)
    {
        _context = context;
        _uow = uow;
    }

    public User Get(Guid id) => _context.Users
        .Where(user => user.Id.Equals(id))
        .FirstOrDefault();

    public User GetByEmail(string email) => _context.Users
        .Where(user => user.Email.Equals(email))
        .FirstOrDefault();

    public UserResult Add(User user) {
        _context.Users.Add(user);
        _uow.Commit();

        var preference = new Preference(user.Id);
        List<DefaultCategory> defaultCategories = new()
        {
            new("Objetivo", "#2196F3")
        };

        _context.Preferences.Add(preference);
        _context.Categories.AddRange(defaultCategories.Select(x => new Category(
            name: x.Name,
            color: x.Color,
            userId: user.Id,
            isAutomaticInput: true,
            maxValueMonthly: null)));

        return UserResult.Create(user, preference);
    }

    public UserResult Update(User user) {
        var preference = GetPreference(user.Id);
        _context.Users.Update(user);
        return UserResult.Create(user, preference);
    }

    public UserResult UpdatePreference(User user, Preference preference)
    {
        _context.Preferences.Update(preference);
        return UserResult.Create(user, preference);
    } 

    public Preference GetPreference(Guid userId) =>
        _context.Preferences.FirstOrDefault(x => x.UserId.Equals(userId));

    public async Task<string> RecoverPassword(User user, string idToken, AwsService awsService)
    {
        try
        {
            var client = new AmazonSimpleEmailServiceClient(awsService._awsAccessKeyId, awsService._awsSecretAccessKey, awsService._awsRegion);
            var accessUrl = $"https://divdados.com.br?idToken={idToken}";
            var htmlContentData = $@"
                <!DOCTYPE html>
                <html>
                    <div style=""font-size: 14px"">
                        Olá, <b>{user.Name}</b>! <br><br>
                        Parece que você esqueceu a sua senha 🤔 <br>
                        Recebemos o seu pedido de redefinição! <br><br>
                        Clique no link abaixo para criar uma nova senha. <br>
                        Link de acesso: <a href=""{accessUrl}"">https://divdados.com.br/auth/change-password</a> <br>
                        <b>Observação</b>: O link possui uma duração de 30 minutos. Faça uma outra solicitação caso o tempo tenha excedido. <br><br>
                        Caso não tenha solicitado a alteração, por favor, desconsidere o e-mail. <br>
                        Se precisar de alguma ajuda, entre em contato conosco através do e-mail: 
                        <a href=""mailto: suporte@divdados.com.br"">suporte@divdados.com.br</a>. <br>   
                        Abraços.
                    </div>
                </html>";

            var emailRequest = new SendEmailRequest(
                source: "suporte@divdados.com.br",
                destination: new Destination { ToAddresses = new List<string> { user.Email } },
                message: new Message
                {
                    Subject = new Content("Equipe DivDados - Recuperação de senha"),
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = "UTF-8",
                            Data = htmlContentData
                        }
                    }
                });

            var emailResponse = await client.SendEmailAsync(emailRequest);
            return $"Email enviado com sucesso. Message ID: {emailResponse.MessageId}";
        } catch (Exception ex)
        {
            return $"Ocorreu um erro ao enviar o email: {ex.Message}";
        }
    }
}